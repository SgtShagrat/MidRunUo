/***************************************************************************
 *                               BagOfOldFillableHouseDeeds.cs
 *
 *   begin                : 27 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Multis.Deeds;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfOldFillableHouseDeeds : Bag
    {
        [Constructable]
        public BagOfOldFillableHouseDeeds()
        {
            DropItem( new FillablePlasterWorkshopDeed() );
            DropItem( new FillablePlasterMerchantHouseDeed() );
            DropItem( new FillablePlasterSmallLabDeed() );
            DropItem( new FillablePlasterShopDeed() );
            DropItem( new FillablePlasterCareHomeDeed() );
            DropItem( new FillableLargePlasterCivilianHouseDeed() );
            DropItem( new FillablePlasterBourgeoisVillaDeed() );
            DropItem( new FillablePlasterGateHouseDeed() );
            DropItem( new FillablePlasterHallDeed() );
            DropItem( new FillablePlasterCivilianHouseDeed() );
            DropItem( new FillablePlasterWarehouseDeed() );
            DropItem( new FillablePlasterParsonageDeed() );
            DropItem( new FillablePlasterHousewithBalconyDeed() );

            DropItem( new FillableLargeSandstoneWorkshopDeed() );
            DropItem( new FillableSandstoneMerchantHouseDeed() );
            DropItem( new FillableSmallSandstoneWorkshopDeed() );
            DropItem( new FillableSandstoneResidenceDeed() );
            DropItem( new FillableSandstoneTwoStoryHouseDeed() );
            DropItem( new FillableSandstoneCivilianHouseDeed() );
            DropItem( new FillableSandstoneVillaDeed() );
            DropItem( new FillableSandstoneRefugeDeed() );
            DropItem( new FillableSandstoneCareHomeDeed() );
            DropItem( new FillableSandstoneLabDeed() );
            DropItem( new FillableSandstoneWareHouseDeed() );
            DropItem( new FillableSandstoneGateHouseDeed() );
            DropItem( new FillableSandstoneHousewithBalconyDeed() );

            DropItem( new FillableTreeFellerWorkshopDeed() );
            DropItem( new FillableWoodenStockHouseDeed() );
            DropItem( new FillableSmallWoodenWorkshopDeed() );
            DropItem( new FillableWoodenWareHouseDeed() );
            DropItem( new FillableTwoStoryTreeFellerHouseDeed() );
            DropItem( new FillableTreeFellerRefugeDeed() );
            DropItem( new FillableTreeFellerVillaDeed() );
            DropItem( new FillableTreeFellerHouseDeed() );
            DropItem( new FillableTreeFellerVillaModelTwoDeed() );
            DropItem( new FillableWoodenLabDeed() );
            DropItem( new FillableTreeFellerStockHouseDeed() );
            DropItem( new FillableLargeTreeFellerHouseDeed() );
            DropItem( new FillableWoodenHousewithBalconyDeed() );

            DropItem( new FillableSmallTreeFellerHouseDeed() );
        }

        #region serialization
        public BagOfOldFillableHouseDeeds( Serial serial )
            : base( serial )
        {
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
        #endregion
    }
}