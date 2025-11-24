using System;
using Midgard.Engines.OldCraftSystem;
using Server;
using Server.Items;
using Server.Engines.Craft;
using Midgard.Items;

namespace Midgard.Engines.AdvancedCooking
{
    public class DefBoiling : CraftSystem
    {
        public override string Name { get { return "Boiling"; } }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Boiling.xml", CraftSystem );

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
            get { return "<basefont color=#FFFFFF><CENTER>Midgard Advanced Cooking: Boiling</CENTER></basefont>"; }
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefBoiling();

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

        private DefBoiling()
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

            #region Soups and Stews
            index = AddCraft( typeof( ChickenNoodleSoup ), "Soups and Stews", "chicken noodle soup", 40.0, 80.0, typeof( CookedBird ), "cooked bird", 1, 1044253 );
            AddRes( index, typeof( PastaNoodles ), "Pasta Noodles", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( TomatoRice ), "Soups and Stews", "tomato and rice", 40.0, 80.0, typeof( Tomato ), "Tomato", 3, 1044253 );
            AddRes( index, typeof( BowlRice ), "Bowl of Rice", 1, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Herbs", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlOfStew ), "Soups and Stews", "beef stew", 45.0, 85.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( Gravy ), "Gravy", 1, 1044253 );
            AddRes( index, typeof( BowlCookedVeggies ), "Cooked Bowl of Vegetables", 1, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Herbs", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( TomatoSoup ), "Soups and Stews", "tomato soup", 30.0, 70.0, typeof( Tomato ), "Tomato", 5, 1044253 );
            AddRes( index, typeof( BasketOfHerbs ), "Herbs", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Vegetables
            index = AddCraft( typeof( BowlBeets ), "Vegetables", "bowl of beets", 20.0, 60.0, typeof( Beet ), "Beet", 4, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlBroccoli ), "Vegetables", "bowl of broccoli", 30.0, 70.0, typeof( Broccoli ), "Broccoli", 4, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlCauliflower ), "Vegetables", "bowl of cauliflower", 30.0, 70.0, typeof( Cauliflower ), "Cauliflower", 4, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlGreenBeans ), "Vegetables", "bowl of green beans", 20.0, 60.0, typeof( GreenBean ), "Green Beans", 20, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            AddRes( index, typeof( Bacon ), "Bacon", 3, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlRice ), "Vegetables", "bowl of rice", 0.0, 40.0, typeof( BagOfRicemeal ), "Bag of Rice", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlSpinach ), "Vegetables", "bowl of spinach", 20.0, 60.0, typeof( Spinach ), "Spinach", 8, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            AddRes( index, typeof( Vinegar ), "Vinegar", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlTurnips ), "Vegetables", "bowl of turnips", 40.0, 80.0, typeof( Turnip ), "Turnip", 4, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlMashedPotatos ), "Vegetables", "bowl of mashed potatos", 30.0, 70.0, typeof( Potato ), "Potato", 5, 1044253 );
            AddRes( index, typeof( Butter ), "Butter", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), "Milk", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( BowlCookedVeggies ), "Vegetables", "cooked bowl of vegetables", 20.0, 60.0, typeof( MixedVegetables ), "Mixed Vegetables", 1, 1044253 );
            AddRes( index, typeof( SoySauce ), "Soy Sauce", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( WoodenBowlCabbage ), "vegetables", "bowl of cabbage", 30.0, 70.0, typeof( Cabbage ), "Cabbage", 2, 1044253 );
            AddRes( index, typeof( Vinegar ), "Vinegar", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( WoodenBowlCarrot ), "Vegetables", "bowl of carrots", 20.0, 60.0, typeof( Carrot ), "Carrot", 12, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( WoodenBowlCorn ), "Vegetables", "bowl of corn", 25.0, 65.0, typeof( Corn ), "Corn", 3, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( WoodenBowlLettuce ), "Vegetables", "bowl of lettuce", 10.0, 50.0, typeof( Lettuce ), "Lettuce", 2, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( WoodenBowlPea ), "Vegetables", "bowl of peas", 10.0, 50.0, typeof( Peas ), "Peas", 20, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( PewterBowlOfPotatos ), "Vegetables", "bowl of potatos", 10.0, 50.0, typeof( Potato ), "Potato", 5, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( CornOnCob ), "Vegetables", "corn on the cob", 30.0, 70.0, typeof( Corn ), "Corn", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Dinners
            index = AddCraft( typeof( Spaghetti ), "Dinners", "spaghetti", 40.0, 80.0, typeof( PastaNoodles ), "Pasta Noodles", 3, 1044253 );
            AddRes( index, typeof( TomatoSauce ), "Tomato Sauce", 1, 1044253 );
            AddRes( index, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( FoodPlate ), "Plate", 1, "You need a plate!" );
            SetNeedOven( index, true );
            #endregion

            #region Food
            index = AddCraft( typeof( BowlOatmeal ), "Food", "bowl of oatmeal", 45.0, 85.0, typeof( BagOfOats ), "Bag of Oats", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            AddRes( index, typeof( JarHoney ), "Honey", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Hotdog ), "Food", "hotdog", 20.0, 60.0, typeof( GroundBeef ), "Ground Beef", 1, 1044253 );
            AddRes( index, typeof( GroundPork ), "Ground Pork", 1, 1044253 );
            AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
            AddRes( index, typeof( BreadLoaf ), "Bread", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( MacaroniCheese ), "Food", "macaroni and cheese", 40.0, 80.0, typeof( PastaNoodles ), "Pasta Noodles", 3, 1044253 );
            AddRes( index, typeof( CheeseSauce ), "Cheese Sauce", 1, 1044253 );
            SetNeedOven( index, true );

            index = AddCraft( typeof( Popcorn ), "Food", "popcorn", 30.0, 70.0, typeof( Corn ), "Corn", 2, 1044253 );
            AddRes( index, typeof( CookingOil ), "Cooking Oil", 1, 1044253 );
            AddRes( index, typeof( Butter ), "Butter", 1, 1044253 );
            SetNeedOven( index, true );
            #endregion

            #region Other
            index = AddCraft( typeof( FruitJam ), "Other", "fruit jam", 40.0, 80.0, typeof( FruitBasket ), "Fruit Basket", 1, 1044253 );
            AddRes( index, typeof( BagOfSugar ), "Bag of Sugar", 1, 1044253 );
            SetNeedOven( index, true );
            #endregion
        }
    }
}