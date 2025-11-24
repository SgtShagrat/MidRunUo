using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3931, // Water Loss - Muro di Fogna che perde acqua
    /// 0x3939, // Water Loss - Muro di Fogna che perde acqua ( orientato y )
    /// </summary>
    [Flipable( 0x3931, 0x3939 )]
    public class WaterLoss : Item
    {
        [Constructable]
        public WaterLoss()
            : base( 0x3931 )
        {
            Weight = 100.0;
        }

        #region serialization
        public WaterLoss( Serial serial )
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