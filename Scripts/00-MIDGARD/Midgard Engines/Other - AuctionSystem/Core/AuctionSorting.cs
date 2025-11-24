namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    /// Defines auction sorting methods
    /// </summary>
    public enum AuctionSorting
    {
        /// <summary>
        /// Sorting by item name
        /// </summary>
        Name,
        /// <summary>
        /// Sorting by date of creation
        /// </summary>
        Date,
        /// <summary>
        /// Sorting by time left for the auction
        /// </summary>
        TimeLeft,
        /// <summary>
        /// Sorting by the number of bids
        /// </summary>
        Bids,
        /// <summary>
        /// Sorting by value of minimum bid
        /// </summary>
        MinimumBid,
        /// <summary>
        /// Sorting by value of the higherst bid
        /// </summary>
        HighestBid
    }
}