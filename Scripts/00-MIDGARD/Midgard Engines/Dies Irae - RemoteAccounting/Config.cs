/***************************************************************************
 *                               Config.cs
 *                            --------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Packager;

namespace Midgard.Engines.RemoteAccounting
{
    public class Config
    {
        public static object[] Package_Info =
            {
                "Script Title", "Remote Accounting",
                "Enabled by Default", true,
                "Script Version", new Version( 1, 0, 6, 0 ),
                "Author name", "Dies Irae",
                "Creation Date", new DateTime( 2009, 08, 07 ),
                "Author mail-contact", "tocasia@alice.it",
                "Author home site", "http://www.midgardshard.it",
                //"Author notes",           null,
                "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                "Provided packages", new string[] { "Midgard.Engines.RemoteAccounting" },
                //"Required packages",      new string[0],
                //"Conflicts with packages",new string[0],
                "Research tags", new string[] { "Remote Accounting" }
            };

        internal static Package Pkg;
        public static bool Debug = true;

        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];

            if( Enabled )
                PendingAccounts.Load();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
                WebCommands.RegisterCommands();
        }
    }
}