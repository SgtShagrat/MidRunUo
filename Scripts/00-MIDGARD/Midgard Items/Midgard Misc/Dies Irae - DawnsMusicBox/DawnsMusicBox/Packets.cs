using Server.Network;

namespace Midgard.Items.MusicBox
{
	public class StopMusic : Packet
	{
        public StopMusic()
            : base( 0x6D, 3 )
        {
            m_Stream.Write( (short)0x1FFF );
        }
    }
}
