using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class MidgardKryss : Kryss, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Midgard Kryss"; } }

        public override int OldStrengthReq { get { return 90; } }
        public override int OldSpeed { get { return 60; } }

        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        public override int NumDice { get { return 1; } }
        public override int NumSides { get { return 26; } }
        public override int DiceBonus { get { return 10; } }

        [Constructable]
        public MidgardKryss()
        {
            Weight = 6.0;
            Hue = 0x48D;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Strenght required: {0}\n", OldStrengthReq );
            builder.AppendFormat( "Speed: {0}\n", OldSpeed );
            builder.AppendFormat( "Damage: {0}d{1}+{2}\n", NumDice, NumSides, DiceBonus );
        }

        #region serialization
        public MidgardKryss( Serial serial )
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