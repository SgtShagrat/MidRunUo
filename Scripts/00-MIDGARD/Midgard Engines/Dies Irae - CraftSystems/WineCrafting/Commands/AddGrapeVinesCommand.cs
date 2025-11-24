using Server;
using Server.Commands;

namespace Midgard.Engines.WineCrafting
{
    public class AddGrapeVinesCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "AddGrapeVines", AccessLevel.Administrator, new CommandEventHandler( AddGrapeVines_OnCommand ) );
        }

        [Usage( "AddGrapeVines" )]
        [Description( "Add different varieties of grape vines for winecrafting." )]
        public static void AddGrapeVines_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new AddGrapeVineGump( e.Mobile, null, 0 ) );
        }
    }
}