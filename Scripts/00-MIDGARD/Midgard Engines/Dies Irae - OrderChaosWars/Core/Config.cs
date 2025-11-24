using System;
using System.IO;

using Midgard.Engines.Packager;

namespace Midgard.Engines.OrderChaosWars
{
    public class Config
    {
        internal static Package Pkg;

        public static bool Debug = false;

        public static object[] Package_Info = {
                                                  "Script Title", "Midgard Order vs Chaos Wars Engine",
                                                  "Enabled by Default", true,
                                                  "Script Version", new Version(1, 0, 0, 0),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime(2009, 08, 07),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[]
                                                                       {
                                                                           "Midgard.Engines.OrderChaosWars"
                                                                       },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[]
                                                                   {
                                                                       "Order Chaos Wars"
                                                                   }
                                              };

        internal static bool LogEnabled = true;

        internal static readonly TimeSpan BattlePendingPeriod = TimeSpan.FromMinutes( 10.0 ); // ( 120.0 );
        internal static readonly TimeSpan PostBattlePeriod = TimeSpan.FromMinutes( 2.0 ); // ( 15.0 );
        internal static readonly TimeSpan PreBattlePeriod = TimeSpan.FromMinutes( 1.0 ); // ( 15.0 );
        internal static readonly TimeSpan RefreshDelay = TimeSpan.FromMinutes( 1.0 );
        internal static readonly bool DebugEnabled = true;
        internal static readonly string WarSavePath = Path.Combine( Path.Combine( "Saves", "OrderChaosWars" ), "Wars.bin" );
        
        internal static readonly bool SaveEnabled = false;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            if( Enabled )
                Core.ConfigSystem();
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            Commands.RegisterCommands();

            if( LogEnabled )
                Logger.InitLogger();
        }
    }
}