using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfSiegeItems : Bag
    {
        [Constructable]
        public BagOfSiegeItems()
        {
            DropItem( new SiegeRepairTool() );

            DropItem( new SiegeCannonDeed() );
            DropItem( new LightCannonball( 10 ) );
            DropItem( new IronCannonball( 10 ) );
            DropItem( new ExplodingCannonball( 10 ) );
            DropItem( new FieryCannonball( 10 ) );
            DropItem( new GrapeShot( 10 ) );

            DropItem( new SiegeCatapultDeed() );

            DropItem( new SiegeRamDeed() );
            DropItem( new LightSiegeLog( 10 ) );
            DropItem( new HeavySiegeLog( 10 ) );
            DropItem( new IronSiegeLog( 10 ) );
        }

        #region serialization
        public BagOfSiegeItems( Serial serial )
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