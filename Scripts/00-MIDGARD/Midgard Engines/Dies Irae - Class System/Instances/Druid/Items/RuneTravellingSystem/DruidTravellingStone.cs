/***************************************************************************
 *                               DruidTravellingStone.cs
 *
 *   begin                : 29 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.Classes
{
    public class DruidTravellingStone : Item
    {
        [Constructable]
        public DruidTravellingStone()
            : base( 0x1363 )
        {
            Weight = 1000.0;
        }

        public override void OnSingleClick( Mobile from )
        {
            if( ClassSystem.Find( from ) == ClassSystem.Druid || from.AccessLevel > AccessLevel.Player )
                LabelTo( from, "a magical druid stone" );
            else
                base.OnSingleClick( from );
        }

        public override bool HandlesOnSpeech { get { return true; } }

        private const string TravelPhrase = "Sister Stone, take me to";
        private const string TravelPhraseITA = "Sorella pietra, portami a";

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled )
            {
                Mobile druid = e.Mobile;
                if( druid == null )
                    return;

                if( ClassSystem.Find( druid ) != ClassSystem.Druid )
                    return;

                if( e.Handled || !druid.Alive || !druid.InRange( GetWorldLocation(), 3 ) )
                    return;

                DruidLocationEntry entryFound = null;

                bool starts = Insensitive.StartsWith( e.Speech.ToLower(), TravelPhrase ) || Insensitive.StartsWith( e.Speech.ToLower(), TravelPhraseITA );

                if( starts )
                {
                    foreach( DruidLocationEntry entry in DruidLocationEntry.DruidLocations )
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

                if( SpellHelper.CheckCombat( druid ) )
                {
                    druid.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
                    return;
                }

                e.Handled = true;
                StartTeleport( druid, entryFound );
            }
        }

        public virtual void StartTeleport( Mobile m, DruidLocationEntry entry )
        {
            Timer.DelayCall( TimeSpan.FromSeconds( Utility.Dice( 1, 4, 1 ) ), new TimerStateCallback( DoTeleport_Callback ), new object[] { m, entry } );
        }

        private void DoTeleport_Callback( object state )
        {
            object[] pars = state as object[];
            if( pars == null )
                return;

            DoTeleport( (Mobile)pars[ 0 ], (DruidLocationEntry)pars[ 1 ] );
        }

        public virtual void DoTeleport( Mobile m, DruidLocationEntry entry )
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
        public DruidTravellingStone( Serial serial )
            : base( serial )
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