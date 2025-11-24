using Server;
using Server.Items;

namespace Midgard.Items
{
    public class VestOfTheMage : BaseOuterTorso
    {
        public override int LabelNumber { get { return 1064987; } } //

        [Constructable]
        public VestOfTheMage() : this( 0 )
        {
        }

        [Constructable]
        public VestOfTheMage( int hue ) : base( 0x30A, hue )
        {
            Weight = 3.0;
        }

        public VestOfTheMage( Serial serial ) : base( serial )
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