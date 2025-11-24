/***************************************************************************
 *                               AnimTimer.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Classes
{
    public class AnimTimer : Timer
    {
        public static readonly int[] AnimIds = new int[] { 245, 266 };

        public static double AnimateDelay = 1.5;
        public static double DurationPerLevel = 2.0;

        private readonly Mobile m_From;
        private readonly int m_Level;
        private readonly Ritual m_Ritual;

        public AnimTimer( Mobile from, Ritual ritual, int count, int level )
            : base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
        {
            m_From = from;
            m_Ritual = ritual;
            m_Level = level;

            Priority = TimerPriority.FiftyMS;
        }

        protected override void OnTick()
        {
            if( !m_From.Mounted && m_From.Body.IsHuman )
                m_From.Animate( Utility.RandomList( AnimIds ), 7, 1, true, false, 0 );

            m_Ritual.DoEffects( m_From, true );
            m_From.PlaySound( 0x208 );
        }
    }
}