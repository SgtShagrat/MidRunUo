/***************************************************************************
 *                                  CrystalHarvester.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Tool per l'harvesting dei cristalli.
 * 
 ***************************************************************************/

using Midgard.Engines;

using Server;
using Server.Engines.Harvest;
using Server.Items;

namespace Midgard.Items
{
    public class CrystalHarvester : BaseHarvestTool
    {
        public override HarvestSystem HarvestSystem { get { return CrystalHarvesting.System; } }

        [Constructable]
        public CrystalHarvester() : this( 50 )
        {
        }

        [Constructable]
        public CrystalHarvester( int uses ) : base( uses, 0x12B3 )
        {
            Name = "Crystal Harvester";
            Weight = 5.0;
        }

        public CrystalHarvester( Serial serial ) : base( serial )
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
        }
    }
}