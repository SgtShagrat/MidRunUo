using System.Collections.Generic;
using Server.Engines.XmlSpawner2;

namespace Server.Engines.XmlPoints
{
    public interface IChallengeGame
    {
        bool AreInGame( Mobile from );
        bool AreTeamMembers( Mobile from, Mobile target );
        bool AreChallengers( Mobile from, Mobile target );
        void OnPlayerKilled( Mobile killer, Mobile killed );
        void OnKillPlayer( Mobile killer, Mobile killed );
        bool InsuranceIsFree( Mobile from, Mobile awardto );
        bool IsOrganizer( Mobile from );
        void GameBroadcast( string msg );
        void GameBroadcast( int msgindex );
        void GameBroadcast( int msgindex, object msgarg );
        void GameBroadcast( int msgindex, object msgarg, object msgarg2 );
        void EndGame();
        void StartGame();
        void OnTick();
        bool ChallengeBeingCancelled { get; }
        List<Mobile> Organizers { get; }
        bool GameInProgress { get; set; }
        bool GameCompleted { get; }
        bool GameLocked { get; set; }
        string ChallengeName { get; }
        bool UseKillDelay { get; }
        bool AllowPoints { get; }
        void OnDoubleClick( Mobile from );
        void OnDelete();
        void Delete();
        bool Deleted { get; }
        void GetProperties( ObjectPropertyList list );
        List<BaseChallengeEntry> Participants { get; set; }
    }
}