using System;

using Server;

namespace Midgard
{
    [AttributeUsage( AttributeTargets.Class )]
    public class CarpetAttribute : Attribute
    {
        public static bool Check( Item item )
        {
            return ( item != null && item.GetType().IsDefined( typeof( CarpetAttribute ), false ) );
        }

        public CarpetAttribute()
        {
        }
    }
}