using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class BagOfDwarvenItems : Bag
    {
        private class InternalComparer : IComparer<Type>
        {
            public static readonly IComparer<Type> Instance = new InternalComparer();

            public int Compare( Type x, Type y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }

        private static List<Type> m_Types = new List<Type>();

        private static List<Type> m_Armors = new List<Type>();
        private static List<Type> m_Weapons = new List<Type>();
        private static List<Type> m_Jewels = new List<Type>();
        private static List<Type> m_Lights = new List<Type>();
        private static List<Type> m_Clothing = new List<Type>();
        private static List<Type> m_Misc = new List<Type>();

        private static Type m_TypeOfItem = typeof( Item );

        private static void ProcessTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                Type[] types = asm.GetTypes();

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type == m_TypeOfItem || type.IsSubclassOf( m_TypeOfItem ) )
                    {
                        bool isConstructable = false;

                        ConstructorInfo[] ctors = type.GetConstructors();

                        for( int k = 0; k < ctors.Length; ++k )
                        {
                            if( ctors[ k ].IsDefined( typeof( ConstructableAttribute ), false ) )
                            {
                                isConstructable = true;
                                break;
                            }
                        }

                        if( !isConstructable )
                            continue;

                        object[] attrs = type.GetCustomAttributes( typeof( RaceAllowanceAttribute ), false );
                        
                        if( attrs.Length > 0 && ( (RaceAllowanceAttribute)attrs[ 0 ] ).RaceType == typeof( MountainDwarf ) )
                            m_Types.Add( type );
                    }
                }
            }

            foreach( Type t  in m_Types )
            {
                if( t.IsSubclassOf( typeof( BaseArmor ) ) )
                    m_Armors.Add( t );
                else if( t.IsSubclassOf( typeof( BaseWeapon ) ) )
                    m_Weapons.Add( t );
                else if( t.IsSubclassOf( typeof( BaseJewel ) ) )
                    m_Jewels.Add( t );
                else if( t.IsSubclassOf( typeof( BaseLight ) ) )
                    m_Lights.Add( t );
                else if( t.IsSubclassOf( typeof( BaseClothing ) ) )
                    m_Clothing.Add( t );
                else
                    m_Misc.Add( t );
            }

            m_Armors.Sort( InternalComparer.Instance );
            m_Weapons.Sort( InternalComparer.Instance );
            m_Jewels.Sort( InternalComparer.Instance );
            m_Lights.Sort( InternalComparer.Instance );
            m_Clothing.Sort( InternalComparer.Instance );
            m_Misc.Sort( InternalComparer.Instance );

            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/dwarvenItems.log", true ) )
                {
                    LogList( m_Armors, op );
                    op.WriteLine( "" );

                    LogList( m_Weapons, op );
                    op.WriteLine( "" );

                    LogList( m_Jewels, op );
                    op.WriteLine( "" );

                    LogList( m_Lights, op );
                    op.WriteLine( "" );

                    LogList( m_Clothing, op );
                    op.WriteLine( "" );

                    LogList( m_Misc, op );
                    op.WriteLine( "" );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        private static void LogList( IEnumerable<Type> list, TextWriter op )
        {
            foreach ( Type type in list )
                op.WriteLine( type.Name );
        }

        private static Item Construct( Type type )
        {
            try
            {
                return Activator.CreateInstance( type ) as Item;
            }
            catch
            {
                return null;
            }
        }

        [Constructable]
        public BagOfDwarvenItems()
        {
            ProcessTypes();

            try
            {
                foreach( Type t in m_Types )
                {
                    Item i = Construct( t );
                    if( i != null )
                        DropItem( i );
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        #region serialization
        public BagOfDwarvenItems( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}