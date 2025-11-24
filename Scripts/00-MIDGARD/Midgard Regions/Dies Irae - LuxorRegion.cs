/***************************************************************************
 *                                  LuxorRegion.cs
 *                            		--------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System.Xml;

namespace Server.Regions
{
	public class LuxorRegion : BaseRegion
	{
		public LuxorRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{			
		}

		public override bool AllowBeneficial( Mobile from, Mobile target )
		{
			if ( from.AccessLevel == AccessLevel.Player )
				from.SendMessage( "You may not do that in Luxor." );

			return ( from.AccessLevel > AccessLevel.Player );
		}

		public override bool AllowHarmful( Mobile from, Mobile target )
		{
			if ( from.AccessLevel == AccessLevel.Player )
				from.SendMessage( "You may not do that in Luxor." );

			return ( from.AccessLevel > AccessLevel.Player );
		}

		public override bool AllowHousing( Mobile from, Point3D p )
		{
			return false;
		}

		public override bool OnSkillUse( Mobile from, int Skill )
		{
			if ( from.AccessLevel == AccessLevel.Player )
				from.SendMessage( "You may not use skills in Luxor." );

			return ( from.AccessLevel > AccessLevel.Player );
		}

		public override bool OnCombatantChange( Mobile from, Mobile Old, Mobile New )
		{
			return ( from.AccessLevel > AccessLevel.Player );
		}

		public override void OnEnter( Mobile m )
		{
			m.SendMessage( "Be aware {0}, you have entered Luxor, temple of Midgard Staff.", m.Name );
		}

		public override bool CanUseStuckMenu( Mobile m )
		{
			if ( m.AccessLevel == AccessLevel.Player )
				m.SendMessage( "You cannot use the Stuck menu in Luxor.");

			return ( m.AccessLevel > AccessLevel.Player );
		}
	}
}