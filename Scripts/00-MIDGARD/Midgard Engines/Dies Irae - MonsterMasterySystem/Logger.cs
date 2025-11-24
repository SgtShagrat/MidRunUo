using System;
using System.IO;
using Server.Mobiles;

namespace Midgard.Engines.MonsterMasterySystem
{
    public class Logger
    {
        private static StreamWriter m_Writer;

        public static void Initialize()
        {
            if( Core.Enabled )
            {
                // Create the log writer
                try
                {
                    string file = Path.Combine( Server.Core.BaseDirectory, Path.Combine( "Logs", "EnemyMasterySystem.txt" ) );

                    m_Writer = new StreamWriter( file, true );
                    m_Writer.AutoFlush = true;

                    m_Writer.WriteLine( "###############################" );
                    m_Writer.WriteLine( "# {0} - {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                    m_Writer.WriteLine();
                }
                catch( Exception err )
                {
                    Console.WriteLine( "Couldn't initialize monster system system log. Reason:" );
                    Console.WriteLine( err.ToString() );
                }
            }
        }

        public static void WriteConversion( BaseCreature bc )
        {
            if( m_Writer == null )
                return;

            try
            {
                m_Writer.WriteLine( "#Converted creature: serial {0} - type {1} - region {2} - mastery {3}",
                    bc.Serial.Value.ToString(), bc.GetType().Name, bc.Region.Name, bc.Mastery );

            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}