/***************************************************************************
 *                               XmlBoatSiege.cs
 *
 *   begin                : 21 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.XmlSpawner2;
using Server.Multis;

namespace Midgard.Multis
{
    public class XmlBoatSiege : XmlSiege
    {
        public override int ShowSiegeColor { get { return 0; } } // color used to flag items with siege attachments

        public override int LightDamageColor { get { return 0; } } // color at 67-99% of hitsmax
        public override int MediumDamageColor { get { return 0; } } // color at 34-66% of hitsmax
        public override int HeavyDamageColor { get { return 0; } } // color at 1-33% of hitsmax

        public override int LightDamageEffectID { get { return 14732; } } // flame effect
        public override int MediumDamageEffectID { get { return 14732; } }
        public override int HeavyDamageEffectID { get { return 14732; } }

        [Attachable]
        public XmlBoatSiege()
            : this( 1000 )
        {
        }

        [Attachable]
        public XmlBoatSiege( int hitsmax )
            : this( hitsmax, 0, 0 )
        {
        }

        [Attachable]
        public XmlBoatSiege( int hitsmax, int resistfire, int resistphysical )
        {
            HitsMax = hitsmax;
            Hits = HitsMax;
            ResistPhysical = resistphysical;
            ResistFire = resistfire;
        }

        #region serialization
        public XmlBoatSiege( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public override void OnAttach()
        {
            base.OnAttach();

            if( !( AttachedTo is BaseBoat ) )
            {
                Console.WriteLine( "Warning: attaching XmlBoatSiege to a item not BaseBoat. Deleting..." );
                Delete();
            }
        }

        public override void OnDestroyed()
        {
            var baseBoat = AttachedTo as BaseBoat;

            if( baseBoat == null || baseBoat.Map == null || baseBoat.Map == Map.Internal )
                return;

            SinkTimer timer = new SinkTimer( baseBoat );
            timer.Start();
        }

        public void KillTheSailorsAndTeleport( BaseBoat baseBoat )
        {
            var mobilesOnBoat = baseBoat.GetMobilesOnBoat();

            foreach( var i in mobilesOnBoat )
                i.Kill();

            foreach( var mobile in mobilesOnBoat )
                TeleportTheDead( mobile );
        }

        private class SinkTimer : Timer
        {
            private readonly BaseBoat m_Boat;
            private int m_Count;

            public SinkTimer( BaseBoat boat )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 5.0 ) )
            {
                m_Boat = boat;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if( m_Count == 20 )
                {
                    m_Boat.Delete();
                    Stop();
                }
                else
                {
                    m_Boat.Location = new Point3D( m_Boat.X, m_Boat.Y, m_Boat.Z - 1 );

                    if( m_Boat.TillerMan != null && m_Count < 6 )
                        m_Boat.TillerMan.Say( 1007168 + m_Count );

                    ++m_Count;
                }
            }
        }

        #region teleport locations
        private static readonly Point2D[] m_Felucca = new Point2D[]
        {
            new Point2D( 2528, 3568 ), new Point2D( 2376, 3400 ), new Point2D( 2528, 3896 ),
            new Point2D( 2168, 3904 ), new Point2D( 1136, 3416 ), new Point2D( 1432, 3648 ),
            new Point2D( 1416, 4000 ), new Point2D( 4512, 3936 ), new Point2D( 4440, 3120 ),
            new Point2D( 4192, 3672 ), new Point2D( 4720, 3472 ), new Point2D( 3744, 2768 ),
            new Point2D( 3480, 2432 ), new Point2D( 3560, 2136 ), new Point2D( 3792, 2112 ),
            new Point2D( 2800, 2296 ), new Point2D( 2736, 2016 ), new Point2D( 4576, 1456 ),
            new Point2D( 4680, 1152 ), new Point2D( 4304, 1104 ), new Point2D( 4496, 984 ),
            new Point2D( 4248, 696 ), new Point2D( 4040, 616 ), new Point2D( 3896, 248 ),
            new Point2D( 4176, 384 ), new Point2D( 3672, 1104 ), new Point2D( 3520, 1152 ),
            new Point2D( 3720, 1360 ), new Point2D( 2184, 2152 ), new Point2D( 1952, 2088 ),
            new Point2D( 2056, 1936 ), new Point2D( 1720, 1992 ), new Point2D( 472, 2064 ),
            new Point2D( 656, 2096 ), new Point2D( 3008, 3592 ), new Point2D( 2784, 3472 ),
            new Point2D( 5456, 2400 ), new Point2D( 5976, 2424 ), new Point2D( 5328, 3112 ),
            new Point2D( 5792, 3152 ), new Point2D( 2120, 3616 ), new Point2D( 2136, 3128 ),
            new Point2D( 1632, 3528 ), new Point2D( 1328, 3160 ), new Point2D( 1072, 3136 ),
            new Point2D( 1128, 2976 ), new Point2D( 960, 2576 ), new Point2D( 752, 1832 ),
            new Point2D( 184, 1488 ), new Point2D( 592, 1440 ), new Point2D( 368, 1216 ),
            new Point2D( 232, 752 ), new Point2D( 696, 744 ), new Point2D( 304, 1000 ),
            new Point2D( 840, 376 ), new Point2D( 1192, 624 ), new Point2D( 1200, 192 ),
            new Point2D( 1512, 240 ), new Point2D( 1336, 456 ), new Point2D( 1536, 648 ),
            new Point2D( 1104, 952 ), new Point2D( 1864, 264 ), new Point2D( 2136, 200 ),
            new Point2D( 2160, 528 ), new Point2D( 1904, 512 ), new Point2D( 2240, 784 ),
            new Point2D( 2536, 776 ), new Point2D( 2488, 216 ), new Point2D( 2336, 72 ),
            new Point2D( 2648, 288 ), new Point2D( 2680, 576 ), new Point2D( 2896, 88 ),
            new Point2D( 2840, 344 ), new Point2D( 3136, 72 ), new Point2D( 2968, 520 ),
            new Point2D( 3192, 328 ), new Point2D( 3448, 208 ), new Point2D( 3432, 608 ),
            new Point2D( 3184, 752 ), new Point2D( 2800, 704 ), new Point2D( 2768, 1016 ),
            new Point2D( 2448, 1232 ), new Point2D( 2272, 920 ), new Point2D( 2072, 1080 ),
            new Point2D( 2048, 1264 ), new Point2D( 1808, 1528 ), new Point2D( 1496, 1880 ),
            new Point2D( 1656, 2168 ), new Point2D( 2096, 2320 ), new Point2D( 1816, 2528 ),
            new Point2D( 1840, 2640 ), new Point2D( 1928, 2952 ), new Point2D( 2120, 2712 )
        };
        #endregion

        private static bool IsStranded( IEntity from )
        {
            Map map = from.Map;

            if( map == null )
                return false;

            object surface = map.GetTopSurface( from.Location );

            if( surface is Tile )
            {
                int id = ( (Tile)surface ).ID;

                return ( id >= 168 && id <= 171 )
                || ( id >= 310 && id <= 311 )
                || ( id >= 0x5796 && id <= 0x57B2 );
            }

            return false;
        }

        private static void TeleportTheDead( Mobile from )
        {
            if( !IsStranded( from ) )
                return;

            Map map = from.Map;

            Point2D[] list;

            if( map == Map.Felucca )
                list = m_Felucca;
            else
                return;

            Point2D p = Point2D.Zero;
            double pdist = double.MaxValue;

            for( int i = 0; i < list.Length; ++i )
            {
                double dist = from.GetDistanceToSqrt( list[ i ] );

                if( dist < pdist )
                {
                    p = list[ i ];
                    pdist = dist;
                }
            }

            int x = p.X, y = p.Y;
            int z;
            bool canFit = false;

            z = map.GetAverageZ( x, y );
            canFit = map.CanSpawnMobile( x, y, z );

            for( int i = 1; !canFit && i <= 40; i += 2 )
            {
                for( int xo = -1; !canFit && xo <= 1; ++xo )
                {
                    for( int yo = -1; !canFit && yo <= 1; ++yo )
                    {
                        if( xo == 0 && yo == 0 )
                            continue;

                        x = p.X + ( xo * i );
                        y = p.Y + ( yo * i );
                        z = map.GetAverageZ( x, y );
                        canFit = map.CanSpawnMobile( x, y, z );
                    }
                }
            }

            if( canFit )
                from.Location = new Point3D( x, y, z );
        }
    }
}