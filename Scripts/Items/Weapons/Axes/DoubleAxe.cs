using System;

using Midgard.Engines.Races;

using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xf4b, 0xf4c )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class DoubleAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 17; } }
		public override int AosSpeed{ get{ return 33; } }
		public override float MlSpeed{ get{ return 3.25f; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int OldMinDamage{ get{ return 5; } }
		public override int OldMaxDamage{ get{ return 35; } }
		public override int OldSpeed{ get{ return 37; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

	    #region mod by Dies Irae
	    public override int NumDice { get { return 1; } }
	    public override int NumSides { get { return 31; } }
	    public override int DiceBonus { get { return 4; } }

        public override int OldHitSound { get { return 571; } }
        public override int OldMissSound { get { return Utility.RandomList( 569, 570 ); } }
	    #endregion

	    [Constructable]
		public DoubleAxe() : base( 0xF4B )
		{
			Weight = 8.0;
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			base.SetupHitRateAndEvasionBonus (out hitrate, out evasion);
			hitrate += +4.0;
		}
		#endregion

		public DoubleAxe( Serial serial ) : base( serial )
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