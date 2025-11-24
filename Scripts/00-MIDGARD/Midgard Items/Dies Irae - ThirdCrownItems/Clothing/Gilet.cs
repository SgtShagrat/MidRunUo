using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3C56 Gilet - ( craftabile dai sarti )
    /// </summary>
    public class Gilet : BaseMiddleTorso
    {
        public override string DefaultName { get { return "gilet"; } }

        [Constructable]
        public Gilet()
            : this( 0 )
        {
        }

        [Constructable]
        public Gilet( int hue )
            : base( 0x3C56, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public Gilet( Serial serial )
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