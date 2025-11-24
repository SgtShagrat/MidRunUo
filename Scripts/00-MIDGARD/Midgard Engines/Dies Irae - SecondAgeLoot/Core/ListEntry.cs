/***************************************************************************
 *                               ListEntry.cs
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
    public class ListEntry : TableEntry
    {
        public IEntry[] Items { get; set; }

        public ListEntry( IEntry[] items, int weight )
            : base( null, weight )
        {
            Items = items;
        }

        public override Item Construct()
        {
            if( Items.Length == 1 )
                return Items[ 0 ].Construct();

            int totalChance = 0;

            foreach( LootItem t in Items )
                totalChance += t.Weight;

            if( Items.Length == totalChance )
                Items[ Utility.Random( Items.Length ) ].Construct();

            int rnd = Utility.Random( totalChance );

            foreach( LootItem t in Items )
            {
                if( rnd < t.Weight )
                    return t.Construct();

                rnd -= t.Weight;
            }

            return Items[ 0 ].Construct();
        }
    }
}