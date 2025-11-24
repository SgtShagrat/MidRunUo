/***************************************************************************
 *                                  Midgard2PlayerMobile.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Midgard;
using Midgard.Engines.Academies;
using Midgard.Engines.AdvancedDisguise;
using Midgard.Engines.Classes;
using Midgard.Engines.HardLabour;
using Midgard.Engines.JailSystem;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.MurderInfo;
using Midgard.Engines.Races;
using Midgard.Engines.SkillSystem;
using Midgard.Engines.SpellSystem;
using Midgard.Engines.WarSystem;
using Midgard.Gumps;
using Midgard.Misc;
using Midgard.Mobiles;

using Server.ContextMenus;
using Server.Engines.BulkOrders;
using Server.Engines.Craft;
using Server.Engines.PartySystem;
using Server.Engines.XmlPoints;
using Server.Engines.XmlSpawner2;
using Server.Guilds;
using Server.Items;
using Server.Network;
using Server.Poker;
using Server.Regions;
using Server.Spells;

using RaceConfig = Midgard.Engines.Races.Config;
using PolSkillHandler = Midgard.Engines.PolRawPoints.MobileSkillHandler;
using Midgard.Engines.StoneEnchantSystem;

namespace Server.Mobiles
{
    public sealed class Midgard2PlayerMobile : PlayerMobile
    {
        #region MidgardPlayerFlags
        [Flags]
        private enum MidgardPlayerFlag
        {
            None = 0x00000000,

            OnlineVisible = 0x00000001,
            IsMidgardSpawner = 0x00000002,
            DisplayCitizenStatus = 0x00000004,
            AcceptPrivateMessages = 0x00000008,
            IsSmothed = 0x00000010,
            DisplayPvpStatus = 0x00000020,
            CrystalHarvesting = 0x00000040,
            DisplayRegionalInfo = 0x00000080,
            AcceptTips = 0x00000100,
            PermaRed = 0x00000200,
            DisplayClassStatus = 0x00000400,
            DisplayAcademyStatus = 0x00000800,
        }

        private bool GetFlag( MidgardPlayerFlag flag )
        {
            return ( ( MidFlags & flag ) != 0 );
        }

        private void SetFlag( MidgardPlayerFlag flag, bool value )
        {
            if( value )
                MidFlags |= flag;
            else
                MidFlags &= ~flag;
        }

        private MidgardPlayerFlag MidFlags { get; set; }
        #endregion

        #region [autostable system]
        /*
        private static readonly Type[] m_AutoStablables = new Type[]
	    {
            typeof(Cat),
            typeof(Dog),
            typeof(Bird),
            typeof(Eagle)
	    };

        public bool SupportsAutoStable( Type type )
        {
            bool contains = false;

            for( int i = 0; !contains && i < m_AutoStablables.Length; ++i )
                contains = ( m_AutoStablables[ i ] == type );

            return contains;
        }
        */

        public const int AutoStableRadius = 5;

        public override void AutoStablePets()
        {
            if( AllFollowers.Count > 0 )
            {
                for( int i = AllFollowers.Count - 1; i >= 0; --i )
                {
                    BaseCreature pet = AllFollowers[ i ] as BaseCreature;

                    if( pet == null || pet.ControlMaster == null || pet.Summoned )
                        continue;

                    if( pet is IMount && ( (IMount)pet ).Rider != null )
                        continue;

                    if( pet.ControlOrder != OrderType.Guard && pet.ControlOrder != OrderType.Follow && pet.ControlOrder != OrderType.Come )
                        continue;

                    if( !pet.InRange( Location, AutoStableRadius ) )
                        continue;

                    if( !pet.CanAutoStable )
                        continue;

                    /* not used anymore
                    if( !SupportsAutoStable( pet.GetType() ) )
                        continue;
                    */

                    pet.ControlTarget = null;
                    pet.ControlOrder = OrderType.Stay;
                    pet.Internalize();

                    pet.SetControlMaster( null );
                    pet.SummonMaster = null;

                    pet.IsStabled = true;

                    pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

                    Stabled.Add( pet );
                    AutoStabled.Add( pet );
                }
            }
        }

        public override void ClaimAutoStabledPets()
        {
            if( AutoStabled.Count <= 0 )
                return;

            if( !Alive )
            {
                SendLocalizedMessage( 1076251 ); // Your pet was unable to join you while you are a ghost.  Please re-login once you have ressurected to claim your pets.				
                return;
            }

            for( int i = AutoStabled.Count - 1; i >= 0; --i )
            {
                BaseCreature pet = AutoStabled[ i ] as BaseCreature;

                if( pet == null )
                    continue;

                if( pet.Deleted )
                {
                    pet.IsStabled = false;

                    if( Stabled.Contains( pet ) )
                        Stabled.Remove( pet );

                    continue;
                }

                if( ( Followers + pet.ControlSlots ) <= FollowersMax )
                {
                    pet.SetControlMaster( this );

                    if( pet.Summoned )
                        pet.SummonMaster = this;

                    pet.ControlTarget = this;
                    pet.ControlOrder = OrderType.Follow;

                    pet.MoveToWorld( Location, Map );

                    pet.IsStabled = false;

                    pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

                    if( Stabled.Contains( pet ) )
                        Stabled.Remove( pet );
                }
                else
                {
                    SendLocalizedMessage( 1049612, pet.Name ); // ~1_NAME~ remained in the stables because you have too many followers.
                }
            }

            AutoStabled.Clear();
        }
        #endregion

        #region [poker system]
        public PokerGame PokerGame { get; set; }
        #endregion

        #region [welcome system]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool AcceptTips
        {
            get { return GetFlag( MidgardPlayerFlag.AcceptTips ); }
            set { SetFlag( MidgardPlayerFlag.AcceptTips, value ); }
        }

        /// <summary>
        /// Sends a scroll message containing the given text to our mobile
        /// </summary>
        /// <param name="message">text in the scroll</param>
        public void SendCustomScrollMessage( string message )
        {
            if( !String.IsNullOrEmpty( message ) )
                Send( new CustomScrollMessage( message ) );
        }
        #endregion

        #region [class system]
        [CommandProperty( AccessLevel.Seer )]
        public Classes Class
        {
            get
            {
                if( ClassState != null && ClassState.ClassSystem != null )
                    return ClassState.ClassSystem.Definition.Class;
                else
                    return Classes.None;
            }
        }

        [CommandProperty( AccessLevel.Seer )]
        public ClassPlayerState ClassState { get; set; }

        public bool DisplayClassStatus
        {
            get { return GetFlag( MidgardPlayerFlag.DisplayClassStatus ); }
            set
            {
                SetFlag( MidgardPlayerFlag.DisplayClassStatus, value );
                Delta( MobileDelta.Properties );
            }
        }
        #endregion

        #region [academy system]
        [CommandProperty( AccessLevel.Seer )]
        public AcademyPlayerState AcademyState { get; set; }

        public bool DisplayAcademyStatus
        {
            get { return GetFlag( MidgardPlayerFlag.DisplayAcademyStatus ); }
            set
            {
                SetFlag( MidgardPlayerFlag.DisplayClassStatus, value );
                Delta( MobileDelta.Properties );
            }
        }
        #endregion

        #region [town system]
        private TownBanAttribute m_TownBans;
        private TownPermaBanAttribute m_TownPermaBans;

        [CommandProperty( AccessLevel.Seer )]
        public TownPlayerState TownState { get; set; }

        public bool DisplayCitizenStatus
        {
            get { return GetFlag( MidgardPlayerFlag.DisplayCitizenStatus ); }
            set
            {
                SetFlag( MidgardPlayerFlag.DisplayCitizenStatus, value );
                Delta( MobileDelta.Properties );
            }
        }

        [CommandProperty( AccessLevel.Seer )]
        public MidgardTowns Town
        {
            get
            {
                if( TownMod != MidgardTowns.None )
                    return TownMod;

                return TownState != null ? TownState.TownSystem.Definition.Town : MidgardTowns.None;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TownBanAttribute TownBans
        {
            get { return m_TownBans; }
            set { }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TownPermaBanAttribute TownPermaBans
        {
            get { return m_TownPermaBans; }
            set { }
        }


        [CommandProperty( AccessLevel.Administrator )]
        public DateTime LastTownLeaving { get; set; }
        #endregion

        #region [hardlabour & jail system]
        private readonly HardLabourAttribute m_HardLabourAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
        public HardLabourAttribute HardLabourAttributes
        {
            get { return m_HardLabourAttributes; }
            set { }
        }

        private int m_Minerals2Mine;

        public int Minerals2Mine
        {
            get { return m_Minerals2Mine; }
            set
            {
                int oldValue = m_Minerals2Mine;
                if( oldValue != value )
                {
                    m_Minerals2Mine = value;

                    OnMinerals2MineChanged( oldValue );
                }
            }
        }

        public string HardLabourInfo { get; set; }

        public string HardLabourCondamner { get; set; }

        public void OnMinerals2MineChanged( int oldValue )
        {
            HardLabourCommands.LogMinerals2MineChanged( this, oldValue, Minerals2Mine );
        }
        #endregion

        #region [quest system]
        private readonly QuestAttributes m_QuestAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
        public QuestAttributes QuestAttributes
        {
            get { return m_QuestAttributes; }
            set { }
        }

        private DateTime m_QuestDeltaTimeExpiration;

        public int QuestVariable1 { get; set; }
        public int QuestVariable2 { get; set; }
        public int QuestVariable3 { get; set; }
        public int QuestVariable4 { get; set; }
        public int QuestVariable5 { get; set; }

        public string QuestString { get; set; }

        public TimeSpan QuestDeltaTimeExpiration
        {
            get
            {
                TimeSpan ts = m_QuestDeltaTimeExpiration - DateTime.Now;

                if( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set { m_QuestDeltaTimeExpiration = DateTime.Now + value; }
        }
        #endregion

        #region [possess command]
        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile PossessMob { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public Mobile PossessStorageMob { get; set; }
        #endregion

        #region [taming system]
        private DateTime m_NextTamingBulkOrder;

        public TamingBOBFilter TamingBOBFilter { get; private set; }

        public TimeSpan NextTamingBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextTamingBulkOrder - DateTime.Now;

                if( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try
                {
                    m_NextTamingBulkOrder = DateTime.Now + value;
                }
                catch
                {
                }
            }
        }

        public void ShrinkAllPets()
        {
            if( AllFollowers.Count <= 0 )
                return;

            for( int i = AllFollowers.Count - 1; i >= 0; --i )
            {
                BaseCreature pet = AllFollowers[ i ] as BaseCreature;
                if( pet == null || pet.ControlMaster == null || pet.Summoned )
                    continue;

                if( pet is IMount && ( (IMount)pet ).Rider != null )
                    continue;

                if( pet is BaseEscortable )
                    continue;

                pet.Shrink();
            }
        }
        #endregion

        #region [contact-info system]
        [CommandProperty( AccessLevel.GameMaster )]
        public PersonalInfo Info { get; set; }
        #endregion

        #region [sphynkx related]
        private DateTime m_SphynxBonusExpiration;

        public FortuneTypes FortuneType { get; set; }

        public int FortuneValue { get; set; }

        public TimeSpan SphynxBonusExpiration
        {
            get
            {
                TimeSpan ts = m_SphynxBonusExpiration - DateTime.Now;

                if( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                if( ts == TimeSpan.Zero )
                {
                    FortuneType = FortuneTypes.None;
                    FortuneValue = 0;
                }

                return ts;
            }
            set { m_SphynxBonusExpiration = DateTime.Now + value; }
        }
        #endregion

        #region [guilds]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsGuildAdept
        {
            get { return Guild != null && SkillsTotal < 400.0 && RawStatTotal < 175; }
        }

        //edit by arlas
        private bool m_TownEnemy;

        [CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
        public bool TownEnemy
        {
            get
            {
                return m_TownEnemy;
            }
            set
            {
                if( m_TownEnemy != value )
                {
                    m_TownEnemy = value;
                    Delta( MobileDelta.Noto );
                    InvalidateProperties();
                }

                if( m_TownEnemy )
                {
                    if( m_ExpireTownEnemy == null )
                        m_ExpireTownEnemy = new ExpireTownEnemyTimer( this );
                    else
                        m_ExpireTownEnemy.Stop();

                    m_ExpireTownEnemy.Start();
                }
                else if( m_ExpireTownEnemy != null )
                {
                    m_ExpireTownEnemy.Stop();
                    m_ExpireTownEnemy = null;
                }
            }
        }

        private Timer m_ExpireTownEnemy;

        private static TimeSpan m_ExpireTownEnemyDelay = TimeSpan.FromMinutes( 5.0 );

        public static TimeSpan ExpireTownEnemyDelay
        {
            get { return m_ExpireTownEnemyDelay; }
            set { m_ExpireTownEnemyDelay = value; }
        }

        private class ExpireTownEnemyTimer : Timer
        {
            private readonly Midgard2PlayerMobile m_Mobile;

            public ExpireTownEnemyTimer( Midgard2PlayerMobile m )
                : base( m_ExpireTownEnemyDelay )
            {
                Priority = TimerPriority.FiveSeconds;
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.TownEnemy = false;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastGuildLeaving { get; set; }
        #endregion

        #region [camuflage]
        private readonly CamuflageAttributes m_CamuflageAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
        public CamuflageAttributes CamuflageAttributes
        {
            get { return m_CamuflageAttributes; }
            set { }
        }

        private int m_KarmaMod = -1;
        private int m_FameMod = -1;
        private bool m_FemaleMod;

        public MidgardTowns TownMod { get; set; }

        public new int Karma
        {
            get { return m_KarmaMod != -1 ? m_KarmaMod : base.Karma; }
            set { base.Karma = value; }
        }

        public int KarmaMod
        {
            get { return m_KarmaMod; }
            set
            {
                if( m_KarmaMod != value )
                    m_KarmaMod = value;
            }
        }

        public new int Fame
        {
            get { return m_FameMod != -1 ? m_FameMod : base.Fame; }
            set { base.Fame = value; }
        }

        public int FameMod
        {
            get { return m_FameMod; }
            set
            {
                if( m_FameMod != value )
                    m_FameMod = value;
            }
        }

        public Mobile Alias { get; set; }

        public bool FemaleMod
        {
            get { return m_FemaleMod; }
            set
            {
                if( m_FemaleMod != value )
                    m_FemaleMod = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public new bool Female
        {
            get { return m_FemaleMod ? m_FemaleMod : base.Female; }
            set { base.Female = value; }
        }
        #endregion

        #region [bulk orders]
        public int LastSmithBulkOrderValue { get; set; }

        public int LastTailorBulkOrderValue { get; set; }

        public new TimeSpan NextSmithBulkOrder
        {
            get { return AccessLevel > AccessLevel.Seer ? TimeSpan.Zero : base.NextSmithBulkOrder; }
            set
            {
                try
                {
                    base.NextSmithBulkOrder = value;
                }
                catch
                {
                }
            }
        }

        public new TimeSpan NextTailorBulkOrder
        {
            get { return AccessLevel > AccessLevel.Seer ? TimeSpan.Zero : base.NextTailorBulkOrder; }
            set
            {
                try
                {
                    base.NextTailorBulkOrder = value;
                }
                catch
                {
                }
            }
        }
        #endregion

        #region [race system]
        private RaceLanguageAttribute m_RaceLanguages;

        [CommandProperty( AccessLevel.GameMaster )]
        public RaceLanguageAttribute RaceLanguages
        {
            get { return m_RaceLanguages; }
            set { }
        }

        public override double RacialSkillBonus
        {
            get { return !RaceConfig.SkillBonusesEnabled ? 0 : base.RacialSkillBonus; }
        }

        public void AcquireLanguage( Race race )
        {
            m_RaceLanguages.AcquireLanguage( race );
        }

        public bool KnowsLanguage( Race race )
        {
            return race == Race || m_RaceLanguages.KnowsLanguage( race );
        }
        #endregion

        #region [bounty system]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool ShowBountyUpdate { get; set; }

        public List<string> BountyUpdateList { get; set; }
        #endregion

        #region [player debug]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool Debug { get; set; }

        public override bool PlayerDebug { get { return Debug; } }
        #endregion

        #region [private messages]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool AcceptPrivateMessages
        {
            get { return GetFlag( MidgardPlayerFlag.AcceptPrivateMessages ); }
            set { SetFlag( MidgardPlayerFlag.AcceptPrivateMessages, value ); }
        }

        public List<Mobile> PlayerMessageIgnoreList { get; private set; }
        #endregion

        #region [skills]
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastSnoopAction { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanSnoop
        {
            get
            {
                int delay = (int)( 11 - ( Skills[ SkillName.Snooping ].Value ) / 10 );
                if( delay < 1 )
                    delay = 1;

                return DateTime.Now > LastSnoopAction + TimeSpan.FromSeconds( delay );
            }
        }
        #endregion

        #region [second age armor system]
        public override Item LegsArmor
        {
            get
            {
                double best = 0.0;
                Item ar = null;

                foreach( Layer layer in m_LegsValidLayers )
                {
                    double temp = GetArmorValueOnLayer( layer );
                    if( temp > best )
                    {
                        best = temp;
                        ar = FindItemOnLayer( layer );
                    }
                }

                return ar;
            }
        }

        public override Item ChestArmor
        {
            get
            {
                double best = 0.0;
                Item ar = null;

                foreach( Layer layer in m_ChestValidLayers )
                {
                    double temp = GetArmorValueOnLayer( layer );
                    if( temp > best )
                    {
                        best = temp;
                        ar = FindItemOnLayer( layer );
                    }
                }

                return ar;
            }
        }

        public bool AbsorbDamage( Mobile attacker, ref int damage )
        {
            bool shouldAbsorb = false;

            if( Morph.UnderTransformation( this ) )
            {
                Morph.MorphContext context = Morph.GetContext( this );
                if( context != null && context.Entry.VirtualArmorMod != -1 )
                    shouldAbsorb = true;
            }
            else if( TransformationSpellHelper.UnderTransformation( this, typeof( ReaperFormSpell ) ) )
                shouldAbsorb = true;

            if( !shouldAbsorb )
                return false;

            int initialDamage = damage;

            double halfAr = ArmorRating / 200.0;
            int absorbed = (int)( damage * ( halfAr + ( halfAr * Utility.RandomDouble() ) ) );

            damage -= absorbed;

            if( attacker.PlayerDebug )
                attacker.SendMessage( "Your enemy absorbed {0} damage (was {1}) with its {2} armor. Final damage is {3}", absorbed, initialDamage, ArmorRating, damage );

            if( damage < 1 )
                damage = 1;

            return true;
        }

        public override double ArmorRating
        {
            get
            {
                if( Core.AOS )
                    return base.ArmorRating;

                if( Morph.UnderTransformation( this ) )
                {
                    Morph.MorphContext context = Morph.GetContext( this );
                    if( context != null && context.Entry.VirtualArmorMod != -1 )
                        return context.Entry.VirtualArmorMod + context.Entry.GetArmorBonusByPack( this );
                }

                TransformContext transformContext = TransformationSpellHelper.GetContext( this );
                if( transformContext != null && transformContext.Spell is ReaperFormSpell )
                    return 35;

                double rating = 0.0;

                AddArmorRating( ref rating, NeckArmor );
                AddArmorRating( ref rating, HandArmor );
                AddArmorRating( ref rating, HeadArmor );
                AddArmorRating( ref rating, ArmsArmor );
                rating += BestLegsArmorOrClothing; // legs and chest must be treated carefully...
                rating += BestChestArmorOrClothing;
                AddArmorRating( ref rating, ShieldArmor );

                return VirtualArmor + VirtualArmorMod + rating;
            }
        }

        private static void AddArmorRating( ref double rating, Item armor )
        {
            BaseArmor ar = armor as BaseArmor;

            if( ar != null )
                rating += ar.ArmorRatingScaled;
            else
            {
                BaseClothing cl = armor as BaseClothing;

                if( cl != null )
                    rating += cl.ArmorRatingScaled;
            }
        }

        public double GetArmorValueOnLayer( Layer layer )
        {
            Item ar = FindItemOnLayer( layer );

            if( ar != null )
            {
                if( ar is BaseArmor )
                    return ( (BaseArmor)ar ).ArmorRatingScaled;
                else if( ar is BaseClothing )
                    return ( (BaseClothing)ar ).ArmorRatingScaled;
            }

            return 0.0;
        }

        private static readonly Layer[] m_LegsValidLayers = new Layer[]
        {
            Layer.InnerLegs, Layer.Pants, Layer.OuterLegs, Layer.Shoes
        };

        public double BestLegsArmorOrClothing
        {
            get
            {
                double best = 0.0;
                foreach( Layer layer in m_LegsValidLayers )
                {
                    double temp = GetArmorValueOnLayer( layer );
                    if( temp > best )
                        best = temp;
                }

                return best;
            }
        }

        private static readonly Layer[] m_ChestValidLayers = new Layer[]
        {
            Layer.InnerTorso, Layer.Shirt, Layer.MiddleTorso, Layer.OuterTorso, Layer.Waist, Layer.Cloak
        };

        public double BestChestArmorOrClothing
        {
            get
            {
                double best = 0.0;
                foreach( Layer layer in m_ChestValidLayers )
                {
                    double temp = GetArmorValueOnLayer( layer );
                    if( temp > best )
                        best = temp;
                }

                return best;
            }
        }
        #endregion

        #region [sounds]
        private static readonly int[] m_MaleHurtSounds = new int[]
                                                {
                                                    340, 341, 342, 344
                                                };

        private static readonly int[] m_FemaleHurtSounds = new int[]
                                                  {
                                                      331, 332, 333, 334, 335
                                                  };

        public override int GetHurtSound()
        {
            return Female ? Utility.RandomList( m_FemaleHurtSounds ) : Utility.RandomList( m_MaleHurtSounds );
        }
        #endregion

        #region [spells]
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastFlameStrike { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int FlameStrikeAddiction { get; set; }
        #endregion

        #region [healing pots]
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastHealPotion { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HealingAddiction { get; set; }
        #endregion

        #region [hunger system]
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastFood { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastBeverage { get; set; }
        #endregion

        #region [notoriety system]
        private readonly NotorietyAttributes m_NotorietyAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
        public NotorietyAttributes NotorietyAttributes
        {
            get { return m_NotorietyAttributes; }
            set { }
        }

        public List<MurderInfo> MurderInfoes
        {
            get { return MurderInfoPersistance.GetMurderInfoForKiller( this ); }
        }

        public int LifeTimeKills { get; set; }

        public bool TempPermaRed { get; set; }

        public bool PermaRed
        {
            get { return GetFlag( MidgardPlayerFlag.PermaRed ) || TempPermaRed; }
            set
            {
                SetFlag( MidgardPlayerFlag.PermaRed, value );
                Delta( MobileDelta.Noto );
            }
        }
        #endregion

        #region [macrocheck]
        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastMacroChecked { get; set; }

        private const double MacroCheckMinimumDelay = 30.0;

        /// <summary>
        /// Sends a macrocheck to our mobile
        /// </summary>
        public void AutoMacroCheck()
        {
            if( DateTime.Now - LastMacroChecked <= TimeSpan.FromMinutes( MacroCheckMinimumDelay ) )
                return;

            if( Utility.RandomDouble() < 0.01 && !HasGump( typeof( UnattendedMacroGump ) ) )
            {
                SendGump( new UnattendedMacroGump( null, this ) );
                LastMacroChecked = DateTime.Now;
            }
        }
        #endregion

        #region [custom resistance system]
        private readonly CustomResistAttributes m_CustResistAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
        public CustomResistAttributes CustResistAttributes
        {
            get { return m_CustResistAttributes; }
            set { }
        }

        public override int CustElectricResistance
        {
            get { return GetStonePower( StoneAttributeFlag.Electrical ); }
        }

        public override int CustFireResistance
        {
            get { return GetStonePower( StoneAttributeFlag.Phoenix ); }
        }

        public override int CustGeneralResistance
        {
            get { return GetStonePower( StoneAttributeFlag.Antimagical ); }
        }

        public override int CustImpactResistance
        {
            get { return GetStonePower( StoneAttributeFlag.Mammoth ); }
        }

        public override int CustMentalResistance
        {
            get { return GetStonePower( StoneAttributeFlag.Lockmind ); }
        }

        public override int CustVenomResistance
        {
            get { return GetStonePower( StoneAttributeFlag.Serpent ); }
        }

        public override int GetCustomResistance( CustomResType type )
        {
            int fireOffset = 0;
            if( type == CustomResType.Fire && TransformationSpellHelper.UnderTransformation( this, typeof( ReaperFormSpell ) ) )
                fireOffset = -25;

            switch( type )
            {
                case CustomResType.Electric: return CustElectricResistance;
                case CustomResType.Fire: return CustFireResistance + fireOffset;
                case CustomResType.General: return CustGeneralResistance;
                case CustomResType.Impact: return CustImpactResistance;
                case CustomResType.Mental: return CustMentalResistance;
                case CustomResType.Venom: return CustVenomResistance;
            }

            return 0;
        }
        #endregion

        #region [stone enchant system]
        private readonly StoneAttributes m_PlayerStoneAttributes;

        [CommandProperty( AccessLevel.GameMaster )]
        public StoneAttributes PlayerStoneAttributes
        {
            get { return m_PlayerStoneAttributes; }
            set { }
        }

        public override void UpdateStonePowers()
        {
            m_PlayerStoneAttributes.InvalidateAll();
        }

        public override void UpdateStonePower( int bitmask )
        {
            m_PlayerStoneAttributes.Invalidate( bitmask );
        }

        public override int GetStonePower( int flag )
        {
            return GetStonePower( (StoneAttributeFlag)flag );
        }

        public int GetStonePower( StoneAttributeFlag flag )
        {
            return m_PlayerStoneAttributes != null ? m_PlayerStoneAttributes[ flag ] : 0;
        }
        #endregion

        #region [staff flags]
        private readonly StaffAttributes m_StaffAttributes;

        [CommandProperty( AccessLevel.Administrator )]
        public StaffAttributes StaffAttributes
        {
            get { return m_StaffAttributes; }
            set { }
        }

        public bool OnlineVisible
        {
            get { return GetFlag( MidgardPlayerFlag.OnlineVisible ); }
            set { SetFlag( MidgardPlayerFlag.OnlineVisible, value ); }
        }

        public bool IsMidgardSpawner
        {
            get { return GetFlag( MidgardPlayerFlag.IsMidgardSpawner ); }
            set
            {
                SetFlag( MidgardPlayerFlag.IsMidgardSpawner, value );
                if( NetState != null )
                    NetState.Dispose();
            }
        }
        #endregion

        #region [misc flags]
        private readonly PlayerFlagsAttributes m_PlayerFlagsAttributes;

        [CommandProperty( AccessLevel.Administrator )]
        public PlayerFlagsAttributes PlayerFlagsAttributes
        {
            get { return m_PlayerFlagsAttributes; }
            set { }
        }

        public bool IsSmothed
        {
            get { return GetFlag( MidgardPlayerFlag.IsSmothed ); }
            set { SetFlag( MidgardPlayerFlag.IsSmothed, value ); }
        }

        public bool DisplayPvpStatus
        {
            get { return GetFlag( MidgardPlayerFlag.DisplayPvpStatus ); }
            set
            {
                SetFlag( MidgardPlayerFlag.DisplayPvpStatus, value );
                InvalidateProperties();
                Delta( MobileDelta.Properties );
            }
        }

        public bool CrystalHarvesting
        {
            get { return GetFlag( MidgardPlayerFlag.CrystalHarvesting ); }
            set { SetFlag( MidgardPlayerFlag.CrystalHarvesting, value ); }
        }

        public bool DisplayRegionalInfo
        {
            get { return GetFlag( MidgardPlayerFlag.DisplayRegionalInfo ); }
            set
            {
                SetFlag( MidgardPlayerFlag.DisplayRegionalInfo, value );
                InvalidateProperties();
            }
        }
        #endregion

        #region [other]
        [CommandProperty( AccessLevel.GameMaster )]
        public override LanguageType TrueLanguage { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public int ToMTotalMonsterFame { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public SkillName CustomJobTitle { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftMarkOption CraftMark { get; set; }

        private string DeathMessage
        {
            get
            {
                string format = m_DeathMessages[ Utility.Random( m_DeathMessages.Length ) ];

                string name = String.IsNullOrEmpty( Name ) ? String.Empty : Name;

                if( name == String.Empty )
                    name = Female ? "She" : "He";

                return String.Format( format, name );
            }
        }

        #region m_DeathMessages
        private static readonly string[] m_DeathMessages = new string[]
                                                    {
                                                        "{0} has shuffled off this mortal coil.",
                                                        "Alas, poor {0}, we hardly knew ye.",
                                                        "{0} has ceased to be.",
                                                        "{0}, with a groan, flees to the shades Below.",
                                                        "{0} has expired.",
                                                        "The sun has set upon {0}.",
                                                        "The world shall trouble {0} no longer.",
                                                        "{0}, is food for Wyrms.",
                                                        "{0}, has gone to a better place.",
                                                        "Bereft of life, {0} rests in peace.",
                                                        "{0} has joined the choir invisible.",
                                                        "{0} has fallen.",
                                                        "No more shall {0} be burdened with life.",
                                                        "And unto ashes has {0} returned.",
                                                        "{0} belongs now but to history.",
                                                        "{0} has lived.",
                                                        "The pipes have called {0} home.",
                                                        "Hail and Farewell, {0}!",
                                                        "{0} sure didn't last long.",
                                                        "{0} needs to brush up on those survival skills.",
                                                        "{0} should find a new profession.",
                                                        "Don't give up your day job, {0}.",
                                                        "{0} has found new meaning to the word Futility.",
                                                        "Everyone point at {0} and laugh.",
                                                        "{0} can't do anything well."
                                                    };

        #endregion

        [CommandProperty( AccessLevel.GameMaster )]
        public WarTeam WarTeam { get; set; }
        #endregion

        #region [PVMTankSystem: by Magius(CHE)]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool NotificationPVMTankEnabled { get; set; }
        #endregion

        #region mod by Arlas [armi doppie]
        [CommandProperty( AccessLevel.GameMaster )]
        public bool UseDoubleWeaponsTogether { get; set; }
        #endregion

        #region mod by Arlas [fix troppi lisci]
        [CommandProperty( AccessLevel.GameMaster )]
        public int Misses { get; set; }
        #endregion

        #region [PolSkillRawPointsSystem: by Magius(CHE)]
        public bool ForceSkillRawGain( SkillName skillToGain, uint rawPoints )
        {
            if( PolSkillRawPointsEnabled )
            {
                if( m_PolRawPoints != null )
                    m_PolRawPoints.GainRaw( skillToGain, rawPoints );

                return true;
            }

            return false;
        }

        private PolSkillHandler m_PolRawPoints;

        /// <summary>
        ///     If PolSkillRawPointsEnabled gain skill and returns true
        /// </summary>
        public bool PolSkillRawGain( SkillName skill, object amObj, double lastChanceCause, bool lastSuccess )
        {
            if( PolSkillRawPointsEnabled )
            {
                if( m_PolRawPoints != null )
                    m_PolRawPoints.SkillRawGain( skill, amObj, lastChanceCause, lastSuccess );

                //skill gain performed by MobileSkillHandler so do not continue with standard method.
                return true;
            }

            //continue using standard method
            return false;
        }

        /// <summary>
        /// This method ENSUREs  m_PolRawPoints and ALWAYS return TRUE!
        /// </summary>
        public bool PolSkillRawPointsEnabled
        {
            get
            {
                if( m_PolRawPoints == null )
                    m_PolRawPoints = new PolSkillHandler( this ); // Initialize MobileSkillHandler here!
                return ( m_PolRawPoints != null );
            }
        }

        /*
        [CommandProperty( AccessLevel.GameMaster )]
        public bool PolSkillRawPointsEnabled
        {
            get { return m_PolRawPoints != null; }
            set
            {
                if( PolSkillRawPointsEnabled != value )
                {
                    if( value )
                        m_PolRawPoints = new PolSkillHandler( this ); // Initialize MobileSkillHandler here!
                    else
                    {
                        m_PolRawPoints.Dispose(); // erase all skillrawvalues.
                        m_PolRawPoints = null;
                    }
                }
            }
        }
        */

        [CommandProperty( AccessLevel.GameMaster )]
        public bool NotificationPolRawPointsEnabled { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsOrder
        {
            get { return Guild != null && Guild.Type == GuildType.Order || ( TownState != null && TownState.TownSystem != null && TownState.TownSystem.Type == VirtueType.Order ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsChaos
        {
            get { return Guild != null && Guild.Type == GuildType.Chaos || ( TownState != null && TownState.TownSystem != null && TownState.TownSystem.Type == VirtueType.Chaos ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastHideDetectionTime { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile LastHideDetector { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int NotoTeam { get; set; }

        public uint RawSkill( SkillName skillname )
        {
            if( !PolSkillRawPointsEnabled )
                return 0;

            return m_PolRawPoints != null ? m_PolRawPoints.RawOf( skillname ) : 0;
        }
        #endregion

        public Midgard2PlayerMobile()
        {
            MidFlags = MidgardPlayerFlag.None;

            BountyUpdateList = new List<string>();
            TownMod = MidgardTowns.None;
            DisplayCitizenStatus = true;
            OnlineVisible = true;

            PermaRed = false;
            LifeTimeKills = 0;

            m_Minerals2Mine = 0;
            HardLabourInfo = string.Empty;
            HardLabourCondamner = string.Empty;

            TrueLanguage = LanguageType.Ita;
            CraftMark = CraftMarkOption.PromptForMark;

            Hunger = 20;
            Thirst = 20;
            LastFood = DateTime.MinValue;
            LastBeverage = DateTime.MinValue;

            QuestVariable1 = 0;
            QuestVariable2 = 0;
            QuestVariable3 = 0;
            QuestVariable4 = 0;
            QuestVariable5 = 0;
            QuestString = string.Empty;

            AcceptPrivateMessages = true;
            PlayerMessageIgnoreList = new List<Mobile>();

            TamingBOBFilter = new TamingBOBFilter();

            Info = new PersonalInfo( this );
            m_TownBans = new TownBanAttribute( this );
            m_TownPermaBans = new TownPermaBanAttribute( this );
            m_RaceLanguages = new RaceLanguageAttribute( this );
            m_PlayerStoneAttributes = new StoneAttributes( this );
            m_QuestAttributes = new QuestAttributes( this );
            m_HardLabourAttributes = new HardLabourAttribute( this );
            m_NotorietyAttributes = new NotorietyAttributes( this );
            m_CustResistAttributes = new CustomResistAttributes( this );
            m_CamuflageAttributes = new CamuflageAttributes( this );
            m_StaffAttributes = new StaffAttributes( this );
            m_PlayerFlagsAttributes = new PlayerFlagsAttributes( this );
        }

        #region overriding methods

        public override string ApplyNameSuffix( string suffix )
        {
            if( IsGuildAdept )
                suffix = suffix.Length == 0 ? "[Adept]" : String.Concat( suffix, " [Adept]" );

            if( RessSystem.IsImmune( this ) )
                suffix = suffix.Length == 0 ? "[Ress Immune]" : String.Concat( suffix, " [Ress Immune]" );

            if( ClassState != null && ClassState.ShowsTitle && DisplayClassStatus )
            {
                string className = String.Format( "[{0}]", ClassState.ClassSystem.Definition.ClassName );
                suffix = suffix.Length == 0 ? className : String.Concat( suffix, " " + className );
            }

            return base.ApplyNameSuffix( suffix );
        }

        /// <summary>
        /// Overridable. Metodo invocato quando un item deve essere visto dal client del pg.
        /// <returns>Ritorna false se non puo' vederlo.</returns>
        /// </summary>
        public override bool CanSee( Item item )
        {
            return item is XmlSpawner ? IsMidgardSpawner : base.CanSee( item );
        }

        /// <summary>
        /// Overridable. Metodo invocato quando un mobile deve essere visto dal client del pg.
        /// <returns>Ritorna false se non puo' vederlo.</returns>
        /// </summary>
        public override bool CanSee( Mobile m )
        {
            if( AccessLevel == AccessLevel.Administrator && m.AccessLevel == AccessLevel.Developer )
                return true;

            /*
            else if( AccessLevel == AccessLevel.Player && m is Midgard2PlayerMobile && ( ( (Midgard2PlayerMobile)m ).IsSmothed ) )
                return false;
            */

            // members of same party cen see even if hidden
            if( Party != null && Party is Party )
            {
                if( ( (Party)Party ).Contains( m ) )
                    return true;
            }

            return base.CanSee( m );
        }

        public override bool CheckContextMenuDisplay( IEntity target )
        {
            return AccessLevel > AccessLevel.Player;
        }

        public override void ComputeBaseLightLevels( out int global, out int personal )
        {
            base.ComputeBaseLightLevels( out global, out personal );

            if( ClassSystem.IsPaladine( this ) )
            {
                BaseWeapon weapon = FindItemOnLayer( Layer.OneHanded ) as BaseWeapon;
                if( weapon == null || !weapon.Movable )
                    weapon = FindItemOnLayer( Layer.TwoHanded ) as BaseWeapon;

                // paladins with a holy weapon in hand can see through night.
                if( LightLevel < 21 && ( weapon != null && weapon.IsXmlHolyWeapon ) )
                    personal = 21;
            }
        }

        /// <summary>
        /// Overridable. Metodo invocato quando un mobile riceve un oggetto draggato
        /// <returns>Ritorna true se permette il drag dentro target</returns>
        /// </summary>
        public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
        {
            if( from != this && from.AccessLevel == AccessLevel.Player && ClassSystem.IsThief( from ) )
                return ThiefSystem.HandleNonLocalDrop( from, item, target );

            return base.CheckNonlocalDrop( from, item, target );
        }

        public override int GetResistance( ResistanceType type )
        {
            if( Race != Race.Human && Race is MidgardRace && RaceConfig.RaceResistancesBonusEnabled )
                return base.GetResistance( type ) + ( (MidgardRace)Race ).GetResistanceBonus( type );

            return base.GetResistance( type );
        }

        /// <summary>
        /// Overridable. Metodo invocato da DoHarmful quando un player commette un atto criminale.
        /// <returns>Ritorna true se questo player ha commesso un'azione criminale.</returns>
        /// </summary>
        public override bool IsHarmfulCriminal( Mobile target )
        {
            if( target == this )
                return false; // azioni su se stessi non sono mai azioni criminali

            if( Midgard.Engines.BountySystem.Core.Attackable( this, target ) ) // coloro che hanno una taglia sono sempre attaccabili
                return false;

            if( target is BaseCreature && BaseCreature.IsReporter( (BaseCreature)target ) )
                return true; // Attaccare una guardia o un innocente in  citta e' SEMPRE un'azione criminale

            if( target is PlayerMobile && target.Guild != null && Guild != null && target.Guild == Guild )
                return false; // per permettere ai co-gildati di allenarsi

            if( Midgard.Engines.MidgardTownSystem.Notoriety.AreCoCitizens( this, target ) )
            {
                if( Town == MidgardTowns.BuccaneersDen && !target.Criminal )
                    return false; // Se sono cittadini di bucca e non sono criminali allora l'azione NON e' criminale

                if( Notoriety.Compute( this, target ) == Notoriety.Ally && !TownSystem.IsInAnyTownArena( target ) )
                    return true; // Se sono concittadini e Alleati (indi non criminali) allora l'azione e' sicuramente criminale
            }

            return base.IsHarmfulCriminal( target );
        }

        /// <summary>
        /// Overridable. Metodo invocato da DoBeneficial quando un player commette un atto benefico.
        /// <returns>Ritorna true se questo layer ha commesso un'azione criminale.</returns>
        /// </summary>
        public override bool IsBeneficialCriminal( Mobile target )
        {
            // Azioni benefiche rendono il pg in città Townenemy per 5 minuti
            if( this != target && target is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)target ).TownEnemy )
            {
                Guild sourceGuild = Guild as Guild;
                Guild targetGuild = target.Guild as Guild;

                // townenemy per attacchi tra cittadini
                if( sourceGuild != null && targetGuild != null )
                {
                    SendAdvMessage( "Your guild enemy can aatack you now!",
                                    "I tuoi nemici di gilda ora possono attaccarti!" );
                    TownEnemy = true;
                }
            }

            if( this != target && Midgard.Engines.MidgardTownSystem.Notoriety.AreCoCitizens( this, target ) && Town == MidgardTowns.BuccaneersDen && !target.Criminal )
                return false; // Se sono cittadini di bucca e non sono criminali allora l'azione NON e' criminale

            return base.IsBeneficialCriminal( target );
        }

        /// <summary>
        /// Overridable. Check per vedere se il nostro m2pm può commettere un'azione cirminale su target
        /// </summary>
        public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
        {
            if( target is PlayerMobile && TownSystem.CheckArenaBounds( this, target ) )
            {
                if( message )
                    SendMessage( 37, "You cannot perform your harmful action on that player." );

                return false;
            }

            return base.CanBeHarmful( target, message, ignoreOurBlessedness );
        }

        /// <summary>
        /// Overridable. Check per vedere se il nostro m2pm può commettere un'azione benefica su target
        /// </summary>
        public override bool CanBeBeneficial( Mobile target, bool message )
        {

            if( ( target is PlayerMobile && TownSystem.CheckArenaBounds( this, target ) ) || ( target == this && Hidden ) )
            {
                if( message )
                    SendLocalizedMessage( 1001017 ); // You can not perform beneficial acts on your target.

                return false;
            }

            return base.CanBeBeneficial( target, message );
        }

        /// <summary>
        /// Overridable. Evento invocato quando un M2pm fa un azione benefica
        /// </summary>
        public override void OnBeneficialAction( Mobile target, bool isCriminal )
        {
            if( target != this && target.Guild != null )
            {
                bool enemyAction = false;
                foreach( Guild enemy in ( (Guild)target.Guild ).Enemies )
                {
                    enemy.AddTempEnemy( this );
                    enemyAction = true;
                }

                if( enemyAction )
                    SendMessage( 37, "For two minutes you are involved in a guild war!" );
            }

            // Se un cittadino non gildato cura un target che e' m2pm ed e' gildato...
            if( target != this && Town != MidgardTowns.None && ( Guild == null || IsGuildAdept ) )
            {
                if( target.Guild != null )
                {
                    SendMessage( 37, "You are a now involved in a guild war!" );
                    if( TownState != null )
                        TownState.CitizenCriminal = true;
                }
                else if( target is BaseCreature )
                {
                    BaseCreature c = target as BaseCreature;

                    if( c.Controlled && c.ControlMaster != null )
                    {
                        if( c.ControlMaster.Guild != null )
                        {
                            SendMessage( 37, "You are a now involved in a guild war!" );
                            if( TownState != null )
                                TownState.CitizenCriminal = true;
                        }
                    }
                }
            }

            if( RessSystem.IsImmune( this ) )
                RessSystem.EndRessImmune( this );

            base.OnBeneficialAction( target, isCriminal );
        }

        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            if( Alive && AccessLevel == AccessLevel.Player )
                RevealingAction( true );

            BandageContext context = BandageContext.GetContext( this );
            if( context != null && !context.HealerDamaged )
            {
                SendMessage( "You have been damaged during heal process..." );
                context.HealerDamaged = true;
            }

            base.OnDamage( amount, from, willKill );
        }

        public override void Attack( Mobile combatant )
        {
            if( combatant != null )
            {
                if( combatant != this && combatant is Midgard2PlayerMobile && TownHelper.IsInHisOwnCity( this ) )
                {
                    Guild sourceGuild = Guild as Guild;
                    Guild targetGuild = combatant.Guild as Guild;

                    // townenemy per attacchi tra cittadini
                    if( sourceGuild != null && targetGuild != null && sourceGuild.IsEnemy( targetGuild ) && !GuildsHelper.IsGuildAdept( combatant ) )
                    {
                        SendMessage( "I tuoi nemici di gilda ora possono attaccarti!" );
                        TownEnemy = true;
                    }
                }
            }

            base.Attack( combatant );
        }

        /// <summary>
        /// Overridable. Evento invocato quando un M2pm fa un azione criminale
        /// </summary>
        public override void OnHarmfulAction( Mobile target, bool isCriminal )
        {

            if( DisguiseTimers.IsDisguised( this ) || SketchGump.IsCamuflated( this ) )
            {
                SendMessage( "Your camuflage is gone..." );
                DisguiseTimers.RemoveTimer( this );
                Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( SketchGump.OnDisguiseExpire ), this );
            }

            if( target != this && target.Guild != null )
            {
                bool enemyAction = false;
                foreach( Guild ally in ( (Guild)target.Guild ).Allies )
                {
                    ally.AddTempEnemy( this );
                    enemyAction = true;
                }

                if( enemyAction )
                    SendMessage( 37, "For two minutes you are involved in a guild war!" );
            }

            if( RessSystem.IsImmune( this ) )
                RessSystem.EndRessImmune( this );

            base.OnHarmfulAction( target, isCriminal );
        }

        /// <summary>
        /// Overridable. Event invoked before the Mobile <see cref="PlayerMobile.Move">moves</see>.
        /// </summary>
        /// <returns>True if the move is allowed, false if not.</returns>
        protected override bool OnMove( Direction d )
        {
            if( PokerGame != null && !HasGump( typeof( PokerLeaveGump ) ) )
            {
                SendGump( new PokerLeaveGump( this, PokerGame ) );
                return false;
            }

            if( AccessLevel == AccessLevel.Player && Alive && Hidden )
            {
                if( ( Skills.Stealth.Value >= 50.0 && Mounted ) || !Mounted )
                {
                    if( ( d & Direction.Running ) != 0 )
                    {
                        SendMessage( "Thou can only move slowly while hidden!" );
                        RevealingAction( true );
                    }
                    else
                    {
                        if( AllowedStealthSteps-- <= 0 )
                            SkillHandlers.Stealth.OnUse( this );

                        return true;
                    }
                }
                else if( ScoutSystem.WalksHiddenInForest && ClassSystem.IsScout( this ) && ScoutSystem.HandleOnMove( this, d ) )
                    return true;
            }

            return base.OnMove( d );
        }

        /// <summary>
        /// Overridable. Metodo invocato quando player tenta di parlare.
        /// </summary>
        public override void OnSaid( SpeechEventArgs e )
        {
            if( IsSmothed )
            {
                SendLocalizedMessage( 1064257 ); // You cannot speak while narcotized! May be this is only a nightmare...
                e.Blocked = true;
            }

            base.OnSaid( e );
        }

        /// <summary>
        /// Invoked when the short term murders change
        /// </summary> 
        public override void OnKillsChange( int oldValue )
        {
            if( oldValue == 5 && Kills == 4 )
                LifeTimeKills = LifeTimeKills + 1;

            if( LifeTimeKills == 5 )
            {
                PermaRed = true;
                SendMessage( 37, "You are known throughout the land as a murderous brigand." );
            }

            base.OnKillsChange( oldValue );
        }

        /// <summary>
        /// Overridable. Metodo invocato quando player tenta di castare.
        /// </summary>
        public override bool CheckSpellCast( ISpell spell )
        {
            if( RessSystem.IsImmune( this ) && spell is Spell && ( (Spell)spell ).ForceEndRessImmune )
                RessSystem.EndRessImmune( this );

            if( ThunderstormSpell.IsUnderEffect( this ) )
            {
                if( spell is Spell )
                    ( (Spell)spell ).DoFizzle();

                SendMessage( "Thou cannot cast any spell while dazed." );
                return false;
            }

            if( spell is Spell && !Morph.CheckSpellAllowed( (Spell)spell, this, true ) )
                return false;

            return base.CheckSpellCast( spell );
        }

        public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
        {
            Poison newPoison = poison;

            if( from != null && poison != null )
            {
                int diff = poison.GetResistDifficulty();

                double tasteID = Skills[ SkillName.TasteID ].Value + CustVenomResistance * 3;
                
                //Say("Resist Serpent: {0}", CustVenomResistance*3);

                int newLevel;
                int livelliTolti;

                if( CheckSkill( SkillName.TasteID, diff, diff + 40 ) )
                {
                    livelliTolti = (int)( 0.5 + ( tasteID / 33.3 ) );
                    newLevel = poison.Level - livelliTolti;
                }
                else
                {
                    livelliTolti = (int)( 0.5 + ( tasteID / 66.6 ) );
                    newLevel = poison.Level - livelliTolti;
                }

                if( ClassSystem.IsScout( this ) )
                {
                    if( Utility.Random( 100 ) < Skills[ SkillName.Camping ].Value )
                    {
                        SendMessage( Language == "ITA" ? "La conoscenza della natura ti è molto utile!" : "Your knowledge of nature is very helpful!" );
                        livelliTolti++;
                        newLevel--;
                        if( Utility.Random( 100 ) < Skills[ SkillName.Poisoning ].Value )
                        {
                            livelliTolti++;
                            newLevel--;
                        }
                    }
                }

                if( PlayerDebug )
                {
                    //SendMessage( "Debug: {0}", (int)( 0.5 + ( tasteID / 33.3 ) ) );
                    SendMessage( "Debug Livelli veleno tolti: {0}", livelliTolti );
                    SendMessage( "Debug: diff: {0} oldLevel: {1} - newLevel: {2}", diff, poison.ToString(), newPoison.ToString() );
                }

                if( poison.RealLevel - livelliTolti < 0 )//edit by Arlas, nuovi veleni.
                {
                    SendMessage( Language == "ITA" ? "Resisti al veleno!" : "You resist to the poison!" );
                    return ApplyPoisonResult.Immune;
                }

                if( newLevel != poison.Level )
                    newPoison = Poison.GetPoison( newLevel );
            }

            return base.ApplyPoison( from, newPoison );
        }

        public override void OnCured( Mobile from, Poison poison )
        {
		if( poison != null )
		{
			if ( poison.Level >= 39 )//Lentezza
				Slowed = false;
			else if ( poison.Level >= 34 )//Blocco
				Frozen = false;
			else if ( poison.Level >= 29 )//Paralisi
				Paralyzed = false;
		}

		//return base.OnCured( from, poison );
        }

        /// <summary>
        /// Overridable. Event invoked after the Mobile is resurrected
        /// </summary>
        public override void OnAfterResurrect()
        {
            RessSystem.BeginRessImmune( this );

            base.OnAfterResurrect();
        }

        public override bool OnBeforeDeath()
        {
            XmlAttach.HandleOnBeforeDeath( this );

            return base.OnBeforeDeath();
        }

        /// <summary>
        /// Overridable. Metodo invocato quando un player e' morto ma prima di AfterDeath
        /// </summary>
        public override void OnDeath( Container c )
        {
            //reset combatant
            Combatant = null;

            if( LastKiller != null )
                ( LastKiller ).Combatant = null;

            if( IsSmothed && c is Corpse )
                ( (Corpse)c ).CorpseName = string.Format( "{0} laying down", ( String.IsNullOrEmpty( Name ) ? "" : Name ) );

            if( c is Corpse )
                c.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, DeathMessage );

            KarmaMod = -1;
            FameMod = -1;
            TownMod = MidgardTowns.None;

            SketchGump.StopTimer( this );

            TownSystem.HandleTownDeath( this );

            XmlAttach.HandleOnDeath( this, c );

            Morph.RemoveContext( this, true );

            if( Utility.Random( 20 ) == 0 || PlayerDebug )
                VisitingDeath.SummonTheDeath( this );

            Midgard.Engines.WarSystem.Core.HandleDeath( this, FindMostRecentDamager( true ) );

            if( AccessLevel > AccessLevel.Counselor || Debug )
                Timer.DelayCall( TimeSpan.FromSeconds( 2 ), new TimerStateCallback( AutoResCallback ), new object[] { this, true } );

            base.OnDeath( c );
        }

        public static void AutoResCallback( object state )
        {
            object[] args = (object[])state;

            Mobile m = (Mobile)args[ 0 ];
            bool refresh = (bool)args[ 1 ];

            if( m == null )
                return;

            m.PlaySound( 0x214 );
            m.FixedEffect( 0x376A, 10, 16 );
            m.Resurrect();

            if( m.Corpse != null )
            {
                m.MoveToWorld( m.Corpse.Location, m.Corpse.Map );

                Corpse c = m.Corpse as Corpse;
                if( c != null )
                    c.Open( m, true, true );

                m.Corpse.LootType = LootType.Regular;
            }

            if( refresh )
            {
                m.Hits = m.HitsMax;
                m.Mana = m.ManaMax;
                m.Stam = m.StamMax;
            }
        }

        /// <summary>
        /// Overridable. Event invoked when the Mobile is single clicked.
        /// </summary>
        public override void OnSingleClick( Mobile from )
        {
            if( SketchGump.IsCamuflated( this ) )
            {
                SketchGump.HandleOnSingleClick( this, from );
                return;
            }

            if( DisplayCitizenStatus && Town != MidgardTowns.None )
            {
                TownPlayerState tps = TownPlayerState.Find( this );

                StringBuilder sb = new StringBuilder();

                if( tps != null )
                {
                    string town = TownHelper.FindTownName( Town );

                    if( tps.IsWarLord )
                        sb.AppendFormat( from.Language == "ITA" ? "WarLord di {0}, " : "WarLord of {0}, ", town );

                    if( !string.IsNullOrEmpty( tps.CustomTownOffice ) )
                        sb.AppendFormat( string.Format( from.Language == "ITA" ? "{0} di {1}" : "{0} of {1}", tps.CustomTownOffice, town ) );
                    else if( tps.TownOffice != TownOffices.None )
                        sb.AppendFormat( string.Format( from.Language == "ITA" ? "{0} di {1}" : "{0} of {1}", tps.TownOffice, town ) );
                    else
                        sb.AppendFormat( string.Format( from.Language == "ITA" ? (from.Female ? "Cittadina di {0}" : "Cittadino di {0}") : "Citizen of {0}", town ) );

                    if( !string.IsNullOrEmpty( tps.CustomProfession ) )
                        sb.AppendFormat( ", {0}", tps.CustomProfession );
                    else if( tps.TownProfession != Professions.None )
                        sb.AppendFormat( string.Format( ", {0}", tps.TownProfession ) );

                    if( tps.TownSystem.Type != VirtueType.Regular )
                        sb.AppendFormat( ", ({0})", tps.TownSystem.Type );

                    int hue;

                    if( NameHue != -1 )
                        hue = NameHue;
                    else if( AccessLevel > AccessLevel.Player )
                        hue = 0xB;
                    else
                        hue = Notoriety.GetHue( Notoriety.Compute( from, this ) );

                    PrivateOverheadMessage( MessageType.Label, hue, true, sb.ToString(), from.NetState );
                }
            }

            if( DisplayPvpStatus )
            {
                XmlPointsAttach a = (XmlPointsAttach)XmlAttach.FindAttachment( this, typeof( XmlPointsAttach ) );

                if( a != null )
                    PrivateOverheadMessage( MessageType.Label, ( from == this ? 0x62 : 0x26 ), true, String.Format( "Kills: {0} | Death: {1} | Rank: {2}", a.Kills, a.Deaths, a.Rank ), from.NetState );
            }

            base.OnSingleClick( from );
        }

        public override bool AllowItemUse( Item item )
        {
            return !BlendWithForestSpell.IsImmune( this ) && base.AllowItemUse( item );
        }

        public override bool AllowEquipFrom( Mobile mob )
        {
            return !BlendWithForestSpell.IsImmune( this ) && base.AllowEquipFrom( mob );
        }

        public override bool AllowSkillUse( SkillName skill )
        {
            return !BlendWithForestSpell.IsImmune( this ) && base.AllowSkillUse( skill );
        }

        /// <summary>
        /// Overridable. Metodo invocato quando un client chiede le contextmenus del pg target.
        /// Da notare come tali contexts devono essere presenti nel cliloc perche' l'attuale client
        /// non supporta context non localizzate.
        /// </summary>
        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            if( from == this )
            {
                Engines.PartySystem.Party.GetSelfContextMenus( from, this, list );
                TownSystem.GetSelfContextMenus( from, this, list );
                PrivateMessageSystem.GetSelfContextMenus( from, this, list );
                JailSystem.GetSelfContextMenus( from, this, list );
                HardLabourPersistance.GetSelfContextMenus( from, this, list );
            }

            if( from.AccessLevel > AccessLevel )
                JailSystem.GetContextMenus( from, this, list );
        }

        public override int GetSeason()
        {
            int season = -1;
            if( Region is BaseRegion )
                season = ( (BaseRegion)Region ).ForcedSeason;

            return season != -1 ? season : base.GetSeason();
        }

        /// <summary>
        /// Overridable. Virtual event invoked when <see cref="Region" /> changes.
        /// </summary>
        public override void OnRegionChange( Region oldRegion, Region newRegion )
        {
            int newHue = newRegion.GetSolidHue();

            if( newHue != -1 && SolidHueOverride != newHue ) // If we are entering a custom solid hue region
                SolidHueOverride = newHue;
            else if( newHue == -1 && SolidHueOverride != -1 ) // If we are leaving a custom solid hue region
                SolidHueOverride = -1;

            int oldSeason = oldRegion.GetSeason();
            int newSeason = newRegion.GetSeason();

            if( Map != null && newSeason != oldSeason )
            {
                if( PlayerDebug )
                    SendMessage( "OnRegionChange: from {0} ({1}) to {2} ({3})", oldRegion.Name, oldSeason, newRegion.Name, newSeason );

                NetState ns = NetState;
                if( ns != null )
                    ns.Send( SeasonChange.Instantiate( newSeason, true ) );
            }

            base.OnRegionChange( oldRegion, newRegion );
        }

        #endregion

        #region serialization
        public Midgard2PlayerMobile( Serial serial )
            : base( serial )
        {
            BountyUpdateList = new List<string>();
            TownMod = MidgardTowns.None;
            PlayerMessageIgnoreList = new List<Mobile>();

            TamingBOBFilter = new TamingBOBFilter();

            Info = new PersonalInfo( this );
            m_TownBans = new TownBanAttribute( this );
            m_TownPermaBans = new TownPermaBanAttribute( this );
            m_RaceLanguages = new RaceLanguageAttribute( this );
            m_PlayerStoneAttributes = new StoneAttributes( this );
            m_QuestAttributes = new QuestAttributes( this );
            m_HardLabourAttributes = new HardLabourAttribute( this );
            m_NotorietyAttributes = new NotorietyAttributes( this );
            m_CustResistAttributes = new CustomResistAttributes( this );
            m_CamuflageAttributes = new CamuflageAttributes( this );
            m_StaffAttributes = new StaffAttributes( this );
            m_PlayerFlagsAttributes = new PlayerFlagsAttributes( this );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 29 );

            // Versione 29:
            m_TownPermaBans.Serialize( writer );

            // Versione 28: added by Arlas: Double weapons.
            writer.Write( UseDoubleWeaponsTogether );

            // Versione 27: added by Magius(CHE): Notifications.
            writer.Write( NotificationPVMTankEnabled );
            writer.Write( NotificationPolRawPointsEnabled );

            // Versione 26: added by Magius(CHE): PolSkillRawPoints.
            writer.Write( PolSkillRawPointsEnabled );
            if( PolSkillRawPointsEnabled )
                m_PolRawPoints.Serialize( writer );

            // Versione 25:
            // rimossa m_Class

            // Versione 24:
            writer.WriteEncodedInt( (int)CraftMark );

            // Versione 23:
            writer.WriteEncodedInt( (int)CustomJobTitle );

            // Versione 22:
            writer.WriteEncodedInt( LifeTimeKills );

            // Versione 21:
            writer.WriteEncodedInt( (int)TrueLanguage );

            // Versione 20:
            writer.Write( LastTownLeaving );

            // Versione 19:
            m_RaceLanguages.Serialize( writer );

            // Versione 18:
            Info.Serialize( writer );

            // Versione 17:
            m_TownBans.Serialize( writer );

            // Versione 16:
            writer.Write( LastSmithBulkOrderValue );
            writer.Write( LastTailorBulkOrderValue );

            // Versione 15:
            writer.Write( SphynxBonusExpiration );

            // Versione 14
            writer.Write( m_FameMod );
            writer.Write( m_KarmaMod );
            writer.Write( (int)TownMod );
            writer.Write( Alias );

            // Versione 13
            writer.Write( LastGuildLeaving );

            // Versione 12 rimosse alcune props

            // Versione 11
            writer.Write( (int)MidFlags );

            // Versione 10
            writer.Write( HardLabourCondamner );
            writer.Write( HardLabourInfo );

            // Versione 9 rimosso m_CustomTownOffice

            // Versione 8
            if( TamingBOBFilter == null )
                TamingBOBFilter = new TamingBOBFilter();
            TamingBOBFilter.Serialize( writer );
            writer.Write( NextTamingBulkOrder );
            writer.Write( PossessMob );
            writer.Write( PossessStorageMob );

            // Versione 7 rimosso m_AcceptPrivateMessages

            // Versione 6 rimosso m_DisplayCitizenStatus

            // Versione 5
            writer.Write( QuestDeltaTimeExpiration );

            // Versione 4 rimosso m_TownOffice

            // Versione 3 rimosso m_Town

            // Versione 2
            writer.Write( m_Minerals2Mine );

            // Versione 1 rimossi m_OnlineVisible e m_IsMidgardSpawner
            // writer.Write( (int)m_Class );
            writer.Write( QuestVariable1 );
            writer.Write( QuestVariable2 );
            writer.Write( QuestVariable3 );
            writer.Write( QuestVariable4 );
            writer.Write( QuestVariable5 );
            writer.Write( QuestString );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 29:
                    {
                        m_TownPermaBans = new TownPermaBanAttribute( this, reader );
                        goto case 28;
                    }
                case 28:
                    {
                        UseDoubleWeaponsTogether = reader.ReadBool();
                        goto case 27;
                    }
                //added by Magius(CHE): Notifications.
                case 27:
                    {
                        NotificationPVMTankEnabled = reader.ReadBool();
                        NotificationPolRawPointsEnabled = reader.ReadBool();
                        goto case 26;
                    }
                //added by Magius(CHE): PolSkillRawPoints.
                case 26:
                    {
                        if( reader.ReadBool() )
                            m_PolRawPoints = new Midgard.Engines.PolRawPoints.MobileSkillHandler( this, reader );
                        goto case 25;
                    }
                case 25:
                    {
                        goto case 24;
                    }
                case 24:
                    {
                        CraftMark = (CraftMarkOption)reader.ReadEncodedInt();
                        goto case 23;
                    }
                case 23:
                    {
                        CustomJobTitle = (SkillName)reader.ReadEncodedInt();
                        goto case 22;
                    }
                case 22:
                    {
                        LifeTimeKills = reader.ReadEncodedInt();
                        goto case 21;
                    }
                case 21:
                    {
                        TrueLanguage = (LanguageType)reader.ReadEncodedInt();
                        goto case 20;
                    }
                case 20:
                    {
                        if( version < 21 )
                            TrueLanguage = LanguageType.Ita;

                        LastTownLeaving = reader.ReadDateTime();
                        goto case 19;
                    }
                case 19:
                    {
                        m_RaceLanguages = new RaceLanguageAttribute( this, reader );
                        goto case 18;
                    }
                case 18:
                    {
                        Info = new PersonalInfo( this, reader );
                        goto case 17;
                    }
                case 17:
                    {
                        m_TownBans = new TownBanAttribute( this, reader );
                        goto case 16;
                    }
                case 16:
                    {
                        LastSmithBulkOrderValue = reader.ReadInt();
                        LastTailorBulkOrderValue = reader.ReadInt();
                        goto case 15;
                    }
                case 15:
                    {
                        SphynxBonusExpiration = reader.ReadTimeSpan();
                        goto case 14;
                    }
                case 14:
                    {
                        m_FameMod = reader.ReadInt();
                        m_KarmaMod = reader.ReadInt();
                        TownMod = (MidgardTowns)reader.ReadInt();
                        Alias = reader.ReadMobile();
                        goto case 13;
                    }
                case 13:
                    {
                        LastGuildLeaving = reader.ReadDateTime();
                        goto case 12;
                    }
                case 12:
                    goto case 11;
                case 11:
                    {
                        MidFlags = (MidgardPlayerFlag)reader.ReadInt();
                        goto case 10;
                    }
                case 10:
                    {
                        HardLabourCondamner = reader.ReadString();
                        HardLabourInfo = reader.ReadString();
                        goto case 9;
                    }
                case 9:
                    {
                        if( version < 12 )
                            reader.ReadString();
                        goto case 8;
                    }
                case 8:
                    {
                        TamingBOBFilter = new TamingBOBFilter( reader );
                        NextTamingBulkOrder = reader.ReadTimeSpan();
                        PossessMob = reader.ReadMobile();
                        PossessStorageMob = reader.ReadMobile();
                        goto case 7;
                    }
                case 7:
                    {
                        if( version < 11 )
                            AcceptPrivateMessages = reader.ReadBool();
                        goto case 6;
                    }
                case 6:
                    {
                        if( version < 11 )
                            DisplayCitizenStatus = reader.ReadBool();
                        goto case 5;
                    }
                case 5:
                    {
                        QuestDeltaTimeExpiration = reader.ReadTimeSpan();
                        goto case 4;
                    }
                case 4:
                    {
                        if( version < 12 )
                            reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                    {
                        if( version < 12 )
                            reader.ReadInt();
                        goto case 2;
                    }
                case 2:
                    {
                        m_Minerals2Mine = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        if( version < 11 )
                            OnlineVisible = reader.ReadBool();

                        if( version < 25 )
                            reader.ReadInt();

                        if( version < 11 )
                            IsMidgardSpawner = reader.ReadBool();

                        QuestVariable1 = reader.ReadInt();
                        QuestVariable2 = reader.ReadInt();
                        QuestVariable3 = reader.ReadInt();
                        QuestVariable4 = reader.ReadInt();
                        QuestVariable5 = reader.ReadInt();
                        QuestString = reader.ReadString();
                        goto case 0;
                    }
                case 0:
                    {
                        break;
                    }
            }

            if( AccessLevel == AccessLevel.Player )
                SkillSubCap.CheckLogin( this );

            UpdateStonePowers();

            if( BankBox != null )
                BankBox.UpdateTotals();
        }

        public void VerifyAccountCallback()
        {
            if( Account != null )
                return;

            try
            {
                using( StreamWriter op = new StreamWriter( "Logs/character-delete.log", true ) )
                    op.WriteLine( "{0}\t{1}", DateTime.Now, string.Format( "Serial: {0} Name: {1}", Serial.Value, Name ?? "" ) );
            }
            catch
            {
            }

            Delete();
        }
        #endregion
    }
}