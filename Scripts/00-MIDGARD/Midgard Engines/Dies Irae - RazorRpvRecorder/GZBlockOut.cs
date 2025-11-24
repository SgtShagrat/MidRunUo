#if !DONOTCOMPILEDEBUGSCRIPT_

using System;
using System.IO;

using Server.Network;

namespace Midgard.Engines.RazorRpvRecorder
{
    internal class GZBlockOut : Stream
    {
        private static byte[] m_CompBuff;
        private bool m_IsCompressed = true;

        public GZBlockOut( string filename, int blockSize )
        {
            Raw = new BinaryWriter( new FileStream( filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None ) );
            BlockSize = blockSize;
            Buffer = new MemoryStream( blockSize + 0x400 );
            Compressed = new BinaryWriter( this );
        }

        public int BlockSize { get; set; }

        public MemoryStream Buffer { get; private set; }

        public bool BufferAll { get; set; }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public BinaryWriter Compressed { get; private set; }

        public bool IsCompressed
        {
            get { return m_IsCompressed; }
            set
            {
                ForceFlush();
                m_IsCompressed = value;
            }
        }

        public override long Length
        {
            get { return RawStream.Length; }
        }

        public override long Position
        {
            get { return !m_IsCompressed ? RawStream.Position : Buffer.Position; }
            set { }
        }

        public BinaryWriter Raw { get; private set; }

        public Stream RawStream
        {
            get { return Raw.BaseStream; }
        }

        public override void Close()
        {
            ForceFlush();

            base.Close();

            Raw.Close();
            Buffer.Close();

            Compressed = null;
        }

        public override void Flush()
        {
            FlushBuffer();
            RawStream.Flush();
        }

        public void FlushBuffer()
        {
            if( ( m_IsCompressed && !BufferAll ) && ( Buffer.Position > 0L ) )
            {
                int destLength = (int)( Buffer.Position * 1.1 );
                if( ( m_CompBuff == null ) || ( m_CompBuff.Length < destLength ) )
                    m_CompBuff = new byte[ destLength ];
                else
                    destLength = m_CompBuff.Length;

                ZLibError error = Compression.Pack( m_CompBuff, ref destLength, Buffer.ToArray(), (int)Buffer.Position, ZLibQuality.Size );

                if( error != ZLibError.Okay )
                    throw new Exception( "ZLib error during copression: " + error );

                Raw.Write( destLength );
                Raw.Write( (int)Buffer.Position );
                Raw.Write( m_CompBuff, 0, destLength );
                Buffer.Position = 0L;
            }
        }

        public void ForceFlush()
        {
            bool bufferAll = BufferAll;
            BufferAll = false;
            Flush();
            BufferAll = bufferAll;
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            return 0;
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            return m_IsCompressed ? Buffer.Seek( offset, origin ) : RawStream.Seek( offset, origin );
        }

        public override void SetLength( long value )
        {
            RawStream.SetLength( value );
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            if( m_IsCompressed )
            {
                Buffer.Write( buffer, offset, count );
                if( Buffer.Position >= BlockSize )
                    FlushBuffer();
            }
            else
                RawStream.Write( buffer, offset, count );
        }

        public override void WriteByte( byte value )
        {
            if( m_IsCompressed )
            {
                Buffer.WriteByte( value );
                if( Buffer.Position >= BlockSize )
                    FlushBuffer();
            }
            else
                RawStream.WriteByte( value );
        }
    }
}

#endif