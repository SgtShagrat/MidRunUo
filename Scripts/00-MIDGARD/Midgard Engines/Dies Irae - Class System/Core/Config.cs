using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.Classes
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static readonly string ClassSystemSavePath = Path.Combine( Path.Combine( "Saves", "ClassSystem" ), "ClassSystemSave.bin" );

        public static readonly bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Class System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.Classes" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Classes" }
                                              };

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];
            ClassSystem.RegisterEventSink();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                ClassSystemCommands.RegisterCommands();
                DruidLocationEntry.RegisterCommands();
                DruidLocationEntry.LoadEntries();
		NecroLocationEntry.RegisterCommands();
		NecroLocationEntry.LoadEntries();
                RodToSnakeSystem.RegisterCommands();
                SkillBonuses.RegisterHandler();
                WebCommands.RegisterCommands();
            }
        }
    }
}