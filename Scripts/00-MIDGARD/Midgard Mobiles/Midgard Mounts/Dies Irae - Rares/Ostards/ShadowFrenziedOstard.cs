using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class ShadowFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public ShadowFrenziedOstard()
            : base( "a shadow frenzied ostard", 20000 )
        {
        }

        public ShadowFrenziedOstard( Serial serial )
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