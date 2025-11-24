/***************************************************************************
 *                               TableEntry.cs
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
    public class TableEntry : IEntry
    {
        public LootTable Table { get; set; }
        public int Weight { get; set; }

        public TableEntry( LootTable table, int weight )
        {
            Table = table;
            Weight = weight;
        }

        public virtual Item Construct()
        {
            return Table.Construct();
        }
    }
}