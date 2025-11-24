using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class BagOfTreeSeeds : Bag
    {
        public override int LabelNumber
        {
            get { return 1065773; } // bag of seeds
        }

        [Constructable]
        public BagOfTreeSeeds()
            : this( 50 )
        {
        }

        [Constructable]
        public BagOfTreeSeeds( int amount )
        {
            DropItem( new AppleTreeSeed( amount ) );
            DropItem( new BananaTreeSeed( amount ) );
            DropItem( new CedarTreeSeed( amount ) );
            DropItem( new CherryTreeSeed( amount ) );
            DropItem( new CoconutPalmSeed( amount ) );

            DropItem( new CommonTreeSeed( amount ) );
            DropItem( new CypressTreeSeed( amount ) );
            DropItem( new DatePalmSeed( amount ) );
            DropItem( new HedgeSeed( amount ) );
            DropItem( new MapleTreeSeed( amount ) );

            DropItem( new OakTreeSeed( amount ) );
            DropItem( new OhiiTreeSeed( amount ) );
            DropItem( new PeachTreeSeed( amount ) );
            DropItem( new PearTreeSeed( amount ) );
            DropItem( new PlumTreeSeed( amount ) );

            DropItem( new WalnutTreeSeed( amount ) );
            DropItem( new WillowTreeSeed( amount ) );

            Hue = 2024;
        }

        public BagOfTreeSeeds( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial

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