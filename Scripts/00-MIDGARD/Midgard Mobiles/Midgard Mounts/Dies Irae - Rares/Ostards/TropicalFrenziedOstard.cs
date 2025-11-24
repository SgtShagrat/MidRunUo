using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class TropicalFrenziedOstard : BaseOstard
    {
        [Constructable( AccessLevel.Administrator )]
        public TropicalFrenziedOstard()
            : base( "a tropical frenzied ostard", 1346 )
        {
        }

        public TropicalFrenziedOstard( Serial serial )
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