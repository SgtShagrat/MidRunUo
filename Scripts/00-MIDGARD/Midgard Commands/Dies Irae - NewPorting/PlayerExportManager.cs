using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class PlayerExportManager
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "ExportPlayers", AccessLevel.Developer, new CommandEventHandler( ExportPlayer_OnCommand ) );

            // Snapshot();
        }

        [Usage( "ExportPlayers" )]
        [Description( "Generates data.xml files containing all playes export data" )]
        public static void ExportPlayer_OnCommand( CommandEventArgs e )
        {
            World.Broadcast( 0x35, true, "Exporting player data. Please wait..." );

            NetState.FlushAll();
            NetState.Pause();

            DateTime startTime = DateTime.Now;
            Snapshot();
            DateTime endTime = DateTime.Now;

            NetState.Resume();

            World.Broadcast( 0x35, true, "Process completed. The entire process took {0:F1} seconds.", ( endTime - startTime ).TotalSeconds );
        }

        private static string ExportPath = Path.Combine( "Porting", "Data.xml" );

        private static void Snapshot()
        {
            XDocument doc = new XDocument(
                new XDeclaration( "1.0", "utf-8", "yes" ),
                new XElement( "data",
                        from Account a in BuildExportList()
                        where a != null
                        select ( new XElement( "account",
                                                       new XAttribute( "user", a.Username ),
                                                       new XAttribute( "created", SafeString( a.Created ) ),
                                                       new XAttribute( "lastLogin", SafeString( a.LastLogin ) ),
                                                       new XAttribute( "totalGameTime", XmlConvert.ToString( a.TotalGameTime ) ),
                                                       new XElement( "chars", from Midgard2PlayerMobile m in GetChars( a )
                                                                              where ( m != null && !m.Deleted )
                                                                              select ExportPg( m ) ) ) ) ) );

            ScriptCompiler.EnsureDirectory( "Porting" );

            doc.Save( ExportPath );
        }

        private static string SafeString( DateTime dateTime )
        {
            return XmlConvert.ToString( dateTime, XmlDateTimeSerializationMode.Local );
        }

        private static List<Midgard2PlayerMobile> GetChars( IAccount account )
        {
            List<Midgard2PlayerMobile> list = new List<Midgard2PlayerMobile>();

            for( int i = 0; i < account.Length; i++ )
            {
                Midgard2PlayerMobile m = account[ i ] as Midgard2PlayerMobile;
                if( m != null && !m.Deleted )
                    list.Add( m );
            }

            return list;
        }

        private static XElement ExportPg( Midgard2PlayerMobile player )
        {
            var skills = new List<Skill>();
            for( int i = 0; i < player.Skills.Length; i++ )
            {
                skills.Add( player.Skills[ i ] );
            }

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
                                       new XElement( "name", player.Name ),
                                       new XElement( "race", player.Race.Name ),
                                       new XElement( "skills", from s in skills
                                                               where s.BaseFixedPoint > 0.0
                                                               select new XElement( "skill", new XAttribute( "id", s.SkillID ), new XAttribute( "value", s.BaseFixedPoint.ToString() ) ) ) );
        }

        private static List<Account> BuildExportList()
        {
            List<Account> list = new List<Account>();

            foreach( IAccount account in Accounts.GetAccounts() )
            {
                if( account is Account )
                    list.Add( account as Account );
            }

            list.Sort( AccountComparer.Instance );

            return list;
        }

        private class AccountComparer : IComparer<Account>
        {
            public static readonly IComparer<Account> Instance = new AccountComparer();

            public int Compare( Account x, Account y )
            {
                if( x == null && y == null )
                    return 0;
                else if( x == null )
                    return -1;
                else if( y == null )
                    return 1;

                return Insensitive.Compare( x.Username, y.Username );
            }
        }
    }
}