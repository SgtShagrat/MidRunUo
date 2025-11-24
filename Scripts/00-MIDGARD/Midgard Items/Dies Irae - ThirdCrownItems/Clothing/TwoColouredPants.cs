using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2675 Two Coloured Pants - ( craft sarto , colorabile )
    /// </summary>
    public class TwoColouredPants : BasePants
    {
        public override string DefaultName { get { return "two coloured pants"; } }

        [Constructable]
        public TwoColouredPants()
            : this( 0 )
        {
        }

        [Constructable]
        public TwoColouredPants( int hue )
            : base( 0x2675, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public TwoColouredPants( Serial serial )
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