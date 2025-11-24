using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class BloccoLesserPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public BloccoLesserPoisonedBolt() : this( 1 )
		{
			Poison = Poison.BloccoLesser;
		}

		[Constructable]
		public BloccoLesserPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoLesser;
		}

		[Constructable]
		public BloccoLesserPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoLesserPoisonedBolt( Serial serial ) : base( serial )
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

	public class BloccoNormalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public BloccoNormalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.BloccoRegular;
		}

		[Constructable]
		public BloccoNormalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoRegular;
		}

		[Constructable]
		public BloccoNormalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoNormalPoisonedBolt( Serial serial ) : base( serial )
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

	public class BloccoGreaterPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public BloccoGreaterPoisonedBolt() : this( 1 )
		{
			Poison = Poison.BloccoGreater;
		}

		[Constructable]
		public BloccoGreaterPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoGreater;
		}

		[Constructable]
		public BloccoGreaterPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoGreaterPoisonedBolt( Serial serial ) : base( serial )
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

	public class BloccoDeadlyPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public BloccoDeadlyPoisonedBolt() : this( 1 )
		{
			Poison = Poison.BloccoDeadly;
		}

		[Constructable]
		public BloccoDeadlyPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoDeadly;
		}

		[Constructable]
		public BloccoDeadlyPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoDeadlyPoisonedBolt( Serial serial ) : base( serial )
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

	public class BloccoLethalPoisonedBolt : BasePoisonedBolt, ICommodity
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
		public BloccoLethalPoisonedBolt() : this( 1 )
		{
			Poison = Poison.BloccoLethal;
		}

		[Constructable]
		public BloccoLethalPoisonedBolt( int amount )
		{
			Stackable = true;
			Amount = amount;
			Poison = Poison.BloccoLethal;
		}

		[Constructable]
		public BloccoLethalPoisonedBolt( int amount, Poison poison )
		{
			Stackable = true;
			Amount = amount;
			Poison = poison;
		}

		#region serialization
		public BloccoLethalPoisonedBolt( Serial serial ) : base( serial )
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