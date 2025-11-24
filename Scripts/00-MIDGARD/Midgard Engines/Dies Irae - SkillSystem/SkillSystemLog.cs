/***************************************************************************
 *                                  SkillSystemLog.cs
 *                            		-----------------
 *  begin                	: Settembre, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.IO;
using Server.Commands;
using Server.Misc;
using Server;

namespace Midgard.Engines.SkillSystem
{
    public class SkillSystemLog
    {
        private static StreamWriter m_Writer;
        public static bool Enabled = Config.SkillLogEnabled;

        private static readonly string SkillSystemSavePath = Path.Combine( Path.Combine( "Saves", "SkillSystem" ), "SkillSave.bin" );

        public static void Configure()
        {
            if( Enabled )
            {
                if( m_UseCounterList == null )
                    m_UseCounterList = new int[ SkillInfo.Table.Length ];

                EventSink.WorldLoad += new WorldLoadEventHandler( Load );
                EventSink.WorldSave += new WorldSaveEventHandler( Save );
            }
        }

        private static int[] m_UseCounterList;

        public static void Load()
        {
            if( Config.Debug )
                Console.Write( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( SkillSystemSavePath ) )
            {
                Console.WriteLine( "Warning: {0} not found.", SkillSystemSavePath );
                Console.WriteLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                if( m_UseCounterList == null )
                    m_UseCounterList = new int[ SkillInfo.Table.Length ];

                if( !File.Exists( SkillSystemSavePath ) )
                {
                    Console.WriteLine( "Warning: {0} not found.", SkillSystemSavePath );
                    Console.WriteLine( "All counters are set to 0." );
                    return;
                }

                BinaryReader bReader = new BinaryReader( File.OpenRead( SkillSystemSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                int version = reader.ReadInt();

                int skillsLength = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            for( int i = 0; i < skillsLength; ++i )
                                m_UseCounterList[ i ] = reader.ReadInt();

                            break;
                        }
                }

                bReader.Close();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error de-serializing {0}.", Config.Pkg.Title );
                Console.WriteLine( ex.ToString() );
                Console.ReadKey();
            }
        }

        public static void Save( WorldSaveEventArgs e )
        {
            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( SkillSystemSavePath ), Path.GetDirectoryName( SkillSystemSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( SkillSystemSavePath, true );

                writer.Write( 0 ); // version

                writer.Write( m_UseCounterList.Length );

                foreach( int t in m_UseCounterList )
                    writer.Write( t );

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error serializing {0}.", Config.Pkg.Title );
                Console.WriteLine( ex.ToString() );
            }
        }

        [Usage( "DumpSkillUses" )]
        [Description( "Trace the actual skill use on this shard" )]
        public static void DumpUses_OnCommand( CommandEventArgs e )
        {
            int sum = 0;
            foreach( int i in m_UseCounterList )
                sum += i;

            using( StreamWriter op = new StreamWriter( "Logs/SkillUses.log" ) )
            {
                op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(),
                              DateTime.Now.ToShortTimeString() );

                op.WriteLine( "Total skill uses {0}", sum );
                op.WriteLine( "" );

                for( int index = 0; index < m_UseCounterList.Length; index++ )
                {
                    op.WriteLine( "{0} - {1} - {2}%", (SkillName)index, m_UseCounterList[ index ],
                                  ( m_UseCounterList[ index ] / (double)sum ) * 100 );
                }
            }
        }

        public static void RegisterUse( int skillID )
        {
            if( skillID < 0 || skillID > SkillInfo.Table.Length )
                return;

            if( m_UseCounterList == null )
                m_UseCounterList = new int[ SkillInfo.Table.Length ];

            m_UseCounterList[ skillID ]++;

            // Console.WriteLine( "Skilluse logged. Skill: {0} - Total uses {1}", (SkillName)skillID, m_UseCounterList[ skillID ] );
        }

        internal static void InitLogger()
        {
            try
            {
                string folder = Path.Combine( Server.Core.BaseDirectory, Path.Combine( "Logs", "Skills" ) );

                if( !Directory.Exists( folder ) )
                    Directory.CreateDirectory( folder );

                string name = string.Format( "{0}.txt", DateTime.Now.ToLongDateString() );
                string file = Path.Combine( folder, name );

                m_Writer = new StreamWriter( file, true );
                m_Writer.AutoFlush = true;

                m_Writer.WriteLine( "###############################" );
                m_Writer.WriteLine( "# {0} - {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                m_Writer.WriteLine();
            }
            catch( Exception err )
            {
                Console.WriteLine( "Couldn't initialize skill system log. Reason:" );
                Console.WriteLine( err.ToString() );
                Enabled = false;
            }
        }

        public static void RegisterCommands()
        {
            CommandSystem.Register( "DumpSkillUses", AccessLevel.Developer, new CommandEventHandler( DumpUses_OnCommand ) );
        }

        /// <summary>
        /// Log a skill change for our mobile
        /// </summary>
        public static void SkillChanged( Mobile m, Skill skill, double oldValue, double newValue )
        {
            if( m_Writer != null )
            {
                try
                {
                    m_Writer.WriteLine( "Skill \'{0}\' changed for mobile {1} (account \'{2}\' - serial {3}) in date and time {4}, {5} from value {6} to {7}.",
                        skill.Name, m.Name, m.Account.Username, m.Serial,
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
                        oldValue, newValue );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        /// <summary>
        /// Log a stat change for our mobile
        /// </summary>
        public static void StatChanged( Mobile m, SkillCheck.Stat stat, int oldValue, int newValue )
        {
            if( m_Writer != null )
            {
                try
                {
                    m_Writer.WriteLine( "Stat \'{0}\' changed for mobile {1} (account \'{2}\' - serial {3}) in date and time {4}, {5} from value {6} to {7}.",
                        stat, m.Name, m.Account.Username, m.Serial,
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
                        oldValue, newValue );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );

                }
            }
        }

        /// <summary>
        /// Log if skill gain should be forced due to GGS system
        /// </summary>
        public static void ForcedSkillGain( Mobile m, Skill skill )
        {
            if( m_Writer != null )
            {
                try
                {
                    m_Writer.WriteLine( "Forced SkillGain for skill \'{0}\' for mobile {1} (account \'{2}\' - serial {3}) in date and time {4}, {5}",
                        skill.Name, m.Name, m.Account.Username, m.Serial,
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        /// <summary>
        /// Log if stat gain should be forced due to GGS system
        /// </summary>
        public static void ForcedStatGain( Mobile m )
        {
            if( m_Writer != null )
            {
                try
                {
                    m_Writer.WriteLine( "Forced StatGain for mobile {0} (account \'{1}\' - serial {2}) in date and time {3}, {4}",
                        m.Name, m.Account.Username, m.Serial,
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        /// <summary>
        /// Log if skill gain should be allowed due to ROT system
        /// </summary>
        public static void SkillGainAllowed( Mobile m, Skill skill, int gains )
        {
            if( m_Writer != null )
            {
                try
                {
                    m_Writer.WriteLine( "Skill gain allowed for mobile {0} (account \'{1}\' - serial {2}) in date and time {3}, {4}. Skill {5}, Gains {6}.",
                        m.Name, m.Account.Username, m.Serial,
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
                        skill.Name, gains );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        /// <summary>
        /// Log if stat gain should be allowed due to ROT system
        /// </summary>
        public static void StatGainAllowed( Mobile m, int gains )
        {
            if( m_Writer != null )
            {
                try
                {
                    m_Writer.WriteLine( "Stat gain allowed for mobile {0} (account \'{1}\' - serial {2}) in date and time {3}, {4}. Gains {5}.",
                        m.Name, m.Account.Username, m.Serial,
                        DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), gains );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }
    }
}
