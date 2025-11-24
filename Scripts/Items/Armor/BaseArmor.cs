using System;
using System.Collections.Generic;
using System.Text;

using Midgard.Engines.Classes;
using Midgard.Engines.FeedStockRecoverySystem;

using Server.Engines.Craft;
using Server.Engines.XmlSpawner2;
using Server.Factions;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Second;
using ABT = Server.Items.ArmorBodyType;
using AMA = Server.Items.ArmorMeditationAllowance;
using AMT = Server.Items.ArmorMaterialType;

using Midgard.Engines.MidgardTownSystem;
using Midgard.Engines.OldCraftSystem;
using Midgard.Engines.Races;
using Midgard.Engines.SpellSystem;
using Midgard.Engines.StoneEnchantSystem;
using Midgard.Items;
using Midgard.Misc;

namespace Server.Items
{
    public abstract class BaseArmor : Item, IScissorable, IFactionItem, ICraftable, IWearableDurability, IEngravable, ISetItem, IStoneEnchantItem, IIdentificable, IRepairable, ISmeltable, IGuildUniform, IRecyclable, IResourceItem
	{
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

		/* Armor internals work differently now (Jun 19 2003)
		 * 
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - ArmorBase
		 *  - StrBonus
		 *  - DexBonus
		 *  - IntBonus
		 *  - StrReq
		 *  - DexReq
		 *  - IntReq
		 *  - MeditationAllowance
		 */

		// Instance values. These values must are unique to each armor piece.
		private int m_MaxHitPoints;
		private int m_HitPoints;
		private Mobile m_Crafter;
		private ArmorQuality m_Quality;
		private ArmorDurabilityLevel m_Durability;
		private ArmorProtectionLevel m_Protection;
		private CraftResource m_Resource;
		private bool m_Identified, m_PlayerConstructed;
		private int m_PhysicalBonus, m_FireBonus, m_ColdBonus, m_PoisonBonus, m_EnergyBonus;

		private AosAttributes m_AosAttributes;
		private AosArmorAttributes m_AosArmorAttributes;
		private AosSkillBonuses m_AosSkillBonuses;

		// Overridable values. These values are provided to override the defaults which get defined in the individual armor scripts.
		private int m_ArmorBase = -1;
		private int m_StrBonus = -1, m_DexBonus = -1, m_IntBonus = -1;
		private int m_StrReq = -1, m_DexReq = -1, m_IntReq = -1;
		private AMA m_Meditate = (AMA)(-1);


		public virtual bool AllowMaleWearer{ get{ return true; } }
		public virtual bool AllowFemaleWearer{ get{ return true; } }

		public abstract AMT MaterialType{ get; }

		public virtual int RevertArmorBase{ get{ return ArmorBase; } }
		public virtual int ArmorBase{ get{ return 0; } }

		public virtual AMA DefMedAllowance{ get{ return AMA.None; } }
		public virtual AMA AosMedAllowance{ get{ return DefMedAllowance; } }
		public virtual AMA OldMedAllowance{ get{ return DefMedAllowance; } }


		public virtual int AosStrBonus{ get{ return 0; } }
		public virtual int AosDexBonus{ get{ return 0; } }
		public virtual int AosIntBonus{ get{ return 0; } }
		public virtual int AosStrReq{ get{ return 0; } }
		public virtual int AosDexReq{ get{ return 0; } }
		public virtual int AosIntReq{ get{ return 0; } }


		public virtual int OldStrBonus{ get{ return 0; } }
		public virtual int OldDexBonus{ get{ return 0; } }
		public virtual int OldIntBonus{ get{ return 0; } }
		public virtual int OldStrReq{ get{ return 0; } }
		public virtual int OldDexReq{ get{ return 0; } }
		public virtual int OldIntReq{ get{ return 0; } }

		public virtual bool CanFortify{ get{ return true; } }

		public override void OnAfterDuped( Item newItem )
		{
			BaseArmor armor = newItem as BaseArmor;

			if ( armor == null )
				return;

			armor.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			armor.m_AosArmorAttributes = new AosArmorAttributes( newItem, m_AosArmorAttributes );
			armor.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );

			#region Mondain's Legacy
			armor.m_SetAttributes = new AosAttributes( newItem, m_SetAttributes );
			armor.m_SetSkillBonuses = new AosSkillBonuses( newItem, m_SetSkillBonuses );
			#endregion
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AMA MeditationAllowance
		{
			get{ return ( m_Meditate == (AMA)(-1) ? Core.AOS ? AosMedAllowance : OldMedAllowance : m_Meditate ); }
			set{ m_Meditate = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int BaseArmorRating
		{
			get
			{
				if ( m_ArmorBase == -1 )
					return ArmorBase;
				else
					return m_ArmorBase;
			}
			set
			{ 
				m_ArmorBase = value; Invalidate(); 
			}
		}

		public double BaseArmorRatingScaled
		{
			get
			{
				return ( BaseArmorRating * ArmorScalar );
			}
		}

		public virtual double ArmorRating
		{
			get
			{
				int ar = BaseArmorRating;

				if ( m_Protection != ArmorProtectionLevel.Regular )
					ar += 10 + (5 * (int)m_Protection);

                #region mod by Dies Irae
                if( Core.T2A )
                {
                    CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

                    int materialBonus = 0;
                    if( resInfo != null )
                    {
                        CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
                        materialBonus = ( attrInfo == null ) ? 0 : attrInfo.OldArmorBonus;
                    }

                    ar += materialBonus;
                }
                else
                {
                    switch( m_Resource )
                    {
                        case CraftResource.DullCopper: ar += 2; break;
                        case CraftResource.ShadowIron: ar += 4; break;
                        case CraftResource.Copper: ar += 6; break;
                        case CraftResource.Bronze: ar += 8; break;
                        case CraftResource.Gold: ar += 10; break;
                        case CraftResource.Agapite: ar += 12; break;
                        case CraftResource.Verite: ar += 14; break;
                        case CraftResource.Valorite: ar += 16; break;
                        case CraftResource.SpinedLeather: ar += 10; break;
                        case CraftResource.HornedLeather: ar += 13; break;
                        case CraftResource.BarbedLeather: ar += 16; break;

                        default: break;
                    }
                }

                if( !Core.AOS && CustomQuality != Server.Quality.Undefined )
                    ar = (int)ScaleArmorByQuality( ar );
                else
                    ar += -8 + ( 8 * (int)m_Quality );
                #endregion
                
                return ScaleArmorByDurability( ar );
			}
		}

		public double ArmorRatingScaled
		{
			get
			{
				return ( ArmorRating * ArmorScalar );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrBonus
		{
			get{ return ( m_StrBonus == -1 ? Core.AOS ? AosStrBonus : OldStrBonus : m_StrBonus ); }
			set{ m_StrBonus = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexBonus
		{
			get{ return ( m_DexBonus == -1 ? Core.AOS ? AosDexBonus : OldDexBonus : m_DexBonus ); }
			set{ m_DexBonus = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntBonus
		{
			get{ return ( m_IntBonus == -1 ? Core.AOS ? AosIntBonus : OldIntBonus : m_IntBonus ); }
			set{ m_IntBonus = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get{ return ( m_StrReq == -1 ? Core.AOS ? AosStrReq : OldStrReq : m_StrReq ); }
			set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexRequirement
		{
			get{ return ( m_DexReq == -1 ? Core.AOS ? AosDexReq : OldDexReq : m_DexReq ); }
			set{ m_DexReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntRequirement
		{
			get{ return ( m_IntReq == -1 ? Core.AOS ? AosIntReq : OldIntReq : m_IntReq ); }
			set{ m_IntReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Identified
		{
			get{ return m_Identified; }
			set{ m_Identified = value; InvalidateProperties(); }
		}

        /* moved to ICraftable implementation
		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}
        */

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get
			{
				return m_Resource;
			}
			set
			{
				if ( m_Resource != value )
				{
					UnscaleDurability();

					m_Resource = value;
					Hue = CraftResources.GetHue( m_Resource );

					Invalidate();
					InvalidateProperties();

                    InvalidateSecondAgeNames(); // mod by Dies Irae

					if ( Parent is Mobile )
						((Mobile)Parent).UpdateResistances();

					ScaleDurability();
				}
			}
		}

		public virtual double ArmorScalar
		{
			get
			{
				int pos = (int)BodyPosition;

                if( pos >= 0 && pos < /* m_ArmorScalars.Length */ ArmorScalars.Length )
                    return /* m_ArmorScalars */ ArmorScalars[ pos ];

				return 1.0;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHitPoints; }
			set{ m_MaxHitPoints = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get 
			{
				return m_HitPoints;
			}
			set 
			{
				if ( value != m_HitPoints && MaxHitPoints > 0 )
				{
					m_HitPoints = value;

					if ( m_HitPoints < 0 )
						Delete();
					else if ( m_HitPoints > MaxHitPoints )
						m_HitPoints = MaxHitPoints;

					InvalidateProperties();
				}
			}
		}

        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); InvalidateSecondAgeNames(); } // mod by Dies Irae
		}
        */
		
		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorQuality Quality
		{
			get
			{
                #region mod by Dies Irae
                if( CustomQuality >= Server.Quality.Exceptional )
                    return ArmorQuality.Exceptional;
                else if( CustomQuality <= Server.Quality.Low )
                    return ArmorQuality.Low;
                #endregion

			    return m_Quality;
			}
			set{ UnscaleDurability(); m_Quality = value; Invalidate(); InvalidateProperties(); ScaleDurability(); InvalidateSecondAgeNames(); } // mod by Dies Irae
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorDurabilityLevel Durability
		{
			get{ return m_Durability; }
			set{ UnscaleDurability(); m_Durability = value; ScaleDurability(); InvalidateProperties(); InvalidateSecondAgeNames(); } // mod by Dies Irae
		}

		public virtual int ArtifactRarity
		{
			get{ return 0; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorProtectionLevel ProtectionLevel
		{
			get
			{
				return m_Protection;
			}
			set
			{
				if ( m_Protection != value )
				{
					m_Protection = value;

					Invalidate();
					InvalidateProperties();

                    InvalidateSecondAgeNames(); // mod by Dies Irae

					if ( Parent is Mobile )
						((Mobile)Parent).UpdateResistances();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosArmorAttributes ArmorAttributes
		{
			get{ return m_AosArmorAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		public int ComputeStatReq( StatType type )
		{
			int v;

			if ( type == StatType.Str )
				v = StrRequirement;
			else if ( type == StatType.Dex )
				v = DexRequirement;
			else
				v = IntRequirement;

			return AOS.Scale( v, 100 - GetLowerStatReq() );
		}

		public virtual int ComputeStatBonus( StatType type )
        {
            CraftAttributeInfo info = GetResourceAttrs(); // mod by Dies Irae

            if ( type == StatType.Str )
				return StrBonus + Attributes.BonusStr + ( info == null ? 0 : info.ArmorBonusStr ); // mod by Dies Irae
			else if ( type == StatType.Dex )
				return DexBonus + Attributes.BonusDex + ( info == null ? 0 : info.ArmorBonusDex ) + ( info == null ? 0 : ComputeOldMaterialDexMalus( info.OldMalusDex ) ); // mod by Dies Irae
			else
				return IntBonus + Attributes.BonusInt + ( info == null ? 0 : info.ArmorBonusInt ); // mod by Dies Irae
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PhysicalBonus{ get{ return m_PhysicalBonus; } set{ m_PhysicalBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireBonus{ get{ return m_FireBonus; } set{ m_FireBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdBonus{ get{ return m_ColdBonus; } set{ m_ColdBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonBonus{ get{ return m_PoisonBonus; } set{ m_PoisonBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyBonus{ get{ return m_EnergyBonus; } set{ m_EnergyBonus = value; InvalidateProperties(); } }

		public virtual int BasePhysicalResistance{ get{ return 0; } }
		public virtual int BaseFireResistance{ get{ return 0; } }
		public virtual int BaseColdResistance{ get{ return 0; } }
		public virtual int BasePoisonResistance{ get{ return 0; } }
		public virtual int BaseEnergyResistance{ get{ return 0; } }

        /*
        public override int PhysicalResistance{ get{ return BasePhysicalResistance + GetProtOffset() + GetResourceAttrs().ArmorPhysicalResist + m_PhysicalBonus + (m_SetEquipped ? m_SetPhysicalBonus : 0 ); } }
		public override int FireResistance{ get{ return BaseFireResistance + GetProtOffset() + GetResourceAttrs().ArmorFireResist + m_FireBonus + (m_SetEquipped ? m_SetFireBonus : 0 ); } }
		public override int ColdResistance{ get{ return BaseColdResistance + GetProtOffset() + GetResourceAttrs().ArmorColdResist + m_ColdBonus + (m_SetEquipped ? m_SetColdBonus : 0 ); } }
		public override int PoisonResistance{ get{ return BasePoisonResistance + GetProtOffset() + GetResourceAttrs().ArmorPoisonResist + m_PoisonBonus + (m_SetEquipped ? m_SetPoisonBonus : 0 ); } }
		public override int EnergyResistance{ get{ return BaseEnergyResistance + GetProtOffset() + GetResourceAttrs().ArmorEnergyResist + m_EnergyBonus + (m_SetEquipped ? m_SetEnergyBonus : 0 ); } }
        */

        public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }
        
		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorBodyType BodyPosition
		{
			get
			{
				switch ( Layer )
				{
					default:
					case Layer.Neck:		return ArmorBodyType.Gorget;
					case Layer.TwoHanded:	return ArmorBodyType.Shield;
					case Layer.Gloves:		return ArmorBodyType.Gloves;
					case Layer.Helm:		return ArmorBodyType.Helmet;
					case Layer.Arms:		return ArmorBodyType.Arms;

					case Layer.InnerLegs:
					case Layer.OuterLegs:
					case Layer.Pants:		return ArmorBodyType.Legs;

					case Layer.InnerTorso:
					case Layer.OuterTorso:
					case Layer.Shirt:		return ArmorBodyType.Chest;
				}
			}
		}

        /*
		public void DistributeBonuses( int amount )
		{
			for ( int i = 0; i < amount; ++i )
			{
				switch ( Utility.Random( 5 ) )
				{
					case 0: ++m_PhysicalBonus; break;
					case 1: ++m_FireBonus; break;
					case 2: ++m_ColdBonus; break;
					case 3: ++m_PoisonBonus; break;
					case 4: ++m_EnergyBonus; break;
				}
			}

			InvalidateProperties();
		}
        */

		public CraftAttributeInfo GetResourceAttrs()
		{
			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info == null )
				return CraftAttributeInfo.Blank;

			return info.AttributeInfo;
		}

        /*
		public int GetProtOffset()
		{
			switch ( m_Protection )
			{
				case ArmorProtectionLevel.Guarding: return 1;
				case ArmorProtectionLevel.Hardening: return 2;
				case ArmorProtectionLevel.Fortification: return 3;
				case ArmorProtectionLevel.Invulnerability: return 4;
			}

			return 0;
		}
        */

		public void UnscaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_HitPoints = ((m_HitPoints * 100) + (scale - 1)) / scale;
			m_MaxHitPoints = ((m_MaxHitPoints * 100) + (scale - 1)) / scale;
			InvalidateProperties();
		}

		public void ScaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_HitPoints = ((m_HitPoints * scale) + 99) / 100;
			m_MaxHitPoints = ((m_MaxHitPoints * scale) + 99) / 100;
			InvalidateProperties();
		}

		public int GetDurabilityBonus()
		{
			int bonus = 0;

			if ( Core.AOS && m_Quality == ArmorQuality.Exceptional ) // mod by Dies Irae
				bonus += 20;
            else
                bonus += OldScaleDurabilityByQuality();

			switch ( m_Durability )
			{
				case ArmorDurabilityLevel.Durable: bonus += 20; break;
				case ArmorDurabilityLevel.Substantial: bonus += 50; break;
				case ArmorDurabilityLevel.Massive: bonus += 70; break;
				case ArmorDurabilityLevel.Fortified: bonus += 100; break;
				case ArmorDurabilityLevel.Indestructible: bonus += 120; break;
			}

			if ( Core.AOS )
			{
				bonus += m_AosArmorAttributes.DurabilityBonus;

				#region Mondain's Legacy				
				//if ( m_Resource == CraftResource.Heartwood )
				//	return bonus;
				#endregion

				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
				CraftAttributeInfo attrInfo = null;

				if ( resInfo != null )
					attrInfo = resInfo.AttributeInfo;

				if ( attrInfo != null )
					bonus += attrInfo.ArmorDurability;
            }
            #region mod by Dies Irae
            else
                bonus += GetResourceAttrs().ArmorDurability;
            #endregion

			return bonus;
		}

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 502437 ); // Items you wish to cut must be in your backpack.
				return false;
			}

			if ( Ethics.Ethic.IsImbued( this ) )
			{
				from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
				return false;
			}

			CraftSystem system = DefTailoring.CraftSystem;

			CraftItem item = system.CraftItems.SearchFor( GetType() );

			if ( item != null && item.Resources.Count == 1 && item.Resources.GetAt( 0 ).Amount >= 2 )
			{
				try
				{
					Item res = (Item)Activator.CreateInstance( CraftResources.GetInfo( m_Resource ).ResourceTypes[0] );

					ScissorHelper( from, res, m_PlayerConstructed ? (item.Resources.GetAt( 0 ).Amount / 2) : 1 );
					return true;
				}
				catch
				{
				}
			}

			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		private static double[] m_ArmorScalars = { 0.07, 0.07, 0.14, 0.15, 0.22, 0.35 };
        
		public static double[] ArmorScalars
		{
			get
			{
                return Core.T2A ? m_OldArmorScalars : m_ArmorScalars; // mod by Dies Irae
			}
			set
			{
				m_ArmorScalars = value;
			}
		}

		public static void ValidateMobile( Mobile m )
		{
			for ( int i = m.Items.Count - 1; i >= 0; --i )
			{
				if ( i >= m.Items.Count )
					continue;

				Item item = m.Items[i];

				if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)item;

                    #region mod by Dies Irae
                    if( !armor.CanEquip( m ) )
                    {
                        m.AddToBackpack( armor );
                        continue;
                    }
				    #endregion

                    /* if( armor.RequiredRace != null && m.Race != armor.RequiredRace )
					{
						if( armor.RequiredRace == Race.Elf )
							m.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
						else
							m.SendMessage( "Only {0} may use this.", armor.RequiredRace.PluralName );

						m.AddToBackpack( armor );
					}
				    else */ if ( !armor.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( armor.AllowFemaleWearer )
							m.SendLocalizedMessage( 1010388 ); // Only females can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( armor );
					}
					else if ( !armor.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( armor.AllowMaleWearer )
							m.SendLocalizedMessage( 1063343 ); // Only males can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( armor );
					}
				}
			}
		}

		public int GetLowerStatReq()
		{
			if ( !Core.AOS )
				return 0;

			int v = m_AosArmorAttributes.LowerStatReq;

			#region Mondain's Legacy	
			//if ( m_Resource == CraftResource.Heartwood )
			//	return v;
			#endregion

			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

			if ( info != null )
			{
				CraftAttributeInfo attrInfo = info.AttributeInfo;

				if ( attrInfo != null )
					v += attrInfo.ArmorLowerRequirements;
			}

			if ( v > 100 )
				v = 100;

			return v;
		}

		public override void OnAdded( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( from );

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

				from.Delta( MobileDelta.Armor ); // Tell them armor rating has changed
			}

            OnPreAoSAdded( parent ); // mod by Dies Irae : pre-AoS stuff
		}

		public virtual double ScaleArmorByDurability( double armor )
		{
			int scale = 100;

			if ( m_MaxHitPoints > 0 && m_HitPoints < m_MaxHitPoints )
				scale = 50 + ((50 * m_HitPoints) / m_MaxHitPoints);

			return ( armor * scale ) / 100;
		}

		protected void Invalidate()
		{
			if ( Parent is Mobile )
				((Mobile)Parent).Delta( MobileDelta.Armor ); // Tell them armor rating has changed
		}

		public BaseArmor( Serial serial ) :  base( serial )
		{
		}

		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		[Flags]
		private enum SaveFlag
		{
			None				= 0x00000000,
			Attributes			= 0x00000001,
			ArmorAttributes		= 0x00000002,
			PhysicalBonus		= 0x00000004,
			FireBonus			= 0x00000008,
			ColdBonus			= 0x00000010,
			PoisonBonus			= 0x00000020,
			EnergyBonus			= 0x00000040,
			Identified			= 0x00000080,
			MaxHitPoints		= 0x00000100,
			HitPoints			= 0x00000200,
			Crafter				= 0x00000400,
			Quality				= 0x00000800,
			Durability			= 0x00001000,
			Protection			= 0x00002000,
			Resource			= 0x00004000,
			BaseArmor			= 0x00008000,
			StrBonus			= 0x00010000,
			DexBonus			= 0x00020000,
			IntBonus			= 0x00040000,
			StrReq				= 0x00080000,
			DexReq				= 0x00100000,
			IntReq				= 0x00200000,
			MedAllowance		= 0x00400000,
			SkillBonuses		= 0x00800000,
			PlayerConstructed	= 0x01000000,
		    EngravedText		= 0x02000000 // modifica by Dies Irae
		}

		#region Mondain's Legacy Sets		
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
			None				= 0x00000000,
			Attributes			= 0x00000001,
			ArmorAttributes		= 0x00000002,
			SkillBonuses		= 0x00000004,
			PhysicalBonus		= 0x00000008,
			FireBonus			= 0x00000010,
			ColdBonus			= 0x00000020,
			PoisonBonus			= 0x00000040,
			EnergyBonus			= 0x00000080,
			Hue					= 0x00000100,
			LastEquipped		= 0x00000200,
			SetEquipped			= 0x00000400,
			SetSelfRepair		= 0x00000800,
			UnSetHue			= 0x00001000, // modifica by Dies Irae
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 10 ); // version

		    MidgardSerialize( writer ); // mod by Dies Irae version 9

		    #region Mondain's Legacy Sets version 8
			SetFlag sflags = SetFlag.None;
			
			SetSaveFlag( ref sflags, SetFlag.Attributes,		!m_SetAttributes.IsEmpty );
			SetSaveFlag( ref sflags, SetFlag.SkillBonuses,		!m_SetSkillBonuses.IsEmpty );
			SetSaveFlag( ref sflags, SetFlag.PhysicalBonus,		m_SetPhysicalBonus != 0 );
			SetSaveFlag( ref sflags, SetFlag.FireBonus,			m_SetFireBonus != 0 );
			SetSaveFlag( ref sflags, SetFlag.ColdBonus,			m_SetColdBonus != 0 );
			SetSaveFlag( ref sflags, SetFlag.PoisonBonus,		m_SetPoisonBonus != 0 );
			SetSaveFlag( ref sflags, SetFlag.EnergyBonus,		m_SetEnergyBonus != 0 );
			SetSaveFlag( ref sflags, SetFlag.Hue,				m_SetHue != 0 );
			SetSaveFlag( ref sflags, SetFlag.LastEquipped,		m_LastEquipped );			
			SetSaveFlag( ref sflags, SetFlag.SetEquipped,		m_SetEquipped );
			SetSaveFlag( ref sflags, SetFlag.SetSelfRepair,		m_SetSelfRepair != 0 );
			SetSaveFlag( ref sflags, SetFlag.UnSetHue,			m_UnSetHue != 0 ); // modifica by Dies Irae

			writer.WriteEncodedInt( (int) sflags );
			
			if ( GetSaveFlag( sflags, SetFlag.Attributes ) )
				m_SetAttributes.Serialize( writer );

			if ( GetSaveFlag( sflags, SetFlag.SkillBonuses ) )
				m_SetSkillBonuses.Serialize( writer );

			if ( GetSaveFlag( sflags, SetFlag.PhysicalBonus ) )
				writer.WriteEncodedInt( m_SetPhysicalBonus );

			if ( GetSaveFlag( sflags, SetFlag.FireBonus ) )
				writer.WriteEncodedInt( m_SetFireBonus );

			if ( GetSaveFlag( sflags, SetFlag.ColdBonus ) )
				writer.WriteEncodedInt( m_SetColdBonus );

			if ( GetSaveFlag( sflags, SetFlag.PoisonBonus ) )
				writer.WriteEncodedInt( m_SetPoisonBonus );

			if ( GetSaveFlag( sflags, SetFlag.EnergyBonus ) )
				writer.WriteEncodedInt( m_SetEnergyBonus );
				
			if ( GetSaveFlag( sflags, SetFlag.Hue ) )
				writer.Write( m_SetHue );
				
			if ( GetSaveFlag( sflags, SetFlag.LastEquipped ) )
				writer.Write( m_LastEquipped );
				
			if ( GetSaveFlag( sflags, SetFlag.SetEquipped ) )
				writer.Write( m_SetEquipped );
				
			if ( GetSaveFlag( sflags, SetFlag.SetSelfRepair ) )
				writer.WriteEncodedInt( m_SetSelfRepair );

			#region modifica by Dies Irae
			if ( GetSaveFlag( sflags, SetFlag.UnSetHue ) )
				writer.WriteEncodedInt( m_UnSetHue );
			#endregion
			#endregion

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Attributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.ArmorAttributes,	!m_AosArmorAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PhysicalBonus,		m_PhysicalBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.FireBonus,			m_FireBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.ColdBonus,			m_ColdBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.PoisonBonus,		m_PoisonBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.EnergyBonus,		m_EnergyBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.Identified,		m_Identified != false );
			SetSaveFlag( ref flags, SaveFlag.MaxHitPoints,		m_MaxHitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.HitPoints,			m_HitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
			SetSaveFlag( ref flags, SaveFlag.Quality,			m_Quality != ArmorQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.Durability,		m_Durability != ArmorDurabilityLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Protection,		m_Protection != ArmorProtectionLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Resource,			m_Resource != DefaultResource );
			SetSaveFlag( ref flags, SaveFlag.BaseArmor,			m_ArmorBase != -1 );
			SetSaveFlag( ref flags, SaveFlag.StrBonus,			m_StrBonus != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexBonus,			m_DexBonus != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntBonus,			m_IntBonus != -1 );
			SetSaveFlag( ref flags, SaveFlag.StrReq,			m_StrReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexReq,			m_DexReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntReq,			m_IntReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.MedAllowance,		m_Meditate != (AMA)(-1) );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed != false );
		    SetSaveFlag( ref flags, SaveFlag.EngravedText, !String.IsNullOrEmpty( m_EngravedText ) ); // mod by Dies Irae

		    writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.ArmorAttributes ) )
				m_AosArmorAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.PhysicalBonus ) )
				writer.WriteEncodedInt( (int) m_PhysicalBonus );

			if ( GetSaveFlag( flags, SaveFlag.FireBonus ) )
				writer.WriteEncodedInt( (int) m_FireBonus );

			if ( GetSaveFlag( flags, SaveFlag.ColdBonus ) )
				writer.WriteEncodedInt( (int) m_ColdBonus );

			if ( GetSaveFlag( flags, SaveFlag.PoisonBonus ) )
				writer.WriteEncodedInt( (int) m_PoisonBonus );

			if ( GetSaveFlag( flags, SaveFlag.EnergyBonus ) )
				writer.WriteEncodedInt( (int) m_EnergyBonus );

			if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
				writer.WriteEncodedInt( (int) m_MaxHitPoints );

			if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
				writer.WriteEncodedInt( (int) m_HitPoints );

			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( (Mobile) m_Crafter );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.WriteEncodedInt( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.Durability ) )
				writer.WriteEncodedInt( (int) m_Durability );

			if ( GetSaveFlag( flags, SaveFlag.Protection ) )
				writer.WriteEncodedInt( (int) m_Protection );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.WriteEncodedInt( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.BaseArmor ) )
				writer.WriteEncodedInt( (int) m_ArmorBase );

			if ( GetSaveFlag( flags, SaveFlag.StrBonus ) )
				writer.WriteEncodedInt( (int) m_StrBonus );

			if ( GetSaveFlag( flags, SaveFlag.DexBonus ) )
				writer.WriteEncodedInt( (int) m_DexBonus );

			if ( GetSaveFlag( flags, SaveFlag.IntBonus ) )
				writer.WriteEncodedInt( (int) m_IntBonus );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.WriteEncodedInt( (int) m_StrReq );

			if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
				writer.WriteEncodedInt( (int) m_DexReq );

			if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
				writer.WriteEncodedInt( (int) m_IntReq );

			if ( GetSaveFlag( flags, SaveFlag.MedAllowance ) )
				writer.WriteEncodedInt( (int) m_Meditate );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );

            #region mod by Dies Irae
            if( GetSaveFlag( flags, SaveFlag.EngravedText ) )
                writer.Write( m_EngravedText );
            #endregion
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                #region mod by Dies Irae
                case 10: goto case 9;
                case 9:
                {
                    MidgardDeserialize( reader );
                    goto case 8;
                }
                #endregion
				#region Mondain's Legacy Sets
				case 8:
					SetFlag sflags = (SetFlag) reader.ReadEncodedInt();
					
					if ( GetSaveFlag( sflags, SetFlag.Attributes ) )
						m_SetAttributes = new AosAttributes( this, reader );
					else
						m_SetAttributes = new AosAttributes( this );
					
					if ( GetSaveFlag( sflags, SetFlag.ArmorAttributes ) )
						m_SetSelfRepair = (new AosArmorAttributes( this, reader )).SelfRepair;
						
					if ( GetSaveFlag( sflags, SetFlag.SkillBonuses ) )
						m_SetSkillBonuses = new AosSkillBonuses( this, reader );
					else
						m_SetSkillBonuses =  new AosSkillBonuses( this );

					if ( GetSaveFlag( sflags, SetFlag.PhysicalBonus ) )
						m_SetPhysicalBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( sflags, SetFlag.FireBonus ) )
						m_SetFireBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( sflags, SetFlag.ColdBonus ) )
						m_SetColdBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( sflags, SetFlag.PoisonBonus ) )
						m_SetPoisonBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( sflags, SetFlag.EnergyBonus ) )
						m_SetEnergyBonus = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( sflags, SetFlag.Hue ) )
						m_SetHue = reader.ReadInt();
						
					if ( GetSaveFlag( sflags, SetFlag.LastEquipped ) )
						m_LastEquipped = reader.ReadBool();
						
					if ( GetSaveFlag( sflags, SetFlag.SetEquipped ) )
						m_SetEquipped = reader.ReadBool();
						
					if ( GetSaveFlag( sflags, SetFlag.SetSelfRepair ) )
						m_SetSelfRepair = reader.ReadEncodedInt();

					#region modifica by Dies Irae
					if ( GetSaveFlag( sflags, SetFlag.UnSetHue ) )
						m_UnSetHue = reader.ReadEncodedInt();					
					#endregion
					
					goto case 7;
				#endregion
				case 7:
				case 6:
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.ArmorAttributes ) )
						m_AosArmorAttributes = new AosArmorAttributes( this, reader );
					else
						m_AosArmorAttributes = new AosArmorAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.PhysicalBonus ) )
						m_PhysicalBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.FireBonus ) )
						m_FireBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.ColdBonus ) )
						m_ColdBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.PoisonBonus ) )
						m_PoisonBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.EnergyBonus ) )
						m_EnergyBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Identified ) )
						m_Identified = ( version >= 7 || reader.ReadBool() );

					if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
						m_MaxHitPoints = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
						m_HitPoints = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
						m_Crafter = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.Quality ) )
						m_Quality = (ArmorQuality)reader.ReadEncodedInt();
					else
						m_Quality = ArmorQuality.Regular;

					if ( version == 5 && m_Quality == ArmorQuality.Low )
						m_Quality = ArmorQuality.Regular;

					if ( GetSaveFlag( flags, SaveFlag.Durability ) )
					{
						m_Durability = (ArmorDurabilityLevel)reader.ReadEncodedInt();

						if ( m_Durability > ArmorDurabilityLevel.Indestructible )
							m_Durability = ArmorDurabilityLevel.Durable;
					}

					if ( GetSaveFlag( flags, SaveFlag.Protection ) )
					{
						m_Protection = (ArmorProtectionLevel)reader.ReadEncodedInt();

						if ( m_Protection > ArmorProtectionLevel.Invulnerability )
							m_Protection = ArmorProtectionLevel.Defense;
					}

					if ( GetSaveFlag( flags, SaveFlag.Resource ) )
						m_Resource = (CraftResource)reader.ReadEncodedInt();
					else
						m_Resource = DefaultResource;

					if ( m_Resource == CraftResource.None )
						m_Resource = DefaultResource;

					if ( GetSaveFlag( flags, SaveFlag.BaseArmor ) )
						m_ArmorBase = reader.ReadEncodedInt();
					else
						m_ArmorBase = -1;

					if ( GetSaveFlag( flags, SaveFlag.StrBonus ) )
						m_StrBonus = reader.ReadEncodedInt();
					else
						m_StrBonus = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexBonus ) )
						m_DexBonus = reader.ReadEncodedInt();
					else
						m_DexBonus = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntBonus ) )
						m_IntBonus = reader.ReadEncodedInt();
					else
						m_IntBonus = -1;

					if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
						m_StrReq = reader.ReadEncodedInt();
					else
						m_StrReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
						m_DexReq = reader.ReadEncodedInt();
					else
						m_DexReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
						m_IntReq = reader.ReadEncodedInt();
					else
						m_IntReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.MedAllowance ) )
						m_Meditate = (AMA)reader.ReadEncodedInt();
					else
						m_Meditate = (AMA)(-1);

					if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );

					if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
						m_PlayerConstructed = true;

                    #region modifica by Dies Irae
                    if( GetSaveFlag( flags, SaveFlag.EngravedText ) )
                        m_EngravedText = reader.ReadString();

                    if( version < 10 )
                    {
                        // IsGuildUniform   = 0x04000000
                        // Guild			= 0x08000000
                        // GuildedOwner		= 0x10000000	
                        if( ( (int)flags & 0x04000000 ) != 0 )
                            IsGuildItem = reader.ReadBool();

                        if( ( (int)flags & 0x08000000 ) != 0 )
                            m_OwnerGuild = reader.ReadGuild();

                        if( ( (int)flags & 0x10000000 ) != 0 )
                            GuildedOwner = reader.ReadMobile();
                    }
				    #endregion
					break;
				}
				case 4:
				{
					m_AosAttributes = new AosAttributes( this, reader );
					m_AosArmorAttributes = new AosArmorAttributes( this, reader );
					goto case 3;
				}
				case 3:
				{
					m_PhysicalBonus = reader.ReadInt();
					m_FireBonus = reader.ReadInt();
					m_ColdBonus = reader.ReadInt();
					m_PoisonBonus = reader.ReadInt();
					m_EnergyBonus = reader.ReadInt();
					goto case 2;
				}
				case 2:
				case 1:
				{
					m_Identified = reader.ReadBool();
					goto case 0;
				}
				case 0:
				{
					m_ArmorBase = reader.ReadInt();
					m_MaxHitPoints = reader.ReadInt();
					m_HitPoints = reader.ReadInt();
					m_Crafter = reader.ReadMobile();
					m_Quality = (ArmorQuality)reader.ReadInt();
					m_Durability = (ArmorDurabilityLevel)reader.ReadInt();
					m_Protection = (ArmorProtectionLevel)reader.ReadInt();

					AMT mat = (AMT)reader.ReadInt();

					if ( m_ArmorBase == RevertArmorBase )
						m_ArmorBase = -1;

					/*m_BodyPos = (ArmorBodyType)*/reader.ReadInt();

					if ( version < 4 )
					{
						m_AosAttributes = new AosAttributes( this );
						m_AosArmorAttributes = new AosArmorAttributes( this );
					}

                    /*
					if ( version < 3 && m_Quality == ArmorQuality.Exceptional )
						DistributeBonuses( 6 );
                    */

					if ( version >= 2 )
					{
						m_Resource = (CraftResource)reader.ReadInt();
					}
					else
					{
						OreInfo info;

						switch ( reader.ReadInt() )
						{
							default:
							case 0: info = OreInfo.Iron; break;
							case 1: info = OreInfo.DullCopper; break;
							case 2: info = OreInfo.ShadowIron; break;
							case 3: info = OreInfo.Copper; break;
							case 4: info = OreInfo.Bronze; break;
							case 5: info = OreInfo.Gold; break;
							case 6: info = OreInfo.Agapite; break;
							case 7: info = OreInfo.Verite; break;
							case 8: info = OreInfo.Valorite; break;
						}

						m_Resource = CraftResources.GetFromOreInfo( info, mat );
					}

					m_StrBonus = reader.ReadInt();
					m_DexBonus = reader.ReadInt();
					m_IntBonus = reader.ReadInt();
					m_StrReq = reader.ReadInt();
					m_DexReq = reader.ReadInt();
					m_IntReq = reader.ReadInt();

					if ( m_StrBonus == OldStrBonus )
						m_StrBonus = -1;

					if ( m_DexBonus == OldDexBonus )
						m_DexBonus = -1;

					if ( m_IntBonus == OldIntBonus )
						m_IntBonus = -1;

					if ( m_StrReq == OldStrReq )
						m_StrReq = -1;

					if ( m_DexReq == OldDexReq )
						m_DexReq = -1;

					if ( m_IntReq == OldIntReq )
						m_IntReq = -1;

					m_Meditate = (AMA)reader.ReadInt();

					if ( m_Meditate == OldMedAllowance )
						m_Meditate = (AMA)(-1);

					if ( m_Resource == CraftResource.None )
					{
						if ( mat == ArmorMaterialType.Studded || mat == ArmorMaterialType.Leather )
							m_Resource = CraftResource.RegularLeather;
						else if ( mat == ArmorMaterialType.Spined )
							m_Resource = CraftResource.SpinedLeather;
						else if ( mat == ArmorMaterialType.Horned )
							m_Resource = CraftResource.HornedLeather;
						else if ( mat == ArmorMaterialType.Barbed )
							m_Resource = CraftResource.BarbedLeather;
						else
							m_Resource = CraftResource.Iron;
					}

					if ( m_MaxHitPoints == 0 && m_HitPoints == 0 )
						m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );

					break;
				}
			}

			#region Mondain's Legacy Sets
			if ( m_SetAttributes == null )
				m_SetAttributes = new AosAttributes( this );
				
			if ( m_SetSkillBonuses == null )
				m_SetSkillBonuses = new AosSkillBonuses( this );
			#endregion
			
			if ( m_AosSkillBonuses == null )
				m_AosSkillBonuses = new AosSkillBonuses( this );

			if ( Core.AOS && Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile)Parent );

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)Parent;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();

			if ( version < 7 )
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted

            #region mod by Dies Irae
            if( Core.AOS && ArtifactRarity == 0 && Resource == CraftResource.Aqua && Attributes.BonusDex == 3 )
                Attributes.BonusDex = 0;

            if( this is ITreasureOfMidgard )
                TreasuresOfMidgard.RegisterExistance( GetType() );
            #endregion
        }

        public virtual CraftResource DefaultResource{ get{ return CraftResource.Iron; } }

		public BaseArmor( int itemID ) :  base( itemID )
		{
			m_Quality = ArmorQuality.Regular;
			m_Durability = ArmorDurabilityLevel.Regular;
			m_Crafter = null;

			m_Resource = DefaultResource;
			Hue = CraftResources.GetHue( m_Resource );

			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );

			Layer = (Layer)ItemData.Quality;

			m_AosAttributes = new AosAttributes( this );
			m_AosArmorAttributes = new AosArmorAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );

			m_UnSetHue = Hue; // modifica by Dies Irae
			
			#region Mondain's Legacy Sets
			m_SetAttributes = new AosAttributes( this );
			m_SetSkillBonuses = new AosSkillBonuses( this );
			#endregion

            #region mod by Dies Irae
            m_MagicalCharges = 0;
            m_MagicalAttribute = ArmorMagicalAttribute.None;
            m_IdentifiersList = new List<Mobile>();

		    CustomQuality = Server.Quality.Standard; // starndard quality;

            if( this is ITreasureOfMidgard )
                TreasuresOfMidgard.RegisterExistance( GetType() );
            #endregion
		}

		public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			if ( !Ethics.Ethic.CheckTrade( from, to, newOwner, this ) )
				return false;

			return base.AllowSecureTrade( from, to, newOwner, accepted );
		}

		public virtual Race RequiredRace { get { return null; } }

		public override bool CanEquip( Mobile from )
		{
            #region mod by Dies Irae
            if( from.Player )
            {
                if( m_OwnerGuild != null && !GuildsHelper.CheckEquip( from, m_OwnerGuild, this ) )
                    return false;

                if( !RaceAllowanceAttribute.IsAllowed( this, from, Midgard.Engines.Races.Core.MountainDwarf, true ) )
                    return false;

                if( RequiredTownSystem != null && !TownHelper.CheckEquip( from, RequiredTownSystem, true ) )
                    return false;

                if( this is ITreasureOfMidgard && !XmlBlessedCursedAttach.CanEquip( from, this ) )
                    return false;

                if( RequiredRace != null && !MidgardRace.CheckEquip( RequiredRace, from, true ) )
                    return false;

                if( RequiredClass != Classes.None && !ClassSystem.CheckEquip( from, ClassSystem.Find( RequiredClass ), true ) )
                    return false;

                if( HasArmorOfType( from, 5, ArmorMaterialType.Dragon ) && BodyPosition == ArmorBodyType.Gorget )
                    return false;

                if( from.ChestArmor != null && from.ChestArmor is BarbarianChest && BodyPosition == ArmorBodyType.Arms )
                    return false;
            }

            /*
            if( from.Skills[ SkillName.Archery ].Base > 50 && MaterialType == ArmorMaterialType.Plate && BodyPosition != c.Shield && !( this is Circlet ) )
            {
                from.SendMessage( "You are an archer! You cannot wear this item." );
                return false;
            }

            if( m_AosArmorAttributes.MageArmor == 0 && BodyPosition != ArmorBodyType.Shield && from.Skills[ SkillName.Magery ].Base > 50 && ( MaterialType == ArmorMaterialType.Studded || MaterialType == ArmorMaterialType.Bone || MaterialType == ArmorMaterialType.Ringmail || MaterialType == ArmorMaterialType.Chainmail || MaterialType == ArmorMaterialType.Plate ) )
            {
                from.SendMessage( "Sei un mago! Non puoi indossare questa armatura." );
                return false;
            }

            if( m_AosArmorAttributes.MageArmor == 0 && from.Skills[ SkillName.Necromancy ].Base > 50 && ( MaterialType == ArmorMaterialType.Ringmail || MaterialType == ArmorMaterialType.Chainmail || MaterialType == ArmorMaterialType.Plate ) )
            {
                from.SendMessage( "Sei un necromante! Non puoi indossare questa armatura." );
                return false;
            }
            */
            #endregion

			if( !Ethics.Ethic.CheckEquip( from, this ) )
				return false;

			if( from.AccessLevel < AccessLevel.GameMaster )
            {
                /* if( RequiredRace != null && from.Race != RequiredRace )
                {
                    if( RequiredRace == Race.Elf )
                        from.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
                    else
                        from.SendMessage( "Only {0} may use this.", RequiredRace.PluralName );

                    return false;
                }
				else */ if( !AllowMaleWearer && !from.Female )
				{
					if( AllowFemaleWearer )
						from.SendLocalizedMessage( 1010388 ); // Only females can wear this.
					else
						from.SendMessage( "You may not wear this." );

					return false;
				}
				else if( !AllowFemaleWearer && from.Female )
				{
					if( AllowMaleWearer )
						from.SendLocalizedMessage( 1063343 ); // Only males can wear this.
					else
						from.SendMessage( "You may not wear this." );

					return false;
				}
				else
				{
					int strBonus = ComputeStatBonus( StatType.Str ), strReq = ComputeStatReq( StatType.Str );
					int dexBonus = ComputeStatBonus( StatType.Dex ), dexReq = ComputeStatReq( StatType.Dex );
					int intBonus = ComputeStatBonus( StatType.Int ), intReq = ComputeStatReq( StatType.Int );

					if( from.Dex < dexReq || (from.Dex + dexBonus) < 1 )
					{
						from.SendLocalizedMessage( 502077 ); // You do not have enough dexterity to equip this item.
						return false;
					}
					else if( from.Str < strReq || (from.Str + strBonus) < 1 )
					{
						from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
						return false;
					}
					else if( from.Int < intReq || (from.Int + intBonus) < 1 )
					{
						from.SendMessage( from.Language == "ITA" ? "Non sei abbastanza intelligente per indossarlo." : "You are not smart enough to equip that." );
						return false;
					}
				}
			}

		    return XmlAttach.CheckCanEquip( this, from ) && base.CanEquip( from ); // XmlAttachment check for CanEquip
		}

		public override bool CheckPropertyConfliction( Mobile m )
		{
			if ( base.CheckPropertyConfliction( m ) )
				return true;

			if ( Layer == Layer.Pants )
				return ( m.FindItemOnLayer( Layer.InnerLegs ) != null );

			if ( Layer == Layer.Shirt )
				return ( m.FindItemOnLayer( Layer.InnerTorso ) != null );

			return false;
		}

		public override bool OnEquip( Mobile from )
		{
			from.CheckStatTimers();

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( strBonus != 0 || dexBonus != 0 || intBonus != 0 )
			{
				string modName = Serial.ToString();

				if ( strBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

		    XmlAttach.CheckOnEquip( this, from ); // ARTEGORDONMOD XmlAttachment check for OnEquip

		    return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile m = (Mobile)parent;
				string modName = Serial.ToString();

				m.RemoveStatMod( modName + "Str" );
				m.RemoveStatMod( modName + "Dex" );
				m.RemoveStatMod( modName + "Int" );

				if ( Core.AOS )
					m_AosSkillBonuses.Remove();

				((Mobile)parent).Delta( MobileDelta.Armor ); // Tell them armor rating has changed
				m.CheckStatTimers();
								
				#region Mondain's Legacy Sets
				if ( IsSetItem && m_SetEquipped )
					SetHelper.RemoveSetBonus( m, SetID, this );
				#endregion

			    OnPreAoSRemoved( parent ); // mod by Dies Irae : pre-AoS stuff
			}

		    XmlAttach.CheckOnRemoved( this, parent ); // ARTEGORDONMOD XmlAttachment check for OnRemoved

		    base.OnRemoved( parent );
			
			InvalidateProperties();
		}
		bool OldPolRules = true;

		public virtual int OnHit( BaseWeapon weapon, int damageTaken )
		{
			// double halfAr = ArmorRating / 2.0;

			// int absorbed = (int)(halfAr + halfAr*Utility.RandomDouble());

			bool ranged = ( weapon != null && weapon.Type == WeaponType.Ranged );
			bool defenderPlayer = ( Parent != null && Parent is PlayerMobile );
			bool attackerPlayer = (weapon != null && weapon.Parent != null && weapon.Parent is PlayerMobile);
			//Mobile attacker = weapon.Parent as Mobile;
			bool pvp = defenderPlayer && attackerPlayer;
			Mobile parent = Parent as Mobile;
			//bool OldPolRules = (parent != null ? parent.PlayerDebug : false);
			int maxDamage = (int)((double)(100 - (parent.ArmorRating/5)));
			int resourceBonus = 0;

			CraftAttributeInfo info = GetResourceAttrs();
			if( info != null )
				resourceBonus = (int)((info.OldStaticMultiply*100) - 100);//1.00-1.28

			if ( resourceBonus < 0 )
				resourceBonus = 0;

			if ( CustomQuality == 1.3 ) //exc bonus
				maxDamage -= 10;

			maxDamage -= resourceBonus;

			if (damageTaken > maxDamage )
				damageTaken = maxDamage;

			if( parent != null && parent.PlayerDebug )
				parent.Say( "Cap danno: {0}", maxDamage);

			double scalar = OldPolRules ? 2.0 : 200.0;
			if( pvp )
			{
				scalar = ranged ? (OldPolRules ? 2.0 : 300.0) : (OldPolRules ? 2.0 : 400.0);//2.0,3.0//3.0,4.0

				if ( OldPolRules && weapon != null && weapon.Layer == Layer.OneHanded )
					scalar = 5.0;
			}

			if ( CustomQuality == 1.3 && weapon != null && weapon.Layer != Layer.OneHanded)
				scalar -= 1.0;

			double halfAr = OldPolRules ? ArmorRatingScaled / scalar : ArmorRating / scalar;

			int absorbed = OldPolRules ? (int)((double)(halfAr + ( halfAr * Utility.RandomDouble()))) : (int)( damageTaken * ( halfAr + ( halfAr * Utility.RandomDouble() ) ) );

			//if ( OldPolRules && weapon != null && (weapon.Type == WeaponType.Bashing || weapon is WarAxe) )
				//absorbed = (int)((double)(parent.ArmorRating/4 + halfAr + ( halfAr * Utility.RandomDouble())));

			//if( parent != null && parent.PlayerDebug )
			//parent.Say("HALFAR = {0} AR {1}/{2}", (int)halfAr, (int)ArmorRating, (int)(parent.ArmorRating/4)  );

			if( parent != null )// assorbimento dovuto a protection
			{
				string name = String.Format( "[Magic] {0} Offset", StatType.AR );
				StatMod mod = parent.GetStatMod( name );
				if( mod != null )
				{//protection + armor indossata. circa 30 AR
					if (damageTaken > 60 )//cap danno massimo
						damageTaken = 60;

					absorbed = (int)((double)(parent.ArmorRating/3 + halfAr*2));

					if ( weapon != null && weapon.Layer == Layer.OneHanded )
						absorbed = (int)((double)(parent.ArmorRating/5 + halfAr*2));

					//parent.Say( "PROTECTION+ARMOR absorbed {0} points of damage.", absorbed );
				}

				if ( parent.PlayerDebug )
					parent.SendMessage( "Your armor absorbed {0} of {1} points of damage.", absorbed, damageTaken );
			}

			damageTaken -= absorbed;
			if ( OldPolRules && weapon != null && (weapon.Type == WeaponType.Bashing || weapon is WarAxe) )
				absorbed += (int)((double)(parent.ArmorRating/4));

			damageTaken = Midgard.Engines.PVMAbsorbtions.Core.OnHitArmour(this,weapon,damageTaken); //mod by Magius(CHE)
			
			if ( damageTaken < 1 ) 
				damageTaken = 1;

            /*
			if ( absorbed < 2 )
				absorbed = 2;
            */

			if ( /*( Core.AOS ? 25 : 5 ) > Utility.Random( 100 )*/ ChanceofRuin > Utility.RandomDouble() && !IsGuildItem ) // 10% chance to lower durability
			{
				// Set self repair
				if ( Core.AOS && m_AosArmorAttributes.SelfRepair + ( IsSetItem && m_SetEquipped ? m_SetSelfRepair : 0 ) > Utility.Random( 10 ) )
				{
					HitPoints += 2;
				}
				else
				{
					int wear;

					if ( weapon.Type == WeaponType.Bashing || weapon is WarAxe ) // mod by Dies Irae
						wear = absorbed;
					else
						wear = Utility.Random( 2 ) + 1;

					if ( wear > 0 && m_MaxHitPoints > 0 )
					{
						if ( m_HitPoints >= wear )
						{
							HitPoints -= wear;
							wear = 0;
						}
						else
						{
							wear -= HitPoints;
							HitPoints = 0;
						}

						if ( wear > 0 )
						{
							if ( m_MaxHitPoints > wear )
							{
								MaxHitPoints -= wear;

								if ( parent != null )
									parent.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061121 ); // Your equipment is severely damaged.
							}
							else
							{
                                				if( parent != null )
                                				{
                                    					parent.SendMessage( parent.Language == "ITA" ? "Il tuo equipaggiamento è vecchio e logoro! Si è rovinato!" : "Your equipment is too old and ruined! It is broken!" );
                                    					parent.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "*Houch!!*" );
                                				}

								Delete();
							}
						}
					}
				}
			}

			return damageTaken;
		}

        /*
		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}
        */

		[Hue, CommandProperty( AccessLevel.GameMaster )]
		public override int Hue
		{
			get{ return base.Hue; }
			set{ base.Hue = value; InvalidateProperties(); }
		}

        /*
		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType;

			switch ( m_Resource )
			{
				case CraftResource.DullCopper:		oreType = 1053108; break; // dull copper
				case CraftResource.ShadowIron:		oreType = 1053107; break; // shadow iron
				case CraftResource.Copper:			oreType = 1053106; break; // copper
				case CraftResource.Bronze:			oreType = 1053105; break; // bronze
				case CraftResource.Gold:			oreType = 1053104; break; // golden
				case CraftResource.Agapite:			oreType = 1053103; break; // agapite
				case CraftResource.Verite:			oreType = 1053102; break; // verite
				case CraftResource.Valorite:		oreType = 1053101; break; // valorite

				#region modifica by Dies Irae
				case CraftResource.Platinum:		oreType = 1064690; break; // platinum
				case CraftResource.Titanium:		oreType = 1064691; break; // titanium
				case CraftResource.Obsidian:		oreType = 1064692; break; // obsidian
				case CraftResource.DarkRuby:		oreType = 1064693; break; // darkRuby
				case CraftResource.EbonSapphire:	oreType = 1064694; break; // ebon sapphire
				case CraftResource.RadiantDiamond:	oreType = 1064695; break; // radiant diamond
				case CraftResource.Eldar:			oreType = 1064696; break; // eldar
				case CraftResource.Crystaline:		oreType = 1064697; break; // crystaline
				case CraftResource.Vulcan:			oreType = 1064698; break; // vulcan
				case CraftResource.Aqua:			oreType = 1064699; break; // aqua

				case CraftResource.Oak:				oreType = 1065471; break; // oak
				case CraftResource.Walnut:			oreType = 1065472; break; // walnut
				case CraftResource.Ohii:			oreType = 1065473; break; // ohii
				case CraftResource.Cedar:			oreType = 1065474; break; // cedar
				case CraftResource.Willow:			oreType = 1065475; break; // willow
				case CraftResource.Cypress:			oreType = 1065476; break; // cypress
				case CraftResource.Yew:				oreType = 1065477; break; // yew
				case CraftResource.Apple:			oreType = 1065478; break; // apple
				case CraftResource.Pear:			oreType = 1065479; break; // pear
				case CraftResource.Peach:			oreType = 1065480; break; // peach
				case CraftResource.Banana:			oreType = 1065481; break; // banana
				case CraftResource.Stonewood:		oreType = 1065482; break; // stonewood
				case CraftResource.Silver:			oreType = 1065483; break; // silver
				case CraftResource.Blood:			oreType = 1065484; break; // blood
				case CraftResource.Swamp:			oreType = 1065485; break; // swamp
				case CraftResource.Crystal:			oreType = 1065486; break; // crystal
				case CraftResource.Elven:			oreType = 1065487; break; // elven
				case CraftResource.Elder:			oreType = 1065488; break; // elder
				case CraftResource.Enchanted:		oreType = 1065489; break; // enchanted
				case CraftResource.Death:			oreType = 1065490; break; // death

                case CraftResource.HumanoidLeather:		oreType = 1066200; break; // humanoid
                case CraftResource.UndeadLeather:		oreType = 1066201; break; // undead
                case CraftResource.WolfLeather:			oreType = 1066202; break; // wolf
                case CraftResource.AracnidLeather:		oreType = 1066203; break; // aracnid
                case CraftResource.FeyLeather:			oreType = 1066204; break; // fey
                case CraftResource.GreenDragonLeather:	oreType = 1066205; break; // green dragon
                case CraftResource.BlackDragonLeather:	oreType = 1066206; break; // black dragon
                case CraftResource.BlueDragonLeather:	oreType = 1066207; break; // blue dragon
                case CraftResource.RedDragonLeather:	oreType = 1066208; break; // red dragon
                case CraftResource.AbyssLeather:		oreType = 1066209; break; // abyss
				#endregion

				case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
				case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
				case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
				case CraftResource.RedScales:		oreType = 1060814; break; // red
				case CraftResource.YellowScales:	oreType = 1060818; break; // yellow
				case CraftResource.BlackScales:		oreType = 1060820; break; // black
				case CraftResource.GreenScales:		oreType = 1060819; break; // green
				case CraftResource.WhiteScales:		oreType = 1060821; break; // white
				case CraftResource.BlueScales:		oreType = 1060815; break; // blue

            	#region Mondain's Legacy
                //case CraftResource.OakWood:			oreType = 1072533; break; // oak
                //case CraftResource.AshWood:			oreType = 1072534; break; // ash
                //case CraftResource.YewWood:			oreType = 1072535; break; // yew
                //case CraftResource.Heartwood:		oreType = 1072536; break; // heartwood
                //case CraftResource.Bloodwood:		oreType = 1072538; break; // bloodwood
                //case CraftResource.Frostwood:		oreType = 1072539; break; // frostwood
				#endregion

				default: oreType = 0; break;
			}

			if ( m_Quality == ArmorQuality.Exceptional )
			{
				if ( oreType != 0 )
					list.Add( 1053100, "#{0}\t{1}", oreType, GetNameString() ); // exceptional ~1_oretype~ ~2_armortype~
				else
					list.Add( 1050040, GetNameString() ); // exceptional ~1_ITEMNAME~
			}
			else
			{
				if ( oreType != 0 )
					list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
				else if ( Name == null )
					list.Add( LabelNumber );
				else
					list.Add( Name );
			}

			#region modifica by Dies Irae
			if( m_EngravedText != null )
				list.Add( 1062613, m_EngravedText ); // "~1_NAME~"
			#endregion
		}
        */

        /*
		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
		}
        */

        /*
		public virtual int GetLuckBonus()
		{
			#region Mondain's Legacy
            //if ( m_Resource == CraftResource.Heartwood )
            //    return 0;
			#endregion

			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

			if ( resInfo == null )
				return 0;

			CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

			if ( attrInfo == null )
				return 0;

			return attrInfo.ArmorLuck;
		}
        */

        /*
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			#region modifica by Dies Irae
            //XmlStoneEnchantAttach a = XmlAttach.FindAttachment( this, typeof(XmlStoneEnchantAttach) ) as XmlStoneEnchantAttach;
            //if( a != null && !a.Deleted )
            //    a.AddStoneProperties( list );

			XmlHolyArmorAttach h = XmlAttach.FindAttachment( this, typeof(XmlHolyArmorAttach) ) as XmlHolyArmorAttach;
			if( h != null && !h.Deleted )
				h.AddHolyArmorProperties( list );

			if( Midgard.Engines.XmlForceIdentify.IsUnidentified( this ) )
			{
				Midgard.Engines.XmlForceIdentify.AddUnidentifiedProperties( list, this );
				return;
			}
			#endregion

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			#region Factions
			if ( m_FactionState != null )
				list.Add( 1041350 ); // faction item
			#endregion

			#region Mondain's Legacy Sets
			if ( IsSetItem )
			{
				if ( MixedSet )
					list.Add( 1073491, Pieces.ToString() ); // Part of a Weapon/Armor Set (~1_val~ pieces)
				else
					list.Add( 1072376, Pieces.ToString() ); // Part of an Armor Set (~1_val~ pieces)
					
				if ( m_SetEquipped )
				{
					if ( MixedSet )
						list.Add( 1073492 ); // Full Weapon/Armor Set Present
					else
						list.Add( 1072377 ); // Full Armor Set Present
				
					GetSetProperties( list );
				}
			}
			#endregion
			
			if( RequiredRace == Race.Elf )
				list.Add( 1075086 ); // Elves Only

			m_AosSkillBonuses.GetProperties( list );

			int prop;

			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~

			if ( (prop = (GetResourceAttrs().ArmorWeaponDamage + m_AosAttributes.WeaponDamage)) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorDefendChance + m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorBonusDex + m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = GetResourceAttrs().ArmorEnhancePotions + m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorCastRecovery + m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = GetResourceAttrs().ArmorCastSpeed + m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = (GetResourceAttrs().ArmorAttackChance + m_AosAttributes.AttackChance)) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorBonusHits + m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = GetResourceAttrs().ArmorBonusInt + m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = GetResourceAttrs().ArmorLowerManaCost + m_AosAttributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorLowerRegCost + m_AosAttributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%

			if ( (prop = GetLowerStatReq()) != 0 )
				list.Add( 1060435, prop.ToString() ); // lower requirements ~1_val~%

			if ( (prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = (GetResourceAttrs().ArmorMageArmor + m_AosArmorAttributes.MageArmor)) != 0 )
				list.Add( 1060437 ); // mage armor

			if ( (prop = GetResourceAttrs().ArmorBonusMana + m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = GetResourceAttrs().ArmorRegenMana + m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = GetResourceAttrs().ArmorNightSight + m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = GetResourceAttrs().ArmorReflectPhysical + m_AosAttributes.ReflectPhysical) != 0 )
					list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorBonusStam + m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = GetResourceAttrs().ArmorBonusHits + m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = GetResourceAttrs().ArmorSelfRepair + m_AosArmorAttributes.SelfRepair) != 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~

			if ( (prop = GetResourceAttrs().ArmorSpellChanneling + m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = GetResourceAttrs().ArmorSpellDamage + m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = GetResourceAttrs().ArmorBonusStam + m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = GetResourceAttrs().ArmorBonusStr + m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = GetResourceAttrs().ArmorWeaponSpeed + m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

			base.AddResistanceProperties( list );

			if ( (prop = GetDurabilityBonus()) > 0 )
				list.Add( 1060410, prop.ToString() ); // durability ~1_val~%

			if ( (prop = ComputeStatReq( StatType.Str )) > 0 )
				list.Add( 1061170, prop.ToString() ); // strength requirement ~1_val~

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~

		    XmlAttach.AddAttachmentProperties( this, list ); // ARTEGORDONMOD mod to display attachment properties

		    #region Mondain's Legacy Sets
			if ( IsSetItem && !m_SetEquipped )
			{
				list.Add( 1072378 ); // <br>Only when full set is present:				
				GetSetProperties( list );
			}
			#endregion
		}
        */

		public override void OnSingleClick( Mobile from )
		{
            OnOldSingleClick( from );

            /*
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

			if ( m_Quality == ArmorQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );

			if ( m_Identified || from.AccessLevel >= AccessLevel.GameMaster)
			{
				if ( m_Durability != ArmorDurabilityLevel.Regular )
					attrs.Add( new EquipInfoAttribute( 1038000 + (int)m_Durability ) );

				if ( m_Protection > ArmorProtectionLevel.Regular && m_Protection <= ArmorProtectionLevel.Invulnerability )
					attrs.Add( new EquipInfoAttribute( 1038005 + (int)m_Protection ) );
			}
			else if ( m_Durability != ArmorDurabilityLevel.Regular || (m_Protection > ArmorProtectionLevel.Regular && m_Protection <= ArmorProtectionLevel.Invulnerability) )
				attrs.Add( new EquipInfoAttribute( 1038000 ) ); // Unidentified

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
            */
		}

		#region ICraftable Members

        public virtual int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ArmorQuality)quality;

			if ( makersMark )
				Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

            /*
            if( Quality == ArmorQuality.Exceptional )
			{
				DistributeBonuses( (tool is BaseRunicTool ? 6 : Core.SE ? 15 : 14) ); // Not sure since when, but right now 15 points are added, not 14.

				if( Core.ML && !(this is BaseShield) )
				{
					int bonus = (int)(from.Skills.ArmsLore.Value / 20);

					for( int i = 0; i < bonus; i++ )
					{
						switch( Utility.Random( 5 ) )
						{
							case 0: m_PhysicalBonus++;	break;
							case 1: m_FireBonus++;		break;
							case 2: m_ColdBonus++;		break;
							case 3: m_EnergyBonus++;	break;
							case 4: m_PoisonBonus++;	break;
						}
					}

					from.CheckSkill( SkillName.ArmsLore, 0, 100 );
				}
			}

			if ( Core.AOS && tool is BaseRunicTool )
				((BaseRunicTool)tool).ApplyAttributesTo( this );
			*/

			/*
			#region Mondain's Legacy	
			if ( craftItem != null && !craftItem.ForceNonExceptional )
			{	
				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
	
				if ( resInfo == null )
					return quality;
	
				CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
	
				if ( attrInfo == null )
					return quality;		
					
				if ( m_Resource != CraftResource.Heartwood )
				{
					m_AosAttributes.WeaponDamage += attrInfo.ArmorDamage;
					m_AosAttributes.AttackChance += attrInfo.ArmorHitChance;
					m_AosAttributes.RegenHits += attrInfo.ArmorRegenHits;				
					m_AosArmorAttributes.MageArmor += attrInfo.ArmorMage;
				}
				else
				{
					switch ( Utility.Random( 5 ) )
					{
						case 0: m_AosAttributes.WeaponDamage += attrInfo.ArmorDamage; break;
						case 1: m_AosAttributes.AttackChance += attrInfo.ArmorHitChance; break;
						case 2: m_AosArmorAttributes.MageArmor += attrInfo.ArmorMage; break;			 
						case 3: m_AosAttributes.Luck += attrInfo.ArmorLuck; break;
						case 4: m_AosArmorAttributes.LowerStatReq += attrInfo.ArmorLowerRequirements; break;
					}
				}
			}
			#endregion
			*/

            #region mod by Dies Irae
            if( !Core.AOS )
                MutateHue();
            #endregion

			return quality;
		}

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed
        {
            get { return m_PlayerConstructed; }
            set { m_PlayerConstructed = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateSecondAgeNames(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
		#endregion
		
		#region Mondain's Legacy Sets
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
		public virtual bool MixedSet{ get{ return false; } }
		public virtual int Pieces{ get{ return 0; } }
		
		public bool IsSetItem{ get{ return ( SetID != SetItem.None ); } }
		
		private int m_SetHue;
		private bool m_SetEquipped;
		private bool m_LastEquipped;
		
		#region modifica by Dies Irae
		private int m_UnSetHue;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int UnSetHue
		{
			get { return m_UnSetHue; }
			set { m_UnSetHue = value; }
		}
		#endregion
		
		[CommandProperty( AccessLevel.GameMaster )]
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
		
		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes SetAttributes
		{
			get{ return m_SetAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SetSkillBonuses
		{
			get{ return m_SetSkillBonuses; }
			set{}
		}	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SetSelfRepair
		{
			get{ return m_SetSelfRepair; }
			set{ m_SetSelfRepair = value; InvalidateProperties(); }
		}
		
		private int m_SetPhysicalBonus, m_SetFireBonus, m_SetColdBonus, m_SetPoisonBonus, m_SetEnergyBonus;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SetPhysicalBonus
		{ 
			get{ return m_SetPhysicalBonus; } 
			set{ m_SetPhysicalBonus = value; InvalidateProperties(); } 
		}	
		
		[CommandProperty( AccessLevel.GameMaster )]	
		public int SetFireBonus
		{ 
			get{ return m_SetFireBonus; } 
			set{ m_SetFireBonus = value; InvalidateProperties(); } 
		}		
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SetColdBonus
		{ 
			get{ return m_SetColdBonus; } 
			set{ m_SetColdBonus = value; InvalidateProperties(); } 
		}		
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SetPoisonBonus
		{ 
			get{ return m_SetPoisonBonus; } 
			set{ m_SetPoisonBonus = value; InvalidateProperties(); } 
		}	
		
		[CommandProperty( AccessLevel.GameMaster )]	
		public int SetEnergyBonus
		{ 
			get{ return m_SetEnergyBonus; } 
			set{ m_SetEnergyBonus = value; InvalidateProperties(); } 
		}
		
		public virtual void GetSetProperties( ObjectPropertyList list )
		{						
			if ( !m_SetEquipped )	
			{
				if ( m_SetPhysicalBonus != 0 )
					list.Add( 1072382, m_SetPhysicalBonus.ToString() ); // physical resist +~1_val~%
					
				if ( m_SetFireBonus != 0 )
					list.Add( 1072383, m_SetFireBonus.ToString() ); // fire resist +~1_val~%
					
				if ( m_SetColdBonus != 0 )
					list.Add( 1072384, m_SetColdBonus.ToString() ); // cold resist +~1_val~%
					
				if ( m_SetPoisonBonus != 0 )
					list.Add( 1072385, m_SetPoisonBonus.ToString() ); // poison resist +~1_val~%
					
				if ( m_SetEnergyBonus != 0 )
					list.Add( 1072386, m_SetEnergyBonus.ToString() ); // energy resist +~1_val~%		
			}					

			SetHelper.GetSetProperties( list, this );

			int prop;	

			if ( (prop = m_SetSelfRepair) != 0 && m_AosArmorAttributes.SelfRepair == 0 )
				list.Add( 1060450, prop.ToString() ); // self repair ~1_val~          		
		}
		#endregion

        #region mod by Dies Irae [Third Crown]
        private static readonly int[] m_CopperHues = new int[] { 0x641, 0x642, 0x643, 0x644, 0x645, 0x646 };
        private static readonly int[] m_BronzeHues = new int[] { 0x465, 0x466, 0x467, 0x468, 0x469, 0x470 };
        private static readonly int[] m_ShadowHues = new int[] { 0x961, 0x962, 0x963, 0x964, 0x965, 0x966 };
        private static readonly int[] m_GoldenHues = new int[] { 0x8f7 };
        private static readonly int[] m_DullCopperHues = new int[] { 0x973, 0x974, 0x975, 0x976, 0x977, 0x978 };
        private static readonly int[] m_AgapiteHues = new int[] { 0x979, 0x97A, 0x97B, 0x97C, 0x97D, 0x97E };
        private static readonly int[] m_ValoriteHues = new int[] { 0x847, 0x848, 0x849, 0x84A, 0x84B, 0x84C };
        private static readonly int[] m_VeriteHues = new int[] { 0x8C5, 0x8C6, 0x8C7, 0x8C8 };

        private void MutateHue()
        {
            if( m_Resource < CraftResource.OldDullCopper || m_Resource > CraftResource.OldValorite )
                return;

            int[] hues = new int[] { 0 };
            switch( m_Resource )
            {
                case CraftResource.OldCopper:
                    hues = m_CopperHues;
                    break;
                case CraftResource.OldBronze:
                    hues = m_BronzeHues;
                    break;
                case CraftResource.OldShadowIron:
                    hues = m_ShadowHues;
                    break;
                case CraftResource.OldGold:
                    hues = m_GoldenHues;
                    break;
                case CraftResource.OldDullCopper:
                    hues = m_DullCopperHues;
                    break;
                case CraftResource.OldAgapite:
                    hues = m_AgapiteHues;
                    break;
                case CraftResource.OldValorite:
                    hues = m_ValoriteHues;
                    break;
                case CraftResource.OldVerite:
                    hues = m_VeriteHues;
                    break;
            }

            if( hues.Length > 0 )
                Hue = Utility.RandomList( hues );
        }

        public const double BaseChanceOfRuin = 0.15;
        public const double ArmsloreScalar = 2000.0;
        public const double QualityScalar = 2.0;
        protected double ChanceofRuin
        {
            get
            {
                Mobile parent = Parent as Mobile;
                if( parent == null )
                    return 0.0;

                double resourceBonus = 1.0;
                CraftAttributeInfo info = GetResourceAttrs();
                if( info != null )
                    resourceBonus = info.OldStaticMultiply; // from 1.00 to 1.28

                // from 0.15 to 0.10 at 100 of armslore
                double armsloreBonus = ( ( (int)parent.Skills[ SkillName.ArmsLore ].Value ) / ArmsloreScalar );

                // custom quality: from 0.70 to 1.30
                double qualityBonus = QualityScalar * CustomQuality;

                double chanceOfRuin = ( BaseChanceOfRuin - armsloreBonus ) / ( qualityBonus * resourceBonus );

                //if( parent.PlayerDebug )
                //{
                //    parent.SendMessage( "Debug armor ruin: chance {0:F2}% (armslore {1:F2} - quality {2:F2} - resource {3:F2}).",
                //        chanceOfRuin * 100, armsloreBonus, qualityBonus, resourceBonus );
                //}

                return chanceOfRuin;
            }
        }

        private string m_EngravedText;

        [CommandProperty( AccessLevel.GameMaster )]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set { m_EngravedText = value; InvalidateSecondAgeNames(); }
        }

        public virtual string OldInitHits { get { return "1d0+0"; } }

        public virtual TownSystem RequiredTownSystem { get { return null; } }

        public virtual Classes RequiredClass { get { return Classes.None; } }

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

        public void VerifyGuildItem()
        {
            if( IsGuildItem && OwnerGuild != null && GuildedOwner == null && OwnerGuild is Guild )
            {
                GuildsHelper.StringToLog( string.Format( "Warning: Guilditem for {0} with serial {1} without owner... deleting.", OwnerGuild.Name, Serial ) );
                GuildsHelper.UnRegisterUniform( null, (Guild)OwnerGuild, this );
                Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if( IsGuildItem && m_OwnerGuild != null && m_OwnerGuild is Guild )
                GuildsHelper.UnRegisterUniform( null, (Guild)m_OwnerGuild, this );

            base.OnAfterDelete();
        }
        #endregion

        #region [stone enchant system]
        private StoneEnchantItem m_StoneEnchantItemState;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public StoneEnchantItem StoneEnchantItemState
        {
            get { return m_StoneEnchantItemState; }
            set { m_StoneEnchantItemState = value; InvalidateSecondAgeNames(); }
        }

        public SkillName[] SkillsRequiredToEnchant
        {
            get
            {
                if( CraftResources.GetType( Resource ) == CraftResourceType.Metal )
                    return StoneEnchantItem.SkillsRequiredToEnchantMetal;
                else if( CraftResources.GetType( Resource ) == CraftResourceType.Leather )
                    return StoneEnchantItem.SkillsRequiredToEnchantLeather;

                return StoneEnchantItem.SkillsRequiredToEnchantWood;
            }
        }
        #endregion

        private ArmorMagicalAttribute m_MagicalAttribute;

        [CommandProperty( AccessLevel.GameMaster )]
        public ArmorMagicalAttribute MagicalAttribute
        {
            get { return m_MagicalAttribute; }
            set { m_MagicalAttribute = value; InvalidateSecondAgeNames(); }
        }

        private int m_MagicalCharges;

        [CommandProperty( AccessLevel.GameMaster )]
        public int MagicalCharges
        {
            get { return m_MagicalCharges; }
            set { m_MagicalCharges = value; InvalidateSecondAgeNames(); }
        }

        private void OnPreAoSAdded( object parent )
        {
            if( Core.AOS )
                return;

            if( parent is Mobile )
            {
                Mobile from = (Mobile)parent;

                if( Parent == from && from.Player )
                {
                    if( from.AccessLevel == AccessLevel.Player && SkillHandlers.Stealth.GetArmorRating( from ) >= 42 )
                    {
                        from.SendLocalizedMessage( 502727 ); // You could not hope to move quietly wearing this much armor.
                        from.RevealingAction( true );
                    }

                    if( from is PlayerMobile && ArmorBase > 0 )
                        ProtectionSpell.RemoveEffect( from, true );

                    if( m_MagicalAttribute != ArmorMagicalAttribute.None || this is BaseGlovesOfDexterity )
                    {
                        if( m_MagicalCharges > 0 )
                            OnPreAosUse( from );
                        else
                            from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
                    }

                    if( StoneEnchantItemState != null )
                        StoneEnchantItemState.Invalidate( from, true );
                }
            }
        }

        private void OnPreAoSRemoved( object parent )
        {
            if( Core.AOS )
                return;

            if( parent is Mobile )
            {
                Mobile m = (Mobile)parent;

                if( m_MagicalAttribute == ArmorMagicalAttribute.Invisibility && m.Hidden )
                {
                    Effects.SendLocationParticles( EffectItem.Create( new Point3D( m.X, m.Y, m.Z + 16 ), m.Map, EffectItem.DefaultDuration ), 0x376A, 10, 15, 5045 );
                    m.PlaySound( 0x3C4 );
                    m.Hidden = false;
                }

                if( StoneEnchantItemState != null )
                    StoneEnchantItemState.Invalidate( m, false );
            }
        }

        protected virtual void OnPreAosUse( Mobile from )
        {
            if( Deleted || m_MagicalCharges <= 0 || Parent != from )
                return;

            if( !from.CanBeginAction( typeof( BaseArmor ) ) )
            {
                from.SendMessage( "You cannot activate this item yet." );
                return;
            }

            if( DoPreAosMagicalEffect( from ) )
                ConsumeCharge( from );
        }

        private bool DoPreAosMagicalEffect( Mobile target )
        {
            if( m_MagicalAttribute != ArmorMagicalAttribute.None && m_MagicalCharges > 0 )
            {
                switch( m_MagicalAttribute )
                {
                    case ArmorMagicalAttribute.Clumsiness:
                        DoClumsiness( target );
                        return true;
                    case ArmorMagicalAttribute.Feeblemindedness:
                        DoFeeblemindedness( target );
                        return true;
                    case ArmorMagicalAttribute.Weakness:
                        DoWeakness( target );
                        return true;
                    case ArmorMagicalAttribute.Agility:
                        DoAgility( target );
                        return true;
                    case ArmorMagicalAttribute.Cunning:
                        DoCunning( target );
                        return true;
                    case ArmorMagicalAttribute.Strength:
                        DoStrength( target );
                        return true;
                    case ArmorMagicalAttribute.Curses:
                        DoCurses( target );
                        return true;
                    case ArmorMagicalAttribute.NightEyes:
                        DoNightEyes( target );
                        return true;
                    case ArmorMagicalAttribute.Blessings:
                        DoBlessings( target );
                        return true;
                    case ArmorMagicalAttribute.SpellReflection:
                        return DoSpellReflection( target );
                    case ArmorMagicalAttribute.Invisibility:
                        DoInvisibility( target );
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        protected virtual void ConsumeCharge( Mobile from )
        {
            --m_MagicalCharges;

            if( m_MagicalCharges <= 0 )
            {
                from.SendLocalizedMessage( 1019073 ); // This item is out of charges.
                m_MagicalAttribute = ArmorMagicalAttribute.None;
            }

            ApplyDelayTo( from );
        }

        private void ApplyDelayTo( Mobile from )
        {
            from.BeginAction( this );
            Timer.DelayCall( GetUseDelay, new TimerStateCallback( ReleaseArmorLock_Callback ), from );
        }

        private void ReleaseArmorLock_Callback( object state )
        {
            ( (Mobile)state ).EndAction( this );
        }

        private static TimeSpan GetUseDelay { get { return TimeSpan.FromSeconds( 4.0 ); } }

        #region [t2a effects]
        private static void DoClumsiness( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Dex );
            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Dex, name, mod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < -10 )
                from.AddStatMod( new StatMod( StatType.Dex, name, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.Paralyzed = false;
            from.FixedParticles( 0x3779, 10, 15, 5002, EffectLayer.Head );
            from.PlaySound( 0x1DF );
        }

        private static void DoFeeblemindedness( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Int );

            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Int, name, mod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Int, name, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x3779, 10, 15, 5004, EffectLayer.Head );
            from.PlaySound( 0x1E4 );
        }

        private static void DoWeakness( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Str );
            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Str, name, mod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Str, name, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x3779, 10, 15, 5009, EffectLayer.Waist );
            from.PlaySound( 0x1E6 );
        }

        private static void DoAgility( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Dex );
            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Dex, name, mod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Dex, name, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x375A, 10, 15, 5010, EffectLayer.Waist );
            from.PlaySound( 0x28E );
        }

        private static void DoCunning( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Int );

            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Int, name, mod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Int, name, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x375A, 10, 15, 5011, EffectLayer.Head );
            from.PlaySound( 0x1EB );
        }

        private static void DoStrength( Mobile from )
        {
            string name = String.Format( "[Magic] {0} Offset", StatType.Str );

            StatMod mod = from.GetStatMod( name );

            if( mod != null && mod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Str, name, mod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( mod == null || mod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Str, name, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x375A, 10, 15, 5017, EffectLayer.Waist );
            from.PlaySound( 0x1EE );
        }

        private static void DoCurses( Mobile from )
        {
            string nameS = String.Format( "[Magic] {0} Offset", StatType.Str );
            string nameD = String.Format( "[Magic] {0} Offset", StatType.Dex );
            string nameI = String.Format( "[Magic] {0} Offset", StatType.Int );
            StatMod strmod = from.GetStatMod( nameS );
            StatMod dexmod = from.GetStatMod( nameD );
            StatMod intmod = from.GetStatMod( nameI );

            if( strmod != null && strmod.Offset > 0 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, strmod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( strmod == null || strmod.Offset > 10 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            if( dexmod != null && dexmod.Offset > 0 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, dexmod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( dexmod == null || dexmod.Offset > 10 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            if( intmod != null && intmod.Offset > 0 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, intmod.Offset + -10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( intmod == null || intmod.Offset > 10 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, -10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
            from.PlaySound( 0x1EA );
        }

        private static void DoNightEyes( Mobile from )
        {
            new LightCycle.NightSightTimer( from ).Start();
            from.LightLevel = 26;

            from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
            from.PlaySound( 0x1E3 );
        }

        private static void DoBlessings( Mobile from )
        {
            string nameS = String.Format( "[Magic] {0} Offset", StatType.Str );
            string nameD = String.Format( "[Magic] {0} Offset", StatType.Dex );
            string nameI = String.Format( "[Magic] {0} Offset", StatType.Int );
            StatMod strmod = from.GetStatMod( nameS );
            StatMod dexmod = from.GetStatMod( nameD );
            StatMod intmod = from.GetStatMod( nameI );

            if( strmod != null && strmod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, strmod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( strmod == null || strmod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Str, nameS, 10, TimeSpan.FromSeconds( 60.0 ) ) );
            if( dexmod != null && dexmod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, dexmod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( dexmod == null || dexmod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Dex, nameD, 10, TimeSpan.FromSeconds( 60.0 ) ) );
            if( intmod != null && intmod.Offset < 0 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, intmod.Offset + 10, TimeSpan.FromSeconds( 60.0 ) ) );
            else if( intmod == null || intmod.Offset < 10 )
                from.AddStatMod( new StatMod( StatType.Int, nameI, 10, TimeSpan.FromSeconds( 60.0 ) ) );

            from.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
            from.PlaySound( 0x1EA );
        }

        private static bool DoSpellReflection( Mobile from )
        {
            if( from.MagicDamageAbsorb > 0 )
                from.SendMessage( "This spell is already in effect." );
            else if( !from.CanBeginAction( typeof( DefensiveSpell ) ) )
                from.SendMessage( "The spell will not adhere to you at this time." );
            else
            {
                from.MagicDamageAbsorb = 1;

                from.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist );
                from.PlaySound( 0x1E9 );
                return true;
            }

            return false;
        }

        private static void DoInvisibility( Mobile from )
        {
            Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z + 16 ), from.Map, EffectItem.DefaultDuration ), 0x376A, 10, 15, 5045 );
            from.PlaySound( 0x3C4 );
            from.Hidden = true;
        }
        #endregion

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsXmlHolyArmor
        {
            get { return XmlAttach.FindAttachment( this, typeof( XmlHolyItemAttach ) ) != null; }
        }

        [CommandProperty( AccessLevel.Developer )]
        public bool IsMetalMade
        {
            get
            {
                return MaterialType == ArmorMaterialType.Ringmail ||
                         MaterialType == ArmorMaterialType.Chainmail ||
                         MaterialType == ArmorMaterialType.Plate;
            }
        }

        private static readonly double[] m_OldArmorScalars = { 0.07, 0.07, 0.14, 0.14, 0.14, 0.44 };

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


        public void DoAreaAttack( Mobile from, Mobile defender, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy )
        {
            Map map = from.Map;

            if( map == null )
                return;

            List<Mobile> list = new List<Mobile>();

            foreach( Mobile m in from.GetMobilesInRange( 10 ) )
            {
                if( from != m && defender != m && SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false ) && ( !Core.ML || from.InLOS( m ) ) )
                    list.Add( m );
            }

            if( list.Count == 0 )
                return;

            Effects.PlaySound( from.Location, map, sound );

            foreach( Mobile m in list )
            {
                double scalar = ( 11 - from.GetDistanceToSqrt( m ) ) / 10;

                if( scalar > 1.0 )
                    scalar = 1.0;
                else if( scalar < 0.0 )
                    continue;

                from.DoHarmful( m, true );
                m.FixedEffect( 0x3779, 1, 15, hue, 0 );
                AOS.Damage( m, from, (int)( 10 * scalar ), phys, fire, cold, pois, nrgy );
            }
        }


        private static bool HasArmorOfType( Mobile from, int pieces, ArmorMaterialType type )
        {
            int equipped = 0;

            for( int i = 0; i < from.Items.Count && equipped < pieces; i++ )
            {
                if( !( from.Items[ i ] is BaseArmor ) )
                    continue;

                if( ( (BaseArmor)from.Items[ i ] ).MaterialType == type )
                    equipped++;
            }

            if( equipped >= pieces )
                return true;

            if( from.PlayerDebug )
                from.SendMessage( "Debug: type {0} pieces -> {1}", type, pieces );

            return false;
        }

        private int ComputeOldMaterialDexMalus( int materialDexMalus )
        {
            if( materialDexMalus >= 0 || this is BronzeShield || ( MaterialType != ArmorMaterialType.Plate && MaterialType != ArmorMaterialType.Chainmail && MaterialType != ArmorMaterialType.Dragon ) )
                return 0;

            int dummy = Math.Abs( materialDexMalus );
            int bonus = 0;

            while( dummy > 0 )
            {
                if( dummy >= 1 && BodyPosition == ArmorBodyType.Gorget )
                    bonus++;
                else if( dummy >= 2 && BodyPosition == ArmorBodyType.Chest )
                    bonus++;
                else if( dummy >= 3 && BodyPosition == ArmorBodyType.Legs )
                    bonus++;
                else if( dummy >= 4 && BodyPosition == ArmorBodyType.Arms )
                    bonus++;
                else if( dummy >= 5 && BodyPosition == ArmorBodyType.Helmet )
                    bonus++;
                else if( dummy >= 6 && BodyPosition == ArmorBodyType.Gloves )
                    bonus++;

                dummy -= 6;
            }

            return bonus;
        }

        private int OldScaleDurabilityByQuality()
        {
            if( Quality == ArmorQuality.Exceptional )
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

        [CommandProperty( AccessLevel.Administrator )]
        public override double CustomQuality
        {
            get { return base.CustomQuality; }
            set
            {
                base.CustomQuality = value;
                Invalidate();
                InvalidateSecondAgeNames();
            }
        }

        private string m_SecondAgeFullName;
        private string m_SecondAgeUnIdentifiedName;

        public virtual void InvalidateSecondAgeNames()
        {
            BuildSecondAgeNames( string.IsNullOrEmpty( Name ) ? StringList.Localization[ LabelNumber ] : Name, "ITA" );
        }

        public virtual void BuildMagicalSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();
		if (language == "ITA")
		{
			// 'plate arms'
			builder.Append( rawName + " " );

			// 'exceptional '
			if( Quality == ArmorQuality.Exceptional )
				builder.Append( "eccezionale " );

			// 'fortified '
			string dur = DurabilityLevelName;
			if( dur.Length > 0 )
				builder.AppendFormat( "{0} ", dur );

			// 'radiand diamond '
			string material = MaterialName;
			if( material.Length > 0 )
				builder.AppendFormat( "{0} ", material );

			// 'cloud '
			if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
				builder.AppendFormat( "{0}", StoneEnchantItemState.Definition.Prefix );
		}
		else
		{
			// 'exceptional '
			if( Quality == ArmorQuality.Exceptional )
				builder.Append( "exceptional " );

			// 'fortified '
			string dur = DurabilityLevelName;
			if( dur.Length > 0 )
				builder.AppendFormat( "{0} ", dur );

			// 'cloud '
			if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
				builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );

			// 'radiand diamond '
			string material = MaterialName;
			if( material.Length > 0 )
				builder.AppendFormat( "{0} ", material );

			// 'plate arms'
			builder.Append( rawName );
		}
            // ' of hardening and clumsiness'
            string pro = ProtectionLevelName;
            string mag = MagicalEffectName;
            if( pro.Length > 0 || mag.Length > 0 )
            {
                if( pro.Length > 0 && mag.Length > 0 )
                    builder.AppendFormat( " of {0} and {1}", pro, mag );
                else if( pro.Length > 0 )
                    builder.AppendFormat( " of {0}", pro );
                else
                    builder.AppendFormat( " of {0}", mag );

                if( m_MagicalAttribute != ArmorMagicalAttribute.None && m_MagicalCharges > 0 )
                    builder.AppendFormat( language == "ITA" ? " con {0} cariche" : " with {0} charges", m_MagicalCharges );
            }

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( IsMetalMade ? (language == "ITA" ? " [forgiato da {0}]" : " [forged by {0}]") : (language == "ITA" ? " [creato da {0}]" : " [crafted by {0}]"), StringUtility.Capitalize( Crafter.Name ) );

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
		if (language == "ITA")
		{
			// 'plate arms'
			builder.Append( rawName+" " );

			// 'exceptional '
			if( Quality == ArmorQuality.Exceptional )
				builder.Append( "eccezionale " );

			// 'radiand diamond '
			if( !CraftResources.IsStandard( m_Resource ) )
				builder.AppendFormat( "{0} ", MaterialName );

			// 'cloud '
			if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
				builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );
		}
		else
		{
			// 'exceptional '
			if( Quality == ArmorQuality.Exceptional )
				builder.Append( "exceptional " );

			// 'cloud '
			if( StoneEnchantItemState != null && StoneEnchantItemState.Definition != null )
				builder.AppendFormat( "{0} ", StoneEnchantItemState.Definition.Prefix );

			// 'radiand diamond '
			if( !CraftResources.IsStandard( m_Resource ) )
				builder.AppendFormat( "{0} ", MaterialName );

			// 'plate arms'
			builder.Append( rawName );
		}

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( IsMetalMade ? (language == "ITA" ? " [forgiato da {0}]" : " [forged by {0}]") : (language == "ITA" ? " [creato da {0}]" : " [crafted by {0}]") , StringUtility.Capitalize( Crafter.Name ) );

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

        public void OnOldSingleClick( Mobile from )
        {
            if( Deleted || !from.CanSee( this ) )
                return;

            if( IsXmlHolyArmor )
            {
                LabelTo( from, "the Holy armor of Virtues" );
                return;
            }

            string name = StringList.GetClilocString( Name, LabelNumber, from.Language );

            if( m_SecondAgeFullName == null || m_SecondAgeUnIdentifiedName == null )
                BuildSecondAgeNames( name, from.Language );

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


		//considerazioni qualità, Arlas
		string messageq = from.Language == "ITA" ? "E' luccicante.": "It's in perfect condition.";
		if ( m_MaxHitPoints > 0 )
		{
			if ( ((double)m_HitPoints/(double)m_MaxHitPoints) <= 0.2 )
				messageq = from.Language == "ITA" ? "Sembra uno scolapasta.": "It's rubbish.";
			else if ( ((double)m_HitPoints/(double)m_MaxHitPoints) <= 0.5 )
				messageq = from.Language == "ITA" ? "Ha ammaccature ovunque.": "It needs repair, obviously.";
			else if ( ((double)m_HitPoints/(double)m_MaxHitPoints) <= 0.8 )
				messageq = from.Language == "ITA" ? "E' in buone condizioni.": "It is pretty good.";
		}
		from.PrivateOverheadMessage( MessageType.Regular, 0x3B2, true, messageq, from.NetState );
        }

        private double ScaleArmorByQuality( double armor )
        {
            if( Quality == ArmorQuality.Exceptional )
                return armor + 8;

            if( CustomQuality == Server.Quality.Undefined )
                return armor;
            else if( CustomQuality <= Server.Quality.VeryLow )
                return armor - 8;
            else if( CustomQuality <= Server.Quality.Low )
                return armor - 4;
            else if( CustomQuality <= Server.Quality.Decent )
                return armor - 2;
            else if( CustomQuality <= Server.Quality.BelowNormal )
                return armor;
            else if( CustomQuality <= Server.Quality.Standard )
                return armor + 2;
            else if( CustomQuality <= Server.Quality.Superior )
                return armor + 4;
            else if( CustomQuality <= Server.Quality.Great )
                return armor + 6;
            else
                return armor + 8;
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

        [CommandProperty( AccessLevel.Developer )]
        public virtual bool IsMagical
        {
            get
            {
                return ( Durability != ArmorDurabilityLevel.Regular ||
                        ProtectionLevel > ArmorProtectionLevel.Regular ||
                        ( MagicalAttribute != ArmorMagicalAttribute.None && MagicalCharges > 0 ) );
            }
        }

        [CommandProperty( AccessLevel.Developer )]
        public string DurabilityLevelName
        {
            get
            {
                if( Durability == ArmorDurabilityLevel.Regular )
                    return string.Empty;

                string name = Enum.GetName( typeof( ArmorDurabilityLevel ), Durability );

                return name.ToLower();
            }
        }

        [CommandProperty( AccessLevel.Developer )]
        public string ProtectionLevelName
        {
            get
            {
                if( ProtectionLevel == ArmorProtectionLevel.Regular )
                    return string.Empty;

                string name = Enum.GetName( typeof( ArmorProtectionLevel ), ProtectionLevel );

                return name.ToLower();
            }
        }

        [CommandProperty( AccessLevel.Developer )]
        public string MaterialName
        {
            get
            {
                if( CraftResources.IsStandard( m_Resource ) )
                    return string.Empty;

                return CraftResources.GetName( m_Resource ).ToLower();
            }
        }

        [CommandProperty( AccessLevel.Developer )]
        public string MagicalEffectName
        {
            get
            {
                switch( MagicalAttribute )
                {
                    case ArmorMagicalAttribute.Clumsiness:
                    case ArmorMagicalAttribute.Feeblemindedness:
                    case ArmorMagicalAttribute.Weakness:
                    case ArmorMagicalAttribute.Agility:
                    case ArmorMagicalAttribute.Cunning:
                    case ArmorMagicalAttribute.Strength:
                    case ArmorMagicalAttribute.Curses:
                    case ArmorMagicalAttribute.Invisibility:
                    case ArmorMagicalAttribute.Blessings:
                        return MagicalAttribute.ToString().ToLower();
                    case ArmorMagicalAttribute.NightEyes:
                        return "night eyes";
                    case ArmorMagicalAttribute.SpellReflection:
                        return "spell reflection";
                    default:
                        return "";
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsStoneEnchanted
        {
            get { return StoneEnchantHelper.IsEnchanted( this ); }
        }

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
            None                = 0x00000000,

            MagicEffect         = 0x00000001,
            MagicCharges        = 0x00000002,
            ItemIDList          = 0x00000004,
            StoneEnchanted      = 0x00000008,

            IsGuildUniform		= 0x00000010,
			Guild				= 0x00000020,
			GuildedOwner		= 0x00000040
        }

        private void MidgardSerialize( GenericWriter writer )
        {
            MidgardFlag flags = MidgardFlag.None;

            SetSaveFlag( ref flags, MidgardFlag.MagicEffect, m_MagicalAttribute != ArmorMagicalAttribute.None );
            SetSaveFlag( ref flags, MidgardFlag.MagicCharges, m_MagicalCharges != 0 );
            SetSaveFlag( ref flags, MidgardFlag.ItemIDList, m_IdentifiersList != null && m_IdentifiersList.Count > 0 );
            SetSaveFlag( ref flags, MidgardFlag.StoneEnchanted, m_StoneEnchantItemState != null );
            SetSaveFlag( ref flags, MidgardFlag.IsGuildUniform, IsGuildItem );
            SetSaveFlag( ref flags, MidgardFlag.Guild, m_OwnerGuild != null );
            SetSaveFlag( ref flags, MidgardFlag.GuildedOwner, GuildedOwner != null );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, MidgardFlag.MagicEffect ) )
                writer.Write( (int)m_MagicalAttribute );

            if( GetSaveFlag( flags, MidgardFlag.MagicCharges ) )
                writer.Write( m_MagicalCharges );

            if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
                writer.Write( m_IdentifiersList, true );

            if( GetSaveFlag( flags, MidgardFlag.StoneEnchanted ) && m_StoneEnchantItemState != null )
                m_StoneEnchantItemState.Serialize( writer );

            if( GetSaveFlag( flags, MidgardFlag.IsGuildUniform ) )
                writer.Write( IsGuildItem );

            if( GetSaveFlag( flags, MidgardFlag.Guild ) )
                writer.Write( m_OwnerGuild );

            if( GetSaveFlag( flags, MidgardFlag.GuildedOwner ) )
                writer.Write( GuildedOwner );
        }

        private void MidgardDeserialize( GenericReader reader )
        {
            MidgardFlag flags = (MidgardFlag)reader.ReadEncodedInt();

            if( GetSaveFlag( flags, MidgardFlag.MagicEffect ) )
                m_MagicalAttribute = (ArmorMagicalAttribute)reader.ReadInt();

            if( GetSaveFlag( flags, MidgardFlag.MagicCharges ) )
                m_MagicalCharges = reader.ReadInt();

            if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
                m_IdentifiersList = reader.ReadStrongMobileList();

            if( GetSaveFlag( flags, MidgardFlag.StoneEnchanted ) )
                m_StoneEnchantItemState = new StoneEnchantItem( reader );

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
                {
                    identificable.AddIdentifier( mobile );
                }
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

            if( Core.Debug )
            {
                Utility.Log( "BaseArmorAddIdentifier.log", "IdentifiersList for armor {0}", Serial.ToString() );
                foreach( Mobile m in m_IdentifiersList )
                    Utility.Log( "BaseArmorAddIdentifier.log", m.Name );
            }
        }

        public bool IsIdentifiedFor( Mobile from )
        {
            if( Crafter != null && from == Crafter )
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
        public bool Repair( Mobile from, BaseTool tool )
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

            if( system.CraftItems.SearchForSubclass( GetType() ) == null )
            {
                number = 1044277; // That item cannot be repaired.
            }
            else if( !IsChildOf( from.Backpack ) )
            {
                number = 1044275; // The item must be in your backpack to repair it.
            }
            else if( MaxHitPoints <= 0 || HitPoints == MaxHitPoints )
            {
                number = 1044281; // That item is in full repair
            }
            else if( MaxHitPoints <= toWeaken )
            {
                number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
            }
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
        #endregion

        #region ISmeltable members
        public bool Resmelt( Mobile from )
        {
            try
            {
                if( CraftResources.GetType( Resource ) != CraftResourceType.Metal )
                    return false;

                CraftResourceInfo info = CraftResources.GetInfo( Resource );

                if( info == null || info.ResourceTypes.Length == 0 )
                    return false;

                CraftItem craftItem = DefBlacksmithy.CraftSystem.CraftItems.SearchFor( GetType() );

                if( craftItem == null || craftItem.Resources.Count == 0 )
                    return false;

                CraftRes craftResource = craftItem.Resources.GetAt( 0 );

                if( craftResource.Amount < 2 )
                    return false; // Not enough metal to resmelt

                Type resourceType = info.ResourceTypes[ 0 ];
                Item ingot = (Item)Activator.CreateInstance( resourceType );

                if( PlayerConstructed )
                    ingot.Amount = craftResource.Amount / 2;
                else
                    ingot.Amount = 1;

                Delete();
                from.AddToBackpack( ingot );

                from.SendMessage( "You melt the item down into ingots." );
                from.PlaySound( 0x2A );
                from.PlaySound( 0x240 );
                return true;
            }
            catch
            {
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

	        if( CraftResources.GetType( Resource ) == CraftResourceType.Scales )
	        {
	            from.SendMessage( "You recycled this item into scales." );
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
        #endregion
	}
}