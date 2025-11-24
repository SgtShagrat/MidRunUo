using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PlateGorgetOfSagiptar : PlateGorget, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Plate Gorget Of Sagiptar"; } }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        public override int OldStrReq { get { return 80; } }
        public override int OldDexBonus { get { return -1; } }

        public override int ArmorBase { get { return 55; } }

        [Constructable]
        public PlateGorgetOfSagiptar()
        {
            Hue = 0x875;
            Weight = 7.0;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Armor bonus: {0}\n", ArmorBase );
            builder.AppendFormat( "Strenght required: {0}\n", OldStrReq );
            builder.AppendFormat( "Dexterity malus: {0}\n", OldDexBonus );
        }

        #region serialization
        public PlateGorgetOfSagiptar( Serial serial )
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