using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;

using Server;
using Server.Engines.Craft;
using Server.Items;

using Ultima;

namespace Midgard.Misc
{
    public class ResourcesDocHandler : DocumentationHandler
    {
        public ResourcesDocHandler()
        {
            Enabled = true;
        }

        private static readonly string CraftDocDir = Path.Combine( PreAoSDocHelper.DocDir, "craft" );
        private static readonly string CraftImagesDocDir = Path.Combine( CraftDocDir, "images" );

        public override void GenerateDocumentation()
        {
            RenderCraftRes( CraftResourceType.Metal );
            RenderCraftRes( CraftResourceType.Leather );
            RenderCraftRes( CraftResourceType.Wood );
            RenderCraftRes( CraftResourceType.Scales );
        }

        private static readonly string ResourcesDocDir = Path.Combine( PreAoSDocHelper.DocDir, "resources" );
        private static readonly string ResourcesImageDocDir = Path.Combine( ResourcesDocDir, "images" );

        private static void RenderCraftRes( CraftResourceType type )
        {
            PreAoSDocHelper.EnsureDirectory( CraftDocDir );
            PreAoSDocHelper.EnsureDirectory( CraftImagesDocDir );

            string name = type.ToString();
            string filePath = Path.Combine( ResourcesDocDir, PreAoSDocHelper.SafeFileName( name ) + ".html" );
            string imageOutputDirectory = ResourcesImageDocDir;

            PreAoSDocHelper.EnsureDirectory( imageOutputDirectory );

            List<string> headers = new List<string>();
            headers.Clear();
            headers.Add( "Name" );
            headers.Add( "Req. Skill To Work" );
            headers.Add( "Difficulty" );

            List<List<string>> contentMatrix = new List<List<string>>();

            CraftSystem sys = null;
            Item item = null;
            switch( type )
            {
                case CraftResourceType.Metal:
                    item = new PlateChest();
                    sys = DefBlacksmithy.CraftSystem;
                    headers.Add( "Armor Bonus" );
                    headers.Add( "Dex Malus" );
                    headers.Add( "Magical Malus" );
                    headers.Add( "Max Magical Level" );
                    headers.Add( "Smelt. Req. Skill" );
                    headers.Add( "Armor Dur. Bonus" );
                    headers.Add( "Weapon Dur. Bonus" );
                    break;
                case CraftResourceType.Leather:
                    item = new Hides();
                    sys = DefTailoring.CraftSystem;
                    headers.Add( "Armor Bonus" );
                    headers.Add( "Dex Malus" );
                    headers.Add( "Magical Malus" );
                    headers.Add( "Max Magical Level" );
                    break;
                case CraftResourceType.Wood:
                    item = new Log();
                    sys = DefCarpentry.CraftSystem;
                    headers.Add( "Min Range Bonus" );
                    headers.Add( "Max Range Bonus" );
                    headers.Add( "Damage Bonus" );
                    headers.Add( "Speed Bonus" );
                    headers.Add( "Wre Evasion Bonus" );
                    headers.Add( "Wre Hit Rate Bonus" );
                    headers.Add( "Axe Req Skill" );
                    break;
                case CraftResourceType.Scales:
                    item = new RedScales();
                    sys = DefBlacksmithy.CraftSystem;
                    headers.Add( "Armor Bonus" );
                    headers.Add( "Dex Malus" );
                    headers.Add( "Magical Malus" );
                    headers.Add( "Max Magical Level" );
                    headers.Add( "Smelt. Req. Skill" );
                    break;
            }

            using( StreamWriter op = new StreamWriter( filePath ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0} resources", name );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    html.AddAttribute( HtmlTextWriterAttribute.Border, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellpadding, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellspacing, "0" );
                    html.RenderBeginTag( HtmlTextWriterTag.Table );

                    string subTitle = string.Format( "{0} Resources", name );

                    contentMatrix.Clear();

                    if( sys == null )
                        return;

                    int count = type == CraftResourceType.Scales ? sys.CraftSubRes2.Count : sys.CraftSubRes.Count;

                    for( int i = 0; i < count; i++ )
                    {
                        List<string> contentLine = new List<string>();

                        CraftSubRes c = type == CraftResourceType.Scales ? sys.CraftSubRes2.GetAt( i ) : sys.CraftSubRes.GetAt( i );
                        CraftResource cr = CraftResources.GetFromType( c.ItemType );
                        CraftResourceInfo info = CraftResources.GetInfo( cr );

                        CraftAttributeInfo attInfo = info.AttributeInfo;

                        contentLine.Add( info.Name ); // Agapite

                        string imageFileName = Path.Combine( imageOutputDirectory, string.Format( "{0}.png", cr ) );
                        try
                        {
                            SaveResourceImage( info.Hue, item, imageFileName );
                            // SaveResourceImage( info.Hue, itemID, imageFileName );
                        }
                        catch( Exception e )
                        {
                            Console.WriteLine( e );
                        }

                        // docs3c/resources/images/Agapite.png
                        PreAoSDocHelper.ImageAliasesDict[ info.Name ] = imageFileName;

                        contentLine.Add( c.RequiredSkill.ToString( "F1" ) );
                        contentLine.Add( attInfo.OldStaticMultiply.ToString( "F2" ) );

                        switch( type )
                        {
                            case CraftResourceType.Metal:
                                contentLine.Add( attInfo.OldArmorBonus.ToString() );
                                contentLine.Add( attInfo.OldMalusDex.ToString() );
                                contentLine.Add( attInfo.OldMagicalLevelMalus.ToString() );
                                contentLine.Add( attInfo.OldMaxMagicalLevel.ToString() );
                                contentLine.Add( attInfo.OldSmeltingRequiredSkill.ToString( "F1" ) );
                                contentLine.Add( attInfo.ArmorDurability.ToString() );
                                contentLine.Add( attInfo.WeaponDurability.ToString() );
                                break;
                            case CraftResourceType.Leather:
                                contentLine.Add( attInfo.OldArmorBonus.ToString() );
                                contentLine.Add( attInfo.OldMalusDex.ToString() );
                                contentLine.Add( attInfo.OldMagicalLevelMalus.ToString() );
                                contentLine.Add( attInfo.OldMaxMagicalLevel.ToString() );
                                break;
                            case CraftResourceType.Wood:
                                contentLine.Add( attInfo.OldMinRange.ToString() );
                                contentLine.Add( attInfo.OldMaxRange.ToString() );
                                contentLine.Add( attInfo.OldDamage.ToString() );
                                contentLine.Add( attInfo.OldSpeed.ToString() );
                                contentLine.Add( attInfo.OldWrestlerEvasion.ToString() );
                                contentLine.Add( attInfo.OldWrestlerHitRate.ToString() );
                                contentLine.Add( attInfo.OldAxeRequiredSkill.ToString( "F1" ) );
                                break;
                            case CraftResourceType.Scales:
                                contentLine.Add( attInfo.OldArmorBonus.ToString() );
                                contentLine.Add( attInfo.OldMalusDex.ToString() );
                                contentLine.Add( attInfo.OldMagicalLevelMalus.ToString() );
                                contentLine.Add( attInfo.OldMaxMagicalLevel.ToString() );
                                contentLine.Add( attInfo.OldSmeltingRequiredSkill.ToString( "F1" ) );
                                break;
                        }

                        contentMatrix.Add( contentLine );
                    }

                    PreAoSDocHelper.AppendTable( html, subTitle, headers, contentMatrix, true, true, "images" );

                    html.RenderEndTag(); // Table

                    html.Write( "<br><br>" );
                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static void SaveResourceImage( int hue, Item item, string output )
        {
            Bitmap bmp = Item.GetBitmap( item.ItemID );

            if( hue > 0 )
            {
                // Console.WriteLine( "Notice: Applying hue {0} to item 0x{1:X4}", hue, item.ItemID );
                var old = bmp;
                bmp = (Bitmap)old.Clone();
                Hues.GetHue( hue - 1 ).ApplyTo( bmp, ( ( item.ItemData.Flags & Server.TileFlag.PartialHue ) == Server.TileFlag.PartialHue ) );
            }

            if( bmp == null )
                Console.WriteLine( "Warning: null bitmap for item 0x{0:X4}", item.ItemID );
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