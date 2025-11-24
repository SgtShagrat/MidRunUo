using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// Summary description for the auction house
    /// </summary>
    public class MyAuctionGump : Gump
    {
        private AuctionGumpCallback m_Callback;

        public MyAuctionGump( Mobile m, AuctionGumpCallback callback )
            : base( 50, 50 )
        {
            m_Callback = callback;
            m.CloseGump( typeof( MyAuctionGump ) );
            MakeGump();
        }

        private void MakeGump()
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddAlphaRegion( 50, 40, 400, 195 );
            AddImageTiled( 49, 39, 402, 197, 3004 );
            AddImageTiled( 50, 40, 400, 195, 2624 );
            AddImageTiled( 90, 32, 323, 16, 10254 );
            AddImage( 165, 65, 10452 );
            AddImage( 0, 20, 10400 );
            AddImage( 0, 185, 10402 );
            AddImage( 35, 20, 10420 );
            AddImage( 421, 20, 10410 );
            AddImage( 410, 20, 10430 );
            AddImage( 420, 185, 10412 );

            // Welcome to the Auction House
            AddLabel( 160, 45, Config.AuctionGreenHue, AuctionSystem.ST[ 8 ] );

            // View your auctions
            AddLabel( 100, 130, Config.AuctionLabelHue, AuctionSystem.ST[ 11 ] );
            AddButton( 60, 130, 4005, 4006, 1, GumpButtonType.Reply, 0 );

            // View your bids
            AddLabel( 285, 130, Config.AuctionLabelHue, AuctionSystem.ST[ 12 ] );
            AddButton( 245, 130, 4005, 4006, 2, GumpButtonType.Reply, 0 );

            // View pendencies
            AddLabel( 100, 165, Config.AuctionLabelHue, AuctionSystem.ST[ 13 ] );
            AddButton( 60, 165, 4005, 4006, 3, GumpButtonType.Reply, 0 );

            // Exit
            AddLabel( 100, 205, Config.AuctionLabelHue, AuctionSystem.ST[ 14 ] );
            AddButton( 60, 205, 4017, 4018, 0, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( !AuctionSystem.Running )
            {
                sender.Mobile.SendMessage( Config.MessageHue, AuctionSystem.ST[ 15 ] );
                // The auction system has been stopped. Please try again later.
                return;
            }

            int buttonid = info.ButtonID;

            if( buttonid < 0 || buttonid > 5 )
                return;

            switch( buttonid )
            {
                case 0: // Exit

                    if( m_Callback != null )
                    {
                        try
                        {
                            m_Callback.DynamicInvoke( new object[] { sender.Mobile } );
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }
                    break;

                case 1: // View your auctions

                    sender.Mobile.SendGump( new AuctionListing( sender.Mobile, AuctionSystem.GetAuctions( sender.Mobile ), false, false ) );
                    break;

                case 2: // View your bids

                    sender.Mobile.SendGump( new AuctionListing( sender.Mobile, AuctionSystem.GetBids( sender.Mobile ), false, false ) );
                    break;

                case 3: // View your pendencies

                    sender.Mobile.SendGump( new AuctionListing( sender.Mobile, AuctionSystem.GetPendencies( sender.Mobile ), false, false ) );
                    break;
            }
        }
    }
}