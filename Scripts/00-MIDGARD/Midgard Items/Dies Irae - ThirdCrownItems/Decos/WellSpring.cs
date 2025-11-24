using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3901, // Wellspring - Fonte d'acqua cristallina
    /// </summary>
    public class WellSpring : Item
    {
        [Constructable]
        public WellSpring()
            : base( 0x3901 )
        {
            Weight = 100.0;
        }

        #region serialization
        public WellSpring( Serial serial )
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