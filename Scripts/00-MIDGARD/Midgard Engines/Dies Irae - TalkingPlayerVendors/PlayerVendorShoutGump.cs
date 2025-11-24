using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.TalkingPlayerVendorSystem
{
	public class PlayerVendorShoutGump : Gump
	{
		private Mobile m_From;
		private PlayerVendor m_Vendor;

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if( info.ButtonID == 1 )
			{
				m_From.SendMessage( m_From.Language == "ITA" ? "Inserisci un messaggio che il tuo mercante dovrebbe gridare." : "Enter a message your vendor should shout." );
				m_From.Prompt = new PlayerVendorShoutPrompt( m_From, m_Vendor );
			}
			else if( info.ButtonID > 1 )
			{
				List<string> entries = m_Vendor.ShoutEntries;

				if( entries == null || entries.Count < 1 )
					return;

				int index = info.ButtonID - 2;

				if( index < entries.Count )
				{
					entries.RemoveAt( index );
					m_From.SendMessage( m_From.Language == "ITA" ? "Hai scelto di rimuovere la frase dalla lista delle grida!" : "You have chosen to remove that entry from vendor shout list!" );
					m_From.SendGump( new PlayerVendorShoutGump( m_From, m_Vendor ) );
				}
			}
		}

		public PlayerVendorShoutGump( Mobile from, PlayerVendor vendor )
			: base( 50, 50 )
		{
			m_From = from;
			m_Vendor = vendor;

			from.CloseGump( typeof( PlayerVendorShoutGump ) );

			AddPage( 0 );

			List<string> entries = m_Vendor.ShoutEntries;

			int count = entries != null ? entries.Count : 0;

			AddImageTiled( 0, 0, 300, 38 + ( count == 0 ? 20 : ( count * 85 ) ), 0xA40 );
			AddAlphaRegion( 1, 1, 298, 36 + ( count == 0 ? 20 : ( count * 85 ) ) );

			AddHtml( 8, 8, 300 - 8 - 30, 20, m_From.Language == "ITA" ? "<basefont color=#FFFFFF><center>GRIDA DEL MERCANTE</center></basefont>" : "<basefont color=#FFFFFF><center>VENDOR SHOUTS</center></basefont>", false, false );

			if( entries != null )
			{
				if( entries.Count < Config.MaxNumberOfShouts )
					AddButton( 300 - 8 - 30, 8, 0xFAB, 0xFAD, 1, GumpButtonType.Reply, 0 );
			}

			if( count == 0 )
				AddHtml( 8, 30, 284, 20, "<basefont color=#FFFFFF>The vendor has no messages.</basefont>", false, false );
			else
			{
				if( entries != null )
				{
					for( int i = 0; i < entries.Count; ++i )
					{
						AddHtml( 8, 35 + ( i * 85 ), 254, 80, entries[ i ], true, true );
						AddButton( 300 - 8 - 26, 35 + ( i * 85 ), 0x15E1, 0x15E5, 2 + i, GumpButtonType.Reply, 0 );
					}
				}
			}
		}
	}
}