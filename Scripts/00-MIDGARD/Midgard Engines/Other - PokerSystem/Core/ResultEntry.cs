using System;
using System.Collections.Generic;

namespace Server.Poker
{
    public class ResultEntry : IComparable
    {
        public PokerPlayer Player { get; private set; }
        public List<Card> BestCards { get; set; }
        public HandRank Rank { get; set; }

        public ResultEntry( PokerPlayer player )
        {
            Player = player;
        }

        #region IComparable Members

        public int CompareTo( object obj )
        {
            if( obj is ResultEntry )
            {
                ResultEntry entry = (ResultEntry)obj;
                RankResult result = HandRanker.IsBetterThan( this, entry );

                if( result == RankResult.Better )
                    return -1;
                if( result == RankResult.Worse )
                    return 1;
            }

            return 0;
        }

        #endregion
    }
}