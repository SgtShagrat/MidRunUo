using System;

using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.Classes
{
	public class NecroTravellingStone : Item
	{
		[Constructable]
		public NecroTravellingStone() : base( 0x1173 )
		{
			Weight = 1000.0;
		}

		public override void OnSingleClick( Mobile from )
		{
			if( ClassSystem.Find( from ) == ClassSystem.Necromancer || from.AccessLevel > AccessLevel.Player )
				LabelTo( from, from.Language == "ITA" ? "una lapide maledetta" : "a damned gravestone" );
			else
				base.OnSingleClick( from );
		}

		public override bool HandlesOnSpeech { get { return true; } }

		private const string TravelPhrase = "Evil one, take me to";
		private const string TravelPhraseITA = "Anima maledetta, portami a";

		public override void OnSpeech( SpeechEventArgs e )
		{
			if( !e.Handled )
			{
				Mobile necro = e.Mobile;
				if( necro == null )
					return;

				if( ClassSystem.Find( necro ) != ClassSystem.Necromancer )
					return;

				if( e.Handled || !necro.Alive || !necro.InRange( GetWorldLocation(), 3 ) )
					return;

				NecroLocationEntry entryFound = null;

				bool starts = Insensitive.StartsWith( e.Speech.ToLower(), TravelPhrase ) || Insensitive.StartsWith( e.Speech.ToLower(), TravelPhraseITA );

				if( starts )
				{
					foreach( NecroLocationEntry entry in NecroLocationEntry.NecroLocations )
					{
						if( e.Speech.ToLower().IndexOf( entry.Name.ToLower() ) >= 0 )
						{
							entryFound = entry;
							break; //Magius(CHE) :-)
						}
					}
				}

				if( entryFound == null )
					return;

				if( SpellHelper.CheckCombat( necro ) )
				{
					necro.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
					return;
				}

				e.Handled = true;
				StartTeleport( necro, entryFound );
			}
		}

		public virtual void StartTeleport( Mobile m, NecroLocationEntry entry )
		{
			Timer.DelayCall( TimeSpan.FromSeconds( Utility.Dice( 1, 4, 1 ) ), new TimerStateCallback( DoTeleport_Callback ), new object[] { m, entry } );
		}

		private void DoTeleport_Callback( object state )
		{
			object[] pars = state as object[];
			if( pars == null )
				return;

			DoTeleport( (Mobile)pars[ 0 ], (NecroLocationEntry)pars[ 1 ] );
		}

		public virtual void DoTeleport( Mobile m, NecroLocationEntry entry )
		{
			Map map = entry.Map;

			if( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = entry.Location;
			if( p == Point3D.Zero )
				p = m.Location;

			BaseCreature.TeleportPets( m, p, map );

			bool sendEffect = m.AccessLevel == AccessLevel.Player || !m.Hidden;

			if( sendEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			m.MoveToWorld( p, map );

			if( sendEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			if( sendEffect )
				Effects.PlaySound( m.Location, m.Map, 0x1FC );
		}

		#region serialization
		public NecroTravellingStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
		#endregion
	}
}