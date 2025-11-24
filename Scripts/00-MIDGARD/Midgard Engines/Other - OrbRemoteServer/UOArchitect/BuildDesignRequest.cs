using OrbServerSDK;

using Server;
using Midgard.Engines.OrbRemoteServer;
using UOArchitectInterface;

namespace Midgard.Engines.UOArchitect
{
    public class BuildDesignRequest : BaseOrbToolRequest
    {
        private DesignItemCol m_Items;

        public DesignItemCol Items
        {
            get { return m_Items; }
        }

        public static void Initialize()
        {
            OrbServer.Register( "UOAR_BuildDesign", typeof( BuildDesignRequest ), AccessLevel.GameMaster, true );
        }

        public override void OnRequest( OrbClientInfo clientInfo, OrbRequestArgs args )
        {
            FindOnlineMobile( clientInfo );

            if( args == null )
                SendResponse( null );
            else if( !( args is BuildRequestArgs ) )
                SendResponse( null );
            else if( !IsOnline )
                SendResponse( null );

            m_Items = ( (BuildRequestArgs)args ).Items;

            Mobile.SendMessage( "Target the ground where you want to place the building" );
            Mobile.Target = new BuildDesignTarget( this );
        }
    }
}