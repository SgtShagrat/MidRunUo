using System;
using System.Collections.Generic;
using Server.Items;

using Midgard.Engines.OldCraftSystem;
using Midgard.Items;

namespace Server.Mobiles
{
	public class SBBeekeeper : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBeekeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				#region mod by Dies Irae
				// Add( new GenericBuyInfo( typeof( JarHoney ), 3, 20, 0x9EC, 0 ) );
				// Add( new GenericBuyInfo( typeof( Beeswax ), 2, 20, 0x1422, 0 ) );

				Add( new GenericBuyInfo( typeof( JarHoney ), 				3, 	    20, 		0x9EC,  0 ) );
				Add( new GenericBuyInfo( typeof( Beeswax ), 				1, 	    20, 		0x1422, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankCandle ), 			200, 	5, 		0x1433, 1150 ) );
				Add( new GenericBuyInfo( typeof( CandleFitSkull ), 			100, 	20, 	0x1AE3, 0 ) );
				Add( new GenericBuyInfo( typeof( CandleWick ), 				100, 	20, 	0x979, 	1052 ) );
				Add( new GenericBuyInfo( typeof( EssenceOfLove ), 			10000,  2, 		0x1C18, 1157 ) );
				Add( new GenericBuyInfo( typeof( WaxCraftingPot ), 			1000, 	20, 	0x142A, 0 ) );
				Add( new GenericBuyInfo( typeof( ApiWaxProcessingPot ), 	1000, 	20, 	2532, 	0 ) );
				Add( new GenericBuyInfo( typeof( HiveTool ), 				500, 	20, 	2549, 	0 ) );
				Add( new GenericBuyInfo( typeof( ApiBeeHiveDeed ), 			10000, 	20, 	0x14F0, 0 ) );

                Add( new GenericBuyInfo( typeof( WaxCraftingBook ), 50, 10, 0xEFA, 0 ) );
				#endregion
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( JarHoney ), 1 );
				Add( typeof( Beeswax ), 1 );
			}
		}
	}
}