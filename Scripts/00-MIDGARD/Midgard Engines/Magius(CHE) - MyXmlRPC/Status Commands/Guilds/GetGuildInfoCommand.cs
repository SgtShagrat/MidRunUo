/***************************************************************************
 *                               GetGuildInfoCommand.cs
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

using Midgard.Engines.MyMidgard;

using Server.Guilds;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetGuildInfoCommand
    {
        public static void RegisterCommands()
        {
            Core.Register( "getGuildInfo", new MyXmlEventHandler( GetGuildInfoOnCommand ), null );
        }

        public static void GetGuildInfoOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetGuildInfo command called..." );

            e.Exitcode = -1;

            string idString = Utility.SafeGetKey( e.Args, "id" );
            if( string.IsNullOrEmpty( idString ) )
                throw new ArgumentNullException( "id" );

            int id = -1;
            if( !int.TryParse( idString, out id ) )
                throw new ArgumentNullException( "id" );

            try
            {
                BaseGuild g = BaseGuild.Find( id );
                if( g == null )
                    throw new ArgumentNullException( "guild" );

                e.CustomResultTree.Add( GetGuildInfoXml( g ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static XElement GetGuildInfoXml( BaseGuild baseGuild )
        {
            Guild guild = baseGuild as Guild;
            if( guild == null )
                return null;

            var guildType = "Standard";
            switch( guild.Type )
            {
                case GuildType.Chaos:
                    guildType = "Chaos";
                    break;
                case GuildType.Order:
                    guildType = "Order";
                    break;
            }

            try
            {
                new XElement( "notice", new XElement( "guild",
                                                      new XAttribute( "id", guild.Id.ToString() ),
                                                      new XAttribute( "name", Utility.SafeString( guild.Name ) ),
                                                      new XAttribute( "abv", guild.Abbreviation == "none" ? string.Empty : "'" + Utility.SafeString( guild.Abbreviation ) + "'" ),
                                                      new XAttribute( "web", guild.Website == null ? string.Empty : "'" + Utility.SafeString( guild.Website ) + "'" ),
                                                      new XAttribute( "char", guild.Charter == null ? string.Empty : "'" + Utility.SafeString( guild.Charter ) + "'" ),
                                                      new XAttribute( "type", guildType ),
                                                      new XAttribute( "enemiescount", guild.Enemies.Count.ToString() ),
                                                      new XAttribute( "memberscount", guild.Members.Count.ToString() ),
                                                      new XAttribute( "leader", guild.Leader.Serial.Value.ToString() ),
                                                      new XElement( "members",
                                                                    from m in guild.Members
                                                                    where m != null
                                                                    select new XElement( "serial", m.Serial.Value.ToString() ) ) ) );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return null;
        }
    }
}