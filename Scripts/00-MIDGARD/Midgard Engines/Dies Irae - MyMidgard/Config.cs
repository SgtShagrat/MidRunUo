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

using Core = Midgard.Engines.Packager.Core;

namespace Midgard.Engines.MyMidgard
{
    public class Config
    {
        public const string EntrySep = "\",\"";
        public const string LineEnd = "\"\n";
        public const char LineStart = '\"';
        public static bool Debug = false;

        public static object[] Package_Info =
            {
                "Script Title", "My Midgard Engine",
                "Enabled by Default", true,
                "Script Version", new Version(1, 0, 3, 0),
                "Author name", "Dies Irae & Magius(CHE)",
                "Creation Date", new DateTime(2009, 08, 07),
                "Author mail-contact", "tocasia@alice.it",
                "Author home site", "http://www.midgardshard.it",
                //"Author notes",           null,
                "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                "Provided packages", new string[] {"Midgard.Engines.MyMidgard"},
                //"Required packages",      new string[0],
                //"Conflicts with packages",new string[0],
                "Research tags", new string[] {"MyMidgard"}
            };

        internal static Package Pkg;
        public static int WebCommandsPort
        {
            get
            {
                if( Server.Core.Debug )
                    return 2601;
                return 80; //change weblistener port need adminpriv to run on 80 (Muletto)
            }
        }
        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];

        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                WebCommandSystem.InitSystem();

                Pkg.LogInfoLine( string.Format( "Midgard Web Commands is listening on port {0}.", WebCommandsPort ) );
            }
        }
    }
}