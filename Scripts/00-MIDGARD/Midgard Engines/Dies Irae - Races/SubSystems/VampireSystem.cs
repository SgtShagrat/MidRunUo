/***************************************************************************
 *                                  VampBurnTimer.cs
 *                            		----------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Items;

using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.Races
{
    public class VampBurnTimer : Timer
    {
        private const int BurnHour = 6;
        private const int NightHour = 18;
        private const string DayWarningMessage = "* il sole sta sorgendo, fai attenzione Vampiro *";
        private const string NightWarningMessage = "* il sole sta calando, l'ora del tuo dominio e' giunta *";

        private List<Mobile> m_DayWarned;
        private List<Mobile> m_NightWarned;

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "VerificaVestiti", AccessLevel.Player, new CommandEventHandler( VampireCountDresses_OnCommand ) );
        }

        internal static void StartTimer()
        {
            new VampBurnTimer().Start();
        }

        [Usage( "VerificaVestiti" )]
        [Description( "Verifica il numero di vestiti indossati dal vampiro." )]
        public static void VampireCountDresses_OnCommand( CommandEventArgs e )
        {
            Mobile m = e.Mobile;

            if( !Config.VampireSystemEnabled )
            {
                m.SendMessage( "Questo comando e' stato disabilitato." );
                return;
            }

            if( m.Race == Core.Vampire || m.AccessLevel > AccessLevel.Player )
                m.SendMessage( "Il numero di vestiti che hai indosso e' {0}", ComputeSunMask( m ).ToString() );
            else
                m.SendMessage( "Non sei un vampiro..." );
        }

        public VampBurnTimer()
            : base( TimeSpan.FromSeconds( 60.0 ), TimeSpan.FromSeconds( 60.0 ) )
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if( !Config.VampireSystemEnabled )
                return;

            for( int i = 0; i < NetState.Instances.Count; ++i )
            {
                NetState ns = NetState.Instances[ i ];
                Mobile m = ns.Mobile;

                if( m != null )
                    CheckBurn( m );
            }
        }

        private void CheckBurn( Mobile m )
        {
            if( m.Race == Core.Vampire )
            {
                int hours, minutes;
                Clock.GetTime( m.Map, m.X, m.Y, out hours, out minutes );

                if( m_DayWarned == null )
                    m_DayWarned = new List<Mobile>();

                if( m_NightWarned == null )
                    m_NightWarned = new List<Mobile>();

                DoWarn( m, hours );

                if( hours > BurnHour && m_DayWarned.Count > 0 )
                    m_DayWarned.Clear();

                if( hours > NightHour && m_NightWarned.Count > 0 )
                    m_NightWarned.Clear();

                if( hours > BurnHour && hours < NightHour )
                {
                    if( !IsAtHome( m ) && !IsDarkRegion( m ) && IsDayLight( m ) )
                    {
                        DoBurn( m );
                    }
                }
            }
        }

        private static void DoBurn( Mobile m )
        {
            if( m != null && m.Alive )
            {
                // m.FixedParticles( 0x91C, 10, 180, 9539, EffectLayer.Waist );
                // m.PlaySound( 0x00E );
                // m.PlaySound( 0x1BC );

                int numOfCloths = ComputeSunMask( m );

                if( numOfCloths >= 7 )
                {
                    m.SendMessage( 37, string.Format( "Le tue vesti, fortunatamente, sembrano proteggerti. (Hai vestito {0}/7 vesti)", numOfCloths ) );
                    return;
                }

                Blood blood = new Blood( Utility.Random( 0x122A, 5 ) );
                blood.MoveToWorld( m.Location, m.Map );

                m.SendMessage( 37, "La luce del sole brucia la tua pelle." );
                m.SendMessage( 37, string.Format( "Hai vestito solo {0} di 7 vesti necessarie a proteggerti.", numOfCloths ) );

                m.Say( true, "* ahhhh! *" );

                int level = (int)( ( DateTime.Now - m.CreationTime ).TotalDays / 365 );

                if( level < 0 )
                    level = 0;
                else if( level > 5 )
                    level = 5;

                int damage = (int)( ( 7 - numOfCloths ) * ( 1 - ( 0.20 * level ) ) );

                if( damage < 1 )
                    damage = 1;
                else if( damage > 7 )
                    damage = 7;

                m.Stam -= Utility.Dice( 1, 5, damage );
                m.Damage( damage );
            }
        }

        private static int ComputeSunMask( Mobile m )
        {
            // Servono 7 oggetti al vampiro per coprirsi
            // stivali
            // gambe
            // busto
            // braccia
            // mani
            // collo
            // testa

            List<Item> items = m.Items;

            int counter = 0;
            bool hasLegs = false;
            bool hasShirt = false;
            bool hasArms = false;
            bool hasShoes = false;
            bool hasNeck = false;
            bool hasHat = false;
            bool hasGloves = false;

            for( int i = 0; i < items.Count; ++i )
            {
                Item item = items[ i ];

                if( !item.Deleted )
                {
                    Layer l = item.Layer;

                    if( !hasNeck && ( l == Layer.Neck || item is BaseHat ) )
                    {
                        hasNeck = true;
                        counter++;
                    }

                    if( !hasShoes && ( l == Layer.Shoes || item is Robe || l == Layer.OuterLegs ) )
                    {
                        hasShoes = true;
                        counter++;
                    }

                    if( !hasHat && ( l == Layer.Helm || item is BaseHat ) )
                    {
                        hasHat = true;
                        counter++;
                    }

                    if( !hasArms && ( l == Layer.Arms || item is Robe ) )
                    {
                        hasArms = true;
                        counter++;
                    }

                    if( !hasLegs && ( l == Layer.InnerLegs || l == Layer.OuterLegs || l == Layer.Pants || item is Robe ) )
                    {
                        hasLegs = true;
                        counter++;
                    }

                    if( !hasShirt && ( l == Layer.Shirt || l == Layer.InnerTorso || l == Layer.MiddleTorso || l == Layer.OuterTorso ) )
                    {
                        hasShirt = true;
                        counter++;
                    }

                    if( !hasGloves && l == Layer.Gloves || item is Robe )
                    {
                        hasGloves = true;
                        counter++;
                    }
                }

                if( m.PlayerDebug )
                    m.SendMessage( "le {0} - sh {1} - ar {2} - sho {3} - ne {4} - ha {5} - gl - {6} - tot {7}", hasLegs, hasShirt, hasArms, hasShoes, hasNeck, hasHat, hasGloves, counter );
            }

            return counter;
        }

        private void DoWarn( Mobile m, int hours )
        {
            if( hours > BurnHour - 2 && hours < BurnHour && !m_DayWarned.Contains( m ) )
            {
                m.SendAsciiMessage( DayWarningMessage );
                m_DayWarned.Add( m );
            }
            else if( hours > NightHour - 2 && hours < NightHour && !m_NightWarned.Contains( m ) )
            {
                m.SendAsciiMessage( NightWarningMessage );
                m_NightWarned.Add( m );
            }
        }

        private static bool IsDayLight( Mobile m )
        {
            int global, personal;
            m.ComputeLightLevels( out global, out personal );

            return global >= LightCycle.DayLevel;
        }

        private static bool IsAtHome( Mobile m )
        {
            BaseHouse house = BaseHouse.FindHouseAt( m );

            return house != null;
        }

        private static bool IsDarkRegion( Mobile m )
        {
            Region r = m.Region;

            return ( r.IsPartOf( typeof( DungeonRegion ) ) ||
                        r.IsPartOf( "Wind" ) ||
                        r.IsPartOf( "Jail" ) );
        }
    }
}