/***************************************************************************
 *                               MiniChampionSpawn.cs
 *
 *   begin                : 14 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.Classes.VirtueChampion;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.MiniChampionSystem
{
    public class MiniChampionSpawn : Item
    {
        #region ChampionSpawnInfo Table
        public static ChampionSpawnInfo[] Table { get { return m_Table; } }

        private static readonly ChampionSpawnInfo[] m_Table = new ChampionSpawnInfo[]
        {
            new CompassionChampionInfo(), new HonestyChampionInfo(), new CompassionChampionInfo(), 
            new ValorChampionInfo(), new JusticeChampionInfo(), new SacrificeChampionInfo(),
            new HonorChampionInfo(), new SpiritualityChampionInfo(), new HumilityChampionInfo(), 
            new DeceitChampionInfo(), new DespiseChampionInfo(), new DestardChampionInfo(),
            new WrongChampionInfo(), new CovetousChampionInfo(), new ShameChampionInfo(), 
            new HythlothChampionInfo(), new PrideChampionInfo()
        };

        public static ChampionSpawnInfo GetInfo( MiniChampionSpawnType type )
        {
            int v = (int)type;

            if( v < 0 || v >= m_Table.Length )
                v = 0;

            return m_Table[ v ];
        }
        #endregion

        private bool m_Active;
        private int m_Level;

        private List<Mobile> m_Creatures;
        private Dictionary<Mobile, int> m_DamageEntries;

        private MiniChampionSpawnRegion m_Region;
        private Timer m_RestartTimer;
        private Rectangle2D m_SpawnArea;

        private Timer m_Timer;

        [Constructable]
        public MiniChampionSpawn()
            : base( 0xBD2 )
        {
            Movable = false;
            Visible = false;

            m_Creatures = new List<Mobile>();
            m_DamageEntries = new Dictionary<Mobile, int>();
            m_Level = 0;

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( SetupTimers ) );
            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( SetupMiniChampion ) );
            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( SetInitialSpawnArea ) );
        }

        protected virtual Item GenSpawner()
        {
            return new MiniChampionSpawner( this );
        }

        public ChampionSpawnInfo ChampionInfo
        {
            get { return GetInfo( Type ); }
        }

        public virtual bool CanAutoRestart
        {
            get { return true; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ConfinedRoaming { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Kills { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Rectangle2D SpawnArea
        {
            get { return m_SpawnArea; }
            set
            {
                m_SpawnArea = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan RestartDelay { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime RestartTime { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan ExpireDelay { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime ExpireTime { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public MiniChampionSpawnType Type { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if( value )
                    Start();
                else
                    Stop();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Champion { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Level
        {
            get { return m_Level; }
            set
            {
                if( value != m_Level && value >= 0 && value <= ChampionInfo.NumLevels )
                    m_Level = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxKills
        {
            get { return ( Level >= 0 && Level <= ChampionInfo.NumLevels ) ? ChampionInfo.GetGroupByLevel( Level ).MaxKills : 0; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Item ChampionSpawner { get; private set; }

        private void SetupTimers()
        {
            ExpireDelay = TimeSpan.FromMinutes( Config.ExpireDelayInMinutes );
            RestartDelay = CanAutoRestart ? TimeSpan.FromMinutes( Config.RestartDelayInMinutes ) : TimeSpan.MaxValue;
        }

        private void SetupMiniChampion()
        {
            ChampionSpawner = GenSpawner();

            if( ChampionSpawner != null )
                ChampionInfo.SetupSpawner( this );

            ChampionInfo.SetupSupportItems( this );
        }

        private void SetInitialSpawnArea()
        {
            SpawnArea = new Rectangle2D( new Point2D( X - 24, Y - 24 ), new Point2D( X + 24, Y + 24 ) );
        }

        private void UpdateRegion()
        {
            if( m_Region != null )
                m_Region.Unregister();

            if( !Deleted && Map != Map.Internal )
            {
                m_Region = new MiniChampionSpawnRegion( this );
                m_Region.Register();
            }
        }

        /*
                private bool IsChampionSpawn( Mobile m )
                {
                    return m_Creatures.Contains( m );
                }
        */

        public void Start()
        {
            if( m_Active || Deleted )
                return;

            m_Active = true;

            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = new SliceTimer( this );
            m_Timer.Start();

            if( m_RestartTimer != null )
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            ChampionInfo.OnChampionStarted( this );
        }

        public void Stop()
        {
            if( !m_Active || Deleted )
                return;

            m_Active = false;

            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = null;

            if( m_RestartTimer != null )
                m_RestartTimer.Stop();

            m_RestartTimer = null;

            ChampionInfo.OnChampionStopped( this );
        }

        public void BeginRestart( TimeSpan ts )
        {
            if( m_RestartTimer != null )
                m_RestartTimer.Stop();

            RestartTime = DateTime.Now + ts;

            m_RestartTimer = new RestartTimer( this, ts );
            m_RestartTimer.Start();
        }

        public void OnSlice()
        {
            if( !m_Active || Deleted )
                return;

            if( Champion != null )
            {
                if( Champion.Deleted )
                {
                    RegisterDamageTo( Champion );

                    m_DamageEntries.Clear();

                    ChampionInfo.OnChampionDeleted( this );

                    Champion = null;
                    Stop();

                    if( CanAutoRestart )
                        BeginRestart( RestartDelay );
                }
            }
            else
            {
                for( int i = 0; i < m_Creatures.Count; ++i )
                {
                    Mobile m = m_Creatures[ i ];

                    if( m.Deleted )
                    {
                        if( m.Corpse != null && !m.Corpse.Deleted )
                            ( (Corpse)m.Corpse ).BeginDecay( TimeSpan.FromMinutes( 1 ) );

                        m_Creatures.RemoveAt( i );
                        --i;
                        ++Kills;

                        RegisterDamageTo( m );
                    }
                }

                double n = Kills / (double)MaxKills;
                var p = (int)( n * 100 );

                if( p >= 90 )
                    AdvanceLevel();

                if( DateTime.Now >= ExpireTime )
                    Expire();

                Respawn();
            }
        }

        public void AdvanceLevel()
        {
            ExpireTime = DateTime.Now + ExpireDelay;

            if( Level < ChampionInfo.NumLevels )
            {
                Kills = 0;

                ++Level;

                ChampionInfo.OnChampionLevelChanged( this );
            }
            else
                SpawnChampion();
        }

        public virtual void SpawnChampion()
        {
            Kills = 0;
            Level = 0;

            Champion = GenChampion();

            if( Champion != null )
                Champion.MoveToWorld( new Point3D( X, Y, Z - 15 ), Map );

            ChampionInfo.OnChampionSpawned( this );
        }

        public virtual Mobile GenChampion()
        {
            try
            {
                return Activator.CreateInstance( ChampionInfo.Champion ) as Mobile;
            }
            catch( Exception e )
            {
                Config.Pkg.LogError( e );
                return null;
            }
        }

        public void Respawn()
        {
            if( !m_Active || Deleted || Champion != null )
                return;

            ChampionSpawnGroup group = ChampionInfo.GetGroupByLevel( Level );
            if( group == null )
                return;

            while( group.ShouldSpawn( m_Creatures ) && group.CanSpawn( m_Creatures ) )
            {
                Mobile m = group.Spawn( m_Creatures );
                if( m == null )
                    return;

                Point3D loc = GetSpawnLocation();

                // Allow creatures to turn into Paragons at Ilshenar champions.
                m.OnBeforeSpawn( loc, Map );

                if( m is BaseCreature && ( (BaseCreature)m ).IsParagon )
                {
                    if( !m.Deleted )
                        m.Delete();
                }

                m_Creatures.Add( m );
                m.MoveToWorld( loc, Map );

                if( m is BaseCreature )
                {
                    BaseCreature bc = m as BaseCreature;
                    bc.Tamable = false;

                    if( !ConfinedRoaming )
                    {
                        bc.Home = Location;
                        bc.RangeHome = (int)( Math.Sqrt( m_SpawnArea.Width * m_SpawnArea.Width + m_SpawnArea.Height * m_SpawnArea.Height ) / 2 );
                    }
                    else
                    {
                        bc.Home = bc.Location;

                        var xWall1 = new Point2D( m_SpawnArea.X, bc.Y );
                        var xWall2 = new Point2D( m_SpawnArea.X + m_SpawnArea.Width, bc.Y );
                        var yWall1 = new Point2D( bc.X, m_SpawnArea.Y );
                        var yWall2 = new Point2D( bc.X, m_SpawnArea.Y + m_SpawnArea.Height );

                        double minXDist = Math.Min( bc.GetDistanceToSqrt( xWall1 ), bc.GetDistanceToSqrt( xWall2 ) );
                        double minYDist = Math.Min( bc.GetDistanceToSqrt( yWall1 ), bc.GetDistanceToSqrt( yWall2 ) );

                        bc.RangeHome = (int)Math.Min( minXDist, minYDist );
                    }
                }

                ChampionInfo.OnChampionRespawned( this, m );
            }
        }

        public Point3D GetSpawnLocation()
        {
            Map map = Map;

            if( map == null )
                return Location;

            // Try 20 times to find a spawnable location.
            for( int i = 0; i < 20; i++ )
            {
                int x = Utility.Random( m_SpawnArea.X, m_SpawnArea.Width );
                int y = Utility.Random( m_SpawnArea.Y, m_SpawnArea.Height );

                int z = Map.GetAverageZ( x, y );

                if( Map.CanSpawnMobile( new Point2D( x, y ), z ) )
                    return new Point3D( x, y, z );
            }

            return Location;
        }

        public void Expire()
        {
            Kills = 0;

            if( Level > 0 )
                --Level;

            ExpireTime = DateTime.Now + ExpireDelay;

            ChampionInfo.OnChampionExpired( this );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( m_Active )
                LabelTo( from, "{0} (Active; Level: {1}; Kills: {2}/{3})", Type, Level, Kills, MaxKills );
            else
                LabelTo( from, "{0} (Inactive)", Type );
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.SendGump( new PropertiesGump( from, this ) );
        }

        public override void OnLocationChange( Point3D oldLoc )
        {
            if( Deleted )
                return;

            if( ChampionSpawner != null )
                ChampionSpawner.Location = new Point3D( X, Y, Z - 15 );

            m_SpawnArea.X += Location.X - oldLoc.X;
            m_SpawnArea.Y += Location.Y - oldLoc.Y;

            UpdateRegion();
        }

        public override void OnMapChange()
        {
            if( Deleted )
                return;

            if( ChampionSpawner != null )
                ChampionSpawner.Map = Map;

            UpdateRegion();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( ChampionSpawner != null )
                ChampionSpawner.Delete();

            if( m_Creatures != null )
            {
                foreach( Mobile mob in m_Creatures )
                {
                    if( !mob.Player )
                        mob.Delete();
                }

                m_Creatures.Clear();
            }

            if( Champion != null && !Champion.Player )
                Champion.Delete();

            Stop();

            UpdateRegion();
        }

        public virtual void RegisterDamageTo( Mobile m )
        {
            if( m == null )
                return;

            foreach( DamageEntry de in m.DamageEntries )
            {
                if( de.HasExpired )
                    continue;

                Mobile damager = de.Damager;

                Mobile master = damager.GetDamageMaster( m );

                if( master != null )
                    damager = master;

                RegisterDamage( damager, de.DamageGiven );
            }
        }

        public void RegisterDamage( Mobile from, int amount )
        {
            if( from == null || !from.Player )
                return;

            if( m_DamageEntries.ContainsKey( from ) )
                m_DamageEntries[ from ] += amount;
            else
                m_DamageEntries.Add( from, amount );
        }

        #region serialization
        public MiniChampionSpawn( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_DamageEntries.Count );
            foreach( var kvp in m_DamageEntries )
            {
                writer.Write( kvp.Key );
                writer.Write( kvp.Value );
            }

            writer.Write( ConfinedRoaming );
            writer.Write( m_SpawnArea );
            writer.Write( Kills );
            writer.Write( m_Active );
            writer.Write( (int)Type );
            writer.Write( m_Creatures, true );
            writer.Write( Level );
            writer.WriteItem( ChampionSpawner );
            writer.Write( ExpireDelay );
            writer.WriteDeltaTime( ExpireTime );
            writer.Write( Champion );
            writer.Write( RestartDelay );
            writer.Write( m_RestartTimer != null );

            if( m_RestartTimer != null )
                writer.WriteDeltaTime( RestartTime );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            m_DamageEntries = new Dictionary<Mobile, int>();

            int version = reader.ReadInt();

            switch( version )
            {
                case 5:
                    {
                        int entries = reader.ReadInt();
                        for( int i = 0; i < entries; ++i )
                        {
                            Mobile m = reader.ReadMobile();
                            int damage = reader.ReadInt();
                            if( m == null )
                                continue;

                            m_DamageEntries.Add( m, damage );
                        }

                        ConfinedRoaming = reader.ReadBool();
                        reader.ReadBool();
                        m_SpawnArea = reader.ReadRect2D();
                        Kills = reader.ReadInt();

                        bool active = reader.ReadBool();
                        Type = (MiniChampionSpawnType)reader.ReadInt();
                        m_Creatures = reader.ReadStrongMobileList();
                        Level = reader.ReadInt();
                        ChampionSpawner = reader.ReadItem<MiniChampionSpawner>();
                        ExpireDelay = reader.ReadTimeSpan();
                        ExpireTime = reader.ReadDeltaTime();
                        Champion = reader.ReadMobile();
                        RestartDelay = reader.ReadTimeSpan();

                        if( reader.ReadBool() )
                        {
                            RestartTime = reader.ReadDeltaTime();
                            BeginRestart( RestartTime - DateTime.Now );
                        }

                        if( ChampionSpawner == null )
                            Delete();
                        else if( active )
                            Start();

                        break;
                    }
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( UpdateRegion ) );
        }
        #endregion
    }
}