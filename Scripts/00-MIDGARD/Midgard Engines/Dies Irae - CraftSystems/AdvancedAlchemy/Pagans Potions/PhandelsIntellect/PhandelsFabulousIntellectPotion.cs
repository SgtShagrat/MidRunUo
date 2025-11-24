using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PhandelsFabulousIntellectPotion : BasePhandelsIntellectPotion
    {
        public override int Strength
        {
            get { return 4; }
        }

        [Constructable]
        public PhandelsFabulousIntellectPotion( int amount )
            : base( PotionEffect.Intelligence, amount )
        {
            // Name = "Phandel's Fabulous Intellect Potion";
            Hue = 2499;
        }

        [Constructable]
        public PhandelsFabulousIntellectPotion()
            : this( 1 )
        {
        }

        #region serial deserial
        public PhandelsFabulousIntellectPotion( Serial serial )
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