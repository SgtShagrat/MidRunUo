/***************************************************************************
 *                               CommercialTimer.cs
 *
 *   begin                : 27 aprile 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Diagnostics;

using Midgard.Engines.WarSystem;

using Server;

namespace Midgard.Engines.CommercialSystem
{
    internal class CommercialTimer : Timer
    {
        public CommercialTimer()
            : base( TimeSpan.FromSeconds( 60.0 ), Config.CommercialRefreshDelay )
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if( BaseWar.DisableTimersDuringBattle && WarSystem.Core.Instance.WarPending )
                return;

            Stopwatch watch = Stopwatch.StartNew();
            Config.Pkg.LogInfo( "Processing commercial status..." );

            Core.Instance.ProcessCommercialSystem();

            watch.Stop();
            Config.Pkg.LogInfoLine( "done in {0:F2} seconds.", watch.Elapsed.TotalSeconds );
        }
    }
}