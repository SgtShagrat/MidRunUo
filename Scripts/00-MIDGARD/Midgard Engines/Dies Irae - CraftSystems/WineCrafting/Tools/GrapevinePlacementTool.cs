using Server;

namespace Midgard.Engines.WineCrafting
{
    public class GrapevinePlacementTool : Item
    {
        [Constructable]
        public GrapevinePlacementTool()
            : base( 0xD1A )
        {
            Weight = 1.0;
            Hue = 0x489;
            Name = "Grapevine Placement Tool";
        }

        public GrapevinePlacementTool( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.SendGump( new AddGrapeVineGump( from, null, 0 ) );
        }

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
    }
}