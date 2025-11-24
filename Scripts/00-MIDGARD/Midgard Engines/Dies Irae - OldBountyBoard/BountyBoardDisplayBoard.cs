using Server.Network;

namespace Server.Items
{
    public class BountyBoardDisplayBoard : Packet
    {
        public BountyBoardDisplayBoard( BaseBountyBoard board )
            : base( 0x71 )
        {
            string name = board.BoardName;

            if( name == null )
                name = "";

            EnsureCapacity( 38 );

            byte[] buffer = Utility.UTF8.GetBytes( name );

            m_Stream.Write( (byte)0x00 ); // PacketID
            m_Stream.Write( board.Serial ); // BountyBoard board serial

            // BountyBoard board name
            if( buffer.Length >= 29 )
            {
                m_Stream.Write( buffer, 0, 29 );
                m_Stream.Write( (byte)0 );
            }
            else
            {
                m_Stream.Write( buffer, 0, buffer.Length );
                m_Stream.Fill( 30 - buffer.Length );
            }
        }
    }
}