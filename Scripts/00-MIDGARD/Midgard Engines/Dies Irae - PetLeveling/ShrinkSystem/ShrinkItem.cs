/***************************************************************************
 *                                  ShrinkItem.cs
 *                            		----------------
 *  begin                	: Febbraio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.PetSystem;
using Server.ContextMenus;
using Server.Factions;
using Server.Mobiles;

namespace Server.Items
{
    public class ShrinkItem : Item
    {
        #region campi
        private bool m_IsDeed = false;// Set True for all shrunken pets to be in deed form.
        private bool m_Disabled = false; //Set True to disable all newly made shrink items. (Use [shrinklockdown to disable all current shrinkitems)
        private BaseCreature m_Mob;
        private int m_PetHue = 0;
        private bool m_PetBonded;
        private Mobile m_PetOwner;
        private bool m_IsFemale;
        private string m_PetTitle;
        private Faction m_Faction;
        private DateTime m_BondingBegin;
        private int m_AbilityPoints;
        private bool m_BardingExceptional;
        private Mobile m_BardingCrafter;
        private int m_BardingHP;
        private bool m_HasBarding;
        private CraftResource m_BardingResource;
        private DateTime m_SheepWool;
        private int m_Exp;
        private int m_NextLevel;
        private int m_Level;
        private int m_MaxLevel;
        private bool m_AllowMating;
        private int m_Gen;
        private DateTime m_MatingDelay;
        private int m_PetHitsNow;
        private int m_PetStamNow;
        private int m_PetManaNow;
        private int m_MountID;
        private Type m_MobType;
        private String m_MobTypeString;
        private bool m_Locked;
        private double m_PetMinTame;
        private int m_PetControlSlots;
        private string m_PetName;
        private int m_PetMinDamage;
        private int m_PetMaxDamage;
        private int m_PetBody;
        private int m_PetSound;
        private int m_PetVArmor;
        private int m_PetStr;
        private int m_PetDex;
        private int m_PetInt;
        private int m_PetHits;
        private int m_PetStam;
        private int m_PetMana;
        private int m_PetPhysicalResist;
        private int m_PetColdResist;
        private int m_PetFireResist;
        private int m_PetPoisonResist;
        private int m_PetEnergyResist;
        private int m_PetPhysicalDamage;
        private int m_PetColdDamage;
        private int m_PetFireDamage;
        private int m_PetPoisonDamage;
        private int m_PetEnergyDamage;
        private double m_PetWrestling;
        private double m_PetTactics;
        private double m_PetResist;
        private double m_PetAnatomy;
        private double m_PetPoisoning;
        private double m_PetMagery;
        private double m_PetEvalInt;
        private double m_PetMed;
        private double m_CapWrestling;
        private double m_CapTactics;
        private double m_CapResist;
        private double m_CapAnatomy;
        private double m_CapPoisoning;
        private double m_CapMagery;
        private double m_CapEvalInt;
        private double m_CapMed;
        private PetRarity.Rarity m_RarityLevel;
        #endregion

        #region proprietà
        [CommandProperty( AccessLevel.Administrator )]
        public bool Disabled
        {
            get { return m_Disabled; }
            set { m_Disabled = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsDeed
        {
            get { return m_IsDeed; }
            set
            {
                m_IsDeed = value;
                if( m_IsDeed )
                {
                    if( m_Mob != null )
                    {
                        ItemID = 0x14EF;
                        Name = "a pet deed";
                    }
                    else
                    {
                        ItemID = 0xFAA;
                        Name = "Unlinked Shrink Item!";
                    }
                }
                else
                {
                    if( m_Mob != null )
                    {
                        ItemID = ShrinkTable.Lookup( m_Mob );
                        Name = "a shrunken pet";
                    }
                    else
                    {
                        ItemID = 0xFAA;
                        Name = "Unlinked Shrink Item!";
                    }
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public BaseCreature Pet
        {
            get { return m_Mob; }
            set
            {
                m_Mob = value;
                if( m_IsDeed != true )
                {
                    ItemID = ShrinkTable.Lookup( m_Mob );
                    Name = "a shrunken pet";
                }
                else
                {
                    ItemID = 0x14EF;
                    Name = "a pet deed";
                }

                if( m_Mob is SwampDragon )
                {
                    SwampDragon sd = (SwampDragon)m_Mob;
                    m_BardingExceptional = sd.BardingExceptional;
                    m_BardingCrafter = sd.BardingCrafter;
                    m_BardingHP = sd.BardingHP;
                    m_HasBarding = sd.HasBarding;
                    m_BardingResource = sd.BardingResource;
                }

                if( m_Mob is Sheep )
                {
                    Sheep s = (Sheep)m_Mob;
                    m_SheepWool = s.NextWoolTime;
                }

                if( m_Mob is FactionWarHorse )
                {
                    FactionWarHorse fwh = (FactionWarHorse)m_Mob;
                    m_Faction = fwh.Faction;
                }

                if( m_Mob is BaseMount )
                {
                    BaseMount mount = (BaseMount)m_Mob;
                    m_MountID = mount.ItemID;
                }

                m_PetTitle = m_Mob.Title;

                m_BondingBegin = m_Mob.BondingBegin;

                m_Exp = m_Mob.Exp;
                m_NextLevel = m_Mob.NextLevel;
                m_Level = m_Mob.Level;
                m_MaxLevel = m_Mob.MaxLevel;
                m_AllowMating = m_Mob.AllowMating;
                m_Gen = m_Mob.Generation;
                MatingDelay = m_Mob.MatingDelay;

                m_AbilityPoints = m_Mob.AbilityPoints;

                m_PetHitsNow = m_Mob.Hits;
                m_PetStamNow = m_Mob.Stam;
                m_PetManaNow = m_Mob.Mana;

                m_PetName = m_Mob.Name;
                m_PetHue = m_Mob.Hue;
                Hue = m_PetHue;
                m_PetBody = m_Mob.BodyValue;
                m_PetSound = m_Mob.BaseSoundID;
                m_PetVArmor = m_Mob.VirtualArmor;

                m_PetMinTame = 0.0;

                m_PetControlSlots = m_Mob.ControlSlots;

                m_IsFemale = m_Mob.Female;

                m_PetMinDamage = m_Mob.DamageMin;
                m_PetMaxDamage = m_Mob.DamageMax;

                m_PetBonded = m_Mob.IsBonded;

                m_PetStr = m_Mob.RawStr;
                m_PetDex = m_Mob.RawDex;
                m_PetInt = m_Mob.RawInt;

                m_PetHits = m_Mob.HitsMax;
                m_PetStam = m_Mob.StamMax;
                m_PetMana = m_Mob.ManaMax;

                if( m_Mob.PhysicalResistance <= m_Mob.PhysicalResistanceSeed )
                    m_PetPhysicalResist = m_Mob.PhysicalResistanceSeed;
                else
                    m_PetPhysicalResist = m_Mob.PhysicalResistance;

                if( m_Mob.ColdResistance <= m_Mob.ColdResistSeed )
                    m_PetColdResist = m_Mob.ColdResistSeed;
                else
                    m_PetColdResist = m_Mob.ColdResistance;

                if( m_Mob.FireResistance <= m_Mob.FireResistSeed )
                    m_PetFireResist = m_Mob.FireResistSeed;
                else
                    m_PetFireResist = m_Mob.FireResistance;

                if( m_Mob.PoisonResistance <= m_Mob.PoisonResistSeed )
                    m_PetPoisonResist = m_Mob.PoisonResistSeed;
                else
                    m_PetPoisonResist = m_Mob.PoisonResistance;

                if( m_Mob.EnergyResistance <= m_Mob.EnergyResistSeed )
                    m_PetEnergyResist = m_Mob.EnergyResistSeed;
                else
                    m_PetEnergyResist = m_Mob.EnergyResistance;

                m_PetPhysicalDamage = m_Mob.PhysicalDamage;
                m_PetColdDamage = m_Mob.ColdDamage;
                m_PetFireDamage = m_Mob.FireDamage;
                m_PetPoisonDamage = m_Mob.PoisonDamage;
                m_PetEnergyDamage = m_Mob.EnergyDamage;

                m_PetWrestling = m_Mob.Skills[ SkillName.Wrestling ].Base;
                m_PetTactics = m_Mob.Skills[ SkillName.Tactics ].Base;
                m_PetResist = m_Mob.Skills[ SkillName.MagicResist ].Base;
                m_PetAnatomy = m_Mob.Skills[ SkillName.Anatomy ].Base;
                m_PetPoisoning = m_Mob.Skills[ SkillName.Poisoning ].Base;
                m_PetMagery = m_Mob.Skills[ SkillName.Magery ].Base;
                m_PetEvalInt = m_Mob.Skills[ SkillName.EvalInt ].Base;
                m_PetMed = m_Mob.Skills[ SkillName.Meditation ].Base;

                m_CapWrestling = m_Mob.Skills[ SkillName.Wrestling ].Cap;
                m_CapTactics = m_Mob.Skills[ SkillName.Tactics ].Cap;
                m_CapResist = m_Mob.Skills[ SkillName.MagicResist ].Cap;
                m_CapAnatomy = m_Mob.Skills[ SkillName.Anatomy ].Cap;
                m_CapPoisoning = m_Mob.Skills[ SkillName.Poisoning ].Cap;
                m_CapMagery = m_Mob.Skills[ SkillName.Magery ].Cap;
                m_CapEvalInt = m_Mob.Skills[ SkillName.EvalInt ].Cap;
                m_CapMed = m_Mob.Skills[ SkillName.Meditation ].Cap;

                m_RarityLevel = m_Mob.RarityLevel;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public PetRarity.Rarity RarityLevel
        {
            get { return m_RarityLevel; }
            set { m_RarityLevel = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetHue
        {
            get { return m_PetHue; }
            set
            {
                m_PetHue = value;
                Hue = m_PetHue;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Type MobType
        {
            get { return m_MobType; }
            set
            {
                m_MobType = value;
                m_MobTypeString = m_MobType.Name;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public String MobTypeString
        {
            get { return m_MobTypeString; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PetBonded
        {
            get { return m_PetBonded; }
            set { m_PetBonded = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile PetOwner
        {
            get { return m_PetOwner; }
            set { m_PetOwner = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Lock
        {
            get { return m_Locked; }
            set { m_Locked = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetBodyValue
        {
            get { return m_PetBody; }
            set { m_PetBody = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetSoundID
        {
            get { return m_PetSound; }
            set { m_PetSound = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetVirtualArmor
        {
            get { return m_PetVArmor; }
            set { m_PetVArmor = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double PetMinTame
        {
            get { return m_PetMinTame; }
            set { m_PetMinTame = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StatStr
        {
            get { return m_PetStr; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StatDex
        {
            get { return m_PetDex; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StatInt
        {
            get { return m_PetInt; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StatHits
        {
            get { return m_PetHits; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StatStam
        {
            get { return m_PetStam; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StatMana
        {
            get { return m_PetMana; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ResistPhysical
        {
            get { return m_PetPhysicalResist; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ResistCold
        {
            get { return m_PetColdResist; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ResistFire
        {
            get { return m_PetFireResist; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ResistPoison
        {
            get { return m_PetPoisonResist; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ResistEnergy
        {
            get { return m_PetEnergyResist; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DamageTypePhysical
        {
            get { return m_PetPhysicalDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DamageTypeCold
        {
            get { return m_PetColdDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DamageTypeFire
        {
            get { return m_PetFireDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DamageTypePoison
        {
            get { return m_PetPoisonDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DamageTypeEnergy
        {
            get { return m_PetEnergyDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillWrestling
        {
            get { return m_PetWrestling; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillTactics
        {
            get { return m_PetTactics; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillMagicResist
        {
            get { return m_PetResist; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillAnatomy
        {
            get { return m_PetAnatomy; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillPoisoning
        {
            get { return m_PetPoisoning; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillMagery
        {
            get { return m_PetMagery; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillEvalInt
        {
            get { return m_PetEvalInt; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double SkillMeditation
        {
            get { return m_PetMed; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetMinDamage
        {
            get { return m_PetMinDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetMaxDamage
        {
            get { return m_PetMaxDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string PetName
        {
            get { return m_PetName; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MountID
        {
            get { return m_MountID; }
            set { m_MountID = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetHitsNow
        {
            get { return m_PetHitsNow; }
            set { m_PetHitsNow = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetStamNow
        {
            get { return m_PetStamNow; }
            set { m_PetStamNow = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PetManaNow
        {
            get { return m_PetManaNow; }
            set { m_PetManaNow = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile BardingCrafter
        {
            get { return m_BardingCrafter; }
            set { m_BardingCrafter = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool BardingExceptional
        {
            get { return m_BardingExceptional; }
            set { m_BardingExceptional = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int BardingHP
        {
            get { return m_BardingHP; }
            set { m_BardingHP = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasBarding
        {
            get { return m_HasBarding; }
            set { m_HasBarding = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource BardingResource
        {
            get { return m_BardingResource; }
            set { m_BardingResource = value; InvalidateProperties(); }
        }

        public DateTime SheepWool
        {
            get { return m_SheepWool; }
            set { m_SheepWool = value; }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public DateTime MatingDelay
        {
            get { return m_MatingDelay; }
            set { m_MatingDelay = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Generation
        {
            get { return m_Gen; }
            set { m_Gen = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Exp
        {
            get { return m_Exp; }
            set { m_Exp = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int NextLevel
        {
            get { return m_NextLevel; }
            set { m_NextLevel = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxLevel
        {
            get { return m_MaxLevel; }
            set { m_MaxLevel = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowMating
        {
            get { return m_AllowMating; }
            set { m_AllowMating = value; }
        }

        public int AbilityPoints
        {
            get { return m_AbilityPoints; }
            set { m_AbilityPoints = value; }
        }

        public DateTime BondingBegin
        {
            get { return m_BondingBegin; }
            set { m_BondingBegin = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public String PetTitle
        {
            get { return m_PetTitle; }
            set { m_PetTitle = value; }
        }
        #endregion

        #region costruttori
        [Constructable]
        public ShrinkItem()
        {
            if( m_Mob != null )
            {
                if( m_IsDeed )
                {
                    ItemID = 0x14EF;
                    Name = "a pet deed";
                }
                else
                {
                    ItemID = ShrinkTable.Lookup( m_Mob );
                    Name = "a shrunken pet";
                }

                Movable = true;
                LootType = LootType.Blessed;
                Hue = m_PetHue;
            }
            else
            {
                ItemID = 0xFAA;
                Movable = true;
                Name = "Unlinked Shrink Item!";
                LootType = LootType.Blessed;
            }

            Weight = 25.0;
        }

        public ShrinkItem( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
		public override void OnSingleClick( Mobile from )
		{
			//base.OnSingleClick( from );

			LabelTo( from, string.Format("{0} (shrinked) (blessed)", m_Mob.Name) );
		}

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }

            bool NearAnimalTrainer = false;
            foreach( Mobile m in from.GetMobilesInRange( 5 ) )
            {
                if( m is AnimalTrainer )
                    NearAnimalTrainer = true;
            }

            if( from.AccessLevel == AccessLevel.Player && !NearAnimalTrainer )
            {
                from.SendLocalizedMessage( 1064359 ); // You must be near an animaltrainer to un-shrink your pet.
                return;
            }

            else if( m_MobTypeString == null || m_PetOwner == null || ItemID == 0xFAA )
            {
                from.SendLocalizedMessage( 1064360 ); // You have encountered an error trying to reclaim your pet, Please contact a gamemaster and tell 
                // them about this error, It could still be possible to reclaim your pet.
            }
            else if( m_Locked && m_PetOwner != from )
            {
                from.SendLocalizedMessage( 1064361 ); // This is locked and only the owner can claim this pet while locked.
                from.SendLocalizedMessage( 1064362 ); // This item is now being returned to its owner.
                m_PetOwner.AddToBackpack( this );
                m_PetOwner.SendLocalizedMessage( 1064363, String.Format( "{0}\t{1}", m_MobTypeString, from.Name ) ); // You pet ~1_PET~  has been returned to you because it was locked and
                // ~2_PLAYERNAME~  was trying to claim the pet.
            }
            else if( from.Skills[ SkillName.AnimalTaming ].Value < m_PetMinTame )
            {
                from.SendLocalizedMessage( 1064364 ); // You do not have the required taming to control this pet.
                from.SendLocalizedMessage( 1064365, m_PetMinTame.ToString( "F1" ) ); // You must have ~1_TAMING~ animal taming to reclaim this pet.
            }
            else if( from.Followers + m_PetControlSlots > from.FollowersMax )
            {
                from.SendLocalizedMessage( 1064366 ); // You have to many followers to claim this pet.
            }
            else if( Spells.SpellHelper.CheckCombat( from ) )
            {
                from.SendLocalizedMessage( 1064373 ); // You cannot reclaim your pet while your fighting.
            }
            else if( m_Disabled )
            {
                from.SendLocalizedMessage( 1064367 ); // The server is on a shrinkitem lockdown. You cannot unshrink your pet at this time.
            }
            else if( from is PlayerMobile && m_MobTypeString != null && m_PetOwner != null )
            {
                if( m_Mob != null )
                {
                    IEntity p1a = new Entity( Serial.Zero, new Point3D( from.X, from.Y, from.Z ), from.Map );
                    IEntity p2a = new Entity( Serial.Zero, new Point3D( from.X, from.Y, from.Z + 50 ), from.Map );

                    Effects.SendMovingParticles( p1a, p2a, ShrinkTable.Lookup( m_PetBody ), 1, 0, true, false, 0, 3, 1153, 1, 0, EffectLayer.Head, 0x100 );
                    from.PlaySound( 492 );

                    m_Mob.ControlMaster = from;
                    m_Mob.ControlTarget = from;
                    m_Mob.ControlOrder = OrderType.Follow;

                    if( m_Mob.IsBonded && m_PetOwner.Account != from.Account )
                        m_Mob.IsBonded = false;

                    m_Mob.IsStabled = false;
                    m_Mob.IsShrinked = false;
                    m_Mob.ShrinkItem = null;

                    m_Mob.MoveToWorld( from.Location, from.Map );

                    Delete();
                }
                else
                {
                    BaseCreature pet = null;

                    if( m_MobTypeString != null )
                    {
                        Type type = SpawnerType.GetType( m_MobTypeString );

                        if( type != null )
                        {
                            object o = Activator.CreateInstance( type );
                            pet = o as BaseCreature;
                        }
                    }

                    if( pet == null )
                    {
                        from.SendLocalizedMessage( 1064368 ); // Pet could not be claimed due to an internal error. Please contact a member of the staff.
                    }
                    else
                    {
                        #region build delle props dello shrink item
                        pet.BondingBegin = m_BondingBegin;

                        if( pet is SwampDragon )
                        {
                            SwampDragon sd = (SwampDragon)pet;
                            sd.BardingExceptional = sd.BardingExceptional;
                            sd.BardingCrafter = sd.BardingCrafter;
                            sd.BardingHP = sd.BardingHP;
                            sd.HasBarding = sd.HasBarding;
                            sd.BardingResource = sd.BardingResource;
                        }

                        if( pet is Sheep )
                        {
                            Sheep s = (Sheep)pet;
                            s.NextWoolTime = m_SheepWool;
                        }

                        if( pet is FactionWarHorse )
                        {
                            FactionWarHorse f = (FactionWarHorse)pet;
                            f.Faction = m_Faction;
                        }

                        pet.Title = m_PetTitle;

                        pet.AbilityPoints = m_AbilityPoints;

                        pet.Exp = m_Exp;
                        pet.NextLevel = m_NextLevel;
                        pet.Level = m_Level;
                        pet.MaxLevel = m_MaxLevel;
                        pet.AllowMating = m_AllowMating;
                        pet.Generation = m_Gen;
                        pet.MatingDelay = MatingDelay;

                        if( pet is BaseMount )
                        {
                            BaseMount mount = pet as BaseMount;
                            if( m_MountID >= 0 )
                                mount.ItemID = m_MountID;
                        }

                        if( m_PetBonded && m_PetOwner.Account == from.Account )
                        {
                            pet.IsBonded = true;
                        }
                        else if( m_PetBonded && m_PetOwner.Account != from.Account )
                        {
                            from.SendLocalizedMessage( 1064369 ); // This pet has lost its bonded status since you are not the original owner.
                            pet.IsBonded = false;
                        }
                        else
                        {
                            pet.IsBonded = false;
                        }

                        pet.Name = m_PetName;

                        pet.Female = m_IsFemale;

                        if( m_PetBody >= 0 )
                            pet.BodyValue = m_PetBody;

                        if( m_PetHue >= 0 )
                            pet.Hue = m_PetHue;

                        if( m_PetSound >= 0 )
                            pet.BaseSoundID = m_PetSound;

                        if( m_PetVArmor >= 0 )
                            pet.VirtualArmor = m_PetVArmor;

                        if( m_PetMinDamage >= 0 )
                            pet.DamageMin = m_PetMinDamage;

                        if( m_PetMaxDamage >= 0 )
                            pet.DamageMax = m_PetMaxDamage;

                        //Start Pet Stats
                        if( m_PetStr >= 0 )
                            pet.RawStr = m_PetStr;

                        if( m_PetDex >= 0 )
                            pet.RawDex = m_PetDex;

                        if( m_PetInt >= 0 )
                            pet.RawInt = m_PetInt;

                        if( m_PetHits >= 0 )
                            pet.HitsMaxSeed = m_PetHits;

                        if( m_PetStam >= 0 )
                            pet.StamMaxSeed = m_PetStam;

                        if( m_PetMana >= 0 )
                            pet.ManaMaxSeed = m_PetMana;
                        pet.Hits = m_PetHitsNow;
                        pet.Stam = m_PetStamNow;
                        pet.Mana = m_PetManaNow;
                        // End Pet Stats

                        // Start Resist Setting
                        if( m_PetPhysicalResist >= 0 )
                            pet.PhysicalResistanceSeed = m_PetPhysicalResist;
                        if( m_PetColdResist >= 0 )
                            pet.ColdResistSeed = m_PetColdResist;
                        if( m_PetFireResist >= 0 )
                            pet.FireResistSeed = m_PetFireResist;
                        if( m_PetPoisonResist >= 0 )
                            pet.PoisonResistSeed = m_PetPoisonResist;
                        if( m_PetEnergyResist >= 0 )
                            pet.EnergyResistSeed = m_PetEnergyResist;
                        //End Resist Setting

                        //Start Damage Type Setting
                        if( m_PetPhysicalDamage >= 0 )
                            pet.PhysicalDamage = m_PetPhysicalDamage;

                        if( m_PetColdDamage >= 0 )
                            pet.ColdDamage = m_PetColdDamage;

                        if( m_PetFireDamage >= 0 )
                            pet.FireDamage = m_PetFireDamage;

                        if( m_PetPoisonDamage >= 0 )
                            pet.PoisonDamage = m_PetPoisonDamage;

                        if( m_PetEnergyDamage >= 0 )
                            pet.EnergyDamage = m_PetEnergyDamage;
                        //End Damage Type Setting

                        //Start Setting Skills
                        if( m_PetWrestling >= 0 )
                            pet.Skills[ SkillName.Wrestling ].Base = m_PetWrestling;
                        if( m_PetTactics >= 0 )
                            pet.Skills[ SkillName.Tactics ].Base = m_PetTactics;
                        if( m_PetResist >= 0 )
                            pet.Skills[ SkillName.MagicResist ].Base = m_PetResist;
                        if( m_PetAnatomy >= 0 )
                            pet.Skills[ SkillName.Anatomy ].Base = m_PetAnatomy;
                        if( m_PetPoisoning >= 0 )
                            pet.Skills[ SkillName.Poisoning ].Base = m_PetPoisoning;
                        if( m_PetMagery >= 0 )
                            pet.Skills[ SkillName.Magery ].Base = m_PetMagery;
                        if( m_PetEvalInt >= 0 )
                            pet.Skills[ SkillName.EvalInt ].Base = m_PetEvalInt;
                        if( m_PetMed >= 0 )
                            pet.Skills[ SkillName.Meditation ].Base = m_PetMed;

                        pet.Skills[ SkillName.Wrestling ].Cap = m_CapWrestling;
                        pet.Skills[ SkillName.Tactics ].Cap = m_CapTactics;
                        pet.Skills[ SkillName.MagicResist ].Cap = m_CapResist;
                        pet.Skills[ SkillName.Anatomy ].Cap = m_CapAnatomy;
                        pet.Skills[ SkillName.Poisoning ].Cap = m_CapPoisoning;
                        pet.Skills[ SkillName.Magery ].Cap = m_CapMagery;
                        pet.Skills[ SkillName.EvalInt ].Cap = m_CapEvalInt;
                        pet.Skills[ SkillName.Meditation ].Cap = m_CapMed;
                        //End Setting Skills

                        // Rarity
                        pet.RarityLevel = m_RarityLevel;

                        if( m_PetOwner == from )
                        {
                            from.SendLocalizedMessage( 1064370, m_PetName ); // You have claimed your pet ~1_PET~.
                        }
                        else if( m_PetOwner != from )
                        {
                            from.SendLocalizedMessage( 1064371, String.Format( "{0}\t{1}", m_MobTypeString, m_PetName ) ); // You have claimed a ~1_PETTYPE~, named ~1_PET~.
                            m_PetOwner.SendLocalizedMessage( 1064372, String.Format( "{0}\t{1}", from.Name, m_PetName ) ); // ~1_PLAYERNAME~  has claimed your old pet ~2_PET~.
                        }

                        IEntity p1 = new Entity( Serial.Zero, new Point3D( from.X, from.Y, from.Z ), from.Map );
                        IEntity p2 = new Entity( Serial.Zero, new Point3D( from.X, from.Y, from.Z + 50 ), from.Map );

                        Effects.SendMovingParticles( p1, p2, ShrinkTable.Lookup( m_PetBody ), 1, 0, true, false, 0, 3, 1153, 1, 0, EffectLayer.Head, 0x100 );
                        from.PlaySound( 492 );

                        pet.Controlled = true;
                        pet.ControlMaster = from;
                        pet.Location = from.Location;
                        pet.ControlOrder = OrderType.Follow;
                        pet.ControlTarget = from;
                        pet.Map = from.Map;

                        if( pet.IsParagon )
                            pet.IsParagon = false;

                        World.AddMobile( pet );

                        if( !PetUtility.IsPackAnimal( pet ) )
                        {
                            List<Item> equipitems = pet.Items;
														
                            foreach( Item item in equipitems.ToArray() ) // Magius(CHE): trasformato in array perchè altrimenti l'insieme viene modificato dal delete e lancia l'eccezione
                            {
                                if( ( item.Layer != Layer.Bank ) && ( item.Layer != Layer.Hair ) && ( item.Layer != Layer.FacialHair ) )
                                {
                                    item.Delete();
                                }
                            }
                        }

                        Delete();
                        #endregion
                    }
                }
            }
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            if( from.Alive )
            {
                if( m_Locked == false && m_PetOwner == from )
                {
                    list.Add( new Midgard.ContextMenus.LockShrinkItem( from, this ) );
                }
                else if( m_Locked && m_PetOwner == from )
                {
                    list.Add( new Midgard.ContextMenus.UnLockShrinkItem( from, this ) );
                }
            }
        }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if( m_MobTypeString != null )
            {
                if( m_PetBonded )
                    list.Add( 1049608 ); // (bonded)

                if( m_Locked )
                    list.Add( 1049644, "Locked" ); // [~1_stuff~]
                else
                    list.Add( 1049644, "Unlocked" ); // [~1_stuff~]

                string s = m_MobTypeString;
                int capsbreak = s.IndexOfAny( "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), 1 );

                if( capsbreak > -1 )
                    list.Add( 1060663, "Name\t{0} Breed: {1} {2}", m_PetName, s.Substring( capsbreak ), s.Substring( 0, capsbreak ) ); // ~1_val~: ~2_val~

                else
                    list.Add( 1060663, "Name\t{0} Breed: {1}", m_PetName, m_MobTypeString ); // ~1_val~: ~2_val~

                list.Add( 1061640, ( m_PetOwner == null ) ? "nobody" : m_PetOwner.Name ); // Owner: ~1_OWNER~

                if( m_IsFemale )
                    list.Add( 1015328 ); // Female
                else
                    list.Add( 1015327 ); // Male

                list.Add( 1060659, "Stats\tStrength {0}, Dexterity {1}, Intelligence {2}", m_PetStr, m_PetDex, m_PetInt );  // ~1_val~: ~2_val~

                list.Add( 1060660, "Combat Skills\tWrestling {0}, Tactics {1}, Anatomy {2}, Poisoning {3}",
                    m_PetWrestling, m_PetTactics, m_PetAnatomy, m_PetPoisoning ); // ~1_val~: ~2_val~

                list.Add( 1060661, "Magic Skills\tMagery {0}, Eval Intel {1}, Magic Resist {2}, Meditation {3}",
                    m_PetMagery, m_PetEvalInt, m_PetResist, m_PetMed ); // ~1_val~: ~2_val~

                if( m_Level != 0 )
                    list.Add( 1060662, "Exp\t{0}, Level: {1}, Maximum Level: {2}", m_Exp, m_Level, m_MaxLevel ); // ~1_val~: ~2_val~
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)16 ); // version

            writer.Write( (int)m_RarityLevel );
            writer.Write( m_PetTitle );
            Faction.WriteReference( writer, m_Faction );
            writer.Write( m_Exp );
            writer.Write( m_NextLevel );
            writer.Write( m_Level );
            writer.Write( m_MaxLevel );
            writer.Write( m_AllowMating );
            writer.Write( m_Gen );
            writer.WriteDeltaTime( m_MatingDelay );
            writer.Write( m_BondingBegin );
            writer.Write( m_AbilityPoints );
            writer.Write( m_BardingExceptional );
            writer.Write( m_BardingCrafter );
            writer.Write( m_HasBarding );
            writer.Write( m_BardingHP );
            writer.Write( (int)m_BardingResource );
            writer.Write( m_CapWrestling );
            writer.Write( m_CapTactics );
            writer.Write( m_CapResist );
            writer.Write( m_CapAnatomy );
            writer.Write( m_CapPoisoning );
            writer.Write( m_CapMagery );
            writer.Write( m_CapEvalInt );
            writer.Write( m_CapMed );
            writer.Write( m_SheepWool );
            writer.Write( m_Disabled );
            writer.Write( m_IsFemale );
            writer.Write( m_PetHitsNow );
            writer.Write( m_PetStamNow );
            writer.Write( m_PetManaNow );
            writer.Write( m_MountID );
            writer.Write( m_IsDeed );
            writer.Write( m_Mob );
            writer.Write( m_PetHue );
            writer.Write( m_PetBonded );
            writer.Write( m_PetOwner );
            writer.Write( m_MobTypeString );
            writer.Write( m_Locked );
            writer.Write( m_PetMinTame );
            writer.Write( m_PetControlSlots );
            writer.Write( m_PetName );
            writer.Write( m_PetMinDamage );
            writer.Write( m_PetMaxDamage );
            writer.Write( m_PetBody );
            writer.Write( m_PetSound );
            writer.Write( m_PetVArmor );
            writer.Write( m_PetStr );
            writer.Write( m_PetDex );
            writer.Write( m_PetInt );
            writer.Write( m_PetHits );
            writer.Write( m_PetStam );
            writer.Write( m_PetMana );
            writer.Write( m_PetPhysicalResist );
            writer.Write( m_PetColdResist );
            writer.Write( m_PetFireResist );
            writer.Write( m_PetPoisonResist );
            writer.Write( m_PetEnergyResist );
            writer.Write( m_PetPhysicalDamage );
            writer.Write( m_PetColdDamage );
            writer.Write( m_PetFireDamage );
            writer.Write( m_PetPoisonDamage );
            writer.Write( m_PetEnergyDamage );
            writer.Write( m_PetWrestling );
            writer.Write( m_PetTactics );
            writer.Write( m_PetResist );
            writer.Write( m_PetAnatomy );
            writer.Write( m_PetPoisoning );
            writer.Write( m_PetMagery );
            writer.Write( m_PetEvalInt );
            writer.Write( m_PetMed );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 16: // rimosse varie variabili inutili
                    {
                        goto case 15;
                    }
                case 15:
                    {
                        m_RarityLevel = (PetRarity.Rarity)reader.ReadInt();
                        goto case 14;
                    }
                case 14: // Fixed War Horese
                    {
                        m_PetTitle = reader.ReadString();
                        goto case 13;
                    }
                case 13: // Fixed War Horese
                    {
                        m_Faction = Faction.ReadReference( reader );
                        goto case 12;
                    }
                case 12: // Advanced Pet Addon
                    {
                        m_Exp = reader.ReadInt();
                        m_NextLevel = reader.ReadInt();
                        m_Level = reader.ReadInt();
                        m_MaxLevel = reader.ReadInt();
                        m_AllowMating = reader.ReadBool();

                        if( version < 15 )
                            reader.ReadBool();

                        m_Gen = reader.ReadInt();
                        m_MatingDelay = reader.ReadDeltaTime();

                        if( version < 15 )
                        {
                            for( int i = 0; i < 18; i++ )
                                reader.ReadInt();
                            for( int i = 0; i < 19; i++ )
                                reader.ReadBool();
                        }
                        goto case 11;
                    }
                case 11: // Fixed Pets Losing Bond Time
                    {
                        m_BondingBegin = reader.ReadDateTime();
                        goto case 10;
                    }
                case 10: // Ability Points Fix
                    {
                        m_AbilityPoints = reader.ReadInt();
                        goto case 9;
                    }
                case 9: // Swamp Dragon Armor Fix
                    {
                        m_BardingExceptional = reader.ReadBool();
                        m_BardingCrafter = reader.ReadMobile();
                        m_HasBarding = reader.ReadBool();
                        m_BardingHP = reader.ReadInt();
                        m_BardingResource = (CraftResource)reader.ReadInt();
                        goto case 8;
                    }
                case 8: // Pet Skill Cap Fix
                    {
                        m_CapWrestling = reader.ReadDouble();
                        m_CapTactics = reader.ReadDouble();
                        m_CapResist = reader.ReadDouble();
                        m_CapAnatomy = reader.ReadDouble();
                        m_CapPoisoning = reader.ReadDouble();
                        m_CapMagery = reader.ReadDouble();
                        m_CapEvalInt = reader.ReadDouble();
                        m_CapMed = reader.ReadDouble();
                        goto case 7;
                    }
                case 7: // Sheep Wool Fix
                    {
                        m_SheepWool = reader.ReadDeltaTime();
                        goto case 6;
                    }
                case 6: // Disable Command Addon
                    {
                        m_Disabled = reader.ReadBool();
                        goto case 5;
                    }
                case 5: // Evo Pet Fix
                    {
                        if( version < 16 )
                        {
                            for( int i = 0; i < 6; i++ )
                                reader.ReadBool();
                        }
                        goto case 4;
                    }
                case 4: // Evo Pet Fix
                    {
                        m_IsFemale = reader.ReadBool();
                        goto case 3;
                    }
                case 3: // Insta Heal On Shrink Fix
                    {
                        m_PetHitsNow = reader.ReadInt();
                        m_PetStamNow = reader.ReadInt();
                        m_PetManaNow = reader.ReadInt();
                        goto case 2;
                    }
                case 2: // Mount Fix
                    {
                        m_MountID = reader.ReadInt();
                        goto case 1;
                    }
                case 1: // Evo Dragon Addon
                    {
                        if( version < 16 )
                        {
                            for( int i = 0; i < 2; i++ )
                                reader.ReadInt();
                            for( int i = 0; i < 3; i++ )
                                reader.ReadBool();
                        }
                        goto case 0;
                    }
                case 0: // Initial Release
                    {
                        m_IsDeed = reader.ReadBool();
                        m_Mob = (BaseCreature)reader.ReadMobile();
                        m_PetHue = reader.ReadInt();
                        m_PetBonded = reader.ReadBool();
                        m_PetOwner = reader.ReadMobile();
                        m_MobTypeString = reader.ReadString();
                        m_Locked = reader.ReadBool();
                        m_PetMinTame = reader.ReadDouble();
                        m_PetControlSlots = reader.ReadInt();
                        m_PetName = reader.ReadString();
                        m_PetMinDamage = reader.ReadInt();
                        m_PetMaxDamage = reader.ReadInt();
                        m_PetBody = reader.ReadInt();
                        m_PetSound = reader.ReadInt();
                        m_PetVArmor = reader.ReadInt();
                        m_PetStr = reader.ReadInt();
                        m_PetDex = reader.ReadInt();
                        m_PetInt = reader.ReadInt();
                        m_PetHits = reader.ReadInt();
                        m_PetStam = reader.ReadInt();
                        m_PetMana = reader.ReadInt();
                        m_PetPhysicalResist = reader.ReadInt();
                        m_PetColdResist = reader.ReadInt();
                        m_PetFireResist = reader.ReadInt();
                        m_PetPoisonResist = reader.ReadInt();
                        m_PetEnergyResist = reader.ReadInt();
                        m_PetPhysicalDamage = reader.ReadInt();
                        m_PetColdDamage = reader.ReadInt();
                        m_PetFireDamage = reader.ReadInt();
                        m_PetPoisonDamage = reader.ReadInt();
                        m_PetEnergyDamage = reader.ReadInt();
                        m_PetWrestling = reader.ReadDouble();
                        m_PetTactics = reader.ReadDouble();
                        m_PetResist = reader.ReadDouble();
                        m_PetAnatomy = reader.ReadDouble();
                        m_PetPoisoning = reader.ReadDouble();
                        m_PetMagery = reader.ReadDouble();
                        m_PetEvalInt = reader.ReadDouble();
                        m_PetMed = reader.ReadDouble();
                        break;
                    }
            }

            // Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( VerifyMob_Callback ), this );
        }
        #endregion

        public static void VerifyMob_Callback( object state )
        {
            ShrinkItem item = (ShrinkItem)state;
            if( item == null )
                return;

            if( item.Pet != null && !item.Pet.Deleted )
            {
                item.Pet.IsShrinked = true;
                item.Pet.ShrinkItem = item;
            }
        }
    }
}