using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.PartySystem;

namespace Midgard.Engines.BossSystem
{
    /// <summary>
    /// Contain all informations about Locations
    /// </summary>
    internal abstract class Dungeon
    {
        public readonly Region Region;
        //private readonly List<Rectangle2D> m_RegionRectanglesList = new List<Rectangle2D>();
        private bool m_Enabled = false;
        private bool m_StartEnabledifNotLoadedfromDb = false;
        private List<QuestItem> QuestItems;
        protected List<DungeonJoinRequirement> JoinRequirements;
        protected Dictionary<PlayerMobile, PlayerRecord> Records;
        private Dictionary<PlayerMobile, DungeonStageSet> StageSets;
        protected bool RandomAreaEffects = false;
        //private List<Mobile> Mobiles;

        /// <summary>
        /// This region is the BOUND of all the system.
        /// </summary>
        /// <param name="region"></param>
        protected Dungeon( Region region )
        {
            Records = new Dictionary<PlayerMobile, PlayerRecord>();
            JoinRequirements = new List<DungeonJoinRequirement>();
            StageSets = new Dictionary<PlayerMobile, DungeonStageSet>();
            QuestItems = new List<QuestItem>();
            //Mobiles = new List<Mobile>();
            Region = region;
            Name = "Dungeon of " + region.Name;
            ResetAsDefault();
        }

        /// <summary>
        /// Dungeon Name 
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Is this dungeon loaded from DB? 
        /// </summary>
        public bool LoadedFromDB { get; private set; }

        /// <summary>
        /// If a dungeon is disabled, all events will not be raised
        /// </summary>
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                if( m_Enabled != value )
                    m_Enabled = OnDungeonEnabledChanged( value );
            }
        }

        /// <summary>
        /// If a dungeon is created. 
        /// </summary>
        public bool Created { get; private set; }

        /// <summary>
        /// Event raised after changing enabled value
        /// </summary>
        /// <param name="changeTo"></param>
        /// <returns></returns>
        protected virtual bool OnDungeonEnabledChanged( bool changeTo )
        {
            return changeTo;
        }

        /// <summary>
        /// Destroy a created dungeon, Enabled will be setted to false.
        /// </summary>
        public void Destroy()
        {
            if( !Created )
                return;
            Enabled = false;
            Created = false;
            FireOnDestroy();
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Dungeon will be instantiated. Is is ready for player incoming. 
        ///  Beware: all resources created here need to be released into OnDestroy Method.
        /// </summary>
        public void Create()
        {
            if( Created )
                return;

            if( Core.Debug )
                Core.Pkg.LogInfo( "Create Dungeon \"{0}\".", this );

            if( FireOnCreate() )
            {
                if( Core.Debug )
                    Core.Pkg.LogInfoLine( "created." );
                Created = true;
                Enabled = false;
            }
            else
            {
                if( Core.Debug )
                    Core.Pkg.LogInfoLine( "Fail to create !" );
                FireOnDestroy(); //force destroy... beware: enabled and created can be FALSE.
                Enabled = false;
                Created = false;
            }
        }

        /// <summary>
        /// Invoked after load from DB if dungeon is not loaded.
        /// </summary>
        internal void ResetAsDefault()
        {
            LoadedFromDB = false;
            Destroy();
        }

        public virtual bool CanBeCreated { get { return !Created; } }

        public bool ReCreateTimers()
        {
            return OnTimersCreate();
        }


        private bool FireOnCreate()
        {
            var ret = ReCreateTimers();
            if( !ret )
                return ret;
            ret = OnCreate();

            return ret;
        }

        /// <summary>
        /// When dungeon is destroyed, this method will be called to recreate dungeon configuration. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        protected abstract bool OnCreate();

        /// <summary>
        /// Invoked after load or before OnCreate. Use this method to reinstantiate timers. 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        protected virtual bool OnTimersCreate() { return true; }

        public void RegisterQuestItem( QuestItem item )
        {
            if( QuestItems.Contains( item ) )
                return;
            QuestItems.Add( item );
        }

        private void FireOnDestroy()
        {
            foreach( var item in QuestItems )
            {
                if( !item.Deleted )
                    item.Delete();
            }
            QuestItems.Clear();
            StageSets.Clear();
            OnDestroy();
        }

        /// <summary>
        /// All dungeon resources need to be disposed here.
        /// </summary>
        protected abstract void OnDestroy();

        /// <summary>
        /// Call first of all (Before Deserialize and After Regions load).
        /// </summary>
        internal void Configure()
        {
            EventSink.PlayerDeath += HandleEventSinkMobileDeath;
            EventSink.NonPlayerDeath += HandleEventSinkMobileDeath;
            Region.OnRegionEvent += HandleRegionOnRegionEvent;
        }

        protected DungeonJoinRequirement[] GetPlayerUnSatifiedJoinRequirements( PlayerMobile player )
        {
            var ret = new List<DungeonJoinRequirement>();
            foreach( var req in JoinRequirements )
            {
                if( !req.SatisfiedBy( this, player ) )
                    ret.Add( req );
            }
            return ret.ToArray();
        }

        protected bool PlayerSatisfyJoinRequirements( PlayerMobile player )
        {
            return ( GetPlayerUnSatifiedJoinRequirements( player ).Length == 0 );
        }

        #region Dungeon Events
        void HandleRegionOnRegionEvent( Region.OnRegionEventHandlerArgs e )
        {
            switch( e.Event )
            {
                case "OnEnter":
                    InternalOnMobileEnter( e.Arguments[ 0 ] as Mobile );
                    break;
                case "OnExit":
                    InternalOnMobileExit( e.Arguments[ 0 ] as Mobile );
                    break;
            }
        }
        private void InternalOnMobileEnter( Mobile mobile )
        {
            OnMobileEnter( mobile );
        }
        protected virtual void OnMobileEnter( Mobile Mobile ) { }

        private void InternalOnMobileExit( Mobile mobile )
        {
            OnMobileExit( mobile );
        }
        protected virtual void OnMobileExit( Mobile Mobile ) { }

        private void HandleEventSinkMobileDeath( MobileDeathEventArgs e )
        {
            if( !Enabled )
                return;

            if( !Region.Contains( e.Mobile.Location ) )
                return;

            if( e.Mobile.Player )
                InternalOnPlayerDeath( (PlayerMobile)e.Mobile, e.Corpse );
            else if( e.Mobile is BaseCreature )
                InternalOnCreatureDeath( (BaseCreature)e.Mobile, e.Corpse );
        }

        private void InternalOnPlayerDeath( PlayerMobile Player, Container Corpse )
        {
            OnPlayerDeath( Player, Corpse );
        }
        private void InternalOnCreatureDeath( BaseCreature Creature, Container Corpse )
        {
            var involvedPlayers = new List<PlayerMobile>();

            foreach( var mobinfo in Creature.Aggressors )
            {
                if( mobinfo.Expired )
                    continue;
                var mob = mobinfo.Attacker;
                var creature = mob as BaseCreature;
                var player = mob as PlayerMobile;
                if( creature != null && player == null ) //summoned or controlled?
                    player = creature.GetMaster() as PlayerMobile;

                if( player == null )
                    continue;

                var party = Party.Get( player );

                if( party != null )
                {
                    foreach( var ply in party.Members )
                    {
                        var stage = GetPlayerStageSet( (PlayerMobile)ply.Mobile );
                        if( stage == null && !involvedPlayers.Contains( (PlayerMobile)ply.Mobile ) )
                            involvedPlayers.Add( (PlayerMobile)ply.Mobile );
                    }
                }
                else
                {
                    var stage = GetPlayerStageSet( player );
                    if( stage == null && !involvedPlayers.Contains( player ) ) //no stage set....can be found ring :-)
                        involvedPlayers.Add( player );
                }
            }

            if( involvedPlayers.Count > 0 )
                OnCreatureDeath( Creature, Corpse, involvedPlayers );
        }

        protected virtual void OnCreatureDeath( BaseCreature Creature, Container Corpse, List<PlayerMobile> InvolvedPlayers ) { }
        protected virtual void OnPlayerDeath( PlayerMobile Player, Container Corpse ) { }
        #endregion

        #region StageSets

        public DungeonStageSet GetPlayerStageSet( PlayerMobile Player )
        {
            if( StageSets.ContainsKey( Player ) )
                return StageSets[ Player ];
            return null;
        }
        public void RemovePlayerStageSet( PlayerMobile Player )
        {
            if( StageSets.ContainsKey( Player ) )
                StageSets.Remove( Player );
        }

        public void ChangePlayerStageSet( DungeonStageSet stage )
        {
            if( !StageSets.ContainsKey( stage.Player ) )
                StageSets.Add( stage.Player, stage );
            else
                StageSets[ stage.Player ] = stage;
        }
        #endregion


        #region Serailization
        /// <summary>
        /// Invoked while load from dB
        /// </summary>
        /// <param name="reader"></param>
        internal void Deserialize( GenericReader reader )
        {
            LoadedFromDB = true;
            var version = reader.ReadUShort();

            switch( version )
            {
                case 0:
                    {
                        Created = reader.ReadBool();
                        m_Enabled = reader.ReadBool();

                        QuestItems.Clear();
                        var count = reader.ReadUShort(); //items
                        for( int h = 0; h < count; h++ )
                        {
                            var it = reader.ReadItem() as QuestItem;
                            if( it != null )
                            {
                                QuestItems.Add( it );
                                it.Dungeon = this;
                            }
                        }

                        Records.Clear();
                        count = reader.ReadUShort(); //records
                        for( int h = 0; h < count; h++ )
                        {
                            var it = reader.ReadMobile() as PlayerMobile;
                            var rec = new PlayerRecord( reader );
                            if( it != null )
                                Records.Add( it, rec );
                        }

                        StageSets.Clear();
                        count = reader.ReadUShort(); //records
                        for( int h = 0; h < count; h++ )
                        {
                            var type = reader.ReadString();
                            var player = reader.ReadMobile() as PlayerMobile;
                            var stage = (DungeonStageSet)GetType().Assembly.GetType( type ).GetConstructor( new Type[] { typeof( Dungeon ), typeof( PlayerMobile ), typeof( GenericReader ) } ).Invoke( new object[] { this, player, reader } );
                            if( player != null )
                                StageSets.Add( player, stage );
                        }

                        break;
                    }
            }

            OnDeserialize( reader );
        }

        protected virtual void OnDeserialize( GenericReader reader )
        {
        }

        /// <summary>
        /// Invoked while Save to DB
        /// </summary>
        /// <param name="writer"></param>
        internal void Serialize( GenericWriter writer )
        {
            writer.Write( (ushort)0 ); //version

            writer.Write( Created );
            writer.Write( Enabled );

            writer.Write( (ushort)QuestItems.Count );
            foreach( var item in QuestItems )
            {
                writer.Write( item );
            }

            writer.Write( (ushort)Records.Count );
            foreach( var elem in Records )
            {
                writer.Write( elem.Key );
                elem.Value.Serialize( writer );
            }

            writer.Write( (ushort)StageSets.Count );
            foreach( var elem in StageSets )
            {
                writer.Write( elem.Value.GetType().FullName );
                writer.Write( elem.Value.Player );
                elem.Value.Serialize( writer );
            }

            OnSerialize( writer );
        }
        protected virtual void OnSerialize( GenericWriter writer )
        {
        }
        #endregion



    }
}