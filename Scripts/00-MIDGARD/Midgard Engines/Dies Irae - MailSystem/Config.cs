/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.MailSystem
{
    public class Config
    {
        public static bool Debug = false;

        public static object[] Package_Info =
        {
            "Script Title", "Mail System",
            "Enabled by Default", true,
            "Script Version", new Version(1, 0, 0, 0),
            "Author name", "Dies Irae",
            "Creation Date", new DateTime(2009, 12, 31),
            "Author mail-contact", "tocasia@alice.it",
            "Author home site", "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Dies Irae",
            "Provided packages", new string[] {"Midgard.Engines.MailSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] {"Mail System"}
        };

        internal static readonly string MailSavePath = Path.Combine( Path.Combine( "Saves", "MailSystem" ), "Mails.bin" );

        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( Enabled )
                Core.ConfigSystem();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Core.RegisterCommands();
                Core.BuildRecipientsList();
            }
        }
    }
}