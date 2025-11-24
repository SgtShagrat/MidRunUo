using System;

namespace Server.Engines.XmlPoints
{
    public class RankEntry : IComparable
    {
        public Mobile Killer;
        public XmlPointsAttach PointsAttachment;
        public int Rank;

        public RankEntry( Mobile m, XmlPointsAttach attachment )
        {
            Killer = m;
            PointsAttachment = attachment;
        }

        #region IComparable Members

        public int CompareTo( object obj )
        {
            var p = (RankEntry)obj;

            if( p.PointsAttachment == null || PointsAttachment == null )
                return 0;

            // break points ties with kills (more kills means higher rank)
            if( p.PointsAttachment.Points - PointsAttachment.Points == 0 )
            {
                // if kills are the same then compare deaths (fewer deaths means higher rank)
                if( p.PointsAttachment.Kills - PointsAttachment.Kills == 0 )
                {
                    // if deaths are the same then use previous ranks
                    if( p.PointsAttachment.Deaths - PointsAttachment.Deaths == 0 )
                    {
                        return p.PointsAttachment.Rank - PointsAttachment.Rank;
                    }

                    return PointsAttachment.Deaths - p.PointsAttachment.Deaths;
                }

                return p.PointsAttachment.Kills - PointsAttachment.Kills;
            }

            return p.PointsAttachment.Points - PointsAttachment.Points;
        }

        #endregion
    }
}