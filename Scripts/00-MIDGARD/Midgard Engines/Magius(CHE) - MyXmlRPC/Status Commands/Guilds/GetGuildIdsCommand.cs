/***************************************************************************
 *                               Dies Irae - GetGuildIdsWebCommand.cs
 *
 *   begin                : 14 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Midgard.Engines.MyMidgard;

using Server.Guilds;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetGuildIdsCommand
    {
        public static void RegisterCommands()
        {
            Core.Register( "getGuildIds", new MyXmlEventHandler( GetGuildIdsOnCommand ), null );
        }

        public static void GetGuildIdsOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
            {
                Core.Pkg.LogInfoLine( "Get Guild Ids command called..." );
            }

            var list = new List<BaseGuild>( BaseGuild.List.Values );

            e.Exitcode = -1;

            try
            {
                e.CustomResultTree.Add( new XElement( "Notice", from guild in list
                                                                where guild != null
                                                                select new XElement( "id", guild.Id.ToString(),
                                                                                    new XAttribute( "name", Utility.SafeString( guild.Name ) ) ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }
    }
}