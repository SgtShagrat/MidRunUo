using System;
using System.Collections.Generic;
using Server.Items;

using Midgard.Engines.OldCraftSystem;

namespace Server.Mobiles
{
	public class SBBowyer : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBowyer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( FletcherTools ), 2, 20, 0x1022, 0 ) );
	
				#region modifica by dies irae per le 999 frecce
				Add( new GenericBuyInfo( typeof( Arrow ), 2, 50 * 5, 0xF3F, 0 ) );
				Add( new GenericBuyInfo( typeof( Bolt ), 5, 50 * 5, 0x1BFB, 0 ) );

                Add( new GenericBuyInfo( typeof( BowFletchingBook ), 50, 10, 0xEFA, 0 ) );
				#endregion
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( FletcherTools ), 1 );
			}
		}
	}
}