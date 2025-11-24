using System;
using System.Text;

using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Engines.Help
{
	public class PagePromptGump : Gump
	{
		private Mobile m_From;
		private PageType m_Type;

		public PagePromptGump( Mobile from, PageType type ) : base( 0, 0 )
		{
			m_From = from;
			m_Type = type;

			from.CloseGump( typeof( PagePromptGump ) );

			AddBackground( 50, 50, 540, 350, 2600 );

			AddPage( 0 );

			AddHtmlLocalized( 264, 80, 200, 24, 1062524, false, false ); // Enter Description
			AddHtmlLocalized( 120, 108, 420, 48, 1062638, false, false ); // Please enter a brief description (up to 200 characters) of your problem:

			AddBackground( 100, 148, 440, 200, 3500 );
			AddTextEntry( 120, 168, 400, 200, 1153, 0, "" );

			AddButton( 175, 355, 2074, 2075, 1, GumpButtonType.Reply, 0 ); // Okay
			AddButton( 405, 355, 2073, 2072, 0, GumpButtonType.Reply, 0 ); // Cancel
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			if ( info.ButtonID == 0 )
			{
				m_From.SendLocalizedMessage( 501235, "", 0x35 ); // Help request aborted.
			}
			else
			{
				TextRelay entry = info.GetTextEntry( 0 );
				string text = ( entry == null ? "" : entry.Text.Trim() );

				if ( text.Length == 0 )
				{
					m_From.SendMessage( 0x35, "You must enter a description." );
					m_From.SendGump( new PagePromptGump( m_From, m_Type ) );
				}
				else
				{
					#region modifica by Dies Irae per il log dei pages
					try
					{
						System.IO.TextWriter tw = System.IO.File.AppendText("Logs/Midgard2PagesLog.txt");
						tw.WriteLine( "Page messo in coda dal pg {0} (account {1}), in data {2} alle ore {3}. Contenuto: {4}",
						              m_From.Name, m_From.Account.Username, DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(), text );
						tw.Close();
					}
					catch( Exception ex )
					{
						Console.Write("Page Log Failed due to Exception: {0}", ex);
					}					
					#endregion
					
					m_From.SendLocalizedMessage( 501234, "", 0x35 ); /* The next available Counselor/Game Master will respond as soon as possible.
																	  * Please check your Journal for messages every few minutes.
																	  */

					// PageQueue.Enqueue( new PageEntry( m_From, text, m_Type ) );
                    PageQueue.Enqueue( new PageEntry( m_From, UnicodeToASCII( text ), m_Type ) );
				}
			}
		}

        public string UnicodeToASCII( string unicodeString )
        {
            try
            {
                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;

                // Convert the string into a byte[].
                byte[] unicodeBytes = unicode.GetBytes( unicodeString );

                // Perform the conversion from one encoding to the other.
                byte[] asciiBytes = Encoding.Convert( unicode, ascii, unicodeBytes );

                // Convert the new byte[] into a char[] and then into a string.
                // This is a slightly different approach to converting to illustrate
                // the use of GetCharCount/GetChars.
                char[] asciiChars = new char[ ascii.GetCharCount( asciiBytes, 0, asciiBytes.Length ) ];

                ascii.GetChars( asciiBytes, 0, asciiBytes.Length, asciiChars, 0 );

                return new string( asciiChars );
            }
            catch
            {
                return "[Unparsable Unicode String]";
            }
        }
	}
}
