using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using Server;
using Server.Accounting;
using Region = Server.Region;

namespace Midgard.Engines.MapRenderizer
{
    internal class RenderQuery
    {
        public RenderQuery( string xmlfile )
        {
            ProgressMessage = null;
            IsCompleted = true;

            var xml = new XmlDocument();
            xml.Load( xmlfile );
            var vars = xml[ "configuration" ][ "variables" ];
            UniqueID = GetVariableFromXml( vars, "UniqueID" );
            var tbound = GetVariableFromXml( vars, "Bounds" ).Split( ',' );
            Bounds = new Rectangle3D( int.Parse( tbound[ 0 ] ), int.Parse( tbound[ 1 ] ), int.Parse( tbound[ 2 ] ), int.Parse( tbound[ 3 ] ), int.Parse( tbound[ 4 ] ),
                                     int.Parse( tbound[ 5 ] ) );
            Map = Map.Parse( GetVariableFromXml( vars, "Map" ) );
            var middle = new Point3D( Bounds.X + ( Bounds.Width / 2 ), Bounds.Y + ( Bounds.Height / 2 ), 0 );

            var region = Region.Find( middle, Map );
            RegionName = region != null ? region.Name : "unknown";

            UseEffects = GetVariableFromXml( vars, "UseEffects" ) == "true";
            UseLightSource = GetVariableFromXml( vars, "UseLightSource" ) == "true";
            GlobalLight = byte.Parse( GetVariableFromXml( vars, "GlobalLight" ) );
            Caller = GetVariableFromXml( vars, "Caller" );
            RequestTime = DateTime.FromBinary( long.Parse( GetVariableFromXml( vars, "RequestTime" ) ) );
            CompletedTime = DateTime.FromBinary( long.Parse( GetVariableFromXml( vars, "CompletedTime" ) ) );
            Error = GetVariableFromXml( vars, "Error" );
            Zoom = 1f;
            try
            {
                Zoom = float.Parse(GetVariableFromXml(vars, "Zoom"));
            }
            catch
            {
                
            }
        }

        public RenderQuery( IAccount caller, Rectangle3D bound, Map map, byte globallight, bool uselightsource, bool useeffects , float zoom)
        {
            IsCompleted = false;
            Bounds = bound;
            Map = map;
            Zoom = zoom;
            GlobalLight = globallight;
            UseEffects = useeffects;
            UseLightSource = uselightsource;
            Caller = caller.Username;
            RequestTime = DateTime.Now;
            ProgressMessage = "Waiting queue...";

            //findmiddle..
            var middle = new Point3D( bound.X + ( bound.Width / 2 ), bound.Y + ( bound.Height / 2 ), 0 );

            var region = Region.Find( middle, map );
            RegionName = region != null ? region.Name : "unknown";
            UniqueID = string.Format( "{0}_{1}_{2}", map.Name, RegionName, Math.Abs( RequestTime.ToBinary() ) );
            //verify filename...
            var validchars = "1234567890qwertyuioplkjhgfdsazxcvbnm_".ToCharArray();
            var newunique = "";
            foreach( var ch in UniqueID )
            {
                bool exists = false;
                foreach( var valid in validchars )
                {
                    if( ( "" + ch ).ToLower() == ( "" + valid ) )
                    {
                        exists = true;
                        break;
                    }
                }
                if( exists )
                    newunique += ch;
            }
            UniqueID = newunique;
        }

        public string RegionName { get; private set; }
        public Rectangle3D Bounds { get; private set; }
        public Map Map { get; private set; }
        public bool UseEffects { get; private set; }
        public byte GlobalLight { get; private set; }
        public bool UseLightSource { get; private set; }
        public string Caller { get; private set; }
        public DateTime RequestTime { get; private set; }
        public DateTime CompletedTime { get; private set; }
        public string UniqueID { get; private set; }
        public string ProgressMessage { get; set; }
        public float Zoom { get; private set; }

        public string ImagePath
        {
            get
            {
                var path = Path.Combine( MapRenderer.ImagesPath, ToString() + ".png" );
                return path;
            }
        }

        public string ImageThumbPath
        {
            get
            {
                var path = Path.Combine( MapRenderer.ImagesPath, ToString() + "_thumb.jpg" );
                return path;
            }
        }

        public string XmlPath
        {
            get
            {
                var path = Path.Combine( MapRenderer.ImagesPath, ToString() + ".xml" );
                return path;
            }
        }

        public string Error { get; set; }
        public bool IsCompleted { get; private set; }
        public bool Aborted { get; private set; }

        private static string GetVariableFromXml( XmlNode vars, string keyname )
        {
            foreach( XmlElement elem in vars.ChildNodes )
            {
                if( elem.Name == "variable" && elem.Attributes[ "name" ] != null && elem.Attributes[ "name" ].Value == keyname )
                    return elem.FirstChild.InnerText;
            }
            return null;
        }

        public override string ToString()
        {
            return UniqueID;
        }

        internal string ImageThumbRelativeUrl( string user, string pass )
        {
            return "getRenderedImage?user=" + user + "&pass=" + pass + "&query=" + ToString() + "&type=thumb";
        }

        internal string ImageRelativeUrl( string user, string pass )
        {
            return "getRenderedImage?user=" + user + "&pass=" + pass + "&query=" + ToString();
        }

        internal void Completed( Image img )
        {
            var path = ImagePath;
            Config.PathEnsureExistance( path );
            ProgressMessage = "Completing...";
            if( img != null )
            {
                img.Save( path, ImageFormat.Png );
                using( var thumb = ImageEffects.ZoomToFit( img, new Size( 128, 128 ), Color.Black ) )
                    thumb.Save( ImageThumbPath, ImageFormat.Jpeg );
                img.Dispose();

                var xmlsave = new XmlDocument();
                xmlsave.AppendChild( xmlsave.CreateXmlDeclaration( "1.0", "utf-8", "yes" ) );
                var config = xmlsave.CreateElement( "configuration" );
                xmlsave.AppendChild( config );
                var vars = xmlsave.CreateElement( "variables" );
                config.AppendChild( vars );

                CompletedTime = DateTime.Now;

                AddVariableToXml( vars, "UniqueID", UniqueID );
                AddVariableToXml( vars, "Bounds", string.Format( "{0},{1},{2},{3},{4},{5}", Bounds.X, Bounds.Y, Bounds.Z, Bounds.Width, Bounds.Height, Bounds.Depth ) );
                AddVariableToXml( vars, "Map", Map.Name );
                AddVariableToXml( vars, "UseEffects", UseEffects.ToString() );
                AddVariableToXml( vars, "GlobalLight", GlobalLight.ToString() );
                AddVariableToXml( vars, "UseLightSource", UseLightSource.ToString() );

                AddVariableToXml( vars, "Caller", Caller );
                AddVariableToXml( vars, "RequestTime", RequestTime.ToBinary().ToString() );
                AddVariableToXml( vars, "CompletedTime", CompletedTime.ToBinary().ToString() );
                AddVariableToXml( vars, "Error", Error );
                AddVariableToXml(vars, "Zoom", Zoom.ToString());

                xmlsave.Save( XmlPath );
            }
            ProgressMessage = "All done!";
            IsCompleted = true;

            //advise admin and caller 
            var account = (Account)Accounts.GetAccount( Caller );
            if( account != null )
            {
                //foraech (Mobile m in account.Mobiles if( m.Netstate != null ) allora e' online
            }
        }

        private static void AddVariableToXml( XmlNode vars, string key, string value )
        {
            var variable = vars.OwnerDocument.CreateElement( "variable" );
            vars.AppendChild( variable );
            var attr = vars.OwnerDocument.CreateAttribute( "name" );
            variable.Attributes.Append( attr );
            attr.Value = key;
            variable.AppendChild( vars.OwnerDocument.CreateCDataSection( value ) );
        }

        internal void AbortAndDelete( string aborter )
        {
            Aborted = true;
            if( File.Exists( ImageThumbPath ) )
                File.Delete( ImageThumbPath );
            if( File.Exists( ImagePath ) )
                File.Delete( ImagePath );
            if( File.Exists( XmlPath ) )
                File.Delete( XmlPath );
            Error = "Aborted by User: " + aborter;
            if(Config.Debug)
            {
                Config.Pkg.LogWarningLine("Delete aborted one. {0}", this);
            }
        }
    }
}