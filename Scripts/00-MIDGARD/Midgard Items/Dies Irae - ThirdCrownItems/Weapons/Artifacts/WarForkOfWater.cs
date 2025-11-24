using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class WarForkOfWater : WarFork, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "War Fork of the Water Element"; } }

        public override int OldStrengthReq { get { return 90; } }
        public override int OldSpeed { get { return 80; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 30; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 17; } }
        public override int DiceBonus { get { return 3; } }

        [Constructable]
        public WarForkOfWater()
        {
            Weight = 14.0;
            Hue = 0x880;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Strenght required: {0}\n", OldStrengthReq );
            builder.AppendFormat( "Speed: {0}\n", OldSpeed );
            builder.AppendFormat( "Damage: {0}d{1}+{2}\n", NumDice, NumSides, DiceBonus );
        }

        #region serialization
        public WarForkOfWater( Serial serial )
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