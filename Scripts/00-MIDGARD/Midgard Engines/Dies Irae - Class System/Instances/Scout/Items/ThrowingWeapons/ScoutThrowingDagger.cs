/***************************************************************************
 *                               ScoutThrowingDagger.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable( 0xF52, 0xF51 )]
    public class ScoutThrowingDagger : BaseScoutThrowingWeapon
    {
        public override string DefaultName { get { return "a throwing dagger"; } }

        public override int OldStrengthReq { get { return 1; } }

        public override int OldMinDamage { get { return 3; } }

        public override int OldMaxDamage { get { return 15; } }

        public override int OldSpeed { get { return 55; } }

        public override int InitMinHits { get { return 31; } }

        public override int InitMaxHits { get { return 50; } }

        public override SkillName DefSkill { get { return SkillName.Fencing; } }

        public override WeaponType DefType { get { return WeaponType.Piercing; } }

        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Pierce1H; } }

        public override int NumDice { get { return 3; } }

        public override int NumSides { get { return 5; } }

        public override int DiceBonus { get { return 0; } }

        [Constructable]
        public ScoutThrowingDagger()
            : base( 0xF52 )
        {
            Weight = 1.0;
            Layer = Layer.OneHanded;
        }

        #region serialization
        public ScoutThrowingDagger( Serial serial )
            : base( serial )
        {
        }

        public override int AnimID { get { return 0x3b73; } }

        public override int AnimHue { get { return 0x481; } }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}