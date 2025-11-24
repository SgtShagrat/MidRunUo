using System;

using Midgard.Engines.Races;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x143E, 0x143F )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class Halberd : BasePoleArm
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq{ get{ return 95; } }
		public override int AosMinDamage{ get{ return 18; } }
		public override int AosMaxDamage{ get{ return 19; } }
		public override int AosSpeed{ get{ return 25; } }
		public override float MlSpeed{ get{ return 4.25f; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int OldMinDamage{ get{ return 5; } }
		public override int OldMaxDamage{ get{ return 49; } }
		public override int OldSpeed{ get{ return 25; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }
        public override string OldInitHits{ get{ return "1d50+30"; } } // mod by Dies Irae : pre-aos stuff

 		public override int NumDice { get { return 2; } }
 		public override int NumSides { get { return 23; } }
 		public override int DiceBonus { get { return 3; } }
        public override int OldHitSound { get { return 566; } }
        public override int OldMissSound { get { return 562; } }

		#region mod by Magius(CHE)
		public override bool ElegibleForLumberBonus {
			get {
				return true; //force Lumber bonus by magius(che)
			}
		}
		#endregion
		
		[Constructable]
		public Halberd() : base( 0x143E )
		{
			Weight = 16.0;
		}

		public Halberd( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			base.SetupHitRateAndEvasionBonus (out hitrate, out evasion);
			hitrate += +10.0;
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