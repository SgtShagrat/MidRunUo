/***************************************************************************
 *                               BleedHelper.cs
 *
 *   begin                : 10 agosto 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;

namespace Midgard
{
    public class BleedHelper
    {
        private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

        public static bool IsBleeding( Mobile m )
        {
            return m_Table.ContainsKey( m );
        }

        public static void BeginBleed( Mobile m, Mobile from, double delay, int count )
        {
            Timer t = m_Table[ m ];
            if( t != null )
                t.Stop();

            t = new InternalTimer( from, m, delay, count );
            m_Table[ m ] = t;

            t.Start();
        }

        public static void DoBleed( Mobile m, Mobile from, int level )
        {
            if( m.Alive )
            {
                int damage = Utility.RandomMinMax( level, level * 2 );
                if( !m.Player )
                    damage *= 2;

                m.PlaySound( 0x133 );
                m.Damage( damage, from );

                Blood blood = new Blood( Utility.Random( 0x122A, 5 ) );
                blood.MoveToWorld( m.Location, m.Map );
            }
            else
                EndBleed( m, false );
        }

        public static void EndBleed( Mobile m, bool message )
        {
            Timer t = m_Table[ m ];
            if( t == null )
                return;

            t.Stop();
            m_Table.Remove( m );

            if( message )
                m.SendLocalizedMessage( 1060167 ); // The bleeding wounds have healed, you are no longer bleeding!
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Mobile;
            private readonly int m_MaxCount;
            private int m_Count;

            public InternalTimer( Mobile from, Mobile m, double delay, int maxCount )
                : base( TimeSpan.FromSeconds( delay ), TimeSpan.FromSeconds( delay ) )
            {
                m_From = from;
                m_Mobile = m;
                m_MaxCount = maxCount;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                DoBleed( m_Mobile, m_From, m_MaxCount - m_Count );

                if( ++m_Count == m_MaxCount )
                    EndBleed( m_Mobile, true );
            }
        }
    }
}