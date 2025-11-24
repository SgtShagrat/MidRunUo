using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2693 Chaos Standard ( item blessed, per seer, lo assegnamo noi ai capi più rappresentativi )
    /// </summary>
    public class ChaosStandard : BaseStandard
    {
        public override string DefaultName { get { return "chaos standard"; } }

        [Constructable]
        public ChaosStandard()
            : this( 0 )
        {
        }

        [Constructable]
        public ChaosStandard( int hue )
            : base( 0x2693, hue )
        {
            LootType = LootType.Blessed;
        }

        #region serialization
        public ChaosStandard( Serial serial )
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