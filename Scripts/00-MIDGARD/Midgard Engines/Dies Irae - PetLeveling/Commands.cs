using System;
using System.Collections;
using System.IO;
using Midgard.Mobiles;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.PetSystem
{
    public class LowPetSkill
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "AbbassaSkillAnimali", AccessLevel.Player, new CommandEventHandler( LowPetSkill_OnCommand ) );
        }

        [Usage( "AbbassaSkillAnimali <skill> <valore>" )]
        [Description( "Abbassa una skill a un pet." )]
        public static void LowPetSkill_OnCommand( CommandEventArgs e )
        {
            if( e.Length != 2 )
            {
                e.Mobile.SendMessage( "AbbassaSkillAnimali <skill> <valore>" );
            }
            else
            {
                string sn = e.GetString( 0 );
                double va = e.GetDouble( 1 );

                if( sn.Length == 0 || va == 0 )
                {
                    e.Mobile.SendMessage( "Almeno uno dei due parametri non e' corretto." );
                }
                else
                {
                    e.Mobile.Target = new InternalTarget( sn, va );
                }
            }
        }

        private static void ChangeSkill( Mobile owner, Mobile pet, string name, double value )
        {
            SkillName index;

            try
            {
                index = (SkillName)Enum.Parse( typeof( SkillName ), name, true );
            }
            catch
            {
                owner.SendMessage( "La skill specificata non esiste!" );
                return;
            }

            Skill skill = pet.Skills[ index ];

            if( skill != null )
            {
                if( value < 0 || value > skill.Value )
                {
                    owner.SendMessage( "Puoi abbassare la skill {0} al massimo a 0 a partire dal valore {1:F1}.", skill.Name, skill.Value );
                }
                else
                {
                    int newFixedPoint = (int)( value * 10.0 );
                    skill.BaseFixedPoint = newFixedPoint;
                }
            }
            else
            {
                owner.SendMessage( "La skill specificata non esiste!" );
            }
        }

        private class InternalTarget : Target
        {
            private string m_Skill;
            private double m_Value;

            public InternalTarget( string skill, double value )
                : base( 12, false, TargetFlags.None )
            {
                m_Skill = skill;
                m_Value = value;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted == from )
                {
                    from.SendMessage( "Devi selezionare un tuo pet." );
                }

                else if( targeted is BaseCreature )
                {
                    BaseCreature bc = (BaseCreature)targeted;

                    if( bc.Controlled == false )
                    {
                        from.SendMessage( "Devi selezionare un tuo pet." );
                    }
                    else if( bc.IsDeadPet )
                    {
                        from.SendMessage( "Devi selezionare un tuo pet vivo." );
                    }
                    else
                    {
                        ChangeSkill( from, bc, m_Skill, m_Value );
                    }
                }
            }
        }
    }

    public class FixPets
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "FixPets", AccessLevel.Developer, new CommandEventHandler( FixPets_OnCommand ) );
            CommandSystem.Register( "FixDrakes", AccessLevel.Developer, new CommandEventHandler( FixDrakes_OnCommand ) );
            CommandSystem.Register( "WipeWilds", AccessLevel.Developer, new CommandEventHandler( WipeWilds_OnCommand ) );
        }

        public static void WipeWilds_OnCommand( CommandEventArgs e )
        {
            ArrayList mobileArray;

            try
            {
                mobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            for( int i = 0; i < mobileArray.Count; i++ )
            {
                BaseCreature m = mobileArray[ i ] as BaseCreature;
                if( m == null || m.Deleted )
                    continue;

                if( m is WildDragon || m is WildHiryu || m is WildKirin || m is WildLesserHiryu ||
                   m is WildWhiteWyrm || m is WildUnicorn || m is WildNightmare )
                {
                    m.Delete();
                    using( StreamWriter tw = new StreamWriter( "Logs/LogWipeWilds.log", true ) )
                    {
                        tw.WriteLine( "Wipe di pet wild dal seriale {0}.", m.Serial );
                    }
                }
            }
        }

        public static void FixDrakes_OnCommand( CommandEventArgs e )
        {
            ArrayList mobileArray;

            try
            {
                mobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            foreach( Mobile mo in mobileArray )
            {
                if( mo == null || mo.Deleted )
                    continue;

                BaseCreature m = mo as BaseCreature;
                if( m == null )
                    continue;

                if( m is Drake )
                {
                    try
                    {
                        PetUtility.PetNormalize( m, true );
                        using( StreamWriter tw = new StreamWriter( "Logs/LogFixDrakes.log", true ) )
                        {
                            tw.WriteLine( "Drake dal seriale {0} normalizzato.", m.Serial );
                        }
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
            }
        }

        public static void FixPets_OnCommand( CommandEventArgs e )
        {
            ArrayList mobileArray;

            try
            {
                mobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            foreach( Mobile mo in mobileArray )
            {
                if( mo == null || mo.Deleted )
                    continue;

                BaseCreature m = mo as BaseCreature;
                if( m == null )
                    continue;

                m.RarityLevel = PetRarity.Rarity.Common;

                int[] mPetDyeHues = { 53, 2118, 18, 43, 2130, 2124, 1153, 1152, 1175, 1161, 1157, 51, 1154, 1151, 1167, 1166, 1150 };

                if( m is WhiteWyrm )
                {
                    if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 0 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Uncommon;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 1 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Rare;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 2 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Unique;
                    else
                        m.Hue = 0;
                }
                if( m is Dragon )
                {
                    if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 4 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Uncommon;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 5 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Rare;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 6 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Unique;
                    else
                        m.Hue = 0;
                }
                if( m is Nightmare )
                {
                    if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 8 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Uncommon;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 9 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Rare;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 10 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Unique;
                    else
                        m.Hue = 0;
                }
                if( m is Unicorn )
                {
                    if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 12 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Uncommon;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 13 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Rare;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 14 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Unique;
                    else
                        m.Hue = 0;
                }
                if( m is Kirin )
                {
                    if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 16 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Uncommon;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 17 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Rare;
                    else if( Array.LastIndexOf( ( PetRarity.PetEntryList[ 18 ] ).Hues, m.Hue ) >= 0 )
                        m.RarityLevel = PetRarity.Rarity.Unique;
                    else
                        m.Hue = 0;
                }

                if( Array.LastIndexOf( mPetDyeHues, m.Hue ) >= 0 && !( ( m is FrostSpider ) || m is Raptor || m is BlackHorse || m is FireRidableLlama || m is OldriverMustang || m is PlainFrenziedOstard ) )
                {
                    m.Hue = 0;
                }

                PetUtility.PetNormalize( m, true );
            }
        }
    }
}