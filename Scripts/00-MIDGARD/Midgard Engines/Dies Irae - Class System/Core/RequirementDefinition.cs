/***************************************************************************
 *                               RequirementDefinition.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

namespace Midgard.Engines.Classes
{
    public class RequirementDefinition
    {
        public RequirementDefinition( Type type, int quantity, string name )
        {
            ItemType = type;
            Quantity = quantity;
            Name = name;
        }

        public Type ItemType { get; private set; }
        public int Quantity { get; private set; }
        public string Name { get; private set; }
    }
}