using System;
using System.Collections.Generic;
using System.Text;

using Midgard.Engines.Classes;
using Midgard.Engines.Races;
using Midgard.Engines.StoneEnchantSystem;
using Midgard.Items;
using Midgard.Misc;

using Server.Engines.Craft;
using Server.Factions;
using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;
using Midgard.Engines.MidgardTownSystem;
using Server.Guilds;

namespace Server.Items
{
    public enum GemType
	{
		None,
		StarSapphire,
		Emerald,
		Sapphire,
		Ruby,
		Citrine,
		Amethyst,
		Tourmaline,
		Amber,
		Diamond
	}

    #region mod by Dies Irae
    public enum JewelQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public enum JewelMagicalAttribute
    {
        None,

        // "wear useful things"
        Clumsiness,
        Feeblemindedness,
        Weakness,
        Curses,

        // "wear benificial items"
        Agility,
        Cunning,
        Strength,
        Blessings,

        // "wear protections"
        // Protection // handled by its way
        SpellReflection,
        Invisibility,

        // "wear useful things"
        NightEyes,
        Teleportation
    }
    #endregion

    public abstract class BaseJewel : Item, ICraftable, ISetItem, IStoneEnchantItem, IIdentificable, IGuildUniform, IResourceItem
	{
        private int m_MaxHitPoints;
        private int m_HitPoints;

		private AosAttributes m_AosAttributes;
		private AosElementAttributes m_AosResistances;
		private AosSkillBonuses m_AosSkillBonuses;
		private CraftResource m_Resource;
		private GemType m_GemType;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHitPoints; }
			set{ m_MaxHitPoints = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get 
			{
				return m_HitPoints;
			}
			set 
			{
				if ( value != m_HitPoints && MaxHitPoints > 0 )
				{
					m_HitPoints = value;

					if ( m_HitPoints < 0 )
						Delete();
					else if ( m_HitPoints > MaxHitPoints )
						m_HitPoints = MaxHitPoints;

					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
            set { m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateSecondAgeNames(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public GemType GemType
		{
			get{ return m_GemType; }
            set { m_GemType = value; InvalidateProperties(); InvalidateSecondAgeNames(); } // mod by Dies Irae
		}

		public override int PhysicalResistance{ get{ return m_AosResistances.Physical; } }
		public override int FireResistance{ get{ return m_AosResistances.Fire; } }
		public override int ColdResistance{ get{ return m_AosResistances.Cold; } }
		public override int PoisonResistance{ get{ return m_AosResistances.Poison; } }
		public override int EnergyResistance{ get{ return m_AosResistances.Energy; } }
		public virtual int BaseGemTypeNumber{ get{ return 0; } }

		public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }

		public override int LabelNumber
		{
			get
			{
				if ( m_GemType == GemType.None )
					return base.LabelNumber;

				return BaseGemTypeNumber + (int)m_GemType - 1;
			}
		}

		public override void OnAfterDuped( Item newItem )
		{
			BaseJewel jewel = newItem as BaseJewel;

			if ( jewel == null )
				return;

			jewel.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			jewel.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			jewel.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );

			#region Mondain's Legacy
			jewel.m_SetAttributes = new AosAttributes( newItem, m_SetAttributes );
			jewel.m_SetSkillBonuses = new AosSkillBonuses( newItem, m_SetSkillBonuses );
			#endregion
		}

		public virtual int ArtifactRarity{ get{ return 0; } }
		
		#region Mondain's Legacy
		private Mobile m_Crafter;
		private JewelQuality m_Quality;
		
        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); InvalidateSecondAgeNames(); }
		}
		*/

		[CommandProperty( AccessLevel.GameMaster )]
		public JewelQuality Quality
		{
			get
			{
                #region mod by Dies Irae
                if( CustomQuality >= Server.Quality.Exceptional )
                    return JewelQuality.Exceptional;
                else if( CustomQuality <= Server.Quality.Low )
                    return JewelQuality.Low;
                #endregion
                
                return m_Quality;
			}
			set{ m_Quality = value; InvalidateProperties(); InvalidateSecondAgeNames(); }
		}
		#endregion

		public BaseJewel( int itemID, Layer layer ) : base( itemID )
		{
			m_AosAttributes = new AosAttributes( this );
			m_AosResistances = new AosElementAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_Resource = CraftResource.Iron;
			m_GemType = GemType.None;

			Layer = layer;

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			#region Mondain's Legacy Sets
			m_SetAttributes = new AosAttributes( this );
			m_SetSkillBonuses = new AosSkillBonuses( this );
			#endregion

            m_IdentifiersList = new List<Mobile>(); // mod by Dies Irae
        }

		public override void OnAdded( object parent )
		{
			if ( Core.AOS && parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.AddTo( from );

				int strBonus = m_AosAttributes.BonusStr;
				int dexBonus = m_AosAttributes.BonusDex;
				int intBonus = m_AosAttributes.BonusInt;

				if ( strBonus != 0 || dexBonus != 0 || intBonus != 0 )
				{
					string modName = this.Serial.ToString();

					if ( strBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

					if ( dexBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

					if ( intBonus != 0 )
						from.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
				}

				from.CheckStatTimers();
								
				#region Mondain's Legacy Sets
				if ( IsSetItem )
				{
					m_SetEquipped = SetHelper.FullSetEquipped( from, SetID, Pieces );
				
					if ( m_SetEquipped )
					{
						m_LastEquipped = true;							
						SetHelper.AddSetBonus( from, SetID );
					}
				}
				#endregion
			}

			#region ARTEGORDONMOD XmlAttachment check for OnEquip and CanEquip
            if( parent is Mobile )
            {
                if( Engines.XmlSpawner2.XmlAttach.CheckCanEquip( this, (Mobile)parent ) )
                    Engines.XmlSpawner2.XmlAttach.CheckOnEquip( this, (Mobile)parent );
                else
                    ( (Mobile)parent ).AddToBackpack( this );
            }
			#endregion

            OnPreAoSAdded( parent ); // mod by Dies Irae : pre-AoS stuff
		}

		public override void OnRemoved( object parent )
		{
			if ( Core.AOS && parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				from.RemoveStatMod( modName + "Str" );
				from.RemoveStatMod( modName + "Dex" );
				from.RemoveStatMod( modName + "Int" );

				from.CheckStatTimers();

				#region Mondain's Legacy Sets
				if ( IsSetItem && m_SetEquipped )
					SetHelper.RemoveSetBonus( from, SetID, this );
				#endregion
			}

            OnPreAoSRemoved( parent ); // mod by Dies Irae : pre-AoS stuff

		    Engines.XmlSpawner2.XmlAttach.CheckOnRemoved( this, parent ); // ARTEGORDONMOD XmlAttachment check for OnRemoved
		}

		public BaseJewel( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			#region mod by Dies Irae
			if( Midgard.Engines.XmlForceIdentify.IsUnidentified( this ) )
			{
				Midgard.Engines.XmlForceIdentify.AddUnidentifiedProperties( list, this );
				return;
			}
			#endregion

			#region Mondain's Legacy
			if ( m_Quality == JewelQuality.Exceptional )
				list.Add( 1063341 ); // exceptional
				
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~
			#endregion

			#region Mondain's Legacy Sets
			if ( IsSetItem )
			{
				list.Add( 1080240, Pieces.ToString() ); // Part of a Jewelry Set (~1_val~ pieces)
					
				if ( m_SetEquipped )
				{
					list.Add( 1080241 ); // Full Jewelry Set Present					
					SetHelper.GetSetProperties( list, this );
				}
			}
			#endregion

			m_AosSkillBonuses.GetProperties( list );

			int prop;

			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~

			if ( (prop = m_AosAttributes.WeaponDamage) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = m_AosAttributes.AttackChance) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%

			if ( (prop = m_AosAttributes.Luck) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			base.AddResistanceProperties( list );

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~

            #region ARTEGORDONMOD mod to display attachment properties
            Engines.XmlSpawner2.XmlAttach.AddAttachmentProperties( this, list );
            #endregion
        }

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 8 ); // version

            writer.Write( PlayerConstructed ); // mod by Dies Irae

            writer.WriteEncodedInt( (int)m_MaxHitPoints );
            writer.WriteEncodedInt( (int)m_HitPoints );

			#region Mondain's Legacy Sets version 5
			writer.Write( (bool) m_LastEquipped );
			writer.Write( (bool) m_SetEquipped );
			writer.Write( (int) m_SetHue );

			m_SetAttributes.Serialize( writer );
			m_SetSkillBonuses.Serialize( writer );
			#endregion

            MidgardSerialize( writer ); // mod by Dies Irae

			#region Mondain's Legacy version 3
			writer.Write( (Mobile) m_Crafter );
			writer.Write( (int) m_Quality );
			#endregion

			writer.WriteEncodedInt( (int) m_Resource );
			writer.WriteEncodedInt( (int) m_GemType );

			m_AosAttributes.Serialize( writer );
			m_AosResistances.Serialize( writer );
			m_AosSkillBonuses.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
            {
                #region mod by Dies Irae
                case 8:
                {
                    PlayerConstructed = reader.ReadBool();
                    goto case 7;
                }
                case 7:
                    goto case 6;
                #endregion
                case 6:
                {
                    m_MaxHitPoints = reader.ReadEncodedInt();
                    m_HitPoints = reader.ReadEncodedInt();

                    goto case 5;
                }
				#region Mondain's Legacy Sets
				case 5:
				{
					m_LastEquipped = reader.ReadBool();
					m_SetEquipped = reader.ReadBool();
					m_SetHue = reader.ReadInt();

					m_SetAttributes = new AosAttributes( this, reader );
					m_SetSkillBonuses = new AosSkillBonuses( this, reader );

					goto case 4;
				}
				#endregion

                #region mod by Dies Irae
                case 4:
                {
                    MidgardDeserialize( reader, version );
                    goto case 3;
                }
                #endregion
				#region Mondain's Legacy
				case 3:
				{
					m_Crafter = reader.ReadMobile();
					m_Quality = (JewelQuality) reader.ReadInt();
					
					goto case 2;
				}
				#endregion
				case 2:
				{
					m_Resource = (CraftResource)reader.ReadEncodedInt();
					m_GemType = (GemType)reader.ReadEncodedInt();

					goto case 1;
				}
				case 1:
				{
					m_AosAttributes = new AosAttributes( this, reader );
					m_AosResistances = new AosElementAttributes( this, reader );
					m_AosSkillBonuses = new AosSkillBonuses( this, reader );

					if ( Core.AOS && Parent is Mobile )
						m_AosSkillBonuses.AddTo( (Mobile)Parent );

					int strBonus = m_AosAttributes.BonusStr;
					int dexBonus = m_AosAttributes.BonusDex;
					int intBonus = m_AosAttributes.BonusInt;

					if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
					{
						Mobile m = (Mobile)Parent;

						string modName = Serial.ToString();

						if ( strBonus != 0 )
							m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

						if ( dexBonus != 0 )
							m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

						if ( intBonus != 0 )
							m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
					}

					if ( Parent is Mobile )
						((Mobile)Parent).CheckStatTimers();

					break;
				}
				case 0:
				{
					m_AosAttributes = new AosAttributes( this );
					m_AosResistances = new AosElementAttributes( this );
					m_AosSkillBonuses = new AosSkillBonuses( this );

					break;
				}
			}

			#region Mondain's Legacy Sets
			if ( m_SetAttributes == null )
				m_SetAttributes = new AosAttributes( this );

			if ( m_SetSkillBonuses == null )
				m_SetSkillBonuses = new AosSkillBonuses( this );
			#endregion

			if ( version < 2 )
			{
				m_Resource = CraftResource.Iron;
				m_GemType = GemType.None;
			}

            #region mod by Dies Irae
            if( this is ITreasureOfMidgard )
                TreasuresOfMidgard.RegisterExistance( GetType() );

#if verify
			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Midgard.CraftHelper.VerifyManaRegenerationItems_Callback ), this );
#endif
            #endregion
        }

		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
            m_Quality = (JewelQuality)quality;

            if( makersMark )
                m_Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			if ( 1 < craftItem.Resources.Count )
			{
				resourceType = craftItem.Resources.GetAt( 1 ).ItemType;

				if ( resourceType == typeof( StarSapphire ) )
					GemType = GemType.StarSapphire;
				else if ( resourceType == typeof( Emerald ) )
					GemType = GemType.Emerald;
				else if ( resourceType == typeof( Sapphire ) )
					GemType = GemType.Sapphire;
				else if ( resourceType == typeof( Ruby ) )
					GemType = GemType.Ruby;
				else if ( resourceType == typeof( Citrine ) )
					GemType = GemType.Citrine;
				else if ( resourceType == typeof( Amethyst ) )
					GemType = GemType.Amethyst;
				else if ( resourceType == typeof( Tourmaline ) )
					GemType = GemType.Tourmaline;
				else if ( resourceType == typeof( Amber ) )
					GemType = GemType.Amber;
				else if ( resourceType == typeof( Diamond ) )
					GemType = GemType.Diamond;
			}

			return quality;
		}

	    [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

	    #endregion
		
		#region Mondain's Legacy Sets
		public override bool OnDragLift( Mobile from )
		{
			if ( Parent is Mobile && from == Parent )
			{			
				if ( IsSetItem && m_SetEquipped )
					SetHelper.RemoveSetBonus( from, SetID, this );
			}			
			
			return base.OnDragLift( from );
		}

		public virtual SetItem SetID{ get{ return SetItem.None; } }
		public virtual int Pieces{ get{ return 0; } }
		public virtual bool MixedSet{ get{ return false; } }
		
		public bool IsSetItem{ get{ return SetID == SetItem.None ? false : true; } }
		
		private int m_SetHue;
		private bool m_SetEquipped;
		private bool m_LastEquipped;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SetHue
		{
			get{ return m_SetHue; }
			set{ m_SetHue = value; InvalidateProperties(); }
		}
		
		public bool SetEquipped
		{
			get{ return m_SetEquipped; }
			set{ m_SetEquipped = value; }
		}
		
		public bool LastEquipped
		{
			get{ return m_LastEquipped; }
			set{ m_LastEquipped = value; }
		}		
		
		private AosAttributes m_SetAttributes;
		private AosSkillBonuses m_SetSkillBonuses;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes SetAttributes
		{
			get{ return m_SetAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SetSkillBonuses
		{
			get{ return m_SetSkillBonuses; }
			set{}
		}	
		#endregion

        #region mod by Dies Irae [Third Crown]
        private string m_EngravedText;

        [CommandProperty( AccessLevel.GameMaster )]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set { m_EngravedText = value; InvalidateSecondAgeNames(); }
        }

        public virtual string OldInitHits { get { return "1d0+0"; } }

        public virtual TownSystem RequiredTownSystem { get { return null; } }

        public virtual Classes RequiredClass { get { return Classes.None; } }

        public virtual Race RequiredRace { get { return null; } }

        #region [guilds]
        [CommandProperty( AccessLevel.Administrator )]
        public bool IsGuildItem { get; set; }

        private BaseGuild m_OwnerGuild;

        [CommandProperty( AccessLevel.Administrator )]
        public BaseGuild OwnerGuild
        {
            get { return m_OwnerGuild; }
            set
            {
                m_OwnerGuild = value;

                LootType = ( m_OwnerGuild == null ? LootType.Regular : LootType.Blessed );
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public Mobile GuildedOwner { get; set; }

	    public void VerifyGuildItem()
        {
            if( IsGuildItem && OwnerGuild != null && GuildedOwner == null && OwnerGuild is Guild )
            {
                GuildsHelper.StringToLog( string.Format( "Warning: Guilditem for {0} with serial {1} without owner... deleting.", OwnerGuild.Name, Serial ) );
                GuildsHelper.UnRegisterUniform( null, (Guild)OwnerGuild, this );
                Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if( IsGuildItem && m_OwnerGuild != null && m_OwnerGuild is Guild )
                GuildsHelper.UnRegisterUniform( null, (Guild)m_OwnerGuild, this );

            base.OnAfterDelete();
        }
        #endregion

        #region [stone enchant system]
        private StoneEnchantItem m_StoneEnchantItemState;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public StoneEnchantItem StoneEnchantItemState
        {
            get { return m_StoneEnchantItemState; }
            set { m_StoneEnchantItemState = value; InvalidateSecondAgeNames(); }
        }

        public SkillName[] SkillsRequiredToEnchant
        {
            get
            {
                if( CraftResources.GetType( Resource ) == CraftResourceType.Metal )
                    return StoneEnchantItem.SkillsRequiredToEnchantMetal;
                else if( CraftResources.GetType( Resource ) == CraftResourceType.Leather )
                    return StoneEnchantItem.SkillsRequiredToEnchantLeather;

                return StoneEnchantItem.SkillsRequiredToEnchantWood;
            }
        }
        #endregion

        private JewelMagicalAttribute m_MagicalAttribute;

        [CommandProperty( AccessLevel.GameMaster )]
        public JewelMagicalAttribute MagicalAttribute
        {
            get { return m_MagicalAttribute; }
            set { m_MagicalAttribute = value; InvalidateSecondAgeNames(); }
        }

        public virtual TimeSpan UseDelay { get { return TimeSpan.FromSeconds( 4.0 ); } }

        private int m_MagicalCharges;

        [CommandProperty( AccessLevel.GameMaster )]
        public int MagicalCharges
        {
            get { return m_MagicalCharges; }
            set { m_MagicalCharges = value; InvalidateSecondAgeNames(); }
        }

        public override bool CanEquip( Mobile from )
        {
            if( m_OwnerGuild != null && !GuildsHelper.CheckEquip( from, m_OwnerGuild, this ) )
                return false;

            if( !RaceAllowanceAttribute.IsAllowed( this, from, Midgard.Engines.Races.Core.MountainDwarf, true ) )
                return false;

            if( RequiredTownSystem != null && TownHelper.CheckEquip( from, RequiredTownSystem, true ) )
                return false;

            if( this is ITreasureOfMidgard && !XmlBlessedCursedAttach.CanEquip( from, this ) )
                return false;

            if( from.Player && RequiredRace != null )
            {
                if( !MidgardRace.CheckEquip( RequiredRace, from, true ) )
                    return false;
            }

            if( Core.AOS && Midgard.Engines.XmlForceIdentify.IsUnidentified( this ) )
            {
                from.SendMessage( "You have to identify this item before using." );
                return false;
            }

            return base.CanEquip( from );
        }

        public virtual void OnPreAoSAdded( object parent )
        {
            if( Core.AOS )
                return;

            if( parent is Mobile )
            {
                Mobile from = (Mobile)parent;

                if( Parent == from )
                {
                    if( m_MagicalAttribute != JewelMagicalAttribute.None || this is BaseRingOfProtection )
                    {
                        if( m_MagicalCharges > 0 )
                            OnPreAosUse( from );
                        else
                            from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
                    }

                    if( StoneEnchantItemState != null )
                        StoneEnchantItemState.Invalidate( from, true );
                }
            }
        }

        public virtual void OnPreAoSRemoved( object parent )
        {
            if( Core.AOS )
                return;

            if( parent is Mobile )
            {
                Mobile m = (Mobile)parent;

                if( m_MagicalAttribute == JewelMagicalAttribute.Invisibility && m.Hidden && m.Player )
                {
                    Effects.SendLocationParticles( EffectItem.Create( new Point3D( m.X, m.Y, m.Z + 16 ), m.Map, EffectItem.DefaultDuration ), 0x376A, 10, 15, 5045 );
                    m.PlaySound( 0x3C4 );
                    m.Hidden = false;
                }

                if( StoneEnchantItemState != null )
                    StoneEnchantItemState.Invalidate( m, false );
            }
        }

        public virtual void OnPreAosUse( Mobile from )
        {
            if( Deleted || m_MagicalCharges <= 0 || Parent != from )
                return;

            if( !from.CanBeginAction( typeof( BaseJewel ) ))
            {
                from.SendMessage( "You cannot activate this item yet." );
                return;
            }

            if( m_MagicalAttribute == JewelMagicalAttribute.Teleportation )
                from.Target = new TeleportTarget( this );
            else if( DoPreAosMagicalEffect( from ) )
                ConsumeCharge( from );
        }

        private class TeleportTarget : Target
        {
            private readonly BaseJewel m_Item;

            public TeleportTarget( BaseJewel item )
                : base( 6, true, TargetFlags.None )
            {
                m_Item = item;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( DoTeleport( from, targeted as IPoint3D ) )
                    m_Item.ConsumeCharge( from );
            }
        }

        private bool DoPreAosMagicalEffect( Mobile target )
        {
            if( m_MagicalAttribute != JewelMagicalAttribute.None && m_MagicalCharges > 0 )
            {
                switch( m_MagicalAttribute )
                {
                    case JewelMagicalAttribute.Clumsiness:
                        DoClumsiness( target );
                        return true;
                    case JewelMagicalAttribute.Feeblemindedness:
                        DoFeeblemindedness( target );
                        return true;
                    case JewelMagicalAttribute.Weakness:
                        DoWeakness( target );
                        return true;
                    case JewelMagicalAttribute.Agility:
                        DoAgility( target );
                        return true;
                    case JewelMagicalAttribute.Cunning:
                        DoCunning( target );
                        return true;
                    case JewelMagicalAttribute.Strength:
                        DoStrength( target );
                        return true;
                    case JewelMagicalAttribute.Curses:
                        DoCurses( target );
                        return true;
                    case JewelMagicalAttribute.NightEyes:
                        DoNightEyes( target );
                        return true;
                    case JewelMagicalAttribute.Blessings:
                        DoBlessings( target );
                        return true;
                    case JewelMagicalAttribute.SpellReflection:
                        return DoSpellReflection( target );
                    case JewelMagicalAttribute.Invisibility:
                        DoInvisibility( target );
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

	    protected void ConsumeCharge( Mobile from )
        {
            --m_MagicalCharges;

            if( m_MagicalCharges <= 0 )
            {
                from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
                m_MagicalAttribute = JewelMagicalAttribute.None;
            }

            ApplyDelayTo( from );
        }

        #region [t2a effects]
        private static void DoClumsiness( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Dex );
            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Dex, name, mod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < -10 )
                from.AddStatMod( new StatMod( StatType.Dex, name, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.Paralyzed = false;
            from.FixedParticles( 0x3779, 10, 15, 5002, EffectLayer.Head );
            from.PlaySound( 0x1DF );
        }

        private static void DoFeeblemindedness( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Int );

            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Int, name, mod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Int, name, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x3779, 10, 15, 5004, EffectLayer.Head );
            from.PlaySound( 0x1E4 );
        }

        private static void DoWeakness( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Str );
            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Str, name, mod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Str, name, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x3779, 10, 15, 5009, EffectLayer.Waist );
            from.PlaySound( 0x1E6 );
        }

        private static void DoAgility( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Dex );
            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Dex, name, mod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Dex, name, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x375A, 10, 15, 5010, EffectLayer.Waist );
            from.PlaySound( 0x28E );
        }

        private static void DoCunning( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Int );

            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Int, name, mod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Int, name, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x375A, 10, 15, 5011, EffectLayer.Head );
            from.PlaySound( 0x1EB );
        }

        private static void DoStrength( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Str );

            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Str, name, mod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Str, name, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x375A, 10, 15, 5017, EffectLayer.Waist );
            from.PlaySound( 0x1EE );
        }

        private static void DoCurses( Mobile from )
        {
            string nameS = String.Format( "[Magic] {0} Offset", StatType.Str );
            string nameD = String.Format( "[Magic] {0} Offset", StatType.Dex );
            string nameI = String.Format( "[Magic] {0} Offset", StatType.Int );
            StatMod strmod = from.GetStatMod( nameS );
            StatMod dexmod = from.GetStatMod( nameD );
            StatMod intmod = from.GetStatMod( nameI );

            if( strmod != null && strmod.Offset > 0 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, strmod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( strmod == null || strmod.Offset > 10 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            if( dexmod != null && dexmod.Offset > 0 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, dexmod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( dexmod == null || dexmod.Offset > 10 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            if( intmod != null && intmod.Offset > 0 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, intmod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( intmod == null || intmod.Offset > 10 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
            from.PlaySound( 0x1EA );
        }

        private static void DoNightEyes( Mobile from )
        {
            new LightCycle.NightSightTimer( from ).Start();
            from.LightLevel = 26;

            from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
            from.PlaySound( 0x1E3 );
        }

        private static void DoBlessings( Mobile from )
        {
            string nameS = String.Format( "[Magic] {0} Offset", StatType.Str );
            string nameD = String.Format( "[Magic] {0} Offset", StatType.Dex );
            string nameI = String.Format( "[Magic] {0} Offset", StatType.Int );
            StatMod strmod = from.GetStatMod( nameS );
            StatMod dexmod = from.GetStatMod( nameD );
            StatMod intmod = from.GetStatMod( nameI );

            if( strmod != null && strmod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, strmod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( strmod == null || strmod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, 10, TimeSpan.FromSeconds( 60.0 ) ) );
            if( dexmod != null && dexmod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, dexmod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( dexmod == null || dexmod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, 10, TimeSpan.FromSeconds( 60.0 ) ) );
            if( intmod != null && intmod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, intmod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( intmod == null || intmod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
            from.PlaySound( 0x1EA );
        }

        private static bool DoSpellReflection( Mobile from )
        {
            if( from.MagicDamageAbsorb > 0 )
                from.SendMessage( "This spell is already in effect." );
            else if( !from.CanBeginAction( typeof( DefensiveSpell ) ) )
                from.SendMessage( "The spell will not adhere to you at this time." );
            else
            {
                from.MagicDamageAbsorb = 1;

                from.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );
                from.PlaySound( 0x1E9 );

                return true;
            }

            return false;
        }

        private static void DoInvisibility( Mobile from )
        {
            Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z + 16 ), from.Map, EffectItem.DefaultDuration ), 0x376A, 10, 15, 5045 );
            from.PlaySound( 0x3C4 );
            from.Hidden = true;
        }

        public static bool DoTeleport( Mobile from, IPoint3D p )
        {
            IPoint3D orig = p;
            Map map = from.Map;

            SpellHelper.GetSurfaceTop( ref p );

            if( WeightOverloading.IsOverloaded( from ) )
            {
                from.SendLocalizedMessage( 502359, "", 0x22 ); // Thou art too encumbered to move.
            }
            else if( !SpellHelper.CheckTravel( from, TravelCheckType.TeleportFrom ) )
            {
            }
            else if( !SpellHelper.CheckTravel( from, map, new Point3D( p ), TravelCheckType.TeleportTo ) )
            {
            }
            else if( map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z ) || SpellHelper.CheckMulti( new Point3D( p ), map ) )
            {
                from.SendLocalizedMessage( 501942 ); // That location is blocked.
            }
            else
            {
                SpellHelper.Turn( from, orig );

                Mobile m = from;

                Point3D fromLoc = m.Location;
                Point3D to = new Point3D( p );

                m.Location = to;
                m.ProcessDelta();

                if( m.Player )
                {
                    Effects.SendLocationParticles( EffectItem.Create( fromLoc, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
                    Effects.SendLocationParticles( EffectItem.Create( to, m.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
                }
                else
                {
                    m.FixedParticles( 0x376A, 9, 32, 0x13AF, EffectLayer.Waist );
                }

                m.PlaySound( 0x1FE );

                return true;
            }

            return false;
        }
        #endregion

	    private void ApplyDelayTo( Mobile from )
        {
            from.BeginAction( typeof( BaseJewel ) );
            Timer.DelayCall( UseDelay, new TimerStateCallback( ReleaseJewelLock_Callback ), from );
        }

	    private static void ReleaseJewelLock_Callback( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BaseJewel ) );
        }

        public override void OnSingleClick( Mobile from )
        {
            OnOldSingleClick( from );
            // base.OnSingleClick( from );
        }

        private string m_SecondAgeFullName;
        private string m_SecondAgeUnIdentifiedName;

        public void InvalidateSecondAgeNames()
        {
            BuildSecondAgeNames( string.IsNullOrEmpty( Name ) ? StringList.Localization[ LabelNumber ] : Name, "ITA" );
        }

        private void BuildMagicalSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();

            // 'exceptional '
            if( Quality == JewelQuality.Exceptional )
                builder.Append( "exceptional " );

            // 'cloud '
            if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
                builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );

            // 'diamond'
            // info.AppendFormat( "{0} ", GetGemName( GemType ) );
            // it is not needed...

            // 'ring'
            builder.Append( rawName );

            // ' of clumsiness'
            string mag = MagicalEffectName;
            if( mag.Length > 0 )
            {
                builder.AppendFormat( " of {0}", mag );

                if( m_MagicalAttribute != JewelMagicalAttribute.None && m_MagicalCharges > 0 )
                    builder.AppendFormat( " with {0} charges", m_MagicalCharges );
            }

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " (creato da {0})" : " (crafted by {0})", StringUtility.Capitalize( Crafter.Name ) );

            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
            m_SecondAgeUnIdentifiedName = string.Format( "magic {0}", rawName );
        }

        private void BuildStandardSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();

            // 'exceptional '
            if( Quality == JewelQuality.Exceptional )
                builder.Append( "exceptional " );

            // 'cloud '
            if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
                builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );

            // 'diamond'
            //string gemName = GetGemName( GemType );
            //if( gemName.Length > 0 )
            //    info.AppendFormat( "{0} ", gemName );

            // 'ring'
            builder.Append( rawName );

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " (creato da {0})" : " (crafted by {0})", StringUtility.Capitalize( Crafter.Name ) );

            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
            m_SecondAgeUnIdentifiedName = builder.ToString();
        }

        private void BuildSecondAgeNames( string rawName, string language )
        {
            if( IsMagical )
                BuildMagicalSecondAgeNames( rawName, language );
            else
                BuildStandardSecondAgeNames( rawName, language );
        }

        public void OnOldSingleClick( Mobile from )
        {
            if( Deleted || !from.CanSee( this ) )
                return;

            string name = StringList.GetClilocString( Name, LabelNumber, from.Language );

            if( m_SecondAgeFullName == null || m_SecondAgeUnIdentifiedName == null )
                BuildSecondAgeNames( name, from.Language );

            if( this is ITreasureOfMidgard )
                XmlBlessedCursedAttach.OnSingleClick( from, this );

            if( ArtifactRarity > 0 || this is ITreasureOfMidgard )
                LabelTo( from, name );
            else if( !String.IsNullOrEmpty( Name ) && Name != DefaultName )
                LabelTo( from, Name );
            else if( IsMagical )
                LabelTo( from, StringUtility.ConvertItemName( ( IsIdentifiedFor( from ) ? m_SecondAgeFullName : m_SecondAgeUnIdentifiedName ), false, from.Language ) );
            else
                LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeFullName, false, from.Language ) );

            if( IsGuildItem )
            {
                if( m_OwnerGuild != null && GuildedOwner != null && GuildedOwner.Name != null )
                    LabelTo( from, from.Language == "ITA" ? "[gilda : {0} ({1})]" : "[guild : {0} ({1})]", m_OwnerGuild.Name, GuildedOwner.Name );
                else if( m_OwnerGuild != null )
                    LabelTo( from, from.Language == "ITA" ? "[gilda : {0}]" : "[guild : {0}]", m_OwnerGuild.Name );
            }

            if( !String.IsNullOrEmpty( m_EngravedText ) )
                LabelTo( from, "* {0} *", m_EngravedText );

            if( IsPortingItem )
                LabelOwnageTo( from );
        }

        [CommandProperty( AccessLevel.Developer )]
        public bool IsMagical
        {
            get { return MagicalAttribute != JewelMagicalAttribute.None && MagicalCharges > 0; }
        }

        [CommandProperty( AccessLevel.Developer )]
        public string MagicalEffectName
        {
            get
            {
                switch( MagicalAttribute )
                {
                    case JewelMagicalAttribute.Clumsiness:
                    case JewelMagicalAttribute.Feeblemindedness:
                    case JewelMagicalAttribute.Weakness:
                    case JewelMagicalAttribute.Agility:
                    case JewelMagicalAttribute.Cunning:
                    case JewelMagicalAttribute.Strength:
                    case JewelMagicalAttribute.Invisibility:
                    case JewelMagicalAttribute.Teleportation:
                    case JewelMagicalAttribute.Blessings:
                    case JewelMagicalAttribute.Curses: return MagicalAttribute.ToString().ToLower();
                    case JewelMagicalAttribute.NightEyes:
                        return "night eyes";
                    case JewelMagicalAttribute.SpellReflection:
                        return "spell reflection";
                    default:
                        return "";
                }
            }
        }

        public static string GetGemName( GemType gem )
        {
            switch( gem )
            {
                case GemType.StarSapphire: return "star sapphire";
                case GemType.Emerald: return "emerald";
                case GemType.Sapphire: return "sapphire";
                case GemType.Ruby: return "ruby";
                case GemType.Citrine: return "citrine";
                case GemType.Amethyst: return "amethyst";
                case GemType.Tourmaline: return "tourmaline";
                case GemType.Amber: return "amber";
                case GemType.Diamond: return "diamond";
                default: return "";
            }
        }

        #region [serialization]
        private static void SetSaveFlag( ref MidgardFlag flags, MidgardFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( MidgardFlag flags, MidgardFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        [Flags]
        private enum MidgardFlag
        {
            None = 0x00000000,

            MagicEffect = 0x00000001,
            MagicCharges = 0x00000002,
            Identified = 0x00000004,
            ItemIDList = 0x00000008,

            EngravedText = 0x00000010,
            IsGuildUniform = 0x00000020,
            Guild = 0x00000040,
            GuildedOwner = 0x00000080,

            StoneEnchanted = 0x00000100
        }

        private void MidgardSerialize( GenericWriter writer )
        {
            MidgardFlag mflags = MidgardFlag.None;

            SetSaveFlag( ref mflags, MidgardFlag.MagicEffect, m_MagicalAttribute != JewelMagicalAttribute.None );
            SetSaveFlag( ref mflags, MidgardFlag.MagicCharges, m_MagicalCharges != 0 );
            SetSaveFlag( ref mflags, MidgardFlag.ItemIDList, m_IdentifiersList != null && m_IdentifiersList.Count > 0 );
            SetSaveFlag( ref mflags, MidgardFlag.EngravedText, !String.IsNullOrEmpty( m_EngravedText ) );
            SetSaveFlag( ref mflags, MidgardFlag.IsGuildUniform, IsGuildItem );
            SetSaveFlag( ref mflags, MidgardFlag.Guild, m_OwnerGuild != null );
            SetSaveFlag( ref mflags, MidgardFlag.GuildedOwner, GuildedOwner != null );
            SetSaveFlag( ref mflags, MidgardFlag.StoneEnchanted, m_StoneEnchantItemState != null );

            writer.WriteEncodedInt( (int)mflags );

            if( GetSaveFlag( mflags, MidgardFlag.MagicEffect ) )
                writer.Write( (int)m_MagicalAttribute );

            if( GetSaveFlag( mflags, MidgardFlag.MagicCharges ) )
                writer.Write( (int)m_MagicalCharges );

            if( GetSaveFlag( mflags, MidgardFlag.ItemIDList ) )
                writer.Write( m_IdentifiersList, true );

            if( GetSaveFlag( mflags, MidgardFlag.EngravedText ) )
                writer.Write( (string)m_EngravedText );

            if( GetSaveFlag( mflags, MidgardFlag.IsGuildUniform ) )
                writer.Write( IsGuildItem );

            if( GetSaveFlag( mflags, MidgardFlag.Guild ) )
                writer.Write( (BaseGuild)m_OwnerGuild );

            if( GetSaveFlag( mflags, MidgardFlag.GuildedOwner ) )
                writer.Write( (Mobile)GuildedOwner );

            if( GetSaveFlag( mflags, MidgardFlag.StoneEnchanted ) )
                m_StoneEnchantItemState.Serialize( writer );
        }

        private void MidgardDeserialize( GenericReader reader, int version )
        {
            MidgardFlag flags = (MidgardFlag)reader.ReadEncodedInt();

            if( GetSaveFlag( flags, MidgardFlag.MagicEffect ) )
                m_MagicalAttribute = (JewelMagicalAttribute)reader.ReadInt();

            if( GetSaveFlag( flags, MidgardFlag.MagicCharges ) )
                m_MagicalCharges = reader.ReadInt();

            if( GetSaveFlag( flags, MidgardFlag.Identified ) )
                reader.ReadBool();

            if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
                m_IdentifiersList = reader.ReadStrongMobileList();

            if( GetSaveFlag( flags, MidgardFlag.EngravedText ) )
                m_EngravedText = reader.ReadString();

            if( GetSaveFlag( flags, MidgardFlag.IsGuildUniform ) )
                IsGuildItem = reader.ReadBool();

            if( GetSaveFlag( flags, MidgardFlag.Guild ) )
                m_OwnerGuild = reader.ReadGuild();

            if( GetSaveFlag( flags, MidgardFlag.GuildedOwner ) )
                GuildedOwner = reader.ReadMobile();

            if( GetSaveFlag( flags, MidgardFlag.StoneEnchanted ) )
                m_StoneEnchantItemState = new StoneEnchantItem( reader );
        }
        #endregion

        #region IIdentificable members
        private List<Mobile> m_IdentifiersList;

        private const int MaxIdentifiers = 50;

        public void CopyIdentifiersTo( IIdentificable identificable )
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
            {
                foreach( Mobile mobile in m_IdentifiersList )
                {
                    identificable.AddIdentifier( mobile );
                }
            }
        }

        public void ClearIdentifiers()
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
                m_IdentifiersList.Clear();
        }

        public void AddIdentifier( Mobile from )
        {
            if( m_IdentifiersList == null )
                m_IdentifiersList = new List<Mobile>();

            if( !m_IdentifiersList.Contains( from ) )
                m_IdentifiersList.Add( from );

            if( m_IdentifiersList.Count > MaxIdentifiers )
                m_IdentifiersList.RemoveAt( 0 );

            if( Core.Debug )
            {
                Utility.Log( "BaseJewelAddIdentifier.log", "IdentifiersList for jewel {0}", Serial.ToString() );
                foreach( Mobile m in m_IdentifiersList )
                    Utility.Log( "BaseJewelAddIdentifier.log", m.Name );
            }
        }

        public bool IsIdentifiedFor( Mobile from )
        {
            if( Crafter != null && from == Crafter )
                return true;

            if( !IsMagical )
                return true;

            if( m_IdentifiersList == null )
                return false;

            return m_IdentifiersList.Contains( from );
        }

        public void DisplayItemInfo( Mobile from )
        {
        }
        #endregion
        #endregion
    }
}