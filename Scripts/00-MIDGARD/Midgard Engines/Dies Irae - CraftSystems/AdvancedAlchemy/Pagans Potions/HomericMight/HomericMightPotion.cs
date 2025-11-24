using Server;
using Server.Items;

namespace Midgard.Items
{
    public class HomericMightPotion : BaseHomericMightPotion
    {
        public override int Strength
        {
            get { return 4; }
        }

        [Constructable]
        public HomericMightPotion( int amount )
            : base( PotionEffect.Bless, amount )
        {
            // Name = "Homeric Might Potion";
            Hue = 2446;
        }

        [Constructable]
        public HomericMightPotion()
            : this( 1 )
        {
        }

        #region serial deserial
        public HomericMightPotion( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}