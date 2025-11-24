/***************************************************************************
 *                               AcademyPlayerState.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Server;
using Server.Accounting;
using Server.Mobiles;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.Academies
{
    [PropertyObject]
    public class AcademyPlayerState : IComparable
    {
        private string m_CustomAcademyTitle;
        private AcademyAccessLevel m_TownAccessLevel;
        private AcademyOffices m_AcademyOffice;
        private string m_CustomAcademyOffice;

        public AcademyPlayerState( AcademySystem system, Mobile mobile )
        {
            Academy = system;
            Mobile = mobile;

            Init();
            Attach();
            Invalidate();
        }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public AcademySystem Academy { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public Mobile Mobile { get; private set; }

        #region [Training & Teaching]
        public List<TrainingState> Trainings { get; private set; }

        public bool BeginLearning( Disciplines discipline )
        {
            if( Trainings == null )
                Trainings = new List<TrainingState>();

            if( IsLearning( discipline ) )
                return false;

            TrainingState state = TrainingState.GetInstance( discipline );
            state.IsLearning = true;

            Trainings.Add( state );
            return true;
        }

       public bool BeginTeaching( Disciplines discipline )
        {
            if( Trainings == null )
                Trainings = new List<TrainingState>();

            TrainingState state = TrainingState.GetInstance( discipline );
            state.IsTeaching = true;

            Trainings.Add( state );
            return true;
        }

        public void EndLearning( Disciplines discipline )
        {
            if( Trainings == null )
                return;

            if( !IsLearning( discipline ) )
                return;

            TrainingState state = GetTraining( discipline );
            if( state != null )
                Trainings.Remove( state );
        }

        public void EndTeaching( Disciplines discipline )
        {
            if( Trainings == null )
                return;

            TrainingState state = GetTraining( discipline );
            if( state != null )
                Trainings.Remove( state );
        }

        public bool AddTraining( TrainingState state )
        {
            if( Trainings == null )
                Trainings = new List<TrainingState>();

            if( !Trainings.Contains( state ) )
            {
                Trainings.Add( state );
                return true;
            }
            else
                return false;
        }

        public bool RemoveTraining( TrainingState state )
        {
            if( Trainings == null )
                return true;

            if( Trainings.Contains( state ) )
            {
                Trainings.Remove( state );
                return true;
            }
            else
                return false;
        }

        public bool HasTraining( TrainingState state )
        {
            if( Trainings == null )
                return false;

            return Trainings.Contains( state );
        }

        public bool IsLearning( Disciplines discipline )
        {
            if( Trainings == null )
                return false;

            foreach( TrainingState state in Trainings )
            {
                if( state.Discipline == discipline )
                    return state.IsLearning;
            }

            return false;
        }

        public TrainingState GetTraining( Disciplines discipline )
        {
            if( Trainings == null )
                return null;

            foreach( TrainingState state in Trainings )
            {
                if( state.Discipline == discipline )
                    return state;
            }

            return null;
        }
        #endregion

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Seer )]
        public string CustomAcademyTitle
        {
            get { return m_CustomAcademyTitle; }
            set
            {
                m_CustomAcademyTitle = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsInactive
        {
            get
            {
                if( Mobile != null )
                {
                    Account acct = Mobile.Account as Account;
                    if( acct != null )
                    {
                        if( ( acct.LastLogin + TimeSpan.FromDays( 60.0 ) ) > DateTime.Now )
                            return false;
                    }
                }

                return true;
            }
        }

        public bool ShowsTitle
        {
            get { return Academy.ShowsTitle; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DisplayAcademyStatus
        {
            get { return Mobile is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)Mobile ).DisplayAcademyStatus; }
            set
            {
                if( Mobile is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)Mobile ).DisplayAcademyStatus = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public AcademyAccessLevel AcademyLevel
        {
            get
            {
                if( Mobile != null )
                {
                    if( Mobile.AccessLevel > AccessLevel.Counselor )
                        return AcademyAccessLevel.Staff;
                }

                return m_TownAccessLevel;
            }
            set
            {
                m_TownAccessLevel = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public AcademyOffices AcademyOffice
        {
            get { return m_AcademyOffice; }
            set
            {
                m_AcademyOffice = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public string CustomAcademyOffice
        {
            get { return m_CustomAcademyOffice; }
            set
            {
                m_CustomAcademyOffice = value;
                Invalidate();
            }
        }

        #region IComparable Members
        public int CompareTo( object obj )
        {
            return 0; // Def - ( (AcademyPlayerState)obj ).GetStatePower();
        }
        #endregion

        #region serialization
        public AcademyPlayerState( AcademySystem system, GenericReader reader )
        {
            Academy = system;
            Init();

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    Mobile = reader.ReadMobile();

                    m_CustomAcademyTitle = reader.ReadString();
                    break;
            }

            if( Mobile == null )
            {
                Config.Pkg.LogInfoLine( "Warning: AcademyPlayerState with null mobile detected. Removing..." );
                Timer.DelayCall( TimeSpan.Zero, Detach );
            }
            else
                Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckAttach ) );
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            // Version 0
            writer.Write( Mobile );

            writer.Write( m_CustomAcademyTitle );
        }
        #endregion

        #region [Find...]
        public static AcademyPlayerState Find( Mobile mob )
        {
            return Find( mob, false );
        }

        public static AcademyPlayerState Find( Mobile mob, bool inherit )
        {
            Midgard2PlayerMobile player = mob as Midgard2PlayerMobile;

            if( player == null )
            {
                if( inherit && mob is BaseCreature )
                {
                    BaseCreature bc = mob as BaseCreature;

                    if( bc.Controlled )
                        player = bc.ControlMaster as Midgard2PlayerMobile;
                    else if( bc.Summoned )
                        player = bc.SummonMaster as Midgard2PlayerMobile;
                }

                if( player == null )
                    return null;
            }

            AcademyPlayerState state = player.AcademyState;

            if( state != null && !state.Academy.IsEligible( state.Mobile ) )
                player.AcademyState = state = null;

            return state;
        }
        #endregion

        public void Invalidate()
        {
            if( Mobile is PlayerMobile )
                Mobile.InvalidateProperties();
        }

        public void Init()
        {
            m_CustomAcademyTitle = String.Empty;
            m_TownAccessLevel = AcademyAccessLevel.Academic;
        }

        public void CheckAttach()
        {
            if( Academy != null && Academy.IsEligible( Mobile ) )
                Attach();
        }

        public void Attach()
        {
            if( Academy == null )
                Config.Pkg.LogInfoLine( "Warning: class player state with null class system. Removing from class system..." );
            else if( Mobile == null )
                Config.Pkg.LogInfoLine( "Warning: class player state with null mobile. Removing from class system..." );
            else if( Academy.GetMobilesFromStates().Contains( Mobile ) )
                Config.Pkg.LogInfoLine( "Warning: playerstate (mobile {0}) is already in {1} class system. Removing from class system...", Mobile.Name, Academy.Definition.AcademyName );
            else
            {
                if( Mobile is Midgard2PlayerMobile )
                {
                    ( (Midgard2PlayerMobile)Mobile ).AcademyState = this;
                    Academy.Players.Add( this );

                    Academy.OnAcademySystemJoined( this );
                }
            }
        }

        public void Detach()
        {
            if( Mobile != null && Mobile is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)Mobile ).AcademyState = null;

            Academy.Players.Remove( this );
        }

        public override string ToString()
        {
            return "...";
        }

        public XElement ToXElement()
        {
            return new XElement( "state", new XAttribute( "owner", Utility.SafeString( Mobile.Name ?? "" ) ),
                                 new XAttribute( "account", Utility.SafeString( Mobile.Account.Username ?? "" ) ),
                                 new XAttribute( "creation", Mobile.CreationTime.ToString() ),
                                 new XAttribute( "title", Utility.SafeString( m_CustomAcademyTitle ?? "" ) ) );
        }
    }
}