using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.CheeseCrafting
{
	public class SBMasterShepherd : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBMasterShepherd()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
                Add( new GenericBuyInfo( typeof( CheeseForm ), 500, 20, 0x0E78, 0 ) );
                Add( new GenericBuyInfo( typeof( MilkBucket ), 1000, 20, 0x0FFA, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			}
		}
	}
}