using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Midgard.Engines.Forger
{
    public enum ForgerPacketTypes : byte
    {
        /// <summary>
        /// Uninitialized packet
        /// </summary>
        None,
        /// <summary>
        /// Initial Packet (Exception packed, 1 byte)
        /// </summary>
        CSInitial,
        /// <summary>
        /// string:encrypteduser, string:encryptedpass, string:encryptedmacaddr
        /// </summary>
        CSCredential,
        /// <summary>
        /// Server respond: Authentication succesfully.
        /// string:encryptionkey
        /// </summary>
        SCAuthenticated,
        /// <summary>
        /// Server send to client an alive packet after 1 minute of inactivity.
        ///  Ping will be never crypted
        /// </summary>
        SCPing,
        /// <summary>
        /// Ping respond
        ///  Pong will be never crypted
        /// </summary>
        CSPong,
        /// <summary>
        /// string:message, bool:iscritical
        /// </summary>
        Error,
        /// <summary>
        /// string[]: names of the process to query.
        ///  Note: this command must expect a response
        /// </summary>
        SCQueryUnauthorizedProcessRunning,
        /// <summary>
        /// string[]: names of the process founded.
        /// </summary>
        CSProcessRunningResponse,
    }

    public class RemoteForgerException : Exception
    {
        public readonly bool IsCritical;

        public RemoteForgerException( string message, bool critical )
            : base( message )
        {
            IsCritical = critical;
        }
    }

    public class SocketClosedException : Exception
    {
        public SocketClosedException( string message )
            : base( message ) { }
        public SocketClosedException( Exception inner )
            : base( null, inner ) { }
        public SocketClosedException( string message, Exception inner )
            : base( message, inner ) { }
    }

    public enum ForgerPacketVariableTypes : byte
    {
        String = 0x0,
        Int32,
        Stream,
        Bool
    }

    public class DoBufferCryptographyEventArgs : EventArgs
    {
        private bool m_toencrypt = false;
        public byte[] Source { get; private set; }
        public byte[] Destination;
        public bool SourceMustBeEncrypted { get { return m_toencrypt; } }
        public bool SourceMustBeDecrypted { get { return !m_toencrypt; } }

        public DoBufferCryptographyEventArgs( byte[] bufferSource, bool needEncrypt )
        {
            Source = bufferSource;
            m_toencrypt = needEncrypt;
        }
    }


    public delegate void DoBufferCryptographyEvent( object sender, DoBufferCryptographyEventArgs e );

    public class ForgerPacket
    {
        public event DoBufferCryptographyEvent DoBufferEncrypt, DoBufferDecrypt;
        private const byte ProtocolIdentifier = 0xAE;

        public ForgerPacketTypes Type { get; private set; }
        public List<object> Arguments { get; private set; }

        private ForgerPacket()
            : this( ForgerPacketTypes.None )
        {
            m_IsInitialized = false;
        }

        public ForgerPacket( ForgerPacketTypes type )
        {
            Type = type;
            m_IsInitialized = true;
            Arguments = new List<object>();
        }


        public static ForgerPacket Receive( Socket socket, string encryptionkey, DoBufferCryptographyEvent onEncrypt, DoBufferCryptographyEvent onDecrypt, ref byte[] prebuffer, TimeSpan timeout, TimeSpan sendPingAfter )
        {
            var minTimeout = timeout;
            if( sendPingAfter != TimeSpan.Zero && minTimeout > sendPingAfter )
                minTimeout = sendPingAfter;

            var ret = Internal_BufferedSecureReceive( socket, ref prebuffer, 3, minTimeout );
            while( ret == null && sendPingAfter != TimeSpan.Zero && minTimeout == sendPingAfter )
            {
                //time to send ping...
                SendPing( socket );
                timeout = timeout.Subtract( sendPingAfter );
                minTimeout = timeout;
                if( sendPingAfter != TimeSpan.Zero && minTimeout > sendPingAfter )
                    minTimeout = sendPingAfter;
                ret = Internal_BufferedSecureReceive( socket, ref prebuffer, 3, minTimeout );
            }
            if( ret == null )
                throw new SocketClosedException( "Socket timeout!" );
            var pack = new ForgerPacket();
            pack.DoBufferDecrypt = onDecrypt;
            pack.DoBufferEncrypt = onEncrypt;
            var toreceive = pack.InitializeBuffer( ret );
            minTimeout = timeout;
            if( sendPingAfter != TimeSpan.Zero && minTimeout > sendPingAfter )
                minTimeout = sendPingAfter;
            ret = Internal_BufferedSecureReceive( socket, ref prebuffer, toreceive, minTimeout );
            while( ret == null && sendPingAfter != TimeSpan.Zero && minTimeout == sendPingAfter )
            {
                //time to send ping...
                SendPing( socket );
                timeout = timeout.Subtract( sendPingAfter );
                minTimeout = timeout;
                if( sendPingAfter != TimeSpan.Zero && minTimeout > sendPingAfter )
                    minTimeout = sendPingAfter;
                ret = Internal_BufferedSecureReceive( socket, ref prebuffer, toreceive, minTimeout );
            }
            if( ret == null )
                throw new SocketClosedException( "Socket timeout!" );
            pack.FinalizePacket( ret );
            if( pack.IsFull )
            {
                if( pack.Type == ForgerPacketTypes.SCPing )
                {
                    var npack = new ForgerPacket( ForgerPacketTypes.CSPong );
                    npack.Send( socket );
                }
            }
            return pack;
        }

        private void FinalizePacket( byte[] ret )
        {
            var finalBuffer = ret;
            if( DoBufferDecrypt != null && m_IsEncrypted )
            {
                var args = new DoBufferCryptographyEventArgs( ret, false );
                DoBufferDecrypt( this, args );
                finalBuffer = args.Destination;
            }
            //buffer is now decrypted.

            using( var reader = new BinaryReader( new MemoryStream( finalBuffer ), Encoding.UTF8 ) )
            {
                Type = (ForgerPacketTypes)reader.ReadByte();
                int argslen = reader.ReadByte();

                for( var h = 0; h < argslen; h++ )
                {
                    var type = (ForgerPacketVariableTypes)reader.ReadByte();
                    switch( type )
                    {
                        case ForgerPacketVariableTypes.Int32:
                            Arguments.Add( reader.ReadInt32() );
                            break;
                        case ForgerPacketVariableTypes.String:
                            Arguments.Add( reader.ReadString() );
                            break;
                        case ForgerPacketVariableTypes.Bool:
                            Arguments.Add( reader.ReadBoolean() );
                            break;
                        case ForgerPacketVariableTypes.Stream:
                            var size = reader.ReadUInt16();
                            Arguments.Add( reader.ReadBytes( size ) );
                            break;
                    }
                }

                if( Type == ForgerPacketTypes.Error )
                    throw new RemoteForgerException( (string)Arguments[ 0 ], (bool)Arguments[ 1 ] );
            }

            IsFull = true;
        }

        private bool m_IsEncrypted = false;

        private int InitializeBuffer( byte[] data )
        {
            if( m_IsInitialized || IsFull )
                throw new Exception( "Packet is already initialized!" );

            if( data.Length != 3 )
                throw new Exception( "Packet initialization must be performed only with 3 length data buffer." );

            Arguments.Clear();
            m_IsEncrypted = BitConverter.ToBoolean( data, 0 );
            var bufferlen = BitConverter.ToUInt16( data, 1 );

            m_IsInitialized = true;
            IsFull = false;

            return bufferlen;
        }

        private static byte[] Internal_BufferedSecureReceive( Socket socket, ref byte[] prebuffer, int len, TimeSpan minTimeout )
        {
            if( prebuffer.Length > 0 )
            {
                var diff = prebuffer.Length - len;
                if( diff == 0 )
                {
                    prebuffer = new byte[ 0 ];
                    var arltbuff = prebuffer;
                    return arltbuff;
                }
                else if( diff > 0 )
                {
                    var arltbuff = new byte[ len ];
                    Array.Copy( prebuffer, arltbuff, len );
                    var swap = new byte[ diff ];
                    Array.Copy( prebuffer, len, swap, 0, diff );
                    prebuffer = swap;
                    return arltbuff;
                }
                else
                {
                    len += diff;
                    var mret = SecureReceive( socket, len, minTimeout );
                    var tot = new byte[ len - diff ];
                    Array.Copy( prebuffer, 0, tot, 0, prebuffer.Length );
                    Array.Copy( mret, 0, tot, prebuffer.Length, mret.Length );
                    prebuffer = new byte[ 0 ];
                    return tot;
                }
            }
            else
                return SecureReceive( socket, len, minTimeout );

        }

        private static void SendPing( Socket socket )
        {
            var pack = new ForgerPacket( ForgerPacketTypes.SCPing );
            pack.Send( socket );
        }

        /// <summary>
        /// Stay blocked until all datas will be received.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="bytes"></param>
        /// <param name="timeout"></param>
        /// <returns>Returns null if timeout</returns>
        public static byte[] SecureReceive( Socket socket, int bytes, TimeSpan timeout )
        {
            try
            {
                var rec = new byte[ bytes ];
                var offset = 0;
                var len = bytes;
                var readen = 0;
                var maxlen = len;
                DateTime startTime = DateTime.Now;
                while( offset < maxlen )
                {
                    if( socket.Available > 0 )
                    {
                        readen = socket.Receive( rec, offset, len, SocketFlags.Partial );
                        offset += readen;
                        len -= offset;
                        startTime = DateTime.Now;
                    }
                    else
                        System.Threading.Thread.Sleep( 10 );
                    if( !socket.Connected )
                        throw new SocketClosedException( "Socket disconnected!" );
                    if( DateTime.Now.Subtract( startTime ) > timeout )
                        return null; //timeout
                }
                return rec;
            }
            catch( SocketException ex )
            {
                throw new SocketClosedException( ex );
            }
        }


        private bool m_IsInitialized = false;

        public bool IsFull { get; private set; }

        public bool IsValidForgerProtocol( byte id )
        {
            return ( id == ProtocolIdentifier );
        }

        public static void SendErrorPacket( Socket socket, string encryptionKey, DoBufferCryptographyEvent onEncrypt, DoBufferCryptographyEvent onDecrypt, string message, bool critical )
        {
            try
            {
                var pack = new ForgerPacket( ForgerPacketTypes.Error );
                pack.DoBufferDecrypt = onDecrypt;
                pack.DoBufferEncrypt = onEncrypt;

                pack.Arguments.Add( message );
                pack.Arguments.Add( critical );
                pack.Send( socket );
            }
            catch
            {

            }
        }

        public void Send( Socket socket )
        {
            try
            {

                if( Type == ForgerPacketTypes.None )
                    throw new Exception( "Unable to send uninitialized packet." );
                if( Type == ForgerPacketTypes.CSInitial )
                {
                    socket.SendBufferSize = 1;
                    socket.Send( new byte[] { ProtocolIdentifier } );
                    return;
                }
                using( var clearMemstream = new MemoryStream() )
                using( var writer = new BinaryWriter( clearMemstream, Encoding.UTF8 ) )
                {
                    writer.Write( (byte)Type );
                    writer.Write( (byte)Arguments.Count );
                    //writer.Write((UInt16)0); //placeholder for size

                    //clearMemstream.Flush();
                    //writer.Flush();
                    //var posoffset = clearMemstream.Position;

                    foreach( var obj in Arguments )
                    {
                        int testInt;
                        if( obj is string )
                        {
                            writer.Write( (byte)ForgerPacketVariableTypes.String );
                            writer.Write( (string)obj );
                        }
                        else if( int.TryParse( "" + obj, out testInt ) )
                        {
                            writer.Write( (byte)ForgerPacketVariableTypes.Int32 );
                            writer.Write( (Int32)testInt );
                        }
                        else if( obj is byte[] )
                        {
                            var stream = (byte[])obj;
                            writer.Write( (byte)ForgerPacketVariableTypes.Stream );
                            writer.Write( (UInt16)stream.Length );
                            writer.Write( stream );
                        }
                        else if( obj is bool )
                        {
                            writer.Write( (byte)ForgerPacketVariableTypes.Bool );
                            writer.Write( (bool)obj );
                        }
                        else
                        {
                            throw new NotSupportedException( "" + obj.GetType() + " is not supported." );
                        }
                    }
                    //writer.Flush();
                    //clearMemstream.Position = 2;
                    //writer.Write((UInt16)(clearMemstream.Length - posoffset));

                    writer.Flush();
                    clearMemstream.Flush();

                    byte[] toSendBuffer = null;
                    bool encrypted = false;

                    if( Type != ForgerPacketTypes.SCPing && Type != ForgerPacketTypes.CSPong && DoBufferEncrypt != null )
                    {
                        var args = new DoBufferCryptographyEventArgs( clearMemstream.ToArray(), true );
                        DoBufferEncrypt( this, args );
                        encrypted = ( args.Destination != null );
                        if( encrypted )
                            toSendBuffer = args.Destination;
                        else
                            toSendBuffer = clearMemstream.ToArray();
                    }
                    else
                        toSendBuffer = clearMemstream.ToArray();


                    /*
                     var ret = Program.EncryptStream(clearMemstream);

                    socket.SendBufferSize = (int)ret.Length;
                    socket.Send(ret);
                    */

                    socket.SendBufferSize = toSendBuffer.Length + 3;
                    socket.Send( BitConverter.GetBytes( encrypted ) );
                    socket.Send( BitConverter.GetBytes( (UInt16)toSendBuffer.Length ) );
                    socket.Send( toSendBuffer );
                }
            }
            catch( SocketException ex )
            {
                throw new SocketClosedException( ex );
            }
        }
    }
}
