using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PaganAlchemicalBag : Bag
    {
        public override int LabelNumber { get { return 1064035; } } // pagan alchemical bag

        [Constructable]
        public PaganAlchemicalBag()
            : this( 10 )
        {
        }

        [Constructable]
        public PaganAlchemicalBag( int amount )
        {
            DropItem( new GrandMageRefreshElixirLesser( amount ) );
            DropItem( new GrandMageRefreshElixir( amount ) );
            DropItem( new GrandMageRefreshElixirGreater( amount ) );

            DropItem( new HomericMightPotion( amount ) );
            DropItem( new HomericMightPotionGreater( amount ) );

            DropItem( new MegoInvulnerabilityPotionLesser( amount ) );
            DropItem( new MegoInvulnerabilityPotion( amount ) );
            DropItem( new MegoInvulnerabilityPotionGreater( amount ) );

            DropItem( new PhandelsFineIntellectPotion( amount ) );
            DropItem( new PhandelsFabulousIntellectPotion( amount ) );
            DropItem( new PhandelsFantasticIntellectPotion( amount ) );

            DropItem( new PaganInvisibilityPotion( amount ) );
            DropItem( new TamlaPotion( amount ) );

            DropItem( new TaintsMinorTransmutationPotion( amount ) );
            DropItem( new TaintsMajorTransmutationPotion( amount ) );

            DropItem( new Totem( amount ) );
            DropItem( new Elixir( amount ) );

            DropItem( new FlashBangPotion( amount ) );

            Hue = 1154;
        }

        #region serial deserial
        public PaganAlchemicalBag( Serial serial )
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