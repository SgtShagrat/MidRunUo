using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfScoutArmor : Bag
    {
        [Constructable]
        public BagOfScoutArmor()
        {
            DropItem( new ScoutChest() );
            DropItem( new ScoutGloves() );
            DropItem( new ScoutGorget() );
            DropItem( new ScoutHelm() );
            DropItem( new ScoutLegs() );
            DropItem( new ScoutSleeves() );
        }

        #region serialization
        public BagOfScoutArmor( Serial serial )
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