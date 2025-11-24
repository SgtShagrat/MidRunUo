using System;
using Server.Items;
using Server.Multis;

namespace Server.Mobiles
{
    public class FlyingAI
    {
        public static void CheckMethods( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            CheckMap( fbc );
            CheckLandTile( fbc );
            CheckStaticLandTile( fbc );
            CheckMapEdge( fbc );
            CheckStatic( fbc );
            CheckHouse( fbc );
            CheckDeepWater( fbc );

            FlyingRange( fbc );
        }

        public static void AnimateFlying( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            fbc.PlaySound( 0x2D0 );
            fbc.Animate( 24, 5, 1, true, false, 0 );
        }

        public static void CheckMap( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.Map == Map.Felucca )
            {
                fbc.LeftSide = 5;
                fbc.RightSide = 5100;
                fbc.TopSide = 5;
                fbc.BottomSide = 4090;
            }

            if( fbc.Map == Map.Trammel )
            {
                fbc.LeftSide = 5;
                fbc.RightSide = 5100;
                fbc.TopSide = 5;
                fbc.BottomSide = 4090;
            }

            if( fbc.Map == Map.Ilshenar )
            {
                fbc.IsFlying = false;
                fbc.IsTakingOff = false;
                return;
            }

            if( fbc.Map == Map.Malas )
            {
                fbc.LeftSide = 515;
                fbc.RightSide = 2555;
                fbc.TopSide = 5;
                fbc.BottomSide = 2045;
            }

            if( fbc.Map == Map.Tokuno )
            {
                fbc.LeftSide = 5;
                fbc.RightSide = 1445;
                fbc.TopSide = 5;
                fbc.BottomSide = 1445;
            }
        }

        public static void RunFly( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            fbc.Direction |= Direction.Running;
        }

        public static void FlyingRange( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            int hx = fbc.Home.X;
            int hy = fbc.Home.Y;
            int hz = fbc.Home.Z;

            if( ( hx + hy + hz ) == 0 )
                return;

            int mx = fbc.X;
            int my = fbc.Y;
            int mz = fbc.Z;

            int range = fbc.RangeHome;

            if( hx > mx )
            {
                if( ( hx - mx ) >= range )
                    ChangeDirection( fbc );
            }
            if( hx < mx )
            {
                if( ( mx - hx ) >= range )
                    ChangeDirection( fbc );
            }
            if( hy > my )
            {
                if( ( hy - my ) >= range )
                    ChangeDirection( fbc );
            }
            if( hy < my )
            {
                if( ( my - hy ) >= range )
                    ChangeDirection( fbc );
            }
            if( hz > mz )
            {
                if( ( hz - mz ) >= range )
                {
                    if( fbc.FlyingDown )
                    {
                        fbc.FlyingDown = false;
                        fbc.FlyingUp = true;
                        return;
                    }
                    fbc.FlyingUp = true;
                    return;
                }
            }
            if( hz < mz )
            {
                if( ( mz - hz ) >= range )
                {
                    if( fbc.FlyingUp )
                    {
                        fbc.FlyingUp = false;
                        fbc.FlyingDown = true;
                        return;
                    }
                    fbc.FlyingDown = true;
                    return;
                }
            }
        }

        public static void TakeOff( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.Ground == fbc.Z )
            {
                fbc.FlyingUp = true;
                RandomChangeDirection( fbc );
                MaskedDirection( fbc );
                AnimateFlying( fbc );
                fbc.Z++;
                return;
            }

            if( fbc.Ground < fbc.Z )
                fbc.Z++;

            if( fbc.Z > ( fbc.Ground + 20 ) )
            {
                fbc.IsTakingOff = false;
            }
        }

        public static void Flying( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( CheckAbove( fbc ) )
            {
                fbc.IsFlying = false;
                fbc.IsTakingOff = false;
                return;
            }

            Timer f_timer = new FlyingTimer( fbc );

            CheckMethods( fbc );

            if( fbc.CanFly == false )
                return;

            if( fbc.FlyStam <= 50 )
            {
                fbc.FlyingUp = false;
                fbc.IsLanding = true;
                fbc.FlyingDown = true;
            }

            if( ( fbc.Z + 20 ) >= fbc.Ceiling )
            {
                fbc.FlyingUp = false;
                fbc.FlyingDown = true;
                fbc.Z--;
            }

            if( fbc.FlyingUp )
                fbc.Z++;
            if( fbc.FlyingDown )
                fbc.Z--;

            CheckMethods( fbc );

            int direct = Utility.Random( 1, 20 );
            int altitude = Utility.Random( 1, 3 );

            if( fbc.IsTakingOff )
            {
                TakeOff( fbc );

                if( direct == 7 )
                    RandomChangeDirection( fbc );
                MaskedDirection( fbc );

                CheckMethods( fbc );

                f_timer.Start();

                if( fbc.FlyAnim == 2 )
                    AnimateFlying( fbc );

                if( fbc.FlyStam > 10 )
                    fbc.FlyStam--;
                return;
            }

            if( fbc.IsLanding )
            {
                Landing( fbc );

                if( fbc.IsLanding == false )
                    return;

                if( direct == 7 )
                    RandomChangeDirection( fbc );
                MaskedDirection( fbc );

                CheckMethods( fbc );

                f_timer.Start();

                if( fbc.FlyAnim == 2 )
                    AnimateFlying( fbc );
                return;
            }

            if( ( fbc.Z - 20 ) <= fbc.Ground )
            {
                fbc.FlyingUp = true;
                fbc.FlyingDown = false;
                fbc.Z++;
            }

            if( direct == 7 )
                RandomChangeDirection( fbc );
            if( altitude == 2 )
                RandomAltitudeDirection( fbc );
            MaskedDirection( fbc );

            CheckMethods( fbc );

            f_timer.Start();

            if( fbc.FlyAnim == 2 )
                AnimateFlying( fbc );
        }

        public static void Landing( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.Ground > ( fbc.Z - 1 ) )
            {
                fbc.Z = fbc.Ground + 1;
                fbc.IsLanding = false;
                fbc.IsFlying = false;
                fbc.FlyingUp = false;
                fbc.FlyingDown = false;
                return;
            }
            if( fbc.Ground == ( fbc.Z - 1 ) )
            {
                fbc.IsLanding = false;
                fbc.IsFlying = false;
                fbc.FlyingUp = false;
                fbc.FlyingDown = false;
                CheckStaticLanding( fbc );
                return;
            }
            if( fbc.Z > ( fbc.Ground + 1 ) )
                fbc.Z--;
        }

        public static void CheckDeepWater( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            bool deepWater = SpecialFishingNet.FullValidation( fbc.Map, fbc.Location.X, fbc.Location.Y );

            fbc.DebugSay( "Checking deep water: my location is {0}deep.", deepWater ? "" : "not " );

            if( deepWater )
                ChangeDirection( fbc );
        }

        public static void CheckStatic( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            const int a = -1;
            const int b = 1;
            const int c = 0;

            int xx = 0;
            int yy = 0;

            if( fbc.CDirection == 1 )
            {
                xx = c;
                yy = a;
            }

            if( fbc.CDirection == 2 )
            {
                xx = b;
                yy = a;
            }

            if( fbc.CDirection == 3 )
            {
                xx = b;
                yy = c;
            }

            if( fbc.CDirection == 4 )
            {
                xx = b;
                yy = b;
            }

            if( fbc.CDirection == 5 )
            {
                xx = c;
                yy = b;
            }

            if( fbc.CDirection == 6 )
            {
                xx = a;
                yy = b;
            }

            if( fbc.CDirection == 7 )
            {
                xx = a;
                yy = c;
            }

            if( fbc.CDirection == 8 )
            {
                xx = a;
                yy = a;
            }

            Tile[] tiles = fbc.Map.Tiles.GetStaticTiles( ( fbc.X + xx ), ( fbc.Y + yy ), true );

            if( tiles == null )
                return;

            for( int i = 0; i < tiles.Length; ++i )
            {
                Tile tile = tiles[ i ];

                if( fbc.Z <= tile.Z )
                {
                    fbc.DebugSay( "Checking static tiles: i have to change my direction" );
                    ChangeDirection( fbc );
                }
            }
        }

        public static void CheckHouse( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            bool house = BaseHouse.FindHouseAt( fbc ) != null;

            fbc.DebugSay( "Checking house: my location is {0}in a house.", house ? "" : "not " );

            if( house )
                ChangeDirection( fbc );
        }

        public static void CheckLandTile( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            Tile landTile = fbc.Map.Tiles.GetLandTile( fbc.X, fbc.Y );

            fbc.Ground = landTile.Z;
        }

        public static void CheckStaticLandTile( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            Tile[] tiles = fbc.Map.Tiles.GetStaticTiles( ( fbc.X ), ( fbc.Y ), true );

            if( tiles == null )
                return;

            for( int i = 0; i < tiles.Length; ++i )
            {
                Tile tile = tiles[ i ];

                if( tile.Z > fbc.Ground )
                    fbc.Ground = tile.Z;
            }
        }

        public static void CheckStaticLanding( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            Tile landTile = fbc.Map.Tiles.GetLandTile( fbc.X, fbc.Y );

            fbc.Ground = landTile.Z;

            Tile[] tiles = fbc.Map.Tiles.GetStaticTiles( ( fbc.X ), ( fbc.Y ), true );

            if( tiles == null )
                return;

            for( int i = 0; i < tiles.Length; ++i )
            {
                Tile tile = tiles[ i ];

                if( tile.Z <= fbc.Ground )
                    return;

                if( tile.Z == fbc.Z )
                    fbc.Z++;

                if( tile.Z == ( fbc.Z - 1 ) )
                {
                    fbc.FlyStam = fbc.FlyStamMax;
                }
            }
        }

        public static bool CheckAbove( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return true;

            Tile landTile = fbc.Map.Tiles.GetLandTile( fbc.X, fbc.Y );

            fbc.Ground = landTile.Z;

            if( ( fbc.Ground - 10 ) > fbc.Z )
                return true;

            Tile[] tiles = fbc.Map.Tiles.GetStaticTiles( ( fbc.X ), ( fbc.Y ), true );

            if( tiles == null )
                return false;

            for( int i = 0; i < tiles.Length; ++i )
            {
                Tile tile = tiles[ i ];

                if( ( tile.Z - 10 ) > fbc.Z )
                    return true;
            }
            return false;
        }

        public static void CheckMapEdge( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.X < fbc.LeftSide )
                ResetNoFly( fbc );
            if( fbc.X > fbc.RightSide )
                ResetNoFly( fbc );
            if( fbc.Y < fbc.TopSide )
                ResetNoFly( fbc );
            if( fbc.Y > fbc.BottomSide )
                ResetNoFly( fbc );

            if( fbc.X - 1 <= fbc.LeftSide )
                ChangeDirection( fbc );
            if( fbc.X + 1 >= fbc.RightSide )
                ChangeDirection( fbc );
            if( fbc.Y - 1 <= fbc.TopSide )
                ChangeDirection( fbc );
            if( fbc.Y + 1 >= fbc.BottomSide )
                ChangeDirection( fbc );
        }

        public static void ChangeDirection( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.CDirection == 1 )
            {
                fbc.Direction = Direction.South;
                fbc.CDirection = 5;
                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 2 )
            {
                fbc.Direction = Direction.Left;
                fbc.CDirection = 6;
                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 3 )
            {
                fbc.Direction = Direction.West;
                fbc.CDirection = 7;

                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 4 )
            {
                fbc.Direction = Direction.Up;
                fbc.CDirection = 8;
                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 5 )
            {
                fbc.Direction = Direction.North;
                fbc.CDirection = 1;
                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 6 )
            {
                fbc.Direction = Direction.Right;
                fbc.CDirection = 2;
                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 7 )
            {
                fbc.Direction = Direction.East;
                fbc.CDirection = 3;
                MaskedDirection( fbc );
                return;
            }
            if( fbc.CDirection == 8 )
            {
                fbc.Direction = Direction.Down;
                fbc.CDirection = 4;
                MaskedDirection( fbc );
                return;
            }
        }

        public static void RandomChangeDirection( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            fbc.CDirection = Utility.Random( 1, 8 );

            if( fbc.CDirection == 1 )
                fbc.Direction = Direction.North;
            if( fbc.CDirection == 2 )
                fbc.Direction = Direction.Right;
            if( fbc.CDirection == 3 )
                fbc.Direction = Direction.East;
            if( fbc.CDirection == 4 )
                fbc.Direction = Direction.Down;
            if( fbc.CDirection == 5 )
                fbc.Direction = Direction.South;
            if( fbc.CDirection == 6 )
                fbc.Direction = Direction.Left;
            if( fbc.CDirection == 7 )
                fbc.Direction = Direction.West;
            if( fbc.CDirection == 8 )
                fbc.Direction = Direction.Up;

            RunFly( fbc );
        }

        public static void MaskedDirection( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.CDirection == 1 )
                fbc.Direction = Direction.North;
            if( fbc.CDirection == 2 )
                fbc.Direction = Direction.Right;
            if( fbc.CDirection == 3 )
                fbc.Direction = Direction.East;
            if( fbc.CDirection == 4 )
                fbc.Direction = Direction.Down;
            if( fbc.CDirection == 5 )
                fbc.Direction = Direction.South;
            if( fbc.CDirection == 6 )
                fbc.Direction = Direction.Left;
            if( fbc.CDirection == 7 )
                fbc.Direction = Direction.West;
            if( fbc.CDirection == 8 )
                fbc.Direction = Direction.Up;

            RunFly( fbc );
        }

        public static void RandomAltitudeDirection( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            int updown = Utility.Random( 1, 10 );

            if( updown <= 1 )
            {
                if( fbc.Ground <= fbc.Z - 21 )
                    fbc.Z--;
                else
                    fbc.Z++;
            }
            if( updown >= 2 )
            {
                if( fbc.Ceiling >= fbc.Z + 21 )
                    fbc.Z++;
                else
                    fbc.Z--;
            }

            if( fbc.FlyStam > 10 )
                fbc.FlyStam--;
        }

        public static void ResetNoFly( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            if( fbc.CanFly )
                fbc.CanFly = false;
        }

        public static bool NullCheck( FlyingCreature fbc )
        {
            if( fbc == null )
                return true;
            else if( !fbc.Alive )
                return true;
            else if( fbc.Deleted )
                return true;
            else
                return false;
        }

        public static void SpawnShadow( FlyingCreature fbc )
        {
            if( NullCheck( fbc ) )
                return;

            int bld = 0;

            Map map = fbc.Map;

            if( map == null )
                return;

            if( fbc.CDirection == 1 )
                bld += 7425;
            if( fbc.CDirection == 2 )
                bld += 7418;
            if( fbc.CDirection == 3 )
                bld += 7411;
            if( fbc.CDirection == 4 )
                bld += 7414;
            if( fbc.CDirection == 5 )
                bld += 7416;
            if( fbc.CDirection == 6 )
                bld += 7428;
            if( fbc.CDirection == 7 )
                bld += 7421;
            if( fbc.CDirection == 8 )
                bld += 7422;
            if( bld == 0 )
                bld += 7428;

            FlyingShadow shadow = new FlyingShadow( bld );

            Point3D loc = fbc.Location;

            int x = fbc.X;
            int y = fbc.Y;
            int z = fbc.Ground + 1;

            loc = new Point3D( x, y, z );

            shadow.MoveToWorld( loc, map );

            if( shadow.Z > fbc.Z )
                fbc.Z = shadow.Z + 2;
        }

        private static double FlySpeed( BaseCreature m )
        {
            FlyingCreature fbc = m as FlyingCreature;

            double speed = 0.2;

            if( fbc != null )
            {
                speed = fbc.FlySpeed;

                if( speed <= 0 )
                    speed = 0.2;
            }

            return speed;
        }

        public class FlyingTimer : Timer
        {
            private Mobile m_Mobile;

            public FlyingTimer( BaseCreature m )
                : base( TimeSpan.FromSeconds( FlySpeed( m ) ) )
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                FlyingCreature fbc = m_Mobile as FlyingCreature;

                if( fbc == null )
                {
                    Stop();
                    return;
                }

                if( !fbc.Alive )
                {
                    Stop();
                    return;
                }

                if( fbc.Deleted )
                {
                    Stop();
                    return;
                }

                if( fbc.FlyAnim >= 2 )
                    fbc.FlyAnim = 0;
                fbc.FlyAnim++;

                if( fbc.IsFlying )
                {
                    if( fbc.CDirection == 1 )
                    {
                        RunFly( fbc );
                        fbc.Y--;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 2 )
                    {
                        RunFly( fbc );
                        fbc.X++;
                        fbc.Y--;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 3 )
                    {
                        RunFly( fbc );
                        fbc.X++;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 4 )
                    {
                        RunFly( fbc );
                        fbc.X++;
                        fbc.Y++;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 5 )
                    {
                        RunFly( fbc );
                        fbc.Y++;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 6 )
                    {
                        RunFly( fbc );
                        fbc.X--;
                        fbc.Y++;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 7 )
                    {
                        RunFly( fbc );
                        fbc.X--;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }

                    if( fbc.CDirection == 8 )
                    {
                        RunFly( fbc );
                        fbc.X--;
                        fbc.Y--;

                        if( fbc.FlyStam > 10 )
                            fbc.FlyStam--;
                        SpawnShadow( fbc );
                    }
                    Flying( fbc );
                    Stop();
                }
                Stop();
            }
        }
    }
}