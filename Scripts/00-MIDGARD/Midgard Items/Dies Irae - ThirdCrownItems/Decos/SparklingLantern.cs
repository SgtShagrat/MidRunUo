using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38D9, Sparkling Lantern - lanterna luminescente per Calen Sul
    /// </summary>
    public class SparklingLantern : Item
    {
        [Constructable]
        public SparklingLantern()
            : base( 0x38D9 )
        {
            Weight = 100.0;
            Light = LightType.Circle225;
        }

        #region serialization
        public SparklingLantern( Serial serial )
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