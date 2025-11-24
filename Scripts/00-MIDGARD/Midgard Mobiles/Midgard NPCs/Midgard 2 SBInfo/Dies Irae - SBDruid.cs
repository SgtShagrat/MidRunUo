using System.Collections.Generic;

using Midgard.Items;

using Server.Items;

namespace Server.Mobiles
{
    public class SBDruid : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBDruid()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            private const int QuantityScalar = 2;
            private const int PriceScalar = 2;

            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( typeof( BlackPearl ), 5 / PriceScalar, 50, 0xF7A, 0 ) );
                Add( new GenericBuyInfo( typeof( Bloodmoss ), 5 / PriceScalar, 50, 0xF7B, 0 ) );
                Add( new GenericBuyInfo( typeof( Garlic ), 3 / PriceScalar, 50, 0xF84, 0 ) );
                Add( new GenericBuyInfo( typeof( Ginseng ), 3 / PriceScalar, 50, 0xF85, 0 ) );
                Add( new GenericBuyInfo( typeof( MandrakeRoot ), 3 / PriceScalar, 50, 0xF86, 0 ) );
                Add( new GenericBuyInfo( typeof( Nightshade ), 3 / PriceScalar, 50, 0xF88, 0 ) );
                Add( new GenericBuyInfo( typeof( SpidersSilk ), 3 / PriceScalar, 50, 0xF8D, 0 ) );
                Add( new GenericBuyInfo( typeof( SulfurousAsh ), 3 / PriceScalar, 50, 0xF8C, 0 ) );

                Add( new GenericBuyInfo( typeof( BatWing ), 3 / PriceScalar, 50 * QuantityScalar, 0xF78, 0 ) );
                Add( new GenericBuyInfo( typeof( GraveDust ), 3 / PriceScalar, 50 * QuantityScalar, 0xF8F, 0 ) );
                Add( new GenericBuyInfo( typeof( DaemonBlood ), 6 / PriceScalar, 50 * QuantityScalar, 0xF7D, 0 ) );
                Add( new GenericBuyInfo( typeof( NoxCrystal ), 6 / PriceScalar, 50 * QuantityScalar, 0xF8E, 0 ) );
                Add( new GenericBuyInfo( typeof( PigIron ), 5 / PriceScalar, 50 * QuantityScalar, 0xF8A, 0 ) );
                Add( new GenericBuyInfo( "Petrified Wood", typeof( PetrifiedWood ), 5 / PriceScalar, 50 * QuantityScalar, 0x97A, 0x46C ) );
                Add( new GenericBuyInfo( "Spring Water", typeof( SpringWater ), 5 / PriceScalar, 50 * QuantityScalar, 0xE24, 0x47F ) );
                Add( new GenericBuyInfo( "Destroying Angel", typeof( DestroyingAngel ), 5 / PriceScalar, 50 * QuantityScalar, 0xE1F, 0x290 ) );
                Add( new GenericBuyInfo( "Kindlings", typeof( Kindling ), 5 / PriceScalar, 50 * QuantityScalar, 0xDE1, 0 ) );
                Add( new GenericBuyInfo( "Fertile Dirt", typeof( FertileDirt ), 5 / PriceScalar, 50 * QuantityScalar, 0xF81, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add( typeof( PetrifiedWood ), 1 );
                Add( typeof( SpringWater ), 1 );
                Add( typeof( DestroyingAngel ), 1 );
            }
        }
    }
}
