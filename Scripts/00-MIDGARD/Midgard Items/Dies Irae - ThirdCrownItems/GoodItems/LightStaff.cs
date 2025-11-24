using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x335D Light Staff - ( craftabile solo da karma positivo, in legno, due mani, 
    /// non subisce la colorazione del legno rimanendo sempre con hue 0 )
    /// </summary>
    public class LightStaff : BlackStaff
    {
        public override string DefaultName { get { return "light staff"; } }

        public override bool CannotBeHuedOnCraft { get { return true; } }

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

        public override int DefHitSound { get { return 0x13C; } }

        public override int BlockCircle { get { return -1; } }

        public override bool StaminaLossOnHit { get { return false; } }

        [Constructable]
        public LightStaff()
        {
            ItemID = 0x335D;
			Layer = Layer.OneHanded;

            Attributes.SpellChanneling = 1;
            Weight = 8.0;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.Karma > 0;
        }

        public override bool AllowEquipedCast( Mobile from )
        {
            return true;
        }

        #region serialization
        public LightStaff( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
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