/***************************************************************************
 *							   KarmaTeleporter.cs
 *
 *   begin				: 23 maggio 2010
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.TalkingVendors;

using Server;
using Server.Items;

namespace Midgard.Items
{
	public class KarmaTeleporter : Teleporter
	{
		#region KarmaLockType enum
		public enum KarmaLockType
		{
			MustBeLower,
			MustBeGreater
		}
		#endregion

		[Constructable]
		public KarmaTeleporter()
		{
			KarmaLevel = 0;
			LockType = KarmaLockType.MustBeGreater;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int KarmaLevel { get; private set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public KarmaLockType LockType { get; private set; }

		public override string DefaultName
		{
			get { return "Midgard Karma Teleporter"; }
		}

		public override bool DisplayWeight
		{
			get { return false; }
		}

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

				if( m.AccessLevel == AccessLevel.Player && (( LockType == KarmaLockType.MustBeGreater && m.Karma <= KarmaLevel ) ||
					( LockType == KarmaLockType.MustBeLower && m.Karma >= KarmaLevel )) )
				{
					if( m.BeginAction( this ) )
					{
						m.SendMessage( 0x22, "Thou may not pass." );

						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
					}

					return false;
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if( from.AccessLevel > AccessLevel.Player )
				LabelTo( from, string.Format( "Lock: {0} Level: {1}", LockType, KarmaLevel ) );
		}

		#region serialization
		public KarmaTeleporter( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version

			writer.WriteEncodedInt( KarmaLevel );
			writer.WriteEncodedInt( (int)LockType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
				case 0:
					{
						KarmaLevel = reader.ReadEncodedInt();
						LockType = (KarmaLockType)reader.ReadEncodedInt();
						break;
					}
			}
		}
		#endregion
	}
}