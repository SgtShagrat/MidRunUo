/***************************************************************************
 *                               DisplayMailMessageGump.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MailSystem
{
    public class DisplayMailMessageGump : Gump
    {
        private readonly MailMessage m_Message;

        private const int TextColor = 0x3e8;
        private const int TitleColor = 0xef0000; // cyan 0xf70000, black 0x3e8, brown 0xef0000 darkblue 0x7fff
        private const int Size = 4;

        public DisplayMailMessageGump( MailMessage message )
            : base( 50, 50 )
        {
            m_Message = message;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            //AddPage( 0 );
            //AddImage( 13, 168, 2080 );
            //AddImage( 30, 204, 2081 );
            //AddLabel( 48, 200, 593, "Messagge from:" );
            //AddLabel( 140, 200, 488, m_From.Name );
            //AddImage( 30, 274, 2081 );
            //AddImage( 32, 344, 2083 );
            //AddHtml( 53, 225, 218, 81, m_Text, false, true );
            //AddLabel( 120, 315, 488, m_From.Name );

            AddPage( 0 );

            AddAlphaRegion( 40, 41, 225, 70 * Size );

            AddImageTiled( 3, 5, 300, 37, 0x820 );
            AddImageTiled( 19, 41, 263, 70, 0x821 );
            for( int i = 1; i < Size; i++ )
                AddImageTiled( 19, 41 + 70 * i, 263, 70, 0x822 );
            AddImageTiled( 20, 111 + 70 * ( Size - 1 ), 273, 34, 0x823 );

            AddOldHtml( 55, 10, 200, 37, HtmlFormat( m_Message.Title, TitleColor ) );
            // AddHtml( 55, 10, 200, 37, HtmlFormat( m_Message.Title, TitleColor ), false, false );

            AddOldHtml( 40, 50, 225, 70 * Size, HtmlFormat( m_Message.Text, TextColor ) );
            // AddHtml( 40, 41, 225, 70 * Size, HtmlFormat( m_Message.Text, TextColor ), false, false );
        }

        public static string HtmlFormat( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
        }
    }
}