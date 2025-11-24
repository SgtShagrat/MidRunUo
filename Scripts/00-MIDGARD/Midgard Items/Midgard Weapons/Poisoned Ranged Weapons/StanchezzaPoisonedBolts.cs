using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class StanchezzaLesserPoisonedBolt : BasePoisonedBolt, ICommodity
	{
		//public override Poison Poison { get { return Poison.Lesser; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} bolt [Lesser Poisoned]" : "{0} bolts [Lesser Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public StanchezzaLesserPoisonedBolt() : this( 1 )
		{
			Poison = Poison.StanchezzaLesser;
		}

		[Constructable]
		public StanchezzaLesserPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaLesser;
		}

		[Constructable]
		public StanchezzaLesserPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaLesserPoisonedBolt( Serial serial ) : base( serial )
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

	public class StanchezzaNormalPoisonedBolt : BasePoisonedBolt, ICommodity
	{
		//public override Poison Poison { get { return Poison.Regular; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} bolt [Poisoned]" : "{0} bolts [Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public StanchezzaNormalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.StanchezzaRegular;
		}

		[Constructable]
		public StanchezzaNormalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaRegular;
		}

		[Constructable]
		public StanchezzaNormalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaNormalPoisonedBolt( Serial serial ) : base( serial )
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

	public class StanchezzaGreaterPoisonedBolt : BasePoisonedBolt, ICommodity
	{
		//public override Poison Poison { get { return Poison.Greater; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} bolt [Greater Poisoned]" : "{0} bolts [Greater Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public StanchezzaGreaterPoisonedBolt() : this( 1 )
		{
			Poison = Poison.StanchezzaGreater;
		}

		[Constructable]
		public StanchezzaGreaterPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaGreater;
		}

		[Constructable]
		public StanchezzaGreaterPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaGreaterPoisonedBolt( Serial serial ) : base( serial )
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

	public class StanchezzaDeadlyPoisonedBolt : BasePoisonedBolt, ICommodity
	{
		//public override Poison Poison { get { return Poison.Deadly; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} bolt [Deadly Poisoned]" : "{0} bolts [Deadly Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public StanchezzaDeadlyPoisonedBolt() : this( 1 )
		{
			Poison = Poison.StanchezzaDeadly;
		}

		[Constructable]
		public StanchezzaDeadlyPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaDeadly;
		}

		[Constructable]
		public StanchezzaDeadlyPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaDeadlyPoisonedBolt( Serial serial ) : base( serial )
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

	public class StanchezzaLethalPoisonedBolt : BasePoisonedBolt, ICommodity
	{
		//public override Poison Poison { get { return Poison.Lethal; } }

		string ICommodity.Description
		{
			get
			{
				return String.Format( Amount == 1 ? "{0} bolt [Lethal Poisoned]" : "{0} bolts [Lethal Poisoned]", Amount );
			}
		}

		int ICommodity.DescriptionNumber { get { return 0; } }

		[Constructable]
		public StanchezzaLethalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.StanchezzaLethal;
		}

		[Constructable]
		public StanchezzaLethalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.StanchezzaLethal;
		}

		[Constructable]
		public StanchezzaLethalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public StanchezzaLethalPoisonedBolt( Serial serial ) : base( serial )
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