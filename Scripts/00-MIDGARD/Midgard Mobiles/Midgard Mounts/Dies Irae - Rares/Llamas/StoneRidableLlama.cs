using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a stone llama corpse" )]
    public class StoneRidableLlama : BaseLlama
    {    
        [Constructable( AccessLevel.Administrator )]
        public StoneRidableLlama()
            : base( "a stone ridable llama", 0x482 )
        {

        }

        public StoneRidableLlama( Serial serial )
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