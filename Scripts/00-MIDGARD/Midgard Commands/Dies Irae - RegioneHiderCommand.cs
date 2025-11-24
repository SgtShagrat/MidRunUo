/***************************************************************************
 *                               RegioneHiderCommand.cs
 *
 *   begin                : 17 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Items;

namespace Midgard.Commands
{
    public class RegioneHiderCommand
    {
        private static int m_Count;

        public static void Initialize()
        {
            CommandSystem.Register( "RegionHide", AccessLevel.Administrator, new CommandEventHandler( RegionHide_OnCommand ) );
        }

        [Usage( "RegionHide <radiusOfDiscover>" )]
        [Description( "Convert selected region to morphed dark one." )]
        public static void RegionHide_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length > 1 )
            {
                from.SendMessage( "Usage: RegionHide <radiusOfDiscover>" );
                return;
            }

            int range = ( e.Length == 1 ) ? e.GetInt32( 1 ) : 2;

            from.SendMessage( "Choose the area thou wish to turn into dark." );
            BoundingBoxPicker.Begin( e.Mobile, new BoundingBoxCallback( StaExBox_Callback ), range );
        }

        private static void StaExBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            object[] states = (object[])state;
            int range = (int)states[ 0 ];

            TurnToDark( from, start.X, start.Y, end.X, end.Y, range );
        }

        private static void TurnToDark( Mobile from, int startX, int startY, int endX, int endY, int range )
        {
            int x1 = startX;
            int y1 = startY;
            int x2 = endX;
            int y2 = endY;

            if( startX > endX )
            {
                x1 = endX;
                x2 = startX;
            }

            if( startY < endY )
            {
                y1 = endY;
                y2 = startY;
            }

            List<Item> toRemove = new List<Item>();

            IPooledEnumerable eable = from.Map.GetItemsInBounds( new Rectangle2D( x1, y1, x2 - x1 + 1, y2 - y1 + 1 ) );
            foreach( Item item in eable )
            {
                CreateMorphItem( item.X, item.Y, item.Z, 0x1, item.ItemID, range );
                toRemove.Add( item );
            }

            eable.Free();

            for( int i = 0; i < toRemove.Count; i++ )
            {
                Item item = toRemove[ i ];
                if( item != null && !item.Deleted )
                    item.Delete();
            }

            from.SendMessage( String.Format( "{0} dynamic item{1} turned to dark.", m_Count, m_Count == 1 ? "" : "s" ) );
        }

        private static void CreateMorphItem( int x, int y, int z, int inactiveItemID, int activeItemID, int range )
        {
            if( FindMorphItem( x, y, z, inactiveItemID, activeItemID ) )
                return;

            MorphItem item = new MorphItem( inactiveItemID, activeItemID, range, 3 );

            item.MoveToWorld( new Point3D( x, y, z ), Map.Felucca );
            m_Count++;
        }

        private static bool FindMorphItem( int x, int y, int z, int inactiveItemID, int activeItemID )
        {
            IPooledEnumerable eable = Map.Felucca.GetItemsInRange( new Point3D( x, y, z ), 0 );

            foreach( Item item in eable )
            {
                if( item is MorphItem && item.Z == z && ( (MorphItem)item ).InactiveItemID == inactiveItemID && ( (MorphItem)item ).ActiveItemID == activeItemID )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }
    }
}