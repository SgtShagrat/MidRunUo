using Server;
using Server.Items;

namespace Midgard.Items
{
    [FlipableAttribute( 0x13F8, 0x13F9 )]
    public class MageStaff : BaseStaff
    {
        public override string DefaultName { get { return "mage staff"; } }

        public override SkillName DefSkill { get { return SkillName.Wrestling; } }
        public override WeaponType DefType { get { return WeaponType.Fists; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

        public override int OldStrengthReq { get { return 25; } }
        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 50; } }

        public override int NumDice { get { return 1; } }
        public override int NumSides { get { return 15; } }
        public override int DiceBonus { get { return 0; } }

		public override int DefHitSound{ get{ return 0x13C; } }

        public override int BlockCircle{ get{ return -1; } }

        public override bool StaminaLossOnHit { get { return false; } }

        [Constructable]
        public MageStaff()
            : base( 0x13F8 )
        {
            Attributes.SpellChanneling = 1;
			Layer = Layer.OneHanded;
            Weight = 8.0;
        }

		public override bool AllowEquipedCast( Mobile from )
		{
		    return true;
		}

        #region serialization
        public MageStaff( Serial serial )
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

            if( Layer != Layer.OneHanded )
                Layer = Layer.OneHanded;
        }
        #endregion
    }
}