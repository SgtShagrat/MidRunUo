using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class StanchezzaLesserPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public StanchezzaLesserPoisonedArrow() : this( 1 )
		{
			Poison = Poison.StanchezzaLesser;
		}

		[Constructable]
		public StanchezzaLesserPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaLesser;
		}

		[Constructable]
		public StanchezzaLesserPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaLesserPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.StanchezzaLesser;
		}
		#endregion
	}

	public class StanchezzaNormalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public StanchezzaNormalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.StanchezzaRegular;
		}

		[Constructable]
		public StanchezzaNormalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaRegular;
		}

		[Constructable]
		public StanchezzaNormalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaNormalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.StanchezzaRegular;
		}
		#endregion
	}

	public class StanchezzaGreaterPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public StanchezzaGreaterPoisonedArrow() : this( 1 )
		{
			Poison = Poison.StanchezzaGreater;
		}

		[Constructable]
		public StanchezzaGreaterPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaGreater;
		}

		[Constructable]
		public StanchezzaGreaterPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaGreaterPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.StanchezzaGreater;
		}
		#endregion
	}

	public class StanchezzaDeadlyPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public StanchezzaDeadlyPoisonedArrow() : this( 1 )
		{
			Poison = Poison.StanchezzaDeadly;
		}

		[Constructable]
		public StanchezzaDeadlyPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaDeadly;
		}

		[Constructable]
		public StanchezzaDeadlyPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaDeadlyPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.StanchezzaDeadly;
		}
		#endregion
	}

	public class StanchezzaLethalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public StanchezzaLethalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.StanchezzaLethal;
		}

		[Constructable]
		public StanchezzaLethalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaLethal;
		}

		[Constructable]
		public StanchezzaLethalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaLethalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.StanchezzaLethal;
		}
		#endregion
	}
}