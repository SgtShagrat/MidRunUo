/***************************************************************************
 *                               BountyStatusGump.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Gumps;

namespace Midgard.Engines.BountySystem
{
    public class BountyStatusGump : Gump
    {
        private static int HtmlHue = 0xFFFFFF;

        public List<string> Lines { get; private set; }

        public string Color( string text, int color )
        {
            return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
        }

        public BountyStatusGump( Mobile from, List<string> lines )
            : base( 50, 50 )
        {
            Lines = lines;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage( 0 );
            AddBackground( 0, 0, 350, 150, 9200 );
            AddHtml( 106, 3, 200, 32, Color( "Bounty Status", HtmlHue ), false, false );

            StringBuilder sb = new StringBuilder();

            for( int i = 0; i < Lines.Count; ++i )
            {
                if( i > 0 )
                    sb.Append( "<br>" );

                sb.Append( Lines[ i ] );
            }

            AddHtml( 20, 40, 310, 90, sb.ToString(), true, true );
        }
    }
}