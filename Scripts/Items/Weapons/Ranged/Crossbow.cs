using System;

using Midgard.Engines.Races;
using Midgard.Items;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF50, 0xF4F )]
	[RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class Crossbow : BaseRanged
	{
		public override int EffectID{ get{ return 0x1BFE; } }
		public override Type AmmoType{ get{ return typeof( Bolt ); } }
		// public override Item Ammo{ get{ return new Bolt(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq{ get{ return 35; } }
		public override int AosMinDamage{ get{ return 18; } }
		public override int AosMaxDamage{ get{ return Core.ML ? 22 : 20; } }
		public override int AosSpeed{ get{ return 24; } }
		public override float MlSpeed{ get{ return 4.50f; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int OldMinDamage{ get{ return 8; } }
		public override int OldMaxDamage{ get{ return 43; } }
		public override int OldSpeed{ get{ return 26; } } // 18 // mod by Dies Irae

		public override int DefMinRange{ get { return 2 + OldMaterialMinRangeBonus; } }//mod by Arlas
		public override int DefMaxRange{ get{ return 8 + OldMaterialMaxRangeBonus; } } // mod by Dies Irae

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		#region mod by Dies Irae
		public override int NumDice { get { return 5; } }
		public override int NumSides { get { return 8; } }
		public override int DiceBonus { get { return 3; } }

        protected override void SetupHitRateAndEvasionBonus( out double hitrate, out double evasion )
        {
            hitrate = +45.0;
            evasion = -15.0;
        }
		#endregion

		[Constructable]
		public Crossbow() : base( 0xF50 )
		{
			Weight = 7.0;
			Layer = Layer.TwoHanded;
		}

		public Crossbow( Serial serial ) : base( serial )
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