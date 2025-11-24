using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class EmeraldFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public EmeraldFrenziedOstard()
            : base( "a emerald frenzied ostard", 1920 )
        {

        }

        public EmeraldFrenziedOstard( Serial serial )
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