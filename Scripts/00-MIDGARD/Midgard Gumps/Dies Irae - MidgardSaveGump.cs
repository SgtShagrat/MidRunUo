/***************************************************************************
 *                               Dies Irae - MidgardSaveGump.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

//#define Gump1
// #define Gump2
#define Gump3

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public class MidgardSaveGump : Gump
    {
        public static void Initialize()
        {
            CommandSystem.Register( "SaveGump", AccessLevel.Developer, new CommandEventHandler( SaveGump_OnCommand ) );
        }

        [Usage( "SaveGump" )]
        [Description( "Open the Midgard Save Gump" )]
        public static void SaveGump_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                e.Mobile.SendGump( new MidgardSaveGump() );
        }

        public MidgardSaveGump()
            : base( 300, 300 )
        {
            Closable = false;
            Disposable = true;
            Dragable = false;
            Resizable = false;

#if Gump1
			AddPage(0);
			AddBackground(5, 5, 415, 100, 9270);
			AddLabel(165, 37, 2062, "Midgard" );
			AddLabel(105, 55, 1165, "The world is saving...   Please wait.");
			AddImage(25, 25, 5608);
			AddItem(360, 50, 6168);
#endif

#if Gump2
            int background = Utility.RandomList( new int[] { 2600, 2620, 3500, 3600, 5100, 5120, 9200, 9250, 9300, 9350, 9400, 9450 } );
            AddPage( 0 );
            AddBackground( 0, 0, 305, 190, background ); // Background
            AddAlphaRegion( 15, 15, 275, 160 ); // Alpha Area
            AddOldHtmlHued( 0, 45, 305, 20, HtmlCenter( "* Midgard Third Crown Shard *" ), Colors.DarkRed );
            AddOldHtmlHued( 0, 120, 305, 20, HtmlCenter( "...world is saving..." ), Colors.Red );
            AddImage( 25, 65, 5608 ); // Image 1 
            AddImage( 220, 65, 5504 ); // Image 2
#endif
            AddImage( 0, 0, 0xb7 );
        }

        #region members
        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Closable = !World.Saving;
        }
        #endregion
    }
}
