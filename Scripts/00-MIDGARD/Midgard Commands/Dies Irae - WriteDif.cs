using System;
using Server.Commands.Generic;
using Server.Items;
using System.IO;
using Server.Multis;

namespace Server.Commands
{
    public class WriteItems
    {
        public static void Initialize()
        {
            CommandSystem.Register( "WriteItems", AccessLevel.GameMaster, new CommandEventHandler( WriteItems_OnCommand ) );
        }

        [Usage( "WriteItems" )]
        [Description( "Writes a targeted area of dynamic items to dif file." )]
        public static void WriteItems_OnCommand( CommandEventArgs e )
        {
            BoundingBoxPicker.Begin( e.Mobile, new BoundingBoxCallback( WriteItems_OnCommand ), null );
        }

        private static void WriteItems_OnCommand( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            Map m = from.Map;

            FileStream stream = new FileStream( "worldfile.dif", FileMode.Create );
            BinaryWriter br = new BinaryWriter( stream );

            br.Seek( 0, SeekOrigin.Begin );

            string id = "CP#1";
            br.Write( id.ToCharArray(), 0, 4 );
            br.Write( (int)0 );
            br.Write( (int)100 );
            br.Write( (int)0 );
            br.Write( (int)0 );
            byte[] reserved = new byte[ 36 ];
            br.Write( reserved, 0, 36 );

            br.Seek( 100, SeekOrigin.Begin );

            // Rectangle2D rect = new Rectangle2D( start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1 );
            Rectangle2D rect = new Rectangle2D( 0, 0, from.Map.Width, from.Map.Height );

            IPooledEnumerable eable = m.GetItemsInBounds( rect );

            int count = 0;

            foreach( Item i in eable )
            {
                if( i is BaseMulti )
                {
                    MultiComponentList mcl = MultiData.Load( i.ItemID - 0x4000 );

                    for( int x = 0; x < mcl.Width; x++ )
                    {
                        for( int y = 0; y < mcl.Height; y++ )
                        {
                            if( mcl.Tiles[ x ][ y ].Length == 0 )
                                continue;

                            for( int t = 0; t < mcl.Tiles[ x ][ y ].Length; t++ )
                            {
                                int realx = ( i.X + mcl.Min.X ) + x;
                                int realy = ( i.Y + mcl.Min.Y ) + y;
                                br.Write( (byte)2 );
                                br.Write( (uint)count + 1 );
                                br.Write( (ushort)( mcl.Tiles[ x ][ y ][ t ].ID - 0x4000 ) );
                                br.Write( (byte)( realx % 8 ) );
                                br.Write( (byte)( realy % 8 ) );
                                br.Write( (ushort)( realx / 8 ) );
                                br.Write( (ushort)( realy / 8 ) );
                                br.Write( (short)( i.Z + mcl.Tiles[ x ][ y ][ t ].Z ) );
                                count++;
                                br.Seek( 100 + ( count * 15 ), SeekOrigin.Begin );
                            }
                        }
                    }

                    /*
                    if( i is HouseFoundation )
                    {
                        DesignState designState = ( (HouseFoundation)i ).CurrentState;

                        if( designState != null )
                        {
                            MultiComponentList components = designState.Components;

                            if( components != null )
                            {
                                for( int x = 0; x < components.Width; x++ )
                                {
                                    for( int y = 0; y < components.Height; y++ )
                                    {
                                        if( components.Tiles[ x ][ y ].Length == 0 )
                                            continue;

                                        for( int t = 0; t < components.Tiles[ x ][ y ].Length; t++ )
                                        {
                                            int realx = ( i.X + components.Min.X ) + x;
                                            int realy = ( i.Y + components.Min.Y ) + y;

                                            br.Write( (byte)2 );
                                            br.Write( (uint)count + 1 );
                                            br.Write( (ushort)( components.Tiles[ x ][ y ][ t ].ID - 0x4000 ) );
                                            br.Write( (byte)( realx % 8 ) );
                                            br.Write( (byte)( realy % 8 ) );
                                            br.Write( (ushort)( realx / 8 ) );
                                            br.Write( (ushort)( realy / 8 ) );
                                            br.Write( (short)( i.Z + components.Tiles[ x ][ y ][ t ].Z ) );
                                            count++;

                                            br.Seek( 100 + ( count * 15 ), SeekOrigin.Begin );
                                        }
                                    }
                                }
                            }
                        }
                     }
                     */
                }
                else if( i.Visible )
                {
                    br.Write( (byte)2 );
                    br.Write( (uint)count + 1 );
                    br.Write( (ushort)i.ItemID );
                    br.Write( (byte)( i.X % 8 ) );
                    br.Write( (byte)( i.Y % 8 ) );
                    br.Write( (ushort)( i.X / 8 ) );
                    br.Write( (ushort)( i.Y / 8 ) );
                    br.Write( (short)i.Z );
                    count++;
                    br.Seek( 100 + ( count * 15 ), SeekOrigin.Begin );
                }
            }

            eable.Free();

            br.Seek( 4, SeekOrigin.Begin );
            br.Write( (uint)count );

            br.Close();
            stream.Close();
        }
    }
}