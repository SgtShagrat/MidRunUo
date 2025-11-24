using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Server;
using Server.Commands;

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = false; // is the script enabled?

		private static TimeSpan RestartTime = TimeSpan.FromHours( 4.0 ); // time of day at which to restart
		private static TimeSpan RestartDelay = TimeSpan.Zero; // how long the server should remain active before restart (period of 'server wars')

		private static TimeSpan WarningDelay = TimeSpan.FromMinutes( 3.0 ); // at what interval should the shutdown message be displayed?

		private static bool m_Restarting;
		private static DateTime m_RestartTime;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static void Initialize()
		{
			CommandSystem.Register( "Restart", AccessLevel.Administrator, new CommandEventHandler( Restart_OnCommand ) );
			new AutoRestart().Start();
		}

        [Usage( "Restart" )]
        [Description( "Force a server restart" )]
		public static void Restart_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting )
			{
				e.Mobile.SendMessage( "The server is already restarting." );
			}
			else
			{
				e.Mobile.SendMessage( "You have initiated server shutdown." );
				Enabled = true;
                // m_RestartTime = DateTime.Now;
                new BroadcastRestartTimer().Start();
			}
		}

        public class BroadcastRestartTimer : Timer
        {
            private int m_Counts = 180;

            public BroadcastRestartTimer()
                : base( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ) )
            {
            }

            protected override void OnTick()
            {
                m_Counts -= Interval.Seconds;

                switch( m_Counts )
                {
                    case 150: Broadcast( m_Counts ); break;
                    case 120: Broadcast( m_Counts ); break;
                    case 90: Broadcast( m_Counts ); break;
                    case 80:
                    case 70:
                    case 60: Broadcast( m_Counts ); break;
                    case 50:
                    case 40:
                    case 30: Broadcast( m_Counts ); break;
                    case 20:
                        Interval = TimeSpan.FromSeconds( 1 );
                        goto case 10;
                    case 10: Broadcast( m_Counts ); break;
                    case 9:
                    case 8:
                    case 7:
                    case 6:
                    case 5:
                    case 4:
                    case 3:
                    case 2:
                    case 1:
                    case 0:
                        m_RestartTime = DateTime.Now;
                        Stop();
                        break;
                    default:
                        break;
                }
            }

		    private static void Broadcast( int seconds )
		    {
			    World.Broadcast( 0x22, true, "The server is restarting in {0} seconds.", seconds );
		    }
        }

		public AutoRestart() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;

			m_RestartTime = DateTime.Now.Date + RestartTime;

			if ( m_RestartTime < DateTime.Now )
				m_RestartTime += TimeSpan.FromDays( 1.0 );
		}

		private void Warning_Callback()
		{
			World.Broadcast( 0x22, true, "The server is going down shortly." );
		}

		private void Restart_Callback()
		{
			Core.Kill( true );
		}

		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

			if ( DateTime.Now < m_RestartTime )
				return;

			if ( WarningDelay > TimeSpan.Zero )
			{
				Warning_Callback();
				Timer.DelayCall( WarningDelay, WarningDelay, new TimerCallback( Warning_Callback ) );
			}

			AutoSave.Save();

			m_Restarting = true;

			Timer.DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );
		}
	}
}