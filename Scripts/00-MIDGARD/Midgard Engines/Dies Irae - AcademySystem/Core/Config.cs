/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.Academies
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static string SiteGuide = "www.midgardshard.it";

        public static readonly string AcademySystemSavePath = Path.Combine( Path.Combine( "Saves", "AcademySystem" ), "AcademySystemSave.bin" );

        public static readonly bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Academy System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2010, 11, 05 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.Academies" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Academies" }
                                              };

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];

            AcademySystem.RegisterEventSink();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                AcademySystemCommands.RegisterCommands();
                // WebCommands.RegisterCommands();
            }
        }
    }
}