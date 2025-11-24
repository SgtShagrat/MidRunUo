/***************************************************************************
 *                               SliceTimer.cs
 *
 *   begin                : 14 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.MiniChampionSystem
{
    public class SliceTimer : Timer
    {
        private readonly MiniChampionSpawn m_Spawn;

        public SliceTimer( MiniChampionSpawn spawn )
            : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
        {
            m_Spawn = spawn;
            Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            m_Spawn.OnSlice();
        }
    }
}