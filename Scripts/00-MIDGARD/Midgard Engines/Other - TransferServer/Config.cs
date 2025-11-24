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

using Server.Commands;

using Packager = Midgard.Engines.Packager;

namespace Server.Engines.XmlSpawner2.Xts
{
    public class Config
    {
        internal static Packager.Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static object[] Package_Info = {
                                                "Script Title", "Xml Transfer Server",
                                                "Enabled by Default", true,
                                                "Script Version", new Version( 1, 0, 5, 0 ),
                                                "Author name", "Artegordon Rev. by Dies Irae",
                                                "Creation Date", new DateTime( 2009, 08, 07 ),
                                                "Author mail-contact", "tocasia@alice.it",
                                                "Author home site", "",
                                                "Script Copyrights", "(C) Midgard Shard - Dies Irae - Artegordon",
                                                "Provided packages", new string[] { "Midgard.Engines.XmlSpawner2.Xts" },
                                                "Research tags", new string[] { "Transfer Server" }
                                                };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            CommandSystem.Register( "XTS", ReqAccessLevel, new CommandEventHandler( TransferServer.XTS_OnCommand ) );
            RemoteMessaging.OnReceiveMessage += new RemoteMessaging.Message( TransferServer.RemoteMessagingReceiveMessage );
        }

        internal static readonly int Port = 8030;
        internal static bool ShouldServerRun = false;
        internal static readonly AccessLevel ReqAccessLevel = AccessLevel.Administrator;
    }
}