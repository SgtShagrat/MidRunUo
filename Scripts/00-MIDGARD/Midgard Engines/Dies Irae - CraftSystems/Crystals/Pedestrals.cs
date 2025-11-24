/***************************************************************************
 *                                  Pedestrals.cs
 *                            		-------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class CrystalPedestralSmall : Item
	{
		[Constructable]
		public CrystalPedestralSmall() : base( 0x3CF4 )
		{
			Name = "Crystal Pedestral";
			Weight = 10.0;
		}

		public CrystalPedestralSmall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class CrystalPedestralMedium : Item
	{
		[Constructable]
		public CrystalPedestralMedium() : base( 0x3CF3 )
		{
			Name = "Crystal Pedestral";
			Weight = 15.0;
		}

		public CrystalPedestralMedium( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class CrystalPedestralLargeEmpty : Item
	{
		[Constructable]
		public CrystalPedestralLargeEmpty() : base( 0x2FD5 )
		{
			Name = "Empty Crystal Pedestral";
			Weight = 10.0;
		}

		public CrystalPedestralLargeEmpty( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}

	public class CrystalPedestralLarge : Item
	{
		[Constructable]
		public CrystalPedestralLarge() : base( 0x2FD4 )
		{
			Name = "Crystal Pedestral";
			Weight = 20.0;
		}

		public CrystalPedestralLarge( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
