using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Midgard.Engines.MyMidgard;
using Midgard.Engines.Packager;

using Server;
using Server.Accounting;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.MyXmlRPC
{
    public class Core
    {
#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif

        public static object[] Package_Info =
            {
                "Script Title", "MyMidgard XML Remote Protocoll Caller",
                "Enabled by Default", true,
                "Script Version", new Version( 1, 0, 0, 0 ),
                "Author name", "Magius(CHE)",
                "Creation Date", new DateTime( 2009, 10, 20 ),
                "Author mail-contact", "cheghe@tiscali.it",
                "Author home site", "http://www.magius.it",
                //"Author notes",           null,
                "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
                "Provided packages", new string[] { "Midgard.Engines.MyXmlRPC" },
                //"Required packages",       new string[]{"Midgard.Engines.MyMidgard"},
                //"Conflicts with packages",new string[0],
                "Research tags", new string[] { "MyXmlRPC", "Listener", "MyMidgard", "Web", "Xml" }
            };

        internal static Package Pkg;

        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Core ) ];
        }

        public static void Package_Initialize()
        {
            EnsureXmlRPCAccountExistance();
            WebCommandSystem.Register( "xmlrpc", new WebCommandEventHandler( XmlRPCCallBack ) );

            // custom commands
            GetVersionCommand.RegisterCommands();

            // player status commands
            GetAccountSerialsCommand.RegisterCommands();
            GetOnlineIdsCommand.RegisterCommands();
            GetPlayerInfoCommand.RegisterCommands();
            GetPlayerSerialsCommand.RegisterCommands();
            GetPlayersStatusCommand.RegisterCommands();

            // guild status commands
            GetGuildIdsCommand.RegisterCommands();
            GetGuildInfoCommand.RegisterCommands();
            GetGuildStatusCommand.RegisterCommands();
        }

        private static void EnsureXmlRPCAccountExistance()
        {
            if( Accounts.GetAccount( "XmlRpcUser" ) == null )
            {
                var user = "XmlRpcUser";
                var pass = "securexmlpass";
                Pkg.LogWarningLine( "XmlRpcUser account is null and is being created. User:\"{0}\", Pass:\"{1}\"", user, pass );
                var acct = new Account( user, pass, "midgard@midgard.it" );
                acct.SetTag( "xmlrpccaller", "1" );
            }
        }
        public static void Register( string method, MyXmlEventHandler callback, object tag )
        {
            m_Entries.Add( method, new Entry { CallBack = callback, Tag = tag } );
        }

        public static void DeRegister( string method )
        {
            m_Entries.Remove( method );
        }

        private static readonly Dictionary<string, Entry> m_Entries = new Dictionary<string, Entry>();

        public static void XmlRPCCallBack( WebCommandsListener listener, OnCommandEventArgs e )
        {
            e.ResponseMimeType = "text/xml";

            if( Debug )
                Pkg.LogInfoLine( "XmlRPCCallBack begins." );

            var response = new XElement( "response" );
            var doc = new XDocument( new XDeclaration( "1.0", "utf-8", "yes" ), response );
            var starttime = DateTime.Now;
            Int32 exitCode = -1;
            var hasHerror = false;
            var nonCriticalExceptions = new List<Exception>();
            var customresult = new XElement( "custom" );

            MyXmlEventArgs args = null;
            try
            {
                if( !Enabled )
                    throw new Exception( Pkg.Title + " disabled by configuration." );

                var isNOTAuthorized = e.Account.AccessLevel < AccessLevel.Administrator && string.IsNullOrEmpty( ( (Account)e.Account ).GetTag( "xmlrpccaller" ) );
#if DEBUG
                isNOTAuthorized = false;
#endif
                if( isNOTAuthorized )
                    throw new Exception( string.Format( "Account \"{0}\" is not authorized. Set \"xmlrpccaller\" tag to authorize it.", e.Account ) );

                if( !e.Args.ContainsKey( "xcmd" ) )
                    throw new ArgumentNullException( "xcmd" );

                var methodname = e.Args[ "xcmd" ];
                MyXmlEventHandler method = null;

                var remoteargs = new Dictionary<string, string>();
                foreach( var elem in e.Args )
                {
                    if( elem.Key.StartsWith( "xarg" ) )
                        remoteargs.Add( elem.Key.Substring( 4 ), elem.Value );
                }

                lock( m_Entries )
                {
                    if( !m_Entries.ContainsKey( methodname ) )
                        throw new Exception( "Received an unhandled command: " + methodname );
                    var entry = m_Entries[ methodname ];
                    method = entry.CallBack;
                    args = new MyXmlEventArgs( customresult, nonCriticalExceptions, remoteargs, entry.Tag, e.Account );
                }
                method( args );

                exitCode = args.Exitcode;
            }
            catch( Exception ex )
            {
                var formex = FormatException( ex );
                exitCode = -1;
                var cdata = new XCData( formex );
                response.Add( new XElement( "exception", cdata ) );
                if( Debug )
                    Pkg.LogErrorLine( formex );
                hasHerror = true;
            }

            if( nonCriticalExceptions.Count > 0 )
            {
                var warns = new StringBuilder();
                foreach( var exp in nonCriticalExceptions )
                {
                    warns.AppendLine( FormatException( exp ) );
                }
                var cdata = new XCData( warns.ToString() );
                response.Add( new XElement( "warnings", cdata ) );
                if( Debug )
                    Pkg.LogWarningLine( warns.ToString() );
            }

            var endtime = DateTime.Now.Subtract( starttime );
            var result = new XElement( "result", new XAttribute( "exitcode", exitCode ), new XAttribute( "computingtime", (int)endtime.TotalSeconds ) );
            response.Add( result );
            if( args != null )
            {
                if( args.ReturnMessage != null && args.ReturnMessage.Length > 0 )
                    result.Add( new XElement( "message", new XCData( "" + args.ReturnMessage ) ) );
                else
                    result.Add( new XElement( "message" ) );
                foreach( var el in args.Returns )
                {
                    result.Add( new XElement( "var", new XAttribute( "name", el.Key ), new XCData( el.Value ) ) );
                }
            }
            else
            {
                result.Add( new XElement( "message" ) );
            }

            result.Add( customresult );

            var writer = new XmlTextWriter( e.UnderlyngStream, Encoding.UTF8 );
            writer.Formatting = Formatting.Indented;
            doc.WriteTo( writer );
            writer.Flush(); //do not close writer or UnderlyngStream will be disposed.

            if( Debug )
                Pkg.LogInfoLine( Utility.XDocumentToString( doc ) );
            if( Debug )
                Pkg.LogInfoLine( "XmlRPCCallBack ends in {0}, exitcode: {1}, errors: {2}, warnings: {3}.", TimeSpan.FromSeconds( (int)endtime.TotalSeconds ), exitCode, hasHerror ? 1 : 0, nonCriticalExceptions.Count );
        }

        private static string FormatException( Exception ex )
        {
            var result = new StringBuilder();
            var act = ex;
            while( act != null )
            {
                result.AppendLine( ex.ToString() );
                act = act.InnerException;
            }
            return result.ToString();
        }
    }
}