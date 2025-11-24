// #define DebugMutateType
// #define DebugGetVeinAt
// #define DebugMutateType
// #define DebugGetHarvestVeinFromTileId

using System;
using Midgard.Engines;
using Midgard.Engines.PlantSystem;
using Midgard.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Harvest
{
	public class Lumberjacking : HarvestSystem
	{
		private static Lumberjacking m_System;

		public static Lumberjacking System
		{
			get
			{
				if ( m_System == null )
					m_System = new Lumberjacking();

				return m_System;
			}
		}

		private HarvestDefinition m_Definition;

		public HarvestDefinition Definition
		{
			get{ return m_Definition; }
		}

		private Lumberjacking()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region Lumberjacking
			HarvestDefinition lumber = new HarvestDefinition();

			// Resource banks are every 4x3 tiles
			lumber.BankWidth = 1; // 4;
			lumber.BankHeight = 1; // 3;

			// Every bank holds from 20 to 45 logs
			lumber.MinTotal = 20;
			lumber.MaxTotal = 45;

			// A resource bank will respawn its content every 20 to 30 minutes
			lumber.MinRespawn = TimeSpan.FromMinutes( 20.0 );
			lumber.MaxRespawn = TimeSpan.FromMinutes( 30.0 );

			// Skill checking is done on the Lumberjacking skill
			lumber.Skill = SkillName.Lumberjacking;

			// Set the list of harvestable tiles
			lumber.Tiles = m_TreeTiles;

			// Players must be within 2 tiles to harvest
			lumber.MaxRange = 2;

			// Ten logs per harvest action
			lumber.ConsumedPerHarvest = 4; // 10; // mod by Dies Irae
			lumber.ConsumedPerFeluccaHarvest = 4; // 20; // mod by Dies Irae

			// The chopping effect
			lumber.EffectActions = new int[]{ 13 };
			lumber.EffectSounds = new int[]{ 0x13E };
			lumber.EffectCounts = (Core.AOS ? new int[]{ 1 } : new int[]{ 1, 2, 2, 2, 3 });
			lumber.EffectDelay = TimeSpan.FromSeconds( 1.6 );
			lumber.EffectSoundDelay = TimeSpan.FromSeconds( 0.9 );

			lumber.NoResourcesMessage = 500493; // There's not enough wood here to harvest.
			lumber.FailMessage = 500495; // You hack at the tree for a while, but fail to produce any useable wood.
			lumber.OutOfRangeMessage = 500446; // That is too far away.
			lumber.PackFullMessage = 500497; // You can't place any wood into your backpack!
			lumber.ToolBrokeMessage = 500499; // You broke your axe.

			#region modifica by Dies Irae
			res = new HarvestResource[]
			{
				new HarvestResource( 00.0, 00.0,  60.0,  500498, typeof( Log ) 			),
				new HarvestResource( 40.0, 10.0,  70.0, 1064900, typeof( OakLog )		),
				new HarvestResource( 45.0, 15.0,  75.0, 1064901, typeof( WalnutLog ) 	),
				new HarvestResource( 50.0, 20.0,  80.0, 1064902, typeof( OhiiLog ) 		),
				new HarvestResource( 55.0, 25.0,  85.0, 1064903, typeof( CedarLog ) 	),
				new HarvestResource( 60.0, 30.0,  90.0, 1064904, typeof( WillowLog ) 	),
				new HarvestResource( 65.0, 35.0,  95.0, 1064905, typeof( CypressLog ) 	),
				new HarvestResource( 70.0, 40.0, 100.0, 1064906, typeof( YewLog )		),
				new HarvestResource( 75.0, 45.0, 105.0, 1064907, typeof( AppleLog ) 	),
				new HarvestResource( 80.0, 50.0, 110.0, 1064908, typeof( PearLog ) 		),
				new HarvestResource( 85.0, 55.0, 115.0, 1064909, typeof( PeachLog ) 	),
				new HarvestResource( 90.0, 60.0, 120.0, 1064910, typeof( BananaLog )	),
				new HarvestResource( 00.0, 00.0, 100.0, "You put some dead wood in your backpack.", typeof( DeadWood )	)
			};

			veins = new HarvestVein[]
			{
				new HarvestVein( 23.1, 0.0, res[0], 	null ),
				new HarvestVein( 23.1, 0.0, res[1], 	null ),
				new HarvestVein( 23.1, 0.0, res[2], 	null ),
				new HarvestVein( 23.1, 0.0, res[3], 	null ),
				new HarvestVein( 23.1, 0.0, res[4], 	null ),
				new HarvestVein( 23.1, 0.0, res[5], 	null ),
				new HarvestVein( 23.1, 0.0, res[6], 	null ),
				new HarvestVein( 23.1, 0.0, res[7], 	null ),
				new HarvestVein( 23.1, 0.0, res[8], 	null ),
				new HarvestVein( 23.1, 0.0, res[9], 	null ),
				new HarvestVein( 23.1, 0.0, res[10], 	null ),
				new HarvestVein( 23.1, 0.0, res[11], 	null ),
				new HarvestVein( 23.1, 0.0, res[12], 	null ), // dead wood
			};

			lumber.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0,	99.90, null, null ),	//Nothing
					new BonusHarvestResource( 90.0, 0.016, 1064378, typeof( TreasureMap ) ),
					new BonusHarvestResource( 100,  0.016, 1072548, typeof( BarkFragment ) ),
					new BonusHarvestResource( 100,  0.016, 1072550, typeof( LuminescentFungi ) ),
					new BonusHarvestResource( 100,  0.016, 1072547, typeof( SwitchItem ) ),
					new BonusHarvestResource( 100,  0.016, 1072549, typeof( ParasiticPlant ) ),
					new BonusHarvestResource( 100,  0.016, 1072551, typeof( BrilliantAmber ) )
				};
			#endregion

			/*
			if ( Core.ML )
			{
				res = new HarvestResource[]
				{
					new HarvestResource(  00.0, 00.0, 100.0, 1072540, typeof( Log ) ),
					new HarvestResource(  65.0, 25.0, 105.0, 1072541, typeof( OakLog ) ),
					new HarvestResource(  80.0, 40.0, 120.0, 1072542, typeof( AshLog ) ),
					new HarvestResource(  95.0, 55.0, 135.0, 1072543, typeof( YewLog ) ),
					new HarvestResource( 100.0, 60.0, 140.0, 1072544, typeof( HeartwoodLog ) ),
					new HarvestResource( 100.0, 60.0, 140.0, 1072545, typeof( BloodwoodLog ) ),
					new HarvestResource( 100.0, 60.0, 140.0, 1072546, typeof( FrostwoodLog ) ),
				};


				veins = new HarvestVein[]
				{
					new HarvestVein( 49.0, 0.0, res[0], null ),	// Ordinary Logs
					new HarvestVein( 30.0, 0.5, res[1], res[0] ), // Oak
					new HarvestVein( 10.0, 0.5, res[2], res[0] ), // Ash
					new HarvestVein( 05.0, 0.5, res[3], res[0] ), // Yew
					new HarvestVein( 03.0, 0.5, res[4], res[0] ), // Heartwood
					new HarvestVein( 02.0, 0.5, res[5], res[0] ), // Bloodwood
					new HarvestVein( 01.0, 0.5, res[6], res[0] ), // Frostwood
				};

				lumber.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0, 83.9, null, null ),	//Nothing
					new BonusHarvestResource( 100, 10.0, 1072548, typeof( BarkFragment ) ),
					new BonusHarvestResource( 100, 03.0, 1072550, typeof( LuminescentFungi ) ),
					new BonusHarvestResource( 100, 02.0, 1072547, typeof( SwitchItem ) ),
					new BonusHarvestResource( 100, 01.0, 1072549, typeof( ParasiticPlant ) ),
					new BonusHarvestResource( 100, 00.1, 1072551, typeof( BrilliantAmber ) )
				};
			}
			else
			{
				res = new HarvestResource[]
				{
					new HarvestResource( 00.0, 00.0, 100.0, 500498, typeof( Log ) )
				};

				veins = new HarvestVein[]
				{
					new HarvestVein( 100.0, 0.0, res[0], null )
				};
			}
			*/
			
			lumber.Resources = res;
			lumber.Veins = veins;

			lumber.RaceBonus = Core.ML;
			lumber.RandomizeVeins = Core.ML;

			m_Definition = lumber;
			Definitions.Add( lumber );
			#endregion
		}

		public override object GetLock( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			return this;
		}

		public override void OnConcurrentHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			from.SendMessage( "You are already lumberjacking." );
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;		

			if ( tool.Parent != from )
			{
				from.SendLocalizedMessage( 500487 ); // The axe must be equipped for any serious wood chopping.
				return false;
			}
			#region modifica by Dies Irae
			else if( from.Mounted )
			{
				from.SendMessage( "You can't lumberjack while riding." );
				return false;
			}
			else if( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendMessage( "You can't lumberjack while polymorphed." );
				return false;
			}
			#endregion

			return true;
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( tool.Parent != from )
			{
				from.SendLocalizedMessage( 500487 ); // The axe must be equipped for any serious wood chopping.
				return false;
			}
			#region modifica by Dies Irae
			else if ( from.Mounted )
			{
				from.SendLocalizedMessage( 1064911 ); // You can't lumberjack while riding.
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendLocalizedMessage( 1064912 ); // You can't lumberjack while polymorphed.
				return false;
			}
			#endregion
			return true;
		}

		#region mod by Dies Irae [harvest chance]
		public override void GetHarvestChancheMods( Mobile from, HarvestResource resource, double minSkill, ref double rawGainChance )
		{
			rawGainChance = rawGainChance / 2.0;

			if( minSkill == 0 )
				rawGainChance = 0.5;
		}
		#endregion

		#region modifica by Dies Irae: gestione dei legni speciali
		public override bool GetHarvestDetails( Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc )
		{
			if( toHarvest is BaseTree )
			{
				BaseTree obj = (BaseTree)toHarvest;

				tileID = ( obj.ItemID & 0x3FFF ) | 0x4000;
				map = obj.Map;
				loc = obj.GetWorldLocation();

				return ( map != null && map != Map.Internal );
			}
			else
				return base.GetHarvestDetails( from, tool, toHarvest, out tileID, out map, out loc );
		}

		public override void SendSuccessTo( Mobile from, Item item, HarvestResource resource )
		{
			#region mod by Dies Irae
			// SendSuccessTo( from, item, resource );
			Type t = item.GetType();

			if( Core.AOS )
			{
				foreach( MutateEntry me in m_LumberMutateTable )
				{
					// Se il tipo di log dato e' un log speciale (mutato) allora non dire il messaggio
					// perche' esso viene detto da MutateType()
					if( t == me.Type )
					return;
				}
			}
			else
			{
				foreach( OldMutateEntry me in m_OldLumberMutateTable )
				{
					// Se il tipo di log dato e' un log speciale (mutato) allora non dire il messaggio
					// perche' esso viene detto da MutateType()
					if( t == me.Type )
						return;
				}
			}
			resource.SendSuccessTo( from );
			#endregion
		}

		public override Type MutateType( Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			if( !Midgard2Persistance.SpecialLogsEnabled )
				return type;

			double skillBase = from.Skills[ SkillName.Lumberjacking ].Base;
			double skillValue = from.Skills[ SkillName.Lumberjacking ].Value;

			// Funzione lineare: passa per i punti (70,0) e (100,5)
			double chanceSpecial = Math.Max( ( ( skillBase - 70.0 ) / 6.0 ) / 100.0, 0.0 );

			#if DebugMutateType
			Console.WriteLine( "\nDebugMutateType" );
			Console.WriteLine( "\tChanceSpecial {0}", chanceSpecial.ToString( "F3" ) );
			if( from.AccessLevel == AccessLevel.Developer )
				chanceSpecial = 2.0;
			#endif
			if( chanceSpecial > Utility.RandomDouble() )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1064388 ); // * You have found a Special Log *
				from.PlaySound( 0x244 );
				from.CheckSkill( SkillName.Lumberjacking, 0 );//edit by Arlas: bonus legno magico

				if( Core.AOS )
				{
					for( int i = m_LumberMutateTable.Length - 1; i > -1; i-- )
					{
						MutateEntry entry = m_LumberMutateTable[ i ];

						if( skillBase >= entry.ReqSkill )
						{
							double chance = ( skillValue - entry.MinSkill ) / ( entry.MaxSkill - entry.MinSkill );
							double success = Utility.RandomDouble() * 100;
							#if DebugMutateType
							Console.WriteLine( "\tEntry.m_Type.Name {0} - chance {1} - success {2}.",
												entry.Type.Name, chance.ToString( "F3" ), success.ToString( "F3" ) );
							#endif
							if( chance > success )
							{
								from.SendLocalizedMessage( entry.Message );
								return entry.Type;
							}
						}
					}
				}
				else
				{
					int success = Utility.Random( 100 ) + 1;

					for( int i = m_OldLumberMutateTable.Length - 1; i > -1; i-- )
					{
						OldMutateEntry entry = m_OldLumberMutateTable[ i ];

						if( skillBase >= entry.ReqSkill && success < entry.Chance )
						{
							#if DebugMutateType
							Console.WriteLine( "\tEntry.m_Type.Name {0} - chance {1} - success {2}.",
												entry.Type.Name, entry.Chance.ToString( "F3" ), success.ToString( "F3" ) );
							#endif
							from.SendLocalizedMessage( entry.Message );
							return entry.Type;
						}
					}
				}
			}
			return type;
		}
		#endregion

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			if ( toHarvest is Mobile )
			{
				Mobile obj = (Mobile)toHarvest;
				obj.PublicOverheadMessage( Server.Network.MessageType.Regular, 0x3E9, 500450 ); // You can only skin dead creatures.
			}
			else if ( toHarvest is Item )
			{
				Item obj = (Item)toHarvest;
				obj.PublicOverheadMessage( Server.Network.MessageType.Regular, 0x3E9, 500464 ); // Use this on corpses to carve away meat and hide
			}
			else if ( toHarvest is Targeting.StaticTarget || toHarvest is Targeting.LandTarget )
				from.SendLocalizedMessage( 500489 ); // You can't use an axe on that.
			else
				from.SendLocalizedMessage( 1005213 ); // You can't do that
		}

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );
			
			// if( Core.ML ) // mod by Dies Irae
				from.RevealingAction( true );//edit by Arlas

			AutoMacroCheck( from ); // mod by Dies Irae
		}

		public static void Initialize()
		{
			Array.Sort( m_TreeTiles );
		}

		#region Tile lists
		private static int[] m_TreeTiles = new int[]
			{
				0x4CCA, 0x4CCB, 0x4CCC, 0x4CCD, 0x4CD0, 0x4CD3, 0x4CD6, 0x4CD8,
				0x4CDA, 0x4CDD, 0x4CE0, 0x4CE3, 0x4CE6, 0x4CF8, 0x4CFB, 0x4CFE,
				0x4D01, 0x4D41, 0x4D42, 0x4D43, 0x4D44, 0x4D57, 0x4D58, 0x4D59,
				0x4D5A, 0x4D5B, 0x4D6E, 0x4D6F, 0x4D70, 0x4D71, 0x4D72, 0x4D84,
				0x4D85, 0x4D86, 0x52B5, 0x52B6, 0x52B7, 0x52B8, 0x52B9, 0x52BA,
				0x52BB, 0x52BC, 0x52BD,

				0x4CCE, 0x4CCF, 0x4CD1, 0x4CD2, 0x4CD4, 0x4CD5, 0x4CD7, 0x4CD9,
				0x4CDB, 0x4CDC, 0x4CDE, 0x4CDF, 0x4CE1, 0x4CE2, 0x4CE4, 0x4CE5,
				0x4CE7, 0x4CE8, 0x4CF9, 0x4CFA, 0x4CFC, 0x4CFD, 0x4CFF, 0x4D00,
				0x4D02, 0x4D03, 0x4D45, 0x4D46, 0x4D47, 0x4D48, 0x4D49, 0x4D4A,
				0x4D4B, 0x4D4C, 0x4D4D, 0x4D4E, 0x4D4F, 0x4D50, 0x4D51, 0x4D52,
				0x4D53, 0x4D5C, 0x4D5D, 0x4D5E, 0x4D5F, 0x4D60, 0x4D61, 0x4D62,
				0x4D63, 0x4D64, 0x4D65, 0x4D66, 0x4D67, 0x4D68, 0x4D69, 0x4D73,
				0x4D74, 0x4D75, 0x4D76, 0x4D77, 0x4D78, 0x4D79, 0x4D7A, 0x4D7B,
				0x4D7C, 0x4D7D, 0x4D7E, 0x4D7F, 0x4D87, 0x4D88, 0x4D89, 0x4D8A,
				0x4D8B, 0x4D8C, 0x4D8D, 0x4D8E, 0x4D8F, 0x4D90, 0x4D95, 0x4D96,
				0x4D97, 0x4D99, 0x4D9A, 0x4D9B, 0x4D9D, 0x4D9E, 0x4D9F, 0x4DA1,
				0x4DA2, 0x4DA3, 0x4DA5, 0x4DA6, 0x4DA7, 0x4DA9, 0x4DAA, 0x4DAB,
				0x52BE, 0x52BF, 0x52C0, 0x52C1, 0x52C2, 0x52C3, 0x52C4, 0x52C5,
				0x52C6, 0x52C7,

				#region mod by Dies Irae
				0x4C9E, 				// O'hii tree
				0x4CA8, 0x4CAA, 0x4CAB,	// Banana Tree
				0x4D94, 0x4D98, 		// Apple tree
				0x4D9C, 0x4DA0, 		// Peach tree
				0x4DA4, 0x4DA8 			// Pear
				#endregion
			};
		#endregion

		#region Trees Tables
		private static readonly int[][] m_TreesTable = new int[][]
		{
			new int[]{ 	0x4CCA, 0x4CCB, 0x4CCC, 0x4CCD, 0x4CD0, 0x4CD3,
					   	0x4D41, 0x4D42, 0x4D43, 0x4D44, 0x4D57, 0x4D58,
					   	0x4D59, 0x4D5A, 0x4D5B, 0x4D6E, 0x4D6F, 0x4D70,
					   	0x4D71, 0x4D72, 0x4D84, 0x4D85, 0x4D86,
					   	0x4CCE, 0x4CCF, 0x4CD1, 0x4CD2, 0x4CD4, 0x4CD5,
						0x4D45, 0x4D46, 0x4D47, 0x4D48, 0x4D49, 0x4D4A,
						0x4D4B, 0x4D4C, 0x4D4D, 0x4D4E, 0x4D4F, 0x4D50, 
						0x4D51, 0x4D52,	0x4D53, 0x4D5C, 0x4D5D, 0x4D5E,
						0x4D5F, 0x4D60, 0x4D61, 0x4D62, 0x4D63, 0x4D64, 
						0x4D65, 0x4D66, 0x4D67, 0x4D68, 0x4D69, 0x4D73,
						0x4D74, 0x4D75, 0x4D76, 0x4D77, 0x4D78, 0x4D79, 
						0x4D7A, 0x4D7B, 0x4D7C, 0x4D7D, 0x4D7E, 0x4D7F,	
						0x4D87, 0x4D88, 0x4D89, 0x4D8A, 0x4D8B, 0x4D8C, 
						0x4D8D, 0x4D8E, 0x4D8F, 0x4D90				},	// Logs Normali
			new int[]{ 	0x4CDA, 0x4CDD, 0x624C, 0x624D,
					   	0x4CDB, 0x4CDC, 0x4CDE, 0x4CDF				}, 	// Oak
			new int[]{ 	0x4CE0, 0x4CE3,
						0x4CE1, 0x4CE2, 0x4CE4, 0x4CE5				},	// Walnut
			new int[]{ 	0x4C9E								},	// O'hii
			new int[]{ 	0x4CD6, 0x4CD8,
					   	0x4CD7, 0x4CD9, 0x4DB7, 0x4DB8				}, 	// Cedar
			new int[]{ 	0x4CE6, 0x624A, 0x624B,
					   	0x4CE7, 0x4CE8						},	// Willow
			new int[]{ 	0x4CF8, 0x4CFB, 0x4CFE, 0x4D01,
						0x4CF9, 0x4CFA, 0x4CFC, 0x4CFD, 0x4CFF, 0x4D00,
						0x4D02, 0x4D03						},	// Cypress
			new int[]{ 	0x52B5, 0x52B6, 0x52B7, 0x52B8, 0x52B9, 0x52BA,
					   	0x52BB, 0x52BC, 0x52BD,
						0x52BE, 0x52BF, 0x52C0, 0x52C1, 0x52C2, 0x52C3, 
						0x52C4, 0x52C5, 0x52C6, 0x52C7				},	// Yew
			new int[]{ 	0x4D94, 0x4D98,
						0x4D95, 0x4D96, 0x4D97, 0x4D99, 0x4D9A, 0x4D9B	},	// Apple
			new int[]{ 	0x4DA4, 0x4DA8,		 							
						0x4DA5, 0x4DA6, 0x4DA7, 0x4DA9, 0x4DAA, 0x4DAB	},	// Pear
			new int[]{ 	0x4D9C, 0x4DA0,
						0x4D9D, 0x4D9E, 0x4D9F, 0x4DA1, 0x4DA2, 0x4DA3	},	// Peach
			new int[]{ 	0x4CA8, 0x4CAA, 0x4CAB		 					}	// Banana
		};

		private static readonly int[] m_DeadTreesTable = new int[]
		{
			0x4Cf3, 0x4Cf4, 0x4Cf5, 0x4Cf6, 0x4Cf7,
		};
		#endregion

		#region mod by Dies Irae
		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
			SpawnCritter( from, tool, def, vein, bank, resource, harvested );
		}

		public override Type GetRandomCritter()
		{
			return m_LumberSpawnTypes[ Utility.Random( m_LumberSpawnTypes.Length ) ];
		}

		private static readonly Type[] m_LumberSpawnTypes = new Type[]
			{
				typeof( Reaper ),
				typeof( Bird ),
				typeof( Bird ),
				typeof( Snake ),
				typeof( Snake ),
				typeof( Snake ),
				typeof( Rat ),
				typeof( Rat ),
				typeof( Rat ),
				typeof( Rabbit ),
				typeof( Rabbit ),
				typeof( Rabbit ),
			};

		private class MutateEntry
		{
			private double m_ReqSkill;
			private double m_MinSkill;
			private double m_MaxSkill;
			private Type m_Type;
			private int m_Message;

			public MutateEntry( double reqSkill, double minSkill, double maxSkill, Type type, int message )
			{
				m_ReqSkill = reqSkill;
				m_MinSkill = minSkill;
				m_MaxSkill = maxSkill;
				m_Type = type;
				m_Message = message;
			}

			public double ReqSkill
			{
				get { return m_ReqSkill; }
			}

			public double MinSkill
			{
				get { return m_MinSkill; }
			}

			public double MaxSkill
			{
				get { return m_MaxSkill; }
			}

			public Type Type
			{
				get { return m_Type; }
			}

			public int Message
			{
				get { return m_Message; }
			}
		}

		private class OldMutateEntry
		{
			private double m_ReqSkill;
			private int m_Chance;
			private Type m_Type;
			private int m_Message;

			public OldMutateEntry( double reqSkill, int chance, Type type, int message )
			{
				m_ReqSkill = reqSkill;
				m_Chance = chance;
				m_Type = type;
				m_Message = message;
			}

			public double ReqSkill
			{
				get { return m_ReqSkill; }
			}

			public int Chance
			{
				get { return m_Chance; }
			}

			public Type Type
			{
				get { return m_Type; }
				set { m_Type = value; }
			}

			public int Message
			{
				get { return m_Message; }
			}
		}

		#region m_LumberMutateTable
		private static readonly OldMutateEntry[] m_OldLumberMutateTable = new OldMutateEntry[]
		{
			new OldMutateEntry( 70.0, 101, typeof(StonewoodLog), 1064389 ),	// 111% a lumber 70
			new OldMutateEntry( 75.0, 75, typeof(SwampLog)	, 1064392 ),	// 25% a lumber 100
			new OldMutateEntry( 80.0, 55, typeof(SilverLog)	, 1064390 ),	// 30% a lumber 100
			new OldMutateEntry( 85.0, 35, typeof(BloodLog)	, 1064391 ),	// 25% a lumber 100
			new OldMutateEntry( 90.0, 20, typeof(CrystalLog)	, 1064393 ),	// 25% a lumber 100
			new OldMutateEntry( 100.0, 10, typeof(EnchantedLog), 1064396 ),	// 20% a lumber 100
		};

		private static readonly MutateEntry[] m_LumberMutateTable = new MutateEntry[]
		{
			new MutateEntry( 70.0, 60.0, 60.09, typeof(StonewoodLog)	, 1064389 ),	// 111% a lumber 70
			new MutateEntry( 70.0, 60.0, 61.33, typeof(SilverLog)	, 1064390 ),	// 30% a lumber 100
			new MutateEntry( 75.0, 65.0, 66.40, typeof(BloodLog)	, 1064391 ),	// 25% a lumber 100
			new MutateEntry( 75.0, 65.0, 66.40, typeof(SwampLog)	, 1064392 ),	// 25% a lumber 100
			new MutateEntry( 80.0, 70.0, 71.20, typeof(CrystalLog)	, 1064393 ),	// 25% a lumber 100
			new MutateEntry( 80.0, 70.0, 71.20, typeof(ElvenLog)	, 1064394 ),	// 25% a lumber 100
			new MutateEntry( 85.0, 75.0, 76.25, typeof(ElderLog)	, 1064395 ),	// 20% a lumber 100
			new MutateEntry( 85.0, 75.0, 76.25, typeof(EnchantedLog)	, 1064396 ),	// 20% a lumber 100
			new MutateEntry( 90.0, 80.0, 81.33, typeof(DeathLog)	, 1064397 ),	// 15% a lumber 100
		};
		#endregion

		public HarvestVein GetHarvestVeinFromTileId( HarvestDefinition def, int tileId )
		{
#if DebugGetHarvestVeinFromTileId
			Console.WriteLine( "\nDebugGetHarvestVeinFromTileId" );
			Console.WriteLine( "\tTileId {0}", tileId );
#endif
			HarvestVein[] veins = def.Veins;

			if( Array.LastIndexOf( ( m_DeadTreesTable ), tileId ) > -1 )
			{
				return veins[ 12 ];
			}

			for( int i = 0; i < m_TreesTable.GetLength( 0 ); i++ )
			{
				if( Array.LastIndexOf( ( m_TreesTable[ i ] ), tileId ) > -1 )
				{
#if DebugGetHarvestVeinFromTileId
					Console.WriteLine( "\tTileId {0} - veinsId {1}", tileId, i );
#endif
					return veins[ i ];
				}
			}

			return veins[ 0 ];
		}

		public HarvestVein GetVeinAt( HarvestDefinition def, Map map, int x, int y, int z, int tileID )
		{
#if DebugGetVeinAt
			Console.WriteLine( "\nDebugGetVeinAt" );
			Console.WriteLine( "\tTileID {0}", tileID );
#endif
			HarvestVein resource;

			if( tileID == 0 )
			{
				Tile[] tiles = map.Tiles.GetStaticTiles( x, y );

				for( int i = 0; i < tiles.Length; ++i )
				{
#if DebugGetVeinAt
					Console.WriteLine( "\t{0}", tiles[ i ].ID );
#endif
					resource = GetHarvestVeinFromTileId( def, tiles[ i ].ID );
					if( resource != null )
					{
#if DebugGetVeinAt
						Console.WriteLine( "\t{0}", resource );
#endif
						return resource;
					}
				}
			}
			else
			{
				resource = GetHarvestVeinFromTileId( def, tileID );
				if( resource != null )
					return resource;
			}
			return null;
		}

		public static readonly bool PerfectGemsEnabled = false;

		public override Item SpecialContruct( Type type, Mobile from )
		{
			Item item;

			if( from.AccessLevel == AccessLevel.Developer )
				from.SendMessage( "Debug lumber rewards: type {0}", type.Name );

			if( type == typeof( TreasureMap ) )
				item = new TreasureMap( Utility.RandomMinMax( 3, 5 ), Map.Felucca );
			else if( type == typeof( BarkFragment ) )
				item = PerfectGemsEnabled ? new BarkFragment() : Loot.RandomGem();
			else if( type == typeof( LuminescentFungi ) )
				item = PerfectGemsEnabled ? new LuminescentFungi() : Loot.RandomGem();
			else if( type == typeof( SwitchItem ) )
				item = PerfectGemsEnabled ? new SwitchItem() : Loot.RandomGem();
			else if( type == typeof( ParasiticPlant ) )
				item = PerfectGemsEnabled ? new ParasiticPlant() : Loot.RandomGem();
			else if( type == typeof( BrilliantAmber ) )
				item = PerfectGemsEnabled ? new BrilliantAmber() : Loot.RandomGem();
			else
				return null;

			if( Core.AOS && ( item is BaseArmor || item is BaseJewel ) )
				XmlForceIdentify.DoMask( item );

			return item;
		}
		#endregion
	}
}