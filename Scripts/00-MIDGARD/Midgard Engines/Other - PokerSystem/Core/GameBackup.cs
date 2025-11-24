using System.Collections.Generic;

namespace Server.Poker
{
    /// <summary>
    /// Provides a protection for players so that if server crashes, they will be refunded money
    /// </summary>
    public class GameBackup 
    {
        /// <summary>
        /// List of all poker games with players
        /// </summary>
        public static List<PokerGame> PokerGames; 
    }
}