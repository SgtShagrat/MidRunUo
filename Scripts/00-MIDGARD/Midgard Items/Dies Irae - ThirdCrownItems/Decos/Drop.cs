using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38f4, // Drop - Goccia per Stalattite e altro
    /// </summary>
    public class Drop : Item
    {
        [Constructable]
        public Drop()
            : base( 0x38f4 )
        {
            Weight = 100.0;
        }

        #region serialization
        public Drop( Serial serial )
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