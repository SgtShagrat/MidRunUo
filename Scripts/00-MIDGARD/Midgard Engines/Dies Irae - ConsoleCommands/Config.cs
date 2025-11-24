/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Packager;

using Server;

namespace Midgard.Engines.ConsoleCommands
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static object[] Package_Info = {
                                                  "Script Title", "Console Commands",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.ConsoleCommands" },
                                                  "Research tags", new string[] { "Console Commands" }
                                              };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                CCPersistance.EnsureExistence();
                Core.RegisterSinks();
                Core.InitializeCommandsList();
            }
        }

        internal static readonly AccessLevel ConsoleAccessLevel = AccessLevel.Developer;
        internal static readonly ConsoleColor ConsoleMessageColor = ConsoleColor.Green;
        internal static bool HearConsole = false;
    }
}