using System;

namespace Midgard.Engines.SpeechLog
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

        public static bool Debug = true;

        public static object[] Package_Info = {
            "Script Title",             "Advanced Speech Log System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.SpeechLog"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"SpeechLog"}
        };

        public static void Configure()
        {
        }

        public static void Initialize()
        {
            if( Enabled )
            {
                Core.RegisterCommands();
                Core.InitSystem();
            }
        }
    }
}