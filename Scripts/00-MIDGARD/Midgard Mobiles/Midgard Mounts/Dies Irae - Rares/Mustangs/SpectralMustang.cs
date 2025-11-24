using Server;

namespace Midgard.Mobiles
{
    public class SpectralMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public SpectralMustang()
            : base( "a spectral mustang", 20000 )
        {
        }

        public SpectralMustang( Serial serial )
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