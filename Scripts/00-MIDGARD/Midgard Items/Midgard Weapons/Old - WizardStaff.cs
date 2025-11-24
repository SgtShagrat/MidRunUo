namespace Server.Items
{
    public class WizardStaff : BaseMeleeWeapon
    {
        public override int DefHitSound { get { return 0x233; } }
        public override int DefMissSound { get { return 0x239; } }
        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.Disarm; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.ParalyzingBlow; } }
        public override SkillName DefSkill { get { return SkillName.Wrestling; } }
        public override WeaponType DefType { get { return WeaponType.Fists; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Bash2H; } }

        public override int AosStrengthReq { get { return 35; } }
        public override int AosMinDamage { get { return 1; } }
        public override int AosMaxDamage { get { return 5; } }
        public override int AosSpeed { get { return 30; } }

        public override int OldStrengthReq { get { return 35; } }
        public override int OldMinDamage { get { return 1; } }
        public override int OldMaxDamage { get { return 5; } }
        public override int OldSpeed { get { return 30; } }

        [Constructable]
        public WizardStaff()
            : base( 709 )
        {
            Name = "a Wizard Staff";
            Weight = 6.0;
        }

        public WizardStaff( Serial serial )
            : base( serial )
        {
        }

        public override bool AllowEquipedCast( Mobile from )
        {
            return true;
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
    }
}