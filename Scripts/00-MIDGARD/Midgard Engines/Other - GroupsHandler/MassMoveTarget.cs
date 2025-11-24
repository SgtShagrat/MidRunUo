using System.Collections;
using Server;
using Server.Commands;
using Server.Targeting;

namespace Midgard.Engines.GroupsHandler
{
    public class MassMoveTarget : Target
    {
        private ArrayList m_List;
        private int m_Range;

        public MassMoveTarget( ArrayList list, int range )
            : base( -1, true, TargetFlags.None )
        {
            m_List = list;
            m_Range = range;
        }

        protected override void OnTarget( Mobile from, object o )
        {
            IPoint3D p = o as IPoint3D;

            if( p != null )
            {
                for( int i = 0; i < m_List.Count; )
                {
                    Item item = m_List[ i ] as Item;
                    if( item == null || item.Deleted || item.RootParent != null || item.GetType().DeclaringType != null ||
                        !GroupsHandler.IsAccessible( from, item ) )
                        m_List.RemoveAt( i );
                    else
                        i++;
                }

                if( p is Item )
                    p = ( (Item)p ).GetWorldTop();

                Point3D origin;
                if( m_List.Count == 0 || !GetCenterPoint( out origin ) )
                {
                    from.SendMessage(
                        "Unable to move objects. Either they do not belong to the same map, or they spread over a too large area, or you do not have access to them" );
                    return;
                }

                Point3D dest = new Point3D( p );
                int xdiff = dest.X - origin.X;
                int ydiff = dest.Y - origin.Y;
                int zdiff = dest.Z - origin.Z;

                foreach( Item item in m_List )
                {
                    Point3D newLoc = item.Location;
                    newLoc.X += xdiff;
                    newLoc.Y += ydiff;
                    newLoc.Z += zdiff;

                    CommandLogging.WriteLine( from, "{0} {1} moving {2} to {3}", from.AccessLevel, CommandLogging.Format( from ),
                                             CommandLogging.Format( item ), newLoc );
                    item.MoveToWorld( newLoc, from.Map );
                }

                from.SendMessage( "Done. {0} objects moved.", m_List.Count );
            }
        }

        private bool GetCenterPoint( out Point3D origin )
        {
            Item item = m_List[ 0 ] as Item;

            Map map = item.Map;
            Point3D loc = item.Location;
            int xmin = loc.X, xmax = loc.X;
            int ymin = loc.Y, ymax = loc.Y;
            int zmin = loc.Z;

            origin = loc;

            for( int i = 1; i < m_List.Count; i++ )
            {
                item = m_List[ i ] as Item;

                if( item.Map != map )
                    return false;

                loc = item.Location;

                if( loc.X < xmin )
                    xmin = loc.X;
                else if( loc.X > xmax )
                    xmax = loc.X;

                if( loc.Y < ymin )
                    ymin = loc.Y;
                else if( loc.Y > ymax )
                    ymax = loc.Y;

                if( loc.Z < zmin )
                    zmin = loc.Z;
            }

            if( xmax - xmin > 2 * m_Range || ymax - ymin > 2 * m_Range )
                return false;

            origin = new Point3D( ( xmin + xmax ) / 2, ( ymin + ymax ) / 2, zmin );
            return true;
        }
    }
}