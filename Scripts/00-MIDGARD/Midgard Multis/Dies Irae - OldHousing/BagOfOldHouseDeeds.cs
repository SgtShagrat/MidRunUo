/***************************************************************************
 *                               BagOfOldHouseDeeds.cs
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
    public class BagOfOldHouseDeeds : Bag
    {
        [Constructable]
        public BagOfOldHouseDeeds()
        {
            DropItem( new PlasterWorkshopDeed() );
            DropItem( new PlasterMerchantHouseDeed() );
            DropItem( new PlasterSmallLabDeed() );
            DropItem( new PlasterShopDeed() );
            DropItem( new PlasterCareHomeDeed() );
            DropItem( new LargePlasterCivilianHouseDeed() );
            DropItem( new PlasterBourgeoisVillaDeed() );
            DropItem( new PlasterGateHouseDeed() );
            DropItem( new PlasterHallDeed() );
            DropItem( new PlasterCivilianHouseDeed() );
            DropItem( new PlasterWarehouseDeed() );
            DropItem( new PlasterParsonageDeed() );
            DropItem( new PlasterHousewithBalconyDeed() );

            DropItem( new LargeSandstoneWorkshopDeed() );
            DropItem( new SandstoneMerchantHouseDeed() );
            DropItem( new SmallSandstoneWorkshopDeed() );
            DropItem( new SandstoneResidenceDeed() );
            DropItem( new SandstoneTwoStoryHouseDeed() );
            DropItem( new SandstoneCivilianHouseDeed() );
            DropItem( new SandstoneVillaDeed() );
            DropItem( new SandstoneRefugeDeed() );
            DropItem( new SandstoneCareHomeDeed() );
            DropItem( new SandstoneLabDeed() );
            DropItem( new SandstoneWareHouseDeed() );
            DropItem( new SandstoneGateHouseDeed() );
            DropItem( new SandstoneHousewithBalconyDeed() );

            DropItem( new TreeFellerWorkshopDeed() );
            DropItem( new WoodenStockHouseDeed() );
            DropItem( new SmallWoodenWorkshopDeed() );
            DropItem( new WoodenWareHouseDeed() );
            DropItem( new TwoStoryTreeFellerHouseDeed() );
            DropItem( new TreeFellerRefugeDeed() );
            DropItem( new TreeFellerVillaDeed() );
            DropItem( new TreeFellerHouseDeed() );
            DropItem( new TreeFellerVillaModelTwoDeed() );
            DropItem( new WoodenLabDeed() );
            DropItem( new TreeFellerStockHouseDeed() );
            DropItem( new LargeTreeFellerHouseDeed() );
            DropItem( new WoodenHousewithBalconyDeed() );

            DropItem( new SmallTreeFellerHouseDeed() );
        }

        #region serialization
        public BagOfOldHouseDeeds( Serial serial )
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