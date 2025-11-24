/***************************************************************************
 *                               Infravision.cs
 *                            -------------------
 *   begin                : 21 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;

namespace Midgard.Engines.Races
{
    public class Infravision
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "Visione", AccessLevel.Player, new CommandEventHandler( Infravision_OnCommand ) );
        }

        [Usage( "Visione" )]
        [Description( "Permette la visione notturna" )]
        public static void Infravision_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            MidgardRace race = from.Race as MidgardRace;
            if( race == null )
                return;

            if( !Config.RaceVisionEnabled )
            {
                e.Mobile.SendMessage( "Questo potere e' stato temporaneamente disabilitato." );
            }
            else if( race.InfravisionLevel <= 0 )
            {
                from.SendMessage( "Non hai l'abilita' di vedere nel buio." );
            }
            else if( !from.Alive )
            {
                from.SendMessage( "Non puoi usare questo comando in questo stato." );
            }
            else if( e.Length > 0 )
            {
                from.SendMessage( "Uso del comando: [Visione" );
            }
            else if( !from.CanBeginAction( typeof( Infravision ) ) )
            {
                from.SendMessage( "Non puoi usare ancora questo potere." );
            }
            else
            {
                StartInfravision( from, race.InfravisionDuration, race.InfravisionLevel );
            }
        }

        private static void StartInfravision( Mobile from, int duration, int level )
        {
            from.BeginAction( typeof( Infravision ) );
            from.LightLevel = level;
            from.PublicOverheadMessage( Server.Network.MessageType.Emote, 1154, true, "*Si concentra per vedere nell'oscurita'*" );

            Timer.DelayCall( TimeSpan.FromMinutes( duration ), new TimerStateCallback( EndInfravision ), from );
        }

        private static void EndInfravision( object state )
        {
            Mobile from = state as Mobile;
            if( from == null )
                return;

            from.EndAction( typeof( Infravision ) );
            from.SendMessage( "I tuoi occhi stanchi chiedono un po' di riposo..." );
            from.LightLevel = 0;

            Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), new TimerStateCallback( ReleaseInfravisionLock ), from );
        }

        private static void ReleaseInfravisionLock( object state )
        {
            Mobile from = state as Mobile;
            if( from == null )
                return;

            from.EndAction( typeof( Infravision ) );
            from.SendMessage( "Ora puoi riusare l'infravisione." );
        }
    }
}