using System;
using Server;
using Server.Engines.Harvest;
using Midgard.Engines.Races;

namespace Server.Items
{
	[RaceAllowanceAttribute( typeof( MountainDwarf ) )]
	public class SturdyPickaxe : BaseAxe, IUsesRemaining
	{
		public override int LabelNumber{ get{ return 1045126; } } // sturdy pickaxe
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 50; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 35; } }
		
		#region Mondain's Legacy
		public override float MlSpeed{ get{ return 3.00f; } }
		#endregion

		public override int OldStrengthReq{ get{ return 25; } }
		public override int OldMinDamage{ get{ return 1; } }
		public override int OldMaxDamage{ get{ return 15; } }
		public override int OldSpeed{ get{ return 35; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

	    #region mod by Dies Irae
	    public override int NumDice { get { return 1; } }
	    public override int NumSides { get { return 15; } }
	    public override int DiceBonus { get { return 0; } }

        public override SkillName OldSkill{ get{ return SkillName.Mining; } }

        public override int BlockCircle { get { return -1; } }

        public override bool CheckForAttackSkillOnSwing { get { return false; } }
	    #endregion

		[Constructable]
		public SturdyPickaxe() : this( 180 * 2 )
		{
		}

		[Constructable]
		public SturdyPickaxe( int uses ) : base( 0xE86 )
		{
			Weight = 11.0;
			Hue = 0x973;
			UsesRemaining = uses;
			ShowUsesRemaining = true;
		}

		public SturdyPickaxe( Serial serial ) : base( serial )
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