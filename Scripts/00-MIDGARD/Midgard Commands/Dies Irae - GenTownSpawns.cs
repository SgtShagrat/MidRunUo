using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class GenTownSpawnsCommand
    {
        private static readonly List<XmlSpawner> m_Spawners = new List<XmlSpawner>();
        private static readonly Dictionary<Point3D, List<SpawnObject>> m_Vendors = new Dictionary<Point3D, List<SpawnObject>>();

        public static void Initialize()
        {
            CommandSystem.Register( "GenTownSpawns", AccessLevel.Developer, GenTownSpawns_OnCommand );
        }

        [Usage( "GenTownSpawns" )]
        [Description( "Generate spawn reports for all town systems" )]
        public static void GenTownSpawns_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendAsciiMessage( 0x35, "Town xml-spawns documentation is being generated, please wait." );

            NetState.FlushAll();
            NetState.Pause();

            Stopwatch watch = Stopwatch.StartNew();

            TownSystem[] systems = TownSystem.TownSystems;

            foreach( Item i in World.Items.Values )
            {
                if( i == null || i.Deleted || !( i is XmlSpawner ) )
                    continue;

                m_Spawners.Add( i as XmlSpawner );
            }

            foreach( TownSystem system in systems )
                Generate( system );

            NetState.Resume();

            if( watch.IsRunning )
                watch.Stop();

            e.Mobile.SendAsciiMessage( 0x35, "Documentation has been completed. The entire process took {0:F2} seconds.",
                                       watch.Elapsed.TotalSeconds );
        }

        private static void EnsureDirectory( string path )
        {
            path = Path.Combine( Core.BaseDirectory, path );

            if( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
        }

        private static void Generate( TownSystem system )
        {
            var list = new List<XmlSpawner>();

            foreach( XmlSpawner spawner in m_Spawners )
            {
                if( TownSystem.Find( spawner.Location, spawner.Map ) == system )
                    list.Add( spawner );
            }

            list.Sort( XmlSpawnerComparer.Instance );

            EnsureDirectory( "Logs/TownSystem/" );

            m_Vendors.Clear();

            foreach( XmlSpawner spawner in list )
            {
                if( spawner == null || spawner.Deleted )
                    continue;

                foreach( SpawnObject spawnObject in spawner.SpawnObjects )
                {
                    if( spawnObject == null )
                        continue;

                    string str = spawnObject.TypeName.Trim();
                    string typestr = BaseXmlSpawner.ParseObjectType( str );
                    if( typestr == null )
                        continue;

                    Type type = SpawnerType.GetType( typestr );
                    if( type == null )
                        continue;

                    if( !type.IsSubclassOf( typeof( BaseVendor ) ) )
                        continue;

                    if( m_Vendors.ContainsKey( spawner.Location ) )
                        m_Vendors[ spawner.Location ].Add( spawnObject );
                    else
                    {
                        var instances = new List<SpawnObject>();
                        instances.Add( spawnObject );
                        m_Vendors.Add( spawner.Location, instances );
                    }
                }
            }

            ScriptCompiler.EnsureDirectory( "Logs/TownSystem/Spawns" );

            using( var op = new StreamWriter( Path.Combine( "Logs/TownSystem/Spawns", string.Format( "{0}.log", system.Definition.Town ) ) ) )
            {
                op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                op.WriteLine( "Total xmlspawner processed {0}", list.Count );
                op.WriteLine( "" );

                foreach( XmlSpawner spawner in list )
                {
                    try
                    {
                        if( spawner == null || spawner.Deleted )
                            continue;

                        op.WriteLine( "Name: {0}", spawner.Name ?? "nullName" );
                        op.WriteLine( "Location: {0}", spawner.Location );
                        op.WriteLine( "MaxCount: {0}", spawner.MaxCount );

                        var spawnObjects = new List<SpawnObject>( spawner.SpawnObjects );

                        op.WriteLine( "Number of valid entries: {0}", spawnObjects.Count );
                        spawnObjects.Sort( SpawnObjectComparer.Instance );

                        foreach( SpawnObject spawnObject in spawnObjects )
                        {
                            if( spawnObject != null )
                                op.WriteLine( spawnObject.TypeName );
                        }

                        op.WriteLine( "" );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }

                op.WriteLine( "Vendors:" );
                foreach( var keyValuePair in m_Vendors )
                {
                    foreach( SpawnObject spawnObject in keyValuePair.Value )
                        op.WriteLine( string.Format( "{0}/Location/{1}", spawnObject.TypeName, keyValuePair.Key ) );
                }
            }
        }

        private class SpawnObjectComparer : IComparer<SpawnObject>
        {
            public static readonly IComparer<SpawnObject> Instance = new SpawnObjectComparer();

            private SpawnObjectComparer()
            {
            }

            public int Compare( SpawnObject x, SpawnObject y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.TypeName, y.TypeName );
            }
        }

        private class XmlSpawnerComparer : IComparer<XmlSpawner>
        {
            public static readonly IComparer<XmlSpawner> Instance = new XmlSpawnerComparer();

            private XmlSpawnerComparer()
            {
            }

            public int Compare( XmlSpawner x, XmlSpawner y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }
    }
}