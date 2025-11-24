#if !DONOTCOMPILEDEBUGSCRIPT_

using System;
using System.Net.Sockets;

using Server;
using Server.Network;

namespace Midgard.Engines.RazorRpvRecorder
{
    internal class RecordingNetState : NetState
    {
        public RecordingNetState( Mobile owner, Socket socket )
            : base( socket, Core.MessagePump )
        {
            Mobile = owner;
            if( Recorder == null )
                Config.Pkg.LogErrorLine( "Error in RecordingNetState: Recorder is null." );
        }

        /// <summary>
        ///     The recording mobile which this netstate belongs to
        /// </summary>
        private RecorderMobile Recorder
        {
            get { return (RecorderMobile)Mobile; }
        }

        /// <summary>
        ///     We override the NetState.Send method to implement Razor protocol
        /// </summary>
        /// <param name = "p">the packet we 'would' to send</param>
        public override void Send( Packet p )
        {
            if( BlockAllPackets )
                return;

            if( Recorder == null || !Recorder.Recording )
                return;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Sending Packet: {0}, CompressEnabled: {1}", p, CompressionEnabled );

            if( Recorder.RPVVideo != null )
                Recorder.RPVVideo.ServerPacket( p );
        }

        /// <summary>
        ///     We override the CheckAlive method to force this netstate to be active and not disposed for inactivity
        /// </summary>
        /// <returns></returns>
        public override bool CheckAlive()
        {
            return true;
        }

        /// <summary>
        ///     Format a bufer of bytes to easy log them
        /// </summary>
        /// <param name = "buffer">the buffer we want to log</param>
        /// <param name = "length">the buffer lenght</param>
        /// <returns>the formatted string</returns>
        private string BufferToString( byte[] buffer, int length )
        {
            string ret = "";
            for( int h = 0; h < length; h++ )
                ret += Convert.ToString( buffer[ h ], 16 ).PadLeft( 2, '0' );
            return ret;
        }
    }
}

#endif