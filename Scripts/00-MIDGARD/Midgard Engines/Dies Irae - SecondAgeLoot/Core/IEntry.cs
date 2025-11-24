/***************************************************************************
 *                               IEntry.cs
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
    public interface IEntry
    {
        Item Construct();
        int Weight { get; set; }
    }
}