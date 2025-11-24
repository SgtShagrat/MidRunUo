using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class HighlandFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public HighlandFrenziedOstard()
            : base( "a highland frenzied ostard", 1325 )
        {
        }

        public HighlandFrenziedOstard( Serial serial )
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