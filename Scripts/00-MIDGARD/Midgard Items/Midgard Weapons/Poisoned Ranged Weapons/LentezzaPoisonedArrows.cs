using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class LentezzaLesserPoisonedArrow : BasePoisonedArrow, ICommodity
	{
		//public override Poison Poison{ get{ return Poison.Lesser; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} arrow [Lesser Poisoned]" : "{0} arrows [Lesser Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public LentezzaLesserPoisonedArrow() : this( 1 )
		{
			Poison = Poison.LentezzaLesser;
		}

		[Constructable]
		public LentezzaLesserPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.LentezzaLesser;
		}

		[Constructable]
		public LentezzaLesserPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LentezzaLesserPoisonedArrow( Serial serial ) : base( serial )
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
			if (Poison == null)
				Poison = Poison.LentezzaLesser;
		}
		#endregion
	}

	public class LentezzaNormalPoisonedArrow : BasePoisonedArrow, ICommodity
	{
		//public override Poison Poison{ get{ return Poison.Regular; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} arrow [Poisoned]" : "{0} arrows [Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public LentezzaNormalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.LentezzaRegular;
		}

		[Constructable]
		public LentezzaNormalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.LentezzaRegular;
		}

		[Constructable]
		public LentezzaNormalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LentezzaNormalPoisonedArrow( Serial serial ) : base( serial )
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
			if (Poison == null)
				Poison = Poison.LentezzaRegular;
		}
		#endregion
	}

	public class LentezzaGreaterPoisonedArrow : BasePoisonedArrow, ICommodity
	{
		//public override Poison Poison{ get{ return Poison.Greater; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} arrow [Greater Poisoned]" : "{0} arrows [Greater Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public LentezzaGreaterPoisonedArrow() : this( 1 )
		{
			Poison = Poison.LentezzaGreater;
		}

		[Constructable]
		public LentezzaGreaterPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.LentezzaGreater;
		}

		[Constructable]
		public LentezzaGreaterPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LentezzaGreaterPoisonedArrow( Serial serial ) : base( serial )
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
			if (Poison == null)
				Poison = Poison.LentezzaGreater;
		}
		#endregion
	}

	public class LentezzaDeadlyPoisonedArrow : BasePoisonedArrow, ICommodity
	{
		//public override Poison Poison{ get{ return Poison.Deadly; } }
		
		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} arrow [Deadly Poisoned]" : "{0} arrows [Deadly Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public LentezzaDeadlyPoisonedArrow() : this( 1 )
		{
			Poison = Poison.LentezzaDeadly;
		}

		[Constructable]
		public LentezzaDeadlyPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.LentezzaDeadly;
		}

		[Constructable]
		public LentezzaDeadlyPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LentezzaDeadlyPoisonedArrow( Serial serial ) : base( serial )
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
			if (Poison == null)
				Poison = Poison.LentezzaDeadly;
		}
		#endregion
	}

	public class LentezzaLethalPoisonedArrow : BasePoisonedArrow, ICommodity
	{
		//public override Poison Poison{ get{ return Poison.Lethal; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} arrow [Lethal Poisoned]" : "{0} arrows [Lethal Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public LentezzaLethalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.LentezzaLethal;
		}

		[Constructable]
		public LentezzaLethalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.LentezzaLethal;
		}

		[Constructable]
		public LentezzaLethalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LentezzaLethalPoisonedArrow( Serial serial ) : base( serial )
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
			if (Poison == null)
				Poison = Poison.LentezzaLethal;
		}
		#endregion
	}
}