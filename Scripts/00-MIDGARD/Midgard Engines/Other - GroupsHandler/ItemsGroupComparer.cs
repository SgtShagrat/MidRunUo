using System.Collections;

namespace Midgard.Engines.GroupsHandler
{
    public class ItemsGroupComparer : IComparer
    {
        public static readonly IComparer Instance = new ItemsGroupComparer();

        public int Compare( object x, object y )
        {
            string a = null, b = null;
            bool sa = false, sb = false;

            if( x is ItemsGroup )
            {
                a = ( (ItemsGroup)x ).Name;
                sa = ( (ItemsGroup)x ).Secure;
            }

            if( y is ItemsGroup )
            {
                b = ( (ItemsGroup)y ).Name;
                sb = ( (ItemsGroup)y ).Secure;
            }

            if( sa && !sb )
                return 1;

            if( !sa && sb )
                return -1;

            if( a == null && b == null )
                return 0;

            if( a == null )
                return 1;

            if( b == null )
                return -1;

            return a.CompareTo( b );
        }
    }
}