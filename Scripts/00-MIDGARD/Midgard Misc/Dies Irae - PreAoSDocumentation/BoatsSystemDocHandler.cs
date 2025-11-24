using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Web.UI;

using Server;
using Server.Multis;

using MultiComponentList = Ultima.MultiComponentList;

namespace Midgard.Misc
{
    public class BoatsSystemDocHandler : DocumentationHandler
    {
        public static void Initialize()
        {
            PreAoSDocHelper.Register( new BoatsSystemDocHandler() );
        }

        private static readonly string BoatsDocDir = Path.Combine( PreAoSDocHelper.DocDir, "boats" );
        private static readonly string BoatsImagesDocDir = Path.Combine( BoatsDocDir, "images" );

        private static readonly List<Type> m_BoatTypes = new List<Type>();

        private static void ProcessBoatTypes()
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( Type type in types )
                {
                    if( type.IsSubclassOf( typeof( BaseBoatDeed ) ) )
                        m_BoatTypes.Add( type );
                }
            }

            m_BoatTypes.Sort( InternalComparer.Instance );
        }

        public override void GenerateDocumentation()
        {
            ProcessBoatTypes();

            PreAoSDocHelper.EnsureDirectory( BoatsDocDir );
            PreAoSDocHelper.EnsureDirectory( BoatsImagesDocDir );

            List<string> headers = new List<string>();
            headers.Add( "Name" );
            headers.Add( "Hold Size" );
            headers.Add( "Cost" );
            headers.Add( "Hits" );
            headers.Add( "Res. Fire" );
            headers.Add( "Res. Physical" );
            headers.Add( "Description" );

            BaseBoat baseBoat;
            BaseBoatDeed deed;

            using( StreamWriter op = new StreamWriter( Path.Combine( BoatsDocDir, "boats.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Boats" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    foreach( Type type in m_BoatTypes )
                    {
                        if( type.IsAbstract )
                            continue;

                        try
                        {
                            deed = Activator.CreateInstance( type ) as BaseBoatDeed;
                            if( deed != null )
                            {
                                baseBoat = deed.Boat;
                                if( baseBoat != null )
                                {
                                    string deedName = type.Name;
                                    deedName = deedName.Replace( "deed", string.Empty ); // remove "deed" from "large ship deed"
                                    deedName = deedName.Trim();

                                    string imageFileName = Path.Combine( BoatsImagesDocDir, string.Format( "{0}.png", deedName ) );
                                    SaveMultiImage( deed.MultiID, imageFileName );

                                    string name = MidgardUtility.GetFriendlyClassName( type.Name );
                                    name = name.Replace( "Deed", "" );
                                    deedName = deedName.Trim();

                                    // docs3c/boats/images/LargeShip.png
                                    PreAoSDocHelper.ImageAliasesDict[ name ] = imageFileName;

                                    List<string> contentLine = new List<string>();
                                    contentLine.Add( name );
                                    contentLine.Add( baseBoat.HoldSize.ToString() );
                                    contentLine.Add( baseBoat.BoatPrice.ToString() );
                                    contentLine.Add( baseBoat.HitsMax.ToString() );
                                    contentLine.Add( baseBoat.ResistFire.ToString() );
                                    contentLine.Add( baseBoat.ResistPhysical.ToString() );
                                    contentLine.Add( baseBoat.Description );
                                    contentMatrix.Add( contentLine );
                                }
                            }
                        }
                        catch( Exception ex )
                        {
                            Console.WriteLine( ex.ToString() );
                        }
                    }

                    PreAoSDocHelper.AppendTable( html, "Midgard Boats", headers, contentMatrix, true, true, "images" );
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