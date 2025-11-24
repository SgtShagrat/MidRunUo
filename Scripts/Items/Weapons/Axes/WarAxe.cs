using System;

using Midgard.Engines.Races;

using Server.Items;
using Server.Network;
using Server.Engines.Harvest;

namespace Server.Items
{
	[FlipableAttribute( 0x13B0, 0x13AF )]
	[RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class WarAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int AosStrengthReq{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 14; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 33; } }
		public override float MlSpeed{ get{ return 3.25f; } }

		public override int OldStrengthReq{ get{ return 35; } }
		public override int OldMinDamage{ get{ return 9; } }
		public override int OldMaxDamage{ get{ return 27; } }
		public override int OldSpeed{ get{ return 40; } }

		public override int DefHitSound{ get{ return 0x233; } }
		public override int DefMissSound{ get{ return 0x239; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		public override SkillName DefSkill{ get{ return SkillName.Macing; } }
		public override WeaponType DefType{ get{ return WeaponType.Bashing; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash1H; } }

		public override HarvestSystem HarvestSystem{ get{ return null; } }

		#region mod by Dies Irae + Arlas
		public override int NumDice { get { return 6; } }
		public override int NumSides { get { return 4; } }
		public override int DiceBonus { get { return 3; } }
		public override int OldHitSound { get { return Utility.RandomList( 571, 572 ); } }
		public override int OldMissSound { get { return Utility.RandomList( 568, 570 ); } }

        /*
		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			base.OnHit( attacker, defender, damageBonus );

            defender.Stam -= Utility.Dice( 1, 3, 2 ); // 3-5 points of stamina loss
		}
        */

		#region mod by Arlas
		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			if( defender.Stam < (int)( defender.StamMax * 0.1 ) )
			{
				damageBonus *= 1.1;
				attacker.SendMessage( "Due to lack of stamina your opponent suffered an extra damage." );
				if( attacker.PlayerDebug )
					attacker.SendMessage( "Low stamina bonus: 10%. Damage bonus is {0:F2}", damageBonus );
			}

			base.OnHit( attacker, defender, damageBonus );
		}
		#endregion

		public override bool StaminaLossOnHit { get { return true; } }

		public override bool ElegibleForLumberBonus { get { return false; } }
		#endregion

		[Constructable]
		public WarAxe() : base( 0x13B0 )
		{
			Weight = 8.0;			
		}

		public WarAxe( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			hitrate = +25.0;
			evasion = -0.0;
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
	}
}