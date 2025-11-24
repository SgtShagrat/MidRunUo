using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3908, // Stone Torch - Torcia a muro in pietra
    /// 0x390D, // Stone Torch - Torcia a muro in pietra ( orientata y )
    /// </summary>
    [Flipable( 0x3908, 0x390D )]
    public class StoneTorch : Item
    {
        [Constructable]
        public StoneTorch()
            : base( 0x3908 )
        {
            Weight = 100.0;
            Light = LightType.Circle225;
        }

        #region serialization
        public StoneTorch( Serial serial )
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