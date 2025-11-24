using Server;

namespace Midgard.Items
{
	public abstract class BasePoisonedArrow : BasePoisonAmmonition
	{
		public BasePoisonedArrow() : base( 0xF3F )
		{
		}

		public override bool StackWith( Mobile from, Item dropped, bool playSound )
		{
			BasePoisonedArrow arrow = dropped as BasePoisonedArrow;

			return arrow != null && ( arrow.Poison == Poison && arrow.PoisonerSkill == PoisonerSkill && base.StackWith( from, dropped, playSound ) );
		}

		public static BasePoisonedArrow GetArrowByPoison( Poison poison, int amount, double skill )
		{
			BasePoisonedArrow arrow = null;

			switch ( poison.Level )
			{
				case 0: arrow = new LesserPoisonedArrow( amount, poison ); break;
				case 1: arrow = new NormalPoisonedArrow( amount, poison ); break;
				case 2:	arrow = new GreaterPoisonedArrow( amount, poison ); break;
				case 3: arrow = new DeadlyPoisonedArrow( amount, poison ); break;
				case 4: arrow = new LethalPoisonedArrow( amount, poison ); break;

				case 19: arrow = new MagiaLesserPoisonedArrow( amount, poison ); break;
				case 20: arrow = new MagiaNormalPoisonedArrow( amount, poison ); break;
				case 21: arrow = new MagiaGreaterPoisonedArrow( amount, poison ); break;
				case 22: arrow = new MagiaDeadlyPoisonedArrow( amount, poison ); break;
				case 23: arrow = new MagiaLethalPoisonedArrow( amount, poison ); break;

				case 24: arrow = new StanchezzaLesserPoisonedArrow( amount, poison ); break;
				case 25: arrow = new StanchezzaNormalPoisonedArrow( amount, poison ); break;
				case 26: arrow = new StanchezzaGreaterPoisonedArrow( amount, poison ); break;
				case 27: arrow = new StanchezzaDeadlyPoisonedArrow( amount, poison ); break;
				case 28: arrow = new StanchezzaLethalPoisonedArrow( amount, poison ); break;

				case 29: arrow = new ParalisiLesserPoisonedArrow( amount, poison ); break;
				case 30: arrow = new ParalisiNormalPoisonedArrow( amount, poison ); break;
				case 31: arrow = new ParalisiGreaterPoisonedArrow( amount, poison ); break;
				case 32: arrow = new ParalisiDeadlyPoisonedArrow( amount, poison ); break;
				case 33: arrow = new ParalisiLethalPoisonedArrow( amount, poison ); break;

				case 34: arrow = new BloccoLesserPoisonedArrow( amount, poison ); break;
				case 35: arrow = new BloccoNormalPoisonedArrow( amount, poison ); break;
				case 36: arrow = new BloccoGreaterPoisonedArrow( amount, poison ); break;
				case 37: arrow = new BloccoDeadlyPoisonedArrow( amount, poison ); break;
				case 38: arrow = new BloccoLethalPoisonedArrow( amount, poison ); break;

				case 39: arrow = new LentezzaLesserPoisonedArrow( amount, poison ); break;
				case 40: arrow = new LentezzaNormalPoisonedArrow( amount, poison ); break;
				case 41: arrow = new LentezzaGreaterPoisonedArrow( amount, poison ); break;
				case 42: arrow = new LentezzaDeadlyPoisonedArrow( amount, poison ); break;
				case 43: arrow = new LentezzaLethalPoisonedArrow( amount, poison ); break;
			}

			if( arrow != null )
				arrow.PoisonerSkill = (int)skill;

			return arrow;
		}

		#region serialization
		public BasePoisonedArrow( Serial serial ) : base( serial )
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