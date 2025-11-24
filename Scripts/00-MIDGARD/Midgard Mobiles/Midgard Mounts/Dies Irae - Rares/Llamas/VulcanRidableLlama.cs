using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a vulcan llama corpse" )]
    public class VulcanRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public VulcanRidableLlama()
            : base( "a vulcan ridable llama", 1919 )
        {

        }

        public VulcanRidableLlama( Serial serial )
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