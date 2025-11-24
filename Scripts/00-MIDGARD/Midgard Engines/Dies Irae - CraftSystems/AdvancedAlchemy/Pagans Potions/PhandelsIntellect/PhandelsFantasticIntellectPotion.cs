using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PhandelsFantasticIntellectPotion : BasePhandelsIntellectPotion
    {
        public override int Strength
        {
            get { return 5; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 60.0; }
        }

        [Constructable]
        public PhandelsFantasticIntellectPotion( int amount )
            : base( PotionEffect.IntelligenceGreater, amount )
        {
            // Name = "Phandel's Fantastic Intellect Potion";
            Hue = 2468;
        }

        [Constructable]
        public PhandelsFantasticIntellectPotion()
            : this( 1 )
        {
        }

        #region serial deserial
        public PhandelsFantasticIntellectPotion( Serial serial )
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