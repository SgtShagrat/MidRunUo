using Server;

namespace Midgard.Mobiles
{
    public class JungleMustang : BaseMustang
    {        
        [Constructable( AccessLevel.Administrator )]
        public JungleMustang()
            : base( "a jungle mustang", 1920 )
        {
        }
        
        #region serial-deserial
        public JungleMustang( Serial serial )
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