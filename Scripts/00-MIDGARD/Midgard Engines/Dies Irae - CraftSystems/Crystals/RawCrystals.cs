/***************************************************************************
 *                                  Crystals.cs
 *                            		-----------
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

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class RawCrystal : Item
	{
		[Constructable]
		public RawCrystal() : base( 0x3678 )
		{
			Name = "a raw crystal";
			Stackable = false;
			Weight = 1.0;
		}

		public RawCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LittleUnshapedCrystal : Item
	{
		[Constructable]
		public LittleUnshapedCrystal() : base( 0x35EB )
		{
			Name = "an unshaped crystal";
			Stackable = false;
			Weight = 1.0;
		}

		public LittleUnshapedCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LargeUnshapedCrystal : Item
	{
		[Constructable]
		public LargeUnshapedCrystal() : base( 0x35EC )
		{
			Name = "an unshaped crystal";
			Stackable = false;
			Weight = 1.0;
		}

		public LargeUnshapedCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class TallUnshapedCrystal : Item
	{
		[Constructable]
		public TallUnshapedCrystal() : base( 0x35Ea )
		{
			Name = "an unshaped crystal";
			Stackable = false;
			Weight = 1.0;
		}

		public TallUnshapedCrystal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[FlipableAttribute(0x35F6, 0x35F7)]
	public class CrystalPillar : Item
	{
		[Constructable]
		public CrystalPillar() : base( 0x35F6 )
		{
			Name = "Crystal Pillar";
			Weight = 20.0;
		}

		public CrystalPillar(Serial serial) : base(serial)
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
