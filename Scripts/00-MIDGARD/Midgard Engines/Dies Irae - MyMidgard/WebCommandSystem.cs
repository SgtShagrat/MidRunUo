/***************************************************************************
 *                               WebCommandSystem.cs
 *                            ------------------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.MyMidgard
{
    public delegate void WebCommandEventHandler( WebCommandsListener listener, OnCommandEventArgs e );

    public class WebCommandSystem
    {
        public static Dictionary<string, WebCommandEntry> Entries { get; private set; }

        static WebCommandSystem()
        {
            Entries = new Dictionary<string, WebCommandEntry>( StringComparer.OrdinalIgnoreCase );
        }

        public static void Register( string command, WebCommandEventHandler handler )
        {
            Entries[ command ] = new WebCommandEntry( command, handler );
        }

        public static WebCommandsListener Listener { get; private set; }

        public static void EventSink_Crashed( CrashedEventArgs e )
        {
            if( Listener != null )
                Listener.Dispose();
        }

        public static void EventSink_Shutdown( ShutdownEventArgs e )
        {
            if( Listener != null )
                Listener.Dispose();
        }

        internal static void InitSystem()
        {
            Listener = new WebCommandsListener( Config.WebCommandsPort );
            Listener.Start();
            Listener.OnCommand += new WebCommandEventHandler( Handle );

            EventSink.Crashed += new CrashedEventHandler( EventSink_Crashed );
            EventSink.Shutdown += new ShutdownEventHandler( EventSink_Shutdown );
        }

        public static void Handle( WebCommandsListener listener, OnCommandEventArgs args )
        {
            string command = args.Cmd;

            if( string.IsNullOrEmpty( command ) )
                return;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( string.Format( "command: {0}", command ) );

            WebCommandEntry entry=null;
            
            Entries.TryGetValue( command, out entry );

            if( entry != null )
            {
                if( Config.Debug )
                    Config.Pkg.LogInfoLine( string.Format( "entry != null: {0}", entry != null ) );

                if( entry.Handler != null )
                {
                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( string.Format( "entry.Handler != null: {0}", entry.Handler != null ) );

                    if( entry.Handler != null )
                        entry.Handler( listener, args );
                }
            }
            else
                throw new ArgumentException( string.Format( "Unexprected command: {0}", args.Cmd ) );
        }
    }
}