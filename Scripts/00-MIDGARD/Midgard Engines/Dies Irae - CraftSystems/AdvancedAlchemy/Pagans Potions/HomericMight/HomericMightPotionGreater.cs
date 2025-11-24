using Server;
using Server.Items;

namespace Midgard.Items
{
    public class HomericMightPotionGreater : BaseHomericMightPotion
    {
        public override int Strength
        {
            get { return 5; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 70.0; }
        }

        [Constructable]
        public HomericMightPotionGreater( int amount )
            : base( PotionEffect.BlessGreater, amount )
        {
            // Name = "Greater Homeric Might Potion";
            Hue = 2068;
        }

        [Constructable]
        public HomericMightPotionGreater()
            : this( 1 )
        {
        }

        #region serial deserial
        public HomericMightPotionGreater( Serial serial )
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