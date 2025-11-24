using System;

using Midgard.Engines.Races;

using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D2B, 0x2D1F )]
	public class MagicalShortbow : BaseRanged
	{
		public override int EffectID{ get{ return 0xF42; } }
		public override Type AmmoType{ get{ return typeof( Arrow ); } }
		public override Item Ammo{ get{ return new Arrow(); } }

		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.LightningArrow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.PsychicAttack; } }

		public override int AosStrengthReq{ get{ return 45; } }
		public override int AosMinDamage{ get{ return 9; } }
		public override int AosMaxDamage{ get{ return 13; } }
		public override int AosSpeed{ get{ return 38; } }
		public override float MlSpeed{ get{ return 3.00f; } }

		public override int OldStrengthReq{ get{ return 45; } }
		public override int OldMinDamage{ get{ return 9; } }
		public override int OldMaxDamage{ get{ return 13; } }
		public override int OldSpeed{ get{ return 30; } } // mod by Dies irae

		public override int DefMinRange{ get{ return 2 + OldMaterialMinRangeBonus; } }//mod by Arlas
		public override int DefMaxRange{ get{ return 10 + OldMaterialMaxRangeBonus; } }

		public override int InitMinHits{ get{ return 41; } }
		public override int InitMaxHits{ get{ return 90; } }

		#region mod by Dies Irae
		public override int NumDice { get { return 4; } }
		public override int NumSides { get { return 9; } }
		public override int DiceBonus { get { return 5; } }
		#endregion

		[Constructable]
		public MagicalShortbow() : base( 0x2D2B )
		{
			Weight = 6.0;
		}

		#region mod by Dies Irae
		//public override Race RequiredRace { get { return Race.Elf; } }

		public override bool CanBeCraftedBy( Mobile from )
		{
			return from.AccessLevel > AccessLevel.Counselor || Midgard.Engines.Races.Core.IsElfRace( from.Race );
		}
		#endregion

		public MagicalShortbow( Serial serial ) : base( serial )
		{
		}

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