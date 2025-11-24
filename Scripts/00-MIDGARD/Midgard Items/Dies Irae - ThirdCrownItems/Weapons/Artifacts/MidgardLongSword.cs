using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class MidgardLongSword : Longsword, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Enchanted Sword Of Midgard"; } }

        public override int OldSpeed { get { return 65; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 30; } }

        public override int NumDice { get { return 3; } }
        public override int NumSides { get { return 5; } }
        public override int DiceBonus { get { return 30; } }

        [Constructable]
        public MidgardLongSword()
        {
            Weight = 10.0;
            Hue = 0x48D;
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Strenght required: {0}\n", OldStrengthReq );
            builder.AppendFormat( "Speed: {0}\n", OldSpeed );
            builder.AppendFormat( "Damage: {0}d{1}+{2}\n", NumDice, NumSides, DiceBonus );
        }

        #region serialization
        public MidgardLongSword( Serial serial )
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