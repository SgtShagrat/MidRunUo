using System;
using System.Collections.Generic;
using System.Xml;
using Server;
using Server.Engines.XmlPoints;

namespace Midgard.Regions
{
    public class WaterArenaRegion : ChallengeGameRegion
    {
        public WaterArenaRegion( XmlElement xml, Map map, Region parent )
            : base( xml, map, parent )
        {
        }

        private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

        public override void OnEnter( Mobile m )
        {
            CheckBreathTimer( m );

            base.OnEnter( m );
        }

        public void CheckBreathTimer( Mobile from )
        {
            if( from.Map != Map.Internal && !from.Deleted )
            {
                if( !m_Table.ContainsKey( from ) )
                {
                    Timer t = new BreathTimer( from );
                    t.Start();

                    m_Table[ from ] = t;
                }
            }
            else
            {
                if( m_Table.ContainsKey( from ) )
                {
                    Timer t = m_Table[ from ];

                    t.Stop();
                    m_Table.Remove( from );
                }
            }
        }

        private class BreathTimer : Timer
        {
            private Mobile m_Owner;

            public BreathTimer( Mobile owner )
                : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 5.0 ) )
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if( m_Owner.Deleted || m_Owner.Map == Map.Internal || !m_Owner.Region.IsPartOf( typeof( WaterArenaRegion ) ) )
                {
                    Stop();
                    m_Table.Remove( m_Owner );
                }
                else if( m_Owner.Alive )
                {
                    Effects.SendLocationEffect( new Point3D( m_Owner.X, m_Owner.Y, m_Owner.Z ), m_Owner.Map, 0x38c0, 30, 1 );
                    Interval = TimeSpan.FromSeconds( Utility.Dice( 1, 10, 5 ) );
                }
            }
        }
    }
}