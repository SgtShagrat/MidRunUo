using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3C55 Servant Shirt trad. maglia da contadino - ( craftabile dai sarti )
    /// </summary>
    public class ServantShirt : BaseShirt
    {
        public override string DefaultName { get { return "servant shirt"; } }

        [Constructable]
        public ServantShirt()
            : this( 0 )
        {
        }

        [Constructable]
        public ServantShirt( int hue )
            : base( 0x3C55, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public ServantShirt( Serial serial )
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