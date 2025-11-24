using System;
using System.Collections.Generic;

namespace Midgard.Engines.MidgardTownSystem
{
    public class GuardList
    {
        public GuardDefinition Definition { get; private set; }
        public List<BaseTownGuard> Guards { get; private set; }

        public BaseTownGuard Construct()
        {
            try { return Activator.CreateInstance( Definition.Type ) as BaseTownGuard; }
            catch { return null; }
        }

        public GuardList( GuardDefinition definition )
        {
            Definition = definition;
            Guards = new List<BaseTownGuard>();
        }
    }
}