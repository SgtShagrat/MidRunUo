using System;

using Midgard.Engines.Packager;

using Server;
using Server.Commands;

namespace Midgard.Engines.NameManager
{
    public class Config
    {
        public static bool Debug = false;

        public static object[] Package_Info =
            {
                "Script Title", "Name Manager Engine",
                "Enabled by Default", true,
                "Script Version", new Version(1, 0, 0, 0),
                "Author name", "Dies Irae",
                "Creation Date", new DateTime(2009, 12, 28),
                "Author mail-contact", "tocasia@alice.it",
                "Author home site", "http://www.midgardshard.it",
                //"Author notes",           null,
                "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                "Provided packages", new string[] {"Midgard.Engines.NameManager"},
                //"Required packages",      new string[0],
                //"Conflicts with packages",new string[0],
                "Research tags", new string[] {"Name Manager"}
            };

        internal static Package Pkg;

        public static bool Enabled = true;

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Core.VerifyNames( true );

                CommandSystem.Register( "ListPlayerNames", AccessLevel.Developer, new CommandEventHandler( Core.ListPlayerNames_OnCommand ) );
                CommandSystem.Register( "VerifyPlayerNames", AccessLevel.Developer, new CommandEventHandler( Core.VerifyPlayerNames_OnCommand ) );

                EventSink.Login += new LoginEventHandler( Core.OnLogin );
            }
        }
    }
}