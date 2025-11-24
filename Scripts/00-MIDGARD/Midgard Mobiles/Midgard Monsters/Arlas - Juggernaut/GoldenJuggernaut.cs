using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	[CorpseName( "the corpse of a golden juggernaut" )]
	public class GoldenJuggernaut : BaseJuggernaut
	{
		[Constructable]
		public GoldenJuggernaut() : base( AIType.AI_Melee, FightMode.Evil )
		{
			Name = "a golden juggernaut";

			Hue = Utility.RandomList( 0x8F7, 0x8F1, 0x8EE, 0x88C );
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
				SetInt( 1040 );

				SetHits( 10000 );
				SetMana( 1040 );
				SetStam( 175 );

				SetDamage( "5d5" );

				VirtualArmor = 10;

				Karma = 15000;
				Fame = 15000;
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

			SetSkill( SkillName.MagicResist, 200.0, 420.0 );
			SetSkill( SkillName.Tactics, 11.0, 12.0 );
			SetSkill( SkillName.Wrestling, 90.0, 100.0 );

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
			get { return ScaleType.Yellow; }
		}

		public override HideType HideType
		{
			get
			{
				return (HideType)Utility.RandomList( (int)HideType.RedDragon, (int)HideType.GreenDragon,
					(int)HideType.BlueDragon, (int)HideType.BlackDragon );
			}
		}

		public override int Hides
		{
			get { return 40; }
		}

		public override bool AlwaysMurderer
		{
			get { return false; }
		}

		public override bool InitialInnocent
		{
			get { return true; }
		}

		public override int CustomWeaponSpeed
		{
			get { return 50; }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.Gems, 8 );
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if( Utility.RandomDouble() < 0.2 && attacker is BaseCreature )
			{
				BaseCreature c = (BaseCreature)attacker;

				if( c.Controlled && c.ControlMaster != null )
				{
					c.ControlTarget = c.ControlMaster;
					c.ControlOrder = OrderType.Attack;
					c.Combatant = c.ControlMaster;
				}
			}
		}

		#region Serialize-Deserialize
		public GoldenJuggernaut( Serial serial ) : base( serial )
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