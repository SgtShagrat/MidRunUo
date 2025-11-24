using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Server;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetPlayerSerialsCommand
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getPlayerSerials
            Core.Register( "getPlayerSerials", new MyXmlEventHandler( GetPlayerSerialsOnCommand ), null );
        }

        public static void GetPlayerSerialsOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "Get Player Serials command called..." );

            var list = new List<Mobile>();
            foreach( var mobile in World.Mobiles.Values )
            {
                if( mobile.Player )
                    list.Add( mobile );
            }

            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetPlayerSerials command called..." );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "Notice", from mob in list
                                                                where mob != null
                                                                select new XElement( "id", mob.Serial.Value,
                                                                                     new XAttribute( "name", MyMidgard.Utility.SafeString( mob.Name ) ) ) ) );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}