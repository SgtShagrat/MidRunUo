using System;

using Server.Network;

namespace Server.Items
{
    public class BountyBoardMessageContent : Packet
    {
        public BountyBoardMessageContent( BaseBountyBoard board, BountyBoardMessage msg )
            : base( 0x71 )
        {
            string poster = SafeString( "The Town Guards" );
            string subject = SafeString( msg.Subject );
            string time = SafeString( msg.GetTimeAsString() );

            EnsureCapacity( 22 + poster.Length + subject.Length + time.Length );

            m_Stream.Write( (byte)0x02 ); // PacketID
            m_Stream.Write( board.Serial ); // BountyBoard board serial
            m_Stream.Write( msg.Serial ); // Message serial

            WriteString( poster );
            WriteString( subject );
            WriteString( time );

            m_Stream.Write( (short)msg.PostedBody );
            m_Stream.Write( (short)msg.PostedHue );

            int len = msg.PostedEquip.Length;

            if( len > 255 )
                len = 255;

            m_Stream.Write( (byte)len );

            for( int i = 0; i < len; ++i )
            {
                BountyBoardEquip eq = msg.PostedEquip[ i ];

                m_Stream.Write( (short)eq.ItemID );
                m_Stream.Write( (short)eq.Hue );
            }

            len = msg.Lines.Length;

            if( len > 255 )
                len = 255;

            m_Stream.Write( (byte)len );

            for( int i = 0; i < len; ++i )
                WriteString( msg.Lines[ i ] );
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