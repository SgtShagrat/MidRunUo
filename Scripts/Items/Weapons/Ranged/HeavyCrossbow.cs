using System;

using Midgard.Engines.Races;
using Midgard.Items;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13FD, 0x13FC )]
	[RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class HeavyCrossbow : BaseRanged
	{
		public override int EffectID{ get{ return 0x1BFE; } }
		public override Type AmmoType{ get{ return typeof( Bolt ); } }
		// public override Item Ammo{ get{ return new Bolt(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.MovingShot; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq{ get{ return 80; } }
		public override int AosMinDamage{ get{ return Core.ML ? 20 : 19; } }
		public override int AosMaxDamage{ get{ return Core.ML ? 24 : 20; } }
		public override int AosSpeed{ get{ return 22; } }
		public override float MlSpeed{ get{ return 5.00f; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 11; } }
		public override int OldMaxDamage{ get{ return 56; } }
		public override int OldSpeed{ get{ return 10; } }

		public override int DefMinRange{ get{ return 3 + OldMaterialMinRangeBonus; } }//mod by Arlas
		public override int DefMaxRange{ get{ return 8 + OldMaterialMaxRangeBonus; } } // mod by Dies Irae

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 100; } }
		public override string OldInitHits{ get{ return "1d70+30"; } } // mod by Dies Irae : pre-aos stuff

		#region mod by Dies Irae
		public override int NumDice { get { return 5; } }
		public override int NumSides { get { return 10; } }
		public override int DiceBonus { get { return 6; } }

		public override int ComputeDamage( Mobile attacker, Mobile defender )
		{
			int damage = base.ComputeDamage( attacker, defender );

			if( defender.Player )
				damage = (int)( damage * 1.05 );

			return damage;
		}

        protected override void SetupHitRateAndEvasionBonus( out double hitrate, out double evasion )
        {
            hitrate = +40.0;
            evasion = -15.0;
        }
		#endregion

		[Constructable]
		public HeavyCrossbow() : base( 0x13FD )
		{
			Weight = 9.0;
			Layer = Layer.TwoHanded;
		}

		public HeavyCrossbow( Serial serial ) : base( serial )
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
		}
	}
}