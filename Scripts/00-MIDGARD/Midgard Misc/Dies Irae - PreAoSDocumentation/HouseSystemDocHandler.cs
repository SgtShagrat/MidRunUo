using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web.UI;

using Midgard.Multis.Deeds;

using Server;
using Server.Multis;
using Server.Multis.Deeds;

using MultiComponentList = Ultima.MultiComponentList;

namespace Midgard.Misc
{
    public class HouseSystemDocHandler : DocumentationHandler
    {
        public static void Initialize()
        {
            PreAoSDocHelper.Register( new HouseSystemDocHandler() );
        }

        public HouseSystemDocHandler()
        {
            Enabled = false;
        }

        private static readonly string HouseDocDir = Path.Combine( PreAoSDocHelper.DocDir, "houses" );
        private static readonly string HouseImagesDocDir = Path.Combine( HouseDocDir, "images" );

        private static readonly List<Type> m_HouseTypes = new List<Type>();

        private static void ProcessTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( Type type in types )
                {
                    if( type.IsSubclassOf( typeof( HouseDeed ) ) )
                        m_HouseTypes.Add( type );
                }
            }

            m_HouseTypes.Sort( InternalComparer.Instance );
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

        public override void GenerateDocumentation()
        {
            ProcessTypes();

            PreAoSDocHelper.EnsureDirectory( HouseDocDir );
            PreAoSDocHelper.EnsureDirectory( HouseImagesDocDir );

            List<string> headers = new List<string>();
            headers.Add( "Name" );
            headers.Add( "Lockdowns" );
            headers.Add( "Secures" );
            headers.Add( "Cost" );

            Mobile owner = new Mobile();
            BaseHouse house;
            HouseDeed deed;

            using( StreamWriter op = new StreamWriter( Path.Combine( HouseDocDir, "houses.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Houses" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    foreach( Type type in m_HouseTypes )
                    {
                        if( type.IsAbstract || type.IsSubclassOf( typeof( FillableHouseDeed ) ) )
                            continue;

                        try
                        {
                            deed = Activator.CreateInstance( type ) as HouseDeed;
                            if( deed != null )
                            {
                                house = deed.GetHouse( owner );
                                if( house != null )
                                {
                                    string deedName = type.Name;
                                    deedName = deedName.Replace( "Deed", string.Empty );

                                    string imageFileName = Path.Combine( HouseImagesDocDir, string.Format( "{0}.png", deedName ) );
                                    SaveMultiImage( deed.MultiID, imageFileName );

                                    string name = MidgardUtility.GetFriendlyClassName( type.Name );
                                    name = name.Replace( "Deed", "" );

                                    // docs3c/houses/images/BlueTent.png
                                    PreAoSDocHelper.ImageAliasesDict[ name ] = imageFileName;

                                    List<string> contentLine = new List<string>();
                                    contentLine.Add( name );
                                    contentLine.Add( house.MaxLockDowns.ToString() );
                                    contentLine.Add( house.MaxSecures.ToString() );
                                    contentLine.Add( house.DefaultPrice.ToString() );

                                    contentMatrix.Add( contentLine );
                                }
                            }
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }

                    PreAoSDocHelper.AppendTable( html, "Midgard Houses", headers, contentMatrix, true, true, "images" );
                    html.Write( "<br><br>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static void SaveMultiImage( int id, string fileName )
        {
            MultiComponentList mcl = Ultima.Multis.Load( id );
            if( mcl == null )
                return;

            Bitmap bmp = mcl.GetImage();
            if( bmp == null )
                Console.WriteLine( "Warning: null bitmap for id {0}\n", id );
            else
                bmp.Save( fileName, ImageFormat.Png );
        }
    }
}