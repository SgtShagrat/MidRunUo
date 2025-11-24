/***************************************************************************
 *                               SpawnGroupElement.cs
 *
 *   begin                : 15 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;

namespace Midgard.Engines.MiniChampionSystem
{
    public class SpawnGroupElement
    {
        /// <summary>
        /// Each definition has a type
        /// </summary>
        public Type SpawnType { get; private set; }

        /// <summary>
        /// The weight of this element in front of other group elements
        /// </summary>
        public int Weight { get; private set; }

        /// <summary>
        /// Each spawn definition can define a max spawn amount
        /// </summary>
        public int MaxSpawnAmount { get; private set; }

        public bool CanSpawn( List<Mobile> creatures )
        {
            if( creatures == null )
            {
                Config.Pkg.LogErrorLine( "Spawn group element called CanSpawn on null creature list." );
                return false;
            }

            int count = 0;
            foreach( Mobile creature in creatures )
            {
                if( creature.GetType() == SpawnType )
                    count++;
            }

            return count <= MaxSpawnAmount;
        }

        public Mobile Spawn()
        {
            try
            {
                return Activator.CreateInstance( SpawnType ) as Mobile;
            }
            catch( Exception e )
            {
                Config.Pkg.LogError( e );
                return null;
            }
        }

        public SpawnGroupElement( Type spawnType, int weight, int maxSpawnAmount )
        {
            SpawnType = spawnType;
            Weight = weight;
            MaxSpawnAmount = maxSpawnAmount;
        }
    }
}