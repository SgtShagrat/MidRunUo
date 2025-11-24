using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Server.ContextMenus;
using Server.Engines.Quests;
using Server.Engines.PartySystem;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Items;
using Server.Misc;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Necromancy;
using Server.Spells.Spellweaving;
using Server.Targeting;

using DruidEmpowermentSpell = Midgard.Engines.SpellSystem.DruidEmpowermentSpell;
using EtherealVoyageSpell = Midgard.Engines.SpellSystem.EtherealVoyageSpell;
using GiftOfLifeSpell = Midgard.Engines.SpellSystem.GiftOfLifeSpell;
using MasterySystem = Midgard.Engines.MonsterMasterySystem;
using PARTY = Server.Engines.PartySystem.Party;
using TalkingVendors = Midgard.Engines.TalkingVendors;

using Midgard;
using Midgard.Engines.Classes;
using Midgard.Engines.CreatureBurningSystem;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.MonsterMasterySystem;
using Midgard.Engines.PetSystem;
using Midgard.Engines.SpellSystem;
using Midgard.Misc;
using Midgard.Items;

namespace Server.Mobiles
{
	#region Enums
	/// <summary>
	/// Summary description for MobileAI.
	/// </summary>
	/// 
	public enum FightMode
	{
		None,			// Never focus on others
		Aggressor,		// Only attack aggressors
		Strongest,		// Attack the strongest
		Weakest,		// Attack the weakest
		Closest, 		// Attack the closest
		Evil			// Only attack aggressor -or- negative karma
	}

	public enum OrderType
	{
		None,			//When no order, let's roam
		Come,			//"(All/Name) come"  Summons all or one pet to your location.  
		Drop,			//"(Name) drop"  Drops its loot to the ground (if it carries any).  
		Follow,			//"(Name) follow"  Follows targeted being.  
						//"(All/Name) follow me"  Makes all or one pet follow you.  
		Friend,			//"(Name) friend"  Allows targeted player to confirm resurrection. 
		Unfriend,		// Remove a friend
		Guard,			//"(Name) guard"  Makes the specified pet guard you. Pets can only guard their owner. 
						//"(All/Name) guard me"  Makes all or one pet guard you.  
		Attack,			//"(All/Name) kill", 
						//"(All/Name) attack"  All or the specified pet(s) currently under your control attack the target. 
		Patrol,			//"(Name) patrol"  Roves between two or more guarded targets.  
		Release,		//"(Name) release"  Releases pet back into the wild (removes "tame" status). 
		Stay,			//"(All/Name) stay" All or the specified pet(s) will stop and stay in current spot. 
		Stop,			//"(All/Name) stop Cancels any current orders to attack, guard or follow.  
		Transfer		//"(Name) transfer" Transfers complete ownership to targeted player. 
	}

	[Flags]
	public enum FoodType
	{
		None			= 0x0000,
		Meat			= 0x0001,
		FruitsAndVegies		= 0x0002,
		GrainsAndHay		= 0x0004,
		Fish			= 0x0008,
		Eggs			= 0x0010,
		Gold			= 0x0020
	}

	[Flags]
	public enum PackInstinct
	{
		None			= 0x0000,
		Canine			= 0x0001,
		Ostard			= 0x0002,
		Feline			= 0x0004,
		Arachnid		= 0x0008,
		Daemon			= 0x0010,
		Bear			= 0x0020,
		Equine			= 0x0040,
		Bull			= 0x0080
	}

	public enum ScaleType
	{
		Red,
		Yellow,
		Black,
		Green,
		White,
		Blue,
		All
	}

	public enum MeatType
	{
		Ribs,
		Bird,
		LambLeg
	}

	public enum HideType
	{
		Regular,
		Spined,
		Horned,
		Barbed,

        #region mod by Dies Irae
        Humanoid,
        Undead,
        Wolf,
        Aracnid,
        Fey,
        GreenDragon,
        BlackDragon,
        BlueDragon,
        RedDragon,
        Abyss,
        #endregion

        #region mod by Dies Irae : pre-aos stuff
        Bear,
        Arachnid,
        Reptile,
        Orcish,
        Ophidian,
        Lava,
        Arctic,
        Demon,
        #endregion
    }

	#endregion

	public class DamageStore : IComparable
	{
		public Mobile m_Mobile;
		public int m_Damage;
		public bool m_HasRight;

		public DamageStore( Mobile m, int damage )
		{
			m_Mobile = m;
			m_Damage = damage;
		}

		public int CompareTo( object obj )
		{
			DamageStore ds = (DamageStore)obj;

			return ds.m_Damage - m_Damage;
		}
	}

	[AttributeUsage( AttributeTargets.Class )]
	public class FriendlyNameAttribute : Attribute
	{

		//future use: Talisman 'Protection/Bonus vs. Specific Creature
		private TextDefinition m_FriendlyName;

		public TextDefinition FriendlyName
		{
			get
			{
				return m_FriendlyName;
			}
		}

		public FriendlyNameAttribute( TextDefinition friendlyName )
		{
			m_FriendlyName = friendlyName;
		}

		public static TextDefinition GetFriendlyNameFor( Type t )
		{
			if( t.IsDefined( typeof( FriendlyNameAttribute ), false ) )
			{
				object[] objs = t.GetCustomAttributes( typeof( FriendlyNameAttribute ), false );

				if( objs != null && objs.Length > 0 )
				{
					FriendlyNameAttribute friendly = objs[0] as FriendlyNameAttribute;

					return friendly.FriendlyName;
				}
			}

			return t.Name;
		}
	}

    public class BaseCreature : Mobile, IHonorTarget, TalkingVendors.ITalkingMobile, ISpeaker
    {
        public const int MaxLoyalty = 100;

		#region Var declarations
		private BaseAI	m_AI;					// THE AI
		
		private AIType	m_CurrentAI;				// The current AI
		private AIType	m_DefaultAI;				// The default AI

		private Mobile	m_FocusMob;				// Use focus mob instead of combatant, maybe we don't whan to fight
		private FightMode m_FightMode;				// The style the mob uses

		private int		m_iRangePerception;		// The view area
		private int		m_iRangeFight;			// The fight distance

		private bool		m_bDebugAI;			// Show debug AI messages

		private int		m_iTeam;			// Monster Team

		private double		m_dActiveSpeed;			// Timer speed when active
		private double		m_dPassiveSpeed;		// Timer speed when not active
		private double		m_dCurrentSpeed;		// The current speed, lets say it could be changed by something;

		private Point3D 	m_pHome;			// The home position of the creature, used by some AI
		private int		m_iRangeHome = 10;		// The home range of the creature

		List<Type>		m_arSpellAttack;		// List of attack spell/power
		List<Type>		m_arSpellDefense;		// List of defensive spell/power

		private bool		m_bControlled;			// Is controlled
		private Mobile		m_ControlMaster;		// My master
		private Mobile		m_ControlTarget;		// My target mobile
		private Point3D		m_ControlDest;			// My target destination (patrol)
		private OrderType	m_ControlOrder;			// My order

		private int		m_Loyalty;

		private double		m_dMinTameSkill;
		private bool		m_bTamable;

		private bool		m_bSummoned = false;
		private DateTime	m_SummonEnd;
		private int			m_iControlSlots = 1;

		private bool		m_bBardProvoked = false;
		private bool		m_bBardPacified = false;
		private Mobile		m_bBardMaster = null;
		private Mobile		m_bBardTarget = null;
		private DateTime	m_timeBardEnd;
		private WayPoint	m_CurrentWayPoint = null;
		private IPoint2D	m_TargetLocation = null;

		private Mobile		m_SummonMaster;

		private int		m_HitsMax = -1;
		private	int		m_StamMax = -1;
		private int		m_ManaMax = -1;
		private int		m_DamageMin = -1;
		private int		m_DamageMax = -1;

		private int		m_PhysicalResistance, m_PhysicalDamage = 100;
		private int		m_FireResistance, m_FireDamage;
		private int		m_ColdResistance, m_ColdDamage;
		private int		m_PoisonResistance, m_PoisonDamage;
		private int		m_EnergyResistance, m_EnergyDamage;
		private int		m_ChaosDamage;
		private int		m_DirectDamage;

		private List<Mobile> 	m_Owners;
		private List<Mobile> 	m_Friends;

		private bool		m_IsStabled;

		private bool		m_HasGeneratedLoot; // have we generated our loot yet?

		private bool		m_Paragon;

		private bool		m_IsPrisoner;

		#endregion

        public virtual InhumanSpeech SpeechType{ get{ return null; } }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
		public bool IsStabled
		{
			get{ return m_IsStabled; }
			set
			{
				m_IsStabled = value;
				if ( m_IsStabled )
					StopDeleteTimer();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsPrisoner
		{
			get{ return m_IsPrisoner; }
			set{ m_IsPrisoner = value; }
		}

		protected DateTime SummonEnd
		{
			get { return m_SummonEnd; }
			set { m_SummonEnd = value; }
		}

		public virtual Faction FactionAllegiance{ get{ return null; } }
		public virtual int FactionSilverWorth{ get{ return 30; } }

		#region Bonding
		public const bool BondingEnabled = true;

        [CommandProperty( AccessLevel.Developer )] // Mod by Dies Irae
        public virtual bool IsBondable { get { return ( BondingEnabled && !Summoned && !Allured ); } }

        [CommandProperty( AccessLevel.Developer )] // Mod by Dies Irae
		public virtual TimeSpan BondingDelay{ get{ return TimeSpan.FromDays( 7.0 ); } }

        [CommandProperty( AccessLevel.Developer )] // Mod by Dies Irae
		public virtual TimeSpan BondingAbandonDelay{ get{ return TimeSpan.FromDays( 1.0 ); } }

		public override bool CanRegenHits{ get{ return !m_IsDeadPet && base.CanRegenHits; } }
		public override bool CanRegenStam{ get{ return !m_IsDeadPet && base.CanRegenStam; } }
		public override bool CanRegenMana{ get{ return !m_IsDeadPet && base.CanRegenMana; } }

        [CommandProperty( AccessLevel.Developer )] // Mod by Dies Irae
		public override bool IsDeadBondedPet{ get{ return m_IsDeadPet; } }

		private bool m_IsBonded;
		private bool m_IsDeadPet;
		private DateTime m_BondingBegin;
		private DateTime m_OwnerAbandonTime;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile LastOwner
		{
			get
			{
				if ( m_Owners == null || m_Owners.Count == 0 )
					return null;

				return m_Owners[m_Owners.Count - 1];
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsBonded
		{
			get{ return m_IsBonded; }
			set
            {
                #region mod by Dies Irae
                bool oldValue = m_IsBonded;

                if( oldValue != value )
                {
                    m_IsBonded = value; 
                    InvalidateProperties();

                    OnIsBondedChanged( oldValue );
                }
                // m_IsBonded = value; 
                // InvalidateProperties();
                #endregion
            }
		}

        [CommandProperty( AccessLevel.Developer )] // Mod by Dies Irae
		public bool IsDeadPet
		{
			get{ return m_IsDeadPet; }
			set{ m_IsDeadPet = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime BondingBegin
		{
			get{ return m_BondingBegin; }
			set{ m_BondingBegin = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime OwnerAbandonTime
		{
			get{ return m_OwnerAbandonTime; }
			set{ m_OwnerAbandonTime = value; }
		}
		#endregion
		
		#region Delete Previously Tamed Timer
		private DeleteTimer		m_DeleteTimer;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan DeleteTimeLeft
		{
			get
			{
				if ( m_DeleteTimer != null && m_DeleteTimer.Running )
					return m_DeleteTimer.Next - DateTime.Now;
				
				return TimeSpan.Zero;
			}
		}
		
		private class DeleteTimer : Timer
		{
			private Mobile m;
			
			public DeleteTimer( Mobile creature, TimeSpan delay ) : base( delay )
			{
				m = creature;
				Priority = TimerPriority.OneMinute;
			}
			
			protected override void OnTick()
			{
				m.Delete();
			}
		}
		
		public void BeginDeleteTimer()
		{
			if ( !(this is BaseEscortable) && !Summoned && !Deleted && !IsStabled )
			{
				StopDeleteTimer();
				m_DeleteTimer = new DeleteTimer( this, TimeSpan.FromDays( 3.0 ) );
				m_DeleteTimer.Start();
			}
		}

		public void StopDeleteTimer()
		{
			if ( m_DeleteTimer != null )
			{
				m_DeleteTimer.Stop();
				m_DeleteTimer = null;
			}
		}

		#endregion

		public virtual double WeaponAbilityChance{ get{ return 0.4; } }

		public virtual WeaponAbility GetWeaponAbility()
		{
			return null;
		}

		#region Elemental Resistance/Damage

		public override int BasePhysicalResistance{ get{ return m_PhysicalResistance; } }
		public override int BaseFireResistance{ get{ return m_FireResistance; } }
		public override int BaseColdResistance{ get{ return m_ColdResistance; } }
		public override int BasePoisonResistance{ get{ return m_PoisonResistance; } }
		public override int BaseEnergyResistance{ get{ return m_EnergyResistance; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PhysicalResistanceSeed{ get{ return m_PhysicalResistance; } set{ m_PhysicalResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireResistSeed{ get{ return m_FireResistance; } set{ m_FireResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdResistSeed{ get{ return m_ColdResistance; } set{ m_ColdResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonResistSeed{ get{ return m_PoisonResistance; } set{ m_PoisonResistance = value; UpdateResistances(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyResistSeed{ get{ return m_EnergyResistance; } set{ m_EnergyResistance = value; UpdateResistances(); } }


		[CommandProperty( AccessLevel.GameMaster )]
		public int PhysicalDamage{ get{ return m_PhysicalDamage; } set{ m_PhysicalDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireDamage{ get{ return m_FireDamage; } set{ m_FireDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdDamage{ get{ return m_ColdDamage; } set{ m_ColdDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonDamage{ get{ return m_PoisonDamage; } set{ m_PoisonDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyDamage{ get{ return m_EnergyDamage; } set{ m_EnergyDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ChaosDamage{ get{ return m_ChaosDamage; } set{ m_ChaosDamage = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int DirectDamage{ get{ return m_DirectDamage; } set{ m_DirectDamage = value; } }

		#endregion

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsParagon
		{
			get{ return m_Paragon; }
			set
			{
				if ( m_Paragon == value )
					return;
				#region mod by Dies Irae per XmlParagon
				/*
				else if ( value )
					Paragon.Convert( this );
				else
					Paragon.UnConvert( this );
				*/
				else if( value )
                    XmlParagon.Convert( this );
                else
                    XmlParagon.UnConvert( this );
                #endregion

				m_Paragon = value;

				InvalidateProperties();
			}
		}

		public virtual FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public virtual PackInstinct PackInstinct{ get{ return PackInstinct.None; } }

		public List<Mobile> Owners { get { return m_Owners; } }

		public virtual bool AllowMaleTamer{ get{ return true; } }
		public virtual bool AllowFemaleTamer{ get{ return true; } }
		public virtual bool SubdueBeforeTame{ get{ return false; } }
		public virtual bool StatLossAfterTame{ get{ return SubdueBeforeTame; } }

		public virtual bool Commandable{ get{ return true; } }

		public virtual Poison HitPoison{ get{ return null; } }
		public virtual double HitPoisonChance{ get{ return 0.5; } }
		public virtual Poison PoisonImmune{ get{ return null; } }

        public virtual bool BardImmune { get { return XmlAttach.FindAttachment( this, typeof( XmlAntiBard ) ) != null; } } // modifica by Dies Irae

		public virtual bool Unprovokable{ get{ return BardImmune || m_IsDeadPet; } }
		public virtual bool Uncalmable{ get{ return BardImmune || m_IsDeadPet; } }
		public virtual bool AreaPeaceImmune { get { return BardImmune || m_IsDeadPet; } }

		public virtual bool BleedImmune{ get{ return false; } }
		public virtual double BonusPetDamageScalar{ get{ return 1.0; } }

		public virtual bool DeathAdderCharmable{ get{ return false; } }

		//TODO: Find the pub 31 tweaks to the DispelDifficulty and apply them of course.
		public virtual double DispelDifficulty{ get{ return 0.0; } } // at this skill level we dispel 50% chance
		public virtual double DispelFocus{ get{ return 20.0; } } // at difficulty - focus we have 0%, at difficulty + focus we have 100%
		public virtual bool DisplayWeight{ get{ return Backpack is StrongBackpack; } }

		#region Breath ability, like dragon fire breath
		private DateTime m_NextBreathTime;

		// Must be overriden in subclass to enable
		public virtual bool HasBreath{ get{ return false; } }

		// Base damage given is: CurrentHitPoints * BreathDamageScalar
		public virtual double BreathDamageScalar{ get{ return /*(Core.AOS ?*/ 0.10 /* : 0.05)*/; } } // mod by Dies Irae

		// Min/max seconds until next breath
		public virtual double BreathMinDelay{ get{ return 10.0 + 5.0; } }
		public virtual double BreathMaxDelay{ get{ return 15.0 + 5.0; } }

		// Creature stops moving for 1.0 seconds while breathing
		public virtual double BreathStallTime{ get{ return 1.0; } }

		// Effect is sent 1.3 seconds after BreathAngerSound and BreathAngerAnimation is played
		public virtual double BreathEffectDelay{ get{ return 1.3; } }

		// Damage is given 1.0 seconds after effect is sent
		public virtual double BreathDamageDelay{ get{ return 1.0; } }

        public virtual int BreathRange { get { return (int)( RangePerception / 2 ); } }

		// Damage types
		public virtual int BreathPhysicalDamage{ get{ return 0; } }
		public virtual int BreathFireDamage{ get{ return 100; } }
		public virtual int BreathColdDamage{ get{ return 0; } }
		public virtual int BreathPoisonDamage{ get{ return 0; } }
		public virtual int BreathEnergyDamage{ get{ return 0; } }
		
		// Is immune to breath damages
		public virtual bool BreathImmune{ get{ return false; } }

		// Effect details and sound
		public virtual int BreathEffectItemID{ get{ return 0x36D4; } }
		public virtual int BreathEffectSpeed{ get{ return 5; } }
		public virtual int BreathEffectDuration{ get{ return 0; } }
		public virtual bool BreathEffectExplodes{ get{ return false; } }
		public virtual bool BreathEffectFixedDir{ get{ return false; } }
		public virtual int BreathEffectHue{ get{ return 0; } }
		public virtual int BreathEffectRenderMode{ get{ return 0; } }

		public virtual int BreathEffectSound{ get{ return 0x227; } }

		// Anger sound/animations
		public virtual int BreathAngerSound{ get{ return GetAngerSound(); } }
		public virtual int BreathAngerAnimation{ get{ return 12; } }

		public virtual void BreathStart( Mobile target )
		{
			BreathStallMovement();
			BreathPlayAngerSound();
			BreathPlayAngerAnimation();

			this.Direction = this.GetDirectionTo( target );

			Timer.DelayCall( TimeSpan.FromSeconds( BreathEffectDelay ), new TimerStateCallback( BreathEffect_Callback ), target );
		}

		public virtual void BreathStallMovement()
		{
			if ( m_AI != null )
				m_AI.NextMove = DateTime.Now + TimeSpan.FromSeconds( BreathStallTime );
		}

		public virtual void BreathPlayAngerSound()
		{
			PlaySound( BreathAngerSound );
		}

		public virtual void BreathPlayAngerAnimation()
		{
			Animate( BreathAngerAnimation, 5, 1, true, false, 0 );
		}

		public virtual void BreathEffect_Callback( object state )
		{
			Mobile target = (Mobile)state;

			if ( !target.Alive || !CanBeHarmful( target ) )
				return;

			BreathPlayEffectSound();
			BreathPlayEffect( target );

			Timer.DelayCall( TimeSpan.FromSeconds( BreathDamageDelay ), new TimerStateCallback( BreathDamage_Callback ), target );
		}

		public virtual void BreathPlayEffectSound()
		{
			PlaySound( BreathEffectSound );
		}

		public virtual void BreathPlayEffect( Mobile target )
		{
			Effects.SendMovingEffect( this, target, BreathEffectItemID,
				BreathEffectSpeed, BreathEffectDuration, BreathEffectFixedDir,
				BreathEffectExplodes, BreathEffectHue, BreathEffectRenderMode );
		}

		public virtual void BreathDamage_Callback( object state )
		{
			Mobile target = (Mobile)state;
			
			if ( target is BaseCreature && ((BaseCreature)target).BreathImmune )
				return;

			if ( CanBeHarmful( target ) )
			{
				DoHarmful( target );
				BreathDealDamage( target );
			}
		}

		public virtual void BreathDealDamage( Mobile target )
		{
			int physDamage = BreathPhysicalDamage;
			int fireDamage = BreathFireDamage;
			int coldDamage = BreathColdDamage;
			int poisDamage = BreathPoisonDamage;
			int nrgyDamage = BreathEnergyDamage;

            /*
			if( Evasion.CheckSpellEvasion( target ) ) 
				return;
            */

			if ( physDamage == 0 && fireDamage == 0 && coldDamage == 0 && poisDamage == 0 && nrgyDamage == 0 )
			{ // Unresistable damage even in AOS
				target.Damage( BreathComputeDamage(), this );
			}
			else
			{
				AOS.Damage( target, this, BreathComputeDamage(), physDamage, fireDamage, coldDamage, poisDamage, nrgyDamage );
			}
		}

		public virtual int BreathComputeDamage()
		{
			int damage = (int)(Hits * BreathDamageScalar);

			#region Modifica by Dies Irae
			if ( IsParagon )
				damage = (int)(damage / XmlParagon.GetHitsBuff( this ));
			//	damage = (int)(damage / Paragon.HitsBuff);

            damage = Math.Min( damage, 90 );
			#endregion

			return damage;
		}
		#endregion

		#region Spill Acid
		public void SpillAcid( int Amount )
		{
			SpillAcid( null, Amount );
		}
		public void SpillAcid( Mobile target, int Amount )
		{
			if ( (target != null && target.Map == null) || this.Map == null )
				return;

			for ( int i = 0; i < Amount; ++i )
			{
				Point3D loc = this.Location;
				Map map = this.Map;
				Item acid = NewHarmfulItem();
			
				if ( target != null && target.Map != null && Amount == 1 )
				{
					loc = target.Location;
					map = target.Map;
				} else
				{
					bool validLocation = false;
					for ( int j = 0; !validLocation && j < 10; ++j )
					{
						loc = new Point3D( 
							loc.X+(Utility.Random(0,3)-2), 
							loc.Y+(Utility.Random(0,3)-2), 
							loc.Z );
						loc.Z = map.GetAverageZ( loc.X, loc.Y );
						validLocation = map.CanFit( loc, 16, false, false ) ;
					}
				}
				acid.MoveToWorld( loc, map );
			}
		}

		/* 
			Solen Style, override me for other mobiles/items: 
			kappa+acidslime, grizzles+whatever, etc. 
		*/
		public virtual Item NewHarmfulItem()
		{
			return new PoolOfAcid( TimeSpan.FromSeconds(10), /* 30, 30 */ 10, 15 ); // mod by Dies Irae
		}
		#endregion

		#region Flee!!!
		private DateTime m_EndFlee;

		public DateTime EndFleeTime
		{
			get{ return m_EndFlee; }
			set{ m_EndFlee = value; }
		}

		public virtual void StopFlee()
		{
			m_EndFlee = DateTime.MinValue;
		}

		public virtual bool CheckFlee()
		{
			if ( m_EndFlee == DateTime.MinValue )
				return false;

			if ( DateTime.Now >= m_EndFlee )
			{
				StopFlee();
				return false;
			}

			return true;
		}

		public virtual void BeginFlee( TimeSpan maxDuration )
		{
			m_EndFlee = DateTime.Now + maxDuration;
		}
		#endregion

		public BaseAI AIObject{ get{ return m_AI; } }

		public const int MaxOwners = 5;

		public virtual OppositionGroup OppositionGroup
		{
			get{ return null; }
		}

		#region Friends
		public List<Mobile> Friends { get { return m_Friends; } }

		public virtual bool AllowNewPetFriend
		{
			get{ return ( m_Friends == null || m_Friends.Count < 5 ); }
		}

		public virtual bool IsPetFriend( Mobile m )
		{
			return ( m_Friends != null && m_Friends.Contains( m ) );
		}

		public virtual void AddPetFriend( Mobile m )
		{
			if ( m_Friends == null )
				m_Friends = new List<Mobile>();

			m_Friends.Add( m );
		}

		public virtual void RemovePetFriend( Mobile m )
		{
			if ( m_Friends != null )
				m_Friends.Remove( m );
		}

		public virtual bool IsFriend( Mobile m )
		{
			OppositionGroup g = this.OppositionGroup;

			if ( g != null && g.IsEnemy( this, m ) )
				return false;

			if ( !(m is BaseCreature) )
				return false;

			BaseCreature c = (BaseCreature)m;

			return ( m_iTeam == c.m_iTeam && ( (m_bSummoned || m_bControlled) == (c.m_bSummoned || c.m_bControlled) )/* && c.Combatant != this */);
		}
		#endregion

		#region Allegiance
		public virtual Ethics.Ethic EthicAllegiance { get { return null; } }

		public enum Allegiance
		{
			None,
			Ally,
			Enemy
		}

		public virtual Allegiance GetFactionAllegiance( Mobile mob )
		{
			if ( mob == null || mob.Map != Faction.Facet || FactionAllegiance == null )
				return Allegiance.None;

			Faction fac = Faction.Find( mob, true );

			if ( fac == null )
				return Allegiance.None;

			return ( fac == FactionAllegiance ? Allegiance.Ally : Allegiance.Enemy );
		}

		public virtual Allegiance GetEthicAllegiance( Mobile mob )
		{
			if ( mob == null || mob.Map != Faction.Facet || EthicAllegiance == null )
				return Allegiance.None;

			Ethics.Ethic ethic = Ethics.Ethic.Find( mob, true );

			if ( ethic == null )
				return Allegiance.None;

			return ( ethic == EthicAllegiance ? Allegiance.Ally : Allegiance.Enemy );
		}
		#endregion

		public virtual bool IsEnemy( Mobile m )
		{
            #region mod by Dies Irae
            if( m == null || m is BaseVendor || m is PlayerVendor || m is BaseHealer )
                return false;
            #endregion

			OppositionGroup g = this.OppositionGroup;

			if ( g != null && g.IsEnemy( this, m ) )
				return true;

			if ( m is BaseGuard )
				return false;

			if ( GetFactionAllegiance( m ) == Allegiance.Ally )
				return false;

			Ethics.Ethic ourEthic = EthicAllegiance;
			Ethics.Player pl = Ethics.Player.Find( m, true );

			if ( pl != null && pl.IsShielded && ( ourEthic == null || ourEthic == pl.Ethic ) )
				return false;

			if ( !(m is BaseCreature) || m is Server.Engines.Quests.Haven.MilitiaFighter )
				return true;

			if( TransformationSpellHelper.UnderTransformation( m, typeof( EtherealVoyageSpell ) ) )
				return false;

			if ( m is PlayerMobile && ( (PlayerMobile)m ).HonorActive )
				return false;
			
			BaseCreature c = (BaseCreature)m;
			
			#region mod by Magius(CHE)
			/* summend mob creatures will no longer considered as enemies*/
			var hismasterisplayer = (c.GetMaster() as PlayerMobile) !=null ;
			var mymasterisplayer = (GetMaster() as PlayerMobile) !=null ;

			return ( m_iTeam != c.m_iTeam || ( hismasterisplayer != mymasterisplayer) /* || c.Combatant == this*/ );
			#endregion
		}

		public override string ApplyNameSuffix( string suffix )
		{
			if ( IsParagon )
			{
				#region modifica by Dies Irae
                /*
                if ( suffix.Length == 0 )
					suffix = "(Paragon)";
				else
					suffix = String.Concat( suffix, " (Paragon)" );
                */
			    suffix = XmlParagon.HandleNameSuffix( this, suffix );
                #endregion
			}

			#region modifica by Dies Irae
			if( this is IBurningCreature )
				suffix = CreatureBurningSystem.HandleNameSuffix( this, suffix );

		    if( Mastery != MasteryLevel.Normal )
		        suffix = MasterySystem.Core.HandleNameSuffix( this, suffix );

		    if ( Controlled && Commandable )
			{
			    string status = "(tame)";

				if ( Summoned )
				    status = "(summoned)";
				else if ( IsBonded && !HideBondedStatus )
				    status = "(bonded)";

                suffix = suffix.Length == 0 ? status : String.Concat( suffix, String.Format( " {0}", status ) );

                if( m_ControlOrder == OrderType.Guard )
                    suffix = suffix.Length == 0 ? "(guarding)" : String.Concat( suffix, " (guarding)" );

                if( this is BaseHireling )
                    suffix = suffix.Length == 0 ? "(hired)" : String.Concat( suffix, " (hired)" );

                if( IsHitched )
                {
                    string hitched = ( IsBonded && !HideBondedStatus ) ? "(hitched and bonded)" : "(hitched)";

                    suffix = suffix.Length == 0 ? hitched : String.Concat( suffix, String.Format( " {0}", hitched ) );
                }

                if( IsPregnant )
                    suffix = suffix.Length == 0 ? "(pregnant)" : String.Concat( suffix, " (pregnant)" );
			}

            if( BardPacified )
                suffix = suffix.Length == 0 ? "(pacified)" : String.Concat( suffix, " (pacified)" );
		    #endregion

            return base.ApplyNameSuffix( suffix );
		}

		public virtual bool CheckControlChance( Mobile m )
		{
			if ( GetControlChance( m ) > Utility.RandomDouble() )
			{
				Loyalty += 1;
				return true;
			}

			PlaySound( GetAngerSound() );

			if ( Body.IsAnimal )
				Animate( 10, 5, 1, true, false, 0 );
			else if ( Body.IsMonster )
				Animate( 18, 5, 1, true, false, 0 );

		    PublicOverheadMessage( MessageType.Regular, MessageHue, !Core.AOS, "* The creature refused to obey its Master *" ); // mod by Dies Irae

		    Loyalty -= 3;
			return false;
		}

		public virtual bool CanBeControlledBy( Mobile m )
		{
			return ( GetControlChance( m ) > 0.0 );
		}

		public double GetControlChance( Mobile m )
		{
			return GetControlChance( m, false );
		}

		public virtual double GetControlChance( Mobile m, bool useBaseSkill )
		{
			if ( m_dMinTameSkill <= 29.1 || m_bSummoned || m.AccessLevel >= AccessLevel.GameMaster )
				return 1.0;

			double dMinTameSkill = m_dMinTameSkill;

			if ( dMinTameSkill > -24.9 && Server.SkillHandlers.AnimalTaming.CheckMastery( m, this ) )
				dMinTameSkill = -24.9;

			int taming = (int)((useBaseSkill ? m.Skills[SkillName.AnimalTaming].Base : m.Skills[SkillName.AnimalTaming].Value ) * 10);
			int lore = (int)((useBaseSkill ? m.Skills[SkillName.AnimalLore].Base : m.Skills[SkillName.AnimalLore].Value )* 10);
			int bonus = 0, chance = 700;

			if( Core.ML )
			{
				int SkillBonus = taming - (int)(dMinTameSkill * 10);
				int LoreBonus = lore - (int)(dMinTameSkill * 10);

				int SkillMod = 6, LoreMod = 6;

				if( SkillBonus < 0 )
					SkillMod = 28;
				if( LoreBonus < 0 )
					LoreMod = 14;

				SkillBonus *= SkillMod;
				LoreBonus *= LoreMod;

				bonus = (SkillBonus + LoreBonus ) / 2;
			}	
			else
			{
				int difficulty = (int)(dMinTameSkill * 10);
				int weighted = ((taming * 4) + lore) / 5;
				bonus = weighted - difficulty;

				if ( bonus <= 0 )
					bonus *= 14;
				else
					bonus *= 6;
			}

			chance += bonus;

			if ( chance >= 0 && chance < 200 )
				chance = 200;
			else if ( chance > 990 )
				chance = 990;

			chance -= (MaxLoyalty - m_Loyalty) * 10;

			return ( (double)chance / 1000 );
		}

		private static Type[] m_AnimateDeadTypes = new Type[]
			{
				typeof( MoundOfMaggots ), typeof( HellSteed ), typeof( SkeletalMount ),
				typeof( WailingBanshee ), typeof( Wraith ), typeof( SkeletalDragon ),
				typeof( LichLord ), typeof( FleshGolem ), typeof( Lich ),
				typeof( SkeletalKnight ), typeof( BoneKnight ), typeof( Mummy ),
				typeof( SkeletalMage ), typeof( BoneMagi ), typeof( PatchworkSkeleton )
			};

		public virtual bool IsAnimatedDead
		{
			get
			{
				if ( !Summoned )
					return false;

				Type type = this.GetType();

				bool contains = false;

				for ( int i = 0; !contains && i < m_AnimateDeadTypes.Length; ++i )
					contains = ( type == m_AnimateDeadTypes[i] );

				return contains;
			}
		}

		public virtual bool IsNecroFamiliar
		{
			get
			{
				if ( !Summoned )
					return false;

				if ( m_ControlMaster != null && SummonFamiliarSpell.Table.Contains( m_ControlMaster ) )
					return SummonFamiliarSpell.Table[ m_ControlMaster ] == this;

				return false;
			}
		}

		public override void Damage( int amount, Mobile from )
		{
			int oldHits = this.Hits;

			if ( Core.AOS && !this.Summoned && this.Controlled && 0.2 > Utility.RandomDouble() )
				amount = (int)(amount * BonusPetDamageScalar);

			if ( Spells.Necromancy.EvilOmenSpell.CheckEffect( this ) )
				amount = (int)(amount * 1.25);

			Mobile oath = Spells.Necromancy.BloodOathSpell.GetBloodOath( from );

			if ( oath == this )
			{
				amount = (int)(amount * 1.1);
				from.Damage( amount, from );
            }

            #region third crown necromancer mod by Dies Irae
            //if( DarkOmenSpell.CheckEffect( this ) )
            //    amount = (int)( amount * 1.25 );

            Mobile oldOath = BloodConjunctionSpell.GetBloodOath( from );

            if( oldOath == this )
            {
                amount = (int)( amount * 1.1 );
                from.Damage( amount, from );
            }
            #endregion

			base.Damage( amount, from );

			if ( SubdueBeforeTame && !Controlled )
			{
				if ( (oldHits > (this.HitsMax / 10)) && (this.Hits <= (this.HitsMax / 10)) )
					PublicOverheadMessage( MessageType.Regular, 0x3B2, false, "* The creature has been beaten into subjugation! *" );
			}
		}

		public virtual bool DeleteCorpseOnDeath
		{
			get
			{
				return !Core.AOS && ( m_bSummoned || ( !Controlled && KilledByGuard ) ); // mod by Dies Irae
			}
		}

		public override void SetLocation( Point3D newLocation, bool isTeleport )
		{
			base.SetLocation( newLocation, isTeleport );

			if ( isTeleport && m_AI != null )
				m_AI.OnTeleported();
		}

		public override void OnBeforeSpawn( Point3D location, Map m )
		{
			#region modifica by Dies Irae
			if ( XmlParagon.CheckConvert( this, location, m ) )
                IsParagon = true;

            if( SupportSpeech )
                Midgard.Engines.TalkingVendors.Core.InitializeTalkingMobile( this );

            VerifyLayersConfliction();
			#endregion

			base.OnBeforeSpawn( location, m );

            #region faxx genetics : Spawned animals are already adult
            Age = 10 * AgingConstant;
            Level = m_OSILevel;
            #endregion
        }

		public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
		{
			if ( !Alive || IsDeadPet )
				return ApplyPoisonResult.Immune;

			if ( Spells.Necromancy.EvilOmenSpell.CheckEffect( this ) )
				poison = PoisonImpl.IncreaseLevel( poison );

            #region third crown necromancer mod by Dies Irae
            //if( DarkOmenSpell.CheckEffect( this ) )
            //    poison = PoisonImpl.IncreaseLevel( poison );
            #endregion

            ApplyPoisonResult result = base.ApplyPoison( from, poison );

			if ( from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer )
				(PoisonTimer as PoisonImpl.PoisonTimer).From = from;

			return result;
		}

		public override bool CheckPoisonImmunity( Mobile from, Poison poison )
		{
			if ( base.CheckPoisonImmunity( from, poison ) )
				return true;

			Poison p = this.PoisonImmune;

			#region Mondain's Legacy mod
			return ( p != null && p.RealLevel >= poison.RealLevel );
			#endregion
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Loyalty
		{
			get
			{
				return m_Loyalty;
			}
			set
            {
                #region mod by Dies Irae
                int oldValue = m_Loyalty;

                if( oldValue != value )
                {
                    m_Loyalty = Math.Min( Math.Max( value, 0 ), MaxLoyalty );

                    OnLoyaltyChanged( oldValue );
                }
                // m_Loyalty = Math.Min( Math.Max( value, 0 ), MaxLoyalty );
                #endregion
            }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WayPoint CurrentWayPoint 
		{
			get
			{
				return m_CurrentWayPoint;
			}
			set
			{
				m_CurrentWayPoint = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public IPoint2D TargetLocation
		{
			get
			{
				return m_TargetLocation;
			}
			set
			{
				m_TargetLocation = value;
			}
		}

		public virtual Mobile ConstantFocus{ get{ return null; } }

		public virtual bool DisallowAllMoves
		{
			get
			{
				return false;
			}
		}

		public virtual bool InitialInnocent
		{
			get
			{
				return false;
			}
		}

		public virtual bool AlwaysMurderer
		{
			get
			{
				return false;
			}
		}

		public virtual bool AlwaysAttackable
		{
			get
			{
				return false;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DamageMin{ get{ return m_DamageMin; } set{ m_DamageMin = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public virtual int DamageMax{ get{ return m_DamageMax; } set{ m_DamageMax = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override int HitsMax
		{
			get
			{
				if ( m_HitsMax > 0 )
				{
					int value = m_HitsMax + GetStatOffset( StatType.Str );
					if (Controlled)
						value += ( Str * StrToHits ) / 100;

					if( value < 1 )
						value = 1;
					else if( value > 65000 )
						value = 65000;

					return value;
				}

				return ( ( Str * StrToHits ) / 100 ) + GetStatOffset( StatType.Str ); // mod by Faxx : genetics
				// return Str;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitsMaxSeed
		{
			get{ return m_HitsMax; }
			set{ m_HitsMax = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int StamMax
		{
			get
			{
				if ( m_StamMax > 0 )
				{
					int value = m_StamMax + GetStatOffset( StatType.Dex );

					if (Controlled)
						value += ( ( Dex * DexToStam ) / 100 );

					if( value < 1 )
						value = 1;
					else if( value > 65000 )
						value = 65000;

					return value;
				}

				return ( ( Dex * DexToStam ) / 100 ) + GetStatOffset( StatType.Dex ); // mod by Faxx : genetics
				// return Dex;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StamMaxSeed
		{
			get{ return m_StamMax; }
			set{ m_StamMax = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ManaMax
		{
			get
			{
				if ( m_ManaMax > 0 )
				{
					int value = m_ManaMax + GetStatOffset( StatType.Int );
					if (Controlled)
						value += (( Int*IntToMana ) / 100);

					if( value < 1 )
						value = 1;
					else if( value > 65000 )
						value = 65000;

					return value;
				}

				return ( ( Int * IntToMana ) ) / 100 + GetStatOffset( StatType.Int ); // mod by Faxx : genetics
				// return Int;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int ManaMaxSeed
		{
			get{ return m_ManaMax; }
			set{ m_ManaMax = value; }
		}

		public virtual bool CanOpenDoors
		{
			get
			{
				return this.Body.IsHuman;//return !this.Body.IsAnimal && !this.Body.IsSea;//edit by Arlas
			}
		}

		public virtual bool CanMoveOverObstacles
		{
			get
			{
				return Core.AOS || this.Body.IsMonster;
			}
		}

		public virtual bool CanDestroyObstacles
		{
			get
			{
				// to enable breaking of furniture, 'return CanMoveOverObstacles;'
				return false;
			}
		}

		public void Unpacify()
		{
			BardEndTime = DateTime.Now;
			BardPacified = false;
		}

		private HonorContext m_ReceivedHonorContext;

		public HonorContext ReceivedHonorContext{ get{ return m_ReceivedHonorContext; } set{ m_ReceivedHonorContext = value; } }

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			#region modifica by Dies Irae
			// if ( BardPacified && (HitsMax - Hits) * 0.001 > Utility.RandomDouble() )
            if( BardPacified && ( ( HitsMax - Hits ) / (double)HitsMax ) * 0.50 > Utility.RandomDouble() )
            {
                PublicOverheadMessage( MessageType.Emote, 0x7E8, true, "*looks very very very furious to be pacified*" );
                Unpacify();
            }
			#endregion

            #region genetics by Faxx
            // Damaging can cause fetus death
            if( Utility.Random( 100 ) < amount )
            {
                if( m_Fetus != null && !m_Fetus.Deleted )
                {
                    m_Fetus.Delete();
                    m_Fetus = null;
                    DebugSay( ":_(" );
                }
            }
            #endregion

			int disruptThreshold;
			//NPCs can use bandages too!
			if( !Core.AOS )
				disruptThreshold = 0;
			else if( from != null && from.Player )
				disruptThreshold = 18;
			else
				disruptThreshold = 25;

			if( amount > disruptThreshold )
			{
				BandageContext c = BandageContext.GetContext( this );

				if( c != null )
					c.Slip();
			}

			if( Confidence.IsRegenerating( this ) )
				Confidence.StopRegenerating( this );

			WeightOverloading.FatigueOnDamage( this, amount );

			InhumanSpeech speechType = this.SpeechType;


			if ( speechType != null && !willKill )
				speechType.OnDamage( this, amount );

			if ( m_ReceivedHonorContext != null )
				m_ReceivedHonorContext.OnTargetDamaged( from, amount );

			if ( willKill && from is PlayerMobile )
				Timer.DelayCall( TimeSpan.FromSeconds( 10 ), new TimerCallback( ((PlayerMobile) from).RecoverAmmo ) );

			base.OnDamage( amount, from, willKill );
		}

		public virtual void OnDamagedBySpell( Mobile from )
		{
		}

		#region Alter[...]Damage From/To

		public virtual void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
		}

		public virtual void AlterDamageScalarTo( Mobile target, ref double scalar )
		{
		}

		public virtual void AlterSpellDamageFrom( Mobile from, ref int damage )
		{
		}

		public virtual void AlterSpellDamageTo( Mobile to, ref int damage )
		{
		}

		public virtual void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
		}

		public virtual void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
		}
		#endregion


		public virtual void CheckReflect( Mobile caster, ref bool reflect )
		{
		}

		public virtual void OnCarve( Mobile from, Corpse corpse )
		{
			OnCarve( from, corpse, null );
		}

		public virtual void OnCarve( Mobile from, Corpse corpse, Item with )
		{
			int feathers = Feathers;
			int wool = Wool;
			int meat = Meat;
			int hides = Hides;
			int scales = Scales;

            bool isSpecialType = CarveHelper.IsSpecialCarveTarget( GetType() ); // mod by Dies Irae

            if( ( feathers == 0 && wool == 0 && meat == 0 && hides == 0 && scales == 0 && !isSpecialType ) || Summoned || IsBonded ) // mod by Dies Irae
			{
				from.SendLocalizedMessage( 500485 ); // You see nothing useful to carve from the corpse.
			}
			else
			{
				if( Core.ML && from.Race == Race.Human )
				{
					hides = (int)Math.Ceiling( hides * 1.1 );	//10% Bonus Only applies to Hides, Ore & Logs
				}

				if ( corpse.Map == Map.Felucca )
				{
					feathers *= 2;
					wool *= 2;
					hides *= 2;
				}

				new Blood( 0x122D ).MoveToWorld( corpse.Location, corpse.Map );

				if ( feathers != 0 )
				{
//					corpse.DropItem( new Feather( feathers ) );
					corpse.DropItem( MakeInstanceOwner( new Feather( feathers ), from ) );
					from.SendLocalizedMessage( 500479 ); // You pluck the bird. The feathers are now on the corpse.
				}

				if ( wool != 0 )
				{
//					corpse.DropItem( new Wool( wool ) );
					corpse.DropItem( MakeInstanceOwner( new Wool( wool ), from ) );
					from.SendLocalizedMessage( 500483 ); // You shear it, and the wool is now on the corpse.
				}

				if ( meat != 0 )
				{
					if ( MeatType == MeatType.Ribs )
						corpse.DropItem( MakeInstanceOwner( new RawRibs( meat ), from ) ); // corpse.DropItem( new RawRibs( meat ) );
					else if ( MeatType == MeatType.Bird )
						corpse.DropItem( MakeInstanceOwner( new RawBird( meat ), from ) ); // corpse.DropItem( new RawBird( meat ) );
					else if ( MeatType == MeatType.LambLeg )
						corpse.DropItem( MakeInstanceOwner( new RawLambLeg( meat ), from ) ); // corpse.DropItem( new RawLambLeg( meat ) );

					from.SendLocalizedMessage( 500467 ); // You carve some meat, which remains on the corpse.
				}

				#region Mondain's Legacy
				if ( hides != 0 && with is ButchersWarCleaver )
				{
					Item leather = null;
					
					if ( HideType == HideType.Regular )
						leather = new Leather( hides );
					else if ( HideType == HideType.Spined )
						leather = new SpinedLeather( hides );
					else if ( HideType == HideType.Horned )
						leather = new HornedLeather( hides );
					else if ( HideType == HideType.Barbed )
						leather = new BarbedLeather( hides );
						
					if ( leather != null )
					{
						if ( !from.PlaceInBackpack( leather ) )
						{						
							leather.MoveToWorld( from.Location, from.Map );
							from.SendLocalizedMessage( 1077182 ); // Your backpack is too full, and it falls to the ground.	
						}	
						else
							from.SendLocalizedMessage( 1073555 ); // You skin it and place the cut-up hides in your backpack.		
					}							
				}
				#endregion
				else if ( hides != 0 )
				{
					if ( HideType == HideType.Regular )
						corpse.DropItem( MakeInstanceOwner( new Hides( hides ), from ) ); // corpse.DropItem( new Hides( hides ) );
                    if( !CarveHelper.CarverHelperEnabled )
                    {
                        if( HideType == HideType.Spined )
                            corpse.DropItem( MakeInstanceOwner( new SpinedHides( hides ), from ) ); // corpse.DropItem( new SpinedHides( hides ) );
                        else if( HideType == HideType.Horned )
                            corpse.DropItem( MakeInstanceOwner( new HornedHides( hides ), from ) ); // corpse.DropItem( new HornedHides( hides ) );
                        else if( HideType == HideType.Barbed )
                            corpse.DropItem( MakeInstanceOwner( new BarbedHides( hides ), from ) ); // corpse.DropItem( new BarbedHides( hides ) );
					}

				    if ( HideType == HideType.Regular )
                        from.SendLocalizedMessage( 500471 ); // You skin it, and the hides are now in the corpse.
                }

                #region mod by Dies Irae
                if( isSpecialType && CarveHelper.CarverHelperEnabled )
                {
                    if( !CarveHelper.HandleLeatherCarve( from, this, corpse, with ) )
                    {
                        if( feathers == 0 && wool == 0 && meat == 0 && hides == 0 && scales == 0 )
                            from.SendLocalizedMessage( 500485 ); // You see nothing useful to carve from the corpse.
                    }
                    else
                        from.SendMessage( "Wonderful! You have skinned some rare leather!" );
                }
                #endregion

				if ( scales != 0 )
				{
					ScaleType sc = this.ScaleType;

					switch ( sc )
					{
						case ScaleType.Red:		corpse.DropItem( MakeInstanceOwner( new RedScales( scales ), from ) );  break; // corpse.DropItem( new RedScales( scales ) ); break;
						case ScaleType.Yellow:	corpse.DropItem( MakeInstanceOwner( new YellowScales( scales ), from ) ); break; // corpse.DropItem( new YellowScales( scales ) ); break;
						case ScaleType.Black:	corpse.DropItem( MakeInstanceOwner( new BlackScales( scales ), from ) ); break; // corpse.DropItem( new BlackScales( scales ) ); break;
						case ScaleType.Green:	corpse.DropItem( MakeInstanceOwner( new GreenScales( scales ), from ) ); break; // corpse.DropItem( new GreenScales( scales ) ); break;
						case ScaleType.White:	corpse.DropItem( MakeInstanceOwner( new WhiteScales( scales ), from ) ); break; // corpse.DropItem( new WhiteScales( scales ) ); break;
						case ScaleType.Blue:	corpse.DropItem( MakeInstanceOwner( new BlueScales( scales ), from ) ); break; // corpse.DropItem( new BlueScales( scales ) ); break;
						case ScaleType.All:
						{
							corpse.DropItem( MakeInstanceOwner( new RedScales( scales ), from ) ); // corpse.DropItem( new RedScales( scales ) );
							corpse.DropItem( MakeInstanceOwner( new YellowScales( scales ), from ) ); // corpse.DropItem( new YellowScales( scales ) );
							corpse.DropItem( MakeInstanceOwner( new BlackScales( scales ), from ) ); // corpse.DropItem( new BlackScales( scales ) );
							corpse.DropItem( MakeInstanceOwner( new GreenScales( scales ), from ) ); // corpse.DropItem( new GreenScales( scales ) );
							corpse.DropItem( MakeInstanceOwner( new WhiteScales( scales ), from ) ); // corpse.DropItem( new WhiteScales( scales ) );
							corpse.DropItem( MakeInstanceOwner( new BlueScales( scales ), from ) ); // corpse.DropItem( new BlueScales( scales ) );
							break;
						}
					}

					from.SendMessage( "You cut away some scales, but they remain on the corpse." );
				}

				corpse.Carved = true;

				#region Instance Corpses by Dies Irae
				if ( m_CorpseFinalizerTimer == null )
                    m_CorpseFinalizerTimer = Timer.DelayCall( TimeSpan.FromMinutes( 3.0 ), new TimerStateCallback( FinalizeInstanceCorpse ), corpse );
				#endregion

				if ( corpse.IsCriminalAction( from ) )
					from.CriminalAction( true );
			}
		}

		public const int DefaultRangePerception = 16;
		public const int OldRangePerception = 10;

		public BaseCreature(AIType ai,
			FightMode mode,
			int iRangePerception,
			int iRangeFight,
			double dActiveSpeed, 
			double dPassiveSpeed)
		{
		    CurrentFightMode = FightMode.None;
		    Level = 1;
		    Generation = 1;
		    InitMidgardCreature(); // mod by Dies irae

			if ( iRangePerception == OldRangePerception )
				iRangePerception = DefaultRangePerception;

			m_Loyalty = MaxLoyalty; // Wonderfully Happy

			m_CurrentAI = ai;
			m_DefaultAI = ai;

			m_iRangePerception = iRangePerception;
			m_iRangeFight = iRangeFight;
			
			m_FightMode = mode;

			m_iTeam = 0;

			SpeedInfo.GetSpeeds( this, ref dActiveSpeed, ref dPassiveSpeed );

			m_dActiveSpeed = dActiveSpeed;
			m_dPassiveSpeed = dPassiveSpeed;
			m_dCurrentSpeed = dPassiveSpeed;

			m_bDebugAI = false;

			m_arSpellAttack = new List<Type>();
			m_arSpellDefense = new List<Type>();

			m_bControlled = false;
			m_ControlMaster = null;
			m_ControlTarget = null;
			m_ControlOrder = OrderType.None;

			m_bTamable = false;

			m_Owners = new List<Mobile>();

			m_NextReacquireTime = DateTime.Now + ReacquireDelay;

			ChangeAIType(AI);

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnConstruct( this );

			GenerateLoot( true );
		}

		public BaseCreature( Serial serial ) : base( serial )
		{
		    CurrentFightMode = FightMode.None;
		    Level = 1;
		    Generation = 1;
		    m_arSpellAttack = new List<Type>();
			m_arSpellDefense = new List<Type>();

			m_bDebugAI = false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			#region modifica by Dies Irae
			writer.Write( (int) 30 ); // version
			#endregion

			writer.Write( (int)m_CurrentAI );
			writer.Write( (int)m_DefaultAI );

			writer.Write( (int)m_iRangePerception );
			writer.Write( (int)m_iRangeFight );

			writer.Write( (int)m_iTeam );

			writer.Write( (double)m_dActiveSpeed );
			writer.Write( (double)m_dPassiveSpeed );
			writer.Write( (double)m_dCurrentSpeed );

			writer.Write( (int) m_pHome.X );
			writer.Write( (int) m_pHome.Y );
			writer.Write( (int) m_pHome.Z );

			// Version 1
			writer.Write( (int) m_iRangeHome );

			int i=0;

			writer.Write( (int) m_arSpellAttack.Count );
			for ( i=0; i< m_arSpellAttack.Count; i++ )
			{
				writer.Write( m_arSpellAttack[i].ToString() );
			}

			writer.Write( (int) m_arSpellDefense.Count );
			for ( i=0; i< m_arSpellDefense.Count; i++ )
			{
				writer.Write( m_arSpellDefense[i].ToString() );
			}

			// Version 2
			writer.Write( (int) m_FightMode );

			writer.Write( (bool) m_bControlled );
			writer.Write( (Mobile) m_ControlMaster );
			writer.Write( (Mobile) m_ControlTarget );
			writer.Write( (Point3D) m_ControlDest );
			writer.Write( (int) m_ControlOrder );
			writer.Write( (double) m_dMinTameSkill );
			// Removed in version 9
			//writer.Write( (double) m_dMaxTameSkill );
			writer.Write( (bool) m_bTamable );
			writer.Write( (bool) m_bSummoned );

			if ( m_bSummoned )
				writer.WriteDeltaTime( m_SummonEnd );

			writer.Write( (int) m_iControlSlots );

			// Version 3
			writer.Write( (int)m_Loyalty );

			// Version 4 
			writer.Write( m_CurrentWayPoint );

			// Verison 5
			writer.Write( m_SummonMaster );

			// Version 6
			writer.Write( (int) m_HitsMax );
			writer.Write( (int) m_StamMax );
			writer.Write( (int) m_ManaMax );
			writer.Write( (int) m_DamageMin );
			writer.Write( (int) m_DamageMax );

			// Version 7
			writer.Write( (int) m_PhysicalResistance );
			writer.Write( (int) m_PhysicalDamage );

			writer.Write( (int) m_FireResistance );
			writer.Write( (int) m_FireDamage );

			writer.Write( (int) m_ColdResistance );
			writer.Write( (int) m_ColdDamage );

			writer.Write( (int) m_PoisonResistance );
			writer.Write( (int) m_PoisonDamage );

			writer.Write( (int) m_EnergyResistance );
			writer.Write( (int) m_EnergyDamage );

			// Version 8
			writer.Write( m_Owners, true );

			// Version 10
			writer.Write( (bool) m_IsDeadPet );
			writer.Write( (bool) m_IsBonded );
			writer.Write( (DateTime) m_BondingBegin );
			writer.Write( (DateTime) m_OwnerAbandonTime );

			// Version 11
			writer.Write( (bool) m_HasGeneratedLoot );

			// Version 12
			writer.Write( (bool) m_Paragon );

			// Version 13
			writer.Write( (bool) ( m_Friends != null && m_Friends.Count > 0 ) );

			if ( m_Friends != null && m_Friends.Count > 0 )
				writer.Write( m_Friends, true );

			// Version 14
			writer.Write( (bool)m_RemoveIfUntamed );
			writer.Write( (int)m_RemoveStep );

			#region modifica by Dies Irae per il pet system
			// Version 17
			writer.Write( (bool) false );
			writer.Write( (int) AbilityPoints );
			writer.Write( (int) Exp );
			writer.Write( (int) NextLevel );
			writer.Write( (int) Level );
			writer.Write( (int) MaxLevel );
			writer.Write( (bool) AllowMating );
			writer.Write( (int) Generation );
			writer.Write( (DateTime) MatingDelay );
			writer.Write( (int) RoarAttack );
			writer.Write( (int) PetPoisonAttack );
			writer.Write( (int) FireBreathAttack );

			// version 18
			writer.Write( (int) RarityLevel );

			// version 20
			writer.Write( (bool)m_IsHitched );
			writer.Write( HitchingPost );

			// version 21
			writer.Write( (bool)m_IsShrinked );
			writer.Write( ShrinkItem );
			#endregion

			#region Mondain's Legacy version 22
			writer.Write( (bool)m_Allured );
			#endregion

			#region versione 23 , modifica by Dies Irae : talking vendor system
			writer.Write( (bool)m_SpeechEnabled );
			writer.Write( (bool)m_CanHaveFriends );

			bool writeList = ( m_TalkingFriends != null && m_TalkingFriends.Count > 0 );
			writer.Write( (bool)( writeList ) );

			if( writeList )
			{
				writer.Write( (int)m_TalkingFriends.Count );

				foreach( KeyValuePair<Mobile, int> kvp in m_TalkingFriends )
				{
					writer.Write( (Mobile)kvp.Key );
					writer.Write( (int)kvp.Value );
				}
			}
			#endregion

			#region version 24: faxx genetics
			writer.Write( m_BirthDate );
			DNA.Serialize( writer );

			if( m_Fetus != null )
			{
				writer.Write( true );
				writer.Write( m_Fetus );
				writer.Write( DeliveryTime );
			}
			else
				writer.Write( false );

			writer.Write( m_Experience );
			#endregion

			#region 25 EnemyMasterySystem
			writer.Write( (int) m_Mastery );
			#endregion

			// Version 26
			writer.Write( DeleteTimeLeft );

			// Version 27
			writer.Write( OriginalName );

			// Version 28
			writer.Write( HasDamageDice );
			if( HasDamageDice )
				DamageDice.Serialize( writer );

			// Version 29
			writer.Write( HideBondedStatus );
			writer.Write( HideFrozenStatus );
			writer.Write( HideInvulStatus );

			//version 30
			writer.Write( HideTag );
		}

		private static double[] m_StandardActiveSpeeds = new double[]
			{
				0.175, 0.1, 0.15, 0.2, 0.25, 0.3, 0.4, 0.5, 0.6, 0.8
			};

		private static double[] m_StandardPassiveSpeeds = new double[]
			{
				0.350, 0.2, 0.4, 0.5, 0.6, 0.8, 1.0, 1.2, 1.6, 2.0
			};

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_CurrentAI = (AIType)reader.ReadInt();
			m_DefaultAI = (AIType)reader.ReadInt();

			m_iRangePerception = reader.ReadInt();
			m_iRangeFight = reader.ReadInt();

			m_iTeam = reader.ReadInt();

			m_dActiveSpeed = reader.ReadDouble();
			m_dPassiveSpeed = reader.ReadDouble();
			m_dCurrentSpeed = reader.ReadDouble();

			if ( m_iRangePerception == OldRangePerception )
				m_iRangePerception = DefaultRangePerception;

			m_pHome.X = reader.ReadInt();
			m_pHome.Y = reader.ReadInt();
			m_pHome.Z = reader.ReadInt();

			if ( version >= 1 )
			{
				m_iRangeHome = reader.ReadInt();

				int i, iCount;
				
				iCount = reader.ReadInt();
				for ( i=0; i< iCount; i++ )
				{
					string str = reader.ReadString();
					Type type = Type.GetType( str );

					if ( type != null )
					{
						m_arSpellAttack.Add( type );
					}
				}

				iCount = reader.ReadInt();
				for ( i=0; i< iCount; i++ )
				{
					string str = reader.ReadString();
					Type type = Type.GetType( str );

					if ( type != null )
					{
						m_arSpellDefense.Add( type );
					}			
				}
			}
			else
			{
				m_iRangeHome = 0;
			}

			if ( version >= 2 )
			{
				m_FightMode = ( FightMode )reader.ReadInt();

				m_bControlled = reader.ReadBool();
				m_ControlMaster = reader.ReadMobile();
				m_ControlTarget = reader.ReadMobile();
				m_ControlDest = reader.ReadPoint3D();
				m_ControlOrder = (OrderType) reader.ReadInt();

				m_dMinTameSkill = reader.ReadDouble();

				if ( version < 9 )
					reader.ReadDouble();

				m_bTamable = reader.ReadBool();
				m_bSummoned = reader.ReadBool();

				if ( m_bSummoned )
				{
					m_SummonEnd = reader.ReadDeltaTime();
					new UnsummonTimer( m_ControlMaster, this, m_SummonEnd - DateTime.Now ).Start();
				}

				m_iControlSlots = reader.ReadInt();
			}
			else
			{
				m_FightMode = FightMode.Closest;

				m_bControlled = false;
				m_ControlMaster = null;
				m_ControlTarget = null;
				m_ControlOrder = OrderType.None;
			}

			if ( version >= 3 )
				m_Loyalty = reader.ReadInt();
			else
				m_Loyalty = MaxLoyalty; // Wonderfully Happy

			if ( version >= 4 )
				m_CurrentWayPoint = reader.ReadItem() as WayPoint;

			if ( version >= 5 )
				m_SummonMaster = reader.ReadMobile();

			if ( version >= 6 )
			{
				m_HitsMax = reader.ReadInt();
				m_StamMax = reader.ReadInt();
				m_ManaMax = reader.ReadInt();
				m_DamageMin = reader.ReadInt();
				m_DamageMax = reader.ReadInt();
			}

			if ( version >= 7 )
			{
				m_PhysicalResistance = reader.ReadInt();
				m_PhysicalDamage = reader.ReadInt();

				m_FireResistance = reader.ReadInt();
				m_FireDamage = reader.ReadInt();

				m_ColdResistance = reader.ReadInt();
				m_ColdDamage = reader.ReadInt();

				m_PoisonResistance = reader.ReadInt();
				m_PoisonDamage = reader.ReadInt();

				m_EnergyResistance = reader.ReadInt();
				m_EnergyDamage = reader.ReadInt();
			}

			if ( version >= 8 )
				m_Owners = reader.ReadStrongMobileList();
			else
				m_Owners = new List<Mobile>();

			if ( version >= 10 )
			{
				m_IsDeadPet = reader.ReadBool();
				m_IsBonded = reader.ReadBool();
				m_BondingBegin = reader.ReadDateTime();
				m_OwnerAbandonTime = reader.ReadDateTime();
			}

			if ( version >= 11 )
				m_HasGeneratedLoot = reader.ReadBool();
			else
				m_HasGeneratedLoot = true;

			if ( version >= 12 )
				m_Paragon = reader.ReadBool();
			else
				m_Paragon = false;

			if ( version >= 13 && reader.ReadBool() )
				m_Friends = reader.ReadStrongMobileList();
			else if ( version < 13 && m_ControlOrder >= OrderType.Unfriend )
				++m_ControlOrder;

			if ( version < 16 && Loyalty != MaxLoyalty )
				Loyalty *= 10;

			double activeSpeed = m_dActiveSpeed;
			double passiveSpeed = m_dPassiveSpeed;

			SpeedInfo.GetSpeeds( this, ref activeSpeed, ref passiveSpeed );

			bool isStandardActive = false;
			for ( int i = 0; !isStandardActive && i < m_StandardActiveSpeeds.Length; ++i )
				isStandardActive = ( m_dActiveSpeed == m_StandardActiveSpeeds[i] );

			bool isStandardPassive = false;
			for ( int i = 0; !isStandardPassive && i < m_StandardPassiveSpeeds.Length; ++i )
				isStandardPassive = ( m_dPassiveSpeed == m_StandardPassiveSpeeds[i] );

			if ( isStandardActive && m_dCurrentSpeed == m_dActiveSpeed )
				m_dCurrentSpeed = activeSpeed;
			else if ( isStandardPassive && m_dCurrentSpeed == m_dPassiveSpeed )
				m_dCurrentSpeed = passiveSpeed;

			if ( isStandardActive && !m_Paragon )
				m_dActiveSpeed = activeSpeed;

			if ( isStandardPassive && !m_Paragon )
				m_dPassiveSpeed = passiveSpeed;

			if ( version >= 14 )
			{
				m_RemoveIfUntamed = reader.ReadBool();
				m_RemoveStep = reader.ReadInt();
			}

			#region modifica by Dies Irae per il pet system
			if ( version >= 17 )
			{
			    reader.ReadBool();
				AbilityPoints = reader.ReadInt();
				Exp = reader.ReadInt();
				NextLevel = reader.ReadInt();
				Level = reader.ReadInt();
				MaxLevel = reader.ReadInt();
				AllowMating = reader.ReadBool();
				
				if ( version == 18 )			
					reader.ReadBool();
				
				Generation = reader.ReadInt();
				MatingDelay = reader.ReadDateTime();
				
				if ( version == 18 )
				{
					for( int i = 0; i < 18; i++ )
						reader.ReadInt();
					
					for( int i = 0; i < 19; i++ )
						reader.ReadBool();
				}
				
				RoarAttack = reader.ReadInt();
				PetPoisonAttack = reader.ReadInt();
				FireBreathAttack = reader.ReadInt();
			}
			
			if( version >= 18 )
			{
				RarityLevel = (PetRarity.Rarity)reader.ReadInt();
			}

            if( version >= 20 )
            {
                m_IsHitched = reader.ReadBool();
                HitchingPost = (Midgard.Items.HitchingPostComponent)reader.ReadItem();
            }

            if( version >= 21 )
            {
                m_IsShrinked = reader.ReadBool();
                ShrinkItem = reader.ReadItem();
            }
			#endregion

            #region Mondain's Legacy version 15
            if( version >= 22 )
                m_Allured = reader.ReadBool();
            #endregion

            #region modifica by Dies Irae : talking vendor system
            if( version >= 23 )
            {
                m_SpeechEnabled = reader.ReadBool();
                m_CanHaveFriends = reader.ReadBool();

                m_TalkingFriends = new Dictionary<Mobile, int>();

                if( reader.ReadBool() )
                {
                    int talkers = reader.ReadInt();

                    for( int i = 0; i < talkers; i++ )
                    {
                        Mobile talker = reader.ReadMobile();
                        int karma = reader.ReadInt();

                        m_TalkingFriends.Add( talker, karma );
                    }
                }
            }

            if( SupportSpeech )
                TalkingVendors.Core.InitializeTalkingMobile( this );
            #endregion

            #region faxx genetics
            if( version >= 24 )
            {
                m_BirthDate = reader.ReadDateTime();

                DNA = Activator.CreateInstance( DNAType, new object[] { reader, this } ) as AnimalDNA;

                if( reader.ReadBool() )
                {
                    m_Fetus = (BaseCreature)reader.ReadMobile();
                    DeliveryTime = reader.ReadDateTime();
                }
                else
                    m_Fetus = null;

                m_Experience = reader.ReadInt();

                CheckAgeTimer();
            }
            #endregion

            #region EnemyMastery
            if( version >= 25 )
		        m_Mastery = (MasteryLevel)reader.ReadInt();
            #endregion

			TimeSpan deleteTime = TimeSpan.Zero;
				
			if ( version >= 26 )
				deleteTime = reader.ReadTimeSpan();

			if ( deleteTime > TimeSpan.Zero || LastOwner != null && !Controlled && !IsStabled )
			{
				if ( deleteTime == TimeSpan.Zero )
					deleteTime = TimeSpan.FromDays( 3.0 );
					
				m_DeleteTimer = new DeleteTimer( this, deleteTime );
				m_DeleteTimer.Start();
			}

            if( version >= 27 )
            {
                OriginalName = reader.ReadString();
            }

            if( version >= 28 )
            {
                bool hasDamageDice = reader.ReadBool();
                if( hasDamageDice )
                    DamageDice = DiceRoll.Deserialize( reader );
            }

            if( version >= 29 )
            {
                HideBondedStatus = reader.ReadBool();
                HideFrozenStatus = reader.ReadBool();
                HideInvulStatus =  reader.ReadBool();
            }

            if( version >= 30 )
                HideTag =  reader.ReadBool();

		    if( version <= 14 && m_Paragon && Hue == 0x31 )
			{
				Hue = Paragon.Hue; //Paragon hue fixed, should now be 0x501.
			}

			CheckStatTimers();

			ChangeAIType(m_CurrentAI);

			AddFollowers();

			if ( IsAnimatedDead )
				Spells.Necromancy.AnimateDeadSpell.Register( m_SummonMaster, this );

            #region mod by Dies Irae
            if( PetLeveling.AosPetSystemEnabled )
                Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( PetUtility.VerifyPoints_Callback ), this );
			#endregion
        }

        public virtual bool IsHumanInTown()
        {
            #region mod by Dies Irae
            GuardedRegion reg = (GuardedRegion)Region.GetRegion( typeof( GuardedRegion ) );

            return Body.IsHuman && reg != null && !reg.Disabled;

            // return ( Body.IsHuman && Region.IsPartOf( typeof( Regions.GuardedRegion ) ) );
            #endregion
        }

		public virtual bool CheckGold( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
				return OnGoldGiven( from, (Gold)dropped );

			return false;
		}

		public virtual bool OnGoldGiven( Mobile from, Gold dropped )
		{
			if ( CheckTeachingMatch( from ) )
			{
				if ( Teach( m_Teaching, from, dropped.Amount, true ) )
				{
					dropped.Delete();
					return true;
				}
			}
			else if ( IsHumanInTown() )
			{
				Direction = GetDirectionTo( from );

				int oldSpeechHue = this.SpeechHue;

				this.SpeechHue = 0x23F;
				SayTo( from, "Thou art giving me gold?" );

				if ( dropped.Amount >= 400 )
					SayTo( from, "'Tis a noble gift." );
				else
					SayTo( from, "Money is always welcome." );

				this.SpeechHue = 0x3B2;
				SayTo( from, 501548 ); // I thank thee.

				this.SpeechHue = oldSpeechHue;

				dropped.Delete();
				return true;
			}

			return false;
		}

		public override bool ShouldCheckStatTimers{ get{ return false; } }

		#region Food
		private static Type[] m_Eggs = new Type[]
			{
				typeof( FriedEggs ), typeof( Eggs )
			};

		private static Type[] m_Fish = new Type[]
			{
				typeof( FishSteak ), typeof( RawFishSteak )
			};

		private static Type[] m_GrainsAndHay = new Type[]
			{
				typeof( BreadLoaf ), typeof( FrenchBread ), typeof( SheafOfHay )
			};

		private static Type[] m_Meat = new Type[]
			{
				/* Cooked */
				typeof( Bacon ), typeof( CookedBird ), typeof( Sausage ),
				typeof( Ham ), typeof( Ribs ), typeof( LambLeg ),
				typeof( ChickenLeg ),

				/* Uncooked */
				typeof( RawBird ), typeof( RawRibs ), typeof( RawLambLeg ),
				typeof( RawChickenLeg ),

				/* Body Parts */
				typeof( Head ), typeof( LeftArm ), typeof( LeftLeg ),
				typeof( Torso ), typeof( RightArm ), typeof( RightLeg )
			};

		private static Type[] m_FruitsAndVegies = new Type[]
			{
				typeof( HoneydewMelon ), typeof( YellowGourd ), typeof( GreenGourd ),
				typeof( Banana ), typeof( Bananas ), typeof( Lemon ), typeof( Lime ),
				typeof( Dates ), typeof( Grapes ), typeof( Peach ), typeof( Pear ),
				typeof( Apple ), typeof( Watermelon ), typeof( Squash ),
				typeof( Cantaloupe ), typeof( Carrot ), typeof( Cabbage ),
				typeof( Onion ), typeof( Lettuce ), typeof( Pumpkin )
			};

		private static Type[] m_Gold = new Type[]
			{
				// white wyrms eat gold..
				typeof( Gold )
			};

		public virtual bool CheckFoodPreference( Item f )
		{
			if ( CheckFoodPreference( f, FoodType.Eggs, m_Eggs ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Fish, m_Fish ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.GrainsAndHay, m_GrainsAndHay ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Meat, m_Meat ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.FruitsAndVegies, m_FruitsAndVegies ) )
				return true;

			if ( CheckFoodPreference( f, FoodType.Gold, m_Gold ) )
				return true;

			return false;
		}

		public virtual bool CheckFoodPreference( Item fed, FoodType type, Type[] types )
		{
			if ( (FavoriteFood & type) == 0 )
				return false;

			Type fedType = fed.GetType();
			bool contains = false;

			for ( int i = 0; !contains && i < types.Length; ++i )
				contains = ( fedType == types[i] );

			return contains;
		}

		public virtual bool CheckFeed( Mobile from, Item dropped )
		{
			if ( !IsDeadPet && Controlled && (ControlMaster == from || IsPetFriend( from )) && (dropped is Food || dropped is Gold || dropped is CookableFood || dropped is Head || dropped is LeftArm || dropped is LeftLeg || dropped is Torso || dropped is RightArm || dropped is RightLeg) )
			{
				Item f = dropped;

				if ( CheckFoodPreference( f ) )
				{
					int amount = f.Amount;

					if ( amount > 0 )
					{
						bool happier = false;

						int stamGain;

						if ( f is Gold )
							stamGain = amount - 50;
						else
							stamGain = (amount * 15) - 50;

						if ( stamGain > 0 )
							Stam += stamGain;

						if ( Core.SE )
						{
							if ( m_Loyalty < MaxLoyalty )
							{
								m_Loyalty = MaxLoyalty;
								happier = true;
							}
						}
						else
						{
							for ( int i = 0; i < amount; ++i )
							{
								if ( m_Loyalty < MaxLoyalty  && 0.5 >= Utility.RandomDouble() )
								{
									m_Loyalty += 10;
									happier = true;
								}
							}
						}

						if ( happier )
							SayTo( from, 502060 ); // Your pet looks happier.

						if ( Body.IsAnimal )
							Animate( 3, 5, 1, true, false, 0 );
						else if ( Body.IsMonster )
							Animate( 17, 5, 1, true, false, 0 );

						if ( IsBondable && !IsBonded )
						{
							Mobile master = m_ControlMaster;

							if ( master != null && master == from )	//So friends can't start the bonding process
							{
								// mod by Dies Irae: ora per bondare checka su Value e non su Base
								// if ( m_dMinTameSkill <= 29.1 || master.Skills[SkillName.AnimalTaming].Base >= m_dMinTameSkill || GetControlChance( master, true ) >= 1.0 )
								if ( m_dMinTameSkill <= 29.1 || master.Skills[SkillName.AnimalTaming].Value >= m_dMinTameSkill || GetControlChance( master, false ) >= 1.0 )
								{
									if ( BondingBegin == DateTime.MinValue )
									{
										BondingBegin = DateTime.Now;
									}
									else if ( (BondingBegin + BondingDelay) <= DateTime.Now )
									{
										IsBonded = true;
										BondingBegin = DateTime.MinValue;
										from.SendLocalizedMessage( 1049666 ); // Your pet has bonded with you!
									}
								}
								else if( Core.ML )
								{
									from.SendLocalizedMessage( 1075268 ); // Your pet cannot form a bond with you until your animal taming ability has risen.
								}
							}
						}

						dropped.Delete();
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		public virtual bool OverrideBondingReqs()
		{
			return false;
		}

		public virtual bool CanAngerOnTame{ get{ return false; } }

		#region OnAction[...]
		public virtual void OnActionWander()
		{
		}

		public virtual void OnActionCombat()
		{
		}

		public virtual void OnActionGuard()
		{
		}

		public virtual void OnActionFlee()
		{
		}

		public virtual void OnActionInteract()
		{
		}

		public virtual void OnActionBackoff()
		{
		}
		#endregion

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
            if( CheckFeed( from, dropped ) )
                return true;
            else if( CheckGold( from, dropped ) )
                return true;
            #region mod by Dies Irae
            else if( CheckBook( from, dropped ) )
                return true;
            #endregion

            return base.OnDragDrop( from, dropped );
		}

		protected virtual BaseAI ForcedAI { get { return null; } }

		public  void ChangeAIType( AIType NewAI )
		{
			if ( m_AI != null )
				m_AI.m_Timer.Stop();

			if( ForcedAI != null )
			{
				m_AI = ForcedAI;
				return;
			}

			m_AI = null;

			switch ( NewAI )
			{
				case AIType.AI_Melee:
					m_AI = new MeleeAI(this);
					break;
				case AIType.AI_Animal:
					m_AI = new AnimalAI(this);
					break;
				case AIType.AI_Berserk:
					m_AI = new BerserkAI(this);
					break;
				case AIType.AI_Archer:
					m_AI = new ArcherAI(this);
					break;
				case AIType.AI_Healer:
					m_AI = new HealerAI(this);
					break;
				case AIType.AI_Vendor:
					m_AI = new VendorAI(this);
					break;
				case AIType.AI_Mage:
					m_AI = new MageAI(this);
					break;
				case AIType.AI_Predator:
					//m_AI = new PredatorAI(this);
					m_AI = new MeleeAI(this);
					break;
				case AIType.AI_Thief:
					m_AI = new ThiefAI(this);
					break;
				#region modifica by Dies Irae aggiunte le AI AI_Necromage e AI_Ninja
				case AIType.AI_Necromage:
					m_AI = new NecromageAI( this );
					break;
				case AIType.AI_Ninja:
					m_AI = new NinjaAI( this );
					break;
				case AIType.AI_BossMelee:
					m_AI = new BossMeleeAI( this );
					break;
				case AIType.AI_DarkFather:
					m_AI = new DarkFatherAI( this );
					break;
				case AIType.AI_BoneDemon:
					m_AI = new BoneDemonAI( this );
					break;
				case AIType.AI_FlyingMelee:
					m_AI = new FlyingMeleeAI( this );
					break;
				case AIType.AI_FlyingAnimal:
					m_AI = new FlyingAnimalAI( this );
					break;
				case AIType.AI_FlyingBerserk:
					m_AI = new FlyingBerserkAI( this );
					break;
				case AIType.AI_FlyingArcher:
					m_AI = new FlyingArcherAI( this );
					break;
				case AIType.AI_FlyingHealer:
					m_AI = new FlyingHealerAI( this );
					break;
				case AIType.AI_FlyingVendor:
					m_AI = new FlyingVendorAI( this );
					break;
				case AIType.AI_FlyingMage:
					m_AI = new FlyingMageAI( this );
					break;
				case AIType.AI_FlyingPredator:
					m_AI = new FlyingMeleeAI( this );
					break;
				case AIType.AI_FlyingThief:
					m_AI = new FlyingThiefAI( this );
					break;
				#endregion
				#region modifica by Arlas
				case AIType.AI_MageHealer:
					m_AI = new MageHealerAI( this );
					break;
				case AIType.AI_MageSummoner:
					m_AI = new MageSummonerAI( this );
					break;
				case AIType.AI_MageDebuffer:
					m_AI = new MageDebufferAI( this );
					break;
				case AIType.AI_MageDamager:
					m_AI = new MageDamagerAI( this );
					break;
				case AIType.AI_MageBuffer:
					m_AI = new MageBufferAI( this );
					break;
				#endregion
			}
		}

		public void ChangeAIToDefault()
		{
			ChangeAIType(m_DefaultAI);
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AIType AI
		{
			get
			{
				return m_CurrentAI;
			}
			set
			{
				m_CurrentAI = value;

				if (m_CurrentAI == AIType.AI_Use_Default)
				{
					m_CurrentAI = m_DefaultAI;
				}
				
				ChangeAIType(m_CurrentAI);
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Debug
		{
			get
			{
				return m_bDebugAI;
			}
			set
			{
				m_bDebugAI = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Team
		{
			get
			{
				return m_iTeam;
			}
			set
			{
				m_iTeam = value;
				
				OnTeamChange();
			}
		}

		public virtual void OnTeamChange()
		{
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile FocusMob
		{
			get
			{
				return m_FocusMob;
			}
			set
			{
				m_FocusMob = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public FightMode FightMode
		{
			get
			{
				return m_FightMode;
			}
			set
			{
				m_FightMode = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangePerception
		{
			get
			{
				return m_iRangePerception;
			}
			set
			{
				m_iRangePerception = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangeFight
		{
			get
			{
				return m_iRangeFight;
			}
			set
			{
				m_iRangeFight = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int RangeHome
		{
			get
			{
				return m_iRangeHome;
			}
			set
			{
				m_iRangeHome = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double ActiveSpeed
		{
			get
			{
				return m_dActiveSpeed;
			}
			set
			{
				m_dActiveSpeed = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double PassiveSpeed
		{
			get
			{
				return m_dPassiveSpeed;
			}
			set
			{
				m_dPassiveSpeed = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double CurrentSpeed
		{
			get
			{
				if ( m_TargetLocation != null )
					return 0.3;

				return m_dCurrentSpeed;
			}
			set
			{
				if ( m_dCurrentSpeed != value )
				{
					m_dCurrentSpeed = value;

					if (m_AI != null)
						m_AI.OnCurrentSpeedChanged();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Home
		{
			get
			{
				return m_pHome;
			}
			set
			{
				m_pHome = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Controlled
		{
			get
			{
				return m_bControlled;
			}
			set
			{
				if ( m_bControlled == value )
					return;

				m_bControlled = value;
				Delta( MobileDelta.Noto );

				InvalidateProperties();
			}
		}

		public override void RevealingAction()
		{
			Spells.Sixth.InvisibilitySpell.RemoveTimer( this );

			base.RevealingAction();
		}

		public void RemoveFollowers()
		{
			if ( m_ControlMaster != null )
			{
				m_ControlMaster.Followers -= ControlSlots;
				//if( m_ControlMaster is PlayerMobile ) //Magius(CHE) all can summon now!
				//{
					(/*(PlayerMobile)*/m_ControlMaster).AllFollowers.Remove( this );					
				//}
				if( m_ControlMaster is PlayerMobile )
				{
					if( ((PlayerMobile)m_ControlMaster).AutoStabled.Contains( this ) )
						((PlayerMobile)m_ControlMaster).AutoStabled.Remove( this );
				}
			}
			else if ( m_SummonMaster != null )
			{
				m_SummonMaster.Followers -= ControlSlots;
				//if( m_SummonMaster is PlayerMobile )  //Magius(CHE) all can summon now!
				//{
					(/*(PlayerMobile)*/m_SummonMaster).AllFollowers.Remove( this );
				//}
			}

			if ( m_ControlMaster != null && m_ControlMaster.Followers < 0 )
				m_ControlMaster.Followers = 0;

			if ( m_SummonMaster != null && m_SummonMaster.Followers < 0 )
				m_SummonMaster.Followers = 0;
		}

		public void AddFollowers()
		{
			if ( m_ControlMaster != null )
			{
				m_ControlMaster.Followers += ControlSlots;
				//if( m_ControlMaster is PlayerMobile )
				//{
					(/*(PlayerMobile)*/m_ControlMaster).AllFollowers.Add( this ); // Mod by Magius(CHE): Mob can controls other creatures.
				//}
			}
			else if ( m_SummonMaster != null )
			{
				m_SummonMaster.Followers += ControlSlots;
				//if( m_SummonMaster is PlayerMobile )
				//{
					(/*(PlayerMobile)*/m_SummonMaster).AllFollowers.Add( this );  // Mod by Magius(CHE): Mob can controls other creatures.
				//}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ControlMaster
		{
			get
			{
				return m_ControlMaster;
			}
			set
			{
				if ( m_ControlMaster == value || this == value )
					return;

				RemoveFollowers();
				m_ControlMaster = value;
				AddFollowers();
				if ( m_ControlMaster != null )
					StopDeleteTimer();

				Delta( MobileDelta.Noto );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile SummonMaster
		{
			get
			{
				return m_SummonMaster;
			}
			set
			{
				if ( m_SummonMaster == value || this == value )
					return;

				RemoveFollowers();
				m_SummonMaster = value;
				AddFollowers();

				Delta( MobileDelta.Noto );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile ControlTarget
		{
			get
			{
				return m_ControlTarget;
			}
			set
			{
				m_ControlTarget = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D ControlDest
		{
			get
			{
				return m_ControlDest;
			}
			set
			{
				m_ControlDest = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public OrderType ControlOrder
		{
			get
			{
				return m_ControlOrder;
			}
			set
			{
				m_ControlOrder = value;
				
				#region Mondain's Legacy
				if ( m_Allured )
				{
					if ( m_ControlOrder == OrderType.Release )
						Say( 502003 ); // Sorry, but no.
					else if ( m_ControlOrder != OrderType.None )
						Say( 1079120 ); // Very well.
				}
				#endregion

				if ( m_AI != null )
					m_AI.OnCurrentOrderChanged();

				InvalidateProperties();

				if ( m_ControlMaster != null )
					m_ControlMaster.InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardProvoked
		{
			get
			{
				return m_bBardProvoked;
			}
			set
			{
				m_bBardProvoked = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BardPacified
		{
			get
			{
				return m_bBardPacified;
			}
			set
			{
				m_bBardPacified = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BardMaster
		{
			get
			{
				return m_bBardMaster;
			}
			set
			{
				m_bBardMaster = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile BardTarget
		{
			get
			{
				return m_bBardTarget;
			}
			set
			{
				m_bBardTarget = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime BardEndTime
		{
			get
			{
				return m_timeBardEnd;
			}
			set
			{
				m_timeBardEnd = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double MinTameSkill
		{
			get
			{
				return m_dMinTameSkill;
			}
			set
			{
				m_dMinTameSkill = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Tamable
		{
			get
			{
				return m_bTamable && !m_Paragon && !Blessed && !m_IsHitched; // mod By Dies Irae
			}
			set
			{
				m_bTamable = value;
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public bool Summoned
		{
			get
			{
				return m_bSummoned;
			}
			set
			{
				if ( m_bSummoned == value )
					return;

				m_NextReacquireTime = DateTime.Now;

				m_bSummoned = value;
				Delta( MobileDelta.Noto );

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.Administrator )]
		public int ControlSlots
		{
			get
			{
				return m_iControlSlots;
			}
			set
			{
				m_iControlSlots = value;
			}
		}

		public virtual bool NoHouseRestrictions{ get{ return false; } }
		public virtual bool IsHouseSummonable{ get{ return false; } }

		#region Corpse Resources
		public virtual int Feathers{ get{ return 0; } }
		public virtual int Wool{ get{ return 0; } }

		public virtual MeatType MeatType{ get{ return MeatType.Ribs; } }
		public virtual int Meat{ get{ return 0; } }

		public virtual int Hides{ get{ return 0; } }
		public virtual HideType HideType{ get{ return HideType.Regular; } }

		public virtual int Scales{ get{ return 0; } }
		public virtual ScaleType ScaleType{ get{ return ScaleType.Red; } }
		#endregion

		public virtual bool AutoDispel{ get{ return false; } }
		public virtual double AutoDispelChance{ get { return ((Core.SE) ? .10 : 0.1); } } //Mod by Magius(CHE)

		public virtual bool IsScaryToPets{ get{ return false; } }
		public virtual bool IsScaredOfScaryThings{ get{ return true; } }

		public virtual bool CanRummageCorpses{ get{ return false; } }

        #region mod by Dies Irae
        public enum DispelModes
        {
            OSI,
            MidgardThirdCrown
        }

        public virtual DispelModes DispelMode{ get{ return DispelModes.MidgardThirdCrown; } }
        #endregion

		public virtual void OnGotMeleeAttack( Mobile attacker )
		{
			if ( AutoDispel && attacker is BaseCreature && ((BaseCreature)attacker).IsDispellable && AutoDispelChance > Utility.RandomDouble() )
            {
                #region Mod by Magius(CHE): all dispel effect must work on the same way.
                if( DispelMode == DispelModes.MidgardThirdCrown )
                    CheckAndDispel( attacker );
                else
                    Dispel( attacker );
                #endregion
            }
		}

        #region Mod by Magius(CHE): all dispel effect must work on the same way.
        public virtual void CheckAndDispel( Mobile attacker )
        {
            double mageryvalue = Skills.Magery.Value;

            BaseCreature bc = attacker as BaseCreature;
            if( bc == null )
            {
                Dispel( attacker );
                return;
            }

            double dispelChance = ( 50.0 + ( ( 100 * ( mageryvalue - bc.DispelDifficulty ) ) / ( bc.DispelFocus * 2 ) ) ) / 100;
		if (dispelChance > 0.5)//troppo facile dispellare
			dispelChance = 0.5;

            if( dispelChance > Utility.RandomDouble() )
                Dispel( attacker );
            else
            {
                attacker.FixedEffect( 0x3779, 10, 20 );

                Mobile owner = bc.GetMaster();
                if( owner != null )
                    owner.SendLocalizedMessage( 1010084 ); // The creature resisted the attempt to dispel it!
            }
        }
		#endregion
		
		public virtual void Dispel( Mobile m )
		{
			Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
			Effects.PlaySound( m, m.Map, 0x201 );

			m.Delete();
		}

		public virtual bool DeleteOnRelease{ get{ return m_bSummoned; } }

		public virtual void OnGaveMeleeAttack( Mobile defender )
		{
			Poison p = HitPoison;
			
			if ( m_Paragon )
				p = PoisonImpl.IncreaseLevel( p );

			if ( p != null && HitPoisonChance >= Utility.RandomDouble() ) {
				defender.ApplyPoison( this, p );
				if ( this.Controlled )
					this.CheckSkill(SkillName.Poisoning, 0, this.Skills[SkillName.Poisoning].Cap);
			}

            if( AutoDispel && defender is BaseCreature && ( (BaseCreature)defender ).IsDispellable && AutoDispelChance > Utility.RandomDouble() )
            {
                #region Mod by Magius(CHE): all dispel effect must work on the same way.
                if( DispelMode == DispelModes.MidgardThirdCrown )
                    CheckAndDispel( defender );
                else
                    Dispel( defender );
                #endregion
            }
		}

		public override void OnAfterDelete()
		{
			if ( m_AI != null )
			{
				if ( m_AI.m_Timer != null )
					m_AI.m_Timer.Stop();

				m_AI = null;
			}
			
			if ( m_DeleteTimer != null )
			{
				m_DeleteTimer.Stop();
				m_DeleteTimer = null;
			}

			FocusMob = null;

			if ( IsAnimatedDead )
				Spells.Necromancy.AnimateDeadSpell.Unregister( m_SummonMaster, this );

			#region mod by Dies Irae
			if( IsAnimatedDead )
				Midgard.Engines.SpellSystem.NecropotenceSpell.Unregister( m_SummonMaster, this );
			#endregion

			base.OnAfterDelete();
		}

		public void DebugSay( string text )
		{
			if ( m_bDebugAI )
				this.PublicOverheadMessage( MessageType.Regular, 41, false, text );
		}

		public void DebugSay( string format, params object[] args )
		{
			if ( m_bDebugAI )
				this.PublicOverheadMessage( MessageType.Regular, 41, false, String.Format( format, args ) );
		}

		/* 
		 * This function can be overriden.. so a "Strongest" mobile, can have a different definition depending
		 * on who check for value
		 * -Could add a FightMode.Prefered
		 * 
		 */
		public virtual double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			if ( ( bPlayerOnly && m.Player ) ||  !bPlayerOnly )
			{
				switch( acqType )
				{
					case FightMode.Strongest : 
						return (m.Skills[SkillName.Tactics].Value + m.Str); //returns strongest mobile

					case FightMode.Weakest : 
						return -m.Hits; // returns weakest mobile

					default : 
						return -GetDistanceToSqrt( m ); // returns closest mobile
				}
			}
			else
			{
				return double.MinValue;
			}
		}

		// Turn, - for left, + for right
		// Basic for now, needs work
		public virtual void Turn(int iTurnSteps)
		{
			int v = (int)Direction;

			Direction = (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80));
		}

		public virtual void TurnInternal(int iTurnSteps)
		{
			int v = (int)Direction;

			SetDirection( (Direction)((((v & 0x7) + iTurnSteps) & 0x7) | (v & 0x80)) );
		}

		public bool IsHurt()
		{
			return ( Hits != HitsMax );
		}

		public double GetHomeDistance()
		{
			return GetDistanceToSqrt( m_pHome );
		}

		public virtual int GetTeamSize(int iRange)
		{
			int iCount = 0;

			foreach ( Mobile m in this.GetMobilesInRange( iRange ) )
			{
				if (m is BaseCreature)
				{
					if ( ((BaseCreature)m).Team == Team )
					{
						if ( !m.Deleted )
						{
							if ( m != this )
							{
								if ( CanSee( m ) )
								{
									iCount++;
								}
							}
						}
					}
				}
			}
			
			return iCount;
		}

		private class TameEntry : ContextMenuEntry
		{
			private BaseCreature m_Mobile;

			public TameEntry( Mobile from, BaseCreature creature ) : base( 6130, 6 )
			{
				m_Mobile = creature;

				Enabled = Enabled && ( from.Female ? creature.AllowFemaleTamer : creature.AllowMaleTamer );
			}

			public override void OnClick()
			{
				if ( !Owner.From.CheckAlive() )
					return;

				Owner.From.TargetLocked = true;
				SkillHandlers.AnimalTaming.DisableMessage = true;

				if ( Owner.From.UseSkill( SkillName.AnimalTaming ) )
					Owner.From.Target.Invoke( Owner.From, m_Mobile );

				SkillHandlers.AnimalTaming.DisableMessage = false;
				Owner.From.TargetLocked = false;
			}
		}

		#region Teaching
		public virtual bool CanTeach{ get{ return false; } }

		public virtual bool CheckTeach( SkillName skill, Mobile from )
		{
			if ( !CanTeach )
				return false;

			if( skill == SkillName.Stealth && from.Skills[SkillName.Hiding].Base < ((Core.SE) ? 50.0 : 80.0) )
				return false;

			if ( skill == SkillName.RemoveTrap && (from.Skills[SkillName.Lockpicking].Base < 50.0 || from.Skills[SkillName.DetectHidden].Base < 50.0) )
				return false;

			if ( !Core.AOS && (skill == SkillName.Focus || skill == SkillName.Chivalry || skill == SkillName.Necromancy) )
				return false;

			return true;
		}

		public enum TeachResult
		{
			Success,
			Failure,
			KnowsMoreThanMe,
			KnowsWhatIKnow,
			SkillNotRaisable,
			NotEnoughFreePoints
		}

		public virtual TeachResult CheckTeachSkills( SkillName skill, Mobile m, int maxPointsToLearn, ref int pointsToLearn, bool doTeach )
		{
			if ( !CheckTeach( skill, m ) || !m.CheckAlive() )
				return TeachResult.Failure;

			Skill ourSkill = Skills[skill];
			Skill theirSkill = m.Skills[skill];

			if ( ourSkill == null || theirSkill == null )
				return TeachResult.Failure;

			int baseToSet = ourSkill.BaseFixedPoint / 3;

			if ( baseToSet > 420 )
				baseToSet = 420;
			else if ( baseToSet < 200 )
				return TeachResult.Failure;

			if ( baseToSet > theirSkill.CapFixedPoint )
				baseToSet = theirSkill.CapFixedPoint;

			pointsToLearn = baseToSet - theirSkill.BaseFixedPoint;

			if ( maxPointsToLearn > 0 && pointsToLearn > maxPointsToLearn )
			{
				pointsToLearn = maxPointsToLearn;
				baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
			}

			if ( pointsToLearn < 0 )
				return TeachResult.KnowsMoreThanMe;

			if ( pointsToLearn == 0 )
				return TeachResult.KnowsWhatIKnow;

			if ( theirSkill.Lock != SkillLock.Up )
				return TeachResult.SkillNotRaisable;

			int freePoints = m.Skills.Cap - m.Skills.Total;
			int freeablePoints = 0;

			if ( freePoints < 0 )
				freePoints = 0;

			for ( int i = 0; (freePoints + freeablePoints) < pointsToLearn && i < m.Skills.Length; ++i )
			{
				Skill sk = m.Skills[i];

				if ( sk == theirSkill || sk.Lock != SkillLock.Down )
					continue;

				freeablePoints += sk.BaseFixedPoint;
			}

			if ( (freePoints + freeablePoints) == 0 )
				return TeachResult.NotEnoughFreePoints;

			if ( (freePoints + freeablePoints) < pointsToLearn )
			{
				pointsToLearn = freePoints + freeablePoints;
				baseToSet = theirSkill.BaseFixedPoint + pointsToLearn;
			}

			if ( doTeach )
			{
				int need = pointsToLearn - freePoints;

				for ( int i = 0; need > 0 && i < m.Skills.Length; ++i )
				{
					Skill sk = m.Skills[i];

					if ( sk == theirSkill || sk.Lock != SkillLock.Down )
						continue;

					if ( sk.BaseFixedPoint < need )
					{
						need -= sk.BaseFixedPoint;
						sk.BaseFixedPoint = 0;
					}
					else
					{
						sk.BaseFixedPoint -= need;
						need = 0;
					}
				}

				/* Sanity check */
				if ( baseToSet > theirSkill.CapFixedPoint || (m.Skills.Total - theirSkill.BaseFixedPoint + baseToSet) > m.Skills.Cap )
					return TeachResult.NotEnoughFreePoints;

				theirSkill.BaseFixedPoint = baseToSet;
			}

			return TeachResult.Success;
		}

		public virtual bool CheckTeachingMatch( Mobile m )
		{
			if ( m_Teaching == (SkillName)(-1) )
				return false;

			if ( m is PlayerMobile )
				return ( ((PlayerMobile)m).Learning == m_Teaching );

			return true;
		}

		private SkillName m_Teaching = (SkillName)(-1);

		public virtual bool Teach( SkillName skill, Mobile m, int maxPointsToLearn, bool doTeach )
		{
			int pointsToLearn = 0;
			TeachResult res = CheckTeachSkills( skill, m, maxPointsToLearn, ref pointsToLearn, doTeach );

			switch ( res )
			{
				case TeachResult.KnowsMoreThanMe:
				{
					Say( 501508 ); // I cannot teach thee, for thou knowest more than I!
					break;
				}
				case TeachResult.KnowsWhatIKnow:
				{
					Say( 501509 ); // I cannot teach thee, for thou knowest all I can teach!
					break;
				}
				case TeachResult.NotEnoughFreePoints:
				case TeachResult.SkillNotRaisable:
				{
					// Make sure this skill is marked to raise. If you are near the skill cap (700 points) you may need to lose some points in another skill first.
					m.SendLocalizedMessage( 501510, "", 0x22 );
					break;
				}
				case TeachResult.Success:
				{
					if ( doTeach )
					{
						Say( 501539 ); // Let me show thee something of how this is done.
						m.SendLocalizedMessage( 501540 ); // Your skill level increases.

						m_Teaching = (SkillName)(-1);

						if ( m is PlayerMobile )
							((PlayerMobile)m).Learning = (SkillName)(-1);
					}
					else
					{
						// I will teach thee all I know, if paid the amount in full.  The price is:
						Say( 1019077, AffixType.Append, String.Format( " {0}", pointsToLearn ), "" );
						Say( 1043108 ); // For less I shall teach thee less.

						m_Teaching = skill;

						if ( m is PlayerMobile )
							((PlayerMobile)m).Learning = skill;
					}

					return true;
				}
			}

			return false;
		}
		#endregion

		public override void AggressiveAction( Mobile aggressor, bool criminal )
		{
			base.AggressiveAction( aggressor, criminal );

			OrderType ct = m_ControlOrder;

			if ( m_AI != null )
			{
				if( !Core.ML || ( ct != OrderType.Follow && ct != OrderType.Stop ) )
				{
					m_AI.OnAggressiveAction( aggressor );
				}
				else
				{
					DebugSay( "I'm being attacked but my master told me not to fight." );
					Warmode = false;
					return;
				}
			}

			StopFlee();

			ForceReacquire();

			if ( !IsEnemy( aggressor ) )
			{
				Ethics.Player pl = Ethics.Player.Find( aggressor, true );

				if ( pl != null && pl.IsShielded )
					pl.FinishShield();
			}

			if ( aggressor.ChangingCombatant && (m_bControlled || m_bSummoned) && (ct == OrderType.Come || ( !Core.ML && ct == OrderType.Stay ) || ct == OrderType.Stop || ct == OrderType.None || ct == OrderType.Follow) )
			{
				ControlTarget = aggressor;
				ControlOrder = OrderType.Attack;
			}
			else if ( Combatant == null && !m_bBardPacified )
			{
				Warmode = true;
				Combatant = aggressor;
			}
		}

		public override bool OnMoveOver( Mobile m )
        {
            #region mod by Dies Irae: speech system
            if( IsHumanInTown() && m.Player && !m.Hidden && DateTime.Now > m_NextWarn + TimeSpan.FromSeconds( 5.0 ) )
            {
                m_NextWarn = DateTime.Now;

                string resp = GetPersonalSpace();
                if( !string.IsNullOrEmpty( resp ) )
                {
                    Say( resp );
                    AdjustAttitude( DiceRoll.Roll( "1d50" ) );
                }
            }
            #endregion
            
            if ( m is BaseCreature && !((BaseCreature)m).Controlled )
				return false;

			return base.OnMoveOver( m );
		}

		public virtual void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			#region mod by Dies Irae : taming system
            if( PetLeveling.AosPetSystemEnabled && from.Alive && Alive && Controlled && Summoned == false && PetUtility.IsLevelablePet( this ) )
                list.Add( new PetMenu( from, this ) );
			#endregion
		}

		public virtual bool CanDrop { get { return IsBonded; } }

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( m_AI != null && Commandable )
				m_AI.GetContextMenuEntries( from, list );

			if ( m_bTamable && !m_bControlled && from.Alive && !m_IsHitched ) // mod by Dies Irae
				list.Add( new TameEntry( from, this ) );

			AddCustomContextEntries( from, list );

			if ( CanTeach && from.Alive )
			{
				Skills ourSkills = this.Skills;
				Skills theirSkills = from.Skills;

				for ( int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i )
				{
					Skill skill = ourSkills[i];
					Skill theirSkill = theirSkills[i];

					if ( skill != null && theirSkill != null && skill.Base >= 60.0 && CheckTeach( skill.SkillName, from ) )
					{
						double toTeach = skill.Base / 3.0;

						if ( toTeach > 42.0 )
							toTeach = 42.0;

						list.Add( new TeachEntry( (SkillName)i, this, from, ( toTeach > theirSkill.Base ) ) );
					}
				}
			}
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && (speechType.Flags & IHSFlags.OnSpeech) != 0 && from.InRange( this, 3 ) )
				return true;

			return ( m_AI != null && m_AI.HandlesOnSpeech( from ) && from.InRange( this, m_iRangePerception ) );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null && speechType.OnSpeech( this, e.Mobile, e.Speech ) )
				e.Handled = true;
			else if ( !e.Handled && m_AI != null && e.Mobile.InRange( this, m_iRangePerception ) )
				m_AI.OnSpeech( e );
		}

		public override bool IsHarmfulCriminal( Mobile target )
		{
			if ( (Controlled && target == m_ControlMaster) || (Summoned && target == m_SummonMaster) )
				return false;

			if ( target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled )
				return false;

			if ( target is PlayerMobile && ((PlayerMobile)target).PermaFlags.Count > 0 )
				return false;
			
			#region Mod by Magius(CHE)
			/*Per far si che la funzione IsHarmfulCriminal usata da un Controllato funzioni con gli stessi criteri che si usano tra i player
			 * controllo qui che sia un player e invoko IsHarmfulCriminal del master */
            Mobile master = GetMaster();
            if( master != null )
                return master.IsHarmfulCriminal( target );
            else
                return base.IsHarmfulCriminal( target );
		    #endregion
		}

		public override void CriminalAction( bool message )
		{
			base.CriminalAction( message );

			if ( Controlled || Summoned )
			{
				if ( m_ControlMaster != null && m_ControlMaster.Player )
					m_ControlMaster.CriminalAction( false );
				else if ( m_SummonMaster != null && m_SummonMaster.Player )
					m_SummonMaster.CriminalAction( false );
			}
		}

		public override void DoHarmful( Mobile target, bool indirect )
		{
			base.DoHarmful( target, indirect );

			if ( target == this || target == m_ControlMaster || target == m_SummonMaster || (!Controlled && !Summoned) )
				return;

			List<AggressorInfo> list = this.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo ai = list[i];

				if ( ai.Attacker == target )
					return;
			}

			list = this.Aggressed;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo ai = list[i];

				if ( ai.Defender == target )
				{
					if ( m_ControlMaster != null && m_ControlMaster.Player && m_ControlMaster.CanBeHarmful( target, false ) )
						m_ControlMaster.DoHarmful( target, true );
					else if ( m_SummonMaster != null && m_SummonMaster.Player && m_SummonMaster.CanBeHarmful( target, false ) )
						m_SummonMaster.DoHarmful( target, true );

					return;
				}
			}
		}

		private static Mobile m_NoDupeGuards;

		public void ReleaseGuardDupeLock()
		{
			m_NoDupeGuards = null;
		}

		public void ReleaseGuardLock()
		{
			EndAction( typeof( GuardedRegion ) );
		}

		private DateTime m_IdleReleaseTime;

		public virtual bool CheckIdle()
		{
			if ( Combatant != null )
				return false; // in combat.. not idling

			if ( m_IdleReleaseTime > DateTime.MinValue )
			{
				// idling...

				if ( DateTime.Now >= m_IdleReleaseTime )
				{
					m_IdleReleaseTime = DateTime.MinValue;
					return false; // idle is over
				}

				return true; // still idling
			}

			if ( 95 > Utility.Random( 100 ) )
				return false; // not idling, but don't want to enter idle state

			m_IdleReleaseTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 15, 25 ) );

			if ( Body.IsHuman )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: Animate( 5, 5, 1, true,  true, 1 ); break;
					case 1: Animate( 6, 5, 1, true, false, 1 ); break;
				}	
			}
			else if ( Body.IsAnimal )
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: Animate(  3, 3, 1, true, false, 1 ); break;
					case 1: Animate(  9, 5, 1, true, false, 1 ); break;
					case 2: Animate( 10, 5, 1, true, false, 1 ); break;
				}
			}
			else if ( Body.IsMonster )
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: Animate( 17, 5, 1, true, false, 1 ); break;
					case 1: Animate( 18, 5, 1, true, false, 1 ); break;
				}
			}

			PlaySound( GetIdleSound() );
			return true; // entered idle state
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{
			Map map = this.Map;
			
			if ( PlayerRangeSensitive && m_AI != null && map != null && map.GetSector( this.Location ).Active )
				m_AI.Activate();
			
			base.OnLocationChange( oldLocation );
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( ReacquireOnMovement || m_Paragon || m_Mastery != MasteryLevel.Normal ) // mod by Dies Irae
				ForceReacquire();

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnMovement( this, m, oldLocation );

		    HandleMovementForSpeech( m, oldLocation ); // mod by Dies Irae

            /* Begin notice sound */
			if ( (!m.Hidden || m.AccessLevel == AccessLevel.Player) && m.Player && m_FightMode != FightMode.Aggressor && m_FightMode != FightMode.None && Combatant == null && !Controlled && !Summoned )
			{
				// If this creature defends itself but doesn't actively attack (animal) or
				// doesn't fight at all (vendor) then no notice sounds are played..
				// So, players are only notified of aggressive monsters

				// Monsters that are currently fighting are ignored

				// Controlled or summoned creatures are ignored

				if ( InRange( m.Location, 18 ) && !InRange( oldLocation, 18 ) )
				{
					if ( Body.IsMonster )
						Animate( 11, 5, 1, true, false, 1 );

					PlaySound( GetAngerSound() );
				}
			}
			/* End notice sound */

			if ( m_NoDupeGuards == m )
				return;

			if ( !Body.IsHuman || Kills >= 5 || AlwaysMurderer || AlwaysAttackable || m.Kills < 5 || !m.InRange( Location, 12 ) || !m.Alive )
				return;

			GuardedRegion guardedRegion = (GuardedRegion) this.Region.GetRegion( typeof( GuardedRegion ) );

			if ( guardedRegion != null )
			{
				if ( !guardedRegion.IsDisabled() && guardedRegion.IsGuardCandidate( m ) && BeginAction( typeof( GuardedRegion ) ) )
				{
					Say( 1013037 + Utility.Random( 16 ) );
					guardedRegion.CallGuards( this.Location );

					Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( ReleaseGuardLock ) );

					m_NoDupeGuards = m;
					Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ReleaseGuardDupeLock ) );
				}
			}
		}


		public void AddSpellAttack( Type type )
		{
			m_arSpellAttack.Add ( type );
		}

		public void AddSpellDefense( Type type )
		{
			m_arSpellDefense.Add ( type );
		}

		public Spell GetAttackSpellRandom()
		{
			if ( m_arSpellAttack.Count > 0 )
			{
				Type type = m_arSpellAttack[Utility.Random(m_arSpellAttack.Count)];

				object[] args = {this, null};
				return Activator.CreateInstance( type, args ) as Spell;
			}
			else
			{
				return null;
			}
		}

		public Spell GetDefenseSpellRandom()
		{
			if ( m_arSpellDefense.Count > 0 )
			{
				Type type = m_arSpellDefense[Utility.Random(m_arSpellDefense.Count)];

				object[] args = {this, null};
				return Activator.CreateInstance( type, args ) as Spell;
			}
			else
			{
				return null;
			}
		}

		public Spell GetSpellSpecific( Type type )
		{
			int i;

			for( i=0; i< m_arSpellAttack.Count; i++ )
			{
				if( m_arSpellAttack[i] == type )
				{
					object[] args = { this, null };
					return Activator.CreateInstance( type, args ) as Spell;
				}
			}

			for ( i=0; i< m_arSpellDefense.Count; i++ )
			{
				if ( m_arSpellDefense[i] == type )
				{
					object[] args = {this, null};
					return Activator.CreateInstance( type, args ) as Spell;
				}			
			}

			return null;
		}

		#region Set[...]

		public void SetDamage( int val )
		{
			m_DamageMin = val;
			m_DamageMax = val;
		}

		public void SetDamage( int min, int max )
		{
            #region genetics by Faxx
            if( IsGeneticCreature )
            { 
                SetDamageByDNA( min, max );
                return; 
            }
            #endregion

			m_DamageMin = min;
			m_DamageMax = max;
		}
		
		#region Mod by Magius(CHE): invariant set hits
		public void SetHitsInvariant( int val )
		{
			m_HitsMax = val;
			Hits = HitsMax;
		}
		#endregion
		
		public void SetHits( int val )
		{
			if ( /* val < 1000 && */ !Core.AOS )
				val = (val * 100) / 60;

			m_HitsMax = val;
			Hits = HitsMax;
		}

		public void SetHits( int min, int max )
		{
			if ( /* min < 1000 && */ !Core.AOS )
			{
				min = (min * 100) / 60;
				max = (max * 100) / 60;
			}

			m_HitsMax = Utility.RandomMinMax( min, max );
			Hits = HitsMax;
		}

		public void SetStam( int val )
		{
			m_StamMax = val;
			Stam = StamMax;
		}

		public void SetStam( int min, int max )
		{
			m_StamMax = Utility.RandomMinMax( min, max );
			Stam = StamMax;
		}

		public void SetMana( int val )
		{
			m_ManaMax = val;
			Mana = ManaMax;
		}

		public void SetMana( int min, int max )
		{
			m_ManaMax = Utility.RandomMinMax( min, max );
			Mana = ManaMax;
		}

		public void SetStr( int val )
		{
			RawStr = val;
			Hits = HitsMax;
		}

		public void SetStr( int min, int max )
		{
            #region genetics by Faxx
            if( IsGeneticCreature )
            {
                m_DNA.Str = Utility.RandomMinMax( min, max );
                InvalidateDNA();
                return;
            } 
            #endregion

			RawStr = Utility.RandomMinMax( min, max );
			Hits = HitsMax;
		}

		public void SetDex( int val )
		{
			RawDex = val;
			Stam = StamMax;
		}

		public void SetDex( int min, int max )
		{
            #region genetics by Faxx
            if( IsGeneticCreature )
            {
                m_DNA.Dex = Utility.RandomMinMax( min, max );
                InvalidateDNA();
                return;
            }
            #endregion

			RawDex = Utility.RandomMinMax( min, max );
			Stam = StamMax;
		}

		public void SetInt( int val )
		{
			RawInt = val;
			Mana = ManaMax;
		}

		public void SetInt( int min, int max )
		{
            #region genetics by Faxx
            if( IsGeneticCreature )
            {
                m_DNA.Int = Utility.RandomMinMax( min, max );
                InvalidateDNA();
                return;
            }
            #endregion

			RawInt = Utility.RandomMinMax( min, max );
			Mana = ManaMax;
		}

		public void SetDamageType( ResistanceType type, int min, int max )
		{
			SetDamageType( type, Utility.RandomMinMax( min, max ) );
		}

		public void SetDamageType( ResistanceType type, int val )
		{
			switch ( type )
			{
				case ResistanceType.Physical: m_PhysicalDamage = val; break;
				case ResistanceType.Fire: m_FireDamage = val; break;
				case ResistanceType.Cold: m_ColdDamage = val; break;
				case ResistanceType.Poison: m_PoisonDamage = val; break;
				case ResistanceType.Energy: m_EnergyDamage = val; break;
			}
		}

		public void SetResistance( ResistanceType type, int min, int max )
		{
			SetResistance( type, Utility.RandomMinMax( min, max ) );
		}

		public void SetResistance( ResistanceType type, int val )
		{
			switch ( type )
			{
				case ResistanceType.Physical: m_PhysicalResistance = val; break;
				case ResistanceType.Fire: m_FireResistance = val; break;
				case ResistanceType.Cold: m_ColdResistance = val; break;
				case ResistanceType.Poison: m_PoisonResistance = val; break;
				case ResistanceType.Energy: m_EnergyResistance = val; break;
			}

			UpdateResistances();
		}

		public void SetSkill( SkillName name, double val )
		{
			Skills[name].BaseFixedPoint = (int)(val * 10);

			if ( Skills[name].Base > Skills[name].Cap )
			{
				if ( Core.SE )
					this.SkillsCap += ( Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint );

				Skills[name].Cap = Skills[name].Base;
			}
		}

		public void SetSkill( SkillName name, double min, double max )
		{
			int minFixed = (int)(min * 10);
			int maxFixed = (int)(max * 10);

			Skills[name].BaseFixedPoint = Utility.RandomMinMax( minFixed, maxFixed );

			if ( Skills[name].Base > Skills[name].Cap )
			{
				if ( Core.SE )
					this.SkillsCap += ( Skills[name].BaseFixedPoint - Skills[name].CapFixedPoint );

				Skills[name].Cap = Skills[name].Base;
			}
		}

		public void SetFameLevel( int level )
		{
			switch ( level )
			{
				case 1: Fame = Utility.RandomMinMax(     0,  1249 ); break;
				case 2: Fame = Utility.RandomMinMax(  1250,  2499 ); break;
				case 3: Fame = Utility.RandomMinMax(  2500,  4999 ); break;
				case 4: Fame = Utility.RandomMinMax(  5000,  9999 ); break;
				case 5: Fame = Utility.RandomMinMax( 10000, 10000 ); break;
			}
		}

		public void SetKarmaLevel( int level )
		{
			switch ( level )
			{
				case 0: Karma = -Utility.RandomMinMax(     0,   624 ); break;
				case 1: Karma = -Utility.RandomMinMax(   625,  1249 ); break;
				case 2: Karma = -Utility.RandomMinMax(  1250,  2499 ); break;
				case 3: Karma = -Utility.RandomMinMax(  2500,  4999 ); break;
				case 4: Karma = -Utility.RandomMinMax(  5000,  9999 ); break;
				case 5: Karma = -Utility.RandomMinMax( 10000, 10000 ); break;
			}
		}

		#endregion

		public static void Cap( ref int val, int min, int max )
		{
			if ( val < min )
				val = min;
			else if ( val > max )
				val = max;
		}

		#region Pack & Loot
		public void PackPotion()
		{
			PackItem( Loot.RandomPotion() );
		}

		public void PackArcanceScroll( double chance )
		{
			if ( !Core.ML || chance <= Utility.RandomDouble() )
				return;

			PackItem( Loot.Construct( Loot.ArcaneScrollTypes ) );
		}

		public void PackNecroScroll( int index )
		{
			if ( !Core.AOS || 0.05 <= Utility.RandomDouble() )
				return;

			PackItem( Loot.Construct( Loot.NecromancyScrollTypes, index ) );
		}

		public void PackScroll( int minCircle, int maxCircle )
		{
			PackScroll( Utility.RandomMinMax( minCircle, maxCircle ) );
		}

		public void PackScroll( int circle )
		{
			int min = (circle - 1) * 8;

			PackItem( Loot.RandomScroll( min, min + 7, SpellbookType.Regular ) );
		}

		public void PackMagicItems( int minLevel, int maxLevel )
		{
			PackMagicItems( minLevel, maxLevel, 0.30, 0.15 );
		}

		public void PackMagicItems( int minLevel, int maxLevel, double armorChance, double weaponChance )
		{
			if ( !PackArmor( minLevel, maxLevel, armorChance ) )
				PackWeapon( minLevel, maxLevel, weaponChance );
		}

		protected bool m_Spawning;
		protected int m_KillersLuck;

		public virtual void GenerateLoot( bool spawning )
		{
			m_Spawning = spawning;

			if ( !spawning )
				m_KillersLuck = LootPack.GetLuckChanceForKiller( this );

			GenerateLoot();

			if ( m_Paragon )
			{
                for( int i = 0; i < 3; i++ )
                {
                    if( Fame < 1250 )
                        AddLoot( LootPack.Meager, 2 );
                    else if( Fame < 2500 )
                        AddLoot( LootPack.Average, 2 );
                    else if( Fame < 5000 )
                        AddLoot( LootPack.Rich );
                    else if( Fame < 10000 )
                        AddLoot( LootPack.FilthyRich );
                    else
                        AddLoot( LootPack.UltraRich );
                }
			}

            Midgard.Engines.MonsterMasterySystem.Core.GenerateLoot( this ); // mod by Dies Irae

			m_Spawning = false;
			m_KillersLuck = 0;
		}

		public virtual void GenerateLoot()
		{
		}

		public virtual void AddLoot( LootPack pack, int amount )
		{
			for ( int i = 0; i < amount; ++i )
				AddLoot( pack );
		}

		public virtual void AddLoot( LootPack pack )
		{
			if ( Summoned )
				return;

			Container backpack = Backpack;

			if ( backpack == null )
			{
				backpack = new Backpack();

				backpack.Movable = false;

				AddItem( backpack );
			}

            if( BaseCreatureReflector.LogLootEnabled )
                BaseCreatureReflector.LogLoot( GetType(), pack );

			pack.Generate( this, backpack, m_Spawning, m_KillersLuck );
		}

		public bool PackArmor( int minLevel, int maxLevel )
		{
			return PackArmor( minLevel, maxLevel, 1.0 );
		}

		public bool PackArmor( int minLevel, int maxLevel, double chance )
		{
			if ( chance <= Utility.RandomDouble() )
				return false;

			Cap( ref minLevel, 0, 5 );
			Cap( ref maxLevel, 0, 5 );

			if ( Core.AOS )
			{
                /*
				Item item = Loot.RandomArmorOrShieldOrJewelry();

				if ( item == null )
					return false;

				int attributeCount, min, max;
				GetRandomAOSStats( minLevel, maxLevel, out attributeCount, out min, out max );

				if ( item is BaseArmor )
					BaseRunicTool.ApplyAttributesTo( (BaseArmor)item, attributeCount, min, max );
				else if ( item is BaseJewel )
					BaseRunicTool.ApplyAttributesTo( (BaseJewel)item, attributeCount, min, max );

				PackItem( item );
                */
			}
			else
			{
				BaseArmor armor = Loot.RandomArmorOrShield();

				if ( armor == null )
					return false;

				armor.ProtectionLevel = (ArmorProtectionLevel)RandomMinMaxScaled( minLevel, maxLevel );
				armor.Durability = (ArmorDurabilityLevel)RandomMinMaxScaled( minLevel, maxLevel );

				PackItem( armor );
			}

			return true;
		}

		public static void GetRandomAOSStats( int minLevel, int maxLevel, out int attributeCount, out int min, out int max )
		{
			int v = RandomMinMaxScaled( minLevel, maxLevel );

			if ( v >= 5 )
			{
				attributeCount = Utility.RandomMinMax( 2, 6 );
				min = 20; max = 70;
			}
			else if ( v == 4 )
			{
				attributeCount = Utility.RandomMinMax( 2, 4 );
				min = 20; max = 50;
			}
			else if ( v == 3 )
			{
				attributeCount = Utility.RandomMinMax( 2, 3 );
				min = 20; max = 40;
			}
			else if ( v == 2 )
			{
				attributeCount = Utility.RandomMinMax( 1, 2 );
				min = 10; max = 30;
			}
			else
			{
				attributeCount = 1;
				min = 10; max = 20;
			}
		}

		public static int RandomMinMaxScaled( int min, int max )
		{
			if ( min == max )
				return min;

			if ( min > max )
			{
				int hold = min;
				min = max;
				max = hold;
			}

			/* Example:
			 *    min: 1
			 *    max: 5
			 *  count: 5
			 * 
			 * total = (5*5) + (4*4) + (3*3) + (2*2) + (1*1) = 25 + 16 + 9 + 4 + 1 = 55
			 * 
			 * chance for min+0 : 25/55 : 45.45%
			 * chance for min+1 : 16/55 : 29.09%
			 * chance for min+2 :  9/55 : 16.36%
			 * chance for min+3 :  4/55 :  7.27%
			 * chance for min+4 :  1/55 :  1.81%
			 */

			int count = max - min + 1;
			int total = 0, toAdd = count;

			for ( int i = 0; i < count; ++i, --toAdd )
				total += toAdd*toAdd*toAdd; // mod by Dies Irae

			int rand = Utility.Random( total );
			toAdd = count;

			int val = min;

			for ( int i = 0; i < count; ++i, --toAdd, ++val )
			{
				rand -= toAdd*toAdd*toAdd; // mod by Dies Irae

				if ( rand < 0 )
					break;
			}

			return val;
		}

		public bool PackSlayer()
		{
			return PackSlayer( 0.05 );
		}

		public bool PackSlayer( double chance )
		{
			if ( chance <= Utility.RandomDouble() )
				return false;

			if ( Utility.RandomBool() )
			{
				BaseInstrument instrument = Loot.RandomInstrument();

				if ( instrument != null )
				{
					instrument.Slayer = SlayerGroup.GetLootSlayerType( GetType() );
					PackItem( instrument );
				}
			}
			else if ( !Core.AOS )
			{
				BaseWeapon weapon = Loot.RandomWeapon();

				if ( weapon != null )
				{
					weapon.Slayer = SlayerGroup.GetLootSlayerType( GetType() );
					PackItem( weapon );
				}
			}

			return true;
		}

		public bool PackWeapon( int minLevel, int maxLevel )
		{
			return PackWeapon( minLevel, maxLevel, 1.0 );
		}

		public bool PackWeapon( int minLevel, int maxLevel, double chance )
		{
			if ( chance <= Utility.RandomDouble() )
				return false;

			Cap( ref minLevel, 0, 5 );
			Cap( ref maxLevel, 0, 5 );

			if ( Core.AOS )
			{
				Item item = Loot.RandomWeaponOrJewelry();

				if ( item == null )
					return false;

				int attributeCount, min, max;
				GetRandomAOSStats( minLevel, maxLevel, out attributeCount, out min, out max );

				if ( item is BaseWeapon )
					BaseRunicTool.ApplyAttributesTo( (BaseWeapon)item, attributeCount, min, max );
				else if ( item is BaseJewel )
					BaseRunicTool.ApplyAttributesTo( (BaseJewel)item, attributeCount, min, max );

				PackItem( item );
			}
			else
			{
				BaseWeapon weapon = Loot.RandomWeapon();

				if ( weapon == null )
					return false;

				if ( 0.05 > Utility.RandomDouble() )
					weapon.Slayer = SlayerName.Silver;

				weapon.DamageLevel = (WeaponDamageLevel)RandomMinMaxScaled( minLevel, maxLevel );
				weapon.AccuracyLevel = (WeaponAccuracyLevel)RandomMinMaxScaled( minLevel, maxLevel );
				weapon.DurabilityLevel = (WeaponDurabilityLevel)RandomMinMaxScaled( minLevel, maxLevel );

				PackItem( weapon );
			}

			return true;
		}

		public void PackGold( int amount )
		{
            #region mod by Dies Irae
            if( LootNoGold )
                return;

            int scaledAmount = (int)( amount * Midgard2Persistance.GoldDemultiplier );

            if( Core.Debug )
                Utility.Log( "BaseCreaturePackGolg.log", "Notice: {0} gold dropped into {1} pack. (demult. is {2})", scaledAmount.ToString(), GetType().Name, Midgard2Persistance.GoldDemultiplier.ToString() );

            if( amount > 0 )
                PackItem( new Gold( scaledAmount ) );
            #endregion
		}

        #region mod by Dies Irae
        public void PackGold( int amount, bool dontScale )
        {
            if( LootNoGold )
                return;

            if( !dontScale && amount > 0 )
                PackItem( new Gold( amount ) );
            else
                PackGold( amount );
        }
        #endregion

		public void PackGold( int min, int max )
		{
			PackGold( Utility.RandomMinMax( min, max ) );
		}

		public void PackStatue( int min, int max )
		{
			PackStatue( Utility.RandomMinMax( min, max ) );
		}

		public void PackStatue( int amount )
		{
			for ( int i = 0; i < amount; ++i )
				PackStatue();
		}

		public void PackStatue()
		{
			PackItem( Loot.RandomStatue() );
		}

		public void PackGem()
		{
			PackGem( 1 );
		}

		public void PackGem( int min, int max )
		{
			PackGem( Utility.RandomMinMax( min, max ) );
		}

		public void PackGem( int amount )
		{
			if ( amount <= 0 )
				return;

			Item gem = Loot.RandomGem();

			gem.Amount = amount;

			PackItem( gem );
		}

		public void PackNecroReg( int min, int max )
		{
			PackNecroReg( Utility.RandomMinMax( min, max ) );
		}

		public void PackNecroReg( int amount )
		{
			for ( int i = 0; i < amount; ++i )
				PackNecroReg();
		}

		public void PackNecroReg()
		{
            //if ( !Core.AOS )
            //    return;

			PackItem( Loot.RandomNecromancyReagent() );
		}

		public void PackReg( int min, int max )
		{
			PackReg( Utility.RandomMinMax( min, max ) );
		}

		public void PackReg( int amount )
		{
			if ( amount <= 0 )
				return;

			Item reg = Loot.RandomReagent();

			reg.Amount = amount;

			PackItem( reg );
		}

		public void PackItem( Item item )
		{
			if ( Summoned || item == null )
			{
				if ( item != null )
					item.Delete();

				return;
			}

			Container pack = Backpack;

			if ( pack == null )
			{
				pack = new Backpack();

				pack.Movable = false;

				AddItem( pack );
			}

			if ( !item.Stackable || !pack.TryDropItem( this, item, false ) ) // try stack
				pack.DropItem( item ); // failed, drop it anyway
		}
		#endregion

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel >= AccessLevel.GameMaster && !Body.IsHuman )
			{
				Container pack = this.Backpack;

				if ( pack != null )
					pack.DisplayTo( from );
			}

			if ( this.DeathAdderCharmable && from.CanBeHarmful( this, false ) )
			{
				DeathAdder da = Spells.Necromancy.SummonFamiliarSpell.Table[from] as DeathAdder;

				if ( da != null && !da.Deleted )
				{
					from.SendAsciiMessage( "You charm the snake.  Select a target to attack." );
					from.Target = new DeathAdderCharmTarget( this );
				}
			}

			base.OnDoubleClick( from );
		}

		private class DeathAdderCharmTarget : Target
		{
			private BaseCreature m_Charmed;

			public DeathAdderCharmTarget( BaseCreature charmed ) : base( -1, false, TargetFlags.Harmful )
			{
				m_Charmed = charmed;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( !m_Charmed.DeathAdderCharmable || m_Charmed.Combatant != null || !from.CanBeHarmful( m_Charmed, false ) )
					return;

				DeathAdder da = Spells.Necromancy.SummonFamiliarSpell.Table[from] as DeathAdder;
				if ( da == null || da.Deleted )
					return;

				Mobile targ = targeted as Mobile;
				if ( targ == null || !from.CanBeHarmful( targ, false ) )
					return;

				from.RevealingAction( true );
				from.DoHarmful( targ, true );

				m_Charmed.Combatant = targ;

				if ( m_Charmed.AIObject != null )
					m_Charmed.AIObject.Action = ActionType.Combat;
			}
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );

			if ( Core.ML )
			{
				if ( DisplayWeight )
					list.Add( TotalWeight == 1 ? 1072788 : 1072789, TotalWeight.ToString() ); // Weight: ~1_WEIGHT~ stones

				if ( m_ControlOrder == OrderType.Guard )
					list.Add( 1080078 ); // guarding
			}

			if ( Summoned && !IsAnimatedDead && !IsNecroFamiliar )
				list.Add( 1049646 ); // (summoned)

			#region modifica by Dies Irae per il sistema custom taming
			PetUtility.HandleAddNameProperties( this, list );

            if( IsHitched )
                list.Add( string.Format( IsBonded ? "(hitched and bonded)" : "(hitched)" ) );
			#endregion

            if( Controlled && Commandable && !( this is BaseHireling ) )
			{	
				if ( Summoned )
					list.Add( 1049646 ); // (summoned)
				else if ( IsBonded )	//Intentional difference (showing ONLY bonded when bonded instead of bonded & tame)
					list.Add( 1049608 ); // (bonded)
				else
					list.Add( 502006 ); // (tame)
            }
            #region modifica by Dies Irae
            else if( this is BaseHireling && Controlled && Commandable )
            {
                list.Add( 1062030 ); // (hired)
            }
            #endregion
        }

		public override void OnSingleClick( Mobile from )
		{
                	int hue = Notoriety.GetHue( Notoriety.Compute( from, this ) );

			if ( Controlled && GiftOfLifeSpell.HasProtection( this ) )//druido
				PrivateOverheadMessage( MessageType.Regular, hue, true, string.Format( from.Language == "ITA" ? "(protetto)" : "(protected)" ), from.NetState );

			#region mod by Dies Irae
			if( Core.T2A )
			{
				base.OnSingleClick( from );
				return;
			}
			#endregion

			if ( Controlled && Commandable )
			{
				int number;

				if ( Summoned )
					number = 1049646; // (summoned)
				else if ( IsBonded )
					number = 1049608; // (bonded)
				else
					number = 502006; // (tame)

				#region mod by Dies Irae
				if( this is BaseHireling && Controlled && Commandable )
					number = 1062030; // (hired)
				#endregion

				PrivateOverheadMessage( MessageType.Regular, hue, number, from.NetState );

				#region mod by Dies Irae
				if( IsHitched )
					PrivateOverheadMessage( MessageType.Regular, hue, true, string.Format( IsBonded ? "(hitched and bonded)" : "(hitched)" ), from.NetState );
				#endregion
			}

			base.OnSingleClick( from );
		}

		public virtual double TreasureMapChance{ get{ return TreasureMap.LootChance; } }
		public virtual int TreasureMapLevel{ get{ return -1; } }

		public virtual bool IgnoreYoungProtection { get { return false; } }

		public override bool OnBeforeDeath()
		{
            #region mod by Dies Irae
            PetLeveling.HandleDeath( this );

            if( KilledByGuard )
                ClearPack();
            #endregion

			int treasureLevel = TreasureMapLevel;

			if ( treasureLevel == 1 && this.Map == Map.Trammel && TreasureMap.IsInHavenIsland( this ) )
			{
				Mobile killer = this.LastKiller;

				if ( killer is BaseCreature )
					killer = ((BaseCreature)killer).GetMaster();

				if ( killer is PlayerMobile && ((PlayerMobile)killer).Young )
					treasureLevel = 0;
			}

			if ( !Summoned && !NoKillAwards && !IsBonded && treasureLevel >= 0 )
			{
				#region modifica by Dies Irae
                if ( m_Paragon && XmlParagon.GetChestChance( this ) > Utility.RandomDouble() )
                    XmlParagon.AddChest( this, treasureLevel );
				#endregion
				else if ( (Map == Map.Felucca || Map == Map.Trammel) && TreasureMap.LootChance >= Utility.RandomDouble() )
					PackItem( new TreasureMap( treasureLevel, Map ) );
			}		

			if ( !Summoned && !NoKillAwards && !m_HasGeneratedLoot )
			{
				m_HasGeneratedLoot = true;
				GenerateLoot( false );
			}

			if ( !NoKillAwards && Region.IsPartOf( "Doom" ) )
			{
				int bones = Engines.Quests.Doom.TheSummoningQuest.GetDaemonBonesFor( this );

				if ( bones > 0 )
					PackItem( new DaemonBone( bones ) );
			}

			if ( IsAnimatedDead )
				Effects.SendLocationEffect( Location, Map, 0x3728, 13, 1, 0x461, 4 );

			InhumanSpeech speechType = this.SpeechType;

			if ( speechType != null )
				speechType.OnDeath( this );

			if ( m_ReceivedHonorContext != null )
				m_ReceivedHonorContext.OnTargetKilled();

			XmlAttach.HandleOnBeforeDeath( this ); // mod by Dies Irae per gestire questo metodo via xmlAttach

			return base.OnBeforeDeath();
		}

        public bool KilledByGuard { get; set; }

        private bool m_NoKillAwards;

		public bool NoKillAwards
		{
			get{ return m_NoKillAwards || KilledByGuard; } // mod by Dies Irae
			set{ m_NoKillAwards = value; }
		}

		public int ComputeBonusDamage( List<DamageEntry> list, Mobile m )
		{
			int bonus = 0;

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				DamageEntry de = list[i];

				if ( de.Damager == m || !(de.Damager is BaseCreature) )
					continue;

				BaseCreature bc = (BaseCreature)de.Damager;
				Mobile master = null;

				master = bc.GetMaster();

				if ( master == m )
					bonus += de.DamageGiven;
			}

			return bonus;
		}

		public Mobile GetMaster()
		{
			#region mod by Magius(CHE): recoursive find root master :-)
			
			Mobile master = null;
			if( Controlled && ControlMaster != null )
			{
				master = ControlMaster;
				var bc = master as BaseCreature;
				if (bc != null)
				{
					var recmaster = bc.GetMaster();
					if (recmaster!=null)
						master = recmaster;
				}
			}
			else if( Summoned && SummonMaster != null )
			{
				master = SummonMaster;
				var bc = master as BaseCreature;
				if (bc != null)
				{
					var recmaster = bc.GetMaster();
					if (recmaster!=null)
						master = recmaster;
				}
			}
			return master;
			#endregion
			/*
			if ( Controlled && ControlMaster != null )
				return ControlMaster;
			else if ( Summoned && SummonMaster != null )
				return SummonMaster;

			return null;*/
		}

		private class FKEntry
		{
			public Mobile m_Mobile;
			public int m_Damage;

			public FKEntry( Mobile m, int damage )
			{
				m_Mobile = m;
				m_Damage = damage;
			}
		}

		public static List<DamageStore> GetLootingRights( List<DamageEntry> damageEntries, int hitsMax )
		{
			return GetLootingRights( damageEntries, hitsMax, false );
		}

		public static List<DamageStore> GetLootingRights( List<DamageEntry> damageEntries, int hitsMax, bool partyAsIndividual )
		{
			List<DamageStore> rights = new List<DamageStore>();

			for ( int i = damageEntries.Count - 1; i >= 0; --i )
			{
				if ( i >= damageEntries.Count )
					continue;

				DamageEntry de = damageEntries[i];

				if ( de.HasExpired )
				{
					damageEntries.RemoveAt( i );
					continue;
				}

				int damage = de.DamageGiven;

				List<DamageEntry> respList = de.Responsible;

				if ( respList != null )
				{
					for ( int j = 0; j < respList.Count; ++j )
					{
						DamageEntry subEntry = respList[j];
						Mobile master = subEntry.Damager;

						if ( master == null || master.Deleted || !master.Player )
							continue;

						bool needNewSubEntry = true;

						for ( int k = 0; needNewSubEntry && k < rights.Count; ++k )
						{
							DamageStore ds = rights[k];

							if ( ds.m_Mobile == master )
							{
								ds.m_Damage += subEntry.DamageGiven;
								needNewSubEntry = false;
							}
						}

						if ( needNewSubEntry )
							rights.Add( new DamageStore( master, subEntry.DamageGiven ) );

						damage -= subEntry.DamageGiven;
					}
				}

				Mobile m = de.Damager;

				if ( m == null || m.Deleted || !m.Player )
					continue;

				if ( damage <= 0 )
					continue;

				bool needNewEntry = true;

				for ( int j = 0; needNewEntry && j < rights.Count; ++j )
				{
					DamageStore ds = rights[j];

					if ( ds.m_Mobile == m )
					{
						ds.m_Damage += damage;
						needNewEntry = false;
					}
				}

				if ( needNewEntry )
					rights.Add( new DamageStore( m, damage ) );
			}

			if ( rights.Count > 0 )
			{
				rights[0].m_Damage = (int)(rights[0].m_Damage * 1.25);	//This would be the first valid person attacking it.  Gets a 25% bonus.  Per 1/19/07 Five on Friday

				if ( rights.Count > 1 )
					rights.Sort(); //Sort by damage

				int topDamage = rights[0].m_Damage;
				int minDamage;

				if ( hitsMax >= 3000 )
					minDamage = topDamage / 16;
				else if ( hitsMax >= 1000 )
					minDamage = topDamage / 8;
				else if ( hitsMax >= 200 )
					minDamage = topDamage / 4;
				else
					minDamage = topDamage / 2;

				for ( int i = 0; i < rights.Count; ++i )
				{
					DamageStore ds = rights[i];

					ds.m_HasRight = ( ds.m_Damage >= minDamage );
				}
			}

			return rights;
		}

		public virtual void OnKilledBy( Mobile mob )
		{
			#region mod by Dies Irae
            LogDeath( mob );

			/*
			if ( m_Paragon && Paragon.CheckArtifactChance( mob, this ) )
				Paragon.GiveArtifactTo( mob );
			*/

		    if( m_Paragon && XmlParagon.CheckArtifactChance( mob, this ) )
                XmlParagon.GiveArtifactTo( mob, this );
			#endregion
				
			#region Mondain's Legacy	
			if ( GivesMinorArtifact && Paragon.CheckArtifactChance( mob, this ) )
				GiveMinorArtifact( mob );
			#endregion
		}

		public override void OnDeath( Container c )
		{
			MeerMage.StopEffect( this, false );
			#region mod by Arlas [Necro]
			if ( c is Corpse )
			{
				( (Corpse)c ).LeechableHp = (this.Player ? 50 : (Summoned ? 5 : 25) );
			}
			#endregion

			#region mod by Dies Irae : hiring system
			if( this is BaseHireling && ( (BaseHireling)this ).GoldOnDeath > 0 && c is Corpse )
			{
				( (Corpse)c ).IsHireable = true;
				( (Corpse)c ).GoldOnDeath = ( (BaseHireling)this ).GoldOnDeath;
				( (Corpse)c ).HireMaster = ControlMaster;
			}
			#endregion

			if ( IsBonded )
			{
				int sound = this.GetDeathSound();

				if ( sound >= 0 )
					Effects.PlaySound( this, this.Map, sound );

				Warmode = false;

				Poison = null;
				Combatant = null;

				Hits = 0;
				Stam = 0;
				Mana = 0;

				IsDeadPet = true;
				ControlTarget = ControlMaster;

				if( IsBonded )
				    ControlOrder = OrderType.Follow;
				else
					ControlOrder = OrderType.Stay; // mod by Dies Irae

				ProcessDeltaQueue();
				SendIncomingPacket();
				SendIncomingPacket();

				List<AggressorInfo> aggressors = this.Aggressors;

				for ( int i = 0; i < aggressors.Count; ++i )
				{
					AggressorInfo info = aggressors[i];

					if ( info.Attacker.Combatant == this )
						info.Attacker.Combatant = null;
				}

				List<AggressorInfo> aggressed = this.Aggressed;

				for ( int i = 0; i < aggressed.Count; ++i )
				{
					AggressorInfo info = aggressed[i];

					if ( info.Defender.Combatant == this )
						info.Defender.Combatant = null;
				}

				Mobile owner = this.ControlMaster;

				if ( owner == null || owner.Deleted || owner.Map != this.Map || !owner.InRange( this, 12 ) || !this.CanSee( owner ) || !this.InLOS( owner ) )
				{
					if ( this.OwnerAbandonTime == DateTime.MinValue )
						this.OwnerAbandonTime = DateTime.Now;
				}
				else
				{
					this.OwnerAbandonTime = DateTime.MinValue;
				}

				GiftOfLifeSpell.HandleDeath( this );

				CheckStatTimers();
			}
			else
			{
				if ( !Summoned && !m_NoKillAwards )
				{
					int totalFame = Fame / 100;
					int totalKarma = -Karma / 100;

					List<DamageStore> list = GetLootingRights( this.DamageEntries, this.HitsMax );
					List<Mobile> titles = new List<Mobile>();
					List<int> fame = new List<int>();
					List<int> karma = new List<int>();
					
					bool givenQuestKill = false;
					bool givenFactionKill = false;
					bool givenToTKill = false;

					for ( int i = 0; i < list.Count; ++i )
					{
						DamageStore ds = list[i];

						if ( !ds.m_HasRight )
							continue;

						Party party = Engines.PartySystem.Party.Get( ds.m_Mobile );
 
						if ( party != null )
						{
							int divedFame = totalFame / party.Members.Count;
							int divedKarma = totalKarma / party.Members.Count;

							for ( int j = 0; j < party.Members.Count; ++j )
							{
								PartyMemberInfo info = party.Members[ j ] as PartyMemberInfo;

								if ( info != null && info.Mobile != null )
								{
									int index = titles.IndexOf( info.Mobile );

									if ( GetDistanceToSqrt( info.Mobile ) < 30 )
										ClassSystem.HandleCorpseDrop( this, c, info.Mobile );

									if ( index == -1 )
									{
										titles.Add( info.Mobile );
										fame.Add( divedFame );
										karma.Add( divedKarma );
									}
									else
									{
										fame[ index ] += divedFame;
										karma[ index ] += divedKarma;
									}
								}
							}
						}
						else
						{
							if ( GetDistanceToSqrt( ds.m_Mobile ) < 30 )
								ClassSystem.HandleCorpseDrop( this, c, ds.m_Mobile );

							titles.Add( ds.m_Mobile );
							fame.Add( totalFame );
							karma.Add( totalKarma );
						}

						PaladinSystem.RegisterKill( ds.m_Mobile, this ); // mod by Dies Irae  
						XmlQuest.RegisterKill( this, ds.m_Mobile); // XMLSpawnermodification to support XmlQuest Killtasks

						OnKilledBy( ds.m_Mobile );

                        #region mod by Dies Irae : pre-aos stuff
                        //ClassSystem.HandleCorpseDrop( this, c, ds.m_Mobile );

                        if( Utility.Random( 20 ) < 1 && Region is DungeonRegion )
                            c.DropItem( new DungeonKey( (DungeonRegion)Region ) );
                        #endregion

						if ( !givenFactionKill )
						{
							givenFactionKill = true;
							Faction.HandleDeath( this, ds.m_Mobile );
						}

						Region region = ds.m_Mobile.Region;
						
						if( !givenToTKill && ( Map == Map.Tokuno || region.IsPartOf( "Yomotsu Mines" ) || region.IsPartOf( "Fan Dancer's Dojo" ) ))
						{
							givenToTKill = true;
							TreasuresOfTokuno.HandleKill( this, ds.m_Mobile );
						}

						if ( givenQuestKill )
							continue;

						PlayerMobile pm = ds.m_Mobile as PlayerMobile;

						if ( pm != null )
						{
							QuestSystem qs = pm.Quest;

							if ( qs != null )
							{
								qs.OnKill( this, c );
								givenQuestKill = true;
							}
							
							#region Mondain's Legacy
							QuestHelper.CheckCreature( pm, this );
							#endregion
						}
					}
					for ( int i = 0; i < titles.Count; ++i )
					{
						Titles.AwardFame( titles[ i ], fame[ i ], true );
						Titles.AwardKarma( titles[ i ], karma[ i ], true );
					}
				}

				#region modifiche by Dies Irae
				if( this is IBurningCreature )
					CreatureBurningSystem.Explode( this );

				XmlAttach.HandleOnDeath( this, c );

			    CheckReportMurder();

                if( KilledByGuard )
                    Console.WriteLine( "Notice: creature {0} was killed by a guard.", GetType().Name );
				#endregion

				base.OnDeath( c );

				if ( DeleteCorpseOnDeath )
					c.Delete();

				#region Instance Corpses by Dies Irae -bug with stackables
				if( Midgard2Persistance.InstanceLootEnabled )
					InstanceCorpse( c );
				#endregion
			}
		}

		/* To save on cpu usage, RunUO creatures only reacquire creatures under the following circumstances:
		 *  - 10 seconds have elapsed since the last time it tried
		 *  - The creature was attacked
		 *  - Some creatures, like dragons, will reacquire when they see someone move
		 * 
		 * This functionality appears to be implemented on OSI as well
		 */

		private DateTime m_NextReacquireTime;

		public DateTime NextReacquireTime{ get{ return m_NextReacquireTime; } set{ m_NextReacquireTime = value; } }

		public virtual TimeSpan ReacquireDelay{ get{ return TimeSpan.FromSeconds( 10.0 ); } }
		public virtual bool ReacquireOnMovement{ get{ return false; } }

		public void ForceReacquire()
		{
			m_NextReacquireTime = DateTime.MinValue;
		}

		public override void OnDelete()
		{
			Mobile m = m_ControlMaster;

			SetControlMaster( null );
			SummonMaster = null;

			if ( m_ReceivedHonorContext != null )
				m_ReceivedHonorContext.Cancel();

            #region Faxx genetics
            if( m_Fetus != null && !m_Fetus.Deleted )
            {
                m_Fetus.Delete();
                m_Fetus = null;
            }
            #endregion

			base.OnDelete();

			if ( m != null )
				m.InvalidateProperties();
		}
		#region edit by Arlas [AREA SPELL]
		public override bool CanAreaHarmful( Mobile target )
		{
			if( target == null || target.AccessLevel > AccessLevel.Player )
				return false;

			if( Deleted || target.Deleted || !Alive || IsDeadBondedPet || !target.Alive || IsDeadBondedPet )
			{
				return false;
			}

			if (target == this )
				return false;

			Mobile master = GetMaster();//se master = null non ha master.

			if (target == master)
				return false;

			if (master == null)//è un mostro
			{
				if (!target.Player)
				{
					if (target is BaseCreature)
					{
						BaseCreature bctar = (BaseCreature)target;
						Mobile mastertar = bctar.GetMaster();//se master = null non ha master.
						if (mastertar == null || mastertar == this || !mastertar.Player)
							return false;
						else if (mastertar.Player)
							return true;
					}
				}
				else
					return true;
			}
			else//ha master
			{
				if (master.Player)//tamato-summonato
				{
					if (target.Player)
					{
						Server.Engines.PartySystem.Party p = Server.Engines.PartySystem.Party.Get( target );
						if( p != null && p.Contains( master ) )
							return false;

						if( target is PlayerMobile && target.Guild != null && master.Guild != null && target.Guild == master.Guild )
							return false;

						return true;
					}
					else if (target is BaseCreature)
					{
						BaseCreature bctar = (BaseCreature)target;
						Mobile mastertar = bctar.GetMaster();//se master = null non ha master.
						if (mastertar == null)//è un mostro
							return true;
						else//è un tamato/summonato
						{
							if (mastertar.Player)
							{
								if (mastertar == master)//mio compagno tamato/summonato
									return false;
								else
								{
									Server.Engines.PartySystem.Party p = Server.Engines.PartySystem.Party.Get( mastertar );
									if( p != null && p.Contains( master ) )
										return false;

									if( mastertar is PlayerMobile && master is PlayerMobile && mastertar.Guild != null && master.Guild != null && mastertar.Guild == master.Guild )
										return false;

									return true;
								}
							}
							else//tamato/summonato appartenente ad un gruppo di mostri
								return true;
						}
					}
				}
				else//mostro app. ad un gruppo di mostri
				{
					if (!target.Player)
					{
						if (target is BaseCreature)
						{
							BaseCreature bctar = (BaseCreature)target;
							Mobile mastertar = bctar.GetMaster();//se master = null non ha master.
							if ( mastertar == null || mastertar == this || mastertar == master )
								return false;
							else if (mastertar.Player)
								return true;
						}
					}
					else
						return true;
				}
			}

			return true;
		}
		#endregion

		public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
		{
			if ( target is BaseFactionGuard || target is BaseTownGuard ) // mod by Dies Irae
				return false;

			if ( (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier )
			{
				if ( message )
				{
					if ( target.Title == null )
						SendMessage( "{0} the vendor cannot be harmed.", target.Name );
					else
						SendMessage( "{0} {1} cannot be harmed.", target.Name, target.Title );
				}

				return false;
			}

			return base.CanBeHarmful( target, message, ignoreOurBlessedness );
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			bool ret = base.CanBeRenamedBy( from );

			if ( Controlled && from == ControlMaster && !from.Region.IsPartOf( typeof( Jail ) ) )
				ret = true;

			return ret;
		}

		public bool SetControlMaster( Mobile m )
		{
			if ( m == null )
			{
				ControlMaster = null;
				Controlled = false;
				ControlTarget = null;
				ControlOrder = OrderType.None;
				Guild = null;

				Delta( MobileDelta.Noto );
			}
			else
			{
				SpawnEntry se = this.Spawner as SpawnEntry;
				if ( se != null && se.UnlinkOnTaming )
				{
					this.Spawner.Remove( this );
					this.Spawner = null;
				}

				if ( m.Followers + ControlSlots > m.FollowersMax )
				{
					m.SendLocalizedMessage( 1049607 ); // You have too many followers to control that creature.
					return false;
				}

				CurrentWayPoint = null;//so tamed animals don't try to go back
			
				ControlMaster = m;
				Controlled = true;
				ControlTarget = null;
				ControlOrder = OrderType.Come;
				Guild = null;
				
				if ( m_DeleteTimer != null )
				{
					m_DeleteTimer.Stop();
					m_DeleteTimer = null;
				}

				Delta( MobileDelta.Noto );
			}
			
			InvalidateProperties();

			return true;
		}

		public override void OnRegionChange( Region Old, Region New )
		{
			base.OnRegionChange( Old, New );

			if ( this.Controlled )
			{
				SpawnEntry se = this.Spawner as SpawnEntry;

				if ( se != null && !se.UnlinkOnTaming && ( New == null || !New.AcceptsSpawnsFrom( se.Region ) ) )
				{
					this.Spawner.Remove( this );
					this.Spawner = null;
				}
            }

            #region mod by Dies Irae
            int newHue = New != null ? New.GetSolidHue() : -1;

            if( newHue != -1 && SolidHueOverride != newHue ) // If we are entering a custom solid hue region
                SolidHueOverride = newHue;
            else if( newHue == -1 && SolidHueOverride != -1 ) // If we are leaving a custom solid hue region
                SolidHueOverride = -1;
            #endregion
        }

		private static bool m_Summoning;

		public static bool Summoning
		{
			get{ return m_Summoning; }
			set{ m_Summoning = value; }
		}

		public static bool Summon( BaseCreature creature, Mobile caster, Point3D p, int sound, TimeSpan duration )
		{
			return Summon( creature, true, caster, p, sound, duration );
		}

		public static bool Summon( BaseCreature creature, bool controlled, Mobile caster, Point3D p, int sound, TimeSpan duration )
		{
			if ( caster.Followers + creature.ControlSlots > caster.FollowersMax )
			{
				caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				creature.Delete();
				return false;
			}

			m_Summoning = true;

			if ( controlled )
				creature.SetControlMaster( caster );

			creature.RangeHome = 10;
			creature.Summoned = true;

			creature.SummonMaster = caster;

			Container pack = creature.Backpack;

			if ( pack != null )
			{
				for ( int i = pack.Items.Count - 1; i >= 0; --i )
				{
					if ( i >= pack.Items.Count )
						continue;

					pack.Items[i].Delete();
				}
			}
			
			#region Mondain's Legacy
			creature.SetHits( (int) Math.Floor( creature.HitsMax * ( 1 + ArcaneEmpowermentSpell.GetSpellBonus( caster, false ) / 100.0 ) ) );
			#endregion

			// mod by Dies Irae
			creature.SetHits( (int)Math.Floor( creature.HitsMax * ( 1 + DruidEmpowermentSpell.GetSpellBonus( caster, false ) / 100.0 ) ) );
			
			#region mod by Magius(CHE): Wild Summon
			if (Utility.RandomDouble()<=0.02)
				new WildTimer( caster, creature, TimeSpan.FromSeconds( Utility.RandomDouble() * duration.TotalSeconds ) ).Start();			
			#endregion
			
			if (creature.HitsMaxSeed > 1000)
			{
				creature.HitsMaxSeed = 1000;
				creature.Hits = 1000;
			}

			new UnsummonTimer( caster, creature, duration ).Start();
			creature.m_SummonEnd = DateTime.Now + duration;

			creature.MoveToWorld( p, caster.Map );

			Effects.PlaySound( p, creature.Map, sound );

			m_Summoning = false;

			return true;
		}
		
		#region mod by Magius(CHE): Untame summoned for wild creatures
		public virtual void MakeWild()
		{
			bool toRemove = false;			
			var m = this;
		    #region mod by Dies Irae
		    if( m.Blessed )
		        return;
		    #endregion

			// added lines to check if a wild creature in a house region has to be removed or not
			if ( (!m.Controlled && ( m.Region.IsPartOf( typeof( HouseRegion ) ) && m.CanBeDamaged()) || ( m.RemoveIfUntamed && m.Spawner == null )) )
			{
				toRemove = true;
			}

			if (m.Summoned)
			{
				m.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );		
				if ((m.GetMaster() as BaseCreature)!=null)
					toRemove = true; //summoned by Mob will be deleted.
			}
			
			m.PlaySound( m.GetAngerSound() );
			
			if (!toRemove)
			{
	            m.OnBadRelease(); // mod by Dies Irae
	
				m.Say( 1043255, m.Name ); // ~1_NAME~ appears to have decided that is better off without a master!
				m.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
				var master = (m.Summoned ? m.SummonMaster : m.Controlled ? m.ControlMaster : null) as PlayerMobile;
				if (m.Summoned)
				{					
					m.Summoned = false;
					m.SummonMaster = null;
					if (m.HitsMaxSeed > 500)
					{
						m.HitsMaxSeed = 500;
						m.Hits = 500;
					}
				}
				if (m.Controlled)
				{
					m.SetControlMaster( null );					
				}
				m.IsBonded = false;
				m.BondingBegin = DateTime.MinValue;
				m.OwnerAbandonTime = DateTime.MinValue;
				m.ControlTarget = null;
				//c.ControlOrder = OrderType.Release;
				if (m.AIObject!=null)
					m.AIObject.DoOrderRelease(); // this will prevent no release of creatures left alone with AI disabled (and consequent bug of Followers)
			}
			else // added code to handle removing of wild creatures in house regions			
			{
				m.Delete();
			}
		}
		
		private class WildTimer : Timer
		{
			private BaseCreature m_Creature;
			private Mobile m_Summoner;			
	
			public WildTimer( Mobile summoner, BaseCreature creature, TimeSpan delay ) : base( delay )
			{
				m_Creature = creature;
				m_Summoner = summoner;
				Priority = TimerPriority.OneSecond;
			}
	
			protected override void OnTick()
			{
				m_Creature.MakeWild();
			}
		}
		#endregion

		private static Type[] m_MinorArtifactsMl = new Type[]
		{
			typeof( AegisOfGrace ), typeof( BladeDance ), typeof( Bonesmasher ),
			typeof( Boomstick ), typeof( FeyLeggings ), typeof( FleshRipper ),
			typeof( HelmOfSwiftness ), typeof( PadsOfTheCuSidhe ), typeof( QuiverOfRage ),
			typeof( QuiverOfElements ), typeof( RaedsGlory ), typeof( RighteousAnger ),
			typeof( RobeOfTheEclipse ), typeof( RobeOfTheEquinox ), typeof( SoulSeeker ),
			typeof( TalonBite ), typeof( WildfireBow ), typeof( Windsong ),
			// TODO: Brightsight lenses, Bloodwood spirit, Totem of the void
		};

		public static Type[] MinorArtifactsMl
		{
			get { return m_MinorArtifactsMl; } 
		}

		private static bool EnableRummaging = true;

	    private const double ChanceToRummage = 0.9; // mod by Dies Irae ( was 0.5; // 50%)

		private const double MinutesToNextRummageMin = 0.25; // mod by Dies Irae ( was 1.0 )
        private const double MinutesToNextRummageMax = 0.5; // mod by Dies Irae ( was 2.0 )

		private const double MinutesToNextChanceMin = 0.12; // mod by Dies Irae ( was 0.25 )
		private const double MinutesToNextChanceMax = 0.50; // mod by Dies Irae ( was 0.75 )

		private DateTime m_NextRummageTime;
		public virtual bool CanBreath { get { return HasBreath && (!Summoned || IsPlayerSummoned); } }  //mod by Magius(CHE) i draghi summonati non soffiano.
		public virtual bool IsDispellable { get { return Summoned; /* && !IsAnimatedDead; */ } }  //mod by Magius(CHE): dannazone i summon undead non possono essere dispellati per ora li tolgo da qui ma non bene mica sa!

		#region Healing
		public virtual double MinHealDelay { get { return 2.0; } }
		public virtual double HealScalar { get { return 1.0; } }
		public virtual bool CanHeal { get { return false; } }
		public virtual bool CanHealOwner { get { return false; } }

		public double MaxHealDelay
		{
			get
			{
				if (ControlMaster != null)
				{
					double max = ControlMaster.Hits / 10;

					if (max > 10)
						max = 10;
					if (max < 1)
						max = 1;

					return max;
				}
				else
					return 7;
			}
		}

		private DateTime m_NextHealTime = DateTime.Now;
		private Timer m_HealTimer = null;

		public virtual void HealStart()
		{
			if (!Alive)
				return;

			if (CanHeal && Hits != HitsMax)
			{
				RevealingAction();

				double seconds = 10 - Dex / 12;

				m_HealTimer = Timer.DelayCall(TimeSpan.FromSeconds(seconds), new TimerStateCallback(Heal_Callback), this);
			}
			else if (CanHealOwner && ControlMaster != null && ControlMaster.Hits < ControlMaster.HitsMax && InRange(ControlMaster, 2))
			{
				ControlMaster.RevealingAction();

				double seconds = 10 - Dex / 15;
				double resDelay = (ControlMaster.Alive ? 0.0 : 5.0);

				seconds += resDelay;

				ControlMaster.SendLocalizedMessage(1008078, false, Name); //  : Attempting to heal you.

				m_HealTimer = Timer.DelayCall(TimeSpan.FromSeconds(seconds), new TimerStateCallback(Heal_Callback), ControlMaster);
			}
		}

		private void Heal_Callback(object state)
		{
			if (state is Mobile)
				Heal((Mobile)state);
		}

		public virtual void Heal(Mobile patient)
		{
			if (!Alive || !patient.Alive || !InRange(patient, 2))
			{
			}
			else if (patient.Poisoned)
			{
				double healing = Skills.Healing.Value;
				double anatomy = Skills.Anatomy.Value;
				double chance = (healing - 30.0) / 50.0 - patient.Poison.Level * 0.1;

				if ((healing >= 60.0 && anatomy >= 60.0) && chance > Utility.RandomDouble())
				{
					if (patient.CurePoison(this))
					{
						patient.SendLocalizedMessage(1010059); // You have been cured of all poisons.						
						patient.PlaySound(0x57);

						CheckSkill(SkillName.Healing, 0.0, 100.0);
						CheckSkill(SkillName.Anatomy, 0.0, 100.0);
					}
				}
			}
			else if (BleedAttack.IsBleeding(patient))
			{
				patient.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!
				BleedAttack.EndBleed(patient, true);
				patient.PlaySound(0x57);
			}
			else
			{
				double healing = Skills.Healing.Value;
				double anatomy = Skills.Anatomy.Value;
				double chance = (healing + 10.0) / 100.0;

				if (chance > Utility.RandomDouble())
				{
					double min, max;

					min = (anatomy / 10.0) + (healing / 6.0) + 4.0;
					max = (anatomy / 8.0) + (healing / 3.0) + 4.0;

					if (patient == this)
						max += 10;

					double toHeal = min + (Utility.RandomDouble() * (max - min));

					toHeal *= HealScalar;

					patient.Heal((int)toHeal);
					patient.PlaySound(0x57);

					CheckSkill(SkillName.Healing, 0.0, 100.0);
					CheckSkill(SkillName.Anatomy, 0.0, 100.0);
				}
			}

			if (m_HealTimer != null)
				m_HealTimer.Stop();

			m_HealTimer = null;

			m_NextHealTime = DateTime.Now + TimeSpan.FromSeconds(MinHealDelay + (Utility.RandomDouble() * MaxHealDelay));
		}
		#endregion

		public virtual void OnThink()
		{
			#region modifica ATS
			if ( Tamable )
            {
                if( NextLevel == 0 )
                    NextLevel = PetLeveling.ComputeNextLevel( this );

                if( MaxLevel == 0 )
                    PetUtility.HandleStartMaxLevel( this );
            }
			#endregion

			if ( EnableRummaging && CanRummageCorpses && !Summoned && !Controlled && DateTime.Now >= m_NextRummageTime )
			{
				double min, max;

				if ( ChanceToRummage > Utility.RandomDouble() && Rummage() )
				{
					min = MinutesToNextRummageMin;
					max = MinutesToNextRummageMax;

					XmlAttach.AttachTo( this, new XmlData( "HasRummaged" ) ); // mod by Dies Irae : flag mobs that have rummaged
				}
				else
				{
					min = MinutesToNextChanceMin;
					max = MinutesToNextChanceMax;
				}

				double delay = min + (Utility.RandomDouble() * (max - min));
				m_NextRummageTime = DateTime.Now + TimeSpan.FromMinutes( delay );
			}

			if ( CanBreath && DateTime.Now >= m_NextBreathTime ) // tested: controled dragons do breath fire, what about summoned skeletal dragons?
			{
				Mobile target = this.Combatant;

				if ( target != null && target.Alive && !target.IsDeadBondedPet && CanBeHarmful( target ) && target.Map == this.Map && !IsDeadBondedPet && target.InRange( this, BreathRange ) && InLOS( target ) && !BardPacified )
					BreathStart( target );

				m_NextBreathTime = DateTime.Now + TimeSpan.FromSeconds( BreathMinDelay + (Utility.RandomDouble() * BreathMaxDelay) );
			}
			
			#region Mondain's Legacy			
			if ( m_HealTimer == null && DateTime.Now >= m_NextHealTime && Map != Map.Internal )
			{				
				if ( this is BaseMount )
				{
					BaseMount mount = (BaseMount) this;
					
					if ( mount.Rider == null )
						HealStart();
				}
				else
					HealStart();
			}
			
			if ( CanAreaDamage && Combatant != null && DateTime.Now >= m_NextAreaDamage && Utility.RandomDouble() < AreaDamageChance )
				AreaDamage();
				
			if ( CanAreaPoison && Combatant != null && DateTime.Now >= m_NextAreaPoison && Utility.RandomDouble() < AreaPosionChance )
				AreaPoison();
							
			if ( CanAnimateDead && Combatant != null && DateTime.Now >= m_NextAnimateDead && Utility.RandomDouble() < AnimateChance )
				AnimateDead();
			#endregion

			#region modifica by Dies Irae
			if( this is IBurningCreature )
				CreatureBurningSystem.FireThoughts( this );
			#endregion
		}

		public virtual bool Rummage()
		{
			Corpse toRummage = null;

			foreach ( Item item in this.GetItemsInRange( 2 ) )
			{
				if ( item is Corpse && item.Items.Count > 0 )
				{
					toRummage = (Corpse)item;
					break;
				}
			}

			if ( toRummage == null )
				return false;

			Container pack = this.Backpack;

			if ( pack == null )
				return false;

			List<Item> items = toRummage.Items;

			bool rejected;
			LRReason reason;

			for ( int i = 0; i < items.Count; ++i )
			{
				Item item = items[Utility.Random( items.Count )];

				Lift( item, item.Amount, out rejected, out reason );

				if ( !rejected && Drop( this, new Point3D( -1, -1, 0 ) ) )
				{
				    #region mod by Dies Irae
				    if( Core.AOS )
				    {
				        // *rummages through a corpse and takes an item*
				        PublicOverheadMessage( MessageType.Emote, 0x3B2, 1008086 );
				    }
				    else
				        SayRummageMessage( toRummage, item );
					//TODO: Instancing of Rummaged stuff.
				    #endregion

                    return true;
				}
			}

			return false;
		}

		public void Pacify( Mobile master, DateTime endtime )
		{
			BardPacified = true;
			BardEndTime = endtime;
		}

		public override Mobile GetDamageMaster( Mobile damagee )
		{
			if ( m_bBardProvoked && damagee == m_bBardTarget )
				return m_bBardMaster;
			else
			{
				Mobile master = GetMaster();
				if (master != null)
					return master;
			}
			//else if ( m_bControlled && m_ControlMaster != null )
			//	return m_ControlMaster;
			//else if ( m_bSummoned && m_SummonMaster != null )
			//	return m_SummonMaster;

			return base.GetDamageMaster( damagee );
		}
 
		public void Provoke( Mobile master, Mobile target, bool bSuccess )
		{
			BardProvoked = true;

			if ( !Core.ML )
			{
				this.PublicOverheadMessage( MessageType.Emote, EmoteHue, false, "*looks furious*" );
			}
			
			if ( bSuccess )
			{
				PlaySound( GetIdleSound() );
 
				BardMaster = master;
				BardTarget = target;
				Combatant = target;
				BardEndTime = DateTime.Now + TimeSpan.FromSeconds( 30.0 );

				if ( target is BaseCreature )
				{
					BaseCreature t = (BaseCreature)target;

					if ( t.Unprovokable || (t.IsParagon && BaseInstrument.GetBaseDifficulty( t ) >= 160.0) )
						return;

					t.BardProvoked = true;

					t.BardMaster = master;
					t.BardTarget = this;
					t.Combatant = this;
					t.BardEndTime = DateTime.Now + TimeSpan.FromSeconds( 30.0 );
				}
			}
			else
			{
				PlaySound( GetAngerSound() );

				BardMaster = master;
				BardTarget = target;
			}
		}

		public bool FindMyName( string str, bool bWithAll )
		{
			int i, j;

			string name = this.Name;
 
			if( name == null || str.Length < name.Length )
				return false;
 
			string[] wordsString = str.Split(' ');
			string[] wordsName = name.Split(' ');
 
			for ( j=0 ; j < wordsName.Length; j++ )
			{
				string wordName = wordsName[j];
 
				bool bFound = false;
				for ( i=0 ; i < wordsString.Length; i++ )
				{
					string word = wordsString[i];

					if ( Insensitive.Equals( word, wordName ) )
						bFound = true;
 
					if ( bWithAll && Insensitive.Equals( word, "all" ) )
						return true;
				}
 
				if ( !bFound )
					return false;
			}
 
			return true;
		}

        public const int TeleportPetsRadius = 5; // mod by Dies Irae

		public static void TeleportPets( Mobile master, Point3D loc, Map map )
		{
			TeleportPets( master, loc, map, false );
		}

		public static void TeleportPets( Mobile master, Point3D loc, Map map, bool onlyBonded )
		{
			List<Mobile> move = new List<Mobile>();
			int radiusbonus = 0;

			if (master.Player && ClassSystem.IsNecromancer( master ))
				radiusbonus = 10;

            foreach( Mobile m in master.GetMobilesInRange( /* 3 */ TeleportPetsRadius + radiusbonus ) ) // mod by Dies Irae
			{
				if ( m is BaseCreature )
				{
					BaseCreature pet = (BaseCreature)m;

					if ( pet.Controlled && pet.ControlMaster == master )
					{
						if ( !onlyBonded || pet.IsBonded )
						{
							if ( pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow || pet.ControlOrder == OrderType.Come )
								move.Add( pet );
						}
					}
				}
			}

			foreach ( Mobile m in move )
				m.MoveToWorld( loc, map );
		}

		public virtual void ResurrectPet()
		{
			if ( !IsDeadPet )
				return;

			OnBeforeResurrect();

			Poison = null;

			Warmode = false;

			Hits = 10;
			Stam = StamMax;
			Mana = 0;

			ProcessDeltaQueue();

			IsDeadPet = false;

			Effects.SendPacket( Location, Map, new BondedStatus( 0, this.Serial, 0 ) );

			this.SendIncomingPacket();
			this.SendIncomingPacket();

			OnAfterResurrect();

			Mobile owner = this.ControlMaster;

			if ( owner == null || owner.Deleted || owner.Map != this.Map || !owner.InRange( this, 12 ) || !this.CanSee( owner ) || !this.InLOS( owner ) )
			{
				if ( this.OwnerAbandonTime == DateTime.MinValue )
					this.OwnerAbandonTime = DateTime.Now;
			}
			else
			{
				this.OwnerAbandonTime = DateTime.MinValue;
			}

			CheckStatTimers();
		}

		public override bool CanBeDamaged()
		{
			if ( IsDeadPet /*|| ( CantWalk && !CanSwim )*/ ) // mod by Dies Irae - remod my Magius(CHE)
				return false;

			return base.CanBeDamaged();
		}

		public virtual bool PlayerRangeSensitive{ get{ return (this.CurrentWayPoint == null); } }	//If they are following a waypoint, they'll continue to follow it even if players aren't around

		public override void OnSectorDeactivate()
		{
			#region modifica by Dies Irae per la non sensitività
			// La disattivazione della AI nn avviene qui ma nel metodo
			// OnTick dell' AI timer.
            /*
            if( PlayerRangeSensitive && m_AI != null )
                m_AI.Deactivate();
            */
			#endregion

			base.OnSectorDeactivate();
		}

		public override void OnSectorActivate()
		{
			if ( PlayerRangeSensitive && m_AI != null )
				m_AI.Activate();

			base.OnSectorActivate();
		}

		private bool m_RemoveIfUntamed;

		// used for deleting untamed creatures [in houses]
		private int m_RemoveStep; 

		[CommandProperty( AccessLevel.GameMaster )] 
		public bool RemoveIfUntamed{ get{ return m_RemoveIfUntamed; } set{ m_RemoveIfUntamed = value; } }

		[CommandProperty( AccessLevel.GameMaster )] 
		public int RemoveStep { get { return m_RemoveStep; } set { m_RemoveStep = value; } }

        #region Mondain's Legacy

        #region Animate Dead
        public virtual bool CanAnimateDead { get { return false; } }
        public virtual double AnimateChance { get { return 0.05; } }
        public virtual int AnimateScalar { get { return 50; } }
        public virtual TimeSpan AnimateDelay { get { return TimeSpan.FromSeconds( 10 ); } }
        public virtual BaseCreature Animates { get { return null; } }

        private DateTime m_NextAnimateDead = DateTime.Now;

        public virtual void AnimateDead()
        {
            Corpse best = null;

            foreach( Item item in Map.GetItemsInRange( Location, 12 ) )
            {
                Corpse c = null;

                if( item is Corpse )
                    c = (Corpse)item;
                else
                    continue;

                if( c.ItemID != 0x2006 || c.Channeled || c.Owner.GetType() == typeof( PlayerMobile ) || c.Owner.GetType() == null || ( c.Owner != null && c.Owner.Fame < 100 ) || ( ( c.Owner != null ) && ( c.Owner is BaseCreature ) && ( ( (BaseCreature)c.Owner ).Summoned || ( (BaseCreature)c.Owner ).IsBonded ) ) )
                    continue;

                best = c;
                break;
            }

            if( best != null )
            {
                BaseCreature animated = Animates;

                if( animated != null )
                {
                    animated.Tamable = false;
                    animated.MoveToWorld( best.Location, Map );
                    Scale( animated, AnimateScalar );
                    Effects.PlaySound( best.Location, Map, 0x1FB );
                    Effects.SendLocationParticles( EffectItem.Create( best.Location, Map, EffectItem.DefaultDuration ), 0x3789, 1, 40, 0x3F, 3, 9907, 0 );
                }


                best.ProcessDelta();
                best.SendRemovePacket();
                best.ItemID = Utility.Random( 0xECA, 9 ); // bone graphic
                best.Hue = 0;
                best.ProcessDelta();
            }

            m_NextAnimateDead = DateTime.Now + AnimateDelay;
        }

        public static void Scale( BaseCreature bc, int scalar )
        {
            int toScale;

            toScale = bc.RawStr;
            bc.RawStr = AOS.Scale( toScale, scalar );

            toScale = bc.HitsMaxSeed;

            if( toScale > 0 )
                bc.HitsMaxSeed = AOS.Scale( toScale, scalar );

            bc.Hits = bc.Hits; // refresh hits
        }
        #endregion

        #region Area Poison
        public virtual bool CanAreaPoison { get { return false; } }
        public virtual Poison HitAreaPoison { get { return Poison.Deadly; } }
        public virtual int AreaPoisonRange { get { return 10; } }
        public virtual double AreaPosionChance { get { return 0.4; } }
        public virtual TimeSpan AreaPoisonDelay { get { return TimeSpan.FromSeconds( 8 ); } }

        private DateTime m_NextAreaPoison = DateTime.Now;

        public virtual void AreaPoison()
        {
            List<Mobile> targets = new List<Mobile>();

            if( Map != null )
                foreach( Mobile m in GetMobilesInRange( AreaDamageRange ) )
                    if( this != m && SpellHelper.ValidIndirectTarget( this, m ) && CanBeHarmful( m, false ) && ( !Core.AOS || InLOS( m ) ) )
                    {
                        if( m is BaseCreature && ( (BaseCreature)m ).Controlled )
                            targets.Add( m );
                        else if( m.Player )
                            targets.Add( m );
                    }

            for( int i = 0; i < targets.Count; ++i )
            {
                Mobile m = targets[ i ];

                m.ApplyPoison( this, HitAreaPoison );

                Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x36B0, 1, 14, 63, 7, 9915, 0 );
                Effects.PlaySound( m.Location, m.Map, 0x229 );
            }

            m_NextAreaPoison = DateTime.Now + AreaPoisonDelay;
        }
        #endregion

        #region Area damage
        public virtual bool CanAreaDamage { get { return false; } }
        public virtual int AreaDamageRange { get { return 10; } }
        public virtual double AreaDamageScalar { get { return 1.0; } }
        public virtual double AreaDamageChance { get { return 0.4; } }
        public virtual TimeSpan AreaDamageDelay { get { return TimeSpan.FromSeconds( 8 ); } }

        public virtual int AreaPhysicalDamage { get { return 0; } }
        public virtual int AreaFireDamage { get { return 100; } }
        public virtual int AreaColdDamage { get { return 0; } }
        public virtual int AreaPoisonDamage { get { return 0; } }
        public virtual int AreaEnergyDamage { get { return 0; } }

        private DateTime m_NextAreaDamage = DateTime.Now;

        public virtual void AreaDamage()
        {
            List<Mobile> targets = new List<Mobile>();

            if( Map != null )
                foreach( Mobile m in GetMobilesInRange( AreaDamageRange ) )
                    if( this != m && SpellHelper.ValidIndirectTarget( this, m ) && CanBeHarmful( m, false ) && ( !Core.AOS || InLOS( m ) ) )
                    {
                        if( m is BaseCreature && ( (BaseCreature)m ).Controlled )
                            targets.Add( m );
                        else if( m.Player )
                            targets.Add( m );
                    }

            for( int i = 0; i < targets.Count; ++i )
            {
                Mobile m = targets[ i ];

                int damage;

                if( Core.AOS )
                {
                    damage = m.Hits / 2;

                    if( !m.Player )
                        damage = Math.Max( Math.Min( damage, 100 ), 15 );

                    damage += Utility.RandomMinMax( 0, 15 );
                }
                else
                {
                    damage = ( m.Hits * 6 ) / 10;

                    if( !m.Player && damage < 10 )
                        damage = 10;
                    else if( damage > 75 )
                        damage = 75;
                }

                damage = (int)( damage * AreaDamageScalar );

                DoHarmful( m );
                AreaDamageEffect( m );
                SpellHelper.Damage( TimeSpan.Zero, m, this, damage, AreaPhysicalDamage, AreaFireDamage, AreaColdDamage, AreaPoisonDamage, AreaEnergyDamage );
            }

            m_NextAreaDamage = DateTime.Now + AreaDamageDelay;
        }

        public virtual void AreaDamageEffect( Mobile m )
        {
            m.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
            m.PlaySound( 0x231 );

            /*
            m.FixedParticles( 0x3709, 10, 30, 5052, EffectLayer.LeftFoot ); // flamestrike
            m.PlaySound( 0x208 );
            */
        }
        #endregion

        private bool m_Allured;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Allured
        {
            get { return m_Allured; }
            set { m_Allured = value; }
        }

        public virtual void OnRelease( Mobile from )
        {
            if( m_Allured )
                Timer.DelayCall( TimeSpan.FromSeconds( 2 ), new TimerCallback( Delete ) );

            #region mod by Dies Irae
            if( !m_Allured && !String.IsNullOrEmpty( OriginalName ) )
                Name = OriginalName;
            #endregion
        }

        public void PackArcaneScroll( int max )
        {
            PackArcaneScroll( 0, max );
        }

        public void PackArcaneScroll( int min, int max )
        {
            double div = -1.0 / ( max - min + 1 );
            int amount = 0;

            for( int i = max - min; i >= 0; i-- )
            {
                if( Utility.RandomDouble() < ( Math.Exp( div * i ) - Math.Exp( div * ( i + 1 ) ) ) )
                {
                    amount = i;
                    break;
                }
            }

            for( int i = 0; i < min + amount; i++ )
                PackItem( Loot.Construct( Loot.ArcanistScrollTypes ) );
        }

        public override void OnItemLifted( Mobile from, Item item )
        {
            base.OnItemLifted( from, item );

            InvalidateProperties();
        }

        public virtual bool GivesMinorArtifact { get { return false; } }

        public static Type[] MLArtifacts
        {
            get { return m_Artifacts; }
        }

        private static Type[] m_Artifacts = new Type[]
		{
			typeof( AegisOfGrace ), typeof( BladeDance ), 
			typeof( Bonesmasher ), typeof( FeyLeggings ),
			typeof( FleshRipper ), typeof( HelmOfSwiftness ),
			typeof( PadsOfTheCuSidhe ), typeof( RaedsGlory ),
			typeof( RighteousAnger ), typeof( RobeOfTheEclipse ),
			typeof( RobeOfTheEquinox ), typeof( SoulSeeker ),
			typeof( TalonBite ), typeof( BloodwoodSpirit ),
			typeof( TotemOfVoid ), typeof( QuiverOfRage ),
			typeof( QuiverOfElements ), typeof( BrightsightLenses ),
			typeof( Boomstick ), typeof( WildfireBow ), 
			typeof( Windsong )
		};

        public static void GiveMinorArtifact( Mobile m )
        {
            // mod by Dies: disabilitato fono all'up di ML
            // Item item = Activator.CreateInstance( m_Artifacts[ Utility.Random( m_Artifacts.Length ) ] ) as Item;
            Item item = Activator.CreateInstance( Paragon.Artifacts[ Utility.Random( Paragon.Artifacts.Length ) ] ) as Item;

            if( item == null )
                return;

            if( m.AddToBackpack( item ) )
            {
                m.SendLocalizedMessage( 1062317 ); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
                m.SendLocalizedMessage( 1072223 ); // An item has been placed in your backpack.
            }
            else if( m.BankBox != null && m.BankBox.TryDropItem( m, item, false ) )
            {
                m.SendLocalizedMessage( 1062317 ); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
                m.SendLocalizedMessage( 1072224 ); // An item has been placed in your bank box.
            }
            else
            {
                item.MoveToWorld( m.Location, m.Map );
                m.SendLocalizedMessage( 1072523 ); // You find an artifact, but your backpack and bank are too full to hold it.
            }
        }
        #endregion

        #region Midgard modifications
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool CanAutoStable
        {
            get { return false; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HideTag { get; set; }

        public override bool ShowTag
        {
            get { return !HideTag; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HideInvulStatus { get; set; }

        public override bool ShowInvulStatus
        {
            get { return !HideInvulStatus; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HideFrozenStatus { get; set; }

        public override bool ShowFrozenStatus
        {
            get { return !HideFrozenStatus; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HideBondedStatus { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string CustomCorpseName { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasCustomCorpseName { get { return !string.IsNullOrEmpty( CustomCorpseName ); } }

        public virtual int CustomWeaponSpeed { get { return 0; } }

        public int HitsRegenBonus { get; set; }
        public int ManaRegenBonus { get; set; }
        public int StaminaRegenBonus { get; set; }

        public virtual bool CheckBook( Mobile from, Item dropped )
        {
            if( dropped is BaseBook )
                return OnBookGiven( from, (BaseBook)dropped );

            return false;
        }

        public virtual bool OnBookGiven( Mobile from, BaseBook dropped )
        {
            if( IsHumanInTown() )
            {
                Direction = GetDirectionTo( from );

                if( !string.IsNullOrEmpty( dropped.Title ) )
                    SayTo( from, "Thou art giving me {0}?", dropped.Title );
                else
                    SayTo( from, "Thou art giving me a book?" );

                SayTo( from, 501548 ); // I thank thee.

                dropped.Delete();
                return true;
            }

            return false;
        }

        public void InitMidgardCreature()
        {
            PetUtility.HandleSex( this );
            PetUtility.HandleStartMaxLevel( this );
            RarityLevel = 0; // rarity is set in child constructors

            if( this is IBurningCreature )
            {
                CreatureBurningSystem.AddLight( this );
                CreatureBurningSystem.HandleHue( this );
            }

            InitGenetics();

            m_Mastery = MasteryLevel.Normal;
        }

        private bool m_IsHitched;

        [CommandProperty( AccessLevel.Developer )]
        public bool IsHitched
        {
            get { return m_IsHitched; }
            set
            {
                bool oldValue = m_IsHitched;

                if( oldValue != value )
                {
                    m_IsHitched = value;

                    OnIsHitchedChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public HitchingPostComponent HitchingPost { get; set; }

        public virtual void OnIsHitchedChanged( bool oldValue )
        {
            if( !oldValue )
            {
                Say( true, "* the pet seems to accept staying there *" );
                Blessed = true;
            }
            else
            {
                Say( true, "* the pet leaves the hitching post *" );
                Blessed = false;

                HitchingPost = null;
            }
        }

        public void ClearPack()
        {
            Container pack = Backpack;
            if( pack == null )
                return;

            for( int i = pack.Items.Count - 1; i >= 0; --i )
            {
                if( i >= pack.Items.Count )
                    continue;

                pack.Items[ i ].Delete();
            }
        }

        public void Shrink()
        {
            if( ControlMaster == null || Summoned )
                return;

            if( IsPregnant )
                return;

            if( this is BaseEscortable )
                return;

            Type type = GetType();
            ShrinkItem si = new ShrinkItem();
            si.MobType = type;
            si.Pet = this;
            si.PetOwner = ControlMaster;

            if( this is BaseMount )
            {
                BaseMount mount = (BaseMount)this;
                si.MountID = mount.ItemID;
            }

            ControlMaster.AddToBackpack( si );

            Controlled = true;
            ControlMaster = null;
            Internalize();

            Loyalty = MaxLoyalty; // Wonderfully happy
            OwnerAbandonTime = DateTime.MinValue;
            IsStabled = true;
            IsShrinked = true;
            ShrinkItem = si;
        }

        private bool m_IsShrinked;

        [CommandProperty( AccessLevel.Developer )]
        public bool IsShrinked
        {
            get { return m_IsShrinked; }
            set
            {
                bool oldValue = m_IsShrinked;

                if( oldValue != value )
                {
                    m_IsShrinked = value;

                    OnIsShrinkedChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Item ShrinkItem { get; set; }

        public virtual void OnIsShrinkedChanged( bool oldValue )
        {
        }

        private static readonly int MessageHue = Core.AOS ? 0x3B2 : 0x3C3;

        public virtual bool CanSpeakMantra
        {
            get { return Body.IsHuman; }
        }

        public virtual Race RequiredRace { get { return null; } }

        public virtual void OnIsBondedChanged( bool oldValue )
        {
            if( oldValue != IsBonded )
            {
                try
                {
                    TextWriter tw = File.AppendText( "Logs/BaseCreatureIsBondedLog.log" );
                    tw.WriteLine( String.Format( "PetName: {0} - Serial {1} - DateTime {2} - IsBonded: {3}.",
                        String.IsNullOrEmpty( Name ) ? GetType().Name : Name, Serial, DateTime.Now, IsBonded ) );
                    tw.Close();
                }
                catch
                {
                }
            }
        }

        public virtual void OnLoyaltyChanged( int oldValue )
        {
            if( Controlled && ControlMaster != null )
            {
                if( oldValue != Loyalty )
                {
                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/BaseCreatureLoyaltyLog.log" );
                        tw.WriteLine( String.Format( "PetName: {0} - Serial {1} - Controller {2} - Account {3} - DateTime {4} - " +
                                                     "Loyalty from {5} - Loyalty to {6}.",
                            String.IsNullOrEmpty( Name ) ? GetType().Name : Name, Serial,
                            String.IsNullOrEmpty( ControlMaster.Name ) ? ControlMaster.Serial.ToString() : ControlMaster.Name,
                            ControlMaster.Account, DateTime.Now, oldValue, Loyalty ) );
                        tw.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }

        public virtual void OnBadRelease()
        {
            if( Controlled && ControlMaster != null )
            {
                try
                {
                    TextWriter tw = File.AppendText( "Logs/PetBadReleased.log" );
                    tw.WriteLine( String.Format( "PetName: {0} - Serial {1} - Controller {2} - Account {3} - DateTime {4} - Loyalty {5}.",
                        String.IsNullOrEmpty( Name ) ? GetType().Name : Name, Serial,
                        String.IsNullOrEmpty( ControlMaster.Name ) ? ControlMaster.Serial.ToString() : ControlMaster.Name,
                        ControlMaster.Account, DateTime.Now, Loyalty ) );
                    tw.Close();
                }
                catch
                {
                }
            }
        }

        public virtual void OnAfterSpawnAndModify()
        {
            if( Midgard.Engines.MonsterMasterySystem.Core.CheckConvert( this, Location, Map ) )
                Midgard.Engines.MonsterMasterySystem.Core.HandleOnAfterSpawn( this );
        }

        protected override void OnMapChange( Map oldMap )
        {
            Map map = Map;

            if( m_AI != null && map != null && map.GetSector( Location ).Active )
                m_AI.Activate();

            if( String.IsNullOrEmpty( OriginalName ) )
                OriginalName = Name;

            base.OnMapChange( oldMap );
        }

        public virtual void SayRummageMessage( Corpse toRummage, Item item )
        {
            if( Utility.Random( 10 ) < 5 )
            {
                PublicOverheadMessage( MessageType.Emote, MessageHue, true, "* yoink *" );
            }
            else
            {
                StringBuilder builder = new StringBuilder();

                builder.AppendFormat( !string.IsNullOrEmpty( Name ) ? string.Format( "*{0} ", Name ) : "*" ); // *an ettin
                builder.AppendFormat( "rummages through a corpse" ); // rummages through a corpse

                if( toRummage.Owner != null && toRummage.Owner.Name != null )
                    builder.AppendFormat( " of {0}", toRummage.Owner.Name ); // of Dies Irae

                if( item.Name != null )
                    builder.AppendFormat( " and takes {0}*", StringUtility.ConvertItemName( item.LabelNumber, item.Amount, "ENG" ) );
                else
                    builder.Append( "an item*" );
            }
        }

        public virtual void DoIdleAnimation()
        {
            if( Body.IsHuman )
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0: Animate( 5, 5, 1, true, true, 1 ); break;
                    case 1: Animate( 6, 5, 1, true, false, 1 ); break;
                }
            }
            else if( Body.IsAnimal )
            {
                switch( Utility.Random( 3 ) )
                {
                    case 0: Animate( 3, 3, 1, true, false, 1 ); break;
                    case 1: Animate( 9, 5, 1, true, false, 1 ); break;
                    case 2: Animate( 10, 5, 1, true, false, 1 ); break;
                }
            }
            else if( Body.IsMonster )
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0: Animate( 17, 5, 1, true, false, 1 ); break;
                    case 1: Animate( 18, 5, 1, true, false, 1 ); break;
                }
            }

            PlaySound( GetIdleSound() );
        }

        public virtual void CheckReportMurder()
        {
            if( IsReporter( this ) )
            {
                List<Mobile> killers = new List<Mobile>();

                foreach( AggressorInfo ai in Aggressors )
                {
                    if( ai.Attacker.Player && ai.CanReportMurder && !ai.Reported && !Midgard.Engines.BountySystem.Core.Attackable( ai.Attacker, this ) )
                    {
                        killers.Add( ai.Attacker );
                        ai.Reported = true;
                    }
                }

                if( killers.Count > 0 )
                {
                    foreach( Mobile killer in killers )
                    {
                        killer.Kills++;
                        killer.ShortTermMurders++;

                        if( killer is PlayerMobile )
                            ( (PlayerMobile)killer ).ResetKillTime();

                        killer.SendLocalizedMessage( 1049067 ); //You have been reported for murder!

                        if( killer.Kills == 5 )
                            killer.SendLocalizedMessage( 502134 );//You are now known as a murderer!
                        else if( SkillHandlers.Stealing.SuspendOnMurder && killer.Kills == 1 && killer is PlayerMobile && ( (PlayerMobile)killer ).NpcGuild == NpcGuild.ThievesGuild )
                            killer.SendLocalizedMessage( 501562 ); // You have been suspended by the Thieves Guild.

                        try
                        {
                            using( StreamWriter op = new StreamWriter( "Logs/creature-murders.log", true ) )
                            {
                                op.WriteLine(
                                    "BaseCreature type {0}, name {1}, serial {2}, has reported a murder to {3} (account {4}) in date and time {5}.",
                                    GetType().Name, string.IsNullOrEmpty( Name ) ? "null name" : Name, Serial,
                                    killer.Name, killer.Account.Username, DateTime.Now );
                                op.WriteLine();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static bool IsReporter( BaseCreature bc )
        {
            if( bc == null || bc.Summoned )
                return false;

            Type type = bc.GetType();

            bool canReport = false;
            foreach( Type t in m_MurderReportersTable )
            {
                if( type == t || type.IsSubclassOf( t ) )
                    canReport = true;
            }

            return ( canReport || bc.IsHumanInTown() ) && bc.Kills < 5 && !bc.AlwaysMurderer;
        }

        private static readonly Type[] m_MurderReportersTable = new Type[]
        {
            typeof( BaseGuard ),
            typeof( TownGuard ),
            typeof( PatrolGuard ),
            typeof( BaseEscortable ),
            typeof( BaseHireling )
        };

        public virtual void LogDeath( Mobile mob )
        {
            if( mob != null && ( mob.AccessLevel > AccessLevel.Player || mob.Account.Username.StartsWith( "st-", false, null ) ) )
            {
                using( StreamWriter tw = new StreamWriter( "Logs/LogKillsMobsByStaff.log", true ) )
                {
                    try
                    {
                        tw.WriteLine( "Il pg {0} - account {1} - in data {2} e ora {3} ha ucciso un mostro di tipo {4} " +
                                        "in locazione {5} con luck {6}.",
                                        mob.Name, mob.Account.Username,
                                        DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString(),
                                        GetType().Name, Location, mob.Luck );
                    }
                    catch( Exception ex )
                    {
                        tw.WriteLine( "Log di kill di una creatura fallito: {0}", ex );
                    }
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool GuardImmune
        {
            get { return false; }
        }

        private bool m_IsPetSum;

        public bool IsPetSum
        {
            get
            {
                if( !m_bSummoned || !m_bControlled )
                    m_IsPetSum = false;

                if( ( m_bSummoned && m_SummonMaster != null ) || ( m_bControlled && m_ControlMaster != null ) )
                    m_IsPetSum = true;

                return m_IsPetSum;
            }
            set { m_IsPetSum = value; }
        }

        private static readonly int[] m_ParagonHues = new int[] { 0 };

        public virtual int[] ParagonHues
        {
            get { return m_ParagonHues; }
        }

        public virtual string ParagonPrefix
        {
            get { return "elder"; }
        }

        private MasteryLevel m_Mastery;

        [CommandProperty( AccessLevel.GameMaster )]
        public MasteryLevel Mastery
        {
            get { return m_Mastery; }
            set
            {
                if( m_Mastery == value )
                    return;
                else if( value == MasteryLevel.Normal )
                {
                    m_Mastery = value;
                    MasterySystem.Core.UnConvert( this, m_Mastery );
                }
                else
                {
                    MasteryLevel oldLevel = m_Mastery;
                    m_Mastery = value;
                    MasterySystem.Core.Convert( this, oldLevel );
                }

                InvalidateProperties();
            }
        }

        /// <summary>
        /// if true the damage isa calculated through weapon damage instead of creature damage min/max
        /// </summary>
        public virtual bool UsesWeaponDamage
        {
            get { return false; }
        }

        /// <summary>
        /// dont smartspawn mobs that have rummaged
        /// </summary>
        public virtual bool HoldSmartSpawning
        {
            get { return XmlAttach.FindAttachment( this, typeof( XmlData ), "HasRummaged" ) != null; }
        }

        #region InstanceCorpses
        private Timer m_CorpseFinalizerTimer;

        private static void FinalizeInstanceCorpse( object o )
        {
            Corpse corpse = o as Corpse;

            if( corpse == null || corpse.Deleted )
                return;

            List<Item> list = new List<Item>( corpse.Items );

            foreach( Item item in list )
            {
                if( item.Stackable )
                    MergeType( corpse, item.GetType() );
                else
                    item.InstanceOwner = null;
            }

            list = null;
        }

        private static void MergeType( Container c, Type t )
        {
            if( t.GetConstructor( Type.EmptyTypes ) == null )
            {
                Console.WriteLine( "[Instance Corpse Bugreport] Attempted to instantioate item of type: {0} -> No suitable ctor found to invoke.", t.FullName );
                return;
            }

            int amount = c.ConsumeUpTo( t, int.MaxValue );

            if( amount > 60000 )
            {
                Item item;

                do
                {
                    c.DropItem( item = (Item)Activator.CreateInstance( t ) );
                    item.Amount = 60000;
                    amount -= 60000;
                } while( amount > 60000 );

                if( amount > 0 )
                {
                    c.DropItem( item = (Item)Activator.CreateInstance( t ) );
                    item.Amount = amount;
                }
            }
            else
            {
                Item item = (Item)Activator.CreateInstance( t );
                item.Amount = amount;

                c.DropItem( item );
            }
        }

        public static Item MakeInstanceOwner( Item item, object o )
        {
            item.InstanceOwner = o;
            return item;
        }

        /// <summary>
        /// Flag to make items un-ownable.
        /// </summary>
        public const int IOFlag = 0x10000000;

        private void InstanceCorpse( Container c )
        {
            List<DamageStore> tempList = GetLootingRights( DamageEntries, HitsMax );
            List<DamageStore> damages = new List<DamageStore>();

            foreach( DamageStore ds in tempList )
            {
                if( ds.m_HasRight )
                {
                    damages.Add( ds );
                }
            }

            tempList = null;

            damages.Sort();

            if( c != null && !c.Deleted && damages.Count != 0 )
            {
                Dictionary<Type, int> stackable = new Dictionary<Type, int>();
                List<Item> ordinary = new List<Item>();
                List<Item> toRemove = new List<Item>();

                foreach( Item item in c.Items )
                {
                    if( item.InstanceOwner == this || item.InstanceOwner == null && ( item.SavedFlags & IOFlag ) == 0 )
                    {
                        if( item.Stackable && !(item is BasePoisonAmmonition) )//temp fix, TODO mass fix for stackables
                        {
                            Type t = item.GetType();

                            if( stackable.ContainsKey( t ) )
                                stackable[ t ] += item.Amount;
                            else
                                stackable[ t ] = item.Amount;

                            toRemove.Add( item );
                        }
                        else
                        {
                            ordinary.Add( item );
                        }
                    }
                }

                for( int i = 0; i < toRemove.Count; i++ )
                {
                    toRemove[ i ].Delete();
                }

                foreach( KeyValuePair<Type, int> ti in stackable )
                {
                    int pile = Math.Max( ( ti.Value / damages.Count ), 1 );

                    Dictionary<PARTY, int> party = new Dictionary<PARTY, int>();
                    PARTY p = null;

                    for( int k = 0; ( ( k < damages.Count ) && ( k < ti.Value ) ); k++ )
                    {
                        DamageStore ds = damages[ k ];

                        p = PARTY.Get( ds.m_Mobile );

                        if( p == null )
                        {
                            Item item = (Item)Activator.CreateInstance( ti.Key );
                            item.Amount = pile;
                            c.DropItem( item );
                            item.InstanceOwner = ds.m_Mobile;
                        }
                        else
                        {
                            if( party.ContainsKey( p ) )
                                party[ p ] += pile;
                            else
                                party[ p ] = pile;
                        }
                    }

                    foreach( KeyValuePair<PARTY, int> kvp in party )
                    {
                        Item item = (Item)Activator.CreateInstance( ti.Key );
                        item.Amount = kvp.Value;
                        c.DropItem( item );
                        item.InstanceOwner = kvp.Key;
                    }

                    party = null;
                }

                for( int j = 0; j < ordinary.Count; j++ )
                {
                    Item current = ordinary[ j ];
                    Mobile owner = damages[ j % damages.Count ].m_Mobile;

                    if( owner.Party == null )
                        current.InstanceOwner = owner;
                    else
                        current.InstanceOwner = owner.Party;
                }

                stackable = null;
                ordinary = null;
                toRemove = null;
            }

            m_CorpseFinalizerTimer = Timer.DelayCall( TimeSpan.FromMinutes( 3.0 ), new TimerStateCallback( FinalizeInstanceCorpse ), c );
        }
        #endregion

        #region Genetics by Faxx
        public virtual void SetDamageByDNA( int min, int max )
        {
            if( m_DNA != null )
            {
                m_DNA.DamageMin = min;
                m_DNA.DamageMax = max;
                InvalidateDNA();
            }
        }

        public void SetVirtualArmor( int min, int max )
        {
            if( IsGeneticCreature )
            {
                m_DNA.VirtualArmor = Utility.RandomMinMax( min, max );
                InvalidateDNA();
                return;
            }

            VirtualArmor = Utility.RandomMinMax( min, max );
        }

        public static bool GeneticSystemEnabled = true;

        /// <summary>
        /// return true to activate Genetics/Aging/Pregnancy
        /// </summary>
        public virtual bool IsGeneticCreature { get { return false; } }

        /// <summary>
        /// The DNA type
        /// </summary>
        public virtual Type DNAType { get { return typeof( AnimalDNA ); } }

        /// <summary>
        /// internal DNA data
        /// </summary>
        private AnimalDNA m_DNA;

        /// <summary>
        /// Accessor for m_DNA
        /// </summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public AnimalDNA DNA
        {
            get
            {
                if( m_DNA == null )
                {
                    m_DNA = Activator.CreateInstance( DNAType ) as AnimalDNA;
                    InvalidateDNA();
                }

                return m_DNA;
            }
            set
            {
                AnimalDNA oldValue = m_DNA;

                if( oldValue != value )
                {
                    m_DNA = value;

                    InvalidateDNA();
                }
            }
        }

        /// <summary>
        /// Genetic expression, change creature's characteristics according to DNA
        /// </summary>
        public void InvalidateDNA()
        {
            if( !IsGeneticCreature )
                return;

            SetStr( m_DNA.Str );
            SetDex( m_DNA.Dex );
            SetInt( m_DNA.Int );

            DamageMin = m_DNA.DamageMin;
            DamageMax = m_DNA.DamageMax;

            VirtualArmor = m_DNA.VirtualArmor;

            // this allows to define DNAs that don't change the body
            if( m_DNA.Color != 0 )
                Hue = m_DNA.Color;

            // and hue
            if( m_DNA.Body != 0 )
                Body = m_DNA.Body;

            m_DNA.ApplySpecialCriteria( this );

            // set this creatue as owner of the DNA
            m_DNA.Owner = this;

            ApplyLeveling();
        }

        /// <summary>
        /// Helper function to hybridize parents' DNA
        /// </summary>
        public void SetParents( BaseCreature father, BaseCreature mother ) //mod by magius(CHE)
        {
            if( father.DNA.GetType() != mother.DNA.GetType() || father.DNA.GetType() != DNA.GetType() )
                return;

            object[] parms = new object[] { father.DNA, mother.DNA };

            DNA = Activator.CreateInstance( DNAType, parms ) as AnimalDNA; //mod by magius(CHE)
        }

        public void InitGenetics()
        {
            m_BirthDate = DateTime.Now;

            // generate a new DNA
            DNA = Activator.CreateInstance( DNAType ) as AnimalDNA;

            // reset experience to force stat leveling
            Experience = 0;

            CheckAgeTimer();
        }

        #region Aging
        /// <summary>
        /// How many real worlds seconds make up a UO minute.
        /// This setting cacn be different from Clock.SecondsPerUoMinute to allow
        /// independent tuning of creatures' growth speed
        /// </summary>
        private static double m_SecondsPerUoMinute = Clock.SecondsPerUOMinute / ( Core.Debug ? 120 : 1 );
        private static readonly double m_SecondsPerUoDay = m_SecondsPerUoMinute * 24 * 60;

        /// <summary>
        /// date of birth.
        /// This can be different from CreationDate so that we can manually set the age of creatures
        /// </summary>
        private DateTime m_BirthDate;

        /// <summary>
        /// The creature's Age in UO days
        /// </summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public double Age
        {
            get { return ( ( DateTime.Now - m_BirthDate ).TotalSeconds / m_SecondsPerUoDay ); }
            set
            {
                if( value < 0 )
                    return;

                TimeSpan age = TimeSpan.FromSeconds( value * m_SecondsPerUoDay );
                m_BirthDate = DateTime.Now - age;
            }
        }

        /// <summary>
        /// Aging time constant "tau".
        /// This vale sets the speed at which stats grow with age
        /// </summary>
        public virtual double AgingConstant { get { return 120.0; } }

        /// <summary> 
        /// Aging function:
        /// goes from 0 to 100 when age goes from 0 to infinity
        /// after AgingConstant UO days --> 63%
        /// after 2*AgingConstant UO days --> 84%
        /// after 3*AgingConstant UO days --> 95%
        /// after 4*AgingConstant UO days --> 98%
        /// </summary>
        public virtual int AgeMultiplier { get { return Convert.ToInt32( 100 * ( 1.0 - Math.Exp( -Age / AgingConstant ) ) ); } }

        // Hits, Stamina and Mana are linked to Str, Dex, Int via a different
        // ratio for every creature (for humans 100%)
        public virtual int StrToHits { get { return 100; } }
        public virtual int DexToStam { get { return 100; } }
        public virtual int IntToMana { get { return 100; } }

        /// <summary>
        /// Event called every m_SecondsPerUoDay by the age timer
        /// Provides stat evolution and handles delivery of pregnant females
        /// </summary>
        public virtual void OnAgeTimerTick()
        {
            #region mod by Dies Irae
            if( Core.Debug )
                Utility.Log( "BaseCreatureOnAgeTimerTick.log", "age for: {0}({1}) = {2}", GetType().Name, Serial.ToString(), Math.Floor( Age ).ToString() );
            #endregion

            Experience += DailyExperienceGain;
		//edit by Arlas
            //RawStr = ( m_DNA.Str * AgeMultiplier ) / 100;
            //RawDex = ( m_DNA.Dex * AgeMultiplier ) / 100;
            //RawInt = ( m_DNA.Int * AgeMultiplier ) / 100;

            if( Map != Map.Internal )
                Deliver(); // try to deliver a baby is present (all checks inside)

            if( Age >= AgingConstant )
                ChangeAIToDefault();

            CheckAgeTimer();
        }

        /// <summary>
        /// Makes sure the aging timer is active if needed and inactive if not
        /// </summary>
        public void CheckAgeTimer()
        {
            if( !Tamable )
                return;

            if( !Controlled )
                return;

            if( m_AgeTimer == null )
            {
                if( Age < 10 * AgingConstant || IsPregnant ) // timer must be on during youth and pregnancy
                {
                    m_AgeTimer = new AgeTimer( this, TimeSpan.FromSeconds( m_SecondsPerUoDay ) );
                    m_AgeTimer.Start();
                }
            }
            else if( Age > 10 * AgingConstant && !IsPregnant ) // timer is running, stop it if it's not needed anymore
            {
                m_AgeTimer.Stop();
                m_AgeTimer = null;
            }
        }

        private AgeTimer m_AgeTimer;

        private class AgeTimer : Timer
        {
            private readonly BaseCreature m_Owner;

            public AgeTimer( BaseCreature m, TimeSpan delay )
                : base( delay, delay )
            {
                m_Owner = m;

                #region mod by Dies Irae
                if( m_Owner != null && Core.Debug )
                    Utility.Log( "AgeTimers.log", "New Age timer for craeture " + m_Owner.GetType().Name );
                #endregion
            }

            protected override void OnTick()
            {
                m_Owner.OnAgeTimerTick();
            }
        }
        #endregion

        #region Pregnancy
        /// <summary>
        /// The fetus (internalized)
        /// </summary>
        private BaseCreature m_Fetus;

        /// <summary>
        /// The age (in UO days) at which females can go pregnant
        /// </summary>
        public virtual int PregnancyAge { get { return Debug ? 1 : 120; } }

        /// <summary>
        /// Time needed for fetus delivery, in UO days
        /// </summary>
        public virtual int PregnancyTime { get { return Debug ? 1 : 12 * 7; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsPregnant
        {
            get { return m_Fetus != null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanBePregnant
        {
            get { return Female && Age > PregnancyAge && !IsPregnant; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime DeliveryTime { get; set; }

        public virtual bool Deliver()
        {
            return Deliver( false );
        }

        public const double ChanceOfFetusDeath = 0.15;

        /// <summary>
        /// Try to give birth to the fetus
        /// </summary>
        public virtual bool Deliver( bool force )
        {
            if( m_Fetus == null || ( DateTime.Now < DeliveryTime && !force ) )
                return false;

            if( Map != null && Map != Map.Internal )
            {
                Mobile master = GetMaster();
                if( master == null )
                    return false;

                if( Utility.RandomDouble() < ChanceOfFetusDeath && !force )
                {
                    m_Fetus.Kill();
                    m_Fetus = null;

                    master.LocalOverheadMessage( MessageType.Regular, 0x1F4, true, "Unfortunately the youngling died." );

                    return false;
                }

                m_Fetus.MoveToWorld( master.Location, master.Map );
                m_Fetus.SetControlMaster( master );
                m_Fetus = null;

                master.LocalOverheadMessage( MessageType.Regular, 0x1F4, true, "A new youngling was born. Take care of it!" );

                return true;
            }

            return false;
        }

        /// <summary>
        /// Causes the creature to be pregnant if possible
        /// </summary>
        public virtual bool GetPregnant( BaseCreature father )
        {
            if( !CanBePregnant || father == null || father.Female || father.GetType() != GetType() )
                return false;

            m_Fetus = CreateFetus( this, father );

            m_Fetus.Female = Utility.RandomBool();
            m_Fetus.AI = AIType.AI_Animal;
            m_Fetus.Internalize();

            DeliveryTime = DateTime.Now + TimeSpan.FromSeconds( PregnancyTime * m_SecondsPerUoDay );

            DebugSay( ":-)" );
            father.DebugSay( ":P" );

            CheckAgeTimer();

            return true;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DeliveryTimeInUoDays
        {
            get { return Math.Max( (int)Math.Floor( ( DeliveryTime - DateTime.Now ).TotalSeconds / m_SecondsPerUoDay ), 0 ); }
        }

        public virtual BaseCreature CreateFetus( BaseCreature mother, params BaseCreature[] otherParents )
        {
            #region mod by Magius(CHE): Initialize genetics
            BaseCreature fetus = Activator.CreateInstance( GetType() ) as BaseCreature;
            if( fetus != null )
            {
                if( fetus.IsGeneticCreature && fetus.DNAType != null )
                    fetus.SetParents( otherParents[ 0 ], mother );

                return fetus;
            }
            else
                return null;
            #endregion
        }
        #endregion

        #region Leveling
        /// <summary>
        /// Level at which performancec is equal to OSI
        /// </summary>
        private const int m_OSILevel = 30;

        /// <summary>
        /// XP between levels
        /// It affects the speed at which levels are gained when doing actions
        /// that provide a fixed amount of exp (e.g. daily exp gain)
        /// </summary>
        private const int m_LevelSpacing = 100;

        /// <summary>
        /// exp points
        /// </summary>
        private int m_Experience = -1;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Experience
        {
            get { return m_Experience; }
            set
            {
                if( value < 1 )
                    return;

                int oldValue = m_Experience;

                if( oldValue != value )
                {
                    m_Experience = value;

                    OnExperienceAmountChanged( oldValue );
                }
            }
        }

        public int NextLevelExperience
        {
            get
            {
                return ( GeneticLevel * m_LevelSpacing ) + m_LevelSpacing;
            }
        }

        public virtual void OnExperienceAmountChanged( int oldValue )
        {
            #region mod by Dies Irae
            if( Core.Debug )
                Utility.Log( "BaseCreatureOnExperienceAmountChanged.log", "Creature: {0} Xp: {1}", Serial.ToString(), m_Experience.ToString() );
            #endregion

            int oldLevel = 1 + oldValue / m_LevelSpacing;

            if( GeneticLevel != oldLevel )
                OnLevelChanged( oldLevel );
        }

        /// <summary>
        /// Event called when level changes
        /// </summary>
        public virtual void OnLevelChanged( int oldValue )
        {
            ApplyLeveling();
        }

        /// <summary>
        /// Event called when level changes
        /// </summary>
        public virtual void ApplyLeveling()
        {
            if( IsGeneticCreature )
                VirtualArmor = ( m_DNA.VirtualArmor * LevelMultiplier ) / 100;
        }

        /// <summary>
        /// Creature's Level
        /// </summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int GeneticLevel
        {
            get { return 1 + Experience / m_LevelSpacing; }
            set
            {
                if( value < 1 )
                    return;

                int oldValue = GeneticLevel;

                if( oldValue != value )
                {
                    Experience = ( value - 1 ) * m_LevelSpacing + m_LevelSpacing / 2;
                    OnLevelChanged( oldValue );
                }
            }
        }

        /// <summary>
        /// Value (%) used to scale performance according to level
        /// When Level == m_OSILevel multiplier is 100
        /// </summary>
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int LevelMultiplier
        {
            get { return ( 100 * Level ) / m_OSILevel; }
        }

        /// <summary>
        /// Experience gained every day without doing anything
        /// by default is m_OSILevel/Level which means that without training we can only reach OSI Level.
        /// </summary>
        public virtual int DailyExperienceGain
        {
            get { return m_OSILevel / ( 1 + Level ); }
        }

        /// <summary>
        /// Experience gained when succeding against an opponent
        /// </summary>
        public virtual void SuccessAgainst( BaseCreature opponent )
        {
            Experience += ( 5 * opponent.GeneticLevel ) / Level;
        }

        /// <summary>
        /// Experience gained on failure
        /// </summary>
        public virtual void FailureAgainst( BaseCreature opponent )
        {
            Experience += DailyExperienceGain;
        }
        #endregion
        #endregion

        #region AOS Pet System
        public static bool OldPetLevelingSystemEnabled = true;

        public int RoarAttack { get; set; }

        public int PetPoisonAttack { get; set; }

        public int FireBreathAttack { get; set; }

        public DateTime MatingDelay { get; set; }

        public int Generation { get; set; }

        public int AbilityPoints { get; set; }

        public int Exp { get; set; }

        public int NextLevel { get; set; }

        public int Level { get; set; }

        public int MaxLevel { get; set; }

        public bool AllowMating { get; set; }

        public PetRarity.Rarity RarityLevel { get; set; }

        public int StepsTaken { get; set; }
        #endregion

        #region pre-aos Set[...]
        public DiceRoll DamageDice { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasDamageDice
        {
            get { return DamageDice != null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string DamageDiceString
        {
            get { return DamageDice != null ? DamageDice.ToString() : ""; }
        }

        private static readonly Dictionary<string, DiceRoll> DiceDict = new Dictionary<string, DiceRoll>();

        public void SetDamage( string diceroll )
        {
            if( DiceDict.ContainsKey( diceroll ) )
                DamageDice = DiceDict[ diceroll ];
            else
            {
                DiceRoll roll = new DiceRoll( diceroll );
                DamageDice = DiceDict[ diceroll ] = roll;
            }
        }

        public void SetArmor( string diceroll )
        {
            if( DiceDict.ContainsKey( diceroll ) )
                VirtualArmor = DiceDict[ diceroll ].Roll();
            else
            {
                DiceDict[ diceroll ] = new DiceRoll( diceroll );
                VirtualArmor = DiceDict[ diceroll ].Roll();
            }
        }

        public void SetHits( string diceVal )
        {
            SetHits( DiceRoll.Roll( diceVal ) );
        }

        public void SetStam( string diceVal )
        {
            SetStam( DiceRoll.Roll( diceVal ) );
        }

        public void SetMana( string diceVal )
        {
            SetMana( DiceRoll.Roll( diceVal ) );
        }

        public void SetStr( string diceVal )
        {
            SetStr( DiceRoll.Roll( diceVal ) );
        }

        public void SetDex( string diceVal )
        {
            SetDex( DiceRoll.Roll( diceVal ) );
        }

        public void SetInt( string diceVal )
        {
            SetInt( DiceRoll.Roll( diceVal ) );
        }

        public void SetSkillF( SkillName name, string diceVal )
        {
            Skills[ name ].BaseFixedPoint = DiceRoll.Roll( diceVal );

            if( Skills[ name ].Base > Skills[ name ].Cap )
                Skills[ name ].Cap = Skills[ name ].Base;
        }

        public void SetSkill( SkillName name, string diceVal )
        {
            SetSkill( name, DiceRoll.Roll( diceVal ) );
        }
        #endregion

        #region ITalkingMobile Members
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool SupportSpeech
        {
            get { return TalkingVendors.Core.SupportSpeech( this ); }
        }

        private string m_SpeechName;

        [CommandProperty( AccessLevel.GameMaster )]
        public string SpeechName
        {
            get { return m_SpeechName; }
            set
            {
                string oldValue = m_SpeechName;

                if( oldValue != value )
                {
                    m_SpeechName = value;

                    OnSpeechNameChanged( oldValue );
                }
            }
        }

        public virtual void OnSpeechNameChanged( string oldValue )
        {
            Say( "My new language is {0}", m_SpeechName );
        }

        private static Dictionary<string, string> m_Speech;

        public Dictionary<string, string> Speech
        {
            get
            {
                if( m_Speech == null && SpeechName != null )
                    m_Speech = TalkingVendors.Core.GetSpeech( SpeechName );

                return m_Speech;
            }
        }

        private bool m_SpeechEnabled = true;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsSpeechEnabled
        {
            get { return m_SpeechEnabled; }
            set
            {
                bool oldValue = m_SpeechEnabled;

                if( oldValue != value )
                {
                    OnSpeechEnabledChanged( oldValue );

                    m_SpeechEnabled = value;
                }
            }
        }

        public virtual void OnSpeechEnabledChanged( bool oldValue )
        {
            if( m_SpeechEnabled )
                Say( "Oh yeah. I'll be a happy talker!" );
            else
                Say( "I'm muted." );
        }

        private bool m_CanHaveFriends = true;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanHaveFriends
        {
            get { return m_CanHaveFriends; }
            set
            {
                bool oldValue = m_CanHaveFriends;

                if( oldValue != value )
                {
                    OnCanHaveFriendsChanged( oldValue );

                    m_CanHaveFriends = value;
                }
            }
        }

        public virtual void OnCanHaveFriendsChanged( bool oldValue )
        {
            if( m_CanHaveFriends )
                Say( "I like friends." );
            else
                Say( "I'll be a lone man." );
        }

        private Dictionary<Mobile, int> m_TalkingFriends;

        public Dictionary<Mobile, int> TalkingFriends
        {
            get
            {
                if( m_TalkingFriends == null && m_CanHaveFriends )
                    m_TalkingFriends = new Dictionary<Mobile, int>();

                return m_TalkingFriends;
            }
            set { m_TalkingFriends = value; }
        }

        private DateTime m_NextFriend;

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime NextFriend
        {
            get { return m_NextFriend; }
            set { m_NextFriend = value; }
        }

        private DateTime m_LastGreet;

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastGreet
        {
            get { return m_LastGreet; }
            set { m_LastGreet = value; }
        }
        #endregion

        #region speech system by Derrick and Dies
        private DateTime m_NextWarn;

        public void HandleMovementForSpeech( Mobile m, Point3D oldLocation )
        {
            if( ( !m.Hidden || m.AccessLevel == AccessLevel.Player ) && m.Player && m.Alive && Combatant == null && !Controlled && !Summoned && IsHumanInTown() )
            {
                bool areFacingEachOther = ( m.Direction & Direction.Mask ) == m.GetDirectionTo( this ) &&
                    ( Direction & Direction.Mask ) == GetDirectionTo( m );

                if( areFacingEachOther && InRange( m.Location, 0x05 ) && !InRange( oldLocation, 0x05 ) )
                {
                    if( IsANewFamousFolk( m ) )
                    {
                        DebugSay( "Some one famous is near me..." );
                        if( ResolveNotoLevel( m ) > NotorietyLevel.Anonymous )
                        {
                            SpellHelper.Turn( this, m );
                            Animate( m.Female ? 32 : 33, 5, 1, true, false, 0 ); // bow or salute
                        }
                    }

                    string greet = GetGreetings();
                    if( greet != null && IsSpeechEnabled )
                        Say( greet );
                }
                else if( InRange( m.Location, 0x07 ) && !InRange( oldLocation, 0x07 ) )
                {
                    DebugSay( "Someone is approaching." );
                    UpdateMemoryNotoriety( m );
                }

                if( InRange( oldLocation, 0x05 ) && !InRange( m.Location, 0x05 ) )
                {
                    DebugSay( "Someone is leaving but I will add him to my recent memory." );
                    AddToMemoryRecent( m );
                }
            }
        }

        public bool HasSpeechFragment { get { return PersonalFragmentObj != null; } }

        public virtual SpeechFragment InternalGreetingsFragment { get { return SpecialFragment.Greetings; } }

        public virtual string GetGreetings()
        {
            string keyFound;

            DebugSay( "I'm responding to @InternalGreeting for noto '{0}', att. '{1}', soph. '{2}'", ResolveNotoLevel( FocusMob ), Attitude, Sophistication );

            string resp = InternalGreetingsFragment.GetResponseFragment( Sophistication, Attitude, ResolveNotoLevel( FocusMob ), "@InternalGreeting", null, out keyFound );

            if( resp != null )
                resp = FragmentHelper.ParseResponceMacros( resp, this, FocusMob, JobString );

            return resp;
        }

        public virtual SpeechFragment InternalReHelloFragment { get { return SpecialFragment.Rehello; } }

        public virtual string GetReHello()
        {
            string keyFound;

            DebugSay( "I'm responding to 'Hello' for noto '{0}', att. '{1}', soph. '{2}'", ResolveNotoLevel( FocusMob ), Attitude, Sophistication );

            string resp = InternalReHelloFragment.GetResponseFragment( Sophistication, Attitude, ResolveNotoLevel( FocusMob ), "Hello", null, out keyFound );

            if( resp != null )
                resp = FragmentHelper.ParseResponceMacros( resp, this, FocusMob, JobString );

            return resp;
        }

        public virtual SpeechFragment InternalPersonalSpaceFragment { get { return SpecialFragment.InternalSpace; } }

        public virtual string GetPersonalSpace()
        {
            string keyFound;

            DebugSay( "I'm responding to @InternalPersonalSpace for noto '{0}', att. '{1}', soph. '{2}'", ResolveNotoLevel( FocusMob ), Attitude, Sophistication );

            string resp = InternalPersonalSpaceFragment.GetResponseFragment( Sophistication, Attitude, ResolveNotoLevel( FocusMob ), "@InternalPersonalSpace", null, out keyFound );

            if( resp != null )
                resp = FragmentHelper.ParseResponceMacros( resp, this, FocusMob, JobString );

            return resp;
        }

        public virtual SpeechFragment PersonalFragmentObj { get { return null; } }

        public virtual SpeechFragment LocalFragmentObj
        {
            get
            {
                if( Region is BaseRegion )
                    return ( (BaseRegion)Region ).SpeechFragment;
                else
                    return SpeechFragment.SFBritainnia; // don't return null here without looking at BaseCreature.GetSpeechResponce
            }
        }

        protected virtual SophisticationLevel BaseSophistication { get { return SophisticationLevel.Medium; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public SophisticationLevel Sophistication
        {
            get
            {
                BaseRegion region = Region as BaseRegion;
                if( region == null )
                    return BaseSophistication;

                int soph = region.RegionSophistication;

                if( soph <= (int)SophisticationLevel.Low )
                    return SophisticationLevel.Low;

                if( soph >= (int)SophisticationLevel.High )
                    return SophisticationLevel.High;

                return (SophisticationLevel)soph;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public AttitudeLevel BaseAttitude
        {
            get
            {
                BaseRegion region = Region as BaseRegion;
                if( region == null )
                    return AttitudeLevel.Neutral;

                int att = region.RegionAttitude;

                if( att <= (int)AttitudeLevel.Goodhearted )
                    return AttitudeLevel.Goodhearted;

                if( att >= (int)AttitudeLevel.Wicked )
                    return AttitudeLevel.Wicked;

                return (AttitudeLevel)att;
            }
        }

        // this should be modified though interactions throughout the day. range from -15000 to 15000
        private int m_AttitudePoints = Utility.Random( 500, 1000 );

        [CommandProperty( AccessLevel.GameMaster )]
        public int AttitudePoints
        {
            get { return m_AttitudePoints; }
            set { m_AttitudePoints = value; }
        }

        public void AdjustAttitude( int adjustment )
        {
            AttitudePoints += adjustment;

            DebugSay( "My attitude changed by {0} and it's now {1}.", adjustment, AttitudePoints );

            if( AttitudePoints < -15000 )
                AttitudePoints = -15000;

            if( AttitudePoints > 15000 )
                AttitudePoints = 15000;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public AttitudeLevel Attitude
        {
            get
            {
                int att = (int)BaseAttitude;

                if( AttitudePoints < -5000 )
                {
                    if( AttitudePoints < -10000 )
                        att = att >> 2;
                    else
                        att = att >> 1;
                }
                else if( AttitudePoints > 5000 )
                {
                    if( AttitudePoints < 10000 )
                        att = att << 1;
                    else
                        att = att << 2;
                }

                if( att <= (int)AttitudeLevel.Goodhearted )
                    return AttitudeLevel.Goodhearted;

                if( att >= (int)AttitudeLevel.Wicked )
                    return AttitudeLevel.Wicked;

                return (AttitudeLevel)att;
            }
        }

        public NotorietyLevel ResolveNotoLevel( Mobile focus )
        {
            if( focus == null )
                return NotorietyLevel.Anonymous;

            // karma is -15000 to 15000
            if( focus.Karma < 5000 )
            {
                if( focus.Karma > -5000 )
                    return NotorietyLevel.Anonymous;
                else if( focus.Karma < -12500 )
                    return NotorietyLevel.Infamous;
                else
                    return NotorietyLevel.Outlaw;
            }
            else
            {
                if( focus.Karma < 12500 )
                    return NotorietyLevel.Known;

                return NotorietyLevel.Famous;
            }
        }

        public string GetSpeechResponce( string speech, List<int> keywords, out string keyFound )
        {
            try
            {
                string resp = null;
                keyFound = null;

                NotorietyLevel noto = ResolveNotoLevel( FocusMob );
                AttitudeLevel attitude = Attitude;
                SophisticationLevel sophistication = Sophistication;

                DebugSay( "I'm responding for noto '{0}', attitude '{1}', sophistication '{2}'", noto, attitude, sophistication );

                if( PersonalFragmentObj != null )
                    resp = PersonalFragmentObj.GetResponseFragment( sophistication, attitude, noto, speech, keywords, out keyFound );  // might be null

                if( resp == null )
                    resp = LocalFragmentObj.GetResponseFragment( sophistication, attitude, noto, speech, keywords, out keyFound );

                if( resp != null )
                    resp = FragmentHelper.ParseResponceMacros( resp, this, FocusMob, JobString );

                return resp;
            }
            catch( Exception ex )
            {
                Console.WriteLine( "BaseCreature.GetSpeechResponce Exception:" );
                Console.WriteLine( ex.ToString() );

                keyFound = null;
                return null;
            }
        }

        public virtual string JobString
        {
            get
            {
                if( string.IsNullOrEmpty( Title ) )
                    return "peasant";
                else
                {
                    string job = Title;

                    if( job.Length > 4 && job.StartsWith( "the " ) )
                        job.Remove( 0, 4 );

                    return job;
                }
            }
        }

        public void AddToMemoryRecent( Mobile from )
        {
            if( m_RecentMemory == null )
                m_RecentMemory = new RecentMemory( this );

            m_RecentMemory.AddToMemoryRecent( from );
        }

        public void UpdateMemoryNotoriety( Mobile from )
        {
            if( m_NotorietyMemory == null )
                m_NotorietyMemory = new NotorietyMemory( this );

            m_NotorietyMemory.Update( from );
        }

        private NotorietyMemory m_NotorietyMemory;

        private RecentMemory m_RecentMemory;

        private class RecentMemory
        {
            public static int Length
            {
                get { return m_MemoryList.Count; }
            }

            private BaseCreature Owner { get; set; }
            private static readonly List<Mobile> m_MemoryList = new List<Mobile>();

            public RecentMemory( BaseCreature owner )
            {
                Owner = owner;
            }

            public void AddToMemoryRecent( Mobile from )
            {
                if( Contains( from ) )
                    Remove( from );

                Append( from );
                DebugSay( string.Format( "I am remembering {0}.", from.Name ?? "a strange being" ) );

                EnsureCapacity();
            }

            public static bool Contains( Mobile m )
            {
                foreach( Mobile mobile in m_MemoryList )
                {
                    if( mobile == m )
                        return true;
                }

                return false;
            }

            private void Remove( Mobile m )
            {
                for( int index = 0; index < m_MemoryList.Count; index++ )
                {
                    Mobile mobile = m_MemoryList[ index ];
                    if( mobile == m )
                        m_MemoryList.Remove( mobile );
                }

                DebugSay( string.Format( "I am forgetting {0}.", m.Name ?? "a strange being" ) );
            }

            private void RemoveAt( int index )
            {
                if( index < 0 || index > m_MemoryList.Count )
                    return;

                Mobile mobile = m_MemoryList[ index ];
                m_MemoryList.Remove( mobile );

                DebugSay( string.Format( "I am forgetting the being in position {0}.", index ) );
            }

            private static void Append( Mobile m )
            {
                m_MemoryList.Add( m );
            }

            private void EnsureCapacity()
            {
                if( m_MemoryList.Count > 0x1E )
                    RemoveAt( 0x00 );
            }

            private void DebugSay( string s )
            {
                if( Owner != null && Owner.Debug )
                    Owner.DebugSay( s );
            }

            public void SpamMemory()
            {
                if( Owner == null || Length < 1 )
                    return;

                Owner.Say( "Just a moment... I remember..." );
                foreach( Mobile mobile in m_MemoryList )
                    Owner.Say( mobile.Name ?? "a nonamed being" );
            }
        }

        private class NotorietyMemory
        {
            public static int Length
            {
                get { return m_MemoryNotorietyList.Count; }
            }

            private BaseCreature Owner { get; set; }
            private static readonly List<MemoryInfo> m_MemoryNotorietyList = new List<MemoryInfo>();

            public NotorietyMemory( BaseCreature owner )
            {
                Owner = owner;
            }

            public void Update( Mobile from )
            {
                bool shouldAdd = false;

                if( m_MemoryNotorietyList.Count < 0x1E )
                    shouldAdd = true;

                if( !shouldAdd )
                {
                    foreach( MemoryInfo i in m_MemoryNotorietyList )
                    {
                        if( from.Fame > i.Fame )
                        {
                            DebugSay( "Found someone to forget." );
                            shouldAdd = true;
                        }
                    }
                }

                if( shouldAdd )
                    AddToMemoryNotoriety( from );
            }

            private void AddToMemoryNotoriety( Mobile from )
            {
                if( Contains( from ) )
                {
                    DebugSay( "Found this guy in notoriety memory, erasing." );
                    Remove( from );
                }

                Append( from );
                DebugSay( string.Format( "I am remembering {0}.", from.Name ?? "a strange being" ) );

                m_MemoryNotorietyList.Sort( MemoryInfoComparer.Instance );
                EnsureCapacity();
            }

            public static bool Contains( Mobile m )
            {
                foreach( MemoryInfo i in m_MemoryNotorietyList )
                {
                    if( i.Owner == m )
                        return true;
                }

                return false;
            }

            private void Remove( Mobile m )
            {
                for( int index = 0; index < m_MemoryNotorietyList.Count; index++ )
                {
                    MemoryInfo i = m_MemoryNotorietyList[ index ];
                    if( i.Owner == m )
                        m_MemoryNotorietyList.Remove( i );
                }

                DebugSay( "Removing a forgettable person." );
                DebugSay( string.Format( "I am forgetting {0}.", m.Name ?? "a strange being" ) );
            }

            private void RemoveAt( int index )
            {
                if( index < 0 || index > m_MemoryNotorietyList.Count )
                    return;

                MemoryInfo i = m_MemoryNotorietyList[ index ];
                m_MemoryNotorietyList.Remove( i );

                DebugSay( string.Format( "I am forgetting the being in position {0}.", index ) );
            }

            private static void Append( Mobile m )
            {
                m_MemoryNotorietyList.Add( new MemoryInfo( m ) );
            }

            private void EnsureCapacity()
            {
                if( m_MemoryNotorietyList.Count > 0x1E )
                    RemoveAt( 0x00 );
            }

            private void DebugSay( string s )
            {
                if( Owner != null && Owner.Debug )
                    Owner.DebugSay( s );
            }

            public void SpamMemory()
            {
                if( Owner == null || Length < 1 )
                    return;

                Owner.Say( "Uhmmm you want v.i.p.'s? I remember..." );
                foreach( MemoryInfo info in m_MemoryNotorietyList )
                    Owner.Say( info.ToString() );
            }

            private class MemoryInfoComparer : IComparer<MemoryInfo>
            {
                public static readonly IComparer<MemoryInfo> Instance = new MemoryInfoComparer();

                public int Compare( MemoryInfo x, MemoryInfo y )
                {
                    if( x == null && y == null )
                        return 0;
                    else if( x == null )
                        return -1;
                    else if( y == null )
                        return 1;

                    if( x.Fame == y.Fame )
                        return Insensitive.Compare( x.Owner.Name, y.Owner.Name );
                    else
                        return x.Fame - y.Fame;
                }
            }

            private class MemoryInfo
            {
                public Mobile Owner { get; private set; }
                public int Fame { get; private set; }

                public MemoryInfo( Mobile owner )
                    : this( owner, owner.Fame )
                {
                }

                private MemoryInfo( Mobile owner, int fame )
                {
                    Owner = owner;
                    Fame = fame;
                }

                public override string ToString()
                {
                    return string.Format( "{0} - {1}", Owner.Name ?? "nonamed", Fame );
                }
            }
        }

        public bool IsInRecentMemory( Mobile m )
        {
            return m_RecentMemory != null && RecentMemory.Contains( m );
        }

        public bool IsInNotoMemory( Mobile m )
        {
            return m_NotorietyMemory != null && NotorietyMemory.Contains( m );
        }

        public bool IsANewFamousFolk( Mobile m )
        {
            return !IsInRecentMemory( m ) && IsInNotoMemory( m );
        }

        public void SpamRecentMemory()
        {
            if( m_RecentMemory == null || RecentMemory.Length == 0 )
                Say( "But I have noone to remember. It's sad, isn't it?" );
            else
                m_RecentMemory.SpamMemory();
        }

        public void SpamNotoMemory()
        {
            if( m_NotorietyMemory == null || NotorietyMemory.Length == 0 )
                Say( "I do not know any famous person. I'm sorry." );
            else
                m_NotorietyMemory.SpamMemory();
        }
        #endregion

        #region enticement
        const string StartEnticeSpeech = "What is that lovely music I'm hearing?";
        const string EndEnticeSpeech = "Oh! It was you playing that lovely music!";

        [CommandProperty( AccessLevel.GameMaster )]
        public FightMode CurrentFightMode { get; set; }

        public virtual void Enticed( Mobile from )
        {
            if( FightMode != FightMode.None )
                CurrentFightMode = FightMode;

            IsEnticed = true;

            Combatant = null;
            Warmode = false;
            FightMode = FightMode.None;

            ControlOrder = OrderType.None;

            if( SpeechType != null )
                SpeechType.ConstructSentance( 7 );
            else if( Body.IsHuman )
                Say( StartEnticeSpeech );
            else if( BaseSoundID != 0 )
                PlaySound( BaseSoundID );

            new EnticementTimer( this, from, m_AI ).Start();
        }

        private class EnticementTimer : Timer
        {
            private readonly BaseCreature m_Enticed;
            private readonly Mobile m_Enticer;
            private int m_Count;
            private readonly BaseAI m_EnticedAI;

            public EnticementTimer( BaseCreature enticed, Mobile enticer, BaseAI enticedAI )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Enticed = enticed;
                m_Enticer = enticer;
                m_EnticedAI = enticedAI;
                m_Count = 0;
            }

            protected override void OnTick()
            {
                if( m_Enticer == null || m_Enticed.Deleted )
                {
                    Stop();
                    return;
                }

                if( m_Enticer.Deleted )
                {
                    Stop();
                    m_Enticed.IsEnticed = false;
                    return;
                }

                if( m_Enticed.IsEnticed && m_Count < 10 )
                {
                    m_Count++;

                    if( m_Enticer.InRange( m_Enticed, 10 ) )
                    {
                        if( m_Enticed.InRange( m_Enticer.Location, 1 ) && m_Enticed.CanSee( m_Enticer ) )
                        {
                            m_Enticed.DebugSay( "I'm arrived." );
                            m_Enticed.Direction = m_Enticed.GetDirectionTo( m_Enticer.Location );

                            m_Enticed.SayEnticementString( EndEnticeSpeech );

                            m_Enticed.IsEnticed = false;
                        }
                        else
                        {
                            m_Enticed.DebugSay( "I will walk towards my enticer." );
                            m_EnticedAI.WalkMobileRange( m_Enticer, 5, false, 1, 1 );
                        }
                    }
                    else
                    {
                        if( m_Enticed != null && !m_Enticed.Deleted )
                            m_Enticed.DebugSay( "I am no longer enticed or my target is out of range." );
                    }
                }
                else
                {
                    m_Enticed.DebugSay( "I am no longer enticed." );

                    Stop();
                    m_Enticed.IsEnticed = false;
                }
            }
        }

        private bool m_IsEnticed = false;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsEnticed
        {
            get { return m_IsEnticed; }
            set
            {
                bool oldValue = m_IsEnticed;

                if( oldValue != value )
                {
                    m_IsEnticed = value;
                    OnIsEnticedChanged( oldValue );
                }
            }
        }

        public virtual void OnIsEnticedChanged( bool oldvalue )
        {
            DebugSay( "I am no longer enticed." );

            FightMode = CurrentFightMode;
        }

        public virtual void SayEnticementString( string message )
        {
            if( !string.IsNullOrEmpty( message ) )
            {
                if( SpeechType != null )
                    SpeechType.ConstructSentance( Math.Max( 1, message.Split( ' ' ).Length ) );
                else if( Body.IsHuman )
                    Say( message );
                else if( BaseSoundID != 0 )
                    PlaySound( BaseSoundID );
            }
        }
        #endregion

        #region layer confliction
        public void VerifyLayersConfliction()
        {
            foreach( Item item in Items )
            {
                if( item == null || item.Deleted )
                    continue;

                LogAndFixLayerConfliction( item );
            }
        }

        public void LogAndFixLayerConfliction( Item item )
        {
            foreach( Item temp in Items )
            {
                if( temp == item )
                    continue;

                if( temp.CheckConflictingLayer( this, item, item.Layer ) || item.CheckConflictingLayer( this, temp, temp.Layer ) )
                {
                    Console.WriteLine( "Warning: confliting items on layer \"{0}\" for {1}", item.Layer, GetType().Name );

                    using( StreamWriter op = new StreamWriter( "Logs/creature-layer-confliction.log", true ) )
                    {
                        op.WriteLine( "Conflicting layer for creature: {0}", GetType().Name );
                        op.WriteLine( "Wrong Layer: {0}", item.Layer );
                        op.WriteLine( "\tItem 1: {0}", item.GetType().Name );
                        op.WriteLine( "\tItem 2: {0}", temp.GetType().Name );
                        op.WriteLine( "" );
                    }

                    if( Backpack != null && item != Backpack )
                        Backpack.DropItem( item );
                    else
                    {
                        if( !item.Deleted )
                            item.Delete();
                        else if( !temp.Deleted )
                            temp.Delete();
                    }
                }
            }
        }

        public override void AddItem( Item item )
        {
            if( !CheckEquip( item ) )
            {
                using( StreamWriter op = new StreamWriter( Path.Combine( "Logs", "creature-layer-errors.log" ), true ) )
                {
                    op.WriteLine( "Warning: conflicting item for creature: {0} ( s: {1} loc {2} )", GetType().Name, Serial, Location );
                    op.WriteLine( "\tItem: {0} - Layer {1}", item.GetType().Name, item.Layer );

                    op.WriteLine( "" );
                }

                if( !item.Deleted )
                    item.Delete();
            }

            base.AddItem( item );
        }
        #endregion

        protected static double FeatherScalar = 0.30;

        [CommandProperty( AccessLevel.GameMaster )]
        public string OriginalName { get; set; }

        public void UpdateOriginalName()
        {
            if( !Tamable )
                return;

            if( !string.IsNullOrEmpty( OriginalName ) )
                return;

            try
            {
                BaseCreature dummy = Activator.CreateInstance( GetType() ) as BaseCreature;
                if( dummy != null && !string.IsNullOrEmpty( dummy.Name ) )
                    OriginalName = dummy.Name;

                if( dummy != null && !dummy.Deleted )
                    dummy.Delete();
            }
            catch
            {
            }
        }

		public override void OnBeneficialAction( Mobile target, bool isCriminal )
		{
			//if( isCriminal )
			//	CriminalAction( false );
		}

        public override void OnRawStrChange( int oldValue )
        {
            if( !Summoned && Controlled )
                Say( true, string.Format( "* {0} strength {1} *", Name ?? "Your friend", RawStr > oldValue ? "increased" : "decreased" ) );

            base.OnRawStrChange( oldValue );
        }

        public override void OnRawIntChange( int oldValue )
        {
            if( !Summoned && Controlled )
                Say( true, string.Format( "* {0} intelligence {1} *", Name ?? "Your friend", RawInt > oldValue ? "increased" : "decreased" ) );

            base.OnRawIntChange( oldValue );
        }

        public override void OnRawDexChange( int oldValue )
        {
            if( !Summoned && Controlled )
                Say( true, string.Format( "* {0} dexterity {1} *", Name ?? "Your friend", RawDex > oldValue ? "increased" : "decreased" ) );

            base.OnRawDexChange( oldValue );
        }

        [CommandProperty( AccessLevel.Developer )]
        public virtual int StoneMaxWeight
        {
            get { return 1600; }
        }
        #endregion

        public bool IsPlayerSummoned
        {
            get { return Summoned && SummonMaster != null && SummonMaster.Player; }
        }

        public bool WasNamed( string speech )
        {
            string name = Name;

            return ( name != null && Insensitive.StartsWith( speech, name ) );
        }

        #region custom loot options
        [CommandProperty( AccessLevel.GameMaster )]
        public bool LootNoGold { get; set; }
        #endregion
    }

	public class LoyaltyTimer : Timer
	{
		private static TimeSpan InternalDelay = TimeSpan.FromMinutes( 5.0 );

		public static void Initialize()
		{
			new LoyaltyTimer().Start();
		}

		public LoyaltyTimer() : base( InternalDelay, InternalDelay )
		{
			m_NextHourlyCheck = DateTime.Now + TimeSpan.FromHours( 1.0 );
			Priority = TimerPriority.FiveSeconds;
		}

		private DateTime m_NextHourlyCheck;

		protected override void OnTick() 
		{
			if ( DateTime.Now >= m_NextHourlyCheck )
				m_NextHourlyCheck = DateTime.Now + TimeSpan.FromHours( 1.0 );
			else
				return;

			List<BaseCreature> toRelease = new List<BaseCreature>();

			// added array for wild creatures in house regions to be removed
			List<BaseCreature> toRemove = new List<BaseCreature>();

			foreach ( Mobile m in World.Mobiles.Values )
			{
			    #region mod by Dies Irae
			    if( m.Blessed )
			        continue;
			    #endregion

			    if ( m is BaseMount && ((BaseMount)m).Rider != null )
				{
					((BaseCreature)m).OwnerAbandonTime = DateTime.MinValue;
					continue;
				}

				if ( m is BaseCreature )
				{
					BaseCreature c = (BaseCreature)m;

					if ( c.IsDeadPet )
					{
						Mobile owner = c.ControlMaster;

						if ( owner == null || owner.Deleted || owner.Map != c.Map || !owner.InRange( c, 12 ) || !c.CanSee( owner ) || !c.InLOS( owner ) )
						{
							if ( c.OwnerAbandonTime == DateTime.MinValue )
								c.OwnerAbandonTime = DateTime.Now;
							else if ( (c.OwnerAbandonTime + c.BondingAbandonDelay) <= DateTime.Now )
								toRemove.Add( c );
						}
						else
						{
							c.OwnerAbandonTime = DateTime.MinValue;
						}
					}
					else if ( c.Controlled && c.Commandable )
					{
						c.OwnerAbandonTime = DateTime.MinValue;
						
						if ( c.Map != Map.Internal )
						{
							c.Loyalty -= (BaseCreature.MaxLoyalty / 10);

							if( c.Loyalty < (BaseCreature.MaxLoyalty / 10) )
							{
								c.Say( 1043270, c.Name ); // * ~1_NAME~ looks around desperately *
								c.PlaySound( c.GetIdleSound() );
							}

							if ( c.Loyalty <= 0 )
								toRelease.Add( c );
						}
					}

					// added lines to check if a wild creature in a house region has to be removed or not
					if ( (!c.Controlled && ( c.Region.IsPartOf( typeof( HouseRegion ) ) && c.CanBeDamaged()) || ( c.RemoveIfUntamed && c.Spawner == null )) )
					{
						c.RemoveStep++;

						if ( c.RemoveStep >= 20 )
							toRemove.Add( c );
					}
					else
					{
						c.RemoveStep = 0;
					}
				}
			}

			foreach ( BaseCreature c in toRelease )
			{
                c.OnBadRelease(); // mod by Dies Irae

				c.Say( 1043255, c.Name ); // ~1_NAME~ appears to have decided that is better off without a master!
				c.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
				c.IsBonded = false;
				c.BondingBegin = DateTime.MinValue;
				c.OwnerAbandonTime = DateTime.MinValue;
				c.ControlTarget = null;
				//c.ControlOrder = OrderType.Release;
				c.AIObject.DoOrderRelease(); // this will prevent no release of creatures left alone with AI disabled (and consequent bug of Followers)
			}

			// added code to handle removing of wild creatures in house regions
			foreach ( BaseCreature c in toRemove )
			{
				c.Delete();
			}
		}
	}
}