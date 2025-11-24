using System;
using System.Collections.Generic;

using Server.Items;

namespace Midgard.Engines.MidgardTownSystem
{
    [Flags]
    public enum ItemFilterFlags
    {
        None = 0,

        Armors = 1 << 0,
        Weapons = 1 << 1,
        Gems = 1 << 2
    }

    public class ItemFilter
    {
        private ItemFilterFlags m_Flags;

        public ItemFilterFlags Flags { get { return m_Flags; } }

        public ItemFilter()
        {
            m_Flags = ItemFilterFlags.None;
        }

        public bool GetFlag( ItemFilterFlags flag )
        {
            return ( ( m_Flags & flag ) != 0 );
        }

        public void SetFlag( ItemFilterFlags flag, bool value )
        {
            if( value )
                m_Flags |= flag;
            else
                m_Flags &= ~flag;
        }

        public void ResetFlags()
        {
            m_Flags = ItemFilterFlags.None;
        }

        public static int GetMaxFlagValue()
        {
            return 3;
        }

        private IEnumerable<Type> GetFilterTypes()
        {
            List<Type> list = new List<Type>();

            if( GetFlag( ItemFilterFlags.Armors ) )
                list.Add( typeof( BaseArmor ) );
            if( GetFlag( ItemFilterFlags.Gems ) )
                list.Add( typeof( IGem ) );
            if( GetFlag( ItemFilterFlags.Weapons ) )
                list.Add( typeof( BaseWeapon ) );

            return list;
        }

        public List<ItemCommercialInfo> ForTypes( List<ItemCommercialInfo> list )
        {
            List<ItemCommercialInfo> results = new List<ItemCommercialInfo>();
            IEnumerable<Type> types = GetFilterTypes();

            if( list == null || types == null )
                return results;

            Console.WriteLine( "I'm searching for" );
            foreach( Type type in types )
            {
                Console.WriteLine( "\t" + type.Name );
            }

            try
            {
                using( IEnumerator<ItemCommercialInfo> ie = list.GetEnumerator() )
                {
                    while( ie.MoveNext() )
                    {
                        ItemCommercialInfo item = ie.Current;
                        if( item == null )
                            continue;

                        foreach( Type t in types )
                        {
                            if( t.IsAssignableFrom( item.ItemType ) )
                            {
                                results.Add( item );
                                break;
                            }
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return results;
        }
    }
}