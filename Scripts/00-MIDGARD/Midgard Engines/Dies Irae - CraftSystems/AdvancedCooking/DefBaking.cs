using System;
using Midgard.Engines.OldCraftSystem;
using Server;
using Server.Items;
using Server.Engines.Craft;
using Midgard.Items;

namespace Midgard.Engines.AdvancedCooking
{
    public class DefBaking : CraftSystem
    {
        public override string Name { get { return "Baking"; } }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Baking.xml", CraftSystem );

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
            get { return "<basefont color=#FFFFFF><CENTER>Midgard Advanced Cooking: Baking</CENTER></basefont>"; }
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefBaking();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA
        {
            get { return Core.AOS ? CraftECA.ChanceMinusSixtyToFourtyFive : CraftECA.ZeroToFourPercentPlusBonus; }
        }

        public override double GetChanceAtMin( CraftItem item )
        {
            return 0.0; // 0%
        }

        private DefBaking()
            : base( 2, 4, 1.25 ) // mod by Dies Irae  // base( 1, 1, 1.5 )
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

        public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality,
                                             bool makersMark, CraftItem item )
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

            #region Breads
            index = AddCraft( typeof( BreadLoaf ), "Breads", "bread", 30.0, 70.0, typeof( Dough ), 1044469, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Muffins ), "Breads", "muffins", 45.0, 50.0, typeof( Batter ), "Batter", 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BananaBread ), "Breads", "banana bread", 50.0, 65.0, typeof( SweetDough ), "Sweet Dough", 1, 1044253 );
            AddRes( index, typeof( Banana ), "Banana", 6, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BlueberryMuffins ), "Breads", "blueberry muffins", 50.0, 65.0, typeof( SweetDough ), "Sweet Dough", 1, 1044253 );
            AddRes( index, typeof( Blueberry ), "Blueberry", 6, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CornBread ), "Breads", "corn bread", 40.0, 65.0, typeof( BagOfCornmeal ), "Bag of Cornmeal", 1, 1044253 );
            AddRes( index, typeof( Batter ), "Batter", 1, 1044253 );
            AddRes( index, typeof( BagOfSugar ), "Bag of Sugar", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Donuts ), "Breads", "donuts", 30.0, 55.0, typeof( SweetDough ), "Sweet Dough", 2, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PumpkinBread ), "Breads", "pumpkin bread", 40.0, 65.0, typeof( SweetDough ), "Sweet Dough", 1, 1044253 );
            AddRes( index, typeof( Pumpkin ), "Pumpkin", 3, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PumpkinMuffins ), "Breads", "pumpkin muffins", 40.0, 65.0, typeof( SweetDough ), "Sweet Dough", 1, 1044253 );
            AddRes( index, typeof( Pumpkin ), "Pumpkin", 2, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Cookies
            index = AddCraft( typeof( Cookies ), "Cookies", 1025643, 40.0, 80.0, typeof( CookieMix ), 1044474, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Cake ), "Cookies", 1022537, 55.0, 95.0, typeof( CakeMix ), 1044471, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( AlmondCookies ), "Cookies", "almond cookies", 50.0, 90.0, typeof( CookieMix ), "Cookie Mix", 1, 1044253 );
            AddRes( index, typeof( Almond ), "Almond", 12, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChocChipCookies ), "Cookies", "chocolate chip cookies", 50.0, 90.0, typeof( CookieMix ), "Cookie Mix", 1, 1044253 );
            AddRes( index, typeof( BagOfCocoa ), "Bag of Cocoa", 1, "YUou need a bag of cocoa" );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( GingerSnaps ), "Cookies", "ginger snaps", 50.0, 90.0, typeof( CookieMix ), "Cookie Mix", 1, 1044253 );
            AddRes( index, typeof( TanGinger ), "Tan Ginger", 12, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( OatmealCookies ), "Cookies", "oatmeal cookies", 50.0, 90.0, typeof( CookieMix ), "Cookie Mix", 1, 1044253 );
            AddRes( index, typeof( BagOfOats ), "Bag of Oats", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PeanutButterCookies ), "Cookies", "peanut butter cookies", 55.0, 95.0, typeof( CookieMix ), "Cookie Mix", 1, 1044253 );
            AddRes( index, typeof( PeanutButter ), "Peanut Butter", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PumpkinCookies ), "Cookies", "pumpkin cookies", 50.0, 90.0, typeof( CookieMix ), "Cookie Mix", 1, 1044253 );
            AddRes( index, typeof( Pumpkin ), "Pumpkin", 6, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );
            #endregion

            #region Desserts
            index = AddCraft( typeof( ApplePie ), "Desserts", "apple pie", 60.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Apple ), "Apple", 8, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BlueberryPie ), "Desserts", "blueberry pie", 60.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Blueberry ), "Blueberry", 8, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CherryPie ), "Desserts", "cherry pie", 60.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Cherry ), "Cherry", 8, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( FruitPie ), "Desserts", "fruit pie", 60.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( FruitBasket ), "Fruit Basket", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( KeyLimePie ), "Desserts", "key lime pie", 60.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Lime ), "Lime", 12, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( LemonMerenguePie ), "Desserts", "lemon merengue pie", 70.0, 100.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Lemon ), "Lemon", 12, 1044253 );
            AddRes( index, typeof( Cream ), "Cream", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PumpkinPie ), "Desserts", "pumpkin pie", 60.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Pumpkin ), "Pumpkin", 2, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BlackberryCobbler ), "Desserts", "blackberry cobbler", 65.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Blackberry ), "Blackberry", 10, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PeachCobbler ), "Desserts", "peach cobbler", 65.0, 95.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Peach ), "Peach", 10, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Brownies ), "Desserts", "brownies", 70.0, 100.0, typeof( ChocolateMix ), "Chocolate Mix", 1, 1044253 );
            AddRes( index, typeof( Eggs ), "Eggs", 2, 1044253 );
            AddRes( index, typeof( CookingOil ), "Cooking Oil", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChocSunflowerSeeds ), "Desserts", "chocolate sunflower seeds", 70.0, 100.0, typeof( EdibleSun ), "Sunflower Seeds", 1, 1044253 );
            AddRes( index, typeof( BagOfCocoa ), "Bag of Cocoa", 1, "you need a bag oc cocoa" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( RiceKrispTreat ), "Desserts", "rice krisp treat", 60.0, 95.0, typeof( BowlRiceKrisps ), "Bowl Of Rice Krips", 1, 1044253 );
            AddRes( index, typeof( Butter ), "Butter", 1, 1044253 );
            AddRes( index, typeof( BagOfSugar ), "Bag of Sugar", 1, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Cakes
            index = AddCraft( typeof( BananaCake ), "Cakes", "banana cake", 60.0, 85.0, typeof( CakeMix ), "Cake Mix", 1, 1044253 );
            AddRes( index, typeof( Banana ), "Banana", 4, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CarrotCake ), "Cakes", "carrot cake", 60.0, 85.0, typeof( CakeMix ), "Cake Mix", 1, 1044253 );
            AddRes( index, typeof( Carrot ), "Carrot", 6, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChocolateCake ), "Cakes", "chocolate cake", 60.0, 85.0, typeof( CakeMix ), "Cake Mix", 1, 1044253 );
            AddRes( index, typeof( BagOfCocoa ), "Bag of Cocoa", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CoconutCake ), "Cakes", "coconut cake", 60.0, 85.0, typeof( CakeMix ), "Cake Mix", 1, 1044253 );
            AddRes( index, typeof( Coconut ), "Coconut", 2, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( LemonCake ), "Cakes", "lemon cake", 60.0, 85.0, typeof( CakeMix ), "Cake Mix", 1, 1044253 );
            AddRes( index, typeof( Lemon ), "Lemon", 4, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Dinners
            index = AddCraft( typeof( MeatPie ), "Dinners", 1041347, 45.0, 55.0, typeof( UnbakedMeatPie ), 1044519, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChickenParmesian ), "Dinners", "chicken parmesian", 75.0, 110.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( TomatoSauce ), "Tomato Sauce", 1, 1044253 );
            AddRes( index, typeof( CheeseWheel ), "Cheese Wheel", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CheeseEnchilada ), "Dinners", "cheese enchilada", 75.0, 110.0, typeof( CheeseWheel ), "Cheese Wheel", 1, 1044253 );
            AddRes( index, typeof( Tortilla ), "Tortilla", 1, 1044253 );
            AddRes( index, typeof( EnchiladaSauce ), "Enchilada Sauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChickenEnchilada ), "Dinners", "chicken enchilada", 75.0, 110.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( Tortilla ), "Tortilla", 1, 1044253 );
            AddRes( index, typeof( EnchiladaSauce ), "Enchilada Sauce", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Lasagna ), "Dinners", "lasagna", 75.0, 105.0, typeof( PastaNoodles ), "Pasta Noodles", 3, 1044253 );
            AddRes( index, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( CheeseWheel ), "Cheese Wheel", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( LemonChicken ), "Dinners", "lemon chicken", 60.0, 90.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( Lemon ), "Lemon", 1, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Herbs", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( OrangeChicken ), "Dinners", "orange chicken", 65.0, 95.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( Orange ), "Orange", 1, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Herbs", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );

            index = AddCraft( typeof( VealParmesian ), "Dinners", "veal parmesian", 75.0, 110.0, typeof( RawLambLeg ), "Raw Lamb Leg", 2, 1044253 );
            AddRes( index, typeof( TomatoSauce ), "Tomato Sauce", 1, 1044253 );
            AddRes( index, typeof( CheeseWheel ), "Cheese Wheel", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );
            #endregion

            #region Food
            index = AddCraft( typeof( BroccoliCheese ), "Food", "broccoli and cheese", 60.0, 80.0, typeof( Broccoli ), "Broccoli", 5, 1044253 );
            AddRes( index, typeof( CheeseSauce ), "Cheese Sauce", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BroccoliCaulCheese ), "Food", "broccoli, cauliflower and cheese", 60.0, 80.0, typeof( Broccoli ), "Broccoli", 5, 1044253 );
            AddRes( index, typeof( Cauliflower ), "Cauliflower", 2, 1044253 );
            AddRes( index, typeof( CheeseSauce ), "Cheese Sauce", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CauliflowerCheese ), "Food", "cauliflower and cheese", 60.0, 80.0, typeof( Cauliflower ), "Cauliflower", 5, 1044253 );
            AddRes( index, typeof( CheeseSauce ), "Cheese Sauce", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ChickenPie ), "Food", "chicken pie", 70.0, 100.0, typeof( RawBird ), "Raw Bird", 1, 1044253 );
            AddRes( index, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( MixedVegetables ), "mixed vegetables", 1, 1044253 );
            AddRes( index, typeof( Gravy ), "Gravy", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BeefPie ), "Food", "beef pie", 70.0, 100.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( MixedVegetables ), "Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( Gravy ), "Gravy", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Meatballs ), "Food", "meatballs", 45.0, 55.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( BreadLoaf ), "Bread", 1, 1044253 );
            AddRes( index, typeof( Eggs ), "Eggs", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Meatloaf ), "Food", "meatloaf", 45.0, 55.0, typeof( GroundBeef ), "Ground Beef", 2, 1044253 );
            AddRes( index, typeof( Eggs ), "Eggs", 2, 1044253 );
            AddRes( index, typeof( Onion ), "Onion", 2, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PotatoStrings ), "Food", "potato strings", 20.0, 40.0, typeof( Potato ), "Potato", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Quiche ), "Food", "quiche", 40.0, 65.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( Eggs ), "Eggs", 1, 1044253 );
            AddRes( index, typeof( RawHamSlices ), "Raw Ham Slices", 3, 1044253 );
            AddRes( index, typeof( Onion ), "Onion", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( ShepherdsPie ), "Food", "shepher's pie", 70.0, 100.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( BowlMashedPotatos ), "Bowl of Mashed Potatos", 1, 1044253 );
            AddRes( index, typeof( Corn ), "Corn", 2, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( TurkeyPie ), "Food", "turkey pie", 70.0, 100.0, typeof( PieMix ), "Pie Mix", 1, 1044253 );
            AddRes( index, typeof( SlicedTurkey ), "Sliced Turkey", 2, 1044253 );
            AddRes( index, typeof( MixedVegetables ), "Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( Gravy ), "Gravy", 1, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Pizzas
            index = AddCraft( typeof( CheesePizza ), "Pizzas", "cheese pizza", 50.0, 90.0, typeof( UncookedPizza ), "Uncooked Pizza", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( SausagePizza ), "Pizzas", 1044517, 50.0, 90.0, typeof( UncookedSausagePizza ), 1044520, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( HamPineapplePizza ), "Pizzas", "ham and pineapple pizza", 55.0, 100.0, typeof( UncookedPizza ), "Uncooked Pizza", 1, 1044253 );
            AddRes( index, typeof( RawHamSlices ), "Raw Ham Slices", 1, 1044253 );
            AddRes( index, typeof( Pineapple ), "Pineapple", 2, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( MushroomOnionPizza ), "Pizzas", "mushroom and onion pizza", 55.0, 100.0, typeof( UncookedPizza ), "Uncooked Pizza", 1, 1044253 );
            AddRes( index, typeof( TanMushroom ), "Tan Mushrooms", 3, 1044253 );
            AddRes( index, typeof( Onion ), "Onion", 3, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( SausOnionMushPizza ), "Pizzas", "sausage onion and mushroom pizza", 55.0, 100.0, typeof( UncookedPizza ), "Uncooked Pizza", 1, 1044253 );
            AddRes( index, typeof( Sausage ), "Sausage", 2, 1044253 );
            AddRes( index, typeof( Onion ), "Onion", 2, 1044253 );
            AddRes( index, typeof( RedMushroom ), "Red Mushrooms", 2, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( TacoPizza ), "Pizzas", "taco pizza", 60.0, 100.0, typeof( UncookedPizza ), "Uncooked Pizza", 1, 1044253 );
            AddRes( index, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( CheeseWheel ), "CheeseWheel", 1, 1044253 );
            AddRes( index, typeof( EnchiladaSauce ), "Enchilada Sauce", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( VeggiePizza ), "Pizzas", "vegetable pizza", 55.0, 100.0, typeof( UncookedPizza ), "Uncooked Pizza", 1, 1044253 );
            AddRes( index, typeof( MixedVegetables ), "Mixed Vegetables", 1, 1044523 );
            SetNeedOven( index, true );
            #endregion
        }
    }
}