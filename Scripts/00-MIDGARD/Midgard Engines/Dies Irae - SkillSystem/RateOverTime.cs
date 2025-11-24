/***************************************************************************
 *                                  RateOverTime.cs
 *                            		---------------
 *  begin                	: Settembre, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Commands;

namespace Midgard.Engines.SkillSystem
{
    /// <summary>
    /// From OSI - http://uo.stratics.com/content/basics/siege.shtml
    /// </summary>
    public class RateOverTime : Timer
    {
        private static DateTime m_LastResetTime = DateTime.Now;

        public static TimeSpan ResetTime = TimeSpan.FromHours( 6.0 ); // Time of the day
        public static string SavePath = "Saves/RateInfo";
        public static string SaveFile = "RoT.bin";

        public static int MaxStatGainAllowed = 8;

        public static void RegisterCommands()
        {
            CommandSystem.Register( "RateOverTimeReset", AccessLevel.Administrator, new CommandEventHandler( Reset_OnCommand ) );
        }

        public static void ConfigSystem()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( OnLoad );
            EventSink.WorldSave += new WorldSaveEventHandler( OnSave );
        }

        internal static void StartTimer()
        {
            new RateOverTime().Start();
        }

        /// <summary>
        /// Check if a stat gain is possible for mobile from
        /// </summary>
        public static bool StatGainAllowed( Mobile from )
        {
            if( from.Player )
            {
                MobileRateInfo info = MobileRateInfo.GetMobileInfo( from );

                if( info.StatGainsCount < MaxStatGainAllowed )
                {
                    SkillSystemLog.StatGainAllowed( from, info.StatGainsCount + 1 );

                    info.StatGainsCount++;
                }
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if skillgain is possible for mobile from
        /// </summary>
        public static bool SkillGainAllowed( Mobile from, Skill skill )
        {
            if( from.Player )
            {
                MobileRateInfo mobileInfo = MobileRateInfo.GetMobileInfo( from );
                SkillRateInfo skillInfo = mobileInfo.GetSkillInfo( skill );

                if( skill.Base >= 100.0 && ( mobileInfo.SkillGainsCount >= 72 || DateTime.Now - skillInfo.LastGainTime < TimeSpan.FromMinutes( 5.0 ) ) )
                    return false;
                if( skill.Base >= 90.0 && ( mobileInfo.SkillGainsCount >= 30 || DateTime.Now - skillInfo.LastGainTime < TimeSpan.FromMinutes( 10.0 ) ) )
                    return false;
                if( skill.Base >= 80.0 && ( mobileInfo.SkillGainsCount >= 60 || DateTime.Now - skillInfo.LastGainTime < TimeSpan.FromMinutes( 5.0 ) ) )
                    return false;
                if( skill.Base >= 70.0 && ( mobileInfo.SkillGainsCount >= 100 || DateTime.Now - skillInfo.LastGainTime < TimeSpan.FromMinutes( 3.0 ) ) )
                    return false;
                if( skill.Base < 70.0 )
                    return true;

                SkillSystemLog.SkillGainAllowed( from, skill, mobileInfo.SkillGainsCount + 1 );

                mobileInfo.SkillGainsCount++;

                skillInfo.LastGainTime = DateTime.Now;
                skillInfo.GainsCount++;
            }

            return true;
        }

        public static void Reset()
        {
            m_LastResetTime = DateTime.Now;

            MobileRateInfo.Entries.Clear();
        }

        [Usage( "RateOverTime" )]
        [Description( "Reset all information stored by Rate over Time system." )]
        private static void Reset_OnCommand( CommandEventArgs e )
        {
            Reset();

            e.Mobile.SendMessage( "Rate over Time system has being reseted." );
        }

        private static void OnSave( WorldSaveEventArgs args )
        {
            WorldSaveProfiler.Instance.StartHandlerProfile( OnSave );

            if( !Directory.Exists( SavePath ) )
                Directory.CreateDirectory( SavePath );

            GenericWriter writer = new BinaryFileWriter( Path.Combine( SavePath, SaveFile ), true );

            writer.Write( m_LastResetTime );

            writer.Write( MobileRateInfo.Entries.Count );

            foreach( KeyValuePair<Mobile, MobileRateInfo> kvp in MobileRateInfo.Entries )
            {
                writer.Write( kvp.Key );

                MobileRateInfo info = kvp.Value;

                info.Serialize( writer );
            }

            writer.Close();

            WorldSaveProfiler.Instance.EndHandlerProfile();
        }

        private static void OnLoad()
        {
            if( !File.Exists( Path.Combine( SavePath, SaveFile ) ) )
                return;

            using( FileStream bin = new FileStream( Path.Combine( SavePath, SaveFile ), FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                GenericReader reader = new BinaryFileReader( new BinaryReader( bin ) );

                m_LastResetTime = reader.ReadDateTime();

                int count = reader.ReadInt();

                for( int i = 0; i < count; ++i )
                {
                    Mobile mobile = reader.ReadMobile();

                    MobileRateInfo info = new MobileRateInfo();

                    info.Deserialize( reader );

                    if( mobile != null )
                        MobileRateInfo.Entries.Add( mobile, info );
                }
            }
        }

        public RateOverTime()
            : base( TimeSpan.FromSeconds( 30.0 ), TimeSpan.FromSeconds( 30.0 ) )
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if( !Config.RateOverTimeEnabled )
                return;

            if( DateTime.Now >= DateTime.Now.Date + ResetTime && DateTime.Now.Date != m_LastResetTime.Date )
            {
                Reset();
            }
        }

        private class MobileRateInfo
        {
            #region fields
            private static Dictionary<Mobile, MobileRateInfo> m_Entries = new Dictionary<Mobile, MobileRateInfo>();

            public static Dictionary<Mobile, MobileRateInfo> Entries
            {
                get { return m_Entries; }
            }

            public static MobileRateInfo GetMobileInfo( Mobile from )
            {
                MobileRateInfo info;

                if( !Entries.TryGetValue( from, out info ) )
                {
                    info = new MobileRateInfo();

                    Entries.Add( from, info );
                }

                return info;
            }
            #endregion

            #region properties
            public Dictionary<int, SkillRateInfo> SkillRates { get; private set; }

            public int SkillGainsCount { get; set; }

            public int StatGainsCount { get; set; }
            #endregion

            public MobileRateInfo()
            {
                StatGainsCount = 0;
                SkillGainsCount = 0;
                SkillRates = new Dictionary<int, SkillRateInfo>();
            }

            public SkillRateInfo GetSkillInfo( Skill skill )
            {
                SkillRateInfo info;

                if( !SkillRates.TryGetValue( skill.SkillID, out info ) )
                {
                    info = new SkillRateInfo();

                    SkillRates.Add( skill.SkillID, info );
                }

                return info;
            }

            #region serialization
            public void Serialize( GenericWriter writer )
            {
                writer.Write( 0 ); // version

                writer.Write( SkillRates.Count );

                foreach( KeyValuePair<int, SkillRateInfo> kvp in SkillRates )
                {
                    writer.Write( kvp.Key );

                    SkillRateInfo info = kvp.Value;

                    info.Serialize( writer );
                }

                writer.Write( SkillGainsCount );
                writer.Write( StatGainsCount );
            }

            public void Deserialize( GenericReader reader )
            {
                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            int count = reader.ReadInt();

                            for( int i = 0; i < count; i++ )
                            {
                                int id = reader.ReadInt();

                                SkillRateInfo info = new SkillRateInfo();

                                info.Deserialize( reader );

                                SkillRates.Add( id, info );
                            }

                            SkillGainsCount = reader.ReadInt();
                            StatGainsCount = reader.ReadInt();

                            break;
                        }
                }
            }
            #endregion
        }

        private class SkillRateInfo
        {
            private DateTime m_LastGainTime;
            private int m_GainsCount;

            public DateTime LastGainTime
            {
                get { return m_LastGainTime; }
                set { m_LastGainTime = value; }
            }

            public int GainsCount
            {
                get { return m_GainsCount; }
                set { m_GainsCount = value; }
            }

            public SkillRateInfo()
            {
                m_LastGainTime = DateTime.MinValue;
                m_GainsCount = 0;
            }

            #region serialization
            public void Serialize( GenericWriter writer )
            {
                writer.Write( 0 ); // version

                writer.Write( m_LastGainTime );
                writer.Write( m_GainsCount );
            }

            public void Deserialize( GenericReader reader )
            {
                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            m_LastGainTime = reader.ReadDateTime();
                            m_GainsCount = reader.ReadInt();

                            break;
                        }
                }
            }
            #endregion
        }
    }
}