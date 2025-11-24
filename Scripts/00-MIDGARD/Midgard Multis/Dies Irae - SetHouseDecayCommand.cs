using Server;
using Server.Commands;
using Server.Multis;

namespace Midgard.Commands
{
    public class SetHouseDecayCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "SetHouseDecay", AccessLevel.Administrator, new CommandEventHandler( SetHouseDecay_OnCommand ) );
        }

        [Usage( "SetHouseDecay <true | false>" )]
        [Description( "Enables or disables automatic shard house decay." )]
        public static void SetHouseDecay_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                BaseHouse.DecayEnabled = e.GetBoolean( 0 );
                e.Mobile.SendMessage( "House Decay have been {0}.", BaseHouse.DecayEnabled ? "enabled" : "disabled" );
            }
            else
            {
                e.Mobile.SendMessage( "Format: SetHouseDecay <true | false>" );
            }
        }
    }
}