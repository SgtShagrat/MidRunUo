using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class FireFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public FireFrenziedOstard()
            : base( "a fire frenzied ostard", 1919 )
        {

        }

        public FireFrenziedOstard( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
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