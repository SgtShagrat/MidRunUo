using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class PlainFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public PlainFrenziedOstard()
            : base( "a plain frenzied ostard", 0x35 )
        {
        }

        public PlainFrenziedOstard( Serial serial )
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