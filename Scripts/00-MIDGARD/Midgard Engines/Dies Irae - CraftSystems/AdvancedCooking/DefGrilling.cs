using System;
using Midgard.Engines.OldCraftSystem;
using Server;
using Server.Items;
using Server.Engines.Craft;
using Midgard.Items;

namespace Midgard.Engines.AdvancedCooking
{
    public class DefGrilling : CraftSystem
    {
        public override string Name { get { return "Grilling"; } }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Grilling.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;

        public override SkillName MainSkill
        {
            get { return SkillName.Cooking; }
        }

        public override int GumpTitleNumber
        {
            get { return 0; } // Use String
        }

        public override string GumpTitleString
        {
            get { return "<basefont color=#FFFFFF><CENTER>Midgard Advanced Cooking: Grilling</CENTER></basefont>"; }
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefGrilling();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return Core.AOS ? CraftECA.ChanceMinusSixtyToFourtyFive : CraftECA.ZeroToFourPercentPlusBonus; } }

        public override double GetChanceAtMin( CraftItem item )
        {
            return 0.0; // 0%
        }

        private DefGrilling()
            : base( 2, 4, 1.25 ) // mod by Dies Irae // base( 1, 1, 1.5 )
        {
        }

        public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
        {
            if( tool.Deleted || tool.UsesRemaining < 0 )
                return 1044038; // You have worn out your tool!
            else if( !BaseTool.CheckAccessible( tool, from ) )
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect( Mobile from )
        {
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
                return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Breakfast
            index = AddCraft( typeof( Pancakes ), "Breakfast", "pancakes", 10.0, 60.0, typeof( Batter ), "Batter", 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Waffles ), "Breakfast", "waffles", 0.0, 50.0, typeof( WaffleMix ), "Waffle Mix", 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( FriedEggs ), "Breakfast", "fried eggs", 0.0, 50.0, typeof( Eggs ), "Eggs", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Bacon ), "Breakfast", "bacon", 10.0, 60.0, typeof( RawBacon ), "Raw Bacon", 1, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Barbecue
            index = AddCraft( typeof( Ribs ), "Barbecue", "ribs", 0.0, 60.0, typeof( RawRibs ), "Raw Ribs", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CookedBird ), "Barbecue", "cooked bird", 0.0, 60.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChickenLeg ), "Barbecue", "chicken leg", 0.0, 60.0, typeof( RawChickenLeg ), "Raw Chicken Leg", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( FishSteak ), "Barbecue", "fish steak", 0.0, 60.0, typeof( RawFishSteak ), "Raw Fish Steak", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( LambLeg ), "Barbecue", "lamb leg", 0.0, 60.0, typeof( RawLambLeg ), "Raw Lamb Leg", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( SmokedRibs ), "Barbecue", "smoked ribs", 55.0, 80.0, typeof( RawRibs ), "Raw Ribs", 2, 1044253 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( SpicedRibs ), "Barbecue", "spiced ribs", 70.0, 95.0, typeof( RawRibs ), "Raw Ribs", 2, 1044253 );
            AddRes( index, typeof( Garlic ), "garlic", 1, 1044253 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( EnergeticRibs ), "Barbecue", "energetic ribs", 85.0, 110.0, typeof( RawRibs ), "Raw Ribs", 2, 1044253 );
            AddRes( index, typeof( Ginseng ), "ginseng", 2, 1044253 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( SmokedFishSteak ), "Barbecue", "smoked fish steak", 55.0, 80.0, typeof( RawFishSteak ), 1044476, 2, 1044253 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( SpicedFishSteak ), "Barbecue", "spiced fish steak", 70.0, 95.0, typeof( RawFishSteak ), 1044476, 2, 1044253 );
            AddRes( index, typeof( Garlic ), "garlic", 1, 1044253 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( EnergeticFishSteak ), "Barbecue", "energetic fish steak", 85.0, 110.0, typeof( RawFishSteak ), 1044476, 2, 1044253 );
            AddRes( index, typeof( MandrakeRoot ), "mandrake root", 2, 1044253 );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( CookedSteak ), "Barbecue", "steak", 10.0, 60.0, typeof( RawSteak ), "Raw Steak", 1, "You need more Raw Steak" );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( HamSlices ), "Barbecue", "ham slices", 15.0, 65.0, typeof( RawHamSlices ), "Raw Ham Slice", 1, "You need more Raw Ham Slice" );
            SetNeedHeat( index, true );

            index = AddCraft( typeof( RoastHam ), "Barbecue", "roast ham", 20.0, 70.0, typeof( RawHam ), "Raw Ham", 1, "You need more Raw Ham" );
            SetNeedHeat( index, true );

            /*
	        index = AddCraft( typeof( HalibutFishSteak ), "Barbecue", "halibut fish steak", 50.0, 120.0, typeof( RawHalibutSteak ), "Raw Halibut Fish Steak", 1,
	                          "you need more Raw Halibut Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( FlukeFishSteak ), "Barbecue", "fluke fish steak", 50.0, 120.0, typeof( RawFlukeSteak ), "Raw Fluke Fish Steak", 1,
	                          "you need more Raw Fluke Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( MahiFishSteak ), "Barbecue", "mahi fish steak", 50.0, 120.0, typeof( RawMahiSteak ), "Raw Mahi Fish Steak", 1,
	                          "you need more Raw Mahi Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( SalmonFishSteak ), "Barbecue", "salmon fish steak", 50.0, 120.0, typeof( RawSalmonSteak ), "Raw Salmon Fish Steak", 1,
	                          "you need more Raw Salmon Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( RedSnapperFishSteak ), "Barbecue", "red snapper fish Steak", 50.0, 120.0, typeof( RawRedSnapperSteak ), "Raw Red Snapper Fish Steak", 1,
	                          "you need more Raw Red Snapper Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( ParrotFishSteak ), "Barbecue", "parrot fish steak", 50.0, 120.0, typeof( RawParrotFishSteak ), "Raw Parrot Fish Steak", 1,
	                          "you need more Raw Parrot Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( TroutFishSteak ), "Barbecue", "trout fish steak", 50.0, 120.0, typeof( RawTroutSteak ), "Raw Trout Fish Steak", 1,
	                          "you need more Raw Trout Fish Steaks" );
	        SetNeedHeat( index, true );

	        index = AddCraft( typeof( CookedShrimp ), "Barbecue", "cooked shrimp", 50.0, 120.0, typeof( RawShrimp ), "Raw Shrimp", 1,
	                          "you need more Raw Shrimp" );
	        SetNeedHeat( index, true );
            */
            #endregion

            #region Dinners
            index = AddCraft( typeof( BeefBBQRibs ), "Dinners", "beef barbecue ribs", 30.0, 80.0, typeof( RawRibs ), "Raw Ribs", 1, 1044253 );
            AddRes( index, typeof( BarbecueSauce ), "Barbecue Sauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BeefBroccoli ), "Dinners", "beef and broccoli", 30.0, 80.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( Broccoli ), "Broccoli", 4, 1044253 );
            AddRes( index, typeof( SoySauce ), "Soy Sauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChoChoBeef ), "Dinners", "cho cho beef", 50.0, 100.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( Teriyaki ), "Teriyaki", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BeefSnowpeas ), "Dinners", "beef and snow peas", 50.0, 100.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( SnowPeas ), "Snow Peas", 4, 1044253 );
            AddRes( index, typeof( SoySauce ), "Soy Sauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Hamburger ), "Dinners", "hamburger", 30.0, 80.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( BreadLoaf ), "Bread", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BeefLoMein ), "Dinners", "beef lo mein", 50.0, 100.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( PastaNoodles ), "Pasta Noodles", 2, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BeefStirfry ), "Dinners", "beef stirfry", 50.0, 100.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChickenStirfry ), "Dinners", "chicken stirfry", 30.0, 80.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( MooShuPork ), "Dinners", "moo shu pork", 60.0, 110.0, typeof( GroundPork ), "Ground Pork", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( MoPoTofu ), "Dinners", "mo po tofu", 60.0, 110.0, typeof( Tofu ), "Tofu", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( ChiliPepper ), "Chili Pepper", 3, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PorkStirfry ), "Dinners", "pork stirfry", 50.0, 100.0, typeof( GroundPork ), "Ground Pork", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( SweetSourChicken ), "Dinners", "sweet and sour chicken", 60.0, 110.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            AddRes( index, typeof( SoySauce ), "SoySauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( SweetSourPork ), "Dinners", "sweet and sour pork", 60.0, 110.0, typeof( GroundPork ), "Ground Pork", 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            AddRes( index, typeof( SoySauce ), "SoySauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BaconAndEgg ), "Dinners", "bacon and eggs", 10.0, 60.0, typeof( Eggs ), "Eggs", 2, 1044253 );
            AddRes( index, typeof( RawBacon ), "Raw Bacon", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );
            #endregion

            #region Other Food
            index = AddCraft( typeof( GarlicBread ), "Other Food", "garlic bread", 50.0, 75.0, typeof( BreadLoaf ), "Bread", 1, 1044253 );
            AddRes( index, typeof( Butter ), "Butter", 1, 1044253 );
            AddRes( index, typeof( Garlic ), "Garlic", 2, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Herbs", 1, 1044253 );

            index = AddCraft( typeof( GrilledHam ), "Other Food", "grilled ham", 60.0, 85.0, typeof( RawHamSlices ), "Raw Sliced Ham", 1, 1044253 );

            index = AddCraft( typeof( Sausage ), "Other Food", "sausage", 20.0, 45.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( GroundPork ), "Ground Pork", 1, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Ground Pork", 1, 1044253 );

            index = AddCraft( typeof( Hotwings ), "Other Food", "hotwings", 20.0, 45.0, typeof( RawChickenLeg ), "Raw Chicken Leg", 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            AddRes( index, typeof( HotSauce ), "Hot Sauce", 1, 1044253 );

            index = AddCraft( typeof( PotatoFries ), "Other Food", "potato fries", 10.0, 35.0, typeof( Potato ), "Potato", 3, 1044253 );
            AddRes( index, typeof( Onion ), "Onion", 1, 1044253 );
            AddRes( index, typeof( Butter ), "Butter", 1, 1044253 );

            index = AddCraft( typeof( Taco ), "Other Food", "taco", 35.0, 50.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( Tortilla ), "Tortilla", 1, 1044253 );
            AddRes( index, typeof( CheeseWheel ), "Cheese Wheel", 1, 1044253 );
            #endregion
        }
    }
}