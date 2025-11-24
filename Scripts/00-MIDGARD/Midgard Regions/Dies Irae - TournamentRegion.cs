/***************************************************************************
 *                                  TournamentRegion.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System.Xml;
using Server.Items;

namespace Server.Regions
{
	public class TournamentRegion : BaseRegion
	{
		public TournamentRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{			
		}

		public override bool TravelRestricted { get { return true; } }

		public override void OnEnter( Mobile m )
		{
			m.SendMessage( "You have entered {0}!", Name );
		}

		public override bool OnDoubleClick( Mobile m, object o )
		{
		    if( o is Corpse && m.AccessLevel < AccessLevel.GameMaster )
		    {
		    	m.SendMessage("You cannot loot that corpse here.");
		    	return false;
		    }
			else
				return base.OnDoubleClick( m, o );
		}
	}
}
