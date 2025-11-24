/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

using Server;

namespace Midgard.Engines.LogConsole
{
    public class Core
    {
        internal static string FormatConsoleLogName()
        {
            DateTime now = DateTime.Now;
            string timeStamp = String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}", now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second );
            return String.Format( "ConsoleLog {0}.log", timeStamp );
        }

        public static void StartLogging()
        {
            if( !Server.Core.Service )
                StartConsoleLogging( FormatConsoleLogName() );
        }

        internal static void StartConsoleLogging( string path )
        {
            if( Server.Core.Service || Server.Core.Closing || World.Saving )
                return;

            try
            {
                if( !Directory.Exists( "Logs" ) )
                    Directory.CreateDirectory( "Logs" );

                if( !Directory.Exists( Config.ConsoleLogDirectory ) )
                    Directory.CreateDirectory( Config.ConsoleLogDirectory );

                string pathLog = Path.Combine( Config.ConsoleLogDirectory, path );
				
				/*mod by Magius(CHE)
				 * bisogna modificare il FileLogger o il multitextwriter in modo che siano threadsafe
				 * altrimenti vanno in sharing violation su server veloci o linix 
				 * */
				
                MultiTextWriter writer = Server.Core.MultiConsoleOut;
                if( writer != null )
                    writer.Add( new FileLogger( pathLog ) );

                Config.Pkg.LogInfoLine( "Console logging started ({0})", path );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}