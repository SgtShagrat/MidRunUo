using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class BagOfReagentCropSeeds : Bag
    {
        [Constructable]
        public BagOfReagentCropSeeds()
            : this( 50 )
        {
        }

        [Constructable]
        public BagOfReagentCropSeeds( int amount )
        {
            DropItem( new GarlicSeed( amount ) );
            DropItem( new MandrakeSeed( amount ) );
            DropItem( new GinsengSeed( amount ) );
            DropItem( new NightshadeSeed( amount ) );

            Hue = 2024;
            Name = "Bag Of Reagent Crop Seeds";
        }

        public BagOfReagentCropSeeds( Serial serial )
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