using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class MagiaLesserPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public MagiaLesserPoisonedArrow() : this( 1 )
		{
			Poison = Poison.MagiaLesser;
		}

		[Constructable]
		public MagiaLesserPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaLesser;
		}

		[Constructable]
		public MagiaLesserPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaLesserPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.MagiaLesser;
		}
		#endregion
	}

	public class MagiaNormalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public MagiaNormalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.MagiaRegular;
		}

		[Constructable]
		public MagiaNormalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaRegular;
		}

		[Constructable]
		public MagiaNormalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaNormalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.MagiaRegular;
		}
		#endregion
	}

	public class MagiaGreaterPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public MagiaGreaterPoisonedArrow() : this( 1 )
		{
			Poison = Poison.MagiaGreater;
		}

		[Constructable]
		public MagiaGreaterPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaGreater;
		}

		[Constructable]
		public MagiaGreaterPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaGreaterPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.MagiaGreater;
		}
		#endregion
	}

	public class MagiaDeadlyPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public MagiaDeadlyPoisonedArrow() : this( 1 )
		{
			Poison = Poison.MagiaDeadly;
		}

		[Constructable]
		public MagiaDeadlyPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaDeadly;
		}

		[Constructable]
		public MagiaDeadlyPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaDeadlyPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.MagiaDeadly;
		}
		#endregion
	}

	public class MagiaLethalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public MagiaLethalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.MagiaLethal;
		}

		[Constructable]
		public MagiaLethalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaLethal;
		}

		[Constructable]
		public MagiaLethalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaLethalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.MagiaLethal;
		}
		#endregion
	}
}