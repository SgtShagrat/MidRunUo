using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfBarbarianArmor : Bag
    {
        [Constructable]
        public BagOfBarbarianArmor()
        {
            DropItem( new BarbarianChest() );
            DropItem( new BarbarianGorget() );
            DropItem( new BarbarianLegs() );
            DropItem( new BarbarianPlateGloves() );
            DropItem( new BarbarianPlateHelm() );
        }

        #region serialization
        public BagOfBarbarianArmor( Serial serial )
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