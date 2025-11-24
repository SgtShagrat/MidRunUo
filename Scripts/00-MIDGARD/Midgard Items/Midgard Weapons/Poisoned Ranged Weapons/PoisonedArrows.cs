using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class LesserPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public LesserPoisonedArrow() : this( 1 )
		{
			Poison = Poison.Lesser;
		}

		[Constructable]
		public LesserPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Lesser;
		}

		[Constructable]
		public LesserPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LesserPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.Lesser;
		}
		#endregion
	}

	public class NormalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public NormalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.Regular;
		}

		[Constructable]
		public NormalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Regular;
		}

		[Constructable]
		public NormalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public NormalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.Regular;
		}
		#endregion
	}

	public class GreaterPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public GreaterPoisonedArrow() : this( 1 )
		{
			Poison = Poison.Greater;
		}

		[Constructable]
		public GreaterPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Greater;
		}

		[Constructable]
		public GreaterPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public GreaterPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.Greater;
		}
		#endregion
	}

	public class DeadlyPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public DeadlyPoisonedArrow() : this( 1 )
		{
			Poison = Poison.Deadly;
		}

		[Constructable]
		public DeadlyPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Deadly;
		}

		[Constructable]
		public DeadlyPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public DeadlyPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.Deadly;
		}
		#endregion
	}

	public class LethalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public LethalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.Lethal;
		}

		[Constructable]
		public LethalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Deadly;
		}

		[Constructable]
		public LethalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LethalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.Lethal;
		}
		#endregion
	}
}