using System;

namespace Midgard.Engines.BossSystem
{
    [AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = true )]
    class DungeonGroupAttribute : Attribute
    {
        public readonly string Group = null;

        public DungeonGroupAttribute( string groupname )
        {
            Group = groupname;
        }
    }
}