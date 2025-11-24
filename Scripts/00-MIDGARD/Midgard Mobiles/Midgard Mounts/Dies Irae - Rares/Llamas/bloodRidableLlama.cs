using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a blood llama corpse" )]
    public class BloodRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public BloodRidableLlama()
            : base( "a blood ridable llama", 0x66D )
        {
        }

        public BloodRidableLlama( Serial serial )
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