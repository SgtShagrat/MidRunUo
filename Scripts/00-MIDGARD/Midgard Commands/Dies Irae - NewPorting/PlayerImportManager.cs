using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Midgard.Commands;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Engines.XmlPoints;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class PlayerImportManager
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "ImportAccount", AccessLevel.Administrator, new CommandEventHandler( ImportAccount_OnCommand ) );
        }

        [Usage( "ImportAccount <user>" )]
        [Description( "Import a midgard 2 saved account into midgard third crown context" )]
        public static void ImportAccount_OnCommand( CommandEventArgs e )
        {
            string user = e.GetString( 0 );

            if( Accounts.GetAccount( user ) != null )
            {
                e.Mobile.SendMessage( "Operation failed. Account already exist on Midgard Third Crown." );
                return;
            }

            e.Mobile.SendMessage( "Importing player data. Please wait..." );

            DateTime startTime = DateTime.Now;
            bool success = ImportAccount( user );

            e.Mobile.SendMessage( "Operation {0}.", success ? "successed" : "failed" );

            if( !success )
                DeleteAccount( e.GetString( 0 ) );

            DateTime endTime = DateTime.Now;

            e.Mobile.SendMessage( "Process completed. The entire process took {0:F1} seconds.", ( endTime - startTime ).TotalSeconds );
        }

        private static bool ImportAccount( string user )
        {
            if( !File.Exists( ExportPath ) )
            {
                Console.WriteLine( "File {0} not found" );
                return false;
            }

            try
            {
                XDocument doc;
                using( var stream = new FileStream( ExportPath, FileMode.Open, FileAccess.Read ) )
                {
                    var reader = XmlReader.Create( stream );
                    doc = XDocument.Load( reader );
                }

                var query = from element in doc.Elements( "data" ).Elements( "account" )
                            let xAttribute = element.Attribute( "user" )
                            where xAttribute != null && Insensitive.Equals( xAttribute.Value, user )
                            select element;

                bool success = false;
                foreach( XElement element in query )
                {
                    if( Accounts.GetAccount( user ) != null )
                        continue;

                    Console.WriteLine( "Importing account: {0}", user );
                    success = ParseAccount( element );
                }

                return success;
            }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
            }

            return false;
        }

        private static readonly string ExportPath = Path.Combine( "Porting", "Data.xml" );
        private static readonly CityInfo StartLocation = new CityInfo( "Cove", "Centro", 2230, 1224, 0, Map.Felucca );

        private static void DeleteAccount( string user )
        {
            IAccount a = Accounts.GetAccount( user );
            if( a != null )
                a.Delete();
        }

        private static bool ParseAccount( XElement element )
        {
            /*
                select ( new XElement( "account",
                   new XAttribute( "user", a.Username ),
                   new XAttribute( "created", SafeString( a.Created ) ),
                   new XAttribute( "lastLogin", SafeString( a.LastLogin ) ),
                   new XAttribute( "totalGameTime", XmlConvert.ToString( a.TotalGameTime ) ),
                   new XElement( "chars", from Midgard2PlayerMobile m in GetChars( a )
                                          where ( m != null && !m.Deleted )
                                          select ExportPg( m ) ) ) ) ) );
             */

            var xAttribute = element.Attribute( "user" );
            if( xAttribute != null )
            {
                Account a = new Account( xAttribute.Value, "AAA", string.Empty );
                var attribute = element.Attribute( "created" );
                if( attribute != null )
                    a.Created = Utility.GetXMLDateTime( attribute.Value, DateTime.Now );
                var xAttribute1 = element.Attribute( "lastLogin" );
                if( xAttribute1 != null )
                    a.LastLogin = Utility.GetXMLDateTime( xAttribute1.Value, DateTime.Now );
                var attribute1 = element.Attribute( "totalGameTime" );
                if( attribute1 != null )
                    a.TotalGameTime = Utility.GetXMLTimeSpan( attribute1.Value, TimeSpan.Zero );

                List<Midgard2PlayerMobile> list = new List<Midgard2PlayerMobile>();
                foreach( XElement charElement in element.Elements( "chars" ).Elements( "char" ) )
                    list.Add( ParsePlayer( charElement ) );

                for( int i = 0; i < list.Count; i++ )
                    a[ i ] = list[ i ];

                a.AddTag( "PortedAccount", "0" );
            }

            return true;
        }

        private static Midgard2PlayerMobile ParsePlayer( XContainer element )
        {
            /*
            return new XElement( "char",
                                       new XElement( "bodyValue", player.BodyValue.ToString() ),
                                       new XElement( "creationTime", player.CreationTime.ToString() ),
                                       new XElement( "dex", player.Dex.ToString() ),
                                       new XElement( "female", player.Female.ToString() ),
                                       new XElement( "hairHue", player.HairHue.ToString() ),
                                       new XElement( "hairItemid", player.HairItemID.ToString() ),
                                       new XElement( "facialHairHue", player.FacialHairHue.ToString() ),
                                       new XElement( "facialHairItemid", player.FacialHairItemID.ToString() ),
                                       new XElement( "hue", player.Hue.ToString() ),
                                       new XElement( "rawStr", player.RawStr.ToString() ),
                                       new XElement( "rawDex", player.RawDex.ToString() ),
                                       new XElement( "rawInt", player.RawInt.ToString() ),
                                       new XElement( "rawName", player.RawName ),
                                       new XElement( "name", Utility.SafeString( player.Name ) ),
                                       new XElement( "skills", from s in GetSkills( player )
                                                               where s.BaseFixedPoint > 0.0
                                                               select new XElement( "skill_" + s.SkillID, s.BaseFixedPoint.ToString() ) ) );
            */

            Midgard2PlayerMobile player = new Midgard2PlayerMobile();

            player.Player = true;
            player.AccessLevel = AccessLevel.Player;
            player.Hunger = 20;
            player.Thirst = 20;

            player.Young = false;

            foreach( XElement el in element.Elements() )
            {
                try
                {
                    if( el.Name == "bodyValue" )
                        player.BodyValue = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "creationTime" )
                        player.CreationTime = Utility.GetXMLDateTime( el.Value.ToLower(), DateTime.Now );
                    else if( el.Name == "dex" )
                        player.Dex = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "female" )
                        player.Female = XmlConvert.ToBoolean( el.Value.ToLower() );
                    else if( el.Name == "race" )
                        player.Race = Race.Parse( el.Value );
                    else if( el.Name == "hairHue" )
                        player.HairHue = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "hairItemid" )
                        player.HairItemID = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "facialHairHue" )
                        player.FacialHairHue = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "facialHairItemid" )
                        player.FacialHairItemID = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "hue" )
                        player.Hue = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "rawStr" )
                        player.RawStr = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "rawDex" )
                        player.RawDex = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "rawInt" )
                        player.RawInt = XmlConvert.ToInt32( el.Value.ToLower() );
                    else if( el.Name == "rawName" )
                        player.Name = el.Value.ToLower();
                    else if( el.Name == "name" )
                        player.Name = el.Value;
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                    Console.WriteLine( "Element {0}: {1}", el.Name, el.Value );
                }
            }

            var xElement = element.Element( "skills" );
            if( xElement != null )
                foreach( XElement el in xElement.Elements( "skill" ) )
                {
                    var xAttribute = el.Attribute( "id" );
                    if( xAttribute == null )
                        continue;

                    var attribute = el.Attribute( "value" );
                    if( attribute != null )
                        player.Skills[ XmlConvert.ToInt32( xAttribute.Value ) ].BaseFixedPoint = XmlConvert.ToInt32( attribute.Value );
                }

            XmlAttach.AttachTo( player, new XmlPointsAttach() );

            PreAoSCharacterCreation.Dress( player );

            player.MoveToWorld( StartLocation.Location, StartLocation.Map );

            player.QuestDeltaTimeExpiration = TimeSpan.FromDays( 15 );

            Console.WriteLine( " - Character: {0} (serial={1})", player.Name, player.Serial );

            player.Map = Map.Internal;

            player.LogoutMap = Map.Felucca;
            player.LogoutLocation = player.Location;

            return player;
        }
    }
}