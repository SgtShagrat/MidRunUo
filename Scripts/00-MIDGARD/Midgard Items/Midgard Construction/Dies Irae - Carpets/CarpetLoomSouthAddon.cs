using Server;
using Server.Items;

namespace Midgard.Items
{
    [Furniture]
    public class CarpetLoomSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new CarpetLoomSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public CarpetLoomSouthAddon()
        {
            AddComponent( new AddonComponent( 0x1061 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 0x1062 ), 1, 0, 0 );
        }

        public CarpetLoomSouthAddon( Serial serial )
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

    public class CarpetLoomSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new CarpetLoomSouthAddon(); } }
        public override int LabelNumber { get { return 1064438; } } // loom (south)

        [Constructable]
        public CarpetLoomSouthDeed()
        {
        }

        public CarpetLoomSouthDeed( Serial serial )
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