/***************************************************************************
 *                               Dies Irae - SBSantaClaus.cs
 *                            ---------------------------------
 *   begin                : 20 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Midgard.Engines.Events.Items;

using Server.Items;

namespace Server.Mobiles
{
    public class SBSantaClaus : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBSantaClaus()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( "candy cane", typeof( ChristmasCandyCane ), 100, 5, 0x2BDD, 0 ) );
                Add( new GenericBuyInfo( "gingerbread cookie", typeof( ChristmasGingerBreadCookie ), 100, 5, 0x2BE1, 0 ) );
                Add( new GenericBuyInfo( "holiday topiary", typeof( ChristmasDecorativeTopiary ), 100, 5, 0x2378, 0 ) );
                Add( new GenericBuyInfo( "holiday cactus", typeof( ChristmasFestiveCactus ), 100, 5, 0x2376, 0 ) );
                Add( new GenericBuyInfo( "snowy tree", typeof( ChristmasSnowyTree ), 100, 5, 0x2377, 0 ) );

                Add( new GenericBuyInfo( "holiday bell", typeof( ChristmasHolidayBell ), 100, 5, 0x1C12, 0 ) );
                Add( new GenericBuyInfo( "mistletoe", typeof( ChristmasMistletoeDeed ), 100, 5, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "glacial snow", typeof( ChristmasPileOfGlacialSnow ), 100, 5, 0x913, 0x480 ) );
                // Add( new GenericBuyInfo( "snow globe", typeof( Christmas2008.SnowGlobe2008 ), 100, 5, 0xE2D, 0 ) );
                // Add( new GenericBuyInfo( "snow man", typeof( Christmas2008.Snowman2008 ), 100, 5, 0x2328, 0 ) );
                // Add( new GenericBuyInfo( "snow pile", typeof( Christmas2008.SnowPile2008 ), 100, 5, 0x913, 0x481 ) );
                // Add( new GenericBuyInfo( "light of winter", typeof( Christmas2008.LightOfTheWinterSolstice2008 ), 100, 5, 0x236E, 0 ) );

                //Add( new GenericBuyInfo( "red stokin", typeof( Christmas2008.RedStockin ), 100, 5, 0x2BDB, 0 ) );
                //Add( new GenericBuyInfo( "green stokin", typeof( Christmas2008.GreenStockin ), 100, 5, 0x2BD9, 0 ) );

                // Add( new GenericBuyInfo( "gingerbread house", typeof( Christmas2008.GingerbreadHouseAddonDeed ), 100, 5, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( "olive reindeer", typeof( OliveTheOtherReindeer ), 1000, 5, 0x3A5F, 0 ) );
                Add( new GenericBuyInfo( "holy reindeer", typeof( SantasReindeer1 ), 100, 5, 0x3A55, 0 ) );
                Add( new GenericBuyInfo( "holy reindeer", typeof( SantasReindeer2 ), 100, 5, 0x3A67, 0 ) );
                Add( new GenericBuyInfo( "holy reindeer", typeof( SantasReindeer3 ), 100, 5, 0x3A6F, 0 ) );
                Add( new GenericBuyInfo( "holy sleigh", typeof( SantasSleighDeed ), 100, 5, 0x14F0, 0 ) );

                //Add( new GenericBuyInfo( "christmas brassiere", typeof( Christmas2008.ReggisenoNatalizio2008 ), 100, 5, 5150, 0 ) );
                //Add( new GenericBuyInfo( "christmas hat", typeof( Christmas2008.CappellinoNatalizio2008 ), 100, 5, 5149, 0 ) );
                //Add( new GenericBuyInfo( "christmas skirt", typeof( Christmas2008.GonnellinoNatalizio2008 ), 100, 5, 5151, 0 ) );

                //Add( new GenericBuyInfo( "wreath", typeof( WreathDeed ), 100, 5, 0x14F0, 0 ) );
                //Add( new GenericBuyInfo( "blue snow flake", typeof( Christmas2008.BlueSnowflake2008 ), 100, 5, 0x232E, 0 ) );
                //Add( new GenericBuyInfo( "white snow flake", typeof( Christmas2008.WhiteSnowflake2008 ), 100, 5, 0x232F, 0 ) );
                //Add( new GenericBuyInfo( "red poinsettia", typeof( Christmas2008.RedPoinsettia2008 ), 100, 5, 0x2330, 0 ) );
                //Add( new GenericBuyInfo( "white poinsettia", typeof( Christmas2008.WhitePoinsettia2008 ), 100, 5, 0x2331, 0 ) );

                Add( new GenericBuyInfo( "holiday tree", typeof( HolidayTreeDeed ), 2000, 5, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( "gift box", typeof( GiftBox ), 100, 30, 0x232A, 0 ) );
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
