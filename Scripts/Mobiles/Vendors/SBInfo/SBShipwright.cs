using System;
using System.Collections.Generic;
using Server.Items;
using Server.Multis;

using Midgard.Multis;

namespace Server.Mobiles
{
	public class SBShipwright : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBShipwright()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "1041205", typeof( SmallBoatDeed ), 10177, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041206", typeof( SmallDragonBoatDeed ), 10177, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041207", typeof( MediumBoatDeed ), 11552, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041208", typeof( MediumDragonBoatDeed ), 11552, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041209", typeof( LargeBoatDeed ), 12927, 20, 0x14F2, 0 ) );
				Add( new GenericBuyInfo( "1041210", typeof( LargeDragonBoatDeed ), 12927, 20, 0x14F2, 0 ) );

                #region mod by Dies Irae
                Add( new GenericBuyInfo( typeof( VesselDeed ), 500000, 1, 0x14F2, 0 ) );
                Add( new GenericBuyInfo( typeof( GalleonDeed ), 500000, 1, 0x14F2, 0 ) );
                Add( new GenericBuyInfo( typeof( BoatDecorator ), 500, 10, 0xFC1, 0 ) );
                #endregion
            }
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				//You technically CAN sell them back, *BUT* the vendors do not carry enough money to buy with
			}
		}
	}
}