/***************************************************************************
 *                               WayPointManager.cs
 *
 *   begin                : 02 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Commands;
using Server.Items;
using Server.Targeting;

namespace Midgard.Commands
{
    public class WayPointManager
    {
        public static void Initialize()
        {
            CommandSystem.Register( "LoadWayPointPath", AccessLevel.Counselor, new CommandEventHandler( LoadWayPointPath_OnCommand ) );
            CommandSystem.Register( "SaveWayPointPath", AccessLevel.Counselor, new CommandEventHandler( SaveWayPointPath_OnCommand ) );
            CommandSystem.Register( "TourWayPointPath", AccessLevel.Counselor, new CommandEventHandler( TourWayPointPath_OnCommand ) );
        }

        [Usage( "LoadWayPointPath <fileName>" )]
        [Description( "Places and links waypoints eachother from a given filename." )]
        public static void LoadWayPointPath_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                string arg = e.GetString( 0 );
                string file = Path.Combine( m_PathsRoot, arg );
                List<Point3D> path = new List<Point3D>();

                if( File.Exists( file ) )
                {
                    path = ParseListOfPoints( file );

                    if( path.Count > 0 )
                    {
                        GeneratePath( path, from.Map );
                        from.SendMessage( "Path completed: {0} valid points found.", path.Count );
                    }
                    else
                    {
                        from.SendMessage( "Warning: No valid point found" );
                    }
                }
                else
                {
                    from.SendMessage( "Warning: file name {0} does not exist.", arg );
                }
            }
            else
            {
                from.SendMessage( "Command use: LoadWayPointPath <fileName>" );
            }
        }

        [Usage( "SaveWayPointPath <fileName>" )]
        [Description( "Saves a waypoints path to a given filename." )]
        public static void SaveWayPointPath_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 1 )
            {
                from.SendMessage( "Choose a way point chain you want to save." );
                from.Target = new SaveTarget( e.GetString( 0 ) );
            }
            else
            {
                from.SendMessage( "Command use: SaveWayPointPath <fileName>" );
            }
        }

        [Usage( "TourWayPointPath" )]
        [Description( "Tour the command user through a way point path." )]
        public static void TourWayPointPath_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 0 )
            {
                from.SendMessage( "Choose a way point chain you want to tour." );
                from.Target = new TourTarget();
            }
            else
            {
                from.SendMessage( "Command use: TourWayPointPath" );
            }
        }

        private static string m_PathsRoot = Path.Combine( "Data", "Paths" );

        private static List<Point3D> ParseListOfPoints( string file )
        {
            List<Point3D> path = new List<Point3D>();

            using( StreamReader ip = new StreamReader( file ) )
            {
                string line;

                while( ( line = ip.ReadLine() ) != null )
                {
                    if( line.Length == 0 || line.StartsWith( "#" ) )
                        continue;

                    try
                    {
                        path.Add( Point3D.Parse( line ) );
                    }
                    catch
                    {
                        Console.WriteLine( "Warning: Invalid Point3D entry:" );
                        Console.WriteLine( line );
                    }
                }
            }

            return path;
        }

        private static void GeneratePath( IList<Point3D> path, Map map )
        {
            List<WayPoint> wayPoints = new List<WayPoint>();
            for( int i = 0; i < path.Count; i++ )
            {
                WayPoint point = new WayPoint();
                point.MoveToWorld( path[ i ], map );
                wayPoints.Add( point );
            }

            for( int i = 0; i < wayPoints.Count - 1; i++ )
                wayPoints[ i ].NextPoint = wayPoints[ i + 1 ];

            // manually set the last to close the path
            wayPoints[ wayPoints.Count - 1 ].NextPoint = wayPoints[ 0 ];
        }

        private static List<WayPoint> GetPath( Mobile from, WayPoint start )
        {
            List<WayPoint> list = new List<WayPoint>();

            WayPoint current = start;
            int sanity = 0;

            do
            {
                list.Add( current );

                if( current.NextPoint == start || current.NextPoint == null )
                    break;
                else
                    current = current.NextPoint;

                sanity++;
                if( sanity >= 100 )
                {
                    from.SendMessage( "An error occurred. Contact Dies." );
                    return null;
                }

            } while( true );

            WayPoint last = list[ list.Count - 1 ];

            if( last.NextPoint == null || last.NextPoint != start )
            {
                from.SendMessage( "Your chain of waypoint is inconsistent. Last waypoint must be linked to first one." );
                return null;
            }
            else
            {
                from.SendMessage( "You specified a valid chain link of waypoints ({0} locations added).", list.Count );
                return list;
            }
        }

        private class SaveTarget : Target
        {
            private string m_FileName;

            public SaveTarget( string fileName )
                : base( 16, true, TargetFlags.None )
            {
                m_FileName = fileName;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( targeted is WayPoint ) )
                    return;

                ScriptCompiler.EnsureDirectory( m_PathsRoot );

                string fileName = Path.Combine( m_PathsRoot, string.Format( "{0}.txt", m_FileName ) );

                using( var writer = new StreamWriter( fileName, true ) )
                {
                    var list = GetPath( from, (WayPoint)targeted );
                    foreach( var wayPoint in list )
                        writer.WriteLine( wayPoint.Location );
                }

                from.SendMessage( "Path exported to: {0}.", fileName );
            }
        }

        private class TourTarget : Target
        {
            public TourTarget()
                : base( 16, true, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                WayPoint wayPoint = targeted as WayPoint;

                if( wayPoint != null )
                {
                    from.MoveToWorld( wayPoint.Location, wayPoint.Map );
                    new TourTimer( from, wayPoint ).Start();
                    from.SendMessage( "The tour started..." );
                }
            }
        }

        private class TourTimer : Timer
        {
            private readonly Mobile m_From;
            private WayPoint m_First;
            private WayPoint m_Current;

            public TourTimer( Mobile from, WayPoint first )
                : base( TimeSpan.FromSeconds( 3.0 ), TimeSpan.FromSeconds( 3.0 ) )
            {
                m_From = from;
                m_First = first;
                m_Current = first;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if( m_Current.NextPoint == null )
                {
                    m_From.SendMessage( "This waypoint has no next one. The tour ended." );
                    Stop();
                    return;
                }

                m_Current = m_Current.NextPoint;
                m_From.MoveToWorld( m_Current.Location, m_Current.Map );

                if( m_Current == m_First )
                {
                    m_From.MoveToWorld( m_First.Location, m_First.Map );
                    m_From.SendMessage( "The tour ended." );
                    Stop();
                }
            }
        }
    }
}