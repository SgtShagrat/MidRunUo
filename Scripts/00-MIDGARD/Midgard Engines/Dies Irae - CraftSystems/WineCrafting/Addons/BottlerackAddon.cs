using Server;
using Server.Items;

namespace Midgard.Engines.WineCrafting
{
    public class BottlerackAddon : BaseAddon
    {
        public override BaseAddonDeed Deed
        {
            get
            {
                return new BottlerackAddonDeed();
            }
        }

        [Constructable]
        public BottlerackAddon()
        {
            AddComponent( new AddonComponent( 2182 ), 0, 2, 0 );
            AddComponent( new AddonComponent( 2182 ), 0, 2, 12 );
            AddComponent( new AddonComponent( 2182 ), 0, 2, 6 );
            AddComponent( new AddonComponent( 2182 ), 0, 1, 12 );
            AddComponent( new AddonComponent( 2182 ), 0, 1, 6 );
            AddComponent( new AddonComponent( 2182 ), 0, 1, 0 );
            AddComponent( new AddonComponent( 2182 ), 0, 0, 12 );
            AddComponent( new AddonComponent( 2182 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 2182 ), 0, 0, 6 );
            AddComponent( new AddonComponent( 2182 ), 0, -1, 12 );
            AddComponent( new AddonComponent( 2182 ), 0, -1, 6 );
            AddComponent( new AddonComponent( 2182 ), 0, -1, 0 );
        }

        public BottlerackAddon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 ); // Version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class BottlerackAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon
        {
            get
            {
                return new BottlerackAddon();
            }
        }

        [Constructable]
        public BottlerackAddonDeed()
        {
            Name = "Bottle Rack";
        }

        public BottlerackAddonDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 ); // Version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}