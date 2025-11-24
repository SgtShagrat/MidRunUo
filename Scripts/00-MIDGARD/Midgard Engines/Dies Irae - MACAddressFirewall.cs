using System.Collections.Generic;
using System.IO;
using Server;

namespace Midgard.Misc
{
    public class MACAddressFirewall
    {
        public static MACAddressFirewall Instance { get; private set; }

        static MACAddressFirewall()
        {
            Instance = new MACAddressFirewall();

            List = new List<MACAddress>();

            string path = "MACfirewall.cfg";

            if( File.Exists( path ) )
            {
                using( StreamReader ip = new StreamReader( path ) )
                {
                    string line;

                    while( ( line = ip.ReadLine() ) != null )
                    {
                        line = line.Trim();

                        if( line.Length == 0 )
                            continue;

                        long toAdd;
                        if( long.TryParse( line, out toAdd ) )
                            List.Add( new MACAddress( toAdd ) );
                    }
                }
            }
        }

        public static List<MACAddress> List { get; private set; }

        public static void RemoveAt( int index )
        {
            List.RemoveAt( index );
            Save();
        }

        public static void Remove( MACAddress add )
        {
            List.Remove( add );
            Save();
        }

        public static void Add( MACAddress address )
        {
            if( !List.Contains( address ) )
                List.Add( address );

            Save();
        }

        public static void Save()
        {
            string path = "MACfirewall.cfg";

            using( StreamWriter op = new StreamWriter( path ) )
            {
                for( int i = 0; i < List.Count; ++i )
                    op.WriteLine( List[ i ] );
            }
        }

        public static bool IsBlocked( MACAddress address )
        {
            for( int i = 0; i < List.Count; i++ )
            {
                if( List[ i ] == address )
                    return true;
            }

            return false;
        }
    }
}