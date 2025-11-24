/***************************************************************************
 *                                  LoginLogoutEventManager.cs
 *                            		--------------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Classe di supporto che gestisce i vari check da fare al login
 * 			ed al logout dei pg.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Engines.FoodDecaySystem;
using Midgard.Engines.HardLabour;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.SkillSystem;
using Midgard.Gumps;
using Server;
using Server.Accounting;
using Midgard.Engines.BountySystem;
//using Server.Engines.XmlPoints;
//using Server.Engines.XmlSpawner2;
using Server.Engines.XmlPoints;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Regions;
using Server.Spells;

namespace Midgard.Misc
{
    public static class LoginLogoutEventManager
    {
        public static readonly Point3D CoveLocation = new Point3D( 2230, 1224, 0 );
        public static readonly Point3D NewbieTownLocation = new Point3D( 5826, 2183, 0 );

        private static readonly string GeneralEventLogPath = Path.Combine( "Logs", "MidgardLoginLogoutEvents.log" );
        private static readonly string StaffLoginLogoutEventLogPath = Path.Combine( "Logs", "Midgard2LoginLogoutStaff.log" );
        private static readonly string FirstLoginEventLogPath = Path.Combine( "Logs", "Midgard2LoginACove.log" );

        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler( OnLogin );
            EventSink.Logout += new LogoutEventHandler( OnLogout );
            EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
        }

        private static void EventSink_Disconnected( DisconnectedEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from.AccessLevel > AccessLevel.Player )
                from.Hidden = true;
        }

        private static void OnLogin( LoginEventArgs e )
        {
            Midgard2PlayerMobile player = e.Mobile as Midgard2PlayerMobile;
            if( player == null )
                return;

            CheckHasOtherPendingCondemns( player );

            CheckFood( player );

            CheckFirstLogin( player );

            CheckStaffMemberLoggedIn( player );

            CheckPrisonerLoggedIn( player );

            CheckIsNewbie( player );

            if( TownSystem.Enabled )
                CheckNewRankedPlayer( player );

            CheckBountySystem( player );

            CheckPreAOS( player );

            AnnounceLogin( player );

            VerifyXmlPointsAttachment( player );

            VerifyHouseLogin( player );

            SkillSubCap.CheckLogin( player );

            VerifyLocation( player );
        }

        private static void VerifyLocation( Midgard2PlayerMobile player )
        {
            if( player.Location != Point3D.Zero )
                return;

            DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} è stato spostato a Cove perche' si trovava al Point.Zero.",
                                       player.Name, player.Serial, player.Account.Username, DateTime.Now ) );

            player.MoveToWorld( CoveLocation, Map.Felucca );
        }

        private static void VerifyHouseLogin( Mobile mobile )
        {
            BaseHouse house = BaseHouse.FindHouseAt( mobile );

            if( house != null && !house.Public && !house.IsFriend( mobile ) )
            {
                bool cannotExit = ( mobile.Aggressors != null && mobile.Aggressors.Count > 0 ) ||
                                  ( mobile.Aggressed != null && mobile.Aggressed.Count > 0 ) || mobile.Criminal;

                if( !cannotExit )
                    mobile.Location = house.BanLocation;
            }
        }

        private static void VerifyXmlPointsAttachment( Midgard2PlayerMobile mobile )
        {
            if( XmlAttach.FindAttachment( mobile, typeof( XmlPointsAttach ) ) == null )
                XmlAttach.AttachTo( mobile, new XmlPointsAttach() );
        }

        private static void OnLogout( LogoutEventArgs e )
        {
            Midgard2PlayerMobile player = e.Mobile as Midgard2PlayerMobile;
            if( player == null )
                return;

            // CheckIlshenar( player );

            CheckDoomGauntletOrFerry( player );

            CheckStaffMemberLoggedOut( player );

            CheckNewbieDungeon( player );
        }

        private static void AnnounceLogin( Mobile from )
        {
            if( from == null || from.AccessLevel > AccessLevel.Player )
                return;

            string destination = from.Region is Jail ? "Jail" : "Midgard";

            List<NetState> list = NetState.Instances;
            for( int i = 0; i < list.Count; ++i )
            {
                Mobile player = list[ i ].Mobile;
                if( player != null && player != from )
                    player.SendAsciiMessage( "Player {0} has arrived in {1}.", from.Name, destination );
            }
        }

        private static void CheckPreAOS( Mobile from )
        {
            if( !Server.Core.AOS && from.AccessLevel == AccessLevel.Player )
            {
                if( from.StatCap != 225 )
                {
                    from.StatCap = 225;
                    DoEventLog( string.Format( "Al pg {0} (seriale {1}) (Account {2}) in data {3} è stato resettato lo statcap a 225.",
                           from.Name, from.Serial, from.Account.Username, DateTime.Now ) );
                }

                if( from.RawStr > 100 )
                    from.RawStr = 100;

                if( from.RawDex > 100 )
                    from.RawDex = 100;

                if( from.RawInt > 100 )
                    from.RawInt = 100;

                if( from.SkillsCap != 7000 )
                {
                    from.SkillsCap = 7000;
                    DoEventLog( string.Format( "Al pg {0} (seriale {1}) (Account {2}) in data {3} è stato resettato lo skillcap a 7000.",
                           from.Name, from.Serial, from.Account.Username, DateTime.Now ) );
                }

                Skills skills = from.Skills;

                for( int i = 0; i < skills.Length; ++i )
                {
                    if( skills[ i ].Base > 100.0 )
                        skills[ i ].Base = 100.0;

                    if( skills[ i ].Cap > 100.0 )
                        skills[ i ].Cap = 100.0;
                }

                Account a = from.Account as Account;
                if( a == null )
                    return;

                from.Frozen = false;
            }
        }

        private static void CheckBountySystem( Midgard2PlayerMobile player )
        {
            if( player.ShowBountyUpdate )
            {
                if( player.BountyUpdateList != null )
                {
                    player.SendGump( new BountyStatusGump( player, player.BountyUpdateList ) );
                    player.BountyUpdateList.Clear();
                    player.ShowBountyUpdate = false;
                }
            }
        }

        private static void CheckIsNewbie( Mobile m )
        {
            if( m.Region.IsPartOf( "Newbie Dungeon" ) && ( m.RawStatTotal > 400 || m.SkillsTotal > 400 ) )
            {
                DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} ha loggato al newbie dungeon ma non e' piu' niubbo.",
                                           m.Name, m.Serial, m.Account.Username, DateTime.Now ) );

                m.MoveToWorld( CoveLocation, Map.Felucca );
            }
        }

        /// <summary>
        /// Check per vedere se chi ha loggato e' un prigioniero.
        /// Se ha dei minerali da minare ed e' fuori dalla colonia
        /// il metodo lo rimanda nel centro di essa.
        /// </summary>
        private static void CheckPrisonerLoggedIn( Midgard2PlayerMobile player )
        {
            if( player.Minerals2Mine > 0 && !player.Region.IsPartOf( "Hard Labour Penitentiary" ) )
            {
                DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} ha loggato ma ha degli ores da minare in sospeso.",
                                           player.Name, player.Serial, player.Account.Username, DateTime.Now ) );

                HardLabourCommands.ExecuteCondemn( player, false );
                HardLabourCommands.EquipPrisoner( player );
            }
        }

        /// <summary>
        /// Verifica che al Login lo stato di fame/sete sia attualizzato
        /// </summary>
        private static void CheckFood( Mobile m )
        {
            FoodDecayTimer.ComputeFoodStatLoss( m );
        }

        /// <summary>
        /// La prima volta che si logga si viene portati a Cove
        /// </summary>
        private static void CheckFirstLogin( Mobile mobile )
        {
            XmlData data = XmlAttach.FindAttachment( mobile, typeof( XmlData ) ) as XmlData;
            if( data != null && data.Data == "JustCreated" )
            {
                mobile.SendGump( new StartLocationGump() );
                data.Delete();
            }
        }

        /// <summary>
        /// Verifica e logga se chi ha effettuato il login e' un membro staff
        /// </summary>
        private static void CheckStaffMemberLoggedIn( Mobile m )
        {
            if ( m.AccessLevel <= AccessLevel.Player )
                return;

            DoEventLog( string.Format( "Il pg staff {0} (seriale {1}) ha loggato in data {2} e ora {3}.",
                                       m.Name, m.Serial, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString() ),
                        StaffLoginLogoutEventLogPath );

            m.LightLevel = 100;

            m.Hunger = 100;
            m.Thirst = 100;

            if( m.Poisoned )
                m.CurePoison( null );

            m.Hits = m.HitsMax;
            m.Stam = m.StamMax;
            m.Mana = m.ManaMax;
        }

        /// <summary>
        /// Verifica che non si possa sloggare su Ilshenar
        /// </summary>
        private static void CheckIlshenar( Mobile m )
        {
            if( m.AccessLevel == AccessLevel.Player && m.Map == Map.Ilshenar )
            {
                DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} ha sloggato su Ilshenar.",
                                           m.Name, m.Serial, m.Account.Username, DateTime.Now ) );

                m.MoveToWorld( CoveLocation, Map.Felucca );
            }
        }

        /// <summary>
        /// Verifica che non si possa sloggare nel Gauntlet e sul Ferry di Doom
        /// </summary>
        private static void CheckDoomGauntletOrFerry( Mobile m )
        {
            if( m.AccessLevel == AccessLevel.Player && ( SpellHelper.IsDoomGauntlet( m.Map, m.Location ) || SpellHelper.IsDoomFerry( m.Map, m.Location ) ) )
            {
                DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} ha sloggato al doom.",
                                           m.Name, m.Serial, m.Account.Username, DateTime.Now ) );

                m.MoveToWorld( CoveLocation, Map.Felucca );
            }
        }

        /// <summary>
        /// Verifica che non si possa sloggare nel NewbieDungeon
        /// </summary>
        private static void CheckNewbieDungeon( Mobile m )
        {
            if( m.AccessLevel == AccessLevel.Player && m.Region.IsPartOf( "Newbie Dungeon" ) )
            {
                DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} ha sloggato al newbie dungeon.",
                                           m.Name, m.Serial, m.Account.Username, DateTime.Now ) );

                m.MoveToWorld( CoveLocation, Map.Felucca );
            }
        }

        /// <summary>
        /// Verifica che si possa effettivamente loggare senza cioe' avere
        /// altre condanne ai lavori forzati. Se ce ne sono disconnette dopo 30 secondi.
        /// </summary>
        private static void CheckHasOtherPendingCondemns( Midgard2PlayerMobile player )
        {
            if( player.AccessLevel != AccessLevel.Player )
                return;

            if( player.Minerals2Mine <= 0 && HardLabourCommands.HasOtherPendingCondemns( player ) )
            {
                HardLabourCommands.HandleOtherPendingCondemns( player );

                DoEventLog( string.Format( "Il pg {0} (seriale {1}) (Account {2}) in data {3} ha loggato ma ha degli ores da minare in sospeso.",
                                           player.Name, player.Serial, player.Account.Username, DateTime.Now ) );
            }
        }

        /// <summary>
        /// Verifica e logga se chi ha effettuato il logout e' un membro staff
        /// </summary>
        private static void CheckStaffMemberLoggedOut( Mobile player )
        {
            if( player == null )
                return;

            if( player.AccessLevel <= AccessLevel.Player )
                return;

            Account a = player.Account as Account;
            if( a == null )
                return;

            try
            {
                string toLog = string.Format( "Il pg staff {0} (seriale {1}) ha sloggato in data {2} e ora {3}. Il suo tempo trascorso online è di {4} minuti.",
                        player.Name, player.Serial, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(),
                        ( DateTime.Now - a.LastLogin ).TotalMinutes.ToString( "F0" ) );

                DoEventLog( toLog, StaffLoginLogoutEventLogPath );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        /// <summary>
        /// Verifica se un pg puo' iniziate ad avere punti per il town system
        /// </summary>
        private static void CheckNewRankedPlayer( Mobile m )
        {
            if( m == null )
                return;

            TownPlayerState tps = TownPlayerState.Find( m );

            if( tps != null && tps.TownRankPoints == -1000 )
                tps.TownRankPoints = 0;
        }

        private static void DoEventLog( string toLog )
        {
            DoEventLog( toLog, GeneralEventLogPath );
        }

        private static void DoEventLog( string toLog, string path )
        {
            try
            {
                using( StreamWriter op = new StreamWriter( path, true ) )
                {
                    op.WriteLine( toLog );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}