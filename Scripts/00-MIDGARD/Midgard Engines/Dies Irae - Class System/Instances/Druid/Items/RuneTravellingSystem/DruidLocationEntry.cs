/***************************************************************************
 *                               DruidLocationEntry.cs
 *
 *   begin                : 29 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;

namespace Midgard.Engines.Classes
{
    public class DruidLocationEntry
    {
        public static DruidLocationEntry[] DruidLocations = new DruidLocationEntry[] { };

        public Map Map { get; private set; }
        public Point3D Location { get; private set; }
        public string Name { get; private set; }
        public bool Enabled { get; set; }

        public DruidLocationEntry( Map map, Point3D loc, string name, bool enabled )
        {
            Map = map;
            Location = loc;
            Name = name;
            Enabled = enabled;
        }

        public static DruidLocationEntry Parse( XmlElement node )
        {
            Map map = Map.Felucca;
            Point3D location = Point3D.Zero;
            string name = "";
            bool enabled = true;

            Region.ReadMap( node, "map", ref map, true );
            Region.ReadPoint3D( node, map, ref location, true );
            Region.ReadString( node, "name", ref name, true );
            Region.ReadBoolean( node, "enabled", ref enabled, false );

            return new DruidLocationEntry( map, location, name, enabled );
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "GenDruidTravelStones", AccessLevel.Administrator, new CommandEventHandler( GenerateStones ) );
        }

        [Usage( "GenDruidTravelStones" )]
        [Description( "Generates druidic stones for travel system." )]
        private static void GenerateStones( CommandEventArgs e )
        {
            int i = 0;

            foreach( DruidLocationEntry entry in DruidLocations )
            {
                if( !FindItem( entry.Location, entry.Map, 0x1363 ) )
                {
                    DruidTravellingStone stone = new DruidTravellingStone();
                    stone.MoveToWorld( entry.Location, entry.Map );
                    stone.Movable = false;
                    i++;
                }
            }

            e.Mobile.SendMessage( "Exactly {0} druid travel stones have been generated.", i );
        }

        internal static void LoadEntries()
        {
            if( !File.Exists( "Data/druidRuneTravellingDefinitions.xml" ) )
            {
                Config.Pkg.LogErrorLine( "Data/druidRuneTravellingDefinitions.xml does not exist" );
                return;
            }

            Config.Pkg.LogInfo( "Druid Definitions: Loading..." );

            XmlDocument doc = new XmlDocument();
            doc.Load( Path.Combine( Core.BaseDirectory, "Data/druidRuneTravellingDefinitions.xml" ) );

            XmlElement root = doc[ "definitions" ];
            int disabled = 0;

            if( root == null )
                Config.Pkg.LogErrorLine( "Could not find root element 'definitions' in druidRuneTravellingDefinitions.xml" );
            else
            {
                List<DruidLocationEntry> defList = new List<DruidLocationEntry>();

                foreach( XmlElement info in root.SelectNodes( "entry" ) )
                {
                    DruidLocationEntry entry = Parse( info );
                    if( !entry.Enabled )
                        disabled++;

                    defList.Add( entry );
                }

                DruidLocations = defList.ToArray();
            }

            Config.Pkg.LogInfoLine( "done" );
            Config.Pkg.LogInfoLine( "Druid system has {0} travelling runes, {1} are disabled.", DruidLocations.Length, disabled );
        }

        public static DruidLocationEntry Find( Point3D point )
        {
            foreach( DruidLocationEntry entry in DruidLocations )
            {
                if( entry.Location == point )
                    return entry;
            }

            return null;
        }

        public static bool FindItem( Point3D p, Map map, int itemID )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p );

            foreach( Item item in eable )
            {
                if( item.Z == p.Z && item.ItemID == itemID )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }
    }
}