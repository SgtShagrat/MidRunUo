using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an ostard corpse" )]
    public class MountainFrenziedOstard : BaseOstard
    {        
        [Constructable( AccessLevel.Administrator )]
        public MountainFrenziedOstard()
            : base( "a mountain frenzied ostard", 1921 )
        {
        }

        public MountainFrenziedOstard( Serial serial )
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