using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10B7 Short Leather Chest - ( Nel craft del sarto, colorabile, solo femmina )
    /// </summary>
    public class ShortLeatherChest : LeatherChest
    {
        public override string DefaultName { get { return "short leather chest"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public ShortLeatherChest()
            : this( 0 )
        {
        }

        [Constructable]
        public ShortLeatherChest( int hue )
        {
            ItemID = 0x10B7;
            Hue = hue;
            Weight = 1.0;
        }

        #region serialization
        public ShortLeatherChest( Serial serial )
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