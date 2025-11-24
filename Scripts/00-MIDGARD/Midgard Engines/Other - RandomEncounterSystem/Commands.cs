using Server;
using Server.Commands;

namespace Midgard.Engines.RandomEncounterSystem
{
    public class RandomEncountersControl
    {
        public static void Initialize()
        {
            if( RandomEncounterEngine.Enabled )
                CommandSystem.Register( "RandEncSys", AccessLevel.Administrator, new CommandEventHandler( RandEncSys_OnCommand ) );
        }

        public static void RandEncSys_OnCommand( CommandEventArgs e )
        {
            if( e.Length >= 1 )
            {
                switch( e.Arguments[ 0 ] )
                {
                    case "import":

                        ImportHelper.Import( e.Mobile );
                        break;

                    case "init":

                        RandomEncounterEngine.Initialize();
                        break;

                    case "now":

                        if( e.Length != 2 )
                        {
                            e.Mobile.SendMessage( "usage: rand now [timertype]" );
                            return;
                        }

                        RandomEncounterEngine.GenerateEncounters( e.Arguments[ 1 ] );
                        break;

                    case "stop":

                        RandomEncounterEngine.Stop();
                        break;
                }
            }
        }
    }
}