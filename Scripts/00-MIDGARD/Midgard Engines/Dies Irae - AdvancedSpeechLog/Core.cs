/***************************************************************************
 *                               AdvancedSpeechLog.cs
 *                            --------------------------
 *   begin                : lunedì 13 ottobre 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Accounting;
using Server.Commands;

namespace Midgard.Engines.SpeechLog
{
    public class Core
    {
        private static StreamWriter m_Writer;

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "ToggleSpeechLogStatus", AccessLevel.Administrator, new CommandEventHandler( ToggleSpeechLogStatus_OnCommand ) );
        }

        public static void InitSystem()
        {
            EventSink.Speech += new SpeechEventHandler( EventSink_Speech );

            ScriptCompiler.EnsureDirectory( "Logs" );
            ScriptCompiler.EnsureDirectory( Path.Combine( "Logs", "Speech" ) );

            string directory = "Logs/Speech";

            try
            {
                m_Writer = new StreamWriter( Path.Combine( directory, String.Format( "{0}.log", DateTime.Now.ToLongDateString() ) ), true );

                m_Writer.AutoFlush = true;

                m_Writer.WriteLine( "##############################" );
                m_Writer.WriteLine( "Log started on {0}", DateTime.Now );
                m_Writer.WriteLine();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Couldn't initialize speech system log. Reason:" );
                Console.WriteLine( ex.ToString() );
            }
        }

        [Usage( "ToggleSpeechLogStatus <true | false>" )]
        [Description( "Enables or disables automatic speech logging." )]
        public static void ToggleSpeechLogStatus_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                Config.Enabled = e.GetBoolean( 0 );
                e.Mobile.SendMessage( "Speech log system have been {0} until next restart.", Config.Enabled ? "enabled" : "disabled" );
            }
            else
            {
                e.Mobile.SendMessage( "Format: ToggleSpeechLogStatus <true | false>" );
            }
        }

        private static void WriteLine( Mobile from, string format, params object[] args )
        {
            WriteLine( from, String.Format( format, args ) );
        }

        private static void WriteLine( Mobile from, string text )
        {
            if( !Config.Enabled )
                return;

            try
            {
                m_Writer.WriteLine( "{0}: {1}: {2}", DateTime.Now, from.NetState, text );

                string path = Server.Core.BaseDirectory;

                Account acct = from.Account as Account;

                string name = ( acct == null ? from.Name : acct.Username );

                AppendPath( ref path, "Logs" );
                AppendPath( ref path, "Speech" );
                AppendPath( ref path, from.AccessLevel.ToString() );
                path = Path.Combine( path, String.Format( "{0}.log", name ) );

                using( StreamWriter sw = new StreamWriter( path, true ) )
                    sw.WriteLine( "{0}: {1}: {2}", DateTime.Now, from.NetState, text );
            }
            catch
            {
            }
        }

        private static void AppendPath( ref string path, string toAppend )
        {
            path = Path.Combine( path, toAppend );

            if( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
        }

        private static string Format( Mobile m )
        {
            if( m.Account == null )
                return String.Format( "{0} (no account)", m );
            else
                return String.Format( "{0} ('{1}')", m, m.Account.Username );
        }

        public static void EventSink_Speech( SpeechEventArgs e )
        {
            if( !Config.Enabled )
                return;

            if( !e.Mobile.Player )
                return;

            WriteLine( e.Mobile, "{0} {1}: '{2}'", e.Mobile.AccessLevel, Format( e.Mobile ), e.Speech );
        }
    }
}