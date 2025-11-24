using Server;
using Server.Commands;

namespace Midgard.Engines.FoodDecaySystem
{
    public class FoodGumpCommand
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "VerificaFame", AccessLevel.Player, new CommandEventHandler( FoodGump_OnCommand ) );
        }

        [Usage( "VerificaFame" )]
        [Description( "Apre un menu con i livelli di fame e sete." )]
        public static void FoodGump_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 0 )
                from.SendGump( new FoodGump( from ) );
        }
    }
}