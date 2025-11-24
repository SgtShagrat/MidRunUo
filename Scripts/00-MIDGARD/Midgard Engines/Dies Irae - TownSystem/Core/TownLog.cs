/***************************************************************************
 *                                  TownLog.cs
 *                            		----------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Classe statica di membri per il log degli eventi cittadini
 * 
 ***************************************************************************/

using System;
using System.IO;

namespace Midgard.Engines.MidgardTownSystem
{
    public enum LogType
    {
        General,
        Membership,
        Commands,
        Access,
        Commercial,
        Traps,
        Fields,
        Errors,
        Kills,
        Treasure,
        Bans,
        Houses,
        Mobiles
    }

    public static class TownLog
    {
        private const string GeneralLogPath = "Logs/TownSystem/General.log";
        private const string MembershipLogPath = "Logs/TownSystem/Membership.log";
        private const string CommandsLogPath = "Logs/TownSystem/Commands.log";
        private const string AccessLogPath = "Logs/TownSystem/Access.log";
        private const string CommercialLogPath = "Logs/TownSystem/Commercial.log";
        private const string TrapsLogPath = "Logs/TownSystem/Traps.log";
        private const string FieldsLogPath = "Logs/TownSystem/Fields.log";
        private const string ErrorsLogPath = "Logs/TownSystem/Errors.log";
        private const string KillsLogPath = "Logs/TownSystem/Kills.log";
        private const string TreasureLogPath = "Logs/TownSystem/Treasure.log";
        private const string BansLogPath = "Logs/TownSystem/Bans.log";
        private const string HousesLogPath = "Logs/TownSystem/TownHouses.log";
        private const string MobilesLogPath = "Logs/TownSystem/Mobiles.log";

        public static void Log( string toLog )
        {
            Log( LogType.General, toLog );
        }

        public static void Log( LogType type, string toLog )
        {
            if( !Directory.Exists( "Logs" ) )
                Directory.CreateDirectory( "Logs" );

            if( !Directory.Exists( "Logs/TownSystem" ) )
                Directory.CreateDirectory( "Logs/TownSystem" );

            string path = GeneralLogPath;

            switch( type )
            {
                case LogType.Membership:
                    path = MembershipLogPath;
                    break;
                case LogType.Commands:
                    path = CommandsLogPath;
                    break;
                case LogType.Access:
                    path = AccessLogPath;
                    break;
                case LogType.Commercial:
                    path = CommercialLogPath;
                    break;
                case LogType.Traps:
                    path = TrapsLogPath;
                    break;
                case LogType.Fields:
                    path = FieldsLogPath;
                    break;
                case LogType.Errors:
                    path = ErrorsLogPath;
                    break;
                case LogType.Kills:
                    path = KillsLogPath;
                    break;
                case LogType.Treasure:
                    path = TreasureLogPath;
                    break;
                case LogType.Bans:
                    path = BansLogPath;
                    break;
                case LogType.Houses:
                    path = HousesLogPath;
                    break;
                case LogType.Mobiles:
                    path = MobilesLogPath;
                    break;
                default:
                    break;
            }

            try
            {
                TextWriter tw = File.AppendText( path );
                tw.WriteLine( toLog );
                tw.Close();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfo( "Log failed: {0}", ex );
            }
        }
    }
}