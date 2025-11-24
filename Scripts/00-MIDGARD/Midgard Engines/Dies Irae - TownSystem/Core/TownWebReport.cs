// #define DebugTownRankReport

/***************************************************************************
 *                                  TownRankReport.cs
 *                            		-----------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *	rebuilder				: Dies Irae
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Server;
using Server.Guilds;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownRankReport : Timer
    {
        private static readonly bool TownRankStatusPageEnabled = Config.TownRankStatusPageEnabled;
        private static readonly bool TownRanFtpEnabled = Config.TownRanFtpEnabled;

#if DebugTownRankReport
        private static readonly double RefreshDelay = 0.2; // once a minute
#else
        private static readonly double RefreshDelay = 5.0; // once every 5 minutes
#endif

        public static void StartTimer()
        {
            new TownRankReport().Start();
        }

        public TownRankReport()
            : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromMinutes( RefreshDelay ) )
        {
#if DebugTownRankReport
            Priority = TimerPriority.FiveSeconds;
#else
            Priority = TimerPriority.OneMinute;
#endif
        }

        protected override void OnTick()
        {
            if( !Directory.Exists( "web" ) )
                Directory.CreateDirectory( "web" );

            try
            {
                WriteReport( "web/townRankStatus.xml" );
            }
            catch
            {
            }

            if( TownRanFtpEnabled )
                FtpService.UploadFile( "web/townRankStatus.xml", "public/townRankStatus.xml" );
        }

        public static void WriteReport( string path )
        {
            if( !TownRankStatusPageEnabled )
                return;

            File.Delete( path ); // pulizia dell'eventuale file vecchio

            XmlDocument doc = new XmlDocument();

            XmlTextWriter xmlTw = new XmlTextWriter( path, Encoding.UTF8 );
            xmlTw.Formatting = Formatting.Indented;
            xmlTw.WriteStartElement( "TownRankReport" );
            xmlTw.WriteElementString( "EoF", "----------" );
            xmlTw.WriteEndElement();
            xmlTw.Flush();
            xmlTw.Close();

            FileStream loadStream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );

            try
            {
                doc.Load( loadStream );
            }
            catch( XmlException ex )
            {
                Config.Pkg.LogErrorLine( "Error in TownRankReport: {0}", ex );
                return;
            }

            loadStream.Close();

            XmlElement root = doc.DocumentElement;

            root.SetAttribute( "dateTime", FormatDateTime( DateTime.Now ) );
            root.SetAttribute( "totalCitizens", TownSystem.GetTotalCitizensInAllSystems().ToString() );

            foreach( TownSystem t in TownSystem.TownSystems )
                root.SetAttribute( t.Definition.Town.ToString(), t.TownKillState.ToString() );

            List<TownPlayerState> states = TownSystem.GetPlayerStatesOnServer( true );
            XmlElement playerElement;

            foreach( TownPlayerState tps in states )
            {
                if( tps == null )
                    continue;

                Mobile m = tps.Mobile;
                if( m == null )
                    continue;

                if( tps.IsInactive || tps.TownRankPoints <= -6 )
                    continue;

                playerElement = doc.CreateElement( "player" );

                playerElement.SetAttribute( "name", String.IsNullOrEmpty( m.Name ) ? "unknown name" : FixNames( m.Name ) );
                playerElement.SetAttribute( "town", tps.TownSystem.Definition.Town.ToString() );
                playerElement.SetAttribute( "guild", FixNames( FormatGuild( m ) ) );
                playerElement.SetAttribute( "kills", tps.TownRankPoints.ToString() );
                playerElement.SetAttribute( "citizenKills", tps.CitizenKills.ToString() );

                doc.DocumentElement.InsertAfter( playerElement, root.LastChild );
            }

            FileStream saveStream = new FileStream( path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite );
            doc.Save( saveStream );
            saveStream.Close();
        }

        private static string FormatGuild( Mobile m )
        {
            Guild g = m.Guild as Guild;

            if( g == null )
                return String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append( String.IsNullOrEmpty( g.Name ) ? "Midgard Guild" : g.Name );

            if( !String.IsNullOrEmpty( g.Abbreviation ) )
                sb.AppendFormat( "[{0}]", g.Abbreviation.Trim() );

            if( !String.IsNullOrEmpty( m.GuildTitle ) )
                sb.AppendFormat( ",{0}", m.GuildTitle.Trim() );

            return sb.ToString();
        }

        private static string FormatDateTime( DateTime d )
        {
            if( d == DateTime.MinValue )
                return "";

            return d.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" );
        }

        private static string FixNames( string input )
        {
            StringBuilder sb = new StringBuilder( input );
            sb.Replace( "'", "" );
            return sb.ToString();
        }
    }
}