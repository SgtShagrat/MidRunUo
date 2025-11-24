using System; 
using System.Collections.Generic; 
using Server.Items;

using Midgard.Items;

namespace Server.Mobiles 
{ 
	public class SBFarmer : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBFarmer() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( typeof( Cabbage ), 5, 20, 0xC7B, 0 ) );
				Add( new GenericBuyInfo( typeof( Cantaloupe ), 6, 20, 0xC79, 0 ) );
				Add( new GenericBuyInfo( typeof( Carrot ), 3, 20, 0xC78, 0 ) );
				Add( new GenericBuyInfo( typeof( HoneydewMelon ), 7, 20, 0xC74, 0 ) );
				Add( new GenericBuyInfo( typeof( Squash ), 3, 20, 0xC72, 0 ) );
				Add( new GenericBuyInfo( typeof( Lettuce ), 5, 20, 0xC70, 0 ) );
				Add( new GenericBuyInfo( typeof( Onion ), 3, 20, 0xC6D, 0 ) );
				Add( new GenericBuyInfo( typeof( Pumpkin ), 11, 20, 0xC6A, 0 ) );
				Add( new GenericBuyInfo( typeof( GreenGourd ), 3, 20, 0xC66, 0 ) );
				Add( new GenericBuyInfo( typeof( YellowGourd ), 3, 20, 0xC64, 0 ) );
                Add( new GenericBuyInfo( typeof( Turnip ), 6, 20, 0xD3A, 0 ) ); // mod by Dies Irae
				Add( new GenericBuyInfo( typeof( Watermelon ), 7, 20, 0xC5C, 0 ) );
                // Add( new GenericBuyInfo( typeof( EarOfCorn ), 3, 20, 0xC7F, 0 ) ); // mod by Dies Irae
				Add( new GenericBuyInfo( typeof( Eggs ), 3, 20, 0x9B5, 0 ) );
				Add( new BeverageBuyInfo( typeof( Pitcher ), BeverageType.Milk, 7, 20, 0x9AD, 0 ) );
				Add( new GenericBuyInfo( typeof( Peach ), 3, 20, 0x9D2, 0 ) );
				Add( new GenericBuyInfo( typeof( Pear ), 3, 20, 0x994, 0 ) );
				Add( new GenericBuyInfo( typeof( Lemon ), 3, 20, 0x1728, 0 ) );
				Add( new GenericBuyInfo( typeof( Lime ), 3, 20, 0x172A, 0 ) );
				Add( new GenericBuyInfo( typeof( Grapes ), 3, 20, 0x9D1, 0 ) );
				Add( new GenericBuyInfo( typeof( Apple ), 3, 20, 0x9D0, 0 ) );
				Add( new GenericBuyInfo( typeof( SheafOfHay ), 2, 20, 0xF36, 0 ) );

                #region mod by Dies Irae

                #region Herbs
                Add( new GenericBuyInfo( typeof( TanGinger ), 50, 20, 0xF85, 0x413 ) );
                Add( new GenericBuyInfo( typeof( TanMushroom ), 50, 20, 0xD19, 0 ) );
                Add( new GenericBuyInfo( typeof( RedMushroom ), 50, 20, 0xD16, 0 ) );
                Add( new GenericBuyInfo( typeof( Mint ), 50, 20, 0x26B8, 0 ) );
                #endregion

                #region stalks
                Add( new GenericBuyInfo( typeof( Corn ), 2, 20, 0x0C81, 0 ) );
                Add( new GenericBuyInfo( typeof( EdibleSun ), 2, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( FieldCorn ), 2, 20, 0xC81, 0 ) );
                // Add( new GenericBuyInfo( typeof( TeaLeaf ), 2, 20, 0x103F, 0 ) );
                #endregion

                #region Trees.Fruits
                Add( new GenericBuyInfo( typeof( Almond ), 2, 20, 0x1AA2, 0 ) );
                Add( new GenericBuyInfo( typeof( Apricot ), 2, 20, 0x9D2, 0 ) );
                // Add( new GenericBuyInfo( typeof( Avocado ), 2, 20, 0xC80, 0 ) );
                Add( new GenericBuyInfo( typeof( Cherry ), 2, 20, 0x9D1, 0 ) );
                // Add( new GenericBuyInfo( typeof( Cranberry ), 2, 20, 0x9D1, 0 ) );
                // Add( new GenericBuyInfo( typeof( Grapefruit ), 2, 20, 0x9D0, 0 ) );
                // Add( new GenericBuyInfo( typeof( Kiwi ), 2, 20, 0xF8B, 0 ) );
                // Add( new GenericBuyInfo( typeof( Mango ), 2, 20, 0x9D0, 0 ) );
                Add( new GenericBuyInfo( typeof( Orange ), 2, 20, 0x9D0, 0 ) );
                Add( new GenericBuyInfo( typeof( Pineapple ), 2, 20, 0xFC4, 0 ) );
                // Add( new GenericBuyInfo( typeof( Pistacio ), 2, 20, 0x1AA2, 0 ) );
                // Add( new GenericBuyInfo( typeof( Plum ), 2, 20, 0x9D2, 0 ) );
                // Add( new GenericBuyInfo( typeof( Pomegranate ), 2, 20, 0x9D0, 0 ) );
                // Add( new GenericBuyInfo( typeof( SourApple ), 2, 20, 0x1039, 0 ) );
                // Add( new GenericBuyInfo( typeof( CocoaNut ), 2, 20, 0x1726, 0 ) );
                #endregion

                #region Trees.Beans
                Add( new GenericBuyInfo( typeof( CocoaNut ), 2, 20, 0x1726, 0 ) );
                Add( new GenericBuyInfo( typeof( CoffeeBean ), 2, 20, 0xC64, 0 ) );
                #endregion

                #endregion
            } 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( Pitcher ), 5 );
				Add( typeof( Eggs ), 1 );
				Add( typeof( Apple ), 1 );
				Add( typeof( Grapes ), 1 );
				Add( typeof( Watermelon ), 3 );
				Add( typeof( YellowGourd ), 1 );
				Add( typeof( GreenGourd ), 1 );
				Add( typeof( Pumpkin ), 5 );
				Add( typeof( Onion ), 1 );
				Add( typeof( Lettuce ), 2 );
				Add( typeof( Squash ), 1 );
				Add( typeof( Carrot ), 1 );
				Add( typeof( HoneydewMelon ), 3 );
				Add( typeof( Cantaloupe ), 3 );
				Add( typeof( Cabbage ), 2 );
				Add( typeof( Lemon ), 1 );
				Add( typeof( Lime ), 1 );
				Add( typeof( Peach ), 1 );
				Add( typeof( Pear ), 1 );
				Add( typeof( SheafOfHay ), 1 );
			} 
		} 
	} 
}