/***************************************************************************
 *                                  CrystalDecorations.cs
 *                            		---------------------
 *  begin                	: Gennaio, 2008
 *  begin                	: Mese, 2000
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
	public class CrystalBrazier : Item
	{
		[Constructable]
		public CrystalBrazier() : base( 0x35EF )
		{
			Name = "Crystal Brazier";
			Weight = 20.0;
		}

		public CrystalBrazier( Serial serial ) : base( serial )
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

	[Furniture]
	public class CrystalThroneSouth : Item
	{
		[Constructable]
		public CrystalThroneSouth() : base( 0x35ED )
		{
			Name = "Crystal Throne";
			Weight = 20.0;
		}

		public CrystalThroneSouth(Serial serial) : base(serial)
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

	[Furniture]
	public class CrystalThroneEast : Item
	{
		[Constructable]
		public CrystalThroneEast() : base( 0x35EE )
		{
			Name = "Crystal Throne";
			Weight = 20.0;
		}

		public CrystalThroneEast(Serial serial) : base(serial)
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

	public class CrystalTableEastAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CrystalTableEastAddonDeed();
			}
		}

		[Constructable]
		public CrystalTableEastAddon()
		{
			AddComponent( new AddonComponent( 13830 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 13829 ), 0, 1, 0 );
		}

		public CrystalTableEastAddon( Serial serial ) : base( serial )
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

	public class CrystalTableEastAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CrystalTableEastAddon();
			}
		}

		[Constructable]
		public CrystalTableEastAddonDeed()
		{
			Name = "Crystal Table East";
		}

		public CrystalTableEastAddonDeed( Serial serial ) : base( serial )
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
	
	public class CrystalTableSouthAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new CrystalTableSouthAddonDeed();
			}
		}

		[Constructable]
		public CrystalTableSouthAddon()
		{
			AddComponent( new AddonComponent( 13831 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 13832 ), 0, 0, 0 );
		}

		public CrystalTableSouthAddon( Serial serial ) : base( serial )
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

	public class CrystalTableSouthAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new CrystalTableSouthAddon();
			}
		}

		[Constructable]
		public CrystalTableSouthAddonDeed()
		{
			Name = "Crystal Table South";
		}

		public CrystalTableSouthAddonDeed( Serial serial ) : base( serial )
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

	public class CrystalAltarAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get	{ return new CrystalAltarAddonDeed(); }
		}

		[Constructable]
		public CrystalAltarAddon()
		{
			AddComponent( new AddonComponent( 13801 ), 0, 0, 0 );
		}

		public CrystalAltarAddon( Serial serial ) : base( serial )
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

	public class CrystalAltarAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get	{	return new CrystalAltarAddon();	}
		}

		[Constructable]
		public CrystalAltarAddonDeed()
		{
			Name = "Crystal Altar";
		}

		public CrystalAltarAddonDeed( Serial serial ) : base( serial )
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
}
