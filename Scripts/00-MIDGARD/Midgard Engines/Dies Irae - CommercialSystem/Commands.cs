/***************************************************************************
 *                               Commands.cs
 *
 *   begin                : 27 aprile 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Commands;

namespace Midgard.Engines.CommercialSystem
{
    internal class Commands
    {
        internal static void RegisterCommands()
        {
            CommandSystem.Register( "CommercialSystem", AccessLevel.Developer, new CommandEventHandler( CommercialSystemCommand_OnCommand ) );
        }

        [Usage( "CommercialSystem <reset|enable|disable|report>" )]
        [Description( "Process a command for commercial system." )]
        public static void CommercialSystemCommand_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null || e.Length != 1 )
                return;

            string command = e.GetString( 0 );

            if( Insensitive.Compare( command, "reset" ) == 0 )
                Core.Instance.DoCommercialSystemReset( e.Mobile );
            else if( Insensitive.Compare( command, "enable" ) == 0 )
                Core.Instance.ToggleCommercialSystemStatus( e.Mobile, true );
            else if( Insensitive.Compare( command, "disable" ) == 0 )
                Core.Instance.ToggleCommercialSystemStatus( e.Mobile, false );
            else if( Insensitive.Compare( command, "report" ) == 0 )
                Core.Instance.DoCommercialSystemReport( e.Mobile );
        }
    }
}