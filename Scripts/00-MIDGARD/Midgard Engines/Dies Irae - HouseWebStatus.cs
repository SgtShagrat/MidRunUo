/***************************************************************************
 *                                  HouseStatusPage.cs
 *                            		-------------
 *  begin                	: November, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *	rebuilder				: Dies Irae
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Multis;

namespace Midgard.Misc
{
    public class HouseStatusPage : Timer
    {
        private static readonly bool Enabled = false;

        private static readonly bool Debug = false;

        private static readonly bool HouseFtpEnabled = false;

        private static readonly double HouseRefreshDelay = Debug ? 1 / 60.0 : 24.0; // once a day

        public static void Initialize()
        {
            if( Enabled )
                new HouseStatusPage().Start();
        }

        public HouseStatusPage()
            : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromHours( HouseRefreshDelay ) )
        {
            Priority = Debug ? TimerPriority.FiveSeconds : TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            if( !Directory.Exists( "web" ) )
                Directory.CreateDirectory( "web" );

            List<BaseHouse> houses = GetHousesOnServer();

            string totalHouses = houses.Count.ToString();
            string lastUptate = FormatDateTime( DateTime.Now );
            string nextUpdate = FormatDateTime( DateTime.Now + TimeSpan.FromHours( HouseRefreshDelay ) );

            try
            {
                using( StreamWriter op = new StreamWriter( "web/houseStatus.html" ) )
                {
                    op.WriteLine( "<html>" );
                    op.WriteLine( "	<head>" );
                    op.WriteLine( "		<br>" );
                    op.WriteLine( "			<title>Midgard House Status</title>" );
                    op.WriteLine( "			" + Par( FontFormat( 4, "Verdana", "0080FF", Center( Bold( "Midgard House Status" ) ) ) ) );
                    op.WriteLine( "	</head>" );
                    op.WriteLine( "	<body bgcolor=\"##000000\">" );
                    op.WriteLine( "		" + FontFormat( 1, "Verdana", "FFFFFF", Center( Bold( String.Format( "House Placed: {0}", totalHouses ) ) ) ) );
                    op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF", Center( String.Format( "Last Status Update: {0}", lastUptate ) ) ) );
                    op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF", Center( String.Format( "Next Status Update: {0}", nextUpdate ) ) ) );
                    op.WriteLine( "		<br><br><br>" );
                    op.WriteLine( "		<table width=\"100%\">" );
                    op.WriteLine( "			<tbody>" );
                    op.WriteLine( "				<tr>" );
                    op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF", Center( Bold( "Owner" ) ) ) ) );
                    op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF", Center( Bold( "Location" ) ) ) ) );
                    op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF", Center( Bold( "City" ) ) ) ) );
                    op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF", Center( Bold( "Map" ) ) ) ) );
                    op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF", Center( Bold( "Last Refreshed" ) ) ) ) );
                    op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF", Center( Bold( "Will Collapse On" ) ) ) ) );
                    op.WriteLine( "				</tr>" );

                    for( int i = 0; i < houses.Count; i++ )
                    {
                        BaseHouse house = houses[ i ];

                        if( house.Owner != null && house.Owner.AccessLevel > AccessLevel.Player )
                            continue;

                        string houseHue = GetRgbHueFor( house );

                        op.WriteLine( "				<tr>" );
                        op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", houseHue, Center( FormatName( house ) ) ) ) );
                        op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", houseHue, Center( FormatLocation( house ) ) ) ) );
                        op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", houseHue, Center( FormatTown( house ) ) ) ) );
                        op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", houseHue, Center( FormatMap( house ) ) ) ) );
                        op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", houseHue, Center( FormatDateTime( house.LastRefreshed ) ) ) ) );
                        op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", houseHue, Center( FormatDateTime( house.LastRefreshed + house.DecayPeriod ) ) ) ) );
                        op.WriteLine( "				</tr>" );
                    }

                    op.WriteLine( "			</tbody>" );
                    op.WriteLine( "		</table>" );
                    op.WriteLine( "	</body>" );
                    op.WriteLine( "</html>" );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            if( HouseFtpEnabled )
                Engines.FtpService.UploadFile( "web/houseStatus.html", "houseStatus.html" );
        }

        private static List<BaseHouse> GetHousesOnServer()
        {
            List<BaseHouse> list = new List<BaseHouse>();

            try
            {
                foreach( Item i in World.Items.Values )
                {
                    if( i is BaseHouse && !( TownFieldSign.IsField( (BaseHouse)i, true ) ) )
                    {
                        BaseHouse h = i as BaseHouse;

                        if( h.DecayType == DecayType.ManualRefresh )
                        {
                            if( h.Owner != null && h.Owner.AccessLevel == AccessLevel.Player )
                                list.Add( h );
                        }
                    }
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( "Warning! Error in House Web Status refresh: {0}", e );
            }

            list.Sort( InternalComparer.Instance );

            return list;
        }

        #region formats
        private static string FormatDateTime( DateTime d )
        {
            if( d == DateTime.MinValue )
                return "";

            return d.ToString( "dd'-'MM'-'yyyy HH':'mm':'ss" );
        }

        private static string FormatName( BaseHouse h )
        {
            if( h == null || h.Deleted )
                return String.Empty;

            if( h.Owner == null )
                return "House without Owner";

            string name = String.IsNullOrEmpty( h.Owner.Name ) ? "Midgard Player" : h.Owner.Name;

            return name;
        }

        private static string FormatLocation( IEntity h )
        {
            return String.Format( "{0}, {1}, {2}", h.Location.X, h.Location.Y, h.Location.Z );
        }

        private static string FormatTown( BaseHouse h )
        {
            if( h.Sign != null && !h.Sign.Deleted )
            {
                TownSystem system = TownSystem.Find( new Point3D( h.Sign.Location.X, h.Sign.Location.Y + 1, h.Sign.Location.Z ), h.Sign.Map );
                if( system != null )
                    return system.Definition.TownName;
            }

            return "Unknown";
        }

        private static string FormatMap( IEntity h )
        {
            return h.Map.Name;
        }
        #endregion

        #region hues
        private static string GetRgbHueFor( BaseHouse h )
        {
            if( h == null )
                return "FFFFFF";

            double daysToCollapse = ( ( h.LastRefreshed + h.DecayPeriod ) - DateTime.Now ).TotalDays;

            if( daysToCollapse < 5.0 )
                return "FF3030"; 				// red
            if( daysToCollapse < 10.0 )
                return "FF7F00";				// orange
            if( daysToCollapse < 20.0 )
                return "FFFF00";				// yellow

            return "00CC33";				    // green
        }
        #endregion

        #region html tags
        private static string Bold( string text )
        {
            return String.Format( "<b>{0}</b>", text );
        }

        private static string Center( string text )
        {
            return String.Format( "<center>{0}</center>", text );
        }

        /*
        private static string FontFormat( int size, string fontName, int color, string text )
        {
            return String.Format( "<font size=\"{0}\" face=\"{1}\" color=\"#{2:X6}\">{3}</font>", size, fontName, color, text );
        }
        */

        private static string FontFormat( int size, string fontName, string color, string text )
        {
            return String.Format( "<font size=\"{0}\" face=\"{1}\" color=\"#{2}\">{3}</font>", size, fontName, color, text );
        }

        private static string TableRaw( string text )
        {
            return String.Format( "<td>{0}</td>", text );
        }

        private static string Par( string text )
        {
            return String.Format( "<p>{0}</p>", text );
        }
        #endregion

        private sealed class InternalComparer : IComparer<BaseHouse>
        {
            public static readonly IComparer<BaseHouse> Instance = new InternalComparer();

            public int Compare( BaseHouse x, BaseHouse y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( x.Owner == null || y.Owner == null || String.IsNullOrEmpty( x.Owner.Name ) || String.IsNullOrEmpty( y.Owner.Name ) )
                    return -1;

                return Insensitive.Compare( x.Owner.Name, y.Owner.Name );
            }
        }
    }
}