using System;
using System.IO;
using Midgard.Engines.Packager;
using Server;
using Ultima;
using Core = Midgard.Engines.Packager.Core;

namespace Midgard.Engines.MapRenderizer
{
    public class Config
    {
#if DEBUG
        public static bool Debug = true;
#else
        public static bool Debug = false;
#endif
        public static object[] Package_Info =
            {
                "Script Title", "Map Renderizer",
                "Enabled by Default", true,
                "Script Version", new Version(1, 0, 3, 0),
                "Author name", "Magius(CHE)",
                "Creation Date", new DateTime(2009, 09, 04),
                "Author mail-contact", "cheghe@tiscali.it",
                "Author home site", "http://www.magius.it",
                //"Author notes",           null,
                "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
                "Provided packages", new string[] {"Midgard.Engines.MapRenderizer"},
                //"Required packages",       new string[]{"Midgard.Engines.MyMidgard"},
                //"Conflicts with packages",new string[0],
                "Research tags", new string[] {"Map", "Render", "Renderizer", "Engine"}
            };

        internal static Package Pkg;

        private static MapRenderer m_Renderer = null;

        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

        public static string DataPath
        {
            get { return Path.GetFullPath( Path.Combine( Path.Combine( ".", "Data" ), "MapRenderizer" ) ); }
        }

        public static string SavePath
        {
            get { return Path.GetFullPath( Path.Combine( Path.Combine( ".", "." ), "MapRenderizer" ) ); }
        }

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
            var localmuls = Path.GetFullPath( "muls" );
            if( Directory.Exists( localmuls ) )
            {
                Pkg.LogInfoLine( "Directory \"{0}\" found. Ultima.dll will be setted on it.", localmuls );
                Client.Directories.Clear();
                Client.Directories.Add( localmuls );
                Client.Directories.Add( localmuls );
            }
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                m_Renderer = new MapRenderer();
                EventSink.Shutdown += new ShutdownEventHandler( EventSink_Shutdown );
                EventSink.Crashed += new CrashedEventHandler( EventSink_Crashed );
            }
        }

        private static void EventSink_Crashed( CrashedEventArgs e )
        {
            if( m_Renderer != null )
                m_Renderer.Dispose();
        }

        private static void EventSink_Shutdown( ShutdownEventArgs e )
        {
            if( m_Renderer != null )
                m_Renderer.Dispose();
        }

        internal static void PathEnsureExistance( string filepath )
        {
            var parent = Path.GetDirectoryName( filepath );
            if( !Directory.Exists( parent ) )
                Directory.CreateDirectory( parent );
        }
    }
}