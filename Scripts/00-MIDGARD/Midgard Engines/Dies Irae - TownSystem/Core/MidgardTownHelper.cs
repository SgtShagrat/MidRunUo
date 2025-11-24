/***************************************************************************
 *                                  MidgardTownHelper.cs
 *                            		--------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Classe statica di membri di supporto er il MidgardTownSystem
 * 
 ***************************************************************************/

// #define DebugTownHelper

using System;
using System.Collections.Generic;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Midgard.Engines.MidgardTownSystem
{
    public static class TownHelper
    {
        #region checks
        /// <summary>
        /// Check per verificare che un Mobile e' cittadinato
        /// </summary>
        public static bool IsCitizenOfAnyTown( Mobile mobile )
        {
            return TownSystem.Find( mobile, ( mobile is BaseCreature ) ) != null;
        }

        /// <summary>
        /// Check per verificare che un Mobile sia nemico cittadino
        /// </summary>
        public static bool IsCitizenCriminal( Mobile mobile )
        {
            TownPlayerState state = TownPlayerState.Find( mobile, true );
            return state != null && state.CitizenCriminal;
        }

        /// <summary>
        /// Check if 'from' can build a house on location 'point'
        /// </summary>
        public static bool CanBuildHouseOnLocation( Point3D point, Mobile from )
        {
            foreach( TownSystem ourTown in TownSystem.TownSystems )
            {
                if( ourTown.IsUnderNoHousingCriteria( point ) )
                {
                    TownSystem fromTown = TownSystem.Find( from );

                    // Non cittadini e cittadini della citta' opposta non possono
                    // costruire nei territori limitrofi alla città 'ourTown'
                    if( fromTown == null )
                        return false;
                    else if( ourTown != fromTown )
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region IsInAlliedCity(...)
        /// <summary>
        /// Metodo invocato per verificare se un Mobile è in una città alleata
        /// </summary>		
        public static bool IsInAlliedCity( Mobile mobile )
        {
            if( mobile == null || mobile.Deleted )
                return false;

            TownSystem sys = TownSystem.Find( mobile, ( mobile is BaseCreature ) );

            return IsInAlliedCity( mobile.Region, sys );
        }

        /// <summary>
        /// Metodo invocato per verificare se un Corpse è nella citta' alleata alla città di 'corpse'
        /// </summary>		
        public static bool IsInAlliedCity( Corpse corpse )
        {
            if( corpse == null || corpse.Deleted )
                return false;

            return IsInHisOwnCity( corpse.Location, corpse.Map, TownSystem.Find( corpse ) );
        }

        /// <summary>
        /// Metodo invocato per verificare se un Point3D e' nella regione alleata alla città che contiene 'point'
        /// </summary>		
        public static bool IsInAlliedCity( Point3D point, Map map, TownSystem system )
        {
            BaseHouse house = BaseHouse.FindHouseAt( point, map, 16 );
            if( house != null )
                return system.IsAlliedTo( TownSystem.Find( house ) );

            return IsInHisOwnCity( Region.Find( point, map ), system );
        }

        /// <summary>
        /// Metodo invocato per verificare se una Region e' alleata alla città system
        /// </summary>		
        public static bool IsInAlliedCity( Region region, TownSystem system )
        {
            return system != null && system.IsAlliedTo( TownSystem.Find( region ) );
        }
        #endregion

        #region IsInHisOwnCity(...)
        /// <summary>
        /// Metodo invocato per verificare se un Mobile è nella proprià città
        /// </summary>		
        public static bool IsInHisOwnCity( Mobile mobile )
        {
            if( mobile == null || mobile.Deleted )
                return false;

            TownSystem sys = TownSystem.Find( mobile, ( mobile is BaseCreature ) );

            if( sys != null )
            {
                BaseHouse house = BaseHouse.FindHouseAt( mobile );
                if( house != null )
                    return sys == TownSystem.Find( house );
            }

            return IsInHisOwnCity( mobile.Region, sys );
        }

        /// <summary>
        /// Metodo invocato per verificare se un Corpse è nella proprià città
        /// </summary>		
        public static bool IsInHisOwnCity( Corpse corpse )
        {
            if( corpse == null || corpse.Deleted )
                return false;

            return IsInHisOwnCity( corpse.Location, corpse.Map, TownSystem.Find( corpse ) );
        }

        /// <summary>
        /// Metodo invocato per verificare se un Point3D e' nella regione cittadina di system
        /// </summary>		
        public static bool IsInHisOwnCity( Point3D point, Map map, TownSystem system )
        {
            BaseHouse house = BaseHouse.FindHouseAt( point, map, 16 );
            if( house != null )
                return system == TownSystem.Find( house );

            return IsInHisOwnCity( Region.Find( point, map ), system );
        }

        /// <summary>
        /// Metodo invocato per verificare se una Region e' nella regione cittadina 'town'
        /// NB: Attenzione che le <see cref="BaseHouse"/> hanno region a se stanti!
        /// </summary>		
        public static bool IsInHisOwnCity( Region region, TownSystem system )
        {
            return system != null && TownSystem.Find( region ) == system;
        }
        #endregion

        #region InInAnyCity(...)
        /// <summary>
        /// Metodo invocato per verificare se un Mobile è in una città
        /// </summary>		
        public static bool InInAnyCity( Mobile mobile )
        {
            if( mobile == null || mobile.Deleted )
                return false;

            return InInAnyCity( mobile.Region );
        }

        /// <summary>
        /// Metodo invocato per verificare se un Corpse è in una città
        /// </summary>		
        public static bool InInAnyCity( Corpse corpse )
        {
            if( corpse == null || corpse.Deleted )
                return false;

            return InInAnyCity( corpse.Location, corpse.Map );
        }

        /// <summary>
        /// Metodo invocato per verificare se un Point3D è in una città
        /// </summary>		
        public static bool InInAnyCity( Point3D point, Map map )
        {
            return InInAnyCity( Region.Find( point, map ) );
        }

        /// <summary>
        /// Metodo invocato per verificare se una Region è in una città
        /// </summary>		
        public static bool InInAnyCity( Region region )
        {
            return TownSystem.Find( region ) != null;
        }
        #endregion

        #region Find(...)
        public static string FindTownName( Mobile mobile )
        {
            TownSystem t = TownSystem.Find( mobile );

            return t != null ? t.Definition.TownName.ToString() : String.Empty;
        }

        public static string FindTownName( MidgardTowns town )
        {
            TownSystem t = TownSystem.Find( town );

            return t != null ? t.Definition.TownName.ToString() : String.Empty;
        }

        public static MidgardTowns ParseMidgardTown( string name )
        {
            if( String.IsNullOrEmpty( name ) || !Enum.IsDefined( typeof( MidgardTowns ), name ) )
                return MidgardTowns.None;

            return (MidgardTowns)Enum.Parse( typeof( MidgardTowns ), name );
        }

        public static TownSystem FindTownSystemFromAccountTag( Mobile from )
        {
            Account a = from.Account as Account;

            if( a != null )
            {
                return TownSystem.Parse( a.GetTag( "Cittadinanza" ) );
            }

            return null;
        }
        #endregion

        #region citizen status handlers
        public static void DoAccountJoin( Mobile mobile, TownSystem system )
        {
            Account a = mobile.Account as Account;
            if( a == null )
                return;

            //a.RemoveTag( "Cittadinanza" );
            //a.AddTag( "Cittadinanza", Enum.GetName( typeof( MidgardTowns ), system.Definition.Town ) );
            TownLog.Log( LogType.Membership, String.Format( "Account {0} joined {1} system on datetime {2}.", a.Username, system.Definition.TownName, DateTime.Now ) );

            Mobile m = mobile as Midgard2PlayerMobile;

            //for( int i = 0; i < a.Count; i++ )
            //{
            //    Midgard2PlayerMobile m = a[ i ] as Midgard2PlayerMobile;

            try
            {
                if( m == null || m.Deleted )
                    return; // continue;

                TownPlayerState tps = TownPlayerState.Find( m );
                if( tps != null )
                    tps.Detach();

                tps = new TownPlayerState( system, m );
                tps.CheckAttach();

                m.InvalidateProperties();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( "Town join process failed: {0}", ex );
            }
            //}

            string message = "Il tuo personaggio è stato reso cittadino di questa comunità.<br>" +
                             "Per farti rimuovere dall'albo cittadino dovrai contattare il notaio cittadino.<br>" +
                             "Con questo personaggio avrai accesso ai servizi cittadini.";

            mobile.SendGump( new NoticeGump( 1060635, 30720, message, 0xFFC000, 420, 280, CloseNoticeCallback, new object[] { } ) );

            //mobile.SendGump( new NoticeGump( 1060635, 30720, "Il tuo account è stato assegnato <em><basefont color=red>permanentemente</basefont></em> alla città di " +
            //                                system.Definition.TownName + ".\n" +
            //                               "Per farti rimuovere dall'albo cittadino dovrai contattare il Governatore.\n" +
            //                               "Ora, con ogni personaggio di questo account (salvo eccezioni), avrai accesso ai servizi cittadini.",
            //                               0xFFC000, 420, 280, CloseNoticeCallback, new object[] { } ) );
        }

        private static void CloseNoticeCallback( Mobile from, object state )
        {
        }

        public static void DoAccountReset( Account a )
        {
            if( a == null )
                return;

            a.RemoveTag( "Cittadinanza" );
            TownLog.Log( LogType.Membership, String.Format( "Account {0} resetted on datetime {1}.", a.Username, DateTime.Now ) );

            for( int i = 0; i < a.Count; i++ )
            {
                Midgard2PlayerMobile m = a[ i ] as Midgard2PlayerMobile;
                if( m == null || m.Deleted )
                    continue;

                DoPlayerReset( m );
            }
        }

        public static void DoPlayerReset( Mobile mobile )
        {
            if( mobile == null || mobile.Deleted )
                return;

            TownPlayerState tps = TownPlayerState.Find( mobile );
            if( tps != null )
                tps.Detach();

            mobile.InvalidateProperties();
        }

        public static void DoSetCitizen( Mobile mobile, TownSystem system )
        {
            if( mobile == null || mobile.Deleted )
                return;

            if( system != null )
            {
                TownPlayerState tps = TownPlayerState.Find( mobile );
                if( tps != null )
                    tps.Detach();

                new TownPlayerState( system, mobile );
            }
        }

        public static void SetBasicCitienAccess()
        {
            foreach( TownSystem t in TownSystem.TownSystems )
            {
                foreach( TownPlayerState tps in t.Players )
                {
                    if( tps != null && tps.Mobile != null )
                    {
                        if( tps.TownLevel != null )
                        {
                            tps.TownLevel.SetCitizen();
                            tps.TownLevel = TownAccessLevel.Citizen;
                        }
                    }
                }
            }
        }

        public static void FixPlayerStates()
        {
            List<Mobile> list = new List<Mobile>();

            foreach( Account acct in Accounts.GetAccounts() )
            {
                for( int i = 0; i < acct.Count; i++ )
                {
                    Mobile m = acct[ i ];
                    if( m == null )
                        continue;

                    list.Add( m );
                }
            }

            foreach( Mobile mob in list )
            {
                TownSystem ts = FindTownSystemFromAccountTag( mob );
                if( ts != null && TownPlayerState.Find( mob ) == null )
                {
                    new TownPlayerState( ts, mob );
                }
            }
        }
        #endregion

        public static bool IsTownMurdererOrBanned( Mobile m )
        {
            return IsTownMurderer( m ) || IsTownBanned( m ) || IsTownPermaBanned( m );
        }

        public static bool IsTownMurderer( Mobile m )
        {
            TownPlayerState state = TownPlayerState.Find( m, true );
            return state != null && state.IsTownMurderer;
        }

        #region TownBan
        #region IsTownBanned
        /// <summary>
        /// Check per verificare che un Mobile e' esilisato.
        /// L'eventuale citta' viene calcolata in base alla sua locazione.
        /// </summary>
        public static bool IsTownBanned( Mobile m )
        {
            TownSystem system = TownSystem.Find( m.Location, m.Map );

            if( m is Midgard2PlayerMobile )
            {
                return IsTownBanned( (Midgard2PlayerMobile)m, system );
            }
            else if( m is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)m;
                Mobile master = bc.GetMaster();

                if( master != null && master is Midgard2PlayerMobile )
                    return IsTownBanned( (Midgard2PlayerMobile)master, system );
            }

            return false;
        }

        /// <summary>
        /// Check per verificare che un Mobile e' esilisato.
        /// L'eventuale citta' viene calcolata in base a p e map
        /// </summary>
        public static bool IsTownBanned( Mobile m, Point3D p, Map map )
        {
            if( m is Midgard2PlayerMobile )
            {
                return IsTownBanned( (Midgard2PlayerMobile)m, TownSystem.Find( p, map ) );
            }

            return false;
        }

        /// <summary>
        /// Check per verificare che un Mobile e' esilisato.
        /// Il sistema cittadino dal quale si verifica l'esilio e' system
        /// </summary>
        public static bool IsTownBanned( Midgard2PlayerMobile m2Pm, TownSystem system )
        {
            if( m2Pm == null || system == null )
                return false;

            if( m2Pm.TownBans == null )
            {
                m2Pm.TownBans = new TownBanAttribute( m2Pm );
                return false;
            }

            return m2Pm.TownBans[ m2Pm.TownBans.GetFlagFromTown( system.Definition.Town ) ];
        }

        /// <summary>
        /// Check per verificare che un Mobile e' esilisato.
        /// L'eventuale citta' viene calcolata in base alla sua locazione.
        /// </summary>
        public static bool IsTownPermaBanned( Mobile m )
        {
            TownSystem system = TownSystem.Find( m.Location, m.Map );

            if( m is Midgard2PlayerMobile )
            {
                return IsTownPermaBanned( (Midgard2PlayerMobile)m, system );
            }
            else if( m is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)m;
                Mobile master = bc.GetMaster();

                if( master != null && master is Midgard2PlayerMobile )
                    return IsTownPermaBanned( (Midgard2PlayerMobile)master, system );
            }

            return false;
        }

        /// <summary>
        /// Check per verificare che un Mobile e' esilisato.
        /// L'eventuale citta' viene calcolata in base a p e map
        /// </summary>
        public static bool IsTownPermaBanned( Mobile m, Point3D p, Map map )
        {
            if( m is Midgard2PlayerMobile )
            {
                return IsTownPermaBanned( (Midgard2PlayerMobile)m, TownSystem.Find( p, map ) );
            }

            return false;
        }

        /// <summary>
        /// Check per verificare che un Mobile e' esilisato.
        /// Il sistema cittadino dal quale si verifica l'esilio e' system
        /// </summary>
        public static bool IsTownPermaBanned( Midgard2PlayerMobile m2Pm, TownSystem system )
        {
            if( m2Pm == null || system == null )
                return false;

            if( m2Pm.TownPermaBans == null )
            {
                m2Pm.TownPermaBans = new TownPermaBanAttribute( m2Pm );
                return false;
            }

            return m2Pm.TownPermaBans[ m2Pm.TownPermaBans.GetFlagFromTown( system.Definition.Town ) ];
        }
        #endregion

        public static void DoTownBan( Mobile mobile, TownSystem townSystem )
        {
            DoTownBan( mobile, townSystem, true );
        }

        public static void DoTownBan( Mobile mobile, TownSystem townSystem, bool shouldBeBanned )
        {
            if( mobile == null || mobile.Deleted )
                return;

            Midgard2PlayerMobile m2Pm = mobile as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            if( shouldBeBanned )
            {
                TownPlayerState tps = TownPlayerState.Find( mobile );
                if( tps != null && tps.TownSystem != null && tps.TownSystem == townSystem )
                    tps.Detach();
            }

            TownBanAttribute att = m2Pm.TownBans;
            if( att == null )
                m2Pm.TownBans = new TownBanAttribute( m2Pm );

            if( att != null )
                att.SetFlag( att.GetFlagFromTown( townSystem.Definition.Town ), shouldBeBanned );

            m2Pm.InvalidateProperties();

            m2Pm.Delta( MobileDelta.Noto );

            TownLog.Log( LogType.Membership, String.Format( "Player {0} (account {1}) {3} from town {2}.",
                m2Pm.Name, m2Pm.Account.Username, townSystem.Definition.TownName, ( shouldBeBanned ? "BANNED" : "UN-BANNED" ) ) );
        }

        public static void DoTownPermaBan( Mobile mobile, TownSystem townSystem )
        {
            DoTownPermaBan( mobile, townSystem, true );
        }

        public static void DoTownPermaBan( Mobile mobile, TownSystem townSystem, bool shouldBePermaBanned )
        {
            if( mobile == null || mobile.Deleted )
                return;

            Midgard2PlayerMobile m2Pm = mobile as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            if( shouldBePermaBanned )
            {
                TownPlayerState tps = TownPlayerState.Find( mobile );
                if( tps != null && tps.TownSystem != null && tps.TownSystem == townSystem )
                    tps.Detach();
            }

            TownPermaBanAttribute att = m2Pm.TownPermaBans;
            if( att == null )
                m2Pm.TownPermaBans = new TownPermaBanAttribute( m2Pm );

            if( att != null )
                att.SetFlag( att.GetFlagFromTown( townSystem.Definition.Town ), shouldBePermaBanned );

            m2Pm.InvalidateProperties();

            m2Pm.Delta( MobileDelta.Noto );

            TownLog.Log( LogType.Membership, String.Format( "Player {0} (account {1}) {3} from town {2}.",
                m2Pm.Name, m2Pm.Account.Username, townSystem.Definition.TownName, ( shouldBePermaBanned ? "PermaBANNED" : "UN-PermaBANNED" ) ) );
        }
        #endregion

        public static bool CheckEquip( Mobile from, TownSystem system, bool message )
        {
            if( TownSystem.Find( from ) != system )
            {
                if( message )
                    from.SendMessage( from.Language == "ITA" ? "Solo i cittadini di {0} possono indossarlo." : "Only citizens of {0} can wear this.", system.Definition.TownName );

                return false;
            }
            else
                return true;
        }
    }
}