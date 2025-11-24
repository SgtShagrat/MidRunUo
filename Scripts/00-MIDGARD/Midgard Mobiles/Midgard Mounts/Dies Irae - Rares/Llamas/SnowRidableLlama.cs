using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a snow llama corpse" )]
    public class SnowRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public SnowRidableLlama()
            : base( "a snow ridable llama", 1915 )
        {

        }

        public SnowRidableLlama( Serial serial )
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