using System;
using Midgard.Gumps;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.PermaredToBlue
{
	public class PermaredCandidateApprovalGump : BaseCandidateApprovalGump
	{
		private readonly BaseBlueGiver m_Giver;

		public PermaredCandidateApprovalGump( BaseBlueGiver giver, Mobile from )
			: base( from, giver.Candidates )
		{
			m_Giver = giver;
		}

		public override void ResponseHandler( Mobile from, Mobile candidate, bool reject )
		{
			if( m_Giver == null || from == null || candidate == null || !m_Giver.IsCandidate( candidate ) || !(candidate is Midgard2PlayerMobile) )
				return;

			if( reject )
			{
				from.SendMessage( "{0} has been rejected from redemption candidates.", candidate.Name );

				if( m_Giver.Candidates != null && m_Giver.Candidates.Contains( candidate ) )
					m_Giver.Candidates.Remove( candidate );
			}
			else
			{
				Midgard2PlayerMobile mob = candidate as Midgard2PlayerMobile;
				from.SendMessage( "{0} is now blue.", candidate.Name);
				mob.PermaRed = false;
				mob.LifeTimeKills = 0;
				mob.Kills = 0;
				mob.ShortTermMurders = 0;
				if( m_Giver.Candidates != null && m_Giver.Candidates.Contains( candidate ) )
					m_Giver.Candidates.Remove( candidate );
				//ClassSystemCommands.DoSetClass( candidate, m_System );
			}
		}
	}
}