using System;

using Midgard.Engines.Packager;

using Server;

using Core=Midgard.Engines.Packager.Core;

namespace Midgard.Engines.MidgardTownSystem
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Town System",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version( 1, 0, 0, 0 ),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime( 2009, 08, 07 ),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[] { "Midgard.Engines.MidgardTownSystem" },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] { "Town System" }
                                              };

        internal static readonly bool TownRankStatusPageEnabled = false;
        internal static readonly bool TownRanFtpEnabled = false;

        public static void Package_Configure()
        {
            Pkg = Core.Singleton[ typeof( Config ) ];

            if( Enabled )
            {
                TownSystem.ConfigSystem();

                TownItemPriceDefinition.Load();

                TownItemPriceDefinition.RegisterCommands();
                TownSystemCommands.RegisterCommands();
                TownJailSystem.ConfigSystem();
            }
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                TownItemPriceDefinition.Populate();
                TownItemPriceDefinition.SaveXml();
                TownItemPriceDefinition.Load();

                TownSystemPersistance.EnsureExistence();

                if( TownRankStatusPageEnabled )
                    TownRankReport.StartTimer();

                WebCommands.RegisterCommands();

                TownJailSystem.InitSystem();
            }
        }

        public static readonly string SiteGuide = "http://www.midgardshard.it/";
        public static readonly Map[] TownWarsMaps = new Map[] { Map.Felucca, Map.Malas };
        public static readonly TimeSpan YoungHours = TimeSpan.FromHours( 100.0 );
    }
}