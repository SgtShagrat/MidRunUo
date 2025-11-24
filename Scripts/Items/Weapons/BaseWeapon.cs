using System;
using System.Collections.Generic;
using System.Text;

using Midgard;

using Server.Engines.Craft;
using Server.Engines.XmlSpawner2;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Engines.Harvest;

using Midgard.Engines.Classes;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.MonsterMasterySystem;
using Midgard.Engines.OldCraftSystem;
using Midgard.Engines.Races;
using Midgard.Engines.SpellSystem;
using Midgard.Engines.StoneEnchantSystem;
using Midgard.Items;
using Server.Spells.Bushido;
using ReaperFormSpell = Midgard.Engines.SpellSystem.ReaperFormSpell;
using Midgard.Engines.FeedStockRecoverySystem;

namespace Server.Items
{
	public interface ISlayer
	{
		SlayerName Slayer { get; set; }
		SlayerName Slayer2 { get; set; }
	}

	public abstract class BaseWeapon : Item, IWeapon, /* IFactionItem, */ ICraftable, ISlayer, IDurability, IEngravable, ISetItem, IStoneEnchantItem, IIdentificable, IRepairable, ISmeltable, IRecyclable, IResourceItem
	{
		private string m_EngravedText;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public string EngravedText
		{
			get{ return m_EngravedText; }
			set{ m_EngravedText = value; InvalidateProperties(); }
		}

#if false
		#region Factions
		private FactionItem m_FactionState;

		public FactionItem FactionItemState
		{
			get{ return m_FactionState; }
			set
			{
				m_FactionState = value;

				if ( m_FactionState == null )
					Hue = CraftResources.GetHue( Resource );

				LootType = ( m_FactionState == null ? LootType.Regular : LootType.Blessed );
			}
		}
		#endregion
#endif

		/* Weapon internals work differently now (Mar 13 2003)
		 * 
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - MinDamage
		 *  - MaxDamage
		 *  - Speed
		 *  - HitSound
		 *  - MissSound
		 *  - StrRequirement, DexRequirement, IntRequirement
		 *  - WeaponType
		 *  - WeaponAnimation
		 *  - MaxRange
		 */

		#region Var declarations

		// Instance values. These values are unique to each weapon.
		private WeaponDamageLevel m_DamageLevel;
		private WeaponAccuracyLevel m_AccuracyLevel;
		private WeaponDurabilityLevel m_DurabilityLevel;
        private WeaponQuality m_Quality; // TODO: implementare CraftQuality
		private Mobile m_Crafter;
		private Poison m_Poison;
		private int m_PoisonCharges;
		// private bool m_Identified; // mod by Dies Irae
		private int m_Hits;
		private int m_MaxHits;
		private SlayerName m_Slayer;
		private SlayerName m_Slayer2;

		private SkillMod m_SkillMod, m_MageMod;
		private CraftResource m_Resource;
		private bool m_PlayerConstructed;

		private bool m_Cursed; // Is this weapon cursed via Curse Weapon necromancer spell? Temporary; not serialized.
		private bool m_Consecrated; // Is this weapon blessed via Consecrate Weapon paladin ability? Temporary; not serialized.

		private AosAttributes m_AosAttributes;
		private AosWeaponAttributes m_AosWeaponAttributes;
		private AosSkillBonuses m_AosSkillBonuses;
		private AosElementAttributes m_AosElementDamages;

		// Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
		private int m_StrReq, m_DexReq, m_IntReq;
		private int m_MinDamage, m_MaxDamage;
		private int m_HitSound, m_MissSound;
		private float m_Speed;
		private int m_MaxRange;
		private SkillName m_Skill;
		private WeaponType m_Type;
		private WeaponAnimation m_Animation;
		#endregion

		#region Virtual Properties
		public virtual WeaponAbility PrimaryAbility{ get{ return null; } }
		public virtual WeaponAbility SecondaryAbility{ get{ return null; } }

		public virtual int DefMaxRange{ get{ return 1; } }
		public virtual int DefMinRange{ get{ return 0; } }//edit by Arlas
		public virtual int DefHitSound{ get{ return 0; } }
		public virtual int DefMissSound{ get{ return 0; } }
		public virtual SkillName DefSkill{ get{ return SkillName.Swords; } }
		public virtual WeaponType DefType{ get{ return WeaponType.Slashing; } }
		public virtual WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

		public virtual int AosStrengthReq{ get{ return 0; } }
		public virtual int AosDexterityReq{ get{ return 0; } }
		public virtual int AosIntelligenceReq{ get{ return 0; } }
		public virtual int AosMinDamage{ get{ return 0; } }
		public virtual int AosMaxDamage{ get{ return 0; } }
		public virtual int AosSpeed{ get{ return 0; } }
		public virtual float MlSpeed{ get{ return 0.0f; } }
		public virtual int AosMaxRange{ get{ return DefMaxRange; } }
		public virtual int AosHitSound{ get{ return DefHitSound; } }
		public virtual int AosMissSound{ get{ return DefMissSound; } }
		public virtual SkillName AosSkill{ get{ return DefSkill; } }
		public virtual WeaponType AosType{ get{ return DefType; } }
		public virtual WeaponAnimation AosAnimation{ get{ return DefAnimation; } }

		public virtual int OldStrengthReq{ get{ return 0; } }
		public virtual int OldDexterityReq{ get{ return 0; } }
		public virtual int OldIntelligenceReq{ get{ return 0; } }
		// public virtual int OldMinDamage{ get{ return 0; } } // mod by Dies Irae
        	// public virtual int OldMaxDamage{ get{ return 0; } } // mod by Dies Irae
		public virtual int OldSpeed{ get{ return 0; } }
		public virtual int OldMaxRange{ get{ return DefMaxRange; } }
		public virtual int OldMinRange{ get{ return DefMinRange; } } // mod by Arlas
		public virtual int OldHitSound{ get{ return DefHitSound; } }
		public virtual int OldMissSound{ get{ return DefMissSound; } }
		public virtual SkillName OldSkill{ get{ return DefSkill; } }
		public virtual WeaponType OldType{ get{ return DefType; } }
		public virtual WeaponAnimation OldAnimation{ get{ return DefAnimation; } }

		public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }

		public virtual bool CanFortify{ get{ return true; } }

		public override int PhysicalResistance{ get{ return m_AosWeaponAttributes.ResistPhysicalBonus; } }
		public override int FireResistance{ get{ return m_AosWeaponAttributes.ResistFireBonus; } }
		public override int ColdResistance{ get{ return m_AosWeaponAttributes.ResistColdBonus; } }
		public override int PoisonResistance{ get{ return m_AosWeaponAttributes.ResistPoisonBonus; } }
		public override int EnergyResistance{ get{ return m_AosWeaponAttributes.ResistEnergyBonus; } }

		public virtual SkillName AccuracySkill { get { return SkillName.Tactics; } }
		#endregion

		#region Getters & Setters
		// [CommandProperty( AccessLevel.GameMaster )] // mod by Dies Irae
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		// [CommandProperty( AccessLevel.GameMaster )] // mod by Dies Irae
		public AosWeaponAttributes WeaponAttributes
		{
			get{ return m_AosWeaponAttributes; }
			set{}
		}

		// [CommandProperty( AccessLevel.GameMaster )] // mod by Dies Irae
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		// [CommandProperty( AccessLevel.GameMaster )] // mod by Dies Irae
		public AosElementAttributes AosElementDamages
		{
			get { return m_AosElementDamages; }
			set { }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Cursed
		{
			get{ return m_Cursed; }
			set{ m_Cursed = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Consecrated
		{
			get{ return m_Consecrated; }
			set{ m_Consecrated = value; }
		}

#if false
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Identified
		{
			get{ return m_Identified; }
			set{ m_Identified = value; InvalidateProperties(); }
		}
#endif

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get{ return m_Hits; }
			set
			{
				if ( m_Hits == value )
					return;

				if ( value > m_MaxHits )
					value = m_MaxHits;

				m_Hits = value;

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHits; }
			set{ m_MaxHits = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonCharges
		{
			get{ return m_PoisonCharges; }
			set{ m_PoisonCharges = value; /* InvalidateProperties(); */ InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Poison Poison
		{
			get{ return m_Poison; }
            set{ m_Poison = value; /* InvalidateProperties(); */ InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponQuality Quality
		{
			get
			{
				#region mod by Dies Irae
				if( CustomQuality >= Server.Quality.Exceptional )
					return WeaponQuality.Exceptional;
				else if( CustomQuality <= Server.Quality.Low )
					return WeaponQuality.Low;
				#endregion

				return m_Quality;
			}
			set{ UnscaleDurability(); m_Quality = value; ScaleDurability(); /* InvalidateProperties(); */ InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); InvalidateSecondAgeNames(); SingleClickChanged(); } 
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer
		{
			get{ return m_Slayer; }
            set { m_Slayer = value; InvalidateProperties(); InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer2
		{
			get { return m_Slayer2; }
            set { m_Slayer2 = value; InvalidateProperties(); InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TalismanSlayerName Slayer3
		{
			get { return m_Slayer3; }
			set { m_Slayer3 = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
            set { UnscaleDurability(); m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateProperties(); ScaleDurability(); InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponDamageLevel DamageLevel
		{
			get{ return m_DamageLevel; }
			set{ m_DamageLevel = value; InvalidateProperties(); InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponDurabilityLevel DurabilityLevel
		{
			get{ return m_DurabilityLevel; }
			set{ UnscaleDurability(); m_DurabilityLevel = value; InvalidateProperties(); ScaleDurability(); InvalidateSecondAgeNames(); SingleClickChanged(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxRange
		{
			get{ return ( m_MaxRange == -1 ? Core.AOS ? AosMaxRange : OldMaxRange : m_MaxRange ); }
			set{ m_MaxRange = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponAnimation Animation
		{
			get{ return ( m_Animation == (WeaponAnimation)(-1) ? Core.AOS ? AosAnimation : OldAnimation : m_Animation ); } 
			set{ m_Animation = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponType Type
		{
			get{ return ( m_Type == (WeaponType)(-1) ? Core.AOS ? AosType : OldType : m_Type ); }
			set{ m_Type = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get{ return ( m_Skill == (SkillName)(-1) ? Core.AOS ? AosSkill : OldSkill : m_Skill ); }
			set{ m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitSound
		{
			get{ return ( m_HitSound == -1 ? Core.AOS ? AosHitSound : OldHitSound : m_HitSound ); }
			set{ m_HitSound = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MissSound
		{
			get{ return ( m_MissSound == -1 ? Core.AOS ? AosMissSound : OldMissSound : m_MissSound ); }
			set{ m_MissSound = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinDamage
		{
			get{ return ( m_MinDamage == -1 ? Core.AOS ? AosMinDamage : OldMinDamage : m_MinDamage ); }
			set{ m_MinDamage = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxDamage
		{
			get{ return ( m_MaxDamage == -1 ? Core.AOS ? AosMaxDamage : OldMaxDamage : m_MaxDamage ); }
			set{ m_MaxDamage = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public float Speed
		{
			get
			{
				#region mod by Dies Irae
				// FUCK runuo team. really fuck! by Dies Irae
				if( m_Speed == -1 )
				{
					if( Core.ML )
						return MlSpeed;
					else if( Core.AOS )
						return AosSpeed;
					else
						return OldSpeed;
				}
				#endregion
				/*mod by Arlas: non verranno mai eseguiti:
				if ( m_Speed != -1 )
					return m_Speed;

				if ( Core.ML )
					return MlSpeed;
				else if ( Core.AOS )
					return AosSpeed;
				*/
				return m_Speed;
			}
			set{ m_Speed = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get{ return ( m_StrReq == -1 ? Core.AOS ? AosStrengthReq : OldStrengthReq : m_StrReq ); }
			set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexRequirement
		{
			get{ return ( m_DexReq == -1 ? Core.AOS ? AosDexterityReq : OldDexterityReq : m_DexReq ); }
			set{ m_DexReq = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntRequirement
		{
			get{ return ( m_IntReq == -1 ? Core.AOS ? AosIntelligenceReq : OldIntelligenceReq : m_IntReq ); }
			set{ m_IntReq = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public WeaponAccuracyLevel AccuracyLevel
		{
			get
			{
				return m_AccuracyLevel;
			}
			set
			{
				if ( m_AccuracyLevel != value )
				{
					m_AccuracyLevel = value;

					if ( UseSkillMod )
					{
						if ( m_AccuracyLevel == WeaponAccuracyLevel.Regular )
						{
							if ( m_SkillMod != null )
								m_SkillMod.Remove();

							m_SkillMod = null;
						}
						else if ( m_SkillMod == null && Parent is Mobile )
						{
							m_SkillMod = new DefaultSkillMod( AccuracySkill, true, /* (int)m_AccuracyLevel * 5 */ AccuracyBonus );
							((Mobile)Parent).AddSkillMod( m_SkillMod );
						}
						else if ( m_SkillMod != null )
						{
							m_SkillMod.Value = /* (int)m_AccuracyLevel * 5 */ AccuracyBonus;
						}
					}

					InvalidateProperties();
                    InvalidateSecondAgeNames(); // mod by Dies Irae
                    SingleClickChanged(); 
				}
			}
		}

		#endregion

		public override void OnAfterDuped( Item newItem )
		{
			BaseWeapon weap = newItem as BaseWeapon;

			if ( weap == null )
				return;

			weap.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			weap.m_AosElementDamages = new AosElementAttributes( newItem, m_AosElementDamages );
			weap.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
			weap.m_AosWeaponAttributes = new AosWeaponAttributes( newItem, m_AosWeaponAttributes );

			#region Mondain's Legacy
			weap.m_SetAttributes = new AosAttributes( newItem, m_SetAttributes );
			weap.m_SetSkillBonuses = new AosSkillBonuses( newItem, m_SetSkillBonuses );
			#endregion
		}

		public virtual void UnscaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_Hits = ((m_Hits * 100) + (scale - 1)) / scale;
			m_MaxHits = ((m_MaxHits * 100) + (scale - 1)) / scale;
			InvalidateProperties();
		}

		public virtual void ScaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_Hits = ((m_Hits * scale) + 99) / 100;
			m_MaxHits = ((m_MaxHits * scale) + 99) / 100;
			InvalidateProperties();
		}

		public int GetDurabilityBonus()
		{
			int bonus = 0;

            	/*
			if ( Core.AOS && m_Quality == WeaponQuality.Exceptional )
				bonus += 20;
			else
				bonus += OldScaleDurabilityByQuality();
            	*/

            	bonus += OldScaleDurabilityByQuality(); // mod by Dies Irae

			switch ( m_DurabilityLevel )
			{
				case WeaponDurabilityLevel.Durable: bonus += 20; break;
				case WeaponDurabilityLevel.Substantial: bonus += 50; break;
				case WeaponDurabilityLevel.Massive: bonus += 70; break;
				case WeaponDurabilityLevel.Fortified: bonus += 100; break;
				case WeaponDurabilityLevel.Indestructible: bonus += 120; break;
			}

            	/*
			if ( Core.AOS )
			{
				bonus += m_AosWeaponAttributes.DurabilityBonus;

				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
				CraftAttributeInfo attrInfo = null;

				if ( resInfo != null )
					attrInfo = resInfo.AttributeInfo;

				if ( attrInfo != null )
					bonus += attrInfo.WeaponDurability;
			}
            	*/

			#region mod by Dies Irae
			if( ResourceInfo != null )
				bonus += ResourceInfo.WeaponDurability;
			#endregion

			return bonus;
		}

		public int GetLowerStatReq()
		{
		    	return 0;

		    	/*
			if ( !Core.AOS )
				return 0;

			int v = m_AosWeaponAttributes.LowerStatReq;

			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info != null )
			{
				CraftAttributeInfo attrInfo = info.AttributeInfo;

				if ( attrInfo != null )
					v += attrInfo.WeaponLowerRequirements;
			}

			if ( v > 100 )
				v = 100;

			return v;
            	*/
		}

		public static void BlockEquip( Mobile m, TimeSpan duration )
		{
			if ( m.BeginAction( typeof( BaseWeapon ) ) )
				new ResetEquipTimer( m, duration ).Start();
		}

		private class ResetEquipTimer : Timer
		{
			private Mobile m_Mobile;

			public ResetEquipTimer( Mobile m, TimeSpan duration ) : base( duration )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				m_Mobile.EndAction( typeof( BaseWeapon ) );
			}
		}

		public override bool CheckConflictingLayer( Mobile m, Item item, Layer layer )
		{
			if ( base.CheckConflictingLayer( m, item, layer ) )
				return true;

			if ( this.Layer == Layer.TwoHanded && layer == Layer.OneHanded )
			{
				m.SendLocalizedMessage( 500214 ); // You already have something in both hands.
				return true;
			}
			else if ( this.Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight) )
			{
				m.SendLocalizedMessage( 500215 ); // You can only wield one weapon at a time.
				return true;
			}

			return false;
		}

		public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			if ( !Ethics.Ethic.CheckTrade( from, to, newOwner, this ) )
				return false;

			return base.AllowSecureTrade( from, to, newOwner, accepted );
		}

		public virtual Race RequiredRace { get { return null; } }	//On OSI, there are no weapons with race requirements, this is for custom stuff

		public override bool CanEquip( Mobile from )
		{
			#region mod by Dies Irae
			if( this is IPracticeWeapon && from.Skills[ DefSkill ].Base > 50.0 )
			{
				from.SendMessage( from.Language == "ITA" ? "Non puoi più usare questa arma." : "You cannot use this item anymore." );
				return false;
			}

			if( this is IDoubleWeapon && from.Skills[ DefSkill ].Base < 90.0 )
			{
				from.SendMessage( from.Language == "ITA" ? "Non puoi usare questo oggetto." : "You cannot use this item." );
				return false;
			}

			if( from.Skills[ SkillName.Magery ].Base >= 50.0 && BlockCircle != -1 )
			{
				from.SendMessage( from.Language == "ITA" ? "Non puoi usare questo oggetto a causa dei tuoi poteri magici." : "You cannot use this item because of your magical power." );
				return false;
			}

			if( !RaceAllowanceAttribute.IsAllowed( this, from, Midgard.Engines.Races.Core.MountainDwarf, true ) )
				return false;

			if( RequiredTownSystem != null && TownHelper.CheckEquip( from, RequiredTownSystem, true ) )
				return false;

			if( this is ITreasureOfMidgard && !XmlBlessedCursedAttach.CanEquip( from, this ) )
 				return false;

			if( from.Player && RequiredRace != null )
			{
				if( !MidgardRace.CheckEquip( RequiredRace, from, true ) )
					return false;
			}

            if( from.Player )
            {
                ClassSystem classSys = ClassSystem.Find( from );
                if( classSys != null && !classSys.CanEquipWeapon( from, this, true ) )
                    return false;
            }
			#endregion

			/*
			if ( !Ethics.Ethic.CheckEquip( from, this ) )
				return false;

			if( RequiredRace != null && from.Race != RequiredRace )
			{
				if( RequiredRace == Race.Elf )
					from.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
				else
					from.SendMessage( from.Language == "ITA" ? "Solo i {0} possono usarlo." : "Only {0} may use this.", RequiredRace.PluralName );

				return false;
			}
			else */ if ( from.Dex < DexRequirement )
			{
				from.SendMessage( from.Language == "ITA" ? "Non sei abbastanza agile per indossarlo." : "You are not nimble enough to equip that." );
				return false;
			} 
			else if ( from.Str < AOS.Scale( StrRequirement, 100 - GetLowerStatReq() ) )
			{
				from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
				return false;
			}
			else if ( from.Int < IntRequirement )
			{
				from.SendMessage( from.Language == "ITA" ? "Non sei abbastanza intelligente per indossarlo." : "You are not smart enough to equip that." );
				return false;
			}
			else if ( !from.CanBeginAction( typeof( BaseWeapon ) ) )
			{
				return false;
			}
			else
			{
			    return XmlAttach.CheckCanEquip( this, from ) && base.CanEquip( from ); // ARTEGORDONMOD XmlAttachment check for CanEquip
			}
		}

		public virtual bool UseSkillMod{ get{ return !Core.AOS; } }

		public override bool OnEquip( Mobile from )
		{
			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = from;

				string modName = this.Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			from.NextCombatTime = DateTime.Now + GetDelay( from );

			if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular )
			{
				if ( m_SkillMod != null )
					m_SkillMod.Remove();

				m_SkillMod = new DefaultSkillMod( AccuracySkill, true, /* (int)m_AccuracyLevel * 5 */ AccuracyBonus );
				from.AddSkillMod( m_SkillMod );
			}

            	/*
			if ( Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30 )
			{
				if ( m_MageMod != null )
					m_MageMod.Remove();

				m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon );
				from.AddSkillMod( m_MageMod );
			}
            	*/

			XmlAttach.CheckOnEquip( this, from ); // ARTEGORDONMOD XmlAttachment check for OnEquip

			return true;
		}

		public override void OnAdded( object parent )
		{
			base.OnAdded( parent );

			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

                		/*
				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( from );
                		*/

				#region Mondain's Legacy Sets
				if ( IsSetItem )
				{
					m_SetEquipped = SetHelper.FullSetEquipped( from, SetID, Pieces );
				
					if ( m_SetEquipped )
					{
						m_LastEquipped = true;						
						SetHelper.AddSetBonus( from, SetID );
					}
				}
				#endregion

				from.CheckStatTimers();
				from.Delta( MobileDelta.WeaponDamage );

				#region mod by Dies Irae
				if( StoneEnchantItemState != null )
					StoneEnchantItemState.Invalidate( from, true );
				#endregion
			}
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile m = (Mobile)parent;
				BaseWeapon weapon = m.Weapon as BaseWeapon;

				string modName = this.Serial.ToString();

				m.RemoveStatMod( modName + "Str" );
				m.RemoveStatMod( modName + "Dex" );
				m.RemoveStatMod( modName + "Int" );

				if ( weapon != null )
					m.NextCombatTime = DateTime.Now + weapon.GetDelay( m );

				if ( UseSkillMod && m_SkillMod != null )
				{
					m_SkillMod.Remove();
					m_SkillMod = null;
				}

				if ( m_MageMod != null )
				{
					m_MageMod.Remove();
					m_MageMod = null;
				}

                		/*
				if ( Core.AOS )
					m_AosSkillBonuses.Remove();
                		*/

				m.CheckStatTimers();

				m.Delta( MobileDelta.WeaponDamage );

				#region Mondain's Legacy Sets
				if ( IsSetItem && m_SetEquipped )
					SetHelper.RemoveSetBonus( m, SetID, this );
				#endregion

				#region mod by Dies Irae
				if( StoneEnchantItemState != null )
					StoneEnchantItemState.Invalidate( m, false );
				#endregion
			}

			XmlAttach.CheckOnRemoved( this, parent ); // ARTEGORDONMOD XmlAttachment check for OnRemoved

			InvalidateProperties();
		}

		public virtual SkillName GetUsedSkill( Mobile m, bool checkSkillAttrs )
		{
			SkillName sk;

			if ( checkSkillAttrs && m_AosWeaponAttributes.UseBestSkill != 0 )
			{
				double swrd = m.Skills[SkillName.Swords].Value;
				double fenc = m.Skills[SkillName.Fencing].Value;
				double mcng = m.Skills[SkillName.Macing].Value;
				double val;

				sk = SkillName.Swords;
				val = swrd;

				if ( fenc > val ){ sk = SkillName.Fencing; val = fenc; }
				if ( mcng > val ){ sk = SkillName.Macing; val = mcng; }
			}
			else if ( m_AosWeaponAttributes.MageWeapon != 0 )
			{
				if ( m.Skills[SkillName.Magery].Value > m.Skills[Skill].Value )
					sk = SkillName.Magery;
				else
					sk = Skill;
			}
			else
			{
				sk = Skill;

				if ( sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman && m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value )
					sk = SkillName.Wrestling;
			}

			return sk;
		}

		public virtual double GetAttackSkillValue( Mobile attacker, Mobile defender )
		{
			return attacker.Skills[GetUsedSkill( attacker, true )].Value;
		}

		public virtual double GetDefendSkillValue( Mobile attacker, Mobile defender )
		{
			return defender.Skills[GetUsedSkill( defender, true )].Value;
		}

#if false
		private static bool CheckAnimal( Mobile m, Type type )
		{
			return AnimalForm.UnderTransformation( m, type );
		}
#endif

		public virtual bool CheckHit( Mobile attacker, Mobile defender )
		{
			double scalar = 1.0;

			#region mod by Dies Irae: Paladin modifiers
			BaseShield shield = defender.ShieldArmor as BaseShield;
			if( shield != null && shield.IsXmlHolyArmor )
				scalar = scalar - ( defender.Skills[ SkillName.Chivalry ].Base / 1000.0 );

			if( IsXmlHolyWeapon )
				scalar = scalar + ( attacker.Skills[ SkillName.Chivalry ].Base / 1000.0 );
			#endregion

			return CheckHit( attacker, defender, scalar );
		}

		public virtual bool CheckHit( Mobile attacker, Mobile defender, double scalarBonus )
		{
			BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
			BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

			Skill atkSkill = attacker.Skills[atkWeapon.Skill];
			Skill defSkill = defender.Skills[defWeapon.Skill];

			double atkValue = atkWeapon.GetAttackSkillValue( attacker, defender );
			double defValue = defWeapon.GetDefendSkillValue( attacker, defender );

			double ourValue, theirValue;

			int bonus = GetHitChanceBonus();

#if false 
            if ( Core.AOS )
			{
				if ( atkValue <= -20.0 )
					atkValue = -19.9;

				if ( defValue <= -20.0 )
					defValue = -19.9;

				bonus += AosAttributes.GetValue( attacker, AosAttribute.AttackChance );

				if ( Spells.Chivalry.DivineFurySpell.UnderEffect( attacker ) )
					bonus += 10; // attacker gets 10% bonus when they're under divine fury

				if ( CheckAnimal( attacker, typeof( GreyWolf ) ) || CheckAnimal( attacker, typeof( BakeKitsune ) ) )
					bonus += 20; // attacker gets 20% bonus when under Wolf or Bake Kitsune form

				if ( HitLower.IsUnderAttackEffect( attacker ) )
					bonus -= 25; // Under Hit Lower Attack effect -> 25% malus

				WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

				if ( ability != null )
					bonus += ability.AccuracyBonus;

				SpecialMove move = SpecialMove.GetCurrentMove( attacker );

				if ( move != null )
					bonus += move.GetAccuracyBonus( attacker );

				// Max Hit Chance Increase = 45%
				if ( bonus > 45 )
					bonus = 45;

				ourValue = (atkValue + 20.0) * (100 + bonus);

				bonus = AosAttributes.GetValue( defender, AosAttribute.DefendChance );

				if ( Spells.Chivalry.DivineFurySpell.UnderEffect( defender ) )
					bonus -= 20; // defender loses 20% bonus when they're under divine fury

				if ( HitLower.IsUnderDefenseEffect( defender ) )
					bonus -= 25; // Under Hit Lower Defense effect -> 25% malus
					
				int blockBonus = 0;

				if ( Block.GetBonus( defender, ref blockBonus ) )
					bonus += blockBonus;

				int surpriseMalus = 0;

				if ( SurpriseAttack.GetMalus( defender, ref surpriseMalus ) )
					bonus -= surpriseMalus;

				int discordanceEffect = 0;

				// Defender loses -0/-28% if under the effect of Discordance.
				if ( SkillHandlers.Discordance.GetEffect( attacker, ref discordanceEffect ) )
					bonus -= discordanceEffect;

				// Defense Chance Increase = 45%
				if ( bonus > 45 )
					bonus = 45;

				theirValue = (defValue + 20.0) * (100 + bonus);

				bonus = 0;
			}
			else
			{
#endif
			if ( atkValue <= -50.0 )
				atkValue = -49.9;

			if ( defValue <= -50.0 )
				defValue = -49.9;

			ourValue = (atkValue + 50.0);
			theirValue = (defValue + 50.0);
			//}

			double attackBonus = attacker.Body.IsHuman ? atkWeapon.HitRateBonus : 0;
			double evasionBonus = ( attacker.Body.IsHuman && defender.Body.IsHuman ) ? defWeapon.EvasionBonus : 0;

			#region mod by Dies Irae
			if( IsWrestlingWeapon && IsWoodenWeapon )
			{
				attackBonus += OldMaterialWrestHitBonus;
				evasionBonus += OldMaterialWrestEvaBonus;
			}

			if( defWeapon.IsWrestlingWeapon && defWeapon.IsWoodenWeapon )
			{
				attackBonus += defWeapon.OldMaterialWrestHitBonus;
				evasionBonus += defWeapon.OldMaterialWrestEvaBonus;
			}
			#endregion

			ourValue += attackBonus;
			theirValue += evasionBonus;

			// Check
			// bool isPvP = attacker.Body.IsHuman && defender.Body.IsHuman;
			// bool isRanged = atkWeapon.IsRanged;

			// Chance to Hit
			double chance = ourValue / ( theirValue * 2.0 );
			//attacker.SendMessage( "Our: {0};", ourValue);
			//attacker.SendMessage( "Their: {0};", theirValue);

			chance *= 1.0 + ( (double)bonus / 100 );

			// Ranged Weapons PvP Modifier (for T2A Only): no attack bonus
			//  if( isRanged && isPvP )
			//      chance = ( ourValue - 35 ) / ( ( theirValue + 10 + evasionBonus ) * 2.0 );

			chance *= scalarBonus;//edit by Arlas, was: 1.0 + ( scalarBonus / 100.0 ); // mod by Dies Irae

			if( attacker is BaseCreature )
				chance += EnemyMasteryDefinition.GetMasteryBonus( ( (BaseCreature)attacker ).Mastery );

			/*
			if( Core.AOS && chance < 0.02 )
				chance = 0.02;
			*/

			if( CheckForAttackSkillOnSwing )
				attacker.CheckSkill( atkSkill.SkillName, chance );

			if ( attacker.Player && !defender.Player && attacker.Skills[atkSkill.SkillName].Value >= 100 )
				chance += 0.2;
			//bool OldPolRules = attacker.PlayerDebug;

			if ( IsRanged && OldPolRules && attacker.Player && attacker.GetDistanceToSqrt( defender ) < 3 )
				chance -= 0.2;

			if ( attacker.Player && ((Midgard2PlayerMobile)attacker).Misses > 0 )
				chance += (((Midgard2PlayerMobile)attacker).Misses)*0.05;

			if ( !attacker.Player || !defender.Player )//edit by arlas [Modifica dovuta a dimensioni Body]
			{
				int mountvalue = attacker.Mounted ? -1 : 0;
				mountvalue += defender.Mounted ? 1 : 0;

				chance += ((mountvalue + defender.Body.Dimension - attacker.Body.Dimension )*0.1);
				//attacker.SendMessage( "Bonus Dimensione attacco: {0}0%;", (mountvalue + defender.Body.Dimension - attacker.Body.Dimension));
			}

			//attacker.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "Chance: {0}%", chance*100 ) );
			//attacker.SendMessage( "Skill: {0};", attacker.Skills[atkSkill.SkillName].Value);
			//attacker.SendMessage( "Chance: {0};", chance);

			bool success = ( chance * 1000 ) >= DiceRoll.OneDiceThousand.Roll();

			//attacker.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, success ? "*hit*" : "*miss*");
			if (defender.Player && defender.Warmode && defender.Combatant == null )
				defender.Combatant = attacker;

			SendDebugOverheadMessage( attacker, success ? "* hit *" : "* miss *" );

			if ( !success && attacker.Player )
				((Midgard2PlayerMobile)attacker).Misses++;
			else if ( success && attacker.Player )
				((Midgard2PlayerMobile)attacker).Misses = 0;

			if( attacker.PlayerDebug )
				attacker.Say("Hit %: {0:F2}",chance );

			//if( attacker.PlayerDebug )
			//{
			//	attacker.SendMessage( "Skill: {0} ({1:F2}) Hit chance: {2:F2}. Enemy -> Skill: {3} ({4:F2})", atkSkill, atkValue, chance, defSkill, defValue );
			//	if( scalarBonus > 0 )
			//		attacker.SendMessage( "Checkhit scalar: {0:F2}", scalarBonus );
			//	attacker.SendMessage( "Debug: att {0:F2} - def {1:F2} - chance {2:F2} - success {3}", ourValue, theirValue, chance, success );
			//}

			//if( defender.PlayerDebug )
			//{
			//	defender.SendMessage( "Skill: {0} ({1:F2}) Parry chance: {2:F2}. Enemy -> Skill: {3} ({4:F2})", defSkill, defValue, chance, atkSkill, atkValue );
			//	defender.SendMessage( "Debug: att {0:F2} - def {1:F2} - chance {2:F2} - success {3}", ourValue, theirValue, chance, success );
			//}

	        return success;
		}
 
		public virtual TimeSpan GetDelay( Mobile m )
		{
		    double speed = this.Speed;

		    if ( speed == 0 )
		        return TimeSpan.FromHours( 1.0 );

		    double delayInSeconds;

		    #region mod by Dies Irae : pre-aos stuff
		    if ( m is BaseCreature && ( (BaseCreature)m ).CustomWeaponSpeed != 0 )
		        speed = ( (BaseCreature)m ).CustomWeaponSpeed;

		    speed += OldMaterialSpeedBonus;

		    if ( m is BaseGuard || m is BaseTownGuard )
		        speed *= 2;
		    #endregion
            #region edit by Arlas [double weapon Mod]
            if( this is IDoubleWeapon && m is Midgard2PlayerMobile )
            {
                if( ( (Midgard2PlayerMobile)m ).UseDoubleWeaponsTogether )
                    speed *= 2 / 3; // 50% più lente se usate insieme
                else
                    speed *= 2; // 50% più veloci se usate insieme
            }
            #endregion
            #region edit by Arlas [slow poison]
            if( m.Slowed )
                speed *= 0.5; //due volte più lente
            #endregion

#if false
			if ( Core.SE )
			{
				/*
				 * This is likely true for Core.AOS as well... both guides report the same
				 * formula, and both are wrong.
				 * The old formula left in for AOS for legacy & because we aren't quite 100%
				 * Sure that AOS has THIS formula
				 */
				int bonus = AosAttributes.GetValue( m, AosAttribute.WeaponSpeed );

				if ( Spells.Chivalry.DivineFurySpell.UnderEffect( m ) )
					bonus += 10;

				// Bonus granted by successful use of Honorable Execution.
				bonus += HonorableExecution.GetSwingBonus( m );

				if( DualWield.Registry.Contains( m ) )
					bonus += ((DualWield.DualWieldTimer)DualWield.Registry[m]).BonusSwingSpeed;

				if( Feint.Registry.Contains( m ) )
					bonus -= ((Feint.FeintTimer)Feint.Registry[m]).SwingSpeedReduction;

				TransformContext context = TransformationSpellHelper.GetContext( m );

				if( context != null && context.Spell is ReaperFormSpell )
					bonus += ((ReaperFormSpell)context.Spell).SwingSpeedBonus;

				int discordanceEffect = 0;

				// Discordance gives a malus of -0/-28% to swing speed.
				if ( SkillHandlers.Discordance.GetEffect( m, ref discordanceEffect ) )
					bonus -= discordanceEffect;

				if( EssenceOfWindSpell.IsDebuffed( m ) )
					bonus -= EssenceOfWindSpell.GetSSIMalus( m );

				if ( bonus > 60 )
					bonus = 60;
				
				double ticks;

				if ( Core.ML )
				{
					int stamTicks = m.Stam / 30;

					ticks = speed * 4;
					ticks = Math.Floor( ( ticks - stamTicks ) * ( 100.0 / ( 100 + bonus ) ) );
				}
				else
				{
					speed = Math.Floor( speed * ( bonus + 100.0 ) / 100.0 );

					if ( speed <= 0 )
						speed = 1;

					ticks = Math.Floor( ( 80000.0 / ( ( m.Stam + 100 ) * speed ) ) - 2 );
				}
				
				// Swing speed currently capped at one swing every 1.25 seconds (5 ticks).
				if ( ticks < 5 )
					ticks = 5;

				delayInSeconds = ticks * 0.25;
			}
			else if ( Core.AOS )
			{
				int v = (m.Stam + 100) * (int) speed;

				int bonus = AosAttributes.GetValue( m, AosAttribute.WeaponSpeed );

				if ( Spells.Chivalry.DivineFurySpell.UnderEffect( m ) )
					bonus += 10;

				int discordanceEffect = 0;

				// Discordance gives a malus of -0/-28% to swing speed.
				if ( SkillHandlers.Discordance.GetEffect( m, ref discordanceEffect ) )
					bonus -= discordanceEffect;

				v += AOS.Scale( v, bonus );

				if ( v <= 0 )
					v = 1;

				delayInSeconds = Math.Floor( 40000.0 / v ) * 0.5;

				// Maximum swing rate capped at one swing per second 
				// OSI dev said that it has and is supposed to be 1.25
				if ( delayInSeconds < 1.25 )
					delayInSeconds = 1.25;
			}
			else
			{ 
#endif

			int v = (m.Stam + 100) * (int) speed;

			if ( v <= 0 )
				return TimeSpan.FromSeconds( 3.5 ); // v = 1;

			delayInSeconds = Math.Max( 15000.0 / v, 0.25 ); // mod by Dies Irae : cap 0.25s

			// }

			#region mod by Arlas:
			//era troncato al mezzo punto, ora lo tronco al millesimo
			delayInSeconds *= 100.0;
			delayInSeconds = (int)( delayInSeconds );
			delayInSeconds /= 100.0;
			#endregion

			//delayInSeconds *= 10.0;
            	//delayInSeconds /= 5.0;
			//delayInSeconds += 0.5;
			//delayInSeconds = (int)( delayInSeconds );
			//delayInSeconds *= 5.0;
			//delayInSeconds /= 10.0;

			if( delayInSeconds > CombatDelayMaxValue )
				delayInSeconds = CombatDelayMaxValue;

			#region mod by Dies Irae
			//if( m.PlayerDebug )
			//	m.SendMessage( "Debug WeaponDelay: mob: {3}, stamina {0}, speed {1}, delay {2:F3}", m.Stam, speed, delayInSeconds, m.Name ?? "" );
			#endregion

			return TimeSpan.FromSeconds( delayInSeconds );
		}

		public virtual void OnBeforeSwing( Mobile attacker, Mobile defender )
		{
#if false
			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

			if( a != null && !a.OnBeforeSwing( attacker, defender ) )
				WeaponAbility.ClearCurrentAbility( attacker );

			SpecialMove move = SpecialMove.GetCurrentMove( attacker );

			if( move != null && !move.OnBeforeSwing( attacker, defender ) )
				SpecialMove.ClearCurrentMove( attacker );
#endif
		}

		public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			return OnSwing( attacker, defender, 1.0 );
		}

		public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender, double damageBonus )
		{
			return OnSwing( attacker, defender, damageBonus, 1.0 );
		}

		public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender, double damageBonus, double chanceBonus )
		{
#if false
			bool canSwing = true;

			if ( Core.AOS )
			{
				canSwing = ( !attacker.Paralyzed && !attacker.Frozen );

				if ( canSwing )
				{
					Spell sp = attacker.Spell as Spell;

					canSwing = ( sp == null || !sp.IsCasting || !sp.BlocksMovement );
				}

				if( canSwing )
				{
					PlayerMobile p = attacker as PlayerMobile;

					canSwing = ( p == null || p.PeacedUntil <= DateTime.Now );
				}
			}
#endif

			if ( /* canSwing && */ attacker.HarmfulCheck( defender ) )
			{
				attacker.DisruptiveAction();

				if ( attacker.NetState != null )
					attacker.Send( new Swing( 0, attacker, defender ) );

#if false
				if ( attacker is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)attacker;
					WeaponAbility ab = bc.GetWeaponAbility();

					if ( ab != null )
					{
						if ( bc.WeaponAbilityChance > Utility.RandomDouble() )
							WeaponAbility.SetCurrentAbility( bc, ab );
						else
							WeaponAbility.ClearCurrentAbility( bc );
					}
				}
#endif

			#region edit by Arlas [double weapon Mod]
                	// usate insieme vale come prima arma
                	// prima arma chance da 80% a 100%
                	// seconda arma chance da 60% a 75%
                	if( this is IDoubleWeapon )
                	{
                    double atckValue = attacker.Skills[ Skill ].Value;
                    double tactValue = attacker.Skills[ SkillName.Tactics ].Value;
                    double anatValue = attacker.Skills[ SkillName.Anatomy ].Value;
                    double mainBonus = 0.80 + ( ( ( atckValue + tactValue + anatValue - 90 )/21.0 )*0.02 );
                    double secBonus = 0.60 + ( ( ( atckValue + tactValue + anatValue - 90 )/21.0 )*0.015 );

                    bool areTogether = false;
                    if( attacker is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)attacker ).UseDoubleWeaponsTogether )
                    {
                        areTogether = true;
                        InDoubleStrike = false;
                    }

                    chanceBonus *= InDoubleStrike ? secBonus : mainBonus;
                    damageBonus *= InDoubleStrike ? secBonus : ( areTogether? mainBonus + secBonus : mainBonus );
                    if( attacker.PlayerDebug )
                        attacker.SendMessage( "Debug DoubleWeapon: chance: {0}, damage: {1}, mainBonus: {2}, secBonus: {3}", chanceBonus, damageBonus, mainBonus, secBonus );
                	}
			#endregion

				if ( CheckHit( attacker, defender, chanceBonus ) )
					OnHit( attacker, defender, damageBonus );
				else
					OnMiss( attacker, defender );
			}

			return GetDelay( attacker );
		}

		#region Sounds
		public virtual int GetHitAttackSound( Mobile attacker, Mobile defender )
		{
			int sound = attacker.GetAttackSound();

			if ( sound == -1 )
				sound = HitSound;

			return sound;
		}

		public virtual int GetHitDefendSound( Mobile attacker, Mobile defender )
		{
			return defender.GetHurtSound();
		}

		public virtual int GetMissAttackSound( Mobile attacker, Mobile defender )
		{
			if ( attacker.GetAttackSound() == -1 )
				return MissSound;
			else
				return -1;
		}

		public virtual int GetMissDefendSound( Mobile attacker, Mobile defender )
		{
			return -1;
		}
		#endregion


		public static bool CheckParry( Mobile defender )
		{
			if ( defender == null )
				return false;

			BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

			double parry = defender.Skills[SkillName.Parry].Value;
			double bushidoNonRacial = defender.Skills[SkillName.Bushido].NonRacialValue;
			double bushido = defender.Skills[SkillName.Bushido].Value;

			if ( shield != null )
			{
				double chance = (parry - bushidoNonRacial) / 400.0;	// As per OSI, no negitive effect from the Racial stuffs, ie, 120 parry and '0' bushido with humans

				if ( chance < 0 ) // chance shouldn't go below 0
					chance = 0;				

				// Parry/Bushido over 100 grants a 5% bonus.
				if ( parry >= 100.0 || bushido >= 100.0)
					chance += 0.05;

				// Evasion grants a variable bonus post ML. 50% prior.
				if ( Evasion.IsEvading( defender ) )
					chance *= Evasion.GetParryScalar( defender );

				// Low dexterity lowers the chance.
				if ( defender.Dex < 80 )
					chance = chance * (20 + defender.Dex) / 100;

				return defender.CheckSkill( SkillName.Parry, chance );
			}
			else if ( !(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged) )
			{
				BaseWeapon weapon = defender.Weapon as BaseWeapon;

				double divisor = (weapon.Layer == Layer.OneHanded) ? 48000.0 : 41140.0;

				double chance = (parry * bushido) / divisor;

				double aosChance = parry / 800.0;

				// Parry or Bushido over 100 grant a 5% bonus.
				if( parry >= 100.0 )
				{
					chance += 0.05;
					aosChance += 0.05;
				}
				else if( bushido >= 100.0 )
				{
					chance += 0.05;
				}

                // Evasion grants a variable bonus post ML. 50% prior.
                //if( Evasion.IsEvading( defender ) )
                //    chance *= Evasion.GetParryScalar( defender );

				// Low dexterity lowers the chance.
				if( defender.Dex < 80 )
					chance = chance * (20 + defender.Dex) / 100;

				if ( chance > aosChance )
					return defender.CheckSkill( SkillName.Parry, chance );
				else
					return (aosChance > Utility.RandomDouble()); // Only skillcheck if wielding a shield & there's no effect from Bushido
			}

			return false;
		}

#if false
		public virtual int AbsorbDamageAOS( Mobile attacker, Mobile defender, int damage )
		{
			bool blocked = false;

			int originaldamage = damage; // ARTEGORDONMOD XmlSpawner

			if ( defender.Player || defender.Body.IsHuman )
			{
				blocked = CheckParry( defender );

				if ( blocked )
				{
					defender.FixedEffect( 0x37B9, 10, 16 );
					damage = 0;

					// Successful block removes the Honorable Execution penalty.
					HonorableExecution.RemovePenalty( defender );

					if ( CounterAttack.IsCountering( defender ) )
					{
						BaseWeapon weapon = defender.Weapon as BaseWeapon;

						if ( weapon != null )
						{
							defender.FixedParticles(0x3779, 1, 15, 0x158B, 0x0, 0x3, EffectLayer.Waist);
							weapon.OnSwing( defender, attacker );
						}

						CounterAttack.StopCountering( defender );
					}

					if ( Confidence.IsConfident( defender ) )
					{
						defender.SendLocalizedMessage( 1063117 ); // Your confidence reassures you as you successfully block your opponent's blow.

						double bushido = defender.Skills.Bushido.Value;

						defender.Hits += Utility.RandomMinMax( 1, (int)(bushido / 12) );
						defender.Stam += Utility.RandomMinMax( 1, (int)(bushido / 5) );
					}

					BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

					if ( shield != null )
					{
						shield.OnHit( this, damage );

						// ARTEGORDONMOD XmlAttachment check for OnArmorHit
                        XmlAttach.OnArmorHit( attacker, defender, shield, this, originaldamage );
					}
				}
			}

			if ( !blocked )
			{
				double positionChance = Utility.RandomDouble();

				Item armorItem;

				if( positionChance < 0.07 )
					armorItem = defender.NeckArmor;
				else if( positionChance < 0.14 )
					armorItem = defender.HandArmor;
				else if( positionChance < 0.28 )
					armorItem = defender.ArmsArmor;
				else if( positionChance < 0.43 )
					armorItem = defender.HeadArmor;
				else if( positionChance < 0.65 )
					armorItem = defender.LegsArmor;
				else
					armorItem = defender.ChestArmor;

				IWearableDurability armor = armorItem as IWearableDurability;

				if ( armor != null )
					armor.OnHit( this, damage ); // call OnHit to lose durability

                damage -= XmlAttach.OnArmorHit( attacker, defender, armorItem, this, originaldamage ); // ARTEGORDONMOD check for OnArmorHit
			}

			return damage;
		}
#endif

		public virtual int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
		{
			/*
			if ( Core.AOS )
				return AbsorbDamageAOS( attacker, defender, damage );
			*/

			// commento di Dies:
			// attacker è il mobile che tiene l'arma in mano
			// defender è colui che viene colpito (e che quindi ha l'armor)

			double chance = Utility.RandomDouble();

			Item armorItem;

			if( chance < 0.07 )
				armorItem = defender.NeckArmor;
			else if( chance < 0.14 )
				armorItem = defender.HandArmor;
			else if( chance < 0.28 )
				armorItem = defender.ArmsArmor;
			else if( chance < 0.42 )
				armorItem = defender.HeadArmor;
			else if( chance < 0.66 )
				armorItem = defender.LegsArmor;
			else
				armorItem = defender.ChestArmor;

            	#region special cases: mod by Magiusche
            	if( defender is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)defender ).AbsorbDamage( attacker, ref damage ) )
                		return Midgard.Engines.PVMAbsorbtions.Core.OnHitArmour( armorItem as BaseArmor, this, damage );
            	#endregion

			int initialDamage = damage;

			IWearableDurability armor = armorItem as IWearableDurability;

			if ( armor != null )
				damage = armor.OnHit( this, damage );
			else// if (defender.Player)
			{
				if( damage < (int)((double)MaxDiceDamage*(1-chance)) )
					damage = (int)((double)MaxDiceDamage*(1-chance));//int rolled = Utility.Dice( NumDice, NumSides, DiceBonus );//MaxDiceDamage
				//attacker.Say("Danno = {0}->{1}, chance {2}, MAXD {3}", initialDamage, damage, (int)(chance*100),(int)((double)MaxDiceDamage*(1-chance)) );
				//	armorItem = defender.NeckArmor;
				//else if( chance < 0.14 )
				//	armorItem = defender.HandArmor;
				//else if( chance < 0.28 )
				//	armorItem = defender.ArmsArmor;
				//else if( chance < 0.42 )
				//	armorItem = defender.HeadArmor;
				//else if( chance < 0.66 )
				//	armorItem = defender.LegsArmor;
				//else
			}

			BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
			if ( shield != null )
				damage = shield.OnHit( this, damage );

			#region ARTEGORDONMOD XmlAttachment check for OnArmorHit
			damage -= XmlAttach.OnArmorHit( attacker, defender, armorItem, this, damage );
			damage -= XmlAttach.OnArmorHit( attacker, defender, shield, this, damage );
			#endregion

			#region mod by Dies Irae : stone enchant system
			StoneEnchantItem stateWeapon = StoneEnchantItemState;
			if( stateWeapon != null )
				stateWeapon.Definition.OnHit( attacker, defender, this, armorItem, damage );

			StoneEnchantItem stateArmor = StoneEnchantItem.Find( armorItem );
			StoneEnchantItem stateShield = StoneEnchantItem.Find( shield );

			if( stateArmor != null && stateShield != null )
			{
				if( Utility.RandomBool() )
					stateArmor.Definition.OnHit( attacker, defender, armorItem, this, damage );
				else
					stateShield.Definition.OnHit( attacker, defender, shield, this, damage );
			}
			else if( stateArmor != null )
				stateArmor.Definition.OnHit( attacker, defender, armorItem, this, damage );
			else if( stateShield != null )
				stateShield.Definition.OnHit( attacker, defender, shield, this, damage );
			#endregion

			#region mod by Dies Irae - Pvm Only: Virtual Armor
			if( !defender.Player )
			{
				int virtualArmor = defender.VirtualArmor + defender.VirtualArmorMod;

				/*
				if( virtualArmor > 0 )
				{
					virtualArmor = (int)( virtualArmor / GetVirtualArmorScalar( defender ) );
					damage -= Utility.RandomMinMax( virtualArmor / 2, virtualArmor );
				}
				*/

				//bool OldPolRules = attacker.PlayerDebug;
				double halfAr = OldPolRules ? (virtualArmor / 2.0) : virtualArmor / 200.0;
				if ( OldPolRules && this.Layer == Layer.OneHanded )
					halfAr = (virtualArmor / 5.0);

				//bool OldPolRules = attacker.PlayerDebug;

				int absorbed = OldPolRules ? (int)((double)(halfAr + (halfAr * Utility.RandomDouble())) ) : (int)( damage * ( halfAr + ( halfAr * Utility.RandomDouble() ) ) );

				damage -= absorbed;

				if( attacker.PlayerDebug )
					attacker.SendMessage( "Your enemy absorbed {0} damage (was {1}) with its {2} armor. Final damage is {3}", absorbed, initialDamage, virtualArmor, damage );
			}
			else if ( defender.Player && armor == null )
			{
				string name = String.Format( "[Magic] {0} Offset", StatType.AR );
				StatMod mod = defender.GetStatMod( name );
				if( mod != null )
				{//protection senza armor circa 21 AR
					if (damage > 70 )//cap danno massimo
						damage = 70;

					int absorbed = (int)((double)(defender.ArmorRating/2));

					if ( this.Layer == Layer.OneHanded || (defender.Spell != null && (defender.Spell).IsCasting) )
						absorbed = (int)((double)(defender.ArmorRating/4));

					damage -= absorbed;
					
					//defender.Say( "PROTECTION absorbed {0} of {1} points of damage.", absorbed, damage + absorbed );
				}
			}

			//Was seeing negative damages with virtual armors
			if( damage < 1 )
				damage = 1;

			if( defender.Hits < 200 && damage > 45 && !attacker.Body.IsHuman )
				damage = 45;
			#endregion

			/* runuo Arranged formulas
			if ( virtualArmor > 0 )
			{
				double scalar;
             
				if ( chance < 0.44 ) { scalar = 0.44; }
				else if ( chance < 0.58 ) { scalar = 0.14; }
				else if ( chance < 0.72 ) { scalar = 0.14; }
				else if ( chance < 0.86 ) { scalar = 0.14; }
				else if ( chance < 0.93 ) { scalar = 0.07; }
				else { scalar = 0.07; }
				int from = (int)(virtualArmor * scalar) / 2;
				int to = (int)(virtualArmor * scalar);
				damage -= Utility.Random( from, (to - from) + 1 );
			}
			*/

			/*
			if ( virtualArmor > 0 )
			{
				double scalar;

				if ( chance < 0.14 )
					scalar = 0.07;
				else if ( chance < 0.28 )
					scalar = 0.14;
				else if ( chance < 0.43 )
					scalar = 0.15;
				else if ( chance < 0.65 )
					scalar = 0.22;
				else
					scalar = 0.35;

				int from = (int)(virtualArmor * scalar) / 2;
				int to = (int)(virtualArmor * scalar);

				damage -= Utility.Random( from, (to - from) + 1 );
			}
			*/

			//return damage;
			return Midgard.Engines.PVMAbsorbtions.Core.OnHitSkin ( defender, this, damage, Midgard.Engines.PVMAbsorbtions.DamageTypes.Physical ); //mod by magius(che)
		}

		public virtual int GetPackInstinctBonus( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player || defender.Player )
				return 0;

			BaseCreature bc = attacker as BaseCreature;

			if ( bc == null || bc.PackInstinct == PackInstinct.None || (!bc.Controlled && !bc.Summoned) )
				return 0;

			Mobile master = bc.ControlMaster;

			if ( master == null )
				master = bc.SummonMaster;

			if ( master == null )
				return 0;

			int inPack = 1;

			foreach ( Mobile m in defender.GetMobilesInRange( 1 ) )
			{
				if ( m != attacker && m is BaseCreature )
				{
					BaseCreature tc = (BaseCreature)m;

					if ( (tc.PackInstinct & bc.PackInstinct) == 0 || (!tc.Controlled && !tc.Summoned) )
						continue;

					Mobile theirMaster = tc.ControlMaster;

					if ( theirMaster == null )
						theirMaster = tc.SummonMaster;

					if ( master == theirMaster && tc.Combatant == defender )
						++inPack;
				}
			}

			if ( inPack >= 5 )
				return 100;
			else if ( inPack >= 4 )
				return 75;
			else if ( inPack >= 3 )
				return 50;
			else if ( inPack >= 2 )
				return 25;

			return 0;
		}

		private static bool m_InDoubleStrike;

		public static bool InDoubleStrike
		{
			get{ return m_InDoubleStrike; }
			set{ m_InDoubleStrike = value; }
		}

		public void OnHit( Mobile attacker, Mobile defender )
		{
			OnHit( attacker, defender, 1.0 );
		}

		public virtual void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			#region mod by Dies Irae
			OnHitOld( attacker, defender, damageBonus );
			#endregion

#if false
			if ( MirrorImage.HasClone( defender ) && (defender.Skills.Ninjitsu.Value / 150.0) > Utility.RandomDouble() )
			{
				Clone bc;

				foreach ( Mobile m in defender.GetMobilesInRange( 4 ) )
				{
					bc = m as Clone;

					if ( bc != null && bc.Summoned && bc.SummonMaster == defender )
					{
						attacker.SendLocalizedMessage( 1063141 ); // Your attack has been diverted to a nearby mirror image of your target!
						defender.SendLocalizedMessage( 1063140 ); // You manage to divert the attack onto one of your nearby mirror images.

						/*
						 * TODO: What happens if the Clone parries a blow?
						 * And what about if the attacker is using Honorable Execution
						 * and kills it?
						 */

						defender = m;
						break;
					}
				}
			}

			PlaySwingAnimation( attacker );
			PlayHurtAnimation( defender );

			attacker.PlaySound( GetHitAttackSound( attacker, defender ) );
			defender.PlaySound( GetHitDefendSound( attacker, defender ) );

			int damage = ComputeDamage( attacker, defender );

			#region Damage Multipliers
			/*
			 * The following damage bonuses multiply damage by a factor.
			 * Capped at x3 (300%).
			 */
			int percentageBonus = 0;

			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );
			SpecialMove move = SpecialMove.GetCurrentMove( attacker );

			if( a != null )
			{
				percentageBonus += (int)(a.DamageScalar * 100) - 100;
			}

			if( move != null )
			{
				percentageBonus += (int)(move.GetDamageScalar( attacker, defender ) * 100) - 100;
			}

			percentageBonus += (int)(damageBonus * 100) - 100;

			CheckSlayerResult cs = CheckSlayers( attacker, defender );

			if ( cs != CheckSlayerResult.None )
			{
				if ( cs == CheckSlayerResult.Slayer )
					defender.FixedEffect( 0x37B9, 10, 5 );

				percentageBonus += 100;
			}

			if ( !attacker.Player )
			{
				if ( defender is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)defender;

					if( pm.EnemyOfOneType != null && pm.EnemyOfOneType != attacker.GetType() )
					{
						percentageBonus += 100;
					}
				}
			}
			else if ( !defender.Player )
			{
				if ( attacker is PlayerMobile )
				{
					PlayerMobile pm = (PlayerMobile)attacker;

					if ( pm.WaitingForEnemy )
					{
						pm.EnemyOfOneType = defender.GetType();
						pm.WaitingForEnemy = false;
					}

					if ( pm.EnemyOfOneType == defender.GetType() )
					{
						defender.FixedEffect( 0x37B9, 10, 5, 1160, 0 );

						percentageBonus += 50;
					}
				}
			}

			int packInstinctBonus = GetPackInstinctBonus( attacker, defender );

			if( packInstinctBonus != 0 )
			{
				percentageBonus += packInstinctBonus;
			}

			if( m_InDoubleStrike )
			{
				percentageBonus -= 10;
			}

			TransformContext context = TransformationSpellHelper.GetContext( defender );

			if( (m_Slayer == SlayerName.Silver || m_Slayer2 == SlayerName.Silver) && context != null && context.Spell is NecromancerSpell && context.Type != typeof( HorrificBeastSpell ) )
			{
				// Every necromancer transformation other than horrific beast takes an additional 25% damage
				percentageBonus += 25;
			}

			if ( attacker is PlayerMobile && !(Core.ML && defender is PlayerMobile ))
			{
				PlayerMobile pmAttacker = (PlayerMobile) attacker;

				if( pmAttacker.HonorActive && pmAttacker.InRange( defender, 1 ) )
				{
					percentageBonus += 25;
				}

				if( pmAttacker.SentHonorContext != null && pmAttacker.SentHonorContext.Target == defender )
				{
					percentageBonus += pmAttacker.SentHonorContext.PerfectionDamageBonus;
				}
			}

			BaseTalisman talisman = attacker.Talisman as BaseTalisman;

			if ( talisman != null && talisman.Killer != null )
				percentageBonus += talisman.Killer.DamageBonus( defender );

			percentageBonus = Math.Min( percentageBonus, 300 );

			damage = AOS.Scale( damage, 100 + percentageBonus );
			#endregion

			if ( attacker is BaseCreature )
				((BaseCreature)attacker).AlterMeleeDamageTo( defender, ref damage );

			if ( defender is BaseCreature )
				((BaseCreature)defender).AlterMeleeDamageFrom( attacker, ref damage );

			damage = AbsorbDamage( attacker, defender, damage );

			if ( !Core.AOS && damage < 1 )
				damage = 1;
			else if ( Core.AOS && damage == 0 ) // parried
			{
				if ( a != null && a.Validate( attacker ) /*&& a.CheckMana( attacker, true )*/ ) // Parried special moves have no mana cost 
				{
					a = null;
					WeaponAbility.ClearCurrentAbility( attacker );

					attacker.SendLocalizedMessage( 1061140 ); // Your attack was parried!
				}
			}
			
			#region Mondain's Legacy
			if ( m_Immolating )
			{
				int d = ImmolatingWeaponSpell.GetDamage( this );
				d = AOS.Damage( defender, attacker, d, 0, 100, 0, 0, 0 );

                AttuneWeaponSpell.TryAbsorb( defender, ref d );
				
				if ( d > 0 )
					defender.Damage( d );				
			}			
			#endregion

			AddBlood( attacker, defender, damage );

			int phys, fire, cold, pois, nrgy, chaos, direct;

			GetDamageTypes( attacker, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );

			if ( Core.ML && this is BaseRanged )
			{
				BaseQuiver quiver = attacker.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;

				if ( quiver != null )
					quiver.AlterBowDamage( ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct );
			}

			if ( m_Consecrated )
			{
				phys = defender.PhysicalResistance;
				fire = defender.FireResistance;
				cold = defender.ColdResistance;
				pois = defender.PoisonResistance;
				nrgy = defender.EnergyResistance;

				int low = phys, type = 0;

				if ( fire < low ){ low = fire; type = 1; }
				if ( cold < low ){ low = cold; type = 2; }
				if ( pois < low ){ low = pois; type = 3; }
				if ( nrgy < low ){ low = nrgy; type = 4; }

				phys = fire = cold = pois = nrgy = chaos = direct = 0;

				if ( type == 0 ) phys = 100;
				else if ( type == 1 ) fire = 100;
				else if ( type == 2 ) cold = 100;
				else if ( type == 3 ) pois = 100;
				else if ( type == 4 ) nrgy = 100;
			}

			// TODO: Scale damage, alongside the leech effects below, to weapon speed.
            //if ( ImmolatingWeaponSpell.IsImmolating( this ) && damage > 0 )
            //    ImmolatingWeaponSpell.DoEffect( this, defender );

			int damageGiven = damage;

			if ( a != null && !a.OnBeforeDamage( attacker, defender ) )
			{
				WeaponAbility.ClearCurrentAbility( attacker );
				a = null;
			}

			if ( move != null && !move.OnBeforeDamage( attacker, defender ) )
			{
				SpecialMove.ClearCurrentMove( attacker );
				move = null;
			}

			#region mod by Dies Irae
			WeaponAbility weaponA;
			bool bladeWeaving = Bladeweave.BladeWeaving( attacker, out weaponA );

			// bool ignoreArmor = ( a is ArmorIgnore || (move != null && move.IgnoreArmor( attacker )) );
			bool ignoreArmor = ( a is ArmorIgnore || (move != null && move.IgnoreArmor( attacker )) || (bladeWeaving && weaponA is ArmorIgnore ) );

            	bool isOneEnemyActive = HolySmiteSpell.IsWaitingPaladine( attacker ) && RPGPaladinSpell.IsEnemy( attacker, defender );
            	if( isOneEnemyActive )
            	{
                		damage += HolySmiteSpell.GetBonusDamage( attacker, defender );
                		ignoreArmor = true;
            	}
			#endregion

			// damage increase after resists applied
			int damageIncrease = 0;
			
			BaseQuiver quiverForDamage = attacker.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;

			if( quiverForDamage != null )
				damageIncrease = quiverForDamage.DamageIncrease;

			damageGiven = AOS.Damage( defender, attacker, damage, ignoreArmor, phys, fire, cold, pois, nrgy, chaos, direct, false, this is BaseRanged );

			double propertyBonus = ( move == null ) ? 1.0 : move.GetPropertyBonus( attacker );

			if ( Core.AOS )
			{
				int lifeLeech = 0;
				int stamLeech = 0;
				int manaLeech = 0;
				int wraithLeech = 0;

				if ( (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitLeechHits ) * propertyBonus) > Utility.Random( 100 ) )
					lifeLeech += 30; // HitLeechHits% chance to leech 30% of damage as hit points

				if ( (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitLeechStam ) * propertyBonus) > Utility.Random( 100 ) )
					stamLeech += 100; // HitLeechStam% chance to leech 100% of damage as stamina

				if ( (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitLeechMana ) * propertyBonus) > Utility.Random( 100 ) )
					manaLeech += 40; // HitLeechMana% chance to leech 40% of damage as mana

				if ( m_Cursed )
					lifeLeech += 50; // Additional 50% life leech for cursed weapons (necro spell)

				context = TransformationSpellHelper.GetContext( attacker );

				if ( context != null && context.Type == typeof( VampiricEmbraceSpell ) )
					lifeLeech += 20; // Vampiric embrace gives an additional 20% life leech

				if ( context != null && context.Type == typeof( WraithFormSpell ) )
				{
					wraithLeech = (5 + (int)((15 * attacker.Skills.SpiritSpeak.Value) / 100)); // Wraith form gives an additional 5-20% mana leech

					// Mana leeched by the Wraith Form spell is actually stolen, not just leeched.
					defender.Mana -= AOS.Scale( damageGiven, wraithLeech );

					manaLeech += wraithLeech;
				}

				if ( lifeLeech != 0 )
					attacker.Hits += AOS.Scale( damageGiven, lifeLeech );

				if ( stamLeech != 0 )
					attacker.Stam += AOS.Scale( damageGiven, stamLeech );

				if ( manaLeech != 0 )
					attacker.Mana += AOS.Scale( damageGiven, manaLeech );

				if ( lifeLeech != 0 || stamLeech != 0 || manaLeech != 0 )
					attacker.PlaySound( 0x44D );
			}

			if ( m_MaxHits > 0 && ((MaxRange <= 1 && (defender is Slime || defender is ToxicElemental)) || Utility.Random( 25 ) == 0) ) // Stratics says 50% chance, seems more like 4%..
			{
				if ( MaxRange <= 1 && (defender is Slime || defender is ToxicElemental) )
					attacker.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500263 ); // *Acid blood scars your weapon!*
				
				// Mondain's Legacy Sets
				if ( Core.AOS && m_AosWeaponAttributes.SelfRepair + ( IsSetItem && m_SetEquipped ? m_SetSelfRepair : 0 )> Utility.Random( 10 ) )
				{
					HitPoints += 2;
				}
				else
				{
					if ( m_Hits > 0 )
					{
						--HitPoints;
					}
					else if ( m_MaxHits > 1 )
					{
						--MaxHitPoints;

						if ( Parent is Mobile )
							((Mobile)Parent).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
					}
					else
					{
						Delete();
					}
				}
			}

			if ( attacker is VampireBatFamiliar )
			{
				BaseCreature bc = (BaseCreature)attacker;
				Mobile caster = bc.ControlMaster;

				if ( caster == null )
					caster = bc.SummonMaster;

				if ( caster != null && caster.Map == bc.Map && caster.InRange( bc, 2 ) )
					caster.Hits += damage;
				else
					bc.Hits += damage;
			}

			if ( Core.AOS )
			{
				// Mondain's Legacy Mod
				int physChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitPhysicalArea ) * propertyBonus);
				int fireChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitFireArea ) * propertyBonus);
				int coldChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitColdArea ) * propertyBonus);
				int poisChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitPoisonArea ) * propertyBonus);
				int nrgyChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitEnergyArea ) * propertyBonus);

				if ( physChance != 0 && physChance > Utility.Random( 100 ) )
					DoAreaAttack( attacker, defender, 0x10E,   50, 100, 0, 0, 0, 0 );

				if ( fireChance != 0 && fireChance > Utility.Random( 100 ) )
					DoAreaAttack( attacker, defender, 0x11D, 1160, 0, 100, 0, 0, 0 );

				if ( coldChance != 0 && coldChance > Utility.Random( 100 ) )
					DoAreaAttack( attacker, defender, 0x0FC, 2100, 0, 0, 100, 0, 0 );

				if ( poisChance != 0 && poisChance > Utility.Random( 100 ) )
					DoAreaAttack( attacker, defender, 0x205, 1166, 0, 0, 0, 100, 0 );

				if ( nrgyChance != 0 && nrgyChance > Utility.Random( 100 ) )
					DoAreaAttack( attacker, defender, 0x1F1,  120, 0, 0, 0, 0, 100 );

				// Mondain's Legacy Mod
				int maChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitMagicArrow ) * propertyBonus);
				int harmChance = (int)(AosWeaponAttributes.GetValue(attacker,  AosWeaponAttribute.HitHarm ) * propertyBonus);
				int fireballChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitFireball ) * propertyBonus);
				int lightningChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitLightning ) * propertyBonus);
				int dispelChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitDispel ) * propertyBonus);

				if ( maChance != 0 && maChance > Utility.Random( 100 ) )
					DoMagicArrow( attacker, defender );

				if ( harmChance != 0 && harmChance > Utility.Random( 100 ) )
					DoHarm( attacker, defender );

				if ( fireballChance != 0 && fireballChance > Utility.Random( 100 ) )
					DoFireball( attacker, defender );

				if ( lightningChance != 0 && lightningChance > Utility.Random( 100 ) )
					DoLightning( attacker, defender );

				if ( dispelChance != 0 && dispelChance > Utility.Random( 100 ) )
					DoDispel( attacker, defender );

				// Mondain's Legacy Mod
				int laChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitLowerAttack ) * propertyBonus);
				int ldChance = (int)(AosWeaponAttributes.GetValue( attacker, AosWeaponAttribute.HitLowerDefend ) * propertyBonus);

				if ( laChance != 0 && laChance > Utility.Random( 100 ) )
					DoLowerAttack( attacker, defender );

				if ( ldChance != 0 && ldChance > Utility.Random( 100 ) )
					DoLowerDefense( attacker, defender );
			}

			if ( attacker is BaseCreature )
				((BaseCreature)attacker).OnGaveMeleeAttack( defender );

			if ( defender is BaseCreature )
				((BaseCreature)defender).OnGotMeleeAttack( attacker );

			if ( a != null )
				a.OnHit( attacker, defender, damage );

			if ( move != null )
				move.OnHit( attacker, defender, damage );

			if ( defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null )
				((IHonorTarget)defender).ReceivedHonorContext.OnTargetHit( attacker );

			if ( !(this is BaseRanged) )
			{
				if ( AnimalForm.UnderTransformation( attacker, typeof( GiantSerpent ) ) )
					defender.ApplyPoison( attacker, Poison.Lesser );

				if ( AnimalForm.UnderTransformation( defender, typeof( BullFrog ) ) )
					attacker.ApplyPoison( defender, Poison.Regular );
			}
#endif
		}

		public virtual double GetAosDamage( Mobile attacker, int bonus, int dice, int sides )
		{
			int damage = Utility.Dice( dice, sides, bonus ) * 100;
			int damageBonus = 0;

			// Inscription bonus
			int inscribeSkill = attacker.Skills[SkillName.Inscribe].Fixed;

			damageBonus += inscribeSkill / 200;

			if ( inscribeSkill >= 1000 )
				damageBonus += 5;

			if ( attacker.Player )
			{
				// Int bonus
				damageBonus += (attacker.Int / 10);

				// SDI bonus
				damageBonus += AosAttributes.GetValue( attacker, AosAttribute.SpellDamage );

				TransformContext context = TransformationSpellHelper.GetContext( attacker );

				if( context != null && context.Spell is ReaperFormSpell )
					damageBonus += ((ReaperFormSpell)context.Spell).SpellDamageBonus;
			}

			damage = AOS.Scale( damage, 100 + damageBonus );

			return damage / 100;
		}

		#region Do<AoSEffect>
		public virtual void DoMagicArrow( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = Core.AOS ? GetAosDamage( attacker, 10, 1, 4 ) : Utility.Random( 4, 4 ); // mod by Dies Irae

			attacker.MovingParticles( defender, 0x36E4, 5, 0, false, true, 3006, 4006, 0 );
			attacker.PlaySound( 0x1E5 );

			SpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, 0, 100, 0, 0, 0 );
		}

		public virtual void DoHarm( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = Core.AOS ? GetAosDamage( attacker, 17, 1, 5 ) : Utility.Random( 1, 15 ); // mod by Dies Irae

			if ( !defender.InRange( attacker, 2 ) )
				damage *= 0.25; // 1/4 damage at > 2 tile range
			else if ( !defender.InRange( attacker, 1 ) )
				damage *= 0.50; // 1/2 damage at 2 tile range

			#region mod by Dies Irae
			// defender.FixedParticles( 0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist );
			// defender.PlaySound( 0x0FC );

			if ( Core.AOS )
			{
				defender.FixedParticles( 0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist );
				defender.PlaySound( 0x0FC );
			}
			else
			{
				defender.FixedParticles( 0x374A, 10, 15, 5013, EffectLayer.Waist );
				defender.PlaySound( 0x1F1 );
			}
			#endregion

			SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 0, 100, 0, 0 );
		}

		public virtual void DoFireball( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = Core.AOS ? GetAosDamage( attacker, 19, 1, 5 ) : Utility.Random( 10, 7 ); // mod by Dies Irae

			attacker.MovingParticles( defender, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
			attacker.PlaySound( 0x15E );

			SpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, 0, 100, 0, 0, 0 );
		}

		public virtual void DoLightning( Mobile attacker, Mobile defender )
		{
			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			double damage = Core.AOS ? GetAosDamage( attacker, 23, 1, 4 ) : Utility.Random( 12, 9 ); // mod by Dies Irae

			defender.BoltEffect( 0 );

			SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 0, 0, 0, 100 );
		}

		public virtual void DoDispel( Mobile attacker, Mobile defender )
		{
			bool dispellable = false;

			if ( defender is BaseCreature )
				dispellable = ((BaseCreature)defender).Summoned && !((BaseCreature)defender).IsAnimatedDead;

			if ( !dispellable )
				return;

			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );
			
			#region Mod by Magius(CHE): all dispel effect must work on the same way.
			/*
			Spells.MagerySpell sp = new Spells.Sixth.DispelSpell( attacker, null );

			if ( sp.CheckResisted( defender ) )
			{
				defender.FixedEffect( 0x3779, 10, 20 );
			}
			else
			{
				Effects.SendLocationParticles( EffectItem.Create( defender.Location, defender.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( defender, defender.Map, 0x201 );

				defender.Delete();
			}*/

			BaseCreature bc = defender as BaseCreature;
			if( bc == null )
				return;

			int mageryvalue = 90;

			double dispelChance = ( 50.0 + ( ( 100 * ( mageryvalue - bc.DispelDifficulty ) ) / ( bc.DispelFocus * 2 ) ) ) / 100;

			if( dispelChance > Utility.RandomDouble() )
			{
				Effects.SendLocationParticles( EffectItem.Create( defender.Location, defender.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
				Effects.PlaySound( defender, defender.Map, 0x201 );

				defender.Delete();
			}
			else
			{
				defender.FixedEffect( 0x3779, 10, 20 );
			}
		    #endregion
		}

		public virtual void DoLowerAttack( Mobile from, Mobile defender )
		{
			if ( HitLower.ApplyAttack( defender ) )
			{
				defender.PlaySound( 0x28E );
				Effects.SendTargetEffect( defender, 0x37BE, 1, 4, 0xA, 3 );
			}
		}

		public virtual void DoLowerDefense( Mobile from, Mobile defender )
		{
			if ( HitLower.ApplyDefense( defender ) )
			{
				defender.PlaySound( 0x28E );
				Effects.SendTargetEffect( defender, 0x37BE, 1, 4, 0x23, 3 );
			}
		}

		public virtual void DoAreaAttack( Mobile from, Mobile defender, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy )
		{
			Map map = from.Map;

			if ( map == null )
				return;

			List<Mobile> list = new List<Mobile>();

			foreach ( Mobile m in from.GetMobilesInRange( 10 ) )
			{
				if ( from != m && defender != m && SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false ) && ( !Core.ML || from.InLOS( m ) ) )
					list.Add( m );
			}

			if ( list.Count == 0 )
				return;

			Effects.PlaySound( from.Location, map, sound );

			// TODO: What is the damage calculation?

			for ( int i = 0; i < list.Count; ++i )
			{
				Mobile m = list[i];

				double scalar = (11 - from.GetDistanceToSqrt( m )) / 10;

				if ( scalar > 1.0 )
					scalar = 1.0;
				else if ( scalar < 0.0 )
					continue;

				from.DoHarmful( m, true );
				m.FixedEffect( 0x3779, 1, 15, hue, 0 );
				AOS.Damage( m, from, (int)(GetBaseDamage( from ) * scalar), phys, fire, cold, pois, nrgy );
			}
		}
		#endregion

		public virtual CheckSlayerResult CheckSlayers( Mobile attacker, Mobile defender )
		{
			BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
			SlayerEntry atkSlayer = SlayerGroup.GetEntryByName( atkWeapon.Slayer );
			SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName( atkWeapon.Slayer2 );

			#region mod by Dies Irae per i vampirli
			if( defender.Race == Midgard.Engines.Races.Core.Vampire )
			{
				if( ( atkSlayer != null && atkSlayer.Name == SlayerName.Silver ) || 
				( atkSlayer2 != null && atkSlayer2.Name == SlayerName.Silver ) )
					return CheckSlayerResult.Slayer;
			}
			#endregion

			if ( atkSlayer != null && atkSlayer.Slays( defender )  || atkSlayer2 != null && atkSlayer2.Slays( defender ) )
				return CheckSlayerResult.Slayer;

			if ( !Core.SE )
			{
				ISlayer defISlayer = Spellbook.FindEquippedSpellbook( defender );

			    if ( atkSlayer != null && atkSlayer.Slays( defender )  || atkSlayer2 != null && atkSlayer2.Slays( defender ) )
				    return CheckSlayerResult.Slayer;

			    BaseTalisman talisman = attacker.Talisman as BaseTalisman;

			    if ( talisman != null && TalismanSlayer.Slays( talisman.Slayer, defender ) )
				    return CheckSlayerResult.Slayer;

				if( defISlayer != null )
				{
					SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defISlayer.Slayer );
					SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName( defISlayer.Slayer2 );

					if( defSlayer != null && defSlayer.Group.OppositionSuperSlays( attacker ) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays( attacker ) )
						return CheckSlayerResult.Opposition;
				}
			}

			return CheckSlayerResult.None;
		}

		public virtual void AddBlood( Mobile attacker, Mobile defender, int damage )
		{
			if ( damage > 0 )
			{
				new Blood().MoveToWorld( defender.Location, defender.Map );

				int extraBlood = (Core.SE ? Utility.RandomMinMax( 3, 4 ) : Utility.RandomMinMax( 0, 1 ) );

				for( int i = 0; i < extraBlood; i++ )
				{
					new Blood().MoveToWorld( new Point3D(
						defender.X + Utility.RandomMinMax( -1, 1 ),
						defender.Y + Utility.RandomMinMax( -1, 1 ),
						defender.Z ), defender.Map );
				}
			}
		}

		public virtual void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			if( wielder is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)wielder;

				phys = bc.PhysicalDamage;
				fire = bc.FireDamage;
				cold = bc.ColdDamage;
				pois = bc.PoisonDamage;
				nrgy = bc.EnergyDamage;
				chaos = bc.ChaosDamage;
				direct = bc.DirectDamage;
			}
			else
			{
				#region mod by Dies Irae
				phys = 100;
				fire = cold = pois = nrgy = chaos = direct = 0;
				#endregion

                /*
				fire = m_AosElementDamages.Fire;
				cold = m_AosElementDamages.Cold;
				pois = m_AosElementDamages.Poison;
				nrgy = m_AosElementDamages.Energy;
				chaos = m_AosElementDamages.Chaos;
				direct = m_AosElementDamages.Direct;

				phys = 100 - fire - cold - pois - nrgy - chaos - direct;

				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

				if( resInfo != null )
				{
					CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

					if( attrInfo != null )
					{
						int left = phys;

						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponColdDamage,		ref cold, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponEnergyDamage,	ref nrgy, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponFireDamage,		ref fire, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponPoisonDamage,	ref pois, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponChaosDamage,	ref chaos, left );
						left = ApplyCraftAttributeElementDamage( attrInfo.WeaponDirectDamage,	ref direct, left );

						phys = left;
					}
				}
                */
			}
		}

#if false
		private int ApplyCraftAttributeElementDamage( int attrDamage, ref int element, int totalRemaining )
		{
			if( totalRemaining <= 0 )
				return 0;

			if ( attrDamage <= 0 )
				return totalRemaining;

			int appliedDamage = attrDamage;

			if ( (appliedDamage + element) > 100 )
				appliedDamage = 100 - element;

			if( appliedDamage > totalRemaining )
				appliedDamage = totalRemaining;

			element += appliedDamage;

			return totalRemaining - appliedDamage;
		}
#endif

		public virtual void OnMiss( Mobile attacker, Mobile defender )
		{
			PlaySwingAnimation( attacker );
			attacker.PlaySound( GetMissAttackSound( attacker, defender ) );
			defender.PlaySound( GetMissDefendSound( attacker, defender ) );

#if false
			WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

			if ( ability != null )
				ability.OnMiss( attacker, defender );

			SpecialMove move = SpecialMove.GetCurrentMove( attacker );

			if ( move != null )
				move.OnMiss( attacker, defender );

			if ( defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null )
				((IHonorTarget)defender).ReceivedHonorContext.OnTargetMissed( attacker );
#endif
		}

		public virtual void GetBaseDamageRange( Mobile attacker, out int min, out int max )
		{
			if ( attacker is BaseCreature )
			{
				BaseCreature c = (BaseCreature)attacker;

				#region mod by Dies Irae
				if( c.DamageDice != null )
				{
					int result = c.DamageDice.Roll();
					SendDebugMessage( attacker, "c.DamageDice: {0}d{1}+{2} rolled {3}", c.DamageDice.Count, c.DamageDice.Sides, c.DamageDice.Bonus, result );

					min = result;
					max = result;
					return;
				}
				#endregion
				else if ( c.DamageMin >= 0 )
				{
					min = c.DamageMin;
					max = c.DamageMax;

					SendDebugOverheadMessage( attacker, string.Format( "m:{0} M:{1}", MinDamage, MaxDamage ) ); // mod by Dies Irae

					return;
				}

				if ( this is Fists && !attacker.Body.IsHuman )
				{
					min = attacker.Str/ 20; // 28;
					max = attacker.Str/ 20; // 28;

					SendDebugOverheadMessage( attacker, string.Format( "m:{0} M:{1}", MinDamage, MaxDamage ) ); // mod by Dies Irae

					return;
				}
			}

			SendDebugOverheadMessage( attacker, string.Format( "m:{0} M:{1}", MinDamage, MaxDamage ) ); // mod by Dies Irae

			min = MinDamage;
			max = MaxDamage;
		}

		public virtual double GetBaseDamage( Mobile attacker )
		{
            #region mod by Dies Irae: pre-AOS stuff
            if( m_CriticalHit )
                return MaxDiceDamage;

            if( attacker.Player || ( attacker is BaseCreature && ( (BaseCreature)attacker ).UsesWeaponDamage ) )
            {
                int morphDamage = Morph.AlterDamage( attacker );
                if( morphDamage != -1 )
                    return morphDamage;

                if( NumDice > 0 )
                {
                    int rolled = Utility.Dice( NumDice, NumSides, DiceBonus );//MaxDiceDamage
                    if( attacker.PlayerDebug )
                        attacker.SendMessage( "Danno arma {0}d{1}+{2}. Uscito: {3}", NumDice, NumSides, DiceBonus, rolled );

						//bool OldPolRules = attacker.PlayerDebug;

                    if( OldPolRules && this.Layer == Layer.OneHanded )
                    {
                        int maxDamage = NumDice * NumSides + DiceBonus;
                        int minDamage = NumDice + DiceBonus;
                        int tactDamage = minDamage + (int)( (double)( ( maxDamage - minDamage ) * ( ( attacker.Skills[ SkillName.Tactics ].Value - 30 ) / 100.0 ) ) );

                        if( tactDamage > maxDamage )
                            tactDamage = maxDamage - 1;

                        rolled = Utility.RandomMinMax( tactDamage, maxDamage );
                        if( attacker.PlayerDebug )
                            attacker.SendMessage( "Modifica onehand, uscito: {0}, Minimo: {1}", (int)rolled, (int)tactDamage );
                    }

                    return rolled;
                }
                else
                {
                    attacker.Say( "This weapon is no longer supported in pre-aos. Please contact Dies Irae." );
                    return 0;
                }
            }
            #endregion

			int min, max;

			GetBaseDamageRange( attacker, out min, out max );

			SendDebugMessage( attacker, "GetBaseDamageRange: {0} {1}", min, max ); // mod by Dies Irae

			return Utility.RandomMinMax( min, max );
		}

		public virtual double GetBonus( double value, double scalar, double threshold, double offset )
		{
			double bonus = value * scalar;

			if ( value >= threshold )
				bonus += offset;

			return bonus / 100;
		}

		public virtual int GetHitChanceBonus()
		{
            return 0;

#if false
            if ( !Core.AOS )
				return 0;

			int bonus = 0;

			switch ( m_AccuracyLevel )
			{
				case WeaponAccuracyLevel.Accurate:		bonus += 02; break;
				case WeaponAccuracyLevel.Surpassingly:	bonus += 04; break;
				case WeaponAccuracyLevel.Eminently:		bonus += 06; break;
				case WeaponAccuracyLevel.Exceedingly:	bonus += 08; break;
				case WeaponAccuracyLevel.Supremely:		bonus += 10; break;
			}

			return bonus;
#endif
        }

#if false
		public virtual int GetDamageBonus()
		{
			int bonus = VirtualDamageBonus;

			switch ( m_Quality )
			{
				case WeaponQuality.Low:			bonus -= 20; break;
				case WeaponQuality.Exceptional:	bonus += 20; break;
			}

			switch ( m_DamageLevel )
			{
				case WeaponDamageLevel.Ruin:	bonus += 15; break;
				case WeaponDamageLevel.Might:	bonus += 20; break;
				case WeaponDamageLevel.Force:	bonus += 25; break;
				case WeaponDamageLevel.Power:	bonus += 30; break;
				case WeaponDamageLevel.Vanq:	bonus += 35; break;
			}

			return bonus;
		}
#endif

		public virtual void GetStatusDamage( Mobile from, out int min, out int max )
		{
			int baseMin, baseMax;

			GetBaseDamageRange( from, out baseMin, out baseMax );

			SendDebugMessage( from, "BaseDamage ->m:{0} M:{1}", baseMin, baseMax );
            min = Math.Max( (int)ScaleDamageSecondAge( from, null, baseMin, false ), 1 );
            max = Math.Max( (int)ScaleDamageSecondAge( from, null, baseMax, false ), 1 );

#if false
			if ( Core.AOS )
			{
				min = Math.Max( (int)ScaleDamageAOS( from, baseMin, false ), 1 );
				max = Math.Max( (int)ScaleDamageAOS( from, baseMax, false ), 1 );
			}
			else
			{
				min = Math.Max( (int)ScaleDamageOld( from, baseMin, false ), 1 );
				max = Math.Max( (int)ScaleDamageOld( from, baseMax, false ), 1 );
			}
#endif
		}

#if false
		public virtual double ScaleDamageAOS( Mobile attacker, double damage, bool checkSkills )
		{
			if ( checkSkills )
			{
				attacker.CheckSkill( SkillName.Tactics, 0.0, attacker.Skills[SkillName.Tactics].Cap ); // Passively check tactics for gain
				attacker.CheckSkill( SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap ); // Passively check Anatomy for gain

				if ( Type == WeaponType.Axe )
					attacker.CheckSkill( SkillName.Lumberjacking, 0.0, 100.0 ); // Passively check Lumberjacking for gain
			}

			#region Physical bonuses
			/*
			 * These are the bonuses given by the physical characteristics of the mobile.
			 * No caps apply.
			 */
			double strengthBonus = GetBonus( attacker.Str,										0.300, 100.0,  5.00 );
			double  anatomyBonus = GetBonus( attacker.Skills[SkillName.Anatomy].Value,			0.500, 100.0,  5.00 );
			double  tacticsBonus = GetBonus( attacker.Skills[SkillName.Tactics].Value,			0.625, 100.0,  6.25 );
			double   lumberBonus = GetBonus( attacker.Skills[SkillName.Lumberjacking].Value,	0.200, 100.0, 10.00 );

			if ( Type != WeaponType.Axe )
				lumberBonus = 0.0;
			#endregion

			#region Modifiers
			/*
			 * The following are damage modifiers whose effect shows on the status bar.
			 * Capped at 100% total.
			 */
			int damageBonus = AosAttributes.GetValue( attacker, AosAttribute.WeaponDamage );

			// Horrific Beast transformation gives a +25% bonus to damage.
			if( TransformationSpellHelper.UnderTransformation( attacker, typeof( HorrificBeastSpell ) ) )
				damageBonus += 25;

			// Divine Fury gives a +10% bonus to damage.
			if ( Spells.Chivalry.DivineFurySpell.UnderEffect( attacker ) )
				damageBonus += 10;

			int defenseMasteryMalus = 0;

			// Defense Mastery gives a -50%/-80% malus to damage.
			if ( Server.Items.DefenseMastery.GetMalus( attacker, ref defenseMasteryMalus ) )
				damageBonus -= defenseMasteryMalus;

			int discordanceEffect = 0;

			// Discordance gives a -2%/-48% malus to damage.
			if ( SkillHandlers.Discordance.GetEffect( attacker, ref discordanceEffect ) )
				damageBonus -= discordanceEffect * 2;

			if ( damageBonus > 100 )
				damageBonus = 100;
			#endregion

			double totalBonus = strengthBonus + anatomyBonus + tacticsBonus + lumberBonus + ((double)(GetDamageBonus() + damageBonus) / 100.0);

			return damage + (int)(damage * totalBonus);
		}
#endif

		public virtual int VirtualDamageBonus{ get{ return 0; } }

#if false
		public virtual int ComputeDamageAOS( Mobile attacker, Mobile defender )
		{
			return (int)ScaleDamageAOS( attacker, GetBaseDamage( attacker ), true );
		}
#endif

		public virtual double ScaleDamageOld( Mobile attacker, double damage, bool checkSkills )
		{
		    return ScaleDamageSecondAge( attacker, null, damage, checkSkills ); // mod by Dies Irae
#if false
			if ( checkSkills )
			{
				attacker.CheckSkill( SkillName.Tactics, 0.0, attacker.Skills[SkillName.Tactics].Cap ); // Passively check tactics for gain
				attacker.CheckSkill( SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap ); // Passively check Anatomy for gain

				if ( Type == WeaponType.Axe )
					attacker.CheckSkill( SkillName.Lumberjacking, 0.0, 100.0 ); // Passively check Lumberjacking for gain
			}

			/* Compute tactics modifier
			 * :   0.0 = 50% loss
			 * :  50.0 = unchanged
			 * : 100.0 = 50% bonus
			 */
			double tacticsBonus = (attacker.Skills[SkillName.Tactics].Value - 50.0) / 100.0;

			/* Compute strength modifier
			 * : 1% bonus for every 5 strength
			 */
			double strBonus = (attacker.Str / 5.0) / 100.0;

			/* Compute anatomy modifier
			 * : 1% bonus for every 5 points of anatomy
			 * : +10% bonus at Grandmaster or higher
			 */
			double anatomyValue = attacker.Skills[SkillName.Anatomy].Value;
			double anatomyBonus = (anatomyValue / 5.0) / 100.0;

			if ( anatomyValue >= 100.0 )
				anatomyBonus += 0.1;

			/* Compute lumberjacking bonus
			 * : 1% bonus for every 5 points of lumberjacking
			 * : +10% bonus at Grandmaster or higher
			 */
			double lumberBonus;

			if ( Type == WeaponType.Axe )
			{
				double lumberValue = attacker.Skills[SkillName.Lumberjacking].Value;

				lumberBonus = (lumberValue / 5.0) / 100.0;

				if ( lumberValue >= 100.0 )
					lumberBonus += 0.1;
			}
			else
			{
				lumberBonus = 0.0;
			}

			// New quality bonus:
			double qualityBonus = ((int)m_Quality - 1) * 0.2;

			// Apply bonuses
			damage += (damage * tacticsBonus) + (damage * strBonus) + (damage * anatomyBonus) + (damage * lumberBonus) + (damage * qualityBonus) + ((damage * VirtualDamageBonus) / 100);

			// Old quality bonus:
// #if false
			/* Apply quality offset
			 * : Low         : -4
			 * : Regular     :  0
			 * : Exceptional : +4
			 */
			damage += ((int)m_Quality - 1) * 4.0;
// #endif

			/* Apply damage level offset
			 * : Regular : 0
			 * : Ruin    : 1
			 * : Might   : 3
			 * : Force   : 5
			 * : Power   : 7
			 * : Vanq    : 9
			 */
			if ( m_DamageLevel != WeaponDamageLevel.Regular )
				damage += (2.0 * (int)m_DamageLevel) - 1.0;

			// Halve the computed damage and return
			damage /= 2.0;

 			return ScaleDamageByDurability( (int)damage );
#endif
		}

		public virtual int ScaleDamageByDurability( int damage )
		{
			int scale = 100;

			if ( m_MaxHits > 0 && m_Hits < m_MaxHits )
				scale = 50 + ((50 * m_Hits) / m_MaxHits);

			return AOS.Scale( damage, scale );
		}

		public virtual int ComputeDamage( Mobile attacker, Mobile defender )
		{
			/*
			if( Core.AOS )
				return ComputeDamageAOS( attacker, defender );
			*/

		    return (int)ScaleDamageSecondAge( attacker, defender, GetBaseDamage( attacker ), true ); // mod by Dies Irae

		    // return (int)ScaleDamageOld( attacker, GetBaseDamage( attacker ), true );
		}

		public virtual void PlayHurtAnimation( Mobile from )
		{
			int action;
			int frames;

			switch ( from.Body.Type )
			{
				case BodyType.Sea:
				case BodyType.Animal:
				{
					action = 7;
					frames = 5;
					break;
				}
				case BodyType.Monster:
				{
					action = 10;
					frames = 4;
					break;
				}
				case BodyType.Human:
				{
					action = 20;
					frames = 5;
					break;
				}
				default: return;
			}

			if ( from.Mounted )
				return;

			from.Animate( action, frames, 1, true, false, 0 );
		}

		public virtual void PlaySwingAnimation( Mobile from )
		{
			int action;

			switch ( from.Body.Type )
			{
				case BodyType.Sea:
				case BodyType.Animal:
				{
					action = Utility.Random( 5, 2 );
					break;
				}
				case BodyType.Monster:
				{
					switch ( Animation )
					{
						default:
						case WeaponAnimation.Wrestle:
						case WeaponAnimation.Bash1H:
						case WeaponAnimation.Pierce1H:
						case WeaponAnimation.Slash1H:
						case WeaponAnimation.Bash2H:
						case WeaponAnimation.Pierce2H:
						case WeaponAnimation.Slash2H: action = Utility.Random( 4, 3 ); break;
						case WeaponAnimation.ShootBow:  return; // 7
						case WeaponAnimation.ShootXBow: return; // 8
					}

					break;
				}
				case BodyType.Human:
				{
					if ( !from.Mounted )
					{
						action = (int)Animation;
					}
					else
					{
						switch ( Animation )
						{
							default:
							case WeaponAnimation.Wrestle:
							case WeaponAnimation.Bash1H:
							case WeaponAnimation.Pierce1H:
							case WeaponAnimation.Slash1H: action = 26; break;
							case WeaponAnimation.Bash2H:
							case WeaponAnimation.Pierce2H:
							case WeaponAnimation.Slash2H: action = 29; break;
							case WeaponAnimation.ShootBow: action = 27; break;
							case WeaponAnimation.ShootXBow: action = 28; break;
						}
					}

					break;
				}
				default: return;
			}

			from.Animate( action, 7, 1, true, false, 0 );
		}

		#region Serialization/Deserialization
		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 14 ); // version

			#region mod by Arlas version 14
			writer.Write( (int) m_MinRange );
			#endregion

			#region Mondain's Legacy version 11
			writer.Write( (int)m_Slayer3 );
			#endregion

			#region mod by Dies Irae version 10
			MidgardSerialize( writer );
			#endregion

			#region Mondain's Legacy version 9
			SetFlag sflags = SetFlag.None;
			
			SetSaveFlag( ref sflags, SetFlag.Attributes,		!m_SetAttributes.IsEmpty );
			SetSaveFlag( ref sflags, SetFlag.SkillBonuses,		!m_SetSkillBonuses.IsEmpty );
			SetSaveFlag( ref sflags, SetFlag.Hue,				m_SetHue != 0 );
			SetSaveFlag( ref sflags, SetFlag.LastEquipped,		m_LastEquipped );
			SetSaveFlag( ref sflags, SetFlag.SetEquipped,		m_SetEquipped );
			SetSaveFlag( ref sflags, SetFlag.SetSelfRepair,		m_SetSelfRepair != 0 );
			
			writer.WriteEncodedInt( (int) sflags );
			
			if ( GetSaveFlag( sflags, SetFlag.Attributes ) )
				m_SetAttributes.Serialize( writer );

			if ( GetSaveFlag( sflags, SetFlag.SkillBonuses ) )
				m_SetSkillBonuses.Serialize( writer );
				
			if ( GetSaveFlag( sflags, SetFlag.Hue ) )
				writer.Write( (int) m_SetHue );
				
			if ( GetSaveFlag( sflags, SetFlag.LastEquipped ) )
				writer.Write( (bool) m_LastEquipped );
				
			if ( GetSaveFlag( sflags, SetFlag.SetEquipped ) )
				writer.Write( (bool) m_SetEquipped );
				
			if ( GetSaveFlag( sflags, SetFlag.SetSelfRepair ) )
				writer.WriteEncodedInt( (int) m_SetSelfRepair );
	
			if ( GetSaveFlag( sflags, SetFlag.UnSetHue ) )
				writer.Write( (int) m_UnSetHue );
			#endregion

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.DamageLevel,		m_DamageLevel != WeaponDamageLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.AccuracyLevel,		m_AccuracyLevel != WeaponAccuracyLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.DurabilityLevel,	m_DurabilityLevel != WeaponDurabilityLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Quality,		m_Quality != WeaponQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.Hits,			m_Hits != 0 );
			SetSaveFlag( ref flags, SaveFlag.MaxHits,		m_MaxHits != 0 );
			SetSaveFlag( ref flags, SaveFlag.Slayer,		m_Slayer != SlayerName.None );
			SetSaveFlag( ref flags, SaveFlag.Poison,		m_Poison != null );
			SetSaveFlag( ref flags, SaveFlag.PoisonCharges,		m_PoisonCharges != 0 );
			SetSaveFlag( ref flags, SaveFlag.Crafter,		m_Crafter != null );
			// SetSaveFlag( ref flags, SaveFlag.Identified,		m_Identified != false );
			SetSaveFlag( ref flags, SaveFlag.StrReq,		m_StrReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexReq,		m_DexReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntReq,		m_IntReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.MinDamage,		m_MinDamage != -1 );
			SetSaveFlag( ref flags, SaveFlag.MaxDamage,		m_MaxDamage != -1 );
			SetSaveFlag( ref flags, SaveFlag.HitSound,		m_HitSound != -1 );
			SetSaveFlag( ref flags, SaveFlag.MissSound,		m_MissSound != -1 );
			SetSaveFlag( ref flags, SaveFlag.Speed,			m_Speed != -1 );
			SetSaveFlag( ref flags, SaveFlag.MaxRange,		m_MaxRange != -1 );
			SetSaveFlag( ref flags, SaveFlag.Skill,			m_Skill != (SkillName)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Type,			m_Type != (WeaponType)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Animation,		m_Animation != (WeaponAnimation)(-1) );
			SetSaveFlag( ref flags, SaveFlag.Resource,		m_Resource != CraftResource.Iron );
			SetSaveFlag( ref flags, SaveFlag.xAttributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.xWeaponAttributes,	!m_AosWeaponAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.Slayer2,		m_Slayer2 != SlayerName.None );
			SetSaveFlag( ref flags, SaveFlag.ElementalDamages,	!m_AosElementDamages.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.EngravedText,		!String.IsNullOrEmpty( m_EngravedText ) );

			writer.Write( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
				writer.Write( (int) m_DamageLevel );

			if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
				writer.Write( (int) m_AccuracyLevel );

			if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
				writer.Write( (int) m_DurabilityLevel );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.Write( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.Hits ) )
				writer.Write( (int) m_Hits );

			if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
				writer.Write( (int) m_MaxHits );

			if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
				writer.Write( (int) m_Slayer );

			if ( GetSaveFlag( flags, SaveFlag.Poison ) )
				Poison.Serialize( m_Poison, writer );

			if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
				writer.Write( (int) m_PoisonCharges );

			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( (Mobile) m_Crafter );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.Write( (int) m_StrReq );

			if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
				writer.Write( (int) m_DexReq );

			if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
				writer.Write( (int) m_IntReq );

			if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
				writer.Write( (int) m_MinDamage );

			if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
				writer.Write( (int) m_MaxDamage );

			if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
				writer.Write( (int) m_HitSound );

			if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
				writer.Write( (int) m_MissSound );

			if ( GetSaveFlag( flags, SaveFlag.Speed ) )
				writer.Write( (float) m_Speed );

			if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
				writer.Write( (int) m_MaxRange );

			if ( GetSaveFlag( flags, SaveFlag.Skill ) )
				writer.Write( (int) m_Skill );

			if ( GetSaveFlag( flags, SaveFlag.Type ) )
				writer.Write( (int) m_Type );

			if ( GetSaveFlag( flags, SaveFlag.Animation ) )
				writer.Write( (int) m_Animation );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.Write( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
				m_AosWeaponAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.Slayer2 ) )
				writer.Write( (int)m_Slayer2 );

			if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
				m_AosElementDamages.Serialize( writer );

			if( GetSaveFlag( flags, SaveFlag.EngravedText ) )
				writer.Write( (string) m_EngravedText );
		}

		[Flags]
		private enum SaveFlag
		{
			None			= 0x00000000,
			DamageLevel		= 0x00000001,
			AccuracyLevel		= 0x00000002,
			DurabilityLevel		= 0x00000004,
			Quality			= 0x00000008,
			Hits			= 0x00000010,
			MaxHits			= 0x00000020,
			Slayer			= 0x00000040,
			Poison			= 0x00000080,
			PoisonCharges		= 0x00000100,
			Crafter			= 0x00000200,
			Identified		= 0x00000400,
			StrReq			= 0x00000800,
			DexReq			= 0x00001000,
			IntReq			= 0x00002000,
			MinDamage		= 0x00004000,
			MaxDamage		= 0x00008000,
			HitSound		= 0x00010000,
			MissSound		= 0x00020000,
			Speed			= 0x00040000,
			MaxRange		= 0x00080000,
			Skill			= 0x00100000,
			Type			= 0x00200000,
			Animation		= 0x00400000,
			Resource		= 0x00800000,
			xAttributes		= 0x01000000,
			xWeaponAttributes	= 0x02000000,
			PlayerConstructed	= 0x04000000,
			SkillBonuses		= 0x08000000,
			Slayer2			= 0x10000000,
			ElementalDamages	= 0x20000000,
			EngravedText		= 0x40000000
		}
		#endregion

		#region Mondain's Legacy
		private static void SetSaveFlag( ref SetFlag flags, SetFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SetFlag flags, SetFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}
		
		[Flags]
		private enum SetFlag
		{
			None			= 0x00000000,
			Attributes		= 0x00000001,
			WeaponAttributes	= 0x00000002,
			SkillBonuses		= 0x00000004,
			Hue			= 0x00000008,
			LastEquipped		= 0x00000010,
			SetEquipped		= 0x00000020,
			SetSelfRepair		= 0x00000040,
			UnSetHue		= 0x00000080, // modifica by Dies Irae
		}
		#endregion

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				#region mod by Arlas
				case 14:
					m_MinRange = reader.ReadInt();
					if ( m_MinRange == 0 )
						m_MinRange = -1;

					goto case 13;
				#endregion
				#region mod by Dies Irae
				case 13:
					goto case 12;
				#endregion
				#region Mondain's Legacy
				case 12:
					goto case 11;
				case 11:
				{
					m_Slayer3 = (TalismanSlayerName)reader.ReadInt();
					goto case 10;
				}
				#endregion
				#region mod by Dies Irae
				case 10:
				{
					MidgardDeserialize( reader );
					goto case 9;
				}
				#endregion
				#region Mondain's Legacy
				case 9:
				{
					SetFlag flags = (SetFlag) reader.ReadEncodedInt();
					
					if ( GetSaveFlag( flags, SetFlag.Attributes ) )
						m_SetAttributes = new AosAttributes( this, reader );
					else
						m_SetAttributes = new AosAttributes( this );
					
					if ( GetSaveFlag( flags, SetFlag.WeaponAttributes ) )
						m_SetSelfRepair = (new AosWeaponAttributes( this, reader )).SelfRepair;
						
					if ( GetSaveFlag( flags, SetFlag.SkillBonuses ) )
						m_SetSkillBonuses = new AosSkillBonuses( this, reader );
					else
						m_SetSkillBonuses =  new AosSkillBonuses( this );
						
					if ( GetSaveFlag( flags, SetFlag.Hue ) )
						m_SetHue = reader.ReadInt();
						
					if ( GetSaveFlag( flags, SetFlag.LastEquipped ) )
						m_LastEquipped = reader.ReadBool();
						
					if ( GetSaveFlag( flags, SetFlag.SetEquipped ) )
						m_SetEquipped = reader.ReadBool();
						
					if ( GetSaveFlag( flags, SetFlag.SetSelfRepair ) )
						m_SetSelfRepair = reader.ReadEncodedInt();

					#region modifica by Dies Irae
					if ( GetSaveFlag( flags, SetFlag.UnSetHue ) )
						m_UnSetHue = reader.ReadInt();					
					#endregion
					goto case 8;
				}
				#endregion
				case 8:
				case 7:
				case 6:
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
					{
						m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();

						if ( m_DamageLevel > WeaponDamageLevel.Vanq )
							m_DamageLevel = WeaponDamageLevel.Ruin;
					}

					if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
					{
						m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();

						if ( m_AccuracyLevel > WeaponAccuracyLevel.Supremely )
							m_AccuracyLevel = WeaponAccuracyLevel.Accurate;
					}

					if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
					{
						m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();

						if ( m_DurabilityLevel > WeaponDurabilityLevel.Indestructible )
							m_DurabilityLevel = WeaponDurabilityLevel.Durable;
					}

					if ( GetSaveFlag( flags, SaveFlag.Quality ) )
						m_Quality = (WeaponQuality)reader.ReadInt();
					else
						m_Quality = WeaponQuality.Regular;

					if ( GetSaveFlag( flags, SaveFlag.Hits ) )
						m_Hits = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
						m_MaxHits = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
						m_Slayer = (SlayerName)reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Poison ) )
						m_Poison = Poison.Deserialize( reader );

					if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
						m_PoisonCharges = reader.ReadInt();

					if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
						m_Crafter = reader.ReadMobile();

                    /*
					if ( GetSaveFlag( flags, SaveFlag.Identified ) )
						m_Identified = ( version >= 6 || reader.ReadBool() );
                    */

					if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
						m_StrReq = reader.ReadInt();
					else
						m_StrReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
						m_DexReq = reader.ReadInt();
					else
						m_DexReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
						m_IntReq = reader.ReadInt();
					else
						m_IntReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
						m_MinDamage = reader.ReadInt();
					else
						m_MinDamage = -1;

					if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
						m_MaxDamage = reader.ReadInt();
					else
						m_MaxDamage = -1;

					if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
						m_HitSound = reader.ReadInt();
					else
						m_HitSound = -1;

					if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
						m_MissSound = reader.ReadInt();
					else
						m_MissSound = -1;

					if ( GetSaveFlag( flags, SaveFlag.Speed ) )
					{
						if ( version < 12 )
							m_Speed = reader.ReadInt();
						else
							m_Speed = reader.ReadFloat();
					}
					else
						m_Speed = -1;

					if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
						m_MaxRange = reader.ReadInt();
					else
						m_MaxRange = -1;

					if ( GetSaveFlag( flags, SaveFlag.Skill ) )
						m_Skill = (SkillName)reader.ReadInt();
					else
						m_Skill = (SkillName)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Type ) )
						m_Type = (WeaponType)reader.ReadInt();
					else
						m_Type = (WeaponType)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Animation ) )
						m_Animation = (WeaponAnimation)reader.ReadInt();
					else
						m_Animation = (WeaponAnimation)(-1);

					if ( GetSaveFlag( flags, SaveFlag.Resource ) )
						m_Resource = (CraftResource)reader.ReadInt();
					else
						m_Resource = CraftResource.Iron;

					if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
						m_AosWeaponAttributes = new AosWeaponAttributes( this, reader );
					else
						m_AosWeaponAttributes = new AosWeaponAttributes( this );

					if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
					{
						m_SkillMod = new DefaultSkillMod( AccuracySkill, true, /* (int)m_AccuracyLevel * 5 */ AccuracyBonus );
						((Mobile)Parent).AddSkillMod( m_SkillMod );
					}

					if ( version < 7 && m_AosWeaponAttributes.MageWeapon != 0 )
						m_AosWeaponAttributes.MageWeapon = 30 - m_AosWeaponAttributes.MageWeapon;

					if ( Core.AOS && m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30 && Parent is Mobile )
					{
						m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon );
						((Mobile)Parent).AddSkillMod( m_MageMod );
					}

					if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
						m_PlayerConstructed = true;

					if( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );
					else
						m_AosSkillBonuses = new AosSkillBonuses( this );

					if( GetSaveFlag( flags, SaveFlag.Slayer2 ) )
						m_Slayer2 = (SlayerName)reader.ReadInt();

					if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
						m_AosElementDamages = new AosElementAttributes( this, reader );
					else
						m_AosElementDamages = new AosElementAttributes( this );

					if( GetSaveFlag( flags, SaveFlag.EngravedText ) )
						m_EngravedText = reader.ReadString();

					break;
				}
				case 4:
				{
					m_Slayer = (SlayerName)reader.ReadInt();

					goto case 3;
				}
				case 3:
				{
					m_StrReq = reader.ReadInt();
					m_DexReq = reader.ReadInt();
					m_IntReq = reader.ReadInt();

					goto case 2;
				}
				case 2:
				{
					// m_Identified = reader.ReadBool();

					goto case 1;
				}
				case 1:
				{
					m_MaxRange = reader.ReadInt();

					goto case 0;
				}
				case 0:
				{
					if ( version == 0 )
						m_MaxRange = 1; // default

					if ( version < 5 )
					{
						m_Resource = CraftResource.Iron;
						m_AosAttributes = new AosAttributes( this );
						m_AosWeaponAttributes = new AosWeaponAttributes( this );
						m_AosElementDamages = new AosElementAttributes( this );
						m_AosSkillBonuses = new AosSkillBonuses( this );
					}

					m_MinDamage = reader.ReadInt();
					m_MaxDamage = reader.ReadInt();

					m_Speed = reader.ReadInt();

					m_HitSound = reader.ReadInt();
					m_MissSound = reader.ReadInt();

					m_Skill = (SkillName)reader.ReadInt();
					m_Type = (WeaponType)reader.ReadInt();
					m_Animation = (WeaponAnimation)reader.ReadInt();
					m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();
					m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();
					m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();
					m_Quality = (WeaponQuality)reader.ReadInt();

					m_Crafter = reader.ReadMobile();

					m_Poison = Poison.Deserialize( reader );
					m_PoisonCharges = reader.ReadInt();

					if ( m_StrReq == OldStrengthReq )
						m_StrReq = -1;

					if ( m_DexReq == OldDexterityReq )
						m_DexReq = -1;

					if ( m_IntReq == OldIntelligenceReq )
						m_IntReq = -1;

					if ( m_MinDamage == OldMinDamage )
						m_MinDamage = -1;

					if ( m_MaxDamage == OldMaxDamage )
						m_MaxDamage = -1;

					if ( m_HitSound == OldHitSound )
						m_HitSound = -1;

					if ( m_MissSound == OldMissSound )
						m_MissSound = -1;

					if ( m_Speed == OldSpeed )
						m_Speed = -1;

					if ( m_MinRange == OldMinRange )
						m_MinRange = -1;

					if ( m_MaxRange == OldMaxRange )
						m_MaxRange = -1;

					if ( m_Skill == OldSkill )
						m_Skill = (SkillName)(-1);

					if ( m_Type == OldType )
						m_Type = (WeaponType)(-1);

					if ( m_Animation == OldAnimation )
						m_Animation = (WeaponAnimation)(-1);

					if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
					{
						m_SkillMod = new DefaultSkillMod( AccuracySkill, true, /* (int)m_AccuracyLevel * 5 */ AccuracyBonus );
						((Mobile)Parent).AddSkillMod( m_SkillMod );
					}

					break;
				}
			}

			#region Mondain's Legacy Sets
			if ( m_SetAttributes == null )
				m_SetAttributes = new AosAttributes( this );
				
			if ( m_SetSkillBonuses == null )
				m_SetSkillBonuses =  new AosSkillBonuses( this );	
			#endregion

			if ( Core.AOS && Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile)Parent );

			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( this.Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)this.Parent;

				string modName = this.Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();

			if ( m_Hits <= 0 && m_MaxHits <= 0 )
			{
				m_Hits = m_MaxHits = Utility.RandomMinMax( InitMinHits, InitMaxHits );
			}

			if ( version < 6 )
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted

            #region mod by Dies Irae
#if verify
			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Midgard.CraftHelper.VerifyManaRegenerationItems_Callback ), this );
#endif

            if( this is ITreasureOfMidgard )
                TreasuresOfMidgard.RegisterExistance( GetType() );

            //if( this is BaseRanged )
            //    Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( CraftHelper.VerifyWillowRunicCraftedItems_Callback ), this );
            #endregion
		}

		public BaseWeapon( int itemID ) : base( itemID )
		{
			Layer = (Layer)ItemData.Quality;

			m_Quality = WeaponQuality.Regular;
			m_StrReq = -1;
			m_DexReq = -1;
			m_IntReq = -1;
			m_MinDamage = -1;
			m_MaxDamage = -1;
			m_HitSound = -1;
			m_MissSound = -1;
			m_Speed = -1;
			m_MinRange = -1;//edit by Arlas
			m_MaxRange = -1;
			m_Skill = (SkillName)(-1);
			m_Type = (WeaponType)(-1);
			m_Animation = (WeaponAnimation)(-1);

			m_Hits = m_MaxHits = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			m_Resource = CraftResource.Iron;

			m_AosAttributes = new AosAttributes( this );
			m_AosWeaponAttributes = new AosWeaponAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_AosElementDamages = new AosElementAttributes( this );
			
			#region Mondain's Legacy Sets
			m_SetAttributes = new AosAttributes( this );
			m_SetSkillBonuses = new AosSkillBonuses( this );
			#endregion

            #region mod by Dies Irae
            SkillsRequiredToEnchant = new SkillName[] { SkillName.ItemID, SkillName.ArmsLore, SkillName.Blacksmith };

            m_SecondAgeUnIdentifiedName = null;
            m_SecondAgeFullName = null;

            MagicalCharges = 0;
            MagicalAttribute = WeaponMagicalAttribute.None;
            m_IdentifiersList = new List<Mobile>();
            PoisonerSkill = 0.0;

            if( this is ITreasureOfMidgard )
                TreasuresOfMidgard.RegisterExistance( GetType() );

            if( this is IOrderWeapon || this is IChaosWeapon )
                LootType = LootType.Newbied;

            CustomQuality = Server.Quality.Standard;
            #endregion

            #region mob my Magius(CHE): HitRate/Evasion
            double hitrate = 0.0;
            double evasion = 0.0;
            SetupHitRateAndEvasionBonus( out hitrate, out evasion );
            SetHitRateBonus( hitrate );
            SetEvasionBonus( evasion );
            #endregion

            m_DisplayAttributes = new BaseWeaponAttribute( this );
        }

		public BaseWeapon( Serial serial ) : base( serial )
		{
			m_SecondAgeUnIdentifiedName = null;
			m_SecondAgeFullName = null;
			SkillsRequiredToEnchant = new SkillName[] { SkillName.ItemID, SkillName.ArmsLore, SkillName.Blacksmith };

			#region mob my Magius(CHE): HitRate/Evasion
			/* RESET dei valori. è necessario che questio valori stiano sia qui che nel costruttore altrimenti saranno 0 al prossimo riavvio*/
			double hitrate = 0.0;
			double evasion = 0.0;
			SetupHitRateAndEvasionBonus( out hitrate, out evasion );
			SetHitRateBonus( hitrate );
			SetEvasionBonus( evasion );
			#endregion

			m_DisplayAttributes = new BaseWeaponAttribute( this );
		}

#if false
		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}
#endif

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; InvalidateProperties(); }
		}

#if false
		public int GetElementalDamageHue()
		{
			int phys, fire, cold, pois, nrgy, chaos, direct;
			GetDamageTypes( null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );
			//Order is Cold, Energy, Fire, Poison, Physical left

			int currentMax = 50;
			int hue = 0;

			if( pois >= currentMax )
			{
				hue = 1267 + (pois - 50) / 10;
				currentMax = pois;
			}

			if( fire >= currentMax )
			{
				hue = 1255 + (fire - 50) / 10;
				currentMax = fire;
			}

			if( nrgy >= currentMax )
			{
				hue = 1273 + (nrgy - 50) / 10;
				currentMax = nrgy;
			}

			if( cold >= currentMax )
			{
				hue = 1261 + (cold - 50) / 10;
				currentMax = cold;
			}

			return hue;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType;

			switch ( m_Resource )
			{
				case CraftResource.DullCopper:		oreType = 1053108; break; // dull copper
				case CraftResource.ShadowIron:		oreType = 1053107; break; // shadow iron
				case CraftResource.Copper:		oreType = 1053106; break; // copper
				case CraftResource.Bronze:		oreType = 1053105; break; // bronze
				case CraftResource.Gold:		oreType = 1053104; break; // golden
				case CraftResource.Agapite:		oreType = 1053103; break; // agapite
				case CraftResource.Verite:		oreType = 1053102; break; // verite
				case CraftResource.Valorite:		oreType = 1053101; break; // valorite
				case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
				case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
				case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
				case CraftResource.RedScales:		oreType = 1060814; break; // red
				case CraftResource.YellowScales:	oreType = 1060818; break; // yellow
				case CraftResource.BlackScales:		oreType = 1060820; break; // black
				case CraftResource.GreenScales:		oreType = 1060819; break; // green
				case CraftResource.WhiteScales:		oreType = 1060821; break; // white
				case CraftResource.BlueScales:		oreType = 1060815; break; // blue
				default: oreType = 0; break;
			}

			if ( oreType != 0 )
				list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
			else if ( Name == null )
				list.Add( LabelNumber );
			else
				list.Add( Name );
				
			/*
			 * Want to move this to the engraving tool, let the non-harmful 
			 * formatting show, and remove CLILOCs embedded: more like OSI
			 * did with the books that had markup, etc.
			 * 
			 * This will have a negative effect on a few event things imgame 
			 * as is.
			 * 
			 * If we cant find a more OSI-ish way to clean it up, we can 
			 * easily put this back, and use it in the deserialize
			 * method and engraving tool, to make it perm cleaned up.
			 */

			if ( !String.IsNullOrEmpty( m_EngravedText ) )
				list.Add( 1062613, m_EngravedText );

				/* list.Add( 1062613, Utility.FixHtml( m_EngravedText ) ); */
		}
#endif

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
		}

		public virtual int ArtifactRarity
		{
			get{ return 0; }
		}

#if false
		public virtual int GetLuckBonus()
		{
			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return 0;

			CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

			if ( attrInfo == null )
				return 0;

			return attrInfo.WeaponLuck;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			#region Factions
			if ( m_FactionState != null )
				list.Add( 1041350 ); // faction item
			#endregion

			#region Mondain's Legacy Sets
			if ( IsSetItem )
			{
				list.Add( 1073491, Pieces.ToString() ); // Part of a Weapon/Armor Set (~1_val~ pieces)
					
				if ( m_SetEquipped )
				{
					list.Add( 1073492 ); // Full Weapon/Armor Set Present			
					GetSetProperties( list );
				}
			}
			#endregion

			if ( m_AosSkillBonuses != null )
				m_AosSkillBonuses.GetProperties( list );

			if ( m_Quality == WeaponQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			if( RequiredRace == Race.Elf )
				list.Add( 1075086 ); // Elves Only

			if ( ArtifactRarity > 0 )
				list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~

			if ( this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining )
				list.Add( 1060584, ((IUsesRemaining)this).UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if ( m_Poison != null && m_PoisonCharges > 0 )
			{
				#region Mondain's Legacy mod
				list.Add( m_Poison.LabelNumber, m_PoisonCharges.ToString() );
				#endregion
			}

			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					list.Add( entry.Title );
			}

			#region Mondain's Legacy
			if ( m_Slayer3 != TalismanSlayerName.None )
				list.Add( (int) m_Slayer3 );
			#endregion


			base.AddResistanceProperties( list );

			int prop;

			if ( Core.ML && this is BaseRanged && ( (BaseRanged) this ).Balanced )
				list.Add( 1072792 ); // Balanced

			if ( (prop = m_AosWeaponAttributes.UseBestSkill) != 0 )
				list.Add( 1060400 ); // use best weapon skill

			if ( (prop = (GetDamageBonus() + m_AosAttributes.WeaponDamage)) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = (GetHitChanceBonus() + m_AosAttributes.AttackChance)) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitColdArea) != 0 )
				list.Add( 1060416, prop.ToString() ); // hit cold area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitDispel) != 0 )
				list.Add( 1060417, prop.ToString() ); // hit dispel ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitEnergyArea) != 0 )
				list.Add( 1060418, prop.ToString() ); // hit energy area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitFireArea) != 0 )
				list.Add( 1060419, prop.ToString() ); // hit fire area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitFireball) != 0 )
				list.Add( 1060420, prop.ToString() ); // hit fireball ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitHarm) != 0 )
				list.Add( 1060421, prop.ToString() ); // hit harm ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechHits) != 0 )
				list.Add( 1060422, prop.ToString() ); // hit life leech ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLightning) != 0 )
				list.Add( 1060423, prop.ToString() ); // hit lightning ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLowerAttack) != 0 )
				list.Add( 1060424, prop.ToString() ); // hit lower attack ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLowerDefend) != 0 )
				list.Add( 1060425, prop.ToString() ); // hit lower defense ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitMagicArrow) != 0 )
				list.Add( 1060426, prop.ToString() ); // hit magic arrow ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechMana) != 0 )
				list.Add( 1060427, prop.ToString() ); // hit mana leech ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitPhysicalArea) != 0 )
				list.Add( 1060428, prop.ToString() ); // hit physical area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitPoisonArea) != 0 )
				list.Add( 1060429, prop.ToString() ); // hit poison area ~1_val~%

			if ( (prop = m_AosWeaponAttributes.HitLeechStam) != 0 )
				list.Add( 1060430, prop.ToString() ); // hit stamina leech ~1_val~%

			if ( ImmolatingWeaponSpell.IsImmolating( this ) )
				list.Add( 1111917 ); // Immolated

			if ( Core.ML && this is BaseRanged && ( prop = ( (BaseRanged) this ).Velocity ) != 0 )
				list.Add( 1072793, prop.ToString() ); // Velocity ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%

			if ( (prop = GetLowerStatReq()) != 0 )
				list.Add( 1060435, prop.ToString() ); // lower requirements ~1_val~%

			if ( (prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_AosWeaponAttributes.MageWeapon) != 0 )
				list.Add( 1060438, (30 - prop).ToString() ); // mage weapon -~1_val~ skill

			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_AosWeaponAttributes.SelfRepair) != 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~

			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			if ( Core.ML && (prop = m_AosAttributes.IncreasedKarmaLoss) != 0 )
				list.Add( 1075210, prop.ToString() ); // Increased Karma Loss ~1val~%

			int phys, fire, cold, pois, nrgy, chaos, direct;

			GetDamageTypes( null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );

			if ( phys != 0 )
				list.Add( 1060403, phys.ToString() ); // physical damage ~1_val~%

			if ( fire != 0 )
				list.Add( 1060405, fire.ToString() ); // fire damage ~1_val~%

			if ( cold != 0 )
				list.Add( 1060404, cold.ToString() ); // cold damage ~1_val~%

			if ( pois != 0 )
				list.Add( 1060406, pois.ToString() ); // poison damage ~1_val~%

			if ( nrgy != 0 )
				list.Add( 1060407, nrgy.ToString() ); // energy damage ~1_val

			if ( Core.ML && chaos != 0 )
				list.Add( 1072846, chaos.ToString() ); // chaos damage ~1_val~%

			if ( Core.ML && direct != 0 )
				list.Add( 1079978, direct.ToString() ); // Direct Damage: ~1_PERCENT~%

			list.Add( 1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString() ); // weapon damage ~1_val~ - ~2_val~

			if ( Core.ML )
				list.Add( 1061167, String.Format( "{0}s", Speed ) ); // weapon speed ~1_val~
			else
				list.Add( 1061167, Speed.ToString() );

			if ( MaxRange > 1 )
				list.Add( 1061169, MaxRange.ToString() ); // range ~1_val~

			int strReq = AOS.Scale( StrRequirement, 100 - GetLowerStatReq() );

			if ( strReq > 0 )
				list.Add( 1061170, strReq.ToString() ); // strength requirement ~1_val~

			if ( Layer == Layer.TwoHanded )
				list.Add( 1061171 ); // two-handed weapon
			else
				list.Add( 1061824 ); // one-handed weapon

			if ( Core.SE || m_AosWeaponAttributes.UseBestSkill == 0 )
			{
				switch ( Skill )
				{
					case SkillName.Swords:  list.Add( 1061172 ); break; // skill required: swordsmanship
					case SkillName.Macing:  list.Add( 1061173 ); break; // skill required: mace fighting
					case SkillName.Fencing: list.Add( 1061174 ); break; // skill required: fencing
					case SkillName.Archery: list.Add( 1061175 ); break; // skill required: archery
				}
			}

			if ( m_Hits >= 0 && m_MaxHits > 0 )
				list.Add( 1060639, "{0}\t{1}", m_Hits, m_MaxHits ); // durability ~1_val~ / ~2_val~
				
            XmlAttach.AddAttachmentProperties( this, list ); // ARTEGORDONMOD mod to display attachment properties

			#region Mondain's Legacy Sets
			if ( IsSetItem && !m_SetEquipped )
			{
				list.Add( 1072378 ); // <br>Only when full set is present:				
				GetSetProperties( list );
			}
			#endregion
		}
#endif

		public override void OnSingleClick( Mobile from )
		{
            OnOldSingleClick( from );
#if false    


			List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

			if ( DisplayLootType )
			{
				if ( LootType == LootType.Blessed )
					attrs.Add( new EquipInfoAttribute( 1038021 ) ); // blessed
				else if ( LootType == LootType.Cursed )
					attrs.Add( new EquipInfoAttribute( 1049643 ) ); // cursed
			}

			#region Factions
			if ( m_FactionState != null )
				attrs.Add( new EquipInfoAttribute( 1041350 ) ); // faction item
			#endregion

			if ( m_Quality == WeaponQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );

			if ( m_Identified || from.AccessLevel >= AccessLevel.GameMaster )
			{
				if( m_Slayer != SlayerName.None )
				{
					SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
					if( entry != null )
						attrs.Add( new EquipInfoAttribute( entry.Title ) );
				}

				if( m_Slayer2 != SlayerName.None )
				{
					SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
					if( entry != null )
						attrs.Add( new EquipInfoAttribute( entry.Title ) );
				}

				if ( m_DurabilityLevel != WeaponDurabilityLevel.Regular )
					attrs.Add( new EquipInfoAttribute( 1038000 + (int)m_DurabilityLevel ) );

				if ( m_DamageLevel != WeaponDamageLevel.Regular )
					attrs.Add( new EquipInfoAttribute( 1038015 + (int)m_DamageLevel ) );

				if ( m_AccuracyLevel != WeaponAccuracyLevel.Regular )
					attrs.Add( new EquipInfoAttribute( 1038010 + (int)m_AccuracyLevel ) );
			}
			else if( m_Slayer != SlayerName.None || m_Slayer2 != SlayerName.None || m_DurabilityLevel != WeaponDurabilityLevel.Regular || m_DamageLevel != WeaponDamageLevel.Regular || m_AccuracyLevel != WeaponAccuracyLevel.Regular )
				attrs.Add( new EquipInfoAttribute( 1038000 ) ); // Unidentified

			if ( m_Poison != null && m_PoisonCharges > 0 )
				attrs.Add( new EquipInfoAttribute( 1017383, m_PoisonCharges ) );

			int number;

			if ( Name == null )
			{
				number = LabelNumber;
			}
			else
			{
				this.LabelTo( from, Name );
				number = 1041000;
			}

			if ( attrs.Count == 0 && Crafter == null && Name != null )
				return;

			EquipmentInfo eqInfo = new EquipmentInfo( number, m_Crafter, false, attrs.ToArray() );

			from.Send( new DisplayEquipmentInfo( this, eqInfo ) );
#endif		
        }

		private static BaseWeapon m_Fists; // This value holds the default--fist--weapon

		public static BaseWeapon Fists
		{
			get{ return m_Fists; }
			set{ m_Fists = value; }
		}

		#region ICraftable Members

		public virtual int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (WeaponQuality)quality;

			if ( makersMark )
				Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType ); // mod by Dies Irae
#if false
			if ( Core.AOS )
			{
				Resource = CraftResources.GetFromType( resourceType );

				CraftContext context = craftSystem.GetContext( from );

				if ( context != null && context.DoNotColor )
					Hue = 0;

				if ( tool is BaseRunicTool )
					((BaseRunicTool)tool).ApplyAttributesTo( this );

				if ( Quality == WeaponQuality.Exceptional )
				{
					if ( Attributes.WeaponDamage > 35 )
						Attributes.WeaponDamage -= 20;
					else
						Attributes.WeaponDamage = 15;

					if( Core.ML )
					{
						Attributes.WeaponDamage += (int)(from.Skills.ArmsLore.Value / 20);

						if ( Attributes.WeaponDamage > 50 )
							Attributes.WeaponDamage = 50;

						from.CheckSkill( SkillName.ArmsLore, 0, 100 );
					}
				}
			}
			else if ( tool is BaseRunicTool )
			{
				CraftResource thisResource = CraftResources.GetFromType( resourceType );

				if ( thisResource == ((BaseRunicTool)tool).Resource )
				{
					Resource = thisResource;

					CraftContext context = craftSystem.GetContext( from );

					if ( context != null && context.DoNotColor )
						Hue = 0;

					switch ( thisResource )
					{
						case CraftResource.DullCopper:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Durable;
							AccuracyLevel = WeaponAccuracyLevel.Accurate;
							break;
						}
						case CraftResource.ShadowIron:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Durable;
							DamageLevel = WeaponDamageLevel.Ruin;
							break;
						}
						case CraftResource.Copper:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Fortified;
							DamageLevel = WeaponDamageLevel.Ruin;
							AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
							break;
						}
						case CraftResource.Bronze:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Fortified;
							DamageLevel = WeaponDamageLevel.Might;
							AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
							break;
						}
						case CraftResource.Gold:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Force;
							AccuracyLevel = WeaponAccuracyLevel.Eminently;
							break;
						}
						case CraftResource.Agapite:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Power;
							AccuracyLevel = WeaponAccuracyLevel.Eminently;
							break;
						}
						case CraftResource.Verite:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Power;
							AccuracyLevel = WeaponAccuracyLevel.Exceedingly;
							break;
						}
						case CraftResource.Valorite:
						{
							Identified = true;
							DurabilityLevel = WeaponDurabilityLevel.Indestructible;
							DamageLevel = WeaponDamageLevel.Vanq;
							AccuracyLevel = WeaponAccuracyLevel.Supremely;
							break;
						}
					}
				}
			}
#endif

			return quality;
		}

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

		#endregion

        #region Mondain's Legacy
        private TalismanSlayerName m_Slayer3;

        private bool m_Immolating; // Is this weapon blessed via Immolating Weapon arcanists spell? Temporary; not serialized.

	    [CommandProperty( AccessLevel.GameMaster )]
        public bool Immolating
        {
            get { return m_Immolating; }
            set { m_Immolating = value; }
        }

		public override bool OnDragLift( Mobile from )
		{
			if ( Parent is Mobile && from == Parent )
			{			
				if ( IsSetItem && m_SetEquipped )
					SetHelper.RemoveSetBonus( from, SetID, this );
			}			
			
			return base.OnDragLift( from );
		}
		
		public virtual SetItem SetID{ get{ return SetItem.None; } }
		public virtual int Pieces{ get{ return 0; } }
				
		public bool IsSetItem{ get{ return SetID != SetItem.None; } }
		
		private int m_SetHue;
		private bool m_SetEquipped;
		private bool m_LastEquipped;
		
		#region modifica by Dies Irae
		private int m_UnSetHue;
		
		// [CommandProperty( AccessLevel.GameMaster )]
		public int UnSetHue
		{
			get { return m_UnSetHue; }
			set { m_UnSetHue = value; }
		}
		#endregion
		
		// [CommandProperty( AccessLevel.GameMaster )]
		public int SetHue
		{
			get{ return m_SetHue; }
			set{ m_SetHue = value; InvalidateProperties(); }
		}
		
		public bool SetEquipped
		{
			get{ return m_SetEquipped; }
			set{ m_SetEquipped = value; }
		}
		
		public bool LastEquipped
		{
			get{ return m_LastEquipped; }
			set{ m_LastEquipped = value; }
		}		
		
		private AosAttributes m_SetAttributes;
		private AosSkillBonuses m_SetSkillBonuses;
		private int m_SetSelfRepair;
		
		// [CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes SetAttributes
		{
			get{ return m_SetAttributes; }
			set{}
		}

		// [CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SetSkillBonuses
		{
			get{ return m_SetSkillBonuses; }
			set{}
		}	
		
		// [CommandProperty( AccessLevel.GameMaster )]
		public int SetSelfRepair
		{
			get{ return m_SetSelfRepair; }
			set{ m_SetSelfRepair = value; InvalidateProperties(); }
		}
		
		public virtual void GetSetProperties( ObjectPropertyList list )
		{					
			int prop;

			if ( (prop = m_SetSelfRepair) != 0 && WeaponAttributes.SelfRepair == 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~	

			SetHelper.GetSetProperties( list, this );
		}
		#endregion

	    #region [Third Crown]
	    public static readonly double CombatDelayMaxValue = 5.4;

	    /*
        private int DamageAdjust( int damage )
        {
            if( damage >= 70 )
                damage = 65 + DiceRoll.OneDiceFive.Roll();
            else if( damage >= 65 )
                damage = 60 + DiceRoll.OneDiceTen.Roll();
            else if( damage >= 60 )
                damage = 55 + DiceRoll.OneDiceTen.Roll();
            else if( damage >= 55 )
                damage = 50 + DiceRoll.OneDiceTen.Roll();
            else if( damage >= 50 )
                damage = 45 + DiceRoll.OneDiceTen.Roll();
            else if( damage >= 45 )
                damage = 40 + DiceRoll.OneDiceTen.Roll();
            else if( damage >= 40 )
                damage = 35 + DiceRoll.OneDiceTen.Roll();

            return damage;
        }
        */

	    /*
	    private double GetVirtualArmorScalar( Mobile defender )
        {
            int virtualArmor = defender.VirtualArmor + defender.VirtualArmorMod;
            double scalar;

            if( virtualArmor >= 60 )
                scalar = 2.0;
            else if( virtualArmor >= 40 )
                scalar = 3.0;
            else if( virtualArmor >= 30 )
                scalar = 4.0;
            else if( virtualArmor >= 20 )
                scalar = 4.5;
            else
                scalar = 5.0;

            return scalar;
        }
        */

	    private readonly BaseWeaponAttribute m_DisplayAttributes;

	    [CommandProperty( AccessLevel.GameMaster )]
	    public BaseWeaponAttribute DisplayAttributes
	    {
	        get { return m_DisplayAttributes; }
	        set { }
	    }

	    #region debug
	    // [CommandProperty( AccessLevel.GameMaster )]
	    public bool Debug { get; set; }

	    private void SendDebugMessage( Mobile to, string format, params object[] args )
	    {
	        if( !Debug )
	            return;

	        if( to != null )
	            to.DebugMessage( format, args );
	    }

	    private void SendDebugOverheadMessage( Mobile to, string format, params object[] args )
	    {
	        if( !Debug )
	            return;

	        if( to != null )
	            to.DebugOverheadMessage( format, args );
	    }
	    #endregion

	    #region special moves
	    public static bool SpearsSpecialMoveEnabled = false;
	    public static bool PoleArmsSpecialMoveEnabled = false;
	    public static bool BashingSpecialMoveEnabled = false;
	    public static bool AxesSpecialMoveEnabled = false;
	    public static bool FistsSpecialMoveEnabled = false;
	    #endregion

	    /// <summary>
	    /// This property is true if this weapon check the attacker main skill on swing event.
	    /// </summary>
	    public virtual bool CheckForAttackSkillOnSwing
	    {
	        get { return true; }
	    }

	    #region classes
	    [CommandProperty( AccessLevel.GameMaster )]
	    public bool IsXmlHolyWeapon
	    {
	        get { return XmlAttach.FindAttachment( this, typeof(XmlHolyItemAttach) ) != null; }
	    }

        public override DeathMoveResult OnParentDeath( Mobile parent )
        {
            XmlHolyItemAttach att = XmlAttach.FindAttachment( this, typeof( XmlHolyItemAttach ) ) as XmlHolyItemAttach;
            if( att != null && !att.Deleted )
                att.Delete();

            DeathMoveResult result;
            if( this is ITreasureOfMidgard && XmlBlessedCursedAttach.CheckOnParentDeath( parent, this, out result ) )
                return result;

            return base.OnParentDeath( parent );
        }
	    #endregion

	    #region criticals
	    public static double PercentualCriticalChance = 0.07; // 10%
        public static double DamageCriticalScalar = 1.30; // 130%

        private bool CheckCriticalHit( Mobile attacker )
        {
	        if( !attacker.Player || this is Fists )
	            return false;

	        double atckValue = attacker.Skills[ Skill ].Value;
	        double tactValue = attacker.Skills[ SkillName.Tactics ].Value;
	        double anatValue = attacker.Skills[ SkillName.Anatomy ].Value;

            double chance = ( ( atckValue + tactValue + anatValue ) / 300.0 ) * PercentualCriticalChance;

            if( attacker is BaseGuard || attacker is BaseTownGuard )
                chance = 0.5;

            return Utility.RandomDouble() < chance;
        }

	    private static void DoCriticalEffects( Mobile attacker, Mobile defender )
	    {
	        attacker.PlaySound( attacker.Female ? 824 : 1098 );
	        attacker.SendMessage( attacker.Language == "ITA" ? "Hai inferto un colpo critico!" : "You delivered a critical hit!" );

	        defender.FixedParticles( 0, 1, 0, 9946, EffectLayer.Head );
	        if( defender.Player )
	            defender.SendMessage( defender.Language == "ITA" ? "Subisci danno aggiuntivo per via del colpo devastante!" : "You take extra damage from that devasting attack!" );
	    }

	    private double ComputeCriticalBonus( Mobile attacker )
	    {
	        if( attacker.Player )
	            return m_CriticalHit ? DamageCriticalScalar : 0.0;
	        else
	            return 0.0;
	    }

        private bool m_CriticalHit = false;
	    #endregion

	    public virtual string OldInitHits
	    {
	        get { return "1d0+0"; }
	    }

	    #region requirements (class, town...)
	    public virtual TownSystem RequiredTownSystem
	    {
	        get { return null; }
	    }

	    public virtual Classes RequiredClass
	    {
	        get { return Classes.None; }
	    }
	    #endregion

	    #region hits, sounds, animations etc.
	    private void SoundAndAnimationEffects( Mobile attacker, Mobile defender )
	    {
	        PlaySwingAnimation( attacker );
	        PlayHurtAnimation( defender );

	        attacker.PlaySound( GetHitAttackSound( attacker, defender ) );
	        defender.PlaySound( GetHitDefendSound( attacker, defender ) );
	    }
	    #endregion

	    #region ruin
	    protected double ChanceOfRuin
	    {
	        get
	        {
	            var parent = Parent as Mobile;
	            if( parent == null )
	                return 0.0;

	            double scalar = 0.15 - ( (int) parent.Skills[ SkillName.ArmsLore ].Value ) / 2000.0;
	            return ( scalar / ( 2 * CustomQuality * OldMaterialStaticMultiply ) );
	        }
	    }

	    private void Ruin( Mobile attacker, Mobile defender, int damage )
	    {
	        bool safeFromRuin = !attacker.Player;

	        if( !safeFromRuin && m_MaxHits > 0 && ( ( MaxRange <= 1 && ( defender is Slime || defender is ToxicElemental ) ) || /* Utility.Random( 100 ) == 0 */ ChanceOfRuin > Utility.RandomDouble() ) )
	        {
	            if( MaxRange <= 1 && ( defender is Slime || defender is ToxicElemental ) )
	                attacker.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500263 ); // *Acid blood scars your weapon!*

	            if( m_Hits > 0 )
	                --HitPoints;
	            else if( m_MaxHits > 1 )
	            {
	                --MaxHitPoints;

	                if( Parent is Mobile )
	                    ( (Mobile) Parent ).LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
	            }
	            else
	            {
	                if( Parent is Mobile )
	                {
	                    Mobile parent = Parent as Mobile;
	                    parent.SendMessage( parent.Language == "ITA" ? "Il tuo equipaggiamento è vecchio e logoro! Si è distrutto!" : "Your equipment is too old and ruined! It is broken!" );
	                    parent.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "*Houch!!*" );
	                }

	                Delete();
	            }
	        }
	    }
	    #endregion

	    #region damage computation
	    public virtual int NumDice
	    {
	        get { return 0; }
	    }

	    public virtual int NumSides
	    {
	        get { return 0; }
	    }

	    public virtual int DiceBonus
	    {
	        get { return 0; }
	    }

	    public string Dice
	    {
	        get { return String.Concat( NumDice, "d", NumSides, "+", DiceBonus ); }
	    }

        public virtual int OldMinDamage
        {
            get
            {
                if( NumDice > 0 )
                    return NumDice + DiceBonus;
                else
                    return 0;
            }
        }

        public virtual int OldMaxDamage
        {
            get
            {
                if( NumDice > 0 )
                    return NumDice*NumSides + DiceBonus;
                else
                    return 0;
            }
        }


        public int MinDiceDamage
        {
            get { return ( NumDice * 1 ) + DiceBonus; }
        }

        public int MaxDiceDamage
        {
            get { return ( NumDice * NumSides ) + DiceBonus; }
        }

		public static bool OldPolRules = true;
	    public static double DamageScalarVsPlayerSummons = 30.0;
	    public static double DamageScalarVsNpcSummons = 15.0;
	    public static double DamageScalarPlayerVsMonsters = 1.5;
	    public static double CreatureVsRangedPlayerDamageScalar = 1.20;
	    public static double CreatureVsMeleePlayerDamageScalar = 0.7;
	    public static double CreatureVsPlayerDamageScalar = 1.10;
	    public static double DamageScalarCreaturesVsPlayerSummons = 3.0;
	    public static double WorkerSkillDamageBonusScalar = 1.5;

	    public virtual void AlterDamageByDistance( Mobile attacker, Mobile defender, ref int percentageBonus )
	    {
	    }

	    private void AlterDamageBySlayer( Mobile attacker, Mobile defender, ref int percentageBonus )
	    {
	        CheckSlayerResult cs = CheckSlayers( attacker, defender );
	        if( cs != CheckSlayerResult.None )
	        {
	            if( cs == CheckSlayerResult.Slayer )
	                defender.FixedEffect( 0x37B9, 10, 5 );
	            percentageBonus += 100;
	        }
	    }

	    private void AlterDamageByPackInstinct( Mobile attacker, Mobile defender, ref int percentageBonus )
	    {
	        int packInstinctBonus = GetPackInstinctBonus( attacker, defender );
	        if( packInstinctBonus != 0 )
	            percentageBonus += packInstinctBonus;
	    }

	    private bool IsRanged
	    {
	        get { return Type == WeaponType.Ranged; }
	    }

	    public virtual bool ElegibleForLumberBonus
	    {
	        get { return Type == WeaponType.Axe; }
	    }

	    public virtual bool ElegibleForSwordsBonus
	    {
	        get { return Type == WeaponType.Slashing; }
	    }

	    public virtual bool ElegibleForMiningBonus
	    {
	        get { return ( this is BaseAxe && ( (BaseAxe) this ).HarvestSystem == Mining.System && Layer == Layer.TwoHanded ); }
	    }

	    public virtual bool ElegibleForBowcraftBonus
	    {
	        get { return IsRanged; }
	    }

	    public virtual double ComputePvpBonus( Mobile attacker, Mobile defender )
	    {
	        return 0;
	    }

	    private double ComputeTacticsBonus( Mobile attacker )
	    {
	        /* Compute tactics modifier
		* :   0.0 = 50% loss
		* :  50.0 = unchanged
		* : 100.0 = 50% bonus (45% for ranged weapons)
		*
		* OldPolRules:
		*	if (weaponelem.skillid="archery") // and NewGetSkill(attacker,cint(SKILLID_BOWCRAFT))>=30)
		*	tacticsper:=cdbl(35 + NewGetSkill(attacker,cint(SKILLID_TACTICS)))/100;
		*	tacticsper:=tacticsper+(cdbl(NewGetSkill(attacker,cint(SKILLID_BOWCRAFT)))/2000);// (100=5%)
		*	else
		*	tacticsper:=cdbl(40 + NewGetSkill(attacker,cint(SKILLID_TACTICS)))/100;
		*/
		
	        // double tacticsBonus = (attacker.Skills[SkillName.Tactics].Value - 50.0) / 100.0;
		//bool OldPolRules = attacker.PlayerDebug;
		double OldPolTactics = attacker.Skills[ SkillName.Tactics ].Value/100.0 + (IsRanged ? attacker.Skills[ SkillName.Fletching ].Value/2000.0 + 0.35 : 0.4);

		if ( OldPolRules && this.Layer == Layer.OneHanded )
			OldPolTactics = attacker.Skills[ SkillName.Tactics ].Value/200.0;//per avere il danno base dell'arma, calcolo il bonus nel tiro del dado.

	        return OldPolRules ? OldPolTactics : ( attacker.Skills[ SkillName.Tactics ].Value - ( 50.0 + ( IsRanged ? 5.0 : 0.0 ) ) ) / 100.0;
	    }

	    private static double ComputeStrengthBonus( Mobile attacker )
	    {
	        /* Compute strength modifier
		* : 1% bonus for every 5 strength
		*
		* OldPolRules
		*	var strper:=cdbl(attacker.strength)/500;
		*/
		//bool OldPolRules = attacker.PlayerDebug;

	        return OldPolRules ? (attacker.Str/500.0) : ( attacker.Str / 5.0 ) / 100.0;
	    }

	    private static double ComputeAnatomyBonus( Mobile attacker )
	    {
		/* Compute anatomy modifier
		* : 1% bonus for every 5 points of anatomy
		* : +10% bonus at Grandmaster or higher
		*
		*OldPolRules
		*	var anatomyper:=cdbl(NewGetSkill(attacker,cint(SKILLID_ANATOMY)))/500;
		*/

		//bool OldPolRules = attacker.PlayerDebug;
	        double anatomyValue = attacker.Skills[ SkillName.Anatomy ].Value;
	        double anatomyBonus = ( anatomyValue / 5.0 ) / 100.0;

	        if( anatomyValue >= 100.0 )
	            anatomyBonus += 0.1;

	        return OldPolRules ? (anatomyValue / 500.0) : anatomyBonus;
	    }

	    private static double ComputeLumberBonus( Mobile attacker )
	    {
		/* Compute lumberjacking bonus
		* : 1% bonus for every 5 points of lumberjacking
		* : +10% bonus at Grandmaster or higher
		*
		*OldPolRules
		*	lumberper:=lumberper+(cdbl(NewGetSkill(attacker,cint(SKILLID_LUMBERJACKING)))/500);// (100=20%)
		*/

		//bool OldPolRules = attacker.PlayerDebug;
	        double lumberValue = attacker.Skills[ SkillName.Lumberjacking ].Value;

	        double lumberBonus = ( lumberValue / 5.0 ) / 100.0;

	        if( lumberValue >= 100.0 )
	            lumberBonus += 0.1;

	        return OldPolRules ? (lumberValue / 500.0) : lumberBonus * WorkerSkillDamageBonusScalar;
	    }

	    private static double ComputeSwordsBonus( Mobile attacker )
	    {
	        /* Compute swordmanship bonus
		* if this weapon not use lumber
		* but only swordmanship (min 30)
		* 1% bonus for every 10 points of swordmanship
		* 10% bonus at Grandmaster or higher
		*/
		//bool OldPolRules = attacker.PlayerDebug;
	        double swordsBonus = 0.0;

	        double swordsValue = attacker.Skills[ SkillName.Swords ].Value;
	        if( swordsValue >= 30.0 )
	        {
	            swordsBonus = ( swordsValue / 10.0 ) / 100.0;

	            if( swordsValue >= 100.0 )
	                swordsBonus += 0.1;
	        }

	        return OldPolRules ? (swordsValue/500.0) : swordsBonus;
	    }

	    private static double ComputeMiningBonus( Mobile attacker )
	    {
	        /* Compute mining bonus
		* if this weapon use mining (min 30) and is twohanded
		* 1% bonus for every 5 points of mining
		* 10% bonus at Grandmaster or higher
		*
		*OldPolRules
		*	miningper:=miningper+(cdbl(NewGetSkill(attacker,cint(SKILLID_MINING)))/500);// (100=20%)
		*/

		//bool OldPolRules = attacker.PlayerDebug;
	        double miningBonus = 0.0;

	        double miningValue = attacker.Skills[ SkillName.Mining ].Value;
	        if( miningValue >= 30.0 )
	        {
	            miningBonus = ( miningValue / 5.0 ) / 100.0;

	            if( miningValue >= 100.0 )
	                miningBonus += 0.1;
	        }

	        return OldPolRules ? (miningValue/500.0) : miningBonus * WorkerSkillDamageBonusScalar;
	    }

	    private static double ComputeBowcraftBonus( Mobile attacker )
	    {
	        /* Compute bowcraft bonus
		* if this weapon use archery (min 30)
		* 1% bonus for every 10 points of bowcraft
		* 10% bonus at Grandmaster or higher
		*
		*OldPolRules
		*	bowcraftper:=bowcraftper+(cdbl(NewGetSkill(attacker,cint(SKILLID_BOWCRAFT)))/1000);// (100=10%)
		*/

		//bool OldPolRules = attacker.PlayerDebug;
	        double bowcraftBonus = 0.0;

	        double bowcraftValue = attacker.Skills[ SkillName.Fletching ].Value;
	        if( bowcraftValue >= 30.0 )
	        {
	            bowcraftBonus = ( bowcraftValue / 10.0 ) / 100.0;

	            if( bowcraftValue >= 100.0 )
	                bowcraftBonus += 0.1;
	        }

	        return OldPolRules ? (bowcraftValue/1000.0): bowcraftBonus * WorkerSkillDamageBonusScalar;
	    }

	    private double ComputeQualityBonus( Mobile attacker )
	    {
	        return attacker.Player ? OldScaleDamageByQuality() : 0.0;
	    }

	    private int ComputeMagicalDamageOffset()
	    {
	        /* Apply damage level offset
             * : Regular : 0
             * : Ruin    : 1
             * : Might   : 3
             * : Force   : 5
             * : Power   : 7
             * : Vanq    : 9
             */
	        if( m_DamageLevel != WeaponDamageLevel.Regular )
	            return (int) ( ( 2.0 * (int) m_DamageLevel ) - 1.0 );
	        else
	            return 0;
	    }

        private double ScaleDamageSecondAge( Mobile attacker, Mobile defender, double damage, bool checkSkills )
        {
            //attacker.SendMessage( "Scale damage attualmente attivo!" );
            //SendDebugMessage( attacker, "Base damage: {0}", GetBaseDamage( attacker ) );
            if( attacker.PlayerDebug )
                attacker.SendMessage( "Base damage: {0}", damage );

		    //bool OldPolRules = attacker.PlayerDebug;

            if( checkSkills )
            {
                attacker.CheckSkill( SkillName.Tactics, 0.0, attacker.Skills[ SkillName.Tactics ].Cap + 20 ); // Passively check tactics for gain
                attacker.CheckSkill( SkillName.Anatomy, 0.0, attacker.Skills[ SkillName.Anatomy ].Cap + 20 ); // Passively check Anatomy for gain
            }

            double bonus = 0.0;

            bonus += ComputeTacticsBonus( attacker );
            bonus += ComputeStrengthBonus( attacker );
            bonus += ComputeAnatomyBonus( attacker );
            bonus += ComputeQualityBonus( attacker );

            if( !OldPolRules )
                bonus += ComputeCriticalBonus( attacker );

            if( ElegibleForLumberBonus )
                bonus += ComputeLumberBonus( attacker );

            if( ElegibleForSwordsBonus )
                bonus += ComputeSwordsBonus( attacker );

            if( ElegibleForMiningBonus )
                bonus += ComputeMiningBonus( attacker );

            if( ElegibleForBowcraftBonus )
                bonus += ComputeBowcraftBonus( attacker );

            //if (OldPolRules)
            //    bonus += ComputeBlacksmithyBonus( attacker );//20% a 100

            //SendDebugMessage( attacker,
            //                  string.Format( "damage: {0:F2} tacticsBonus: {1:F2} strBonus: {2:F2} anatomyBonus: {3:F2} lumberBonus: {4:F2} qualityBonus: {5:F2}",
            //                                damage,
            //                                 ComputeTacticsBonus( attacker ),ComputeStrengthBonus( attacker ),ComputeAnatomyBonus( attacker ),ComputeLumberBonus( attacker ),ComputeQualityBonus( attacker ) ) );
            //
            //SendDebugMessage( attacker, string.Format( "damage: {0:F2} swordsBonus: {1:F2} bowcraftBonus: {2:F2} miningBonus: {3:F2}", damage, ComputeSwordsBonus( attacker ), ComputeBowcraftBonus( attacker ), ComputeMiningBonus( attacker ) ) );
            if( OldPolRules )
                damage = ( damage * ( bonus ) ) + ( ( damage * VirtualDamageBonus ) / 100 );
            else
                damage = ( damage * ( 1.0 + bonus ) ) + ( ( damage * VirtualDamageBonus ) / 100 );

            // damage += GetQualityDamageOffset( attacker );
            // SendDebugMessage( attacker, string.Format( "damage after quality: {0}", damage ) );

            damage += ComputeMagicalDamageOffset();

            //SendDebugMessage( attacker, string.Format( "damage after WeaponDamageLevel: {0}", damage ) );

            damage += OldMaterialDamageBonus;

            //SendDebugMessage( attacker, string.Format( "damage after OldMaterialDamageBonus: {0}", damage ) );

            if( TransformationSpellHelper.UnderTransformation( attacker, typeof( EvilAvatarSpell ) ) )
                damage *= 1.50;

            if( ( attacker is BaseGuard || attacker is BaseTownGuard ) && defender != null )
                damage *= defender.Player ? 2.0 : 10.0;

	        if( attacker is BaseCreature && ( (BaseCreature) attacker ).GetMaster() == null && defender.Player )
            {
                if( defender.Weapon != null && defender.Weapon is BaseWeapon && ( (BaseWeapon)( defender.Weapon ) ).IsRanged )
                    damage *= CreatureVsRangedPlayerDamageScalar;
                else
                    damage *= CreatureVsMeleePlayerDamageScalar;
            }

		//if ( attacker is BaseCreature && defender != null && defender is BaseCreature && ((BaseCreature)attacker).IsPlayerSummoned )
		//{
		//	attacker.Say("double");
		//	damage *= 2;
		//}
            if( attacker.Player )
                damage = ScaleDamageByDurability( (int)damage );

            //if (attacker.PlayerDebug)
            //	attacker.SendMessage("Base damage after scale: {0}", damage);
            //SendDebugMessage( attacker, "damage after ScaleDamageOld: {0}", damage );

            return damage;
        }
        #endregion

        #region hit chance computation
        private static readonly double[] m_AccuracyScalars = {
	                                                             0.0, 4.6, 9.3, 14.0, 18.7, 23.3
	                                                         };

        [CommandProperty( AccessLevel.Developer )]
        public double AccuracyBonus
        {
            get
            {
                if( AccuracySkill == SkillName.Archery && m_AccuracyLevel >= 0 && (int)m_AccuracyLevel < m_AccuracyScalars.Length )
                    return ( m_AccuracyScalars[ (int)m_AccuracyLevel ] );

                return (int)m_AccuracyLevel * 5;
            }
        }

        /*I GM possono modificare questi valori per una fase di test, quando viene deciso se il valore testato è accettabile allora si potranno cambiare le 
         * classi sottostanti che dovranno cmq e sempre usare SetHitRateBonus e SetEvasionBonus invece della normale assegnazione 
         * 
         * Nota: le modifiche alle armi saranno cmq resettate al prossimo caricamento in modo che tutto torni alla normalità e che 
         * eventuali modifiche siano RETROATTIVE in questo modo le armi non serializzano questi 2 valori che invece restano attivi solo in fase
         * di esecuzione ma non vengono salvate :-)
         */

        protected virtual void SetupHitRateAndEvasionBonus( out double hitrate, out double evasion )
        {
            hitrate = 0.0;
            evasion = 0.0;
        }

        private double m_HitRateBonus;

        [CommandProperty( AccessLevel.GameMaster )]
        public double HitRateBonus
        {
            get { return m_HitRateBonus; }
            set
            {
                SetHitRateBonus( value );
                PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "HitRateBonus is now: {0}. This value will preserved until nel startup.", value ) );
            }
        }

        private double m_EvasionBonus;

        [CommandProperty( AccessLevel.GameMaster )]
        public double EvasionBonus
        {
            get { return m_EvasionBonus; }
            set
            {
                SetEvasionBonus( value );
                PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "EvasionBonus is now: {0}. This value will preserved until nel startup.", value ) );
            }
        }

        private void SetHitRateBonus( double newvalue )
        {
            m_HitRateBonus = newvalue;
        }

        private void SetEvasionBonus( double newvalue )
        {
            m_EvasionBonus = newvalue;
        }

        private void OnHitOld( Mobile attacker, Mobile defender, double damageBonus )
        {
            SoundAndAnimationEffects( attacker, defender );

            bool isOneEnemyActive =  HolySmiteSpell.IsWaitingPaladine( attacker ) && RPGPaladinSpell.IsEnemy( attacker, defender );

            m_CriticalHit = CheckCriticalHit( attacker ) || isOneEnemyActive;

            int damage = ComputeDamage( attacker, defender );

            if( attacker.PlayerDebug )
                attacker.SendMessage( "Base damage after scale: {0}", damage );
            else if( attacker is BaseCreature && defender.PlayerDebug )
                defender.SendMessage( "ComputeDamage for enemy (base): {0}", damage );

            if( IsXmlHolyWeapon )
                RPGPaladinSpell.AlterDamage( this, attacker, defender, ref damage );

            int percentageBonus = (int)( damageBonus * 100 ) - 100;

            AlterDamageBySlayer( attacker, defender, ref percentageBonus );
            AlterDamageByPackInstinct( attacker, defender, ref percentageBonus );

            TransformContext context = TransformationSpellHelper.GetContext( defender );
            if( ( m_Slayer == Server.Items.SlayerName.Silver || m_Slayer2 == Server.Items.SlayerName.Silver ) && context != null && context.Spell is RPGNecromancerSpell && context.Type != typeof( EvilAvatarSpell ) )
                percentageBonus += 50;

            AlterDamageByDistance( attacker, defender, ref percentageBonus );

            damage = AOS.Scale( damage, 100 + percentageBonus );

            if( attacker is BaseCreature )
                ( (BaseCreature)attacker ).AlterMeleeDamageTo( defender, ref damage );

            if( defender is BaseCreature )
                ( (BaseCreature)defender ).AlterMeleeDamageFrom( attacker, ref damage );

            if( attacker.Player && defender is BaseNecroFamiliar )
            {
                damage = (int)( damage * DamageScalarPlayerVsMonsters * 3 );
            }
            else if( attacker.Player && defender is BaseCreature )
            {
                if( ( (BaseCreature)defender ).IsPlayerSummoned )
                    damage = (int)( damage * DamageScalarVsPlayerSummons );
                else if( ( (BaseCreature)defender ).Summoned )
                    damage = (int)( damage * DamageScalarVsNpcSummons );
                if( !( this is BaseRanged ) )
                {
                    int preScalar = damage;

                    damage = (int)( damage * DamageScalarPlayerVsMonsters );
                    if( attacker.PlayerDebug && DamageScalarPlayerVsMonsters != 1 )
                        attacker.SendMessage( "Damage vs NPC + {0} points.", damage - preScalar );
                }
            }

            if( attacker is BaseCreature && !( (BaseCreature)attacker ).IsPlayerSummoned && defender is BaseCreature && ( (BaseCreature)defender ).IsPlayerSummoned )
                damage = (int)( damage * DamageScalarCreaturesVsPlayerSummons );

            if( attacker is BaseCreature && ( (BaseCreature)attacker ).Controlled && defender is BaseCreature )
                damage = (int)( damage * DamageScalarPlayerVsMonsters );
            if( attacker is BaseCreature && ( (BaseCreature)attacker ).Controlled && defender is BaseCreature && ( (BaseCreature)defender ).Summoned )
                damage = (int)( damage * DamageScalarVsNpcSummons );

		//if ( attacker is BaseCreature && defender != null && defender is BaseCreature && ((BaseCreature)attacker).IsPlayerSummoned )
		//{
		//	damage *= 3;
		//	attacker.Say("triple, {0}", damage);
		//}

            if( attacker.PlayerDebug )
                attacker.SendMessage( "Before AbsorbDamage: {0}", damage );
            else if( attacker is BaseCreature && defender.PlayerDebug )
                defender.SendMessage( "Before AbsorbDamage for enemy: {0}", damage );

            #region Mob by Magius(CHE) - Midgard, tutti i tipi di danno sono sempre e solo fisici
            damage = AbsorbDamage( attacker, defender, damage );
            #endregion

            if( attacker.Player )
                SendDebugMessage( attacker, "After AbsorbDamage: {0}", damage );
            else if( attacker is BaseCreature && defender.Player )
                SendDebugMessage( defender, "After AbsorbDamage for enemy: {0}", damage );

            ImmolatingWeaponSpell.AlterDamage( this, attacker, defender, ref damage );

            AddBlood( attacker, defender, damage );

            /*
            bool isOneEnemyActive = HolySmiteSpell.IsWaitingPaladine( attacker ) && RPGPaladinSpell.IsEnemy( attacker, defender );
            if( isOneEnemyActive )
                damage += HolySmiteSpell.GetBonusDamage( attacker, defender );
            */
            int damageGiven = OldPolRules ? Math.Max( damage, 1 ) : Math.Max( damage / 2, 1 ); // Per Stratics, damage is halved after armor absorb is calculated

            if( !attacker.Player || !defender.Player )//edit by arlas [Modifica dovuta a dimensioni Body]
            {
                int mountvalue = attacker.Mounted ? -1 : 0;
                mountvalue += defender.Mounted ? 1 : 0;

                //attacker.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( "{0} + {1}", damage, -(mountvalue + defender.Body.Dimension - attacker.Body.Dimension) ) );
                damage -= ( mountvalue + defender.Body.Dimension - attacker.Body.Dimension );
                //attacker.SendMessage( "Bonus Danno: {0};", -(mountvalue + defender.Body.Dimension - attacker.Body.Dimension));
            }

            if( attacker.Player )
                SendDebugMessage( attacker, "Final Damage: {0}", damageGiven );

            Midgard.Engines.PVMAbsorbtions.Core.PreventAbsorbtionFrom( attacker, defender, true ); //Mod by MagiusCHE: PVMTanbk absorbtion is already calculated using AbsorbDamage
            damageGiven = AOS.Damage( defender, attacker, damageGiven, false, 100, 0, 0, 0, 0, 0, 0, false, this is BaseRanged );
            //defender.Damage( damageGiven, attacker );
            Midgard.Engines.PVMAbsorbtions.Core.PreventAbsorbtionFrom( attacker, defender, false ); //Mod by MagiusCHE: PVMTanbk absorbtion is already calculated using AbsorbDamage

            if( attacker.PlayerDebug )
                attacker.SendMessage( "Final Damage: {0}; Enemy Hits: {1}; Enemy Hitsmax: {2};", damageGiven, defender.Hits, defender.HitsMax );

            if( isOneEnemyActive )
                HolySmiteSpell.DoEffect( attacker, defender );

            if( m_CriticalHit )
                DoCriticalEffects( attacker, defender );

            Ruin( attacker, defender, damage );

            if( attacker is VampireBatFamiliar )
            {
                BaseCreature bc = (BaseCreature)attacker;
                Mobile caster = bc.ControlMaster;

                if( caster == null )
                    caster = bc.SummonMaster;

                if( caster != null && caster.Map == bc.Map && caster.InRange( bc, 2 ) )
                    caster.Hits += damage;
                else
                    bc.Hits += damage;
            }

            #region sound effects
            if( defender is PlayerMobile && damage <= defender.Hits )
            {
                if( damage > 25 )
                {
                    if( defender.Female )
                        defender.PlaySound( 814 );
                    else
                        defender.PlaySound( 1088 );
                }
                else
                {
                    if( defender.Female )
                        defender.PlaySound( 806 );
                    else
                        defender.PlaySound( 1078 );
                }
            }
            #endregion

            if( attacker.Player && attacker.CanBeginAction( this ) )
            {
                if( !attacker.CanBeginAction( typeof( BaseWeapon ) ) )
                {
                    if( Parent == attacker && MagicalCharges > 0 )
                        DoPreAosMagicalEffect( attacker, defender );
                }
            }

            #region rebirth code
            /*
            if( SpellEffectOnHit && defender.Alive && !defender.Deleted && m_Effect != SpellEffect.None && m_EffectCharges > 0 )
            {
                if( SpellCastEffect.InvokeEffect( m_Effect, attacker, defender ) )
                {
                    SpellCharges--;
                    if( SpellCharges <= 0 )
                    {
                        m_Effect = SpellEffect.None;
                        attacker.SendAsciiMessage( "This magic item is out of charges." );
                    }
                }
            }
            */
            #endregion

            if( attacker is BaseCreature )
                ( (BaseCreature)attacker ).OnGaveMeleeAttack( defender );

            if( defender is BaseCreature )
                ( (BaseCreature)defender ).OnGotMeleeAttack( attacker );

            XmlAttach.OnWeaponHit( this, attacker, defender, damageGiven );

            if( StaminaLossOnHit )
                defender.Stam -= Utility.Dice( 1, 3, 2 ); // 1d3+2 points of stamina loss
        }

        public virtual bool SpellEffectOnHit { get { return true; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual bool StaminaLossOnHit { get { return false; } }
        #endregion

        #region [guilds]
        [CommandProperty( AccessLevel.Administrator )]
        public bool IsGuildItem { get; set; }

        private BaseGuild m_OwnerGuild;

        [CommandProperty( AccessLevel.Administrator )]
        public BaseGuild OwnerGuild
        {
            get { return m_OwnerGuild; }
            set
            {
                m_OwnerGuild = value;

                LootType = ( m_OwnerGuild == null ? LootType.Regular : LootType.Blessed );
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public Mobile GuildedOwner { get; set; }

        /*
        private void VerifyGuildItem_Callback()
        {
            if( IsGuildItem && OwnerGuild != null && GuildedOwner == null && OwnerGuild is Guild )
            {
                GuildsHelper.StringToLog( string.Format( "Warning: Guilditem for {0} with serial {1} without owner... deleting.", OwnerGuild.Name, Serial ) );
                GuildsHelper.UnRegisterUniform( null, (Guild)OwnerGuild, this );
                Delete();
            }
        }
        */
        #endregion

	    #region [stone enchant system]
	    [CommandProperty( AccessLevel.GameMaster )]
	    public bool IsStoneEnchanted
	    {
	        get { return StoneEnchantHelper.IsEnchanted( this ); }
	    }

	    private StoneEnchantItem m_StoneEnchantItemState;

	    [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
	    public StoneEnchantItem StoneEnchantItemState
	    {
	        get { return m_StoneEnchantItemState; }
	        set
	        {
	            m_StoneEnchantItemState = value;
	            InvalidateSecondAgeNames();
	        }
	    }

	    public SkillName[] SkillsRequiredToEnchant { get; private set; }
	    #endregion

	    #region magicals

	    #region rebirth code
        /*
        private SpellEffect m_Effect;
        private int m_EffectCharges;

        [CommandProperty( AccessLevel.GameMaster )]
        public SpellEffect SpellEffect
        {
            get { return m_Effect; }
            set { m_Effect = value; SingleClickChanged(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int SpellCharges
        {
            get { return m_EffectCharges; }
            set { m_EffectCharges = value; SingleClickChanged(); }
        }
         
        public bool IsMagic
		{
			get
			{
				return m_Slayer != SlayerName.None || m_DurabilityLevel != DurabilityLevel.Regular || m_DamageLevel != WeaponDamageLevel.Regular || m_AccuracyLevel != WeaponAccuracyLevel.Regular || ( m_Effect != SpellEffect.None && m_EffectCharges > 0 );
			}
		}

  		public void OnIdentify( Mobile from )
		{
			if ( IsMagic && from.AccessLevel == AccessLevel.Player )
				m_Identified.Add( from );
		}       
        */
        #endregion

        private WeaponMagicalAttribute m_MagicalAttribute;

	    [CommandProperty( AccessLevel.GameMaster )]
	    public WeaponMagicalAttribute MagicalAttribute
	    {
	        get { return m_MagicalAttribute; }
	        set
	        {
	            m_MagicalAttribute = value;
	            InvalidateSecondAgeNames();
	        }
	    }

	    private int m_MagicalCharges;

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int MagicalCharges
	    {
	        get { return m_MagicalCharges; }
	        set
	        {
	            m_MagicalCharges = value;
	            InvalidateSecondAgeNames();
	        }
	    }

	    private const int ChanceOfPreAosEffect = 35;

	    private void DoPreAosMagicalEffect( Mobile attacker, Mobile defender )
	    {
	        if( Utility.Random( 100 ) < ChanceOfPreAosEffect )
	        {
	            if( MagicalAttribute == WeaponMagicalAttribute.Clumsiness )
	                DoClumsy( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.Feeblemindedness )
	                DoFeeblemind( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.Weakness )
	                DoWeakness( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.Burning )
	                DoBurn( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.Wounding )
	                DoWound( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.DaemonBreath )
	                DoDaemonBreath( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.Thunder )
	                DoThunder( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.MagesBane )
	                DoMagesBane( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.GhoulTouch )
	                DoGhoulTouch( attacker, defender );
	            else if( MagicalAttribute == WeaponMagicalAttribute.Evil )
	                DoEvil( attacker, defender );
	        }
	    }

	    private void ConsumeMagicalCharge( Mobile from )
	    {
	        --MagicalCharges;

	        if( MagicalCharges <= 0 )
	        {
	            from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
	            MagicalAttribute = WeaponMagicalAttribute.None;
	        }

	        ApplyDelayTo( from );
	    }

	    protected virtual void ApplyDelayTo( Mobile from )
	    {
	        from.BeginAction( this );
	        Timer.DelayCall( UseDelay, new TimerStateCallback( ReleaseWeaponLock_Callback ), from );
	    }

	    private void ReleaseWeaponLock_Callback( object state )
	    {
	        ( (Mobile) state ).EndAction( this );
	    }

	    protected virtual TimeSpan UseDelay
	    {
	        get { return TimeSpan.FromSeconds( 4.0 ); }
	    }

	    #region [t2a effects]
	    public bool HandleSelfMagicalAbsorption( Mobile attacker, Mobile defender )
	    {
	        if( attacker == null || defender == null )
	            return false;

	        if( attacker == defender && attacker.MagicDamageAbsorb > 0 )
	        {
	            attacker.MagicDamageAbsorb = 0;
	            attacker.SendMessage( attacker.Language == "ITA" ? "L'incantesimo di riflessione ti ha salvato!" : "Your magic reflect spell saved you!" );
	            return true;
	        }

	        return false;
	    }

	    private void DoClumsy( Mobile attacker, Mobile defender )
	    {
	        SpellHelper.CheckReflect( 1, attacker, ref defender );

	        if( HandleSelfMagicalAbsorption( attacker, defender ) )
	            return;

	        SpellHelper.AddStatCurse( attacker, defender, StatType.Dex );

	        if( defender.Spell != null )
	            defender.Spell.OnCasterHurt();

	        defender.Paralyzed = false;

	        defender.FixedParticles( 0x3779, 10, 15, 5002, EffectLayer.Head );
	        defender.PlaySound( 0x1DF );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoFeeblemind( Mobile attacker, Mobile defender )
	    {
	        SpellHelper.CheckReflect( 1, attacker, ref defender );

	        if( HandleSelfMagicalAbsorption( attacker, defender ) )
	            return;

	        SpellHelper.AddStatCurse( attacker, defender, StatType.Int );

	        if( defender.Spell != null )
	            defender.Spell.OnCasterHurt();

	        defender.Paralyzed = false;

	        defender.FixedParticles( 0x3779, 10, 15, 5004, EffectLayer.Head );
	        defender.PlaySound( 0x1E4 );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoWeakness( Mobile attacker, Mobile defender )
	    {
	        SpellHelper.CheckReflect( 1, attacker, ref defender );

	        if( HandleSelfMagicalAbsorption( attacker, defender ) )
	            return;

	        SpellHelper.AddStatCurse( attacker, defender, StatType.Str );

	        if( defender.Spell != null )
	            defender.Spell.OnCasterHurt();

	        defender.Paralyzed = false;

	        defender.FixedParticles( 0x3779, 10, 15, 5009, EffectLayer.Waist );
	        defender.PlaySound( 0x1E6 );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoBurn( Mobile attacker, Mobile defender )
	    {
            if( !attacker.CanSee( defender ) || attacker.InLOS( defender ) || !defender.Alive )
                return;

			if ( !attacker.CanBeHarmful( defender, false ) )
				return;

			attacker.DoHarmful( defender );

			SpellHelper.Turn( attacker, defender );
			SpellHelper.CheckReflect( 1, ref attacker, ref defender );

            double damage = DiceRoll.Roll( "1d3+3" );

			attacker.MovingParticles( defender, 0x36E4, 5, 0, false, true, 3006, 4006, 0 );
			attacker.PlaySound( 0x1E5 );

			MidgardSpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, SpellType.Fire );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoWound( Mobile attacker, Mobile defender )
	    {
	        DoHarm( attacker, defender );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoDaemonBreath( Mobile attacker, Mobile defender )
	    {
	        DoFireball( attacker, defender );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoThunder( Mobile attacker, Mobile defender )
	    {
	        DoLightning( attacker, defender );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoMagesBane( Mobile attacker, Mobile defender )
	    {
	        SpellHelper.CheckReflect( 4, attacker, ref defender );

	        if( HandleSelfMagicalAbsorption( attacker, defender ) )
	            return;

	        if( defender.Spell != null )
	            defender.Spell.OnCasterHurt();

	        defender.Paralyzed = false;

	        if( defender.Mana >= 100 )
	            defender.Mana -= Utility.Random( 1, 100 );
	        else
	            defender.Mana -= Utility.Random( 1, defender.Mana );

	        defender.FixedParticles( 0x374A, 10, 15, 5032, EffectLayer.Head );
	        defender.PlaySound( 0x1F8 );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoGhoulTouch( Mobile attacker, Mobile defender )
	    {
	        SpellHelper.CheckReflect( 5, attacker, ref defender );

	        if( HandleSelfMagicalAbsorption( attacker, defender ) )
	            return;

	        defender.Paralyze( TimeSpan.FromSeconds( 7.0 ) );
	        defender.PlaySound( 0x204 );
	        defender.FixedEffect( 0x376A, 6, 1 );

	        ConsumeMagicalCharge( attacker );
	    }

	    private void DoEvil( Mobile attacker, Mobile defender )
	    {
	        SpellHelper.Turn( attacker, defender );

	        SpellHelper.CheckReflect( 4, attacker, ref defender );

	        if( HandleSelfMagicalAbsorption( attacker, defender ) )
	            return;

	        SpellHelper.AddStatCurse( attacker, defender, StatType.Str );
	        SpellHelper.AddStatCurse( attacker, defender, StatType.Dex );
	        SpellHelper.AddStatCurse( attacker, defender, StatType.Int );

	        if( defender.Spell != null )
	            defender.Spell.OnCasterHurt();

	        defender.Paralyzed = false;

	        defender.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
	        defender.PlaySound( 0x1EA );

	        ConsumeMagicalCharge( attacker );
	    }
	    #endregion

	    #endregion

	    #region poison
	    [CommandProperty( AccessLevel.GameMaster )]
	    public double PoisonerSkill { get; set; }

        private double m_PoisonChance;

        [CommandProperty( AccessLevel.GameMaster )]
        public double PoisonChance
        {
            get { return m_PoisonChance; }
            set { m_PoisonChance = value; InvalidateProperties(); InvalidateSecondAgeNames(); SingleClickChanged(); }
        }

	    public virtual bool CanPoison( Mobile from )
	    {
	        return Poison == null || from.Skills[ SkillName.Poisoning ].Value >= GetMinSkillToPoison( Poison );
	    }

	    public static double GetMinSkillToPoison( Poison poison )
	    {
	        if( poison == null )
	            return 0.0;

            switch( poison.RealLevel )
            {
                case 0: return 30;  // Lesser
                case 1: return 45;  // Regular
                case 2: return 63;  // Great
                case 3: return 80;  // Deadly
                case 4: return 100; // Lethal
                default: return 100;
            }
	    }
	    #endregion

        private int m_MinRange; //edit by Arlas

        [CommandProperty( AccessLevel.GameMaster )]
        public int MinRange
        {
            get { return ( m_MinRange == -1 ? OldMinRange : m_MinRange ); }
            set { m_MinRange = value; InvalidateProperties(); }
        }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public override int BlockCircle
	    {
	        get { return 1; }
	    }

	    #region quality
	    public int OldScaleDurabilityByQuality()
	    {
	        if( Quality == WeaponQuality.Exceptional )
	            return 20;

	        if( CustomQuality == Server.Quality.Undefined )
	            return 0;
	        else if( CustomQuality <= Server.Quality.VeryLow )
	            return -20;
	        else if( CustomQuality <= Server.Quality.Low )
	            return -10;
	        else if( CustomQuality <= Server.Quality.Decent )
	            return -4;
	        else if( CustomQuality <= Server.Quality.BelowNormal )
	            return 0;
	        else if( CustomQuality <= Server.Quality.Standard )
	            return +8;
	        else if( CustomQuality <= Server.Quality.Superior )
	            return +12;
	        else if( CustomQuality <= Server.Quality.Great )
	            return +16;
	        else
	            return +20;
	    }

	    public double OldScaleDamageByQuality()
	    {
            if( CustomQuality == Server.Quality.Undefined )
                return 0.0;

            double value = CustomQuality - 1.0;

            if( IsMagical )
                return 0.3;

            if( value > 0.3 )
                value = 0.3;
            else if( value < -0.3 )
                value = -0.3;

            return value;

            /*
	        if( Quality == WeaponQuality.Exceptional )
	            return +4;

	        if( CustomQuality == Server.Quality.Undefined )
	            return +0;
	        else if( CustomQuality <= Server.Quality.VeryLow )
	            return -4;
	        else if( CustomQuality <= Server.Quality.Low )
	            return -3;
	        else if( CustomQuality <= Server.Quality.Decent )
	            return -1;
	        else if( CustomQuality <= Server.Quality.BelowNormal )
	            return +0;
	        else if( CustomQuality <= Server.Quality.Standard )
	            return +1;
	        else if( CustomQuality <= Server.Quality.Superior )
	            return +2;
	        else if( CustomQuality <= Server.Quality.Great )
	            return +3;
	        else
	            return +4;
            */
	    }

	    [CommandProperty( AccessLevel.Administrator )]
	    public override double CustomQuality
	    {
	        get { return base.CustomQuality; }
	        set
	        {
	            base.CustomQuality = value;
	            InvalidateSecondAgeNames();
	        }
	    }
	    #endregion

	    #region naming system
	    private string m_SecondAgeFullName;
	    private string m_SecondAgeUnIdentifiedName;

	    public void InvalidateSecondAgeNames()
	    {
	        BuildSecondAgeNames( string.IsNullOrEmpty( Name ) ? StringList.LocalizationIta[ LabelNumber ] : Name, "ITA" );
	    }

        private void BuildMagicalSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();
            if( language == "ITA" )
            {
                // 'katana'
                builder.Append( rawName +" " );

                // 'exceptional '
                if( Quality == WeaponQuality.Exceptional )
                    builder.Append( "eccezionale " );


                // 'radiand diamond '
                if( !CraftResources.IsStandard( m_Resource ) )
                    builder.AppendFormat( "{0} ", MaterialName.ToLower() );

                // 'cloud '
                if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
                    builder.AppendFormat( "{0}", StoneEnchantItemState.Definition.Prefix );

                // 'silver ' or empty string
                string slayer = OldSlayerName;
                if( slayer.Length > 0 )
                    builder.AppendFormat( "{0} ", slayer );

                // 'fortified, supremely accurate '
                string dur = DurabilityLevelName;
                string acc = AccuracyLevelName;

                if( dur.Length > 0 && acc.Length > 0 )
                    builder.AppendFormat( "{0}, {1} ", dur, acc );
                else if( dur.Length > 0 )
                    builder.AppendFormat( "{0} ", dur );
                else if( acc.Length > 0 )
                    builder.AppendFormat( "{0} ", acc );
            }
            else
            {
                // 'exceptional '
                if( Quality == WeaponQuality.Exceptional )
                    builder.Append( "exceptional " );

                // 'silver ' or empty string
                string slayer = OldSlayerName;
                if( slayer.Length > 0 )
                    builder.AppendFormat( "{0} ", slayer );

                // 'fortified, supremely accurate '
                string dur = DurabilityLevelName;
                string acc = AccuracyLevelName;

                if( dur.Length > 0 && acc.Length > 0 )
                    builder.AppendFormat( "{0}, {1} ", dur, acc );
                else if( dur.Length > 0 )
                    builder.AppendFormat( "{0} ", dur );
                else if( acc.Length > 0 )
                    builder.AppendFormat( "{0} ", acc );

                // 'cloud '
                if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
                    builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );

                // 'radiand diamond '
                string material = MaterialName.ToLower();
                if( material.Length > 0 )
                    builder.AppendFormat( "{0} ", material );

                // 'katana'
                builder.Append( rawName );
            }

            // ' of vanquishing and Ghoul's Touch'
            string dam = DamageLevelName;
            string mag = MagicalEffectName;
            if( dam.Length > 0 || mag.Length > 0 )
            {
                if( dam.Length > 0 && mag.Length > 0 )
                    builder.AppendFormat( " of {0} and {1}", dam, mag );
                else if( dam.Length > 0 )
                    builder.AppendFormat( " of {0}", dam );
                else
                    builder.AppendFormat( " of {0}", mag );

                if( MagicalCharges > 0 )
                    builder.AppendFormat( language == "ITA" ? " con {0} cariche" : " with {0} charges", MagicalCharges );
            }

            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " [forgiato da {0}]" : " [forged by {0}]", StringUtility.Capitalize( Crafter.Name ) );

            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
            m_SecondAgeUnIdentifiedName = string.Format( language == "ITA" ? "{0} magica" : "magic {0}", rawName );
        }

        private void BuildStandardSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();
            if( language == "ITA" )
            {
                // 'katana'
                builder.Append( rawName + " " );

                // 'exceptional '
                if( Quality == WeaponQuality.Exceptional )
                    builder.Append( "eccezionale " );

                // 'radiand diamond '
                if( !CraftResources.IsStandard( m_Resource ) )
                    builder.AppendFormat( "{0} ", MaterialName.ToLower() );

                // 'cloud '
                if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
                    builder.AppendFormat( "{0}", StoneEnchantItemState.Definition.Prefix );
            }
            else
            {
                // 'exceptional '
                if( Quality == WeaponQuality.Exceptional )
                    builder.Append( "exceptional " );

                // 'cloud '
                if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
                    builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );

                // 'radiand diamond '
                if( !CraftResources.IsStandard( m_Resource ) )
                    builder.AppendFormat( "{0} ", MaterialName.ToLower() );

                // 'katana'
                builder.Append( rawName );
            }

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " [forgiato da {0}]" : " [forged by {0}]", StringUtility.Capitalize( Crafter.Name ) );

            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
            m_SecondAgeUnIdentifiedName = builder.ToString();
        }

        private void BuildSecondAgeNames( string rawName, string language )
        {
            if( IsMagical )
                BuildMagicalSecondAgeNames( rawName, language );
            else
                BuildStandardSecondAgeNames( rawName, language );
        }

        public virtual void AppendPoison( Mobile from )
        {
            if( m_Poison != null && m_PoisonCharges > 0 )
            {
                string veleno = ( from.Language == "ITA" ? "veleno" : "poison" );

                if( m_Poison.Level >= 39 )//Lentezza
                    veleno = ( from.Language == "ITA" ? "vel. lentezza" : "slow poison" );
                else if( m_Poison.Level >= 34 )//Blocco
                    veleno = ( from.Language == "ITA" ? "vel. blocco" : "block poison" );
                else if( m_Poison.Level >= 29 )//Paralisi
                    veleno = ( from.Language == "ITA" ? "vel. paralisi" : "paralysis poison" );
                else if( m_Poison.Level >= 24 )//Stanchezza
                    veleno = ( from.Language == "ITA" ? "vel. stanchezza" : "fatigue poison" );
                else if( m_Poison.Level >= 19 )//Magia
                    veleno = ( from.Language == "ITA" ? "vel. magia" : "magic poison" );
                else
                    veleno = ( from.Language == "ITA" ? "veleno" : "poison" );

                switch( m_Poison.RealLevel )
                {
                    case 0: veleno = ( from.Language == "ITA" ? "[" + veleno + " minore: {0}]" : "[lesser " + veleno + ": {0}]" ); break; // Lesser
                    case 1: veleno = ( from.Language == "ITA" ? "[" + veleno + ": {0}]" : "[" + veleno + ": {0}]" ); break; // Regular
                    case 2: veleno = ( from.Language == "ITA" ? "[" + veleno + " maggiore: {0}]" : "[great " + veleno + ": {0}]" ); break; // Great
                    case 3: veleno = ( from.Language == "ITA" ? "[" + veleno + " mortale: {0}]" : "[deadly " + veleno + ": {0}]" ); break; // Deadly
                    case 4: veleno = ( from.Language == "ITA" ? "[" + veleno + " letale: {0}]" : "[lethal " + veleno + ": {0}]" ); break; // Lethal
                    default: veleno = "[" + veleno + " : {0}]"; break;
                }

                LabelTo( from, string.Format( veleno, m_PoisonCharges ) ); //string.Format( from.Language == "ITA" ? "[avvelenato]" : "[Poisoned]" ) );
            }
        }

        public virtual void LabelQualityTo( Mobile from )
        {
            //considerazioni qualità, Arlas
            string messageq = from.Language == "ITA" ? "E' in ottimo stato." : "It's in perfect condition.";
            if( m_MaxHits > 0 )
            {
                if( ( m_Hits / (double)m_MaxHits ) <= 0.2 )
                    messageq = from.Language == "ITA" ? "E' da buttare." : "It's rubbish.";
                else if( ( m_Hits / (double)m_MaxHits ) <= 0.5 )
                    messageq = from.Language == "ITA" ? "E' certamente da riparare." : "It needs repair, obviously.";
                else if( ( m_Hits / (double)m_MaxHits ) <= 0.8 )
                    messageq = from.Language == "ITA" ? "E' in buone condizioni." : "It is pretty good.";
            }
            from.PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, messageq, from.NetState );
        }

        public override void OnOldSingleClick( Mobile from )
	    {
	        if( Deleted || !from.CanSee( this ) )
	            return;

	        string name = StringList.GetClilocString( Name, LabelNumber, from.Language );

	        if( m_SecondAgeFullName == null || m_SecondAgeUnIdentifiedName == null )
	            BuildSecondAgeNames( name, from.Language );

            AppendPoison( from );

	        if( this is ITreasureOfMidgard )
	            XmlBlessedCursedAttach.OnSingleClick( from, this );

	        if( ArtifactRarity > 0 || this is ITreasureOfMidgard )
	            LabelTo( from, name );
	        else if( !String.IsNullOrEmpty( Name ) && Name != DefaultName )
	            LabelTo( from, Name );
	        else if( IsMagical )
	            LabelTo( from, StringUtility.ConvertItemName( ( IsIdentifiedFor( from ) ? m_SecondAgeFullName : m_SecondAgeUnIdentifiedName ), false, from.Language ) );
	        else
	            LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeFullName, false, from.Language ) );

	        if( IsGuildItem )
	        {
	            if( m_OwnerGuild != null && GuildedOwner != null && GuildedOwner.Name != null )
	                LabelTo( from, from.Language == "ITA" ? "[gilda : {0} ({1})]" : "[guild : {0} ({1})]", m_OwnerGuild.Name, GuildedOwner.Name );
	            else if( m_OwnerGuild != null )
	                LabelTo( from, from.Language == "ITA" ? "[gilda : {0}]" : "[guild : {0}]", m_OwnerGuild.Name );
	        }

	        if( !String.IsNullOrEmpty( m_EngravedText ) )
	            LabelTo( from, "* {0} *", m_EngravedText );

	        if( IsPortingItem )
	            LabelOwnageTo( from );

            LabelQualityTo( from );
	    }

	    public override string QualityName
	    {
	        get
	        {
	            if( CustomQuality == Server.Quality.Undefined )
	                return String.Empty;
	            else if( CustomQuality <= Server.Quality.VeryLow )
	                return "very low quality";
	            else if( CustomQuality <= Server.Quality.Low )
	                return "low quality";
	            else if( CustomQuality <= Server.Quality.Decent )
	                return String.Empty;
	            else if( CustomQuality <= Server.Quality.BelowNormal )
	                return String.Empty;
	            else if( CustomQuality <= Server.Quality.Standard )
	                return String.Empty;
	            else if( CustomQuality <= Server.Quality.Superior )
	                return "superior";
	            else if( CustomQuality <= Server.Quality.Great )
	                return "great";
	            else
	                return "exceptional";
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public string OldSlayerName
	    {
	        get
	        {
	            if( Slayer == Server.Items.SlayerName.Silver )
	                return "silver";

	            return string.Empty;
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public string DamageLevelName
	    {
	        get
	        {
	            if( DamageLevel == WeaponDamageLevel.Regular )
	                return string.Empty;

	            string level;

	            switch( DamageLevel )
	            {
	                case WeaponDamageLevel.Ruin:
	                case WeaponDamageLevel.Might:
	                case WeaponDamageLevel.Force:
	                case WeaponDamageLevel.Power:
	                    level = DamageLevel.ToString().ToLower();
	                    break;
	                case WeaponDamageLevel.Vanq:
	                    level = "vanquishing";
	                    break;
	                default:
	                    level = string.Empty;
	                    break;
	            }

	            return level;
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public string AccuracyLevelName
	    {
	        get
	        {
	            string level;

	            switch( AccuracyLevel )
	            {
	                case WeaponAccuracyLevel.Accurate:
	                    level = "accurate";
	                    break;
	                case WeaponAccuracyLevel.Surpassingly:
	                case WeaponAccuracyLevel.Eminently:
	                case WeaponAccuracyLevel.Exceedingly:
	                case WeaponAccuracyLevel.Supremely:
	                    level = string.Format( "{0} accurate", AccuracyLevel.ToString().ToLower() );
	                    break;
	                default:
	                    level = string.Empty;
	                    break;
	            }

	            return level;
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public string DurabilityLevelName
	    {
	        get
	        {
	            if( DurabilityLevel == WeaponDurabilityLevel.Regular )
	                return string.Empty;

	            string level;

	            switch( DurabilityLevel )
	            {
	                case WeaponDurabilityLevel.Durable:
	                case WeaponDurabilityLevel.Substantial:
	                case WeaponDurabilityLevel.Massive:
	                case WeaponDurabilityLevel.Fortified:
	                case WeaponDurabilityLevel.Indestructible:
	                    level = DurabilityLevel.ToString().ToLower();
	                    break;
	                default:
	                    level = string.Empty;
	                    break;
	            }

	            return level.ToLower();
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public string MagicalEffectName
	    {
	        get
	        {
	            switch( MagicalAttribute )
	            {
	                case WeaponMagicalAttribute.Clumsiness:
	                case WeaponMagicalAttribute.Feeblemindedness:
	                case WeaponMagicalAttribute.Weakness:
	                case WeaponMagicalAttribute.Burning:
	                case WeaponMagicalAttribute.Wounding:
	                case WeaponMagicalAttribute.Thunder:
	                case WeaponMagicalAttribute.Evil:
	                    return MagicalAttribute.ToString().ToLower();
	                case WeaponMagicalAttribute.DaemonBreath:
	                    return "daemon's breath";
	                case WeaponMagicalAttribute.MagesBane:
	                    return "mage's bane";
	                case WeaponMagicalAttribute.GhoulTouch:
	                    return "ghoul's touch";
	                default:
	                    return "";
	            }
	        }
	    }
	    #endregion

	    #region Is... something
	    [CommandProperty( AccessLevel.GameMaster )]
	    public bool IsMagical
	    {
	        get
	        {
	            return ( Slayer != Server.Items.SlayerName.None || DurabilityLevel != WeaponDurabilityLevel.Regular || DamageLevel != WeaponDamageLevel.Regular || AccuracyLevel != WeaponAccuracyLevel.Regular ||
	                     MagicalAttribute != WeaponMagicalAttribute.None );
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public bool IsWrestlingWeapon
	    {
	        get { return DefSkill == SkillName.Wrestling; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public bool IsWoodenWeapon
	    {
	        get { return CraftResources.GetType( Resource ) == CraftResourceType.Wood; }
	    }
	    #endregion

	    #region craft resource related
	    [CommandProperty( AccessLevel.GameMaster )]
	    public string MaterialName
	    {
	        get { return CraftResources.IsStandard( m_Resource ) ? string.Empty : CraftResources.GetName( m_Resource ); }
	    }

	    private CraftAttributeInfo ResourceInfo
	    {
	        get
	        {
	            CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
	            if( resInfo != null )
	            {
	                CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
	                if( attrInfo != null )
	                    return attrInfo;
	            }

	            return null;
	        }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int OldMaterialWrestEvaBonus
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldWrestlerEvasion : 0; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int OldMaterialWrestHitBonus
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldWrestlerHitRate : 0; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int OldMaterialSpeedBonus
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldSpeed : 0; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int OldMaterialMinRangeBonus
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldMinRange : 0; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int OldMaterialMaxRangeBonus
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldMaxRange : 0; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public int OldMaterialDamageBonus
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldDamage : 0; }
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public double OldMaterialStaticMultiply
	    {
	        get { return ResourceInfo != null ? ResourceInfo.OldStaticMultiply : 1.0; }
	    }
	    #endregion

	    #region [serialization]
	    private static void SetSaveFlag( ref MidgardFlag flags, MidgardFlag toSet, bool setIf )
	    {
	        if( setIf )
	            flags |= toSet;
	    }

	    private static bool GetSaveFlag( MidgardFlag flags, MidgardFlag toGet )
	    {
	        return ( ( flags & toGet ) != 0 );
	    }

	    [Flags]
	    private enum MidgardFlag
	    {
	        None = 0x00000000,

	        MagicEffect = 0x00000001,
	        MagicCharges = 0x00000002,
	        PoisonerSkill = 0x00000004,
	        ItemIDList = 0x00000008,

	        StoneEnchanted = 0x00000010,
	        IsGuildUniform = 0x00000020,
	        Guild = 0x00000040,
	        GuildedOwner = 0x00000080
	    }

	    private void MidgardSerialize( GenericWriter writer )
	    {
	        MidgardFlag flags = MidgardFlag.None;

	        SetSaveFlag( ref flags, MidgardFlag.MagicEffect, MagicalAttribute != WeaponMagicalAttribute.None );
	        SetSaveFlag( ref flags, MidgardFlag.MagicCharges, MagicalCharges != 0 );
	        SetSaveFlag( ref flags, MidgardFlag.PoisonerSkill, PoisonerSkill != 0.0 );
	        SetSaveFlag( ref flags, MidgardFlag.ItemIDList, m_IdentifiersList != null && m_IdentifiersList.Count > 0 );
	        SetSaveFlag( ref flags, MidgardFlag.StoneEnchanted, StoneEnchantItemState != null );

	        writer.WriteEncodedInt( (int) flags );

	        if( GetSaveFlag( flags, MidgardFlag.MagicEffect ) )
	            writer.Write( (int) MagicalAttribute );

	        if( GetSaveFlag( flags, MidgardFlag.MagicCharges ) )
	            writer.Write( MagicalCharges );

	        if( GetSaveFlag( flags, MidgardFlag.PoisonerSkill ) )
	            writer.Write( PoisonerSkill );

	        if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
	            writer.Write( m_IdentifiersList, true );

	        if( GetSaveFlag( flags, MidgardFlag.StoneEnchanted ) && StoneEnchantItemState != null )
	            StoneEnchantItemState.Serialize( writer );

	        if( GetSaveFlag( flags, MidgardFlag.IsGuildUniform ) )
	            writer.Write( IsGuildItem );

	        if( GetSaveFlag( flags, MidgardFlag.Guild ) )
	            writer.Write( m_OwnerGuild );

	        if( GetSaveFlag( flags, MidgardFlag.GuildedOwner ) )
	            writer.Write( GuildedOwner );
	    }

	    private void MidgardDeserialize( GenericReader reader )
	    {
	        var flags = (MidgardFlag) reader.ReadEncodedInt();

	        if( GetSaveFlag( flags, MidgardFlag.MagicEffect ) )
	            MagicalAttribute = (WeaponMagicalAttribute) reader.ReadInt();

	        if( GetSaveFlag( flags, MidgardFlag.MagicCharges ) )
	            MagicalCharges = reader.ReadInt();

	        if( GetSaveFlag( flags, MidgardFlag.PoisonerSkill ) )
	            PoisonerSkill = reader.ReadDouble();

	        if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
	            m_IdentifiersList = reader.ReadStrongMobileList();

	        if( GetSaveFlag( flags, MidgardFlag.StoneEnchanted ) )
	            StoneEnchantItemState = new StoneEnchantItem( reader );

	        if( GetSaveFlag( flags, MidgardFlag.IsGuildUniform ) )
	            IsGuildItem = reader.ReadBool();

	        if( GetSaveFlag( flags, MidgardFlag.Guild ) )
	            m_OwnerGuild = reader.ReadGuild();

	        if( GetSaveFlag( flags, MidgardFlag.GuildedOwner ) )
	            GuildedOwner = reader.ReadMobile();
	    }
	    #endregion

	    #region IIdentificable members
	    private List<Mobile> m_IdentifiersList;

	    private const int MaxIdentifiers = 50;

	    public void CopyIdentifiersTo( IIdentificable identificable )
	    {
	        if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
	        {
	            foreach( Mobile mobile in m_IdentifiersList )
	                identificable.AddIdentifier( mobile );
	        }
	    }

	    public void ClearIdentifiers()
	    {
	        if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
	            m_IdentifiersList.Clear();
	    }

	    public void AddIdentifier( Mobile from )
	    {
	        if( m_IdentifiersList == null )
	            m_IdentifiersList = new List<Mobile>();

	        if( !m_IdentifiersList.Contains( from ) )
	            m_IdentifiersList.Add( from );

	        if( m_IdentifiersList.Count > MaxIdentifiers )
	            m_IdentifiersList.RemoveAt( 0 );

	        SendDebugMessage( from, "IdentifiersList for weapon {0}", Serial.ToString() );
	        foreach( Mobile m in m_IdentifiersList )
	            SendDebugMessage( from, m.Name );
	    }

	    public bool IsIdentifiedFor( Mobile from )
	    {
	        if( Crafter != null && Crafter == from )
	            return true;

	        if( !IsMagical )
	            return true;

	        if( m_IdentifiersList == null )
	            return false;

	        return m_IdentifiersList.Contains( from );
	    }

	    public void DisplayItemInfo( Mobile from )
	    {
	    }
	    #endregion

	    #region IRepairable members
	    public virtual bool Repair( Mobile from, BaseTool tool )
	    {
	        CraftSystem system = tool.CraftSystem;
	        int number;

	        SkillName skill = system.MainSkill;
	        int toWeaken = 0;

	        if( skill != SkillName.Tailoring )
	        {
	            double skillLevel = from.Skills[ skill ].Base;

	            if( skillLevel >= 90.0 )
	                toWeaken = 1;
	            else if( skillLevel >= 70.0 )
	                toWeaken = 2;
	            else
	                toWeaken = 3;
	        }

	        if( system.CraftItems.SearchForSubclass( GetType() ) == null && !IsSpecialWeapon( system ) )
	            number = 1044277; // That item cannot be repaired.
	        else if( !IsChildOf( from.Backpack ) )
	            number = 1044275; // The item must be in your backpack to repair it.
	        else if( MaxHitPoints <= 0 || HitPoints == MaxHitPoints )
	            number = 1044281; // That item is in full repair
	        else if( MaxHitPoints <= toWeaken )
	            number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
	        else
	        {
	            if( RepairHelper.CheckWeaken( from, skill, HitPoints, MaxHitPoints ) )
	            {
	                MaxHitPoints -= toWeaken;
	                HitPoints = Math.Max( 0, HitPoints - toWeaken );
	            }

	            if( RepairHelper.CheckRepairDifficulty( from, skill, HitPoints, MaxHitPoints ) )
	            {
	                number = 1044279; // You repair the item.
	                system.PlayCraftEffect( from );
	                HitPoints = MaxHitPoints;
	            }
	            else
	            {
	                number = 1044280; // You fail to repair the item.
	                system.PlayCraftEffect( from );
	            }
	        }

	        if( number > 0 )
	            from.SendLocalizedMessage( number );

	        return ( number == 1044279 );
	    }

	    public bool IsSpecialWeapon( CraftSystem system )
	    {
	        // Weapons repairable but not craftable

	        if( system is DefTinkering )
	            return ( this is Cleaver ) || ( this is Hatchet ) || ( this is Pickaxe ) || ( this is ButcherKnife ) || ( this is SkinningKnife );
	        else if( system is DefCarpentry )
	            return ( this is Club ) || ( this is BlackStaff ) || ( this is MagicWand );
	        else if( system is DefBlacksmithy )
	            return ( this is Pitchfork );

	        return false;
	    }
	    #endregion

	    #region ISmeltable members
	    public static bool WoodSmeltEnabled = true;

	    public virtual bool Resmelt( Mobile from )
	    {
	        if( !IsChildOf( from.Backpack ) )
	        {
	            from.SendMessage( from.Language == "ITA" ? "L'oggetto da usare deve essere nel tuo zaino." : "Items you wish to reuse must be in your backpack." );
	            return false;
	        }

	        try
	        {
	            if( CraftResources.GetType( Resource ) != CraftResourceType.Metal && !WoodSmeltEnabled )
	                return false;

	            CraftResourceInfo info = CraftResources.GetInfo( Resource );
	            if( info == null || info.ResourceTypes.Length == 0 )
	                return false;

	            bool found = false;

	            CraftItem craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor( GetType() );
	            if( craftItem != null && craftItem.Resources.Count > 0 )
	                found = true;
	            else
	            {
	                craftItem = DefBowFletching.CraftSystem.CraftItems.SearchFor( GetType() );
	                if( ( craftItem != null && craftItem.Resources.Count > 0 ) )
	                    found = true;
	                else
	                {
	                    craftItem = DefCarpentry.CraftSystem.CraftItems.SearchFor( GetType() );
	                    if( ( craftItem != null && craftItem.Resources.Count > 0 ) )
	                        found = true;
	                }
	            }

	            if( !found )
	                return false;

	            CraftRes craftResource = craftItem.Resources.GetAt( 0 );
	            if( craftResource.Amount < 2 )
	                return false; // Not enough metal or wood to resmelt

	            Type resourceType = info.ResourceTypes[ 0 ];
	            var res = (Item) Activator.CreateInstance( resourceType );

	            if( PlayerConstructed )
	                res.Amount = craftResource.Amount / 2;
	            else
	                res.Amount = 1;

	            Delete();

	            from.AddToBackpack( res );

	            if( CraftResources.GetType( Resource ) == CraftResourceType.Metal )
	            {
	                from.SendMessage( "You melt the item down into ingots." );
	                from.PlaySound( 0x2A );
	                from.PlaySound( 0x240 );
	            }
	            else if( CraftResources.GetType( Resource ) != CraftResourceType.Wood )
	            {
	                from.SendMessage( "You managed to obtain some wood disassembling that item." );
	                from.PlaySound( 0x55 );
	                from.PlaySound( 0x240 );
	            }
	            else
	            {
	                from.SendMessage( "You obtained some usable resouce disassembling that item." );
	                from.PlaySound( 0x240 );
	            }
	            return true;
	        }
            catch( Exception e )
            {
                Console.WriteLine( e.ToString() );
            }

	        return false;
	    }
	    #endregion

	    #region IRecyclable members
	    public bool CanBeRecycledBy( Mobile from )
	    {
	        return true;
	    }

	    public double GetDifficulty( Mobile from )
	    {
	        CraftItem craftItem;
	        CraftSystem sys = Midgard.Engines.FeedStockRecoverySystem.Core.Find( this, out craftItem );

	        return sys != null ? sys.GetMaterialDifficulty( Resource ) : 1.0;
	    }

	    public bool Recycle( Mobile from, BaseRecyclingTool tool )
	    {
	        CraftItem craftItem;
	        CraftSystem sys = Midgard.Engines.FeedStockRecoverySystem.Core.Find( this, out craftItem );

	        CraftResourceInfo info = CraftResources.GetInfo( Resource );
	        if( info == null || info.ResourceTypes.Length == 0 )
	            return false;

	        if( sys == null )
	            return false;

	        CraftRes craftResource = craftItem.Resources.GetAt( 0 );
	        if( craftResource.Amount < 2 )
	            return false;

	        Type resourceType = info.ResourceTypes[ 0 ];
	        var res = (Item) Activator.CreateInstance( resourceType );

	        if( PlayerConstructed )
	            res.Amount = craftResource.Amount / 2;
	        else
	            res.Amount = 1;

	        Delete();

	        from.AddToBackpack( res );

	        if( CraftResources.GetType( Resource ) == CraftResourceType.Metal )
	        {
	            from.SendMessage( "You recycled this weapon into ingots." );
	            from.PlaySound( 0x2A );
	            from.PlaySound( 0x240 );
	        }
	        else if( CraftResources.GetType( Resource ) == CraftResourceType.Wood )
	        {
	            from.SendMessage( "You managed to obtain some wood disassembling that item." );
	            from.PlaySound( 0x55 );
	            from.PlaySound( 0x240 );
	        }
	        else
	        {
	            from.SendMessage( "You obtained some usable resource disassembling that item." );
	            from.PlaySound( 0x240 );
	        }

	        return true;
	    }
	    #endregion

        // TODO DIES!!
/*
        #region [old naming]
        private Packet m_MagicSingleClick;

        public override void SingleClickChanged()
        {
            base.SingleClickChanged();
            Packet.Release( ref m_MagicSingleClick );
        }

        public override void SendSingleClickTo( Mobile from )
        {
            if( !IsMagical || !( m_IdentifiersList.Contains( from ) || from.AccessLevel > AccessLevel.Counselor ) )
            {
                base.SendSingleClickTo( from );
            }
            else
            {
                if( m_MagicSingleClick == null )
                {
                    m_MagicSingleClick = new AsciiMessage( Serial, ItemID, MessageType.Label, 0x3B2, 3, "", BuildMagicSingleClick() );
                    m_MagicSingleClick.SetStatic();
                }

                from.NetState.Send( m_MagicSingleClick );
            }
        }

        public override string BuildSingleClick()
        {
            StringBuilder sb = new StringBuilder();

            if( AppendLootType( sb ) )
                sb.Append( ", " );

            // 'exceptional ' or 'exceptional, '
            if( Quality == WeaponQuality.Exceptional )
                sb.Append( "exceptional, " );

            if( IsMagical )
                sb.Append( "magic, " );

            if( m_PoisonCharges > 0 && m_Poison != null )
                sb.Append( "poisoned, " );

            if( sb.Length > 2 )
                sb.Remove( sb.Length - 2, 1 ); // remove the last comma

            AppendClickName( sb );
            InsertNamePrefix( sb );

            if( m_Crafter != null && !m_Crafter.Deleted )
                sb.AppendFormat( " (crafted by {0})", m_Crafter.Name );

            return sb.ToString();
        }

        public virtual string BuildMagicSingleClick()
        {
            StringBuilder sb = new StringBuilder();

            if( AppendLootType( sb ) )
                sb.Append( ", " );

            if( m_Quality == WeaponQuality.Exceptional )
                sb.Append( "exceptional, " );

            if( m_DurabilityLevel != WeaponDurabilityLevel.Regular )
                sb.AppendFormat( "{0}, ", m_DurabilityLevel.ToString().ToLower() );

            if( m_AccuracyLevel != WeaponAccuracyLevel.Regular )
            {
                if( m_AccuracyLevel !=  WeaponAccuracyLevel.Accurate )
                {
                    sb.Append( m_AccuracyLevel.ToString().ToLower() );
                    sb.Append( ' ' );
                }
                sb.Append( "accurate, " );
            }

            if( m_Slayer == SlayerName.Silver )
                sb.Append( "silver, " );

            if( m_PoisonCharges > 0 && m_Poison != null )
                sb.Append( "poisoned, " );

            if( sb.Length > 2 )
                sb.Remove( sb.Length - 2, 1 ); // remove the last comma

            AppendClickName( sb );
            InsertNamePrefix( sb );

            if( m_DamageLevel > WeaponDamageLevel.Regular )
            {
                sb.Append( " of " );
                sb.Append( m_DamageLevel == WeaponDamageLevel.Vanq ? "vanquishing" : m_DamageLevel.ToString().ToLower() );
            }

            if( m_Effect != SpellEffect.None && m_EffectCharges > 0 )
            {
                if( m_DamageLevel > WeaponDamageLevel.Regular )
                    sb.Append( " and " );
                else
                    sb.Append( " of " );
                sb.Append( SpellCastEffect.GetName( m_Effect ) );
                sb.AppendFormat( " with {0} charge{1}", m_EffectCharges, m_EffectCharges != 1 ? "s" : "" );
            }
            
            if( m_Crafter != null && !m_Crafter.Deleted )
                sb.AppendFormat( " (crafted by {0})", m_Crafter.Name );

            return sb.ToString();
        }
        #endregion
*/
        #endregion
    }

	public enum CheckSlayerResult
	{
		None,
		Slayer,
		Opposition
	}
}