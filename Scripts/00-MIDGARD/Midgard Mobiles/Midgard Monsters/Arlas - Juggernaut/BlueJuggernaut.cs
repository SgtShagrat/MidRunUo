using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	[CorpseName( "the corpse of a blue juggernaut" )]
	public class BlueJuggernaut : BaseJuggernaut
	{
		[Constructable]
		public BlueJuggernaut() : base( AIType.AI_Melee, FightMode.Closest )
		{
			Name = "a blue juggernaut";

			Hue = Utility.RandomList( 0x880, 0xB0A, 0xA0A, 0x78B );
			Body = 752;
			BaseSoundID = 268;

			if( Core.AOS )
			{
				SetStr( 796, 825 );
				SetDex( 86, 105 );
				SetInt( 436, 475 );

				SetHits( 478, 495 );

				SetDamage( 16, 22 );

				VirtualArmor = 80;

				Karma = -8000;
				Fame = +8000;
			}
			else
			{
				SetStr( 600 );
				SetDex( 120 );
				SetInt( 200 );

				SetHits( 1900 );
				SetMana( 200 );
				SetStam( 120 );

				SetDamage( "5d5" );

				VirtualArmor = 10;

				Karma = Utility.RandomMinMax( -7500, -8000 );
				Fame = Utility.RandomMinMax( 3750, 4000 );
			}

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 0 );
			SetDamageType( ResistanceType.Cold, 100 );
			SetDamageType( ResistanceType.Poison, 0 );
			SetDamageType( ResistanceType.Energy, 0 );

			SetResistance( ResistanceType.Physical, 0, 10 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 90, 100 );
			SetResistance( ResistanceType.Poison, 90, 100 );
			SetResistance( ResistanceType.Energy, 90, 100 );

			SetSkill( SkillName.MagicResist, 200.0, 420.0 );
			SetSkill( SkillName.Tactics, 11.0, 12.0 );
			SetSkill( SkillName.Wrestling, 80.0, 90.0 );

			Tamable = false;
		}

		public override int BreathColdDamage
		{
			get { return 100; }
		}

		public override int BreathFireDamage
		{
			get { return 0; }
		}

		public override int BreathEnergyDamage
		{
			get { return 0; }
		}

		public override int BreathPhysicalDamage
		{
			get { return 0; }
		}

		public override int BreathPoisonDamage
		{
			get { return 0; }
		}

		public override ScaleType ScaleType
		{
			get { return ScaleType.Blue; }
		}

		public override HideType HideType
		{
			get { return HideType.BlueDragon; }
		}

		public override int Hides
		{
			get { return 20; }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 8 );
		}

		public override int CustomWeaponSpeed
		{
			get { return 45; }
		}

		public override bool AlwaysMurderer
		{
			get { return false; }
		}

		#region Serialize-Deserialize
		public BlueJuggernaut( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}