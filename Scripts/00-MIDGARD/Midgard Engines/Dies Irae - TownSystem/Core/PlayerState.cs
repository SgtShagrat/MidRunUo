/***************************************************************************
 *                                  TownPlayerState.cs
 *                            		--------------------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Accounting;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownPlayerStateCollection : System.Collections.ObjectModel.Collection<TownPlayerState>
    {
    }

    [PropertyObject]
    public class TownPlayerState : IComparable
    {
        public static TownPlayerState Find( Mobile mob )
        {
            return Find( mob, false );
        }

        public static TownPlayerState Find( Mobile mob, bool inherit )
        {
            Midgard2PlayerMobile m2Pm = mob as Midgard2PlayerMobile;

            if( m2Pm == null )
            {
                if( inherit && mob is BaseCreature )
                {
                    BaseCreature bc = mob as BaseCreature;

                    if( bc.Controlled )
                        m2Pm = bc.ControlMaster as Midgard2PlayerMobile;
                    else if( bc.Summoned )
                        m2Pm = bc.SummonMaster as Midgard2PlayerMobile;
                }

                if( m2Pm == null )
                    return null;
            }

            TownPlayerState pl = m2Pm.TownState;

            if( pl != null && !pl.TownSystem.IsEligible( pl.Mobile ) )
                m2Pm.TownState = pl = null;

            return pl;
        }

        private TownOffices m_TownOffice;
        private TownAccessLevel m_TownAccessLevel;
        private string m_CustomTownOffice;
        private Professions m_TownProfession;
        private string m_CustomProfession;

        private int m_TownRankPoints;
        private int m_CitizenKills;
        private int m_Deaths;
        private List<KillPointsGivenEntry> m_KillsGiven;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public TownSystem TownSystem { get; private set; }

        public Mobile Mobile { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public TownOffices TownOffice
        {
            get { return m_TownOffice; }
            set
            {
                m_TownOffice = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public TownAccessLevel TownLevel
        {
            get
            {
                if( Mobile != null )
                {
                    if( Mobile.AccessLevel > AccessLevel.Counselor )
                        return TownAccessLevel.Staff;
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
        public string CustomTownOffice
        {
            get { return m_CustomTownOffice; }
            set
            {
                m_CustomTownOffice = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Professions TownProfession
        {
            get { return m_TownProfession; }
            set
            {
                m_TownProfession = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public string CustomProfession
        {
            get { return m_CustomProfession; }
            set
            {
                m_CustomProfession = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int TownRankPoints
        {
            get { return m_TownRankPoints; }
            set
            {
                m_TownRankPoints = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int CitizenKills
        {
            get { return m_CitizenKills; }
            set
            {
                m_CitizenKills = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int Deaths
        {
            get { return m_Deaths; }
            set
            {
                m_Deaths = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int EnemyKills { get; set; }

        public List<KillPointsGivenEntry> KillsGiven { get { return m_KillsGiven; } }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile LastKiller
        {
            get
            {
                if( m_KillsGiven == null )
                    return null;

                return m_KillsGiven[ m_KillsGiven.Count - 1 ].GivenTo;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile LastCitizenKilled { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public DateTime LastCitizenKilledDateTime { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile LastEnemyKilled { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public DateTime LastEnemyKilledDateTime { get; set; }

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

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsWarLord
        {
            get
            {
                return Mobile != null && TownSystem != null && TownSystem.WarLord == Mobile;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DisplayCitizenStatus
        {
            get { return Mobile is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)Mobile ).DisplayCitizenStatus; }
            set
            {
                if( Mobile is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)Mobile ).DisplayCitizenStatus = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsTownMurderer
        {
            get { return ( TownSystem.IsMurdererTown ); }
        }

        private bool m_CitizenCriminal;
        private Timer m_ExpireCitizenCriminal;

        [CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
        public bool CitizenCriminal
        {
            get { return m_CitizenCriminal; }
            set
            {
                if( m_CitizenCriminal != value )
                {
                    m_CitizenCriminal = value;
                    Mobile.Delta( MobileDelta.Noto );
                    Mobile.InvalidateProperties();
                }

                if( m_CitizenCriminal )
                {
                    if( m_ExpireCitizenCriminal == null )
                        m_ExpireCitizenCriminal = new ExpireCitizenCriminalTimer( this );
                    else
                        m_ExpireCitizenCriminal.Stop();

                    m_ExpireCitizenCriminal.Start();
                }
                else if( m_ExpireCitizenCriminal != null )
                {
                    m_ExpireCitizenCriminal.Stop();
                    m_ExpireCitizenCriminal = null;
                }
            }
        }

        private static readonly TimeSpan m_ExpireCriminalDelay = TimeSpan.FromMinutes( 5.0 );

        private class ExpireCitizenCriminalTimer : Timer
        {
            private readonly TownPlayerState m_Owner;

            public ExpireCitizenCriminalTimer( TownPlayerState m )
                : base( m_ExpireCriminalDelay )
            {
                Priority = TimerPriority.FiveSeconds;

                m_Owner = m;
            }

            protected override void OnTick()
            {
                m_Owner.CitizenCriminal = false;
            }
        }

        public bool CanGivePointsTo( Mobile mob )
        {
            if( m_KillsGiven == null )
                return true;

            for( int i = 0; i < m_KillsGiven.Count; ++i )
            {
                KillPointsGivenEntry sge = m_KillsGiven[ i ];

                if( sge.IsExpired )
                    m_KillsGiven.RemoveAt( i-- );
                else if( sge.GivenTo == mob )
                    return false;
            }

            return true;
        }

        public void OnGivenKillPointsTo( Mobile mob )
        {
            if( m_KillsGiven == null )
                m_KillsGiven = new List<KillPointsGivenEntry>();

            m_KillsGiven.Add( new KillPointsGivenEntry( mob ) );

        }

        public void Invalidate()
        {
            if( Mobile is PlayerMobile )
                Mobile.InvalidateProperties();
        }

        public TownPlayerState( TownSystem town, Mobile mobile )
        {
            TownSystem = town;
            Mobile = mobile;

            Init();

            Attach();
            Invalidate();
        }

        public void Init()
        {
            m_TownOffice = TownOffices.None;
            m_CustomTownOffice = String.Empty;
            m_TownProfession = Professions.None;
            m_CustomProfession = String.Empty;
            m_TownAccessLevel = TownAccessLevel.Citizen;

            m_TownRankPoints = 0;
            m_CitizenKills = 0;
            m_Deaths = 0;
            EnemyKills = 0;
            m_KillsGiven = new List<KillPointsGivenEntry>();
            LastCitizenKilled = null;
            LastCitizenKilledDateTime = DateTime.MinValue;
            LastEnemyKilled = null;
            LastEnemyKilledDateTime = DateTime.MinValue;
        }

        public void CheckAttach()
        {
            if( TownSystem != null && TownSystem.IsEligible( Mobile ) )
                Attach();
        }

        public void Attach()
        {
            if( TownSystem == null )
            {
                Config.Pkg.LogWarningLine( "Warning: playerstate with null townsystem. Removing from town system..." );
                return;
            }

            if( Mobile == null )
            {
                Config.Pkg.LogWarningLine( "Warning: playerstate with null mobile. Removing from town system..." );
                return;
            }

            if( TownSystem.GetMobilesFromStates().Contains( Mobile ) )
            {
                Config.Pkg.LogWarningLine( "Warning: playerstate (mobile {0}) is already in {1} townsystem. Removing from town system...", Mobile.Name, TownSystem.Definition.TownName );
                return;
            }

            if( Mobile is Midgard2PlayerMobile )
            {
                ( (Midgard2PlayerMobile)Mobile ).TownState = this;
                TownSystem.Players.Add( this );

                TownSystem.OnTownSystemJoined( this );
            }
        }

        public void Detach()
        {
            if( Mobile != null && Mobile is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)Mobile ).TownState = null;

            TownSystem.Players.Remove( this );
        }

        public override string ToString()
        {
            return "...";
        }

        public int CompareTo( object obj )
        {
            return m_TownRankPoints - ( (TownPlayerState)obj ).TownRankPoints;
        }

        #region serialization
        public TownPlayerState( TownSystem town, GenericReader reader )
        {
            TownSystem = town;

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 5:
                    if( m_TownAccessLevel == null )
                        m_TownAccessLevel = TownAccessLevel.Citizen;
                    m_TownAccessLevel = new TownAccessLevel( reader );
                    goto case 4;
                case 4:
                    m_Deaths = reader.ReadInt();
                    EnemyKills = reader.ReadInt();
                    LastCitizenKilled = reader.ReadMobile();
                    LastCitizenKilledDateTime = reader.ReadDateTime();
                    LastEnemyKilled = reader.ReadMobile();
                    LastEnemyKilledDateTime = reader.ReadDateTime();
                    goto case 3;
                case 3:
                    m_TownRankPoints = reader.ReadInt();
                    goto case 2;
                case 2:
                    if( version < 5 )
                    {
                        int level = reader.ReadInt();
                        int maxLevel = TownAccessLevel.Levels.Length - 1;
                        if( level > maxLevel )
                            level = maxLevel;
                        if( level < 0 )
                            level = 0;
                        m_TownAccessLevel = TownAccessLevel.Levels[ level ];
                    }
                    goto case 1;
                case 1:
                    m_CitizenKills = reader.ReadInt();
                    m_TownOffice = (TownOffices)reader.ReadInt();
                    m_CustomTownOffice = reader.ReadString();
                    m_TownProfession = (Professions)reader.ReadInt();
                    m_CustomProfession = reader.ReadString();
                    goto case 0;
                case 0:
                    Mobile = reader.ReadMobile();
                    break;
            }

            if( version < 4 )
            {
                m_TownRankPoints = -1000;
                m_Deaths = 0;
                EnemyKills = 0;
            }

            if( Mobile == null )
            {
                Config.Pkg.LogWarningLine( "Warning: TownPlayerstate with null mobile detected. Removing..." );
                Timer.DelayCall( TimeSpan.Zero, Detach );
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 5 ); // version

            // Version 5
            m_TownAccessLevel.Serialize( writer );

            // Version 4 
            writer.Write( m_Deaths );
            writer.Write( EnemyKills );
            writer.Write( LastCitizenKilled );
            writer.Write( LastCitizenKilledDateTime );
            writer.Write( LastEnemyKilled );
            writer.Write( LastEnemyKilledDateTime );

            // Version 3
            writer.Write( m_TownRankPoints );

            // Version 2
            //writer.Write( m_TownAccessLevel.Level );

            // Version 1
            writer.Write( m_CitizenKills );
            writer.Write( (int)m_TownOffice );
            writer.Write( m_CustomTownOffice );
            writer.Write( (int)m_TownProfession );
            writer.Write( m_CustomProfession );

            // Version 0
            writer.Write( Mobile );
        }
        #endregion
    }
}