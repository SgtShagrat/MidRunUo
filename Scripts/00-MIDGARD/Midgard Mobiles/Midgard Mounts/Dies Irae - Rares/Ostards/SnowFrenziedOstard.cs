using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class SnowFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public SnowFrenziedOstard()
            : base( "a snow frenzied ostard", 0x481 )
        {
        }

        public SnowFrenziedOstard( Serial serial )
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