using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x392a, // Round Fountain - Fontana 
    /// </summary>
    public class RoundFountain : Item
    {
        [Constructable]
        public RoundFountain()
            : base( 0x392a )
        {
            Weight = 100.0;
        }

        #region serialization
        public RoundFountain( Serial serial )
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