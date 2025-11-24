using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfSeaArmor : Bag
    {
        [Constructable]
        public BagOfSeaArmor()
        {
            DropItem( new SeaChainChest() );
            DropItem( new SeaChainCoif() );
            DropItem( new SeaChainLegs() );
        }

        #region serialization
        public BagOfSeaArmor( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}