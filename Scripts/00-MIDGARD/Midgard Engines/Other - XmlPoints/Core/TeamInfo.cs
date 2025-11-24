using System.Collections.Generic;

namespace Server.Engines.XmlPoints
{
    public class TeamInfo
    {
        public int ID;
        public int NActive;
        public int Score;
        public bool Winner;
        public List<IChallengeEntry> Members = new List<IChallengeEntry>();

        public TeamInfo( int teamid )
        {
            ID = teamid;
        }
    }
}