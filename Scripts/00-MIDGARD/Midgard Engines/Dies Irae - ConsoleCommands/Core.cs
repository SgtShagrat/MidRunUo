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
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Server;
using Server.Commands;

namespace Midgard.Engines.ConsoleCommands
{
    public class Core
    {
        public static bool Paging;

        internal static void RegisterSinks()
        {
            EventSink.ServerStarted += new ServerStartedEventHandler( OnStarted );

            if( Config.HearConsole )
                EventSink.Speech += new SpeechEventHandler( OnSpeech );
        }

        public static void OnStarted()
        {
            ThreadPool.QueueUserWorkItem( new WaitCallback( ConsoleListen ) );
        }

        private static void OnSpeech( SpeechEventArgs args )
        {
            if( !Config.HearConsole )
                return;

            Mobile m = args.Mobile;

            if( m == null || m.Deleted )
                return;

            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append( "CC hear console - " );

                sb.Append( !String.IsNullOrEmpty( m.Name ) ? m.Name : m.Serial.ToString() );

                string rName = m.Region.Name;

                if( !String.IsNullOrEmpty( rName ) )
                    sb.AppendFormat( " in {0} said: ", rName );
                else
                    sb.Append( " said: " );

                sb.Append( args.Speech );
            }
            catch( Exception e )
            {
                Console.WriteLine( "Warning: Hear console from Console Commands failed: {0}", e );
            }
        }

        public static void ConsoleListen( Object stateInfo )
        {
            ProcessCommand( Console.ReadLine() );
            ThreadPool.QueueUserWorkItem( new WaitCallback( ConsoleListen ) );
        }

        private static List<string> m_CommandsList = new List<string>();

        internal static void InitializeCommandsList()
        {
            m_CommandsList.Add( "Save" );
            m_CommandsList.Add( "Restart" );
            m_CommandsList.Add( "S" );
            m_CommandsList.Add( "BC" );
        }

        private static Mobile GetDummyMobile()
        {
            if( CCPersistance.Instance != null )
                return CCPersistance.Instance;
            else
            {
                Config.Pkg.LogWarningLine( "Warning: ConsoleCommandsPersistance was null" );
                return new CCPersistance();
            }
        }

        private static bool IsSupported( string commandToParse )
        {
            if( String.IsNullOrEmpty( commandToParse ) )
                return false;

            bool isSupported = false;
            for( int i = 0; i < m_CommandsList.Count && !isSupported; i++ )
                isSupported = Insensitive.StartsWith( commandToParse, m_CommandsList[ i ] );

            return isSupported;
        }

        private static void ProcessCommand( string commandToParse )
        {
            try
            {
                if( !String.IsNullOrEmpty( commandToParse ) )
                {
                    Mobile dummy = GetDummyMobile();

                    if( IsSupported( commandToParse ) )
                    {
                        string commandString = HandleString( commandToParse );

                        if( !String.IsNullOrEmpty( commandString ) )
                        {
                            bool successed = CommandSystem.Handle( dummy, commandString );
                            if( !successed )
                                Config.Pkg.LogWarningLine( "Warning: unable to handle that command." );
                        }
                        else
                            Config.Pkg.LogWarningLine( "Warning: null string returned from HandleString." );
                    }
                    else
                        Config.Pkg.LogWarningLine( "Warning: that command is not supported." );
                }
                else
                    Config.Pkg.LogWarningLine( "Warning: null command to process." );
            }
            catch( Exception e )
            {
                Config.Pkg.LogWarningLine( "Warning: unhandled exception in ConsoleCommands: {0}", e );
            }
        }

        private static string HandleString( string commandToParse )
        {
            if( String.IsNullOrEmpty( commandToParse ) )
                return String.Empty;

            if( commandToParse.StartsWith( CommandSystem.Prefix ) )
            {
                Config.Pkg.LogInfoLine( "Console commands don't need command prefix." );
                return String.Empty;
            }

            int indexOf = commandToParse.IndexOf( ' ' );

            string command;
            string[] args;

            if( indexOf >= 0 )
            {
                command = commandToParse.Substring( 0, indexOf );
                args = CommandSystem.Split( commandToParse.Substring( indexOf + 1 ) );
            }
            else
            {
                command = commandToParse.Trim();
                args = new string[ 0 ];
            }

            StringBuilder sb = new StringBuilder();

            sb.Append( CommandSystem.Prefix );
            sb.Append( command ); // append the main command

            for( int i = 0; i < args.Length; i++ )
                sb.AppendFormat( " {0}", args[ i ] );

            return sb.ToString();
        }
    }
}