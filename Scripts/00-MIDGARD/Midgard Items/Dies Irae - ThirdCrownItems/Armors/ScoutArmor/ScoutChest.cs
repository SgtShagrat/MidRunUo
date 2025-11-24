using Midgard.Engines.Classes;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    /// <summary>
    /// 0x1162 Scout Chest - ( Nel craft del sarto , indossabile solo da classe : scout )
    /// </summary>
    public class ScoutChest : LeatherChest
    {
        public override string DefaultName { get { return "scout chest"; } }

        [Constructable]
        public ScoutChest()
            : this( 1810 )
        {
        }

        [Constructable]
        public ScoutChest( int hue )
        {
            ItemID = 0x1162;
            Hue = hue;
        }

        public override Classes RequiredClass
        {
            get { return Classes.Scout; }
        }

        #region serialization
        public ScoutChest( Serial serial )
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