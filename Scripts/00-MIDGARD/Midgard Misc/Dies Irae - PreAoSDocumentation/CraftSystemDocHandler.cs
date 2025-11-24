using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.UI;

using Midgard.Engines.AdvancedCooking;
using Midgard.Engines.BrewCrafing;
using Midgard.Engines.Craft;
using Midgard.Items;

using Server;
using Server.Engines.Craft;

using Ultima;

using StringList = Server.StringList;

namespace Midgard.Misc
{
    public class CraftSystemDocHandler : DocumentationHandler
    {
        public CraftSystemDocHandler()
        {
            Enabled = false;
        }

        private static readonly string CraftDocDir = Path.Combine( PreAoSDocHelper.DocDir, "craft" );
        private static readonly string CraftImagesDocDir = Path.Combine( CraftDocDir, "images" );

        public override void GenerateDocumentation()
        {
            RenderCraftSystem( DefAlchemy.CraftSystem );
            RenderCraftSystem( DefBlacksmithy.CraftSystem );
            RenderCraftSystem( DefBowFletching.CraftSystem );
            RenderCraftSystem( DefCarpentry.CraftSystem );
            RenderCraftSystem( DefCartography.CraftSystem );
            RenderCraftSystem( DefCooking.CraftSystem );
            RenderCraftSystem( DefGlassblowing.CraftSystem );
            RenderCraftSystem( DefInscription.CraftSystem );
            RenderCraftSystem( DefMasonry.CraftSystem );
            RenderCraftSystem( DefTailoring.CraftSystem );
            RenderCraftSystem( DefTinkering.CraftSystem );

            RenderCraftSystem( DefBaking.CraftSystem );
            RenderCraftSystem( DefBoiling.CraftSystem );
            RenderCraftSystem( DefGrilling.CraftSystem );
            RenderCraftSystem( DefBrewing.CraftSystem );
            RenderCraftSystem( DefCrystalCrafting.CraftSystem );
            RenderCraftSystem( DefWaxCrafting.CraftSystem );
        }

        private static void RenderCraftSystem( CraftSystem sys )
        {
            PreAoSDocHelper.EnsureDirectory( CraftDocDir );
            PreAoSDocHelper.EnsureDirectory( CraftImagesDocDir );

            string filePath = Path.Combine( CraftDocDir, PreAoSDocHelper.SafeFileName( sys.Name ) + ".html" );
            string imageOutputDirectory = Path.Combine( CraftImagesDocDir, sys.Name.ToLower() );
            PreAoSDocHelper.EnsureDirectory( imageOutputDirectory );

            List<string> headers = new List<string>();
            headers.Clear();
            headers.Add( "Item Name" );
            headers.Add( "Materials Needed" );
            headers.Add( "Minimum Skill" );

            List<List<string>> contentMatrix = new List<List<string>>();
            StringBuilder resBuilder = new StringBuilder();

            CraftGroupCol groups = sys.CraftGroups;

            using( StreamWriter op = new StreamWriter( filePath ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", sys.Name );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    html.AddAttribute( HtmlTextWriterAttribute.Border, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellpadding, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellspacing, "0" );
                    html.RenderBeginTag( HtmlTextWriterTag.Table );

                    for( int i = 0; i < groups.Count; i++ )
                    {
                        CraftGroup group = groups.GetAt( i );
                        string subTitle = StringList.GetClilocString( group.NameString, group.NameNumber );
                        contentMatrix.Clear();

                        CraftItemCol items = group.CraftItems;
                        for( int j = 0; j < items.Count; j++ )
                        {
                            CraftItem item = items.GetAt( j );
                            if( item.ItemType == null || IsNotDocumentableType( item.ItemType ) )
                                continue;

                            List<string> contentLine = new List<string>();
                            string name = StringUtility.Capitalize( StringList.GetClilocString( item.NameString, item.NameNumber ) );

                            contentLine.Add( name );

                            string imageFileName = Path.Combine( imageOutputDirectory, string.Format( "{0}.png", item.ItemType.Name ) );
                            try
                            {
                                SaveCraftItemImage( item, imageFileName );
                            }
                            catch( Exception e )
                            {
                                Console.WriteLine( e );
                            }

                            // docs3c/craft/images/alchemy/Garlic.png
                            PreAoSDocHelper.ImageAliasesDict[ name ] = imageFileName;

                            resBuilder.Remove( 0, resBuilder.Length );
                            for( int k = 0; k < item.Resources.Count; k++ )
                            {
                                CraftRes craftResource = item.Resources.GetAt( k );
                                string resName = StringList.GetClilocString( craftResource.NameString, craftResource.NameNumber );
                                resBuilder.AppendFormat( "{0} {1}<br>", craftResource.Amount, StringUtility.Capitalize( resName ) );
                            }
                            contentLine.Add( resBuilder.ToString() );

                            double skill = Math.Max( item.GetMinMainCraftSkill( sys ), 0.0 );
                            contentLine.Add( skill.ToString( "F2" ) );

                            contentMatrix.Add( contentLine );
                        }

                        PreAoSDocHelper.AppendTable( html, subTitle, headers, contentMatrix, false, true, Path.Combine( "images", sys.Name ) );
                    }

                    html.RenderEndTag(); // Table

                    html.Write( "<br><br>" );
                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static readonly Type[] m_NotDocumentables = new Type[]
                                                            {
                                                                typeof(ScoutBlueTentRoll),
                                                                typeof(ScoutGreenTentRoll)
                                                            };

        private static bool IsNotDocumentableType( Type type )
        {
            bool contains = false;

            for( int i = 0; !contains && i < m_NotDocumentables.Length; ++i )
                contains = ( m_NotDocumentables[ i ] == type );

            return contains;
        }

        private static void SaveCraftItemImage( CraftItem craftItem, string output )
        {
            Type type = craftItem.ItemType;
            if( type == null )
            {
                Console.WriteLine( "Warning: null item type for craft item " + StringList.GetClilocString( craftItem.NameString, craftItem.NameNumber ) );
                return;
            }

            Item item = null;
            try { item = Activator.CreateInstance( type ) as Item; }
            catch( Exception e )
            { Console.WriteLine( e.ToString() ); }

            if( item == null )
            {
                Console.WriteLine( "Warning: null item instance for item " + type.Name );
                return;
            }

            int itemID = item.ItemID;
            int hue = item.Hue;

            Bitmap bmp = Item.GetBitmap( itemID );
            if( hue > 0 )
            {
                // Console.WriteLine( "Notice: Applying hue {0} to item {1}", hue, type.Name );
                var old = bmp;
                bmp = (Bitmap)old.Clone();
                Hues.GetHue( hue - 1 ).ApplyTo( bmp, ( ( item.ItemData.Flags & Server.TileFlag.PartialHue ) == Server.TileFlag.PartialHue ) );
            }

            if( bmp == null )
                Console.WriteLine( "Warning: null bitmap for item " + type.Name );
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
    }
}