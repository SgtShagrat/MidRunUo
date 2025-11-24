/***************************************************************************
 *                                  GenChampion.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Il comando genera i champions.
 *  
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Engines.CannedEvil;

namespace Midgard.Commands
{
	public class GenChampionSpawn
	{
        private static Point3D[] OldLocations = new Point3D[]
        {
            new Point3D( 5424, 3413, 36),   // OldBarraLocation
            new Point3D( 5517, 2774, 45 ),  // OldNeiraLocation
            new Point3D( 6104, 3466, -2 ),  // OldMephitisLocation
            new Point3D( 5988, 2357, 22 ),  // OldSemidarLocation
            new Point3D( 6007, 2903, 13 ),  // OldRikktorLocation
            new Point3D( 5279, 72, 15 )     // OldLordOakLocation
        };

        private static int[] OldTypes = new int[]
        {
            (int)ChampionSpawnType.VerminHorde,
            (int)ChampionSpawnType.UnholyTerror,
            (int)ChampionSpawnType.Arachnid,
            (int)ChampionSpawnType.Abyss,
            (int)ChampionSpawnType.ColdBlood,
            (int)ChampionSpawnType.ForestLord,
        };

		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "GenChampions" , AccessLevel.Developer, new CommandEventHandler( Champ_OnCommand ) );
		}
		#endregion

        private static void GenerateOldChampion()
        {
            for (int i = 0; i < OldLocations.Length; i++)
            {
                ChampionSpawn altar = new ChampionSpawn();

                altar.Active = true;
                altar.RandomizeType = false;
                altar.Type = (ChampionSpawnType)OldTypes[i];
                altar.MoveToWorld( OldLocations[i], Map.Felucca );
            }
        }

		#region callback
		[Usage( "GenChampions" )]
		[Description( "Spawns Champion Spawns for all maps." )]
		public static void Champ_OnCommand( CommandEventArgs e )
		{
            if( !Core.AOS )
            {
                GenerateOldChampion();
                return;
            }

			Map map1 = Map.Ilshenar;
			Map map2 = Map.Felucca;
			Map map3 = Map.Tokuno;

			e.Mobile.SendMessage( "Generating Champions spawns..." );

			ChampionSpawn valor = new ChampionSpawn();
			ChampionSpawn humility = new ChampionSpawn();
			ChampionSpawn forest = new ChampionSpawn();

			ChampionSpawn Despise = new ChampionSpawn();
			ChampionSpawn Deceit = new ChampionSpawn();
			ChampionSpawn Destard = new ChampionSpawn();
			ChampionSpawn Fire = new ChampionSpawn();
			ChampionSpawn TerathanKeep = new ChampionSpawn();
			ChampionSpawn LostLands1 = new ChampionSpawn();
			ChampionSpawn LostLands2 = new ChampionSpawn();
			ChampionSpawn LostLands3 = new ChampionSpawn();
			ChampionSpawn LostLands4 = new ChampionSpawn();
			ChampionSpawn LostLands5 = new ChampionSpawn();
			ChampionSpawn LostLands6 = new ChampionSpawn();
			ChampionSpawn LostLands7 = new ChampionSpawn();
			ChampionSpawn LostLands8 = new ChampionSpawn();
			ChampionSpawn LostLands9 = new ChampionSpawn();
			ChampionSpawn LostLands10 = new ChampionSpawn();
			ChampionSpawn LostLands11 = new ChampionSpawn();
			ChampionSpawn LostLands12 = new ChampionSpawn();

			ChampionSpawn isamu = new ChampionSpawn();

			valor.RandomizeType = true;
			valor.Active = false;
			valor.MoveToWorld( new Point3D( 382, 328, -30 ), map1 );

			humility.RandomizeType = true;
			humility.Active = true;
			humility.MoveToWorld( new Point3D( 462, 926, -67 ), map1 );

			forest.Active = true;
			forest.RandomizeType = false;
			forest.Type = ChampionSpawnType.ForestLord;
			forest.MoveToWorld( new Point3D( 1645, 1107, 8 ), map1 );

			e.Mobile.SendMessage( "Ilshenar Champions Complete." );

			Despise.Active = true;
			Despise.RestartDelay = TimeSpan.FromDays( 9999.0 );
			Despise.RandomizeType = false;
			Despise.Type = ChampionSpawnType.VerminHorde;
			Despise.MoveToWorld( new Point3D( 5557, 824, 65 ), map2 );

			Deceit.Active = true;
			Deceit.RestartDelay = TimeSpan.FromDays( 9999.0 );
			Deceit.RandomizeType = false;
			Deceit.Type = ChampionSpawnType.UnholyTerror;
			Deceit.MoveToWorld( new Point3D( 5178, 708, 20 ), map2 );

			Destard.Active = true;
			Destard.RestartDelay = TimeSpan.FromDays( 9999.0 );
			Destard.RandomizeType = false;
			Destard.Type = ChampionSpawnType.ColdBlood;
			Destard.MoveToWorld( new Point3D( 5259, 837, 61 ), map2 );

			Fire.Active = true;
			//Fire.SpawnRange = 24;
			Fire.RestartDelay = TimeSpan.FromDays( 9999.0 );
			Fire.RandomizeType = false;
			Fire.Type = ChampionSpawnType.Abyss;
			Fire.MoveToWorld( new Point3D( 5814, 1350, 2 ), map2 );

			TerathanKeep.Active = true;
			TerathanKeep.RestartDelay = TimeSpan.FromDays( 9999.0 );
			TerathanKeep.RandomizeType = false;
			TerathanKeep.Type = ChampionSpawnType.Arachnid;
			TerathanKeep.MoveToWorld( new Point3D( 5190, 1605, 20 ), map2 );

			e.Mobile.SendMessage( "Felucca Dungeon Champions complete" );

			LostLands1.Active = true;
			LostLands1.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands1.RandomizeType = true;
			LostLands1.MoveToWorld( new Point3D( 5511, 2360, 40 ), map2 );

			LostLands2.Active = true;
			LostLands2.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands2.RandomizeType = true;
			LostLands2.MoveToWorld( new Point3D( 6038, 2400, 45 ), map2 );

			LostLands3.Active = true;
			LostLands3.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands3.RandomizeType = true;
			LostLands3.MoveToWorld( new Point3D( 5549, 2640, 18 ), map2 );

			LostLands4.Active = true;
			LostLands4.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands4.RandomizeType = true;
			LostLands4.MoveToWorld( new Point3D( 5636, 2916, 38 ), map2 );

			LostLands5.Active = true;
			LostLands5.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands5.RandomizeType = true;
			LostLands5.MoveToWorld( new Point3D( 6035, 2943, 50 ), map2 );

			LostLands6.Active = true;
			LostLands6.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands6.RandomizeType = true;
			LostLands6.MoveToWorld( new Point3D( 5265, 3171, 107 ), map2 );

			LostLands7.Active = true;
			LostLands7.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands7.RandomizeType = true;
			LostLands7.MoveToWorld( new Point3D( 5282, 3368, 50 ), map2 );

			LostLands8.Active = true;
			LostLands8.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands8.RandomizeType = true;
			LostLands8.MoveToWorld( new Point3D( 5954, 3475, 25 ), map2 );

			LostLands9.Active = true;
			LostLands9.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands9.RandomizeType = true;
			LostLands9.MoveToWorld( new Point3D( 5207, 3637, 20 ), map2 );

			LostLands10.Active = true;
			LostLands10.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands10.RandomizeType = true;
			LostLands10.MoveToWorld( new Point3D( 5559, 3757, 21 ), map2 );

			LostLands11.Active = true;
			LostLands11.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands11.RandomizeType = true;
			LostLands11.MoveToWorld( new Point3D( 5982, 3882, 20 ), map2 );

			LostLands12.Active = true;
			LostLands12.RestartDelay = TimeSpan.FromDays( 9999.0 );
			LostLands12.RandomizeType = true;
			LostLands12.MoveToWorld( new Point3D( 5724, 3991, 41 ), map2 );

			e.Mobile.SendMessage( "Felucca - Lost Lands Champions complete" );

            isamu.Active = true;
            isamu.RandomizeType = false;
            isamu.Type = ChampionSpawnType.SleepingDragon;
            isamu.MoveToWorld( new Point3D( 948, 434, 29 ), map3 );

            e.Mobile.SendMessage( "Tokuno Champion complete" );
            
            e.Mobile.SendMessage( "Generation Complete." );
		}
		#endregion
	}
}
