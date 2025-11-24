using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class LesserPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public LesserPoisonedBolt() : this( 1 )
		{
			Poison = Poison.Lesser;
		}

		[Constructable]
		public LesserPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Lesser;
		}

		[Constructable]
		public LesserPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LesserPoisonedBolt( Serial serial ) : base( serial )
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

	public class NormalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public NormalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.Regular;
		}

		[Constructable]
		public NormalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Regular;
		}

		[Constructable]
		public NormalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public NormalPoisonedBolt( Serial serial ) : base( serial )
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

	public class GreaterPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public GreaterPoisonedBolt() : this( 1 )
		{
			Poison = Poison.Greater;
		}

		[Constructable]
		public GreaterPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Greater;
		}

		[Constructable]
		public GreaterPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public GreaterPoisonedBolt( Serial serial ) : base( serial )
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

	public class DeadlyPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public DeadlyPoisonedBolt() : this( 1 )
		{
			Poison = Poison.Deadly;
		}

		[Constructable]
		public DeadlyPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Deadly;
		}

		[Constructable]
		public DeadlyPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public DeadlyPoisonedBolt( Serial serial ) : base( serial )
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

	public class LethalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public LethalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.Lethal;
		}

		[Constructable]
		public LethalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.Lethal;
		}

		[Constructable]
		public LethalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public LethalPoisonedBolt( Serial serial ) : base( serial )
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