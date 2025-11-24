using OrbServerSDK;

using Server;

using UOArchitectInterface;
using Midgard.Engines.OrbRemoteServer;

namespace Midgard.Engines.UOArchitect
{
    public class DeleteItems : OrbCommand
    {
        public static void Initialize()
        {
            OrbServer.Register( "UOAR_DeleteItems", typeof( DeleteItems ), AccessLevel.GameMaster, true );
        }

        public override void OnCommand( OrbClientInfo clientInfo, OrbCommandArgs cmdArgs )
        {
            if( cmdArgs == null || !( cmdArgs is DeleteCommandArgs ) )
                return;

            DeleteCommandArgs args = (DeleteCommandArgs)cmdArgs;

            if( args.Count > 0 )
            {
                int[] serials = args.ItemSerials;

                for( int i = 0; i < serials.Length; ++i )
                {
                    Item item = World.FindItem( serials[ i ] );

                    if( item == null || item.Deleted )
                        continue;

                    item.Delete();
                }
            }
        }
    }
}
