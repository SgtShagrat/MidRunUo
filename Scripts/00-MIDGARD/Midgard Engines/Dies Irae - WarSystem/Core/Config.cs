/***************************************************************************
 *                               Config.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.Packager;

using Server;

namespace Midgard.Engines.WarSystem
{
    public class Config
    {
        public static object[] Package_Info = {
                                                  "Script Title", "Midgard War System",
                                                  "Enabled by Default", false,
                                                  "Script Version", new Version(1, 1, 0, 0),
                                                  "Author name", "Dies Irae",
                                                  "Creation Date", new DateTime(2010, 2, 20),
                                                  "Author mail-contact", "tocasia@alice.it",
                                                  "Author home site", "http://www.midgardshard.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Dies Irae",
                                                  "Provided packages", new string[]
                                                                       {
                                                                           "Midgard.Engines.WarSystem"
                                                                       },
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[]
                                                                   {
                                                                       "War System"
                                                                   }
                                              };

        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Packager.Core.Singleton[ typeof( Config ) ].Enabled; }
            set { Packager.Core.Singleton[ typeof( Config ) ].Enabled = value; }
        }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Config ) ];

            BaseWar.BattlePendingPeriod = DefaultBattlePendingPeriod;
            BaseWar.PreBattlePeriod = DefaultPreBattlePeriod;
            BaseWar.PostBattlePeriod = DefaultPostBattlePeriod;

            BaseWar.RefreshDelay = DefaultRefreshDelay;

            if( Enabled )
            {
                if( SaveEnabled )
                {
                    EventSink.WorldLoad += new WorldLoadEventHandler( Core.Load );
                    EventSink.WorldSave += new WorldSaveEventHandler( Core.Save );
                }

                Core.Instance.StartTimer();
            }
        }

        public static void Package_Initialize()
        {
            if( !Enabled )
                return;

            Commands.RegisterCommands();

            if( LogEnabled )
                Logger.InitLogger();
        }

        public static readonly bool Debug = false;
        public static readonly bool DebugEnabled = true;
        public static readonly bool LogEnabled = true;

        internal static readonly TimeSpan DefaultBattlePendingPeriod = TimeSpan.FromMinutes( 10.0 );
        internal static readonly TimeSpan DefaultPostBattlePeriod = TimeSpan.FromMinutes( 2.0 );
        internal static readonly TimeSpan DefaultPreBattlePeriod = TimeSpan.FromMinutes( 1.0 );
        internal static readonly TimeSpan DefaultRefreshDelay = TimeSpan.FromMinutes( 1.0 );

        #region serialization
        internal static readonly string WarSavePath = Path.Combine( Path.Combine( "Saves", "WarSystem" ), "War.bin" );

        internal static readonly bool SaveEnabled = false;
        #endregion
    }
}