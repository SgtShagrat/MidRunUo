using Server;

namespace Midgard.Items
{
    public class Stalactite : Item
    {
        [Constructable]
        public Stalactite()
            : base( 0x38f9 )
        {
            Weight = 100.0;
        }

        #region serialization
        public Stalactite( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}