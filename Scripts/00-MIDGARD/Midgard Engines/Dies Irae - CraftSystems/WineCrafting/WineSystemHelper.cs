/***************************************************************************
 *                                    WineCrafting.cs
 *                            		-------------------
 *  begin                	: Maj, 2007
 *  version					: 2.0.1
 *  author					: dracana from RunUO.com
 * 	revision				: Dies Irae
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: adminpirateroberts@hotmail.com - tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************

 	Info:
    Wine crafting - using Alchemy skill. Alchemy of 80 required. Exceptionally crafted wines can 
		bear vinyard name (maker's mark) and are made as "Special Reserve" wines.
    	Too add to realism, players craft kegs instead of bottles. Kegs must ferment for 7 real world 
		days, before they can be bottled. Kegs are equiv of 15.5 gallon kegs and can bottle 75 wine bottles. 
		If you prefer players to craft bottles, simply open defWinecrafting.cs and uncomment line 
		94 and comment out line 96.

    Enhanced Sigma Grapevines - Includes gump to place, move, delete vines as well as set 
		which variety of grape vine will hold.
    Grape Picking - Multiple varieties of grapes can be picked from the vine and used in crafted wines. 
		These varieties are...

        	Cabernet Sauvignon, Chardonnay, Chenin Blanc, Merlot, Pinot Noir, Riesling, Sangiovese, 
			Sauvignon Blanc, Shiraz, Viognier, and Zinfandel

    Winecrafter NPC - Sells/Buys grape varieties. Sells winecrafting supplies (sugar, yeast, 
		empty wine bottles, and winecrafter kit). Sells the grapevine placement tool and the vinyard ground addon deed.
    Vintner NPC - Sells/Buys Bottles of wine

    Tools
        Grapevine placement tool. This tool will open the same gump that admins use, 
        	but it is locked down when used by players to validate that they are placing/moving/deleting 
			vines in their own house and only on dirt/ground tiles.
        	You can set the rules for placing grapevines by editing the following parameters 
			in VinePlacement.cs…

            AllowPlayerYards - (default = false) - Option if your shard allows players to build 
				outside their house foundation. Currently set to allow players to place within 5 tiles of house
            AllowAllHouseTiles - (default = false) - Option to allow players to place on all 
				house tiles or only on dirt or vinyard ground addon.
            AllowAllYardTiles - (default = false) - Option to work when AllowPlayerYards is true. 
				To allow players to place on any type of tile or only on ground and dirt tiles.
            m_VinePrice - (default = 250) - the price that each vine will cost the player and the price 
				they will be refunded.

        * Vinyard ground addon. This is only needed by players that don't have custom houseing. 
        	It is an addon that will add dirt tiles into their house.	
        	NEW: Added two new addons that were created by masternightmage. 
			These are the Keg Storage Rack and Bottle Rack.

        * Vinyard Label Tool. This tool can be purchased from the Winecrafter NPC 
        	and can be used by players to name their own vinyard. To use, players need to double 
			click label tool, then click on one of the following (depending on what they want to do...

            Crafted Bottle of wine (crafted by them and exceptional only) - this will only label that specific bottle.
            Crafted Keg of Wine (crafted by them and exceptional only) - this will label all bottles poured from that keg.
            The Vinyard label Tool itself - If they name the tool, all future kegs, etc they craft will hold that name. 
				Tool must be in backpack at time of crafting kegs!

    Addons two new addons that were created by masternightmage. 
		These are the Keg Storage Rack and Bottle Rack. 
		These are currently not sold by NPC’s. You can add in whatever manner you want.

 ***************************************************************************/

using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.PlantSystem;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using Midgard.Engines.BrewCrafing;

namespace Midgard.Engines.WineCrafting
{
    #region enums
    /*
    public enum WineQuality
    {
        Low,
        Regular,
        Exceptional
    }
    */

    /*
    public enum BrewVariety
    {
        None = 0,

        CabernetSauvignon,
        Chardonnay,
        CheninBlanc,
        Merlot,
        PinotNoir,
        Riesling,
        Sangiovese,
        SauvignonBlanc,
        Shiraz,
        Viognier,
        Zinfandel
    }
    */

    /*
    public enum BrewVarietyType
    {
        None,

        Grapes
    }
    */

    public enum VinyardGroundType
    {
        FurrowNoBorder,
        FurrowBorder,
        PlainNoBorder,
        PlainBorder
    }

    public enum VinyardGroundPosition
    {
        Top,
        Bottom,
        Left,
        Right,
        West,
        North,
        East,
        South,
        Center
    }
    #endregion

    public static class WineSystemHelper
    {
        public const int VinePrice = 1000;

        public static bool ValidatePlacement( Point3D loc, Mobile from, object targeted )
        {
            Map map = from.Map;
            if( map == null )
                return false;

            if( !map.CanFit( loc, 20 ) )
                return false;

            LandTarget land = targeted as LandTarget;
            if( land == null )
                return false;

            if( from.AccessLevel > AccessLevel.Player )
                return true;

            BaseHouse house = BaseHouse.FindHouseAt( from.Location, map, 20 );

            if( PlantHelper.CheckHouse( from, loc, map, 20 ) )
            {
                // i townfield sono statici o su mappa indi i check son fatti su LandTiles
                if( !PlantHelper.ValidateFarmLand( map, loc.X, loc.Y ) && !PlantHelper.ValidateDirt( map, loc.X, loc.Y ) && !PlantHelper.ValidateGround( map, loc.X, loc.Y ) )
                {
                    from.SendMessage( "Grapevines must be placed on dirt, ground, or farm tiles." );
                    return false;
                }

                // se e' un campo e ha i tiles giusti allora permetti la coltura
                if( TownFieldSign.IsField( house, true ) )
                    return true;
            }
            else
                from.SendMessage( "You must be standing in your house or in your field to place this." );

            return false;
        }

        public static bool ValidateVinyardPlot( Map map, int x, int y )
        {
            bool ground = false;

            // Test for Dynamic Item
            IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( x, y, 1, 1 ) );
            foreach( Item item in eable )
            {
                if( item.ItemID == 0x32C9 || item.ItemID == 0x31F4 ) // dirt; possibly also 0x32CA 
                    ground = true;
            }
            eable.Free();

            // Test for Frozen into Map
            if( !ground )
            {
                Tile[] tiles = map.Tiles.GetStaticTiles( x, y );
                for( int i = 0; i < tiles.Length; ++i )
                {
                    if( ( tiles[ i ].ID & 0x3FFF ) == 0x32C9 || ( tiles[ i ].ID & 0x3FFF ) == 0x31F4 )
                        ground = true;
                }
            }

            return ground;
        }

        public static bool PayForVine( Mobile from )
        {
            if( Banker.Withdraw( from, VinePrice ) )
            {
                from.SendLocalizedMessage( 1060398, VinePrice.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

                return true;
            }
            else
            {
                if( from.Backpack.ConsumeTotal( typeof( Gold ), VinePrice ) )
                {
                    from.SendMessage( string.Format( "{0} gold has been removed from your pack", VinePrice ) );

                    return true;
                }
            }

            return false;
        }

        public static bool RefundForVine( Mobile from )
        {
            Container c = from.Backpack;
            Gold t = new Gold( ( VinePrice ) );

            if( c.TryDropItem( from, t, true ) )
            {
                from.SendMessage( string.Format( "You have been refunded {0} gold for the deleted vine.", VinePrice ) );

                return true;
            }
            else
            {
                t.Delete();
                from.SendMessage( "For some reason, the refund didn't work!  Please page a GM" );

                return false;
            }
        }

        public static double GetHarvestSkill( BrewVariety variety )
        {
            switch( variety )
            {
                case BrewVariety.CabernetSauvignon: return 80.0;
                case BrewVariety.Chardonnay: return 80.0;
                case BrewVariety.CheninBlanc: return 80.0;
                case BrewVariety.Merlot: return 80.0;
                case BrewVariety.Riesling: return 80.0;
                case BrewVariety.Sangiovese: return 80.0;
                case BrewVariety.SauvignonBlanc: return 80.0;
                case BrewVariety.Shiraz: return 80.0;
                case BrewVariety.Viognier: return 80.0;
                case BrewVariety.Zinfandel: return 80.0;
                default: return 80.0;
            }
        }
    }
}