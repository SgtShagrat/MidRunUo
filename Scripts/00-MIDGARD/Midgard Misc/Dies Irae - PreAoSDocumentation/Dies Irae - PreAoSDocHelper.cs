using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.UI;

using Server;
using Server.Commands;
using Server.Network;

using HtmlTag = System.Web.UI.HtmlTextWriterTag;
using HtmlAttr = System.Web.UI.HtmlTextWriterAttribute;
using Midgard.Engines.Packager;

namespace Midgard.Misc
{
    public static class PreAoSDocHelper
    {
        private static bool AutoStart = true;

#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif

        public static object[] Package_Info =
        {
            "Script Title", "PreAoS DocHelper",
            "Enabled by Default", true,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name", "Dies Irae",
            "Creation Date", new DateTime( 2009, 10, 20 ),
             "Author mail-contact", "tocasia@alice.it",
             "Author home site", "http://www.midgardshard.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Dies Irae",
            "Provided packages", new string[] { "Midgard.Misc.PreAoSDocHelper" },
            "Research tags", new string[] { "PreAoSDocHelper", "Doc", "Html", "Web" }
        };

        internal static Package Pkg;

        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Midgard.Engines.Packager.Core.Singleton[ typeof( PreAoSDocHelper ) ];

            Register( new WeaponsExtendedDocHandler() );
        }

        private static bool DuringInitialization = false;
        public static void Package_Initialize()
        {
            DuringInitialization = true;

            CommandSystem.Register( "ThirdCrownDocGen", AccessLevel.Administrator, new CommandEventHandler( DocGen_OnCommand ) );

            try
            {
                if( AutoStart )
                    DocGen_OnCommand( null );
            }
            finally
            {
                DuringInitialization = false;
            }
        }

        [Usage( "ThirdCrownDocGen" )]
        [Description( "Generates ThirdCrown documentation." )]
        private static void DocGen_OnCommand( CommandEventArgs e )
        {
            if( !DuringInitialization )
                World.Broadcast( 0x35, true, "Third Crown Documentation is being generated, please wait." );
            else
                Pkg.LogInfo( "Third Crown Documentation is being generated, please wait..." );

            NetState.FlushAll();
            NetState.Pause();

            DateTime startTime = DateTime.Now;

            Document();

            DateTime endTime = DateTime.Now;

            NetState.Resume();

            if( !DuringInitialization )
                World.Broadcast( 0x35, true,
                             "Third Crown Documentation has been completed. The entire process took {0:F1} seconds.",
                             ( endTime - startTime ).TotalSeconds );
            else
                Pkg.LogInfoLine( "done in {0:F1} seconds.", ( endTime - startTime ).TotalSeconds );
        }

        #region FileSystem

        private static readonly char[] ReplaceChars = "<>".ToCharArray();

        public static string GetFileName( string root, string name, string ext )
        {
            if( name.IndexOfAny( ReplaceChars ) >= 0 )
            {
                StringBuilder sb = new StringBuilder( name );

                foreach( char t in ReplaceChars )
                {
                    sb.Replace( t, '-' );
                }

                name = sb.ToString();
            }

            int index = 0;
            string file = String.Concat( name, ext );

            while( File.Exists( Path.Combine( root, file ) ) )
            {
                file = String.Concat( name, ++index, ext );
            }

            return file;
        }

        private static readonly string m_RootDirectory = Path.GetDirectoryName( Environment.GetCommandLineArgs()[ 0 ] );

        public static void EnsureDirectory( string path )
        {
            path = Path.Combine( m_RootDirectory, path );

            if( !Directory.Exists( path ) )
                Directory.CreateDirectory( path );
        }

        private static void DeleteDirectory( string path )
        {
            path = Path.Combine( m_RootDirectory, path );

            if( Directory.Exists( path ) )
                Directory.Delete( path, true );
        }

        /*
                private static StreamWriter GetWriter( string root, string name )
                {
                    return new StreamWriter( Path.Combine( Path.Combine( m_RootDirectory, root ), name ) );
                }
        */

        /*
                private static StreamWriter GetWriter( string path )
                {
                    return new StreamWriter( Path.Combine( m_RootDirectory, path ) );
                }
        */

        #endregion

        public static string DocDir = "docs3c";

        private static void Document()
        {
            try
            {
                DeleteDirectory( "docs3c/" );
            }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
                return;
            }

            EnsureDirectory( DocDir );

            foreach( DocumentationHandler handler in m_Handlers )
            {
                if( handler != null && handler.Enabled )
                    handler.GenerateDocumentation();
            }
        }

        private static readonly List<DocumentationHandler> m_Handlers = new List<DocumentationHandler>();

        public static void Register( DocumentationHandler handler )
        {
            if( handler != null && !m_Handlers.Contains( handler ) )
                m_Handlers.Add( handler );
        }

        static PreAoSDocHelper()
        {
            ImageAliasesDict = new Dictionary<string, string>();
        }

        private static void DisplayImage( HtmlTextWriter html, string source, string content )
        {
            // <a href="images/BlueTent.png">BlueTent</a>
            html.AddAttribute( HtmlTextWriterAttribute.Href, source );
            html.RenderBeginTag( HtmlTextWriterTag.A );
            html.Write( content );
            html.RenderEndTag();

            //html.Write( "<!-- " );

            //html.AddAttribute( HtmlTextWriterAttribute.Href, "#" );
            //html.AddAttribute( HtmlTextWriterAttribute.Onclick,
            //                   String.Format(
            //                       "javascript:window.open('images/{0}.png','ChildWindow','width={1},height={2},resizable=no,status=no,toolbar=no')",
            //                       fileName, bmp.Width + 30, bmp.Height + 80 ) );
            //html.RenderBeginTag( HtmlTextWriterTag.A );
            //html.Write( fileName );
            //html.RenderEndTag();

            //html.Write( " -->" );
        }

        private static Bitmap GetBitmapByName( string fileName )
        {
            if( !File.Exists( fileName ) )
                return null;

            Image image = Image.FromFile( fileName );
            return new Bitmap( image );
        }

        public static string SafeFileName( string name )
        {
            return name.ToLower().Replace( ' ', '_' );
        }

        public static void AppendTable( HtmlTextWriter html, string title, ICollection<string> headers, IList<List<string>> content, bool isSelfContained, bool images, string relativeImageDirectory )
        {
            if( isSelfContained )
            {
                html.AddAttribute( HtmlTextWriterAttribute.Border, "1" );
                html.AddAttribute( HtmlTextWriterAttribute.Cellpadding, "1" );
                html.AddAttribute( HtmlTextWriterAttribute.Cellspacing, "0" );
                html.RenderBeginTag( HtmlTextWriterTag.Table );
            }

            #region Title of the Documentation

            // <td><font face=\"Arial\" color=ffffcc size=\"2\"><A name=\"{0}\">{0}</A></font></td>"
            html.AddAttribute( HtmlTextWriterAttribute.Align, "center" );
            html.AddAttribute( HtmlTextWriterAttribute.Bgcolor, "#800000" );
            html.AddAttribute( HtmlTextWriterAttribute.Style, "font-family:Arial;font-size:12pt;color:#ffffcc" );
            html.RenderBeginTag( HtmlTextWriterTag.Tr );

            html.AddAttribute( HtmlTextWriterAttribute.Colspan, headers.Count.ToString() );
            html.RenderBeginTag( HtmlTextWriterTag.Td );
            html.Write( title );
            html.RenderEndTag(); // Td

            html.RenderEndTag(); // Tr

            #endregion

            #region Documentation Column Headings

            if( headers.Count > 0 )
            {
                html.AddAttribute( HtmlTextWriterAttribute.Align, "center" );
                html.AddAttribute( HtmlTextWriterAttribute.Style, "font-family:Arial;font-size:12pt;font-weight:bold;" );
                html.RenderBeginTag( HtmlTextWriterTag.Tr );

                foreach( string s in headers )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Td );
                    html.Write( s );
                    html.RenderEndTag(); // Td
                }

                html.RenderEndTag(); // Tr
            }
            #endregion

            #region Documentation Content

            foreach( List<string> contents in content )
            {
                html.AddAttribute( HtmlTextWriterAttribute.Align, "center" );
                html.AddAttribute( HtmlTextWriterAttribute.Style, "font-family:Arial;font-size:12pt;" );

                int start = 0;

                if( contents.Count > 0 )
                {
                    //check for custom styles
                    if( contents[ 0 ].StartsWith( "{#STYLE:" ) && contents[ 0 ].EndsWith( "}" ) )
                    {
                        start++;

                        string optstyle = contents[ 0 ].Substring( contents[ 0 ].IndexOf( ":" ) + 1 );
                        optstyle = optstyle.Substring( 0, optstyle.Length - 1 );

                        foreach( string elem in optstyle.Split( ';' ) )
                            if( !string.IsNullOrEmpty( elem ) )
                                html.AddStyleAttribute( elem.Split( ':' )[ 0 ], elem.Split( ':' )[ 1 ] );
                    }
                }

                html.RenderBeginTag( HtmlTextWriterTag.Tr );

                for( int i = start; i < contents.Count; i++ )
                {
                    string cont = contents[ i ];

                    if( cont.StartsWith( "{#STYLE:" ) && cont.EndsWith( "}" ) )
                    {
                        string optstyle = cont.Substring( cont.IndexOf( ":" ) + 1 );
                        optstyle = optstyle.Substring( 0, optstyle.Length - 1 );

                        foreach( string elem in optstyle.Split( ';' ) )
                            if( !string.IsNullOrEmpty( elem ) )
                                html.AddStyleAttribute( elem.Split( ':' )[ 0 ], elem.Split( ':' )[ 1 ] );

                        continue;
                    }

                    if( i == start )
                        html.AddAttribute( HtmlTextWriterAttribute.Align, "left" );

                    html.RenderBeginTag( HtmlTextWriterTag.Td );

                    if( i == start && images )
                    {
                        string imageFileName = ImageAliasesDict.ContainsKey( cont ) ? ImageAliasesDict[ cont ] : null;

                        string strippedFileName = Path.GetFileName( imageFileName );

                        if( imageFileName != null )
                        {
                            Bitmap image = GetBitmapByName( imageFileName );
                            if( image != null )
                                DisplayImage( html, Path.Combine( relativeImageDirectory, strippedFileName ), contents[ i ] );
                            else
                            {
                                html.Write( contents[ i ] );
                                Console.WriteLine( "Warning: {0} not found.", imageFileName );
                            }
                        }
                        else
                        {
                            html.Write( contents[ i ] );
                            Console.WriteLine( "Warning: {0} is invalid content.", cont );
                        }
                    }
                    else
                        html.Write( contents[ i ] );

                    html.RenderEndTag(); // Td
                }

                html.RenderEndTag(); // Tr
            }

            #endregion

            if( isSelfContained )
                html.RenderEndTag(); // Table
        }

        public static Dictionary<string, string> ImageAliasesDict { get; set; }
    }
}