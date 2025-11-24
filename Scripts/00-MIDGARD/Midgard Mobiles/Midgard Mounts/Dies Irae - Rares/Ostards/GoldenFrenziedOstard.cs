using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class GoldenFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public GoldenFrenziedOstard()
            : base( "a golden frenzied ostard", 0x7E8 )
        {

        }

        public GoldenFrenziedOstard( Serial serial )
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