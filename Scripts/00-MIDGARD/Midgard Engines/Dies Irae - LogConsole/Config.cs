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
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.LogConsole
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
                                                "Script Title", "Log Console System",
                                                "Enabled by Default", true, 
			/* in generale c'è un errore sul thread safe control di questo stream
			 * la procedura ideale sarebbe di implementare uno streamwriter con i lock di sicurezza per i thread
			 * perchè potrebbe andare in AccessSharingViolation mentre 2 thread scrivono sulla console.
			 * */
                                                "Script Version", new Version( 1, 0, 0, 0 ),
                                                "Author name", "Dies Irae",
                                                "Creation Date", new DateTime( 2009, 08, 07 ),
                                                "Author mail-contact", "tocasia@alice.it",
                                                "Author home site", "http://www.midgardshard.it",
                                                "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                "Provided packages", new string[] { "Midgard.Engines.LogConsole" },
                                                "Research tags", new string[] { "Log Console" }
                                                };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( Enabled )
                Core.StartLogging();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
                Commands.RegisterCommands();
        }

        public static readonly string ConsoleLogDirectory = Path.Combine( Server.Core.BaseDirectory, Path.Combine( "Logs", "ConsoleLogs" ) );
        public static readonly ConsoleColor CastOnConsoleColor = ConsoleColor.Green;
    }
}