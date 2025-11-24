/***************************************************************************
 *                                  Statues.cs
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
	public class CrystalBullSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CrystalBullSouthAddonDeed();
			}
		}

		[Constructable]
		public CrystalBullSouthAddon()
		{
			AddComponent( new AddonComponent( 13825 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 13824 ), 1, 0, 0 );
		}

		public CrystalBullSouthAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CrystalBullSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CrystalBullSouthAddon();
			}
		}

		[Constructable]
		public CrystalBullSouthAddonDeed()
		{
			Name = "Crystal Bull South";
		}

		public CrystalBullSouthAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CrystalBullEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CrystalBullEastAddonDeed();
			}
		}

		[Constructable]
		public CrystalBullEastAddon()
		{
			AddComponent( new AddonComponent( 13822 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 13823 ), 0, 0, 0 );
		}

		public CrystalBullEastAddon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CrystalBullEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CrystalBullEastAddon();
			}
		}

		[Constructable]
		public CrystalBullEastAddonDeed()
		{
			Name = "Crystal Bull East";
		}

		public CrystalBullEastAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CrystalBeggarStatueSouth : Item
	{
		[Constructable]
		public CrystalBeggarStatueSouth() : base( 0x35F8 )
		{
			Name = "Crystal Beggar Statue";
			Weight = 20.0;
		}

		public CrystalBeggarStatueSouth(Serial serial) : base(serial)
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

	public class CrystalBeggarStatueEast : Item
	{
		[Constructable]
		public CrystalBeggarStatueEast() : base( 0x35F9 )
		{
			Name = "Crystal Beggar Statue";
			Weight = 20.0;
		}

		public CrystalBeggarStatueEast(Serial serial) : base(serial)
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

	public class CrystalSupplicantStatueSouth : Item
	{
		[Constructable]
		public CrystalSupplicantStatueSouth() : base( 0x35FB )
		{
			Name = "Crystal Supplicant Statue";
			Weight = 20.0;
		}

		public CrystalSupplicantStatueSouth(Serial serial) : base(serial)
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

	public class CrystalSupplicantStatueEast : Item
	{
		[Constructable]
		public CrystalSupplicantStatueEast() : base( 0x35FA )
		{
			Name = "Crystal Supplicant Statue";
			Weight = 20.0;
		}

		public CrystalSupplicantStatueEast(Serial serial) : base(serial)
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

	public class CrystalRunnerStatueSouth : Item
	{
		[Constructable]
		public CrystalRunnerStatueSouth() : base( 0x35FD )
		{
			Name = "Crystal Runner Statue";
			Weight = 20.0;
		}

		public CrystalRunnerStatueSouth(Serial serial) : base(serial)
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

	public class CrystalRunnerStatueEast : Item
	{
		[Constructable]
		public CrystalRunnerStatueEast() : base( 0x35FC )
		{
			Name = "Crystal Runner Statue";
			Weight = 20.0;
		}

		public CrystalRunnerStatueEast(Serial serial) : base(serial)
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
