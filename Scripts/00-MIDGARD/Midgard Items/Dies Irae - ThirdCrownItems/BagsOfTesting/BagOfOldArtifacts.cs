using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfOldArtifacts : Bag
    {
        [Constructable]
        public BagOfOldArtifacts()
        {
            DropItem( new GnarledStaffOfPoison() );
            DropItem( new ClubOfPoison() );
            DropItem( new MidgardLongSword() );

            DropItem( new MidgardRobe() );
            DropItem( new MidgardVikingSword() );
            DropItem( new MidgardHalberd() );
            DropItem( new MidgardKryss() );
            DropItem( new MidgardStaff() );
            DropItem( new MidgardKatana() );
            DropItem( new MidgardWarMace() );
            DropItem( new MidgardWarAxe() );
            DropItem( new WarForkOfWater() );
            DropItem( new DaggerOfExtraStrike() );

            DropItem( new FemalePlateOfDarkness() );
            DropItem( new PlateHelmOfDarkness() );
            DropItem( new PlateArmsOfDarkness() );
            DropItem( new PlateLegsOfDarkness() );
            DropItem( new PlateGlovesOfDarkness() );
            DropItem( new PlateGorgetOfDarkness() );
            DropItem( new PlateChestOfDarkness() );

            DropItem( new FemalePlateOfRyous() );
            DropItem( new PlateHelmOfRyous() );
            DropItem( new PlateArmsOfRyous() );
            DropItem( new PlateLegsOfRyous() );
            DropItem( new PlateGlovesOfRyous() );
            DropItem( new PlateGorgetOfRyous() );
            DropItem( new PlateChestOfRyous() );

            DropItem( new FemalePlateOfSagiptar() );
            DropItem( new PlateHelmOfSagiptar() );
            DropItem( new PlateArmsOfSagiptar() );
            DropItem( new PlateLegsOfSagiptar() );
            DropItem( new PlateGlovesOfSagiptar() );
            DropItem( new PlateGorgetOfSagiptar() );
            DropItem( new PlateChestOfSagiptar() );

            DropItem( new LeatherGorgetOfWater() );
            DropItem( new LeatherArmsOfWater() );
            DropItem( new LeatherGlovesOfWater() );
            DropItem( new LeatherCapOfWater() );
            DropItem( new LeatherLegsOfWater() );
            DropItem( new LeatherTunicOfWater() );

            DropItem( new PlateHelmOfWater() );
            DropItem( new PlateArmsOfWater() );
            DropItem( new PlateLegsOfWater() );
            DropItem( new PlateGlovesOfWater() );
            DropItem( new PlateGorgetOfWater() );
            DropItem( new PlateChestOfWater() );
        }

        #region serialization
        public BagOfOldArtifacts( Serial serial )
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