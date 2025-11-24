using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PlateGorgetOfWater : PlateGorget, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Plate Gorget Of The Water Element"; } }

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        public override int OldStrReq { get { return 100; } }
        public override int OldDexBonus { get { return -1; } }

        public override int ArmorBase { get { return 55; } }

        [Constructable]
        public PlateGorgetOfWater()
        {
            Hue = 0x880;
            Weight = 7.0;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Armor bonus: {0}\n", ArmorBase );
            builder.AppendFormat( "Strenght required: {0}\n", OldStrReq );
            builder.AppendFormat( "Dexterity malus: {0}\n", OldDexBonus );
        }

        #region serialization
        public PlateGorgetOfWater( Serial serial )
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