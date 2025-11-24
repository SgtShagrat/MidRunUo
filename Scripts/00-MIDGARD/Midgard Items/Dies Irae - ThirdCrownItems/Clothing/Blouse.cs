using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x266F Blouse - ( solo craft sarto , colorabile )
    /// </summary>
    public class Blouse : BaseMiddleTorso
    {
        public override string DefaultName { get { return "blouse"; } }

        [Constructable]
        public Blouse()
            : this( 0 )
        {
        }

        [Constructable]
        public Blouse( int hue )
            : base( 0x266F, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public Blouse( Serial serial )
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