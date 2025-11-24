using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x10B6 Short Leather Skirt - ( Nel craft del sarto, colorabile, solo femmina )
    /// </summary>
    public class ShortLeatherSkirt : LeatherSkirt
    {
        public override string DefaultName { get { return "short leather skirt"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public ShortLeatherSkirt()
            : this( 0 )
        {
        }

        [Constructable]
        public ShortLeatherSkirt( int hue )
        {
            ItemID = 0x10B6;
            Hue = hue;
            Weight = 1.0;
            Layer = Layer.OuterLegs;
        }

        #region serialization
        public ShortLeatherSkirt( Serial serial )
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