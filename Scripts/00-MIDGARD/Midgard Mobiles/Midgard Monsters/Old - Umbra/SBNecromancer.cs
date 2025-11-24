using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBNecromancer : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBNecromancer()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( typeof( BatWing ), 3 / 2, 50, 0xF78, 0 ) );
                Add( new GenericBuyInfo( typeof( GraveDust ), 3 / 2, 50, 0xF8F, 0 ) );
                Add( new GenericBuyInfo( typeof( DaemonBlood ), 5 / 2, 50, 0xF7D, 0 ) );
                Add( new GenericBuyInfo( typeof( NoxCrystal ), 5 / 2, 50, 0xF8E, 0 ) );
                Add( new GenericBuyInfo( typeof( PigIron ), 5 / 2, 50, 0xF8A, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add( typeof( BatWing ), 1 );
                Add( typeof( GraveDust ), 1 );
                Add( typeof( DaemonBlood ), 1 );
                Add( typeof( NoxCrystal ), 1 );
                Add( typeof( PigIron ), 1 );
            }
        }
    }
}