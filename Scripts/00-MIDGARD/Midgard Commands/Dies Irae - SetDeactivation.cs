using System;
using Server;
using Server.Commands;

namespace Midgard.Commands
{
	public class SetDeactivation
	{
		public static double DefaultDeactivationDelay = 2.0; // default AI deactivation delay in minutes

		public static void Initialize()
		{
            CommandSystem.Register("SetDeactivation", AccessLevel.Developer, new CommandEventHandler(SetDeactivation_OnCommand));
		}

        [Usage( "SetDeactivation [minutes]" )]
        [Description( "Sets/reports the default AI deactivation delay for the PlayerRangeSensitive mod in minutes" )]
        public static void SetDeactivation_OnCommand( CommandEventArgs e )
        {
                if( e.Arguments.Length > 0 ){
                  try{
                    DefaultDeactivationDelay = double.Parse(e.Arguments[0]);
                  } catch{}
                }
                e.Mobile.SendMessage("Default AI deactivation delay set to {0} minutes",DefaultDeactivationDelay);
        }
    }
}
