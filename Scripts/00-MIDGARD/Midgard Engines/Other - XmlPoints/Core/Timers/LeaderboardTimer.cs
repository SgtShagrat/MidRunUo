using System;

namespace Server.Engines.XmlPoints
{
    public class LeaderboardTimer : Timer
    {
        private readonly string m_Filename;
        private readonly int m_Nranks;

        public LeaderboardTimer( string filename, TimeSpan delay, int nranks )
            : base( delay, delay )
        {
            m_Filename = filename;
            m_Nranks = nranks;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            XmlPointsAttach.WriteLeaderboard( m_Filename, m_Nranks );
        }
    }
}