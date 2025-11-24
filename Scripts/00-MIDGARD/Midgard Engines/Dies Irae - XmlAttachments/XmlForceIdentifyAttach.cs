/***************************************************************************
 *                                  XmlForceIdentify.cs
 *                            		-------------------
 *  begin                	: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Midgard.Engines
{
    public enum JewelIntensity
    {
        Light,
        Common,
        Uncommon,
        Rare,
        Unique,
        Legendary
    }

    public class XmlForceIdentify : XmlAttachment
    {
        private static readonly bool Enabled = false;

        private const double SuperRarityLevel = 3.5;
        private const double EnchantDurationPerLevel = 2.0;
        private const double AnimateDelay = 1.5;
        private const double DeltaChanceToDestroyOnFailedIdentification = 0.8;

        #region fields and props
        private JewelIntensity m_Level;

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Developer)]
        public int Originalhue { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public JewelIntensity Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                JewelIntensity oldValue = m_Level;
                if( m_Level != value )
                {
                    m_Level = value;
                    OnLevelChanged( oldValue );
                }
            }
        }
        #endregion

        [Attachable]
        public XmlForceIdentify( int level )
        {
            m_Level = (JewelIntensity)level;
        }

        public XmlForceIdentify( ASerial serial )
            : base( serial )
        {
        }

        private static double[] m_ChanceToMask = { 0.1, 0.3, 0.5, 0.7, 0.8, 0.9 };

        public static void DoMask( Item item )
        {
            if( !Enabled )
                return;

            double level;

            if( item is BaseJewel )
                level = GetItemRarity( (BaseJewel)item );
            else if( item is BaseArmor )
                level = GetItemRarity( (BaseArmor)item );
            else
                return;

            int maskLevel = (int)Math.Floor( level );

            if( maskLevel < 1 )
                maskLevel = 1;
            if( maskLevel > 6 )
                maskLevel = 6;

            if( Utility.RandomDouble() < m_ChanceToMask[ maskLevel - 1 ] )
            {
                XmlForceIdentify attach = new XmlForceIdentify( maskLevel );
                attach.Name = "[force identify]";
                XmlAttach.AttachTo( item, attach );
            }
        }

        public double GetEnchantDuration()
        {
            return EnchantDurationPerLevel * (int)m_Level;
        }

        public static void HandleIdentification( Mobile from, Item item )
        {
            XmlForceIdentify att = XmlAttach.FindAttachment( item, typeof( XmlForceIdentify ) ) as XmlForceIdentify;
            if( att == null || att.Deleted )
                return;

            if( from.CanBeginAction( typeof( XmlForceIdentify ) ) )
            {
                double itemId = from.Skills[ SkillName.ItemID ].Base;

                if( itemId >= GetIdEntryFromLevel( att.Level ).ReqSkill )
                {
                    from.BeginAction( typeof( XmlForceIdentify ) );

                    int count = (int)Math.Ceiling( att.GetEnchantDuration() / AnimateDelay );

                    if( count > 0 )
                    {
                        AnimTimer animTimer = new AnimTimer( from, count );
                        animTimer.Start();

                        double effectiveDuration = ( count * AnimateDelay ) + 1.0;
                        from.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );
                        Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( Identify_Callback ), new object[] { from, att, item } );
                    }
                }
                else
                    from.SendMessage( "You don't know how to identify this item." );
            }
            else
                from.SendMessage( "You cannot start a new identification now." );
        }

        private static readonly int[] AnimIds = new int[] { 245, 266 };

        private class AnimTimer : Timer
        {
            private Mobile m_From;

            public AnimTimer( Mobile from, int count )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
            {
                m_From = from;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( !m_From.Mounted && m_From.Body.IsHuman )
                    m_From.Animate( Utility.RandomList( AnimIds ), 7, 1, true, false, 0 );
            }
        }

        private static void Identify_Callback( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            XmlForceIdentify att = (XmlForceIdentify)states[ 1 ];
            Item item = (Item)states[ 2 ];

            double identSkill = 0.0;

            if( item is BaseJewel )
                identSkill = from.Skills[ SkillName.ItemID ].Base;
            else if( item is BaseArmor )
                identSkill = from.Skills[ SkillName.ArmsLore ].Base;

            IdentificationEntry entry = GetIdEntryFromLevel( att.Level );

            if( identSkill >= entry.ReqSkill )
            {
                double chance = ( identSkill - entry.MinSkill ) / ( entry.MaxSkill - entry.MinSkill );
                double chanceToDestroy = chance + DeltaChanceToDestroyOnFailedIdentification;
                double trial = Utility.RandomDouble();

                if( trial < chance )
                {
                    from.SendMessage( "With your knowledge and some luck you have successfully identified this powerfull item!" );
                    att.Delete();
                }
                else
                {
                    if( trial > chanceToDestroy )
                    {
                        from.SendMessage( "You damaged this item. It crasks and dissolves in a pile of dust!" );
                        if( att.AttachedTo != null && att.AttachedTo is Item && !( (Item)att.AttachedTo ).Deleted )
                            ( (Item)att.AttachedTo ).Delete();
                    }
                    else
                        from.SendMessage( "You failed to get this item identified. Luckly it seems not damaged from your identification process!" );
                }
            }
            else
                from.SendMessage( "You don't know how to identify this item." );

            from.EndAction( typeof( XmlForceIdentify ) );
        }

        public static bool IsUnidentified( Item item )
        {
            XmlForceIdentify att = XmlAttach.FindAttachment( item, typeof( XmlForceIdentify ) ) as XmlForceIdentify;
            return att != null && !att.Deleted;
        }

        public static int GetLevel( Item item )
        {
            XmlForceIdentify att = XmlAttach.FindAttachment( item, typeof( XmlForceIdentify ) ) as XmlForceIdentify;
            if( att != null && !att.Deleted )
                return (int)att.Level;
            else
                return 0;
        }

        public static double GetItemRarity( BaseArmor armor )
        {
            double value = 0;

            #region Valori Massimi di intensità delle props
            int[] capAoSAttributes = {
				2, 		//  RegenHitsCap = 2;
				3, 		//  RegenStamCap = 3;
				2, 		//  RegenManaCap = 2;
				15, 	//  DefenceChanceCap = 15;
				15, 	// 	AttackChanceCap = 15;
				8, 		//  BonusStrCap = 8;
				8, 		//  BonusDexCap = 8;
				8, 		//  BonusIntCap = 8;
				5, 		//  BonusHitsCap = 5;
				8, 		//  BonusStamCap = 8;
				8, 		//  BonusManaCap = 8;
				25, 	//  WeaponDamageCap = 25 ;
				30, 	//  WeaponSpeedCap = 30 ;
				12, 	//  SpellDamageCap = 12;
				3, 		//  CastRecovery = 3;
				1, 		//  CastSpeed = 1;
				8, 		//  LowerManaCostCap = 8;
				20, 	//  LowerRegCostCap = 20;
				15, 	//  ReflectPhysicalCap = 15;
				25, 	//  EnhancePotionsCap = 25;
				100, 	//  LuckCap = 100;
				1, 		//  SpellChanneling = 1;	
				1, 		//  NightSight= 1;
			};

            int[] capAoSArmorAttributes = {
				100, 	// LowerStatReq
				5,		// SelfRepair
				1,		// MageArmor
				100		// DurabilityBonus
			};

            int ResistancesCap = 15;
            #endregion

            int i = 0;
            foreach( int j in Enum.GetValues( typeof( AosAttribute ) ) )
            {
                if( armor.Attributes[ (AosAttribute)j ] > 0 )
                    value += ( armor.Attributes[ (AosAttribute)j ] / (double)capAoSAttributes[ i ] );
                i++;
            }

            int k = 0;
            foreach( int a in Enum.GetValues( typeof( AosArmorAttribute ) ) )
            {
                if( armor.ArmorAttributes[ (AosArmorAttribute)a ] > 0 )
                    value += ( armor.ArmorAttributes[ (AosArmorAttribute)a ] / (double)capAoSArmorAttributes[ k ] );
                k++;
            }

            if( armor.PhysicalBonus > 0 )
                value += ( armor.PhysicalBonus / (double)ResistancesCap );
            if( armor.FireBonus > 0 )
                value += ( armor.FireBonus / (double)ResistancesCap );
            if( armor.ColdBonus > 0 )
                value += ( armor.ColdBonus / (double)ResistancesCap );
            if( armor.PoisonBonus > 0 )
                value += ( armor.PoisonBonus / (double)ResistancesCap );
            if( armor.EnergyBonus > 0 )
                value += ( armor.EnergyBonus / (double)ResistancesCap );

            return value * 0.5;
        }

        public static double GetItemRarity( BaseJewel jewel )
        {
            double value = 0;

            #region Valori Massimi di intensità delle props
            int[] capAoSAttributes = {
				2, 		//  RegenHitsCap = 2;
				3, 		//  RegenStamCap = 3;
				2, 		//  RegenManaCap = 2;
				15, 	//  DefenceChanceCap = 15;
				15, 	// 	AttackChanceCap = 15;
				8, 		//  BonusStrCap = 8;
				8, 		//  BonusDexCap = 8;
				8, 		//  BonusIntCap = 8;
				5, 		//  BonusHitsCap = 5;
				8, 		//  BonusStamCap = 8;
				8, 		//  BonusManaCap = 8;
				25, 	//  WeaponDamageCap = 25 ;
				30, 	//  WeaponSpeedCap = 30 ;
				12, 	//  SpellDamageCap = 12;
				3, 		//  CastRecovery = 3;
				1, 		//  CastSpeed = 1;
				8, 		//  LowerManaCostCap = 8;
				20, 	//  LowerRegCostCap = 20;
				15, 	//  ReflectPhysicalCap = 15;
				25, 	//  EnhancePotionsCap = 25;
				100, 	//  LuckCap = 100;
				1, 		//  SpellChanneling = 1;	
				1, 		//  NightSight= 1;
			};

            const int resistancesCap = 15;

            const int skillBonusCap = 15;
            #endregion

            int i = 0;
            foreach( int j in Enum.GetValues( typeof( AosAttribute ) ) )
            {
                if( jewel.Attributes[ (AosAttribute)j ] > 0 )
                    value += ( jewel.Attributes[ (AosAttribute)j ] / (double)capAoSAttributes[ i ] );
                i++;
            }

            foreach( int j in Enum.GetValues( typeof( AosElementAttribute ) ) )
            {
                if( jewel.Resistances[ (AosElementAttribute)j ] > 0 )
                    value += ( jewel.Resistances[ (AosElementAttribute)j ] / (double)resistancesCap );
            }

            for( int j = 0; j < 5; j++ )
            {
                if( jewel.SkillBonuses.GetBonus( j ) > 0 )
                    value += ( jewel.SkillBonuses.GetBonus( j ) / (double)skillBonusCap );
            }

            // Se un singolo gioiello ha ValoreGioiello molto alto il valore viene incrementato.
            if( value > SuperRarityLevel )
                value = ( Math.Exp( value - SuperRarityLevel ) ) - 1 + SuperRarityLevel;

            if( value > 20.0 )
                value = 20.0;

            return value;
        }

        private static IdentificationEntry[] m_IdentificationTable = new IdentificationEntry[]
		{
			//						ReqSkill   MinSkill	   MaxSkill
			new IdentificationEntry( 30.0, 		20.0, 		120.0 ),
			new IdentificationEntry( 50.0, 		40.0, 		140.0 ),
			new IdentificationEntry( 70.0, 		60.0, 		160.0 ),
			new IdentificationEntry( 90.0, 		80.0, 		180.0 ),
			new IdentificationEntry( 100.0, 	90.0, 		190.0 ),
			new IdentificationEntry( 100.0, 	95.0, 		195.0 )
		};

        public static IdentificationEntry GetIdEntryFromLevel( JewelIntensity level )
        {
            int entryLevel = (int)level;

            if( entryLevel < 1 )
                entryLevel = 1;

            if( entryLevel > 6 )
                entryLevel = 6;

            return m_IdentificationTable[ entryLevel - 1 ];
        }

        public static void AddUnidentifiedProperties( ObjectPropertyList list, Item item )
        {
            list.Add( 1038000 ); // Unidentified
            list.Add( 1060658, "Intensity\t{0}", Enum.GetName( typeof( JewelIntensity ), GetLevel( item ) ) );
        }

        public class IdentificationEntry
        {
            public double ReqSkill { get; private set; }
            public double MinSkill { get; private set; }
            public double MaxSkill { get; private set; }

            public IdentificationEntry( double reqSkill, double minSkill, double maxSkill )
            {
                ReqSkill = reqSkill;
                MinSkill = minSkill;
                MaxSkill = maxSkill;
            }
        }

        #region virtual and overriden members
        public virtual void OnLevelChanged( JewelIntensity oldValue )
        {
            if( m_Level != oldValue )
            {
                if( AttachedTo != null && AttachedTo is Item )
                {
                    Item i = (Item)AttachedTo;
                    i.Hue = GetHueFromLevel();
                    i.InvalidateProperties();
                }
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if( AttachedTo != null && AttachedTo is Item )
            {
                Item i = (Item)AttachedTo;
                Originalhue = i.Hue;
                i.Hue = GetHueFromLevel();
            }
            else
                Delete();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if( AttachedTo != null && AttachedTo is Item )
            {
                Item i = (Item)AttachedTo;
                i.Hue = Originalhue;
                i.InvalidateProperties();
            }
        }
        #endregion

        #region private members
        public int GetHueFromLevel()
        {
            switch( (int)m_Level )
            {
                case 1:
                    return 2959;
                case 2:
                    return 138;
                case 3:
                    return 2548;
                case 4:
                    return 2984;
                case 5:
                    return 1486;
                case 6:
                    return 2148;
                default:
                    return 0;
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( (int)m_Level );
            writer.Write( Originalhue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Level = (JewelIntensity)reader.ReadInt();
            Originalhue = reader.ReadInt();
        }
        #endregion
    }
}