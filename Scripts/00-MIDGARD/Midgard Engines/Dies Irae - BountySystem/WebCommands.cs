using System;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

namespace Midgard.Engines.BountySystem
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getBountyStatus
            MyXmlRPC.Core.Register( "getBountyStatus", new MyXmlEventHandler( GetBountyStatusOnCommand ), null );
        }

        public static void GetBountyStatusOnCommand( MyXmlEventArgs e )
        {
            if( MyXmlRPC.Core.Debug )
                MyXmlRPC.Core.Pkg.LogInfoLine( "GetBountyStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from entry in Core.Entries
                                                                where ( entry != null && !entry.Expired )
                                                                select entry.ToXElement() ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}