/***************************************************************************
 *                                  TreasureSpawningCommands.cs
 *                            		---------------------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Commands;
using Server.Regions;

namespace Midgard.Commands
{
	public class TreasureSpawningCommands
	{
		public static void Initialize()
		{
			CommandSystem.Register( "StartTreasureSpawning" , AccessLevel.Developer, new CommandEventHandler( StartTreasureSpawning_OnCommand ) );
			CommandSystem.Register( "ResetTreasureSpawning" , AccessLevel.Developer, new CommandEventHandler( ResetTreasureSpawning_OnCommand ) );
		}

		[Usage( "StartTreasureSpawning" )]
		[Description( "Spawns treasures definitions" )]
		public static void StartTreasureSpawning_OnCommand( CommandEventArgs e )
		{
			TextWriter tw = File.AppendText( "Logs/MidgardTreasureChests.log" );

			foreach( SpawnEntry entry in SpawnEntry.Table.Values )
			{
                if( entry.Definition is SpawnTreasureChest )
				{
					entry.Respawn();

					try
					{
						tw.WriteLine( "Entry with Id {0} RESPAWNED in region {1}.", entry.ID, entry.Region.Name );
					}
					catch( Exception ex )
					{
						Console.Write( "Log failed: {0}", ex );
					}
				}
			}

			tw.Close();
		}

		[Usage( "ResetTreasureSpawning" )]
		[Description( "Reset treasures definitions" )]
		public static void ResetTreasureSpawning_OnCommand( CommandEventArgs e )
		{
			TextWriter tw = File.AppendText( "Logs/MidgardTreasureChests.log" );

			foreach( SpawnEntry entry in SpawnEntry.Table.Values )
			{
                if( entry.Definition is SpawnTreasureChest )
				{
					entry.DeleteSpawnedObjects();

					try
					{
						tw.WriteLine( "Entry with Id {0} RESET in region {1}.", entry.ID, entry.Region.Name );
					}
					catch( Exception ex )
					{
						Console.Write( "Log failed: {0}", ex );
					}
				}
			}

			tw.Close();
		}
	}
}
