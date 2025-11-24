/***************************************************************************
 *                                     BasePlant.cs
 *                            		------------------
 *  begin                	: Agosto, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Main class of the plant system.
 * 			Trees and crops inherits from this class.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Midgard.Engines.WarSystem;

using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Network;
using Map = Server.Map;
using Utility = Server.Utility;

namespace Midgard.Engines.PlantSystem
{
    public abstract class BasePlant : Item
    {
        private int m_CareLevel; // How much care a plant received. Rated from -10 (worst) to 10 (best).
        private int m_CurrentPhase; // The current phase of the plant.
        private bool m_GotFungus; // True if plant got fungus
        private bool m_GotPest; // True if plant got pest									
        private bool m_IsGrowing; // A plant will only be able to change phase or produce if growing. 
        private bool m_IsInDebugMode; // If true plants speaks main events of its life.
        private bool m_IsPublic; // If true anyone can access its fruits and destroy.
        private Mobile m_Owner; // The owner of the plant. If plant is not public only Owner has access to it.
        private DateTime m_LastWatered; // Last time the plant got atered in real world time.
        private int m_Seeds; // Seeds produced by our plant
        private DateTime m_TimePlanted; // When is the plant planted								
        private DateTime m_UntilIsFertilized; // DateTime of last fertilization
        private int m_Yield; // Current number of crops left on the plant

        private static readonly Dictionary<Mobile, List<BasePlant>> m_Table = new Dictionary<Mobile, List<BasePlant>>();

        private static readonly List<BasePlant> m_AllPlants = new List<BasePlant>();

        // General variables about plants
        public abstract bool IsDestroyable { get; } // True if a player can destroy it.
        public abstract bool NeedWater { get; } // True if the plant needs water to grow. Not used yet.		

        // Variables concerning plant evolution and death
        public abstract bool CanGrow { get; } // If false plant life is "paused".
        public abstract TimeSpan DormantDrought { get; } // How long a drought a plant can endure before dying.
        public abstract int GrowthTick { get; } // How many ticks a young plant will take a phase change.
        public abstract TimeSpan LifeSpan { get; } // How long a plant will live.
        public abstract bool LimitedLifeSpan { get; } // True if a plant has a limited life. Plant will die after life span ends.
        public abstract TimeSpan LongDrought { get; } // How long a drought a plant can endure before carelevel directly drops to below -5.

        // Variables concerning phases and ids
        public abstract int[] PhaseIDs { get; } // Graphics af all phases.
        public abstract string[] PhaseName { get; } // Name displayed on plant for each phase.

        // Variables concerning produce action
        public abstract bool CanProduce { get; } // True if plant will produce anything after full grown.
        public abstract int Capacity { get; } // Max amount of crop/fruit on our plant. 0 if does not produce.
        public abstract string CropName { get; } // Name of product of crop/fruit on our plant. "" if does not produce.
        public abstract string CropPluralName { get; }
        // Plural name of product of crop/fruit on our plant. "" if does not produce.
        public abstract int ProduceTick { get; } // How many ticks it takes to regrow crop/fruit. 0 if does not produce.
        public abstract int ProductPhaseID { get; }
        // Id of phase with crops/fruits. 0 if does not have one or does not produce.
        public abstract bool HasParentSeed { get; } // If plant is born from a seed or not
        public abstract Type TypeOfParentSeed { get; } // Type of the seed this plant born from

        // Variables concerning harvesting
        public abstract double HarvestDelay { get; } // Delay in seconds between harvest actions.
        public abstract Type HarvestingTool { get; } // Harvest tool. If the player need any tool to harvest.
        public abstract bool HarvestInPack { get; } // Where the product is placed (on ground or in pack).
        public abstract double MinSkillToHarvest { get; } // Minimum skill to harvest this plant;
        public SkillName RequiredSkillNameToHarvest { get { return SkillName.Camping; } }
        public abstract double MinDiffSkillToCare { get; }
        public SkillName SkillNameToCare { get { return SkillName.Camping; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int AgeInDays
        {
            get { return ( (int)( DateTime.Now - TimePlanted ).TotalDays ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CareLevel
        {
            get { return m_CareLevel; }
            set
            {
                int oldValue = m_CareLevel;
                if( oldValue != value )
                {
                    if( value <= PlantHelper.CareLevelWorst )
                        m_CareLevel = PlantHelper.CareLevelWorst;
                    else if( value > PlantHelper.CareLevelBest )
                        m_CareLevel = PlantHelper.CareLevelBest;
                    else
                        m_CareLevel = value;

                    OnCareLevelChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int CurrentPhase
        {
            get { return m_CurrentPhase; }
            set
            {
                int oldValue = m_CurrentPhase;
                if( oldValue != value )
                {
                    if( value < 0 )
                        m_CurrentPhase = 0;
                    else if( value > PhaseCap )
                        m_CurrentPhase = PhaseCap;
                    else
                        m_CurrentPhase = value;

                    OnCurrentPhaseChanged( oldValue );
                }
            }
        }

        public TimeSpan Drought // the time past since last watering in real world time duration
        {
            get { return DateTime.Now - LastWatered; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DroughtInfo
        {
            get { return ( (int)Drought.TotalMinutes ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool GotFungus
        {
            get { return m_GotFungus; }
            set
            {
                bool oldValue = m_GotFungus;
                if( oldValue != value )
                {
                    m_GotFungus = value;

                    OnGotFungusChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool GotPest
        {
            get { return m_GotPest; }
            set
            {
                bool oldValue = m_GotPest;
                if( oldValue != value )
                {
                    m_GotPest = value;

                    OnGotPestChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasAnyDesease
        {
            get { return m_GotFungus || m_GotPest; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsFertilized // If true our plant is in fertilizer-boost status
        {
            get { return DateTime.Now < m_UntilIsFertilized; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsFullGrown // If true a plant is an adult one. Only adult plants can produce
        {
            get { return m_CurrentPhase == PhaseCap; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsInGarden { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsGrowing
        {
            get { return m_IsGrowing; }
            set
            {
                bool oldValue = m_IsGrowing;
                if( oldValue != value )
                {
                    m_IsGrowing = value;

                    OnIsGrowingChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsInDebugMode
        {
            get { return m_IsInDebugMode; }
            set
            {
                bool oldValue = m_IsInDebugMode;
                if( oldValue != value )
                {
                    m_IsInDebugMode = value;

                    OnIsInDebugModeChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsPublic
        {
            get { return m_IsPublic; }
            set
            {
                bool oldValue = m_IsPublic;
                if( oldValue != value )
                {
                    m_IsPublic = value;

                    OnIsPublicChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public DateTime LastWatered
        {
            get { return m_LastWatered; }
            set
            {
                DateTime oldValue = m_LastWatered;
                if( oldValue != value )
                {
                    if( value < DateTime.MinValue )
                        m_LastWatered = DateTime.MinValue;
                    else if( value > DateTime.MaxValue )
                        m_LastWatered = DateTime.MaxValue;
                    else
                        m_LastWatered = value;

                    OnLastWateredChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set
            {
                if( m_Owner != null )
                {
                    List<BasePlant> list;
                    m_Table.TryGetValue( m_Owner, out list );

                    if( list == null )
                        m_Table[ m_Owner ] = list = new List<BasePlant>();

                    list.Remove( this );
                }

                m_Owner = value;

                if( m_Owner != null )
                {
                    List<BasePlant> list;
                    m_Table.TryGetValue( m_Owner, out list );

                    if( list == null )
                        m_Table[ m_Owner ] = list = new List<BasePlant>();

                    list.Add( this );
                }

                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PhaseCap
        {
            get { return PhaseIDs.Length - 1; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Seeds
        {
            get { return m_Seeds; }
            set
            {
                int oldValue = m_Seeds;
                if( oldValue != value )
                {
                    if( value < 0 )
                        m_Seeds = 0;
                    else if( value > Capacity )
                        m_Seeds = Capacity;
                    else
                        m_Seeds = value;

                    OnSeedsChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public DateTime TimePlanted
        {
            get { return m_TimePlanted; }
            set { m_TimePlanted = value; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public DateTime UntilIsFertilized
        {
            get { return m_UntilIsFertilized; }
            set
            {
                DateTime oldValue = m_UntilIsFertilized;
                if( oldValue != value )
                {
                    if( value < DateTime.MinValue )
                        m_LastWatered = DateTime.MinValue;
                    else if( value > DateTime.MaxValue )
                        m_UntilIsFertilized = DateTime.MaxValue;
                    else
                        m_UntilIsFertilized = value;

                    OnUntilIsFertilizedChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Yield
        {
            get { return m_Yield; }
            set
            {
                int oldValue = m_Yield;
                if( oldValue != value )
                {
                    if( value < 0 )
                        m_Yield = 0;
                    else if( value > Capacity )
                        m_Yield = Capacity;
                    else
                        m_Yield = value;

                    OnYieldChanged( oldValue );
                }
            }
        }

        #region metodi

        /// <summary>
        /// Event occurring when a plant is created.
        /// </summary>
        public virtual void InitPlant()
        {
            ItemID = PhaseIDs[ 0 ];
            Name = PhaseName[ 0 ];

            TimePlanted = DateTime.Now;
            LastWatered = DateTime.Now;
        }

        public static void Grow_OnTick()
        {
            if( BaseWar.DisableTimersDuringBattle && WarSystem.Core.Instance.WarPending )
                return;

            Stopwatch watch = Stopwatch.StartNew();

            Config.Pkg.LogInfo( "Checking plants grow..." );

            foreach( BasePlant plant in m_AllPlants )
                plant.CheckGrow();

            watch.Stop();
            Config.Pkg.LogInfoLine( "done in {0:F2} seconds.", watch.Elapsed.TotalSeconds );
        }

        /// <summary>
        /// Counter that trigger growth and produce events
        /// </summary>
        public int GrowCounter { get; private set; }

        /// <summary>
        /// Ccounter that trigger health check events
        /// </summary>
        public int HealthCounter { get; private set; }

        public DateTime LastGrowCheck { get; private set; }

        public TimeSpan GrowDelay
        {
            get
            {
                TimeSpan delay = PlantHelper.PlantTimerInterval;

                if( IsInDebugMode )
                    delay = PlantHelper.DebugPlantTimerInterval;

                if( IsFertilized )
                    delay = TimeSpan.FromMinutes( delay.TotalMinutes / PlantHelper.FertilizerDivider );

                return delay;
            }
        }

        private void CheckGrow()
        {
            if( Deleted )
                return;

            if( DateTime.Now < TimePlanted + PlantHelper.PlantTimerDormant )
                return;

            if( DateTime.Now < LastGrowCheck + GrowDelay )
                return;
            else
                LastGrowCheck = DateTime.Now;

            if( !CanGrow ) // first validate that plant still exists and can grow
                return;

            if( LimitedLifeSpan ) // check for old plant that should be removed
            {
                if( DateTime.Now > TimePlanted + LifeSpan )
                {
                    PlantSystemLog.WriteInfo( this, "Our plant is dead." );
                    Timer.DelayCall( TimeSpan.FromSeconds( 20 ), new TimerCallback( Die ) );
                }
            }

            if( IsFertilized ) // If is time to remove fertilizer Boost
            {
                if( DateTime.Now > UntilIsFertilized )
                {
                    PlantSystemLog.WriteInfo( this, "Our plant fertilizer effect ended." );
                    UntilIsFertilized = DateTime.MinValue;
                }
            }

            if( !IsGrowing ) // dormeant plants cannot grow or produce
                return;

            DebugMessage( IsInDebugMode, string.Format( "Health: {0} - Grow: {1}", HealthCounter, GrowCounter ), this );

            HealthCounter++;

            #region it's time to check the health
            if( HealthCounter > PlantHelper.HealthCheckTick )
            {
                HealthCounter = 0;
                PlantSystemLog.WriteInfo( this, "HealthCounter is 0." );

                if( Drought > DormantDrought )
                {
                    if( CareLevel > PlantHelper.CareLevelWorst )
                    {
                        PlantSystemLog.WriteInfo( this, "Our plant is now in DormantDrought." );
                        CareLevel = PlantHelper.CareLevelWorst;
                        return;
                    }
                }

                if( IsGrowing )
                {
                    if( Drought > LongDrought ) // long drought will drop carelevel to bad
                    {
                        if( CareLevel > PlantHelper.CareLevelBad )
                        {
                            PlantSystemLog.WriteInfo( this, "Our plant is now in LongDrought." );
                            CareLevel = PlantHelper.CareLevelBad;
                        }
                    }

                    bool hasToDropByFungus = Utility.RandomDouble() < PlantHelper.FungusDropChance;
                    if( GotFungus && hasToDropByFungus ) // fungus may drop care level
                    {
                        PlantSystemLog.WriteInfo( this, "Our plant CareLevel lowered because of a fungus." );
                        CareLevel--;
                    }

                    bool hasToDropByPest = Utility.RandomDouble() < PlantHelper.PestDropChance;
                    if( GotPest && hasToDropByPest ) // pest may drop care level
                    {
                        PlantSystemLog.WriteInfo( this, "Our plant CareLevel lowered because of pest attack." );
                        CareLevel--;
                    }

                    if( !HasAnyDesease ) // plant without desease can grow
                    {
                        if( DateTime.Now < LastWatered + PlantHelper.HealthCheckWaterInterval )
                        {
                            if( CareLevel < PlantHelper.CareLevelGood )
                            {
                                PlantSystemLog.WriteInfo( this, "Our plant CareLevel increased because of a good watering." );
                                CareLevel++;
                            }
                        }
                    }

                    CheckGetDisease(); // check if a pest or a fungus attacked our plant
                }
            }
            #endregion

            if( HasAnyDesease ) // affected plants cannot grow or produce
                return;

            #region it's time to check grow and maybe to produce
            if( !IsFullGrown ) // -9 will barely grow, above 5 will always grow
            {
                bool successGrow = CareLevel > Utility.RandomMinMax( PlantHelper.CareLevelWorst, PlantHelper.CareLevelGood );

                if( IsInDebugMode )
                    successGrow = true;

                if( successGrow ) // check if has managed to grow
                {
                    PlantSystemLog.WriteInfo( this, "Our plant GrowCounter increased." );
                    GrowCounter++;

                    if( GrowCounter >= GrowthTick )
                    {
                        PlantSystemLog.WriteInfo( this, "Our plant has grown." );
                        Grow(); // plant grow to next phase
                        GrowCounter = 0;
                    }
                }
            }
            else
            {
                if( CareLevel > PlantHelper.CareLevelBad ) // bad care level plant force not to produce
                {
                    bool successCropProduce = CareLevel > Utility.RandomMinMax( PlantHelper.CareLevelWorst, PlantHelper.CareLevelGood );

                    if( IsInDebugMode )
                        successCropProduce = true;

                    if( CanProduce && Yield < Capacity )
                    {
                        if( successCropProduce ) // check if has managed to produce
                        {
                            PlantSystemLog.WriteInfo( this, "Our plant GrowCounter has increased (Crop Check)." );
                            GrowCounter++;

                            if( GrowCounter >= ProduceTick )
                            {
                                PlantSystemLog.WriteInfo( this, "Our plant has Produce Crop." );
                                ProduceCrop(); // produce crops/fruits
                                GrowCounter = 0;
                            }
                        }
                    }
                }
                if( CareLevel > PlantHelper.CareLevelGood ) // bad care level plant force not to produce
                {
                    bool successSeedProduce = CareLevel > 0.05 * Utility.RandomMinMax( PlantHelper.CareLevelWorst, PlantHelper.CareLevelBest );

                    if( IsInDebugMode )
                        successSeedProduce = true;

                    if( CanProduce && Seeds < 1 )
                    {
                        if( successSeedProduce ) // check if has managed to produce
                        {
                            PlantSystemLog.WriteInfo( this, "Our plant GrowCounter has increased (Seed Check)." );
                            GrowCounter++;

                            if( GrowCounter >= ProduceTick )
                            {
                                PlantSystemLog.WriteInfo( this, "Our plant has Produce Seed." );
                                ProduceSeed(); // produce seeds
                                GrowCounter = 0;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        public static List<BasePlant> GetPlants( Mobile m )
        {
            List<BasePlant> list = new List<BasePlant>();

            if( m != null )
            {
                List<BasePlant> exists;
                m_Table.TryGetValue( m, out exists );

                if( exists != null )
                {
                    foreach( BasePlant plant in exists )
                    {
                        if( plant != null && !plant.Deleted && plant.Owner == m )
                            list.Add( plant );
                    }
                }
            }

            return list;
        }

        #region OnChanged events

        /// <summary>
        /// Event invoked when a plant care level changes.
        /// </summary>
        public virtual void OnCareLevelChanged( int oldValue )
        {
            CheckPlantID(); // check if something happens to plant ID

            DebugMessage( m_IsInDebugMode, "My level of care changed...", this );

            if( CareLevel <= PlantHelper.CareLevelBad ) // put on warning hue if carelevel lower than -4
            {
                if( Yield > 0 )
                {
                    Yield = 0;
                    DebugMessage( m_IsInDebugMode,
                                 string.Format( "I've lost my >>products<< due to a level of care of {0}.", m_CareLevel ),
                                 this );
                }
                if( Seeds > 0 )
                {
                    Seeds = 0;
                    DebugMessage( m_IsInDebugMode,
                                 string.Format( "I've lost my >>seeds<< due to a level of care of {0}.", m_CareLevel ),
                                 this );
                }
                Hue = PlantHelper.CareWarningHue[ m_CareLevel + 10 ];
                DebugMessage( m_IsInDebugMode,
                             string.Format( "My hue changed due to a level of care of {0}.", m_CareLevel ), this );
            }
            else if( Hue != 0 ) // remove signal of bad care.
            {
                Hue = 0;
                DebugMessage( m_IsInDebugMode,
                             string.Format( "My hue returned normal due to a level of care of {0}.", m_CareLevel ), this );
            }

            if( CareLevel <= PlantHelper.CareLevelWorst ) // if plant is at level CareLevelWorst has to go dormant
            {
                StopGrowth();
                DebugMessage( m_IsInDebugMode, "I've stopped growing due to my drastic level of care.", this );
            }

            if( CareLevel > oldValue )
            {
                if( !IsGrowing )
                    LeaveDormant();
                PublicOverheadMessage( MessageType.Regular, 0x3B2, false, "* good job *" );
            }
            else
            {
                PublicOverheadMessage( MessageType.Regular, 0x3B2, false, "* not a good deal for this plant *" );
            }
        }

        /// <summary>
        /// Event invoked when a plant phase changes.
        /// </summary>
        public virtual void OnCurrentPhaseChanged( int oldLevel )
        {
            DebugMessage( m_IsInDebugMode, string.Format( "My phase changed to {0} from {1}", CurrentPhase, oldLevel ),
                         this );
            CheckPlantID();
            CheckName();
        }

        /// <summary>
        /// Event invoked when a plant is watered.
        /// </summary>
        public virtual void OnLastWateredChanged( DateTime oldValue )
        {
            if( !m_IsGrowing ) // giving water to a dried plant will wake it
                CareLevel++;
            else if( !HasAnyDesease )
            {
                if( DateTime.Now > oldValue + PlantHelper.ProperWaterInterval )
                {
                    CheckFarmerSkillIncreased( EventSkillType.Growing );
                    bool waterShouldBoost = Utility.RandomDouble() < PlantHelper.WaterBoostChance;
                    if( CareLevel < PlantHelper.CareLevelGood && waterShouldBoost )
                        CareLevel++;
                    DebugMessage( m_IsInDebugMode, "Oh yeah! I like water!", this );
                }
                else
                {
                    bool waterShouldLower = Utility.RandomDouble() < PlantHelper.TooWaterDropChance;
                    if( CareLevel > PlantHelper.CareLevelWorst && waterShouldLower )
                        CareLevel--;
                    DebugMessage( m_IsInDebugMode, "Hey! You gave me too much water, fool!", this );
                }
            }
        }

        /// <summary>
        /// Event invoked when a plant fungus status changes.
        /// </summary>
        public virtual void OnGotFungusChanged( bool oldLevel )
        {
            if( m_GotFungus )
                DebugMessage( m_IsInDebugMode, "Sorry, I've got a terrible fungus...", this );
            else
                DebugMessage( m_IsInDebugMode, "Yeah! I hate fungs...", this );
        }

        /// <summary>
        /// Event invoked when a plant pest status changes.
        /// </summary>
        public virtual void OnGotPestChanged( bool oldLevel )
        {
            if( m_GotPest )
                DebugMessage( m_IsInDebugMode, "Hey, I need a pesticide...", this );
            else
                DebugMessage( m_IsInDebugMode, "Gooood job. Pest has gone...", this );
        }

        /// <summary>
        /// Event invoked when a plant debug mode status changes.
        /// </summary>
        public virtual void OnIsInDebugModeChanged( bool oldValue )
        {
            if( m_IsInDebugMode )
            {
                DebugMessage( true, "I'm talking you all my problems...", this );
            }
            else
            {
                DebugMessage( true, "I'm leaving debug mode. Bye!", this );
            }
        }

        /// <summary>
        /// Event invoked when a plant isGrowing status changes.
        /// </summary>
        public virtual void OnIsGrowingChanged( bool oldValue )
        {
            DebugMessage( m_IsInDebugMode, string.Format( "I'm {0} growing now.", ( oldValue ? "stopped" : "started" ) ),
                         this );
        }

        /// <summary>
        /// Event invoked when a plant isPublic status changes.
        /// </summary>
        public virtual void OnIsPublicChanged( bool oldValue )
        {
            DebugMessage( m_IsInDebugMode,
                         string.Format( "My security status changed to {0}.", ( m_IsPublic ? "public" : "private" ) ), this );
        }

        /// <summary>
        /// Event invoked when fertilizer is applied or removed from our plant
        /// </summary>
        public virtual void OnUntilIsFertilizedChanged( DateTime oldValue )
        {
            if( DateTime.Now > UntilIsFertilized )
            {
                DebugMessage( m_IsInDebugMode, "Hey! Fertilized effect ended... don't you have another dose?", this );
            }
            else
            {
                DebugMessage( m_IsInDebugMode, "Nice! This fertilizer will boost my growth!", this );
            }
        }

        /// <summary>
        /// Event invoked when our plant seeds amount change
        /// </summary>
        public virtual void OnSeedsChanged( int oldValue )
        {
            DebugMessage( m_IsInDebugMode, "My seeds quantity changed.", this );
            CheckPlantID();
        }

        /// <summary>
        /// Event invoked when a plant yield changes.
        /// </summary>
        public virtual void OnYieldChanged( int oldValue )
        {
            DebugMessage( m_IsInDebugMode, "My crops quantity changed.", this );
            CheckPlantID();
        }

        #endregion

        #region Life action from e to our plant

        /// <summary>
        /// Event occurring when a plant dies.
        /// </summary>
        public virtual void Die()
        {
            if( Deleted )
                return;

            Delete();
        }

        /// <summary>
        /// Event occurring when a plant got fertilizer.
        /// </summary>
        public virtual void Fertilize( PlantPotionLevel level )
        {
            if( Deleted || !CanGrow )
                return;

            if( !IsFertilized )
            {
                if( m_IsGrowing )
                {
                    CheckFarmerSkillIncreased( EventSkillType.Growing );
                    double duration = BaseFertilizerPotion.GetFertilizerDuration( level );
                    UntilIsFertilized = DateTime.Now + TimeSpan.FromDays( duration );
                }
            }
            else
            {
                DebugMessage( m_IsInDebugMode, "Hey! I'm already boosted!", this );
            }
        }

        /// <summary>
        /// Event occurring when a plant got fungicized.
        /// </summary>
        public virtual void Fungicide()
        {
            if( Deleted || !IsGrowing )
                return;

            if( GotFungus )
            {
                CheckFarmerSkillIncreased( EventSkillType.Growing );
                GotFungus = false;
            }
            else
            {
                bool tooFungicideShouldLower = Utility.RandomDouble() < PlantHelper.TooFungicideDropChance;
                if( tooFungicideShouldLower && CareLevel > PlantHelper.CareLevelWorst )
                    CareLevel--;
                DebugMessage( m_IsInDebugMode, "Coff Coff! Too much powder on me sir!", this );
            }
        }

        /// <summary>
        /// Event occurring when a plant has to grow up a phase.
        /// </summary>
        public virtual void Grow()
        {
            if( Deleted || IsFullGrown )
                return;

            CurrentPhase++;

            CheckFarmerSkillIncreased( EventSkillType.Growing );
        }

        /// <summary>
        /// Overridable: harvest a crop if possible from a plant.
        /// </summary>
        /// <param name="harvester">who would the fruits of our plant</param>
        /// <param name="harvestToPack">if true crops are put in harvester pack instead of the ground</param>    
        public virtual void GotHarvested( Mobile harvester, bool harvestToPack )
        {
            if( harvester == null )
                return;

            if( Deleted || Map == Map.Internal )
                return;

            Point3D location = Location;
            Map map = Map;

            if( harvester.BeginAction( typeof( BasePlant ) ) )
            {
                harvester.Direction = harvester.GetDirectionTo( location );

                harvester.Animate( 32, 5, 1, true, false, 0 );
                harvester.PlaySound( 0x4F );

                harvester.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* you begin harvesting *" );

                int cropsHarvested = Utility.Random( Yield + 1 ); // crops came always in 0-5 amount
                cropsHarvested = Math.Min( cropsHarvested, 5 ); // obv if plant has enought crops

                int seedsHarvested = Utility.Random( Seeds + 1 ); // seeds came always in 0-3 amount
                seedsHarvested = Math.Min( seedsHarvested, 3 ); // obv if plant has enought seeds

                if( cropsHarvested + seedsHarvested < 1 )
                {
                    harvester.SendMessage( "You didn't manage to harvest anything." );
                }
                else
                {
                    Yield -= cropsHarvested; // remove crops from plant
                    Seeds -= seedsHarvested; // and remove seeds

                    System.Text.StringBuilder message = new System.Text.StringBuilder();
                    message.Append( "You harvested " );

                    if( cropsHarvested > 0 )
                        message.AppendFormat( "{0} {1}", cropsHarvested, ( cropsHarvested > 1 ? CropPluralName : CropName ) );

                    if( seedsHarvested > 0 )
                    {
                        if( cropsHarvested > 0 )
                            message.Append( " and " );

                        message.AppendFormat( "{0} seed{1}", seedsHarvested, ( seedsHarvested > 1 ? "s" : "" ) );
                    }

                    message.Append( "." );
                    harvester.SendMessage( message.ToString() );

                    Item crop = GetCropObject(); // crops have their abstract constructor
                    Item seed = null;

                    if( HasParentSeed )
                        seed = Activator.CreateInstance( TypeOfParentSeed ) as Item; // seeds have not

                    if( crop != null && cropsHarvested > 1 )
                    {
                        if( crop.Stackable ) // try to stack right amount for crops
                            crop.Amount = cropsHarvested;
                        else
                            Console.WriteLine( "Warning: crop harvested not stackable: {0}", crop.GetType().Name );
                    }

                    if( seed != null && seedsHarvested > 1 )
                    {
                        if( seed.Stackable ) // try to stack right amount for seeds
                            seed.Amount = seedsHarvested;
                        else
                            Console.WriteLine( "Warning: seed harvested not stackable: {0}", seed.GetType().Name );
                    }

                    if( crop != null ) // if crops are successfully created...
                    {
                        if( harvestToPack ) // put them in right place
                            harvester.AddToBackpack( crop );
                        else
                            crop.MoveToWorld( location, map );
                    }

                    if( seed != null ) // if seeds are successfully created...
                    {
                        harvester.AddToBackpack( seed ); // put them always in harvester pack
                    }
                }
                Timer.DelayCall( TimeSpan.FromSeconds( HarvestDelay ), new TimerStateCallback( ReleaseHarvestLock ),
                                harvester );
            }
            else
            {
                harvester.SendMessage( "Harvesting require patience..." );
            }
        }

        /// <summary>
        /// Event occurring when our plant recover from dormant
        /// </summary>
        public virtual void LeaveDormant()
        {
            if( Deleted )
                return;

            IsGrowing = true;
        }

        /// <summary>
        /// Event occurring when a plant got pesticized.
        /// </summary>
        public virtual void Pesticide()
        {
            if( Deleted || !IsGrowing )
                return;

            if( GotPest )
            {
                CheckFarmerSkillIncreased( EventSkillType.Growing );
                GotPest = false;
            }
            else
            {
                bool tooPesticideShouldLower = Utility.RandomDouble() < PlantHelper.TooPesticideDropChance;
                if( tooPesticideShouldLower && CareLevel > PlantHelper.CareLevelWorst )
                    CareLevel--;
                DebugMessage( m_IsInDebugMode, "I've not pest! Too much pesticide in the air!", this );
            }
        }

        /// <summary>
        /// Event occurring when a plant has to produce the crops.
        /// </summary>
        public virtual void ProduceCrop()
        {
            if( Deleted || !IsFullGrown )
                return;

            Yield++;
        }

        /// <summary>
        /// Event occurring when a plant has to produce a seed.
        /// </summary>
        public virtual void ProduceSeed()
        {
            if( Deleted || !IsFullGrown )
                return;

            Seeds++;
        }

        /// <summary>
        /// Event occurring when our plant goes dormant.
        /// </summary>
        public virtual void StopGrowth()
        {
            if( Deleted || !IsGrowing )
                return;

            IsGrowing = false;
        }

        /// <summary>
        /// Event occurring when a plant got watered.
        /// </summary>
        public virtual void Waterize()
        {
            if( Deleted || !CanGrow )
                return;

            LastWatered = DateTime.Now;
        }

        #endregion

        #region checks

        public static int GetPlantForMobile( Mobile m )
        {
            if( m == null )
                return 0;

            List<BasePlant> list;
            m_Table.TryGetValue( m, out list );

            if( list == null )
                return 0;

            int counter = 0;
            foreach( BasePlant p in list )
            {
                if( !p.Deleted )
                    counter++;
            }

            return counter;
        }

        /// <summary>
        /// Check if from can use plant. NB: Staff members can always use plants
        /// </summary>
        /// <param name="from">mobile to check access</param>
        /// <param name="plant">plant that mobile want to access</param>
        public static bool CheckAccess( Mobile from, BasePlant plant )
        {
            if( from == null || from.Deleted )
                return false;

            if( from.AccessLevel > AccessLevel.Player )
                return true;

            if( !from.CheckAlive() )
                return false;

            if( !Utility.InRange( from.Location, plant.Location, 2 ) )
                return false;

            if( !from.InLOS( plant ) )
                return false;

            if( !plant.IsPublic && plant.Owner != null && from == plant.Owner )
                return true;

            if( plant.IsPublic && plant.Owner != null && plant.Owner.AccessLevel > AccessLevel.Player &&
                from.AccessLevel == AccessLevel.Player )
                return false;

            return plant.IsPublic;
        }

        /// <summary>
        /// Check that gives fungus or pest to our plant
        /// </summary>
        public virtual void CheckGetDisease()
        {
            if( Deleted || !IsGrowing )
                return;

            if( CareLevel < Utility.RandomMinMax( PlantHelper.CareLevelWorst, PlantHelper.CareLevelBest ) && Utility.Random( 10 ) == 0 )
            {
                bool pestOrFungus = Utility.RandomBool();
                if( pestOrFungus )
                {
                    if( !GotFungus )
                        GotFungus = true;
                }
                else
                {
                    if( !GotPest )
                        GotPest = true;
                }
            }
        }

        /// <summary>
        /// Check the current name of the plant.
        /// </summary>
        public virtual void CheckName()
        {
            if( Deleted )
                return;

            if( Name != null && Name != PhaseName[ CurrentPhase ] )
                Name = PhaseName[ CurrentPhase ];
        }

        /// <summary>
        /// Check the ID of the plant.
        /// </summary>
        public virtual void CheckPlantID()
        {
            if( Deleted )
                return;

            if( Yield > 0 && ProductPhaseID > 0 )
                ItemID = ProductPhaseID; // if there are fruits shift itemID to ProductPhaseID
            else if( ItemID != PhaseIDs[ CurrentPhase ] )
                ItemID = PhaseIDs[ CurrentPhase ]; // else shift itemid to current phase ID
        }

        #endregion

        #region general methods
        public enum EventSkillType
        {
            Growing,
            Harvesting
        }

        /// <summary>
        /// Verify if required skill can be raised for plant owner
        /// </summary>
        public virtual void CheckFarmerSkillIncreased( EventSkillType skillType )
        {
            Mobile from = Owner;
            if( from != null && CheckAccess( from, this ) )
            {
                if( skillType == EventSkillType.Growing )
                    from.CheckSkill( SkillNameToCare, MinDiffSkillToCare, GetMaxSkillDifficulty( skillType ) );
                else if( skillType == EventSkillType.Harvesting )
                    from.CheckSkill( RequiredSkillNameToHarvest, MinSkillToHarvest, GetMaxSkillDifficulty( skillType ) );
                else
                    from.CheckSkill( SkillNameToCare, MinDiffSkillToCare, GetMaxSkillDifficulty( skillType ) );
            }
        }

        /// <summary>
        /// Get the skill difficulty for our plant
        /// </summary>
        /// <returns>skill difficulty</returns>
        public virtual double GetMaxSkillDifficulty( EventSkillType skillType )
        {
            if( skillType == EventSkillType.Growing )
                return MinDiffSkillToCare + 50.0;
            else if( skillType == EventSkillType.Harvesting )
                return MinSkillToHarvest + 50.0;
            else
                return MinDiffSkillToCare + 50.0;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( m_Owner != null )
            {
                List<BasePlant> list;
                m_Table.TryGetValue( m_Owner, out list );

                if( list == null )
                    m_Table[ m_Owner ] = list = new List<BasePlant>();

                list.Remove( this );
            }

            m_AllPlants.Remove( this );
        }

        /// <summary>
        /// Give general infoes of plant.
        /// </summary>
        public virtual string GiveGeneralInfo()
        {
            string message = string.Empty;

            if( !Deleted )
            {
                if( !IsGrowing && CurrentPhase == 0 )
                    message = "the seed is not sprouting due to lack of water";
                else if( CurrentPhase == 0 )
                    message = "this plant is really young";
                else if( !IsGrowing )
                    message = "this plant is withering due to lack of proper care.";
                else
                    message = "this plant is growing";
            }

            return message;
        }

        /// <summary>
        /// Give disease infoes of plant.
        /// </summary>
        public virtual List<string> GiveDiseaseInfo()
        {
            List<string> messages = new List<string>();

            if( !Deleted )
            {
                if( HasAnyDesease )
                {
                    if( GotPest )
                        messages.Add( "you saw some aphids sucking the leaves!" );
                    if( GotFungus )
                        messages.Add( "you saw some leaves covered in white powder!" );
                }
                else
                    messages.Add( "there is no kind of desease on this plant." );
            }

            return messages;
        }

        /// <summary>
        /// Release the Lock added on started harvesting
        /// </summary>
        /// <param name="state">object containing the harvester</param>
        private static void ReleaseHarvestLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BasePlant ) );
        }

        /// <summary>
        /// Abstract: Plants must give a constructor for their products returning an Item object.
        /// </summary>
        public abstract Item GetCropObject();

        /// <summary>
        /// Utility that sends message over our plant if it is in debug mode.
        /// </summary>
        /// <param name="isDebugMode">Plant will speak only if <c>IsDebugMode</c> is true</param>
        /// <param name="message">message to deliver. Can be localized (int) or string</param>
        /// <param name="sender">plant speaking</param>	
        public static void DebugMessage( bool isDebugMode, object message, BasePlant sender )
        {
            if( isDebugMode )
            {
                if( message is string )
                    sender.PublicOverheadMessage( MessageType.Regular, 68, false, (string)message );
                else
                    sender.PublicOverheadMessage( MessageType.Regular, 68, (int)message );
            }
        }

        /// <summary>
        /// Double click event: given information, and if harvestable, take some action.
        /// </summary>
        /// <param name="from">clicker</param>
        public override void OnDoubleClick( Mobile from )
        {
            Map map = Map;
            Point3D loc = Location;

            if( Parent != null || Movable || IsLockedDown || IsSecure || map == null || map == Map.Internal )
                return;

            if( !CheckAccess( from, this ) )
            {
                from.SendMessage( "You cannot access that plant." );
                return;
            }

            if( CanProduce )
            {
                if( !IsFullGrown )
                {
                    from.SendMessage( "This plant is too young to harvest." );
                }
                else if( from.Mounted )
                {
                    from.SendMessage( "You cannot harvest a crop while mounted." );
                }
                else
                {
                    if( from.Skills[ RequiredSkillNameToHarvest ].Value < MinSkillToHarvest )
                    {
                        from.SendMessage( string.Format( "You require {0} in {1} to harvest this plant.", MinSkillToHarvest, RequiredSkillNameToHarvest ) );
                        // from.SendMessage( "You have no idea how to harvest this plant." );
                    }
                    else if( !from.InRange( loc, 2 ) || !from.InLOS( this ) )
                    {
                        from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "You are too far away to harvest anything." );
                    }
                    else if( Yield + Seeds < 1 )
                    {
                        from.SendMessage( "There is nothing here to harvest." );
                    }
                    else
                    {
                        DebugMessage( m_IsInDebugMode, "So you would harvest me ?!", this );
                        GotHarvested( from, HarvestInPack );
                    }
                }
            }
        }

        /// <summary>
        /// Single click event: open the context menu.
        /// </summary>
        /// <param name="from">clicker</param>
        /// <param name="list">list of context menu entries</param>
        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            list.Add( new ExaminePlantEntry( this, CheckAccess( from, this ) ) );
            list.Add( new ToggleIsPublicEntry( this, CheckAccess( from, this ) ) );

            if( from.AccessLevel > AccessLevel.Counselor )
            {
                list.Add( new DestroyPlantEntry( this, true ) );
                list.Add( new GMStopGrowthEntry( this, true ) );
                list.Add( new GMWaterizeEntry( this, true ) );
                list.Add( new GMFungicideEntry( this, true ) );
                list.Add( new GMPesticideEntry( this, true ) );
                list.Add( new GMCheckDiseaseEntry( this, true ) );
                list.Add( new GMToggleDebugModeEntry( this, true ) );
                list.Add( new GMFertilizeEntry( this, true ) );
            }
            else
            {
                if( IsDestroyable )
                    list.Add( new DestroyPlantEntry( this, CheckAccess( from, this ) ) );
            }
        }

        #endregion

        #endregion

        #region ContextMenuEntries

        private class ExaminePlantEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;
            private readonly bool m_Enabled;

            #region costruttori

            public ExaminePlantEntry( BasePlant plant, bool enabled )
                : base( 1053 ) // Examine this plant
            {
                m_Plant = plant;

                if( !enabled )
                {
                    Flags |= CMEFlags.Disabled;
                    m_Enabled = false;
                }
                else
                    m_Enabled = true;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                if( !m_Enabled )
                    return;

                Mobile from = Owner.From;

                if( CheckAccess( from, m_Plant ) )
                {
                    from.SendGump( new PlantInfoGump( m_Plant, from ) );
                }
            }

            #endregion
        }

        private class DestroyPlantEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;
            private readonly bool m_Enabled;

            #region costruttori

            public DestroyPlantEntry( BasePlant plant, bool enabled )
                : base( 1044 ) // Uproot this plant
            {
                m_Plant = plant;

                if( !enabled )
                {
                    Flags |= CMEFlags.Disabled;
                    m_Enabled = false;
                }
                else
                    m_Enabled = true;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                if( !m_Enabled )
                    return;

                Mobile from = Owner.From;

                if( CheckAccess( from, m_Plant ) && m_Plant.IsDestroyable )
                {
                    from.SendGump( new WarningGump( 1060635, 30720, "You are going to uproot this pant.<br>" +
                                                                  "That action is irreversible and will cause plat death and removement.<br>" +
                                                                  "Are you sure you want to uproot this plant?",
                                                  0xFFC000, 420, 280, new WarningGumpCallback( ConfirmDestroyCallback ),
                                                  new object[] { m_Plant }, true ) );
                }
            }

            private static void ConfirmDestroyCallback( Mobile from, bool okay, object state )
            {
                object[] states = (object[])state;
                BasePlant plant = (BasePlant)states[ 0 ];

                if( okay )
                {
                    from.SendMessage( "You have decided to proceede" );
                    plant.Die();
                }
            }

            #endregion
        }

        private class ToggleIsPublicEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;
            private readonly bool m_Enabled;

            #region costruttori

            public ToggleIsPublicEntry( BasePlant plant, bool enabled )
                : base( 1045 ) // Toggle public status of this pant
            {
                m_Plant = plant;

                if( !enabled )
                {
                    Flags |= CMEFlags.Disabled;
                    m_Enabled = false;
                }
                else
                    m_Enabled = true;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                if( !m_Enabled )
                    return;

                Mobile from = Owner.From;

                if( CheckAccess( from, m_Plant ) )
                {
                    if( !m_Plant.IsPublic )
                    {
                        from.SendGump( new WarningGump( 1060635, 30720, "You are going to make this plant public.<br>" +
                                                                      "Every one will have access and could harvest or even destroy that plant.<br>" +
                                                                      "Are you sure you want to make this plant public?",
                                                      0xFFC000, 420, 280,
                                                      new WarningGumpCallback( ConfirmMakePublicCallback ),
                                                      new object[] { m_Plant }, true ) );
                    }
                    else
                    {
                        from.SendGump( new WarningGump( 1060635, 30720, "You are going to make this plant private.<br>" +
                                                                      "Only you, the plant owner, will have access and could harvest or even destroy that plant.<br>" +
                                                                      "Are you sure you want to make this plant private?",
                                                      0xFFC000, 420, 280,
                                                      new WarningGumpCallback( ConfirmMakePrivateCallback ),
                                                      new object[] { m_Plant }, true ) );
                    }
                }
            }

            private static void ConfirmMakePublicCallback( Mobile from, bool okay, object state )
            {
                object[] states = (object[])state;
                BasePlant plant = (BasePlant)states[ 0 ];

                if( okay )
                {
                    from.SendMessage( "You have decided to proceede" );
                    plant.IsPublic = true;
                }
            }

            private static void ConfirmMakePrivateCallback( Mobile from, bool okay, object state )
            {
                object[] states = (object[])state;
                BasePlant plant = (BasePlant)states[ 0 ];

                if( okay )
                {
                    from.SendMessage( "You have decided to proceede" );
                    plant.IsPublic = false;
                }
            }

            #endregion
        }

        private class GMStopGrowthEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMStopGrowthEntry( BasePlant plant, bool enabled )
                : base( 1046 ) // GM Stop plant grow
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.StopGrowth();
                from.SendMessage( "You have stopped that plant life timer." );
            }

            #endregion
        }

        private class GMWaterizeEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMWaterizeEntry( BasePlant plant, bool enabled )
                : base( 1047 ) // GM Give water
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.Waterize();
                from.SendMessage( "You have watered that plant.." );
            }

            #endregion
        }

        private class GMFungicideEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMFungicideEntry( BasePlant plant, bool enabled )
                : base( 1048 ) // GM Give fungicide
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.Fungicide();
                from.SendMessage( "You have fungicized that plant." );
            }

            #endregion
        }

        private class GMPesticideEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMPesticideEntry( BasePlant plant, bool enabled )
                : base( 1049 ) // GM Give pesticide
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.Pesticide();
                from.SendMessage( "You have pesticized that plant." );
            }

            #endregion
        }

        private class GMFertilizeEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMFertilizeEntry( BasePlant plant, bool enabled )
                : base( 1054 ) // GM Give fertilizer
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.Fertilize( PlantPotionLevel.Heavy );
                from.SendMessage( "You have fertilized that plant." );
            }

            #endregion
        }

        private class GMCheckDiseaseEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMCheckDiseaseEntry( BasePlant plant, bool enabled )
                : base( 1050 ) // GM Check disease
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.CheckGetDisease();
                from.SendMessage( "You have checked that plant health." );
            }

            #endregion
        }

        private class GMToggleDebugModeEntry : ContextMenuEntry
        {
            private readonly BasePlant m_Plant;

            #region costruttori

            public GMToggleDebugModeEntry( BasePlant plant, bool enabled )
                : base( 1052 ) // GM Toggle debug mode
            {
                m_Plant = plant;

                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            #endregion

            #region metodi

            public override void OnClick()
            {
                if( m_Plant.Deleted )
                    return;

                Mobile from = Owner.From;

                m_Plant.IsInDebugMode = !m_Plant.IsInDebugMode;
                from.SendMessage( "You have {0} debug mode for this plant.",
                                 ( m_Plant.IsInDebugMode ? "enabled" : "disabled" ) );
            }

            #endregion
        }

        #endregion

        #region costruttori

        public BasePlant( int itemID )
            : this( itemID, null )
        {
        }

        public BasePlant( int itemID, Mobile owner )
            : base( itemID )
        {
            m_AllPlants.Add( this );

            m_Owner = owner;
            m_IsPublic = ( m_Owner == null );
            m_CareLevel = PlantHelper.CareLevelOk;

            m_CurrentPhase = 0;
            m_Yield = 0;
            m_Seeds = 0;

            m_IsGrowing = true;
            IsInGarden = false;
            m_GotFungus = false;
            m_GotPest = false;

            GrowCounter = 0;
            HealthCounter = 0;

            InitPlant();

            if( owner != null )
            {
                List<BasePlant> list;
                m_Table.TryGetValue( owner, out list );

                if( list == null )
                    m_Table[ owner ] = list = new List<BasePlant>();

                list.Add( this );
            }

            Movable = false;
        }

        public BasePlant( Serial serial )
            : base( serial )
        {
            m_AllPlants.Add( this );
        }

        #endregion

        #region serial deserial

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 1 ); // version

            writer.Write( m_Seeds );
            writer.Write( m_IsInDebugMode );
            writer.Write( m_IsPublic );
            writer.WriteMobile( m_Owner );

            writer.Write( m_IsGrowing );

            writer.Write( m_UntilIsFertilized );
            writer.Write( m_TimePlanted );
            writer.Write( m_LastWatered );

            writer.Write( m_CurrentPhase );
            writer.Write( m_CareLevel );
            writer.Write( m_GotFungus );
            writer.Write( m_GotPest );

            writer.Write( m_Yield );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    m_Seeds = reader.ReadInt();
                    goto case 0;
                case 0:
                    m_IsInDebugMode = reader.ReadBool();
                    m_IsPublic = reader.ReadBool();
                    m_Owner = reader.ReadMobile();

                    m_IsGrowing = reader.ReadBool();

                    m_UntilIsFertilized = reader.ReadDateTime();
                    m_TimePlanted = reader.ReadDateTime();
                    m_LastWatered = reader.ReadDateTime();

                    m_CurrentPhase = reader.ReadInt();
                    m_CareLevel = reader.ReadInt();
                    m_GotFungus = reader.ReadBool();
                    m_GotPest = reader.ReadBool();

                    m_Yield = reader.ReadInt();
                    break;
            }

            if( ( Map == null || Map == Map.Internal ) && Location == Point3D.Zero )
                Delete();

            if( m_Owner != null )
            {
                List<BasePlant> list;
                m_Table.TryGetValue( m_Owner, out list );

                if( list == null )
                    m_Table[ m_Owner ] = list = new List<BasePlant>();

                list.Add( this );
            }
        }

        #endregion
    }
}