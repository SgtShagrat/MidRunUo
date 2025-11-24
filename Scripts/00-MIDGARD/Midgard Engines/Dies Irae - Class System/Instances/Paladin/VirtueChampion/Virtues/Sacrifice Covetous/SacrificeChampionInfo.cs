/***************************************************************************
 *                               SacrificeChampionInfo.cs
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
    public class SacrificeChampionInfo : VirtueChampionInfo
    {
        public override string Name
        {
            get { return "Virtues - Sacrifice"; }
        }

        public override Type Champion
        {
            get { return typeof( Soulkeeper ); }
        }

        public override ChampionSpawnGroup[] SpawnGroups
        {
            get { return Config.DefaultRedeemSpawnInfo; }
        }

        public override Virtues Virtue
        {
            get { return Virtues.Sacrifice; }
        }
    }
}