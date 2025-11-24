/***************************************************************************
 *                               LootItem.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.SecondAgeLoot
{
    public class LootItem : IEntry
    {
        public Type Type { get; set; }
        public int Weight { get; set; }
        public int Mirror { get; set; }

        public Item Construct()
        {
            try
            {
                Item i = Activator.CreateInstance( Type ) as Item;

                if( Mirror != 0 && i != null )
                    i.ItemID = Mirror;

                return i;
            }
            catch
            {
            }

            return null;
        }

        public LootItem( Type type, int weight )
            : this( type, weight, 0 )
        {
        }

        public LootItem( Type type, int weight, int mirror )
        {
            Type = type;
            Weight = weight;
            Mirror = mirror;
        }
    }
}