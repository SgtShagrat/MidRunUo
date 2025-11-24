using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x1097 Sharped Shoes trad. scarpe a punta - ( craft sarto , colorabile )
    /// </summary>
    public class SharpedShoes : BaseShoes
    {
        public override string DefaultName { get { return "sharped shoes"; } }

        [Constructable]
        public SharpedShoes()
            : this( 0 )
        {
        }

        [Constructable]
        public SharpedShoes( int hue )
            : base( 0x1097, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public SharpedShoes( Serial serial )
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