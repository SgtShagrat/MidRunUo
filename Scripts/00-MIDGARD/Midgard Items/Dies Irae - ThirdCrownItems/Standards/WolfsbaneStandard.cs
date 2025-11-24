using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3371 Wolfsbane Standard ( item blessed, per seer, lo assegnamo noi ai capi più rappresentativi )
    /// </summary>
    public class WolfsbaneStandard : BaseStandard
    {
        public override string DefaultName { get { return "wolfsbane standard"; } }

        [Constructable]
        public WolfsbaneStandard()
            : this( 0 )
        {
        }

        [Constructable]
        public WolfsbaneStandard( int hue )
            : base( 0x3371, hue )
        {
            LootType = LootType.Blessed;
        }

        #region serialization
        public WolfsbaneStandard( Serial serial )
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