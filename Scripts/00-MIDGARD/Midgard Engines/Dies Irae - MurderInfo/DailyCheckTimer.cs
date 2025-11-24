/***************************************************************************
 *                                  DailyCheckTimer.cs
 *                            		------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;

namespace Midgard.Engines.MurderInfo
{
    public class DailyCheckTimer : Timer
    {
        private static TimeSpan m_CheckInterval = TimeSpan.FromHours( 24.0 );
        private static DateTime m_CheckTime;

        public static void RegisterCommands()
        {
            CommandSystem.Register( "GlobalCheckMurderInfoes", AccessLevel.Developer, new CommandEventHandler( GlobalCheckMurderInfoes_OnCommand ) );
            CommandSystem.Register( "GlobalClearMurderInfoes", AccessLevel.Developer, new CommandEventHandler( GlobalClearMurderInfoes_OnCommand ) );
            CommandSystem.Register( "GenerateMurderInfoReport", AccessLevel.Developer, new CommandEventHandler( GenerateMurderInfoReport_OnCommand ) );
        }

        public static void StartTimer()
        {
            new DailyCheckTimer().Start();
        }

        [Usage( "GlobalCheckMurderInfoes" )]
        [Description( "Unregisters all murder infos" )]
        public static void GlobalClearMurderInfoes_OnCommand( CommandEventArgs e )
        {
            MurderInfoPersistance.UnRegisterAll();
        }

        [Usage( "GlobalClearMurderInfoes" )]
        [Description( "Force a global murder info check" )]
        public static void GlobalCheckMurderInfoes_OnCommand( CommandEventArgs e )
        {
            m_CheckTime = DateTime.Now;
        }

        [Usage( "GenerateMurderInfoReport" )]
        [Description( "Generates murder info system report" )]
        public static void GenerateMurderInfoReport_OnCommand( CommandEventArgs e )
        {
            try
            {
                MurderInfoReport.WriteReport( "web/MurderReport.xml" );
            }
            catch
            {
            }

            if( Config.MurderInfoEnabled )
                FtpService.UploadFile( "web/MurderReport.xml", "MurderReport.xml" );
        }

        public DailyCheckTimer()
            : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 5.0 ) )
        {
            Priority = TimerPriority.FiveSeconds;

            m_CheckTime = DateTime.Now.Date + m_CheckInterval;
        }

        protected override void OnTick()
        {
            if( World.Saving || Server.Misc.AutoRestart.Restarting )
                return;

            if( DateTime.Now < m_CheckTime )
                return;
            else
            {
                Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckMurdersInfo_Callback ) );
                m_CheckTime = DateTime.Now.Date + m_CheckInterval;
                Console.WriteLine( "Next Check Murders Info process will be at {0}. ", m_CheckTime.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" ) );
            }
        }

        private static void CheckMurdersInfo_Callback()
        {
            MurderInfoHelper.HandleGlobalMurderInfoes( true );
        }
    }
}