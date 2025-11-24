#define DiesDebugTimers

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

using Midgard.ContextMenus;
using Midgard.Items;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.JailSystem
{
    public class JailSystem
    {
        private static readonly string JailDirectory = Path.Combine( "Saves", "Jailings" );
        private static readonly string BinPath = Path.Combine( JailDirectory, "Jailings.bin" );
        private static readonly string IdxPath = Path.Combine( JailDirectory, "Jailings.idx" );

        public static bool AllowStaffBadWords = true;
        public static bool AllowStaffOOC = true;

        public static List<string> BadWords = new List<string>();
        public static bool BlockOOCSpeech = false;

        public static List<Point3D> Cells = new List<Point3D>();
        public static List<Point3D> DefaultRelease = new List<Point3D>();
        public static Map DefaultReleaseFacet = Map.Felucca;
        public static string FoulJailorName = "Language Auto Jailor";
        public static List<TimeSpan> FoulMouthJailTimes = new List<TimeSpan>();
        public static Map JailMap = Map.Felucca;
        public static string JSName = "JailSystem";
        private static Hashtable m_FwalkWarnings;
        private static int m_NextID;
        private static Hashtable m_Warnings;
        public static string OocJailorName = "OOC Jailor";
        public static List<TimeSpan> OOCJailTimes = new List<TimeSpan>();
        public static string OoclistCommand = "ooclist";
        public static List<string> OOCParts = new List<string>();
        public static int Oocwarns = 2;
        public static List<string> OOCWords = new List<string>();
        public static bool SingleFacetOnly = true;
        public static string StatusCommand = "jailtime";
        public static string TimeCommand = "what time is it";
        public static bool TimersRunning;
        public static bool UseLanguageFilter = false;
        public static bool UseOOCFilter = false;
        public static bool UseSmokingFootGear = true;
        public static bool Warnspeedy;
        private readonly Dictionary<Serial, ReleaseLoc> m_ReleasePoints = new Dictionary<Serial, ReleaseLoc>();
        public string FreedBy = JSName;
        public string Jailor;
        private Jailing m_AutoReleasor;
        private AccessLevel m_JailorAC = AccessLevel.Counselor;
        private bool m_ReturnToPoint = true;
        public string Reason;

        public JailSystem()
        {
            BuildJail();
        }

        public JailSystem( Mobile m )
            : this( m, AccessLevel.Counselor )
        {
        }

        public JailSystem( Mobile m, AccessLevel l )
        {
            BuildJail();
            Name = ( (Account)m.Account ).Username;
            m_JailorAC = l;
        }

        public JailSystem( Serial serial )
        {
            Serial = serial;
            BuildJail();
        }

        public static Hashtable FWalkWarns
        {
            get
            {
                if( m_FwalkWarnings == null )
                    m_FwalkWarnings = new Hashtable();
                return m_FwalkWarnings;
            }
        }

        public static Hashtable Warns
        {
            get
            {
                if( m_Warnings == null )
                    m_Warnings = new Hashtable();
                return m_Warnings;
            }
        }

        public static Hashtable JailSystemList { get; private set; }

        public DateTime ReleaseDate { get; private set; }

        public bool Jailed
        {
            get { return ( ReleaseDate > DateTime.Now ); }
        }

        public int ID { get; private set; }

        public Account Prisoner
        {
            get { return Accounts.GetAccount( Name ) as Account; }
        }

        public string Name { get; set; }

        public Serial Serial { get; set; }

        public static void DefaultSettings()
        {
            StatusCommand = "jailtime";
            TimeCommand = "what time is it";
            OoclistCommand = "ooclist";
            SingleFacetOnly = true;
            JSName = "JailSystem";
            UseSmokingFootGear = true;
            JailMap = Map.Felucca;
            DefaultReleaseFacet = Map.Felucca;
            DefaultRelease.Clear();
            DefaultRelease.Add( new Point3D( 2708, 693, 0 ) );
            DefaultRelease.Add( new Point3D( 4476, 1281, 0 ) );
            DefaultRelease.Add( new Point3D( 1344, 1994, 0 ) );
            DefaultRelease.Add( new Point3D( 1507, 3769, 0 ) );
            DefaultRelease.Add( new Point3D( 780, 754, 0 ) );
            DefaultRelease.Add( new Point3D( 1833, 2942, -22 ) );
            DefaultRelease.Add( new Point3D( 651, 2066, 0 ) );
            DefaultRelease.Add( new Point3D( 3556, 2150, 26 ) );
            UseLanguageFilter = true;
            FoulJailorName = "Language Auto Jailor";
            AllowStaffBadWords = true;
            BadWords.Clear();
            BadWords.Add( "fuck" );
            BadWords.Add( "cunt" );
            FoulMouthJailTimes.Clear();
            FoulMouthJailTimes.Add( new TimeSpan( 0, 0, 30, 0 ) );
            FoulMouthJailTimes.Add( new TimeSpan( 0, 1, 0, 0 ) );
            FoulMouthJailTimes.Add( new TimeSpan( 0, 1, 30, 0 ) );
            FoulMouthJailTimes.Add( new TimeSpan( 0, 2, 0, 0 ) );
            FoulMouthJailTimes.Add( new TimeSpan( 0, 2, 30, 0 ) );
            //ooc section
            OocJailorName = "OOC Jailor";
            BlockOOCSpeech = true;
            UseOOCFilter = true;
            OOCWords.Add( "lol" );
            AllowStaffOOC = true;
            OOCJailTimes.Clear();
            OOCJailTimes.Add( new TimeSpan( 0, 0, 10, 0 ) );
            OOCJailTimes.Add( new TimeSpan( 0, 0, 20, 0 ) );
            OOCJailTimes.Add( new TimeSpan( 0, 0, 30, 0 ) );
            OOCJailTimes.Add( new TimeSpan( 0, 0, 40, 0 ) );
            OOCJailTimes.Add( new TimeSpan( 0, 1, 0, 0 ) );
            Oocwarns = 2;
            Cells.Clear();
            Cells.Add( new Point3D( 5276, 1164, 0 ) );
            Cells.Add( new Point3D( 5286, 1164, 0 ) );
            Cells.Add( new Point3D( 5296, 1164, 0 ) );
            Cells.Add( new Point3D( 5306, 1164, 0 ) );
            Cells.Add( new Point3D( 5276, 1174, 0 ) );
            Cells.Add( new Point3D( 5286, 1174, 0 ) );
            Cells.Add( new Point3D( 5296, 1174, 0 ) );
            Cells.Add( new Point3D( 5306, 1174, 0 ) );
            Cells.Add( new Point3D( 5283, 1184, 0 ) );
        }

        public static JailSystem FromMobile( Mobile m )
        {
            return FromAccount( (Account)m.Account );
        }

        public static JailSystem FromAccount( Account a )
        {
            if( a == null )
            {
                Console.WriteLine( "JailSystem Warning: account null in FromAccount." );
                return null;
            }

            string un = a.Username;
            foreach( JailSystem js in JailSystemList.Values )
            {
                if( js.Name == un )
                    return js;
            }
            return null;
        }

        public XElement ToXElement()
        {
            StringBuilder sb = new StringBuilder();
            Account a = Accounts.GetAccount( Name ) as Account;
            if( a != null )
            {
                foreach( var m in a.Mobiles )
                    sb.Append( ( m.Name ?? "" ) + "-" );
            }

            return new XElement( "condemn", new XAttribute( "name", Utility.SafeString( sb.ToString() ) ),
                                new XAttribute( "release", ReleaseDate.ToString() ),
                                new XAttribute( "jailor", Utility.SafeString( Jailor ) ),
                                new XAttribute( "reason", Utility.SafeString( Reason ) ) );
        }

        public static JailSystem Lockup( Mobile m )
        {
            try
            {
                JailSystem js = FromMobile( m ) ?? new JailSystem( m );

                js.LockupMobile( m );
                return js;
            }
            catch
            {
                Console.WriteLine( "{0}: Lockup call failed on-{1}", JSName, m.Name );
                return null;
            }
        }

        #region Jail
        public static void Jail( Mobile badBoy, bool foul, string reason, bool releasetoJailing, string jailedBy,
                                 AccessLevel l )
        {
            Jail( badBoy, GetJailTerm( badBoy, foul ), reason, releasetoJailing, jailedBy, l );
        }

        public static void Jail( Mobile badBoy, TimeSpan ts, string reason, bool releasetoJailing, string jailedBy,
                                 AccessLevel l )
        {
            Jail( badBoy, ts, reason, releasetoJailing, jailedBy, l, true );
        }

        public static void Jail( Mobile badBoy, TimeSpan ts, string reason, bool releasetoJailing, string jailedBy,
                                 AccessLevel l, bool useBoot )
        {
            DateTime dt = DateTime.Now.Add( ts );
            Jail( badBoy, dt, reason, releasetoJailing, jailedBy, l, useBoot );
        }

        public static void Jail( Mobile badBoy, int days, int hours, int minutes, string reason, bool releasetoJailing,
                                 string jailedBy, AccessLevel l )
        {
            DateTime dt = DateTime.Now.AddDays( days ).AddHours( hours ).AddMinutes( minutes );
            Jail( badBoy, dt, reason, releasetoJailing, jailedBy, l );
        }

        public static void Jail( Mobile badBoy, DateTime dt, string reason, bool releasetoJailing, string jailedBy,
                                 AccessLevel l )
        {
            Jail( badBoy, dt, reason, releasetoJailing, jailedBy, l, true );
        }

        public static void Jail( Mobile badBoy, DateTime dt, string reason, bool releasetoJailing, string jailedBy,
                                 AccessLevel l, bool useBoot )
        {
            JailSystem js = FromMobile( badBoy );
            if( js == null )
            {
                js = new JailSystem( badBoy, l );
            }
            else
            {
                if( js.Jailed )
                {
                    js.LockupMobile( badBoy, useBoot );
                    return;
                }
            }
            js.FillJailReport( badBoy, dt, reason, releasetoJailing, jailedBy );
            js.LockupMobile( badBoy, useBoot );
        }

        public static void Jail( Mobile badBoy, bool foul, string reason, bool releasetoJailing, string jailedBy )
        {
            Jail( badBoy, GetJailTerm( badBoy, foul ), reason, releasetoJailing, jailedBy, AccessLevel.Counselor );
        }

        public static void Jail( Mobile badBoy, TimeSpan ts, string reason, bool releasetoJailing, string jailedBy )
        {
            DateTime dt = DateTime.Now.Add( ts );
            Jail( badBoy, dt, reason, releasetoJailing, jailedBy, AccessLevel.Counselor );
        }

        public static void Jail( Mobile badBoy, int days, int hours, int minutes, string reason, bool releasetoJailing,
                                 string jailedBy )
        {
            DateTime dt = DateTime.Now.AddDays( days ).AddHours( hours ).AddMinutes( minutes );
            Jail( badBoy, dt, reason, releasetoJailing, jailedBy, AccessLevel.Counselor );
        }

        public static void Jail( Mobile badBoy, DateTime dt, string reason, bool releasetoJailing, string jailedBy )
        {
            Jail( badBoy, dt, reason, releasetoJailing, jailedBy, AccessLevel.Counselor );
        }
        #endregion

        public static void ConfigSystem()
        {
            JailSystemList = new Hashtable();
            EventSink.WorldLoad += new WorldLoadEventHandler( OnLoad );
            EventSink.WorldSave += new WorldSaveEventHandler( OnSave );
            //EventSink.FastWalk += new FastWalkEventHandler( OnFastWalk );
        }

        public static bool MovementThrottle_Callback( NetState ns )
        {
            bool hotSteppen = PlayerMobile.MovementThrottle_Callback( ns );
            if( !hotSteppen )
            {
                //Console.WriteLine( "Client: {0}: Fast movement detected (name={1})", ns, ns.Mobile.Name );
                if( Warnspeedy )
                    FWalkWarn( ns.Mobile );
            }
            return hotSteppen;
        }

        public static void OnFastWalk( FastWalkEventArgs e )
        {
            e.Blocked = true; //disallow this fastwalk
            //Console.WriteLine( "Client: {0}: Fast movement detected 2(name={1}) in jail system", e.NetState, e.NetState.Mobile.Name );

            if( Warnspeedy )
                FWalkWarn( e.NetState.Mobile );
        }

        public static void FWalkWarn( Mobile m )
        {
            if( !FWalkWarns.Contains( ( (Account)m.Account ).Username ) )
            {
                FWalkWarns.Add( ( (Account)m.Account ).Username, new Hashtable() );
            }
            var w = (Hashtable)FWalkWarns[ ( (Account)m.Account ).Username ];
            if( w == null )
            {
                Jail( m, false,
                      "Fastwalk Detected, warning system was unable to issue a warning and jailed you.", true,
                      "Fastwalk Jailor", AccessLevel.GameMaster );
            }
            DateTime k = DateTime.Now;
            int i = 0;
            if( w != null )
                while( w.Contains( k ) )
                {
                    k = k.Subtract( new TimeSpan( 0, 0, 1 ) );
                    if( i > 10 )
                        continue;
                    i++;
                }
            if( i <= 10 )
            {
                string s = "Fastwalk detection";
                if( w != null )
                    w.Add( k, s );
                new WarnRemover( w, k );
            }
            if( w != null )
                if( w.Count > 5 )
                {
                    Jail( m, false, "Fastwalk detection after repeated warnings.", true, OocJailorName );
                    FWalkWarns.Remove( ( (Account)m.Account ).Username );
                }
                else
                {
                    m.SendMessage(
                        "You have been detected using fastwalk.  If you are using a fastwalk/speed hack, stop now or go to jail." );
                }
        }

        public static void OnLoad()
        {
            FileStream idxFileStream;
            FileStream binFileStream;

            BinaryFileReader idxReader;
            BinaryFileReader binReader;

            JailSystem tJail;
            int jailCount = 0;

            if( ( File.Exists( IdxPath ) ) && ( File.Exists( BinPath ) ) )
            {
                idxFileStream = new FileStream( IdxPath, (FileMode)3, (FileAccess)1, (FileShare)1 );
                binFileStream = new FileStream( BinPath, (FileMode)3, (FileAccess)1, (FileShare)1 );

                try
                {
                    idxReader = new BinaryFileReader( new BinaryReader( idxFileStream ) );
                    binReader = new BinaryFileReader( new BinaryReader( binFileStream ) );

                    jailCount = idxReader.ReadInt();

                    if( jailCount > 0 )
                    {
                        for( int i = 0; i < jailCount; i++ )
                        {
                            idxReader.ReadInt();

                            int tID = idxReader.ReadInt();
                            long tPos = idxReader.ReadLong();
                            int tLength = idxReader.ReadInt();

                            tJail = new JailSystem( tID );
                            binReader.Seek( tPos, 0 );

                            try
                            {
                                tJail.Deserialize( binReader );
                                if( binReader.Position != ( tPos + tLength ) )
                                    throw new Exception( String.Format( "***** Bad serialize on {0} *****", tID ) );
                            }
                            catch( Exception ex )
                            {
                                Console.WriteLine( ex.ToString() );
                            }
                        }
                    }
                    Loadingameeditsettings( binReader );
                }
                finally
                {
                    idxFileStream.Close();
                    binFileStream.Close();
                }
            }
            else
            {
                DefaultSettings();
                Console.WriteLine( "{0}: No prior Jailsystem save, using default settings", JSName );
            }

            if( Config.Debug )
            {
                Server.Utility.PushColor( ConsoleColor.Cyan );
                Console.WriteLine( "Jail System loaded. Jailing counter is {0}.", jailCount );
                Server.Utility.PopColor();
            }
        }

        public static void Loadingameeditsettings( GenericReader idxReader )
        {
            try
            {
                int version;
                try
                {
                    version = idxReader.ReadInt();
                }

                catch( Exception err )
                {
                    DefaultSettings();
                    Console.WriteLine( "{0}: settings not found in save file, using default settings.",
                                       JSName );
                    Console.WriteLine( "{0}", err );
                    return;
                }
                switch( version )
                {
                    case 0:
                        try
                        {
                            JSName = idxReader.ReadString().Trim();
                            StatusCommand = idxReader.ReadString().Trim();
                            TimeCommand = idxReader.ReadString().Trim();
                            OoclistCommand = idxReader.ReadString().Trim();
                            FoulJailorName = idxReader.ReadString().Trim();
                            OocJailorName = idxReader.ReadString().Trim();
                            Oocwarns = idxReader.ReadInt();
                            JailMap = idxReader.ReadMap();
                            DefaultReleaseFacet = idxReader.ReadMap();
                            SingleFacetOnly = idxReader.ReadBool();
                            UseSmokingFootGear = idxReader.ReadBool();
                            UseLanguageFilter = idxReader.ReadBool();
                            AllowStaffBadWords = idxReader.ReadBool();
                            BlockOOCSpeech = idxReader.ReadBool();
                            UseOOCFilter = idxReader.ReadBool();
                            AllowStaffOOC = idxReader.ReadBool();
                            //load default releases
                            int temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                DefaultRelease.Add( idxReader.ReadPoint3D() );
                            }
                            //load cells
                            temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                Cells.Add( idxReader.ReadPoint3D() );
                            }
                            //load bad words
                            temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                BadWords.Add( idxReader.ReadString().Trim() );
                            }

                            //load ooc words
                            temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                OOCWords.Add( idxReader.ReadString().Trim() );
                            }

                            //load ooc part words
                            temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                OOCParts.Add( idxReader.ReadString().Trim() );
                            }

                            //load oocjail times
                            temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                OOCJailTimes.Add( idxReader.ReadTimeSpan() );
                            }

                            //load foul mouth jail times
                            temp = idxReader.ReadInt();
                            for( int i = 0; i < temp; i++ )
                            {
                                FoulMouthJailTimes.Add( idxReader.ReadTimeSpan() );
                            }
                        }
                        catch
                        {
                            DefaultSettings();
                            Console.WriteLine( "{0}: settings not found in save file, using default settings.",
                                               JSName );
                            return;
                        }
                        break;
                    case -1:
                        DefaultSettings();
                        break;
                    default:
                        Console.WriteLine( "{0} warning:{1}-{2}", JSName, "Loading-", "Unknown version" );
                        break;
                }
            }
            catch( Exception err )
            {
                DefaultSettings();
                Console.WriteLine( "{0}: settings not found in save file, using default settings:",
                                   JSName );
                Console.WriteLine( "{0}", err );
                return;
            }
        }

        public static void Saveingameeditsettings( GenericWriter idxWriter )
        {
            idxWriter.Write( 0 ); //version#
            idxWriter.Write( JSName.Trim() );
            idxWriter.Write( StatusCommand.Trim() );
            idxWriter.Write( TimeCommand.Trim() );
            idxWriter.Write( OoclistCommand.Trim() );
            idxWriter.Write( FoulJailorName.Trim() );
            idxWriter.Write( OocJailorName.Trim() );
            idxWriter.Write( Oocwarns );
            idxWriter.Write( JailMap );
            idxWriter.Write( DefaultReleaseFacet );
            idxWriter.Write( SingleFacetOnly );
            idxWriter.Write( UseSmokingFootGear );
            idxWriter.Write( UseLanguageFilter );
            idxWriter.Write( AllowStaffBadWords );
            idxWriter.Write( BlockOOCSpeech );
            idxWriter.Write( UseOOCFilter );
            idxWriter.Write( AllowStaffOOC );
            idxWriter.Write( DefaultRelease.Count );
            foreach( Point3D p in DefaultRelease )
            {
                idxWriter.Write( p );
            }
            idxWriter.Write( Cells.Count );
            foreach( Point3D p in Cells )
            {
                idxWriter.Write( p );
            }

            idxWriter.Write( BadWords.Count );
            foreach( string s in BadWords )
            {
                idxWriter.Write( s.Trim() );
            }

            idxWriter.Write( OOCWords.Count );
            foreach( string s in OOCWords )
            {
                idxWriter.Write( s.Trim() );
            }

            idxWriter.Write( OOCParts.Count );
            foreach( string s in OOCParts )
            {
                idxWriter.Write( s.Trim() );
            }
            idxWriter.Write( OOCJailTimes.Count );
            foreach( TimeSpan t in OOCJailTimes )
            {
                idxWriter.Write( t );
            }
            idxWriter.Write( FoulMouthJailTimes.Count );
            foreach( TimeSpan t in FoulMouthJailTimes )
            {
                idxWriter.Write( t );
            }
        }

        public static void OnSave( WorldSaveEventArgs e )
        {
            WorldSaveProfiler.Instance.StartHandlerProfile( OnSave );

            //			Console.WriteLine("Saving Jailings");
            if( !Directory.Exists( JailDirectory ) )
                Directory.CreateDirectory( JailDirectory );
            GenericWriter idxWriter;
            GenericWriter binWriter;
            if( World.SaveType == 0 )
            {
                idxWriter = new BinaryFileWriter( IdxPath, false );
                binWriter = new BinaryFileWriter( BinPath, true );
            }
            else
            {
                idxWriter = new AsyncWriter( IdxPath, false );
                binWriter = new AsyncWriter( BinPath, true );
            }
            idxWriter.Write( JailSystemList.Count );
            try
            {
                foreach( JailSystem tJail in JailSystemList.Values )
                {
                    long tPos = binWriter.Position;
                    idxWriter.Write( 0 );
                    idxWriter.Write( tJail.ID );
                    idxWriter.Write( tPos );
                    try
                    {
                        tJail.Serialize( binWriter );
                    }
                    catch( Exception err )
                    {
                        Console.WriteLine( "{0}, {1} serialize", err.Message, err.TargetSite );
                    }
                    idxWriter.Write( (int)( binWriter.Position - tPos ) );
                }
                Saveingameeditsettings( binWriter );
            }
            catch( Exception er )
            {
                Console.WriteLine( "{0}, {1}", er.Message, er.TargetSite );
            }

            idxWriter.Close();
            binWriter.Close();

            WorldSaveProfiler.Instance.EndHandlerProfile();
        }

        public void BuildJail()
        {
            ID = m_NextID;
            ReleaseDate = DateTime.Now.AddDays( 1 );
            m_NextID += 1;
            JailSystemList.Add( ID, this );
            Jailor = JSName;
            m_JailorAC = AccessLevel.Counselor;
            Reason = "Please wait while the gm fills in a jailing report";
        }

        public void AddNote( string from, string text )
        {
            Prisoner.Comments.Add( new AccountComment( JSName + "-note", text + " by: " + from ) );
        }

        public void FillJailReport( Mobile badBoy, int days, int hours, int minutes, string why, bool mreturn, string jailor )
        {
            DateTime dtUnJail = DateTime.Now.Add( new TimeSpan( days, hours, minutes, 0 ) );
            FillJailReport( badBoy, dtUnJail, why, mreturn, jailor );
        }

        public void FillJailReport( Mobile badBoy, DateTime dtUnJail, string why, bool mreturn, string jailor )
        {
            Name = ( (Account)badBoy.Account ).Username;
            ReleaseDate = dtUnJail;
            Reason = why;
            Jailor = jailor;
            ( (Account)badBoy.Account ).Comments.Add(
                new AccountComment( JSName + "-jailed",
                                    "Jailed for \"" + why + "\" By:" + jailor + " On:" + DateTime.Now + " Until:" +
                                    dtUnJail ) );
            m_ReturnToPoint = mreturn;
            StartTimer();
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.Write( 2 );

            //version 2
            writer.Write( (int)m_JailorAC );

            //version 1
            writer.Write( FreedBy );

            //version 0 here
            writer.Write( Name );
            writer.Write( ReleaseDate );

            writer.Write( m_ReleasePoints.Count );
            foreach( ReleaseLoc rl in m_ReleasePoints.Values )
                rl.Serialize( writer );

            writer.Write( Jailor );
            writer.Write( Reason );
        }

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadInt();
            switch( version )
            {
                case 2:
                    m_JailorAC = (AccessLevel)reader.ReadInt();
                    goto case 1;
                case 1:
                    FreedBy = reader.ReadString().Trim();
                    goto case 0;
                case 0:
                    Name = reader.ReadString().Trim();
                    ReleaseDate = reader.ReadDateTime();
                    int imax = reader.ReadInt();
                    for( int i = 0; i < imax; i++ )
                    {
                        var rl = new ReleaseLoc();
                        rl.Deserialize( reader );

                        m_ReleasePoints.Add( rl.Mobile, rl );
                    }
                    Jailor = reader.ReadString().Trim();
                    Reason = reader.ReadString().Trim();
                    break;
                default:
                    break;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public void TimerRelease()
        {
            if( ReleaseDate <= DateTime.Now )
            {
                Release();
            }
            else
                Console.WriteLine(
                    "JailSystem: A Jail Timer fired but the timer was incorrect so the release was not honored." );
        }

        public void ForceRelease( Mobile releasor )
        {
            try
            {
                if( m_JailorAC > releasor.AccessLevel )
                {
                    releasor.SendLocalizedMessage( 1061637 );
                    return;
                }
            }
            catch( Exception err )
            {
                Console.WriteLine( "{0}: access level error, resume release-{1}", JSName,
                                   err );
            }
            FreedBy = releasor.Name + " (At:" + DateTime.Now + ")";
            try
            {
                if( m_AutoReleasor != null )
                    m_AutoReleasor.Stop();
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
            ReleaseDate = DateTime.Now.Subtract( new TimeSpan( 1, 0, 0, 0, 0 ) );
            Release();
        }

        public void Release( NetState ns )
        {
            try
            {
                if( m_AutoReleasor != null )
                {
                    if( m_AutoReleasor.Running )
                        m_AutoReleasor.Stop();
                    m_AutoReleasor = null;
                }
                try
                {
                    if( !( ns.Mobile.Region is Jail ) )
                        return;
                    ns.Mobile.SendLocalizedMessage( 501659 );
                }
                catch( Exception err )
                {
                    Console.WriteLine( "{0}: {1} Mobile not released", JSName, err );
                    return;
                }
                ReleaseLoc rl;
                try
                {
                    rl = m_ReleasePoints[ ns.Mobile.Serial.Value ];
                }
                catch
                {
                    rl = new ReleaseLoc();
                    rl.Mobile = ns.Mobile.Serial.Value;
                    m_ReleasePoints.Add( ns.Mobile.Serial.Value, rl );
                }
                if( rl.Release( FreedBy ) )
                    m_ReleasePoints.Remove( ns.Mobile.Serial.Value );
            }
            catch( Exception err )
            {
                Console.WriteLine( "{0}: {1}", JSName, err );
            }
            if( m_ReleasePoints.Count == 0 )
            {
                Console.WriteLine( "Jailing removed for account {0}", Name );
                try
                {
                    JailSystemList.Remove( ID );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        public void Ban( Mobile from )
        {
            try
            {
                Prisoner.Comments.Add(
                    new AccountComment( JSName + "-jailed",
                                        string.Format( "{0} banned this account on {1}.", from.Name,
                                                       DateTime.Now ) ) );
                Prisoner.Banned = true;
                CommandLogging.WriteLine( from, "{0} {1} {3} account {2}", from.AccessLevel,
                                          CommandLogging.Format( from ),
                                          Prisoner.Username, Prisoner.Banned ? "banning" : "unbanning" );
                JailSystemList.Remove( ID );
            }
            catch
            {
                from.SendMessage(
                    "Banning Failed.  If you are trying to remove the jailing release the person, or use 'killjail {0}'",
                    ID );
                from.SendMessage( "Using killjail on an unbanned account can cause problems for that account." );
            }
        }

        public static void KillJailing( int tID )
        {
            JailSystemList.Remove( tID );
        }

        public void Release()
        {
            try
            {
                if( m_AutoReleasor != null )
                {
                    if( m_AutoReleasor.Running )
                        m_AutoReleasor.Stop();
                }
            }
            catch( Exception err )
            {
                Console.WriteLine( "{0}: auto releasor not found-{1}", JSName, err );
            }
            try
            {
                VerifyMobs();
            }
            catch( Exception err )
            {
                Console.WriteLine( "{0}: Verify Mobiles failed-{1}", JSName, err );
            }
            try
            {
                foreach( NetState ns in NetState.Instances )
                {
                    if( ( (Account)ns.Account ).Username == Name )
                    {
                        Release( ns );
                    }
                }
            }
            catch( Exception err )
            {
                Console.WriteLine(
                    "{0}: Release failed-{1} **The most common occurance of this is when an account has been deleted while in jail ***Use the adminjail command to cycle through the jailings and automaticly remove them.",
                    JSName, err );
            }
            if( m_ReleasePoints.Count == 0 )
            {
                try
                {
                    JailSystemList.Remove( ID );
                    Console.WriteLine( "Jailing removed for account {0}", Name );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        public void VerifyMobs()
        {
            var temp = new List<ReleaseLoc>();
            foreach( ReleaseLoc r in m_ReleasePoints.Values )
            {
                try
                {
                    Mobile m = World.FindMobile( r.Mobile );
                    if( m == null )
                    {
                        temp.Add( r );
                    }
                }
                catch
                {
                    temp.Add( r );
                }
            }
            foreach( ReleaseLoc r in temp )
            {
                m_ReleasePoints.Remove( r.Mobile );
            }
        }

        public void KillJail()
        {
            if( Prisoner == null )
                JailSystemList.Remove( ID );
        }

        public void LockupMobile( Mobile m )
        {
            LockupMobile( m, true );
        }

        public void LockupMobile( Mobile m, bool useFootWear )
        {
            if( !m_ReleasePoints.ContainsKey( m.Serial.Value ) )
                m_ReleasePoints.Add( m.Serial.Value, new ReleaseLoc( m.Location, m.Map, m.Serial.Value, m_ReturnToPoint ) );

            m.SendMessage( "While in jail, you can say \"{0}\" at any time to check your jail status.", StatusCommand );

            #region mod by Dies Irae
            if( m is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)m ).ShrinkAllPets();

            if( m.NetState == null && m.Map == Map.Felucca )
                m.Map = Map.Internal;
            #endregion

            if( m.Region is Jail )
                return;

            Point3D cell = Cells[ ( ( new Random() ).Next( 0, Cells.Count - 1 ) ) ];

            if( ( UseSmokingFootGear ) && ( useFootWear ) )
                new SmokingFootGear( m );

            m.Location = cell;
            m.Map = JailMap;
        }

        public void StartTimer()
        {
            if( m_AutoReleasor != null )
            {
                if( m_AutoReleasor.Running )
                    m_AutoReleasor.Stop();
                m_AutoReleasor = null;
            }
            if( !Jailed )
            {
                Release();
                return;
            }
            m_AutoReleasor = new Jailing( this );
            m_AutoReleasor.Start();
        }

        public void ResetReleaseDateOneDay()
        {
            ReleaseDate = DateTime.Now.AddDays( 1 );
            StartTimer();
        }

        public void ResetReleaseDateNow()
        {
            ReleaseDate = DateTime.Now;
        }

        #region time managment
        public void AddDays( int days )
        {
            ReleaseDate = ReleaseDate.AddDays( days );
            StartTimer();
        }

        public void AddMonths( int months )
        {
            ReleaseDate = ReleaseDate.AddMonths( months );
            StartTimer();
        }

        public void AddHours( int hours )
        {
            ReleaseDate = ReleaseDate.AddHours( hours );
            StartTimer();
        }

        public void AddMinutes( int minutes )
        {
            ReleaseDate = ReleaseDate.AddMinutes( minutes );
            StartTimer();
        }

        public void SubtractDays( int days )
        {
            RemoveTime( days, 0, 0 );
        }

        public void SubtractHours( int hours )
        {
            RemoveTime( 0, hours, 0 );
        }

        public void SubtractMinutes( int minutes )
        {
            RemoveTime( 0, 0, minutes );
        }

        public void RemoveTime( int days, int hours, int minutes )
        {
            ReleaseDate = ReleaseDate.Subtract( new TimeSpan( days, hours, minutes, 0, 0 ) );
            StartTimer();
        }
        #endregion

        public static void EventSink_Speech( SpeechEventArgs args )
        {
            Mobile mob = args.Mobile;
            if( mob == null || !mob.Player )
                return;

            if( !( mob.Region is Jail ) )
                return;

            if( Insensitive.Equals( args.Speech, StatusCommand ) )
            {
                JailSystem js = FromMobile( mob );
                if( js != null )
                {
                    mob.SendMessage( "You were jailed by: {0}", js.Jailor );
                    mob.SendMessage( "You were jailed for: {0}", js.Reason );
                    mob.SendMessage( "You are to be released at: {0}", js.ReleaseDate.ToString() );
                }
                else
                {
                    mob.SendMessage( "You are missing a jailing object, page for Dies Irae" );
                }

                args.Blocked = true;
                return;
            }
        }

        public static void OnLoginJail( LoginEventArgs e )
        {
            if( !TimersRunning ) //start the timers on the first user to login
            {
                TimersRunning = true; //so no-one else causes the process to run
                bool loopdone = false;
                while( !loopdone )
                {
                    try
                    {
                        foreach( JailSystem tjs in JailSystemList.Values )
                            tjs.StartTimer();
                        loopdone = true;
                    }
                    catch( Exception err )
                    {
                        Console.WriteLine( "Restarting the Jail timer load process:{0}", err.Message );
                    }
                }
                Console.WriteLine( "The Jail timer load process has finished" );
            }
            if( e.Mobile == null )
                return;
            JailSystem js = FromMobile( e.Mobile ); //check to see if they have a jail object
            if( js == null )
            {
                #region mod by Dies Irae

                if( e.Mobile.Region.IsPartOf( "Jail" ) )
                {
                    e.Mobile.MoveToWorld( new Point3D( 2230, 1224, 0 ), Map.Felucca );

                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/JailForcedRelease.log" );
                        tw.WriteLine( "Account {0} - DateTime {1}", e.Mobile.Account.Username, DateTime.Now );
                        tw.Close();
                    }
                    catch( Exception ex )
                    {
                        Console.Write( "Log failed: {0}", ex );
                    }
                }

                #endregion

                return; //they don’t so we bail
            }

            if( js.Jailed ) //are they jailed?
            {
                js.LockupMobile( e.Mobile ); //yup so lock them up
            }
            else
                js.Release( e.Mobile.NetState ); //no so we release them
        }

        public static void OOCWarn( Mobile m, string s )
        {
            if( !Warns.Contains( ( (Account)m.Account ).Username ) )
            {
                Warns.Add( ( (Account)m.Account ).Username, new Hashtable() );
            }
            var w = (Hashtable)Warns[ ( (Account)m.Account ).Username ];
            if( w == null )
            {
                Jail( m, false, "Going OOC, warning system was unable to issue a warning and jailed you.",
                      true, "OOC Jailor" );
            }
            DateTime k = DateTime.Now;
            int i = 0;
            if( w != null )
                while( w.Contains( k ) )
                {
                    k = k.Subtract( new TimeSpan( 0, 0, 1 ) );
                    if( i > 10 )
                        continue;
                    i++;
                }
            if( i <= 10 )
            {
                if( w != null )
                    w.Add( k, s );
                new WarnRemover( w, k );
            }
            if( w != null )
                if( w.Count > Oocwarns )
                {
                    Jail( m, false, "Going OOC after repeated warnings.", true, OocJailorName );
                    Warns.Remove( ( (Account)m.Account ).Username );
                }
                else
                {
                    m.SendMessage( k.ToString() );
                    m.SendMessage(
                        "'{0}' is an out of character term.  Going ooc too much can land you in Jail.  For a list of ooc words say '{1}'",
                        s, OoclistCommand );
                }
        }

        public static TimeSpan GetJailTerm( Mobile m, bool foul )
        {
            int visits = CountJailings( m, foul );
            if( foul )
                return GetFoulJailTerm( visits );
            else
                return GetOOCJailTerm( visits );
        }

        public static TimeSpan GetOOCJailTerm( int visits )
        {
            OOCJailTimes.Sort();
            if( visits >= OOCJailTimes.Count )
                visits = OOCJailTimes.Count - 1;
            if( visits < 0 )
                visits = 0;
            return OOCJailTimes[ visits ];
        }

        public static TimeSpan GetFoulJailTerm( int visits )
        {
            FoulMouthJailTimes.Sort();
            if( visits >= FoulMouthJailTimes.Count )
                visits = FoulMouthJailTimes.Count - 1;
            if( visits < 0 )
                visits = 0;
            return FoulMouthJailTimes[ visits ];
        }

        public static int CountJailings( Mobile m, bool foul )
        {
            return CountJailings( ( (Account)m.Account ), foul );
        }

        public static int CountJailings( Account a, bool foul )
        {
            int foulCt = 0;
            int oocCt = 0;
            foreach( AccountComment note in a.Comments )
            {
                if( note.Content.IndexOf( " By:" + OocJailorName ) >= 0 )
                    oocCt++;
                if( note.Content.IndexOf( " By:" + FoulJailorName ) >= 0 )
                    foulCt++;
            }
            if( foul )
                return foulCt;
            else
                return oocCt;
        }

        internal static void RegisterCommands()
        {
            CommandSystem.Register( "Unjail", AccessLevel.GameMaster, new CommandEventHandler( UnjailOnCommand ) );
            CommandSystem.Register( "Release", AccessLevel.GameMaster, new CommandEventHandler( UnjailOnCommand ) );
            CommandSystem.Register( "Jail", AccessLevel.Counselor, new CommandEventHandler( JailOnCommand ) );
            CommandSystem.Register( "Review", AccessLevel.Counselor, new CommandEventHandler( ReviewOnCommand ) );
            CommandSystem.Register( "Warn", AccessLevel.Counselor, new CommandEventHandler( WarnOnCommand ) );
            CommandSystem.Register( "Macro", AccessLevel.Counselor, new CommandEventHandler( MacroCheckOnCommand ) );
            CommandSystem.Register( "AdminJail", AccessLevel.Administrator, new CommandEventHandler( AdminJailOnCommand ) );
            CommandSystem.Register( "KillJail", AccessLevel.Administrator, new CommandEventHandler( KillJail_OnCommand ) );
            CommandSystem.Register( "Cage", AccessLevel.GameMaster, new CommandEventHandler( CageOnCommand ) );
        }

        internal static void InitSystem()
        {
            EventSink.Login += new LoginEventHandler( OnLoginJail );
            EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
        }

        public static void GetContextMenus( Mobile from, Mobile player, List<ContextMenuEntry> list )
        {
            if( from.AccessLevel >= AccessLevel.Counselor )
                list.Add( new JailEntry( from, player ) );
            if( from.AccessLevel >= AccessLevel.GameMaster )
                list.Add( new UnJailEntry( from, player ) );
            if( from.AccessLevel >= AccessLevel.Counselor )
                list.Add( new ReviewEntry( from, player ) );
            if( from.AccessLevel >= AccessLevel.Counselor )
                list.Add( new MacroerEntry( from, player ) );
        }

        public static void GetSelfContextMenus( Mobile from, Mobile player, List<ContextMenuEntry> list )
        {
            if( from.Region is Jail )
                list.Add( new CallbackPlayerEntry( 1056, new ContextPlayerCallback( GetJailInfo ), player ) ); // Get info on my Jail status
        }

        /// <summary>
        /// Metodo invocato quando un player vuole vedere le info sulla sua Jail
        /// </summary>
        private static void GetJailInfo( Mobile from )
        {
            if( !( from.Region is Jail ) )
                return;

            JailSystem js = FromMobile( from );
            if( js != null )
            {
                StringBuilder s = new StringBuilder();
                s.Append( "You are in Midgard jails.<br>" );
                if( !string.IsNullOrEmpty( js.Jailor ) )
                    s.AppendFormat( "You were jailed by {0}.<br>", js.Jailor );
                if( !string.IsNullOrEmpty( js.Reason ) )
                    s.AppendFormat( "You were jailed for: {0}.<br>", js.Reason );
                s.AppendFormat( "You are to be released at: {0}.", js.ReleaseDate );

                from.SendGump( new NoticeGump( 1060635, 30720, s.ToString(),
                                         0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeGumpCallBack ), null ) );
            }
        }

        private static void CloseNoticeGumpCallBack( Mobile @from, object state )
        {
        }

        #region command callbacks
        [Usage( "AdminJail" )]
        [Description( "Displays the jail sentence gump." )]
        public static void AdminJailOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.SendGump( new JailAdminGump() );
            }
        }

        [Usage( "KillJail <ID>" )]
        [Description( "Deletes the jailing with the specified ID.  Used only to recover from deadly errors" )]
        public static void KillJail_OnCommand( CommandEventArgs e )
        {
            try
            {
                int tID = Convert.ToInt32( e.ArgString.Trim() );
                KillJailing( tID );
            }
            catch( Exception err )
            {
                e.Mobile.SendMessage( "Kill jailing failed: {0}", err.ToString() );
            }
        }

        [Usage( "UnJail" )]
        [Description( "Releases the selected player from jail." )]
        public static void UnjailOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new JailTarget( true );
                e.Mobile.SendLocalizedMessage( 3000218 );
            }
        }

        [Usage( "Cage" )]
        [Description( "places a cage around the target." )]
        public static void CageOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new CageTarget();
                e.Mobile.SendMessage( (e.Mobile.Language == "ITA" ? "Chi vorresti ingabbiare?" : "Who would you like to cage?") );
            }
        }

        [Usage( "Macro" )]
        [Description( "Issues a macroing check dialog." )]
        public static void MacroCheckOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new MacroTarget();
                e.Mobile.SendLocalizedMessage( 3000218 );
            }
        }

        [Usage( "Jail" )]
        [Description( "Places the selected player in jail." )]
        public static void JailOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new JailTarget( false );
                e.Mobile.SendLocalizedMessage( 3000218 );
            }
        }

        [Usage( "Warn" )]
        [Description( "Issues a warning to the player." )]
        public static void WarnOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new WarnTarget();
                e.Mobile.SendLocalizedMessage( 3000218 );
            }
        }

        [Usage( "Review" )]
        [Description( "Reviews the jailings, GM notes and warnings of the selected player." )]
        public static void ReviewOnCommand( CommandEventArgs e )
        {
            if( e.Mobile is PlayerMobile )
            {
                e.Mobile.Target = new WarnTarget( false );
                e.Mobile.SendLocalizedMessage( 3000218 );
            }
        }
        #endregion

        public static void MacroTest( Mobile from, Mobile badBoy )
        {
            if( badBoy.NetState == null )
            {
                from.SendMessage( "They are not online." );
                return;
            }
            badBoy.SendGump( new UnattendedMacroGump( from, badBoy ) );
        }

        public static void NewJailingFromGMandPlayer( Mobile from, Mobile m )
        {
            JailSystem js = FromMobile( m );
            if( js == null )
            {
                js = new JailSystem();
            }
            else
            {
                if( js.Jailed )
                {
                    from.SendMessage( "{0} is already jailed", m.Name );
                    return;
                }
            }

            js.ResetReleaseDateOneDay();
            js.LockupMobile( m );
            js.Jailor = from.Name;

            m.SendLocalizedMessage( m.Equals( from ) ? 503315 : 503316 );

            from.SendGump( ( new JailGump( js, from, m, 0, "", "" ) ) );
        }

        public static void DoJailAccount( Mobile from, Account a, out string notice )
        {
            if( a == null || a.Count == 0 )
            {
                notice = "No mobile can be jailed in this account.";
                return;
            }

            JailSystem js = FromAccount( a );
            Mobile m = a[ 0 ];

            if( js == null )
                js = new JailSystem();
            else
            {
                if( js.Jailed )
                {
                    notice = "That account is already jailed";
                    return;
                }
            }

            js.ResetReleaseDateOneDay();
            js.LockupMobile( m );
            js.Jailor = from != null ? from.Name : "Midgard Staff";
            if( from != null )
                from.SendGump( ( new JailGump( js, from, m, 0, "", "" ) ) );

            notice = "Jail process started.";
        }

        public static void DoReleaseAccount( Mobile from, Account a, out string notice )
        {
            if( a == null )
            {
                notice = "Invalid account.";
                return;
            }

            JailSystem js = FromAccount( a );
            if( js == null )
            {
                notice = "That is not jailed.";
                return;
            }
            else
            {
                if( js.Jailed )
                {
                    js.ForceRelease( from );
                    notice = "That account has been unjailed.";
                }
                else
                {
                    notice = "That is already unjailed.";
                }
            }
        }

        public class Jailing : Timer
        {
            public JailSystem Prisoner;

            public Jailing( JailSystem js )
                : base( js.ReleaseDate.Subtract( DateTime.Now ) )
            {
#if DiesDebugTimers
                Priority = TimerPriority.OneSecond;
#endif
                Prisoner = js;
            }

            protected override void OnTick()
            {
                Prisoner.TimerRelease();
            }
        }

        public class ReleaseLoc
        {
            private Point3D m_Location;
            private Map m_Map;
            private int m_Mobile;
            private bool m_ReturnToPoint;

            public ReleaseLoc()
            {
                m_ReturnToPoint = false;
            }

            public ReleaseLoc( bool rel2JailPoint )
            {
                m_ReturnToPoint = rel2JailPoint;
            }

            public ReleaseLoc( Point3D loc, Map m, int mob, bool rel2JailPoint )
            {
                m_Location = loc;
                m_Map = m;
                m_Mobile = mob;
                m_ReturnToPoint = rel2JailPoint;
            }

            public Point3D Location
            {
                get { return m_Location; }
            }

            public Map Map
            {
                get { return m_Map; }
            }

            public int Mobile
            {
                get { return m_Mobile; }
                set { m_Mobile = value; }
            }

            public bool ReturnToPoint
            {
                get { return m_ReturnToPoint; }
            }

            public bool Release( string releasor )
            {
                Mobile m = World.FindMobile( Mobile );
                if( m == null )
                {
                    Console.WriteLine( "release location error, Mobile not found." );
                    return false;
                }
                if( !ReturnToPoint )
                {
                    //not returning to the jailing point so rewrites this release point info
                    if( SingleFacetOnly )
                        m_Map = DefaultReleaseFacet;
                    else
                    {
                        if( m.Kills >= 5 )
                            m_Map = Map.Felucca;
                        else
                            m_Map = Map.Felucca;
                    }
                    m_Location =
                        DefaultRelease[ ( new Random() ).Next( 0, DefaultRelease.Count - 1 ) ];
                }
                try
                {
                    m.Location = Location;
                    m.Map = Map;
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
                if( m.Region is Jail )
                {
                    try
                    {
                        ( (Account)m.Account ).Comments.Add(
                            new AccountComment( JSName,
                                                releasor + "'s release Failed for " + m.Name + "(" +
                                                ( (Account)m.Account ).Username + ") at " + DateTime.Now + " to " +
                                                Location + "(" + Map + ")" ) );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                    return false;
                }
                else
                {
                    try
                    {
                        ( (Account)m.Account ).Comments.Add(
                            new AccountComment( JSName,
                                                releasor + " released " + m.Name + "(" + ( (Account)m.Account ).Username +
                                                ") at " + DateTime.Now + " to " + Location + "(" + Map + ")" ) );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                    return true;
                }
            }

            public void Serialize( GenericWriter writer )
            {
                writer.Write( m_Map );
                writer.Write( m_Location );
                writer.Write( m_Mobile );
                writer.Write( m_ReturnToPoint );
            }

            public void Deserialize( GenericReader reader )
            {
                m_Map = reader.ReadMap();
                m_Location = reader.ReadPoint3D();
                m_Mobile = reader.ReadInt();
                m_ReturnToPoint = reader.ReadBool();
            }
        }

        public class WarnRemover : Timer
        {
            public Hashtable IssuedWarnings;
            public DateTime Key;

            public WarnRemover( Hashtable w, DateTime k )
                : base( new TimeSpan( 1, 0, 0, 0 ) )
            {
                Key = k;
                IssuedWarnings = w;
                Start();
            }

            protected override void OnTick()
            {
                if( IssuedWarnings.Contains( Key ) )
                {
                    IssuedWarnings.Remove( Key );
                }
            }
        }
    }
}