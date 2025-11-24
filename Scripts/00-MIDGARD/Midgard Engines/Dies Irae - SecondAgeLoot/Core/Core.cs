/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Items;

namespace Midgard.Engines.SecondAgeLoot
{
    public class Core
    {
        #region [loot test]
        private static void FixArmor( BaseArmor armor )
        {
            if( armor.IsMagical )
            {
                Magics.ClearArmorBonus( armor );
                Magics.ApplyArmorBonus( armor );
                armor.ClearIdentifiers();
            }
        }

        private static void FixWeapon( BaseWeapon weapon )
        {
            if( weapon.IsMagical )
            {
                Magics.ClearWeaponBonus( weapon );
                Magics.ApplyWeaponBonus( weapon );
                weapon.ClearIdentifiers();
            }
        }

        private static void FixJewel( BaseJewel jewel )
        {
            if( jewel.IsMagical )
            {
                Magics.ClearJewelBonus( jewel );
                Magics.ApplyJewelBonus( jewel );
                jewel.ClearIdentifiers();
            }
        }

        private static void FixClothing( BaseClothing clothing )
        {
            if( clothing.IsMagical )
            {
                Magics.ClearClothingBonus( clothing );
                Magics.ApplyClothingBonus( clothing );
            }
        }

        public static void FixActualMagicItems()
        {
            Config.Pkg.StartWatcher( "Fixing actual magic items", true );

            foreach( Item item in World.Items.Values )
            {
                if( item is BaseArmor )
                    FixArmor( item as BaseArmor );
                else if( item is BaseWeapon )
                    FixWeapon( item as BaseWeapon );
                else if( item is BaseJewel )
                    FixJewel( item as BaseJewel );
                else if( item is BaseClothing )
                    FixClothing( item as BaseClothing );
            }

            Config.Pkg.StopWatcher( true );
        }

        public static void LogActualMagicItems( Type t, string name )
        {
            Config.Pkg.StartWatcher( "Reporting actual magic items for type " + name, true );

            Dictionary<string, int> stringDict = new Dictionary<string, int>();

            int count = 0;

            foreach( Item item in World.Items.Values )
            {
                if( item.GetType() == t || item.GetType().IsSubclassOf( t ) )
                {
                    string bonus = ComputeBonus( item );

                    if( !string.IsNullOrEmpty( bonus ) )
                    {
                        if( !stringDict.ContainsKey( bonus ) )
                            stringDict[ bonus ] = 1;
                        else
                            stringDict[ bonus ]++;

                        count++;
                    }
                }
            }

            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>( stringDict );

            list.Sort( new StringSorter() );

            using( StreamWriter op = new StreamWriter( "Logs/actual-" + name + "-magic-objects.log" ) )
            {
                op.WriteLine( "# Object count table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine( "# Count: {0}", count );
                op.WriteLine();
                op.WriteLine( "# Items:" );
                op.WriteLine();

                foreach( KeyValuePair<string, int> kvp in list )
                    op.WriteLine( "{0}\t{1:F2}%\t{2}", kvp.Value, ( 100 * kvp.Value ) / (double)count, kvp.Key );
            }

            Config.Pkg.StopWatcher( true );
        }

        public static void LogMagicItems()
        {
            Config.Pkg.StartWatcher( "Reporting magics", true );

            Dictionary<string, int> stringDict = new Dictionary<string, int>();

            int count = 0;

            while( count < 2758 )
            {
                BaseArmor armor = BuildRandomArmor() as BaseArmor;
                if( armor != null )
                {
                    Magics.ApplyArmorBonus( armor );

                    string bonus = ComputeArmorBonus( armor );

                    if( !string.IsNullOrEmpty( bonus ) )
                    {
                        if( !stringDict.ContainsKey( bonus ) )
                            stringDict[ bonus ] = 1;
                        else
                            stringDict[ bonus ]++;

                        count++;
                    }

                    armor.Delete();
                }
            }

            List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>( stringDict );

            list.Sort( new StringSorter() );

            using( StreamWriter op = new StreamWriter( "Logs/test-magic-armor.log" ) )
            {
                op.WriteLine( "# Object count table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine( "# Count: {0}", count );
                op.WriteLine();
                op.WriteLine( "# Items:" );
                op.WriteLine();

                foreach( KeyValuePair<string, int> kvp in list )
                    op.WriteLine( "{0}\t{1:F2}%\t{2}", kvp.Value, ( 100 * kvp.Value ) / (double)count, kvp.Key );
            }

            stringDict.Clear();

            count = 0;

            while( count < 3837 )
            {
                BaseWeapon weapon = BuildRandomWeapon() as BaseWeapon;
                if( weapon != null )
                {
                    Magics.ApplyWeaponBonus( weapon );

                    string bonus = ComputeWeaponBonus( weapon );

                    if( !string.IsNullOrEmpty( bonus ) )
                    {
                        if( !stringDict.ContainsKey( bonus ) )
                            stringDict[ bonus ] = 1;
                        else
                            stringDict[ bonus ]++;

                        count++;
                    }

                    weapon.Delete();
                }
            }

            list = new List<KeyValuePair<string, int>>( stringDict );

            list.Sort( new StringSorter() );

            using( StreamWriter op = new StreamWriter( "Logs/test-magic-weapon.log" ) )
            {
                op.WriteLine( "# Object count table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine( "# Count: {0}", count );
                op.WriteLine();
                op.WriteLine( "# Items:" );
                op.WriteLine();

                foreach( KeyValuePair<string, int> kvp in list )
                    op.WriteLine( "{0}\t{1:F2}%\t{2}", kvp.Value, ( 100 * kvp.Value ) / (double)count, kvp.Key );
            }

            Config.Pkg.StopWatcher( true );
        }

        private static string ComputeBonus( Item i )
        {
            if( i is BaseArmor )
                return ComputeArmorBonus( i as BaseArmor );
            else if( i is BaseWeapon )
                return ComputeWeaponBonus( i as BaseWeapon );

            return "";
        }

        private static string ComputeArmorBonus( BaseArmor armor )
        {
            bool other = armor.MagicalAttribute != ArmorMagicalAttribute.None;
            int level = (int)armor.ProtectionLevel + (int)armor.Durability;

            return level > 0 ? string.Format( "{0}{1}", level, ( other ? "+" : "" ) ) : "";
        }

        private static string ComputeWeaponBonus( BaseWeapon weapon )
        {
            bool other = weapon.MagicalAttribute != WeaponMagicalAttribute.None;
            int level = (int)weapon.AccuracyLevel + (int)weapon.DamageLevel + (int)weapon.DurabilityLevel;

            return level > 0 ? string.Format( "{0}{1}", level, ( other ? "+" : "" ) ) : "";
        }

        public static void LogItems()
        {
            Dictionary<Type, int> dict = new Dictionary<Type, int>();

            int count = 0;

            for( int i = 0; i < 10000000; i++ )
            {
                Item item = BuildRandomItem();

                if( item != null )
                {
                    Type t = item.GetType();

                    if( !dict.ContainsKey( t ) )
                        dict[ t ] = 1;
                    else
                        dict[ t ]++;

                    count++;

                    item.Delete();
                }
            }

            List<KeyValuePair<Type, int>> list = new List<KeyValuePair<Type, int>>( dict );

            list.Sort( new CountSorter() );

            using( StreamWriter op = new StreamWriter( "Logs/objects.log" ) )
            {
                op.WriteLine( "# Object count table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine( "# Count: {0}", count );

                op.WriteLine( "# Items:" );

                foreach( KeyValuePair<Type, int> kvp in list )
                    op.WriteLine( "{0}\t{1:F2}%\t{2}", kvp.Value, ( 100 * kvp.Value ) / (double)count, kvp.Key );
            }

            Console.WriteLine( "Done." );
        }

        private class CountSorter : IComparer<KeyValuePair<Type, int>>
        {
            public int Compare( KeyValuePair<Type, int> a, KeyValuePair<Type, int> b )
            {
                int v = -a.Value.CompareTo( b.Value );

                if( v == 0 )
                    if( a.Key.FullName != null )
                        v = a.Key.FullName.CompareTo( b.Key.FullName );

                return v;
            }
        }

        private class StringSorter : IComparer<KeyValuePair<string, int>>
        {
            public int Compare( KeyValuePair<string, int> a, KeyValuePair<string, int> b )
            {
                int v = -a.Value.CompareTo( b.Value );

                if( v == 0 )
                    v = a.Key.CompareTo( b.Key );

                return v;
            }
        }
        #endregion

        #region [accessors]

        /// <summary>
        /// weapons 3
        /// armor 3
        /// scroll 3
        /// targetables 2
        /// wearables 2
        /// activators 1
        /// reagents 1
        /// </summary>
        public static readonly LootTable RandomItem = new LootTable( new TableEntry[]
                                                                     {
                                                                         new TableEntry( OldWeaponsLoot.Weapons, 3 ),
                                                                         new TableEntry( OldArmorsLoot.Armors, 3 ),
                                                                         new TableEntry( OldMiscLoot.Scrolls, 3 ),
                                                                         new TableEntry( OldMiscLoot.GetTargetable, 2 ),
                                                                         new TableEntry( OldWearableLoot.Wearable, 2 ),
                                                                         new TableEntry( OldMiscLoot.GetActivator, 1 ),
                                                                         new TableEntry( OldMiscLoot.Reagents, 1 )
                                                                     } );

        public static Item BuildRandomItem()
        {
            return RandomItem.Construct();
        }

        public static Item BuildRandomWeapon()
        {
            return OldWeaponsLoot.Weapons.Construct();
        }

        public static Item BuildRandomReagent()
        {
            return OldMiscLoot.Reagents.Construct();
        }

        public static Item BuildRandomActivators()
        {
            return OldMiscLoot.GetActivator.Construct();
        }

        public static Item BuildRandomTargetable()
        {
            return OldMiscLoot.GetTargetable.Construct();
        }

        public static Item BuildRandomScroll()
        {
            return OldMiscLoot.Scrolls.Construct();
        }

        public static Item BuildRandomArmor()
        {
            return OldArmorsLoot.Armors.Construct();
        }

        public static Item BuildRandomWearable()
        {
            return OldWearableLoot.Wearable.Construct();
        }

        public static Item BuildRandomJewel()
        {
            return OldWearableLoot.GetJewelry.Construct();
        }

        public static Item BuildRandomClothing()
        {
            return OldWearableLoot.GetClothing.Construct();
        }

        #endregion
    }
}