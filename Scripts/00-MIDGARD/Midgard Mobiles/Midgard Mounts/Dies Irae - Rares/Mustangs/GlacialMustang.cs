using Server;

namespace Midgard.Mobiles
{
    public class GlacialMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public GlacialMustang()
            : base( "a glacial mustang", 1928 )
        {
        }

        #region serial-deserial
        public GlacialMustang( Serial serial )
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
        #endregion
    }
}