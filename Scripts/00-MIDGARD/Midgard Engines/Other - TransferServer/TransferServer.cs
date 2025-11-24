using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

using Server.Commands;

namespace Server.Engines.XmlSpawner2.Xts
{
    public class TransferAccess : Attribute
    {
        public TransferAccess( AccessLevel level )
        {
            Level = level;
        }

        public AccessLevel Level { get; set; }
    }

    public class TransferServer
    {
        protected static List<QueuedMessage> ServerRequests = new List<QueuedMessage>();
        private static ServerTimer m_ServerTimer;
        private static readonly List<AuthEntry> AuthList = new List<AuthEntry>();
        private static readonly TimeSpan AuthenticationLifetime = TimeSpan.FromMinutes( 30 );

        public static void StartServerThread()
        {
            ThreadPool.QueueUserWorkItem( new WaitCallback( StartServer ) );
            StartServerTimer( TimeSpan.FromSeconds( 1 ) );
        }

        private static void AddRequest( TransferMessage msg )
        {
            ServerRequests.Add( new QueuedMessage( msg ) );
        }

        private static void RemoveRequest( TransferMessage msg )
        {
            // search the queue to see if the message is still being processed
            foreach( QueuedMessage q in ServerRequests )
            {
                if( q.MessageIn == msg )
                {
                    q.Remove = true;
                    break;
                }
            }
        }

        private static bool ServerRequestProcessed( TransferMessage msg )
        {
            // search the queue to see if the message is still being processed
            foreach( QueuedMessage q in ServerRequests )
            {
                if( q.MessageIn == msg && !q.Completed )
                    return false;
            }

            // return true if the message is no longer on the queue or it there and has been completed
            return true;
        }

        private static TransferMessage ServerRequestResult( TransferMessage msg )
        {
            // find the queue entry for the message and return the result
            foreach( QueuedMessage q in ServerRequests )
            {
                if( q.MessageIn == msg )
                    return q.MessageOut;
            }

            return null;
        }

        private static void DefragRequests()
        {
            var removelist = new List<QueuedMessage>();

            foreach( QueuedMessage q in ServerRequests )
            {
                if( q.Remove )
                    removelist.Add( q );
            }

            foreach( QueuedMessage q in removelist )
                ServerRequests.Remove( q );
        }

        // set up the timer service for messages that need to be processed in the main RunUO thread for safety reasons
        public static void StartServerTimer( TimeSpan delay )
        {
            if( m_ServerTimer != null )
                m_ServerTimer.Stop();

            m_ServerTimer = new ServerTimer( delay );

            m_ServerTimer.Start();
        }

        [Usage( "XTS [auth id][start][stop][list]" )]
        [Description( "Issues commands to the XmlSpawner2 Transfer Server" )]
        public static void XTS_OnCommand( CommandEventArgs e )
        {
            if( e == null || e.Arguments == null || e.Mobile == null )
                return;

            if( e.Arguments.Length > 0 )
            {
                // syntax is "XTS auth id"
                if( e.Arguments[ 0 ].ToLower() == "auth" && e.Arguments.Length > 1 )
                {
                    try
                    {
                        // add the authentication id to the authentication list
                        var id = new Guid( e.Arguments[ 1 ] );
                        AddAuthenticationEntry( id, e.Mobile.AccessLevel, e.Mobile );
                        e.Mobile.SendMessage( "Transfer Server Authentication registered." );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
                else if( e.Arguments[ 0 ].ToLower() == "list" && e.Mobile.AccessLevel >= AccessLevel.Administrator )
                {
                    // list all of the current auth tickets
                    e.Mobile.SendMessage( "Current Authentication Tickets:" );

                    foreach( AuthEntry a in AuthList )
                    {
                        string name = null;
                        if( a.User != null )
                            name = a.User.Name;

                        e.Mobile.SendMessage( "{0} {1} {2}", name, a.Level, a.Timestamp );
                    }
                }
                else if( e.Arguments[ 0 ].ToLower() == "start" && e.Mobile.AccessLevel >= AccessLevel.Administrator )
                {
                    if( Config.ShouldServerRun )
                        e.Mobile.SendMessage( "TransferServer is currently active" );
                    else
                    {
                        e.Mobile.SendMessage( "TransferServer starting up" );
                        Config.ShouldServerRun = true;
                        StartServerThread();
                    }
                }
                else if( e.Arguments[ 0 ].ToLower() == "stop" && e.Mobile.AccessLevel >= AccessLevel.Administrator )
                {
                    e.Mobile.SendMessage( "TransferServer shutting down" );
                    Config.ShouldServerRun = false;
                }
            }
            else
            {
                if( Config.ShouldServerRun )
                    e.Mobile.SendMessage( "TransferServer is active" );
                else
                    e.Mobile.SendMessage( "TransferServer is not running" );
            }
        }

        /// <summary>
        /// Registers the TransferRemote class for remote access on the server system
        /// </summary>
        private static void StartServer( object obj )
        {
            Config.ShouldServerRun = true;
            TcpServerChannel channel;

            try
            {
                channel = new TcpServerChannel( "transferserver", Config.Port );
                ChannelServices.RegisterChannel( channel, false );
            }
            catch
            {
                Config.Pkg.LogErrorLine( "TransferServer version {1} failed to initialize on port {0}", Config.Port, Config.Pkg.Version );
                return;
            }

            RemotingConfiguration.RegisterWellKnownServiceType( typeof( RemoteMessaging ), "RemoteMessaging", WellKnownObjectMode.Singleton );

            Config.Pkg.LogInfoLine( "TransferServer version {1} listening on port {0}", Config.Port, Config.Pkg.Version );

            while( Config.ShouldServerRun )
                Thread.Sleep( 10000 );

            ChannelServices.UnregisterChannel( channel );
            Config.Pkg.LogInfoLine( "TransferServer on port {0} shut down.", Config.Port );
        }

        public static void AddAuthenticationEntry( Guid authid, AccessLevel level, Mobile user )
        {
            AuthList.Add( new AuthEntry( authid, DateTime.Now, level, user ) );
        }

        public static AuthEntry GetAuthEntry( TransferMessage msg )
        {
            // go through the auth list and find the corresponding auth entry for the message
            foreach( AuthEntry a in AuthList )
            {
                // confirm authentication id match and accesslevel
                if( a.AuthenticationID == msg.AuthenticationID )
                    return a;
            }

            return null;
        }

        private static AccessLevel GetAccessLevel( TransferMessage msg )
        {
            // default accesslevel is admin
            AccessLevel level = AccessLevel.Seer;

            MethodInfo minfo = msg.GetType().GetMethod( "ProcessMessage" );
            object[] attr = minfo.GetCustomAttributes( typeof( TransferAccess ), false );
            foreach( object t in attr )
            {
                if( ( (TransferAccess)t ).Level < level )
                    level = ( (TransferAccess)t ).Level;
            }

            Config.Pkg.LogInfoLine( "GetAccessLevel returned: {0}.", level.ToString() );

            return level;
        }

        private static string Authenticate( TransferMessage msg )
        {
            if( msg == null )
                return "Empty message";

            // check to make sure that an authentication entry for this message is on the authentication list
            var removelist = new List<AuthEntry>();

            // default no authentication status
            string errorstatus = "Renew your Session Authentication";

            foreach( AuthEntry a in AuthList )
            {
                // check for entry expiration
                if( a.Timestamp < DateTime.Now - AuthenticationLifetime )
                {
                    removelist.Add( a );
                    continue;
                }

                // confirm authentication id match and accesslevel
                if( a.AuthenticationID == msg.AuthenticationID )
                {
                    // confirm required accesslevel on the msg is below the access level of the auth entry					
                    errorstatus = a.Level < GetAccessLevel( msg ) ? "Insufficient Access Level" : null;
                }
            }

            // clean up the list
            foreach( AuthEntry a in removelist )
                AuthList.Remove( a );

            return errorstatus;
        }

        // process incoming messages
        public static byte[] RemoteMessagingReceiveMessage( string typeName, byte[] data, out string answerType )
        {
            Config.Pkg.LogInfoLine( "Message requested. (typeName {0})", typeName );

            TransferMessage inMsg;
            TransferMessage outMsg;

            Type type = Type.GetType( typeName );

            if( type != null )
            {
                inMsg = TransferMessage.Decompress( data, type );

                if( inMsg != null )
                {
                    // check message authentication
                    string authstatus = Authenticate( inMsg );

                    if( authstatus != null )
                    {
                        outMsg = new ErrorMessage( String.Format( "Message request refused. {0}", authstatus ) );
                        Config.Pkg.LogInfoLine( "Message request refused. {0}", authstatus );
                    }
                    else
                    {
                        // if the message has been tagged for execution in the RunUO server thread
                        // then queue it up and wait for the response
                        if( inMsg.UseMainThread )
                        {
                            AddRequest( inMsg );

                            while( !ServerRequestProcessed( inMsg ) )
                                Thread.Sleep( 100 );

                            outMsg = ServerRequestResult( inMsg );
                            RemoveRequest( inMsg );
                        }
                        else
                        {
                            // otherwise just run it in the current thread
                            try
                            {
                                // process the message
                                outMsg = inMsg.ProcessMessage();
                            }
                            catch( Exception e )
                            {
                                outMsg = new ErrorMessage( String.Format( "Error processing outgoing message. {0}", e.Message ) );
                                Config.Pkg.LogInfoLine( String.Format( "Error processing outgoing message. {0}", e.Message ) );

                            }
                        }
                    }
                }
                else
                    outMsg = new ErrorMessage( "Error decompressing incoming message. No zero arg msg constructor?" );
                Config.Pkg.LogInfoLine( "Error decompressing incoming message. No zero arg msg constructor?" );

            }
            else
                outMsg = null;

            if( outMsg != null )
            {
                answerType = outMsg.GetType().FullName;
                byte[] result = outMsg.Compress();

                return result;
            }
            else
            {
                answerType = null;
                return null;
            }
        }

        #region Nested type: AuthEntry
        public class AuthEntry
        {
            public Guid AuthenticationID;
            public AccessLevel Level;
            public DateTime Timestamp;
            public Mobile User;

            public AuthEntry( Guid id, DateTime time, AccessLevel level, Mobile user )
            {
                AuthenticationID = id;
                Timestamp = time;
                Level = level;
                User = user;
            }
        }
        #endregion

        #region Nested type: QueuedMessage
        public class QueuedMessage
        {
            public bool Completed;
            public TransferMessage MessageIn;
            public TransferMessage MessageOut;
            public bool Remove;

            public QueuedMessage( TransferMessage msg )
            {
                MessageIn = msg;
                Completed = false;
            }
        }
        #endregion

        #region Nested type: ServerTimer
        private class ServerTimer : Timer
        {
            public ServerTimer( TimeSpan delay )
                : base( delay, delay )
            {
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                // check the server requests list and process anything found on it
                // find the queue entry for the message and return the result
                DefragRequests();

                foreach( QueuedMessage q in ServerRequests )
                {
                    if( q.Completed )
                        continue;

                    // otherwise just run it in the current thread
                    try
                    {
                        q.MessageOut = q.MessageIn.ProcessMessage();
                    }
                    catch( Exception e )
                    {
                        q.MessageOut = new ErrorMessage( String.Format( "Error processing outgoing message. {0}", e.Message ) );
                    }

                    q.Completed = true;

                    // just do one per tick to minimize server blocking
                    break;
                }

                if( !Config.ShouldServerRun )
                {
                    // clear any pending requests and stop the service
                    ServerRequests.Clear();
                    Stop();
                }
            }
        }
        #endregion
    }
}