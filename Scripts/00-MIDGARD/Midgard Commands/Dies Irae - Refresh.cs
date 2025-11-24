using Server;
using Server.Commands;

namespace Midgard.Commands
{
    public class Refresh
    {
        public static void Initialize()
        {
            CommandSystem.Register( "RefreshMe", AccessLevel.GameMaster, new CommandEventHandler( RefreshMe_OnCommand ) );
        }

        [Usage( "RefreshMe" )]
        [Description( "to refresh our status..." )]
        private static void RefreshMe_OnCommand( CommandEventArgs e )
        {
            Mobile m = e.Mobile;

            if( m.Poisoned )
                m.CurePoison( null );

            m.Hits = m.HitsMax;
            m.Stam = m.StamMax;
            m.Mana = m.ManaMax;

            m.Hunger = 20;
            m.Thirst = 20;

            m.SendMessage( "Thou're in peace." );
        }
    }
}