using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfElvenArmor : Bag
    {
        [Constructable]
        public BagOfElvenArmor()
        {
            DropItem( new ElvenPlateArms() );
            DropItem( new ElvenPlateChest() );
            DropItem( new ElvenPlateGloves() );
            DropItem( new ElvenPlateGorget() );
            DropItem( new ElvenPlateHelm() );
            DropItem( new ElvenPlateLegs() );
        }

        #region serialization
        public BagOfElvenArmor( Serial serial )
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