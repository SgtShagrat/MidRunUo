using Midgard.Gumps;

using Server;

namespace Midgard.Engines.Races
{
    public class RaceCandidateApprovalGump : BaseCandidateApprovalGump
    {
        private readonly MidgardRace m_Race;

        public RaceCandidateApprovalGump( MidgardRace system, Mobile from )
            : base( from, system.Candidates )
        {
            m_Race = system;
        }

        public override void ResponseHandler( Mobile from, Mobile candidate, bool reject )
        {
            if( m_Race.Candidates == null || !m_Race.Candidates.Contains( candidate ) )
                return;

            if( reject )
            {
                from.SendMessage( "{0} has been rejected from {1} candidates.", candidate.Name, m_Race.ToString() );
            }
            else
            {
                candidate.Race = m_Race;
                from.SendMessage( "{0} is now a {1}.", candidate.Name, m_Race.ToString() );
            }

            if( m_Race.Candidates.Contains( candidate ) )
                m_Race.Candidates.Remove( candidate );
        }
    }
}