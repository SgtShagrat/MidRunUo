// #define DebugMiningRewards
#define DebugGetVeinAt
#define DebugMutateType
#define DebugGetHarvestVeinFromTileId

using System;
using System.Collections;

using Midgard.Engines.StoneEnchantSystem;

using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Targeting;

using Midgard;
using Midgard.Engines.Harvesting;
using Midgard.Items;

namespace Server.Engines.Harvest
{
	public class Mining : HarvestSystem
	{
		private static Mining m_System;

		public static Mining System
		{
			get
			{
				if ( m_System == null )
					m_System = new Mining();

				return m_System;
			}
		}

		private HarvestDefinition m_OreAndStone, m_Sand;

		public HarvestDefinition OreAndStone
		{
			get{ return m_OreAndStone; }
		}

		public HarvestDefinition Sand
		{
			get{ return m_Sand; }
		}

		private Mining()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region Mining for ore and stone
			HarvestDefinition oreAndStone = m_OreAndStone = new HarvestDefinition();

			// Resource banks are every 8x8 tiles
			oreAndStone.BankWidth = 2;  // 8 // mod by Dies Irae
			oreAndStone.BankHeight = 2; // 8 // mod by Dies Irae

			// Every bank holds from 10 to 34 ore
			#region modifica by Dies Irae
			oreAndStone.MinTotal = 27; // 10; // modifica by Dies Irae
			oreAndStone.MaxTotal = 50; //90; // 34; // modifica by Dies Irae
			#endregion

			// A resource bank will respawn its content every 10 to 20 minutes
			oreAndStone.MinRespawn = TimeSpan.FromMinutes( 20.0 ); // TimeSpan.FromMinutes( 10.0 ); // modifica by Dies Irae
			oreAndStone.MaxRespawn = TimeSpan.FromMinutes( 35.0 ); // TimeSpan.FromMinutes( 20.0 ); // modifica by Dies Irae

			// Skill checking is done on the Mining skill
			oreAndStone.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
			oreAndStone.Tiles = m_MountainAndCaveTiles;

			// Players must be within 2 tiles to harvest
			oreAndStone.MaxRange = 2;

			// One ore per harvest action
			oreAndStone.ConsumedPerHarvest = 1;
			oreAndStone.ConsumedPerFeluccaHarvest = 2;

			// The digging effect
			oreAndStone.EffectActions = new int[]{ 11 };
			oreAndStone.EffectSounds = new int[]{ 0x125, 0x126 };
			oreAndStone.EffectCounts = new int[]{ 1, 2, 2, 2, 3 }; // new int[]{ 1 }; // mod by Dies Irae
			oreAndStone.EffectDelay = TimeSpan.FromSeconds( 1.6 );
			oreAndStone.EffectSoundDelay = TimeSpan.FromSeconds( 0.9 );

			oreAndStone.NoResourcesMessage = 503040; // There is no metal here to mine.
			oreAndStone.DoubleHarvestMessage = 503042; // Someone has gotten to the metal before you.
			oreAndStone.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
			oreAndStone.OutOfRangeMessage = 500446; // That is too far away.
			oreAndStone.FailMessage = 503043; // You loosen some rocks but fail to find any useable ore.
			oreAndStone.PackFullMessage = "Your backpack is full, so the ore you mined is lost."; // mod by Dies Irae per una frase sbagliata nel cliloc.ita
			oreAndStone.ToolBrokeMessage = 1044038; // You have worn out your tool!

			#region AOS
			if( Core.AOS )
			{
				res = new HarvestResource[]
					{
						#region modifica by Dies Irae
						/*
						new HarvestResource( 00.0, 00.0, 100.0, 1007072, typeof( IronOre ),			typeof( Granite ) ),
						new HarvestResource( 65.0, 25.0, 105.0, 1007073, typeof( DullCopperOre ),	typeof( DullCopperGranite ),	typeof( DullCopperElemental ) ),
						new HarvestResource( 70.0, 30.0, 110.0, 1007074, typeof( ShadowIronOre ),	typeof( ShadowIronGranite ),	typeof( ShadowIronElemental ) ),
						new HarvestResource( 75.0, 35.0, 115.0, 1007075, typeof( CopperOre ),		typeof( CopperGranite ),		typeof( CopperElemental ) ),
						new HarvestResource( 80.0, 40.0, 120.0, 1007076, typeof( BronzeOre ),		typeof( BronzeGranite ),		typeof( BronzeElemental ) ),
						new HarvestResource( 85.0, 45.0, 125.0, 1007077, typeof( GoldOre ),			typeof( GoldGranite ),			typeof( GoldenElemental ) ),
						new HarvestResource( 90.0, 50.0, 130.0, 1007078, typeof( AgapiteOre ),		typeof( AgapiteGranite ),		typeof( AgapiteElemental ) ),
						new HarvestResource( 95.0, 55.0, 135.0, 1007079, typeof( VeriteOre ),		typeof( VeriteGranite ),		typeof( VeriteElemental ) ),
						new HarvestResource( 99.0, 59.0, 139.0, 1007080, typeof( ValoriteOre ),		typeof( ValoriteGranite ),		typeof( ValoriteElemental ) )
						*/

						new HarvestResource( 00.0, 00.0, 100.0, 1007072, 	typeof( IronOre ),			typeof( Granite ) ),
						new HarvestResource( 55.6, 25.0, 103.0, 1007073, 	typeof( DullCopperOre ),	typeof( DullCopperGranite ),		typeof( DullCopperElemental ) ),
						new HarvestResource( 58.4, 28.0, 105.0, 1007074, 	typeof( ShadowIronOre ),	typeof( ShadowIronGranite ),		typeof( ShadowIronElemental ) ),
						new HarvestResource( 62.9, 32.0, 108.0, 1007075, 	typeof( CopperOre ),		typeof( CopperGranite ),			typeof( CopperElemental ) ),
						new HarvestResource( 64.2, 35.0, 112.0, 1007076, 	typeof( BronzeOre ),		typeof( BronzeGranite ),			typeof( BronzeElemental ) ),
						new HarvestResource( 66.7, 37.0, 116.0, 1007077, 	typeof( GoldOre ),			typeof( GoldGranite ),				typeof( GoldenElemental ) ),
						new HarvestResource( 69.5, 39.0, 118.0, 1007078, 	typeof( AgapiteOre ),		typeof( AgapiteGranite ),			typeof( AgapiteElemental ) ),
						new HarvestResource( 71.4, 42.0, 121.0, 1007079, 	typeof( VeriteOre ),		typeof( VeriteGranite ),			typeof( VeriteElemental ) ),
						new HarvestResource( 74.0, 46.0, 126.0, 1007080, 	typeof( ValoriteOre ),		typeof( ValoriteGranite ),			typeof( ValoriteElemental ) ),
						new HarvestResource( 77.1, 48.0, 129.0, 1064880,	typeof( PlatinumOre ),		typeof( PlatinumGranite ),			typeof( PlatinumElemental ) ),
						new HarvestResource( 79.3, 51.0, 132.0, 1064881, 	typeof( TitaniumOre ),		typeof( TitaniumGranite ),			typeof( TitaniumElemental ) ),
						new HarvestResource( 82.5, 53.0, 133.0, 1064882, 	typeof( ObsidianOre ),		typeof( ObsidianGranite ),			typeof( ObsidianElemental ) ),
						new HarvestResource( 85.6, 57.0, 136.0, 1064883, 	typeof( DarkRubyOre ),		typeof( DarkRubyGranite ),			typeof( DarkRubyElemental ) ),
						new HarvestResource( 87.3, 60.0, 139.0, 1064884, 	typeof( EbonSapphireOre ),	typeof( EbonSapphireGranite ),		typeof( EbonSapphireElemental ) ),
						new HarvestResource( 90.4, 62.0, 142.0, 1064885, 	typeof( RadiantDiamondOre ),typeof( RadiantDiamondGranite ),	typeof( RadiantDiamondElemental ) ),
						new HarvestResource( 91.0, 63.0, 143.0, 1064886, 	typeof( EldarOre ),			typeof( EldarGranite ),				typeof( EldarElemental ) ),
						new HarvestResource( 91.0, 63.0, 143.0, 1064887, 	typeof( CrystalineOre ),	typeof( CrystalineGranite ),		typeof( CrystalineElemental ) ),
						new HarvestResource( 92.0, 63.0, 143.0, 1064888, 	typeof( VulcanOre ),		typeof( VulcanGranite ),			typeof( VulcanElemental ) ),
						new HarvestResource( 93.0, 64.0, 144.0, 1064889, 	typeof( AquaOre ),			typeof( AquaGranite ),				typeof( AquaElemental ) ),
	//					new HarvestResource( 90.0, 65.0, 120.0, 1064890,	typeof( SulfurousAsh ),		typeof( Granite ),					typeof( FireElemental ) )
						#endregion				
					};
			}
			#endregion
			else
			{
				res = new HarvestResource[]
				{
					new HarvestResource( 00.0, 00.0, 100.0, "You dig some iron ore and put it in your backpack.",		    typeof( IronOre ),			typeof( Granite ) ),
					new HarvestResource( 60.0, 35.0, 105.0, "You dig some dull copper ore and put it in your backpack.",	typeof( DullCopperOre ),	typeof( DullCopperGranite ),	typeof( DullCopperElemental ) ),
					new HarvestResource( 62.0, 37.0, 110.0, "You dig some shadow iron ore and put it in your backpack.",	typeof( ShadowIronOre ),	typeof( ShadowIronGranite ),	typeof( ShadowIronElemental ) ),
					new HarvestResource( 64.0, 39.0, 120.0, "You dig some bronze ore and put it in your backpack.",		    typeof( BronzeOre ),		typeof( BronzeGranite ),		typeof( BronzeElemental ) ),
					new HarvestResource( 66.0, 41.0, 115.0, "You dig some copper ore and put it in your backpack.",		    typeof( CopperOre ),		typeof( CopperGranite ),		typeof( CopperElemental ) ),					
					new HarvestResource( 68.0, 43.0, 130.0, "You dig some agapite ore and put it in your backpack.",		typeof( AgapiteOre ),		typeof( AgapiteGranite ),		typeof( AgapiteElemental ) ),
					new HarvestResource( 70.0, 45.0, 139.0, "You dig some graphite ore and put it in your backpack.",	    typeof( GraphiteOre ),		typeof( GraphiteGranite ),		typeof( GraphiteElemental ) ),
					new HarvestResource( 72.0, 47.0, 135.0, "You dig some verite ore and put it in your backpack.",		    typeof( VeriteOre ),		typeof( VeriteGranite ),		typeof( VeriteElemental ) ),
					new HarvestResource( 74.0, 49.0, 139.0, "You dig some valorite ore and put it in your backpack.",	    typeof( ValoriteOre ),		typeof( ValoriteGranite ),		typeof( ValoriteElemental ) ),

					new HarvestResource( 76.0, 51.0, 139.0, "You dig some pyrite ore and put it in your backpack.",		    typeof( PyriteOre ),		typeof( PyriteGranite ),		typeof( PyriteElemental ) ),
					new HarvestResource( 78.0, 53.0, 139.0, "You dig some azurite ore and put it in your backpack.",		typeof( AzuriteOre ),		typeof( AzuriteGranite ),		typeof( AzuriteElemental ) ),
					new HarvestResource( 80.0, 55.0, 139.0, "You dig some vanadium ore and put it in your backpack.",	    typeof( VanadiumOre ),		typeof( VanadiumGranite ),		typeof( VanadiumElemental ) ),
					new HarvestResource( 82.0, 57.0, 139.0, "You dig some silver ore and put it in your backpack.",		    typeof( SilverOre ),		typeof( SilverGranite ),		typeof( SilverElemental ) ),
					new HarvestResource( 84.0, 59.0, 139.0, "You dig some platinum ore and put it in your backpack.",	    typeof( PlatinumOre ),		typeof( PlatinumGranite ),		typeof( PlatinumElemental ) ),
					new HarvestResource( 87.0, 62.0, 139.0, "You dig some amethyst ore and put it in your backpack.",	    typeof( AmethystOre ),		typeof( AmethystGranite ),		typeof( AmethystElemental ) ),
					new HarvestResource( 90.0, 65.0, 139.0, "You dig some titanium ore and put it in your backpack.",	    typeof( TitaniumOre ),		typeof( TitaniumGranite ),		typeof( TitaniumElemental ) ),

					new HarvestResource( 93.0, 68.0, 125.0, "You dig some gold ore and put it in your backpack.",		    typeof( GoldOre ),			typeof( GoldGranite ),			typeof( GoldenElemental ) ),
					
					new HarvestResource( 96.0, 71.0, 139.0, "You dig some xenian ore and put it in your backpack.",		    typeof( XenianOre ),		typeof( XenianGranite ),		typeof( XenianElemental ) ),
					new HarvestResource( 99.0, 74.0, 139.0, "You dig some rubidian ore and put it in your backpack.",	    typeof( RubidianOre ),		typeof( RubidianGranite ),		typeof( RubidianElemental ) ),
					new HarvestResource( 99.9, 74.9, 139.0, "You dig some obsidian ore and put it in your backpack.",	    typeof( ObsidianOre ),		typeof( ObsidianGranite ),		typeof( ObsidianElemental ) ),
					new HarvestResource( 99.9, 79.9, 139.0, "You dig some ebon sapphire ore and put it in your backpack.",  typeof( EbonSapphireOre ),	typeof( EbonSapphireGranite ),	typeof( EbonSapphireElemental ) ),
					new HarvestResource( 99.9, 84.9, 139.0, "You dig some dark rubidian ore and put it in your backpack.",  typeof( DarkRubyOre ),		typeof( DarkRubyGranite ),		typeof( DarkRubyElemental ) ),
					new HarvestResource( 99.9, 89.9, 139.0, "You dig some radiant diamond ore and put it in your backpack.",typeof( RadiantDiamondOre ),typeof( RadiantDiamondGranite ),typeof( RadiantDiamondElemental ) ),
					
					new HarvestResource( 00.0, 00.0, 100.0, "You dig some coil and put it in your backpack.",		        typeof( Coil ),			    typeof( Granite ),				typeof( FireElemental ) )
				};
			}

			#region AOS
			if( Core.AOS )
			{
				veins = new HarvestVein[]
				{
					#region modifica by Dies Irae
					/*
					new HarvestVein( 49.6, 0.0, res[0], null   ), // Iron
					new HarvestVein( 11.2, 0.5, res[1], res[0] ), // Dull Copper
					new HarvestVein( 09.8, 0.5, res[2], res[0] ), // Shadow Iron
					new HarvestVein( 08.4, 0.5, res[3], res[0] ), // Copper
					new HarvestVein( 07.0, 0.5, res[4], res[0] ), // Bronze
					new HarvestVein( 05.6, 0.5, res[5], res[0] ), // Gold
					new HarvestVein( 04.2, 0.5, res[6], res[0] ), // Agapite
					new HarvestVein( 02.8, 0.5, res[7], res[0] ), // Verite
					new HarvestVein( 01.4, 0.5, res[8], res[0] )  // Valorite
					*/

					// 79.5%
					new HarvestVein( 12.0, 0.0,  res[0], null   ), 	// Iron
					new HarvestVein( 11.0, 0.15, res[1], res[0] ), 	// Dull Copper
					new HarvestVein( 10.0, 0.15, res[2], res[0] ), 	// Shadow Iron
					new HarvestVein( 09.0, 0.15, res[3], res[0] ), 	// Copper
					new HarvestVein( 08.5, 0.15, res[4], res[0] ), 	// Bronze
					new HarvestVein( 08.0, 0.18, res[5], res[0] ), 	// Gold
					new HarvestVein( 07.5, 0.18, res[6], res[0] ), 	// Agapite
					new HarvestVein( 07.0, 0.22, res[7], res[0] ), 	// Verite
					new HarvestVein( 06.5, 0.22, res[8], res[0] ), 	// Valorite

					// 20.5%
					new HarvestVein( 4.50, 0.23, res[9], res[0] ), 	// Platinum
					new HarvestVein( 3.50, 0.23, res[10], res[0] ), // Titanium
					new HarvestVein( 3.00, 0.23, res[11], res[0] ), // Obsidian
					new HarvestVein( 2.50, 0.23, res[12], res[0] ), // DarkRuby
					new HarvestVein( 2.00, 0.23, res[13], res[0] ), // EbonSapphire
					new HarvestVein( 1.50, 0.23, res[14], res[0] ), // RadiantDiamond
					new HarvestVein( 1.25, 0.23, res[15], res[0] ), // Eldar
					new HarvestVein( 1.00, 0.23, res[16], res[0] ), // Crystaline
					new HarvestVein( 0.75, 0.23, res[17], res[0] ), // Vulcan
					new HarvestVein( 0.50, 0.23, res[18], res[0] ), // Aqua

					// 4.5
					// new HarvestVein( 00.0, 0.30, res[19], res[0] ) // Zolfo
					#endregion
				};
			}
			#endregion
			else
			{
				veins = new HarvestVein[]
				{//edit by arlas tolta chance di Callback, 0.5 su tutti tranne iron e coil
					new HarvestVein( 49.6, 0.0, res[0], null   ), // Iron
					new HarvestVein( 11.2, 0.2, res[1], res[0] ), // Dull Copper
					new HarvestVein( 09.8, 0.2, res[2], res[0] ), // Shadow Iron
					new HarvestVein( 08.4, 0.2, res[3], res[0] ), // Copper
					new HarvestVein( 07.0, 0.2, res[4], res[0] ), // Bronze
					new HarvestVein( 05.6, 0.2, res[5], res[0] ), // Gold
					new HarvestVein( 04.2, 0.2, res[6], res[0] ), // Agapite
					new HarvestVein( 02.8, 0.2, res[7], res[0] ), // Verite
					new HarvestVein( 01.4, 0.2, res[8], res[0] ), // Valorite

					new HarvestVein( 00.0, 0.2, res[9], res[0] ), // Graphite 
					new HarvestVein( 00.0, 0.2, res[10], res[0] ),// Pyrite
					new HarvestVein( 00.0, 0.2, res[11], res[0] ),// Azurite
					new HarvestVein( 00.0, 0.2, res[12], res[0] ),// Vanadium

					new HarvestVein( 00.0, 0.2, res[13], res[0] ),// Silver
					new HarvestVein( 00.0, 0.2, res[14], res[0] ),// Platinum
					new HarvestVein( 00.0, 0.2, res[15], res[0] ),// Amethyst
					new HarvestVein( 00.0, 0.2, res[16], res[0] ),// Titanium
						
					new HarvestVein( 00.0, 0.2, res[17], res[0] ),// Xenian
					new HarvestVein( 00.0, 0.2, res[18], res[0] ),// Rubidian
					new HarvestVein( 00.0, 0.2, res[19], res[0] ),// Obsidian
					new HarvestVein( 00.0, 0.2, res[20], res[0] ),// Ebon Twilight Sapphire
						
					new HarvestVein( 00.0, 0.2, res[21], res[0] ),// Dark Sable Ruby
					new HarvestVein( 00.0, 0.2, res[22], res[0] ),// Radiant Nimbus Diamond

					new HarvestVein( 00.0, 0.0, res[23], res[0] ),// Coil
				};
			}

			oreAndStone.Resources = res;
			oreAndStone.Veins = veins;

			//if ( Core.ML )
			//{
				oreAndStone.BonusResources = new BonusHarvestResource[]
				{
					#region modifica by Dies Irae
					/*
					new BonusHarvestResource( 0, 99.8998, null, null ),	//Nothing	//Note: Rounded the below to .0167 instead of 1/6th of a %.  Close enough
					new BonusHarvestResource( 100, .0167, 1072562, typeof( BlueDiamond ) ),
					new BonusHarvestResource( 100, .0167, 1072567, typeof( DarkSapphire ) ),
					new BonusHarvestResource( 100, .0167, 1072570, typeof( EcruCitrine ) ),
					new BonusHarvestResource( 100, .0167, 1072564, typeof( FireRuby ) ),
					new BonusHarvestResource( 100, .0167, 1072566, typeof( PerfectEmerald ) ),
					new BonusHarvestResource( 100, .0167, 1072568, typeof( Turquoise ) )
					*/

					new BonusHarvestResource( 0.00, 99.50, null, null ),

					new BonusHarvestResource( 70.0, 0.010, 1064377, typeof( AncientSmithyHammer ) ),
					new BonusHarvestResource( 70.0, 0.010, 1064378, typeof( TreasureMap ) ),
					new BonusHarvestResource( 70.0, 0.010, 1064378, typeof( ArcaneGem ) ),

					new BonusHarvestResource( 70.0, 0.003, 1064380, typeof( BaseWeapon ) ),
					new BonusHarvestResource( 70.0, 0.003, 1064380, typeof( BaseArmor ) ),
					new BonusHarvestResource( 70.0, 0.003, 1064380, typeof( BaseJewel ) ),

					new BonusHarvestResource( 50.0, 0.100, 1064381, typeof( IGem ) ),
					new BonusHarvestResource( 30.0, 0.291, 1064382, typeof( Lantern ) )
					#endregion
				};
			//}

			oreAndStone.RaceBonus = Core.ML;
			oreAndStone.RandomizeVeins = true; // Core.ML;

			Definitions.Add( oreAndStone );
			#endregion

			#region Mining for sand
			HarvestDefinition sand = m_Sand = new HarvestDefinition();

			// Resource banks are every 8x8 tiles
			sand.BankWidth = 4;
			sand.BankHeight = 4;

			// Every bank holds from 6 to 12 sand
			sand.MinTotal = 6 * 4;
			sand.MaxTotal = 12 * 4;

			// A resource bank will respawn its content every 10 to 20 minutes
			sand.MinRespawn = TimeSpan.FromMinutes( 22.0 );
			sand.MaxRespawn = TimeSpan.FromMinutes( 37.0 );

			// Skill checking is done on the Mining skill
			sand.Skill = SkillName.Mining;

			// Set the list of harvestable tiles
			sand.Tiles = m_SandTiles;

			// Players must be within 2 tiles to harvest
			sand.MaxRange = 2;

			// One sand per harvest action
			sand.ConsumedPerHarvest = 1;
			sand.ConsumedPerFeluccaHarvest = 1;

			// The digging effect
			sand.EffectActions = new int[]{ 11 };
			sand.EffectSounds = new int[]{ 0x125, 0x126 };
			sand.EffectCounts = new int[]{ 1, 2, 2, 2, 3 };
			sand.EffectDelay = TimeSpan.FromSeconds( 1.6 );
			sand.EffectSoundDelay = TimeSpan.FromSeconds( 0.9 );

			sand.NoResourcesMessage = 1044629; // There is no sand here to mine.
			sand.DoubleHarvestMessage = 1044629; // There is no sand here to mine.
			sand.TimedOutOfRangeMessage = 503041; // You have moved too far away to continue mining.
			sand.OutOfRangeMessage = 500446; // That is too far away.
			sand.FailMessage = 1044630; // You dig for a while but fail to find any of sufficient quality for glassblowing.
			sand.PackFullMessage = 1044632; // Your backpack can't hold the sand, and it is lost!
			sand.ToolBrokeMessage = 1044038; // You have worn out your tool!

			res = new HarvestResource[]
			{
				new HarvestResource( 70.0, 45.0, 139.0, 1044631, typeof( Sand ) )
			};

			veins = new HarvestVein[]
			{
				new HarvestVein( 100.0, 0.0, res[0], null )
			};

			sand.BonusResources = new BonusHarvestResource[]
			{
				new BonusHarvestResource( 0.00, 99.50, null, null ),

				new BonusHarvestResource( 70.0, 0.0125, 1064378, typeof( TreasureMap ) ),
				new BonusHarvestResource( 70.0, 0.0125, 1064380, typeof( BaseWeapon ) ),
				new BonusHarvestResource( 70.0, 0.0125, 1064380, typeof( BaseArmor ) ),
				new BonusHarvestResource( 70.0, 0.0125, 1064380, typeof( BaseJewel ) ),

				new BonusHarvestResource( 50.0, 0.1000, 1064381, typeof( IGem ) ),
				new BonusHarvestResource( 30.0, 0.3000, 1064382, typeof( CommonShell ) )
			};

			sand.Resources = res;
			sand.Veins = veins;

			Definitions.Add( sand );
			#endregion
		}

		public override object GetLock( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			return this;
		}

		public override void OnConcurrentHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			from.SendMessage( "You are already mining." );
		}

		public override Type GetResourceType( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			#region Modifica by Dies Irae
			if( tool is StoneHarvester )
				return ( resource == m_OreAndStone.Resources[ 0 ]  ) ? typeof( RawStone ) : null;
			#endregion

			if ( def == m_OreAndStone )
			{
				PlayerMobile pm = from as PlayerMobile;
				if ( pm != null && pm.StoneMining && pm.ToggleMiningStone /* && from.Skills[SkillName.Mining].Base >= 100.0 */ && 0.1 > Utility.RandomDouble() )
					return resource.Types[1];

				return resource.Types[0];
			}

			return base.GetResourceType( from, tool, def, map, loc, resource );
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;

			#region modifica by Dies Irae
			if ( tool.Parent != from && !( tool is Shovel || tool is SturdyShovel || tool is StoneHarvester ) )
			{
				from.SendMessage( "The harvesting tool must be equipped for any serious mining!" );
				return false;
			}
			#endregion

			if ( from.Mounted )
			{
				from.SendLocalizedMessage( 501864 ); // You can't mine while riding.
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendLocalizedMessage( 501865 ); // You can't mine while polymorphed.
				return false;
			}

			return true;
		}

		#region mod by Dies Irae [harvest chance]
		public override void GetHarvestChancheMods( Mobile from, HarvestResource resource, double minSkill, ref double rawGainChance )
		{
			if( minSkill == 0 )
				rawGainChance = 0.9;
		}
		#endregion

		public override void SendSuccessTo( Mobile from, Item item, HarvestResource resource )
		{
			if ( item is BaseGranite )
				from.SendLocalizedMessage( 1044606 ); // You carefully extract some workable stone from the ore vein!
			#region mod by Dies Irae
			if( item is RawStone )
				from.SendMessage( "You extract some raw stone!" );
			#endregion
			else
				base.SendSuccessTo( from, item, resource );
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( def == m_Sand && !(from is PlayerMobile /* && from.Skills[SkillName.Mining].Base >= 100.0 */ && ((PlayerMobile)from).SandMining) )
			{
				OnBadHarvestTarget( from, tool, toHarvest );
				return false;
			}

			#region modifica by Dies Irae
			if( def == m_OreAndStone && tool.Parent != from && !( tool is StoneHarvester ) )
			{
				from.SendMessage( "The harvesting tool must be equipped for any serious mining!" );
				return false;
			}

			if ( tool.Parent != from && !(tool is Shovel || tool is SturdyShovel || tool is StoneHarvester ) )
			{
				from.SendMessage( "The harvesting tool must be equipped for any serious mining!" );
				return false;
			}
			#endregion

			if ( from.Mounted )
			{
				from.SendLocalizedMessage( 501864 ); // You can't mine while riding.
				return false;
			}
			else if ( from.IsBodyMod && !from.Body.IsHuman )
			{
				from.SendLocalizedMessage( 501865 ); // You can't mine while polymorphed.
				return false;
			}

			return true;
		}

        public override HarvestResource MutateResource( Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestVein vein, HarvestResource primary, HarvestResource fallback )
        {
			double skillValue = from.Skills[def.Skill].Value;

            if( from.PlayerDebug )
                from.LocalOverheadMessage( MessageType.Emote, 0x59, true, string.Format( "Vena di {0}", primary ) );

            if( fallback == null ) // iron
                vein.FoundVein = true;

            if( !vein.FoundVein && ( skillValue >= primary.ReqSkill && skillValue >= primary.MinSkill ) && vein.ChanceToFallback > Utility.RandomDouble() )
            {
                from.SendMessage( "You discovered {0} ores mine!", StringUtility.AddArticle( primary.ToString().ToLower() ) );
                vein.FoundVein = true;
            }

            return ( vein.FoundVein ? primary : fallback );
        }

		public override HarvestVein MutateVein( Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein )
		{
			if ( tool is GargoylesPickaxe && def == m_OreAndStone )
			{
				int veinIndex = Array.IndexOf( def.Veins, vein );

				if ( veinIndex >= 0 && veinIndex < (def.Veins.Length - 1) )
					return def.Veins[veinIndex + 1];
			}

			return base.MutateVein( from, tool, def, bank, toHarvest, vein );
		}

		private static int[] m_Offsets = new int[]
		{
			-1, -1,
			-1,  0,
			-1,  1,
			 0, -1,
			 0,  1,
			 1, -1,
			 1,  0,
			 1,  1
		};

		public override void OnHarvestFinished( Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested )
		{
			if ( tool is GargoylesPickaxe && def == m_OreAndStone && 0.1 > Utility.RandomDouble() )
			{
				HarvestResource res = vein.PrimaryResource;

				if ( res == resource && res.Types.Length >= 3 )
				{
					try
					{
						Map map = from.Map;

						if ( map == null )
							return;

						BaseCreature spawned = Activator.CreateInstance( res.Types[2], new object[]{ 25 } ) as BaseCreature;

						if ( spawned != null )
						{
							int offset = Utility.Random( 8 ) * 2;

							for ( int i = 0; i < m_Offsets.Length; i += 2 )
							{
								int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
								int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

								if ( map.CanSpawnMobile( x, y, from.Z ) )
								{
									spawned.OnBeforeSpawn( new Point3D( x, y, from.Z ), map );
									spawned.MoveToWorld( new Point3D( x, y, from.Z ), map );
									spawned.Combatant = from;
									return;
								}
								else
								{
									int z = map.GetAverageZ( x, y );

									if ( map.CanSpawnMobile( x, y, z ) )
									{
										spawned.OnBeforeSpawn( new Point3D( x, y, z ), map );
										spawned.MoveToWorld( new Point3D( x, y, z ), map );
										spawned.Combatant = from;
										return;
									}
								}
							}

							spawned.OnBeforeSpawn( from.Location, from.Map );
							spawned.MoveToWorld( from.Location, from.Map );
							spawned.Combatant = from;
						}
					}
					catch
					{
					}
				}
			}
			#region mod by Dies Irae
			else if( !Core.AOS )
				SpawnCritter( from, tool, def, vein, bank, resource, harvested );
			#endregion
		}

		public override bool BeginHarvesting( Mobile from, Item tool )
		{
			if ( !base.BeginHarvesting( from, tool ) )
				return false;

			from.SendLocalizedMessage( 503033 ); // Where do you wish to dig?
			return true;
		}

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );

			//if ( Core.ML )
				from.RevealingAction( true ); // mod by Arlas

			AutoMacroCheck( from ); // mod by Dies Irae
		}

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			if ( toHarvest is LandTarget )
				from.SendLocalizedMessage( 501862 ); // You can't mine there.
			else
				from.SendLocalizedMessage( 501863 ); // You can't mine that.
		}

		#region Tile lists
		private static int[] m_MountainAndCaveTiles = new int[]
			{
				220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
				230, 231, 236, 237, 238, 239, 240, 241, 242, 243,
				244, 245, 246, 247, 252, 253, 254, 255, 256, 257,
				258, 259, 260, 261, 262, 263, 268, 269, 270, 271,
				272, 273, 274, 275, 276, 277, 278, 279, 286, 287,
				288, 289, 290, 291, 292, 293, 294, 296, 296, 297,
				321, 322, 323, 324, 467, 468, 469, 470, 471, 472,
				473, 474, 476, 477, 478, 479, 480, 481, 482, 483,
				484, 485, 486, 487, 492, 493, 494, 495, 543, 544,
				545, 546, 547, 548, 549, 550, 551, 552, 553, 554,
				555, 556, 557, 558, 559, 560, 561, 562, 563, 564,
				565, 566, 567, 568, 569, 570, 571, 572, 573, 574,
				575, 576, 577, 578, 579, 581, 582, 583, 584, 585,
				586, 587, 588, 589, 590, 591, 592, 593, 594, 595,
				596, 597, 598, 599, 600, 601, 610, 611, 612, 613,

				1010, 1741, 1742, 1743, 1744, 1745, 1746, 1747, 1748, 1749,
				1750, 1751, 1752, 1753, 1754, 1755, 1756, 1757, 1771, 1772,
				1773, 1774, 1775, 1776, 1777, 1778, 1779, 1780, 1781, 1782,
				1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1801, 1802,
				1803, 1804, 1805, 1806, 1807, 1808, 1809, 1811, 1812, 1813,
				1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823,
				1824, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839,
				1840, 1841, 1842, 1843, 1844, 1845, 1846, 1847, 1848, 1849,
				1850, 1851, 1852, 1853, 1854, 1861, 1862, 1863, 1864, 1865,
				1866, 1867, 1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875,
				1876, 1877, 1878, 1879, 1880, 1881, 1882, 1883, 1884, 1981,
				1982, 1983, 1984, 1985, 1986, 1987, 1988, 1989, 1990, 1991,
				1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001,
				2002, 2003, 2004, 2028, 2029, 2030, 2031, 2032, 2033, 2100,
				2101, 2102, 2103, 2104, 2105,

				0x453B, 0x453C, 0x453D, 0x453E, 0x453F, 0x4540, 0x4541,
				0x4542, 0x4543, 0x4544,	0x4545, 0x4546, 0x4547, 0x4548,
				0x4549, 0x454A, 0x454B, 0x454C, 0x454D, 0x454E,	0x454F
			};

		private static int[] m_SandTiles = new int[]
			{
				22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
				32, 33, 34, 35, 36, 37, 38, 39, 40, 41,
				42, 43, 44, 45, 46, 47, 48, 49, 50, 51,
				52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
				62, 68, 69, 70, 71, 72, 73, 74, 75,

				286, 287, 288, 289, 290, 291, 292, 293, 294, 295,
				296, 297, 298, 299, 300, 301, 402, 424, 425, 426,
				427, 441, 442, 443, 444, 445, 446, 447, 448, 449,
				450, 451, 452, 453, 454, 455, 456, 457, 458, 459,
				460, 461, 462, 463, 464, 465, 642, 643, 644, 645,
				650, 651, 652, 653, 654, 655, 656, 657, 821, 822,
				823, 824, 825, 826, 827, 828, 833, 834, 835, 836,
				845, 846, 847, 848, 849, 850, 851, 852, 857, 858,
				859, 860, 951, 952, 953, 954, 955, 956, 957, 958,
				967, 968, 969, 970,

				1447, 1448, 1449, 1450, 1451, 1452, 1453, 1454, 1455,
				1456, 1457, 1458, 1611, 1612, 1613, 1614, 1615, 1616,
				1617, 1618, 1623, 1624, 1625, 1626, 1635, 1636, 1637,
				1638, 1639, 1640, 1641, 1642, 1647, 1648, 1649, 1650
			};
		#endregion

		public static readonly bool GuardedZoneBonusEnabled = false;

		public HarvestVein GetVeinAt( HarvestDefinition def, Map map, int x, int y, int z, int tileID )
		{
			return GetRegionalVein( def, map, x, y, z, tileID );

			/*edit by arlas
			double chance = Utility.RandomDouble() * 100.0;

			if( GuardedZoneBonusEnabled )
			{
				bool isGuarded = false;

				GuardedRegion reg = Region.Find( new Point3D( x * def.BankWidth, y * def.BankHeight, z ), map ) as GuardedRegion;

				if( reg != null && !reg.Disabled )
					isGuarded = true;

				// Differenziazione a seconda della protezione guardie
				if( isGuarded && Utility.RandomDouble() < 0.8 )
					chance = chance * 74;
				else
					chance = ( isGuarded ? chance * 96 : chance * 100 );

				chance = Math.Min( chance, 100.0 );
			}

			double rangeMin = 0;
			double rangeMax = 0;

			for( int i = 0; i < def.Veins.Length; ++i )
			{
				// Aggiorna il valore minimo allo step successivo
				if( i > 0 )
					rangeMin = rangeMax;

				// Aggiorna il valore massimo al valore attuale della vena
				rangeMax += def.Veins[ i ].VeinChance;

				if( ( chance < rangeMax ) && ( chance >= rangeMin ) )
					return def.Veins[ i ];
			}

			return null;
			*/
		}

		public HarvestVein GetRegionalVein( HarvestDefinition def, Map map, int x, int y, int z, int tileID )
		{
			RegionalHarvestDefinition definition = RegionalHarvestDefinition.GetRegHarDefByPoint( map, new Point3D( x, y, z ) );

			if( Core.Debug )
				Utility.Log( "MiningGetRegionalVein.log", "GetRegionalVein: {0}", definition.Name );

			if( definition.IsCoilRegion )
			{
				return def.Veins[ 23 ]; // Coil
			}

			HarvestVein[] veins = def.Veins;

			if( definition.GetChanchesLength != veins.Length )
			{
				Console.WriteLine( "Warning: not matching length for regional harvest definition: {0}", definition.Name );
				return def.Veins[ 0 ];
			}

			for( int i = veins.Length - 1; i > -1; i-- )
			{
				int chance = Utility.Random( 100 ) + 1;
				int regChance = definition.GetChanceByIndex( i );
				string name = veins[ i ].PrimaryResource.Types[ 0 ].Name;

				if( Core.Debug )
					Utility.Log( "MiningGetRegionalVein.log", "\t{0} - c:{1} - rc:{2}", name, chance.ToString(), regChance.ToString() );

				if( chance <= regChance )
				{
					if( Core.Debug )
						Utility.Log( "MiningGetRegionalVein.log", "\t\tFOUND:{0}", name );

					return veins[ i ];
				}
			}

			return veins[ 0 ];
		}

		private static readonly Type[] m_MiningCommonRewards = new Type[]
		{
			typeof( Lantern ),
			typeof( CandleSkull ),
			typeof( SturdyShovel ),
			typeof( SturdyPickaxe ),
		};

		public Item RandomMiningCommonReward()
		{
			return Loot.Construct( m_MiningCommonRewards );
		}

		public static readonly bool PerfectGemsEnabled = false;

		public override Item SpecialContruct( Type type, Mobile from )
		{
			Item item;

			if( from.AccessLevel == AccessLevel.Developer )
				from.SendMessage( "Debug mining rewards: type {0}", type.Name );

			if( type == typeof( AncientSmithyHammer ) )
			{
				int bonus = Utility.Random( 10 ) + 1;
				item = new AncientSmithyHammer( bonus, 50 - ( ( bonus - 1 ) / 3 ) * 10 );
			}
			else if( type == typeof( TreasureMap ) )
				item = new TreasureMap( Utility.RandomMinMax( 3, 5 ), Map.Felucca );
			else if( type == typeof( ArcaneGem ) )
				item = new ArcaneGem();
			else if( type == typeof( EnchantStone ) )
				item = new EnchantStone();
			else if( type == typeof( BaseWeapon ) )
			{
				item = Loot.RandomWeapon();
				Midgard.Engines.SecondAgeLoot.Magics.ApplyBonusTo( item );
			}
			else if( type == typeof( BaseArmor ) )
			{
				item = Loot.RandomArmor();
				Midgard.Engines.SecondAgeLoot.Magics.ApplyBonusTo( item );
			}
			else if( type == typeof( BaseJewel ) )
			{
				item = Loot.RandomJewelry();
				Midgard.Engines.SecondAgeLoot.Magics.ApplyBonusTo( item );
			}
			else if( type == typeof( IGem ) )
				item = Loot.RandomGem();
			else if( type == typeof( CommonShell ) )
				item = RandomShell( from );
			else
				item = RandomMiningCommonReward();

			return item;
		}

		private static Item RandomShell( Mobile from )
		{
			if( from.Skills[ SkillName.Fishing ].Value == 100.0 && Utility.Random( 20 ) == 10 )
			{
				return Loot.Construct( new Type[]
							  {
								  typeof (NautilusShell), typeof (ConchShell)
							  } );
			}
			else
				return new CommonShell();
		}

		public override Type GetRandomCritter()
		{
			return m_MiningSpawnTypes[ Utility.Random( m_MiningSpawnTypes.Length ) ];
		}

		private static readonly Type[] m_MiningSpawnTypes = new Type[]
		{
			typeof( Slime ),
			typeof( Slime ),
			typeof( Slime ),
			typeof( Mongbat ),
			typeof( Mongbat ),
			typeof( Mongbat ),
			typeof( Rat ),
			typeof( Rat ),
			typeof( Rat ),
		};

		public static double HitsBuff   = 2.00;
		public static double StrBuff	= 1.05;
		public static double IntBuff	= 1.20;
		public static double DexBuff	= 1.20;
		public static double SkillsBuff = 1.20;
		public static double SpeedBuff  = 1.20;
		public static double FameBuff   = 1.20;
		public static double KarmaBuff  = 1.20;
		public static double DamageBuff = 1.05;

		public static void BoostElemental( BaseCreature bc, CraftResource resource )
		{
			double scalar = DefBlacksmithy.CraftSystem.GetMaterialDifficulty( resource );

			Console.WriteLine( "Debug: booting elemental of {0} with scalar of {1:F2}.", resource, scalar );

			if( bc.HitsMaxSeed >= 0 )
				bc.HitsMaxSeed = (int)( bc.HitsMaxSeed * HitsBuff * scalar );

			bc.RawStr = (int)( bc.RawStr * StrBuff * scalar );
			bc.RawInt = (int)( bc.RawInt * IntBuff * scalar );
			bc.RawDex = (int)( bc.RawDex * DexBuff * scalar );

			bc.Hits = bc.HitsMax;
			bc.Mana = bc.ManaMax;
			bc.Stam = bc.StamMax;

			for( int i = 0; i < bc.Skills.Length; i++ )
			{
				Skill skill = bc.Skills[ i ];

				if( skill.Base > 0.0 )
					skill.Base *= SkillsBuff * scalar;
			}

			bc.PassiveSpeed /= ( SpeedBuff * scalar );
			bc.ActiveSpeed /= ( SpeedBuff * scalar );

			bc.DamageMin = (int)( bc.DamageMin * DamageBuff * scalar );
			bc.DamageMax = (int)( bc.DamageMax * DamageBuff * scalar );

			if( bc.Fame > 0 )
				bc.Fame = (int)( bc.Fame * FameBuff * scalar );

			if( bc.Fame > 32000 )
				bc.Fame = 32000;

			if( bc.Karma != 0 )
			{
				bc.Karma = (int)( bc.Karma * KarmaBuff * scalar );

				if( Math.Abs( bc.Karma ) > 32000 )
					bc.Karma = 32000 * Math.Sign( bc.Karma );
			}
		}
	}
}