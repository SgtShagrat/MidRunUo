using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a mountain llama corpse" )]
    public class MountainRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public MountainRidableLlama()
            : base( "a mountain ridable llama", 1921 )
        {

        }

        public MountainRidableLlama( Serial serial )
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