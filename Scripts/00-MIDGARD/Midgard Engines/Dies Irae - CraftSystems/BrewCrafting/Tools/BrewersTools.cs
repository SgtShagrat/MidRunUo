using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Engines.BrewCrafing
{
    public class BrewersTools : BaseTool
    {
        public override CraftSystem CraftSystem
        {
            get { return DefBrewing.CraftSystem; }
        }

        [Constructable]
        public BrewersTools()
            : base( 0xE7F )
        {
            Weight = 2.0;
            Name = "Brewer's Tools";
            Hue = 0x3EF;
        }

        public BrewersTools( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}