/***************************************************************************
 *                               Utility.cs
 *                            ----------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)				
 *
 ***************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Midgard.Engines.MyMidgard
{
    public static class Utility
    {
        public static string XmlDocumentToString( XmlDocument xmlDoc )
        {
            StringWriter sw = new StringWriter();

            XmlTextWriter writer = new XmlTextWriter( sw );
            writer.Formatting = Formatting.Indented;
            xmlDoc.WriteTo( writer );

            return sw.ToString();
        }

        public static string XDocumentToString( XDocument xDoc )
        {
            StringWriter sw = new StringWriter();

            XmlTextWriter writer = new XmlTextWriter( sw );
            writer.Formatting = Formatting.Indented;
            xDoc.WriteTo( writer );

            return sw.ToString();
        }

        public static string SafeString( string input )
        {
            if( input == null )
                return "";

            StringBuilder sb = null;

            for( int i = 0; i < input.Length; ++i )
            {
                char c = input[ i ];

                if( c < 0x20 || c > 0x80 )
                {
                    AppendCharEntity( input, i, ref sb, c );
                }
                else
                {
                    switch( c )
                    {
                        case '&': AppendEntityRef( input, i, ref sb, "&amp;" ); break;
                        case '>': AppendEntityRef( input, i, ref sb, "&gt;" ); break;
                        case '<': AppendEntityRef( input, i, ref sb, "&lt;" ); break;
                        case '"': AppendEntityRef( input, i, ref sb, "&quot;" ); break;
                        case '\'':
                        case ':':
                        case '/':
                        case '\\': AppendCharEntity( input, i, ref sb, c ); break;
                        default:
                            {
                                if( sb != null )
                                    sb.Append( c );

                                break;
                            }
                    }
                }
            }

            if( sb != null )
                return sb.ToString();

            return input;
        }

        public static void AppendCharEntity( string input, int charIndex, ref StringBuilder sb, char c )
        {
            if( sb == null )
            {
                if( charIndex > 0 )
                    sb = new StringBuilder( input, 0, charIndex, input.Length + 20 );
                else
                    sb = new StringBuilder( input.Length + 20 );
            }

            sb.Append( "&#" );
            sb.Append( (int)c );
            sb.Append( ";" );
        }

        public static void AppendEntityRef( string input, int charIndex, ref StringBuilder sb, string ent )
        {
            if( sb == null )
            {
                if( charIndex > 0 )
                    sb = new StringBuilder( input, 0, charIndex, input.Length + 20 );
                else
                    sb = new StringBuilder( input.Length + 20 );
            }

            sb.Append( ent );
        }

        public static void AppendLineStart( ref StringBuilder sb )
        {
            sb.Append( Config.LineStart );
        }

        public static void AppendLineEnd( ref StringBuilder sb )
        {
            sb.Append( Config.LineEnd );
        }

        public static void AppendEntrySep( ref StringBuilder sb )
        {
            sb.Append( Config.EntrySep );
        }

        public static string SafeGetKey( Dictionary<string, string> args, string key )
        {
            return SafeGetKey( args, key, null );
        }

        public static string SafeGetKey( Dictionary<string, string> args, string key, string defaultvalueifnotexists )
        {
            return args.ContainsKey( key ) ? args[ key ] : defaultvalueifnotexists;
        }
    }

    /*
    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument( this XDocument xDocument )
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load( xDocument.CreateReader() );
            return xmlDocument;
        }

        public static XDocument ToXDocument( this XmlDocument xmlDocument )
        {
            using( var nodeReader = new XmlNodeReader( xmlDocument ) )
            {
                nodeReader.MoveToContent();
                return XDocument.Load( nodeReader );
            }
        }
    }
    */
}