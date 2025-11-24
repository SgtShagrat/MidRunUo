using System.Collections.Generic;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;

namespace Midgard.Engines.Classes
{
    public class NecroLocationEntry
    {
        public static NecroLocationEntry[] NecroLocations = new NecroLocationEntry[] { };

        public Map Map { get; private set; }
        public Point3D Location { get; private set; }
        public string Name { get; private set; }
        public bool Enabled { get; set; }

        public NecroLocationEntry( Map map, Point3D loc, string name, bool enabled )
        {
            Map = map;
            Location = loc;
            Name = name;
            Enabled = enabled;
        }

        public static NecroLocationEntry Parse( XmlElement node )
        {
            Map map = Map.Felucca;
            Point3D location = Point3D.Zero;
            string name = "";
            bool enabled = true;

            Region.ReadMap( node, "map", ref map, true );
            Region.ReadPoint3D( node, map, ref location, true );
            Region.ReadString( node, "name", ref name, true );
            Region.ReadBoolean( node, "enabled", ref enabled, false );

            return new NecroLocationEntry( map, location, name, enabled );
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "GenNecroTravelStones", AccessLevel.Administrator, new CommandEventHandler( GenerateStones ) );
        }

        [Usage( "GenNecroTravelStones" )]
        [Description( "Generates necromantic stones for travel system." )]
        private static void GenerateStones( CommandEventArgs e )
        {
            int i = 0;

            foreach( NecroLocationEntry entry in NecroLocations )
            {
                if( !FindItem( entry.Location, entry.Map, 0x1363 ) )
                {
                    NecroTravellingStone stone = new NecroTravellingStone();
                    stone.MoveToWorld( entry.Location, entry.Map );
                    stone.Movable = false;
                    i++;
                }
            }

            e.Mobile.SendMessage( "Exactly {0} necro travel stones have been generated.", i );
        }

        internal static void LoadEntries()
        {
            if( !File.Exists( "Data/necroTravellingDefinitions.xml" ) )
            {
                Config.Pkg.LogErrorLine( "Data/necroTravellingDefinitions.xml does not exist" );
                return;
            }

            Config.Pkg.LogInfo( "Necro Definitions: Loading..." );

            XmlDocument doc = new XmlDocument();
            doc.Load( Path.Combine( Core.BaseDirectory, "Data/necroTravellingDefinitions.xml" ) );

            XmlElement root = doc[ "definitions" ];
            int disabled = 0;

            if( root == null )
                Config.Pkg.LogErrorLine( "Could not find root element 'definitions' in necroTravellingDefinitions.xml" );
            else
            {
                List<NecroLocationEntry> defList = new List<NecroLocationEntry>();

                foreach( XmlElement info in root.SelectNodes( "entry" ) )
                {
                    NecroLocationEntry entry = Parse( info );
                    if( !entry.Enabled )
                        disabled++;

                    defList.Add( entry );
                }

                NecroLocations = defList.ToArray();
            }

            Config.Pkg.LogInfoLine( "done" );
            Config.Pkg.LogInfoLine( "Necro system has {0} travelling runes, {1} are disabled.", NecroLocations.Length, disabled );
        }

        public static NecroLocationEntry Find( Point3D point )
        {
            foreach( NecroLocationEntry entry in NecroLocations )
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