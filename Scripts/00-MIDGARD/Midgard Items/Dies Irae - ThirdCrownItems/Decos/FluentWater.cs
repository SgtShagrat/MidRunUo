using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38DE, // Fluent Water - Acqua che scorre
    /// </summary>
    public class FluentWater : Item
    {
        [Constructable]
        public FluentWater()
            : base( 0x38DE )
        {
            Weight = 100.0;
        }

        #region serialization
        public FluentWater( Serial serial )
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