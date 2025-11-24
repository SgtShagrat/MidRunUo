using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Regions;

namespace Midgard.Engines.GroupsHandler
{
    public class FindDecorations
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "FindDecorations", AccessLevel.Administrator, new CommandEventHandler( FindDecorations_OnCommand ) );
        }

        [Usage( "FindDecorations" )]
        [Description( "Find orphan items in the world" )]
        public static void FindDecorations_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            int added = 0;

            ItemsGroup g = GroupsHandler.AddGroup( "orphans", "Non movable forgotten items in the world", true );
            foreach( Item item in World.Items.Values )
            {
                if( item.Movable || item is Plank || item is Hold || item is TillerMan || item.GetType().Name.Equals( "Ornament" ) ||
                    item is FarmableCrop || item.RootParent != null || item.Map == Map.Internal ||
                    ( Region.Find( item.Location, item.Map ) is HouseRegion ) || IsGreenAcres( item.Map, item.Location ) ||
                    ( item is BaseTrap && IsDoomGauntlet( item.Map, item.Location ) ) || StealableArtifactsSpawner.GetStealableInstance( item ) != null ||
                    GroupsHandler.InGroup( item ) )
                    continue;

                if( g.AddItem( item, true ) )
                    added++;
            }

            from.SendMessage( String.Format( "Done. {0} item{1} were found.", added, added == 1 ? "" : "s" ) );
        }

        private static bool IsGreenAcres( Map map, IPoint2D loc )
        {
            int x = loc.X, y = loc.Y;

            bool r1 = x >= 5888 && y >= 512 && x <= 6143 && y <= 1023;
            bool r2 = x >= 5376 && y >= 1024 && x <= 6143 && y <= 1279;
            bool r3 = x >= 5376 && y >= 1536 && x <= 6143 && y <= 1775;
            bool r4 = x >= 5935 && y >= 1776 && x <= 6143 && y <= 2047;
            bool r5 = x >= 5120 && y >= 2048 && x <= 6143 && y <= 2303;
            bool r6 = x >= 5632 && y >= 2039 && x <= 6143 && y <= 2047;
            bool r7 = x >= 5632 && y >= 1776 && x <= 5640 && y <= 2038;
            bool r8 = x >= 5575 && y >= 1776 && x <= 5631 && y <= 1792;
            bool r9 = x >= 5271 && y >= 1159 && x <= 5311 && y <= 1192;
            bool r0 = x >= 5120 && y >= 256 && x <= 5375 && y <= 515;

            return ( r1 || r2 || r3 || r4 || r5 || r6 || r7 || r8 || r9 || r0 ) && ( map == Map.Felucca || map == Map.Trammel );
        }

        private static bool IsDoomGauntlet( Map map, IPoint2D loc )
        {
            if( map != Map.Malas )
                return false;

            int x = loc.X - 256, y = loc.Y - 304;

            return ( x >= 0 && y >= 0 && x < 256 && y < 256 );
        }
    }
}