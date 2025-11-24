using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfOldWeapons : Bag
    {
        [Constructable]
        public BagOfOldWeapons()
        {
            DropItem( new MageStaff() );
            // DropItem( new LightCrossbow() );
            DropItem( new BackstabbingKnife() );
        }

        #region serialization
        public BagOfOldWeapons( Serial serial )
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