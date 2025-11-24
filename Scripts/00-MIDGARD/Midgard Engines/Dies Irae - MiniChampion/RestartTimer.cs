/***************************************************************************
 *                               RestartTimer.cs
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
    public class RestartTimer : Timer
    {
        private readonly MiniChampionSpawn m_Spawn;

        public RestartTimer( MiniChampionSpawn spawn, TimeSpan delay )
            : base( delay )
        {
            m_Spawn = spawn;
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            m_Spawn.Start();
        }
    }
}