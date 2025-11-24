/***************************************************************************
 *                               Commands.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Commands;

namespace Midgard.Engines.LogConsole
{
    public class Commands
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "EnableConsoleLog", AccessLevel.Developer, new CommandEventHandler( LogConsole_OnCommand ) );
            CommandSystem.Register( "CastOnConsole", AccessLevel.Developer, new CommandEventHandler( CastOnConsole_OnCommand ) );
        }

        [Usage( "StartLogConsole [<OptionalPath>]" )]
        [Description( "Start logging console if server is not in service mode" )]
        public static void LogConsole_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( Server.Core.Service )
                from.SendMessage( "Core is already in ServiceMode. This command wont do anything." );

            string customConsoleLogPath = null;
            string finalPath = null;

            if( e.Length == 1 )
                customConsoleLogPath = e.GetString( 0 );

            if( e.Length < 2 )
            {
                try
                {
                    finalPath = String.IsNullOrEmpty( customConsoleLogPath ) ? Core.FormatConsoleLogName() : customConsoleLogPath;
                    Core.StartConsoleLogging( finalPath );
                    from.SendMessage( "Console logging started successfully to file {0}", finalPath );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
            else
            {
                from.SendMessage( "Command use: StartLogConsole [<OptionalPath>]" );
            }
        }

        [Usage( "CastOnConsole <message>" )]
        [Description( "Broadcast a message on console" )]
        public static void CastOnConsole_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            try
            {
                string messageToBCast = e.ArgString;
                if( !String.IsNullOrEmpty( messageToBCast ) )
                {
                    Utility.PushColor( Config.CastOnConsoleColor );
                    Console.WriteLine( "[{0}] - {1}", from.Name, messageToBCast );
                    Utility.PopColor();
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}