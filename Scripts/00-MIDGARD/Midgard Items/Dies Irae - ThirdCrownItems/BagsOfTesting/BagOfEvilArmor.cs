using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfEvilArmor : Bag
    {
        [Constructable]
        public BagOfEvilArmor()
        {
            DropItem( new EvilPlateArms() );
            DropItem( new EvilPlateChest() );
            DropItem( new EvilPlateGloves() );
            DropItem( new EvilPlateGorget() );
            DropItem( new EvilPlateHelm() );
            DropItem( new EvilPlateLegs() );
        }

        #region serialization
        public BagOfEvilArmor( Serial serial )
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