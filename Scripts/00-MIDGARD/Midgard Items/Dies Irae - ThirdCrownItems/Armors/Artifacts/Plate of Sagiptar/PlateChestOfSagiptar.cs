using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PlateChestOfSagiptar : PlateChest, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Plate Chest Of Sagiptar"; } }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        public override int OldStrReq { get { return 80; } }
        public override int OldDexBonus { get { return -8; } }

        public override int ArmorBase { get { return 55; } }

        [Constructable]
        public PlateChestOfSagiptar()
        {
            Hue = 0x875;
            Weight = 25.0;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Armor bonus: {0}\n", ArmorBase );
            builder.AppendFormat( "Strenght required: {0}\n", OldStrReq );
            builder.AppendFormat( "Dexterity malus: {0}\n", OldDexBonus );
        }

        #region serialization
        public PlateChestOfSagiptar( Serial serial )
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