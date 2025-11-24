using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a shining llama corpse" )]
    public class ShiningRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public ShiningRidableLlama()
            : base( "a shining ridable llama", 0x9A0 )
        {

        }

        public ShiningRidableLlama( Serial serial )
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