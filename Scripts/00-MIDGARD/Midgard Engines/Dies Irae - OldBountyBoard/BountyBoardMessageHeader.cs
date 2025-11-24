using System;

using Server.Network;

namespace Server.Items
{
    public class BountyBoardMessageHeader : Packet
    {
        public BountyBoardMessageHeader( BaseBountyBoard board, BountyBoardMessage msg )
            : base( 0x71 )
        {
            string poster = SafeString( "" );
            string subject = ""; // "SafeString( string.Concat( msg.Subject + ":  " + msg.Poster.Bounty + "gold.                                                      " ) );
            string time = SafeString( msg.GetTimeAsString() );

            EnsureCapacity( 22 + poster.Length + subject.Length + time.Length );

            m_Stream.Write( (byte)0x01 ); // PacketID
            m_Stream.Write( board.Serial ); // BountyBoard board serial
            m_Stream.Write( msg.Serial ); // Message serial

            BountyBoardMessage thread = msg.Thread;

            if( thread == null )
                m_Stream.Write( 0 ); // Thread serial--root
            else
                m_Stream.Write( thread.Serial ); // Thread serial--parent

            WriteString( poster );
            WriteString( subject );
            WriteString( time );
        }

        public void WriteString( string v )
        {
            byte[] buffer = Utility.UTF8.GetBytes( v );
            int len = buffer.Length + 1;

            if( len > 255 )
                len = 255;

            m_Stream.Write( (byte)len );
            m_Stream.Write( buffer, 0, len - 1 );
            m_Stream.Write( (byte)0 );
        }

        public string SafeString( string v )
        {
            return v ?? String.Empty;
        }
    }
}