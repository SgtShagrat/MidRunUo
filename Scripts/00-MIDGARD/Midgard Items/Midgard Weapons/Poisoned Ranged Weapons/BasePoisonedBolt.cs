using Server;

namespace Midgard.Items
{
	public abstract class BasePoisonedBolt : BasePoisonAmmonition
	{
		public BasePoisonedBolt() : base( 0x1BFB )
		{
		}

		public override bool StackWith( Mobile from, Item dropped, bool playSound )
		{
			BasePoisonedBolt bolt = dropped as BasePoisonedBolt;

			return bolt != null && ( bolt.Poison == Poison && bolt.PoisonerSkill == PoisonerSkill && base.StackWith( from, dropped, playSound ) );
		}

		public static BasePoisonedBolt GetBoltByPoison( Poison poison, int amount, double skill )
		{
			BasePoisonedBolt bolt = null;


			switch ( poison.Level )
			{
				case 0: bolt = new LesserPoisonedBolt( amount, poison ); break;
				case 1: bolt = new NormalPoisonedBolt( amount, poison ); break;
				case 2: bolt = new GreaterPoisonedBolt( amount, poison ); break;
				case 3: bolt = new DeadlyPoisonedBolt( amount, poison ); break;
				case 4: bolt = new LethalPoisonedBolt( amount, poison ); break;

				case 19: bolt = new MagiaLesserPoisonedBolt( amount, poison ); break;
				case 20: bolt = new MagiaNormalPoisonedBolt( amount, poison ); break;
				case 21: bolt = new MagiaGreaterPoisonedBolt( amount, poison ); break;
				case 22: bolt = new MagiaDeadlyPoisonedBolt( amount, poison ); break;
				case 23: bolt = new MagiaLethalPoisonedBolt( amount, poison ); break;

				case 24: bolt = new StanchezzaLesserPoisonedBolt( amount, poison ); break;
				case 25: bolt = new StanchezzaNormalPoisonedBolt( amount, poison ); break;
				case 26: bolt = new StanchezzaGreaterPoisonedBolt( amount, poison ); break;
				case 27: bolt = new StanchezzaDeadlyPoisonedBolt( amount, poison ); break;
				case 28: bolt = new StanchezzaLethalPoisonedBolt( amount, poison ); break;

				case 29: bolt = new ParalisiLesserPoisonedBolt( amount, poison ); break;
				case 30: bolt = new ParalisiNormalPoisonedBolt( amount, poison ); break;
				case 31: bolt = new ParalisiGreaterPoisonedBolt( amount, poison ); break;
				case 32: bolt = new ParalisiDeadlyPoisonedBolt( amount, poison ); break;
				case 33: bolt = new ParalisiLethalPoisonedBolt( amount, poison ); break;

				case 34: bolt = new BloccoLesserPoisonedBolt( amount, poison ); break;
				case 35: bolt = new BloccoNormalPoisonedBolt( amount, poison ); break;
				case 36: bolt = new BloccoGreaterPoisonedBolt( amount, poison ); break;
				case 37: bolt = new BloccoDeadlyPoisonedBolt( amount, poison ); break;
				case 38: bolt = new BloccoLethalPoisonedBolt( amount, poison ); break;

				case 39: bolt = new LentezzaLesserPoisonedBolt( amount, poison ); break;
				case 40: bolt = new LentezzaNormalPoisonedBolt( amount, poison ); break;
				case 41: bolt = new LentezzaGreaterPoisonedBolt( amount, poison ); break;
				case 42: bolt = new LentezzaDeadlyPoisonedBolt( amount, poison ); break;
				case 43: bolt = new LentezzaLethalPoisonedBolt( amount, poison ); break;
			}

			if( bolt != null )
				bolt.PoisonerSkill = (int)skill;

			return bolt;
		}

		#region serialization
		public BasePoisonedBolt( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	}
}