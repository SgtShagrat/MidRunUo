using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Server;
using Ultima;
using ItemData = Ultima.ItemData;
using Tile = Ultima.Tile;
using TileData = Ultima.TileData;
using TileFlag = Ultima.TileFlag;

namespace Midgard.Engines.MapRenderizer
{
    internal enum EntityTypes
    {
        Item,
        Mobile,
        Tile,
        Static,
    }

    internal enum PointPositions
    {
        Top,
        Left,
        Right,
        Bottom
    }

    internal class LightPoint
    {
        public int Lightadd;
        public Point Location2D;
        public Point3D Location3D;
        public PointPositions Position;
    }

    internal struct TileBitmapData
    {
        public LightPoint[] Lights;
        public Image Screen;
        public byte[] ScreenData;
        public Image Texture;
        public byte[] TextureData;
    }

    internal class GeneralEntity : IDisposable
    {
        private static readonly Dictionary<int, Image> m_CacheStatic = new Dictionary<int, Image>();
        private static readonly Dictionary<int, Image> m_CacheTexture = new Dictionary<int, Image>();
        private static readonly Dictionary<int, Image> m_CacheTile = new Dictionary<int, Image>();
        private static readonly Dictionary<int, Bitmap> m_Lights = new Dictionary<int, Bitmap>();

        public static float MaxLight = float.MinValue;
        public static float MinLight = float.MaxValue;

        public List<GeneralEntity> AllElements;
        private bool m_Disposed = false;
        private Point m_LightImageOffset = Point.Empty;
        private Image m_EffectedImg = null;
        private Size m_EffectedImgSize = new Size( -1, -1 );
        private Image m_Light;
        private Image m_NormalImage;
        private Size m_NormalImageSize = new Size( -1, -1 );
        private Image m_TileConnector;
        private Point m_TileConnectorOffset = Point.Empty;
        private List<IDisposable> toDispose = new List<IDisposable>();

        public GeneralEntity( object originalEntity, Point3D location )
        {
            Hidden = false;
            Element = originalEntity;
            if( originalEntity is ExtendedTile )
            {
                Type = EntityTypes.Tile;
                Element = ( (ExtendedTile)originalEntity ).Tile;
            }
            else if( originalEntity is ExtendedStatic )
            {
                Type = EntityTypes.Static;
                Element = ( (ExtendedStatic)originalEntity ).Static;
            }
            else if( originalEntity is Mobile )
                Type = EntityTypes.Mobile;
            else if( originalEntity is Item )
                Type = EntityTypes.Item;
            else
            {
                throw new ArgumentException( "Invalid argument passed!" );
            }
            Location = location;
        }

        public EntityTypes Type { get; private set; }
        public object Element { get; private set; }
        public Point3D Location { get; private set; }
        public Point3D StartRegionPoint { get; set; }
        public Point Offset { get; set; }
        public bool UseEffectedImage { get; set; }
        public bool Hidden { get; private set; }

        public static int CacheTiles
        {
            get { return m_CacheTile.Count; }
        }

        public static int CacheTextures
        {
            get { return m_CacheTexture.Count; }
        }

        public static int CacheArts
        {
            get { return m_CacheStatic.Count; }
        }

        public int AbsoluteID
        {
            get
            {
                switch( Type )
                {
                    case EntityTypes.Item:
                        return Item.ItemID;
                    case EntityTypes.Mobile:
                        return Mobile.Body.BodyID;
                    case EntityTypes.Static:
                        return Static.ID - 0x4000;
                    case EntityTypes.Tile:
                        return Tile.ID;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public Tile Tile
        {
            get { return (Tile)Element; }
        }

        public HuedTile Static
        {
            get { return (HuedTile)Element; }
        }

        public Item Item
        {
            get { return (Item)Element; }
        }

        public ItemData ItemData
        {
            get
            {
                var id = ( Type == EntityTypes.Static ? Static.ID : Item.ItemID );
                return TileData.ItemTable[ id & 0x3FFF ];
            }
        }

        public Mobile Mobile
        {
            get { return (Mobile)Element; }
        }

        public static int TileBlock
        {
            get { return MapRenderer.TileBlock; }
        }

        public static int TileHeight
        {
            get { return MapRenderer.TileHeight; }
        }

        public Point3D RelativeLocation
        {
            get
            {
                if( StartRegionPoint == Point3D.Zero )
                    throw new ArgumentException( "You must set StartRegionPoint before use this method." );
                return new Point3D( Location.Y - StartRegionPoint.Y, Location.X - StartRegionPoint.X, Location.Z );
            }
        }

        public Image EffectImage
        {
            get
            {
                if( m_EffectedImg != null )
                    return m_EffectedImg;

                var img = NormalImage;
                if( img == null )
                    return null;
                if( Type == EntityTypes.Static || Type == EntityTypes.Item )
                {
                    if( ( ItemData.Flags & TileFlag.Translucent ) == TileFlag.Translucent )
                    {
                        using( var lup = img = ImageEffects.CreateLightUp( img, 10 ) )
                        {
                            img = ImageEffects.SetAlphaPower( lup, 250 );
                        }
                        toDispose.Add( img ); //dispose after....Dispose()  
                    }
                    /*if ((ItemData.Flags & Ultima.TileFlag.Transparent) == Ultima.TileFlag.Transparent && (ItemData.Flags & Ultima.TileFlag.LightSource) != Ultima.TileFlag.LightSource)
                    {
                        //make item semi trasparent
                        img = SetAlphaPower(img, 125);
                        toDispose.Add(img); //dispose after....Dispose()                            
                    }*/
                }

                m_EffectedImgSize = img.Size;
                m_EffectedImg = img;
                return img;
            }
        }

        public Size ImageSize
        {
            get
            {
                var size = UseEffectedImage ? m_EffectedImgSize : m_NormalImageSize;
                if( size.Width == -1 )
                {
                    var img = Image;
                }
                return UseEffectedImage ? m_EffectedImgSize : m_NormalImageSize;
            }
        }

        public Image Image
        {
            get
            {
                if( UseEffectedImage )
                    return EffectImage;
                return NormalImage;
            }
        }

        public int HueID
        {
            get
            {
                if( Type == EntityTypes.Static )
                    return Static.Hue;
                if( Type == EntityTypes.Item )
                    return Item.Hue;
                throw new Exception( "Hue not supported for " + Type );
            }
        }

        public Image TileConnectorImage
        {
            get
            {
                if( m_TileConnector != null )
                    return m_TileConnector;
                var connector = true;
                if( Image == null )
                {
                    Hidden = true;
                    return null;
                }

                //m_TileConnectorApplied = true;
                //CreateStretchTileConnector_MapRenderMode();
                //project to ground...
                var myLoc = LocationToPoint();
                var bottom = FindTileAtLocation( Location.X + 1, Location.Y + 1 );
                var right = FindTileAtLocation( Location.X + 1, Location.Y );
                var left = FindTileAtLocation( Location.X, Location.Y + 1 );
                var myZ = myLoc.Y;
                var bottomoffset = 0;
                if( bottom == null )
                {
                    if( left != null )
                    {
                        bottom = left;
                        bottomoffset += ( TileBlock / 2 );
                    }
                    else if( right != null )
                    {
                        bottom = right;
                        bottomoffset += ( TileBlock / 2 );
                    }
                }
                var bottomZ = ( bottom != null ? bottom.LocationToPoint().Y : myZ + TileBlock ) + bottomoffset;
                var rightZ = ( right != null ? right.LocationToPoint().Y : myZ + ( TileBlock / 2 ) );
                var leftZ = ( left != null ? left.LocationToPoint().Y : myZ + ( TileBlock / 2 ) );
                var zoffs_min = 0;
                var zoffs_max = 0;
                int trng = 0;

                //if (bottom != null)
                {
                    trng = bottomZ - myZ;
                    zoffs_min = Math.Min( trng, zoffs_min );
                    zoffs_max = Math.Max( trng, zoffs_max );
                }
                //if (right != null)
                {
                    trng = rightZ - myZ;
                    zoffs_min = Math.Min( trng, zoffs_min );
                    zoffs_max = Math.Max( trng, zoffs_max );
                }
                //if (left != null)
                {
                    trng = leftZ - myZ;
                    zoffs_min = Math.Min( trng, zoffs_min );
                    zoffs_max = Math.Max( trng, zoffs_max );
                }
                if( zoffs_max == 0 )
                    zoffs_max = TileBlock;

                var harange = 0;
                if( zoffs_min < 0 )
                    harange -= zoffs_min;
                if( zoffs_max > 0 )
                    harange += zoffs_max;
                var finaloffsetY = 0 - zoffs_min;

                if( leftZ - myZ == TileBlock / 2 && bottomZ - myZ == TileBlock && rightZ - myZ == TileBlock / 2 && false )
                {
                }
                else
                {
                    //m_TileConnectorOffset = new Point(0, finaloffsetY);
                    var bmp = new Bitmap( TileBlock, harange, PixelFormat.Format32bppArgb );
                    m_TileConnectorOffset.Offset( 0, -finaloffsetY );
                    toDispose.Add( bmp );
                    //draw 4 trinagles to create tile object

                    var pts = new Point[]
                                  {
                                      /*0*/new Point((TileBlock/2), finaloffsetY),
                                           /*1*/new Point(0, Math.Max(0, (leftZ - myZ + finaloffsetY) - 1)),
                                           /*2*/new Point((TileBlock/2), harange - 1),
                                           /*3*/new Point(TileBlock - 1, Math.Max(0, (rightZ - myZ + finaloffsetY) - 1)),
                                  };

                    var lightpt = new List<LightPoint>();

                    lightpt.Add( new LightPoint { Position = PointPositions.Top, Lightadd = GetMapTileLight( Location ), Location2D = pts[ 0 ], Location3D = RelativeLocation } );
                    lightpt.Add( new LightPoint { Position = PointPositions.Bottom, Lightadd = GetMapTileLight( bottom != null ? bottom.Location : Location ), Location2D = pts[ 2 ], Location3D = bottom != null ? bottom.RelativeLocation : RelativeLocation } );
                    lightpt.Add( new LightPoint { Position = PointPositions.Right, Lightadd = GetMapTileLight( right != null ? right.Location : Location ), Location2D = pts[ 3 ], Location3D = right != null ? right.RelativeLocation : RelativeLocation } );
                    lightpt.Add( new LightPoint { Position = PointPositions.Left, Lightadd = GetMapTileLight( left != null ? left.Location : Location ), Location2D = pts[ 2 ], Location3D = left != null ? left.RelativeLocation : RelativeLocation } );

                    var debugPts = new Point[]
                                       {
                                           new Point(8, 10),
                                           //new Point(5,9),
                                       };
                    foreach( var pt in debugPts )
                        if( RelativeLocation.X == pt.X && RelativeLocation.Y == pt.Y )
                        {
                            //int a = 1;
                            //UseConnector = true;
                            //Hidden = false;
                            break;
                        }
                        else
                        {
                            //UseConnector = false;
                            //Hidden = true;
                        }

                    if( true ) //use advancetexturization
                    {
                        Bitmap texture = null;
                        if( Tile.ID != 0 )
                            texture = (Bitmap)GetTexture( Tile.ID );
                        if( texture == null )
                        {
                            Hidden = false;
                            return null;
                            /*texture = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
                            toDispose.Add(texture);
                            using (var g = Graphics.FromImage(texture))
                            {
                                g.DrawImage(Image, new Rectangle(Point.Empty, texture.Size), new Rectangle(Point.Empty, Image.Size),
                                            GraphicsUnit.Pixel);
                            }
                            texture.RotateFlip(RotateFlipType.Rotate270FlipNone);*/
                        }

                        BitmapData txtdata = null;
                        BitmapData screendata = null;

                        try
                        {
                            txtdata = texture.LockBits( new Rectangle( Point.Empty, texture.Size ), ImageLockMode.ReadOnly,
                                                       PixelFormat.Format32bppArgb );

                            var txtBdata = new byte[ texture.Width * texture.Height * 4 ];
                            var bmpBdata = new byte[ bmp.Width * bmp.Height * 4 ];
                            Marshal.Copy( txtdata.Scan0, txtBdata, 0, txtBdata.Length );
                            texture.UnlockBits( txtdata );
                            txtdata = null;

                            var data = new TileBitmapData
                                           {
                                               Screen = bmp,
                                               Texture = texture,
                                               ScreenData = bmpBdata,
                                               TextureData = txtBdata,
                                               Lights = lightpt.ToArray(),
                                           };


                            DrawPolygonTexture( data, pts );

                            screendata = bmp.LockBits( new Rectangle( Point.Empty, bmp.Size ), ImageLockMode.WriteOnly,
                                                      PixelFormat.Format32bppArgb );

                            Marshal.Copy( data.ScreenData, 0, screendata.Scan0, data.ScreenData.Length );
                        }
                        finally
                        {
                            if( txtdata != null )
                                texture.UnlockBits( txtdata );
                            if( screendata != null )
                                bmp.UnlockBits( screendata );
                        }
                    }
                    Hidden = true;
                    if( !connector )
                        bmp = null;
                    m_TileConnector = bmp;
                }

                return m_TileConnector;
            }
        }

        public Image NormalImage
        {
            get
            {
                if( m_NormalImage != null )
                    return m_NormalImage;

                Image img = null;
                if( Type == EntityTypes.Tile )
                    img = GetImage( Type, Tile.ID );
                else if( Type == EntityTypes.Static )
                    img = GetImage( Type, Static.ID );
                else if( Type == EntityTypes.Item )
                    img = GetImage( Type, Item.ItemID );
                else if( Type == EntityTypes.Mobile )
                    img = null;
                else
                    throw new NotImplementedException();

                if( img != null )
                {
                    if( Type == EntityTypes.Static || Type == EntityTypes.Item )
                    {
                        if( HueID != 0 )
                        {
                            var old = img;
                            img = (Bitmap)old.Clone();
                            /*img = new Bitmap(old.Width, old.Height, PixelFormat.Format32bppArgb);
                            using (Graphics gg = Graphics.FromImage(img))
                                gg.DrawImage(old, Point.Empty);*/
                            toDispose.Add( img );
                            //img.Save("C:\\temp_before.bmp");
                            Hues.GetHue( HueID-1 ).ApplyTo( (Bitmap)img,
                                                       ( ( ItemData.Flags & TileFlag.PartialHue ) == TileFlag.PartialHue ) );
                            //img.Save("C:\\temp_after.bmp");
                        }
                    }
                }
                else
                {
                    return null;
                }
                m_NormalImageSize = img.Size;
                m_NormalImage = img;
                return img;
            }
        }

        public bool IsLightSource
        {
            get
            {
                if( Type != EntityTypes.Item && Type != EntityTypes.Static )
                    return false;
                if( ( ItemData.Flags & TileFlag.LightSource ) != TileFlag.LightSource )
                    return false;
                if( Type == EntityTypes.Item )
                    return true;
                //only static stay here
                if( ( ( ItemData.Flags & TileFlag.Window ) == TileFlag.Window ) )
                    return false;
                return true;
            }
        }

        public Image LightImage
        {
            get
            {
                if( m_Light != null )
                    return m_Light;

                if( IsLightSource )
                {
                    var lightid = ItemData.Quality;
                    if( Type == EntityTypes.Item )
                        lightid = (int)Item.Light;

                    var lightmbp = GetLight( lightid );

                    var uncolored = lightmbp;
                    if( HueID != 0 )
                    {
                        lightmbp = (Bitmap)uncolored.Clone();
                        toDispose.Add( lightmbp );
                        Hues.GetHue( HueID ).ApplyTo( lightmbp, true );
                    }

                    var newbmp = new Bitmap( lightmbp.Width, lightmbp.Height, PixelFormat.Format32bppArgb );
                    toDispose.Add( newbmp );

                    using( var g = Graphics.FromImage( newbmp ) )
                    {
                        //transform img...
                        var black = Color.FromArgb( 255, 0, 0, 0 );
                        var pureblack = Color.FromArgb( 0, 0, 0, 0 );
                        //var newimg = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                        for( var x = 0; x < newbmp.Width; x++ )
                            for( var y = 0; y < newbmp.Height; y++ )
                            {
                                var nothued = uncolored.GetPixel( x, y );
                                var hued = lightmbp.GetPixel( x, y );
                                if( nothued == black || nothued.G != nothued.B || nothued.B != nothued.R || nothued.R != nothued.G )
                                {
                                    //do not write
                                    newbmp.SetPixel( x, y, pureblack );
                                }
                                else
                                {
                                    //is gray....
                                    var power = nothued.R * 2;
                                    newbmp.SetPixel( x, y, Color.FromArgb( power, hued.R * 5, hued.G * 5, hued.B * 5 ) ); //<-- transform into alpha colored Pixel
                                }
                            }
                        toDispose.Add( newbmp );
                        m_LightImageOffset.Offset( 0, ( ( newbmp.Height - NormalImage.Height ) / 2 ) );
                        m_Light = newbmp;
                    }

                    return m_Light;
                }

                throw new Exception( "This is not a light source." );
            }
        }

        public Point LightImageToPoint
        {
            get
            {
                var reloc = RelativeLocation;

                var x = -( reloc.X * ( TileBlock / 2 ) ) + ( reloc.Y * ( TileBlock / 2 ) );
                var y = ( reloc.X * ( TileBlock / 2 ) ) + ( reloc.Y * ( TileBlock / 2 ) ) - ( reloc.Z * TileHeight );

                var pt = new Point( x, y );

                pt.Offset( m_LightImageOffset );
                pt.Offset( -( LightImage.Width / 2 ), -LightImage.Height );
                pt.Offset( Offset );

                return pt;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if( m_Disposed )
                return;
            m_Disposed = true;

            foreach( var disp in toDispose )
                disp.Dispose();
            toDispose.Clear();
        }

        #endregion

        private static Image GetImage( EntityTypes type, int id )
        {
            Image img = null;
            if( type == EntityTypes.Tile )
            {
                if( id <= 0x2 )
                    return null;
                if( m_CacheTile.ContainsKey( id ) )
                    return m_CacheTile[ id ];
                img = Art.GetLand( id );
                if( img != null )
                    m_CacheTile.Add( id, img );
            }
            else if( type == EntityTypes.Static )
            {
                if( id - 0x4000 <= 0x1 )
                    return null;

                if( id - 0x4000 >= 0x2198 && id - 0x4000 <= 0x21A4 )
                    return null;

                if( m_CacheStatic.ContainsKey( id ) )
                    return m_CacheStatic[ id ];
                img = Art.GetStatic( id - 0x4000 );
                if( img != null )
                    m_CacheStatic.Add( id, img );
            }
            else if( type == EntityTypes.Item )
            {
                if( id <= 0x2 )
                    return null;
                if( m_CacheStatic.ContainsKey( id ) )
                    return m_CacheStatic[ id ];
                img = Art.GetStatic( id );
                if( img != null )
                    m_CacheStatic.Add( id, img );
            }
            else if( type == EntityTypes.Mobile )
                img = null;
            else
                throw new NotImplementedException();
            return img;
        }

        public static void DisposeCache()
        {
            /*foreach(var elem in m_CacheStatic.Values)
                elem.Dispose();
            foreach (var elem in m_CacheTexture.Values)
                elem.Dispose();
            foreach (var elem in m_CacheTile.Values)
                elem.Dispose();*/
            m_CacheStatic.Clear();
            m_CacheTexture.Clear();
            m_CacheTile.Clear();
        }

        private static Image GetTexture( int id )
        {
            Image img = null;
            if( id <= 0x2 )
                return null;
            if( m_CacheTexture.ContainsKey( id ) )
                return m_CacheTexture[ id ];
            img = Textures.GetTexture( id );
            if( img != null )
                m_CacheTexture.Add( id, img );
            return img;
        }

        public override bool Equals( object obj )
        {
            var mm = obj as GeneralEntity;
            if( mm == null )
                throw new Exception( "Must be GeneralEntity" );
            return ToString().Equals( obj.ToString() );
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            var tempz = Location.Z;
            if( Type == EntityTypes.Static || Type == EntityTypes.Item )
            {
                tempz += ItemData.CalcHeight;
                if( ( ItemData.Flags & TileFlag.Foliage ) == TileFlag.Foliage )
                    tempz += 128; //drasticallyincrease z to emulate foliage upperelement.
            }
            var ret = ( Location.X + Location.Y ).ToString().PadLeft( 6, '0' ) + ( ( tempz ) + 128 ).ToString().PadLeft( 6, '0' );
            /*if (Type == EntityTypes.Tile)
                ret += "00";
            else
            {
                ret += "1";
                if (Type == EntityTypes.Static)
                    ret += ItemData.Background ? "0" :"1"; //background stay back ?
                else
                    ret += "1";
            }*/
            ret += Convert.ToString( AbsoluteID, 16 ).PadLeft( 4, '0' );

            return ret;
        }

        public static int CompareTo( GeneralEntity source, GeneralEntity dest )
        {
            return ( source.ToString().CompareTo( dest.ToString() ) );
        }

        private int GetMapTileLight( Point3D tileLocation )
        {
            var mTop = tileLocation.Z;

            var bottom = FindTileAtLocation( tileLocation.X + 1, tileLocation.Y + 1 );
            var right = FindTileAtLocation( tileLocation.X + 1, tileLocation.Y );
            var left = FindTileAtLocation( tileLocation.X, tileLocation.Y + 1 );

            var mRight = ( right != null ? right.Location : tileLocation ).Z;
            var mBottom = ( bottom != null ? bottom.Location : tileLocation ).Z;
            var mLeft = ( left != null ? left.Location : tileLocation ).Z;


            var myalpha = +( mBottom - mTop ) * 0.3f + ( mRight - mTop ) * 0.3f - ( mLeft - mTop ) * 0.3f;
            //20000.0f / (float)(y2 * y2 + y3 * y3 + 1) - 60.0f;
            myalpha *= 6f;
            MinLight = Math.Min( myalpha, MinLight );
            MaxLight = Math.Max( myalpha, MaxLight );
            if( myalpha > 70.0f ) myalpha = 70.0f;
            if( myalpha < -70.0f ) myalpha = -70.0f;
            return (int)myalpha;
        }

        private static void DrawPolygonTexture( TileBitmapData data, Point[] pts )
        {
            var min = new int[ 2 ];
            var max = new int[ 2 ];
            var p1 = new int[] { pts[ 0 ].X, pts[ 0 ].Y };
            var p2 = new int[] { pts[ 1 ].X, pts[ 1 ].Y };
            var p3 = new int[] { pts[ 2 ].X, pts[ 2 ].Y };
            var p4 = new int[] { pts[ 3 ].X, pts[ 3 ].Y };
            var q1 = new int[ 2 ];
            var q2 = new int[ 2 ];
            var q3 = new int[ 2 ];
            var q4 = new int[ 2 ];

            int i;
            for( i = 0; i < 2; i++ )
            {
                min[ i ] = 1000000;
                max[ i ] = -1000000;
                if( p1[ i ] < min[ i ] )
                    min[ i ] = p1[ i ];
                if( p1[ i ] > max[ i ] )
                    max[ i ] = p1[ i ];
                if( p2[ i ] < min[ i ] )
                    min[ i ] = p2[ i ];
                if( p2[ i ] > max[ i ] )
                    max[ i ] = p2[ i ];
                if( p3[ i ] < min[ i ] )
                    min[ i ] = p3[ i ];
                if( p3[ i ] > max[ i ] )
                    max[ i ] = p3[ i ];
                if( p4[ i ] < min[ i ] )
                    min[ i ] = p4[ i ];
                if( p4[ i ] > max[ i ] )
                    max[ i ] = p4[ i ];
            }
            // int w = max[ 0 ] - min[ 0 ] + 1;
            // int h = max[ 1 ] - min[ 1 ] + 1;
            for( i = 0; i < 2; i++ )
            {
                q1[ i ] = p1[ i ] - min[ i ] + 0;
                q2[ i ] = p2[ i ] - min[ i ] + 0;
                q3[ i ] = p3[ i ] - min[ i ] + 0;
                q4[ i ] = p4[ i ] - min[ i ] + 0;
            }

            DrawTriangleTexture( data, true, new Point( q1[ 0 ], q1[ 1 ] ), new Point( q2[ 0 ], q2[ 1 ] ), new Point( q3[ 0 ], q3[ 1 ] ) );
            DrawTriangleTexture( data, false, new Point( q3[ 0 ], q3[ 1 ] ), new Point( q4[ 0 ], q4[ 1 ] ), new Point( q1[ 0 ], q1[ 1 ] ) );
        }

        private static void DrawTriangleTexture( TileBitmapData data, bool left, Point p1, Point p2, Point p3 )
        {
            Point upper, middle, lower;
            if( ( p1.Y < p2.Y ) && ( p1.Y < p3.Y ) )
            {
                upper = p1;
                if( p2.Y < p3.Y )
                {
                    middle = p2;
                    lower = p3;
                }
                else
                {
                    middle = p3;
                    lower = p2;
                }
            }
            else if( ( p2.Y < p1.Y ) && ( p2.Y < p3.Y ) )
            {
                upper = p2;
                if( p1.Y < p3.Y )
                {
                    middle = p1;
                    lower = p3;
                }
                else
                {
                    middle = p3;
                    lower = p1;
                }
            }
            else
            {
                upper = p3;
                if( p1.Y < p2.Y )
                {
                    middle = p1;
                    lower = p2;
                }
                else
                {
                    middle = p2;
                    lower = p1;
                }
            }


            if( ( upper.Y != middle.Y ) && ( lower.Y != upper.Y ) )
            {
                float m1 = ( upper.X - middle.X ) / (float)( upper.Y - middle.Y );
                float m2 = ( upper.X - lower.X ) / (float)( upper.Y - lower.Y );
                float t = ( upper.Y - middle.Y ) / (float)( upper.Y - lower.Y );
                DrawHalfTriangle( left, false, data, upper, m2, m1, middle.Y - upper.Y, t );
            }
            if( ( lower.Y != middle.Y ) && ( lower.Y != upper.Y ) )
            {
                float m1 = ( lower.X - middle.X ) / (float)( lower.Y - middle.Y );
                float m2 = ( lower.X - upper.X ) / (float)( lower.Y - upper.Y );
                float t = ( middle.Y - lower.Y ) / (float)( upper.Y - lower.Y );
                DrawHalfTriangle( left, true, data, lower, -m2, -m1, lower.Y - middle.Y + 1, t );
            }
        }

        private static void DrawHalfTriangle( bool left, bool lowerHalf, TileBitmapData data, Point up, float m1, float m2, int lines, float t )
        {
            int i, j;
            var p = new PointF( up.X, up.Y );
            var q = new PointF( up.X, up.Y );
            for( i = 0; i < lines; i++ )
            {
                int len = (int)( q.X - p.X ); //OK
                if( len > 0 )
                    j = (int)( p.X ); //ok
                else
                    j = (int)( q.X ); //ok


                //texture coords
                float texY, texX, mX, mY;
                texY = texX = mX = mY = 0f;

                /*
                tex_y = (float)i / (float)lines;
                m_y = (1.0f - t) / (float)(lines);


                var tex2_x = tex_y * (lowerHalf ? 1 :  t );
                m_x = 0.0f;
                
                if (len != 0)
                    m_x = (tex2_x / ((float)len * t));
                
                tex_x = 0f;*/

                if( !lowerHalf )
                {
                    if( left )
                    {
                        texX = 0.0f;
                        texY = i / (float)lines;
                        if( len != 0 )
                        {
                            var mtx = i / (float)lines;
                            mX = mtx / Math.Abs( len );
                            mX = mX / 2f;
                            var mty = texY / Math.Abs( len );
                            mY = -mty / 2f;
                        }
                    }
                    else
                    {
                        texX = ( i / (float)lines );
                        if( len != 0 )
                        {
                            var mty = i / (float)lines;
                            mY = mty / Math.Abs( len );
                            mY = -mY / 2f;
                            texY = mty / 2f;
                            var mtx = texX / Math.Abs( len );
                            mX = mtx / 2f;
                        }
                        texX = texX / 2f;
                    }
                }
                else
                {
                    if( left )
                    {
                        //tex_x = 1.0f;
                        texY = 1.0f;
                        var mty = i / (float)lines;
                        texX = ( i / (float)lines );
                        if( len != 0 )
                        {
                            mY = mty / Math.Abs( len );
                            mY = -mY / 2f;
                            texY += mY;
                            var mtx = texX / Math.Abs( len );
                            mX = mtx / 2f;
                        }
                        texX = 1.0f - texX;
                    }
                    else
                    {
                        var mty = i / (float)lines;
                        var mtx = i / (float)lines;
                        if( len != 0 )
                        {
                            mX = mtx / ( Math.Abs( len ) );
                            mX = mX / 2f;
                            mY = mty / ( Math.Abs( len ) );
                            mY = -mY / 2f;
                        }
                        texX = 1.0f - ( mtx / 2f );
                        texY = 1.0f - ( mty / 2f );
                    }
                }

                var screenPoint = new Point( j, up.Y + ( lowerHalf ? -i : i ) );

                PutTexturedPixel( data, screenPoint.X, screenPoint.Y, Math.Abs( len ), mX, mY, texX, texY );

                p.X += m1; //ok 
                q.X += m2; //ok
            }
        }

        private static void PutTexturedPixel( TileBitmapData data, int x, int y, int len, float mX, float mY, float texX, float texY )
        {
            byte bpp = 4;

            UInt32 op = (UInt32)( data.Texture.Height - 1 );
            UInt32 q = 0;

            var pixel = (UInt32)0xff000000;


            var bits = (UInt32)( ( ( y * data.Screen.Width ) + x ) );
            var tx = x;
            while( len >= 0 )
            {
                q = (UInt32)( ( ( (int)( texY * data.Texture.Height ) ) & op ) * data.Texture.Width );
                q = (UInt32)( q + ( (int)( data.Texture.Width * texX ) & op ) );
                //Config.Pkg.LogInfoLine("PutTexturedPixel({0},{1}) - Tex({2},{3}) - TIncrement({2},{3})", tx, y, texX, texY,mX,mY);
                var texture = BitConverter.ToUInt32( data.TextureData, (Int32)q * bpp );
                texture |= pixel;
                //texture = 0xFFFFFFFF;

                texture = SetLightForTexturedPixel( data, new Point( tx, y ), texture );

                Array.Copy( BitConverter.GetBytes( texture ), 0, data.ScreenData, bits * bpp, bpp );
                texX += mX;
                texY += mY;
                bits++;
                tx++;
                len--;
            }
        }

        private static uint SetLightForTexturedPixel( TileBitmapData data, Point point, uint color )
        {
            var col = Color.FromArgb( (int)color );

            //find nearest vertex...
            var distance = new List<float>();
            var maxdistance = 0f;

            foreach( var pt in data.Lights )
            {
                //
                var range = Math.Sqrt( Math.Pow( pt.Location2D.X - point.X, 2 ) + Math.Pow( pt.Location2D.Y - point.Y, 2 ) );
                distance.Add( (float)range );
                maxdistance = Math.Max( maxdistance, (float)range );
            }
            var lighten = 0f;
            foreach( var pt in data.Lights )
            {
                var range = Math.Sqrt( Math.Pow( pt.Location2D.X - point.X, 2 ) + Math.Pow( pt.Location2D.Y - point.Y, 2 ) );
                lighten += ( (float)range / maxdistance ) * pt.Lightadd;
            }

            lighten = lighten / 4f;
            if( (int)lighten == 0 )
                return color;
            col = Color.FromArgb( col.A,
                                 Math.Max( 0, Math.Min( 255, col.R + (int)lighten ) ),
                                 Math.Max( 0, Math.Min( 255, col.G + (int)lighten ) ),
                                 Math.Max( 0, Math.Min( 255, col.B + (int)lighten ) ) );
            return (uint)col.ToArgb();
        }

        private GeneralEntity FindTileAtLocation( int x, int y )
        {
            foreach( var elem in AllElements )
            {
                elem.Offset = Offset;
                if( elem.Type == EntityTypes.Tile && elem.Location.X == x && elem.Location.Y == y )
                    return elem;
            }
            return null;
        }

        /*
                private static int CalcStretchHeight( int y2, int y3, int y4, out int minY )
                {
                    var maxY = 0;
                    minY = 0;

                    if( y2 < minY ) minY = y2;
                    if( y2 > maxY ) maxY = y2;
                    if( y3 < minY ) minY = y3;
                    if( y3 > maxY ) maxY = y3;
                    if( y4 < minY ) minY = y4;
                    if( y4 > maxY ) maxY = y4;

                    return maxY - minY + 1;
                }
        */

        /*
                private static Point CalcMapLocationAtPos( int relativeX, int relativeY )
                {
                    return CalcMapLocationAtPos( relativeX, relativeY, 0 );
                }
        */

        /*
                private static Point CalcMapLocationAtPos( IPoint3D relative )
                {
                    return CalcMapLocationAtPos( relative.X, relative.Y, relative.Z );
                }
        */

        /*
                private static Point CalcMapLocationAtPos( int relativeX, int relativeY, int relativeZ )
                {
                    var reloc = new Point( relativeX, relativeY );

                    var x = -( reloc.X * ( TileBlock / 2 ) ) + ( reloc.Y * ( TileBlock / 2 ) );
                    var y = ( reloc.X * ( TileBlock / 2 ) ) + ( reloc.Y * ( TileBlock / 2 ) ) - ( relativeZ * TileHeight );

                    var pt = new Point( x, y );

                    pt.Offset( -( TileBlock / 2 ), -TileBlock );

                    return pt;
                }
        */

        /*
                private static int TBPnt( int partx )
                {
                    return (int)( ( TileBlock / 16f ) * partx );
                }
        */

        public Point TileConnectorToPoint()
        {
            var pt = LocationToPoint();
            pt.Offset( m_TileConnectorOffset );
            return pt;
        }

        /// <summary>
        /// Get  / load light (Note: please do not dispose this bitmap.. it will be m_Disposed automatically)
        /// </summary>
        /// <param name="lightid"></param>
        /// <returns></returns>
        private static Bitmap GetLight( int lightid )
        {
            DisposeLights();
            if( lightid == 255 )
                lightid = 2;

            if( m_Lights.ContainsKey( lightid ) )
                return m_Lights[ lightid ];

            var path = Path.Combine( Config.DataPath, "lights_bmp" );
            path = Path.Combine( path, "0x" + Convert.ToString( lightid, 16 ) + ".bmp" );

            m_Lights.Add( lightid, new Bitmap( path ) );
            return m_Lights[ lightid ];
        }

        public static void DisposeLights()
        {
            foreach( var l in m_Lights )
                l.Value.Dispose();
            m_Lights.Clear();
        }

        public Point LocationToPoint()
        {
            var reloc = RelativeLocation;

            var x = -( reloc.X * ( TileBlock / 2 ) ) + ( reloc.Y * ( TileBlock / 2 ) );
            var y = ( reloc.X * ( TileBlock / 2 ) ) + ( reloc.Y * ( TileBlock / 2 ) ) - ( reloc.Z * TileHeight );

            var pt = new Point( x, y );

            if( Type == EntityTypes.Tile || Type == EntityTypes.Static || Type == EntityTypes.Item )
            {
                pt.Offset( -( ImageSize.Width / 2 ), -ImageSize.Height );
            }
            else if( Type == EntityTypes.Mobile )
            {
                // TO DO
            }
            else
                throw new NotImplementedException();

            pt.Offset( Offset );

            return pt;
        }

        internal void DisposeUnusedImage()
        {
            foreach( var img in toDispose.ToArray() )
            {
                if( img != m_EffectedImg && img != m_NormalImage && img != m_TileConnector && img != m_Light )
                {
                    img.Dispose();
                    toDispose.Remove( img );
                }
            }
        }
    }
}