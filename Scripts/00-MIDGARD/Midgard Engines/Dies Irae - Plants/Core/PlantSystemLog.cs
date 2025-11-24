/***************************************************************************
 *                                     PlantSystemLog.cs
 *                            		-----------------------
 *  begin                	: Settembre, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Log system.
 * 
 ***************************************************************************/

using System;
using System.IO;

using Server;

namespace Midgard.Engines.PlantSystem
{
    public class PlantSystemLog
    {
        private static StreamWriter m_Writer;
        private static bool m_Enabled;

        public static void Initialize()
        {
            if( PlantHelper.EnableLogging )
            {
                // Create the log writer
                try
                {
                    string folder = Path.Combine( Core.BaseDirectory, Path.Combine( "Logs", "PlantSystem" ) );

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
                    Console.WriteLine( "Couldn't initialize plant system log. Reason:" );
                    Console.WriteLine( ex.ToString() );
                    m_Enabled = false;
                }
            }
        }

        public static void WriteInfo( BasePlant plant, string stringToLog )
        {
            if( !m_Enabled || m_Writer == null || plant == null )
                return;

            try
            {
                m_Writer.WriteLine( "Log for plant '{0}' (serial {1}). DateTime {2}", plant.GetType().Name, plant.Serial, DateTime.Now.ToShortTimeString() );
                m_Writer.WriteLine( "\t{0}", stringToLog );
                m_Writer.WriteLine();
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}
