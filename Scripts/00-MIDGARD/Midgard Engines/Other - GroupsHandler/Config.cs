using System;

namespace Midgard.Engines.GroupsHandler
{
    public class Config
    {
        public static bool Enabled
        {
            get
            {
                return Packager.Core.Singleton[ typeof( Config ) ].Enabled;
            }
            set
            {
                Packager.Core.Singleton[ typeof( Config ) ].Enabled = value;
            }
        }

        internal static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Groups Handler System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Revised by Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.GroupsHandler"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Groups Handler"}
        };

        public static void Package_Configure()
        {
            if( Enabled )
                GroupsHandler.ConfigSystem();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                FindDecorations.RegisterCommands();
                GroupsHandler.RegisterCommands();
                GroupsHandler.CreateDefaults();
            }
        }
    }
}