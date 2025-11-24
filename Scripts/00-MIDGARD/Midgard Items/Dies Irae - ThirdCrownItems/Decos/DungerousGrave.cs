using Server;

namespace Midgard.Items
{
    /// <summary>
    /// Dangerous Grave - tomba molto carina con una mano di scheletro che spunta dal terreno.
    /// </summary>
    public class DungerousGrave : Item
    {
        [Constructable]
        public DungerousGrave()
            : base( 0x38cc )
        {
            Weight = 100.0;
        }

        #region serialization
        public DungerousGrave( Serial serial )
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