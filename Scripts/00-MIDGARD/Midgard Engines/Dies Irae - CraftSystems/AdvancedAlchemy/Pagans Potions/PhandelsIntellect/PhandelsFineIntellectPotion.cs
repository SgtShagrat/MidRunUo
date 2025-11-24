using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PhandelsFineIntellectPotion : BasePhandelsIntellectPotion
    {
        public override int Strength
        {
            get { return 3; }
        }

        [Constructable]
        public PhandelsFineIntellectPotion( int amount )
            : base( PotionEffect.IntelligenceLesser, amount )
        {
            // Name = "Phandel's Fine Intellect Potion";
            Hue = 2458;
        }

        [Constructable]
        public PhandelsFineIntellectPotion()
            : this( 1 )
        {
        }

        public PhandelsFineIntellectPotion( Serial serial )
            : base( serial )
        {
        }

        #region serial deserial
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