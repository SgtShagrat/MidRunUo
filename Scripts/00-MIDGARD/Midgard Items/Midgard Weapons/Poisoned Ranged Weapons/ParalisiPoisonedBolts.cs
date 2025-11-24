using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class ParalisiLesserPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public ParalisiLesserPoisonedBolt() : this( 1 )
		{
			Poison = Poison.ParalisiLesser;
		}

		[Constructable]
		public ParalisiLesserPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.ParalisiLesser;
		}

		[Constructable]
		public ParalisiLesserPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public ParalisiLesserPoisonedBolt( Serial serial ) : base( serial )
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
				Poison = Poison.ParalisiLesser;
		}
		#endregion
	}

	public class ParalisiNormalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public ParalisiNormalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.ParalisiRegular;
		}

		[Constructable]
		public ParalisiNormalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.ParalisiRegular;
		}

		[Constructable]
		public ParalisiNormalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public ParalisiNormalPoisonedBolt( Serial serial ) : base( serial )
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
				Poison = Poison.ParalisiRegular;
		}
		#endregion
	}

	public class ParalisiGreaterPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public ParalisiGreaterPoisonedBolt() : this( 1 )
		{
			Poison = Poison.ParalisiGreater;
		}

		[Constructable]
		public ParalisiGreaterPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.ParalisiGreater;
		}

		[Constructable]
		public ParalisiGreaterPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public ParalisiGreaterPoisonedBolt( Serial serial ) : base( serial )
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
				Poison = Poison.ParalisiGreater;
		}
		#endregion
	}

	public class ParalisiDeadlyPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public ParalisiDeadlyPoisonedBolt() : this( 1 )
		{
			Poison = Poison.ParalisiDeadly;
		}

		[Constructable]
		public ParalisiDeadlyPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.ParalisiDeadly;
		}

		[Constructable]
		public ParalisiDeadlyPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public ParalisiDeadlyPoisonedBolt( Serial serial ) : base( serial )
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
				Poison = Poison.ParalisiDeadly;
		}
		#endregion
	}

	public class ParalisiLethalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public ParalisiLethalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.ParalisiLethal;
		}

		[Constructable]
		public ParalisiLethalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.ParalisiLethal;
		}

		[Constructable]
		public ParalisiLethalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public ParalisiLethalPoisonedBolt( Serial serial ) : base( serial )
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
				Poison = Poison.ParalisiLethal;
		}
		#endregion
	}
}