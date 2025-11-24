/***************************************************************************
 *                                  StoneHarvester.cs
 *                            		-----------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Tool per l'harvesting delle pietre.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Engines.Harvest;
using Server.Items;

namespace Midgard.Items
{
	public class StoneHarvester : BaseHarvestTool
	{
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

		[Constructable]
		public StoneHarvester() : this( 50 )
		{
		}

		[Constructable]
		public StoneHarvester( int uses ) : base( uses, 0x12B3 )
		{
			Name = "Stone Harvester";
			Hue = 0x7E7;
			Weight = 5.0;
		}

		public StoneHarvester( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}

