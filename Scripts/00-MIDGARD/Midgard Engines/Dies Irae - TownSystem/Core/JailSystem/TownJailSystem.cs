using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownJailSystem
    {
        #region Singleton pattern
        private static TownJailSystem m_Instance;

        public static TownJailSystem Instance
        {
            get
            {
                if( m_Instance == null )
                {
                    m_Instance = new TownJailSystem();
                }
                return m_Instance;
            }
        }
        #endregion

        public static Map JailsMap = Map.Felucca;
        public static Point3D JailReleaseLocation = new Point3D( 535, 992 + 3, 15 );

        public static string JailMessage = "Thou have been condamned for a violation of the Laws of {0}.\n" +
                                    "The crimes you have been condamned for are:\n" +
                                    "{1}\n" +
                                    "The sheriff or one of his patrol guards reported your last crime " +
                                    "and you have been deported to the Prisons of Yew.\n\n" +
                                    "*The Town of {0}*";

        public static string ReleaseMessage = "{0}, for your crimes, you have been condamned to jail.\n" +
                                    "Your detenction ends now: you are free.\n" +
                                    "All your belongings have been located in your bankbox.\n\n" +
                                    "* The Town of {1} *";

        private static readonly List<JailCell> m_Cells = new List<JailCell>();

        private static List<Condemn> m_Condemns = new List<Condemn>();

        private static readonly string TownJailSystemSavePath = Path.Combine( Path.Combine( "Saves", "TownSystem" ), "TownJailSystemSave.bin" );

        public static void CheckRelease_OnTick()
        {
            if( m_Condemns != null && m_Condemns.Count > 0 )
            {
                foreach( Condemn t in m_Condemns )
                    t.CheckRelease();
            }

            foreach( JailCell t in m_Cells )
                t.CheckRelease();
        }

        public static void EnsureExistence()
        {
            if( m_Instance == null )
                m_Instance = new TownJailSystem();
        }

        public static void ConfigSystem()
        {
            EnsureExistence();

            // EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            // EventSink.WorldSave += new WorldSaveEventHandler( Save );

            Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), TimeSpan.FromMinutes( 1.0 ), new TimerCallback( CheckRelease_OnTick ) );
        }

        public static void Load()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( TownJailSystemSavePath ) )
            {
                Config.Pkg.LogWarningLine( "Warning: {0} not found.", TownJailSystemSavePath );
                Config.Pkg.LogInfoLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( TownJailSystemSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            int condemnsCount = reader.ReadEncodedInt();

                            if( Config.Debug )
                                Config.Pkg.LogInfoLine( "Loading condemns..." );

                            if( m_Condemns == null )
                                m_Condemns = new List<Condemn>();

                            for( int i = 0; i < condemnsCount; i++ )
                                m_Condemns.Add( new Condemn( reader ) );

                            if( Config.Debug )
                                Config.Pkg.LogInfoLine( "Loading profiles..." );

                            foreach( TownSystem system in TownSystem.TownSystems )
                            {
                                int profilesCount = reader.ReadEncodedInt();
                                if( profilesCount <= 0 )
                                    continue;

                                if( system.CriminalProfiles == null )
                                    system.CriminalProfiles = new List<CriminalProfile>();

                                for( int i = 0; i < profilesCount; ++i )
                                    system.CriminalProfiles.Add( new CriminalProfile( system, reader ) );
                            }

                            break;
                        }
                }

                bReader.Close();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( "Error de-serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogInfoLine( ex.ToString() );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "{0} System: Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( TownJailSystemSavePath ), Path.GetDirectoryName( TownJailSystemSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( TownJailSystemSavePath, true );

                writer.Write( 0 ); // version

                writer.WriteEncodedInt( m_Condemns.Count );
                foreach( Condemn t in m_Condemns )
                    t.Serialize( writer );

                foreach( TownSystem system in TownSystem.TownSystems )
                {
                    if( system.CriminalProfiles != null )
                    {
                        writer.WriteEncodedInt( system.CriminalProfiles.Count );

                        foreach( CriminalProfile t in system.CriminalProfiles )
                            t.Serialize( writer );
                    }
                    else
                        writer.WriteEncodedInt( 0 );
                }

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogErrorLine( "Error serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogErrorLine( ex.ToString() );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void WriteReference( GenericWriter writer, Condemn condemn )
        {
            int idx = m_Condemns.IndexOf( condemn );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static Condemn ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < m_Condemns.Count )
                return m_Condemns[ idx ];

            return null;
        }

        public static void InitSystem()
        {
            foreach( JailCell cell in m_Cells )
                cell.VerifyIntegrity();

            for( int i = 0; i < m_Condemns.Count; i++ )
            {
                Condemn condemn = m_Condemns[ i ];
                if( condemn.Profile != null )
                    continue;

                Config.Pkg.LogWarning( "Warning: Condemn with null profile in Town Jail System. Removing..." );
                m_Condemns.Remove( condemn );
            }

            foreach( Condemn condemn in m_Condemns )
                condemn.CheckRelease();
        }

        public bool HasNoMoreCondemns( Mobile prisoner )
        {
            if( m_Condemns == null )
                return true;

            bool canBeReleased = true;
            foreach( Condemn condemn in m_Condemns )
            {
                if( !condemn.Expired )
                    canBeReleased = false;
            }

            return canBeReleased;
        }

        public Condemn FindCondemnForPrisoner( Mobile prisoner )
        {
            if( m_Condemns == null )
                return null;

            foreach( Condemn condemn in m_Condemns )
            {
                if( condemn.Prisoner == prisoner )
                    return condemn;
            }

            return null;
        }

        public bool IsActuallyCondemned( Mobile prisoner )
        {
            return FindCondemnForPrisoner( prisoner ) != null;
        }

        public void RegisterCell( JailCell cell )
        {
            if( !m_Cells.Contains( cell ) )
                m_Cells.Add( cell );
        }

        public JailCell FindValidJailCell()
        {
            foreach( JailCell cell in m_Cells )
            {
                if( !cell.HasDoor )
                    continue;

                foreach( Condemn condemn in m_Condemns )
                {
                    if( condemn.AssignedCell == cell )
                        continue;
                }

                return cell;
            }

            return null;
        }

        public JailCell FindCellByName( string name )
        {
            if( m_Cells == null )
                return null;

            foreach( JailCell jailCell in m_Cells )
            {
                if( jailCell.Name == name )
                    return jailCell;
            }

            return null;
        }

        public TimeSpan GetCondemnDuration( CriminalProfile profile )
        {
            TimeSpan duration = TimeSpan.Zero;

            foreach( TownCrime crime in profile.Crimes )
                duration += crime.DefaultDuration;

            return duration;
        }

        public void Arrest( CriminalProfile profile, Mobile from )
        {
            Midgard2PlayerMobile criminal = profile.Criminal as Midgard2PlayerMobile;
            if( criminal == null || criminal.Deleted )
                return;

            JailCell cell = Instance.FindValidJailCell();
            if( cell == null )
            {
                from.SendMessage( "There are no free cells at the prison of Yew. Try again later." );
                return;
            }

            DateTime expirationTime = DateTime.Now + GetCondemnDuration( profile );

            Condemn condemn = new Condemn( profile, from, expirationTime, cell );
            condemn.Execute();

            profile.UpdateForCondemn( condemn );

            profile.SendCondemnMessage();
        }

        internal static void EffectCircle( IPoint3D center, Map map, int radius )
        {
            Point3D current = new Point3D( center.X + radius, center.Y, center.Z );

            for( int i = 0; i <= 360; i++ )
            {
                Point3D next = new Point3D( (int)Math.Round( Math.Cos( i ) * radius ) + center.X, (int)Math.Round( Math.Sin( i ) * radius ) + center.Y, current.Z );
                Effects.SendLocationEffect( next, map, 0x3728, 13 );
            }
        }

        public static bool RegisterCondemn( Condemn condemn )
        {
            if( m_Condemns == null )
                m_Condemns = new List<Condemn>();

            lock( m_Condemns )
            {
                if( !m_Condemns.Contains( condemn ) )
                {
                    m_Condemns.Add( condemn );

                    return true;
                }
            }

            return false;
        }

        public bool IsValidJailor( Mobile mobile )
        {
            if( mobile.AccessLevel > AccessLevel.GameMaster )
                return true;

            foreach( Condemn condemn in m_Condemns )
            {
                if( condemn.Jailor == mobile )
                    return true;
            }

            return false;
        }

        public Condemn FindCondemnByCrime( TownCrime crime )
        {
            if( m_Condemns == null )
                return null;

            foreach( Condemn condemn in m_Condemns )
            {
                if( condemn.Profile == null || condemn.Profile.Crimes == null )
                    continue;

                if( condemn.Profile.Crimes.Contains( crime ) )
                    return condemn;
            }

            return null;
        }
    }
}