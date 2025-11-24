using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfCruciformArmor : Bag
    {
        [Constructable]
        public BagOfCruciformArmor()
        {
            DropItem( new CruciformPlateArms() );
            DropItem( new CruciformPlateChest() );
            DropItem( new CruciformPlateGloves() );
            DropItem( new CruciformPlateGorget() );
            DropItem( new CruciformPlateHelm() );
            DropItem( new CruciformPlateLegs() );
            DropItem( new CruciformShield() );
        }

        #region serialization
        public BagOfCruciformArmor( Serial serial )
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