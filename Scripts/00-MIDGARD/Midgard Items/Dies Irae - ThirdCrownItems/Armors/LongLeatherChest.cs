using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x1151 Long Leather Chest - ( Nel craft del sarto, colorabile, solo femmina )
    /// </summary>
    public class LongLeatherChest : LeatherChest
    {
        public override string DefaultName { get { return "long leather chest"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public LongLeatherChest()
            : this( 0 )
        {
        }

        [Constructable]
        public LongLeatherChest( int hue )
        {
            ItemID = 0x1151;
            Hue = hue;
            Weight = 1.0;
        }

        #region serialization
        public LongLeatherChest( Serial serial )
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