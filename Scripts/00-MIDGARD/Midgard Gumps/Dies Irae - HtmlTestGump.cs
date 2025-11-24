/***************************************************************************
 *                               Dies Irae - HtmlTestGump.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Network;
using Server.Gumps;

namespace Midgard.Gumps
{
    public class HtmlTestGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register( "HtmlTestGump", AccessLevel.GameMaster, HtmlTestGump_OnCommand
            );
        }

        [Usage( "[HtmlTestGump <string> <background> <scrollbar: true|false>" )]
        [Description( "Sends an html gump to user. " )]
        private static void HtmlTestGump_OnCommand( CommandEventArgs e )
        {
            try
            {
                e.Mobile.SendGump( new HtmlTestGump( e.GetString( 0 ), e.GetBoolean( 1 ), e.GetBoolean( 2 ) ) );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex );
            }
        }

        public HtmlTestGump( string text, bool background, bool scrollbar )
            : base( 200, 200 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage( 0 );
            AddBackground( 0, 0, 400, 300, 9200 );
            AddHtml( 20, 20, 360, 260, text, background, scrollbar );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
        }
    }
}