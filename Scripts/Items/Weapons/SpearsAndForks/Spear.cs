using System;

using Midgard.Engines.Races;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xF62, 0xF63 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class Spear : BaseSpear
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 42; } }
		public override float MlSpeed{ get{ return 2.75f; } }

		public override int OldStrengthReq{ get{ return 30; } }
		public override int OldMinDamage{ get{ return 2; } }
		public override int OldMaxDamage{ get{ return 36; } }
		public override int OldSpeed{ get{ return 46; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

	    #region mod by Dies Irae
	    public override int NumDice { get { return 2; } }
	    public override int NumSides { get { return 18; } }
	    public override int DiceBonus { get { return 0; } }

        public override int OldHitSound { get { return 572; } }
        public override int OldMissSound { get { return 569; } }
	    #endregion

	    [Constructable]
		public Spear() : base( 0xF62 )
		{
			Weight = 7.0;
		}

		public Spear( Serial serial ) : base( serial )
		{
		}
		
		#region Mod by Magius(CHE)
		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			base.SetupHitRateAndEvasionBonus (out hitrate, out evasion);
			hitrate = 5.0;
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