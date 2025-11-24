using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x153F Bandana with Pearls - ( Nel craft del sarto, colorabile )
    /// </summary>
    public class BandanaWithPearls : BaseHat
    {
        public override string DefaultName { get { return "bandana with pearls"; } }

        [Constructable]
        public BandanaWithPearls()
            : this( 0 )
        {
        }

        [Constructable]
        public BandanaWithPearls( int hue )
            : base( 0x1150, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public BandanaWithPearls( Serial serial )
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

            if( ItemID != 0x1150 )
                ItemID = 0x1150;
        }
        #endregion
    }
}