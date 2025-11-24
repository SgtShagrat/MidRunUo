using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using Midgard.Engines.Packager;
using Server;
using Server.Accounting;
using Timer = Server.Timer;

namespace Midgard.Engines.Forger
{
    class ForgerClient : IDisposable
    {
        public enum DisposeReasons
        {
            Unknown,
            GeneralException,
            NotAuthorized,
            UolClientMobileDisconnected,
            GeneralInactivityTimeout,
            TimeoutAfterQueryUnauthorizedProcesses,
            ClientUsesUnauthorizedProcesses
        }

        private const int SecondsAfterTimeout = 90;
        private const int SecondsAfterTimeoutBeforeAuthentication = 30;
        private const int SecondsToSendingPing = 60;
		private const int SecondsToSendQueryUnauthorizedProcesses = 60*5; //every 5 minutes
		private const int SecondsToTimeout4QueryUnauthorizedProcesses = 60;

		private Package pkg;

        /*public class OnPackedReceivedHandlerArgs : EventArgs
        {
            public bool CloseClient = false;
            public ForgerPacket Packet { get; private set; }
            public OnPackedReceivedHandlerArgs( ForgerPacket packet )
            {
                Packet = packet;
            }
        }*/

        private CPI_Socket m_Socket;
        private byte[] m_IntialBufferedDatas;

        public delegate void OnClosedHandler( ForgerClient sender );
		public delegate void OnQueryUnauthorizedProcessesHandler (ForgerClient sender);
        //public delegate void OnPackedReceivedHandler( ForgerClient sender, OnPackedReceivedHandlerArgs args );

        //public event OnPackedReceivedHandler OnPackedReceived;
        public event OnClosedHandler OnClosed;
		public event OnQueryUnauthorizedProcessesHandler OnQueryUnauthorizedProcesses;

        private Thread m_RunninThread;

        public IAccount Account { get; private set; }
        public string EncryptionKey { get; private set; }
        public string MacAddress { get; private set; }

        public bool IsAuthenticated { get; private set; }
        public Exception LastException { get; private set; }

        private Package Pkg;

        private DisposeReasons m_DisposeReason = DisposeReasons.Unknown;
        public DisposeReasons DisposeReason
        {
            get
            {
                return m_DisposeReason;
            }
            set
            {
                if( m_DisposeReason == DisposeReasons.Unknown )
                    m_DisposeReason = value;
            }
        }

        public ForgerClient( CPI_Socket socket, byte[] intialbuffereddatas )
        {
            Pkg = Packager.Core.Singleton[ typeof( Core ) ];

            EncryptionKey = "TMFAccou";
            socket.InternalSocket.SetSocketOption( System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.KeepAlive, true );
            pkg = Packager.Core.Singleton[ typeof( Core ) ];
            m_IntialBufferedDatas = intialbuffereddatas;
            m_Socket = socket;
            IsAuthenticated = false;
            m_RunninThread = new Thread( new ThreadStart( RunninThreadFun ) );
        }

        void ForgerPacket_DoBufferDecrypt( object sender, DoBufferCryptographyEventArgs e )
        {
            try
            {
                e.Destination = Decrypt( e.Source, EncryptionKey );
            }
            catch
            {
                Pkg.LogErrorLine( "{0} send invalid encrypted datas.", this );
                Dispose( DisposeReasons.GeneralException );
            }
        }

        void ForgerPacket_DoBufferEncrypt( object sender, DoBufferCryptographyEventArgs e )
        {
            e.Destination = Encrypt( e.Source, EncryptionKey );
        }

        public void Start()
        {
            m_RunninThread.Start();
            //RunninThreadFun();
        }
        private ForgerPacket CreatePacket( ForgerPacketTypes type )
        {
            var pack = new ForgerPacket( type );
            pack.DoBufferEncrypt += new DoBufferCryptographyEvent( ForgerPacket_DoBufferEncrypt );
            pack.DoBufferDecrypt += new DoBufferCryptographyEvent( ForgerPacket_DoBufferDecrypt );
            return pack;
        }

        private void RunninThreadFun()
        {
            try
            {
                while( m_Socket != null && m_Socket.InternalSocket.Connected )
                {
                    var newpack = ForgerPacket.Receive( m_Socket.InternalSocket, EncryptionKey, ForgerPacket_DoBufferEncrypt, ForgerPacket_DoBufferDecrypt, ref m_IntialBufferedDatas,
                                                       TimeSpan.FromSeconds( IsAuthenticated
                                                                                ? SecondsAfterTimeout : SecondsAfterTimeoutBeforeAuthentication ),
                                                       TimeSpan.FromSeconds( SecondsToSendingPing ) );
                    if( newpack.Type == ForgerPacketTypes.CSCredential )
                    {
                        if( IsAuthenticated )
                            throw new Exception( "Already authenticated!" );
                        Authenticate( (string)newpack.Arguments[ 0 ], (string)newpack.Arguments[ 1 ], (string)newpack.Arguments[ 2 ] );
                        if( !IsAuthenticated )
                        {
                            Pkg.LogWarningLine( this + " is not authenticated with credentials: {0} -> {1}\"", newpack.Arguments[ 0 ], newpack.Arguments[ 1 ] );
                            break;
                        }

                        pkg.LogInfoLine( "{0} authenticatd.", this );

                        continue;
                    }

                    if( !AnalizeIncomingPacket( newpack ) )
                        break;
                }
                Dispose( DisposeReasons.GeneralException );
            }
            catch( RemoteForgerException )
            {
                Dispose( DisposeReasons.GeneralException );
            }
            catch( Exception ex )
            {
                LastException = ex;
                Dispose( DisposeReasons.GeneralException );
            }
        }

        private bool AnalizeIncomingPacket (ForgerPacket pack)
        {
        	switch (pack.Type)
            {
				case ForgerPacketTypes.CSProcessRunningResponse:
					if (OnQueryUnauthorizedProcesses != null)
					{
						try
						{
							OnQueryUnauthorizedProcesses (this);
						}
						catch (Exception ex)
						{
							Pkg.LogErrorLine ("{0} error: {1}", this,ex);
						}
					}
                    m_QueryUnauthorizedProcessesReceived = true;
                    m_QueryUnauthorizedProcessesSent = false;
                    if( pack.Arguments.Count > 0 )
                    {
                        if( "ALLISOK:-)PlaceholderString".Equals( pack.Arguments[ 0 ] ) )
                        {
                            //all ok
                            return true;
                        }
                        Pkg.LogWarningLine( "{0} uses unauthorized programs.", this );
                        foreach( var arg in pack.Arguments )
                            Pkg.LogWarningLine( " - {0}", arg );
                        return false;
                    }
                    Pkg.LogWarningLine( "{0} attempt to HACK forger protocol.", this );
                    return false;
                case ForgerPacketTypes.CSPong:
                    return true; //good job client :-)                    
                default:
                    Pkg.LogWarningLine( "Receive an unsupported packet '{0}' from {1}.", pack.Type, this );
                    return false;
            }
        }

        public override string ToString()
        {
            if( !IsAuthenticated )
                return "ForgerClient<Unknown>";
            else
                return "ForgerClient<" + Account.Username + ">";
        }

        #region IDisposable Members
        bool m_Disposed = false;

        public void Dispose (DisposeReasons reason)
        {
        	if (m_Disposed)
        		return;
        	DisposeReason = reason;
            Dispose();
        }

        public void Dispose()
        {
            if( m_Disposed )
                return;
            m_Disposed = true;
            if( m_Socket != null )
            {
                if( m_Socket.InternalSocket.Connected )
                {
                    if( LastException != null && ( ( LastException as SocketClosedException ) == null ) )
                        ForgerPacket.SendErrorPacket( m_Socket.InternalSocket, EncryptionKey, ForgerPacket_DoBufferEncrypt, ForgerPacket_DoBufferDecrypt, "Server Error. " + LastException.GetType().Name + ": " + LastException.Message, true );
                    else
                        ForgerPacket.SendErrorPacket( m_Socket.InternalSocket, EncryptionKey, ForgerPacket_DoBufferEncrypt, ForgerPacket_DoBufferDecrypt, "Close Connection.", false );
                }

                m_Socket.Dispose();
            }
            m_Socket = null;
            if( m_RunninThread != null )
            {
                if( Thread.CurrentThread != m_RunninThread )
                    m_RunninThread.Join();
                m_RunninThread = null;
            }

            IsAuthenticated = false;

            if( OnClosed != null )
                OnClosed( this );

        }
        #endregion


        private void Authenticate( string user, string password, string macaddr )
        {
            MacAddress = macaddr;
            Account = Accounts.GetAccount( user );
            if( Account == null )
                return;
            IsAuthenticated = Account.CheckPassword( password );
            if( !IsAuthenticated )
                return;

            var letters = "1234567890'ì+èpòùloikjuyhgtrfdewsqazxcvbnm,.-".ToCharArray();
            string newEncryptionKey = null;
            for( var h = 0; h < 8; h++ )
                newEncryptionKey += letters[ Utility.Random( letters.Length ) ];
            var mpack = CreatePacket( ForgerPacketTypes.SCAuthenticated );
            mpack.Arguments.Add( newEncryptionKey );
            mpack.Send( m_Socket.InternalSocket );
            EncryptionKey = newEncryptionKey;

            if( Core.Debug )
                Pkg.LogInfoLine( "{0} has now a new Ecryption key: {1}", this, EncryptionKey );
        }

        #region QueryUnauthorizedProcesses

        private bool m_QueryUnauthorizedProcessesSent = false;
        private bool m_QueryUnauthorizedProcessesReceived = false;
        public void QueryUnauthorizedProcesses (params string[] procs)
        {
        	if (m_QueryUnauthorizedProcessesSent)
        		return;
        	if (!IsAuthenticated)
        		return;


            var pack = new ForgerPacket (ForgerPacketTypes.SCQueryUnauthorizedProcessRunning);
        	pack.Arguments.AddRange (procs);

            try
            {
        		m_QueryUnauthorizedProcessesSent = true;
        		m_QueryUnauthorizedProcessesReceived = false;
        		pack.Send (m_Socket.InternalSocket);
        		Timer.DelayCall (TimeSpan.FromSeconds( SecondsToTimeout4QueryUnauthorizedProcesses ), new Server.TimerCallback( QueryUnauthorizedProcesses_Timeout ) );
            }
            catch
            {
                m_QueryUnauthorizedProcessesSent = false;
                m_QueryUnauthorizedProcessesReceived = true;
            }



        }
        private void QueryUnauthorizedProcesses_Timeout ()
        {
        	if (m_Disposed)
        		return;
            if( !m_QueryUnauthorizedProcessesSent )
                return;
            if( m_QueryUnauthorizedProcessesReceived )
                return;
            if( !IsAuthenticated )
                return;

            //No response from client after QueryUnauthorizedProcesses packet. Disconnect it.

            Pkg.LogWarningLine( "{0} do not respond after QueryUnauthorizedProcesses.", this );

            Dispose( DisposeReasons.TimeoutAfterQueryUnauthorizedProcesses );
        }

        #endregion

        #region DES Encryption
        public static byte[] Encrypt( byte[] buffer, string keystr8Char )
        {
            if( buffer.Length > UInt16.MaxValue )
                throw new NotSupportedException( "Buffer length must be lesser then " + UInt16.MaxValue );
            var key = new DESCryptoServiceProvider();
            var chars = keystr8Char.ToCharArray();
            var keyb = new byte[ chars.Length ];
            for( var h = 0; h < keyb.Length; h++ )
                keyb[ h ] = Convert.ToByte( chars[ h ] );
            key.Key = keyb;

            var inverse = new byte[ keyb.Length ];
            Array.Copy( keyb, inverse, keyb.Length );
            Array.Reverse( inverse );

            key.IV = inverse;

            // Create a memory stream.
            using( var ms = new MemoryStream() )
            {
                // Create a CryptoStream using the memory stream and the 
                // CSP DES key.  
                ms.Write( BitConverter.GetBytes( (UInt16)buffer.Length ), 0, 2 );
                using( var encStream = new CryptoStream( ms, key.CreateEncryptor(), CryptoStreamMode.Write ) )
                using( var sw = new BinaryWriter( encStream ) )
                    sw.Write( buffer );

                ms.Flush();
                // Return the encrypted byte array.
                return ms.ToArray();
            }
        }

        public static string Encrypt( string stringToCrypt, string keystr8Char )
        {
            var key = new DESCryptoServiceProvider();
            var chars = keystr8Char.ToCharArray();

            var keyb = new byte[ chars.Length ];
            for( var h = 0; h < keyb.Length; h++ )
                keyb[ h ] = Convert.ToByte( chars[ h ] );
            key.Key = keyb;

            var inverse = new byte[ keyb.Length ];
            Array.Copy( keyb, inverse, keyb.Length );
            Array.Reverse( inverse );

            key.IV = inverse;

            // Create a memory stream.
            using( var ms = new MemoryStream() )
            {
                // Create a CryptoStream using the memory stream and the 
                // CSP DES key.  
                using( var encStream = new CryptoStream( ms, key.CreateEncryptor(), CryptoStreamMode.Write ) )
                {
                    // Create a StreamWriter to write a string
                    // to the stream.
                    using( var sw = new StreamWriter( encStream ) )
                    {

                        // Write the plaintext to the stream.
                        sw.WriteLine( stringToCrypt );

                        // Close the StreamWriter and CryptoStream.
                        sw.Close();
                    }
                }
                ms.Flush();
                // Return the encrypted byte array.
                string ret = "";
                foreach( var by in ms.ToArray() )
                    ret += Convert.ToString( by, 16 ).PadLeft( 2, '0' );
                return ret;
            }
        }

        public static byte[] Decrypt( byte[] bufferToDecrypt, string keystr8Char )
        {
            if( bufferToDecrypt.Length > UInt16.MaxValue )
                throw new NotSupportedException( "Buffer length must be lesser then " + UInt16.MaxValue );

            var key = new DESCryptoServiceProvider();
            var chars = keystr8Char.ToCharArray();

            var keyb = new byte[ chars.Length ];
            for( var h = 0; h < keyb.Length; h++ )
                keyb[ h ] = Convert.ToByte( chars[ h ] );
            key.Key = keyb;
            var inverse = new byte[ keyb.Length ];
            Array.Copy( keyb, inverse, keyb.Length );
            Array.Reverse( inverse );

            key.IV = inverse;

            using( var ms = new MemoryStream( bufferToDecrypt ) )
            {
                var len = BitConverter.ToUInt16( ms.ToArray(), 0 );
                ms.Position = 2; //move out of buffer len
                using( var encStream = new CryptoStream( ms, key.CreateDecryptor(), CryptoStreamMode.Read ) )
                using( var sr = new BinaryReader( encStream ) )
                    // Read the stream as a string.
                    return sr.ReadBytes( len );
            }
        }

        public static string Decrypt( string stringToDecrypt, string keystr8Char )
        {
            var key = new DESCryptoServiceProvider();
            var chars = keystr8Char.ToCharArray();

            var keyb = new byte[ chars.Length ];
            for( var h = 0; h < keyb.Length; h++ )
                keyb[ h ] = Convert.ToByte( chars[ h ] );
            key.Key = keyb;
            var inverse = new byte[ keyb.Length ];
            Array.Copy( keyb, inverse, keyb.Length );
            Array.Reverse( inverse );

            key.IV = inverse;

            var cypherText = new byte[ stringToDecrypt.Length / 2 ];
            for( var h = 0; h < cypherText.Length; h++ )
                cypherText[ h ] = Convert.ToByte( "0x" + stringToDecrypt.Substring( h * 2, 2 ), 16 );

            // Create a memory stream to the passed buffer.
            using( var ms = new MemoryStream( cypherText ) )
            {
                // Create a CryptoStream using the memory stream and the 
                // CSP DES key. 
                using( var encStream = new CryptoStream( ms, key.CreateDecryptor(), CryptoStreamMode.Read ) )
                {

                    // Create a StreamReader for reading the stream.
                    using( var sr = new StreamReader( encStream ) )
                    {

                        // Read the stream as a string.
                        string val = sr.ReadLine();

                        return val;
                    }
                }
            }
        }
        #endregion

        internal void FireOnMobileLinked (Mobile mobile)
        {
        	Timer.DelayCall (TimeSpan.FromSeconds (5), new Server.TimerCallback (QueryClientProcess));
        }
		
        private void QueryClientProcess ()
        {
        	if (!IsAuthenticated)
        		return;
        	try
            {
        		QueryUnauthorizedProcesses (Core.UnauthorizedClientProcesses);
        	}
            finally
            {
        		Timer.DelayCall (TimeSpan.FromSeconds( SecondsToSendQueryUnauthorizedProcesses ), new Server.TimerCallback( QueryClientProcess ) );
            }
        }
    }
}