// #define DebugWebStatus

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server.Accounting;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using Midgard.Engines.MidgardTownSystem;

namespace Server.Misc
{
	public class StatusPage : Timer
	{
		private static readonly bool StatusPageEnabled = false;
        private static readonly bool FtpEnabled = false;

#if DebugWebStatus
		private static readonly double refreshDelay = 10.0;
#else
		private static readonly double refreshDelay = 180.0;
#endif

		public static void Initialize()
		{
			if( StatusPageEnabled )
				new StatusPage().Start();
		}

		public StatusPage() : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( refreshDelay ) )
		{
			Priority = TimerPriority.FiveSeconds;
		}

        public void Serialize( BinaryWriter writer )
        {
            // Version
            writer.Write( 0 );

            writer.Write( NetState.Instances.Count );

            // upTimeString
            writer.Write( FormatTimeSpan( DateTime.Now - Clock.ServerStart ) );

            writer.Write( Accounts.Count );
            writer.Write( World.Mobiles.Count );
            writer.Write( World.Items.Count );

            // memoryInUse
            writer.Write( FormatByteAmount( GC.GetTotalMemory( false ) ) );

            List<Mobile> players = BuildList();
            if( players == null || players.Count < 1 )
                writer.Write( 0 );
            else
            {
                writer.Write( players.Count );

                for( int i = 0; i < players.Count; i++ )
                {
                    Mobile m = players[ i ];

                    writer.Write( GetRgbHueFor( m ) );
                    writer.Write( FormatName( m ) );
                    writer.Write( FormatTown( m ) );
                    writer.Write( FormatKarmaFame( m ) );
                    writer.Write( FormatGuild( m ) );
                }
            }
        }

		protected override void OnTick()
		{
			if ( !Directory.Exists( "web" ) )
				Directory.CreateDirectory( "web" );

            using( BinaryWriter binaryWriter = new BinaryWriter( new FileStream( "web/status.bin", FileMode.Create ) ) )
		    {
		        Serialize( binaryWriter );
		    }

			string netstatesString = NetState.Instances.Count.ToString();
			string upTimeString = FormatTimeSpan( DateTime.Now - Clock.ServerStart);
			string accountsCount = Accounts.Count.ToString();
			string mobilesCount = World.Mobiles.Count.ToString();
			string itemsCount =  World.Items.Count.ToString();
			string memoryInUse = FormatByteAmount( GC.GetTotalMemory( false ) );

			List<Mobile> players = BuildList();

			try
			{
				using ( StreamWriter op = new StreamWriter( "web/status.html" ) )
				{
					op.WriteLine( "<html>" );
					op.WriteLine( "	<head>" );
					op.WriteLine( "		<br>" );
	   				op.WriteLine( "			<title>Midgard Server Status</title>" );
					op.WriteLine( "			" + Par( FontFormat( 4, "Verdana", "0080FF",  Center( Bold( "Midgard Server Status" ) ) ) ) );
	   				op.WriteLine( "	</head>" );
					op.WriteLine( "	<body bgcolor=\"##000000\">" );
					op.WriteLine( "		" + FontFormat( 1, "Verdana", "FFFFFF",  Center( Bold( String.Format( "Players Online: <!--players-->{0}<!--/players-->", netstatesString ) ) ) ) );
					op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF",  Center( String.Format( "Current uptime: {0}", upTimeString ) ) ) );
					op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF",  Center( String.Format( "Accounts: {0}", accountsCount ) ) ) );
					op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF",  Center( String.Format( "Mobiles: {0}", mobilesCount ) ) ) );
					op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF",  Center( String.Format( "Items: {0}", itemsCount ) ) ) );
					op.WriteLine( "		" + FontFormat( 1, "Verdana", "0080FF",  Center( String.Format( "Memory in Use: {0}", memoryInUse ) ) ) );
					op.WriteLine( "		<br><br><br>" );
					op.WriteLine( "		<table width=\"100%\">" );
					op.WriteLine( "			<tbody>" );
	        		op.WriteLine( "				<tr>" );
	        		op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF",  Center( Bold( "Name" ) ) ) ) );
	        		op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF",  Center( Bold( "Town" ) ) ) ) );
	        		op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF",  Center( Bold( "Karma / Fame" ) ) ) ) );
	        		op.WriteLine( "					" + TableRaw( FontFormat( 2, "Verdana", "0080FF",  Center( Bold( "Guild / Guild Title" ) ) ) ) );
		 			op.WriteLine( "				</tr>" );
	
		 			for( int i = 0; i < players.Count; i++ )
					{
						Mobile m = players[i];
						string nameHue = GetRgbHueFor( m );
	
						op.WriteLine( "				<tr>" );
						op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", nameHue,  Center( FormatName( m ) ) ) ) );
						op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", "FFFFFF",  Center( FormatTown( m ) ) ) ) );
						op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", "FFFFFF",  Center( FormatKarmaFame( m ) ) ) ) );
						op.WriteLine( "					" + TableRaw( FontFormat( 1, "Verdana", "FFFFFF",  Center( FormatGuild( m ) ) ) ) );
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

            if( FtpEnabled )
                Midgard.Engines.FtpService.UploadFile( "web/status.html", "serverstatus.html" );
		}

/*
		private static string Encode( string input )
		{
			StringBuilder sb = new StringBuilder( input );

			sb.Replace( "&", "&amp;" );
			sb.Replace( "<", "&lt;" );
			sb.Replace( ">", "&gt;" );
			sb.Replace( "\"", "&quot;" );
			sb.Replace( "'", "&apos;" );

			return sb.ToString();
		}
*/

		#region formats
		private static string FormatName( Mobile m )
		{
			if( m == null || m.Deleted )
				return String.Empty;

			string fName = String.IsNullOrEmpty( m.Name ) ? "Midgard Player" : m.Name;

			if( m.AccessLevel > AccessLevel.Player )
				fName = String.Concat( fName, String.Format( " (Midgard {0})", m.AccessLevel ) );

			return fName;
		}

		private static string FormatTown( Mobile mobile )
		{
			if( mobile == null || mobile.Deleted )
				return String.Empty;

            TownPlayerState state = TownPlayerState.Find( mobile );
            if( state != null )
                return state.TownSystem.Definition.TownName;
            else
                return "-";
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

		private static string FormatKarmaFame( Mobile m )
		{
			return String.Format( "{0} / {1}", m.Karma, m.Fame );
		}

		private static string FormatTimeSpan( TimeSpan ts ) 
		{ 
			return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 ); 
		} 

		private static string FormatByteAmount( long totalBytes ) 
		{ 
			if( totalBytes > 1000000000 ) 
				return String.Format( "{0:F1} GB", (double)totalBytes / 1073741824 ); 
		
			if( totalBytes > 1000000 ) 
				return String.Format( "{0:F1} MB", (double)totalBytes / 1048576 ); 
		
			if( totalBytes > 1000 ) 
				return String.Format( "{0:F1} KB", (double)totalBytes / 1024 ); 
		
			return String.Format( "{0} Bytes", totalBytes ); 
		} 
		#endregion

		#region hues
		private static string GetRgbHueFor( Mobile m )
		{
			if( m == null )
				return "FFFFFF";

			switch ( m.AccessLevel )
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator: 	return "9999FF";
				case AccessLevel.Seer: 				return "00CC33";
				case AccessLevel.GameMaster: 		return "CC0000";
				case AccessLevel.Counselor: 		return "FFFF00";
				default: 							return "FFFFFF";
			}
		}

/*
		private static int GetHueFor( Mobile m )
		{
			if( m == null )
				return 0x58;

			switch ( m.AccessLevel )
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator: 	return 0x516;
				case AccessLevel.Seer: 				return 0x144;
				case AccessLevel.GameMaster: 		return 0x21;
				case AccessLevel.Counselor: 		return 0x2;
				case AccessLevel.Player: 
				default:							return 0x58;
			}
		}
*/
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
			return String.Format( "<font size=\"{0}\" face=\"{1}\" color=\"#{2:X6}\">{3}<font>", size, fontName, color, text );
		}
*/

		private static string FontFormat( int size, string fontName, string color, string text )
		{
			return String.Format( "<font size=\"{0}\" face=\"{1}\" color=\"#{2}\">{3}<font>", size, fontName, color, text );
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

		private static List<Mobile> BuildList()
		{
			List<Mobile> list = new List<Mobile>();
			List<NetState> states = NetState.Instances;

			for( int i = 0; i < states.Count; i++ )
			{
				Midgard2PlayerMobile m2pm = states[i].Mobile as Midgard2PlayerMobile;
				if( m2pm != null && !m2pm.Deleted && m2pm.OnlineVisible )
				{
					list.Add( m2pm );
#if DebugWebStatus
					for( int j = 0; j < 50; j++ )
						list.Add( m2pm );
#endif
				}
			}

			list.Sort( InternalComparer.Instance );

			return list;
		}

		private class InternalComparer : IComparer<Mobile>
		{
			public static readonly IComparer<Mobile> Instance = new InternalComparer();

		    public int Compare( Mobile x, Mobile y )
			{
				if ( x == null || y == null )
					throw new ArgumentException();

				if ( x.AccessLevel > y.AccessLevel )
					return -1;
				else if ( x.AccessLevel < y.AccessLevel )
					return 1;
				else
					return Insensitive.Compare( x.Name, y.Name );
			}
		}
	}
}