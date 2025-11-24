/***************************************************************************
 *                               BagOfScoutEquipment.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfScoutEquipment : Bag
    {
        [Constructable]
        public BagOfScoutEquipment()
        {
            DropItem( new ScoutBlueTentRoll() );
            DropItem( new ScoutGreenTentRoll() );

            DropItem( new ScoutThrowingDagger() );
            DropItem( new ScoutThrowingHatchet() );

            DropItem( new BagOfScoutArmor() );

            DropItem( new IncendiaryArrow( 25 ) );
            DropItem( new AcidArrow( 25 ) );
            DropItem( new DisarmingArrow( 25 ) );
            DropItem( new DismountingArrow( 25 ) );
            DropItem( new StuningArrow( 25 ) );
        }

        #region serialization
        public BagOfScoutEquipment( Serial serial )
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