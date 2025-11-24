using System;
using Server;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// Summary description for AuctionItemCheck.
    /// </summary>
    public class AuctionItemCheck : AuctionCheck
    {
        private static int ItemSoldHue = 2119;
        private static int ItemReturnedHue = 335; // old value: 52
        private Item m_Item;

        /// <summary>
        /// Creates a check that will deliver an item for the auction system
        /// </summary>
        /// <param name="auction">The auction generating this check</param>
        /// <param name="result">Specifies the reason for the generation of this check</param>
        public AuctionItemCheck( AuctionItem auction, AuctionResult result )
        {
            Name = auction.Creature ? AuctionSystem.ST[ 131 ] : AuctionSystem.ST[ 132 ];

            m_Auction = auction.ID;
            m_ItemName = auction.ItemName;
            m_Item = auction.SoldAuctionItem;

            /*
            if ( m_Item != null )
            {
                AuctionSystem.ControlStone.RemoveItem( m_Item );
                m_Item.Parent = this; // This will avoid cleanup
            }
            */

            switch( result )
            {
                // Returning the item to the owner
                case AuctionResult.NoBids:
                case AuctionResult.PendingRefused:
                case AuctionResult.SystemStopped:
                case AuctionResult.PendingTimedOut:
                case AuctionResult.ItemDeleted:
                case AuctionResult.StaffRemoved:

                    m_Owner = auction.Owner;
                    Hue = ItemReturnedHue;

                    switch( result )
                    {
                        case AuctionResult.NoBids:
                            Message = string.Format( AuctionSystem.ST[ 133 ], m_ItemName );
                            break;

                        case AuctionResult.PendingRefused:
                            Message = string.Format( AuctionSystem.ST[ 134 ], m_ItemName );
                            break;

                        case AuctionResult.SystemStopped:
                            Message = string.Format( AuctionSystem.ST[ 135 ], m_ItemName );
                            break;

                        case AuctionResult.PendingTimedOut:
                            Message = AuctionSystem.ST[ 127 ];
                            break;

                        case AuctionResult.ItemDeleted:
                            Message = AuctionSystem.ST[ 136 ];
                            break;
                        case AuctionResult.StaffRemoved:
                            Message = AuctionSystem.ST[ 203 ];
                            break;
                    }
                    break;

                case AuctionResult.PendingAccepted:
                case AuctionResult.Succesful:
                case AuctionResult.BuyNow:

                    m_Owner = auction.HighestBid.Mobile;
                    Hue = ItemSoldHue;
                    Message = string.Format( AuctionSystem.ST[ 137 ], m_ItemName, auction.HighestBid.Amount.ToString( "#,0" ) );
                    break;

                default:
                    throw new Exception( string.Format( AuctionSystem.ST[ 138 ], result ) );
            }
        }

        /// <summary>
        /// Gets the name of the item returned by this check
        /// </summary>
        public override string ItemName
        {
            get { return m_ItemName; }
        }

        /// <summary>
        /// Gets the item that should be delivered to the players bank
        /// </summary>
        public override Item AuctionedItem
        {
            get { return m_Item; }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060659, m_ItemName ); // "Item\t{0}"
        }

        /// <summary>
        /// Delivers the item carried by this check
        /// </summary>
        /// <param name="to">The mobile the check should be delivered to</param>
        /// <returns>True if the item has been delivered to the player's bank</returns>
        public override bool Deliver( Mobile to )
        {
            if( Delivered )
                return true;

            Item item = AuctionedItem;

            if( null == item )
            {
                to.SendMessage( Config.MessageHue, AuctionSystem.ST[ 116 ] );
                return false;
            }
            else // if ( to.BankBox.TryDropItem( to, item, false ))
            {
                item.Internalize();
                to.BankBox.AddItem( item );
                item.UpdateTotals();
                DeliveryComplete();
                Delete();
                to.SendMessage( Config.MessageHue, AuctionSystem.ST[ 117 ] );
                return true;
            }
        }

        public override void OnDelete()
        {
            if( Delivered )
                m_Item = null;
            else
                ForceDelete();

            base.OnDelete();
        }

        public void ForceDelete()
        {
            if( m_Item != null )
            {
                if( m_Item is MobileStatuette )
                {
                    ( m_Item as MobileStatuette ).ForceDelete();
                }
                else
                {
                    m_Item.Delete();
                }
            }
        }

        #region Serialization

        public AuctionItemCheck( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // Version

            writer.Write( m_Item );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Item = reader.ReadItem();
        }

        #endregion
    }
}