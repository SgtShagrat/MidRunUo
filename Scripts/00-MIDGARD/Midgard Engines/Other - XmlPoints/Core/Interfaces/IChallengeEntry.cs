using System;

namespace Server.Engines.XmlPoints
{
    public interface IChallengeEntry
    {
        Mobile Participant { get; set; }
        ChallengeStatus Status { get; set; }
        ChallengeStatus Caution { get; set; }
        bool Accepted { get; set; }
        DateTime LastCaution { get; set; }
        int PageBeingViewed { get; set; }
        int Team { get; set; }
        int Score { get; set; }
        bool Winner { get; set; }
    }
}