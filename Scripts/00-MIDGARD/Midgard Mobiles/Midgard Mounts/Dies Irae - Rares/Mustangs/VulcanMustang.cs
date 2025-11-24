using Server;

namespace Midgard.Mobiles
{
    public class VulcanMustang : BaseMustang
    {  
        [Constructable( AccessLevel.Administrator )]
        public VulcanMustang()
            : base( "a vulcan mustang", 1919 )
        {
        }

        #region serial-deserial
        public VulcanMustang( Serial serial )
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