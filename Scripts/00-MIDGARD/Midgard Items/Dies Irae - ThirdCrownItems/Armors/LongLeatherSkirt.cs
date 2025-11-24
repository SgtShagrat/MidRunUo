using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x1164 Long Leather Skirt - ( Nel craft del sarto, colorabile, solo femmina )
    /// </summary>
    public class LongLeatherSkirt : LeatherSkirt
    {
        public override string DefaultName { get { return "long leather skirt"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public LongLeatherSkirt()
            : this( 0 )
        {
        }

        [Constructable]
        public LongLeatherSkirt( int hue )
        {
            ItemID = 0x1164;
            Hue = hue;
            Weight = 1.0;
            Layer = Layer.OuterLegs;
        }

        #region serialization
        public LongLeatherSkirt( Serial serial )
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