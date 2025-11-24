using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class BloccoLesserPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public BloccoLesserPoisonedArrow() : this( 1 )
		{
			Poison = Poison.BloccoLesser;
		}

		[Constructable]
		public BloccoLesserPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoLesser;
		}

		[Constructable]
		public BloccoLesserPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoLesserPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.BloccoLesser;
		}
		#endregion
	}

	public class BloccoNormalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public BloccoNormalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.BloccoRegular;
		}

		[Constructable]
		public BloccoNormalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoRegular;
		}

		[Constructable]
		public BloccoNormalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoNormalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.BloccoRegular;
		}
		#endregion
	}

	public class BloccoGreaterPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public BloccoGreaterPoisonedArrow() : this( 1 )
		{
			Poison = Poison.BloccoGreater;
		}

		[Constructable]
		public BloccoGreaterPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoGreater;
		}

		[Constructable]
		public BloccoGreaterPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoGreaterPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.BloccoGreater;
		}
		#endregion
	}

	public class BloccoDeadlyPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public BloccoDeadlyPoisonedArrow() : this( 1 )
		{
			Poison = Poison.BloccoDeadly;
		}

		[Constructable]
		public BloccoDeadlyPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoDeadly;
		}

		[Constructable]
		public BloccoDeadlyPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoDeadlyPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.BloccoDeadly;
		}
		#endregion
	}

	public class BloccoLethalPoisonedArrow : BasePoisonedArrow, ICommodity
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
		public BloccoLethalPoisonedArrow() : this( 1 )
		{
			Poison = Poison.BloccoLethal;
		}

		[Constructable]
		public BloccoLethalPoisonedArrow( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoLethal;
		}

		[Constructable]
		public BloccoLethalPoisonedArrow( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoLethalPoisonedArrow( Serial serial ) : base( serial )
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
				Poison = Poison.BloccoLethal;
		}
		#endregion
	}
}