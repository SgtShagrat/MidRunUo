using Server;
using Server.Items;

namespace Midgard.Items
{
    public class GrandMageRefreshElixirGreater : BaseGrandMageRefreshPotion
    {
        public override int Strength
        {
            get { return 10; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 90.0; }
        }

        public override double MageryRequired
        {
            get { return 100.0; }
        }

        [Constructable]
        public GrandMageRefreshElixirGreater( int amount )
            : base( PotionEffect.ManaRefreshGreater, amount )
        {
            // Name = "Greater Grand Mage Refresh Elixir";
            Hue = 2447;
        }

        [Constructable]
        public GrandMageRefreshElixirGreater()
            : this( 1 )
        {
        }

        #region serial deserial
        public GrandMageRefreshElixirGreater( Serial serial )
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