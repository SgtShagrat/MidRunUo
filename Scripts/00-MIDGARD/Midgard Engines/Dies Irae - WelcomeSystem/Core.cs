/***************************************************************************
 *                               Dies Irae - Welcome.cs
 *                            ----------------------------
 *   begin                : 16 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

using Midgard.Engines.Packager;

using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;

namespace Midgard.Engines.WelcomeSystem
{
    public class Core
    {
        public static readonly bool OldSchoolRules = Server.Core.T2A;

        public static string Motd { get; internal set; }
        public static string[] Tips { get; internal set; }
        public static string[] TipsIta { get; internal set; }
        public static string News { get; set; }

        static Core()
        {
            Motd = string.Empty;
            News = string.Empty;
        }

        internal static void RegisterCommands()
        {
            if( Server.Core.AOS )
            {
                CommandSystem.Register( "AddNews", AccessLevel.Administrator, new CommandEventHandler( AddNews_OnCommand ) );
                CommandSystem.Register( "Notizie", AccessLevel.Player, new CommandEventHandler( WelcomeGump_OnCommand ) );
            }

            CommandSystem.Register( "MessaggioDelGiorno", AccessLevel.Player, new CommandEventHandler( MotdGump_OnCommand ) );
            CommandSystem.Register( "ChangeMotd", AccessLevel.Administrator, new CommandEventHandler( ChangeMotd_OnCommand ) );

            CommandSystem.Register( "Suggerimento", AccessLevel.Player, new CommandEventHandler( ShowRandomTip_OnCommand ) );
        }

        internal static void InitSystem()
        {
            EventSink.Login += new LoginEventHandler( OnLogin );

            ReadTips();
            ReadNews();
            ReadMotd();
        }

        private static void OnLogin( LoginEventArgs e )
        {
            if( !OldSchoolRules && !String.IsNullOrEmpty( News ) )
                e.Mobile.SendGump( new WelcomeGump() );
            else
            {
                if( e.Mobile is Midgard2PlayerMobile )
                {
                    Midgard2PlayerMobile m2Pm = (Midgard2PlayerMobile)( e.Mobile );

                    if( ( m2Pm.AccessLevel > AccessLevel.Player || TestCenter.Enabled ) && !e.Mobile.HasAnyGump() )
                        m2Pm.SendCustomScrollMessage( Motd );

                    string tip = GetRandomTip( m2Pm.TrueLanguage );

                    if( tip != null && !e.Mobile.HasAnyGump() )
                        ( (Midgard2PlayerMobile)( e.Mobile ) ).SendCustomScrollMessage( tip );
                }
            }
        }

        [Usage( "Suggerimento <abilita|disabilita>" )]
        [Description( "Mostra un messaggio di suggerimento scelto a caso" )]
        public static void ShowRandomTip_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 1 )
            {
                string command = e.GetString( 0 );

                if( !string.IsNullOrEmpty( command ) )
                {
                    Midgard2PlayerMobile player = (Midgard2PlayerMobile)e.Mobile;
                    if( player == null )
                        return;

                    switch (command)
                    {
                        case "abilita":
                            player.AcceptTips = true;
                            break;
                        case "disabilita":
                            player.AcceptTips = false;
                            break;
                        default: break;
                    }

                    player.SendMessage( player.AcceptTips
                                         ? "You will now receive tips of the day."
                                         : "You will no longer receive tips of the day." );
                }
            }
            if( e.Length == 0 && e.Mobile is Midgard2PlayerMobile )
            {
                string tip = GetRandomTip( e.Mobile.TrueLanguage );

                if( tip != null )
                    ( (Midgard2PlayerMobile)( e.Mobile ) ).SendCustomScrollMessage( tip );
                else
                    e.Mobile.SendMessage( "There is no message at this time." );
            }
            else
                e.Mobile.SendMessage( "Command Use: [Tip" );
        }

        [Usage( "Notizie" )]
        [Description( "Apre il gump delle notizie di Midgard" )]
        public static void WelcomeGump_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                e.Mobile.SendGump( new WelcomeGump() );
            else
                e.Mobile.SendMessage( "Command Use: [Notizie" );
        }

        [Usage( "MessaggioDelGiorno" )]
        [Description( "Apre il gump del messaggio del giorno di Midgard" )]
        public static void MotdGump_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 && e.Mobile is Midgard2PlayerMobile && !String.IsNullOrEmpty( Motd ) )
                ( (Midgard2PlayerMobile)( e.Mobile ) ).SendCustomScrollMessage( Motd );
            else
                e.Mobile.SendMessage( "Command Use: [MessaggioDelGiorno" );
        }

        [Usage( "AddNews" )]
        [Description( "Open a gump to add a news to Midgard Welcome Gump" )]
        public static void AddNews_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                e.Mobile.SendGump( new AddNewsGump() );
            else
                e.Mobile.SendMessage( "Command Use: [AddNews" );
        }

        [Usage( "ChangeMotd" )]
        [Description( "Open a gump to change message of the day" )]
        public static void ChangeMotd_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
                e.Mobile.SendGump( new ChangeMotdGump() );
            else
                e.Mobile.SendMessage( "Command Use: [ChangeMotd" );
        }

        private static void ReadTips()
        {
            Package pkg = Packager.Core.Singleton[ typeof( Config ) ];
            List<string> tips = new List<string>();
            List<string> tipsIta = new List<string>();

            XmlDocument doc = new XmlDocument();

            if( File.Exists( Config.TipsFilePath ) )
            {
                try
                {
                    doc.Load( Config.TipsFilePath );
                }
                catch( Exception ex )
                {
                    pkg.LogWarningLine( "Warning: error loading Tips.xml: {0}", ex );
                    return;
                }

                try
                {
                    XmlElement root = doc.DocumentElement;
                    string tip;

                    foreach( XmlElement element in root.GetElementsByTagName( "enu" ) )
                    {
                        tip = element.InnerText;
                        tip = tip.Replace( Char.ConvertFromUtf32( 0x0A ), Char.ConvertFromUtf32( 0x0D ) );
                        tip = tip.Replace( (char)0x0A, (char)0x0D );

                        tips.Add( tip );
                    }

                    foreach( XmlElement element in root.GetElementsByTagName( "ita" ) )
                    {
                        tip = element.InnerText;
                        tip = tip.Replace( Char.ConvertFromUtf32( 0x0A ), Char.ConvertFromUtf32( 0x0D ) );
                        tip = tip.Replace( (char)0x0A, (char)0x0D );

                        tipsIta.Add( tip );
                    }
                }
                catch( Exception ex )
                {
                    pkg.LogWarningLine( "Warning: error reading Tips.xml: {0}", ex );
                }
            }

            Tips = tips.ToArray();
            TipsIta = tipsIta.ToArray();

            pkg.LogInfoLine( "Welcome system: there are {0} enu and {1} ita tips.", Tips.Length, TipsIta.Length );
        }

        private static string GetRandomTip( LanguageType language )
        {
            if( Config.Debug )
                Console.WriteLine( "Requested {0} tip", language );

            if( language == LanguageType.Ita && TipsIta != null && TipsIta.Length > 0 )
                return TipsIta[ Utility.Random( TipsIta.Length ) ];
            else if( Tips != null && Tips.Length > 0 )
                return Tips[ Utility.Random( Tips.Length ) ];
            else
                return null;
        }

        private static void ReadNews()
        {
            string news = string.Empty;

            XmlDocument doc = new XmlDocument();

            if( File.Exists( Config.NewsFilePath ) )
            {
                try
                {
                    doc.Load( Config.NewsFilePath );
                }
                catch
                {
                    news = "No news is available at now.";
                }

                StringBuilder sb = new StringBuilder();

                try
                {
                    XmlElement root = doc.DocumentElement;
                    XmlNodeList elemList = root.GetElementsByTagName( "Notizia" );

                    for( int i = 0; i < elemList.Count; i++ )
                    {
                        sb.Append( root.GetElementsByTagName( "DataImmissione" )[ i ].InnerText );
                        sb.Append( "<br>" );
                        sb.Append( root.GetElementsByTagName( "Creatore" )[ i ].InnerText );
                        sb.Append( "<br>" );
                        sb.Append( root.GetElementsByTagName( "Testo" )[ i ].InnerText );
                        sb.Append( "<br>" + "<br>" );

                        news = sb.ToString();
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Warning: error reading Notizie.xml: {0}", ex );
                }
            }
            else
            {
                news = "No news is available at now. (FNF)";
            }

            News = news;
        }

        private static void ReadMotd()
        {
            string motd = string.Empty;

            XmlDocument doc = new XmlDocument();

            if( File.Exists( Config.MotdFilePath ) )
            {
                try
                {
                    doc.Load( Config.MotdFilePath );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                    motd = "No motd is available at now.";
                }

                StringBuilder sb = new StringBuilder();

                try
                {
                    XmlElement root = doc.DocumentElement;
                    XmlNodeList elemList = root.GetElementsByTagName( "Motd" );

                    for( int i = 0; i < elemList.Count; i++ )
                        sb.Append( root.GetElementsByTagName( "Testo" )[ i ].InnerText );

                    motd = sb.ToString();
                }
                catch( Exception ex )
                {
                    Console.WriteLine( "Warning: error reading Motd.xml: {0}", ex );
                }
            }
            else
            {
                motd = "No motd is available at now. (FNF)";
            }

            motd = motd.Replace( Char.ConvertFromUtf32( 0x0A ), Char.ConvertFromUtf32( 0x0D ) );
            motd = motd.Replace( (char)0x0A, (char)0x0D );

            Motd = motd;
        }

        internal static bool WriteNews( string text, Mobile from )
        {
            bool success = true;

            XmlDocument doc = new XmlDocument();

            if( !File.Exists( Config.NewsFilePath ) )
            {
                XmlTextWriter xmlTw = new XmlTextWriter( Config.NewsFilePath, null );
                xmlTw.Formatting = Formatting.Indented;
                xmlTw.WriteStartElement( "MidgardNews" );
                xmlTw.WriteElementString( "FineFile", "----------" );
                xmlTw.WriteEndElement();
                xmlTw.Flush();
                xmlTw.Close();
            }

            // Aggiunta della notizia
            FileStream fsLoadXml = new FileStream( Config.NewsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );

            try
            {
                doc.Load( fsLoadXml );
            }
            catch( Exception e )
            {
                success = false;
                Console.WriteLine( "Warning: WriteNews Operation Failed: {0}", e );
                from.SendMessage( "Warning: WriteNews Operation Failed." );
            }

            XmlElement root = doc.DocumentElement;
            fsLoadXml.Close();

            // Crea l'elemento NuovaNotizia
            XmlElement newsElement = doc.CreateElement( "Notizia" );

            XmlElement creator = doc.CreateElement( "Creatore" );
            creator.InnerText = from.Name;
            newsElement.AppendChild( creator );

            XmlElement dateNode = doc.CreateElement( "DataImmissione" );
            dateNode.InnerText = DateTime.Now.Date.ToShortDateString();
            newsElement.AppendChild( dateNode );

            // Aggiunge il testo della notizia
            XmlElement textNode = doc.CreateElement( "Testo" );
            textNode.InnerText = text;
            newsElement.AppendChild( textNode );

            // Aggiunge la notizia all'albero xml
            if( doc.DocumentElement != null )
                if( root != null )
                    doc.DocumentElement.InsertBefore( newsElement, root.FirstChild );

            FileStream stream = new FileStream( Config.NewsFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite );
            doc.Save( stream );
            stream.Close();

            return success;
        }

        internal static bool WriteMotd( string text, Mobile from )
        {
            bool success = true;

            XmlDocument doc = new XmlDocument();

            if( !File.Exists( Config.MotdFilePath ) )
            {
                XmlTextWriter xmlTw = new XmlTextWriter( Config.MotdFilePath, null );
                xmlTw.Formatting = Formatting.Indented;
                xmlTw.WriteStartElement( "Motd" );
                xmlTw.WriteElementString( "FineFile", "----------" );
                xmlTw.WriteEndElement();
                xmlTw.Flush();
                xmlTw.Close();
            }

            // Aggiunta della notizia
            FileStream fsLoadXml = new FileStream( Config.MotdFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite );

            try
            {
                doc.Load( fsLoadXml );
            }
            catch( Exception e )
            {
                success = false;
                Console.WriteLine( "Warning: WriteMotd Operation Failed: {0}", e );
                from.SendMessage( "Warning: WriteMotd Operation Failed." );
            }

            XmlElement root = doc.DocumentElement;
            fsLoadXml.Close();

            // Crea l'elemento NuovaNotizia
            XmlElement motdElement = doc.CreateElement( "Motd" );

            XmlElement creator = doc.CreateElement( "Creatore" );
            creator.InnerText = from.Name;
            motdElement.AppendChild( creator );

            XmlElement dateNode = doc.CreateElement( "DataImmissione" );
            dateNode.InnerText = DateTime.Now.Date.ToShortDateString();
            motdElement.AppendChild( dateNode );

            // Aggiunge il testo della notizia
            XmlElement textNode = doc.CreateElement( "Testo" );
            textNode.InnerText = text;
            motdElement.AppendChild( textNode );

            // Aggiunge la notizia all'albero xml
            if( doc.DocumentElement != null )
                if( root != null )
                    doc.DocumentElement.InsertBefore( motdElement, root.FirstChild );

            FileStream stream = new FileStream( Config.NewsFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite );
            doc.Save( stream );
            stream.Close();

            return success;
        }

        internal static void UploadNews( FileSystemInfo fileName )
        {
            try
            {
				FtpService.UploadFile(fileName.FullName,fileName.Name);
                /*Uri uri = new Uri( Path.Combine( Config.AddressFTP, fileName.Name ) );
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential( Config.UserFTP, Config.PasswordFTP );
                client.UploadFileAsync( uri, fileName.FullName );*/
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}