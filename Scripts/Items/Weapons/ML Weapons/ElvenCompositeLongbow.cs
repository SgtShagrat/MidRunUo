using System;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D1E, 0x2D2A )]
	public class ElvenCompositeLongbow : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ForceArrow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.SerpentArrow; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return 12; } }
		public override int AosMaxDamage{ get{ return 16; } }
		public override int AosSpeed{ get{ return 27; } }
		public override float MlSpeed{ get{ return 4.00f; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int OldMinDamage{ get{ return 12; } }
		public override int OldMaxDamage{ get{ return 16; } }
		public override int OldSpeed{ get{ return 27; } }

		public override int DefMinRange{ get{ return 2 + OldMaterialMinRangeBonus; } }//mod by Arlas
		public override int DefMaxRange{ get{ return 10 + OldMaterialMaxRangeBonus; } }//mod by Arlas

		public override int InitMinHits{ get{ return 41; } }
		public override int InitMaxHits{ get{ return 90; } }

		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		#region mod by Dies Irae
		public override int NumDice { get { return 4; } }
		public override int NumSides { get { return 9; } }
		public override int DiceBonus { get { return 5; } }

		//public override Race RequiredRace { get { return Race.Elf; } }

		protected override void SetupHitRateAndEvasionBonus (out double hitrate, out double evasion)
		{
			hitrate = +43.0;
			evasion = -15.0;
		}
		#endregion
		
		[Constructable]
		public ElvenCompositeLongbow() : base( 0x2D1E )
		{
			Weight = 8.0;
		}

		public ElvenCompositeLongbow( Serial serial ) : base( serial )
		{
		}

		#region mod by Dies Irae
		public override bool CanBeCraftedBy( Mobile from )
		{
			return from.AccessLevel > AccessLevel.Counselor || Midgard.Engines.Races.Core.IsElfRace( from.Race );
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}