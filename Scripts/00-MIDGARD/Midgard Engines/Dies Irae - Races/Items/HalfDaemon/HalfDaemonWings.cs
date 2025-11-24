using Server;

namespace Midgard.Engines.Races
{
    public class HalfDaemonWings : Item
    {
        [Constructable]
        public HalfDaemonWings()
            : base( 0x38C8 )
        {
            Layer = Layer.Cloak;
        }

        public HalfDaemonWings( Serial serial )
            : base( serial )
        {
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
        }

        public override bool IsVirtualItem { get { return true; } }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}