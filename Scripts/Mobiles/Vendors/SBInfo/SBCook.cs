using System; 
using System.Collections.Generic; 
using Server.Items; 

using Midgard.Engines.CheeseCrafting;
using Midgard.Engines.OldCraftSystem;
using Midgard.Items;

namespace Server.Mobiles 
{ 
	public class SBCook : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBCook() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( BreadLoaf ), 5, 20, 0x103B, 0 ) );
				Add( new GenericBuyInfo( typeof( BreadLoaf ), 5, 20, 0x103C, 0 ) );
				Add( new GenericBuyInfo( typeof( ApplePie ), 7, 20, 0x1041, 0 ) ); //OSI just has Pie, not Apple/Fruit/Meat
				Add( new GenericBuyInfo( typeof( Cake ), 13, 20, 0x9E9, 0 ) );
				Add( new GenericBuyInfo( typeof( Muffins ), 3, 20, 0x9EA, 0 ) );

				Add( new GenericBuyInfo( typeof( CheeseWheel ), 21, 10, 0x97E, 0 ) );
				Add( new GenericBuyInfo( typeof( CookedBird ), 17, 20, 0x9B7, 0 ) );
				Add( new GenericBuyInfo( typeof( LambLeg ), 8, 20, 0x160A, 0 ) );
				Add( new GenericBuyInfo( typeof( ChickenLeg ), 5, 20, 0x1608, 0 ) );

				Add( new GenericBuyInfo( typeof( WoodenBowlOfCarrots ), 3, 20, 0x15F9, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBowlOfCorn ), 3, 20, 0x15FA, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBowlOfLettuce ), 3, 20, 0x15FB, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBowlOfPeas ), 3, 20, 0x15FC, 0 ) );
				Add( new GenericBuyInfo( typeof( EmptyPewterBowl ), 2, 20, 0x15FD, 0 ) );
				Add( new GenericBuyInfo( typeof( PewterBowlOfCorn ), 3, 20, 0x15FE, 0 ) );
				Add( new GenericBuyInfo( typeof( PewterBowlOfLettuce ), 3, 20, 0x15FF, 0 ) );
				Add( new GenericBuyInfo( typeof( PewterBowlOfPeas ), 3, 20, 0x1600, 0 ) );
				Add( new GenericBuyInfo( typeof( PewterBowlOfPotatos ), 3, 20, 0x1601, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBowlOfStew ), 3, 20, 0x1604, 0 ) );
				Add( new GenericBuyInfo( typeof( WoodenBowlOfTomatoSoup ), 3, 20, 0x1606, 0 ) );

				Add( new GenericBuyInfo( typeof( RoastPig ), 106, 20, 0x9BB, 0 ) );
				Add( new GenericBuyInfo( typeof( SackFlour ), 3, 20, 0x1039, 0 ) );
				Add( new GenericBuyInfo( typeof( JarHoney ), 3, 20, 0x9EC, 0 ) );
				Add( new GenericBuyInfo( typeof( RollingPin ), 2, 20, 0x1043, 0 ) );
				Add( new GenericBuyInfo( typeof( FlourSifter ), 2, 20, 0x103E, 0 ) );
				Add( new GenericBuyInfo( "1044567", typeof( Skillet ), 3, 20, 0x97F, 0 ) );

                #region mod by Dies Irae
                // Tools
                Add( new GenericBuyInfo( typeof( FoodPlate ), 5, 20, 0x9D7, 0 ) );
                Add( new GenericBuyInfo( typeof( BakersBoard ), 10, 20, 0x14EA, 0 ) );
                Add( new GenericBuyInfo( typeof( CooksCauldron ), 10, 20, 0x9ED, 0 ) );
                Add( new GenericBuyInfo( typeof( FryingPan ), 10, 20, 0x9E2, 0 ) );

                // Ingredients
                Add( new GenericBuyInfo( typeof( CoffeeBean ), 500, 20, 0xC64, 0 ) );
                Add( new GenericBuyInfo( typeof( FieldCorn ), 50, 20, 0xC81, 0 ) );
                Add( new GenericBuyInfo( typeof( OatSheath ), 50, 20, 0x1EBD, 0 ) );
                Add( new GenericBuyInfo( typeof( RiceSheath ), 50, 20, 0x1A9D, 0 ) );
                Add( new GenericBuyInfo( typeof( Soybean ), 50, 20, 0x1EBD, 0 ) );
                Add( new GenericBuyInfo( typeof( Sugarcane ), 500, 20, 0x1A9D, 0 ) );
                Add( new GenericBuyInfo( typeof( SackcornFlour ), 100, 20, 0x1039, 0 ) );
                Add( new GenericBuyInfo( typeof( CowMilkBottle ), 20, 20, 0x0f09, 0 ) );

                Add( new GenericBuyInfo( typeof( CookingBook ), 50, 10, 0xEFA, 0 ) );
                Add( new GenericBuyInfo( typeof( BakingBook ), 50, 10, 0xEFA, 0 ) ); 
                Add( new GenericBuyInfo( typeof( BoilingBook ), 50, 10, 0xEFA, 0 ) );
                Add( new GenericBuyInfo( typeof( GrillingBook ), 50, 10, 0xEFA, 0 ) );
                #endregion
            } 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{
            #region mod by Dies Irae
            public override double Demultiplicator
            {
                get { return 0.6; }
            }
            #endregion

			public InternalSellInfo() 
			{ 
				Add( typeof( CheeseWheel ), 12 );
				Add( typeof( CookedBird ), 8 );
				Add( typeof( RoastPig ), 53 );
				Add( typeof( Cake ), 5 );
				Add( typeof( JarHoney ), 1 );
				Add( typeof( SackFlour ), 1 );
				Add( typeof( BreadLoaf ), 2 );
				Add( typeof( ChickenLeg ), 3 );
				Add( typeof( LambLeg ), 4 );
				Add( typeof( Skillet ), 1 );
				Add( typeof( FlourSifter ), 1 );
				Add( typeof( RollingPin ), 1 );
				Add( typeof( Muffins ), 1 );
				Add( typeof( ApplePie ), 3 );

				Add( typeof( WoodenBowlOfCarrots ), 1 );
				Add( typeof( WoodenBowlOfCorn ), 1 );
				Add( typeof( WoodenBowlOfLettuce ), 1 );
				Add( typeof( WoodenBowlOfPeas ), 1 );
				Add( typeof( EmptyPewterBowl ), 1 );
				Add( typeof( PewterBowlOfCorn ), 1 );
				Add( typeof( PewterBowlOfLettuce ), 1 );
				Add( typeof( PewterBowlOfPeas ), 1 );
				Add( typeof( PewterBowlOfPotatos ), 1 );
				Add( typeof( WoodenBowlOfStew ), 1 );
				Add( typeof( WoodenBowlOfTomatoSoup ), 1 );
			} 
		} 
	} 
}