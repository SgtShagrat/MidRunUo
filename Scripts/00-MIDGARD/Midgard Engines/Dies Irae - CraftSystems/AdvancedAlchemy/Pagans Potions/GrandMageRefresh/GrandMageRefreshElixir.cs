using Server;
using Server.Items;

namespace Midgard.Items
{
    public class GrandMageRefreshElixir : BaseGrandMageRefreshPotion
    {
        public override int Strength
        {
            get { return 5; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 70.0; }
        }

        public override double MageryRequired
        {
            get { return 90; }
        }

        [Constructable]
        public GrandMageRefreshElixir( int amount )
            : base( PotionEffect.ManaRefresh, amount )
        {
            // Name = "Grand Mage Refresh Elixir";
            Hue = 2072;
        }

        [Constructable]
        public GrandMageRefreshElixir()
            : this( 1 )
        {
        }

        public GrandMageRefreshElixir( Serial serial )
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