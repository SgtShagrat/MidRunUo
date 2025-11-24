using System;
using Server.Engines.XmlPoints;

namespace Server.Engines.XmlSpawner2
{
    public abstract class BaseChallengeEntry : IChallengeEntry
    {
        public virtual Mobile Participant { get; set; }

        public virtual ChallengeStatus Status { get; set; }

        public virtual ChallengeStatus Caution { get; set; }

        public virtual bool Accepted { get; set; }

        public virtual DateTime LastCaution { get; set; }

        public virtual int PageBeingViewed { get; set; }

        public virtual int Score { get; set; }

        public virtual int Team { get; set; }

        public virtual bool Winner { get; set; }

        protected BaseChallengeEntry( Mobile m )
        {
            Participant = m;
            Status = ChallengeStatus.Active;
            Accepted = false;
        }

        protected BaseChallengeEntry()
        {
        }
    }
}