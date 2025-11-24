using System;
using Midgard.Engines.OldCraftSystem;
using Server;
using Server.Items;
using Server.Engines.Craft;
using Midgard.Engines.WineCrafting;
using Midgard.Items;

namespace Midgard.Engines.BrewCrafing
{
    public class DefBrewing : CraftSystem
    {
        public override string Name { get{ return "Brewing"; } }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Brewing.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;

        public override SkillName MainSkill
        {
            get { return SkillName.Alchemy; }
        }

        public override int GumpTitleNumber
        {
            get { return 0; } // <CENTER>BREWING MENU</CENTER>
        }

        public override string GumpTitleString
        {
            get { return "<basefont color=#FFFFFF><CENTER>Midgard: Brewing Menu</CENTER></basefont>"; }
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefBrewing();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin( CraftItem item )
        {
            return Core.AOS ? 0.5 : 0.0; // mod by Dies Irae
        }

        private DefBrewing()
            : base( 2, 4, 1.25 ) // mod by Dies Irae // base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
        {
            if( tool.Deleted || tool.UsesRemaining < 0 )
                return 1044038; // You have worn out your tool!
            //else if ( !BaseTool.CheckAccessible( tool, from ) )
            //return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect( Mobile from )
        {
            from.PlaySound( 0x241 );
            //from.PlaySound( 0x242 );
        }

        public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
        {
            if( toolBroken )
                from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

            if( failed )
            {
                if( lostMaterial )
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if( quality == 0 )
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if( makersMark && quality == 2 )
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if( quality == 2 )
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Juices
            index = AddCraft( typeof( BottleOfAppleJuice ), "Juices", "apple juice", 90.0, 106.5, typeof( Apple ), "Apple", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BottleOfLemonJuice ), "Juices", "lemon juice", 90.0, 106.5, typeof( Lemon ), "Lemon", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BottleOfGrapeJuice ), "Juices", "grape juice", 90.0, 106.5, typeof( Grapes ), "Grapes", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BottleOfOrangeJuice ), "Juices", "orange juice", 90.0, 106.5, typeof( Orange ), "Orange", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BottleOfGrapeFruitJuice ), "Juices", "grape fruit juice", 90.0, 106.5, typeof( GrapeFruit ), "GrapeFruit", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BottleOfPruneJuice ), "Juices", "prune juice", 90.0, 106.5, typeof( Prune ), "Prune", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BottleOfPeachJuice ), "Juices", "peach juice", 90.0, 106.5, typeof( Peach ), "Peach", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );
            #endregion

            #region Cocktails
            index = AddCraft( typeof( GinJuice ), "CockTails", "gin & juice", 90.0, 106.5, typeof( BottleOfGin ), "BottleOfGin", 1 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( BottleOfOrangeJuice ), "OrangeJuice", 1 );
            AddRes( index, typeof( Lime ), "Lime", 1 );

            index = AddCraft( typeof( ScrewDriver ), "CockTails", "screw driver", 90.0, 106.5, typeof( BottleOfVodka ), "BottleOfVodka", 1 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( BottleOfOrangeJuice ), "OrangeJuice", 1 );

            index = AddCraft( typeof( SunRise ), "CockTails", "tequila sunrise", 90.0, 106.5, typeof( BottleOfTequila ), "BottleOfTequila", 1 );
            AddRes( index, typeof( BottleOfOrangeJuice ), "OrangeJuice", 1 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );

            index = AddCraft( typeof( MintShnops ), "CockTails", "peppermint shnops", 90.0, 106.5, typeof( BottleOfBrandy ), "BottleOfBrandy", 1 );
            AddRes( index, typeof( BottleOfRum ), "BottleOfRum", 1 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Mint ), "Mint", 1 );

            index = AddCraft( typeof( PeachShnops ), "CockTails", "peach shnops", 90.0, 106.5, typeof( BottleOfBrandy ), "BottleOfBrandy", 1 );
            AddRes( index, typeof( BottleOfRum ), "BottleOfRum", 1 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Peach ), "Peach", 1 );
            #endregion

            #region Red Wines

            index = AddCraft( typeof( CabernetSauvignonWineKeg ), "Red Wines", "keg of \"Tannico di Yew\"", 80.0, 105.6, typeof( CabernetSauvignonGrapes ), "Cabernet Sauvignon Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( MerlotWineKeg ), "Red Wines", "keg of \"Tempestoso di Daekarthane\"", 80.0, 105.6, typeof( MerlotGrapes ), "Merlot Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( PinotNoirWineKeg ), "Red Wines", "keg of \"Dolce di Magincia\"", 80.0, 105.6, typeof( PinotNoirGrapes ), "Pinot Noir Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( ShirazWineKeg ), "Red Wines", "keg of \"Longevo di Nunjelm\"", 80.0, 105.6, typeof( ShirazGrapes ), "Barbera Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( ZinfandelWineKeg ), "Red Wines", "keg of \"Luminoso di Britain\"", 80.0, 105.6, typeof( ZinfandelGrapes ), "Brunello Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( SangioveseWineKeg ), "Red Wines", "keg of \"Nebbiolo di Serpent's\"", 80.0, 105.6, typeof( SangioveseGrapes ), "Brunello Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );
            #endregion

            #region White Wines

            index = AddCraft( typeof( RieslingWineKeg ), "White Wines", "keg of \"Luminoso di Trinsic\"", 80.0, 105.6, typeof( RieslingGrapes ), "Aligote Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( CheninBlancWineKeg ), "White Wines", "keg of \"Oleoso di Buccaner's\"", 80.0, 105.6, typeof( CheninBlancGrapes ), "Chenin Blanc Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( ChardonnayWineKeg ), "White Wines", "keg of \"Ambra di Vesper\"", 80.0, 105.6, typeof( ChardonnayGrapes ), "Chardonnay Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( SauvignonBlancWineKeg ), "White Wines", "keg of \"Perlato di Moonglow\"", 80.0, 105.6, typeof( SauvignonBlancGrapes ), "Columbard Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( ViognierWineKeg ), "White Wines", "keg of \"Dorato di Cove\"", 80.0, 105.6, typeof( ViognierGrapes ), "Folle Blanche Grapes", 50 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 2 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );
            AddRes( index, typeof( Keg ), "Keg", 1 );
            #endregion

            #region Meads/Ale

            index = AddCraft( typeof( RawAleKeg ), "Ales", "raw ale compost keg", 50.0, 65.0, typeof( WaterPitcher ), "Water Pitcher", 2, "You need some full water pitchers" );
            AddRes( index, typeof( Keg ), "Keg", 1 );

            index = AddCraft( typeof( StoutAleKeg ), "Ales", "keg of stout ale", 90.0, 106.5, typeof( Malt ), "Malt", 2, "You need some malt" );
            AddRes( index, typeof( Hop ), "Hops", 1 );
            AddRes( index, typeof( CereviasiaeYeast ), "Cereviasiae Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( PorterAleKeg ), "Ales", "keg of porter ale", 90.0, 106.5, typeof( Wheat ), "Wheat", 2, "You need some wheat" );
            AddRes( index, typeof( Hop ), "Hops", 1 );
            AddRes( index, typeof( CereviasiaeYeast ), "Cereviasiae Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( MildAleKeg ), "Ales", "keg of mild ale", 90.0, 106.5, typeof( Malt ), "Malt", 3, "You need some malt" );
            AddRes( index, typeof( Hop ), "Hops", 2 );
            AddRes( index, typeof( CarlsbergensisYeast ), "Carlsbergensis Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( WinterAleKeg ), "Ales", "keg of winter ale", 90.0, 106.5, typeof( Wheat ), "Wheat", 3, "You need some wheat" );
            AddRes( index, typeof( Hop ), "Hops", 2 );
            AddRes( index, typeof( CarlsbergensisYeast ), "Carlsbergensis Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( BlondAleKeg ), "Ales", "keg of blond ale", 90.0, 106.5, typeof( Malt ), "Malt", 2, "You need some malt" );
            AddRes( index, typeof( Hop ), "Hops", 2 );
            AddRes( index, typeof( CereviasiaeYeast ), "Cereviasiae Yeast", 1 );
            AddRes( index, typeof( Peach ), "Peach", 2 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( TrappisteDoubelAleKeg ), "Ales", "keg of trappiste doubel ale", 90.0, 106.5, typeof( Malt ), "Malt", 3, "You need some malt" );
            AddRes( index, typeof( Hop ), "Hops", 2 );
            AddRes( index, typeof( CarlsbergensisYeast ), "Carlsbergensis Yeast", 2 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( TrappisteTripelAleKeg ), "Ales", "keg of trappiste tripel ale", 90.0, 106.5, typeof( Malt ), "Malt", 3, "You need some malt" );
            AddRes( index, typeof( Hop ), "Hops", 3 );
            AddRes( index, typeof( CarlsbergensisYeast ), "Carlsbergensis Yeast", 3 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( PilsenerKeg ), "Ales", "keg of pilsener ale", 90.0, 106.5, typeof( Wheat ), "Wheat", 3, "You need some wheat" );
            AddRes( index, typeof( Hop ), "Hops", 2 );
            AddRes( index, typeof( CarlsbergensisYeast ), "Carlsbergensis Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( AltBierKeg ), "Ales", "keg of alt bier", 90.0, 106.5, typeof( Wheat ), "Wheat", 3, "You need some wheat" );
            AddRes( index, typeof( Hop ), "Hops", 3 );
            AddRes( index, typeof( CereviasiaeYeast ), "Cereviasiae Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );

            index = AddCraft( typeof( KarakIdrilKeg ), "Ales", "keg of Karak Idril", 90.0, 106.5, typeof( Wheat ), "Wheat", 2, "You need some wheat" );
            AddRes( index, typeof( Malt ), "Malt", 1 );
            AddRes( index, typeof( Hop ), "Hops", 3 );
            AddRes( index, typeof( CarlsbergensisYeast ), "Carlsbergensis Yeast", 1 );
            AddRes( index, typeof( RawAleKeg ), "Raw Ale Compost", 1 );
            SetNeededRace( index, Races.Core.MountainDwarf );

            #endregion

            #region Whiskeys

            index = AddCraft( typeof( BurbonKeg ), "Whiskeys", "keg of burbon", 90.0, 106.5, typeof( Barley ), "Barley", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            // AddRes( index, typeof ( Rye ), "Rye", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( RyeWhiskeyKeg ), "Whiskeys", "keg of rye whiskey", 90.0, 106.5, typeof( Malt ), "Malt", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Corn ), "Corn", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( IrishWhiskeyKeg ), "Whiskeys", "keg of irish whiskey", 90.0, 106.5, typeof( Barley ), "Barley", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( BrandyKeg ), "Whiskeys", "keg of brandy", 90.0, 106.5, typeof( BlackBerry ), "BlackBerry", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Cherries ), "Cherries", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( VodkaKeg ), "Whiskeys", "keg of vodka", 90.0, 106.5, typeof( Malt ), "Malt", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( Corn ), "Corn", 1 );
            AddRes( index, typeof( Barley ), "Barley", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( GinKeg ), "Whiskeys", "keg of gin", 90.0, 106.5, typeof( JuniperBerry ), "JuniperBerry", 50 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );
            AddRes( index, typeof( Barley ), "Barley", 1 );

            index = AddCraft( typeof( RumKeg ), "Whiskeys", "keg of rum", 90.0, 106.5, typeof( Sugar ), "Sugar", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );

            index = AddCraft( typeof( ScotchKeg ), "Whiskeys", "keg of scotch", 90.0, 106.5, typeof( Barley ), "Barley", 5 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );

            index = AddCraft( typeof( TequilaKeg ), "Whiskeys", "keg of tequila", 90.0, 106.5, typeof( Malt ), "Malt", 5 );
            AddRes( index, typeof( Sugar ), "Sugar", 1 );
            AddRes( index, typeof( Bottle ), "Bottle", 1 );
            AddRes( index, typeof( WaterPitcher ), "Water Pitcher", 1 );
            AddRes( index, typeof( CommonYeast ), "Yeast", 1 );

            #endregion

            /*
            // Set the overidable material
            SetSubRes( typeof( SweetHops ), "Sweet Hops" );

            // Add every material you want the player to be able to chose from
            // This will overide the overidable material

            AddSubRes( typeof( SweetHops ), "Sweet Hops", 50.0, 1044268 );
            AddSubRes( typeof( BitterHops ), "Bitter Hops", 70.0, 1044268 );
            AddSubRes( typeof( SnowHops ), "Snow Hops", 80.0, 1044268 );
            AddSubRes( typeof( ElvenHops ), "Elven Hops", 90.0, 1044268 );
            */

            MarkOption = true;
            Repair = false;
        }
    }
}