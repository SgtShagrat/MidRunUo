using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a shadow llama corpse" )]
    public class ShadowRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public ShadowRidableLlama()
            : base( "a shadow ridable llama", 0x4E20 )
        {

        }

        public ShadowRidableLlama( Serial serial )
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