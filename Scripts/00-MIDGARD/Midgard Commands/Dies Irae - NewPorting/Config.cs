using System;

using Midgard.Engines.Packager;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }


        public static readonly bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Third Crown Porting",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.ThirdCrownPorting" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Third Crown Porting" }
                                              };

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                // GetSerial.RegisterCommand();
                // ConvertServerToPreAos.RegisterCommands();
                // GetPortingItems.RegisterCommands();
                // PlayerExportManager.RegisterCommands();
                PlayerImportManager.RegisterCommands();
                PortingLogger.RegisterCommands();
            }
        }
    }
}