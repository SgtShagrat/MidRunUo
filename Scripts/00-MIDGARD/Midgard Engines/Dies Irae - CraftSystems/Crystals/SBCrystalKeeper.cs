/***************************************************************************
 *                                  SBCrystalKeeper.cs
 *                            		------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.OldCraftSystem;

using Server.Items;
using Midgard.Engines.Craft;
using Midgard.Items;

namespace Server.Mobiles
{
    public class SBCrystalKeeper : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBCrystalKeeper()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( typeof( CrystalBrazierRecipe ), 1, 10000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalThroneEastRecipe ), 1, 20000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalThroneSouthRecipe ), 1, 20000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalTableEastAddonDeedRecipe ), 1, 15000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalTableSouthAddonDeedRecipe ), 1, 15000, 0x2831, 0 ) );

                Add( new GenericBuyInfo( typeof( CrystalAltarAddonDeedRecipe ), 1, 20000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalPedestralSmallRecipe ), 1, 5000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalPedestralMediumRecipe ), 1, 10000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalPedestralLargeEmptyRecipe ), 1, 20000, 0x2831, 0 ) );

                Add( new GenericBuyInfo( typeof( CrystalPedestralLargeRecipe ), 1, 30000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalBullSouthAddonDeedRecipe ), 1, 50000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalBullEastAddonDeedRecipe ), 1, 50000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalBeggarStatueSouthRecipe ), 1, 30000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalBeggarStatueEastRecipe ), 1, 30000, 0x2831, 0 ) );

                Add( new GenericBuyInfo( typeof( CrystalSupplicantStatueSouthRecipe ), 1, 30000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalSupplicantStatueEastRecipe ), 1, 30000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalRunnerStatueSouthRecipe ), 1, 30000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( CrystalRunnerStatueEastRecipe ), 1, 30000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( PurpleCrystalRecipe ), 1, 10000, 0x2831, 0 ) );

                Add( new GenericBuyInfo( typeof( GreenCrystalRecipe ), 1, 10000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( MagicCrystalBallRecipe ), 1, 100000, 0x2831, 0 ) );
                Add( new GenericBuyInfo( typeof( GlobeOfSosariaAddonRecipe ), 1, 200000, 0x2831, 0 ) );

                Add( new GenericBuyInfo( typeof( CrystalCraftingBook ), 50, 10, 0xEFA, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add( typeof( RawCrystal ), 100 );
                Add( typeof( LittleUnshapedCrystal ), 200 );
                Add( typeof( LargeUnshapedCrystal ), 500 );
                Add( typeof( TallUnshapedCrystal ), 200 );
                Add( typeof( CrystalPillar ), 1000 );
            }
        }
    }
}