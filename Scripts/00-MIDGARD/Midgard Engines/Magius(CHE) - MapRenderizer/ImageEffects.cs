/* BEWARE: this file is EXCLUSIVE property of Magius(CHE). In concession to Midgard Shard.
 *  other uses are UNPERMITTED and UNAUTHORIZED.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Midgard.Engines.MapRenderizer
{
    internal class ImageEffects
    {
        public static Bitmap ZoomToFit( Image imgImage, Size fitSize, Color color )
        {
            int w = imgImage.Width;
            int h = imgImage.Height;
            float per = 0;
            // bool isLarger = w > fitSize.Width;
            // bool isHeigher = h > fitSize.Height;
            if( imgImage.Width < fitSize.Width && imgImage.Height < fitSize.Height )
            {
                if( fitSize.Width > 0 )
                {
                    per = imgImage.Width / (float)fitSize.Width;
                    w = fitSize.Width;
                    h = Math.Max( (int)Math.Ceiling( imgImage.Height / per ), 1 );
                }
                if( fitSize.Height > 0 )
                {
                    if( h > fitSize.Height || fitSize.Width == 0 ) //h is better
                    {
                        per = imgImage.Height / (float)fitSize.Height;
                        w = Math.Max( (int)Math.Ceiling( imgImage.Width / per ), 1 );
                        h = fitSize.Height;
                    }
                }
            }
            else if( imgImage.Width > fitSize.Width || fitSize.Height == 0 )
            {
                per = imgImage.Width / (float)fitSize.Width;
                w = fitSize.Width;
                h = Math.Max( (int)Math.Ceiling( imgImage.Height / per ), 1 );
                if( h > fitSize.Height && fitSize.Height != 0 )
                {
                    per = h / (float)fitSize.Height;
                    w = Math.Max( (int)Math.Ceiling( w / per ), 1 );
                    h = fitSize.Height;
                }
            }
            else if( imgImage.Height > fitSize.Height || fitSize.Width == 0 )
            {
                per = imgImage.Height / (float)fitSize.Height;
                w = Math.Max( (int)Math.Ceiling( imgImage.Width / per ), 1 );
                h = fitSize.Height;
            }
            return new Bitmap( imgImage, w, h );
        }

        public static Bitmap SetAlphaPower( Image origin, byte alpha )
        {
            var alphapw = Convert.ToSingle( alpha ) / Convert.ToSingle( 0xFF );
            var ptsArray = new float[][]
                               {
                                   new float[] {1, 0, 0, 0, 0}, new float[] {0, 1, 0, 0, 0}, new float[] {0, 0, 1, 0, 0},
                                   new float[] {0, 0, 0, alphapw, 0}, new float[] {0, 0, 0, 0, 1},
                               };
            // Create a ColorMatrix object using pts array
            var clrMatrix = new ColorMatrix( ptsArray );
            // Create an ImageAttributes object
            var imgAttributes = new ImageAttributes();
            // Set color matrix of imageAttributes
            imgAttributes.SetColorMatrix( clrMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );

            var newo = new Bitmap( origin.Width, origin.Height, PixelFormat.Format32bppArgb );

            using( var g = Graphics.FromImage( newo ) )
            {
                // Draw the changable painting image over white and text
                g.DrawImage( origin, new Rectangle( 0, 0, origin.Width, origin.Height ), 0, 0, origin.Width, origin.Height, GraphicsUnit.Pixel,
                            imgAttributes );
            }
            return newo;
        }

        public static Bitmap CreateLightUp( Image image, byte power )
        {
            return CreateColorizedImage( image, Color.FromArgb( 0, power, power, power ), true );
        }
        public static Bitmap Zoom(Image imgImage, float zoom, bool antialias)
        {
            if (!antialias)
            {
                var w = Convert.ToInt32(imgImage.Width * zoom);
                var h = Convert.ToInt32(imgImage.Height * zoom);
                var bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb); //imgImage.PixelFormat);
                var g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(imgImage, new Rectangle(0, 0, w, h), new Rectangle(0, 0, imgImage.Width, imgImage.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return bmp;
            }
            else
            {
                var neww = Convert.ToInt32(imgImage.Width * zoom);
                var newh = Convert.ToInt32(imgImage.Height * zoom);
                var bmp = new Bitmap(imgImage, neww, newh);
                return bmp;
            }
        }
        public static Bitmap CreateColorizedImage( Image image, Color color, bool leaveAlphaUntouched )
        {
            int w = image.Width;
            int h = image.Height;
            float r = ( color.R / 255f );
            float g = ( color.G / 255f );
            float b = ( color.B / 255f );
            float a = ( color.A / 255f );
            if( leaveAlphaUntouched )
                a = 0;

            Bitmap filled = new Bitmap( w, h, PixelFormat.Format32bppArgb );
            Graphics sg = Graphics.FromImage( filled );
            ColorMatrix matr = new ColorMatrix( new float[][]
                                                   {
                                                       new float[] {1f, 0, 0, 0, 0},
                                                       new float[] {0, 1f, 0, 0, 0},
                                                       new float[] {0, 0, 1f, 0, 0},
                                                       new float[] {0, 0, 0, 1f, 0},
                                                       new float[] {r, g, b, a, 1f},
                                                   } );
            ImageAttributes attr = new ImageAttributes();
            attr.SetColorMatrix( matr, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );
            sg.DrawImage( image, new Rectangle( 0, 0, w, h ), 0, 0, w, h, GraphicsUnit.Pixel, attr );

            sg.Dispose();
            return filled;
        }

        public static Bitmap GetFilledImageWithColor( Image bmp, Color col )
        {
            int w = bmp.Width;
            int h = bmp.Height;
            float r = ( col.R / 255f );
            float g = ( col.G / 255f );
            float b = ( col.B / 255f );
            float a = ( col.A / 255f );

            Bitmap filled = new Bitmap( w, h, PixelFormat.Format32bppArgb );
            using( Graphics sg = Graphics.FromImage( filled ) )
            {
                ColorMatrix matr = new ColorMatrix( new float[][]
                                                       {
                                                           new float[] {0, 0, 0, 0, 0},
                                                           new float[] {0, 0, 0, 0, 0},
                                                           new float[] {0, 0, 0, 0, 0},
                                                           new float[] {0, 0, 0, a, 0},
                                                           new float[] {r, g, b, 0, 1f},
                                                       } );
                ImageAttributes attr = new ImageAttributes();
                attr.SetColorMatrix( matr, ColorMatrixFlag.Default, ColorAdjustType.Bitmap );
                sg.DrawImage( bmp, new Rectangle( 0, 0, w, h ), 0, 0, w, h, GraphicsUnit.Pixel, attr );
            }

            return filled;
        }
    }
}