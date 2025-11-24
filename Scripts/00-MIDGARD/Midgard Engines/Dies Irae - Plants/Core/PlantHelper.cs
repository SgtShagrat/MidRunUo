/***************************************************************************
 *                                     PlantHelper.cs
 *                            		-------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Utility class for plant system.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Multis;

namespace Midgard.Engines.PlantSystem
{
    public class PlantHelper
    {
        /// <summary>
        /// URL of the plant guide.
        /// It is sent to player that click the help button
        /// </summary>
        public static readonly string HelpPlantSystemURL = @"www.midgardshard.it";

        /// <summary>
        /// Variable should be true if logging must be enabled
        /// </summary>
        public static readonly bool EnableLogging = true;

        /// <summary>
        /// The minimum time interval in the world of plant, in unit of real world minutes
        /// Currently is set to 120
        /// </summary>
        public static readonly double PlantTimerIntervalInt = 60 * 2;

        /// <summary>
        /// Delay of proper plant groth
        /// Currently is set to 120 minutes
        /// </summary>
        public static TimeSpan PlantTimerInterval = TimeSpan.FromMinutes( PlantTimerIntervalInt );

        /// <summary>
        /// Debug time interval. Is quite short
        /// Currently is set to 30 seconds
        /// </summary>
        public static TimeSpan DebugPlantTimerInterval = TimeSpan.FromSeconds( PlantTimerIntervalInt );

        /// <summary>
        /// The delay before start a plant timer
        /// </summary>
        public static TimeSpan PlantTimerDormant = TimeSpan.Zero;

        /// <summary>
        /// Divider for timer interval on fertilization
        /// </summary>
        public static readonly double FertilizerDivider = 2.0;

        /// <summary>
        /// Duration of fertilizer in real world
        /// </summary>
        public static TimeSpan FertilizerDuration = TimeSpan.FromDays( 2.0 );

        /// <summary>
        /// Cap for long drought plant
        /// </summary>
        public static readonly int CareLevelWorst = -10;

        /// <summary>
        /// Cap for drought plant
        /// </summary>
        public static readonly int CareLevelBad = -5;

        /// <summary>
        /// Care level to start from when plant in seeded
        /// </summary>
        public static readonly int CareLevelOk = 0;

        /// <summary>
        /// Cap of care level for watered plant
        /// </summary>
        public static readonly int CareLevelGood = 5;

        /// <summary>
        /// Maximum care level
        /// </summary>
        public static readonly int CareLevelBest = 10;

        /// <summary>
        /// Hues for bad living plants (from -10 to -5 of care level)
        /// </summary>
        public static readonly int[] CareWarningHue = { 47, 48, 49, 50, 51, 54 };

        /// <summary>
        /// Hue for plant leaves without dried ID
        /// </summary>
        public static readonly int DriedLeavesHue = 2012;

        /// <summary>
        /// Chance of water boosting carelevel
        /// </summary>
        public static readonly double WaterBoostChance = 0.3;

        /// <summary>
        /// Chance of too water dropping carelevel
        /// </summary>
        public static readonly double TooWaterDropChance = 0.7;

        /// <summary>
        /// Chance of pest lowering carelevel
        /// </summary>
        public static readonly double PestDropChance = 0.7;

        /// <summary>
        /// Chance of too pesticide dropping carelevel
        /// </summary>
        public static readonly double TooPesticideDropChance = 0.1;

        /// <summary>
        /// Chance of fungs lowering carelevel
        /// </summary>
        public static readonly double FungusDropChance = 0.7;

        /// <summary>
        /// Chance of too fungicide dropping carelevel
        /// </summary>
        public static readonly double TooFungicideDropChance = 0.1;

        /// <summary>
        /// Watering within one tick of plant timer will not do any help
        /// Currently is set to 20 hours
        /// </summary>
        public static TimeSpan ProperWaterInterval = TimeSpan.FromMinutes( PlantTimerIntervalInt * 10.0 );

        /// <summary>
        /// When doing healthCheck, watered within this interval will boost CareLevel for sure
        /// Its set to half the ProperWaterInterval ( 12 hours )
        /// </summary>
        public static TimeSpan HealthCheckWaterInterval = TimeSpan.FromMinutes( PlantTimerIntervalInt * 12.0 * 0.5 );

        /// <summary>
        /// How many ticks a fungus/pest check will take place. This is equal to half a real life day.
        /// Currently is set to 24
        /// </summary>
        public static int HealthCheckTick = (int)( 720 / PlantTimerIntervalInt );

        #region tiles
        /// <summary>
        /// All farm tiles in pairs as starting and ending tile ID of a range
        /// </summary>
        public static int[] FarmTiles = new int[]
		{
			0x009, 0x00A, 
			0x00C, 0x00E, 
			0x013, 0x015, 
			0x150, 0x155, 
			0x15A, 0x15C 
		};

        /// <summary>
        /// All dirt tiles in pairs as starting and ending tile ID of a range
        /// </summary>
        public static int[] DirtTiles = new int[]
		{
			0x071, 0x07C, // Roads
			0x165, 0x174, // Roads
			0x1DC, 0x1EF, // Rock Border
			0x306, 0x31F, // Snow Border
			0x08D, 0x0A7, // Steep Slopes
			0x2E5, 0x305, // Steep Slopes
			0x777, 0x791, // Steep Slopes
			0x98C, 0x9BF, // Steep Slopes
		};

        /// <summary>
        /// All ground tiles in pairs as starting and ending tile ID of a range
        /// </summary>
        public static int[] GroundTiles = new int[]
		{
			0x003, 0x006, 
			0x033, 0x03E, 
			0x078, 0x08C, 
			0x0AC, 0x0DB, 
			0x108, 0x10B, 
			0x14C, 0x174, 
			0x1A4, 0x1A7, 
			0x1B1, 0x1B2, 
			0x26E, 0x281, 
			0x292, 0x295, 
			0x355, 0x37E, 
			0x3CB, 0x3CE, 
			0x547, 0x5A6, 
			0x5E3, 0x618, 
			0x66B, 0x66E, 
			0x6A1, 0x6C2, 
			0x6DE, 0x6E1, 
			0x73F, 0x742, 
		};

        /// <summary>
        /// All swamp tiles in pairs as starting and ending tile ID of a range
        /// </summary>
        public static int[] SwampTiles = new int[]
		{
			//0x7DC, 0x808, 
			0x3DC1, 0x3DC2, 
			0x3DD9, 0x3EF0, 
		};

        /// <summary>
        /// All sand tiles in pairs as starting and ending tile ID of a range
        /// </summary>
        public static int[] SandTiles = new int[]
		{
			0x016, 0x019, 
			0x033, 0x03E, 
			0x1A8, 0x1AB, 
			0x282, 0x291, 
			0x335, 0x35C, 
			0x3B7, 0x3CA, 
			0x5A7, 0x5BA, 
			0x64B, 0x66A, 
			0x66F, 0x672, 
			0x7D5, 0x7D8, 
		};

        public static int[] HouseTiles = new int[]  //ADDED HOUSETILES
            {
                0x31F4, 0x31F5, 
                0x31F6, 0x31F7, 
                0x515, 0x516, 
                0x517, 0x518, 
                0x31F4, 0x31F9, 
                0x31FA, 0x31FB 
            };
        #endregion

        /// <summary>
        /// Check whether a tile can grow the plant
        /// </summary>
        /// <param name="sowable">the plant being checked</param>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <param name="from"></param>
        /// <returns>whether can grow</returns>
        public static bool ValidatePlacement( ISowable sowable, Map map, int x, int y, Mobile from )
        {
            if( map == null || map == Map.Internal )
                return false;

            Point3D location = from.Location;

            if( !map.CanFit( location, 20, true, false ) )
                return false;

            if( from.AccessLevel > AccessLevel.Player )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( location, map, 20 );

            if( CheckHouse( from, location, map, 20 ) )
            {
                if( sowable.CanGrowFarm && ValidateFarmLand( map, x, y ) )
                    return true;

                if( sowable.CanGrowDirt && ValidateDirt( map, x, y ) )
                    return true;

                if( sowable.CanGrowGround && ValidateGround( map, x, y ) )
                    return true;

                if( sowable.CanGrowSand && ValidateSand( map, x, y ) )
                    return true;

                if( sowable.CanGrowSwamp && ValidateSwamp( map, x, y ) )
                    return true;

                if( sowable.CanGrowGarden && ValidateGardenPlot( map, x, y ) )
                    return true;

                // se e' un campo e ha i tiles giusti allora permetti la coltura
                if( house != null && TownFieldSign.IsField( house, true ) )
                    return true;
            }
            else
                from.SendMessage( "You must be standing in your field to place this." );

            return false;
        }

        #region Validate...
        /// <summary>
        /// Try detect a house garden in the current location.
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>whether can grow</returns>
        public static bool ValidateHouseTiles( Map map, int x, int y )
        {
            bool ground = false;

            Tile[] tiles = map.Tiles.GetStaticTiles( x, y, true );

            foreach( Tile t in tiles )
            {
                int tileID = t.ID & 0x3FFF;

                for( int j = 0; !ground && j < HouseTiles.Length; j += 2 )
                {
                    ground = ( tileID >= HouseTiles[ j ] && tileID <= HouseTiles[ j + 1 ] );
                }
            }

            return ground;
        }

        /// <summary>
        /// Try detect a dynamic garden dirt patch in the current location.
        /// If failes, tries to detect a static garden dirt patch frozen into Map
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>whether can grow</returns>
        public static bool ValidateGardenPlot( Map map, int x, int y )
        {
            bool ground = false;

            IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( x, y, 1, 1 ) );
            foreach( Item item in eable )
            {
                if( item.ItemID == 0x32C9 ) // found a dirt patch; possibly also 0x32CA 
                    ground = true;
            }
            eable.Free();

            if( !ground )
            {
                Tile[] tiles = map.Tiles.GetStaticTiles( x, y );
                for( int i = 0; i < tiles.Length; ++i )
                {
                    if( ( tiles[ i ].ID & 0x3FFF ) == 0x32C9 )
                        ground = true;
                }
            }

            if( !ground )
            {
                if( ValidateHouseTiles( map, x, y ) )
                    ground = true;
            }

            return ground;
        }

        /// <summary>
        /// Check if the location is a Farmland.
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>if can grow</returns>
        public static bool ValidateFarmLand( Map map, int x, int y )
        {
            int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
            bool ground = false;

            for( int i = 0; !ground && i < FarmTiles.Length; i += 2 )
                ground = ( tileID >= FarmTiles[ i ] && tileID <= FarmTiles[ i + 1 ] );

            return ground;
        }

        /// <summary>
        /// Check if the location is a Dirt land.
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>if can grow</returns>
        public static bool ValidateDirt( Map map, int x, int y )
        {
            int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
            bool ground = false;

            for( int i = 0; !ground && i < DirtTiles.Length; i += 2 )
                ground = ( tileID >= DirtTiles[ i ] && tileID <= DirtTiles[ i + 1 ] );

            return ground;
        }

        /// <summary>
        /// Check if the location is a valid Ground tile.
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>if can grow</returns>
        public static bool ValidateGround( Map map, int x, int y )
        {
            int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
            bool ground = false;

            for( int i = 0; !ground && i < GroundTiles.Length; i += 2 )
                ground = ( tileID >= GroundTiles[ i ] && tileID <= GroundTiles[ i + 1 ] );

            return ground;
        }

        /// <summary>
        /// Check if the location is a Swamp.
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>if can grow</returns>
        public static bool ValidateSwamp( Map map, int x, int y )
        {
            int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
            bool ground = false;

            for( int i = 0; !ground && i < SwampTiles.Length; i += 2 )
                ground = ( tileID >= SwampTiles[ i ] && tileID <= SwampTiles[ i + 1 ] );

            return ground;
        }

        /// <summary>
        /// Check if the location is a Sand tile.
        /// </summary>
        /// <param name="map">the map being checked</param>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        /// <returns>if can grow</returns>
        public static bool ValidateSand( Map map, int x, int y )
        {
            int tileID = map.Tiles.GetLandTile( x, y ).ID & 0x3FFF;
            bool ground = false;

            for( int i = 0; !ground && i < SandTiles.Length; i += 2 )
                ground = ( tileID >= SandTiles[ i ] && tileID <= SandTiles[ i + 1 ] );

            return ground;
        }
        #endregion

        /// <summary>
        /// Check if the location is a huose whose owner is from
        /// </summary>
        public static bool CheckHouse( Mobile from, Point3D p, Map map, int height )
        {
            if( from == null || from.AccessLevel >= AccessLevel.GameMaster )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( p, map, height );

            if( house == null || !house.IsOwner( from ) )
                return false;

            return true;
        }

        /// <summary>
        /// Check whether there are other plants in a range around the target plot
        /// </summary>
        /// <param name="point">target point</param>
        /// <param name="map">map</param>
        /// <param name="range">range around the target</param>
        /// <returns>all plants within range</returns>
        public static List<BasePlant> GetPlantsInRange( Point3D point, Map map, int range )
        {
            List<BasePlant> plants = new List<BasePlant>();

            IPooledEnumerable eable = map.GetItemsInRange( point, range );
            foreach( Item i in eable )
            {
                if( i != null && !i.Deleted )
                {
                    if( i is BasePlant )
                    {
                        plants.Add( (BasePlant)i );
                    }
                }
            }
            eable.Free();

            return plants;
        }

        public static int GetMaxPlantsForPlayer( Mobile from )
        {
            return (int)( from.Skills[ SkillName.Camping ].Value / 2 + 30 );
        }
    }
}