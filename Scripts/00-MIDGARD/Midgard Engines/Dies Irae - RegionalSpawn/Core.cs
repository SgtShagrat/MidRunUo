/***************************************************************************
 *                                      Core.cs
 *
 *   begin                : 24 aprile 2012
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Server;
using Server.Regions;


namespace Midgard.Engines.RegionalSpawningSystem
{
    public class Core
    {
        public static Dictionary<string, SpawnEntry> Table { get { return SpawnEntry.Table; } }

        /*
        <spawning density="Normal" area="13197" numToSpawn="8">
            <group name="ForestedIslandAnimals" id="20014" minSpawnTime="PT10S" maxSpawnTime="PT30S" amount="5" weight="5" weightNormalized="56" />
            <group name="ForestedIslandMonsters" id="20015" minSpawnTime="PT05M" maxSpawnTime="PT20M" amount="3" weight="3" weightNormalized="33" density="Low"/>
            <group name="Healers" id="20016" minSpawnTime="PT05M" maxSpawnTime="PT20M" amount="1" weight="1" weightNormalized="11" forceAmount="1"/>
        </spawning>
        */

        public static List<SpawnEntry> GetSpawnEntries( BaseRegion region )
        {
            List<SpawnEntry> entries = new List<SpawnEntry>();

            if( region == null || region.Spawns == null )
                return entries;

            entries.AddRange( region.Spawns );
            return entries;
        }

        public static List<SpawnGroup> GetSpawnGroups()
        {
            List<SpawnGroup> allGroups = new List<SpawnGroup>();

            foreach( DictionaryEntry entry in SpawnGroup.Table )
                allGroups.Add( (SpawnGroup)entry.Value );

            allGroups.Sort();

            return allGroups;
        }

        public static string GetEntryName( SpawnEntry entry )
        {
            return entry.ToString();
        }

        public static int GetRunningSpawnEntries( SpawnEntry[] entries )
        {
            if( entries == null )
                return 0;

            int count = 0;
            foreach( SpawnEntry entry in entries )
            {
                if( entry.Running )
                    count++;
            }
            return count;
        }

        public static int GetSpawningSpawnEntries( SpawnEntry[] entries )
        {
            if( entries == null )
                return 0;

            int count = 0;
            foreach( SpawnEntry entry in entries )
            {
                if( entry.Spawning )
                    count++;
            }
            return count;
        }

        public static void HandleSpawnDefinition( BaseRegion r, ref XmlElement spawning, ref List<SpawnEntry> list )
        {
            if( spawning == null || list == null || list.Count <= 0 )
                return;

            int weightSum = 0;
            foreach( SpawnEntry spawnEntry in list )
                weightSum += spawnEntry.Max;

            NormalizeWeights( ref spawning, weightSum );

            UpdateSpawnAmount( r, ref list, weightSum );

            UpdateSpawnDensity( r, spawning );

            LogSpawnDensity( r );
        }

        public const int SpawnDensityDivisor = 5000;

        public static int NumToSpawn( BaseRegion region )
        {
            // var sizeofregion := (x2 - x1) * (y2 - y1);
            // var numtospawn := cint((sizeofregion * groupdensity)/10000);

            return (int)Math.Ceiling( ( region.SizeOfRegion * (int)region.Density ) / (double)SpawnDensityDivisor );
        }

        public static int NumToSpawn( BaseRegion region, SpawnDensity customDensity )
        {
            return (int)Math.Ceiling( ( region.SizeOfRegion * (int)customDensity ) / (double)SpawnDensityDivisor );
        }

        public static void UpdateSpawnAmount( BaseRegion r, ref List<SpawnEntry> spawns, int weightSum )
        {
            foreach( SpawnEntry spawnEntry in spawns )
            {
                if( spawnEntry.ForceAmount > 0 )
                    spawnEntry.RelativeAmount = spawnEntry.ForceAmount;
                else
                {
                    int toSpawn = NumToSpawn( r, spawnEntry.Density );
                    spawnEntry.RelativeAmount = (int)Math.Ceiling( ( toSpawn / (double)weightSum ) * spawnEntry.Max );
                }
            }
        }

        public static void UpdateSpawnDensity( BaseRegion r, XmlElement spawning )
        {
            if( r == null )
                return;
            SpawnDensity oldDensity = r.Density;

            int real = 0;
            int relative = NumToSpawn( r );

            if( r.Spawns == null )
                return;

            foreach( SpawnEntry spawnEntry in r.Spawns )
                real += spawnEntry.Max;

            int scalar = real / relative;

            if( scalar <= (int)SpawnDensity.VeryLow )
                r.Density = SpawnDensity.VeryLow;
            else if( scalar <= (int)SpawnDensity.Low )
                r.Density = SpawnDensity.Low;
            else if( scalar <= (int)SpawnDensity.Normal )
                r.Density = SpawnDensity.Normal;
            else if( scalar <= (int)SpawnDensity.Dense )
                r.Density = SpawnDensity.Dense;
            else
                r.Density = SpawnDensity.VeryDense;

            if( r.Density != oldDensity )
                Region.RegionsFileHasBeenModified = true;
        }

        private static void LogSpawnDensity( BaseRegion region )
        {
            if( region == null )
                return;

            if( region.Spawns == null )
                return;

            using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "regional-spawns.log" ), true ) )
            {
                op.WriteLine( string.Format( "region: {0} - area {1} - density {2} - max {3}.",
                    region.Name, region.SizeOfRegion, region.Density, NumToSpawn( region ) ) );

                List<SpawnEntry> localSpawns = new List<SpawnEntry>( region.Spawns );

                foreach( SpawnEntry entry in localSpawns )
                    op.WriteLine( "\t{0} (max {1} - relative {2})", entry.Definition, entry.Max, entry.RelativeAmount );

                op.WriteLine( "" );
            }
        }

        public static void NormalizeWeights( ref XmlElement spawning, int weightSum )
        {
            foreach( XmlNode node in spawning.ChildNodes )
            {
                XmlElement el = node as XmlElement;
                if( el == null )
                    continue;

                SpawnDefinition def = SpawnDefinition.GetSpawnDefinition( el );
                if( def == null )
                    continue;

                int weight = 0;
                if( !Region.ReadInt32( el, "weight", ref weight, true ) )
                    continue;

                if( !Region.RegionsFileHasBeenModified )
                    Region.RegionsFileHasBeenModified = true;

                // weight : weightSum = x : 100
                el.SetAttribute( "weightNormalized", Math.Round( 100 * ( weight / (double)weightSum ), 0 ).ToString() );
            }
        }

        public static void Snapshot()
        {
            XDocument doc = new XDocument(
                new XDeclaration( "1.0", "utf-8", "yes" ),
                SpawnGroup.AllGroupsToXElement() );

            ScriptCompiler.EnsureDirectory( "Data" );
            string fileName = String.Format( "SpawnDefinitions_{0}.xml", GetTimeStamp() );
            doc.Save( Path.Combine( "Data", fileName ) );
        }

        public static void AAA( string docName, string groupName, SpawnGroup group )
        {
            XmlDocument doc = new XmlDocument();
            doc.Load( Path.Combine( "Data", "{0}.xml" + docName ) );

            XmlElement root = doc[ "spawnDefinitions" ];
            if( root == null )
                return;

            UpdateNode( root, groupName, group );

            doc.Save( String.Format( "{0}_{1}.xml", docName, GetTimeStamp() ) );
        }

        public static void UpdateNode( XmlElement root, string groupName, SpawnGroup val )
        {
            if( root == null )
                return;

            foreach( XmlElement element in root.SelectNodes( "spawnGroup" ) )
            {
                //if( element.HasAttribute( "name" ) && element["name"].Value == groupName )
                //    element.SetAttribute( "active", XmlConvert.ToString( val ) );
            }
        }

        public static void SaveXml( XmlDocument document, string name )
        {
            string fileName = String.Format( "{0}_{1}.xml", name, GetTimeStamp() );
            string path = Path.Combine( "Data", fileName );

            try
            {
                document.Save( path );
            }
            catch( Exception e )
            {
                Console.WriteLine( "Failed writing {0} file.", fileName );
                Console.WriteLine( e );
            }
        }

        private static string GetTimeStamp()
        {
            DateTime now = DateTime.Now;

            return String.Format( "{0}-{1}-{2}_{3}-{4:D2}-{5:D2}", now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second );
        }

        #region single entry handlers
        /// <summary>
        /// "Respawns the entry and sets the spawners as running."
        /// </summary>
        internal static void RespawnEntry( Mobile from, SpawnEntry entry )
        {
            entry.Respawn();

            from.SendMessage( "Entry has respawned." );
        }

        /// <summary>
        /// "Deletes all spawned objects of the entry and sets the spawners as not running."
        /// </summary>
        internal static void DelEntrySpawns( Mobile from, SpawnEntry entry )
        {
            entry.DeleteSpawnedObjects();

            from.SendMessage( "Spawned objects of this entry have been deleted." );
        }

        /// <summary>
        /// "Sets the region spawners of the entry as running."
        /// </summary>
        internal static void StartEntrySpawns( Mobile from, SpawnEntry entry )
        {
            entry.Start();

            from.SendMessage( "Entry spawners have started." );
        }

        /// <summary>
        /// "Sets the region spawners of the entry as not running."
        /// </summary>
        internal static void StopEntrySpawns( Mobile from, SpawnEntry entry )
        {
            entry.Stop();

            from.SendMessage( "Entry spawners have stopped." );
        }
        #endregion

        #region single region handlers
        /// <summary>
        /// "Respawns the region in which you are (or that you provided) and sets the spawners as running."
        /// </summary>
        internal static void RespawnRegion( Mobile from, BaseRegion region )
        {
            foreach( SpawnEntry t in region.Spawns )
                t.Respawn();

            from.SendMessage( "Region '{0}' has respawned.", region );
        }

        /// <summary>
        /// "Deletes all spawned objects of the region in which you are (or that you provided) and sets the spawners as not running."
        /// </summary>
        internal static void DelRegionSpawns( Mobile from, BaseRegion region )
        {
            foreach( SpawnEntry t in region.Spawns )
                t.DeleteSpawnedObjects();

            from.SendMessage( "Spawned objects of region '{0}' have been deleted.", region );
        }

        /// <summary>
        /// "Sets the region spawners of the region in which you are (or that you provided) as running."
        /// </summary>
        internal static void StartRegionSpawns( Mobile from, BaseRegion region )
        {
            foreach( SpawnEntry t in region.Spawns )
                t.Start();

            from.SendMessage( "Spawners of region '{0}' have started.", region );
        }

        /// <summary>
        /// "Sets the region spawners of the region in which you are (or that you provided) as not running."
        /// </summary>
        internal static void StopRegionSpawns( Mobile from, BaseRegion region )
        {
            foreach( SpawnEntry t in region.Spawns )
                t.Stop();

            from.SendMessage( "Spawners of region '{0}' have stopped.", region );
        }
        #endregion

        #region all region handlers
        /// <summary>
        /// "Respawns all regions and sets the spawners as running."
        /// </summary>
        internal static void RespawnAllRegions( Mobile from )
        {
            foreach( SpawnEntry entry in Table.Values )
                entry.Respawn();

            from.SendMessage( "All regions have respawned." );
        }

        /// <summary>
        /// "Deletes all spawned objects of every regions and sets the spawners as not running."
        /// </summary>
        internal static void DelAllRegionSpawns( Mobile from )
        {
            foreach( SpawnEntry entry in Table.Values )
                entry.DeleteSpawnedObjects();

            from.SendMessage( "All region spawned objects have been deleted." );
        }

        /// <summary>
        /// "Sets the region spawners of all regions as running."
        /// </summary>
        internal static void StartAllRegionSpawns( Mobile from )
        {
            foreach( SpawnEntry entry in Table.Values )
                entry.Start();

            from.SendMessage( "All region spawners have started." );
        }

        /// <summary>
        /// "Sets the region spawners of all regions as not running."
        /// </summary>
        internal static void StopAllRegionSpawns( Mobile from )
        {
            foreach( SpawnEntry entry in Table.Values )
                entry.Stop();

            from.SendMessage( "All region spawners have stopped." );
        }

        /// <summary>
        /// "List all the regional spawns to a file named regionalSpawnings.log"
        /// </summary>
        internal static void ListRegionSpawns( Mobile from )
        {
            List<SpawnEntry> entries = new List<SpawnEntry>( Table.Values );

            entries.Sort( new Sorter() );

            using( StreamWriter op = new StreamWriter( Config.SpawnsDocPath ) )
            {
                foreach( SpawnEntry entry in entries )
                    op.WriteLine( "{0} - \"{1}\" ({2})", entry.ID, entry.Definition, entry.Region.Name ?? "none" );
            }

            using( StreamWriter op = new StreamWriter( Config.SpawnsByRegionDocPath ) )
            {
                foreach( Region region in Region.Regions )
                {
                    BaseRegion r = region as BaseRegion;
                    if( r == null )
                        continue;

                    if( r.Spawns == null || r.Spawns.Length <= 0 )
                        continue;

                    List<SpawnEntry> localSpawns = new List<SpawnEntry>( r.Spawns );

                    op.WriteLine( "Region: {0} Area: {1}", r.Name, region.Area );

                    foreach( SpawnEntry entry in localSpawns )
                        op.WriteLine( "\t{0} {1}", entry.Definition, entry.Max );

                    op.WriteLine( "" );
                }
            }
        }

        private class Sorter : IComparer<SpawnEntry>
        {
            public int Compare( SpawnEntry x, SpawnEntry y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.ID, y.ID );
            }
        }
        #endregion
    }
}