/***************************************************************************
 *                                   AlchemistTableAddon.cs
 *                            		------------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *  		Tavolo per l'alchimia.
 ***************************************************************************/

using System;

namespace Server.Items
{
	#region East
	public class AlchemistTableEastAddon : BaseAddon
	{
		#region proprietà
		public override BaseAddonDeed Deed{ get{ return new AlchemistTableEastDeed(); } }
		#endregion
		
		#region costruttori
		[Constructable]
		public AlchemistTableEastAddon()
		{
			AddComponent( new AddonComponent( 0x2DD3 ), 0, 0, 0 );
		}

		public AlchemistTableEastAddon( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial deserial
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
		#endregion
	}

	public class AlchemistTableEastDeed : BaseAddonDeed
	{
		#region proprietà
		public override BaseAddon Addon{ get{ return new AlchemistTableEastAddon(); } }
		public override int LabelNumber{ get{ return 1064037; } } // alchemist table (east)
		#endregion
		
		#region costruttori
		[Constructable]
		public AlchemistTableEastDeed()
		{
		}

		public AlchemistTableEastDeed( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial deserial
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
		#endregion
	}
	#endregion
	
	#region south
	public class AlchemistTableSouthAddon : BaseAddon
	{
		#region proprietà
		public override BaseAddonDeed Deed{ get{ return new AlchemistTableSouthDeed(); } }
		#endregion
		
		#region costruttori
		[Constructable]
		public AlchemistTableSouthAddon()
		{
			AddComponent( new AddonComponent( 0x2DD4 ), 0, 0, 0 );
		}

		public AlchemistTableSouthAddon( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial deserial
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
		#endregion
	}

	public class AlchemistTableSouthDeed : BaseAddonDeed
	{
		#region proprietà
		public override BaseAddon Addon{ get{ return new AlchemistTableSouthAddon(); } }
		public override int LabelNumber{ get{ return 1064038; } } // alchemist table (south)
		#endregion
		
		#region costruttori
		[Constructable]
		public AlchemistTableSouthDeed()
		{
		}

		public AlchemistTableSouthDeed( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region serial deserial
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
		#endregion
	}
	#endregion
}
