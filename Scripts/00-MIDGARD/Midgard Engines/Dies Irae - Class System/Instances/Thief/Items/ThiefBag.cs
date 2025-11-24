using Midgard.Engines.AdvancedDisguise;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class ThiefBag : Bag
    {
        public override string DefaultName
        {
            get { return "a bag of thief tools"; }
        }

        [Constructable]
        public ThiefBag()
            : this( 10 )
        {
            Movable = true;
            Hue = 2444;
        }

        [Constructable]
        public ThiefBag( int amount )
        {
            DropItem( new SketchBook() );
            // DropItem( new DisguiseKit() );
            DropItem( new DisguiseCleaner() );

            for( int i = 0; i < 10; i++ )
                DropItem( new NarcoticBandage() );

            DropItem( new BackstabbingKnife() );

            DropItem( new RegularNarcoticPotion( amount ) );
            DropItem( new MediumNarcoticPotion( amount ) );
            DropItem( new HeavyNarcoticPotion( amount ) );
        }

        #region serial deserial
        public ThiefBag( Serial serial )
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