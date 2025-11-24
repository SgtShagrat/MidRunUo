using Server;

namespace Midgard.Mobiles
{
    public class OldriverMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public OldriverMustang()
            : base( "an oldriver mustang", 53 )
        {
        }

        public OldriverMustang( Serial serial )
            : base( serial )
        {
        }

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