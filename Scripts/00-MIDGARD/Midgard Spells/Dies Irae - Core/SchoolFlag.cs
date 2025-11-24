using System;

namespace Midgard.Engines.SpellSystem
{
    [Flags]
    public enum SchoolFlag
    {
        None            = 0x00000000,

        Druid           = 0x00000001,
        Paladin         = 0x00000002,
        Necromancer     = 0x00000004
    }
}