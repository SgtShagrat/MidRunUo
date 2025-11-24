/***************************************************************************
 *                               BaseTaintsTransmutationPotion
 *                            -----------------------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseTaintsTransmutationPotion : BasePaganPotion
    {
        public override int DelayUse
        {
            get { return 14; }
        }

        public override int BonusOnDelayAtHundred
        {
            get { return 6; }
        }

        public override int CustomSound
        {
            get { return 0x020a; }
        }

        public override int CustomAnim
        {
            get { return 0x0022; }
        }

        public override int CustomEffects
        {
            get { return 0x3728; }
        }

        private static Hashtable m_Table = new Hashtable();

        public BaseTaintsTransmutationPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        public override bool DoPaganEffect( Mobile from )
        {
            /*
            function DoPolyEffect(me,strength)

	            var critter := RandomInt(8);
	            var newcolor := 1401+(cint(strength)*10)+critter;
	            var mod_amount := (cint(strength)*3)+critter;
	            var duration := cint(strength)*60;
	            if (CanMod(me, "bstr")=0)
		            SendSysMessage(me,"You have to wait the end of bless effect!");
		            return;
	            endif

	            if (CanMod(me, "bdex")=0)
		            SendSysMessage(me,"You have to wait the end of bless effect!");
		            return;
	            endif

	            if (CanMod(me, "bint")=0)
		            SendSysMessage(me,"You have to wait the end of bless effect!");
		            return;
	            endif

	            if (CanMod(me, "ar")=0)
		            SendSysMessage(me,"You have to wait the end of ar effect!");
		            return;
	            endif

	            if (CanMod(me, "poly")=0)
		            SendSysMessage(me,"You are already polymorphed.");
		            return;
	            endif
	            DoTempMod(me, "poly", mod_amount, duration);
	            DoTempMod(me, "bstr", mod_amount, duration);
	            DoTempMod(me, "bdex", mod_amount, duration);
	            DoTempMod(me, "bint", mod_amount, duration);
            	
	            mod_amount := cint((cint(strength)+critter)/2);
	            DoTempMod(me, "ar", mod_amount, duration);
	            DoPersistedMod(me, "color",newcolor,me.color,duration);

            endfunction
            */

            if( TransformationSpellHelper.UnderTransformation( from ) )
            {
                from.SendLocalizedMessage( 1063219 ); // You cannot mimic an animal while in that form.
            }
            else if( !from.CanBeginAction( typeof( PolymorphSpell ) ) )
            {
                from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
            }
            else if( !from.CanBeginAction( typeof( IncognitoSpell ) ) || ( from.IsBodyMod && GetContext( from ) == null ) )
            {
                from.SendMessage( "You cannot mimic an animal while incognitoed." );
            }
            else if( GetContext( from ) != null )
            {
                from.SendMessage( "You are already polymorphed." );
            }
            else if( IsUnderBless( from ) )
            {
                from.SendMessage( "You have to wait the end of ar effect!" );
            }
            else
            {
                int random = Utility.Dice( 1, 8, 0 );

                SpellHelper.DisableSkillCheck = true;
                SpellHelper.AddStatOffset( from, StatType.Str, ( Strength * 3 ) + random, TimeSpan.FromSeconds( Strength * 60 ) );
				SpellHelper.AddStatOffset( from, StatType.Dex, ( Strength * 3 ) + random, TimeSpan.FromSeconds( Strength * 60 ) );
				SpellHelper.AddStatOffset( from, StatType.Int, ( Strength * 3 ) + random, TimeSpan.FromSeconds( Strength * 60 ) );
                SpellHelper.AddStatOffset( from, StatType.AR, ( Strength + random ) / 2, TimeSpan.FromSeconds( Strength * 60 ) );
                SpellHelper.DisableSkillCheck = false;

                Transmute( from, Utility.Random( Entries.Length - 1 ), Strength * 60 );
                LockBasePotionUse( from );

                return true;
            }

            return false;
        }

        private static bool IsUnderBless( Mobile m )
        {
            return m.GetStatMod( "[Magic] Str Offset" ) != null || m.GetStatMod( "[Magic] Dex Offset" ) != null ||
                   m.GetStatMod( "[Magic] Int Offset" ) != null || m.GetStatMod( "[Magic] AR Offset" ) != null;
        }

        public void Transmute( Mobile from, int entryID, int duration )
        {
            TransmutationEntry entry = m_Entries[ entryID ];

            Timer.DelayCall( TimeSpan.FromSeconds( duration ), delegate
                                                                {
                                                                    RemoveContext( from, true );
                                                                } );

            BaseMount.Dismount( from );
            from.BodyMod = entry.BodyMod;

            if( entry.HueMod > 0 )
                from.HueMod = entry.HueMod;

            if( entry.SpeedBoost )
                from.Send( SpeedControl.MountSpeed );

            AddContext( from, new TransmutationContext( null, entry.SpeedBoost, entry.Type ) );
        }

        public static void AddContext( Mobile from, TransmutationContext context )
        {
            m_Table[ from ] = context;
        }

        public static void RemoveContext( Mobile from, bool resetGraphics )
        {
            TransmutationContext context = GetContext( from );

            if( context != null )
                RemoveContext( from, context, resetGraphics );
        }

        public static void RemoveContext( Mobile from, TransmutationContext context, bool resetGraphics )
        {
            m_Table.Remove( from );

            if( context.SpeedBoost )
            {
                if( from.Region is Server.Regions.TwistedWealdDesert )
                    from.Send( SpeedControl.WalkSpeed );
                else
                    from.Send( SpeedControl.Disable );
            }

            SkillMod mod = context.Mod;
            if( mod != null )
                from.RemoveSkillMod( mod );

            if( resetGraphics )
            {
                from.HueMod = -1;
                from.BodyMod = 0;
            }
        }

        public static TransmutationContext GetContext( Mobile from )
        {
            return ( m_Table[ from ] as TransmutationContext );
        }

        public static bool UnderTransformation( Mobile from )
        {
            return ( GetContext( from ) != null );
        }

        public static bool UnderTransformation( Mobile from, Type type )
        {
            TransmutationContext context = GetContext( from );

            return ( context != null && context.Type == type );
        }

        public static TransmutationEntry[] Entries { get { return m_Entries; } }

        #region serial deserial
        public BaseTaintsTransmutationPotion( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public class TransmutationContext
        {
            public SkillMod Mod { get; private set; }
            public bool SpeedBoost { get; private set; }
            public Type Type { get; private set; }

            public TransmutationContext( SkillMod mod, bool speedBoost, Type type )
            {
                Mod = mod;
                SpeedBoost = speedBoost;
                Type = type;
            }
        }

        public class TransmutationEntry
        {
            public Type Type { get; private set; }

            public int BodyMod { get; private set; }
            public int HueMod { get; private set; }
            public bool SpeedBoost { get; private set; }

            public TransmutationEntry( Type type, int hueMod, int bodyMod, bool speedBoost )
            {
                Type = type;
                BodyMod = bodyMod;
                HueMod = hueMod;
                SpeedBoost = speedBoost;
            }
        }

        #region m_Entries
        private static TransmutationEntry[] m_Entries = new TransmutationEntry[]
            {
                new TransmutationEntry( typeof( GreyWolf ), 2309, 0x19,  true ),
                new TransmutationEntry( typeof( Llama ),    0, 0xDC,  true ),
                new TransmutationEntry( typeof( ForestOstard ), 2212, 0xDA,  true ),
                new TransmutationEntry( typeof( BullFrog ), 2003, 0x51, false ),
                new TransmutationEntry( typeof( GiantSerpent ), 2009, 0x15, false ),
                new TransmutationEntry( typeof( Dog ), 2309, 0xD9, false ),
                new TransmutationEntry( typeof( Cat ), 2309, 0xC9, false ),
                new TransmutationEntry( typeof( Rat ), 2309, 0xEE, false ),
                new TransmutationEntry( typeof( Rabbit ), 2309, 0xCD, false )
            };
        #endregion
    }
}