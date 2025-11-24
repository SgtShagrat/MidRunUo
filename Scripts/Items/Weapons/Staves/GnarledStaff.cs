using System;

using Midgard.Engines.Races;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13F8, 0x13F9 )]
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class GnarledStaff : BaseStaff
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 17; } }
		public override int AosSpeed{ get{ return 33; } }
		public override float MlSpeed{ get{ return 3.25f; } }

		public override int OldStrengthReq{ get{ return 20; } }
		public override int OldMinDamage{ get{ return 10; } }
		public override int OldMaxDamage{ get{ return 30; } }
		public override int OldSpeed{ get{ return 33; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 50; } }
        public override string OldInitHits{ get{ return "1d20+30"; } } // mod by Dies Irae : pre-aos stuff

	    #region mod by Dies Irae
	    public override int NumDice { get { return 5; } }
	    public override int NumSides { get { return 5; } }
	    public override int DiceBonus { get { return 5; } }

		public override int DefHitSound{ get{ return 0x13C; } }
	    #endregion

		[Constructable]
		public GnarledStaff() : base( 0x13F8 )
		{
			Weight = 3.0;
		}

		public GnarledStaff( Serial serial ) : base( serial )
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