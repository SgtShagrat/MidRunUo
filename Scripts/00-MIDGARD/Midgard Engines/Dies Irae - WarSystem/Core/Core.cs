/***************************************************************************
 *                               Core.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Midgard.Engines.WarSystem
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
                Logger.Log( "War System singleton instanced." );

            LastStateTime = DateTime.Now;
            CurrentPhase = WarPhase.Idle;
            LastTeamWon = null;
            CurrentBattle = null;
        }
        #endregion

        #region external props
        public bool InProgress
        {
            get { return CurrentBattle != null; }
        }

        public bool WarPending
        {
            get { return CurrentPhase == WarPhase.BattlePending; }
        }
        #endregion

        private List<ScoreRegion> m_ScoreRegions;
        private Timer m_Timer;
        private Region m_WarRegion;

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
                        period = BaseWar.PreBattlePeriod;
                        break;
                    case WarPhase.BattlePending:
                        period = BaseWar.BattlePendingPeriod;
                        break;
                    case WarPhase.PostBattle:
                        period = BaseWar.PostBattlePeriod;
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
                        period = BaseWar.PreBattlePeriod;
                        break;
                    case WarPhase.BattlePending:
                        period = BaseWar.BattlePendingPeriod;
                        break;
                    case WarPhase.PostBattle:
                        period = BaseWar.PostBattlePeriod;
                        break;
                    default:
                        period = TimeSpan.FromDays( 365.0 );
                        break;
                }

                LastStateTime = DateTime.Now - period + value;
            }
        }

        public WarTeam LastTeamWon { get; private set; }

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
                        if( r.Name == CurrentBattle.Definition.MainRegionName )
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

        /// <summary>
        /// Starts the main timer of our system
        /// </summary>
        internal void StartTimer()
        {
            if( Config.Debug )
                Logger.Log( "War system core timer requested to start." );

            if( m_Timer == null || !m_Timer.Running )
            {
                m_Timer = Timer.DelayCall( BaseWar.RefreshDelay, BaseWar.RefreshDelay, new TimerCallback( Slice ) );

                if( Config.Debug )
                    Logger.Log( "War system core has started." );
            }
        }

        public static void HandleDeath( Mobile mob )
        {
            HandleDeath( mob, null );
        }

        /// <summary>
        /// Usefull to handle death, ress, loot etc.
        /// </summary>
        public static void HandleDeath( Mobile killed, Mobile killer )
        {
            if( !Config.Enabled )
                return;

            if( Instance != null && Instance.CurrentBattle != null )
                Instance.CurrentBattle.HandleDeath( killer, killer );

            #region reset notoriety
            if( killed != null )
                Utility.ClearAggressed( killed );

            if( killer != null )
                Utility.ClearAggressed( killer );

            if( killed != null )
            {
                if( killed.Criminal )
                    killed.Criminal = false;
            }

            if( killer != null )
            {
                if( killer.Criminal )
                    killer.Criminal = false;
            }
            #endregion

            if( BaseWar.AutoRes )
            {
                // immediately bless the corpse to prevent looting
                if( killed != null )
                {
                    if( killed.Corpse != null )
                        killed.Corpse.LootType = LootType.Blessed;

                    // prepare the autores callback
                    Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( AutoResCallback ), new object[] { killed } );
                }
            }

            if( BaseWar.BroadcastDeath && killed != null )
            {
                Utility.Broadcast( "{0} has been killed by {1}!", killed.Name, ( killer != null ) ? killer.Name : "<unknown>" );
                Utility.BroadcastToStaff( "{0} has been killed by {1}!", killed.Name, ( killer != null ) ? killer.Name : "<unknown>" );
            }
        }

        private static void AutoResCallback( object state )
        {
            object[] args = (object[])state;

            Mobile m = (Mobile)args[ 0 ];

            if( m != null )
	        {
				//reset combatant
				m.Combatant = null;
		
				if (m.LastKiller != null)
					(m.LastKiller).Combatant = null;

                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16 );

                m.Resurrect();

                if( m.Corpse != null && BaseWar.RetrieveCorpseAfterRes )
                {
                    m.MoveToWorld( m.Corpse.Location, m.Corpse.Map );

                    Corpse c = m.Corpse as Corpse;
                    if( c != null )
                        c.Open( m, true, true );

                    m.Corpse.LootType = LootType.Regular;
                }

                if( BaseWar.RefreshStatsAfterRes )
                {
                    m.Hits = m.HitsMax;
                    m.Mana = int.MaxValue;
                    m.Stam = int.MaxValue;
                }
            }
        }

        public void DisableSpawners()
        {
            List<Spawner> distroSpawners = new List<Spawner>();
            List<XmlSpawner> xmlSpawners = new List<XmlSpawner>();

            lock( World.Items.Values )
            {
                foreach( Item item in World.Items.Values )
                {
                    if( item is Spawner && !distroSpawners.Contains( (Spawner)item ) )
                        distroSpawners.Add( (Spawner)item );
                    else if( item is XmlSpawner && !xmlSpawners.Contains( (XmlSpawner)item ) )
                        xmlSpawners.Add( (XmlSpawner)item );
                }
            }

            Config.Pkg.LogInfo( "Disabling spawns: {0} spawners found, {1} xmlspawners found.", distroSpawners.Count, xmlSpawners.Count );

            int creaturesBefore = World.Mobiles.Count;

            foreach( Spawner spawner in distroSpawners )
            {
                spawner.BringToHome();
                spawner.RemoveCreatures();
                Logger.Log( "Disabled spawner at location: {0}", spawner.Location );
            }

            foreach( XmlSpawner spawner in xmlSpawners )
            {
                spawner.BringToHome();
                spawner.RemoveSpawnObjects();
                Logger.Log( "Disabled xmlspawner at location: {0}", spawner.Location );
            }

            int creaturesAfter = World.Mobiles.Count;

            Logger.Log( "Deleted {0} creatures during spawner disable step. A total of {1} spawners has been disabled.",
                ( creaturesAfter - creaturesBefore ), ( distroSpawners.Count + xmlSpawners.Count ) );
        }

        /// <summary>
        /// Called by runuo notoriety handler to update the noto status.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="noto"></param>
        /// <returns>-1 if noto is not handled, noto status otherwise.</returns>
        public static bool HandleNotoriety( Mobile source, Mobile target, out int noto )
        {
            noto = -1;

            return false;
        }

        public static bool AreEnemies( Mobile source, Mobile target )
        {
            WarTeam team = Utility.Find( source );

            return ( team != null && Utility.Find( target ) != null && team.IsEnemy( target ) );
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
                            break;

                        StartPreWar();

                        CurrentBattle.DoStartPreWarActions();

                        Phase = WarPhase.PreBattle;

                        break;
                    }
                case WarPhase.PreBattle:
                    {
                        if( ( LastStateTime + BaseWar.PreBattlePeriod ) > DateTime.Now )
                            break;

                        CurrentBattle.DoEndPreWarActions();

                        StartWar();

                        CurrentBattle.DoStartWarActions();

                        Phase = WarPhase.BattlePending;

                        break;
                    }
                case WarPhase.BattlePending:
                    {
                        if( !CheckWarEnd() )
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
                        if( ( LastStateTime + BaseWar.PostBattlePeriod ) > DateTime.Now )
                            break;

                        CurrentBattle.DoEndPostWarActions();

                        EndPostWar();

                        Phase = WarPhase.Idle;

                        break;
                    }
            }
        }

        public bool CheckWarEnd()
        {
            if( !CurrentBattle.Timed )
                return CurrentBattle.EvaluateEndCriteria();

            return ( LastStateTime + BaseWar.BattlePendingPeriod ) > DateTime.Now;
        }

        public void ToggleGuardStatus( bool enable )
        {
            if( enable )
            {
                if( WarRegion != null && WarRegion is GuardedRegion )
                {
                    ( (GuardedRegion)WarRegion ).Disabled = false;
                    Utility.Broadcast( "Town guards of {0} has been enabled.", WarRegion.Name );
                }
            }
            else
            {
                if( WarRegion != null && WarRegion is GuardedRegion )
                {
                    ( (GuardedRegion)WarRegion ).Disabled = true;
                    Utility.Broadcast( "Town guards of {0} has been disabled.", WarRegion.Name );
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

                foreach( WarTeam warTeam in CurrentBattle.Definition.WarTeams )
                    HeadQuarterRegions.Add( new HeadQuarterRegion( warTeam, warTeam.HeadQuarterDefinition ) );

                if( m_ScoreRegions == null )
                    m_ScoreRegions = new List<ScoreRegion>();
                else
                    m_ScoreRegions.Clear();

                if( CurrentBattle != null )
                {
                    foreach( BaseObjective objective in CurrentBattle.Objectives )
                    {
                        if( !( objective is ConquerScoreRegionObjective ) )
                            continue;

                        ConquerScoreRegionObjective obj = (ConquerScoreRegionObjective)objective;

                        m_ScoreRegions.Add( obj.Region );
                        obj.Region.Register();
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

        #region get utilities
        public static BaseWar GetWarFromType( BattleType type )
        {
            foreach( BaseWar war in BaseWar.Wars )
            {
                if( war.Definition.WarNameEnum == type )
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

            /*
            if( Map.Trammel.Regions.TryGetValue( name, out reg ) )
                return reg;
            */

            if( Map.Felucca.Regions.TryGetValue( name, out reg ) )
                return reg;

            /*
            if( Map.Ilshenar.Regions.TryGetValue( name, out reg ) )
                return reg;

            if( Map.Malas.Regions.TryGetValue( name, out reg ) )
                return reg;

            if( Map.Tokuno.Regions.TryGetValue( name, out reg ) )
                return reg;
            */

            return reg;
        }

        public List<Mobile> GetPlayersForTeam( WarTeam team )
        {
            return WarPlayers == null ? null : team.Members;
        }

        public static int GetHue( WarTeam team )
        {
            return (int)( team != null ? team.TeamHue : 0 );
        }
        #endregion

        #region war phases
        public void StartPreWar()
        {
            if( CurrentBattle == null )
            {
                Logger.Log( "Warning: War system pre-war is starting but war plan is not defined." );
                return;
            }

            double minutes = BaseWar.PreBattlePeriod.TotalMinutes;

            Utility.Broadcast( "The {0} will start in {1} minute{2}.", CurrentBattle.Definition.WarName, minutes, minutes > 1 ? "s" : "" );

            CurrentBattle.CreateWarStones();

            CurrentBattle.CreateWargates();

            ToggleRegionRegistration( true );

            ToggleGuardStatus( false );
        }

        public void StartWar()
        {
            if( CurrentBattle == null )
            {
                Logger.Log( "Warning: War system war is starting but war plan is not defined." );
                return;
            }

            Utility.Broadcast( "The {0} has begun!", CurrentBattle.Definition.WarName );

            CurrentBattle.StartTime = DateTime.Now;
        }

        public void EndWar()
        {
            if( CurrentBattle == null )
            {
                Logger.Log( "Warning: system war war is ending but war plan is not defined." );
                return;
            }

            Utility.Broadcast( "The {0} ended!", CurrentBattle.Definition.WarName );

            CurrentBattle.EndTime = DateTime.Now;

            LastTeamWon = CurrentBattle.ComputeWinners();
        }

        public void EndPostWar()
        {
            Utility.Broadcast( "The post war phase ended." );

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

                string dir = Path.Combine( Server.Core.BaseDirectory, Path.Combine( "Saves", Config.WarSavePath ) );
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

            if( CurrentBattle != null )
            {
                writer.WriteEncodedInt( (int)CurrentBattle.Definition.WarNameEnum );
                CurrentBattle.Serialize( writer );
            }
            else
                writer.WriteEncodedInt( (int)BattleType.None );
        }
        #endregion

        #region register
        public void RegisterPlayer( Mobile m )
        {
            if( m == null || !( m is PlayerMobile ) )
                return;

            WarTeam t = Utility.Find( m );

            if( t == null || WarPlayers.Contains( m ) )
                return;

            WarPlayers.Add( m );
            Logger.Log( "Player {0} registered ({1})", m.Name, DateTime.Now );
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