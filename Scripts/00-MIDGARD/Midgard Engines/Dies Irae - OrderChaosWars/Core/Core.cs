/***************************************************************************
 *                               Core.cs
 *                            -----------------------
 *   begin                : 4 novembre 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.OrderChaosWars
{
    public sealed class Core
    {
        #region Singleton pattern
        private static Core m_Instance;

        public static Core Instance
        {
            get
            {
                if( m_Instance == null )
                {
                    m_Instance = new Core();
                }
                return m_Instance;
            }
        }

        private Core()
        {
            if( Config.Debug )
                Logger.Log( "OrderChaosWars singleton instanced." );

            LastStateTime = DateTime.Now;
            CurrentPhase = WarPhase.Idle;
            LastVirtueWon = Virtue.None;
            CurrentBattle = null;
        }
        #endregion

        #region fields
        private List<ScoreRegion> m_ScoreRegions;
        private Timer m_Timer;
        private Region m_WarRegion;
        #endregion

        #region accessors
        public static Map WarMap
        {
            get { return Map.Felucca; }
        }

        public List<Mobile> WarPlayers { get; set; }

        public List<ScoreRegion> ScoreRegions
        {
            get { return m_ScoreRegions ?? ( m_ScoreRegions = new List<ScoreRegion>() ); }
            set { m_ScoreRegions = value; }
        }

        public List<HeadQuarterRegion> HeadQuarterRegions { get; set; }

        public WarPhase Phase
        {
            get { return CurrentPhase; }
            set
            {
                CurrentPhase = value;
                LastStateTime = DateTime.Now;
            }
        }

        public DateTime LastStateTime { get; private set; }

        public WarPhase CurrentPhase { get; private set; }

        public TimeSpan NextStateTime
        {
            get
            {
                TimeSpan period;

                switch( CurrentPhase )
                {
                    case WarPhase.Idle:
                        period = TimeSpan.FromDays( 365.0 );
                        break;
                    case WarPhase.PreBattle:
                        period = Config.PreBattlePeriod;
                        break;
                    case WarPhase.BattlePending:
                        period = Config.BattlePendingPeriod;
                        break;
                    case WarPhase.PostBattle:
                        period = Config.PostBattlePeriod;
                        break;
                    default:
                        period = TimeSpan.FromDays( 365.0 );
                        break;
                }

                TimeSpan until = ( LastStateTime + period ) - DateTime.Now;

                if( until < TimeSpan.Zero )
                    until = TimeSpan.Zero;

                return until;
            }
            set
            {
                TimeSpan period;

                switch( CurrentPhase )
                {
                    case WarPhase.Idle:
                        period = TimeSpan.FromDays( 365.0 );
                        break;
                    case WarPhase.PreBattle:
                        period = Config.PreBattlePeriod;
                        break;
                    case WarPhase.BattlePending:
                        period = Config.BattlePendingPeriod;
                        break;
                    case WarPhase.PostBattle:
                        period = Config.PostBattlePeriod;
                        break;
                    default:
                        period = TimeSpan.FromDays( 365.0 );
                        break;
                }

                LastStateTime = DateTime.Now - period + value;
            }
        }

        public int OrderScore
        {
            get
            {
                if( CurrentBattle != null && CurrentBattle.OrderWarState != null )
                    return CurrentBattle.OrderWarState.Score;
                else
                    return 0;
            }
        }

        public int ChaosScore
        {
            get
            {
                if( CurrentBattle != null && CurrentBattle.ChaosWarState != null )
                    return CurrentBattle.ChaosWarState.Score;
                else
                    return 0;
            }
        }

        public List<Mobile> OrderPlayers
        {
            get { return GetPlayersForVirtue( Virtue.Order ); }
        }

        public List<Mobile> ChaosPlayers
        {
            get { return GetPlayersForVirtue( Virtue.Chaos ); }
        }

        public Virtue LastVirtueWon { get; private set; }

        public BaseWar CurrentBattle { get; set; }

        public List<DecorationItem> DecoList { get; private set; }

        public bool HasTimer
        {
            get { return m_Timer != null; }
        }

        public Region WarRegion
        {
            get
            {
                if( CurrentBattle != null && CurrentBattle.Definition.MainRegionName != null && m_WarRegion == null )
                {
                    foreach( Region r in Region.Regions )
                    {
                        if( r is GuardedRegion && r.Name == CurrentBattle.Definition.MainRegionName )
                            m_WarRegion = r;
                    }
                }

                return m_WarRegion;
            }
            set { m_WarRegion = value; }
        }

        public string CurrentPhaseName
        {
            get
            {
                switch( Phase )
                {
                    default:
                        return "";
                    case WarPhase.Idle:
                        return "no war in progress";
                    case WarPhase.PreBattle:
                        return "pre-battle period";
                    case WarPhase.BattlePending:
                        return "war in progress";
                    case WarPhase.PostBattle:
                        return "post-battle period";
                }
            }
        }
        #endregion

        internal static void ConfigSystem()
        {
            if( Config.SaveEnabled )
            {
                EventSink.WorldLoad += new WorldLoadEventHandler( Load );
                EventSink.WorldSave += new WorldSaveEventHandler( Save );
            }

            Instance.StartTimer();
        }

        /// <summary>
        /// Starts the main timer of our system
        /// </summary>
        internal void StartTimer()
        {
            if( Config.Debug )
                Logger.Log( "Core timer requested to start." );

            if( m_Timer == null || !m_Timer.Running )
            {
                m_Timer = Timer.DelayCall( Config.RefreshDelay, Config.RefreshDelay, new TimerCallback( Slice ) );

                if( Config.Debug )
                    Logger.Log( "Core timer has started." );
            }
        }

        public static void HandleDeath( Mobile victim, Mobile killer )
        {
            if( !Config.Enabled )
                return;
        }

        public static bool HandleOrderVsChaosWarsNotoriety( Mobile source, Mobile target, out int noto )
        {
            noto = -1;

            return false;
        }

        public static bool AreEnemies( Mobile source, Mobile target )
        {
            Virtue sourceVirtue = Find( source );
            Virtue targetVirtue = Find( target );

            return ( sourceVirtue != Virtue.None && targetVirtue != Virtue.None && sourceVirtue != targetVirtue );
        }

        public void ToggleTownStatus( Virtue virtue, string regionName )
        {
            if( m_WarRegion != null && m_WarRegion is GuardedRegion )
                ( (GuardedRegion)m_WarRegion ).WarVirtue = virtue;
        }

        private void Slice()
        {
            if( !Config.Enabled )
            {
                if( m_Timer != null )
                    m_Timer.Stop();

                m_Timer = null;

                return;
            }

            switch( CurrentPhase )
            {
                case WarPhase.Idle:
                    {
                        if( CurrentBattle == null )
                        {
                            break;
                        }

                        StartPreWar();

                        CurrentBattle.DoStartPreWarActions();

                        Phase = WarPhase.PreBattle;

                        break;
                    }
                case WarPhase.PreBattle:
                    {
                        if( ( LastStateTime + Config.PreBattlePeriod ) > DateTime.Now )
                        {
                            break;
                        }

                        CurrentBattle.DoEndPreWarActions();

                        StartWar();

                        CurrentBattle.DoStartWarActions();

                        Phase = WarPhase.BattlePending;

                        break;
                    }
                case WarPhase.BattlePending:
                    {
                        if( ( LastStateTime + Config.BattlePendingPeriod ) > DateTime.Now )
                        {
                            CurrentBattle.DoWarSliceActions();
                            break;
                        }

                        CurrentBattle.DoEndWarActions();

                        EndWar();

                        CurrentBattle.DoStartPostWarActions();

                        Phase = WarPhase.PostBattle;

                        break;
                    }
                case WarPhase.PostBattle:
                    {
                        if( ( LastStateTime + Config.PostBattlePeriod ) > DateTime.Now )
                        {
                            break;
                        }

                        CurrentBattle.DoEndPostWarActions();

                        EndPostWar();

                        Phase = WarPhase.Idle;

                        break;
                    }
            }
        }

        public void ToggleGuardStatus( bool enable )
        {
            if( enable )
            {
                if( WarRegion != null && WarRegion is GuardedRegion )
                {
                    ( (GuardedRegion)WarRegion ).Disabled = false;
                    Broadcast( "Town guards of {0} has been enabled.", WarRegion.Name );
                }
            }
            else
            {
                if( WarRegion != null && WarRegion is GuardedRegion )
                {
                    ( (GuardedRegion)WarRegion ).Disabled = true;
                    Broadcast( "Town guards of {0} has been disabled.", WarRegion.Name );
                }
            }
        }

        public void ToggleRegionRegistration( bool register )
        {
            if( register )
            {
                if( HeadQuarterRegions == null )
                    HeadQuarterRegions = new List<HeadQuarterRegion>();
                else
                    HeadQuarterRegions.Clear();

                HeadQuarterRegions.Add( new HeadQuarterRegion( Virtue.Order, CurrentBattle.Definition.OrderHeadQuarter ) );
                HeadQuarterRegions.Add( new HeadQuarterRegion( Virtue.Chaos, CurrentBattle.Definition.ChaosHeadQuarter ) );

                if( m_ScoreRegions == null )
                    m_ScoreRegions = new List<ScoreRegion>();
                else
                    m_ScoreRegions.Clear();

                if( CurrentBattle != null )
                {
                    foreach( BaseObjective objective in CurrentBattle.Objectives )
                    {
                        if( objective is ConquerScoreRegionObjective )
                        {
                            ConquerScoreRegionObjective obj = (ConquerScoreRegionObjective)objective;

                            m_ScoreRegions.Add( obj.Region );
                            obj.Region.Register();
                        }
                    }
                }
            }
            else
            {
                if( HeadQuarterRegions != null )
                {
                    foreach( HeadQuarterRegion quarterRegion in HeadQuarterRegions )
                        quarterRegion.Unregister();

                    HeadQuarterRegions.Clear();
                }

                if( m_ScoreRegions != null )
                {
                    foreach( ScoreRegion scoreRegion in m_ScoreRegions )
                        scoreRegion.Unregister();

                    m_ScoreRegions.Clear();
                }
            }
        }

        public void CreateWarStones()
        {
            WarStone orderStone = new WarStone();

            if( !FindItem( CurrentBattle.Definition.OrderStoneLocation, WarMap, orderStone ) )
                orderStone.MoveToWorld( CurrentBattle.Definition.OrderStoneLocation, WarMap );

            WarStone chaosStone = new WarStone();
            if( !FindItem( CurrentBattle.Definition.ChaosStoneLocation, WarMap, chaosStone ) )
                chaosStone.MoveToWorld( CurrentBattle.Definition.OrderStoneLocation, WarMap );

            if( orderStone.Map == Map.Internal )
                orderStone.Delete();

            if( chaosStone.Map == Map.Internal )
                chaosStone.Delete();
        }

        public static bool FindItem( int x, int y, int z, Map map, Item test )
        {
            return FindItem( new Point3D( x, y, z ), map, test );
        }

        public static bool FindItem( Point3D p, Map map, Item test )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p );

            foreach( Item item in eable )
            {
                if( item.Z == p.Z && item.ItemID == test.ItemID )
                {
                    eable.Free();
                    return true;
                }
            }

            eable.Free();
            return false;
        }

        #region get utilities
        public static string GetVirtueName( Virtue virtue )
        {
            return Enum.GetName( typeof( Virtue ), virtue );
        }

        public static BaseWar GetWarFromType( BattleType type )
        {
            foreach( BaseWar war in BaseWar.Wars )
            {
                if( war.Definition.Wartype == type )
                    return war;
            }

            return null;
        }

        public ScoreRegion GetScoreRegionByIndex( int index )
        {
            if( m_ScoreRegions == null )
                return null;

            foreach( ScoreRegion region in m_ScoreRegions )
            {
                if( region.Index == index )
                    return region;
            }

            return null;
        }

        public static Region GetRegion( string name )
        {
            if( name == null )
                return null;

            Region reg;

            if( Map.Trammel.Regions.TryGetValue( name, out reg ) )
                return reg;

            if( Map.Felucca.Regions.TryGetValue( name, out reg ) )
                return reg;

            if( Map.Ilshenar.Regions.TryGetValue( name, out reg ) )
                return reg;

            if( Map.Malas.Regions.TryGetValue( name, out reg ) )
                return reg;

            if( Map.Tokuno.Regions.TryGetValue( name, out reg ) )
                return reg;

            return reg;
        }

        public List<Mobile> GetPlayersForVirtue( Virtue virtue )
        {
            if( WarPlayers == null )
                return null;

            List<Mobile> list = new List<Mobile>();

            foreach( Mobile m in WarPlayers )
            {
                if( Find( m ) == virtue )
                    list.Add( m );
            }

            return list;
        }

        public int GetPoints( Virtue virtue )
        {
            switch( virtue )
            {
                case Virtue.None:
                    return 0;
                case Virtue.Order:
                    return OrderScore;
                case Virtue.Chaos:
                    return ChaosScore;
            }

            return 0;
        }

        public static int GetHue( Virtue virtue )
        {
            switch( virtue )
            {
                case Virtue.None:
                    return 0;
                case Virtue.Order:
                    return 1154;
                case Virtue.Chaos:
                    return 2444;
            }

            return 0;
        }
        #endregion

        #region war phases
        public void StartPreWar()
        {
            if( CurrentBattle == null )
            {
                Logger.Log( "Warning: Order/chaos pre-war is starting but war plan is not defined." );
                return;
            }

            double minutes = Config.PreBattlePeriod.TotalMinutes;

            Broadcast( "The {0} will start in {1} minute{2}.", CurrentBattle.Definition.WarName, minutes, minutes > 1 ? "s" : "" );

            CreateWarStones();

            ToggleRegionRegistration( true );

            ToggleGuardStatus( false );
        }

        public void StartWar()
        {
            if( CurrentBattle == null )
            {
                Logger.Log( "Warning: Order/chaos war is starting but war plan is not defined." );
                return;
            }

            Broadcast( "The {0} has begun!", CurrentBattle.Definition.WarName );
        }

        public void EndWar()
        {
            if( CurrentBattle == null )
            {
                Logger.Log( "Warning: Order/chaos war is ending but war plan is not defined." );
                return;
            }

            Broadcast( "The {0} ended!", CurrentBattle.Definition.WarName );

            LastVirtueWon = CurrentBattle.ComputeWinners();
        }

        public void EndPostWar()
        {
            Broadcast( "The post war phase ended." );

            ToggleGuardStatus( true );

            ToggleRegionRegistration( false );

            if( CurrentBattle != null )
                CurrentBattle = null;
        }
        #endregion

        #region serialization
        public static void Save( WorldSaveEventArgs e )
        {
            if( Config.Debug )
                Console.Write( "{0}: Saving...", Config.Pkg.Title );

            try
            {
                WorldSaveProfiler.Instance.StartHandlerProfile( Save );

                string dir = Path.Combine( Path.GetPathRoot( Config.WarSavePath ), Path.GetDirectoryName( Config.WarSavePath ) );
                if( !Directory.Exists( dir ) )
                    Directory.CreateDirectory( dir );

                BinaryFileWriter writer = new BinaryFileWriter( Config.WarSavePath, true );
                Instance.Serialize( writer );
                writer.Close();

                WorldSaveProfiler.Instance.EndHandlerProfile();
            }
            catch
            {
                Console.WriteLine( "Error serializing {0}.", Config.Pkg.Title );
            }

            if( Config.Debug )
                Console.WriteLine( "done." );
        }

        public static void Load()
        {
            if( Config.Debug )
                Console.Write( "{0}: Loading...", Config.Pkg.Title );

            while( !File.Exists( Config.WarSavePath ) )
            {
                Console.WriteLine( "Warning: {0} not found.", Config.WarSavePath );
                Console.WriteLine( " - Press return to continue, or R to try again." );
                string str = Console.ReadLine();

                if( str == null || str.ToLower() != "r" )
                    return;
            }

            try
            {
                BinaryReader bReader = new BinaryReader( File.OpenRead( Config.WarSavePath ) );
                BinaryFileReader reader = new BinaryFileReader( bReader );
                new Core( reader );

                bReader.Close();
            }
            catch
            {
                Console.WriteLine( "Error deserializing {0}.", Config.Pkg.Title );
            }

            if( Config.Debug )
                Console.WriteLine( "done." );
        }

        public Core( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        LastStateTime = reader.ReadDateTime();
                        CurrentPhase = (WarPhase)reader.ReadEncodedInt();
                        LastVirtueWon = (Virtue)reader.ReadEncodedInt();
                        BattleType warType = (BattleType)reader.ReadEncodedInt();

                        if( warType != BattleType.None )
                        {
                            CurrentBattle = GetWarFromType( warType );
                            CurrentBattle.Deserialize( reader );
                        }

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            writer.Write( LastStateTime );
            writer.WriteEncodedInt( (int)CurrentPhase );
            writer.WriteEncodedInt( (int)LastVirtueWon );

            if( CurrentBattle != null )
            {
                writer.WriteEncodedInt( (int)CurrentBattle.Definition.Wartype );
                CurrentBattle.Serialize( writer );
            }
            else
                writer.WriteEncodedInt( (int)BattleType.None );
        }
        #endregion

        #region broadcast
        public static void Broadcast( Virtue virtue, int hue, string format, params object[] args )
        {
            List<NetState> list = NetState.Instances;

            foreach( NetState netState in list )
            {
                if( netState != null && netState.Mobile != null )
                {
                    if( Find( netState.Mobile ) == virtue )
                        netState.Mobile.SendAsciiMessage( hue, format, args );
                }
            }

            Console.WriteLine( "O/C War System: " + String.Format( format, args ) );
        }

        public static void Broadcast( string message )
        {
            World.Broadcast( 37, true, message );

            Console.WriteLine( "O/C War System: " + message );
        }

        public static void Broadcast( string format, params object[] args )
        {
            Broadcast( String.Format( format, args ) );
        }
        #endregion

        #region find
        public static Virtue Find( GuildType type )
        {
            switch( type )
            {
                case GuildType.Order:
                    return Virtue.Order;
                case GuildType.Chaos:
                    return Virtue.Chaos;
                default:
                    return Virtue.None;
            }
        }

        public static Virtue Find( Mobile mob )
        {
            return Find( mob, false, false );
        }

        public static Virtue Find( Mobile mob, bool inherit )
        {
            return Find( mob, inherit, false );
        }

        public static Virtue Find( Mobile mob, bool inherit, bool creatureAllegiances )
        {
            BaseGuild g = mob.Guild;
            if( g != null )
                return Find( g.Type );

            if( inherit && mob is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)mob;

                if( bc.Controlled )
                    return Find( bc.ControlMaster, false );
                else if( bc.Summoned )
                    return Find( bc.SummonMaster, false );
            }

            return Virtue.None;
        }
        #endregion

        #region register
        public void RegisterPlayer( Mobile m )
        {
            if( m == null || !( m is PlayerMobile ) )
                return;

            Virtue v = Find( m );

            if( v != Virtue.None )
                WarPlayers.Add( m );

            Logger.Log( "Player {0} registered", m.Name );
        }

        public void RegisterDeco( DecorationItem decorationItem )
        {
            if( decorationItem == null )
                return;

            if( DecoList == null )
                DecoList = new List<DecorationItem>();

            DecoList.Add( decorationItem );

            Logger.Log( "Decoration {0} registered", decorationItem.GetType().Name );
        }

        public void RegisterScoreRegion( ScoreRegion scoreRegion )
        {
            if( scoreRegion == null )
                return;

            if( m_ScoreRegions == null )
                m_ScoreRegions = new List<ScoreRegion>();

            m_ScoreRegions.Add( scoreRegion );

            Logger.Log( "Region {0} registered", scoreRegion.Name );
        }

        public void RegisterHeadQuarterRegion( HeadQuarterRegion headQuarterRegion )
        {
            if( headQuarterRegion == null )
                return;

            if( HeadQuarterRegions == null )
                HeadQuarterRegions = new List<HeadQuarterRegion>();

            HeadQuarterRegions.Add( headQuarterRegion );

            Logger.Log( "HeadQuarterRegion {0} registered", headQuarterRegion.Name );
        }
        #endregion
    }
}