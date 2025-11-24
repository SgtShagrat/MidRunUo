using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyXmlRPC;

namespace Midgard.Engines.JailSystem
{
    public class WebCommands
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getJailStatus
            Core.Register( "getJailStatus", new MyXmlEventHandler( GetJailStatusOnCommand ), null );
        }

        public static void GetJailStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetjailStatus command called..." );

            e.Exitcode = -1;

            List<JailSystem> jails = new List<JailSystem>();
            foreach( JailSystem js in JailSystem.JailSystemList.Values )
            {
                if( js.ReleaseDate > DateTime.Now )
                    jails.Add( js );
            }

            try
            {
                e.CustomResultTree.Add( new XElement( "status", from js in jails
                                                                where js != null
                                                                select js.ToXElement() ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}