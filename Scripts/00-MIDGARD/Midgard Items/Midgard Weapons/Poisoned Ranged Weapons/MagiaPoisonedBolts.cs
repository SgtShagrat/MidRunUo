using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class MagiaLesserPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public MagiaLesserPoisonedBolt() : this( 1 )
		{
			Poison = Poison.MagiaLesser;
		}

		[Constructable]
		public MagiaLesserPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaLesser;
		}

		[Constructable]
		public MagiaLesserPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaLesserPoisonedBolt( Serial serial ) : base( serial )
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

	public class MagiaNormalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public MagiaNormalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.MagiaRegular;
		}

		[Constructable]
		public MagiaNormalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaRegular;
		}

		[Constructable]
		public MagiaNormalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaNormalPoisonedBolt( Serial serial ) : base( serial )
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

	public class MagiaGreaterPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public MagiaGreaterPoisonedBolt() : this( 1 )
		{
			Poison = Poison.MagiaGreater;
		}

		[Constructable]
		public MagiaGreaterPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaGreater;
		}

		[Constructable]
		public MagiaGreaterPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaGreaterPoisonedBolt( Serial serial ) : base( serial )
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

	public class MagiaDeadlyPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public MagiaDeadlyPoisonedBolt() : this( 1 )
		{
			Poison = Poison.MagiaDeadly;
		}

		[Constructable]
		public MagiaDeadlyPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaDeadly;
		}

		[Constructable]
		public MagiaDeadlyPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaDeadlyPoisonedBolt( Serial serial ) : base( serial )
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

	public class MagiaLethalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public MagiaLethalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.MagiaLethal;
		}

		[Constructable]
		public MagiaLethalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.MagiaLethal;
		}

		[Constructable]
		public MagiaLethalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public MagiaLethalPoisonedBolt( Serial serial ) : base( serial )
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