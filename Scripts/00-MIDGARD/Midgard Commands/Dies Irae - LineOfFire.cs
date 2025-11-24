using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Spells;

namespace Midgard.Commands
{
    public class LoFCommand : BaseCommand
    {
        public const int Version = 101; // Script version (do not change)
        public const int TileRange = 10; // Default range - overrideable in command args
        public static TimeSpan FlameDelay = TimeSpan.FromSeconds( 0.075 ); // Delay between each flame

        public static void Initialize()
        {
            TargetCommands.Register( new LoFCommand() ); // Registers the command to the server
        }

        public LoFCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple;
            Commands = new string[] { "LineOfFire", "LoF" };
            ObjectTypes = ObjectTypes.All;
            Usage = "LineOfFire <range>";
            Description = "Sends a line of fire from your character to a targeted location.";
        }

        public override void Execute( CommandEventArgs e, object obj )
        {
            // Find the range
            int range = TileRange; // Default is set in LoFMain; distro is 10

            if( e.Length > 0 ) // If there are args present...
                range = Utility.GetXMLInt32( e.Arguments[ 0 ], TileRange );

            // Try to parse the first arg into an int (if that doesn't work, revert to default)
            if( range < 1 ) // If the range is less than 1...
                range = 1; // Set it to 1 (don't want a nasty crash)

            // Find the location of the caster and the target location
            IPoint3D p = obj as IPoint3D;
            if( p == null )
                return;

            Mobile caster = e.Mobile;
            SpellHelper.GetSurfaceTop( ref p );
            Point3D fromLoc = caster.Location,
                    toLoc = new Point3D( p );

            // If the caster targets the tile he is standing on, abort
            if( fromLoc.X == toLoc.X && fromLoc.Y == toLoc.Y )
            {
                caster.SendMessage( "You cannot target yourself." );
                return;
            }

            // Obtain the list of tiles based on the caster's location, his target location, and the range specified
            List<Point3D> list = Tiles( fromLoc, toLoc, range );

            // Initiate a new LoFTimer for the list
            LoFTimer timer = new LoFTimer( caster, caster.Map, list );
            timer.Start();
        }

        // Find the tiles in a straight line from Point A to Point B
        public static List<Point3D> Tiles( Point3D a, Point3D b, int range )
        {
            List<Point3D> list = new List<Point3D>();

            int x1 = a.X,
                y1 = a.Y,
                x2 = b.X,
                y2 = b.Y,
                dx = Math.Abs( x2 - x1 ),
                dy = Math.Abs( y2 - y1 ),
                xInc = ( x2 < x1 ) ? -1 : 1,
                yInc = ( y2 < y1 ) ? -1 : 1;

            bool xLonger = ( dx >= dy );

            int dPr = ( xLonger ? dy : dx ) << 1,
                dPru = dPr - ( ( xLonger ? dx : dy ) << 1 ),
                p = dPr - ( xLonger ? dx : dy );

            for( int i = ( xLonger ? dx : dy ); i >= 0; i-- )
            {
                list.Add( new Point3D( x1, y1, 0 ) );

                if( p > 0 )
                {
                    x1 += xInc;
                    y1 += yInc;
                    p += dPru;
                }
                else
                {
                    if( xLonger )
                        x1 += xInc;
                    else
                        y1 += yInc;
                    p += dPr;
                }
            }

            return Straighten( list, range );
        }

        // Make sure the number of tiles in the list matches up to the range
        private static List<Point3D> Straighten( List<Point3D> list, int range )
        {
            if( list.Count > range ) // If there are more than # tiles...
                list.RemoveRange( range + 1, list.Count - ( range + 1 ) ); // Shorten the list
            else if( list.Count < range ) // Otherwise, if there are less than # tiles...
            {
                // Find the direction the tiles are going
                List<int[]> slopes = new List<int[]>();

                for( int i = 1; i < list.Count; i++ )
                    slopes.Add( new int[] { list[ i ].X - list[ i - 1 ].X, list[ i ].Y - list[ i - 1 ].Y } );

                // Repeat this loop for # of iterations equal to [range - list.Count]
                int index = 0;
                for( int i = list.Count - 1; i < range; i++ )
                {
                    // Add a tile to the list
                    list.Add( new Point3D( list[ i ].X + slopes[ index ][ 0 ], list[ i ].Y + slopes[ index ][ 1 ], 0 ) );

                    index++;
                    if( index == slopes.Count )
                        index = 0;
                }
            }

            return FixList( list );
        }

        // Fix the Z values of the tiles in the list, making sure they are on the surface of the ground
        private static List<Point3D> FixList( List<Point3D> list )
        {
            IPoint3D temp1;
            Point3D temp2;
            for( int i = 0; i < list.Count; i++ )
            {
                temp1 = list[ i ];
                SpellHelper.GetSurfaceTop( ref temp1 );
                temp2 = new Point3D( temp1 );
                list[ i ] = temp2;
            }

            return list;
        }

        private class LoFTimer : Timer
        {
            private Map Map; // This is a little self-explanatory...
            private List<Point3D> Tiles; // The list of tiles

            private int m_Count = 1;
            // The index for the list (starts out at 1; skips 0, the tile that the caster is standing on)

            public LoFTimer( Mobile caster, Map map, List<Point3D> tiles )
                : base( FlameDelay, FlameDelay )
            {
                Map = map;
                Tiles = tiles;
                Priority = TimerPriority.EveryTick;
            }

            protected override void OnTick()
            {
                // If the list is empty, abort
                if( Tiles.Count == 0 )
                {
                    Stop();
                    return;
                }

                // Display the flame effect
                Effects.SendLocationParticles( EffectItem.Create( Tiles[ m_Count ], Map, EffectItem.DefaultDuration ), 0x3709,
                                               10, 30, 5052 );
                Effects.PlaySound( Tiles[ m_Count ], Map, 0x208 );

                // Find any mobiles on the current tile
                IPooledEnumerable eable = Map.GetMobilesInRange( Tiles[ m_Count ], 0 );

                // For each mobile in the pool...
                try
                {
                    foreach( Mobile m in eable )
                    {
                        if( m == null || m.Deleted || m.Blessed )
                            continue;
                        // Dead targets are invalid.
                        if( !m.Alive || m.IsDeadBondedPet )
                            continue;
                        // Staff members cannot be targeted.
                        if( m.AccessLevel > AccessLevel.Player )
                            continue;

                        m.Kill(); // Die, bish
                    }
                }
                catch( Exception e ) { Console.WriteLine( e.Message ); }

                m_Count++; // Scroll to the next tile in the list

                // If the end of the list has been reached, abort
                if( m_Count == Tiles.Count )
                {
                    Stop();
                    return;
                }
            }
        }
    }
}