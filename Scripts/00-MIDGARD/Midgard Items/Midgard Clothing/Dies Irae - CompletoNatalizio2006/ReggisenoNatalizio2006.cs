/***************************************************************************
 *                                   ReggisenoNatalizio2006.cs
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
 * 			Reggiseno Natalizio per l'inverno 2006.
 * 			Attenzione: molto sexy :)
 * 
 * 	TODO
 * 
 ***************************************************************************/
 
using System;

namespace Server.Items
{
	public class ReggisenoNatalizio2006 : BaseShirt
	{
		#region proprieta'
		public override bool AllowMaleWearer{ get{ return false; } }
		#endregion
		
		#region costruttori
		[Constructable]
		public ReggisenoNatalizio2006() : this( 0 )
		{			
		}

		[Constructable]
		public ReggisenoNatalizio2006( int hue ) : base( 5150, hue )
		{
			Name = "Buon Natale 2006!";
			LootType = LootType.Blessed;
			Weight = 2.0;
		}

		public ReggisenoNatalizio2006( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi
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
