/***************************************************************************
 *                                  CrystalOreElemental.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Elementale che spawna raccogiendo cristalli.
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
	public class CrystalOreElemental : CrystalElemental
	{
		[Constructable]
		public CrystalOreElemental() : this( 2 )
		{
		}

		[Constructable]
		public CrystalOreElemental( int crystalsAmount ) : base()
		{
			PackItem( new Midgard.Items.CrystalOre( crystalsAmount ) );
		}

		public CrystalOreElemental( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
