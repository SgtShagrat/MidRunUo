using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

using Server;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.Races
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://93.63.153.178/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getRaceStatus
            MyXmlRPC.Core.Register( "getRaceStatus", new MyXmlEventHandler( GetRaceStatusOnCommand ), null );
        }

        public static void GetRaceStatusOnCommand( MyXmlEventArgs e )
        {
            if( MyXmlRPC.Core.Debug )
                MyXmlRPC.Core.Pkg.LogInfoLine( "GetRaceStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from m in BuildList()
                                                                where ( m != null && !m.Deleted )
                                                                select new XElement( "player", new XAttribute( "name", Utility.SafeString( m.Name ) ),
                                                                                    new XAttribute( "account", Utility.SafeString( m.Account.Username ) ),
                                                                                    new XAttribute( "creation", m.CreationTime.ToString() ),
                                                                                    new XAttribute( "race", m.Race.Name ) ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        public static List<Mobile> BuildList()
        {
            List<Mobile> mobs = new List<Mobile>();

            foreach( Mobile m in World.Mobiles.Values )
            {
                if( m.Player && m.Race != Race.Human )
                    mobs.Add( m );
            }

            return mobs;
        }
    }
}