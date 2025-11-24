using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38c9 Scout helm - ( Nel craft del sarto , indossabile solo da classe : scout )
    /// </summary>
    public class ScoutHelm : LeatherCap
    {
        public override string DefaultName { get { return "scout helm"; } }

        [Constructable]
        public ScoutHelm()
            : this( 1810 )
        {
        }

        [Constructable]
        public ScoutHelm( int hue )
        {
            ItemID = 0x38c9;
            Hue = hue;
        }

        public override Classes RequiredClass
        {
            get { return Classes.Scout; }
        }

        #region serialization
        public ScoutHelm( Serial serial )
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