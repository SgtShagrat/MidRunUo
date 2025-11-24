using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3fed Order Standard ( item blessed, per seer, lo assegnamo noi ai capi più rappresentativi )
    /// </summary>
    public class OrderStandard : BaseStandard
    {
        public override string DefaultName { get { return "order standard"; } }

        [Constructable]
        public OrderStandard()
            : this( 0 )
        {
        }

        [Constructable]
        public OrderStandard( int hue )
            : base( 0x3fed, hue )
        {
            LootType = LootType.Blessed;
        }

        #region serialization
        public OrderStandard( Serial serial )
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