using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class HeavenlyFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public HeavenlyFrenziedOstard()
            : base( "a heavenly frenzied ostard", 1942 )
        {

        }

        public HeavenlyFrenziedOstard( Serial serial )
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