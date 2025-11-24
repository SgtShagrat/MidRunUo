/*using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMidgardArchitectA : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBMidgardArchitectA()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return m_SellInfo;
            }
        }
        public override ArrayList BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( "1041280", typeof( InteriorDecorator ), 10001, 20, 0xFC1, 0 ) ); // mod by Dies Irae

                Add( new GenericBuyInfo( typeof( HCD_TwoStoryVilla ), HCD_TwoStoryVilla.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_SandStonePatio ), HCD_SandStonePatio.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_GuildHouse ), HCD_GuildHouse.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_TwoStoryHouseA ), HCD_TwoStoryHouseA.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_TwoStoryHouseB ), HCD_TwoStoryHouseB.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_LargePatioHouse ), HCD_LargePatioHouse.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_LargeMarbleHouse ), HCD_LargeMarbleHouse.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_Tower ), HCD_Tower.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_Keep ), HCD_Keep.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_Castle ), HCD_Castle.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {

            }
        }
    }
}*/