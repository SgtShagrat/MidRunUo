/***************************************************************************
 *                                    FactionChat.cs
 *                            		------------------
 *  begin                	: Luglio, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Il comando FactionChat (alias FC) manda un messaggio ai 
 * 			cofazionati.
 * 
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Commands;
using Server.Factions;

namespace Midgard.Commands
{
	public class FactionChat
	{
		#region registrazione
		public static void Initialize()
		{
			// CommandSystem.Register( "FactionChat" , 		AccessLevel.Player, new CommandEventHandler( FactionChat_OnCommand ) );
			// CommandSystem.Register( "FC" , 					AccessLevel.Player, new CommandEventHandler( FactionChat_OnCommand ) );
		}
		#endregion

		// factionBroadcastDelay è l'intervallo minimo tra i messaggi
		public static readonly TimeSpan factionBroadcastDelay = TimeSpan.FromSeconds( 3 );
		
		// HastTable privata che tiene la lista dei mobiles che usano il comando 
		// con successo
		private static Hashtable table = new Hashtable();
		
		#region callback
		[Usage( "FactionChat or FC" )]
		[Description( "Send a message to all faction members." )]
		public static void FactionChat_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			
			if( from == null )
				return;
			
			// Se from è nella hash allora verifica se può o meno mandare messaggi
			if( table.ContainsKey( from ) )//&& (DateTime)table[ from ] != null ) <----(MagiusCHE: DateTime non può essere mai NULL!)
			{
				if( (DateTime)table[ from ] > DateTime.Now )
				{
					from.SendMessage( "You cannot use this command yet!" );
					return;
				}
			}
			
			// Cerca la fazione del sender
			Faction f = Faction.Find( from );
			
			if( f != null && !string.IsNullOrEmpty( from.Name ) )
			{
				// Processa tutta la stringa e non solo il primo argomento
				string message = e.ArgString; 
				
				if( !string.IsNullOrEmpty( message ) )
				{
					f.Broadcast( 15, String.Format( "[{0}] {1}", from.Name, message ) );
					
					// Aggiunge alla hashtable valore[chiave] come NextBroadcastAvailable[from]
					table[ from ] = DateTime.Now + factionBroadcastDelay;
				}
				else
				{
					from.SendMessage( "Command usage: [FactionChat or [FC followed by a message to broadcast" );
				}
			}
			else
			{
				from.SendMessage( "You cannot use this command!" );
			}
		}
		#endregion
	}
}
