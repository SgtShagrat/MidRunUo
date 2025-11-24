using System;
using System.IO;

using Server;

namespace Midgard.Engines.CommercialSystem
{
    public class CommercialLog
    {
        private static StreamWriter m_Writer;
        private static bool m_Enabled;

        public static void InitLog()
        {
            if( Config.Enabled )
            {
                try
                {
                    string folder = Path.Combine( Server.Core.BaseDirectory, Config.CommercialLogPath );

                    if( !Directory.Exists( folder ) )
                        Directory.CreateDirectory( folder );

                    string name = string.Format( "{0}.txt", DateTime.Now.ToLongDateString() );
                    string file = Path.Combine( folder, name );

                    m_Writer = new StreamWriter( file, true );
                    m_Writer.AutoFlush = true;

                    m_Writer.WriteLine( "###############################" );
                    m_Writer.WriteLine( "# Started on: {0} - {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                    m_Writer.WriteLine( "###############################" );
                    m_Writer.WriteLine();

                    m_Enabled = true;
                }
                catch( Exception err )
                {
                    Console.WriteLine( "Couldn't initialize commercial system log. Reason:" );
                    Console.WriteLine( err.ToString() );
                    m_Enabled = false;
                }
            }
        }

        public static void WriteBuyFromVendorAction(Mobile from, Type type, int amount, int price)
        {
            if( !m_Enabled || m_Writer == null )
                return;

            try
            {
                m_Writer.WriteLine( "Buy Action: {0} at {1}, {2} (0x{3:X4}) - type: {4} - amount: {5} - price {6}",
                                    DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
                                    from.Name ?? "noName", from.Serial.Value, type.Name, amount, price );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        public static void WriteSellToVendorAction(Mobile from, Type type, int amount, int price)
        {
            try
            {
                m_Writer.WriteLine( "Sell Action: {0} at {1}, {2} (0x{3:X4}) - type: {4} - amount: {5} - price {6}",
                                    DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(),
                                    from.Name ?? "noName", from.Serial.Value, type.Name, amount, price );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}