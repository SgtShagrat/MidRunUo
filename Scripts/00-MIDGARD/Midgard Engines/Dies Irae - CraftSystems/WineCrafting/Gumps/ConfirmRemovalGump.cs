using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.WineCrafting
{
    public class ConfirmRemovalGump : Gump
    {
        private VinyardGroundAddon m_VGAddon;

        public ConfirmRemovalGump( VinyardGroundAddon vgaddon )
            : base( 50, 50 )
        {
            m_VGAddon = vgaddon;

            AddBackground( 0, 0, 450, 260, 9270 );

            AddAlphaRegion( 12, 12, 426, 22 );
            AddTextEntry( 13, 13, 379, 20, 32, 0, @"Warning!" );

            AddAlphaRegion( 12, 39, 426, 209 );

            AddHtml( 15, 50, 420, 185, "<BODY>" +
                                      "<BASEFONT COLOR=YELLOW>You are about to remove your vinyard ground addon!<BR><BR>" +
                                      "<BASEFONT COLOR=YELLOW>Before removing, be sure to use your grapevine placement tool " +
                                      "<BASEFONT COLOR=YELLOW>to delete any grapevines that you have placed.<BR><BR>" +
                                      "<BASEFONT COLOR=YELLOW>Upon removal of this addon, a replacement vinyard ground addon deed " +
                                      "<BASEFONT COLOR=YELLOW>will be placed in your backpack.<BR><BR>" +
                                      "<BASEFONT COLOR=YELLOW>Are you sure you want to remove this addon?<BR><BR>" +
                                      "</BODY>", false, false );

            AddButton( 13, 220, 0xFA5, 0xFA6, 1, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 47, 222, 150, 20, 1052072, 0x7FFF, false, false ); // Continue

            AddButton( 350, 220, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 385, 222, 100, 20, 1060051, 0x7FFF, false, false ); // CANCEL
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == 0 )
                return;

            Mobile from = sender.Mobile;

            from.AddToBackpack( new VinyardGroundAddonDeed() );
            m_VGAddon.Delete();

            from.SendMessage( "Vinyard ground addon deleted" );
        }
    }
}