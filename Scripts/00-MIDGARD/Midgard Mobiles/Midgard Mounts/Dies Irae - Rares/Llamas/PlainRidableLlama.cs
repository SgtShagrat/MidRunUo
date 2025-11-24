using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a plain llama corpse" )]
    public class PlainRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public PlainRidableLlama()
            : base( "a plain ridable llama", 2464 )
        {

        }

        public PlainRidableLlama( Serial serial )
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