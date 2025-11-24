/***************************************************************************
 *                               SBFillableHouseDeed.cs
 *
 *   begin                : 27 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Midgard.Multis.Deeds;

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public class SBFillableHouseDeed : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo
        {
            get { return m_SellInfo; }
        }

        public override List<GenericBuyInfo> BuyInfo
        {
            get { return m_BuyInfo; }
        }

        #region Nested type: InternalBuyInfo
        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add( new GenericBuyInfo( "fillable deed to a stone-and-plaster house", typeof( FillableStonePlasterHouseDeed ), 4380, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a field stone house", typeof( FillableFieldStoneHouseDeed ), 4380, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small brick house", typeof( FillableSmallBrickHouseDeed ), 4380, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a wooden house", typeof( FillableWoodHouseDeed ), 4380, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a wood-and-plaster house", typeof( FillableWoodPlasterHouseDeed ), 4380, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a thatched-roof cottage", typeof( FillableThatchedRoofCottageDeed ), 4380, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a brick house", typeof( FillableBrickHouseDeed ), 14450, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a two-story wood-and-plaster house", typeof( FillableTwoStoryWoodPlasterHouseDeed ), 19240, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a tower", typeof( FillableTowerDeed ), 43320 * 2, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small stone keep", typeof( FillableKeepDeed ), 66520 * 3, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a castle", typeof( FillableCastleDeed ), 102280 * 3, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a large house with patio", typeof( FillableLargePatioDeed ), 15280, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a marble house with patio", typeof( FillableLargeMarbleDeed ), 19200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small stone tower", typeof( FillableSmallTowerDeed ), 8850, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a two story log cabin", typeof( FillableLogCabinDeed ), 9780, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone house with patio", typeof( FillableSandstonePatioDeed ), 9090, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a two story villa", typeof( FillableVillaDeed ), 13650, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small stone workshop", typeof( FillableStoneWorkshopDeed ), 6060, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small marble workshop", typeof( FillableMarbleWorkshopDeed ), 6300, 20, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( "fillable deed to a plaster workshop", typeof( FillablePlasterWorkshopDeed ), 7200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster merchant's house", typeof( FillablePlasterMerchantHouseDeed ), 11550, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster small lab", typeof( FillablePlasterSmallLabDeed ), 5050, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster shop", typeof( FillablePlasterShopDeed ), 6250, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster care home", typeof( FillablePlasterCareHomeDeed ), 7900, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a large plaster civilian house", typeof( FillableLargePlasterCivilianHouseDeed ), 11500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster bourgeois villa", typeof( FillablePlasterBourgeoisVillaDeed ), 15500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster gate house", typeof( FillablePlasterGateHouseDeed ), 7200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster hall", typeof( FillablePlasterHallDeed ), 10500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster civilian house", typeof( FillablePlasterCivilianHouseDeed ), 8650, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster warehouse", typeof( FillablePlasterWarehouseDeed ), 6200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster parsonage", typeof( FillablePlasterParsonageDeed ), 6800, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a plaster house with balcony", typeof( FillablePlasterHousewithBalconyDeed ), 6800, 20, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( "fillable deed to a large sandstone workshop", typeof( FillableLargeSandstoneWorkshopDeed ), 7200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone merchant's house", typeof( FillableSandstoneMerchantHouseDeed ), 11550, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small sandstone workshop", typeof( FillableSmallSandstoneWorkshopDeed ), 5050, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone residence", typeof( FillableSandstoneResidenceDeed ), 6250, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone two story house", typeof( FillableSandstoneTwoStoryHouseDeed ), 7900, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone civilian house", typeof( FillableSandstoneCivilianHouseDeed ), 11500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone villa", typeof( FillableSandstoneVillaDeed ), 15500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone refuge", typeof( FillableSandstoneRefugeDeed ), 7200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone care home", typeof( FillableSandstoneCareHomeDeed ), 10500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone lab", typeof( FillableSandstoneLabDeed ), 8650, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone warehouse", typeof( FillableSandstoneWareHouseDeed ), 6200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone gate house", typeof( FillableSandstoneGateHouseDeed ), 6800, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a sandstone house with balcony", typeof( FillableSandstoneHousewithBalconyDeed ), 6800, 20, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( "fillable deed to a tree feller workshop", typeof( FillableTreeFellerWorkshopDeed ), 7200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a wooden stockhouse", typeof( FillableWoodenStockHouseDeed ), 11550, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small wooden workshop", typeof( FillableSmallWoodenWorkshopDeed ), 5050, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a wooden warehouse", typeof( FillableWoodenWareHouseDeed ), 6250, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a two story tree feller house", typeof( FillableTwoStoryTreeFellerHouseDeed ), 7900, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a tree feller refuge", typeof( FillableTreeFellerRefugeDeed ), 11500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a tree feller villa", typeof( FillableTreeFellerVillaDeed ), 15500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a tree feller house", typeof( FillableTreeFellerHouseDeed ), 7200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a tree feller villa (two)", typeof( FillableTreeFellerVillaModelTwoDeed ), 10500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a wooden lab", typeof( FillableWoodenLabDeed ), 8650, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a tree feller stockhouse", typeof( FillableTreeFellerStockHouseDeed ), 6200, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a large tree feller house", typeof( FillableLargeTreeFellerHouseDeed ), 6800, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a wooden house with balcony", typeof( FillableWoodenHousewithBalconyDeed ), 6800, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "fillable deed to a small tree feller house", typeof( FillableSmallTreeFellerHouseDeed ), 3800, 20, 0x14F0, 0 ) );
            }
        }
        #endregion

        #region Nested type: InternalSellInfo
        public class InternalSellInfo : GenericSellInfo
        {
        }
        #endregion
    }

    public class FillableArchitect : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public FillableArchitect()
            : base( "the architect" )
        {
        }

        public FillableArchitect( Serial serial )
            : base( serial )
        {
        }

        public override SpeechFragment PersonalFragmentObj
        {
            get { return PersonalFragment.Architect; }
        }

        protected override List<SBInfo> SBInfos
        {
            get { return m_SBInfos; }
        }

        public override NpcGuild NpcGuild
        {
            get { return NpcGuild.TinkersGuild; }
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBFillableHouseDeed() );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}