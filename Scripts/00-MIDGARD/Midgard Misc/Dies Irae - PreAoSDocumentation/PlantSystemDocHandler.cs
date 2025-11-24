using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web.UI;

using Midgard.Engines.PlantSystem;

using Server;

using Ultima;

namespace Midgard.Misc
{
    public class PlantSystemDocHandler : DocumentationHandler
    {
        public PlantSystemDocHandler()
        {
            Enabled = false;
        }

        private static readonly string PlantsDocDir = Path.Combine( PreAoSDocHelper.DocDir, "gardening" );
        private static readonly string PlantsImagesDocDir = Path.Combine( PlantsDocDir, "images" );

        private static readonly List<Type> m_SeedTypes = new List<Type>();

        private static void ProcessSeedTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( Type type in types )
                {
                    if( type.IsSubclassOf( typeof( BaseSeed ) ) )
                        m_SeedTypes.Add( type );
                }
            }

            m_SeedTypes.Sort( InternalComparer.Instance );
        }

        public static Bitmap MergeImages( Image img1, int x1, int y1, Image img2, int x2, int y2 )
        {
            if( y1 < 0 )
            {
                y2 -= y1;
                y1 = 0;
            }
            if( x1 < 0 )
            {
                x2 -= x1;
                x1 = 0;
            }
            if( x2 < 0 )
            {
                x1 -= x2;
                x2 = 0;
            }
            if( y2 < 0 )
            {
                y1 -= y2;
                y2 = 0;
            }
            var neww = Math.Max( img1.Width + x1, img2.Width + x2 );
            var newh = Math.Max( img1.Height + y1, img2.Height + y2 );
            var bmp = new Bitmap( neww, newh, PixelFormat.Format32bppArgb );
            using( var g = Graphics.FromImage( bmp ) )
            {
                g.DrawImage( img1, x1, y1 );
                g.DrawImage( img2, x2, y2 );
            }
            return bmp;
        }

        public override void GenerateDocumentation()
        {
            ProcessSeedTypes();

            PreAoSDocHelper.EnsureDirectory( PlantsDocDir );
            PreAoSDocHelper.EnsureDirectory( PlantsImagesDocDir );

            List<string> headers = new List<string>();
            headers.Add( "Name" );
            headers.Add( "Roots Radius" );
            headers.Add( "Plant Name" );
            headers.Add( "Req Skill to Plant" );
            headers.Add( "Req Skill to Harvest" );

            BasePlant basePlant;
            BaseSeed baseSeed;
            Mobile from = new Mobile();

            using( StreamWriter op = new StreamWriter( Path.Combine( PlantsImagesDocDir, "gardening.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Gardening" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    foreach( Type type in m_SeedTypes )
                    {
                        if( type.IsAbstract )
                            continue;

                        try
                        {
                            baseSeed = Activator.CreateInstance( type ) as BaseSeed;

                            if( baseSeed != null )
                            {
                                basePlant = Activator.CreateInstance( baseSeed.PlantTypes[ 0 ], new object[] { from } ) as BasePlant;

                                if( basePlant != null )
                                {
                                    string plantName = type.Name;

                                    string imageFileName = Path.Combine( PlantsDocDir, string.Format( "{0}.png", plantName ) );
                                    SavePlantImage( basePlant, basePlant.GetType(), imageFileName );

                                    string name = MidgardUtility.GetFriendlyClassName( type.Name );

                                    PreAoSDocHelper.ImageAliasesDict[ name ] = imageFileName;

                                    List<string> contentLine = new List<string>();
                                    contentLine.Add( name );
                                    contentLine.Add( baseSeed.RootRadius.ToString() );
                                    contentLine.Add( baseSeed.PlantName );
                                    contentLine.Add( baseSeed.RequiredSkillToPlant.ToString( "F2" ) );
                                    contentLine.Add( basePlant.MinSkillToHarvest.ToString( "F2" ) );

                                    contentMatrix.Add( contentLine );
                                }
                            }
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }

                    PreAoSDocHelper.AppendTable( html, "Midgard Gardening", headers, contentMatrix, true, true, "images" );
                    html.Write( "<br><br>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static void SavePlantImage( BasePlant plant, Type plantType, string imageFileName )
        {
            //if( plantType.IsSubclassOf( typeof( BaseTree ) ) )
            //    return; // TODO...

            Bitmap bmp = Item.GetBitmap( plant.PhaseIDs[ plant.PhaseIDs.Length - 1 ] );

            if( plant.Hue > 0 )
            {
                // Console.WriteLine( "Notice: Applying hue {0} to item 0x{1:X4}", hue, item.ItemID );
                var old = bmp;
                bmp = (Bitmap)old.Clone();
                Hues.GetHue( plant.Hue - 1 ).ApplyTo( bmp, ( ( plant.ItemData.Flags & Server.TileFlag.PartialHue ) == Server.TileFlag.PartialHue ) );
            }

            if( bmp == null )
                Console.WriteLine( "Warning: null bitmap for item 0x{0:X4}", plant.ItemID );
            else
            {
                try
                {
                    bmp.Save( imageFileName, ImageFormat.Png );
                }
                catch( Exception e )
                {
                    Console.WriteLine( imageFileName );
                    Console.WriteLine( e.ToString() );
                }
            }
        }

        private class InternalComparer : IComparer<Type>
        {
            public static readonly IComparer<Type> Instance = new InternalComparer();

            private InternalComparer()
            {
            }

            public int Compare( Type x, Type y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                return Insensitive.Compare( x.Name, y.Name );
            }
        }
    }
}