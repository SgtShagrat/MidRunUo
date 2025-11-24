using Server;
using Server.Items;

namespace Midgard.Items
{
    public class GrandMageRefreshElixirLesser : BaseGrandMageRefreshPotion
    {
        public override int Strength
        {
            get { return 3; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 50.0; }
        }

        public override double MageryRequired
        {
            get { return 80; }
        }

        [Constructable]
        public GrandMageRefreshElixirLesser( int amount )
            : base( PotionEffect.ManaRefreshLesser, amount )
        {
            // Name = "Lesser Grand Mage Refresh Elixir";
            Hue = 2067;
        }

        [Constructable]
        public GrandMageRefreshElixirLesser()
            : this( 1 )
        {
        }

        #region serial deserial
        public GrandMageRefreshElixirLesser( Serial serial )
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