/***************************************************************************
 *                               BagOfFillableHouseDeeds.cs
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
    public class BagOfFillableHouseDeeds : Bag
    {
        [Constructable]
        public BagOfFillableHouseDeeds()
        {
            DropItem( new FillableStonePlasterHouseDeed() );
            DropItem( new FillableFieldStoneHouseDeed() );
            DropItem( new FillableSmallBrickHouseDeed() );
            DropItem( new FillableWoodHouseDeed() );
            DropItem( new FillableWoodPlasterHouseDeed() );
            DropItem( new FillableThatchedRoofCottageDeed() );
            DropItem( new FillableBrickHouseDeed() );
            DropItem( new FillableTwoStoryWoodPlasterHouseDeed() );
            DropItem( new FillableTowerDeed() );
            DropItem( new FillableKeepDeed() );
            DropItem( new FillableCastleDeed() );
            DropItem( new FillableLargePatioDeed() );
            DropItem( new FillableLargeMarbleDeed() );
            DropItem( new FillableSmallTowerDeed() );
            DropItem( new FillableLogCabinDeed() );
            DropItem( new FillableSandstonePatioDeed() );
            DropItem( new FillableVillaDeed() );
            DropItem( new FillableStoneWorkshopDeed() );
            DropItem( new FillableMarbleWorkshopDeed() );
        }

        #region serialization
        public BagOfFillableHouseDeeds( Serial serial )
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