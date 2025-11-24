using System.Collections.Generic;
using OrbServerSDK;

using Server;
using Midgard.Engines.OrbRemoteServer;
using Server.Items;
using Server.Multis;
using UOArchitectInterface;

namespace Midgard.Engines.UOArchitect
{
    public class ExtractItemsRequest : BaseOrbToolRequest
    {
        private BoundingBoxPickerEx m_Picker;
        private Rect2DCol m_Rects = new Rect2DCol();
        private Map m_Map;
        private DesignItemCol m_Items = new DesignItemCol();
        private ExtractRequestArgs m_Args;
        private List<int> m_ExtractedMultiIds = new List<int>();

        public static void Initialize()
        {
            OrbServer.Register( "UOAR_ExtractDesign", typeof( ExtractItemsRequest ), AccessLevel.GameMaster, true );
        }

        public override void OnRequest( OrbClientInfo client, OrbRequestArgs args )
        {
            FindOnlineMobile( client );

            if( args == null )
                SendResponse( null );
            else if( !( args is ExtractRequestArgs ) )
                SendResponse( null );
            else if( !IsOnline )
                SendResponse( null );

            m_Args = args as ExtractRequestArgs;

            if( m_Args != null )
                if( m_Args.ItemSerials == null )
                {
                    m_Picker = new BoundingBoxPickerEx();
                    m_Picker.OnCancelled += new BoundingBoxExCancelled( OnTargetCancelled );
                    m_Picker.Begin( Mobile, new BoundingBoxCallback( BoundingBox_Callback ), null );
                }
                else
                {
                    ExtractItems( m_Args.ItemSerials );
                }
        }

        private void BoundingBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            Utility.FixPoints( ref start, ref end );
            Rectangle2D rect = new Rectangle2D( start, end );
            m_Map = map;

            m_Rects.Add( new Rect2D( rect.Start.X, rect.Start.Y, rect.Width, rect.Height ) );

            if( m_Args.MultipleRects )
            {
                m_Picker.Begin( Mobile, new BoundingBoxCallback( BoundingBox_Callback ), null );
            }
            else
            {
                ExtractItems();
            }
        }

        private void ExtractItems( int[] itemSerials )
        {
            for( int i = 0; i < itemSerials.Length; ++i )
            {
                Item item = World.FindItem( itemSerials[ i ] );

                if( item != null )
                {
                    DesignItem designItem = new DesignItem();
                    designItem.ItemID = (short)item.ItemID;
                    designItem.X = item.X;
                    designItem.Y = item.Y;
                    designItem.Z = item.Z;
                    designItem.Hue = (short)item.Hue;

                    m_Items.Add( designItem );
                }
            }

            ExtractResponse resp;

            if( m_Items.Count > 0 )
                resp = new ExtractResponse( m_Items );
            else
                resp = null;

            SendResponse( resp );
        }

        private void ExtractItems()
        {
            foreach( Rect2D rect in m_Rects )
            {
                for( int x = 0; x <= rect.Width; ++x )
                {
                    for( int y = 0; y <= rect.Height; ++y )
                    {
                        int tileX = rect.TopX + x;
                        int tileY = rect.TopY + y;

                        Sector sector = m_Map.GetSector( tileX, tileY );

                        if( m_Args.NonStatic || m_Args.Static )
                        {
                            for( int i = 0; i < sector.Items.Count; ++i )
                            {
                                Item item = sector.Items[ i ];

                                if( !item.Visible )
                                    continue;
                                else if( ( !m_Args.NonStatic ) && !( item is Static ) )
                                    continue;
                                else if( ( !m_Args.Static ) && ( item is Static ) )
                                    continue;
                                else if( m_Args.MinZSet && item.Z < m_Args.MinZ )
                                    continue;
                                else if( m_Args.MaxZSet && item.Z > m_Args.MaxZ )
                                    continue;

                                int hue = 0;

                                if( m_Args.ExtractHues )
                                    hue = item.Hue;

                                if( item.X == tileX && item.Y == tileY && !( ( item is BaseMulti ) || ( item is HouseSign ) ) )
                                {
                                    DesignItem designItem = new DesignItem();
                                    designItem.ItemID = (short)item.ItemID;
                                    designItem.X = item.X;
                                    designItem.Y = item.Y;
                                    designItem.Z = item.Z;
                                    designItem.Hue = (short)hue;

                                    m_Items.Add( designItem );
                                }

                                // extract multi
                                if( item is HouseFoundation )
                                {
                                    HouseFoundation house = (HouseFoundation)item;

                                    if( m_ExtractedMultiIds.IndexOf( house.Serial.Value ) == -1 )
                                        ExtractCustomMulti( house );
                                }
                            }
                        }
                    }
                }
            }

            ExtractResponse response = new ExtractResponse( m_Items );

            if( m_Args.Frozen )
            {
                response.Rects = m_Rects;
                response.Map = m_Map.Name;
            }

            // send response back to the UOAR tool
            SendResponse( response );
        }

        private void ExtractCustomMulti( BaseHouse house )
        {
            m_ExtractedMultiIds.Add( house.Serial.Value );

            for( int x = 0; x < house.Components.Width; ++x )
            {
                for( int y = 0; y < house.Components.Height; ++y )
                {
                    Tile[] tiles = house.Components.Tiles[ x ][ y ];

                    for( int i = 0; i < tiles.Length; ++i )
                    {
                        DesignItem designItem = new DesignItem();
                        designItem.ItemID = (short)( tiles[ i ].ID ^ 0x4000 );

                        designItem.X = x + house.Sign.Location.X;
                        designItem.Y = ( y + house.Sign.Location.Y ) - ( house.Components.Height - 1 );
                        designItem.Z = house.Location.Z + tiles[ i ].Z;

                        m_Items.Add( designItem );
                    }
                }
            }

            DesignItem sign = new DesignItem();
            sign.ItemID = (short)( house.Sign.ItemID );

            sign.X = house.Sign.Location.X;
            sign.Y = house.Sign.Location.Y;
            sign.Z = house.Sign.Location.Z;

            m_Items.Add( sign );
        }

        private void OnTargetCancelled()
        {
            if( m_Rects.Count > 0 )
                ExtractItems();
            else
                SendResponse( null );
        }
    }
}