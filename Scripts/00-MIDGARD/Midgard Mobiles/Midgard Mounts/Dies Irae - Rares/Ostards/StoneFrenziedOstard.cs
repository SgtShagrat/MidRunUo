using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class StoneFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public StoneFrenziedOstard()
            : base( "a stone frenzied ostard", 0x482 )
        {
        }

        public StoneFrenziedOstard( Serial serial )
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