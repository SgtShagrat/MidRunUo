/***************************************************************************
 *                                  CrystalChisel.cs
 *                            		----------------
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

using Midgard.Engines.Craft;

using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    [Flipable( 0x1EB8, 0x1EB9 )]
    public class CrystalChisel : BaseTool
    {
        public override CraftSystem CraftSystem { get { return DefCrystalCrafting.CraftSystem; } }

        [Constructable]
        public CrystalChisel() : base( 50, 0x12B3 )
        {
            Name = "Crystal Chisel";
            Hue = 0x791;
            Weight = 2.0;
        }

        public CrystalChisel( Serial serial ) : base( serial )
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