using System;
using System.IO;

namespace Midgard.Engines.WarSystem
{
    public class Logger
    {
        private static StreamWriter m_Writer;

        internal static void InitLogger()
        {
            if( !Config.LogEnabled )
                return;

            string folder = Path.Combine( Path.Combine( Server.Core.BaseDirectory, "Logs" ), "WarSystem" );

            if( !Directory.Exists( folder ) )
                Directory.CreateDirectory( folder );

            string name = string.Format( "WarSystem_{0}.txt", DateTime.Now.ToLongDateString() );
            string file = Path.Combine( folder, name );

            try
            {
                m_Writer = new StreamWriter( file, true );
                m_Writer.AutoFlush = true;

                m_Writer.WriteLine( "###############################" );
                m_Writer.WriteLine( "# {0} - {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                m_Writer.WriteLine();
            }
            catch( Exception err )
            {
                Console.WriteLine( "Couldn't initialize wars system log. Reason:" );
                Console.WriteLine( err.ToString() );
            }
        }

        /// <summary>
        /// Log a string to our stream logger
        /// </summary>
        public static void Log( string message )
        {
            if( m_Writer == null )
                return;

            try
            {
                m_Writer.WriteLine( "{0}\t{1}", DateTime.Now, message );

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( message );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        /// <summary>
        /// Log a formatted string to our stream logger
        /// </summary>
        public static void Log( string format, params object[] args )
        {
            Log( String.Format( format, args ) );
        }
    }
}