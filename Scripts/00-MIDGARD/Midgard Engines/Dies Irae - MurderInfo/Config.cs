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
using System.IO;

namespace Midgard.Engines.MurderInfo
{
    public class Config
    {
        public static bool Enabled
        {
            get
            {
                return Packager.Core.Singleton[ typeof( Config ) ].Enabled;
            }
            set
            {
                Packager.Core.Singleton[ typeof( Config ) ].Enabled = value;
            }
        }

        public static bool Debug = true;

        public static object[] Package_Info = {
            "Script Title",             "Murder System",
            "Enabled by Default",       true,
            "Script Version",           new Version(1,0,0,0),
            "Author name",              "Dies Irae", 
            "Creation Date",            new DateTime(2009, 08, 07), 
            "Author mail-contact",      "tocasia@alice.it", 
            "Author home site",         "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights",        "(C) Midgard Shard - Dies Irae",
            "Provided packages",        new string[]{"Midgard.Engines.MurderInfo"},
            //"Required packages",      new string[0],
            //"Conflicts with packages",new string[0],
            "Research tags",            new string[]{"MurderInfo"}
        };

        public static void Package_Initialize()
        {
            if( Enabled )
            {
                if( CheckType == MurderInfoCheckTipe.DailyGlobal )
                {
                    DailyCheckTimer.RegisterCommands();
                    DailyCheckTimer.StartTimer();
                }

                MurderInfoPersistance.EnsureExistence();
            }
        }

        public static readonly bool MurderInfoEnabled = true;

        internal static readonly bool FtpEnabled = true;

        internal static readonly bool RejectedEnabled = false;
        internal static readonly bool OresEnabled = true;
        internal static readonly bool JailEnabled = true;

        internal static readonly string KillsLogPath = Path.Combine( "Logs", "CitizenKills.txt" );
        internal static readonly string PunishmentsLogPath = Path.Combine( "Logs", "Midgard2CitizenPunishments.txt" );

        internal static readonly TimeSpan MurderInfoDecay = TimeSpan.FromHours( 60.0 );
        public static readonly TimeSpan LamerDelay = TimeSpan.FromHours( 3.0 );

        internal static readonly int MineralsToMine = 4000;
        internal static readonly TimeSpan HoursOfJail = TimeSpan.FromHours( 48.0 );

        internal const int InfoesToGetJail = 3;
        internal const int InfoesToGetOres = 5;
        internal const int InfoesToGetRejected = 8;

        internal const int InfoEqForKilledInLamerTime = 1;
        internal const int InfoEqForKilledOutOfLamerTime = 1;

        public static readonly MurderInfoCheckTipe CheckType = MurderInfoCheckTipe.DailyGlobal;
    }
}