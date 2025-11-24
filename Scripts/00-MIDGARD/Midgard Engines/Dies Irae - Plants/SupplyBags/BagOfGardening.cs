using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class BagOfGarneding : Bag
    {
        public override int LabelNumber
        {
            get { return 1065772; }
        } // bag of gardening

        [Constructable]
        public BagOfGarneding()
            : this( 50 )
        {
        }

        [Constructable]
        public BagOfGarneding( int amount )
        {
            DropItem( new LightFertilizerPotion( amount ) );
            DropItem( new MediumFertilizerPotion( amount ) );
            DropItem( new HeavyFertilizerPotion( amount ) );

            DropItem( new LightFungicidePotion( amount ) );
            DropItem( new MediumFungicidePotion( amount ) );
            DropItem( new HeavyFungicidePotion( amount ) );

            DropItem( new LightPesticidePotion( amount ) );
            DropItem( new MediumPesticidePotion( amount ) );
            DropItem( new HeavyPesticidePotion( amount ) );

            DropItem( new WateringCan() );

            DropItem( new GardeningShovel() );

            Hue = 2247;
        }

        public BagOfGarneding( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial

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