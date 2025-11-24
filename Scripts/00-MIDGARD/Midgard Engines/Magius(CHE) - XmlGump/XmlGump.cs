using System;
using System.Collections;
using System.Xml.Linq;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.XmlGumps
{
    public abstract class XmlGump : Gump
    {
        private readonly string m_Filename;
        private bool m_Preventdeadlock;
        // private Dictionary<string, string> m_Xmlvariables = new Dictionary<string, string>();

        public XmlGump( string xmlgumpname )
            : base( 0, 0 )
        {
            m_Filename = xmlgumpname;
            Source = Config.LoadGumpXml( xmlgumpname );
        }

        protected XDocument Source { get; private set; }
        public bool Designed { get; private set; }

        public override void SendTo( NetState state )
        {
            if( m_Preventdeadlock )
                return;
            m_Preventdeadlock = true;
            if( !Designed )
            {
                try
                {
                    InternalBeforeDesign();
                    Design();
                    AfterDesign();
                }
                catch( Exception ex )
                {
                    state.Mobile.SendMessage( "Interface error: contact an admin. [" + m_Filename + "]" );
                    Config.Pkg.LogErrorLine( "Error while design gump {0}.", m_Filename );
                    Config.Pkg.LogError( ex );
                }

                Designed = true;
            }
            base.SendTo( state );
            m_Preventdeadlock = false;
        }

        private void InternalBeforeDesign()
        {
            XElement gump = Source.Element( "gump" );

            if( gump != null )
            {
                foreach( XElement node in gump.Elements( "variable" ) )
                {
                    if( node == null )
                        continue;

                    if( Variables.ContainsKey( node.Attribute( "name" ).Value ) )
                        Variables[ node.Attribute( "name" ).Value ] = ParseText( node.Value );
                }
            }

            BeforeDesign();
        }

        /// <summary>
        /// Do not call Send into this method
        /// </summary>
        protected virtual void BeforeDesign()
        {
        }

        /// <summary>
        /// Do not call Send into this method
        /// </summary>
        protected virtual void AfterDesign()
        {
        }

        /// <summary>
        /// Do not call Send into this method
        /// </summary>
        protected virtual void Design()
        {
            XElement gump = Source.Element( "gump" );
            Point2D mloc = ParseLocation( gump );

            X = mloc.X;
            Y = mloc.Y;

            Closable = ParseBool( gump, "closable" );
            Disposable = ParseBool( gump, "disposable" );
            Dragable = ParseBool( gump, "dragable" );
            Resizable = ParseBool( gump, "resizable" );

            if( gump != null )
            {
                foreach( XElement upage in gump.Elements( "page" ) )
                {
                    XElement page = upage;
                    if( page == null )
                        continue;
                    foreach( XElement uelement in page.Elements() )
                    {
                        XElement element = uelement;
                        if( element == null )
                            continue;

                        switch( element.Name.ToString() )
                        {
                            case "background":
                                {
                                    Rectangle2D rect = ParseRectangle( element );
                                    int id = ParseInt( element, "id" );
                                    AddBackground( rect.X, rect.Y, rect.Width, rect.Height, id );
                                }
                                break;
                            case "item":
                                {
                                    Point2D loc = ParseLocation( element );
                                    int id = ParseInt( element, "id" );
                                    int hue = ParseInt( element, "hue", 0 );
                                    AddItem( loc.X, loc.Y, id, hue );
                                }
                                break;
                            case "image":
                                {
                                    Point2D loc = ParseLocation( element );
                                    int id = ParseInt( element, "id" );
                                    int hue = ParseInt( element, "hue", 0 );
                                    AddImage( loc.X, loc.Y, id, hue + 2 );
                                }
                                break;
                            case "label":
                                {
                                    Point2D loc = ParseLocation( element );
                                    string txt = ParseText( element.Value );
                                    int hue = ParseInt( element, "hue", 0 );
                                    AddLabel( loc.X, loc.Y, hue, txt );
                                }
                                break;
                            case "html":
                                {
                                    Rectangle2D rect = ParseRectangle( element );
                                    bool back = ParseBool( element, "background" );
                                    bool scbar = ParseBool( element, "scrollbar" );
                                    bool hasborder = ( element.Attribute( "border" ) != null );
                                    if( hasborder && scbar )
                                        throw new Exception( string.Format( "{0}[{1}] bordered html cannot has scrollbar=\"true\".", m_Filename, element ) );
                                    // string border = null;
                                    string color = ParseText( element, "color", "FFFFFF" );
                                    int defsize = ParseInt( element, "fontsize", 3 );
                                    Variables[ ":color" ] = color;
                                    Variables[ ":fontsize" ] = defsize;
                                    string txt = ParseText( element.Value );
                                    bool replacelf = ParseBool( element, "replacelfwithbr", true );
                                    /*if (hasborder)
							    border = ParseText(element,"border");
						    
						    if( hasborder )
						    {
							    if (!txt.Contains("{:borderorcolor}"))
								    txt = PrepareHYMLColorSize(txt,"{:borderorcolor}",defsize);
							
							    if( string.IsNullOrEmpty(color))
								    color = "FFFFFF";
							    
							    AddHtml(rect.X,rect.Y,rect.Width,rect.Height,PrepareHTML( "<b>" + txt.Replace("{:borderorcolor}","" + border) + "</b>"), back,false);
						    }*/
                                    //AddHtml(rect.X + (hasborder ? 1 : 0),rect.Y+ (hasborder ? 1 : 0),rect.Width,rect.Height, PrepareHTML( txt ).Replace("{:borderorcolor}",color), (hasborder ? false : back),scbar);
                                    AddHtml( rect.X, rect.Y, rect.Width, rect.Height, HTMLBound( HTMLBoundColorSize( txt, color, defsize ), replacelf ), back, scbar );
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static string HTMLBoundColorSize( string html, string color, int size )
        {
            //impressive... UOL Gump system do not parse correctly html TAG. Cole tag will destroy ALL previously opened tags.
            // so.. do not use </basefont> here
            return string.Format( "<BASEFONT COLOR=#{0} SIZE={1}>{2}", color.Trim(), size, html.Trim() );
        }

        private static string HTMLBound( string html, bool replacelfwithbr )
        {
            if( replacelfwithbr )
                html = html.Replace( "\r", "" ).Replace( "\n", "<br>" );
            while( html.IndexOf( "  " ) > -1 )
                html = html.Replace( "  ", " " );
            return "<BODY><p>" + html.Trim() + "</p></BODY>";
        }

        /*
		protected string Color( string text, int color )
		{
			return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
		}
		protected string Size( string text, int size )
		{
			return String.Format( "<BASEFONT SIZE={0}>{1}</BASEFONT>", size, text );
		}*/

        public void Send( Mobile mobile )
        {
            if( mobile.HasGump( GetType() ) )
            {
                mobile.CloseGump( GetType() );
                int opsnd = ParseInt( Source.Element( "gump" ), "updatesound", -1 );
                if( opsnd > -1 )
                    mobile.PlaySound( opsnd );
            }
            else
            {
                int opsnd = ParseInt( Source.Element( "gump" ), "opensound", -1 );
                if( opsnd > -1 )
                    mobile.PlaySound( opsnd );
            }

            mobile.SendGump( this );
        }

        #region Parse
        protected Hashtable Variables = new Hashtable();

        private bool ParseBool( XElement node, string attributename, bool defaultvalue )
        {
            XAttribute attr = node.Attribute( attributename );
            if( attr == null )
                return defaultvalue;
            return bool.Parse( ParseText( attr.Value ) );
        }

        private bool ParseBool( XElement node, string attributename )
        {
            return ParseBool( node, attributename, false );
        }

        private Rectangle2D ParseRectangle( XElement node )
        {
            //try rect
            XAttribute rect = node.Attribute( "rect" );
            if( rect == null )
                rect = node.Attribute( "rectangle" );
            if( rect == null )
            {
                Point2D location, size;
                // bool locationok = false;
                // bool sizeok = false;

                XAttribute l = node.Attribute( "left" );
                XAttribute t = node.Attribute( "top" );
                if( l != null && t != null )
                {
                    location = new Point2D( ParseInt( l ), ParseInt( t ) );
                    // locationok = true;
                }
                else
                {
                    //try size location
                    XAttribute loc = node.Attribute( "location" );
                    if( loc == null )
                        throw new Exception( string.Format( "{0}[{1}] missing attribute location or left/top or rectancle.", m_Filename, node ) );
                    else
                        location = ParseLocation( node );
                }

                XAttribute w = node.Attribute( "width" );
                XAttribute h = node.Attribute( "height" );
                if( w != null && h != null )
                    size = new Point2D( ParseInt( w ), ParseInt( h ) );
                else
                {
                    //try size location
                    XAttribute loc = node.Attribute( "size" );
                    if( loc == null )
                        throw new Exception( string.Format( "{0}[{1}] missing attribute size or rectabgle.", m_Filename, node ) );
                    else
                    {
                        size = ParseSize( node );
                        // sizeok = true;
                    }
                }
                return new Rectangle2D( location, size );
            }
            else
            {
                string[] args = ParseText( rect.Value ).Split( 'x' );

                return new Rectangle2D( ParseInt( args[ 0 ] ), ParseInt( args[ 1 ] ), ParseInt( args[ 2 ] ), ParseInt( args[ 3 ] ) );
            }
        }

        private Point2D ParseLocation( XElement node )
        {
            XAttribute loc = node.Attribute( "location" );
            if( loc == null )
            {
                //try left right
                XAttribute l = node.Attribute( "left" );
                XAttribute t = node.Attribute( "top" );
                if( l == null || t == null )
                {
                    //try rect
                    XAttribute rect = node.Attribute( "rect" );
                    if( rect == null )
                        rect = node.Attribute( "rectangle" );
                    if( rect == null )
                        throw new Exception( string.Format( "{0}[{1}] missing attribute location or left/top or rectancle.", m_Filename, node ) );
                    else
                    {
                        Rectangle2D ret = ParseRectangle( node );
                        return new Point2D( ret.X, ret.Y );
                    }
                }
                else
                    return new Point2D( ParseInt( l ), ParseInt( t ) );
            }
            else
            {
                string[] args = ParseText( loc.Value ).Split( 'x' );

                return new Point2D( ParseInt( args[ 0 ] ), ParseInt( args[ 1 ] ) );
            }
        }

        private Point2D ParseSize( XElement node )
        {
            XAttribute loc = node.Attribute( "size" );
            if( loc == null )
            {
                //try left right
                XAttribute w = node.Attribute( "width" );
                XAttribute h = node.Attribute( "height" );
                if( w == null || h == null )
                {
                    //try rect
                    XAttribute rect = node.Attribute( "rect" );
                    if( rect == null )
                        rect = node.Attribute( "rectangle" );
                    if( rect == null )
                        throw new Exception( string.Format( "{0}[{1}] missing attribute size or rectancle.", m_Filename, node ) );
                    else
                    {
                        Rectangle2D ret = ParseRectangle( node );
                        return new Point2D( ret.Width, ret.Height );
                    }
                }
                else
                    return new Point2D( ParseInt( w ), ParseInt( h ) );
            }
            else
            {
                string[] args = ParseText( loc.Value ).Split( 'x' );

                return new Point2D( ParseInt( args[ 0 ] ), ParseInt( args[ 1 ] ) );
            }
        }

        private int ParseInt( XElement node, string attributename, int defaultvalue )
        {
            XAttribute attr = node.Attribute( attributename );
            if( attr == null )
                return defaultvalue;
            return ParseInt( attr );
        }

        private int ParseInt( XElement node, string attributename )
        {
            XAttribute attr = node.Attribute( attributename );
            if( attr == null )
                throw new Exception( string.Format( "{0}[{1}] missing attribute \"{2}\" of type int/hex.", m_Filename, node, attributename ) );
            return ParseInt( attr );
        }

        private int ParseInt( XAttribute attr )
        {
            return ParseInt( ParseText( attr ) );
        }

        private static int ParseInt( string value )
        {
            if( value.ToLower().IndexOf( "x" ) == 1 )
                return Convert.ToInt32( value, 16 );
            int ret;
            if( int.TryParse( value, out ret ) )
                return ret;
            throw new Exception( string.Format( "Unable to parse to int value=\"{0}\"", value ) );
        }

        private string ParseText( XElement node, string attributename, string defaultvalue )
        {
            XAttribute attr = node.Attribute( attributename );
            if( attr == null )
                return defaultvalue;
            return ParseText( attr );
        }

/*
        private string ParseText( XElement node, string attributename )
        {
            XAttribute attr = node.Attribute( attributename );
            if( attr == null )
                throw new Exception( string.Format( "{0}[{1}] missing attribute \"{2}\" of type string.", m_Filename, node, attributename ) );
            return ParseText( attr );
        }
*/

        private string ParseText( XAttribute attr )
        {
            return ParseText( attr.Value );
        }

        private string ParseText( string input )
        {
            if( Variables == null )
                return input;

            //parse all variables here
            bool replaced = true;
            while( replaced )
            {
                replaced = false;
                foreach( object vari in Variables.Keys )
                {
                    if( input.Contains( "{" + vari + "}" ) )
                    {
                        input = input.Replace( "{" + vari + "}", "" + Variables[ vari ] );
                        replaced = true;
                    }
                }
            }
            return input;
        }
        #endregion
    }
}