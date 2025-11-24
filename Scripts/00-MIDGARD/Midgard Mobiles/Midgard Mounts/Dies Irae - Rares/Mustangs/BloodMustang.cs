using Server;

namespace Midgard.Mobiles
{
    public class BloodMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public BloodMustang()
            : base( "a blood mustang", 1645 )
        {
        }

        #region serial-deserial
        public BloodMustang( Serial serial )
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