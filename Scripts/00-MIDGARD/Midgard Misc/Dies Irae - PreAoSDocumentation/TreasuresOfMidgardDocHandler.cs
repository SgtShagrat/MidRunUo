using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;

using Midgard.Items;

using Server;

namespace Midgard.Misc
{
    public class TreasuresOfMidgardDocHandler : DocumentationHandler
    {
        public TreasuresOfMidgardDocHandler()
        {
            Enabled = true;
        }

        private static readonly string TreasuresOfMidgardDocDir = Path.Combine( PreAoSDocHelper.DocDir, "treasuresOfMidgard" );
        private static readonly string TreasuresOfMidgardImagesDocDir = Path.Combine( TreasuresOfMidgardDocDir, "images" );

        public override void GenerateDocumentation()
        {
            PreAoSDocHelper.EnsureDirectory( TreasuresOfMidgardDocDir );
            PreAoSDocHelper.EnsureDirectory( TreasuresOfMidgardImagesDocDir );

            List<string> headers = new List<string>();

            using( StreamWriter op = new StreamWriter( Path.Combine( TreasuresOfMidgardDocDir, "treasuresOfMidgard.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Treasures Of Midgard" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    html.AddAttribute( HtmlTextWriterAttribute.Border, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellpadding, "1" );
                    html.AddAttribute( HtmlTextWriterAttribute.Cellspacing, "0" );
                    html.RenderBeginTag( HtmlTextWriterTag.Table );

                    contentMatrix.Clear();

                    StringBuilder sb;

                    foreach( Type t in TreasuresOfMidgard.Artifacts )
                    {
                        Item item = GetInstance( t );
                        if( item == null )
                            continue;

                        ITreasureOfMidgard treasure = item as ITreasureOfMidgard;
                        if( treasure == null )
                            continue;

                        sb = new StringBuilder();
                        sb.AppendLine( "<b>" + item.Name + "</b>" );
                        treasure.Doc( sb );
                        sb.Replace( "\n", "<br>" );

                        List<string> contentLine = new List<string>();
                        contentLine.Add( sb.ToString() );

                        contentMatrix.Add( contentLine );
                    }

                    PreAoSDocHelper.AppendTable( html, "Midgard Treasures", headers, contentMatrix, true, false, "" );

                    html.Write( "<br><br>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static Item GetInstance( Type type )
        {
            if( type == null )
                return null;

            Item item = null;
            try
            {
                item = Activator.CreateInstance( type ) as Item;
            }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
            }

            if( item == null )
                Console.WriteLine( "Warning: null item instance for item " + type.Name );

            return item;
        }
    }
}