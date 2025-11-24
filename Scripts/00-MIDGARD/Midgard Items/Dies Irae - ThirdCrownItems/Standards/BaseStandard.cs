using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseStandard : BaseMeleeWeapon
    {
        public override int DefHitSound { get { return 0x233; } }
        public override int DefMissSound { get { return 0x239; } }

        public override SkillName DefSkill { get { return SkillName.Macing; } }
        public override WeaponType DefType { get { return WeaponType.Staff; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Bash2H; } }

        public BaseStandard( int itemID )
            : base( itemID )
        {
            Weight = 10.0;
        }

        public BaseStandard( int itemID, int hue )
            : this( itemID )
        {
            Hue = hue;
        }

        #region serialization
        public BaseStandard( Serial serial )
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