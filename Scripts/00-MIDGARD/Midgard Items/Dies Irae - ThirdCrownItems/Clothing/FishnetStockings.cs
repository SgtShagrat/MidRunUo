using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3360 Fishnet Stockings trad. calze a rete - ( craftabile dai sarti,colorabile da hue )
    /// </summary>
    public class FishnetStockings : BasePants // da verificare!!
    {
        public override string DefaultName { get { return "fishnet stockings"; } }

        public override bool AllowMaleWearer { get { return false; } }

        [Constructable]
        public FishnetStockings()
            : this( 0 )
        {
        }

        [Constructable]
        public FishnetStockings( int hue )
            : base( 0x3360, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public FishnetStockings( Serial serial )
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