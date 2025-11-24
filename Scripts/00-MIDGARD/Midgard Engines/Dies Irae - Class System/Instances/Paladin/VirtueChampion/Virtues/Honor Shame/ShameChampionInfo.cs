/***************************************************************************
 *                               ShameChampionInfo.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MiniChampionSystem;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class ShameChampionInfo : AntiVirtueChampionInfo
    {
        public override string Name
        {
            get { return "AntiVirtues - Shame"; }
        }

        public override Type Champion
        {
            get { return typeof( CorruptedDaemon ); }
        }

        public override ChampionSpawnGroup[] SpawnGroups
        {
            get { return Config.DefaultCorruptionSpawnInfo; }
        }

        public override AntiVirtues AntiVirtue
        {
            get { return AntiVirtues.Shame; }
        }
    }
}