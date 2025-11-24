using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38E5 Flowing Water - Acqua che scorre
    /// </summary>
    public class FlowingWater : Item
    {
        [Constructable]
        public FlowingWater()
            : base( 0x38E5 )
        {
            Weight = 100.0;
        }

        #region serialization
        public FlowingWater( Serial serial )
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