using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "an emerald llama corpse" )]
    public class EmeraldRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public EmeraldRidableLlama()
            : base( "an emerald ridable llama", 1920 )
        {

        }

        public EmeraldRidableLlama( Serial serial )
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