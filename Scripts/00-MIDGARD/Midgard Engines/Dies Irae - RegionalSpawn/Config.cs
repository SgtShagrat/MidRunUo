/***************************************************************************
 *                               Config.cs
 *
 *   begin                : Aprile 2012
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

namespace Midgard.Engines.RegionalSpawningSystem
{
    public class Config
    {
        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Midgard Regional Spawn System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2012, 04, 24 ), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.RegionalSpawningSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"RegionalSpawningSystem System"}
        };

        public static void Package_Initialize()
        {
            if( Enabled )
                Commands.RegisterCommands();
        }

        public static string SpawnsDocPath = Path.Combine( "docs3c", "regionalSpawnings.log" );
        public static string SpawnsByRegionDocPath = Path.Combine( "docs3c", "regionalSpawningsByRegion.log" );

        public static string Guide = "www.midgardshard.it";
    }
}