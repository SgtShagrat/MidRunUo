/***************************************************************************
 *                                  CrystalOre.cs
 *                            		-------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Risorsa base per il crafting delle cose in cristallo.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
    public class CrystalOre : Item, ICommodity
    {
        string ICommodity.Description
        {
            get
            {
                return String.Format( Amount == 1 ? "{0} raw broken crystal" : "{0} raw broken crystals", Amount );
            }
        }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override int LabelNumber { get { return 1074262; } } // crushed crystal pieces

        [Constructable]
        public CrystalOre()
            : this( 1 )
        {
        }

        [Constructable]
        public CrystalOre( int amount )
            : base( Utility.RandomMinMax( 0x223A, 0x2249 ) )
        {
            Stackable = false;
            Weight = 5.0;
        }

        public CrystalOre( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}
