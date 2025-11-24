using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a highland llama corpse" )]
    public class HighlandRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public HighlandRidableLlama()
            : base( "a highland ridable llama", 1325 )
        {

        }

        public HighlandRidableLlama( Serial serial )
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