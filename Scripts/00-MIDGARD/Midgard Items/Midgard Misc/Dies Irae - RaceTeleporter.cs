/***************************************************************************
 *								  RaceTeleporter.cs
 *									-------------------
 *  begin					: Luglio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Teleperter che non fa passare chi non e' di una certa razza.
 * 
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Items
{
	public class RaceTeleporter : Teleporter
	{
		private static readonly string DefaultNoRaceString = "Your race is not allowed to enter here.";

		[CommandProperty( AccessLevel.GameMaster )]
		public string NoRaceMessageString { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public Race Race { get; set; }

		private void EndMessageLock( object state )
		{
			( (Mobile)state ).EndAction( this );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if( Active )
			{
				if( !Creatures && !m.Player )
					return true;

				if( m.AccessLevel == AccessLevel.Player && Race != m.Race )
				{
					if( m.BeginAction( this ) )
					{
						if( !String.IsNullOrEmpty( NoRaceMessageString ) )
							m.SendMessage( 0x22, NoRaceMessageString );

						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
					}

					return false;
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}

		public override string DefaultName
		{
			get { return "Midgard Race Teleporter"; }
		}

		public override bool DisplayWeight
		{
			get { return false; }
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( Race != null && !String.IsNullOrEmpty( Race.Name ) )
				list.Add( "Allowed: {0}", Race.PluralName );
		}

		public override void OnSingleClick( Mobile from )
		{
			if( Race != null && !String.IsNullOrEmpty( Race.Name ) )
				LabelTo( from, "Allowed: {0}", Race.PluralName );

			base.OnSingleClick( from );
		}

		[Constructable]
		public RaceTeleporter()
		{
			NoRaceMessageString = DefaultNoRaceString;
		}

		#region serialization
		public RaceTeleporter( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version

			writer.Write( NoRaceMessageString );
			writer.Write( Race );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
				case 0:
					{
						NoRaceMessageString = reader.ReadString();
						Race = reader.ReadRace();
						break;
					}
			}
		}
		#endregion
	}
}