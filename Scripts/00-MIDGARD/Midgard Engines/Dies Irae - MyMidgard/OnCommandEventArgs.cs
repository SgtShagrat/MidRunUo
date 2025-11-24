/***************************************************************************
 *                               OnCommandEventArgs.cs
 *                            ---------------------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)				
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server.Accounting;

namespace Midgard.Engines.MyMidgard
{
    public class OnCommandEventArgs : EventArgs, IDisposable
    {
        private MemoryStream m_DataStream;

        public IAccount Account { get; internal set; }
        public bool CompressData { get; set; }

        public OnCommandEventArgs( string cmd, Dictionary<string, string> args, Encoding streamEncoding )
        {
            Cmd = cmd;
            Args = args;
            Response = null;
            StreamEncoding = streamEncoding;

            m_DataStream = new MemoryStream();
            ResponseHeaders = new Dictionary<string, string>();
        }

        public Encoding StreamEncoding { get; set; }
        public string ResponseMimeType { get; set; }
        public Dictionary<string, string> ResponseHeaders { get; private set; }
        public string Cmd { get; private set; }

        public string Response
        {
            get { return StreamEncoding.GetString( DataBuffer ); }
            set
            {
                using ( m_DataStream )
                    m_DataStream = new MemoryStream();

                m_StreamWriter = null;
                if( !string.IsNullOrEmpty( value ) )
                    StreamWriter.Write( value );
            }
        }

        public byte[] DataBuffer
        {
            get
            {
                if( m_DataStream == null )
                    return new byte[ 0 ];

                if( m_StreamWriter != null )
                    m_StreamWriter.Flush();
                if( m_DataStream != null )
                    m_DataStream.Flush();

                return m_DataStream != null ? m_DataStream.ToArray() : new byte[ 0 ];
            }
        }

        public Stream UnderlyngStream
        {
            get { return m_DataStream; }
        }

        private BinaryWriter m_StreamWriter;

        /// <summary>
        /// Do not dispose this writer, cause internal stream to dispose.
        /// </summary>
        public BinaryWriter StreamWriter
        {
            get
            {
                if( m_StreamWriter == null )
                    m_StreamWriter = new BinaryWriter( m_DataStream, StreamEncoding );
                return m_StreamWriter;
            }
        }

        public Dictionary<string, string> Args { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            if( m_StreamWriter != null )
                m_StreamWriter.Close();
            if( m_DataStream != null )
                m_DataStream.Dispose();
            m_DataStream = null;
            m_StreamWriter = null;
        }

        #endregion
    }
}