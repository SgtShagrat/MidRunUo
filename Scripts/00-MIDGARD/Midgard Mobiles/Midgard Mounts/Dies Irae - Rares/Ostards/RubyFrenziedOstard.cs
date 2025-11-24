using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class RubyFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public RubyFrenziedOstard()
            : base( "a ruby frenzied ostard", 1645 )
        {
        }

        public RubyFrenziedOstard( Serial serial )
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