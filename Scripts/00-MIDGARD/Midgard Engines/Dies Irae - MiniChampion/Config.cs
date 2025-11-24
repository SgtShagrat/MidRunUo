/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 15 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Packager;

namespace Midgard.Engines.MiniChampionSystem
{
    public class Config
    {
        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Mini Champion System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version(1, 1, 0, 0),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime(2011, 5, 15),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[]
                                                                           {
                                                                               "Midgard.Engines.MiniChampionSystem"
                                                                           },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[]
                                                                       {
                                                                           "Mini Champion System"
                                                                       }
                                              };



        internal static Package Pkg;

        public static double ExpireDelayInMinutes = 10.0;
        public static double RestartDelayInMinutes = 60.0;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];

            if( !Enabled )
                return;
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;
        }
    }
}