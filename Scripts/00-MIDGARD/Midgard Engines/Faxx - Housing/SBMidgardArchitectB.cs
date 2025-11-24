/*using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMidgardArchitectB : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBMidgardArchitectB()
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

                Add( new GenericBuyInfo( typeof( HCD_7x7TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_7x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_7x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_7x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_7x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_7x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x7TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_8x13TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x7TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_9x13TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x7TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_10x13TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x7TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_11x13TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x7TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_12x13TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x8TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x9TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x10TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x11TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x12TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
                Add( new GenericBuyInfo( typeof( HCD_13x13TwoStoryFoundation ), HCD_TwoStoryFoundation.VendorPrice, 1, CommittmentDeed.ID, 0 ) );
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