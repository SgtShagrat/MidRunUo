using System;

namespace Midgard.Engines.TalkingPlayerVendorSystem
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
            "Script Title",             "Talking Player Vendors System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.TalkingPlayerVendorSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Talking Player Vendors"}
        };

        internal static readonly int MinDelayMessage = 10;
        internal static readonly int MaxDelayMessage = 20;
        internal static readonly int MessageRange = 4;
        internal static readonly int ChanceToShout = 101;
        internal static readonly int MaxNumberOfShouts = 5;

        public static void Package_Configure()
        {
        }

        public static void Package_Initialize()
        {
        }
    }
}