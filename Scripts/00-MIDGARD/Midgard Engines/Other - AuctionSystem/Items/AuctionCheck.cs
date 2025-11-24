using System;
using Server;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// Base class for the auction system checks
    /// </summary>
    public abstract class AuctionCheck : Item
    {
        protected Guid m_Auction;
        protected string m_ItemName;
        protected Mobile m_Owner;

        /// <summary>
        /// Gets the message accompanying this check
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Gets the auction that originated this check. This value might be null
        /// </summary>
        public AuctionItem Auction
        {
            get { return AuctionSystem.Find( m_Auction ); }
        }

        /// <summary>
        /// True once the auction item has been delivered
        /// </summary>
        public bool Delivered { get; private set; }

        /// <summary>
        /// Gets the html message used in gumps
        /// </summary>
        public string HtmlDetails
        {
            get { return string.Format( "<basefont color=#FFFFFF>{0}", Message ); }
        }

        /// <summary>
        /// Gets the name of the item returned by this check
        /// </summary>
        public abstract string ItemName { get; }

        public AuctionCheck()
            : base( 5360 )
        {
            LootType = LootType.Blessed;
            Delivered = false;
        }

        public AuctionCheck( Serial serial )
            : base( serial )
        {
        }

        /// <summary>
        /// Gets the item that should be delivered to the players bank
        /// </summary>
        public virtual Item AuctionedItem
        {
            get { return null; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( AuctionedItem is MobileStatuette )
            {
                // Send pet retrieval gump
                from.CloseGump( typeof( CreatureDeliveryGump ) );
                from.SendGump( new CreatureDeliveryGump( this ) );
            }
            else
            {
                // Send item retrieval gump
                from.CloseGump( typeof( AuctionDeliveryGump ) );
                from.SendGump( new AuctionDeliveryGump( this ) );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060658, "Message\t{0}", Message ); // ~1_val~: ~2_val~
        }

        /// <summary>
        /// Delivers the item carried by this check
        /// </summary>
        /// <param name="to">The mobile the check should be delivered to</param>
        /// <returns>True if the item has been delivered to the player's bank</returns>
        public abstract bool Deliver( Mobile to );

        public void DeliveryComplete()
        {
            Delivered = true;
        }

        #region Serialization

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // Version

            writer.Write( m_Auction.ToString() );
            writer.Write( Message );
            writer.Write( m_ItemName );
            writer.Write( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Auction = new Guid( reader.ReadString() );
            Message = reader.ReadString();
            m_ItemName = reader.ReadString();
            m_Owner = reader.ReadMobile();
        }

        #endregion
    }
}