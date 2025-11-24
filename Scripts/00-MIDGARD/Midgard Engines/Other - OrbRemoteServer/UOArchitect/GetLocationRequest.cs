using Midgard.Engines.OrbRemoteServer;

using OrbServerSDK;

using Server;

using UOArchitectInterface;

namespace Midgard.Engines.UOArchitect
{
    public class GetLocationRequest : BaseOrbToolRequest
    {
        public static void Initialize()
        {
            OrbServer.Register( "UOAR_GetLocation", typeof( GetLocationRequest ), AccessLevel.GameMaster, true );
        }

        public override void OnRequest( OrbClientInfo clientInfo, OrbRequestArgs args )
        {
            FindOnlineMobile( clientInfo );

            if( !IsOnline )
            {
                SendResponse( null );
                return;
            }

            UoarObjectTarget target = new UoarObjectTarget();
            target.OnTargetObject += new UoarObjectTarget.TargetObjectEvent( OnTarget );
            target.OnCancelled += new UoarObjectTarget.TargetCancelEvent( OnCancelTarget );

            Mobile.SendMessage( "Target an object" );
            Mobile.Target = target;
        }

        private void OnTarget( object targeted )
        {
            if( targeted != null )
            {
                IPoint3D location = (IPoint3D)targeted;
                SendResponse( new GetLocationResp( location.X, location.Y, location.Z ) );
            }
            else
            {
                SendResponse( null );
            }
        }

        private void OnCancelTarget()
        {
            SendResponse( null );
        }
    }
}