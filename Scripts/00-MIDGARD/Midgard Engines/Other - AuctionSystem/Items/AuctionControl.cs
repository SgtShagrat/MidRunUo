using System.Collections.Generic;
using Server;
using Server.Network;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// This is the auction control stone. This item should NOT be deleted
    /// </summary>
    public class AuctionControl : Item
    {
        /// <summary>
        /// Flag used to force the deletion of the system
        /// </summary>
        private bool m_Delete;

        /// <summary>
        /// The max number of concurrent auctions for each account
        /// </summary>
        private int m_MaxAuctionsParAccount = 5;

        /// <summary>
        /// Gets or sets the list of current auction entries
        /// </summary>
        public List<AuctionItem> Auctions { get; set; }

        /// <summary>
        /// Gets or sets the pending auction entries
        /// </summary>
        public List<AuctionItem> Pending { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        /// <summary>
        /// Gets or sets the max number of auctions a single account can have
        /// </summary>
        public int MaxAuctionsParAccount
        {
            get { return m_MaxAuctionsParAccount; }
            set { m_MaxAuctionsParAccount = value; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public int MinAuctionDays { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public int MaxAuctionDays { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CostOfAuction
        {
            get { return Config.CostOfAuction; }
        }

        public AuctionControl()
            : base( 4484 )
        {
            MaxAuctionDays = 14;
            MinAuctionDays = 1;
            Name = "Auction System";
            Visible = false;
            Movable = false;
            Auctions = new List<AuctionItem>();
            Pending = new List<AuctionItem>();

            AuctionSystem.ControlStone = this;
        }

        public AuctionControl( Serial serial )
            : base( serial )
        {
            MaxAuctionDays = 14;
            MinAuctionDays = 1;
            Auctions = new List<AuctionItem>();
            Pending = new List<AuctionItem>();
        }

        #region Serialization

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // Version

            // Version 1 : changes in AuctionItem
            // Version 0
            writer.Write( m_MaxAuctionsParAccount );
            writer.Write( MinAuctionDays );
            writer.Write( MaxAuctionDays );

            writer.Write( Auctions.Count );

            foreach( AuctionItem auction in Auctions )
            {
                auction.Serialize( writer );
            }

            writer.Write( Pending.Count );

            foreach( AuctionItem auction in Pending )
            {
                auction.Serialize( writer );
            }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                case 0:
                    m_MaxAuctionsParAccount = reader.ReadInt();
                    MinAuctionDays = reader.ReadInt();
                    MaxAuctionDays = reader.ReadInt();

                    int count = reader.ReadInt();

                    for( int i = 0; i < count; i++ )
                    {
                        Auctions.Add( AuctionItem.Deserialize( reader, version ) );
                    }

                    count = reader.ReadInt();

                    for( int i = 0; i < count; i++ )
                    {
                        Pending.Add( AuctionItem.Deserialize( reader, version ) );
                    }
                    break;
            }

            AuctionSystem.ControlStone = this;
        }

        #endregion

        public override void OnDelete()
        {
            // Don't allow users to delete this item unless it's done through the control gump
            if( !m_Delete )
            {
                AuctionControl newStone = new AuctionControl();
                newStone.Auctions.AddRange( Auctions );
                newStone.MoveToWorld( Location, Map );

                newStone.Items.AddRange( Items );
                Items.Clear();
                foreach( Item item in newStone.Items )
                {
                    item.Parent = newStone;
                }

                newStone.PublicOverheadMessage( MessageType.Regular, 0x3B2, false, AuctionSystem.ST[ 121 ] );
            }

            base.OnDelete();
        }

        /// <summary>
        /// Deletes the item from the world without triggering the auto-recreation
        /// This function also closes all current auctions
        /// </summary>
        public void ForceDelete()
        {
            m_Delete = true;
            Delete();
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( AuctionSystem.Running ? 3005117 : 3005118 ); // [Active] - [Inactive]
        }
    }
}