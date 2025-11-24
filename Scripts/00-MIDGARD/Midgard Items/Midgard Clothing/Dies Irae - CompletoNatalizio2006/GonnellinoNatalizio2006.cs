/***************************************************************************
 *                                   GonnellinoNatalizio2006.cs
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
 * 			Gonnellino Natalizio per l'inverno 2006.
 * 			Attenzione: molto sexy :)
 * 
 * 	TODO
 * 
 ***************************************************************************/
 
using System;

namespace Server.Items
{
	public class GonnellinoNatalizio2006 : BaseOuterLegs
	{
		#region proprieta'
		public override bool AllowMaleWearer{ get{ return false; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public GonnellinoNatalizio2006() : this( 0 )
		{
		}

		[Constructable]
		public GonnellinoNatalizio2006( int hue ) : base( 5151, hue )
		{
			Name = "Buon Natale 2006!";
			LootType = LootType.Blessed;
			Weight = 2.0;
		}

		public GonnellinoNatalizio2006( Serial serial ) : base( serial )
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
