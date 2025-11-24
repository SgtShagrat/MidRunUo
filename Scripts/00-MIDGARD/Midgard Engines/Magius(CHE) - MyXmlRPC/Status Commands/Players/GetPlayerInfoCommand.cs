/***************************************************************************
 *                               GetPlayerInfoCommand.cs
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

using Server;
using Server.Misc;
using Server.Mobiles;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.MyXmlRPC
{
    public class GetPlayerInfoCommand
    {
        public static void RegisterCommands()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getPlayerInfo&xargskills="1"&xargitems="1"
            Core.Register( "getPlayerInfo", new MyXmlEventHandler( GetPlayerInfoOnCommand ), null );
        }

        public static void GetPlayerInfoOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
            {
                Core.Pkg.LogInfoLine( "GetPlayerInfoOnCommand command called..." );
            }

            e.Exitcode = -1;

            string idString = Utility.SafeGetKey( e.Args, "id" );
            if( string.IsNullOrEmpty( idString ) )
            {
                throw new ArgumentNullException( "id" );
            }

            int id = -1;
            if( !int.TryParse( idString, out id ) )
            {
                throw new ArgumentNullException( "id" );
            }

            Serial s = id;
            if( !s.IsValid || !s.IsMobile )
            {
                throw new ArgumentNullException( "id" );
            }

            try
            {
                Mobile mobile = World.FindMobile( s );
                if( mobile == null )
                {
                    throw new ArgumentNullException( "mobile" );
                }

                e.CustomResultTree.Add( GetPlayerInfoXml( mobile, e.Args.ContainsKey( "skills" ), e.Args.ContainsKey( "items" ) ) );
                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static XElement GetPlayerInfoXml( Mobile mob, bool skills, bool items )
        {
            var noticeElement = new XElement( "notice", new XElement( "player",
                                                                    new XAttribute( "id", mob.Serial.Value.ToString() ),
                                                                    new XAttribute( "account", mob.Account.Username ) ) );

            XElement playerElement = noticeElement.Descendants( "player" ).Last();

            AppendPersonalInfoXml( playerElement, mob );

            if( skills )
            {
                AppendPlayerSkillsXml( playerElement, mob );
            }

            if( items )
            {
                AppendPlayerItemsXml( playerElement, mob );
            }

            return noticeElement;
        }

        private static void AppendPersonalInfoXml( XContainer element, Mobile mob )
        {
            string guildTitle = mob.GuildTitle;
            if( guildTitle == null || ( guildTitle = guildTitle.Trim() ).Length == 0 )
            {
                guildTitle = string.Empty;
            }
            else
            {
                guildTitle = Utility.SafeString( guildTitle );
            }

            string notoTitle = Utility.SafeString( Titles.ComputeTitle( null, mob ) );
            string female = ( mob.Female ? "1" : "0" );
            bool pubBool = ( mob is PlayerMobile ) && ( ( (PlayerMobile)mob ).PublicMyRunUO );
            string pubString = ( pubBool ? "1" : "0" );
            string guildId = ( mob.Guild == null ? string.Empty : mob.Guild.Id.ToString() );

            element.Add( new XElement( "infos",
                                     new XAttribute( "name", Utility.SafeString( mob.Name ) ),
                                     new XAttribute( "str", mob.RawStr.ToString() ),
                                     new XAttribute( "dex", mob.RawDex.ToString() ),
                                     new XAttribute( "int", mob.RawInt.ToString() ),
                                     new XAttribute( "fem", female ),
                                     new XAttribute( "kil", mob.Kills.ToString() ),
                                     new XAttribute( "gid", guildId ),
                                     new XAttribute( "gtit", guildTitle ),
                                     new XAttribute( "ntit", notoTitle ),
                                     new XAttribute( "race", mob.Race.Name ),
                                     new XAttribute( "body", mob.BodyValue ),
                                     new XAttribute( "hue", mob.Hue.ToString() ),
                                     new XAttribute( "pub", pubString ) ) );
        }

        private static void AppendPlayerSkillsXml( XContainer element, Mobile mob )
        {
            var skills = new List<Skill>();
            for( int i = 0; i < mob.Skills.Length; i++ )
            {
                skills.Add( mob.Skills[ i ] );
            }

            element.Add( new XElement( "skills", from s in skills
                                                 where s.BaseFixedPoint > 0.0
                                                 select new XElement( "skill", new XAttribute( "id", s.SkillID ), new XAttribute( "value", s.BaseFixedPoint.ToString() ) ) ) );
        }

        private static void AppendPlayerItemsXml( XContainer element, Mobile mob )
        {
            var items = new List<Item>( mob.Items );
            items.Sort( LayerComparer.Instance );

            var itemsElement = new XElement( "items" );

            int index = 0;

            bool hidePants = false;
            bool alive = mob.Alive;
            bool hideHair = !alive;

            foreach( Item item in items )
            {
                if( !LayerComparer.IsValid( item ) )
                {
                    break;
                }

                if( !alive && item.ItemID != 8270 )
                {
                    continue;
                }

                if( item.ItemID == 0x1411 || item.ItemID == 0x141A ) // plate legs
                {
                    hidePants = true;
                }
                else if( hidePants && item.Layer == Layer.Pants )
                {
                    continue;
                }

                if( !hideHair && item.Layer == Layer.Helm )
                {
                    hideHair = true;
                }

                AppendItemInfo( itemsElement, index++, item.ItemID, item.Hue );
            }

            if( mob.FacialHairItemID != 0 && alive )
            {
                AppendItemInfo( itemsElement, index++, mob.FacialHairItemID, mob.FacialHairHue );
            }

            if( mob.HairItemID != 0 && !hideHair )
            {
                AppendItemInfo( itemsElement, index++, mob.HairItemID, mob.HairHue );
            }

            element.Add( itemsElement );
        }

        private static void AppendItemInfo( XContainer element, int index, int itemID, int hue )
        {
            element.Add( new XElement( "item",
                                     new XAttribute( "index", index.ToString() ),
                                     new XAttribute( "id", itemID.ToString() ),
                                     new XAttribute( "hue", hue.ToString() ) ) );
        }
    }
}