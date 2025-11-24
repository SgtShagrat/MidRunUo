/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

namespace Midgard.Engines.WelcomeSystem
{
    public class Config
    {
        public static bool Enabled
        {
            get
            {
                return Packager.Core.Singleton[typeof(Config)].Enabled;
            }
            set
            {
                Packager.Core.Singleton[typeof(Config)].Enabled = value;
            }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Midgard Welcome System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2007, 1, 1), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.WelcomeSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Welcome System"}
        };

        internal const string NewsFilePath = "Data/Notizie.xml";
        internal const string MotdFilePath = "Data/Motd.xml";
        internal const string TipsFilePath = "Data/Tips.xml";

        /*internal static readonly string AddressFTP = FtpService.addressRootFTP.AbsoluteUri;// "ftp://ftp.midgardshard.it/midgardshard.it/"; // path al sito ftp
        internal static readonly string UserFTP = FtpService.UserFTP;
        internal static readonly string PasswordFTP = FtpService.PasswordFTP;*/

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Core.RegisterCommands();
                Core.InitSystem();
            }
        }
    }
}