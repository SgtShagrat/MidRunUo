using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfTraps : Bag
    {
        public override string DefaultName
        {
            get
            {
                return "a bag of traps";
            }
        }

        [Constructable]
        public BagOfTraps()
            : this( 10 )
        {
            Movable = true;
            Hue = 2444;
        }

        [Constructable]
        public BagOfTraps( int amount )
        {
            DropItem( new TrapRemovalKit( amount ) );

            for( int i = 0; i < amount; i++ )
            {
                DropItem( new LightSpikeTrapDeed() );
                DropItem( new MediumSpikeTrapDeed() );
                DropItem( new HeavySpikeTrapDeed() );

                DropItem( new LightSawTrapDeed() );
                DropItem( new MediumSawTrapDeed() );
                DropItem( new HeavySawTrapDeed() );

                DropItem( new LightGasTrapDeed() );
                DropItem( new MediumGasTrapDeed() );
                DropItem( new HeavyGasTrapDeed() );

                DropItem( new LightExplosionTrapDeed() );
                DropItem( new MediumExplosionTrapDeed() );
                DropItem( new HeavyExplosionTrapDeed() );

                DropItem( new LightDismountTrapDeed() );
                DropItem( new MediumDismountTrapDeed() );
                DropItem( new HeavyDismountTrapDeed() );
            }
        }

        #region serial deserial
        public BagOfTraps( Serial serial )
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