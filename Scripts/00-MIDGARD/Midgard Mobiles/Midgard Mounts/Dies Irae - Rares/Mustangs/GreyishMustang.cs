using Server;

namespace Midgard.Mobiles
{
    public class GreyishMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public GreyishMustang()
            : base( "a greysh mustang", 1411 )
        {
        }
        
        #region serial-deserial
        public GreyishMustang( Serial serial )
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