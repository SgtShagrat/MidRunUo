using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfItemTestingArAndSh : Bag
    {
        [Constructable]
        public BagOfItemTestingArAndSh()
        {
            DropItem( new ChaosPlatemail() );
            DropItem( new Crown() );
            DropItem( new IanPlatemailArms() );

            DropItem( new LongLeatherChest() );
            DropItem( new LongLeatherSkirt() );
            DropItem( new OrderPlatemail() );
            DropItem( new PlumedHelm() );
            DropItem( new ShortLeatherChest() );
            DropItem( new ShortLeatherSkirt() );
            DropItem( new AllianceShield() );

            DropItem( new OldShadowShield() );
        }

        #region serialization
        public BagOfItemTestingArAndSh( Serial serial )
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