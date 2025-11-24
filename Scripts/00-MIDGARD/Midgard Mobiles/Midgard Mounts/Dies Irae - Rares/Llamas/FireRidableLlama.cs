using Server;

namespace Midgard.Mobiles
{
    [CorpseName( "a fire llama corpse" )]
    public class FireRidableLlama : BaseLlama
    {        
        [Constructable( AccessLevel.Administrator )]
        public FireRidableLlama()
            : base( "a fire ridable llama", 1161 )
        {

        }

        public FireRidableLlama( Serial serial )
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