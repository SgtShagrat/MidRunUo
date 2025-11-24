using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Midgard.Engines.MyMidgard;
using Server;
using Ultima;
using Map = Server.Map;
using Tile = Ultima.Tile;

namespace Midgard.Engines.MapRenderizer
{
    internal class ExtendedTile
    {
        public Point3D Location;
        public Tile Tile;
    }

    internal class ExtendedStatic
    {
        public Point3D Location;
        public HuedTile Static;
    }

    internal class MapRenderer : IDisposable
    {
        public const int TileBlock = 44;
        public const int TileHeight = 4;

        private List<RenderQuery> deletedoraborted = new List<RenderQuery>();
        private Thread executorHandle;

        private RenderQuery m_LastExecution;
        private string m_OptionsRegions;

        private List<RenderQuery> queries = new List<RenderQuery>();

        public MapRenderer()
        {
            WebCommandSystem.Register( "queryMapRender", new WebCommandEventHandler( QueryMapRender ) );
            WebCommandSystem.Register( "infoMapRender", new WebCommandEventHandler( InfoMapRender ) );
            WebCommandSystem.Register( "getRenderedImage", new WebCommandEventHandler( GetRenderedImage ) );
            WebCommandSystem.Register( "deleteRenderedMap", new WebCommandEventHandler( DeleteRenderedMap ) );

            executorHandle = new Thread( new ThreadStart( Executor ) );
            executorHandle.Name = "MapRendererizer Thread";
            executorHandle.Priority = ThreadPriority.BelowNormal;
            executorHandle.Start();
        }

        public static string ImagesPath
        {
            get { return Path.Combine( Config.SavePath, "RenderedMaps" ); }
        }

        private string OptionsRegions
        {
            get
            {
                if( m_OptionsRegions != null )
                    return m_OptionsRegions;

                var regdata = new SortedDictionary<string, string>();
                foreach( var map in Map.AllMaps )
                    foreach( var reg in map.Regions )
                    {
                        int xmin = reg.Value.Area[ 0 ].X;
                        int ymin = reg.Value.Area[ 0 ].Y;
                        int xmax = xmin;
                        int ymax = ymin;
                        foreach( var a in reg.Value.Area )
                        {
                            xmin = Math.Min( xmin, a.Start.X );
                            ymin = Math.Min( ymin, a.Start.Y );
                            xmax = Math.Max( xmax, a.End.X );
                            ymax = Math.Max( ymax, a.End.Y );
                        }
                        regdata.Add( map.Name + " &raquo; " + reg.Value.Name + " (" + ( xmax - xmin ) + "," + ( ymax - ymin ) + ")",
                                    map.Name + ";(" + xmin + "," + ymin + ",-127);(" + ( xmax - 1 ) + "," + ( ymax - 1 ) + ",127)" );
                    }

                var regions = new StringBuilder();
                foreach( var eleme in regdata )
                {
                    regions.Append( "<option value=\"" + eleme.Value + "\">" );
                    regions.AppendLine( eleme.Key + "</option>" );
                }
                m_OptionsRegions = regions.ToString();
                return m_OptionsRegions;
            }
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if( Disposed )
                return;
            Disposed = true;
            if( executorHandle != null && executorHandle.IsAlive )
                executorHandle.Abort();
        }

        private void Executor()
        {
            m_LastExecution = null;

            while( !Disposed )
            {
                try
                {
                    lock( queries )
                    {
                        if( queries.Count > 0 )
                        {
                            m_LastExecution = queries[ 0 ];
                            queries.RemoveAt( 0 );
                        }
                    }
                    if( m_LastExecution != null )
                    {
                        if( Config.Debug )
                            Config.Pkg.LogInfoLine( "Start executing render query: {0}. [Queue:{1}]", m_LastExecution, queries.Count );
                        try
                        {
                            using( var img = DoRender( m_LastExecution ) )
                            {
                                m_LastExecution.Completed( img );
                            }
                        }
                        catch( Exception ex )
                        {
                            try
                            {
                                Config.Pkg.LogErrorLine("Error while rendering query: {0}.", m_LastExecution);
                                Config.Pkg.LogErrorLine(ex.ToString());
                            }
                            catch
                            {
                            }
                            m_LastExecution.Error = ex.ToString();
                            m_LastExecution.Completed( null );
                            deletedoraborted.Add( m_LastExecution );
                        }
                        m_LastExecution = null;
                    }
                    Thread.Sleep( 100 );
                }
                catch( Exception ex )
                {
                    Config.Pkg.LogErrorLine( ex.ToString() );
                    m_LastExecution = null;
                }
            }
        }

        private void DeleteRenderedMap( WebCommandsListener listener, OnCommandEventArgs e )
        {
            TestAuthorization( e );

            var queryid = e.Args.ContainsKey( "query" ) ? e.Args[ "query" ] : "";
            if( queryid == null )
                throw new ArgumentException( "missing query=" );

            var query = FindExistingQuery( queryid );
            if( query == null )
                throw new ArgumentException( "Query with id:" + queryid + " not exists." );
            var isdeletedoraborted = query.Aborted;
            if( !isdeletedoraborted )
                lock( deletedoraborted )
                    isdeletedoraborted = deletedoraborted.Contains( query );

            if( isdeletedoraborted )
            {
                lock( deletedoraborted )
                    deletedoraborted.Remove( query );
            }
            else
            {
                query.AbortAndDelete( e.Args[ "user" ] );
                deletedoraborted.Add( query );
                lock( queries )
                {
                    if( queries.Contains( query ) )
                        queries.Remove( query );
                }
            }

            InfoMapRender( listener, e );
        }

        private void GetRenderedImage( WebCommandsListener listener, OnCommandEventArgs e )
        {
            TestAuthorization( e );
            var imageid = e.Args.ContainsKey( "query" ) ? e.Args[ "query" ] : "";
            var zoom = (e.Args.ContainsKey( "zoom" ) ? int.Parse( e.Args[ "zoom" ] ) : 100) / 100f;
            var responsemime = ( e.Args.ContainsKey( "format" ) && e.Args[ "format" ] == "png" ) ? ImageFormat.Png : ImageFormat.Jpeg;
            if( imageid == null )
                throw new ArgumentException( "missing query=" );
            var thumb = e.Args.ContainsKey( "type" ) ? e.Args[ "type" ] == "thumb" : false;

            var query = FindExistingQuery( imageid );
            if( query == null )
                throw new ArgumentException( "Query with id:" + imageid + " not exists." );

            Image img = null;
            try
            {
                if( thumb && File.Exists( query.ImageThumbPath ) )
                    img = Image.FromFile( query.ImageThumbPath );
                else if( File.Exists( query.ImagePath ) )
                {
                    img = Image.FromFile( query.ImagePath );
                    if( zoom < 1.0f )
                    {
                        var zoomedSize = new Size( (int)( img.Width * zoom ), (int)( img.Height * zoom ) );
                        using( var old = img )
                        {
                            img = new Bitmap( zoomedSize.Width, zoomedSize.Height, PixelFormat.Format32bppArgb );
                            using( var g = Graphics.FromImage( img ) )
                            {
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.DrawImage( old, new Rectangle( Point.Empty, zoomedSize ), new Rectangle( Point.Empty, old.Size ), GraphicsUnit.Pixel );
                            }
                        }
                    }
                }

                var fname = string.Format("{0}_{1}", query.Map, query.RegionName);

                if( img != null )
                {
                    if( responsemime == ImageFormat.Png )
                    {
                        e.ResponseMimeType = "image/png";  
                        img.Save( e.UnderlyngStream, ImageFormat.Png );
                        fname += ".png";
                    }
                    else
                    {
                        e.ResponseMimeType = "image/jpg";
                        img.Save( e.UnderlyngStream, ImageFormat.Jpeg );
                        fname += ".jpg";
                    }
                }
                e.ResponseHeaders.Add("Content-Disposition", "inline; filename=\"" + fname + "\"");
                //e.ResponseHeaders.Add("Content-Length", "" + e.DataBuffer.Length);
            }
            finally
            {
                if( img != null )
                    img.Dispose();
            }
        }

        private RenderQuery FindExistingQuery( string queryID )
        {
            if( m_LastExecution != null && m_LastExecution.UniqueID == queryID )
                return m_LastExecution;
            lock( queries )
                foreach( var qry in queries )
                    if( qry.UniqueID == queryID )
                        return qry;

            lock( deletedoraborted )
                foreach( var qry in deletedoraborted )
                    if( qry.UniqueID == queryID )
                        return qry;

            foreach( var file in Directory.GetFiles( ImagesPath, "*.xml" ) )
            {
                if( Path.GetFileNameWithoutExtension( file ).Equals( queryID, StringComparison.InvariantCultureIgnoreCase ) )
                    return new RenderQuery( file );
            }
            return null;
        }

        private void InfoMapRender( WebCommandsListener listener, OnCommandEventArgs e )
        {
            TestAuthorization( e );
            var html = LoadHtmlFromTemplate( "InfoMapRender.html" );
            //lastExecution = null;
            e.ResponseMimeType = "text/html";
            e.StreamEncoding = Encoding.UTF8;
            var writer = new StreamWriter( e.UnderlyngStream, e.StreamEncoding );

            var stored = new List<RenderQuery>();
            lock( queries )
            {
                Config.PathEnsureExistance( Path.Combine( ImagesPath, "fake" ) );
                foreach( var file in Directory.GetFiles( ImagesPath, "*.xml" ) )
                {
                    var myid = Path.GetFileNameWithoutExtension( file );
                    var exists = m_LastExecution != null && m_LastExecution.UniqueID == myid;
                    if( !exists )
                        foreach( var qry in queries )
                            if( qry.UniqueID == myid )
                            {
                                exists = true;
                                break;
                            }
                    if( !exists )
                    {
                        stored.Add( new RenderQuery( file ) );
                    }
                }
            }

            ReplaceHtmlDatas( ref html, stored.Count, e, null );
            var totalxml = "";
            if( m_LastExecution != null && !m_LastExecution.Aborted )
            {
                totalxml = LoadHtmlFromTemplate( "queryelement_executing.html" );
                ReplaceHtmlDatas( ref totalxml, stored.Count, e, m_LastExecution );
            }
            FastReplace( ref html, "{$ActiveRenderizationDetails}", totalxml );

            totalxml = "";
            lock( deletedoraborted )
            {
                if( deletedoraborted.Count > 0 )
                {
                    foreach( var elem in deletedoraborted )
                    {
                        var cyclehtml = LoadHtmlFromTemplate( "queryelement_errors.html" );
                        ReplaceHtmlDatas( ref cyclehtml, stored.Count, e, elem );
                        totalxml += cyclehtml;
                    }
                }
            }
            FastReplace( ref html, "{$ErrorsRenderizationsDetails}", totalxml );

            totalxml = "";
            if( stored.Count > 0 )
            {
                foreach( var elem in stored )
                    if( !elem.Aborted )
                    {
                        var cyclehtml = LoadHtmlFromTemplate( "queryelement_stored.html" );
                        ReplaceHtmlDatas( ref cyclehtml, stored.Count, e, elem );
                        totalxml += cyclehtml;
                    }
            }
            FastReplace( ref html, "{$StoredRenderizationsDetails}", totalxml );
            totalxml = "";
            lock( queries )
                if( queries.Count > 0 )
                {
                    foreach( var elem in queries )
                        if( !elem.Aborted )
                        {
                            var cyclehtml = LoadHtmlFromTemplate( "queryelement_queued.html" );
                            ReplaceHtmlDatas( ref cyclehtml, stored.Count, e, elem );
                            totalxml += cyclehtml;
                        }
                }
            FastReplace( ref html, "{$QueuedRenderizationsDetails}", totalxml );

            writer.Write( html );
            writer.Flush();
            return;
        }

        private void ReplaceHtmlDatas( ref string html, int stored, OnCommandEventArgs e, RenderQuery thisElement )
        {
            FastReplace( ref html, "{$Pkg.Name}", Config.Pkg.Title );
            FastReplace( ref html, "{$Pkg.Version}", Config.Pkg.Version.ToString() );
            FastReplace( ref html, "{$Pkg.AuthorName}", Config.Pkg.AuthorName );
            FastReplace( ref html, "{$ActiveRenderizationsCount}", m_LastExecution != null ? "1" : "0" );
            FastReplace( ref html, "{$QueuedRenderizationsCount}", "" + queries.Count );
            FastReplace( ref html, "{$ErrorsRenderizationsCount}", "" + deletedoraborted.Count );
            FastReplace( ref html, "{$StoredRenderizationsCount}", "" + stored );
            FastReplace( ref html, "{$QueryStringCredential}", "user=" + e.Args[ "user" ] + "&pass=" + e.Args[ "pass" ] );

            FastReplace( ref html, "{$OptionsRegions}", "" + OptionsRegions );
            if( thisElement != null )
            {
                FastReplace( ref html, "{$Query.MapName}", thisElement.Map.Name );
                FastReplace( ref html, "{$Query.RegionName}", thisElement.RegionName );
                FastReplace( ref html, "{$Query.Caller}", thisElement.Caller );
                FastReplace( ref html, "{$Query.RequestedTime}", thisElement.RequestTime.ToString() );
                FastReplace( ref html, "{$Query.CompletedTime}", thisElement.CompletedTime.ToString() );
                FastReplace( ref html, "{$Query.EllapsedTime}", thisElement.CompletedTime.Subtract( thisElement.RequestTime ).ToString() );
                FastReplace( ref html, "{$Query.Bounds}", "Start(" + thisElement.Bounds.Start.X + "," + thisElement.Bounds.Start.Y + "," + thisElement.Bounds.Start.Z + "), End(" + ( thisElement.Bounds.End.X - 1 ) + "," + ( thisElement.Bounds.End.Y - 1 ) + "," + ( thisElement.Bounds.End.Z ) + "), Size(" + thisElement.Bounds.Width + "," + thisElement.Bounds.Height + "," + thisElement.Bounds.Depth + ")" );
                FastReplace( ref html, "{$Query.BoundsForQueryString}", "start=(" + thisElement.Bounds.Start.X + "," + thisElement.Bounds.Start.Y + "," + thisElement.Bounds.Start.Z + ")&end=(" + ( thisElement.Bounds.End.X - 1 ) + "," + ( thisElement.Bounds.End.Y - 1 ) + "," + ( thisElement.Bounds.End.Z ) + ")" );
                FastReplace( ref html, "{$Query.UseEffects}", thisElement.UseEffects.ToString() );
                FastReplace( ref html, "{$Query.GlobalLight}", thisElement.GlobalLight.ToString() );
                FastReplace( ref html, "{$Query.RenderLightSources}", thisElement.UseLightSource.ToString() );
                FastReplace( ref html, "{$Query.ID}", thisElement.UniqueID );
                FastReplace( ref html, "{$Query.ProgressMessage}", thisElement.ProgressMessage );
                FastReplace( ref html, "{$Query.Error}", thisElement.Error );
                FastReplace(ref html, "{$Query.Zoom}", string.Format("{0:P}", thisElement.Zoom));
                FastReplace( ref html, "{$Query.Bounds.Start}", "(" + thisElement.Bounds.Start.X + "," + thisElement.Bounds.Start.Y + "," + thisElement.Bounds.Start.Z + ")" );
                FastReplace( ref html, "{$Query.Bounds.End}", "(" + ( thisElement.Bounds.End.X - 1 ) + "," + ( thisElement.Bounds.End.Y - 1 ) + "," + ( thisElement.Bounds.End.Z ) + ")" );
            }
        }

        private static void FastReplace( ref string text, string toreplace, string replacewith )
        {
            if( text.IndexOf( toreplace ) == -1 )
                return;
            text = text.Replace( toreplace, replacewith );
        }

        private static string LoadHtmlFromTemplate( string relativefile )
        {
            return File.ReadAllText( Path.Combine( Config.DataPath, Path.Combine( "html", relativefile ) ) );
        }

        private static void TestAuthorization( OnCommandEventArgs e )
        {
            /*
            if (e.Account.AccessLevel < AccessLevel.Administrator)
            {
                Config.Pkg.LogWarningLine("Account {0} attempt to execute command {1} but it's not authorized.", e.Account, e.Cmd);
                throw new Exception("Account not authorized. (Only >= Admin");
            }
            */
        }

        private void QueryMapRender( WebCommandsListener listener, OnCommandEventArgs e )
        {
            TestAuthorization( e );


            // felucca 0,0,-125 -> 7096,4096,125
            Map map = null;
            var bounds = new Rectangle3D( 0, 0, 0, 0, 0, 0 );
            try
            {
                map = Map.Parse( e.Args[ "map" ] );
                var start = Point3D.Parse( e.Args[ "start" ] );
                var end = Point3D.Parse( e.Args[ "end" ] );
                bounds = new Rectangle3D( start, end );
            }
            catch
            {
                throw new ArgumentException( "We need: map=<mapname>&start=(x,y,z)&end=(x,y,z)" );
            }
            if( map == null )
                throw new ArgumentException( "No valid map specified (" + e.Args[ "map" ] + ")" );

            if( Config.Debug )
            {
                Config.Pkg.LogInfoLine( e.Cmd );
                Config.Pkg.LogInfoIndentLine( "map:{0}", 2, map );
                Config.Pkg.LogInfoIndentLine( "start:{0}", 2, bounds.Start );
                Config.Pkg.LogInfoIndentLine( "end:{0}", 2, bounds.End );
            }

            var newEnd = bounds.End;
            newEnd.Offset( 1, 1, 0 ); //increase size to catch item on End line.
            bounds = new Rectangle3D( bounds.Start, newEnd );

            var useeffects = e.Args.ContainsKey( "effects" ) && e.Args[ "effects" ].ToLower() == "true";
            var globallight = e.Args.ContainsKey( "globallight" ) ? byte.Parse( e.Args[ "globallight" ] ) : (byte)255;
            var realuselightsource = ( e.Args.ContainsKey( "lightsource" ) && e.Args[ "lightsource" ].ToLower() == "true" );
            var uselightsource = realuselightsource && ( globallight < 255 );
            var zoom = (e.Args.ContainsKey("zoom") ? int.Parse(("" + e.Args["zoom"]).Replace("%","").Split(',')[0]) : 100) / 100f;

            var newQuery = new RenderQuery( e.Account, bounds, map, globallight, uselightsource, useeffects , zoom);

            queries.Add( newQuery );

            InfoMapRender( listener, e );
        }

        private Image DoRender( RenderQuery query )
        {
            query.ProgressMessage = "Starting...";

            var bounds = query.Bounds;
            var map = query.Map;
            var useeffects = query.UseEffects;
            var globallight = query.GlobalLight;
            var uselightsource = query.UseLightSource;

            bool nocache = true;

            if( Config.Debug )
            {
                Config.Pkg.LogInfoIndentLine( "Effects on Images: {0}", 2, useeffects );
                Config.Pkg.LogInfoIndentLine( "Global Light Value (0-255): {0}", 2, globallight );
                if( globallight == 255 && !uselightsource )
                    Config.Pkg.LogWarningIndentLine( "Light Source will be disabled with Global Light Value = 255", 2 );
                Config.Pkg.LogInfoIndentLine( "Use item Light Source: {0}", 2, uselightsource );
                //Config.Pkg.LogInfoIndentLine("Create bitmap of type: {0}", 2, responsemime);
                Config.Pkg.LogInfoIndentLine( "Cache: Tiles: {0}, Arts: {1}, Textures: {2}, Total: {3}", 2, GeneralEntity.CacheTiles,
                                             GeneralEntity.CacheArts, GeneralEntity.CacheTextures,
                                             GeneralEntity.CacheTiles + GeneralEntity.CacheArts + GeneralEntity.CacheTextures );
                if( nocache )
                {
                    Config.Pkg.LogInfoIndentLine( "Cache disposed.", 2 );
                    GeneralEntity.DisposeCache();
                    Config.Pkg.LogInfoIndentLine( "Cache: Tiles: {0}, Arts: {1}, Textures: {2}, Total: {3}", 2, GeneralEntity.CacheTiles,
                                                 GeneralEntity.CacheArts, GeneralEntity.CacheTextures,
                                                 GeneralEntity.CacheTiles + GeneralEntity.CacheArts + GeneralEntity.CacheTextures );
                }
            }

            DateTime assembleStart = DateTime.Now;

            //create image :-)
            var elements = GetClientElements( map, bounds );
            try
            {
                //search for (Y) top element
                var lowerY = 0;
                var lowerX = 0;
                var graterY = 0;
                var graterX = 0;
                // var size = new Point3D( bounds.Width, bounds.Height, bounds.Depth );
                m_LastExecution.ProgressMessage = "Organizing elements...";

                foreach( var elem in elements )
                {
                    if( m_LastExecution.Aborted )
                        return null;
                    elem.StartRegionPoint = bounds.Start;
                    elem.UseEffectedImage = useeffects;
                    elem.AllElements = elements;
                    //if (elem.Type == EnbtityTypes.Tile)
                    {
                        var translateed = elem.LocationToPoint();

                        lowerY = Math.Min( lowerY, translateed.Y );
                        graterY = Math.Max( graterY, translateed.Y );
                        lowerX = Math.Min( lowerX, translateed.X );
                        graterX = Math.Max( graterX, translateed.X );
                    }
                }
                var offset = new Point( Math.Max( 0, 0 - lowerX ), Math.Max( 0, 0 - lowerY ) );

                var bitmapSize = new Size( graterX + offset.X + TileBlock, graterY + offset.Y + TileBlock );
                if(query.Zoom<1f && query.Zoom>0.05f)
                {
                    bitmapSize = new Size((int) ((float) bitmapSize.Width*query.Zoom), (int) ((float) bitmapSize.Height*query.Zoom));
                }
                

                if( Config.Debug )
                    Config.Pkg.LogProgressStart("Assemble Image Map [{0}, bytes: {1}]", bitmapSize, ConvertSizeToString( bitmapSize.Width * bitmapSize.Height*4));

                int count = 0;

                elements.Sort( new Comparison<GeneralEntity>( GeneralEntity.CompareTo ) );


                m_LastExecution.ProgressMessage = string.Format("Assemble Image Map [{0}, bytes: {1}]", bitmapSize, ConvertSizeToString(bitmapSize.Width * bitmapSize.Height * 4));

                Bitmap mainBmp = null;
                try
                {
                    mainBmp = new Bitmap(bitmapSize.Width, bitmapSize.Height, PixelFormat.Format32bppArgb);
                }
                catch
                {
                    throw new OutOfMemoryException("Cannot allocate bitmap of size " + bitmapSize + ".");
                }
                {
                    // var visiblearea = new GraphicsPath();
                    using( var g = Graphics.FromImage( mainBmp ) )
                    {
                        var lights = new List<GeneralEntity>();
                        var lasttime = DateTime.MinValue;

                        #region Draw Elements

                        foreach( var elem in elements )
                        {
                            if( m_LastExecution.Aborted )
                                return null;

                            elem.Offset = offset;
                            count++;
                            m_LastExecution.ProgressMessage = string.Format( "{0:P}", count / (float)elements.Count );
                            if( Config.Debug && DateTime.Now.Subtract( lasttime ).TotalMilliseconds > 200 )
                            {
                                Config.Pkg.LogProgressContinue( count, elements.Count );
                                lasttime = DateTime.Now;
                            }

                            var bmp = elem.Image;
                            if( bmp != null )
                            {
                                Image tileconn = null;
                                if( elem.Type == EntityTypes.Tile )
                                    tileconn = elem.TileConnectorImage;

                                if (!elem.Hidden)
                                    DrawZoommedImage(g, elem.Image, elem.LocationToPoint(),query.Zoom);

                                if( tileconn != null )
                                {
                                    DrawZoommedImage(g, tileconn, elem.TileConnectorToPoint(), query.Zoom);
                                }
                            }
                            if( elem.IsLightSource )
                                lights.Add( elem );
                            else
                                elem.Dispose();
                           
                            //g.DrawString("" + count, SystemFonts.DefaultFont, Brushes.White, newcoord);
                        }
                        if( Config.Debug )
                            Config.Pkg.LogProgressContinue( count, elements.Count );

                        #endregion

                        #region Lights

                        if( uselightsource )
                        {
                            m_LastExecution.ProgressMessage = "Assemble Lights...";

                            if( Config.Debug )
                            {
                                Config.Pkg.LogProgressContinue( count, elements.Count );
                                Config.Pkg.LogInfoLine( "" );
                                Config.Pkg.LogProgressStart( "Write Lights" );
                            }

                            using( var obscureimage = ImageEffects.GetFilledImageWithColor( mainBmp, Color.FromArgb( 255 - globallight, 0, 0, 0 ) ) )
                            //using (var obg = Graphics.FromImage(obscureimage))
                            {
                                count = 0;
                                foreach( var elem in lights )
                                {
                                    if( m_LastExecution.Aborted )
                                        return null;

                                    count++;
                                    m_LastExecution.ProgressMessage = string.Format( "Assemble Lights...{0:P}", count / (float)lights.Count );

                                    if( Config.Debug )
                                        Config.Pkg.LogProgressContinue( count, lights.Count );

                                    var bmp = (Bitmap)elem.LightImage;
                                    var dest = elem.LightImageToPoint;
                                    for( int x = 0; x < bmp.Width; x++ )
                                        for( int y = 0; y < bmp.Height; y++ )
                                        {
                                            var lightcol = bmp.GetPixel( x, y );
                                            if( dest.X + x < obscureimage.Width && dest.Y + y < obscureimage.Height )
                                            {
                                                var obsccol = obscureimage.GetPixel( dest.X + x, dest.Y + y );
                                                if( lightcol.R > 1 )
                                                {
                                                    int a = Math.Max( 0, obsccol.A - ( lightcol.A * 3 ) );
                                                    var newcol = Color.FromArgb( a, obsccol );
                                                    obscureimage.SetPixel( dest.X + x, dest.Y + y, newcol );
                                                }
                                            }
                                        }
                                    if( elem.HueID != 0 )
                                    {
                                        var pureblack = Color.FromArgb( 0, 0, 0, 0 );
                                        for( int x = 0; x < bmp.Width; x++ )
                                            for( int y = 0; y < bmp.Height; y++ )
                                            {
                                                if( dest.X + x >= 0 && dest.X + x < mainBmp.Width && dest.Y + y < mainBmp.Height && dest.Y + y >= 0 )
                                                {
                                                    var backcol = mainBmp.GetPixel( dest.X + x, dest.Y + y );
                                                    if( backcol.A == 0 )
                                                        bmp.SetPixel( x, y, pureblack );
                                                    // LightImage will be modfied here!!!!!                                              
                                                }
                                            }
                                        g.DrawImage( bmp, dest );
                                    }
                                    //g.DrawString("" + count, SystemFonts.DefaultFont, Brushes.White, newcoord);
                                }
                                g.DrawImage( obscureimage, Point.Empty );
                            }
                        }

                        #endregion

                        if( Config.Debug )
                        {
                            Config.Pkg.LogProgressContinue( count, elements.Count );
                            Config.Pkg.LogInfoLine( "" );
                        }
                        
                    }

                    if( Config.Debug )
                    {
                        //Config.Pkg.LogInfoLine("PixelShadowLight rendered: Min ({0}) ; Max({1})", GeneralEntity.MinLight, GeneralEntity.MaxLight);
                        Config.Pkg.LogInfoLine( "Map Assembled in {0}", DateTime.Now.Subtract( assembleStart ) );
                        Config.Pkg.LogInfoIndentLine( "Cache: Tiles: {0}, Arts: {1}, Textures: {2}, Total: {3}", 2, GeneralEntity.CacheTiles,
                                                     GeneralEntity.CacheArts, GeneralEntity.CacheTextures,
                                                     GeneralEntity.CacheTiles + GeneralEntity.CacheArts + GeneralEntity.CacheTextures );
                        foreach( var elem in elements )
                        {
                            if( elem.Location.X == 1454 && elem.Location.Y == 1669 && false )

                                if( elem.Type == EntityTypes.Item || elem.Type == EntityTypes.Static )
                                    Config.Pkg.LogInfoLine( "{0},0x{1:x}:{2},{3},{4},CH:{5},Surf:{6},Bkg:{7}", elem,
                                                           ( elem.Type == EntityTypes.Item ? elem.Item.ItemID : elem.Static.ID - 0x4000 ), elem.Type,
                                                           elem.RelativeLocation, elem.ItemData.Name, elem.ItemData.CalcHeight,
                                                           elem.ItemData.Surface ? "Y" : "N", elem.ItemData.Background ? "Y" : "N" );
                                else
                                    Config.Pkg.LogInfoLine( "{0},0x{1:x}:{2},{3}", elem, elem.Tile.ID, elem.Type, elem.RelativeLocation );
                        }
                    }


                    return mainBmp;
                }
            }
            finally
            {
                foreach( var elem in elements )
                    elem.Dispose();
            }
        }

        private void DrawZoommedImage(Graphics g, Image image, Point point, float Zoom)
        {
            if (Zoom < 1f && Zoom > 0.05f)
            {
                using (var newimg = ImageEffects.Zoom(image, Zoom,true))
                {
                    point = new Point((int)Math.Ceiling((float)point.X * Zoom), (int)Math.Ceiling((float)point.Y * Zoom));
                    g.DrawImage(newimg, point);
                }
            }
            else
                g.DrawImage(image, point);
        }

        private List<GeneralEntity> GetClientElements( Map map, Rectangle3D bounds )
        {
            m_LastExecution.ProgressMessage = "Collecting Items and Mobiles...";
            var ret = GetAllElements( map, bounds ); //load items and mobiles (from server)

            //transform Server.Map to Ultima.Map
            var clientMap =
                (Ultima.Map)
                ( typeof( Ultima.Map ).GetField( map.Name, BindingFlags.Static | BindingFlags.Public ).GetValue( null ) );

            if( Config.Debug )
                Config.Pkg.LogInfo( "[Ultima.dll] Loading Land Tiles..." );
            m_LastExecution.ProgressMessage = "Collecting Tiles...";

            var elems = GetLandTiles( clientMap, bounds );
            if( m_LastExecution != null && m_LastExecution.Aborted )
                return ret;
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "[{0}]", elems.Count );
            foreach( var elem in elems )
                ret.Add( new GeneralEntity( elem, elem.Location ) );

            if( Config.Debug )
                Config.Pkg.LogInfo( "[Ultima.dll] Loading Statics Tiles..." );

            if (m_LastExecution != null)
                m_LastExecution.ProgressMessage = "Collecting Statics...";

            var selems = GetStatics( clientMap, bounds );
            if( m_LastExecution != null && m_LastExecution.Aborted )
                return ret;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "[{0}]", selems.Count );

            foreach( var elem in selems )
                ret.Add( new GeneralEntity( elem, elem.Location ) );

            return ret;
        }

        private List<GeneralEntity> GetAllElements( Map map, Rectangle3D bounds )
        {
            var allgroup = new List<GeneralEntity>();
            if( Config.Debug )
                Config.Pkg.LogInfo( "Loading Items..." );
            var ielems = GetItems( map, bounds );
            if( m_LastExecution != null && m_LastExecution.Aborted )
                return allgroup;
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "[{0}]", ielems.Count );
            foreach( var elem in ielems )
                allgroup.Add( new GeneralEntity( elem, elem.Location ) );

            if( Config.Debug )
                Config.Pkg.LogInfo( "Loading Mobiles..." );
            var melems = GetMobiles( map, bounds );
            if( m_LastExecution != null && m_LastExecution.Aborted )
                return allgroup;
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "[{0}]", melems.Count );
            foreach( var elem in melems )
                allgroup.Add( new GeneralEntity( elem, elem.Location ) );
            return allgroup;
        }

        /// <summary>
        /// Returns Items in bound (cheched on X < end.X)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="bound"></param>
        /// <returns>Returns Items in bound </returns>
        private List<Item> GetItems( Map map, Rectangle3D bound )
        {
            if( map == null || map == Map.Internal )
                return new List<Item>();
            var list = new List<Item>();
            var eable = map.GetItemsInBounds( bound.ToRectangle2D() );

            foreach( Item item in eable )
            {
                if( m_LastExecution != null && m_LastExecution.Aborted )
                    break;
                if( item.Z < bound.Start.Z && item.Z > bound.End.Z )
                    continue;
                if( !item.Visible )
                    continue;
                if( item.Deleted )
                    continue;

                list.Add( item );
            }

            eable.Free();

            return list;
        }

        /// <summary>
        /// Returns Mobiles in bound (cheched on X < end.X)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
        private List<Mobile> GetMobiles( Map map, Rectangle3D bound )
        {
            if( map == null || map == Map.Internal )
                return new List<Mobile>();

            var list = new List<Mobile>();
            var eable = map.GetMobilesInBounds( bound.ToRectangle2D() );

            foreach( Mobile mobile in eable )
            {
                if( m_LastExecution != null && m_LastExecution.Aborted )
                    break;

                if( mobile.Z < bound.Start.Z && mobile.Z > bound.End.Z )
                    continue;
                if( mobile.Hidden )
                    continue;
                if( mobile.Account != null ) //do not catch players...
                    continue;
                if( mobile.Deleted )
                    continue;
                list.Add( mobile );
            }

            eable.Free();

            return list;
        }

        /// <summary>
        /// Returns Static Items in bound (cheched on X < end.X)
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private List<ExtendedStatic> GetStatics( Ultima.Map map, Rectangle3D bound )
        {
            var staticTiles = new List<ExtendedStatic>();

            for( int x = bound.Start.X; x < bound.End.X; x++ )
                for( int y = bound.Start.Y; y < bound.End.Y; y++ )
                    foreach( var tile in map.Tiles.GetStaticTiles( x, y ) )
                    {
                        if( m_LastExecution != null && m_LastExecution.Aborted )
                            return staticTiles;
                        if( tile.Z >= bound.Start.Z && tile.Z < bound.End.Z )
                        {
                            staticTiles.Add( new ExtendedStatic { Static = tile, Location = new Point3D( x, y, tile.Z ) } );
                        }
                    }
            return staticTiles;
        }

        /// <summary>
        /// Returns Land Tiles in bound (cheched on X < end.X)
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private List<ExtendedTile> GetLandTiles( Ultima.Map map, Rectangle3D bound )
        {
            var landTiles = new List<ExtendedTile>();

            for( int x = bound.Start.X; x < bound.End.X; x++ )
            {
                for( int y = bound.Start.Y; y < bound.End.Y; y++ )
                {
                    var tile = map.Tiles.GetLandTile( x, y );
                    if( m_LastExecution != null && m_LastExecution.Aborted )
                        return landTiles;
                    if( tile.Z >= bound.Start.Z && tile.Z <= bound.End.Z )
                        landTiles.Add( new ExtendedTile { Tile = tile, Location = new Point3D( x, y, tile.Z ) } );
                }
            }

            return landTiles;
        }
        public static string ConvertSizeToString(Int64 bytes)
        {
            return ConvertSizeToString(Convert.ToUInt64(bytes));
        }
        public static string ConvertSizeToString(int bytes)
        {
            return ConvertSizeToString(Convert.ToUInt64(bytes));
        }
        public static string ConvertSizeToString(UInt64 bytes)
        {
            try
            {
                Decimal dec = Convert.ToDecimal(bytes);
                if (dec < 1024)
                {
                    return String.Format("{0:f2} bytes", dec);
                }

                Decimal kbytes = dec / (Decimal)1024;
                if (kbytes < 1024)
                {
                    return String.Format("{0:f2} Kb", kbytes);
                }

                Decimal mbytes = dec / ((Decimal)1024 * (Decimal)1024);
                if (mbytes < 1024)
                {
                    return String.Format("{0:f2} Mb", mbytes);
                }

                Decimal gbyte = dec / ((Decimal)1024 * (Decimal)1024 * (Decimal)1024);
                if (gbyte < 1024)
                {
                    return String.Format("{0:f2} Gb", gbyte);
                }

            }
            catch
            {
                throw;
            }
            return "???";

        }
    }
}