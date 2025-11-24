using System;

using Midgard.Engines.Races;
using Midgard.Items;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13B2, 0x13B1 )]
	[RaceAllowance( typeof( MountainDwarf ) )]
	public class Bow : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		//public override Item Ammo{ get{ return new Arrow(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq{ get{ return 30; } }
		public override int AosMinDamage{ get{ return Core.ML ? 15 : 16; } }
		public override int AosMaxDamage{ get{ return Core.ML ? 19 : 18; } }
		public override int AosSpeed{ get{ return 25; } }
		public override float MlSpeed{ get{ return 4.25f; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int OldMinDamage{ get{ return 9; } }
		public override int OldMaxDamage{ get{ return 41; } }
		public override int OldSpeed { get { return 30; } } //  20; } } // mod by Dies Irae

		public override int DefMinRange{ get { return 2 + OldMaterialMinRangeBonus; } } //mod by Arlas
		public override int DefMaxRange{ get{ return 10 + OldMaterialMaxRangeBonus; } } // mod by Dies Irae

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		#region mod by Dies Irae
		public override int NumDice { get { return 4; } }
		public override int NumSides { get { return 9; } }
		public override int DiceBonus { get { return 5; } }

		protected override void SetupHitRateAndEvasionBonus( out double hitrate, out double evasion )
		{
			hitrate = +30.0;
			evasion = -15.0;
		}
		#endregion

		[Constructable]
		public Bow() : base( 0x13B2 )
		{
			Weight = 6.0;
			Layer = Layer.TwoHanded;
		}

		public Bow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 7.0 )
				Weight = 6.0;
		}
	}
}