using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyMidgard;

using Server.Guilds;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetGuildStatusCommand
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getGuildsStatus
            Core.Register( "getGuildsStatus", new MyXmlEventHandler( GetGuildsStatusOnCommand ), null );
        }

        private static void GetGuildsStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
            {
                Core.Pkg.LogInfoLine( "Get Guilds List command called..." );
            }

            var list = new List<BaseGuild>( BaseGuild.List.Values );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "Notice", from guild in list
                                                                where guild != null
                                                                select new XElement( "guild",
                                                                                     new XAttribute( "name", Utility.SafeString( guild.Name ) ),
                                                                                     new XAttribute( "abbr", Utility.SafeString( guild.Abbreviation ) ),
                                                                                     new XAttribute( "type", guild.Type ),
                                                                                     new XAttribute( "members", ( (Guild)guild ).Members.Count ) ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}