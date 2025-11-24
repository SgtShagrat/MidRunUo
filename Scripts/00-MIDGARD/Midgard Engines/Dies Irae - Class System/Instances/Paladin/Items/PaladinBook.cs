using Server;

namespace Midgard.Engines.Classes
{
    public class PaladinBook : BaseClassPowersBook
    {
        public override ClassSystem BookSystem
        {
            get { return ClassSystem.Paladin; }
        }

        [Constructable]
        public PaladinBook() : base( 0x2252 )
        {
        }

        #region serialization
        public PaladinBook( Serial serial )
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