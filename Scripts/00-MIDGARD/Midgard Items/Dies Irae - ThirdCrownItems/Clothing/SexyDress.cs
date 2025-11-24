using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3C54 Sexy Dress - ( craftabile dai sarti )
    /// </summary>
    public class SexyDress : BaseOuterTorso
    {
        public override string DefaultName { get { return "sexy dress"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public SexyDress()
            : this( 0 )
        {
        }

        [Constructable]
        public SexyDress( int hue )
            : base( 0x3C54, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public SexyDress( Serial serial )
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