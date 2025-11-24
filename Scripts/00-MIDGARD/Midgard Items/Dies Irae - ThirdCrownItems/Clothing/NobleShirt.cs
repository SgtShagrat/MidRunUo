using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10B4 Noble Shirt - ( craft del sarto , colorabile da hue, solo uomo )
    /// </summary>
    public class NobleShirt : BaseShirt
    {
        public override string DefaultName { get { return "noble shirt"; } }

        public override bool AllowFemaleWearer { get { return false; } }

        [Constructable]
        public NobleShirt()
            : this( 0 )
        {
        }

        [Constructable]
        public NobleShirt( int hue )
            : base( 0x10B4, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public NobleShirt( Serial serial )
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