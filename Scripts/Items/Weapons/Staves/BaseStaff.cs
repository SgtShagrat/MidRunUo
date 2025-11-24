using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public abstract class BaseStaff : BaseMeleeWeapon
	{
		public override int DefHitSound{ get{ return 0x233; } }
		public override int DefMissSound{ get{ return 0x239; } }

		public override SkillName DefSkill{ get{ return SkillName.Macing; } }
		public override WeaponType DefType{ get{ return WeaponType.Staff; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash2H; } }

		public BaseStaff( int itemID ) : base( itemID )
		{
		}

		public BaseStaff( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{			
			hitrate = +8.0;
			evasion = -4.0;
		}
		#endregion
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

        #region mod by Dies Irae
        /*
		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

			defender.Stam -= Utility.Random( 3, 3 ); // 3-5 points of stamina loss
		}*/

        public override bool StaminaLossOnHit { get { return true; } }
        #endregion
	}
}