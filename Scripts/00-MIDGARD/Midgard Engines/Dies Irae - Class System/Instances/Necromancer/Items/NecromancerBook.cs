using Server;

namespace Midgard.Engines.Classes
{
    public class NecromancerBook : BaseClassPowersBook
    {
        public override ClassSystem BookSystem
        {
            get { return ClassSystem.Necromancer; }
        }

        [Constructable]
        public NecromancerBook() : base( 0x2252 )
        {
        }

        #region serialization
        public NecromancerBook( Serial serial )
        : base( serial )
        {
        }

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