using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38fa, // Stone Fountain - Fontana in pietra scura animata
    /// </summary>
    public class StoneFountain : Item
    {
        [Constructable]
        public StoneFountain()
            : base( 0x38fa )
        {
            Weight = 100.0;
        }

        #region serialization
        public StoneFountain( Serial serial )
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