using System;

using Midgard.Engines.Races;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1439, 0x1438 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class WarHammer : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.WhirlwindAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.CrushingBlow; } }

		public override int AosStrengthReq{ get{ return 95; } }
		public override int AosMinDamage{ get{ return 17; } }
		public override int AosMaxDamage{ get{ return 18; } }
		public override int AosSpeed{ get{ return 28; } }
		public override float MlSpeed{ get{ return 3.75f; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 8; } }
		public override int OldMaxDamage{ get{ return 36; } }
		public override int OldSpeed{ get{ return 31; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Bash2H; } }

	    #region mod by Dies Irae
	    public override int NumDice { get { return 7; } }
	    public override int NumSides { get { return 5; } }
	    public override int DiceBonus { get { return 1; } }

		public override int DefHitSound{ get{ return 316; } }
		public override int DefMissSound{ get{ return Utility.RandomList( 562, 563 ); } }
	    #endregion
		
		#region mod by Magius(CHE)
		public override bool ElegibleForMiningBonus {
			get {
				return true; // WarHammer must use mining bonus!
			}
		}
		#endregion

	    [Constructable]
		public WarHammer() : base( 0x1439 )
		{
			Weight = 10.0;
			Layer = Layer.TwoHanded;	
		}

		public WarHammer( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			hitrate = +36.0;
			evasion = -8.0;
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