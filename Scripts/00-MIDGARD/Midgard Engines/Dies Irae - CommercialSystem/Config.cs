/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 27 aprile 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.CommercialSystem
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        internal static bool Debug = false;

        public static object[] Package_Info = {
            "Script Title",             "Commercial System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.CommercialSystem"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"Commercial System"}
        };

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            Core.ConfigSystem();
        }

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                Commands.RegisterCommands();
                WebCommands.RegisterCommands();
                Core.Instance.StartTimer();
                CommercialLog.InitLog();
            }
        }

        /// <summary>
        /// The delay at which commercial core checks for expired modifications
        /// </summary>
        internal static TimeSpan CommercialRefreshDelay = TimeSpan.FromMinutes( 5.0 );

        /// <summary>
        /// The default minimum scalar applyed to items bought from vendors
        /// </summary>
        internal static double DefaultMinBuyScalar = 0.95;

        /// <summary>
        /// The default maximum scalar applyed to items bought from vendors
        /// </summary>
        internal static double DefaultMaxBuyScalar = 10.0;

        /// <summary>
        /// The default minimum scalar applyed to items sold to vendors
        /// </summary>
        internal static double DefaultMinSellScalar = 0.001;

        /// <summary>
        /// The default maximum scalar applyed to items sold to vendors
        /// </summary>
        internal static double DefaultMaxSellScalar = 1.05;

        /// <summary>
        /// The percentual increase for each commercial buy/sell update
        /// </summary>
        internal static double CommercialDelta = 0.001;

        /// <summary>
        /// Time at which a given commercial info should decay to default value (1.0)
        /// </summary>
        internal static TimeSpan CommercialInfoDecay = TimeSpan.FromMinutes( 60.0 );

        public static bool BuyInfoScalarnabled = true;

        public static bool SellInfoScalarnabled = true;

        internal static readonly string CommercialSavePath = Path.Combine( Path.Combine( "Saves", "CommercialSystem" ), "CommercialStatus.bin" );

        internal const string CommercialLogPath = "Logs/CommercialSystem";
    }
}