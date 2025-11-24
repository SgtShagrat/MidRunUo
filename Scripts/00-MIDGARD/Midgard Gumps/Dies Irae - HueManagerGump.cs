/***************************************************************************
 *                                  HueManagerGump.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public class HueManagerGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register( "HueManagerGump", AccessLevel.Counselor, new CommandEventHandler( HueManagerGump_OnCommand ) );
            CommandSystem.Register( "HtmlHueManagerGump", AccessLevel.Counselor, new CommandEventHandler( HtmlHueManagerGump_OnCommand ) );
        }

        [Usage( "HueManagerGump" )]
        [Description( "Open a gump with colors available in gumps" )]
        public static void HueManagerGump_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from != null )
            {
                if( e.Length == 0 )
                    from.SendGump( new HueManagerGump( false ) );
                else
                    from.SendMessage( "Command Use: [HueManagerGump" );
            }
        }

        [Usage( "HtmlHueManagerGump" )]
        [Description( "Open a gump with html colors available in gumps" )]
        public static void HtmlHueManagerGump_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from != null )
            {
                if( e.Length <= 1 )
                    from.SendGump( new HueManagerGump( true, e.GetInt32( 0 ) ) );
                else
                    from.SendMessage( "Command Use: [HtmlHueManagerGump" );
            }
        }

        private const int NumColumns = 5;
        private const int NumRows = 30;
        private const int RawsOffset = 20;
        private const int ColumnsOffset = 150;
        private const int ColumnsStringOffset = 100;
        private const string TestString = "MidgardShard";
        private const int FirstLabelX = 15;
        private const int FirstLabelY = 5;

        public HueManagerGump( bool html )
            : this( html, 0 )
        {
        }

        public HueManagerGump( bool html, int bg )
            : base( 0, 0 )
        {
            AddPage( 0 );

            if( bg > 0 )
                AddBackground( 0, 0, 760, 600, bg );
            else
                AddBlackAlpha( 0, 0, 750, 600 );

            int hue = 0;

            if( !html )
            {
                for( int i = 0; i < NumColumns; i++ )
                {
                    for( int j = 0; j < NumRows; j++ )
                    {
                        AddLabel( ( FirstLabelX + ( i * ColumnsOffset ) ), ( FirstLabelY + ( j * RawsOffset ) ), hue, TestString );
                        AddLabel( ( FirstLabelX + ( i * ColumnsOffset ) + ColumnsStringOffset ),
                                 ( FirstLabelY + ( j * RawsOffset ) ),
                                 hue,
                                 hue.ToString() );
                        hue++;
                    }
                }
            }
            else
            {
                int htmlValue;

                for( int i = 0; i < NumColumns; i++ )
                {
                    for( int j = 0; j < NumRows; j++ )
                    {
                        htmlValue = Colors.GetHue( hue );

                        AddOldHtmlHued( FirstLabelX + ( i * ColumnsOffset ),
                                       FirstLabelY + ( j * RawsOffset ),
                                       300,
                                       18,
                                       Colors.GetName( hue ),
                                       htmlValue );
                        hue++;
                    }
                }
            }

        }

        private void AddBlackAlpha( int x, int y, int width, int height )
        {
            AddImageTiled( x, y, width, height, 2624 );
            AddAlphaRegion( x, y, width, height );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
        }
    }
}
