/***************************************************************************
 *							   BuccaTeleporter.cs
 *
 *   begin				: 23 maggio 2010
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Races;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
	public class BuccaTeleporter : Teleporter
	{
		[Constructable]
		public BuccaTeleporter()
		{
		}

		public override string DefaultName
		{
			get { return "Midgard Bucca Teleporter"; }
		}

		public override bool DisplayWeight
		{
			get { return false; }
		}

		public override bool OnMoveOver( Mobile m )
		{
			if( !Active )
				return true;

			if( !Creatures && !m.Player )
				return true;

			if( m is Midgard2PlayerMobile )
			{
				Midgard2PlayerMobile player = (Midgard2PlayerMobile)m;

				if( player.AccessLevel == AccessLevel.Player && Morph.UnderTransformation( m ) )
				{
					m.SendMessage( 0x22, m.Language == "ITA" ? "Non puoi passare in questa forma." : "Thou may not pass in a such form." );
					return false;
				}

				if( player.AccessLevel == AccessLevel.Player && !( /*m.Karma < -5000 ||*/ player.PermaRed || player.Town == MidgardTowns.BuccaneersDen || player.Kills > 5 ) )
				{
					if( m.BeginAction( this ) )
					{
						m.SendMessage( 0x22, m.Language == "ITA" ? "Non puoi passare." : "Thou may not pass." );

						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), delegate { m.EndAction( this ); } );
					}

					return false;
				}

				StartTeleport( m );
				return false;
			}

			return false;
		}

		#region serialization
		public BuccaTeleporter( Serial serial ) : base( serial )
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
		#endregion
	}
}