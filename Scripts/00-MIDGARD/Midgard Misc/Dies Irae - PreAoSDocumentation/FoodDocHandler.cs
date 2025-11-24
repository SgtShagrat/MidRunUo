/***************************************************************************
 *                               FoodDocHandler.cs
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
using Server;
using Server.Items;
using Ultima;

namespace Midgard.Misc
{
    public class FoodDocHandler : DocumentationHandler
    {
        public static void Initialize()
        {
            PreAoSDocHelper.Register( new FoodDocHandler() );
        }

        private static readonly string DocDir = Path.Combine( PreAoSDocHelper.DocDir, "food" );
        private static readonly string ImagesDocDir = Path.Combine( DocDir, "images" );

        public FoodDocHandler()
        {
            Enabled = true;
        }

        private static IEnumerable<Type> ProcessTypes( Type baseType, bool excludeAbstract, bool excludeNotConstructable, bool sortByName )
        {
            Assembly[] asms = ScriptCompiler.Assemblies;

            List<Type> list = new List<Type>();

            foreach( Assembly asm in asms )
            {
                TypeCache tc = ScriptCompiler.GetTypeCache( asm );
                Type[] types = tc.Types;

                foreach( Type type in types )
                {
                    if( excludeAbstract && type.IsAbstract )
                        continue;

                    if( excludeNotConstructable && !IsConstructable( type ) )
                        continue;

                    if( type.IsSubclassOf( baseType ) )
                        list.Add( type );
                }
            }

            if( sortByName )
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
                                                    "Fill Factor"
                                                };

        public override void GenerateDocumentation()
        {
            IEnumerable<Type> food = ProcessTypes( typeof( Food ), true, true, true );

            PreAoSDocHelper.EnsureDirectory( DocDir );
            PreAoSDocHelper.EnsureDirectory( ImagesDocDir );

            List<string> headers = new List<string>();
            foreach( string s in m_Headers )
                headers.Add( s );

            using( StreamWriter op = new StreamWriter( Path.Combine( DocDir, "food.html" ) ) )
            {
                using( HtmlTextWriter html = new HtmlTextWriter( op, "\t" ) )
                {
                    html.RenderBeginTag( HtmlTextWriterTag.Html );

                    html.RenderBeginTag( HtmlTextWriterTag.Head );

                    html.RenderBeginTag( HtmlTextWriterTag.Title );
                    html.Write( "Midgard Third Crown Documentation - {0}\n", "Food" );
                    html.RenderEndTag(); // Title

                    html.RenderEndTag(); // Head

                    html.RenderBeginTag( HtmlTextWriterTag.Body );

                    List<List<string>> contentMatrix = new List<List<string>>();

                    RenderContent( food, contentMatrix );
                    PreAoSDocHelper.AppendTable( html, "Midgard -  Food", headers, contentMatrix, true, true, "images" );

                    contentMatrix.Clear();

                    html.Write( "<br><br>" );

                    html.RenderEndTag(); // Body

                    html.RenderEndTag(); // Html
                }
            }
        }

        private static void RenderContent( IEnumerable<Type> types, List<List<string>> contentMatrix )
        {
            foreach( Type type in types )
            {
                try
                {
                    Food food = Activator.CreateInstance( type ) as Food;
                    if( food != null )
                    {
                        string typeName = type.Name;

                        string imageFileName = Path.Combine( ImagesDocDir, string.Format( "{0}.png", typeName ) );
                        SaveImage( food, imageFileName );

                        string name = MidgardUtility.GetFriendlyClassName( type.Name );

                        PreAoSDocHelper.ImageAliasesDict[ name ] = imageFileName;

                        List<string> contentLine = new List<string>();
                        contentLine.Add( MidgardUtility.GetFriendlyClassName( type.Name ) );

                        contentLine.Add( food.FillFactor.ToString() );

                        contentMatrix.Add( contentLine );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Failed to document type: {0}", type.Name );
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        private static void SaveImage( Item item, string output )
        {
            Bitmap bmp = Item.GetBitmap( item.ItemID );

            if( item.Hue > 0 )
            {
                Bitmap old = bmp;
                bmp = (Bitmap)old.Clone();
                Hues.GetHue( item.Hue - 1 ).ApplyTo( bmp, ( ( item.ItemData.Flags & Server.TileFlag.PartialHue ) == Server.TileFlag.PartialHue ) );
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

        private static readonly Type m_ConstructableType = typeof( ConstructableAttribute );

        public static bool IsConstructable( Type t )
        {
            ConstructorInfo[] ctors = t.GetConstructors();
            foreach( ConstructorInfo ctor in ctors )
            {
                if( ctor == null )
                    continue;

                if( IsConstructable( ctor ) )
                    return true;
            }

            return false;
        }

        public static bool IsConstructable( ConstructorInfo ctor )
        {
            object[] attrs = ctor.GetCustomAttributes( m_ConstructableType, false );
            return attrs.Length > 0;
        }
    }
}