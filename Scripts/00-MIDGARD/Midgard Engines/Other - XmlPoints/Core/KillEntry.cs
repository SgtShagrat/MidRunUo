using System;

namespace Server.Engines.XmlPoints
{
    public class KillEntry
    {
        public Mobile Killed;
        public DateTime WhenKilled;

        public KillEntry( Mobile m, DateTime t )
        {
            Killed = m;
            WhenKilled = t;
        }
    }
}