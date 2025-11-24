using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	[CorpseName( "the corpse of a red juggernaut" )]
	public class RedJuggernaut : BaseJuggernaut
	{
		[Constructable]
		public RedJuggernaut() : base( AIType.AI_Melee, FightMode.Closest )
		{
			Name = "a red juggernaut";

			Hue = Utility.RandomList( 0x991, 0x993, 0x78D, 0xA73 );
			Body = 752;
			BaseSoundID = 268;

			if( Core.AOS )
			{
				SetStr( 796, 825 );
				SetDex( 86, 105 );
				SetInt( 436, 475 );

				SetHits( 478, 495 );

				SetDamage( 16, 22 );

				VirtualArmor = 90;
			}
			else
			{
				SetStr( 1500 );
				SetDex( 175 );
				SetInt( 500 );

				SetHits( 10000 );
				SetMana( 500 );
				SetStam( 175 );

				SetDamage( "5d5" );

				VirtualArmor = 10;

				Karma = -15000;
				Fame = +15000;
			}

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Fire, 100 );
			SetDamageType( ResistanceType.Cold, 0 );
			SetDamageType( ResistanceType.Poison, 0 );
			SetDamageType( ResistanceType.Energy, 0 );

			SetResistance( ResistanceType.Physical, 9, 10 );
			SetResistance( ResistanceType.Fire, 90, 100 );
			SetResistance( ResistanceType.Cold, 70, 100 );
			SetResistance( ResistanceType.Poison, 90, 100 );
			SetResistance( ResistanceType.Energy, 90, 100 );

			SetSkill( SkillName.MagicResist, 200.0, 400.0 );
			SetSkill( SkillName.Tactics, 11.0, 12.0 );
			SetSkill( SkillName.Wrestling, 80.0, 90.0 );

			Tamable = false;
		}

		public override int BreathColdDamage
		{
			get { return 0; }
		}

		public override int BreathFireDamage
		{
			get { return 100; }
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
			get { return ScaleType.Red; }
		}

		public override HideType HideType
		{
			get { return HideType.RedDragon; }
		}

		public override int Hides
		{
			get { return 20; }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.Gems, 8 );
		}

		public override int CustomWeaponSpeed
		{
			get { return 50; }
		}

		#region Serialize-Deserialize
		public RedJuggernaut( Serial serial ) : base( serial )
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