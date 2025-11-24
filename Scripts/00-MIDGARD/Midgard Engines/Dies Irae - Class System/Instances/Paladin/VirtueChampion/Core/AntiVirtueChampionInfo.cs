/***************************************************************************
 *                               AntiVirtueChampionInfo.cs
 *
 *   begin                : 18 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Midgard.Engines.MiniChampionSystem;

using Server;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public abstract class AntiVirtueChampionInfo : ChampionSpawnInfo
    {
        public abstract AntiVirtues AntiVirtue { get; }

        public Mobile GenChampion()
        {
            return new CorruptedDaemon( AntiVirtue );
        }
    }
}