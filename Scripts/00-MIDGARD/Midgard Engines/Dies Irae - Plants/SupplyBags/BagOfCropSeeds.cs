using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class BagOfCropSeeds : Bag
    {
        [Constructable]
        public BagOfCropSeeds()
            : this( 50 )
        {
        }

        [Constructable]
        public BagOfCropSeeds( int amount )
        {
            DropItem( new CarrotSeed( amount ) );
            DropItem( new CornSeed( amount ) );
            DropItem( new CottonSeed( amount ) );
            DropItem( new CabbageSeed( amount ) );
            DropItem( new WheatSeed( amount ) );

            DropItem( new LettuceSeed( amount ) );
            DropItem( new OnionSeed( amount ) );
            DropItem( new FlaxSeed( amount ) );
            DropItem( new PumpkinSeed( amount ) );
            DropItem( new TurnipSeed( amount ) );

            Hue = 2024;
            Name = "Bag Of Crop Seeds";
        }

        public BagOfCropSeeds( Serial serial )
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