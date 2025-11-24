using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class ValleyFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public ValleyFrenziedOstard()
            : base( "a valley frenzied ostard", 1991 )
        {
        }

        public ValleyFrenziedOstard( Serial serial )
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