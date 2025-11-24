using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3B38 // Lava Cocoon - Bozzoli di Lava
    /// </summary>
    public class LavaCocoon : Item
    {
        [Constructable]
        public LavaCocoon()
            : base( 0x3B38 )
        {
            Light = LightType.Circle225;
            Weight = 100.0;
        }

        #region serialization
        public LavaCocoon( Serial serial )
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