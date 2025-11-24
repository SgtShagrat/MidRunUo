using Server;

namespace Midgard.Mobiles
{
    public class StormMustang : BaseMustang
    {
        [Constructable( AccessLevel.Administrator )]
        public StormMustang()
            : base( "a storm mustang", 1354 )
        {
        }

        #region serialization
        public StormMustang( Serial serial )
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