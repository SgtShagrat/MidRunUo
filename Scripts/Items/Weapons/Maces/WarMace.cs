using System;

using Midgard.Engines.Races;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1407, 0x1406 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class WarMace : BaseBashing
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.BleedAttack; } }

		public override int AosStrengthReq{ get{ return 80; } }
		public override int AosMinDamage{ get{ return 16; } }
		public override int AosMaxDamage{ get{ return 17; } }
		public override int AosSpeed{ get{ return 26; } }
		public override float MlSpeed{ get{ return 4.00f; } }
        
		public override int OldStrengthReq{ get{ return 30; } }
		public override int OldMinDamage{ get{ return 10; } }
		public override int OldMaxDamage{ get{ return 30; } }
		public override int OldSpeed{ get{ return 32; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 110; } }

	    #region mod by Dies Irae
	    public override int NumDice { get { return 5; } }
	    public override int NumSides { get { return 5; } }
	    public override int DiceBonus { get { return 5; } }

		public override int DefHitSound{ get{ return 0x13C; } }
	    #endregion

	    [Constructable]
		public WarMace() : base( 0x1407 )
		{
			Weight = 17.0;
		}

		public WarMace( Serial serial ) : base( serial )
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