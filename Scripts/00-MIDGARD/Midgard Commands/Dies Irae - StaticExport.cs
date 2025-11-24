/***************************************************************************
 *                               Dies Irae - StaticExport.cs
 *
 *   begin                : 18 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
    public class StaticExport
    {
        public static void Initialize()
        {
            CommandSystem.Register( "StaticExport", AccessLevel.Administrator, new CommandEventHandler( StaticExport_OnCommand ) );
        }

        [Usage( "StaticExport" )]
        [Description( "Convert Statics in a cfg decoration file." )]
        public static void StaticExport_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 5 )
            {
                string file = e.GetString( 0 );
                int x1 = e.GetInt32( 1 );
                int y1 = e.GetInt32( 2 );
                int x2 = e.GetInt32( 3 );
                int y2 = e.GetInt32( 4 );
                Export( e.Mobile, file, x1, y1, x2, y2, false, false );
            }
            else if( e.Length == 2 )
            {
                string file = e.GetString( 0 );
                if( e.GetString( 1 ) == "region" )
                    Export( e.Mobile, file, 0, 0, 0, 0, true, false );
                else if( e.GetString( 1 ) == "facet" )
                    Export( e.Mobile, file, 0, 0, 0, 0, false, true );
            }
            else if( e.Arguments.Length == 1 )
                BeginStaEx( e.Mobile, e.GetString( 0 ) );
            else
                e.Mobile.SendMessage( "Usage: StaticExport filename <x1 y1 x2 y2 | region | facet>" );
        }

        public static void BeginStaEx( Mobile from, string file )
        {
            BoundingBoxPicker.Begin( from, new BoundingBoxCallback( StaExBox_Callback ), new object[] { file } );
        }

        private static void StaExBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            object[] states = (object[]) state;
            string file = (string) states[ 0 ];

            Export( from, file, start.X, start.Y, end.X, end.Y, false, false );
        }

        private static void Export( Mobile from, string file, int X1, int Y1, int X2, int Y2, bool region, bool facet )
        {
            int x1 = X1;
            int y1 = Y1;
            int x2 = X2;
            int y2 = Y2;

            if( X1 > X2 )
            {
                x1 = X2;
                x2 = X1;
            }

            if( Y1 < Y2 )
            {
                y1 = Y2;
                y2 = Y1;
            }

            Map map = from.Map;
            List< Item > list = new List< Item >();
            Dictionary< string, List< string > > dicOfItemIDs = new Dictionary< string, List< string > >();

            if( !Directory.Exists( @"./Export/" ) )
                Directory.CreateDirectory( @"./Export/" );

            using ( StreamWriter op = new StreamWriter( String.Format( @"./Export/{0}.cfg", file ) ) )
            {
                from.SendMessage( "Saving Statics..." );

                if( facet )
                {
                    foreach ( Item item in World.Items.Values )
                    {
                        if( item.Decays == false && item.Movable == false && item.Parent == null && ( ( item.X >= x1 && item.X <= x2 ) && ( item.Y <= y1 && item.Y >= y2 ) && item.Map == map ) )
                        {
                            list.Add( item );
                        }
                    }
                }
                else if( region )
                {
                    foreach ( var rectangle3D in from.Region.Area )
                    {
                        Point3D start = rectangle3D.Start;
                        Point3D end = rectangle3D.End;

                        int sx = ( start.X > end.X ) ? end.X : start.X;
                        int sy = ( start.Y > end.Y ) ? end.Y : start.Y;
                        int ex = ( start.X < end.X ) ? end.X : start.X;
                        int ey = ( start.Y < end.Y ) ? end.Y : start.Y;

                        IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( sx, sy, ex - sx + 1, ey - sy + 1 ) );
                        foreach ( Item item in eable )
                        {
                            // is it within the bounding area
                            if( item.Parent == null )
                            {
                                // add the item
                                if( item.Visible && !item.Movable )
                                {
                                    list.Add( item );
                                }
                            }
                        }

                        eable.Free();
                    }
                }
                else
                {
                    IPooledEnumerable eable = map.GetItemsInBounds( new Rectangle2D( x1, y1, x2 - x1, y2 - y1 ) );
                    foreach ( Item item in eable )
                    {
                        // is it within the bounding area
                        if( item.Parent == null )
                        {
                            // add the item
                            if( item.Visible && !item.Movable )
                            {
                                list.Add( item );
                            }
                        }
                    }

                    eable.Free();
                }

                foreach ( Item item in list )
                {
                    Map mapDestFinal;

                    if( item is PublicMoongate )
                    {
                        op.WriteLine( "PublicMoongate 3948" );
                        op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
                        op.WriteLine( "" );
                    }
                    else if( item is Moongate )
                    {
                        op.WriteLine( "Moongate 3948 (Target={0}; TargetMap={1})", ( (Moongate) item ).Target, ( (Moongate) item ).TargetMap );
                        op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
                        op.WriteLine( "" );
                    }
                    else if( item is Teleporter )
                    {
                        mapDestFinal = ( (Teleporter) item ).MapDest ?? ( item ).Map;
                        op.WriteLine( "Teleporter 7107 (PointDest={0}; MapDestination={1})", ( (Teleporter) item ).PointDest, mapDestFinal );
                        op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
                        op.WriteLine( "" );
                    }
                    else if( item is KeywordTeleporter )
                    {
                        mapDestFinal = ( (KeywordTeleporter) item ).MapDest ?? ( item ).Map;
                        op.WriteLine( "KeywordTeleporter 7107 (PointDest={0}; MapDestination={1}; Range={2}; Substring={3})", ( (KeywordTeleporter) item ).PointDest, mapDestFinal, ( (KeywordTeleporter) item ).Range, ( (KeywordTeleporter) item ).Substring );
                        op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
                        op.WriteLine( "" );
                    }
                    else if( item is Spawner )
                    {
                        op.WriteLine( "Spawner 0x1F13 (Spawn={0}; Count={1}; HomeRange={2})", ( (Spawner) item ).CreaturesName[ 0 ], ( (Spawner) item ).Count, ( (Spawner) item ).HomeRange );
                        op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
                        op.WriteLine( "" );
                    }
                    else if( item is LocalizedSign )
                    {
                        op.WriteLine( "LocalizedSign {0} (LabelNumber={1})", ( item ).ItemID, ( item ).LabelNumber );
                        op.WriteLine( "{0} {1} {2}", item.X, item.Y, item.Z );
                        op.WriteLine( "" );
                    }
                    else
                    {
                        /* How would Jack the Ripper said: divided into parts!
                                             *
                                             * 1.- Some items are statics other are not. So lets get the "real" item name, that is
                                             *     something like: 0x40098345 "LampPost1". The real hame has hex number plus a
                                             *     name inside quotes. So lets remove the 12 first caracters fromk the beginning
                                             *     (the hex number plus the space and the first quote before the name). Now we just
                                             *     need to remove the last caracter (the last quote) to get our "real name".
                                             *     The end is just: LampPost1, without quotes.
                                             */

                        string itemname = item.ToString();
                        itemname = itemname.Remove( 0, 12 ); //remove 12 caracters from the beginning of the string
                        itemname = itemname.Remove( itemname.Length - 1 ); // remove the last caracter (the ")

                        /*
                                             * 2.- Some items are Addons. Addons are collections of static items, called
                                             *     AddonComponent. We don't need the AddonComponent, but just the Addon item.
                                             *     The "real name" of the Addon will have an ItemID equal to 1. But this is not good
                                             *     for us, because if the configuration file has, example:
                                             *
                                             *		StoneFountainAddon 1
                                             *		1437 1678 10
                                             *     
                                             *     The [decorate command will place the Addon in the right place, but will forget
                                             *     one static item of that collection: the one at coordinates 1437 1678 10.
                                             *
                                             *     So lets change the ItemID from 1 to 0, that will generate the Addon correctly.
                                             */

                        int itemid = item.ItemID;

                        if( itemid == 1 )
                        {
                            itemid = 0; // if it is Addon "main", change the ID to 0, because 1 forget one static
                        }

                        string itemidhex = itemid.ToString( "X" );

                        /*
                                             * 3.- Now lets begin the work. Decoration files starts with an item name plus a number
                                             *     (ItemID) in the same line. In the line bellow, there are all events of that
                                             *     exactly item translated in coordinates. We will simplify, creating a string that
                                             *     sums item "real name" and ItemID. Plus a string that sums all the coordinates
                                             *     X, Y and Z in one line string.
                                             */

                        string hexConstruct = " 0x";

                        if( itemidhex.Length == 1 )
                        {
                            hexConstruct = " 0x000";
                        }
                        else if( itemidhex.Length == 2 )
                        {
                            hexConstruct = " 0x00";
                        }
                        else if( itemidhex.Length == 3 )
                        {
                            hexConstruct = " 0x0";
                        }

                        string namePlusID = itemname + hexConstruct + itemidhex;
                        string coord = ( item.X ) + " " + ( item.Y ) + " " + ( item.Z );

                        /*
                                             * 4.- Lets customize the NamePlusID and Coord strings, because some decorations has
                                             *     custom Names and Hues (custom properties).
                                             */

                        if( item.Name == null && item.Hue != 0 )
                        {
                            namePlusID = namePlusID + " (Hue=" + item.Hue + ")";
                        }

                        else if( item.Hue == 0 && item.Name != null )
                        {
                            namePlusID = namePlusID + " (Name=" + item.Name + ")";
                        }

                        else if( item.Name != null && item.Hue != 0 )
                        {
                            namePlusID = namePlusID + " (Name=" + item.Name + "; Hue=" + item.Hue + ")";
                        }

                        else if( item is BaseBeverage && item.Hue == 0 && item.Name == null )
                        {
                            string sContent = " (Content=" + ( (BaseBeverage) item ).Content + ")";
                            namePlusID = namePlusID + sContent;
                        }

                        /*
                                             * 5.- Now, the main job. Above, we created a Dictionary, that holds collections of
                                             *     Keys and values. Each item in a decoration file has a Key (NamePlusID string)
                                             *     plus a lot of Values (all the coordinates where that item appear). Because of
                                             *     it we create a Dictionary of String plus ArrayList as value!
                                             *     The code bellow will add the NamePlusID (the key) to the Dictionary if that
                                             *     key was not added yet. If was, it will add the new coordinates to the
                                             *     ArrayList of values, and update the key with more one coordinate!
                                             */

                        if( dicOfItemIDs.ContainsKey( namePlusID ) )
                        {
                            List< string > coordXYZupdated = dicOfItemIDs[ namePlusID ];
                            coordXYZupdated.Add( coord );
                            dicOfItemIDs.Remove( namePlusID );
                            dicOfItemIDs.Add( namePlusID, coordXYZupdated );
                        }
                        else
                        {
                            if( itemname != "AddonComponent" && itemname != "InternalItem" ) // ignore AddonComponent and InternalItem
                            {
                                List< string > coordXYZ = new List< string >();
                                coordXYZ.Add( coord );
                                dicOfItemIDs.Add( namePlusID, coordXYZ );
                            }
                        }
                    }
                }

                /*
                             * 6.- Final job. Lets analyze the Dictionary and write id up in
                             *     the configuration file.
                             */

                foreach ( KeyValuePair< string, List< string > > pair in dicOfItemIDs )
                {
                    op.WriteLine( "{0}", pair.Key );

                    foreach ( string elementInArray in dicOfItemIDs[ pair.Key ] )
                    {
                        op.WriteLine( elementInArray );
                    }

                    op.WriteLine( "" );
                }

                from.SendMessage( String.Format( "You exported {0} Statics from this facet.", list.Count ) );
            }
        }
    }
}