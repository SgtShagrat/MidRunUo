using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38EC, // Shit with Fly - Merda con mosche
    /// </summary>
    public class ShitWithFlies : Item
    {
        [Constructable]
        public ShitWithFlies()
            : base( 0x38EC )
        {
            Weight = 100.0;
        }

        #region serialization
        public ShitWithFlies( Serial serial )
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