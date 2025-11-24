using System;

using Midgard.Engines.Packager;

namespace Midgard.Engines.StoneEnchantSystem
{
    public class Config
    {
        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Midgard Stone Enchant System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae - Magius(CHE)",
            "Provided packages",        new string[]{"Midgard.Engines.StoneEnchantSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Stone Enchant System"}
        };

        internal static Package Pkg;

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void SendDebug( string format, params object[] args )
        {
            if( Debug )
                SendDebug( String.Format( format, args ) );
        }

        public static void SendDebug( string log )
        {
            if( Debug )
                Pkg.LogInfoLine( log );
        }
    }
}