using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Engines.WineCrafting
{
    public class VinyardGroundPlacedGump : Gump
    {
        private BaseAddonDeed m_Deed;

        public VinyardGroundPlacedGump( BaseAddonDeed deed )
            : base( 30, 30 )
        {
            m_Deed = deed;

            AddPage( 0 );

            AddBackground( 0, 0, 450, 250, 9250 );

            AddAlphaRegion( 12, 12, 426, 22 );
            AddHtml( 13, 13, 379, 20, "<BASEFONT COLOR=WHITE>Vinyard Ground Addon Placement Successful</BASEFONT>", false, false );

            AddAlphaRegion( 12, 39, 426, 199 );

            AddHtml( 15, 50, 420, 185, "<BODY>" +
                                       "<BASEFONT COLOR=YELLOW>Your vinyard ground addon has been successfully placed!<BR>" +
                                       "<BASEFONT COLOR=YELLOW>You may now begin placing grapevines in your vinyard using a " +
                                       "<BASEFONT COLOR=YELLOW>grapevine placement tool.<BR><BR>" +
                                       "<BASEFONT COLOR=YELLOW>You may delete this vinyard ground addon at any time.  " +
                                       "<BASEFONT COLOR=YELLOW>To do so... <BR>" +
                                       "<BASEFONT COLOR=YELLOW>   1. Delete all grapevines first.<BR>" +
                                       "<BASEFONT COLOR=YELLOW>   2. Double click the vinyard addon and accept prompt to delete.<BR>" +
                                       "<BASEFONT COLOR=YELLOW>   *Note* You must be within 3 tiles of western corner to delete addon.<BR><BR>" +
                                       "</BODY>", false, false );

            AddButton( 190, 210, 0xF7, 0xF8, 0, GumpButtonType.Reply, 0 );

        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            Mobile from = sender.Mobile;

            switch( info.ButtonID )
            {
                case 0: //Case uses the ActionIDs defined above. Case 0 defines the actions for the button with the action id 0 
                    {
                        //Cancel 
                        from.SendMessage( "Enjoy your new vinyard." );
                        break;
                    }

            }
        }
    }
}