using Server;

namespace Midgard.Mobiles
{
    public class CloudMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public CloudMustang()
            : base( "a cloud mustang", 0x482 )
        {
        }

        #region serial-deserial
        public CloudMustang( Serial serial )
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