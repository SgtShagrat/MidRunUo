using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a greyish llama corpse" )]
    public class GreyishRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public GreyishRidableLlama()
            : base( "a greyish ridable llama", 0x583 )
        {

        }

        public GreyishRidableLlama( Serial serial )
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