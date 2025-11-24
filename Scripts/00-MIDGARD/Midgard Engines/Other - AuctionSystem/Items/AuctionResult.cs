namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// Defines various conditions that can apply when an auction is terminated
    /// </summary>
    public enum AuctionResult
    {
        /// <summary>
        /// The auction has been succesful and the item has been sold.
        /// </summary>
        Succesful,
        /// <summary>
        /// The auction ends and no bids have been made. The item will be returned to the owner.
        /// </summary>
        NoBids,
        /// <summary>
        /// The auction has ended, and there have been bids but the reserve hasn't been met.
        /// </summary>
        ReserveNotMet,
        /// <summary>
        /// A user has been outbid in an auction
        /// </summary>
        Outbid,
        /// <summary>
        /// The auction had pending status and both parts agreed to finalize the auction
        /// </summary>
        PendingRefused,
        /// <summary>
        /// The auction had pending status and at least one part decided to cancel the auction
        /// </summary>
        PendingAccepted,
        /// <summary>
        /// The pending period has timed out
        /// </summary>
        PendingTimedOut,
        /// <summary>
        /// The Auction System has been forced to stop and the auction ends unsuccesfully
        /// </summary>
        SystemStopped,
        /// <summary>
        /// The auctioned item has been deleted from the world
        /// </summary>
        ItemDeleted,
        /// <summary>
        /// The auction has been removed from the system by the staff
        /// </summary>
        StaffRemoved,
        /// <summary>
        /// The auction ended because a buyer used the buy now feature
        /// </summary>
        BuyNow
    }
}