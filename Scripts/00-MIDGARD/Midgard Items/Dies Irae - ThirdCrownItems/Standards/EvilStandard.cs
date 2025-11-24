using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0xe98 Evil Standard ( item blessed, per seer, lo assegnamo noi ai capi più rappresentativi )
    /// </summary>
    public class EvilStandard : BaseStandard
    {
        public override string DefaultName { get { return "evil standard"; } }

        [Constructable]
        public EvilStandard()
            : this( 0 )
        {
        }

        [Constructable]
        public EvilStandard( int hue )
            : base( 0xe98, hue )
        {
            LootType = LootType.Blessed;
        }

        #region serialization
        public EvilStandard( Serial serial )
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