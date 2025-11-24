/***************************************************************************
 *                               RessSystem.cs
 *                            -------------------
 *   begin                : 24 ottobre 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using Server;

namespace Midgard.Misc
{
    public class RessSystem
    {
        private static TimeSpan m_ImmuneDuration = TimeSpan.FromSeconds( 10.0 );
        private static readonly Hashtable m_Table = new Hashtable();

        /// <summary>
        /// Check if our mobile is under ress-immune effect
        /// </summary>
        /// <param name="m">mobile to check</param>
        /// <returns>true if immune</returns>
        public static bool IsImmune( Mobile m )
        {
            return m_Table.Contains( m );
        }

        /// <summary>
        /// Start ress immunity for our mobile
        /// </summary>
        /// <param name="m">mobile to make immune</param>
        public static void BeginRessImmune( Mobile m )
        {
            Timer t = (Timer)m_Table[ m ];

            if( t != null )
                t.Stop();

            t = new InternalTimer( m );

            m_Table[ m ] = t;

            t.Start();

            m.SendMessage( 37, "You are now immune to player and monster attacks for {0} seconds.", m_ImmuneDuration.TotalSeconds );
            m.SolidHueOverride = 0x035;
        }

        /// <summary>
        /// End ress immunity for our mobile
        /// </summary>
        /// <param name="m">mobile to remove immunity to</param>
        public static void EndRessImmune( Mobile m )
        {
            Timer t = (Timer)m_Table[ m ];

            if( t != null )
                t.Stop();

            m_Table.Remove( m );

            m.SendMessage( 37, "Be aware. You are now vulnerable" );
            m.SolidHueOverride = -1;
            m.InvalidateProperties();
            m.Delta( MobileDelta.Noto );
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;

            private readonly Point3D m_StartingLocation;
            private readonly Point3D m_CorpseLocation;

            private readonly DateTime m_EndTime;

            private readonly bool m_InitialNearCorpse;

            public InternalTimer( Mobile m )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Mobile = m;

                m_StartingLocation = m.Location;

                if( m.Corpse != null )
                    m_CorpseLocation = m.Corpse.Location;

                m_EndTime = DateTime.Now + m_ImmuneDuration;

                m_InitialNearCorpse = ( m.Corpse != null ) && m_Mobile.GetDistanceToSqrt( m_CorpseLocation ) < 5.0;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if( m_Mobile == null )
                    EndRessImmune( m_Mobile );
                else if( DateTime.Now > m_EndTime )
                    EndRessImmune( m_Mobile );
                else if( m_Mobile.GetDistanceToSqrt( m_StartingLocation ) > 5.0 )
                    EndRessImmune( m_Mobile );
                else if( !m_InitialNearCorpse && ( m_Mobile.Corpse != null ) && m_Mobile.GetDistanceToSqrt( m_CorpseLocation ) < 5.0 )
                    EndRessImmune( m_Mobile );
            }
        }
    }
}