/***************************************************************************
 *                               BountySystemLog.cs
 *
 *   begin                : 03 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;

using Server;

namespace Midgard.Engines.BountySystem
{
    public class BountySystemLog
    {
        private static StreamWriter m_Writer;
        private static bool m_Enabled;

        private static bool EnableLogging = true;

        public enum LogType
        {
            Added,
            Removed,
            Requested,
            HeadGiven
        }

        public static void Initialize()
        {
            if( EnableLogging )
            {
                // Create the log writer
                try
                {
                    string folder = Path.Combine( Server.Core.BaseDirectory, Path.Combine( "Logs", "BountySystem" ) );

                    if( !Directory.Exists( folder ) )
                        Directory.CreateDirectory( folder );

                    string name = string.Format( "{0}.txt", DateTime.Now.ToLongDateString() );
                    string file = Path.Combine( folder, name );

                    m_Writer = new StreamWriter( file, true );
                    m_Writer.AutoFlush = true;

                    m_Enabled = true;
                }
                catch( Exception ex )
                {
                    Config.Pkg.LogInfoLine( "Couldn't initialize bounty system log. Reason:" );
                    Config.Pkg.LogInfoLine( ex.ToString() );
                    m_Enabled = false;
                }
            }
        }

        public static void WriteInfo( BountyBoardEntry be, LogType type )
        {
            WriteInfo( be, type, null );
        }

        public static void WriteInfo( BountyBoardEntry be, LogType type, Mobile giver )
        {
            if( !m_Enabled || m_Writer == null || be == null )
                return;

            try
            {
                m_Writer.WriteLine( "Entry {0} ( {1} )", type, DateTime.Now );
                m_Writer.WriteLine( string.Format( "Onwer: {0} (serial {1} - account {2}) - Wanted {3} (serial {4} - account {5}) - Price {6} - Expire {7}.",
                be.Owner.Name, be.Owner.Serial, be.Owner.Account.Username,
                be.Wanted.Name, be.Wanted.Serial, be.Wanted.Account.Username,
                be.Price, be.ExpireTime ) );

                if( type == LogType.HeadGiven && giver != null )
                    m_Writer.WriteLine( string.Format( "Head given by Player: {0} (serial {1} - account {2})", giver.Name, giver.Serial, giver.Account.Username ) );

                m_Writer.WriteLine();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( ex.ToString() );
            }
        }

        public static void WriteInfo( string toLog )
        {
            if( !m_Enabled || m_Writer == null )
                return;

            try
            {
                m_Writer.WriteLine( "Generic log ( {0} )", DateTime.Now );
                m_Writer.WriteLine( toLog );
                m_Writer.WriteLine();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( ex.ToString() );
            }
        }
    }
}
