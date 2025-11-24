using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DoorTrapsBag : Bag
    {
        public override string DefaultName
        {
            get
            {
                return "a bag of door traps";
            }
        }

        [Constructable]
        public DoorTrapsBag()
        : this( 10 )
        {
            Movable = true;
            Hue = 2444;
        }

        [Constructable]
        public DoorTrapsBag( int amount )
        {
            for( int i = 0; i < amount; i++ )
            {
                DropItem( new ExplosionTrapDeed() );
                DropItem( new DartTrapDeed() );
                DropItem( new PoisonTrapDeed() );
            }
        }

        #region serial deserial
        public DoorTrapsBag( Serial serial )
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