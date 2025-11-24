using Server;

namespace Midgard.Engines.Classes
{
    public class DruidBook : BaseClassPowersBook
    {
        public override ClassSystem BookSystem
        {
            get { return ClassSystem.Druid; }
        }

        [Constructable]
        public DruidBook() : base( 0x2D50 )
        {
        }

        #region serialization
        public DruidBook( Serial serial )
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