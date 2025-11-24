/***************************************************************************
 *                               ChampionSpawnInfo.cs
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
    public abstract class ChampionSpawnInfo
    {
        /// <summary>
        /// The main naim of this mini champion
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The boss which spawns at champion end
        /// </summary>
        public abstract Type Champion { get; }

        /// <summary>
        /// List of spawn groups. One for each level of our mini champion
        /// </summary>
        public abstract ChampionSpawnGroup[] SpawnGroups { get; }

        public int NumLevels
        {
            get { return SpawnGroups.Length; }
        }

        public ChampionSpawnGroup GetGroupByLevel( int level )
        {
            if( level >= 0 && level < SpawnGroups.Length )
                return SpawnGroups[ level ];
            else
            {
                Config.Pkg.LogErrorLine( "Champion info called GetGroupByLevel but no valid group found." );
                return SpawnGroups[ 0 ];
            }
        }

        public virtual void SetupSpawner( MiniChampionSpawn spawn )
        {
        }

        public virtual void SetupSupportItems( MiniChampionSpawn spawn )
        {
        }

        public virtual void OnChampionStarted( MiniChampionSpawn spawn )
        {
            if( spawn.ChampionSpawner != null )
                spawn.ChampionSpawner.Hue = Champion != null ? 0x26 : 0;
        }

        public virtual void OnChampionStopped( MiniChampionSpawn spawn )
        {
            if( spawn.ChampionSpawner != null )
                spawn.ChampionSpawner.Hue = 0;
        }

        public virtual void OnChampionLevelChanged( MiniChampionSpawn spawn )
        {
            if( spawn.ChampionSpawner != null )
            {
                Effects.PlaySound( spawn.ChampionSpawner.Location, spawn.ChampionSpawner.Map, 0x29 );
                Effects.SendLocationEffect( new Point3D( spawn.ChampionSpawner.X + 1, spawn.ChampionSpawner.Y + 1, spawn.ChampionSpawner.Z ), spawn.ChampionSpawner.Map, 0x3728, 10 );
            }
        }

        public virtual void OnChampionDeleted( MiniChampionSpawn spawn )
        {
            if( spawn.ChampionSpawner != null )
                spawn.ChampionSpawner.Hue = 0;
        }

        public virtual void OnChampionSpawned( MiniChampionSpawn spawn )
        {
            if( spawn.ChampionSpawner != null )
                spawn.ChampionSpawner.Hue = 0x26;
        }

        public virtual void OnChampionRespawned( MiniChampionSpawn spawn, Mobile m )
        {
        }

        public virtual void OnChampionExpired( MiniChampionSpawn spawn )
        {
        }
    }
}