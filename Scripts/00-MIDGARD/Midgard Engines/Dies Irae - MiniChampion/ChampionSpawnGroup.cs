/***************************************************************************
 *                               ChampionSpawnGroup.cs
 *
 *   begin                : 15 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;

namespace Midgard.Engines.MiniChampionSystem
{
    public class ChampionSpawnGroup
    {
        /// <summary>
        /// The spawn elements of this group.
        /// For ex: elem 1: Orc -> W 1
        ///         elem 2: Cat -> W 5
        /// </summary>
        public SpawnGroupElement[] Elements { get; set; }

        /// <summary>
        /// The number of kills reqired to force champion level advance
        /// </summary>
        public int MaxKills { get; private set; }

        private int TotalWeight { get; set; }

        public int MaxLevelSpawnAmount { get; set; }

        public bool ShouldSpawn( List<Mobile> creatures )
        {
            if( creatures == null )
            {
                Config.Pkg.LogErrorLine( "Spawn group called ShouldSpawn on null creature list." );
                return false;
            }

            return creatures.Count < MaxLevelSpawnAmount;
        }

        public bool CanSpawn( List<Mobile> creatures )
        {
            if( creatures == null )
            {
                Config.Pkg.LogErrorLine( "Spawn group called CanSpawn on null creature list." );
                return false;
            }

            foreach( SpawnGroupElement element in Elements )
            {
                if( element.CanSpawn( creatures ) )
                    return true;
            }

            return false;
        }

        private void InvalidateTotals()
        {
            TotalWeight = 0;
            MaxLevelSpawnAmount = 0;

            foreach( SpawnGroupElement element in Elements )
            {
                TotalWeight += element.Weight;
                MaxLevelSpawnAmount += element.MaxSpawnAmount;
            }
        }

        public Mobile Spawn( List<Mobile> creatures )
        {
            if( creatures == null )
            {
                Config.Pkg.LogErrorLine( "Spawn group called Spawn on null creature list." );
                return null;
            }

            List<SpawnGroupElement> validElements = new List<SpawnGroupElement>();
            int totalValidWeight = 0;

            foreach( SpawnGroupElement element in Elements )
            {
                if( !element.CanSpawn( creatures ) )
                    continue;

                validElements.Add( element );
                totalValidWeight += element.Weight;
            }

            if( totalValidWeight <= 0 )
            {
                Config.Pkg.LogInfoLine( "Spawn group called Spawn but no valid element could spawn." );
                return null;
            }

            int index = Utility.Random( totalValidWeight );

            foreach( SpawnGroupElement element in validElements )
            {
                if( index < element.Weight )
                    return element.Spawn();

                index -= element.Weight;
            }

            return null;
        }

        public ChampionSpawnGroup( SpawnGroupElement[] elements, int maxKills )
        {
            Elements = elements;
            MaxKills = maxKills;

            InvalidateTotals();
        }
    }
}