/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 04 agosto, 2009
 *   author               :	Magius(CHE)
 *   copyright            : (C) Midgard Shard - Magius(CHE)		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Engines.Packager;
using Server;
using Server.Accounting;
using Server.Gumps;

namespace Midgard.Engines.Forger
{
    public class Core
    {
#if DEBUG
        public static bool Debug = false;
#else
		public static bool Debug = false;
#endif		
        public static object[] Package_Info = {
            "Script Title",             "Forger Protocol Engine",
            "Enabled by Default",       false,
            "Script Version",           new Version(1,0,0,1),
            "Author name",              "Magius(CHE)", 
            "Creation Date",            new DateTime(2009, 08, 04), 
            "Author mail-contact",      "cheghe@tiscali.it", 
            "Author home site",         "http://www.magius.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Magius(CHE)",
            "Provided packages",        new string[]{"Midgard.Engines.Forger"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Network","Patcher","Client","Engine","Forger"}
        };

        public static string[] UnauthorizedClientProcesses = { "gear", "hackuo" };

        /// <summary>
        /// Singleton Pattern
        /// </summary>
        internal static Core Singleton { get; set; }

        static Core()
        {
            Singleton = new Core();
        }

        private Core()
        {
        }

        bool m_Enabled = false;
        bool m_Initialized = false;

        private class ForgerClientMobile
        {
            public ForgerClient Client;
            public Mobile Mobile;
        }

        private readonly List<ForgerClientMobile> m_Connected = new List<ForgerClientMobile>();

        public static void Package_Configure ()
        {
        	Singleton.Pkg = Packager.Core.Singleton[typeof(Core)];

            //hook on socket intercept system...
        	EventSink.ClientPacketIntercepted += new ClientPacketInterceptedEventHandler (Singleton.EventSink_ClientPacketIntercepted);
        	EventSink.Shutdown += delegate { Singleton.Shutdown (); };
        	EventSink.Crashed += delegate { Singleton.Shutdown (); }; 
			
            EventSink.Disconnected += new DisconnectedEventHandler( Singleton.EventSink_Disconnected );
            EventSink.Connected += new ConnectedEventHandler( Singleton.EventSink_Connected );

            if( !InstaKickOnFailure )
                EventSink.Login += new LoginEventHandler( EventSink_Login );
        }

        public Package Pkg { get; private set; }

        public static void Package_Initialize()
        {
            Singleton.m_Enabled = Singleton.Pkg.Enabled;
            Singleton.m_Initialized = true;
        }

        private static void Package_OnEnabledChanged( bool isEnabledBefore )
        {
            var pkg = Packager.Core.Singleton[ typeof( Core ) ];
            Singleton.m_Enabled = pkg.Enabled;
            if( !Singleton.m_Initialized )
                return;
            pkg.WriteEnableStatusToConsole( false );
        }

        /// <summary>
        /// New client connected. If no forger is enabled client will be disconnected.
        /// </summary>
        /// <param name="e">Connections args</param>
        private void EventSink_Connected( ConnectedEventArgs e )
        {
            if( !Singleton.m_Enabled )
                return;

            lock( m_Connected )
            {
                if( e.Mobile.Account == null )
				{
					if (e.Mobile.GetType().Name != "RecorderMobile")
                    	Pkg.LogWarningLine( "Mobile Player \"{0}\" has no account. It will be Authorized anyway.", e.Mobile );
				}
                else
                {
                    foreach( var elem in m_Connected )
                        if( elem.Client.IsAuthenticated &&
                            elem.Client.Account.Username.Equals( e.Mobile.Account.Username, StringComparison.InvariantCultureIgnoreCase ) )
                        {
                            elem.Mobile = e.Mobile;

                            Pkg.LogInfoLine( "\"{0}\" -> {1} [ OK ]", e.Mobile, elem.Client );

                            elem.Client.FireOnMobileLinked( e.Mobile );

                            return;
                        }

                    var requireforger = e.Mobile.AccessLevel < AccessLevel.Administrator && string.IsNullOrEmpty( ( (Account)e.Mobile.Account ).GetTag( "noforger" ) );
#if DEBUG
                    //requireforger = false;
#endif
                    if( !requireforger )
                    {
                        Pkg.LogWarningLine( "Mobile Player \"{0}\" has 'noforger' tag so will be authorized anyway.", e.Mobile );
                    }
                    else
                    {
                        //this client is not authorized...
                        Pkg.LogWarningLine( "Mobile Player \"{0}\" is not authorized and will be disconnected.", 0, e.Mobile );

                        if( InstaKickOnFailure )
                            DisconnectMobile( e.Mobile );
                        else
                            m_Disconnected.Add( e.Mobile );
                    }
                }
            }
        }

        private static List<Mobile> m_Disconnected = new List<Mobile>();

        /// <summary>
        /// True if we want to kick the netstate before loggin in
        /// </summary>
        public static readonly bool InstaKickOnFailure = false;

        /// <summary>
        /// The delay between player login and the gump sending
        /// </summary>
        public static readonly TimeSpan DelayFromLogin = TimeSpan.FromSeconds( 5.0 );

        /// <summary>
        /// How long to show warning message before they are disconnected
        /// </summary>
        public static readonly TimeSpan DisconnectDelay = TimeSpan.FromSeconds( 15.0 );

        /// <summary>
        /// The message in the warning gump
        /// </summary>
        public const string WarningMessage = "The server was unable to recognize the Midgard Forger client active on your machine.<BR>" +
            "You must be running the <A HREF=\"http://www.midgardshard.it/TMF_patching/TheMidgardForgerInstaller.exe\">latest version of the Midgard Forger</A> to play on Midgard.<BR>" +
            "Before installing the Midgard Forger be sure to install the <A HREF=\"http://download.microsoft.com/download/6/0/f/60fc5854-3cb8-4892-b6db-bd4f42510f28/dotnetfx35.exe\">lastest Microsoft .Net 3.5 libraries.</A>";

        private static void EventSink_Login( LoginEventArgs e )
        {
            Mobile m = e.Mobile;
            if( m == null )
                return;

            if( !m_Disconnected.Contains( m ) )
                return;

            if( m.NetState != null && m.NetState.Running )
            {
                // Start the first delay... at the end the warning gump will be displayed
                Timer.DelayCall( DelayFromLogin, OnTimeout, m );
            }
            else
            {
                // Remove our guy from the table if we already kicked them
                if( m_Disconnected.Contains( m ) )
                    m_Disconnected.Remove( m );
            }
        }

        /// <summary>
        /// Invoked after the player has logged in
        /// </summary>
        /// <param name="state"></param>
        private static void OnTimeout( object state )
        {
            Mobile m = state as Mobile;
            if( m == null )
                return;

            if( m.NetState != null && m.NetState.Running )
            {
                m.SendGump( new WarningGump( 1060635, 30720, WarningMessage, 0xFFC000, 420, 250, null, null ) );

                // Start the second delay... at the end the netstate will be disconnected
                Timer.DelayCall( DisconnectDelay, OnDisconnect, m );
            }

            // Remove our guy from the table
            if( m_Disconnected.Contains( m ) )
                m_Disconnected.Remove( m );

            Timer.DelayCall( DisconnectDelay, OnDisconnect, m );
        }

        /// <summary>
        /// Called after the warning gump has been sent
        /// </summary>
        /// <param name="state"></param>
        private static void OnDisconnect( object state )
        {
            if( !( state is Mobile ) )
                return;

            Mobile m = (Mobile)state;

            if( m.NetState != null && m.NetState.Running )
                m.NetState.Dispose();

            if( m_Disconnected.Contains( m ) )
                m_Disconnected.Remove( m );
        }

        /// <summary>
        /// Disconnect player
        /// </summary>
        /// <param name="mobile">Mobile Player</param>
        private static void DisconnectMobile( Mobile mobile )
        {
            if( mobile == null )
                return;

            if( mobile.NetState == null )
                return;

            if( !mobile.NetState.Running )
                return;

            mobile.NetState.Dispose();
        }

        /// <summary>
        /// Client disconnected. Purge forger client.
        /// </summary>
        /// <param name="e"></param>
        private void EventSink_Disconnected( DisconnectedEventArgs e )
        {
            lock( m_Connected )
            {
                ForgerClientMobile founded = null;
                foreach( var elem in m_Connected )
                    if( elem.Mobile == e.Mobile )
                    {
                        founded = elem;
                        break;
                    }

                if( founded != null )
                {
                    var clitxt = founded.Client.ToString();
                    m_Connected.Remove( founded );

                    // this cause ForgerClient.OnClosed
                    founded.Client.Dispose( ForgerClient.DisposeReasons.UolClientMobileDisconnected );
                    Pkg.LogInfoLine( "{0} disconnected. [{1}]", clitxt, m_Connected.Count );
                }
            }
        }

        /// <summary>
        /// Clear resources
        /// </summary>
        /// <param name="e"></param>
        private void Shutdown ()
        {
        	//Rearrange connectedclient into new list, cause fast exit from Cli_OnClosed
        	var rearranged = new List<ForgerClientMobile> ();

			lock (m_Connected)
            {
        		rearranged.AddRange (m_Connected.ToArray ());
        		m_Connected.Clear ();
        	}
			
            foreach( var sk in rearranged )
                sk.Client.Dispose( ForgerClient.DisposeReasons.UolClientMobileDisconnected );
        }

        /// <summary>
        /// Packet Intercepted from socket. Check for identity
        /// </summary>
        /// <param name="e"></param>
        private void EventSink_ClientPacketIntercepted( ClientPacketInterceptedEventArgs e )
        {
            //identify for forger
            if( Singleton.m_Enabled && e.PacketId == 0xAE )
            {
                e.Intercepted = true;

                var cli = new ForgerClient( e.Socket, e.BufferedDatas );
                cli.OnClosed += new ForgerClient.OnClosedHandler( Cli_OnClosed );
                //cli.OnPackedReceived += new ForgerClient.OnPackedReceivedHandler( Cli_OnPackedReceived );

                lock( m_Connected )
                    m_Connected.Add( new ForgerClientMobile { Client = cli } );

                Pkg.LogInfoLine( "{0} intercepted. [{1}]", cli, m_Connected.Count );
                cli.Start();
            }
        }

        /// <summary>
        /// Forger client is closed. Check connected player modible and disconnect it.
        /// </summary>
        /// <param name="sender"></param>
        private void Cli_OnClosed( ForgerClient sender )
        {
            var connected = GetConnected( sender );
            if( connected == null )
                return; //caused by EventSink_Disconnected

            lock( m_Connected )
            {
                var closereason = sender.LastException;
                var mobile = connected.Mobile;

                m_Connected.Remove( connected );

                if( mobile != null )
                    DisconnectMobile( mobile );

                if( closereason != null && ( ( closereason as SocketClosedException ) == null ) )
                {
                    Pkg.LogErrorLine( "{0} closed for Exception. {1}: {2}", sender, closereason.GetType().Name, closereason.Message );
                    Pkg.LogErrorLine( "Stacktrace: {0}", closereason.StackTrace );
                }

                Pkg.LogInfoLine( "{0} removed. [{1}]", sender, m_Connected.Count );
            }
        }

        /// <summary>
        /// Return connected ForgerClientMobile
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private ForgerClientMobile GetConnected( ForgerClient sender )
        {
            lock( m_Connected )
            {
                foreach( var elem in m_Connected )
                    if( elem.Client == sender )
                        return elem;
            }
            return null;
        }
    }
}