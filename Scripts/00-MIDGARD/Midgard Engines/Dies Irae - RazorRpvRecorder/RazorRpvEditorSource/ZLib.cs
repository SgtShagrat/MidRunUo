/*
using System;
using System.Runtime.InteropServices;

namespace Midgard.Engines.RazorRpvRecorder
{
    internal enum ZLibError
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

    [Flags]
    internal enum ZLibCompressionLevel
    {
        Z_BEST_COMPRESSION = 9,
        Z_BEST_SPEED = 1,
        Z_DEFAULT_COMPRESSION = -1,
        Z_NO_COMPRESSION = 0
    }

    internal class ZLib
    {
        [DllImport( "zlib" )]
        public static extern ZLibError compress( byte[] dest, ref int destLength, byte[] source, int sourceLength );

        [DllImport( "zlib" )]
        public static extern ZLibError compress2( byte[] dest, ref int destLength, byte[] source, int sourceLength,
                                                  ZLibCompressionLevel level );

        [DllImport( "zlib" )]
        public static extern ZLibError uncompress( byte[] dest, ref int destLen, byte[] source, int sourceLen );

        [DllImport( "zlib" )]
        public static extern string zlibVersion();
    }
}
*/