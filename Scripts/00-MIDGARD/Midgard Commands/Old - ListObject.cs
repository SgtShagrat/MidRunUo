using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Midgard.Engines.Classes;
using Midgard.Engines.OldCraftSystem;
using Midgard.Engines.PlantSystem;
using Midgard.Items;

using Server;
using Server.Commands;
using Server.Items;
using Server.Multis.Deeds;

namespace Midgard.Commands
{
    public class ListObject
    {
        public static void Initialize()
        {
            CommandSystem.Register( "ListObjects", AccessLevel.Developer, new CommandEventHandler( ListObjects_OnCommand ) );
        }

        [Usage( "ListObjects" )]
        [Description( "List in a file all items and mobiles organized in definitions" )]
        public static void ListObjects_OnCommand( CommandEventArgs e )
        {
            Console.Write( "Counting objects and mobiles..." );

            Server.Network.NetState.FlushAll();
            Server.Network.NetState.Pause();

            Stopwatch watch = Stopwatch.StartNew();

            CountObjectsAndMobiles();

            watch.Stop();

            Console.WriteLine( "done ({0:F2} seconds).", watch.Elapsed.TotalSeconds );

            Server.Network.NetState.Resume();

            e.Mobile.SendMessage( "Object table has been generated. See the file : <runuo root>/Logs/objects.log" );
        }

        private static List<KeyValuePair<Type, int>> m_Items;
        private static List<KeyValuePair<Type, int>> m_Mobiles;

        private static Dictionary<Type, int> m_Table;
        private static Dictionary<string, IEnumerable<KeyValuePair<Type, int>>> m_SubTable;

        private static IEnumerable<KeyValuePair<Type, int>> GetPairsForType( Type t, bool removeFromMainTable )
        {
            List<KeyValuePair<Type, int>> list = new List<KeyValuePair<Type, int>>( from i in m_Items
                                                                                    where t.IsAssignableFrom( i.Key )
                                                                                    select i );

            // typeof(IWhatever).IsAssignableFrom(type)

            list.Sort( new CountSorter() );

            if( !removeFromMainTable )
                return list;

            foreach( KeyValuePair<Type, int> keyValuePair in list )
            {
                for( int index = 0; index < m_Items.Count; index++ )
                {
                    if( m_Items[ index ].Key == keyValuePair.Key )
                        m_Items.RemoveAt( index );
                }
            }

            return list;
        }

        private static void CountObjectsAndMobiles()
        {
            m_Table = new Dictionary<Type, int>();

            foreach( Item i in World.Items.Values )
            {
                Type type = i.GetType();

                if( !m_Table.ContainsKey( type ) )
                    m_Table[ type ] = 1;
                else
                    m_Table[ type ] = m_Table[ type ] + 1;
            }

            m_Items = new List<KeyValuePair<Type, int>>( m_Table );

            m_SubTable = new Dictionary<string, IEnumerable<KeyValuePair<Type, int>>>();

            m_SubTable.Add( "Treasures of Midgard", GetPairsForType( typeof( ITreasureOfMidgard ), true ) );

            m_SubTable.Add( "Armors", GetPairsForType( typeof( BaseArmor ), true ) );
            m_SubTable.Add( "Weapons", GetPairsForType( typeof( BaseWeapon ), true ) );
            m_SubTable.Add( "Jewels", GetPairsForType( typeof( BaseJewel ), true ) );
            m_SubTable.Add( "Clothing", GetPairsForType( typeof( BaseClothing ), true ) );
            m_SubTable.Add( "Book", GetPairsForType( typeof( BaseBook ), true ) );
            m_SubTable.Add( "Ores", GetPairsForType( typeof( BaseOre ), true ) );
            m_SubTable.Add( "Ingot", GetPairsForType( typeof( BaseIngot ), true ) );
            m_SubTable.Add( "Logs", GetPairsForType( typeof( BaseLog ), true ) );
            m_SubTable.Add( "Boards", GetPairsForType( typeof( BaseBoards ), true ) );
            m_SubTable.Add( "Leather", GetPairsForType( typeof( BaseLeather ), true ) );
            m_SubTable.Add( "Hides", GetPairsForType( typeof( BaseHides ), true ) );
            m_SubTable.Add( "Granites", GetPairsForType( typeof( BaseGranite ), true ) );
            m_SubTable.Add( "Lights", GetPairsForType( typeof( BaseLight ), true ) );
            m_SubTable.Add( "Food", GetPairsForType( typeof( Food ), true ) );
            m_SubTable.Add( "Pillow", GetPairsForType( typeof( BasePillow ), true ) );
            m_SubTable.Add( "Craft Book", GetPairsForType( typeof( GenericCraftBook ), true ) );
            m_SubTable.Add( "Gems", GetPairsForType( typeof( IGem ), true ) );
            m_SubTable.Add( "Furniture", GetPairsForType( typeof( CraftableFurniture ), true ) );
            m_SubTable.Add( "Ritual Item", GetPairsForType( typeof( RitualItem ), true ) );
            m_SubTable.Add( "Traps", GetPairsForType( typeof( BaseTrap ), true ) );
            m_SubTable.Add( "Seeds", GetPairsForType( typeof( BaseSeed ), true ) );
            m_SubTable.Add( "Crops", GetPairsForType( typeof( BaseCrop ), true ) );
            m_SubTable.Add( "Reagent", GetPairsForType( typeof( BaseReagent ), true ) );
            m_SubTable.Add( "Potion", GetPairsForType( typeof( BasePotion ), true ) );
            m_SubTable.Add( "Instruments", GetPairsForType( typeof( BaseInstrument ), true ) );
            m_SubTable.Add( "Tools", GetPairsForType( typeof( BaseTool ), true ) );
            m_SubTable.Add( "Scrolls", GetPairsForType( typeof( SpellScroll ), true ) );
            m_SubTable.Add( "Game pieces", GetPairsForType( typeof( BasePiece ), true ) );
            m_SubTable.Add( "House Deeds", GetPairsForType( typeof( HouseDeed ), true ) );
            m_SubTable.Add( "Beverages", GetPairsForType( typeof( BaseBeverage ), true ) );
            m_SubTable.Add( "Containers", GetPairsForType( typeof( BaseContainer ), true ) );
            m_SubTable.Add( "Multis", GetPairsForType( typeof( BaseMulti ), true ) );
            m_SubTable.Add( "Addons", GetPairsForType( typeof( BaseAddon ), true ) );
            m_SubTable.Add( "Addon Deeds", GetPairsForType( typeof( BaseAddonDeed ), true ) );

            m_Table.Clear();

            foreach( Mobile m in World.Mobiles.Values )
            {
                Type type = m.GetType();

                if( !m_Table.ContainsKey( type ) )
                    m_Table[ type ] = 1;
                else
                    m_Table[ type ] = m_Table[ type ] + 1;
            }

            m_Mobiles = new List<KeyValuePair<Type, int>>( m_Table );

            m_Table.Clear();

            m_Items.Sort( new CountSorter() );
            m_Mobiles.Sort( new CountSorter() );

            using( StreamWriter op = new StreamWriter( "Logs/objects.log" ) )
            {
                op.WriteLine( "# Object count table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                op.WriteLine( "# Items:" );

                foreach( KeyValuePair<Type, int> kvp in m_Items )
                    op.WriteLine( "{0}\t{1:F2}%\t{2}", kvp.Value, ( 100 * kvp.Value ) / (double)World.Items.Count, kvp.Key );

                foreach( KeyValuePair<string, IEnumerable<KeyValuePair<Type, int>>> keyValuePair in m_SubTable )
                {
                    op.WriteLine();
                    op.WriteLine( "# {0}:", keyValuePair.Key );

                    foreach( KeyValuePair<Type, int> kvp in keyValuePair.Value )
                        op.WriteLine( "{0}\t{1:F2}%\t{2}", kvp.Value, ( 100 * kvp.Value ) / (double)World.Items.Count, kvp.Key );
                }

                op.WriteLine();
                op.WriteLine();

                op.WriteLine( "#Mobiles:" );

                foreach( KeyValuePair<Type, int> de in m_Mobiles )
                    op.WriteLine( "{0}\t{1:F2}%\t{2}", de.Value, ( 100 * de.Value ) / (double)World.Mobiles.Count, de.Key );
            }
        }

        private class CountSorter : IComparer<KeyValuePair<Type, int>>
        {
            public int Compare( KeyValuePair<Type, int> a, KeyValuePair<Type, int> b )
            {
                int v = -a.Value.CompareTo( b.Value );

                if( v == 0 )
                    v = a.Key.FullName.CompareTo( b.Key.FullName );

                return v;
            }
        }
    }
}