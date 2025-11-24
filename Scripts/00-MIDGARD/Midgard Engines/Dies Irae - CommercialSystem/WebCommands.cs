using System;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.MyXmlRPC;

using Server;
using Server.Mobiles;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.CommercialSystem
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getVendorStatus
            MyXmlRPC.Core.Register( "getVendorStatus", new MyXmlEventHandler( GetVendorStatusOnCommand ), null );
        }

        public static void GetVendorStatusOnCommand( MyXmlEventArgs e )
        {
            if( MyXmlRPC.Core.Debug )
                MyXmlRPC.Core.Pkg.LogInfoLine( "GetVendorStatus command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from pv in PlayerVendor.AllVendors
                                                                where ( pv != null && !pv.Deleted && pv.Owner != null )
                                                                select new XElement( "vendor",
                                                                                    new XAttribute( "name", Utility.SafeString( pv.Name ?? "" ) ),
                                                                                    new XAttribute( "location", pv.Location.ToString() ),
                                                                                    new XAttribute( "town", Utility.SafeString( GetTownFromlocation( pv.Map, pv.Location ) ) ),
                                                                                    new XAttribute( "owner", Utility.SafeString( pv.Owner.Name ?? "" ) ),
                                                                                    new XAttribute( "commercial", Utility.SafeString( pv.CommercialMessage ?? "" ) ),
                                                                                    new XAttribute( "gold", pv.HoldGold.ToString() ),
                                                                                    new XAttribute( "visits", pv.Visits.ToString() ) ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static string GetTownFromlocation( Map map, Point3D location )
        {
            TownSystem sys = TownSystem.Find( location, map );
            return sys != null ? (string)sys.Definition.TownName : string.Empty;
        }
    }
}