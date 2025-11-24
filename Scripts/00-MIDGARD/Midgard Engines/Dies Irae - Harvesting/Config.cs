/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Packager;

namespace Midgard.Engines.Harvesting
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Advanced Harvesting System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.Harvesting"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Harvesting"}
        };

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                HarvestSpawnDefinition.RegisterCommands();

                HarvestSpawnDefinition.Load();

                RegionalHarvestDefinition.RegisterCommands();

                RegionalHarvestDefinition.Load();
            }
        }
    }
}