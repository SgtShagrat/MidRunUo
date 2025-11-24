using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Server.Engines.XmlSpawner2
{
    public class ZLib
    {
        public static bool CheckVersion()
        {
            string[] strArray = null;
            try
            {
                strArray = zlibVersion().Split( new char[]
                                                    {
                                                        '.'
                                                    } );
            }
            catch( DllNotFoundException )
            {
                return false;
            }
            return ( strArray[ 0 ] == "1" );
        }

        [DllImport( "zlib" )]
        private static extern ZLibError compress( byte[] dest, ref int destLength, byte[] source, int sourceLength );

        public static byte[] Compress( object source )
        {
            try
            {
                var serializer = new XmlSerializer( source.GetType() );
                var stream = new MemoryStream();
                serializer.Serialize( stream, source );
                byte[] buffer = stream.ToArray();
                stream.Close();
                int length = buffer.Length;
                int destLength = buffer.Length + 1;
                var dest = new byte[destLength];
                if( compress2( dest, ref destLength, buffer, buffer.Length, ZLibCompressionLevel.Z_BEST_COMPRESSION ) != ZLibError.Z_OK )
                    return new byte[0];
                var destinationArray = new byte[destLength + 4];
                Array.Copy( dest, 0, destinationArray, 4, destLength );
                destinationArray[ 0 ] = (byte) length;
                destinationArray[ 1 ] = (byte) ( length >> 8 );
                destinationArray[ 2 ] = (byte) ( length >> 0x10 );
                destinationArray[ 3 ] = (byte) ( length >> 0x18 );
                return destinationArray;
            }
            catch( Exception )
            {
                return new byte[0];
            }
        }

        public static byte[] Compress( byte[] data )
        {
            int length = data.Length;
            int destLength = data.Length;
            var dest = new byte[data.Length];
            if( compress( dest, ref destLength, data, data.Length ) != ZLibError.Z_OK )
                return null;
            var destinationArray = new byte[destLength + 4];
            Array.Copy( dest, 0, destinationArray, 4, destLength );
            destinationArray[ 0 ] = (byte) ( length & 0xff );
            destinationArray[ 1 ] = (byte) ( ( length >> 8 ) & 0xff );
            destinationArray[ 2 ] = (byte) ( ( length >> 0x10 ) & 0xff );
            destinationArray[ 3 ] = (byte) ( ( length >> 0x18 ) & 0xff );
            return destinationArray;
        }

        [DllImport( "zlib" )]
        private static extern ZLibError compress2( byte[] dest, ref int destLength, byte[] source, int sourceLength, ZLibCompressionLevel level );

        public static byte[] Decompress( byte[] data )
        {
            int destLen = ( ( data[ 0 ] | ( data[ 1 ] << 8 ) ) | ( data[ 2 ] << 0x10 ) ) | ( data[ 3 ] << 0x18 );
            var destinationArray = new byte[data.Length - 4];
            Array.Copy( data, 4, destinationArray, 0, data.Length - 4 );
            var dest = new byte[destLen];
            if( uncompress( dest, ref destLen, destinationArray, destinationArray.Length ) != ZLibError.Z_OK )
                return null;
            return dest;
        }

        public static object Decompress( byte[] data, Type type )
        {
            try
            {
                int destLen = ( ( data[ 0 ] | ( data[ 1 ] << 8 ) ) | ( data[ 2 ] << 0x10 ) ) | ( data[ 3 ] << 0x18 );
                var destinationArray = new byte[data.Length - 4];
                Array.Copy( data, 4, destinationArray, 0, data.Length - 4 );
                var dest = new byte[destLen];
                if( uncompress( dest, ref destLen, destinationArray, destinationArray.Length ) != ZLibError.Z_OK )
                    return null;
                var stream = new MemoryStream( dest );
                object obj2 = new XmlSerializer( type ).Deserialize( stream );
                stream.Close();
                return obj2;
            }
            catch
            {
                return null;
            }
        }

        [DllImport( "zlib" )]
        private static extern ZLibError uncompress( byte[] dest, ref int destLen, byte[] source, int sourceLen );

        [DllImport( "zlib" )]
        private static extern string zlibVersion();

        #region Nested type: ZLibCompressionLevel
        private enum ZLibCompressionLevel
        {
            Z_BEST_COMPRESSION = 9,
            Z_BEST_SPEED = 1,
            Z_DEFAULT_COMPRESSION = -1,
            Z_NO_COMPRESSION = 0
        }
        #endregion

        #region Nested type: ZLibError
        private enum ZLibError
        {
            Z_BUF_ERROR = -5,
            Z_DATA_ERROR = -3,
            Z_ERRNO = -1,
            Z_MEM_ERROR = -4,
            Z_NEED_DICT = 2,
            Z_OK = 0,
            Z_STREAM_END = 1,
            Z_STREAM_ERROR = -2,
            Z_VERSION_ERROR = -6
        }
        #endregion
    }
}