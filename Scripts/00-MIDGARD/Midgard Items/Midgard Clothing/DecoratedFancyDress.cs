using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DecoratedFancyDress : BaseOuterTorso
    {
        public override int LabelNumber { get { return 1064985; } } // "Decorated Fancy Dress

        [Constructable]
        public DecoratedFancyDress() : this( 0 )
        {
        }

        [Constructable]
        public DecoratedFancyDress( int hue )
            : base( 0x3FEE, hue )
        {
            Weight = 3.0;
        }

        public DecoratedFancyDress( Serial serial ) : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}