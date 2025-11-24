using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using OrbServerSDK;

using Server;
using Server.Accounting;
using Server.Commands;

namespace Midgard.Engines.OrbRemoteServer
{
    public class OrbServer
    {
        private static readonly string m_Version = "2.0.0";
        private static readonly int Port = 5955;
        private static bool m_Enabled;

        private static readonly AccessLevel ReqAccessLevel = AccessLevel.Developer;

        private static Hashtable m_Registry = CollectionsUtil.CreateCaseInsensitiveHashtable( 0 );
        private static Hashtable m_Clients = new Hashtable();

        private static bool m_ServerActive;

        public static void Initialize()
        {
            CommandSystem.Register( "OrbServer", ReqAccessLevel, new CommandEventHandler( OrbServer_OnCommand ) );

            OrbConnection.OnLogin += new OrbConnection.LoginEvent( OnLogin );
            OrbConnection.OnExecuteCommand += new OrbConnection.ExecuteCommandEvent( OnExecuteCommand );
            OrbConnection.OnExecuteRequest += new OrbConnection.ExecuteRequestEvent( OnExecuteRequest );

            if( m_Enabled )
            {
                StartServerThread();
            }
        }

        private static void StartServerThread()
        {
            // Run the OrbServer in a worked thread
            ThreadPool.QueueUserWorkItem( new WaitCallback( StartServer ), null );
        }

        private static void OnExecuteCommand( string alias, OrbClientInfo clientInfo, OrbCommandArgs args )
        {
            OrbClientState client = GetClientState( clientInfo );

            if( client == null )
                return;

            try
            {
                OrbCommand command = GetCommand( alias, client );

                if( command != null )
                {
                    new CommandSyncTimer( client, command, args ).Start();
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( "Exception occurred for OrbServer command {0}\nMessage: {1}", alias, e.Message );
            }
        }

        private static OrbClientState GetClientState( OrbClientInfo clientInfo )
        {
            return (OrbClientState)m_Clients[ clientInfo.ClientID ];
        }

        private static OrbResponse OnExecuteRequest( string alias, OrbClientInfo clientInfo, OrbRequestArgs args )
        {
            OrbClientState client = GetClientState( clientInfo );

            if( client == null )
                return null;

            OrbResponse response = null;

            try
            {
                OrbRequest request = GetRequest( alias, client );

                if( request != null )
                {
                    ManualResetEvent reset = new ManualResetEvent( false );
                    request.ResetEvent = reset;

                    new RequestSyncTimer( client, request, args ).Start();
                    reset.WaitOne();

                    response = request.Response;
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( "Exception occurred for OrbServer request {0}\nMessage: {1}", alias, e.Message );
            }

            return response;
        }

        private static OrbRequest GetRequest( string alias, OrbClientState client )
        {
            OrbRegistryEntry entry = (OrbRegistryEntry)m_Registry[ alias ];
            OrbRequest request = null;

            if( entry != null )
            {
                if( CanConnectionAccess( client, entry ) )
                {
                    try
                    {
                        request = (OrbRequest)Activator.CreateInstance( entry.Type );
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( "OrbServer Exception: " + e.Message );
                    }
                }
            }

            return request;
        }

        private static OrbCommand GetCommand( string alias, OrbClientState client )
        {
            OrbRegistryEntry entry = (OrbRegistryEntry)m_Registry[ alias ];
            OrbCommand command = null;

            if( entry != null )
            {
                if( CanConnectionAccess( client, entry ) )
                {
                    try
                    {
                        command = (OrbCommand)Activator.CreateInstance( entry.Type );
                    }
                    catch( Exception e )
                    {
                        Console.WriteLine( "OrbServer Exception: " + e.Message );
                    }
                }
            }

            return command;
        }

        private static bool CanConnectionAccess( OrbClientState client, OrbRegistryEntry entry )
        {
            bool authorized = false;

            if( entry.RequiresLogin )
            {
                if( client.OnlineMobile != null )
                {
                    authorized = IsAccessAllowed( client.OnlineMobile, entry.RequiredLevel );
                }
            }
            else
            {
                authorized = IsAccessAllowed( client.Account, entry.RequiredLevel );
            }

            return authorized;
        }

        public static void Register( string alias, Type type, AccessLevel requiredLevel, bool requiresLogin )
        {
            if( !type.IsSubclassOf( typeof( OrbRequest ) ) && !type.IsSubclassOf( typeof( OrbCommand ) ) )
            {
                Console.WriteLine(
                    "OrbRemoteServer Error: The type {0} isn't a subclass of the OrbCommand or OrbRequest classes.",
                    type.FullName );
            }
            else if( m_Registry.ContainsKey( alias ) )
            {
                Console.WriteLine( "OrbRemoteServer Error: The type {0} has been assigned a duplicate alias.",
                                  type.FullName );
            }
            else
            {
                m_Registry.Add( alias, new OrbRegistryEntry( type, requiredLevel, requiresLogin ) );
            }
        }

        [Usage( "OrbServer [start][stop]" )]
        [Description( "Issues commands to the OrbServer engine" )]
        public static void OrbServer_OnCommand( CommandEventArgs e )
        {
            if( e == null || e.Arguments == null || e.Mobile == null )
                return;

            if( e.Arguments.Length > 0 )
            {
                if( e.Arguments[ 0 ].ToLower() == "start" && e.Mobile != null && e.Mobile.AccessLevel >= ReqAccessLevel )
                {
                    if( m_ServerActive )
                    {
                        e.Mobile.SendMessage( "OrbServer is currently active" );
                    }
                    else
                    {
                        e.Mobile.SendMessage( "OrbServer starting up" );
                        m_Enabled = true;
                        StartServerThread();
                    }
                }
                else if( e.Arguments[ 0 ].ToLower() == "stop" && e.Mobile != null &&
                         e.Mobile.AccessLevel >= AccessLevel.Administrator )
                {
                    e.Mobile.SendMessage( "OrbServer shutting down" );
                    m_Enabled = false;
                }
            }
            else
            {
                if( m_ServerActive )
                {
                    e.Mobile.SendMessage( "OrbServer is active" );
                }
                else
                {
                    e.Mobile.SendMessage( "OrbServer is not running" );
                }
            }
        }

        public static void StartServer( object o )
        {
            m_ServerActive = true;
            TcpServerChannel channel;

            try
            {
                channel = new TcpServerChannel( "orbserver", Port );
                ChannelServices.RegisterChannel( channel, false );
            }
            catch
            {
                Console.WriteLine( "OrbServer version {1} failed to initialize on port {0}", Port, m_Version );
                return;
            }

            RemotingConfiguration.RegisterWellKnownServiceType( typeof( OrbConnection ), "OrbConnection",
                                                               WellKnownObjectMode.Singleton );

            Utility.PushColor( ConsoleColor.Blue );
            Console.WriteLine( "OrbServer version {1} listening on port {0}", Port, m_Version );
            Utility.PopColor();

            while( m_Enabled )
            {
                Thread.Sleep( 10000 );
            }

            m_ServerActive = false;
            ChannelServices.UnregisterChannel( channel );

            Console.WriteLine( "OrbSeerver on port {0} shut down.", Port );
        }

        private static LoginCodes OnLogin( OrbClientInfo clientInfo, string password )
        {
            LoginCodes code = LoginCodes.Success;

            //Console.WriteLine("OnValidateAccount");
            IAccount account = Accounts.GetAccount( clientInfo.UserName );

            if( account == null || account.CheckPassword( password ) == false )
            {
                code = LoginCodes.InvalidAccount;
            }
            else
            {
                if( !IsAccessAllowed( account, ReqAccessLevel ) )
                {
                    Mobile player = GetOnlineMobile( account );

                    if( player == null || !IsAccessAllowed( player, ReqAccessLevel ) )
                    {
                        // Neither the account or the char the account is logged in with has
                        // the required accesslevel to make this connection.
                        code = LoginCodes.NotAuthorized;
                    }
                }

                Console.WriteLine( "{0} connected to the Orb Script Server", account.Username );
            }

            if( code == LoginCodes.Success )
            {
                if( m_Clients.ContainsKey( clientInfo.ClientID ) )
                    m_Clients.Remove( clientInfo.ClientID );

                m_Clients.Add( clientInfo.ClientID, new OrbClientState( clientInfo, account, DateTime.Now ) );
            }

            return code;
        }

        private static bool IsAccessAllowed( IAccount acct, AccessLevel accessLevel )
        {
            bool accessAllowed = false;

            if( acct != null )
            {
                if( (int)acct.AccessLevel >= (int)accessLevel )
                {
                    accessAllowed = true;
                }
            }

            return accessAllowed;
        }

        private static bool IsAccessAllowed( Mobile mobile, AccessLevel accessLevel )
        {
            bool accessAllowed = false;

            if( mobile != null )
            {
                if( (int)mobile.AccessLevel >= (int)accessLevel )
                {
                    accessAllowed = true;
                }
            }

            return accessAllowed;
        }

        // get logged in char for an account
        internal static Mobile GetOnlineMobile( IAccount acct )
        {
            if( acct == null )
                return null;

            Mobile mobile = null;

            // syncronize the account object to keep this access thread safe
            lock( acct )
            {
                for( int i = 0; i < 5; ++i )
                {
                    Mobile mob = acct[ i ];

                    if( mob == null )
                        continue;

                    if( mob.NetState != null )
                    {
                        mobile = mob;
                        break;
                    }
                }
            }

            return mobile;
        }

        // Stores info regarding the registered OrbCommand or OrbRequest
        private class OrbRegistryEntry
        {
            public Type Type;
            public AccessLevel RequiredLevel;
            public bool RequiresLogin;

            public OrbRegistryEntry( Type type, AccessLevel requiredLevel, bool requiresLogin )
            {
                Type = type;
                RequiredLevel = requiredLevel;
                RequiresLogin = requiresLogin;
            }
        }

        private class RequestSyncTimer : Server.Timer
        {
            private OrbClientState m_Client;
            private OrbRequest m_Request;
            private OrbRequestArgs m_Args;

            public RequestSyncTimer( OrbClientState client, OrbRequest request, OrbRequestArgs args )
                : base( TimeSpan.FromMilliseconds( 20.0 ), TimeSpan.FromMilliseconds( 20.0 ) )
            {
                m_Client = client;
                m_Request = request;
                m_Args = args;
            }

            protected override void OnTick()
            {
                if( m_Request != null )
                    m_Request.OnRequest( m_Client, m_Args );

                Stop();
            }
        }

        private class CommandSyncTimer : Server.Timer
        {
            private OrbClientState m_Client;
            private OrbCommand m_Command;
            private OrbCommandArgs m_Args;

            public CommandSyncTimer( OrbClientState client, OrbCommand command, OrbCommandArgs args )
                : base( TimeSpan.FromMilliseconds( 20.0 ), TimeSpan.FromMilliseconds( 20.0 ) )
            {
                m_Client = client;
                m_Command = command;
                m_Args = args;
            }

            protected override void OnTick()
            {
                if( m_Command != null )
                    m_Command.OnCommand( m_Client, m_Args );

                Stop();
            }
        }
    }

    internal class OrbClientState : OrbClientInfo
    {
        internal OrbClientState( OrbClientInfo clientInfo, IAccount account, DateTime loginTime )
            : base( clientInfo.ClientID, clientInfo.UserName )
        {
            Account = account;
            LoginTime = loginTime;
        }

        internal IAccount Account { get; private set; }

        internal Mobile OnlineMobile
        {
            get { return OrbServer.GetOnlineMobile( Account ); }
        }

        internal DateTime LoginTime { get; private set; }
    }
}