/***************************************************************************
 *                               Dies Irae - GetStatusCommand.cs
 *
 *   begin                : 14 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Linq;
using System.Xml.Linq;

using Server.Network;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetOnlineIdsCommand
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getOnlineIds
            Core.Register( "getOnlineIds", new MyXmlEventHandler( GetStatusOnCommand ), null );
        }

        public static void GetStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
            {
                Core.Pkg.LogInfoLine( "GetStatus command called..." );
            }

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from state in NetState.Instances
                                                                where state.Mobile != null
                                                                select new XElement( "id", state.Mobile.Serial.Value ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}