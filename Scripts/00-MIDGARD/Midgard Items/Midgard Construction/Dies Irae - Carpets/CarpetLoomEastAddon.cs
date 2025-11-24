using Server;
using Server.Items;

namespace Midgard.Items
{
    [Furniture]
    public class CarpetLoomEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new CarpetLoomEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public CarpetLoomEastAddon()
        {
            AddComponent( new AddonComponent( 0x1060 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 0x105F ), 0, 1, 0 );
        }

        public CarpetLoomEastAddon( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }

    public class CarpetLoomEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new CarpetLoomEastAddon(); } }
        public override int LabelNumber { get { return 1064438; } }

        [Constructable]
        public CarpetLoomEastDeed()
        {
        }

        public CarpetLoomEastDeed( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}