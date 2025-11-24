using System.Text;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public interface IPoisonWeapon
    {
        int SpecialPoisonLevel { get; }
    }

    public class GnarledStaffOfPoison : GnarledStaff, IPoisonWeapon, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Gnarled Staff Of Poison"; } }

        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 70; } }
        public override int InitMaxHits { get { return 70; } }

        public override int NumDice { get { return 5; } }
        public override int NumSides { get { return 5; } }
        public override int DiceBonus { get { return 5; } }

        #region IPoisonWeapon members

        public int SpecialPoisonLevel { get { return 3; } }

        #endregion

        [Constructable]
        public GnarledStaffOfPoison()
        {
            Weight = 7.0;
            Hue = 0x431;
        }

        public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
        {
            base.OnHit( attacker, defender, damageBonus );

            if( Utility.RandomDouble() >= 0.5 ) // 50% chance to poison
                defender.ApplyPoison( attacker, Poison.GetPoison( SpecialPoisonLevel ) );
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Strenght required: {0}\n", OldStrengthReq );
            builder.AppendFormat( "Speed: {0}\n", OldSpeed );
            builder.AppendFormat( "Damage: {0}d{1}+{2}\n", NumDice, NumSides, DiceBonus );
            builder.AppendFormat( "Special: 50% of enemy poison\n" );
        }

        #region serialization
        public GnarledStaffOfPoison( Serial serial )
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

    public class ClubOfPoison : Club, IPoisonWeapon, ITreasureOfMidgard
    {
        public override string DefaultName { get { return "Club of Poison"; } }

        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 30; } }
        public override int InitMaxHits { get { return 30; } }

        public override int NumDice { get { return 4; } }
        public override int NumSides { get { return 5; } }
        public override int DiceBonus { get { return 4; } }

        #region IPoisonWeapon members

        public int SpecialPoisonLevel { get { return 3; } }

        #endregion

        [Constructable]
        public ClubOfPoison()
        {
            Weight = 14.0;
            Hue = 0x431;
        }

        public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
        {
            base.OnHit( attacker, defender, damageBonus );

            if( Utility.RandomDouble() >= 0.5 ) // 50% chance to poison
                defender.ApplyPoison( attacker, Poison.GetPoison( SpecialPoisonLevel ) );
        }

        public void Doc( StringBuilder builder )
        {
            builder.AppendFormat( "Strenght required: {0}\n", OldStrengthReq );
            builder.AppendFormat( "Speed: {0}\n", OldSpeed );
            builder.AppendFormat( "Damage: {0}d{1}+{2}\n", NumDice, NumSides, DiceBonus );
            builder.AppendFormat( "Special: 50% of enemy poison\n" );
        }

        #region serialization
        public ClubOfPoison( Serial serial )
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
