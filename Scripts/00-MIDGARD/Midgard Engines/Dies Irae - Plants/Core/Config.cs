using System;

using Midgard.Engines.Packager;

using Server;

using Core = Midgard.Engines.Packager.Core;

namespace Midgard.Engines.PlantSystem
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = true;

        public static object[] Package_Info = {
            "Script Title",             "Midgard Plant System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.PlantSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Plants"}
        };

        internal static bool LogEnabled = true;

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            Timer t = Timer.DelayCall( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromMinutes( 5.0 ), new TimerCallback( BasePlant.Grow_OnTick ) );
            t.Priority = TimerPriority.OneMinute;
        }
    }
}