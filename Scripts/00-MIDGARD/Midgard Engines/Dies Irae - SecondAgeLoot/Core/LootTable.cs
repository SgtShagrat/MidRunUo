/***************************************************************************
 *                               LootTable.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.SecondAgeLoot
{
    public class LootTable
    {
        public IEntry[] Entries { get; set; }

        public LootTable( IEntry[] entries )
        {
            Entries = entries;
        }

        public Item Construct()
        {
            if( Entries.Length == 1 )
                return Entries[ 0 ].Construct();

            int totalChance = 0;

            foreach( IEntry t in Entries )
                totalChance += t.Weight;

            if( Entries.Length == totalChance )
                Entries[ Utility.Random( Entries.Length ) ].Construct();

            int rnd = Utility.Random( totalChance );

            foreach( IEntry t in Entries )
            {
                if( rnd < t.Weight )
                    return t.Construct();

                rnd -= t.Weight;
            }

            return Entries[ 0 ].Construct();
        }
    }
}