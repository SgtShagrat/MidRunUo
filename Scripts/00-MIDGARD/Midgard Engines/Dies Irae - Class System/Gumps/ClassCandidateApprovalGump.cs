/***************************************************************************
 *                               ClassCandidateApprovalGump.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Gumps;

using Server;

namespace Midgard.Engines.Classes
{
    public class ClassCandidateApprovalGump : BaseCandidateApprovalGump
    {
        private readonly BaseClassGiver m_Giver;
        private readonly ClassSystem m_System;

        public ClassCandidateApprovalGump( BaseClassGiver giver, ClassSystem system, Mobile from )
            : base( from, system.Candidates )
        {
            m_Giver = giver;
            m_System = system;
        }

        public override void ResponseHandler( Mobile from, Mobile candidate, bool reject )
        {
            if( !m_System.IsCandidate( candidate ) )
                return;

            if( reject )
            {
                from.SendMessage( "{0} has been rejected from {1} candidates.", candidate.Name, m_System.ToString() );

                if( m_System.Candidates.Contains( candidate ) )
                    m_System.Candidates.Remove( candidate );
            }
            else
            {
                from.SendMessage( "{0} is now a {1}.", candidate.Name, m_System.ToString() );

                ClassSystemCommands.DoSetClass( candidate, m_System );

                try
                {
                    if( m_Giver != null )
                        candidate.AddToBackpack( m_Giver.Book );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e );
                }
            }
        }
    }
}