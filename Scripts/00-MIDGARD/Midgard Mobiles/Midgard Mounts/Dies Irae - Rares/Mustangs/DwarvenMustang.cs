using Server;

namespace Midgard.Mobiles
{
    public class DwarvenMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public DwarvenMustang()
            : base( "a dwarven mustang", 1921 )
        {
        }

        #region serial-deserial
        public DwarvenMustang( Serial serial )
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