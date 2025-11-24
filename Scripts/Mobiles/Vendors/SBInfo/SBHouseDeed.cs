using System;
using System.Collections.Generic;
using Server.Multis.Deeds;

using Midgard.Multis.Deeds;

namespace Server.Mobiles
{
	public class SBHouseDeed: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBHouseDeed()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "deed to a stone-and-plaster house", typeof( StonePlasterHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a field stone house", typeof( FieldStoneHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small brick house", typeof( SmallBrickHouseDeed), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wooden house", typeof( WoodHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a wood-and-plaster house", typeof( WoodPlasterHouseDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a thatched-roof cottage", typeof( ThatchedRoofCottageDeed ), 43800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a brick house", typeof( BrickHouseDeed ), 144500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two-story wood-and-plaster house", typeof( TwoStoryWoodPlasterHouseDeed ), 192400, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a tower", typeof( TowerDeed ), (int)( 433200 * 1.5 ), 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone keep", typeof( KeepDeed ), 665200 * 2, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a castle", typeof( CastleDeed ), 1022800 * 2, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a large house with patio", typeof( LargePatioDeed ), 152800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a marble house with patio", typeof( LargeMarbleDeed ), 192000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone tower", typeof( SmallTowerDeed ), 88500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story log cabin", typeof( LogCabinDeed ), 97800, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a sandstone house with patio", typeof( SandstonePatioDeed ), 90900, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a two story villa", typeof( VillaDeed ), 136500, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small stone workshop", typeof( StoneWorkshopDeed ), 60600, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo( "deed to a small marble workshop", typeof( MarbleWorkshopDeed ), 63000, 20, 0x14F0, 0 ) );

                #region mod by Dies Irae
                Add( new GenericBuyInfo( "deed to a plaster workshop", typeof( PlasterWorkshopDeed ), 72000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster merchant's house", typeof( PlasterMerchantHouseDeed ), 115500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster small lab", typeof( PlasterSmallLabDeed ), 50500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster shop", typeof( PlasterShopDeed ), 62500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster care home", typeof( PlasterCareHomeDeed ), 79000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a large plaster civilian house", typeof( LargePlasterCivilianHouseDeed ), 115000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster bourgeois villa", typeof( PlasterBourgeoisVillaDeed ), 155000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster gate house", typeof( PlasterGateHouseDeed ), 72000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster hall", typeof( PlasterHallDeed ), 105000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster civilian house", typeof( PlasterCivilianHouseDeed ), 86500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster warehouse", typeof( PlasterWarehouseDeed ), 62000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster parsonage", typeof( PlasterParsonageDeed ), 68000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a plaster house with balcony", typeof( PlasterHousewithBalconyDeed ), 68000, 20, 0x14F0, 0 ) );
                
                Add( new GenericBuyInfo( "deed to a large sandstone workshop", typeof( LargeSandstoneWorkshopDeed ), 72000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone merchant's house", typeof( SandstoneMerchantHouseDeed ), 115500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a small sandstone workshop", typeof( SmallSandstoneWorkshopDeed ), 50500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone residence", typeof( SandstoneResidenceDeed ), 62500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone two story house", typeof( SandstoneTwoStoryHouseDeed ), 79000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone civilian house", typeof( SandstoneCivilianHouseDeed ), 115000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone villa", typeof( SandstoneVillaDeed ), 155000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone refuge", typeof( SandstoneRefugeDeed ), 72000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone care home", typeof( SandstoneCareHomeDeed ), 105000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone lab", typeof( SandstoneLabDeed ), 86500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone warehouse", typeof( SandstoneWareHouseDeed ), 62000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone gate house", typeof( SandstoneGateHouseDeed ), 68000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a sandstone house with balcony", typeof( SandstoneHousewithBalconyDeed ), 68000, 20, 0x14F0, 0 ) );
                
                Add( new GenericBuyInfo( "deed to a tree feller workshop", typeof( TreeFellerWorkshopDeed ), 72000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a wooden stockhouse", typeof( WoodenStockHouseDeed ), 115500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a small wooden workshop", typeof( SmallWoodenWorkshopDeed ), 50500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a wooden warehouse", typeof( WoodenWareHouseDeed ), 62500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a two story tree feller house", typeof( TwoStoryTreeFellerHouseDeed ), 79000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a tree feller refuge", typeof( TreeFellerRefugeDeed ), 115000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a tree feller villa", typeof( TreeFellerVillaDeed ), 155000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a tree feller house", typeof( TreeFellerHouseDeed ), 72000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a tree feller villa (two)", typeof( TreeFellerVillaModelTwoDeed ), 105000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a wooden lab", typeof( WoodenLabDeed ), 86500, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a tree feller stockhouse", typeof( TreeFellerStockHouseDeed ), 62000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a large tree feller house", typeof( LargeTreeFellerHouseDeed ), 68000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a wooden house with balcony", typeof( WoodenHousewithBalconyDeed ), 68000, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a small tree feller house", typeof( SmallTreeFellerHouseDeed ), 38000, 20, 0x14F0, 0 ) );

                Add( new GenericBuyInfo( "deed to a blue tent", typeof( BlueTentDeed ), 22800, 20, 0x14F0, 0 ) );
                Add( new GenericBuyInfo( "deed to a green tent", typeof( GreenTentDeed ), 22800, 20, 0x14F0, 0 ) );
                #endregion
            }
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				/*Add( typeof( StonePlasterHouseDeed ), 43800 );
				Add( typeof( FieldStoneHouseDeed ), 43800 );
				Add( typeof( SmallBrickHouseDeed ), 43800 );
				Add( typeof( WoodHouseDeed ), 43800 );
				Add( typeof( WoodPlasterHouseDeed ), 43800 );
				Add( typeof( ThatchedRoofCottageDeed ), 43800 );
				Add( typeof( BrickHouseDeed ), 144500 );
				Add( typeof( TwoStoryWoodPlasterHouseDeed ), 192400 );
				Add( typeof( TowerDeed ), 433200 );
				Add( typeof( KeepDeed ), 665200 );
				Add( typeof( CastleDeed ), 1022800 );
				Add( typeof( LargePatioDeed ), 152800 );
				Add( typeof( LargeMarbleDeed ), 192800 );
				Add( typeof( SmallTowerDeed ), 88500 );
				Add( typeof( LogCabinDeed ), 97800 );
				Add( typeof( SandstonePatioDeed ), 90900 );
				Add( typeof( VillaDeed ), 136500 );
				Add( typeof( StoneWorkshopDeed ), 60600 );
				Add( typeof( MarbleWorkshopDeed ), 60300 );
				Add( typeof( SmallBrickHouseDeed ), 43800 );*/
            }
		}
	}
}
