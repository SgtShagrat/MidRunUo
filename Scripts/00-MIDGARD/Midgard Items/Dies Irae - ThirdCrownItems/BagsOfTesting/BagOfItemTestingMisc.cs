using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfItemTestingMisc : Bag
    {
        [Constructable]
        public BagOfItemTestingMisc()
        {
            DropItem( new ChaosStandard() );
            DropItem( new OrderStandard() );
            DropItem( new EvilStandard() );
            DropItem( new WolfsbaneStandard() );
            DropItem( new HeavyBracelet() );
            DropItem( new Handcuffs() );
            DropItem( new Pipe() );
            DropItem( new BonesNecklace() );
            DropItem( new Hook() );
            DropItem( new TapestryOfMidgard() );
        }

        #region serialization
        public BagOfItemTestingMisc( Serial serial )
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