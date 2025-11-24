using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfPracticeWeapons : Bag
    {
        [Constructable]
        public BagOfPracticeWeapons()
        {
            DropItem( new PracticeShepherdsCrook() );
            DropItem( new PracticeGnarledStaff() );
            DropItem( new PracticeWoodenSpear() );
            DropItem( new PracticeBow() );

            DropItem( new PracticeWoodenHatchet() );
            DropItem( new PracticeWoodenMace() );
            DropItem( new PacticeWoodenLongsword() );
            DropItem( new PacticeSkinningKnife() );

            DropItem( new PracticeWoodenKryss() );
            DropItem( new PracticeBattleAxe() );
            DropItem( new PracticeClub() );
        }

        #region serialization
        public BagOfPracticeWeapons( Serial serial )
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