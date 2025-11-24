/***************************************************************************
 *                                  MurderInfoHelper.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Midgard.Engines.HardLabour;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.JailSystem;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.MurderInfo
{
    public enum MurderInfoCheckTipe
    {
        DailyGlobal,
        AtKill
    }

    public class MurderInfoHelper
    {
        #region Misc checks and members
        /// <summary>
        /// Metodo per verificare se la vittima e' presente nella lista di murders info di killer
        /// </summary>
        public static bool IsInList( Mobile killer, Mobile victim )
        {
            if( killer == null || killer.Deleted || victim == null || victim.Deleted )
                return true;

            Midgard2PlayerMobile m2pm = killer as Midgard2PlayerMobile;
            if( m2pm == null )
                return true;

            if( m2pm.MurderInfoes != null )
            {
                foreach( MurderInfo i in m2pm.MurderInfoes )
                {
                    if( i.Victim != null && i.Victim == victim )
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Metodo per verificare se la vittima e' stata uccisa precedentemente da meno di tot ore
        /// </summary>
        public static bool IsInLamerTime( Mobile killer, Mobile victim )
        {
            if( killer == null || killer.Deleted )
                return true;

            Midgard2PlayerMobile m2pm = killer as Midgard2PlayerMobile;
            if( m2pm == null )
                return true;

            MurderInfo info = GetLastInfo( killer, victim );

            return ( info == null || DateTime.Now - info.TimeOfDeath < Config.LamerDelay );
        }

        /// <summary>
        /// Metodo per verificare se murder info e' scaduta o non piu' applicabile
        /// </summary>
        public static bool IsValidInfo( MurderInfo info )
        {
            if( info == null || info.Victim == null || info.Victim.Deleted )
                return false;

            if( Config.CheckType == MurderInfoCheckTipe.DailyGlobal )
            {
                // Console.WriteLine( "IsValidInfo: " + ( DateTime.Compare( DateTime.Now.Date, info.TimeOfDeath.Date ) == 0 ).ToString() );
                return DateTime.Compare( DateTime.Now.Date, info.TimeOfDeath.Date ) == 0;
            }
            else
                return ( DateTime.Now - info.TimeOfDeath < Config.MurderInfoDecay );
        }

        /// <summary>
        /// Se esistono criteri di differenziazione dei kills, calcola la somma
        /// dei kills equivalenti
        /// </summary>
        public static int GetInfoesEquivalent( Mobile killer )
        {
            int equivalent = 0;

            if( !( killer is Midgard2PlayerMobile ) || ( (Midgard2PlayerMobile)killer ).MurderInfoes == null )
                return 0;

            // citizens of bukka den cannot be punished for killing cocitizens.
            if( TownSystem.Find( killer ) == TownSystem.BuccaneersDen )
                return 0;

            foreach( MurderInfo info in ( (Midgard2PlayerMobile)killer ).MurderInfoes )
            {
                if( info == null )
                    continue;
                else if( DateTime.Now - info.TimeOfDeath < Config.LamerDelay )
                    equivalent += Config.InfoEqForKilledInLamerTime;
                else
                    equivalent += Config.InfoEqForKilledOutOfLamerTime;
            }

            return equivalent;
        }

        /// <summary>
        /// Metodo per loggare gli eventi del sistema MurderInfo
        /// </summary>
        public static void PunishmentLog( string log )
        {
            try
            {
                TextWriter tw = File.AppendText( Config.PunishmentsLogPath );
                tw.WriteLine( log );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.Write( "Log di un kill proibito fallito: {0}", ex );
            }
        }
        #endregion

        #region GetInfoes...
        /// <summary>
        /// Metodo per ottenere la info piu' recente relativa a killer
        /// </summary>
        /// <returns>info se non nulla, null altrimenti</returns>
        public static MurderInfo GetFirstInfo( Mobile killer, Mobile victim )
        {
            Midgard2PlayerMobile m2pm = killer as Midgard2PlayerMobile;
            if( m2pm == null )
                return null;

            List<MurderInfo> list = m2pm.MurderInfoes;
            if( list == null || list.Count < 1 )
                return null;

            try
            {
                if( list.Count > 1 )
                    list.Sort( MurderInfo.MurderInfoesComparer.Instance );
            }
            catch { }

            for( int i = 0; i < list.Count; i++ )
            {
                if( list[ i ] != null && list[ i ].Victim != null && list[ i ].Victim == victim )
                    return list[ i ];
            }

            return null;
        }

        /// <summary>
        /// Metodo per ottenere la info meno recente relativa a killer
        /// </summary>
        /// <returns>info se non nulla, null altrimenti</returns>
        public static MurderInfo GetLastInfo( Mobile killer, Mobile victim )
        {
            Midgard2PlayerMobile m2pm = killer as Midgard2PlayerMobile;
            if( m2pm == null || m2pm.MurderInfoes == null || m2pm.MurderInfoes.Count < 1 )
                return null;

            List<MurderInfo> list = m2pm.MurderInfoes;

            if( list == null )
                return null;

            if( list.Count > 1 )
                list.Sort( MurderInfo.MurderInfoesComparer.Instance );

            for( int i = list.Count - 1; i >= 0; i-- )
            {
                if( list[ i ] != null && list[ i ].Victim != null && list[ i ].Victim == victim )
                    return list[ i ];
            }

            return null;
        }
        #endregion

        #region daily and atKill handlers
        /// <summary>
        /// Metodo per la gestione degli eventi relativi alle murder info
        /// </summary>		
        public static void HandleInfo( Mobile killer, Mobile victim )
        {
            if( Config.MurderInfoEnabled )
                return;

            if( killer == null || killer.Deleted || victim == null || victim.Deleted || !( killer is Midgard2PlayerMobile ) )
                return;

            Console.WriteLine( "IsInList " + IsInList( killer, victim ) );
            Console.WriteLine( "IsInLamerTime " + IsInLamerTime( killer, victim ) );

            if( ( (Midgard2PlayerMobile)killer ).MurderInfoes != null )
                Console.WriteLine( "MurderInfoes " + ( (Midgard2PlayerMobile)killer ).MurderInfoes.Count );

            if( IsInList( killer, victim ) && IsInLamerTime( killer, victim ) )
                return;

            if( ( (Midgard2PlayerMobile)killer ).MurderInfoes != null )
                Console.WriteLine( "MurderInfoes.Count pre " + ( (Midgard2PlayerMobile)killer ).MurderInfoes.Count );

            MurderInfoPersistance.RegisterInfo( killer, new MurderInfo( killer, victim, DateTime.Now ) );

            if( ( (Midgard2PlayerMobile)killer ).MurderInfoes != null )
                Console.WriteLine( "MurderInfoes.Count post " + ( (Midgard2PlayerMobile)killer ).MurderInfoes.Count );

            if( Config.CheckType == MurderInfoCheckTipe.AtKill )
            {
                int infoes = GetInfoesEquivalent( killer );

                if( infoes >= Config.InfoesToGetRejected )
                    DoRejected( killer, victim );
                else if( infoes >= Config.InfoesToGetOres )
                    DoOres( killer, victim, Config.MineralsToMine );
                else if( infoes >= Config.InfoesToGetJail )
                    DoJail( killer, victim, Config.HoursOfJail );
            }
        }

        /// <summary>
        /// Metodo per la gestione GLOBALE degli eventi relativi alle murder info
        /// </summary>	
        public static void HandleGlobalMurderInfoes( bool clearAfterHandle )
        {
            if( Config.MurderInfoEnabled )
                return;

            if( Config.CheckType == MurderInfoCheckTipe.AtKill )
                return;

            PunishmentLog( String.Format( "Daily global murder info check for date {0}.", DateTime.Now.ToShortDateString() ) );

            try
            {
                MurderInfoReport.WriteReport( "web/MurderReport.xml" );
            }
            catch
            {
            }

            if( Config.FtpEnabled )
                FtpService.UploadFile( "web/MurderReport.xml", "public/MurderReport.xml" );

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                List<Mobile> killers = MurderInfoPersistance.GetKillers();
                if( killers != null )
                {
                    foreach( Mobile k in killers )
                    {
                        int infoes = GetInfoesEquivalent( k );

                        if( Config.RejectedEnabled && infoes >= Config.InfoesToGetRejected )
                            DoRejected( k );
                        else if( Config.OresEnabled && infoes >= Config.InfoesToGetOres )
                            DoOres( k, Config.MineralsToMine );
                        else if( Config.JailEnabled && infoes >= Config.InfoesToGetJail )
                            DoJail( k, Config.HoursOfJail );
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( "HandleGlobalMurderInfoes failed: {0}", ex );
            }

            try
            {
                if( clearAfterHandle )
                    MurderInfoPersistance.UnRegisterAll();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "HandleGlobalMurderInfoes failed: {0}", ex );
            }

            Console.WriteLine( "Operazione conclusa in {0:F4} secondi.", ( sw.ElapsedMilliseconds / 1000 ) );
            sw.Stop();
        }

        public static void HandleKiller( Mobile killer, bool clearAfterHandle )
        {
            if( !Config.MurderInfoEnabled )
                return;

            int infoes = GetInfoesEquivalent( killer );

            if( Config.RejectedEnabled && infoes >= Config.InfoesToGetRejected )
                DoRejected( killer );
            else if( Config.OresEnabled && infoes >= Config.InfoesToGetOres )
                DoOres( killer, Config.MineralsToMine );
            else if( Config.JailEnabled && infoes >= Config.InfoesToGetJail )
                DoJail( killer, Config.HoursOfJail );

            if( clearAfterHandle )
                MurderInfoPersistance.UnregisterKiller( killer );
        }
        #endregion

        #region DoPunishment...
        /// <summary>
        /// Metodo per eseguire una Jail
        /// </summary>
        public static void DoJail( Mobile killer, Mobile victim, TimeSpan hours )
        {
            JailSystem.JailSystem.Jail( killer, hours, "Killing a co-citizen", true, "The Citizen Guards" );

            PunishmentLog( string.Format( "Il pg {0} di citt. {1} ha ucciso il pg {2} di citt. {3} in data {4} e viene jailato.",
                                          killer.Name, TownHelper.FindTownName( killer ), victim.Name, TownHelper.FindTownName( victim ), DateTime.Now.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) ) );
        }

        /// <summary>
        /// Metodo per eseguire una Jail
        /// </summary>
        public static void DoJail( Mobile killer, TimeSpan hours )
        {
            JailSystem.JailSystem.Jail( killer, hours, "Killing a co-citizen", true, "The Citizen Guards" );

            PunishmentLog( string.Format( "Il pg {0} di citt. {1} ha collezionato almeno {2} murder infoes in data {3} e viene jailato.",
                                          killer.Name, TownHelper.FindTownName( killer ), Config.InfoesToGetJail, DateTime.Now.ToLongTimeString() ) );
        }

        /// <summary>
        /// Metodo per assegnare degli ores da minare
        /// </summary>
        public static void DoOres( Mobile killer, Mobile victim, int ores )
        {
            HardLabourCommands.DoHardLabourCondemn( killer, ores, "The Citizen Guards" );

            PunishmentLog( string.Format( "Il pg {0} di citt. {1} ha ucciso il pg {2} di citt. {3} in data {4} e viene condannato a minare.",
                                          killer.Name, TownHelper.FindTownName( killer ), victim.Name, TownHelper.FindTownName( victim ), DateTime.Now.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) ) );
        }

        /// <summary>
        /// Metodo per assegnare degli ores da minare
        /// </summary>
        public static void DoOres( Mobile killer, int ores )
        {
            HardLabourCommands.DoHardLabourCondemn( killer, ores, "The Citizen Guards" );

            PunishmentLog( string.Format( "Il pg {0} di citt. {1} ha collezionato almeno {2} murder infoes in data {3} e viene condannato a minare.",
                                          killer.Name, TownHelper.FindTownName( killer ), Config.InfoesToGetOres, DateTime.Now.ToLongTimeString() ) );
        }

        /// <summary>
        /// Metodo per eseguire eseguire il reject di un player
        /// </summary>
        public static void DoRejected( Mobile killer, Mobile victim )
        {
            // TODO: Implementare il rejected in Midgard2PlayerMobile

            PunishmentLog( string.Format( "Il pg {0} di citt. {1} ha ucciso il pg {2} di citt. {3} in data {4} e viene reso rejetto.",
                                          killer.Name, TownHelper.FindTownName( killer ), victim.Name, TownHelper.FindTownName( victim ), DateTime.Now.ToLongTimeString() ) );
        }

        /// <summary>
        /// Metodo per eseguire eseguire il reject di un player
        /// </summary>
        public static void DoRejected( Mobile killer )
        {
            // TODO: Implementare il rejected in Midgard2PlayerMobile

            PunishmentLog( string.Format( "Il pg {0} di citt. {1} ha collezionato almeno {2} murder infoes in data {3} e viene rejettao.",
                                          killer.Name, TownHelper.FindTownName( killer ), Config.InfoesToGetRejected, DateTime.Now.ToLongTimeString() ) );
        }
        #endregion
    }
}