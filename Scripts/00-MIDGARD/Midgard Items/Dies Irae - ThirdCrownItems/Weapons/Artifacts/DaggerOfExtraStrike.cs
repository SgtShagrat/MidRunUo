using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class DaggerOfExtraStrike : Dagger, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Dagger of Extra Strike"; } }

        public override int OldSpeed { get { return 90; } }

        public override int InitMinHits { get { return 5; } }
        public override int InitMaxHits { get { return 5; } }

        public override int NumDice { get { return 2; } }
        public override int NumSides { get { return 10; } }
        public override int DiceBonus { get { return 2; } }

        [Constructable]
        public DaggerOfExtraStrike()
        {
            Weight = 3.0;
            Hue = 0x68F;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Speed: {0}\n", OldSpeed );
            builder.AppendFormat( "Damage: {0}d{1}+{2}\n", NumDice, NumSides, DiceBonus );
        }

        #region serialization
        public DaggerOfExtraStrike( Serial serial )
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