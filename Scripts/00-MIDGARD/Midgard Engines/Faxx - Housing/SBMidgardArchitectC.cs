/*using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMidgardArchitectC : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

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

                Add( new GenericBuyInfo( typeof( HCD_9x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x18ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x9ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x10ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x11ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x12ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x13ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_14x18ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x10ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x11ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x12ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x13ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_15x18ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x11ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x12ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x13ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_16x18ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x12ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x13ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_17x18ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_18x13ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_18x14ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_18x15ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_18x16ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_18x17ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_18x18ThreeStoryFoundation ), HCD_ThreeStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
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