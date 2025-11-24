using System;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

namespace Midgard.Engines.AuctionSystem
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getAuctionStatus
            Core.Register( "getAuctionStatus", new MyXmlEventHandler( GetAuctionStatusOnCommand ), null );
        }

        public static void GetAuctionStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetAuctionStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from entry in AuctionSystem.Auctions
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