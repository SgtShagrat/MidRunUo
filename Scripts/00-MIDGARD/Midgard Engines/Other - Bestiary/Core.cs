using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Xml;
using Midgard.Misc;
using Server;
using Server.Commands;
using Server.Network;

namespace Midgard.Engines.Bestiary
{
    public static class Core
    {
        public static readonly string ShardName = Midgard2Persistance.ServerName;

        /// <summary>
        /// Maps typeof mobile to its BeastInfo
        /// </summary>
        public static Dictionary<Type, BeastInfo> TypeRegistry = new Dictionary<Type, BeastInfo>();

        /// <summary>
        /// Should we use the bitmap fix system for inconsistent bodyIds?
        /// </summary>
        public static bool UseFixes;

        /// <summary>
        /// Heart and soul of the system, first it analyzes all mobiles and then generates a webpage 
        /// from the result.
        /// </summary>
        public static void Generate()
        {
            Alphabetctionary<List<MobileEntry>> entries = new Alphabetctionary<List<MobileEntry>>();
            List<MobileEntry> all = new List<MobileEntry>();

            entries.Initialize();

            foreach( Type type in TypeRegistry.Keys )
            {
                MobileEntry entry = new MobileEntry( type );

                if( !entry.GuessEmpty ) // TODO: log empty types so they can be excluded from the config file.
                {
                    entries[ TypeRegistry[ type ].Name ].Add( entry );
                }
            }

            // this can't be done in the loop up there 'cause we need :all: to be sorted
            foreach( List<MobileEntry> var in entries )
            {
                all.AddRange( var );
            }

            using( StreamWriter writer = new StreamWriter( Path.Combine( "./Bestiary/", "index.html" ) ) )
            {
                int index = 0;
                char letter = 'a';

                foreach( List<MobileEntry> var in entries )
                {
                    if( var.Count != 0 )
                    {
                        writer.WriteLine( "<font size=\"4\">{0}</font> ({1} {2})", letter++, var.Count, ( var.Count == 1 ? "mobile" : "mobiles" ) );
                        writer.WriteLine( "	<div style=\"padding-left: 15px\">" );

                        foreach( MobileEntry entry in var )
                        {
                            writer.WriteLine( "		<a href=\"./content/mobile.{0}.html\">{1}</a><br />", entry.MasterType.Name, entry.Name );

                            if( index != 0 && all.Count != 1 )
                            {
                                entry.PrevLink = string.Format( "		<a href=\"mobile.{0}.html\" style=\"font-weight: bold; font-size: 11px; color: #ccc\">&lt; {1}</a>", all[ index - 1 ].MasterType.Name, all[ index - 1 ].Name );
                            }

                            if( ( index + 1 ) != all.Count )
                            {
                                entry.NextLink = string.Format( "		<a href=\"mobile.{0}.html\" style=\"font-weight: bold; font-size: 11px; color: #ccc\">{1} &gt;</a>", all[ index + 1 ].MasterType.Name, all[ index + 1 ].Name );
                            }

                            using( StreamWriter entryWriter = new StreamWriter( Path.Combine( "./Bestiary/content/", string.Format( "mobile.{0}.html", entry.MasterType.Name ) ) ) )
                            {
                                entryWriter.Write( entry.Html );
                            }

                            ++index;
                        }

                        writer.WriteLine( "	</div>" );
                        writer.WriteLine( "	<hr noshade />" );
                    }
                }
            }
            // all mobiles, unless they're empty, have been indexed. Our job's done!

            #region stylish index
            using( StreamWriter op = new StreamWriter( "bestiaryIndex.html" ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Bestiary" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    html.AddAttribute( HtmlTextWriterAttribute.Border, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellpadding, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellspacing, "0" );
                    html.RenderBeginTag( HtmlTextWriterTag.Table );

                    html.AddAttribute( HtmlTextWriterAttribute.Align, "center" );
                    html.AddAttribute( HtmlTextWriterAttribute.Bgcolor, "#800000" );
                    html.AddAttribute( HtmlTextWriterAttribute.Style, "font-family:Arial;font-size:12pt;color:#ffffcc" );
                    html.RenderBeginTag( HtmlTextWriterTag.Tr );

                    html.AddAttribute( HtmlTextWriterAttribute.Colspan, 1.ToString() );
                    html.RenderBeginTag( HtmlTextWriterTag.Td );
                    html.Write( "Midgard Third Crown Bestiary" );
                    html.RenderEndTag(); // Td

                    html.RenderEndTag(); // Tr

                    #region Documentation Column Headings
                    char letter = 'a';

                    foreach( List<MobileEntry> var in entries )
                    {
                        if( var.Count == 0 )
                        {
                            letter++;
                            continue;
                        }

                        var.Sort();

                        html.AddAttribute( HtmlTextWriterAttribute.Align, "center" );
                        html.AddAttribute( HtmlTextWriterAttribute.Style, "font-family:Arial;font-size:12pt;font-weight:bold;" );
                        html.RenderBeginTag( HtmlTextWriterTag.Tr );

                        html.RenderBeginTag( HtmlTextWriterTag.Td );
                        html.Write( letter++ );
                        html.RenderEndTag(); // Td

                        html.RenderEndTag(); // Tr

                        foreach( MobileEntry entry in var )
                        {
                            html.AddAttribute( HtmlTextWriterAttribute.Align, "center" );
                            html.AddAttribute( HtmlTextWriterAttribute.Style, "font-family:Arial;font-size:12pt;" );
                            html.RenderBeginTag( HtmlTextWriterTag.Tr );

                            html.RenderBeginTag( HtmlTextWriterTag.Td );

                            html.AddAttribute( HtmlTextWriterAttribute.Href, string.Format( "./Bestiary/content/mobile.{0}.html", entry.MasterType.Name ) );
                            html.RenderBeginTag( HtmlTextWriterTag.A );
                            html.Write( entry.Name );
                            html.RenderEndTag(); // A

                            html.RenderEndTag(); // Td

                            html.RenderEndTag(); // Tr
                        }
                    }
                    #endregion
                    html.RenderEndTag(); // Table

                    html.Write( "<br><br>" );
                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
            #endregion
        }

        /// <summary>
        /// Read the XML config file.
        /// </summary>
        public static void ReadXMLList()
        {
            string filePath = Path.Combine( "Data/Bestiary", "data.xml" );

            if( !File.Exists( filePath ) )
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load( filePath );

            XmlElement root = doc[ "mobiles" ];

            UseFixes = bool.Parse( Utility.GetAttribute( root, "useFixes", "false" ) );

            if( root == null )
                return;

            foreach( XmlElement mobile in root.GetElementsByTagName( "mobile" ) )
            {
                string typeName = Utility.GetAttribute( mobile, "type", null );
                Type t = Type.GetType( typeName );

                if( t == null )
                {
                    Console.WriteLine( "Mobile type: {0} doesn't exist. Skipping.", typeName );
                    continue;
                }

                string name = Utility.GetAttribute( mobile, "name", "empty" );
                string background = Utility.GetAttribute( mobile, "background", null );

                if( !TypeRegistry.ContainsKey( t ) )
                    TypeRegistry.Add( t, new BeastInfo( name, background ) );
                else
                    Config.Pkg.LogError( "Warning: \"{0}\" is already in the TypeRegistry.", t.Name );
            }
        }

        public static void RegisterCommands()
        {
            CommandSystem.Register( "GenerateBestiary", AccessLevel.Developer, GenerateBestiary_OnCommand );
        }

        [Usage( "[GenerateBestiary" )]
        [Description( "Generates xml data for bestiary system." )]
        private static void GenerateBestiary_OnCommand( CommandEventArgs e )
        {
            World.Broadcast( 0x35, true, "Bestiary is being generated, please wait." );
            Console.WriteLine( "Bestiary is being generated, please wait." );

            NetState.FlushAll();
            NetState.Pause();

            DateTime startTime = DateTime.Now;

            ReadXMLList();
            Generate();

            DateTime endTime = DateTime.Now;

            NetState.Resume();

            World.Broadcast( 0x35, true, "Bestiary has been completed. The entire process took {0:F1} seconds.", ( endTime - startTime ).TotalSeconds );
            Console.WriteLine( "Bestiary complete." );
        }
    }
}