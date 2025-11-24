/***************************************************************************
 *                               SpeechCommands.cs
 *
 *   begin                : 16 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Network;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class SpeechCommands
    {
        public static void Initialize()
        {
            if( TownSystem.Enabled )
                EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
        }

        private static void ShowScoreCallback( object state )
        {
            TownPlayerState pl = (TownPlayerState)state;

            if( pl != null )
                pl.Mobile.PublicOverheadMessage( MessageType.Regular, pl.Mobile.SpeechHue, true, pl.TownRankPoints.ToString( "N0" ) );
        }

        private static void EventSink_Speech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;
            if( !( from is PlayerMobile ) )
                return;

            TownSystem sys = TownSystem.Find( from );
            if( sys == null )
                return;

            bool isInHisTown = sys.IsInTown( from );

            if( !e.Handled )
            {
                if( isInHisTown && sys.CanCommandGuards( from ) && Insensitive.Contains( e.Speech, "thou are fired" ) )
                {
                    sys.BeginOrderFiring( from );
                }
                else if( Insensitive.Contains( e.Speech, "i wish to know my rank" ) )
                {
                    TownPlayerState pl = TownPlayerState.Find( from );

                    if( pl != null )
                        Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ShowScoreCallback ), pl );
                }
            }
        }
    }
}