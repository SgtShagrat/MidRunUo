using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Server;
using Server.Commands;

namespace Midgard.Commands
{
    /// <summary>
    /// this command searches the items in the world and checks to see if a duplicate
    /// copy exists on the static map.  the purpose of this is to overcome the duplication
    /// of items and statics when the statics map is edited on a separate server from
    /// the production server.  This command is intended to be run after the production 
    /// statics have been changed, and an updated patch is available for distribution to
    /// all users.
    /// </summary>
    public class StaticsClean
    {
        public static void Initialize()
        {
            CommandSystem.Register( "StaticsClean", AccessLevel.Developer, new CommandEventHandler( StaticsClean_OnCommand ) );
        }

        [Usage( "StaticsClean " )]
        [Description( "Searches the items in the world and checks to see if a duplicate copy exists on the static map." +
            "The purpose of this is to overcome the duplication of items and statics when the statics map is edited on a separate server from the production server." +
            "This command is intended to be run after the production statics have been changed, and an updated patch is available for distribution to all users." )]
        private static void StaticsClean_OnCommand( CommandEventArgs e )
        {
            Console.Write( "Verifing statics duplicates..." );

            Server.Network.NetState.FlushAll();
            Server.Network.NetState.Pause();

            Stopwatch watch = Stopwatch.StartNew();

            int count = CleanStatics();

            watch.Stop();

            Console.WriteLine( "done ({0:F2} seconds).", watch.Elapsed.TotalSeconds );
            Console.WriteLine( "Detected {0} invalid items, removing..", count );

            Server.Network.NetState.Resume();

            e.Mobile.SendMessage( "Object table has been generated. See the file : <runuo root>/statics-clean.log" );
        }

        private static int CleanStatics()
        {
            List<Item> duplicateItems = new List<Item>();

            foreach( Item item in World.Items.Values )
            {
                Map map = item.Map;

                Tile[] tiles = map.Tiles.GetStaticTiles( item.X, item.Y, false );
                foreach( Tile tile in tiles )
                {
                    //this check looks statics that are in the same place,and same ItemID as the items.
                    if( ( tile.ID & 0x3FFF ) == item.ItemID && tile.Z == item.Z && !item.Movable )
                    {
                        duplicateItems.Add( item );
                        break;
                    }
                }
            }

            int count = duplicateItems.Count;

            using( StreamWriter op = new StreamWriter( "Logs/statics-clean.log" ) )
            {
                op.WriteLine( "# Statics clean list generated on {0}", DateTime.Now );
                op.WriteLine();

                op.WriteLine( "# Items:" );

                for( int i = 0; i < count; i++ )
                {
                    op.WriteLine( "\tItemID: {0} Location {1}", duplicateItems[ i ].ItemID, duplicateItems[ i ].Location );
                    duplicateItems[ i ].Delete();
                }
            }

            return count;
        }
    }
}