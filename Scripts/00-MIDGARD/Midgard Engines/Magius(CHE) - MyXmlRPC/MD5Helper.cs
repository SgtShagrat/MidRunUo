using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Midgard.Misc
{
    public class MD5Helper : IDisposable
    {
        public delegate void CalculateMD5HashProgressHandler( CalculateMD5HashProgressHandlerArgs args );

        private readonly string m_Key;
        private readonly MD5 m_MD5;

        public MD5Helper( string key )
        {
            m_MD5 = MD5.Create();
            m_Key = key;
        }

        public string HashString( string value )
        {
            string tot = value + m_Key;
            byte[] datas = Encoding.Default.GetBytes( tot );
            byte[] retdata = m_MD5.TransformFinalBlock( datas, 0, datas.Length );
            StringBuilder ret = new StringBuilder();
            foreach( byte by in m_MD5.Hash )
            {
                ret.Append( Convert.ToString( by, 16 ).PadLeft( 2, '0' ).ToUpper() );
            }
            return ret.ToString();
        }

        public static byte[] Hash( byte[] datas )
        {
            MD5 md5 = MD5.Create();
            byte[] retdata = md5.TransformFinalBlock( datas, 0, datas.Length );
            return retdata;
        }

        public static string HashString( string key, string value )
        {
            using( MD5Helper help = new MD5Helper( key ) )
            {
                return help.HashString( value );
            }
        }

        /// <summary>
        /// Calculate Md5 hash on single file.
        /// </summary>
        /// <param name="filename">File to scan</param>
        /// <returns>Md5 Hash (string of 32 char each 2 chars = 1 byte)</returns>
        public static string CalculateMD5Hash( string filename )
        {
            return CalculateMD5Hash( filename, null );
        }

        /// <summary>
        /// Calculate Md5 hash on single file.
        /// </summary>
        /// <param name="filename">File to scan</param>
        /// <param name="callback"></param>
        /// <returns>Md5 Hash (string of 32 char each 2 chars = 1 byte)</returns>
        public static string CalculateMD5Hash( string filename, CalculateMD5HashProgressHandler callback )
        {
            MD5 mmd = MD5.Create();

            FileStream stream = null;
            try
            {
                stream = new FileInfo( filename ).OpenRead();
                int block = 1024 * 512;
                long actualpos = stream.Position;
                long length = stream.Length;
                byte[] inputhash = new byte[ block ];
                while( actualpos < length )
                {
                    DateTime lastreadtime = DateTime.Now;
                    int readen = stream.Read( inputhash, 0, block );
                    actualpos += readen;
                    long msec = Convert.ToInt64( DateTime.Now.Subtract( lastreadtime ).TotalMilliseconds );
                    if( actualpos < length )
                    {
                        mmd.TransformBlock( inputhash, 0, readen, inputhash, 0 );
                    }
                    else
                    {
                        mmd.TransformFinalBlock( inputhash, 0, readen );
                    }
                    if( msec < 100 )
                    {
                        block += 1024;
                        inputhash = new byte[ block ];
                        Thread.Sleep( 1 );
                    }
                    else if( msec > 500 )
                    {
                        block = Math.Max( 1024, block - 1024 );
                    }
                    if( callback != null )
                    {
                        CalculateMD5HashProgressHandlerArgs args = new CalculateMD5HashProgressHandlerArgs( length, actualpos, readen, (float)( actualpos / (double)length ) );
                        callback( args );
                        if( args.Cancel )
                        {
                            return null;
                        }
                    }
                }

                StringBuilder st = new StringBuilder();
                foreach( byte by in mmd.Hash )
                {
                    st.Append( Convert.ToString( by, 16 ).PadLeft( 2, '0' ).ToUpper() );
                }
                return st.ToString();
            }
            catch
            {
            }
            finally
            {
                if( stream != null )
                {
                    stream.Close();
                }
            }
            return null;
        }

        public void Dispose()
        {
            m_MD5.Clear();
        }

        public class CalculateMD5HashProgressHandlerArgs : EventArgs
        {
            public readonly long BytesChunkScanned;
            public readonly long BytesScanned;
            public readonly float Percentage;
            public readonly long TotalBytes;
            public bool Cancel = false;

            public CalculateMD5HashProgressHandlerArgs( long totalbytes, long bytesscanned, long bytesChunkScanned, float percentage )
            {
                TotalBytes = totalbytes;
                BytesScanned = bytesscanned;
                Percentage = percentage;
                BytesChunkScanned = bytesChunkScanned;
            }
        }
    }
}