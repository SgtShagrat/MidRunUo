/***************************************************************************
 *                               AcademySystem.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;

namespace Midgard.Engines.Academies
{
    public abstract class AcademySystem
    {
        #region instances
        public static readonly AcademySystem SerpentsHoldAcademy = new SerpentsHoldAcademy();

        public static readonly AcademySystem[] AcademySystems = new AcademySystem[]
                                                                {
                                                                    SerpentsHoldAcademy
                                                                };
        #endregion

        #region Is... something
        public static bool IsSerpentsHoldAcedemic( Mobile m )
        {
            AcademyPlayerState state = AcademyPlayerState.Find( m );
            return state != null && state.Academy == SerpentsHoldAcademy;
        }
        #endregion

        protected AcademySystem()
        {
            InitializeAcademySystem();
        }

        public AcademyDefinition Definition { get; protected set; }

        public virtual Disciplines[] Trainings { get { return null; } }

        public static string DefaultWelcomeMessage
        {
            get { return "Benvenuto in questa Accedemia.<br>"; }
        }

        public static void RegisterEventSink()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler( Load );
            EventSink.WorldSave += new WorldSaveEventHandler( Save );

            EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
        }

        public void InitializeAcademySystem()
        {
            m_Players = new AcademyPlayerStateCollection();
            m_Candidates = new List<Mobile>();
        }

        public static void EventSink_Speech( SpeechEventArgs args )
        {
            AcademySystem sys = Find( args.Mobile );
            if( sys != null && sys.HandlesOnSpeech )
                sys.OnSpeech( args );
        }

        public virtual bool HandlesOnSpeech { get { return false; } }

        public virtual void OnSpeech( SpeechEventArgs e )
        {
        }

        public virtual bool IsAllowedSkill( SkillName skillName )
        {
            return true;
        }

        public static bool CheckEquip( Mobile from, AcademySystem system, bool message )
        {
            if( Find( from ) != system )
            {
                if( message )
                    from.SendMessage( "Only a {0} can wear this.", system.Definition.AcademyName );

                return false;
            }
            else
                return true;
        }

        #region other
        public static int DisabledLabelHue = Colors.DimGray;

        public virtual bool IsMurdererAcademy
        {
            get { return false; }
        }

        public virtual bool IsEvilAlignedAcademy
        {
            get { return false; }
        }

        public virtual bool IsGoodAlignedAcademy
        {
            get { return false; }
        }

        public override string ToString()
        {
            return Definition.AcademyName;
        }

        public static AcademySystem Parse( string name )
        {
            if( String.IsNullOrEmpty( name ) )
                return null;

            foreach( AcademySystem t in AcademySystems )
            {
                if( t.Definition.AcademyName == name )
                    return t;
            }

            Config.Pkg.LogInfoLine( "Warning: Null Parse in AcademySystem.Parse. Name: {0}", name );

            return null;
        }

        public int CompareTo( object obj )
        {
            return Insensitive.Compare( Definition.AcademyName, ( (AcademySystem)obj ).Definition.AcademyName );
        }

        public static AcademySystem Find( Mobile mob )
        {
            return Find( mob, false, false );
        }

        public static AcademySystem Find( Mobile mob, bool inherit )
        {
            return Find( mob, inherit, false );
        }

        public static AcademySystem Find( Mobile mob, bool inherit, bool allegiance )
        {
            AcademyPlayerState pl = AcademyPlayerState.Find( mob );

            if( pl != null )
                return pl.Academy;

            if( inherit && mob is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)mob;

                if( bc.Controlled )
                    return Find( bc.ControlMaster, false );
                else if( bc.Summoned )
                    return Find( bc.SummonMaster, false );
            }

            return null;
        }

        public virtual bool IsEligible( Mobile mob )
        {
            return ( mob != null && mob is Midgard2PlayerMobile );
        }

        public virtual void OnAcademySystemInitialized()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "AcademySystem of {0} initializing...", Definition.AcademyName );

            VerifyPlayerStates();

            if( Config.Debug )
                Config.Pkg.LogInfo( "done\n" );
        }

        public virtual void OnAcademySystemJoined( AcademyPlayerState tps )
        {
        }
        #endregion

        #region serial-deserial
        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "{0} saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( Config.AcademySystemSavePath ), Path.GetDirectoryName( Config.AcademySystemSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( Config.AcademySystemSavePath, true );

                writer.Write( 0 ); // version

                writer.Write( AcademySystems.Length );

                foreach( AcademySystem system in AcademySystems )
                    system.Serialize( writer );

                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch( Exception ex )
            {
                Config.Pkg.LogInfoLine( "Error serializing {0}.", Config.Pkg.Title );
                Config.Pkg.LogInfoLine( ex.ToString() );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public static void Load()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( Config.AcademySystemSavePath ) )
            {
                Config.Pkg.LogInfoLine( "Warning: {0} not found.", Config.AcademySystemSavePath );
                Config.Pkg.LogInfoLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( Config.AcademySystemSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );

                int version = reader.ReadInt();

                switch( version )
                {
                    case 0:
                        {
                            int count = reader.ReadInt();

                            for( int i = 0; i < count; ++i )
                                AcademySystems[ i ].Deserialize( reader );

                            break;
                        }
                    default:
                        break;
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

        public virtual void Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        m_Candidates = reader.ReadStrongMobileList();

                        int playerCount = reader.ReadEncodedInt();

                        for( int i = 0; i < playerCount; ++i )
                            new AcademyPlayerState( this, reader );

                        break;
                    }
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( OnAcademySystemInitialized ) );
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            // Version 0
            writer.Write( m_Candidates, true );

            writer.WriteEncodedInt( m_Players.Count );

            foreach( AcademyPlayerState playerState in m_Players )
                playerState.Serialize( writer );
        }

        public static void WriteReference( GenericWriter writer, AcademySystem sys )
        {
            int idx = Array.IndexOf( AcademySystems, sys );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static AcademySystem ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < AcademySystems.Length )
                return AcademySystems[ idx ];

            return null;
        }
        #endregion

        #region player states
        private List<Mobile> m_Candidates;
        protected AcademyPlayerStateCollection m_Players;

        public virtual string JoinQuestion
        {
            get { return "Will thou join this Academy?"; }
        }

        public AcademyPlayerStateCollection Players
        {
            get { return m_Players; }
        }

        public List<Mobile> Candidates
        {
            get { return m_Candidates; }
        }

        public virtual bool ShowsTitle
        {
            get { return true; }
        }

        public static bool AcademyAccessEnabled { get; set; }

        public List<TrainingClass> TrainingClasses { get; set; }

        public bool AddClass( Disciplines discipline, string name )
        {
            if( TrainingClasses == null )
                TrainingClasses = new List<TrainingClass>();

            if( !HasClass( discipline ) )
            {
                TrainingClasses.Add( new TrainingClass( discipline, name ) );
                return true;
            }
            else
                return false;
        }

        public bool HasClass( Disciplines discipline )
        {
            foreach( TrainingClass trainingClass in TrainingClasses )
            {
                if( trainingClass.Discipline == discipline )
                    return true;
            }

            return false;
        }

        public bool IsMember( Mobile from )
        {
            AcademyPlayerState state = AcademyPlayerState.Find( from );
            return state != null && m_Players != null && m_Players.Contains( state );
        }

        public bool IsCandidate( Mobile from )
        {
            return m_Candidates != null && m_Candidates.Contains( from );
        }

        public void AddCandidate( Mobile m )
        {
            if( m_Candidates != null && m_Candidates.Contains( m ) )
                return;

            if( m_Candidates == null )
                m_Candidates = new List<Mobile>();

            m_Candidates.Add( m );
        }

        public void ClearInactiveStates()
        {
            List<AcademyPlayerState> list = GetInactiveStates( false );

            foreach( AcademyPlayerState state in list )
            {
                if( state == null || !state.IsInactive )
                    continue;

                Account a = state.Mobile.Account as Account;

                if( a != null )
                {
                    // TODO
                    // AcademyHelper.DoAccountReset( a );
                }
            }
        }

        public static int GetTotalAcademyedInAllSystems()
        {
            int counter = 0;
            foreach( AcademySystem t in AcademySystems )
            {
                if( t != null && t.Players != null )
                    counter += t.Players.Count;
            }

            return counter;
        }

        public List<AcademyPlayerState> GetInactiveStates( bool sort )
        {
            List<AcademyPlayerState> list = new List<AcademyPlayerState>();

            if( Players != null )
            {
                foreach( AcademyPlayerState state in Players )
                {
                    if( state.IsInactive )
                        list.Add( state );
                }
            }

            if( sort )
                list.Sort();

            return list;
        }

        public int GetInactiveStatesCount()
        {
            int counter = 0;
            foreach( AcademyPlayerState state in Players )
            {
                if( state.IsInactive )
                    counter++;
            }

            return counter;
        }

        public List<Mobile> GetMobilesFromStates()
        {
            List<Mobile> list = new List<Mobile>();

            foreach( AcademyPlayerState tps in Players )
            {
                if( tps.Mobile != null )
                    list.Add( tps.Mobile );
            }

            return list;
        }

        public static List<AcademyPlayerState> GetAllInactiveStates( bool sort )
        {
            List<AcademyPlayerState> list = new List<AcademyPlayerState>();

            foreach( AcademySystem t in AcademySystems )
            {
                list.AddRange( t.GetInactiveStates( false ) );
            }

            if( sort )
                list.Sort();

            return list;
        }

        public static List<AcademyPlayerState> GetPlayerStatesOnServer( bool sort )
        {
            List<AcademyPlayerState> list = new List<AcademyPlayerState>();

            foreach( AcademySystem t in AcademySystems )
            {
                if( t.Players != null )
                    list.AddRange( t.Players );
            }

            if( sort )
                list.Sort();

            return list;
        }

        private void VerifyPlayerStates()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "Verifying {0} class player states...", Definition.AcademyName );

            foreach( AcademyPlayerState state in Players )
            {
                if( state != null )
                {
                    // TODO Verify?
                }
            }

            if( Config.Debug )
                Config.Pkg.LogInfo( "done\n" );
        }
        #endregion

        public virtual void SetStartingSkills( Mobile mobile )
        {
        }
    }
}