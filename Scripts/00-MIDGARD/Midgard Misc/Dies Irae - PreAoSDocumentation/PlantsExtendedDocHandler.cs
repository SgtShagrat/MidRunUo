/***************************************************************************
 *                               PlantsExtendedDocHandler.cs
 *
 *   begin                : 27 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

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
    public class PlantsExtendedDocHandler : DocumentationHandler
    {
        public static void Initialize()
        {
            PreAoSDocHelper.Register( new PlantsExtendedDocHandler() );
        }

        private static readonly string PlantsDocDir = Path.Combine( PreAoSDocHelper.DocDir, "plants" );
        private static readonly string PlantsImagesDocDir = Path.Combine( PlantsDocDir, "images" );

        public PlantsExtendedDocHandler()
        {
            Enabled = true;
        }

        private static IEnumerable<Type> ProcessPlantTypes( Type baseType )
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            List<Type> list = new List<Type>();

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( Type type in types )
                {
                    if( type.IsSubclassOf( baseType ) )
                        list.Add( type );
                }
            }

            list.Sort( InternalComparer.Instance );

            return list;
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

        private static readonly string[] m_Headers = new string[]
                                                {
                                                    "Name", 
                                                    "DormantDrought", "GrowthTick", "LifeSpan", "LimitedLifeSpan", "LongDrought", 
                                                    "CanProduce", "Capacity", "CropName", "ProduceTick", 
                                                    "HarvestDelay", "HarvestingTool","HarvestInPack","MinSkillToHarvest", "RequiredSkillNameToHarvest",
                                                    "MinDiffSkillToCare", "SkillNameToCare"
                                                };

        public override void GenerateDocumentation()
        {
            IEnumerable<Type> crops = ProcessPlantTypes( typeof( BaseCrop ) );
            IEnumerable<Type> trees = ProcessPlantTypes( typeof( BaseTree ) );

            PreAoSDocHelper.EnsureDirectory( PlantsDocDir );
            PreAoSDocHelper.EnsureDirectory( PlantsImagesDocDir );

            List<string> headers = new List<string>();
            foreach( string s in m_Headers )
                headers.Add( s );

            using( StreamWriter op = new StreamWriter( Path.Combine( PlantsDocDir, "plants.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Plants" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    RenderContent( crops, contentMatrix );
                    PreAoSDocHelper.AppendTable( html, "Midgard Plants -  Crops", headers, contentMatrix, true, true, "images" );

                    contentMatrix.Clear();

                    RenderContent( trees, contentMatrix );
                    PreAoSDocHelper.AppendTable( html, "Midgard Plants - Trees", headers, contentMatrix, true, true, "images" );

                    html.Write( "<br><br>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static void RenderContent( IEnumerable<Type> types, List<List<string>> contentMatrix )
        {
            Mobile owner = new Mobile();

            foreach( Type type in types )
            {
                if( type.IsAbstract )
                    continue;

                try
                {
                    BasePlant basePlant = Activator.CreateInstance( type, owner ) as BasePlant;
                    if( basePlant != null )
                    {
                        string plantName = type.Name;

                        string imageFileName = Path.Combine( PlantsImagesDocDir, string.Format( "{0}.png", plantName ) );
                        SavePlantImage( basePlant, basePlant.GetType(), imageFileName );

                        string name = MidgardUtility.GetFriendlyClassName( type.Name );

                        PreAoSDocHelper.ImageAliasesDict[ name ] = imageFileName;

                        List<string> contentLine = new List<string>();
                        contentLine.Add( GetFriendlyClassName( type.Name ) );

                        contentLine.Add( basePlant.DormantDrought.TotalHours.ToString() );
                        contentLine.Add( basePlant.GrowthTick.ToString() );
                        contentLine.Add( basePlant.LifeSpan.TotalDays.ToString() );
                        contentLine.Add( basePlant.LimitedLifeSpan.ToString() );
                        contentLine.Add( basePlant.LongDrought.TotalHours.ToString() );

                        contentLine.Add( basePlant.CanProduce.ToString() );
                        contentLine.Add( basePlant.Capacity.ToString() );
                        contentLine.Add( basePlant.CropName );
                        contentLine.Add( basePlant.ProduceTick.ToString() );

                        contentLine.Add( basePlant.HarvestDelay.ToString() );
                        contentLine.Add( basePlant.HarvestingTool.Name );
                        contentLine.Add( basePlant.HarvestInPack.ToString() );
                        contentLine.Add( basePlant.MinSkillToHarvest.ToString( "F1" ) );
                        contentLine.Add( basePlant.RequiredSkillNameToHarvest.ToString() );

                        contentLine.Add( basePlant.MinDiffSkillToCare.ToString( "F1" ) );
                        contentLine.Add( basePlant.SkillNameToCare.ToString() );

                        contentMatrix.Add( contentLine );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        private static void SavePlantImage( BasePlant plant, Type plantType, string output )
        {
            Bitmap bmp = Item.GetBitmap( plant.PhaseIDs[ plant.PhaseIDs.Length - 1 ] );

            if( plant.Hue > 0 )
            {
                Bitmap old = bmp;
                bmp = (Bitmap)old.Clone();
                Hues.GetHue( plant.Hue - 1 ).ApplyTo( bmp, ( ( plant.ItemData.Flags & Server.TileFlag.PartialHue ) == Server.TileFlag.PartialHue ) );
            }

            if( plantType.IsSubclassOf( typeof( BaseTree ) ) && ( (BaseTree)plant ).LeavesIDs.Length > 0 )
            {
                BaseTree tree = ( (BaseTree)plant );
                Bitmap leaves = null;

                if( tree.CanProduce && tree.ProductLeavesIDs.Length > 0 )
                    leaves = Item.GetBitmap( ( (BaseTree)plant ).ProductLeavesIDs[ 0 ] );
                else
                    leaves = Item.GetBitmap( ( (BaseTree)plant ).LeavesIDs[ 0 ] );

                if( leaves != null )
                    bmp = MergeImages( bmp, 0, 0, leaves, 0, 0 + ( (BaseTree)plant ).FixLeavesAltitude );
            }

            if( bmp == null )
                Console.WriteLine( "Warning: null bitmap for item 0x{0:X4}", plant.ItemID );
            else
            {
                try
                {
                    bmp.Save( output, ImageFormat.Png );
                }
                catch( Exception e )
                {
                    Console.WriteLine( output );
                    Console.WriteLine( e.ToString() );
                }
            }
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

        private static string GetFriendlyClassName( string typeName )
        {
            string temp = typeName;

            for( int index = 1; index < temp.Length; index++ )
            {
                if( char.IsUpper( temp, index ) )
                    temp = temp.Insert( index++, " " );
            }

            return temp;
        }
    }
}