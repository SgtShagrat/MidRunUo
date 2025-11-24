/***************************************************************************
 *                                  GuaranteedGainSystem.cs
 *                            		-----------------------
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
    /// From OSI - http://guide.uo.com/skill_1001.html
    /// </summary>
    public class GuaranteedGainSystem : Timer
    {
        private static int[] m_Terms350 = new int[] { 1, 4, 7, 9, 12, 14, 17, 20, 23, 25, 27, 33, 55, 78, 114, 144, 180, 228, 276, 336, 396, 468, 540, 618 };
        private static int[] m_Terms500 = new int[] { 3, 10, 17, 24, 31, 38, 45, 52, 60, 66, 72, 90, 150, 216, 294, 384, 492, 606, 744, 894, 1056, 1242, 1440, 1662 };
        private static int[] m_Terms700 = new int[] { 5, 18, 30, 44, 57, 72, 84, 96, 108, 120, 138, 162, 264, 390, 540, 708, 900, 1116, 1356, 1620, 1920, 2280, 2580, 3060 };

        private static DateTime m_LastResetTime = DateTime.Now;

        public static TimeSpan ResetTime = TimeSpan.FromHours( 6.0 ); // Time of the day
        public static string SavePath = "Saves/RateInfo";
        public static string SaveFile = "GGS.bin";

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "GuaranteedGainSystemReset", AccessLevel.Administrator, new CommandEventHandler( Reset_OnCommand ) );
        }

        internal static void ConfigureSystem()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( OnLoad );
            EventSink.WorldSave += new WorldSaveEventHandler( OnSave );
        }

        internal static void StartTimer()
        {
            new GuaranteedGainSystem().Start();
        }

        /// <summary>
        /// Check if our mobile is in time to gain our skill
        /// </summary>
        public static bool ForceSkillGain( Mobile from, Skill skill )
        {
            if( from.Player )
            {
                MobileRateInfo mobileInfo = MobileRateInfo.GetMobileInfo( from );
                SkillRateInfo skillInfo = mobileInfo.GetSkillInfo( skill );

                int[] table;

                if( from.Skills.Total <= 350 )
                    table = m_Terms350;
                else if( from.Skills.Total <= 500 )
                    table = m_Terms500;
                else
                    table = m_Terms700;

                int index = skill.BaseFixedPoint / 50;

                if( DateTime.Now - skillInfo.LastGainTime < TimeSpan.FromMinutes( table[ index > 23 ? 23 : index ] ) )
                    return false;

                SkillSystemLog.ForcedSkillGain( from, skill );

                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if our mobile is in time to gain our stat
        /// </summary>
        public static bool ForceStatGain( Mobile from )
        {
            if( from.Player )
            {
                MobileRateInfo mobileInfo = MobileRateInfo.GetMobileInfo( from );

                if( mobileInfo.StatGainsCount < 10 && DateTime.Now - mobileInfo.LastStatGainTime > TimeSpan.FromMinutes( 15 ) )
                {
                    SkillSystemLog.ForcedStatGain( from );

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Register our skill gain
        /// </summary>
        public static void RegisterSkillGain( Mobile from, Skill skill )
        {
            if( from.Player )
            {
                MobileRateInfo mobileInfo = MobileRateInfo.GetMobileInfo( from );
                SkillRateInfo skillInfo = mobileInfo.GetSkillInfo( skill );

                skillInfo.LastGainTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Register our stat gain
        /// </summary>
        public static void RegisterStatGain( Mobile from )
        {
            if( from.Player )
            {
                MobileRateInfo mobileInfo = MobileRateInfo.GetMobileInfo( from );

                mobileInfo.LastStatGainTime = DateTime.Now;
                mobileInfo.StatGainsCount++;
            }
        }

        /// <summary>
        /// Reset all info for this system. If full is true even skill info are reset.
        /// </summary>
        public static void Reset( bool full )
        {
            m_LastResetTime = DateTime.Now;

            foreach( MobileRateInfo mobileInfo in MobileRateInfo.Entries.Values )
            {
                mobileInfo.LastStatGainTime = DateTime.MinValue;
                mobileInfo.StatGainsCount = 0;

                if( full )
                {
                    foreach( SkillRateInfo skillInfo in mobileInfo.SkillRates.Values )
                        skillInfo.LastGainTime = DateTime.MinValue;
                }
            }
        }

        [Usage( "GuaranteedGainSystemReset <full>" )]
        [Description( "Reset all information stored by Guaranteed Gain System system." )]
        private static void Reset_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                bool full = e.GetString( 0 ).Trim().ToLower() == "true";

                Reset( full );

                if( full )
                    e.Mobile.SendMessage( "Guaranteed Gain System has being fully reseted." );
                else
                    e.Mobile.SendMessage( "Guaranteed Gain System stat info has being reseted." );
            }
            else
            {
                e.Mobile.SendMessage( "Usage: GuaranteedGainSystemReset <full>" );
            }
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

        public GuaranteedGainSystem()
            : base( TimeSpan.FromSeconds( 30.0 ), TimeSpan.FromSeconds( 30.0 ) )
        {
            Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            if( !Config.GuaranteedGainSystemEnabled )
                return;

            if( DateTime.Now >= DateTime.Now.Date + ResetTime && DateTime.Now.Date != m_LastResetTime.Date )
            {
                Reset( false );
            }
        }

        private class MobileRateInfo
        {
            #region properties
            public Dictionary<int, SkillRateInfo> SkillRates { get; private set; }
            public DateTime LastStatGainTime { get; set; }
            public int StatGainsCount { get; set; }
            public static Dictionary<Mobile, MobileRateInfo> Entries { get; private set; }
            #endregion

            public MobileRateInfo()
            {
                StatGainsCount = 0;
                LastStatGainTime = DateTime.MinValue;
                SkillRates = new Dictionary<int, SkillRateInfo>();
            }

            static MobileRateInfo()
            {
                Entries = new Dictionary<Mobile, MobileRateInfo>();
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

                writer.Write( LastStatGainTime );
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

                            LastStatGainTime = reader.ReadDateTime();
                            StatGainsCount = reader.ReadInt();

                            break;
                        }
                }
            }
            #endregion
        }

        private class SkillRateInfo
        {
            public DateTime LastGainTime { get; set; }

            public SkillRateInfo()
            {
                LastGainTime = DateTime.MinValue;
            }

            #region serialization
            public void Serialize( GenericWriter writer )
            {
                writer.Write( 0 ); // version

                writer.Write( LastGainTime );
            }

            public void Deserialize( GenericReader reader )
            {
                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            LastGainTime = reader.ReadDateTime();

                            break;
                        }
                }
            }
            #endregion
        }
    }
}