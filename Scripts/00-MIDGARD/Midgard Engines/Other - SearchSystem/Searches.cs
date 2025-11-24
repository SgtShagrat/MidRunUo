using System;
using Server;

namespace Midgard.Engines.Searches
{
    //------------------------------------------------------------------------------
    //  Geometries (API).
    //------------------------------------------------------------------------------
    public class Search
    {
        //------------------------------------------------------------------------------
        //  SearchOctant -- search all cells in an octant, from start range, going Inwards
        //                 or going Outwards. Search will terminate if the Search delegate
        //                 returns false any time during the tour
        //------------------------------------------------------------------------------
        public static bool Octant( Map map, Direction direction, int start, SearchDirection searchDirection, Tour tour )
        {
            Compass compass = m_CompassLookup[ (int)direction ];

            return m_OctantSearch.SearchOctant( map, compass, start, searchDirection, tour );
        }

        //------------------------------------------------------------------------------
        //  SearchLine   -- search all cells on a line, from begin to end. Search will
        //                 terminate if the Search delegate returns false any time during
        //                 the tour
        //------------------------------------------------------------------------------
        public static bool Line( Map map, Point2D begin, Point2D end, Tour tour )
        {
            return m_LineSearch.SearchLine( map, begin, end, tour );
        }

        //------------------------------------------------------------------------------
        //  SearchCircle -- search all cells in a circle, around the center
        //------------------------------------------------------------------------------
        public static bool Circle( Map map, int radius, Tour tour )
        {
            return m_CircleSearch.SearchCircle( map, radius, tour );
        }

        public static bool Circle( Map map, int radius, ClockDirection clockDirection, Tour tour )
        {
            return m_CircleSearch.SearchCircle( map, radius, clockDirection, tour );
        }

        //------------------------------------------------------------------------------
        //  SearchConcentric -- search all cells in a circle, around the center
        //------------------------------------------------------------------------------
        public static bool ConcentricCircles( Map map, int start, SearchDirection searchDirection, Tour tour )
        {
            return m_CircleSearch.SearchConcentricCircles( map, start, searchDirection, tour );
        }

        //------------------------------------------------------------------------------
        //  SearchSpiral    --  search a square mosaic spiral, going in or out (allows
        //                     a random placement within the first search ring)
        //------------------------------------------------------------------------------
        public static bool Spiral( Map map, int start, SearchDirection direction, bool randomStart, Tour tour )
        {
            return m_SpiralSearch.SearchSpiral( map, start, direction, randomStart, tour );
        }

        //------------------------------------------------------------------------------
        //  FROM HERE DOWN NOT PART OF API (IMPLEMENTATION)
        //------------------------------------------------------------------------------
        public static void Initialize()
        {
            try
            {
                m_OctantSearch = new OctantSearch( 24 );
                m_CircleSearch = new CircleSearch( 24 );
                m_SpiralSearch = new SpiralSearch();
                m_LineSearch = new LineSearch();
            }
            catch( Exception e )
            {
                Console.WriteLine( "Searches: ***INITIALIZATION FAILED!!!***" );
                Console.WriteLine( e );
            }
        }

        //------------------------------------------------------------------------------
        //  For testing
        //------------------------------------------------------------------------------
        //    public static void Main( string[] args )
        //    {
        //        Initialize();
        //
        //        try
        //        {
        //            //m_OctantSearch.DumpMatrix();
        //            //m_OctantSearch.DumpOctants();
        //            //m_CircleSearch.DumpMatrices();
        //            //m_CircleSearch.DumpSearchSets();
        //            //m_SpiralSearch.DumpSearchSets();
        //
        //            Search searchOctant = delegate( Map map, int x, int y )
        //            {
        //                //int distance = (int) Math.Sqrt ( x * x + y * y  );
        //                //Console.WriteLine( "{0},{1} --> {2}", x, y, distance );
        //                return true;
        //            };
        //
        //            Octant( 
        //                new Map(), 
        //                Direction.North,
        //                12,
        //                SearchDirection.Inwards,
        //                searchOctant
        //                );
        //
        //            Search searchLine = delegate( Map map, int x, int y )
        //            {
        //                //int distance = (int) Math.Sqrt ( x * x + y * y  );
        //                //Console.WriteLine( "{0},{1} --> {2}", x, y, distance );
        //                return true;
        //            };
        //
        //            Line( 
        //                new Map(), 
        //                new Point2D( -3, 7 ), 
        //                new Point2D( 7, -3 ), 
        //                searchLine
        //                );
        //
        //            Search searchCircle = delegate( Map map, int x, int y )
        //            {
        //                int distance = (int) Math.Sqrt ( x * x + y * y  );
        //                //Console.WriteLine( "{0},{1} --> {2}", x, y, distance );
        //                return true;
        //            };
        //
        //            Circle(
        //                new Map(),
        //                10, 
        //                searchCircle
        //                );
        //
        //            Search searchSpiral = delegate( Map map, int x, int y )
        //            {
        //                Console.Write( "{0}:{1}, ", x, y );
        //                return true;
        //            };
        //
        //            Spiral( 
        //                new Map(), 
        //                10,
        //                SearchDirection.Inwards,
        //                true,
        //                searchSpiral 
        //                );
        //        }
        //        catch( Exception e )
        //        {
        //            Console.WriteLine( e );
        //        }
        //    }
        //------------------------------------------------------------------------------
        //  Data
        //------------------------------------------------------------------------------

        private static OctantSearch m_OctantSearch;
        private static LineSearch m_LineSearch;
        private static CircleSearch m_CircleSearch;
        private static SpiralSearch m_SpiralSearch;

        private static Compass[] m_CompassLookup = {Compass.North, // North 0
                                                    Compass.NorthEast, // Right 1
                                                    Compass.East, // East  2
                                                    Compass.SouthEast, // Down  3
                                                    Compass.South, // South 4 
                                                    Compass.SouthWest, // Left  5
                                                    Compass.West, // West  6
                                                    Compass.NorthWest // Up    7
                                                   };
    }
}