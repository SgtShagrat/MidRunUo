/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 23 aprile 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.FeedStockRecoverySystem
{
    public class Config
    {
        public static bool Debug;

        public static object[] Package_Info = {
                                                  "Script Title", "FeedStock Recovery System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2011, 4, 23 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.FeedStockRecoverySystem" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "FeedStock Recovery" }
                                              };

        public static bool Enabled { get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; } set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;
        }

        internal static Package Pkg;

        public static string LogFileName = Path.Combine( "Logs", "recycles.log" );

        public static bool FullChanceToRestock = true;

        public static bool ResmeltWithoutConfirm = true;

        public const string RecycleString = "Will you recycle this?";
    }
}