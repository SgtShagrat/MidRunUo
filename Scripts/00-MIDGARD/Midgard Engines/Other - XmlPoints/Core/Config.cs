using System;

namespace Server.Engines.XmlPoints
{
    public class Config
    {
        public static bool Debug;

        public static object[] Package_Info = {
                                                  "Script Title", "XmlPoints System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version(1, 0, 0, 0),
                                                  "Author name", "Artegordon (rev Dies Irae)",
                                                  "Revision Date", new DateTime(2010, 02, 15),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] {"Server.Engines.XmlPoints"},
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] {"Order Chaos Wars"}
                                              };

        public static bool Enabled
        {
            get { return Midgard.Engines.Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Midgard.Engines.Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static void Package_Configure()
        {
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                if( Enabled )
                    XmlPointsAttach.Initialize();
            }
        }
    }
}