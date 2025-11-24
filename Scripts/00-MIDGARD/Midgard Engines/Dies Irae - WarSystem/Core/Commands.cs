/***************************************************************************
 *                               Commands.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;

namespace Midgard.Engines.WarSystem
{
    public class Commands
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "PuntiGuerra", AccessLevel.Player, new CommandEventHandler( ShowScore_OnCommand ) );
            CommandSystem.Register( "WarSystemEnabled", AccessLevel.Administrator, new CommandEventHandler( SetWarSystemStatus_OnCommand ) );
            CommandSystem.Register( "StartWar", AccessLevel.Administrator, new CommandEventHandler( StartServerWar_OnCommand ) );
        }

        [Usage( "WarSystemEnabled <true | false>" )]
        [Description( "Enables or disables war system." )]
        public static void SetWarSystemStatus_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                Config.Enabled = e.GetBoolean( 0 );
                e.Mobile.SendMessage( "War system have been {0}.", Config.Enabled ? "enabled" : "disabled" );
            }
            else
                e.Mobile.SendMessage( "Format: WarSystemEnabled <true | false>" );
        }

        [Usage( "PuntiGuerra" )]
        [Description( "Restituisce i punti della fazione a cui il personaggio appartiene" )]
        public static void ShowScore_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Mobile from = e.Mobile;

            if( Core.Instance.CurrentBattle == null )
            {
                from.SendMessage( "There is no war pending." );
                return;
            }

            switch( Core.Instance.CurrentPhase )
            {
                case WarPhase.Idle:
                    from.SendMessage( "There is no war pending." );
                    break;
                case WarPhase.PreBattle:
                    int until = (int)( ( DateTime.Now - Core.Instance.LastStateTime ).TotalMinutes );
                    from.SendMessage( "A war for {0} will start in {1} minutes.", Core.Instance.CurrentBattle.Definition.WarName, until );
                    break;
                case WarPhase.BattlePending:
                    from.SendMessage( "War status." );
                    foreach( WarState warState in Core.Instance.CurrentBattle.WarStates )
                        from.SendMessage( "{0}: {1}", warState.StateTeam.Name, warState.Score );
                    break;
                case WarPhase.PostBattle:
                    from.SendMessage( "{0} faction has won the last war.", Core.Instance.LastTeamWon );
                    break;
            }
        }

        [Usage( "StartServerWar" )]
        [Description( "Start selected war plan." )]
        public static void StartServerWar_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null )
                return;

            Mobile from = e.Mobile;

            if( e.Length == 0 )
                from.SendGump( new WarChoiceGump( from ) );
            else
                from.SendMessage( "Command use: [StartServerWar" );
        }
    }
}