using System.Collections.Generic;
using OrbServerSDK;

using Server;
using Midgard.Engines.OrbRemoteServer;
using Server.Items;
using Server.Multis;
using UOArchitectInterface;

namespace Midgard.Engines.UOArchitect
{
    public class SelectItemsRequest : BaseOrbToolRequest
    {
        private List<int> m_ItemSerials = new List<int>();
        private SelectItemsRequestArgs m_Args = null;

        public static void Initialize()
        {
            OrbServer.Register( "UOAR_SelectItems", typeof( SelectItemsRequest ), AccessLevel.GameMaster, true );
        }

        public override void OnRequest( OrbClientInfo clientInfo, OrbRequestArgs reqArgs )
        {
            FindOnlineMobile( clientInfo );

            if( reqArgs == null || !( reqArgs is SelectItemsRequestArgs ) || !IsOnline )
                SendResponse( null );

            m_Args = (SelectItemsRequestArgs)reqArgs;

            if( m_Args != null )
                if( m_Args.SelectType == SelectTypes.Area )
                {
                    BoundingBoxPickerEx picker = new BoundingBoxPickerEx();
                    picker.OnCancelled += new BoundingBoxExCancelled( OnTargetCancelled );
                    picker.Begin( Mobile, new BoundingBoxCallback( BoundingBox_Callback ), null );
                }
                else
                {
                    UoarObjectTarget target = new UoarObjectTarget();
                    target.OnCancelled += new UoarObjectTarget.TargetCancelEvent( OnTargetCancelled );
                    target.OnTargetObject += new UoarObjectTarget.TargetObjectEvent( OnTargetObject );

                    Mobile.SendMessage( "Target the first item you want to select." );
                    // send the target to the char
                    Mobile.Target = target;
                }
        }

        private void BoundingBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            Utility.FixPoints( ref start, ref end );
            Rectangle2D rect = new Rectangle2D( start, end );

            for( int x = 0; x <= rect.Width; ++x )
            {
                for( int y = 0; y <= rect.Height; ++y )
                {
                    int tileX = rect.Start.X + x;
                    int tileY = rect.Start.Y + y;

                    Sector sector = map.GetSector( tileX, tileY );

                    for( int i = 0; i < sector.Items.Count; ++i )
                    {
                        Item item = (Item)sector.Items[ i ];

                        if( m_Args.UseMinZ && item.Z < m_Args.MinZ )
                            continue;
                        else if( m_Args.UseMaxZ && item.Z > m_Args.MaxZ )
                            continue;

                        if( item.Visible && item.X == tileX && item.Y == tileY &&
                            !( ( item is BaseMulti ) || ( item is HouseSign ) ) )
                        {
                            m_ItemSerials.Add( item.Serial.Value );
                        }
                    }
                }
            }

            if( m_ItemSerials.Count > 0 )
                SendResponse( new SelectItemsResponse( m_ItemSerials.ToArray() ) );
            else
                SendResponse( null );
        }

        private void OnTargetCancelled()
        {
            if( m_ItemSerials.Count > 0 )
            {
                SendResponse( new SelectItemsResponse( m_ItemSerials.ToArray() ) );
            }
            else
            {
                SendResponse( null );
            }
        }

        private void OnTargetObject( object obj )
        {
            if( ( obj is Item ) && !( ( obj is BaseMulti ) || ( obj is HouseSign ) ) )
            {
                int serial = ( obj as Item ).Serial.Value;

                if( m_ItemSerials.IndexOf( serial ) == -1 )
                {
                    // add the item's serial # to the ArrayList
                    m_ItemSerials.Add( ( (Item)obj ).Serial.Value );
                }
            }
            else
            {
                Mobile.SendMessage( "That object is not valid for this selection." );
            }

            if( m_Args.Multiple )
            {
                UoarObjectTarget target = new UoarObjectTarget();
                target.OnCancelled += new UoarObjectTarget.TargetCancelEvent( OnTargetCancelled );
                target.OnTargetObject += new UoarObjectTarget.TargetObjectEvent( OnTargetObject );

                Mobile.SendMessage( "Select another item to add it to your selection or press ESC to finish." );
                // send the target to the char
                Mobile.Target = target;
            }
            else
            {
                OnTargetCancelled();
            }
        }
    }
}