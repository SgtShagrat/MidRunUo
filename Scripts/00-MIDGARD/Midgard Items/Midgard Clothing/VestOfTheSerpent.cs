using Server;
using Server.Items;

namespace Midgard.Items
{
    public class VestOfTheSerpent : BaseOuterTorso
    {
        public override int LabelNumber { get { return 1064991; } } //

        [Constructable]
        public VestOfTheSerpent() : this( 0 )
        {
        }

        [Constructable]
        public VestOfTheSerpent( int hue )
            : base( 0x3DBC, hue )
        {
            Weight = 3.0;
        }

        public VestOfTheSerpent( Serial serial ) : base( serial )
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