/***************************************************************************
 *                                      FactionStartElection.cs
 *                            		------------------------------
 *  begin                	: Febbraio 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 				Fa partire un'elezione di fazione
 *  
 ***************************************************************************/

using Server;
using Server.Commands;
using Server.Factions;
using Server.Targeting;

namespace Midgard.Commands
{
	public class FactionStartElection
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "FactionStartElection" , AccessLevel.Administrator, new CommandEventHandler( FactionStartElection_OnCommand ) );
		}
		#endregion

		#region callback
		[Usage( "FactionStartElection <faction>" )]
		[Description( "Fa partire un'elezione di fazione." )]
		public static void FactionStartElection_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Target a faction stone to start an election for faction commander." );
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( FactionStartElection_OnTarget ) );
		}
		
		public static void FactionStartElection_OnTarget( Mobile from, object obj )
		{
			if( obj is FactionStone )
			{
				Faction faction = ((FactionStone)obj).Faction;
				
				if( faction != null )
				{
					faction.Broadcast( 1038023 ); // Campaigning for the Faction Commander election has begun.

					faction.Election.Candidates.Clear();
					faction.Election.State = ElectionState.Campaign;
					from.SendMessage( "La fazione {0} ha iniziato il periodo di \"{1}\".", faction.ToString(), faction.Election.State.ToString() );
				}
				else
				{
					from.SendMessage( "That stone has no faction assigned." );
				}
			}
			else
			{
				from.SendMessage( "That is not a faction stone." );
			}
		}
		#endregion
	}
}

