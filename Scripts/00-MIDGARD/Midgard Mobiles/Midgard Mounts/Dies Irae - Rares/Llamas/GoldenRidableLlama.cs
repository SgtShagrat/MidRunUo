using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a golden llama corpse" )]
    public class GoldenRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public GoldenRidableLlama()
            : base( "a golden ridable llama", 0x7E8 )
        {

        }

        public GoldenRidableLlama( Serial serial )
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