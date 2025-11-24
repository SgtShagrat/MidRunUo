using Server;

namespace Midgard.Engines.Races
{
    public class UndeadBody : Item
    {
        [Constructable]
        public UndeadBody()
            : base( 16087 )
        {
            Layer = Layer.Cloak;
        }

        public UndeadBody( Serial serial )
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