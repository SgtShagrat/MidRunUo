/***************************************************************************
 *                                   CappellinoNatalizio2006.cs
 *                            		---------------------
 *  begin               	: Dicembre, 2006
 * 	version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Cappelino Natalizio per l'inverno 2006.
 * 			Attenzione: molto sexy :)
 * 
 * 	TODO
 * 
 ***************************************************************************/
 
using System;

namespace Server.Items
{
	public class CappellinoNatalizio2006 : BaseHat
	{
		#region proprietà
		public override int BasePhysicalResistance{ get{ return 1; } }
		public override int BaseFireResistance{ get{ return 2; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 5; } }

		public override int InitMinHits{ get{ return 19; } }
		public override int InitMaxHits{ get{ return 38; } }
		
		public override bool AllowMaleWearer{ get{ return false; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public CappellinoNatalizio2006() : this( 0 )
		{		
		}

		[Constructable]
		public CappellinoNatalizio2006( int hue ) : base( 5149, hue )
		{
			Name = "Buon Natale 2006!";
			LootType = LootType.Blessed;
			Weight = 2.0;
		}

		public CappellinoNatalizio2006( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi
		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}
			
		public bool Validate( Mobile m )
		{
			if ( m == null || !m.Player || !m.Female )			
				return true;
			return true;
		}
		#endregion
		
		#region Serialize-deserialize
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
		#endregion
	}
}
