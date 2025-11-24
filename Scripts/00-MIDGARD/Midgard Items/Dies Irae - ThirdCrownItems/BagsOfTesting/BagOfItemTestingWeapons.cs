using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfItemTestingWeapons : Bag
    {
        [Constructable]
        public BagOfItemTestingWeapons()
        {
            DropItem( new BoneBlade() );
            DropItem( new DarkenStaff() );
            DropItem( new DarkScythe() );
            DropItem( new DoubleBoneBlade() );
            DropItem( new DoubleHatchet() );
            DropItem( new DoubleKatana() );
            DropItem( new DoubleKryss() );
            DropItem( new DrowPike() );

            DropItem( new DwarvenAxe() );
            DropItem( new ElvenLongSword() );
            DropItem( new HordeCutlass() );
            DropItem( new HordeShield() );
            DropItem( new LightStaff() );
        }

        #region serialization
        public BagOfItemTestingWeapons( Serial serial )
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