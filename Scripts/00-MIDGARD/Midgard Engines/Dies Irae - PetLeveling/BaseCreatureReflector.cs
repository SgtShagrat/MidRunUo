/***************************************************************************
 *                               BaseCreatureReflector.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

#define debugCreatures

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Midgard.Engines.CreatureBurningSystem;
using Midgard.Mobiles;
using Server;
using Server.Commands;
using Server.Engines.Quests;
using Server.Factions;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.PetSystem
{
    public class BaseCreatureReflector
    {
        private class InternalLootComparer : IComparer<LootPackEnum>
        {
            public static readonly IComparer<LootPackEnum> Instance = new InternalLootComparer();

            public int Compare( LootPackEnum x, LootPackEnum y )
            {
                return x.CompareTo( y );
            }
        }

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

        private static Dictionary<Type, bool> m_Types;
        private static Dictionary<Type, List<Type>> m_BaseGroups;
        private static Dictionary<SlayerGroup, List<Type>> m_BaseSlayerGroups;
        private static SlayerGroup[] m_Slayers = SlayerGroup.Groups;

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "GenCreaturesReflectionLog", AccessLevel.Developer, new CommandEventHandler( GenCreaturesReflectionLog_OnCommand ) );
            CommandSystem.Register( "GenCreaturesLootLog", AccessLevel.Developer, new CommandEventHandler( GenCreaturesLootLog_OnCommand ) );
        }

        [Usage( "GenCreaturesReflectionLog" )]
        [Description( "Generates the reflection log for midgard creature classes" )]
        private static void GenCreaturesReflectionLog_OnCommand( CommandEventArgs e )
        {
            ProcessTypes();
            GenerateClassTree();
            List();
        }

        public static bool LogLootEnabled = false;
        public static Type CurrentType = null;
        private static string LootLogPath = "Logs/midgard_loot_creatures.log";
        private static void GenCreaturesLootLog_OnCommand( CommandEventArgs e )
        {
            ProcessTypesTidy();

            LogLootEnabled = true;

            foreach( Type t in m_TypesTidy )
            {
                CurrentType = t;
                BaseCreature bc = null;

                try
                {
                    bc = Activator.CreateInstance( CurrentType ) as BaseCreature;
                }
                catch
                {
                    Console.WriteLine( "Unable to construct: {0}", t.Name );
                }

                if( bc != null )
                {
                    //using( StreamWriter op = new StreamWriter( LootLogPath, true ) )
                    //{
                    //    op.WriteLine( "" );
                    //    op.WriteLine( "{0}", CurrentType.Name );
                    //}

                    bc.GenerateLoot();
                }
            }
        }

        public enum LootPackEnum
        {
            None,
            Poor,
            Meager,
            Average,
            Rich,
            FilthyRich,
            UltraRich,
            SuperBoss,

            LowScrolls,
            MedScrolls,
            HighScrolls,
            Gems,
            Potions,
            Parrot,
            Recipe,
            EnchantStone
        }

        public static LootPackEnum GetLootEnum( LootPack pack )
        {
            if( pack == LootPack.Poor )
                return LootPackEnum.Poor;
            else if( pack == LootPack.Meager )
                return LootPackEnum.Meager;
            else if( pack == LootPack.Average )
                return LootPackEnum.Average;
            else if( pack == LootPack.Rich )
                return LootPackEnum.Rich;
            else if( pack == LootPack.FilthyRich )
                return LootPackEnum.FilthyRich;
            else if( pack == LootPack.UltraRich )
                return LootPackEnum.UltraRich;
            else if( pack == LootPack.SuperBoss )
                return LootPackEnum.SuperBoss;

            else if( pack == LootPack.LowScrolls )
                return LootPackEnum.LowScrolls;
            else if( pack == LootPack.MedScrolls )
                return LootPackEnum.MedScrolls;
            else if( pack == LootPack.HighScrolls )
                return LootPackEnum.HighScrolls;
            else if( pack == LootPack.Gems )
                return LootPackEnum.Gems;
            else if( pack == LootPack.Potions )
                return LootPackEnum.Potions;
            if( pack == LootPack.Parrot )
                return LootPackEnum.Parrot;
            else if( pack == LootPack.Recipe )
                return LootPackEnum.Recipe;
            else if( pack == LootPack.EnchantStone )
                return LootPackEnum.EnchantStone;
            return LootPackEnum.None;
        }

        public static void LogLoot( Type creatureType, LootPack pack )
        {
            if( creatureType != CurrentType )
                return;

            LootPackEnum loot = GetLootEnum( pack );

            using( StreamWriter op = new StreamWriter( LootLogPath, true ) )
            {
                op.WriteLine( "\t{0} - {1}", creatureType, Enum.GetName( typeof( LootPackEnum ), loot ) );
            }
        }

        private static void List()
        {
            using( StreamWriter op = new StreamWriter( "Logs/midgard_creatures.log" ) )
            {
                op.WriteLine( "{0}\t{1}", DateTime.Now, "Midgard creatures list." );
                op.WriteLine();

                var orphans = new List<Type>();
                foreach( KeyValuePair<Type, bool> keyValuePair in m_Types )
                {
                    if( !keyValuePair.Value ) // the type is not handled by ancestors
                        orphans.Add( keyValuePair.Key );
                }

                orphans.Sort( InternalComparer.Instance );

                op.WriteLine( "ORPHANS:" );
                foreach( Type type in orphans )
                    op.WriteLine( "\t{0}", type.Name );

                op.WriteLine( "" );
                op.WriteLine( "SUBGROUPS:" );
                foreach( Type type in m_BaseGroups.Keys )
                    op.WriteLine( "\t{0}", type.Name );

                op.WriteLine( "" );
                op.WriteLine( "INTERFACES:" );
                foreach( Type type in m_Interfaces )
                    op.WriteLine( "\t{0}", type.Name );

                op.WriteLine( "" );
                op.WriteLine( "ABSTRACTS:" );
                foreach( Type type in m_Abstracts )
                    op.WriteLine( "\t{0}", type.Name );

                Dictionary<Type, List<Type>>.KeyCollection baseg = m_BaseGroups.Keys;
                foreach( Type type in baseg )
                {
                    List<Type> list;
                    m_BaseGroups.TryGetValue( type, out list );

                    op.WriteLine( "" );
                    op.WriteLine( "SUBGROUP {0}", type.Name );

                    list.Sort( InternalComparer.Instance );

                    foreach( Type type1 in list )
                        op.WriteLine( type1.Name );
                }

                Dictionary<SlayerGroup, List<Type>>.KeyCollection baseSlayerg = m_BaseSlayerGroups.Keys;
                foreach( SlayerGroup g in baseSlayerg )
                {
                    List<Type> list;
                    m_BaseSlayerGroups.TryGetValue( g, out list );

                    list.Sort( InternalComparer.Instance );

                    op.WriteLine( "" );
                    op.WriteLine( "SUBGROUP {0}", g.Super.Name );
                    foreach( Type type1 in list )
                        op.WriteLine( type1.Name );
                }
            }
        }

        private static Type[] m_ToSkip = new Type[]
                                             {
                                                 typeof (BaseWarHorse),
                                                 typeof (BaseCollectionMobile),
                                                 typeof (HeritageQuester),
                                                 typeof (BaseFactionGuard),
                                                 typeof (BaseFactionVendor)
                                             };

        private static Type[] m_Interfaces = new Type[]
                                                 {
                                                     typeof (ITownFolk),
                                                     typeof (ISolenFolk),
                                                     typeof (IBurningCreature),
                                                     typeof (IVirtueCreature),
                                                     typeof (IJhelomFolk),
                                                     typeof (IHythlothFolk),
                                                     typeof (IMoonglowFolk),
                                                     typeof (IOrc),
                                                     typeof (ISkeleton),
                                                     typeof (IUmbraFolk),
                                                 };

        private static List<Type> GetSubClasses( Type t )
        {
            var subclasses = new List<Type>();

            foreach( KeyValuePair<Type, bool> kvp in m_Types )
            {
                if( GetHandled( kvp.Key ) || kvp.Key == t )
                    continue;
                else if( t.IsInterface && t.IsAssignableFrom( kvp.Key ) )
                    subclasses.Add( kvp.Key );
                else if( kvp.Key.IsSubclassOf( t ) )
                    subclasses.Add( kvp.Key );
            }

            return subclasses;
        }

        private static void AddGroup( Type t )
        {
            var children = GetSubClasses( t );

            //if( t == typeof( BaseVendor ) )
            //{
            //    Console.WriteLine( "Bub of basevendor" );
            //    foreach (Type type in children)
            //        Console.WriteLine("\t" + type.Name);
            //}

            foreach( Type child in children )
            {
                if( child.IsAbstract )
                    AddGroup( child );
            }

            AddFatherAndChildren( t, children );
        }

        private static void AddFatherAndChildren( Type father, List<Type> children )
        {
            if( !m_BaseGroups.ContainsKey( father ) )
            {
                var list = new List<Type>();

                foreach( Type child in children )
                {
                    if( !list.Contains( child ) && !GetHandled( child ) )
                        list.Add( child );
                }

                m_BaseGroups.Add( father, list );
            }
            else
            {
                var list = m_BaseGroups[ father ] ?? new List<Type>();

                foreach( Type child in children )
                {
                    if( !list.Contains( child ) && !GetHandled( child ) )
                        list.Add( child );
                }
            }

            SetHandled( father );

            foreach( Type child in children )
                SetHandled( child );
        }

        private static bool GetHandled( Type t )
        {
            if( !m_Types.ContainsKey( t ) )
                Console.WriteLine( "Warning: m_Types does not contain: " + t.Name );

            return m_Types.ContainsKey( t ) && m_Types[ t ];
        }

        private static void SetHandled( Type t )
        {
            if( !m_Types.ContainsKey( t ) )
                Console.WriteLine( "Warning: m_Types does not contain: " + t.Name );

            if( m_Types.ContainsKey( t ) )
                m_Types[ t ] = true;
        }

        private static void GenerateClassTree()
        {
            #region [Handle skip]
            foreach( Type skip in m_ToSkip )
            {
                var children = GetSubClasses( skip );
                foreach( var child in children )
                    SetHandled( child );
            }
            #endregion

            #region [handle interfaces]
            foreach( Type interf in m_Interfaces )
            {
                var children = GetSubClasses( interf );
                foreach( var child in children )
                    SetHandled( child );

                AddFatherAndChildren( interf, children );
            }
            #endregion

            #region [handle abstracts]
            foreach( Type type in m_Abstracts )
            {
                if( !GetHandled( type ) )
                    AddGroup( type );
            }
            #endregion

            //for( int i = 0; i < m_Slayers.Length; i++ )
            //{
            //    m_BaseSlayerGroups.Add( m_Slayers[ i ], null );

            //    foreach( Type t in m_Slayers[ i ].Super.Types )
            //    {
            //        if( m_BaseSlayerGroups[ m_Slayers[ i ] ] == null )
            //            m_BaseSlayerGroups[ m_Slayers[ i ] ] = new List<Type>();

            //        m_BaseSlayerGroups[ m_Slayers[ i ] ].Add( t );

            //        toRemove.Add( t );
            //    }
            //}
        }

        private static List<Type> m_Abstracts;

        private static void ProcessTypes()
        {
            m_BaseGroups = new Dictionary<Type, List<Type>>();
            m_Types = new Dictionary<Type, bool>();
            m_Abstracts = new List<Type>();
            m_BaseSlayerGroups = new Dictionary<SlayerGroup, List<Type>>();

            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type.IsSubclassOf( typeof( BaseCreature ) ) )
                        m_Types.Add( type, false );
                }
            }

            foreach( KeyValuePair<Type, bool> type in m_Types )
            {
                if( type.Key.IsAbstract )
                    m_Abstracts.Add( type.Key );
            }

            m_Abstracts.Sort( InternalComparer.Instance );
        }

        private static List<Type> m_TypesTidy = new List<Type>();

        private static void ProcessTypesTidy()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            for( int i = 0; i < asms.Length; ++i )
            {
                Assembly asm = asms[ i ];
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                for( int j = 0; j < types.Length; ++j )
                {
                    Type type = types[ j ];

                    if( type.IsSubclassOf( typeof( BaseCreature ) ) && !type.IsAbstract )
                        m_TypesTidy.Add( type );
                }
            }

            m_TypesTidy.Sort( InternalComparer.Instance );
        }
    }
}