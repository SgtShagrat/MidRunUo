/***************************************************************************
 *                               MiniChampionSpawnRegion.cs
 *
 *   begin                : 14 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Regions;

namespace Midgard.Engines.MiniChampionSystem
{
    public class MiniChampionSpawnRegion : BaseRegion
    {
        public MiniChampionSpawn MiniChampionSpawn { get; private set; }

        public MiniChampionSpawnRegion( MiniChampionSpawn spawn )
            : base( null, spawn.Map, Find( spawn.Location, spawn.Map ), spawn.SpawnArea )
        {
            MiniChampionSpawn = spawn;
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }

        public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
        {
            base.AlterLightLevel( m, ref global, ref personal );
            global = Math.Max( global, 1 + MiniChampionSpawn.Level );
        }
    }
}