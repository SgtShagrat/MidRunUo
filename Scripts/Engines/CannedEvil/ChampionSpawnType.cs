using System;
using Midgard.Mobiles;
using Server.Mobiles;
using Midgard.Engines.PlagueBeastLordPuzzle;

namespace Server.Engines.CannedEvil
{
	public enum ChampionSpawnType
	{
		Abyss,
		Arachnid,
		ColdBlood,
		ForestLord,
		VerminHorde,
		UnholyTerror,
		SleepingDragon,
		MidgardAdventure,
		#region mod by Dies Irae
		Corrupt,
        MoonglowCemetery,
        Hythloth,
        Labyrinth
		#endregion
	}

	public class ChampionSpawnInfo
	{
		private string m_Name;
		private Type m_Champion;
		private Type[][] m_SpawnTypes;
		private string[] m_LevelNames;

		public string Name { get { return m_Name; } }
		public Type Champion { get { return m_Champion; } }
		public Type[][] SpawnTypes { get { return m_SpawnTypes; } }
		public string[] LevelNames { get { return m_LevelNames; } }

		public ChampionSpawnInfo( string name, Type champion, string[] levelNames, Type[][] spawnTypes )
		{
			m_Name = name;
			m_Champion = champion;
			m_LevelNames = levelNames;
			m_SpawnTypes = spawnTypes;
		}

		public static ChampionSpawnInfo[] Table{ get { return m_Table; } }

		private static readonly ChampionSpawnInfo[] m_Table = new ChampionSpawnInfo[]
			{
				new ChampionSpawnInfo( "Abyss", typeof( Semidar ), new string[]{ "Foe", "Assassin", "Conqueror" }, new Type[][]	// Abyss
				{																											// Abyss
					new Type[]{ typeof( GreaterMongbat ), typeof( Imp ) },													// Level 1
					new Type[]{ typeof( Gargoyle ), typeof( Harpy ) },														// Level 2
					new Type[]{ typeof( FireGargoyle ), typeof( StoneGargoyle ) },											// Level 3
					new Type[]{ typeof( Daemon ), typeof( Succubus ) }														// Level 4
				} ),
				new ChampionSpawnInfo( "Arachnid", typeof( Mephitis ), new string[]{ "Bane", "Killer", "Vanquisher" }, new Type[][]	// Arachnid
				{																											// Arachnid
					new Type[]{ typeof( Scorpion ), typeof( GiantSpider ) },												// Level 1
					new Type[]{ typeof( TerathanDrone ), typeof( TerathanWarrior ) },										// Level 2
					new Type[]{ typeof( DreadSpider ), typeof( TerathanMatriarch ) },										// Level 3
					new Type[]{ typeof( PoisonElemental ), typeof( TerathanAvenger ) }										// Level 4
				} ),
				new ChampionSpawnInfo( "Cold Blood", typeof( Rikktor ), new string[]{ "Blight", "Slayer", "Destroyer" }, new Type[][]	// Cold Blood
				{																											// Cold Blood
					new Type[]{ typeof( Lizardman ), typeof( Snake ) },														// Level 1
					new Type[]{ typeof( LavaLizard ), typeof( OphidianWarrior ) },											// Level 2
					new Type[]{ typeof( Drake ), typeof( OphidianArchmage ) },												// Level 3
					new Type[]{ typeof( Dragon ), typeof( OphidianKnight ) }												// Level 4
				} ),
				new ChampionSpawnInfo( "Forest Lord", typeof( LordOaks ), new string[]{ "Enemy", "Curse", "Slaughterer" }, new Type[][]	// Forest Lord
				{																											// Forest Lord
					new Type[]{ typeof( Pixie ), typeof( ShadowWisp ) },													// Level 1
					new Type[]{ typeof( Kirin ), typeof( Wisp ) },															// Level 2
					new Type[]{ typeof( Centaur ), typeof( Unicorn ) },														// Level 3
					new Type[]{ typeof( EtherealWarrior ), typeof( SerpentineDragon ) }										// Level 4
				} ),
				new ChampionSpawnInfo( "Vermin Horde", typeof( Barracoon ), new string[]{ "Adversary", "Subjugator", "Eradicator" }, new Type[][]	// Vermin Horde
				{																											// Vermin Horde
					new Type[]{ typeof( GiantRat ), typeof( Slime ) },														// Level 1
					new Type[]{ typeof( DireWolf ), typeof( Ratman ) },														// Level 2
					new Type[]{ typeof( HellHound ), typeof( RatmanMage ) },												// Level 3
					new Type[]{ typeof( RatmanArcher ), typeof( SilverSerpent ) }											// Level 4
				} ),
				new ChampionSpawnInfo( "Unholy Terror", typeof( Neira ), new string[]{ "Scourge", "Punisher", "Nemesis" }, new Type[][]	// Unholy Terror
				{																											// Unholy Terror
					(Core.AOS ? 
					new Type[]{ typeof( Bogle ), typeof( Ghoul ), typeof( Shade ), typeof( Spectre ), typeof( Wraith ) }	// Level 1 (Pre-AoS)
					: new Type[]{ typeof( Ghoul ), typeof( Shade ), typeof( Spectre ), typeof( Wraith ) } ),				// Level 1

					new Type[]{ typeof( BoneMagi ), typeof( Mummy ), typeof( SkeletalMage ) },								// Level 2
					new Type[]{ typeof( BoneKnight ), typeof( Lich ), typeof( SkeletalKnight ) },							// Level 3
					new Type[]{ typeof( LichLord ), typeof( RottingCorpse ) }												// Level 4
				} ),
				new ChampionSpawnInfo( "Sleeping Dragon", typeof( Serado ), new string[]{ "Rival", "Challenger", "Antagonist" } , new Type[][]
				{																											// Unholy Terror
					new Type[]{ typeof( DeathwatchBeetleHatchling ), typeof( Lizardman ) },
					new Type[]{ typeof( DeathwatchBeetle ), typeof( Kappa ) },
					new Type[]{ typeof( LesserHiryu ), typeof( RevenantLion ) },
					new Type[]{ typeof( Hiryu ), typeof( Oni ) }
				} ),
				new ChampionSpawnInfo( "Midgard Adventure", typeof( MrAdventure ), new string[]{ "Challenger", "Conqueror" }, new Type[][]	// Abyss
				{																											// Abyss
					new Type[]{ typeof( Snake ), typeof( LavaSnake ), typeof( IceSnake ) },													// Level 1
					new Type[]{ typeof( GiantSerpent ), typeof( SilverSerpent ), typeof( LavaSerpent ), typeof( IceSerpent )},														// Level 2
					new Type[]{ typeof( PoisonElemental ), typeof( ToxicElemental ) , typeof( FireElemental ) , typeof( EarthElemental ) , typeof( BloodElemental ), typeof( AirElemental ) , typeof( IceElemental ) },											// Level 3
					new Type[]{ typeof( GargoyleDestroyer ), typeof( Succubus ),typeof( Daemon ) }														// Level 4
				} ),
				#region mod by Dies Irae
				new ChampionSpawnInfo( "Corrupt", typeof( Ilhenir ), new string[]{ "Cleanser", "Expunger" }, new Type[][]	// BedlamCrypt
				{																											// BedlamCrypt
				    new Type[]{ typeof( PlagueSpawn ), typeof( Bogling )},													// Level 1
					new Type[]{ typeof( PlagueBeast ), typeof( BogThing )},													// Level 2
					new Type[]{ typeof( PlagueBeastLord ), typeof( InterredGrizzle ) },										// Level 3
					new Type[]{ typeof( FetidEssence ), typeof( PestilentBandages ) }										// Level 4
				} ),
                
				new ChampionSpawnInfo( "Moonglow cemetery", typeof( LoxtirFeather ), new string[]{ "Challenger", "Conqueror" }, new Type[][]	    // Moonglow cemetery
				{																											    // Moonglow cemetery
				    new Type[]{ typeof( MoonglowGoreFiend ), typeof( MoonglowBogle ), typeof(MoonglowGhoul), typeof(MoonglowShade), 
                                typeof( MoonglowSpectre), typeof(MoonglowWraith), typeof(MoonglowWailingBanshee)   },		    // Level 1
					new Type[]{ typeof( MoonglowBoneMagi ), typeof( MoonglowMummy ), typeof( MoonglowSkeletalMage ), 
                                typeof( MoonglowBoneKnight )},													                // Level 2
					new Type[]{ typeof( MoonglowSkeletalPoisoner ), typeof( MoonglowLich ), typeof( MoonglowSkeletalKnight ) },	// Level 3
					new Type[]{ typeof( MoonglowLichLord ), typeof( MoonglowRottingCorpse ), typeof( MoonglowAncientLich ) }	// Level 4
				} ),

				new ChampionSpawnInfo( "Queen of air and darkness", typeof( HythlothQueenOfAirAndDarkness ), new string[]{ "Enemy", "Curse", "Slaughterer" }, new Type[][]	// Hythloth
				{																											// Forest Lord
					new Type[]{ typeof( HythlothPixie ), typeof( HythlothWisp ) },													// Level 1
					new Type[]{ typeof( HythlothKirin ), typeof( HythlothWisp ) },															// Level 2
					new Type[]{ typeof( HythlothCentaur ), typeof( HythlothUnicorn ) },														// Level 3
					new Type[]{ typeof( HythlothEtherealWarrior ), typeof( HythlothSerpentineDragon ) }										// Level 4
				} ),

				new ChampionSpawnInfo( "Tormented Minotaur", typeof( Meraktus ), new string[]{ "Enemy", "Curse", "Slaughterer" }, new Type[][]	// Labyrinth
				{
					new Type[]{ typeof( Minotaur ), typeof( TormentedMinotaur ) },			        // Level 1
					new Type[]{ typeof( MinotaurScout ), typeof( TormentedMinotaur ) },			    // Level 2
					new Type[]{ typeof( MinotaurCaptain ), typeof( TormentedMinotaur ) },		    // Level 3
					new Type[]{ typeof( LeatherArmoredMinotaur ), typeof( PlateArmoredMinotaur ) }	// Level 4
				} )
				#endregion
			};

		public static ChampionSpawnInfo GetInfo( ChampionSpawnType type )
		{
			int v = (int)type;

			if( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}
	}
}