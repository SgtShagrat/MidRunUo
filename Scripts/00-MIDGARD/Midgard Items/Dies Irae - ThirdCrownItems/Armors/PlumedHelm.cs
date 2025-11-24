using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2601 Plumed Helm ( craft del fabbro, magari trovare altri ingredienti come piume )
    /// </summary>
    public class PlumedHelm : PlateHelm
    {
        public override string DefaultName { get { return "plumed helm"; } }

        [Constructable]
        public PlumedHelm()
            : this( 0 )
        {
        }

        [Constructable]
        public PlumedHelm( int hue )
        {
            ItemID = 0x2601;
            Hue = hue;
        }

        #region serialization
        public PlumedHelm( Serial serial )
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