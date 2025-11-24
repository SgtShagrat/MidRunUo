/***************************************************************************
 *                               PowerDefinition.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Midgard.Engines.Classes
{
    public class PowerDefinition
    {
        public PowerDefinition( Type type, string name, int maxRituals, RequirementDefinition[] requirements, double requiredSkill )
        {
            PowerType = type;
            Name = name;
            MaxRituals = maxRituals;
            Requirements = requirements;
            RequiredSkill = requiredSkill;
        }

        public Type PowerType { get; private set; }
        public string Name { get; private set; }
        public int MaxRituals { get; private set; }
        public RequirementDefinition[] Requirements { get; private set; }
        public double RequiredSkill { get; private set; }

        /// <summary>
        /// No ritual required to cast this spell. Max level forever.
        /// </summary>
        public virtual bool IsGranted
        {
            get { return false; }
        }

        /// <summary>
        /// On playerstate init this definition grants level 1.
        /// </summary>
        public virtual bool FirstLevelGranted
        {
            get { return false; }
        }

        public override string ToString()
        {
            return Name;
        }

        public void GetReqTypesQuantArray( out Type[] typesArray, out int[] quantitiesArray )
        {
            List<Type> types = new List<Type>();
            List<int> quantities = new List<int>();

            foreach( RequirementDefinition definition in Requirements )
            {
                types.Add( definition.ItemType );
                quantities.Add( definition.Quantity );
            }

            typesArray = types.ToArray();
            quantitiesArray = quantities.ToArray();
        }
    }
}