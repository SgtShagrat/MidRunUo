/***************************************************************************
 *                               Reflector.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

/*
using System;
using System.Collections.Generic;
using System.Reflection;
using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public class Reflector
    {
        private static List<Type> m_Types = new List<Type>();

        private static List<BaseWar> m_Wars;

        public static List<BaseWar> Wars
        {
            get
            {
                if( m_Wars == null )
                    ProcessTypes();

                return m_Wars;
            }
        }

        public static void RegisterWar( BaseWar war )
        {
            BaseWar.Wars.Add( war );
        }

        public static void Configure()
        {
            EventSink.WorldSave += new WorldSaveEventHandler( EventSink_WorldSave );
        }

        private static void EventSink_WorldSave( WorldSaveEventArgs e )
        {
            m_Types.Clear();
        }

        public static void Serialize( GenericWriter writer, Type type )
        {
            int index = m_Types.IndexOf( type );

            writer.WriteEncodedInt( index + 1 );

            if( index == -1 )
            {
                writer.Write( type == null ? null : type.FullName );
                m_Types.Add( type );
            }
        }

        public static Type Deserialize( GenericReader reader )
        {
            int index = reader.ReadEncodedInt();

            if( index == 0 )
            {
                string typeName = reader.ReadString();

                if( typeName == null )
                    m_Types.Add( null );
                else
                    m_Types.Add( ScriptCompiler.FindTypeByFullName( typeName, false ) );

                return m_Types[ m_Types.Count - 1 ];
            }
            else
            {
                return m_Types[ index - 1 ];
            }
        }

        private static object Construct( Type type )
        {
            try { return Activator.CreateInstance( type ); }
            catch { return null; }
        }

        private static void ProcessTypes()
        {
            m_Wars = new List<BaseWar>();

            foreach( Assembly asm in ScriptCompiler.Assemblies )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );

                foreach( Type type in tc.Types )
                {
                    if( !type.IsSubclassOf( typeof( BaseWar ) ) )
                        continue;

                    BaseWar war = Construct( type ) as BaseWar;
                    if( war != null )
                        RegisterWar( war );
                }
            }
        }
    }
}
*/