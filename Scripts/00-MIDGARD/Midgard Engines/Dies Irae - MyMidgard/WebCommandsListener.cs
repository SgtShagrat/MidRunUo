/***************************************************************************
 *                               WebCommandsListener.cs
 *                            ----------------------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Server.Accounting;

namespace Midgard.Engines.MyMidgard
{
    public class WebCommandsListener : IDisposable
    {
        public event WebCommandEventHandler OnCommand;

        private HttpListener m_Listener;
        private Thread m_RunningTh;
        private bool m_Connected;
        private bool m_Initialized = true;

        public WebCommandsListener( int port )
        {
            m_Listener = new HttpListener();
            m_Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            string prefix = string.Format( "http://+:{0}/", port );
            m_Listener.Prefixes.Add( prefix );
            m_RunningTh = new Thread( new ThreadStart( RunningFun ) );
            m_RunningTh.Name = "WebCommandsListener Thread";
        }

        public void Start()
        {
            if( !m_Initialized )
                throw new ObjectDisposedException( GetType().Name );

            if( m_Connected )
                return;

            if( m_RunningTh != null && m_RunningTh.IsAlive )
            {
                m_RunningTh.Abort();
                m_RunningTh.Join();
            }
            if( m_RunningTh != null )
                m_RunningTh.Start();
        }

        private void RunningFun()
        {
            try
            {
                if( !m_Initialized )
                    throw new ObjectDisposedException( GetType().Name );

                m_Listener.Start();
                m_Connected = true;

                while( m_Connected )
                {
                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( "BeginGetContext" );
                    var result = m_Listener.BeginGetContext( new AsyncCallback( BeginGetContextCallBack ), m_Listener );
                    result.AsyncWaitHandle.WaitOne();

                    Thread.Sleep( 10 );
                }
            }
            catch( Exception ex )
            {
                if( m_Connected )
                    Config.Pkg.LogErrorLine( ex.ToString() );
            }
        }

        private void BeginGetContextCallBack( IAsyncResult result )
        {
            try
            {
                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "BeginGetContextCallBack" );

                var listener = (HttpListener)result.AsyncState;

                // Call EndGetContext to complete the asynchronous operation.
                var context = listener.EndGetContext( result );
                var request = context.Request;

                // Obtain a response object.
                var response = context.Response;

                if( Config.Debug && request != null && !string.IsNullOrEmpty( request.RawUrl ) )
                    Config.Pkg.LogInfoLine( request.RawUrl.Substring( 1 ) );

                #region args

                string[] cmdargs = request.RawUrl.Substring( 1 ).Split( '?' );
                string cmd = cmdargs[ 0 ];
                var margs = new Dictionary<string, string>( StringComparer.InvariantCultureIgnoreCase );

                foreach( var str in request.QueryString.AllKeys )
                {
                    margs.Add( str, request.QueryString[str] );
                }

                #endregion

                using( var args = new OnCommandEventArgs( cmd, margs, request.ContentEncoding ) )
                {
                    args.ResponseMimeType = "text/plain"; //always respond in text/plat, heach command must set it's own mimetype
                    byte[] tosend = null;

                    try
                    {
                        if( !Config.Enabled )
                            throw new Exception( Config.Pkg.Title + " is disabled by config." );

                        if( cmd.ToLower() == "favicon.ico" )
                        {
                            var icofile = Path.Combine( "Server", "thirdCrown.ico" );
                            args.ResponseMimeType = "image/x-icon";
                            if( !File.Exists( icofile ) )
                                icofile = Path.Combine( "Server", "runuo.ico" );
                            if( File.Exists( icofile ) )
                                tosend = File.ReadAllBytes( icofile );
                        }
                        else
                        {
                            //check credentials...
                            var user = margs.ContainsKey( "user" ) ? margs[ "user" ] : null;
                            var pass = margs.ContainsKey( "pass" ) ? margs[ "pass" ] : null;

                            if( string.IsNullOrEmpty( user ) )
                                throw new Exception( "Missing credentials: user=&pass=" );

                            var account = Accounts.GetAccount( user );

                            if( account == null )
                                throw new Exception( "User not exists: " + user );
                            if( !account.CheckPassword( pass ) )
                                throw new Exception( "Invalid password for user: " + user );

                            args.CompressData = margs.ContainsKey( "compressresult" ) ? margs[ "compressresult" ] == "true" : false;
                            args.Account = account;

                            if( OnCommand != null )
                                OnCommand( this, args );

                            tosend = args.CompressData ? Compress( args.DataBuffer ) : args.DataBuffer;
                        }
                    }
                    catch( Exception ex )
                    {
                        Config.Pkg.LogErrorLine( ex.ToString() );

                        using( var writer = new StreamWriter( response.OutputStream, args.StreamEncoding ) )
                        {
                            args.ResponseMimeType = "text/plain";
                            writer.WriteLine( "ERROR" );
                            writer.WriteLine( ex.Message );
                            return; //this is not a critical error
                        }
                    }

                    response.ContentType = args.ResponseMimeType;
                    foreach(var elem in args.ResponseHeaders)
                    {
                        response.AddHeader(elem.Key, elem.Value);
                    }
                    // set mimetype setted by each command
                    // response.AddHeader("Content-Disposition", "attachment; filename=\"" + cmd + "\"");
                    // response.AddHeader("Content-Length", "" + args.DataBuffer.Length);

                    if( tosend == null )
                        tosend = new byte[ 0 ];
                    response.OutputStream.Write( tosend, 0, tosend.Length );
                    response.OutputStream.Close();

                    if( Config.Debug )
                        Config.Pkg.LogInfoLine( "Sent {0} bytes of \"{1}\" ({2}).", args.DataBuffer.Length, args.ResponseMimeType, args.StreamEncoding );
                }
            }
            catch( Exception ex )
            {
                if( m_Connected )
                    Config.Pkg.LogErrorLine( ex.ToString() );
            }
        }

        private static byte[] Compress( byte[] dataBuffer )
        {
            if( dataBuffer.Length == 0 )
                return dataBuffer;

            if( Config.Debug )
                Config.Pkg.LogInfo( "Compressing..." );

            using( var memstream = new MemoryStream() )
            {
                memstream.Write( BitConverter.GetBytes( (uint)dataBuffer.Length ), 0, 4 );
                memstream.Flush();

                var deflater = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater( 9 );
                var zOut = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream( memstream, deflater );
                zOut.Write( dataBuffer, 0, dataBuffer.Length );
                zOut.Flush();
                memstream.Flush();

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "done!" );

                return memstream.ToArray();
            }
        }

        public void Dispose()
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Disposing..." );

            m_Initialized = false;
            m_Connected = false;

            if( m_Listener != null )
            {
                m_Listener.Abort();

                try
                {
                    m_Listener.Close();
                }
                catch( Exception ex )
                {
                    Config.Pkg.LogInfoLine( ex.ToString() );
                }
            }

            m_Listener = null;

            if( m_RunningTh != null && m_RunningTh.IsAlive )
            {
                m_RunningTh.Abort();
                m_RunningTh.Join();
            }

            m_RunningTh = null;
        }
    }
}