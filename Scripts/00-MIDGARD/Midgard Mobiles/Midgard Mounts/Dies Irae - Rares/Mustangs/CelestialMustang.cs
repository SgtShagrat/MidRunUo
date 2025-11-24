using Server;

namespace Midgard.Mobiles
{
    public class CelestialMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public CelestialMustang()
            : base( "a celestial mustang", 90 )
        {
        }

        #region serial-deserial
        public CelestialMustang( Serial serial )
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