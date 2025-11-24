/***************************************************************************
 *                               Commands.cs
 *                            -----------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;

namespace Midgard.Engines.OrderChaosWars
{
    public class Commands
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "PuntiVirtu", AccessLevel.Player, new CommandEventHandler( Battlescore_OnCommand ) );

            CommandSystem.Register( "WarSystemEnabled", AccessLevel.Administrator, new CommandEventHandler( SetWarSystemStatus_OnCommand ) );
            CommandSystem.Register( "StartServerWar", AccessLevel.Administrator, new CommandEventHandler( StartServerWar_OnCommand ) );
        }

        [Usage( "WarSystemEnabled <true | false>" )]
        [Description( "Enables or disables order/chaos war system." )]
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

        [Usage( "PuntiVirtu" )]
        [Description( "Restituisce i punti della fazione (Order/Chaos) a cui il personaggio appartiene" )]
        public static void Battlescore_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 0 )
                return;

            Mobile from = e.Mobile;

            if( Core.Instance.CurrentBattle == null )
            {
                from.SendMessage( "There is no war pending." );
                return;
            }
            
            Virtue v = Core.Find( from );
            if( v == Virtue.None && from.AccessLevel == AccessLevel.Player )
            {
                from.SendMessage( "You cannot use this command." );
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
                    from.SendMessage( "War status. Order: {0} - Chaos {1}",
                        Core.Instance.GetPoints( Virtue.Order ),
                        Core.Instance.GetPoints( Virtue.Chaos ) );
                    break;
                case WarPhase.PostBattle:
                    from.SendMessage( "{0} faction has won the last war.", Core.Instance.LastVirtueWon );
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