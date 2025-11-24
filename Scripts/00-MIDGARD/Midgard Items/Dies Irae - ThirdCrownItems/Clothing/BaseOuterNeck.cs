using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseOuterNeck : BaseClothing
    {
        public BaseOuterNeck( int itemID )
            : this( itemID, 0 )
        {
        }

        public BaseOuterNeck( int itemID, int hue )
            : base( itemID, Layer.Neck, hue )
        {
        }

        #region serialization
        public BaseOuterNeck( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}