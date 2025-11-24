using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3C53 Lace Dress trad. vestito di pizzo - ( craftabile dai sarti )
    /// </summary>
    public class LaceDress : BaseOuterTorso
    {
        public override string DefaultName { get { return "lace dress"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public LaceDress()
            : this( 0 )
        {
        }

        [Constructable]
        public LaceDress( int hue )
            : base( 0x3C53, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public LaceDress( Serial serial )
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