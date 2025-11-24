using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Midgard.Items
{
    public class BagOfHouseDeeds : Bag
    {
        [Constructable]
        public BagOfHouseDeeds()
        {
            DropItem( new StonePlasterHouseDeed() );
            DropItem( new FieldStoneHouseDeed() );
            DropItem( new SmallBrickHouseDeed() );
            DropItem( new WoodHouseDeed() );
            DropItem( new WoodPlasterHouseDeed() );
            DropItem( new ThatchedRoofCottageDeed() );
            DropItem( new BrickHouseDeed() );
            DropItem( new TwoStoryWoodPlasterHouseDeed() );
            DropItem( new TowerDeed() );
            DropItem( new KeepDeed() );
            DropItem( new CastleDeed() );
            DropItem( new LargePatioDeed() );
            DropItem( new LargeMarbleDeed() );
            DropItem( new SmallTowerDeed() );
            DropItem( new LogCabinDeed() );
            DropItem( new SandstonePatioDeed() );
            DropItem( new VillaDeed() );
            DropItem( new StoneWorkshopDeed() );
            DropItem( new MarbleWorkshopDeed() );
            DropItem( new BlueTentDeed() );
            DropItem( new GreenTentDeed() );
        }

        #region serialization
        public BagOfHouseDeeds( Serial serial )
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