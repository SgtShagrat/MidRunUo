/***************************************************************************
 *                                     SBGardener.cs
 *                            		------------------
 *  begin                	: Giugno, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Classe SB per il giardiniere
 * 
 ***************************************************************************/

using System.Collections.Generic;
using Midgard.Engines.PlantSystem;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.WineCrafting
{
    public class SBGardener : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBGardener()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( typeof( CarrotSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CornSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CottonSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CabbageSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( WheatSeed ), 30, 20, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( LettuceSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( OnionSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( FlaxSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( PumpkinSeed ), 30, 20, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( TurnipSeed ), 30, 20, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( LightFertilizerPotion ), 500, 20, 0xF0E, 0 ) );
                Add( new GenericBuyInfo( typeof( MediumFertilizerPotion ), 2000, 20, 0xF0E, 0 ) );
                Add( new GenericBuyInfo( typeof( HeavyFertilizerPotion ), 5000, 20, 0xF0E, 0 ) );

                Add( new GenericBuyInfo( typeof( LightFungicidePotion ), 500, 20, 0xF0E, 0 ) );
                Add( new GenericBuyInfo( typeof( MediumFungicidePotion ), 1000, 20, 0xF0E, 0 ) );
                Add( new GenericBuyInfo( typeof( HeavyFungicidePotion ), 2000, 20, 0xF0E, 0 ) );

                Add( new GenericBuyInfo( typeof( LightPesticidePotion ), 500, 20, 0xF0E, 0 ) );
                Add( new GenericBuyInfo( typeof( MediumPesticidePotion ), 1000, 20, 0xF0E, 0 ) );
                Add( new GenericBuyInfo( typeof( HeavyPesticidePotion ), 2000, 20, 0xF0E, 0 ) );

                Add( new GenericBuyInfo( typeof( WateringCan ), 100, 10, 0x2004, 0 ) );

                Add( new GenericBuyInfo( typeof( GardeningShovel ), 100, 10, 0xF39, 0 ) );

                Add( new GenericBuyInfo( typeof( GarlicSeed ), 200, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( MandrakeSeed ), 200, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( GinsengSeed ), 200, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( NightshadeSeed ), 200, 30, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( AppleTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( BananaTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CedarTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CherryTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CoconutPalmSeed ), 2000, 30, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( CommonTreeSeed ), 1000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( CypressTreeSeed ), 1000, 30, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( DatePalmSeed ), 3000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( HedgeSeed ), 500, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( MapleTreeSeed ), 3000, 30, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( OakTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( OhiiTreeSeed ), 2000, 30, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( PeachTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( PearTreeSeed ), 2000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( PlumTreeSeed ), 2000, 30, 0xF27, 0 ) );

                Add( new GenericBuyInfo( typeof( WalnutTreeSeed ), 1000, 30, 0xF27, 0 ) );
                Add( new GenericBuyInfo( typeof( WillowTreeSeed ), 1000, 30, 0xF27, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public override double Demultiplicator
            {
                get { return 0.25; }
            }

            public InternalSellInfo()
            {
                Add( typeof( CarrotSeed ), 3 );
                Add( typeof( CornSeed ), 3 );
                Add( typeof( CottonSeed ), 3 );
                Add( typeof( CabbageSeed ), 3 );
                Add( typeof( WheatSeed ), 3 );

                Add( typeof( LettuceSeed ), 3 );
                Add( typeof( OnionSeed ), 3 );
                Add( typeof( FlaxSeed ), 3 );
                Add( typeof( PumpkinSeed ), 3 );
                Add( typeof( TurnipSeed ), 3 );

                Add( typeof( HeavyFertilizerPotion ), 50 );
                Add( typeof( HeavyFungicidePotion ), 20 );
                Add( typeof( HeavyPesticidePotion ), 20 );

                Add( typeof( GarlicSeed ), 20 );
                Add( typeof( MandrakeSeed ), 20 );
                Add( typeof( GinsengSeed ), 20 );
                Add( typeof( NightshadeSeed ), 20 );

                Add( typeof( AppleTreeSeed ), 20 );
                Add( typeof( BananaTreeSeed ), 20 );
                Add( typeof( CedarTreeSeed ), 20 );
                Add( typeof( CherryTreeSeed ), 20 );
                Add( typeof( CoconutPalmSeed ), 20 );

                Add( typeof( DatePalmSeed ), 30 );
                Add( typeof( HedgeSeed ), 50 );
                Add( typeof( MapleTreeSeed ), 30 );

                Add( typeof( PeachTreeSeed ), 20 );
                Add( typeof( PearTreeSeed ), 20 );
                Add( typeof( PlumTreeSeed ), 20 );
            }
        }
    }
}