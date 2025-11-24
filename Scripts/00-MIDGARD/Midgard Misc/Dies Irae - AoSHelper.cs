/***************************************************************************
 *                                  AoSHelper.cs
 *                            		------------
 *  begin                	: Settembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			File di supporto per le modifiche di Midgard ad Aos.
 * 			Se devono essere aggiunte modifiche, farlo qui senza
 * 			intaccare AoS.cs.
 * 
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard
{
	public class AoSHelper
	{
		/// <summary>
		/// Static. Check if 6 pieces of same metal are equipped on m.
		/// </summary>
		/// <param name="m">PlayerMobile to check.</param>
        /// <returns>true if full metal armor is present</returns>
		public static bool IsFullMetalArmorSet( PlayerMobile m )
		{
		    CraftResource resource = CraftResource.None;
			bool onlyOneResource = true;
			int counter =  1;

			for( int i = 0; i < m.Items.Count && counter < 6 && onlyOneResource; i++ )
			{
				if( m.Items[ i ] is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)m.Items[ i ];
					if( resource == CraftResource.None )
						resource = armor.Resource;
					else if( armor.Resource != resource )
						onlyOneResource = false;
					else
						counter++;
				}
			}

			return onlyOneResource && counter >= 6;
		}
		
		/// <summary>
		/// Static. Calculate bonus on BaseArmor item for a particular AosAttribute
		/// </summary>
		/// <param name="armor">BaseArmor to check</param>
		/// <param name="attribute">AosAttribute we want the value</param>
        /// <returns>value for attribute on armor</returns>
		public static int GetResourceBonus( BaseArmor armor, AosAttribute attribute )
		{
			if( armor == null || armor.Deleted )
				return 0;

			CraftResourceInfo resInfo = CraftResources.GetInfo( armor.Resource );
			if( resInfo == null )
				return 0;

			CraftAttributeInfo attInfo = resInfo.AttributeInfo;
			if( attInfo == null )
				return 0;

			int value = 0;

			switch( attribute )
			{
				case AosAttribute.AttackChance:			value += attInfo.ArmorAttackChance; break;
				case AosAttribute.BonusDex:				value += attInfo.ArmorBonusDex; break;
				case AosAttribute.BonusHits:			value += attInfo.ArmorBonusHits; break;
				case AosAttribute.BonusInt:				value += attInfo.ArmorBonusInt; break;
				case AosAttribute.BonusMana:			value += attInfo.ArmorBonusMana; break;
				case AosAttribute.BonusStam:			value += attInfo.ArmorBonusStam; break;
				case AosAttribute.BonusStr:				value += attInfo.ArmorBonusStr; break;
				case AosAttribute.CastRecovery:			value += attInfo.ArmorCastRecovery; break;
				case AosAttribute.CastSpeed:			value += attInfo.ArmorCastSpeed; break;
				case AosAttribute.DefendChance:			value += attInfo.ArmorDefendChance; break;
				case AosAttribute.EnhancePotions:		value += attInfo.ArmorEnhancePotions; break;
				case AosAttribute.LowerManaCost:		value += attInfo.ArmorLowerManaCost; break;
				case AosAttribute.LowerRegCost:			value += attInfo.ArmorLowerRegCost; break;
				case AosAttribute.NightSight:			value += attInfo.ArmorNightSight; break;
				case AosAttribute.ReflectPhysical:		value += attInfo.ArmorReflectPhysical; break;
				case AosAttribute.RegenHits:			value += attInfo.ArmorRegenHits; break;
				case AosAttribute.RegenMana:			value += attInfo.ArmorRegenMana; break;
				case AosAttribute.RegenStam:			value += attInfo.ArmorRegenStam; break;
				case AosAttribute.SpellChanneling:		value += attInfo.ArmorSpellChanneling; break;
				case AosAttribute.SpellDamage:			value += attInfo.ArmorSpellDamage; break;
				case AosAttribute.WeaponDamage:			value += attInfo.ArmorWeaponDamage; break;
				case AosAttribute.WeaponSpeed:			value += attInfo.ArmorWeaponSpeed; break;
				default: break;
			}

			return value;
		}

		/// <summary>
		/// Static. Calculate bonus on BaseWeapon item for a particular AosAttribute
		/// </summary>
		/// <param name="weapon">our weapon</param>
		/// <param name="attribute">AosAttribute we want the value</param>
        /// <returns>value for attribute on our weapon</returns>
		public static int GetResourceBonus( BaseWeapon weapon, AosAttribute attribute )
		{
			if( weapon == null || weapon.Deleted )
				return 0;

			CraftResourceInfo resInfo = CraftResources.GetInfo( weapon.Resource );
			if( resInfo == null )
				return 0;

			CraftAttributeInfo attInfo = resInfo.AttributeInfo;
			if( attInfo == null )
				return 0;

			int value = 0;

			switch( attribute )
			{
				case AosAttribute.AttackChance:			value += attInfo.WeaponAttackChance; break;
				case AosAttribute.BonusDex:				value += attInfo.WeaponBonusDex; break;
				case AosAttribute.BonusHits:			value += attInfo.WeaponBonusHits; break;
				case AosAttribute.BonusInt:				value += attInfo.WeaponBonusInt; break;
				case AosAttribute.BonusMana:			value += attInfo.WeaponBonusMana; break;
				case AosAttribute.BonusStam:			value += attInfo.WeaponBonusStam; break;
				case AosAttribute.BonusStr:				value += attInfo.WeaponBonusStr; break;
				case AosAttribute.CastRecovery:			value += attInfo.WeaponCastRecovery; break;
				case AosAttribute.CastSpeed:			value += attInfo.WeaponCastSpeed; break;
				case AosAttribute.DefendChance:			value += attInfo.WeaponDefendChance; break;
				case AosAttribute.EnhancePotions:		value += attInfo.WeaponEnhancePotions; break;
				case AosAttribute.LowerManaCost:		value += attInfo.WeaponLowerManaCost; break;
				case AosAttribute.LowerRegCost:			value += attInfo.WeaponLowerRegCost; break;
				case AosAttribute.NightSight:			value += attInfo.WeaponNightSight; break;
				case AosAttribute.ReflectPhysical:		value += attInfo.WeaponReflectPhysical; break;
				case AosAttribute.RegenHits:			value += attInfo.WeaponRegenHits; break;
				case AosAttribute.RegenMana:			value += attInfo.WeaponRegenMana; break;
				case AosAttribute.RegenStam:			value += attInfo.WeaponRegenStam; break;
				case AosAttribute.SpellChanneling:		value += attInfo.WeaponSpellChanneling; break;
				case AosAttribute.SpellDamage:			value += attInfo.WeaponSpellDamage; break;
				case AosAttribute.WeaponDamage:			value += attInfo.WeaponDamage; break;
				case AosAttribute.WeaponSpeed:			value += attInfo.WeaponSpeed; break;
				default: break;
			}

			return value;
		}

		/// <summary>
		/// Static. Calculate bonus on BaseArmor item for a particular AosArmorAttribute
		/// </summary>
		/// <param name="armor">BaseArmor to check</param>
		/// <param name="attribute">AosArmorAttribute we want the value</param>
        /// <returns>value for attribute on armor</returns>
		public static int GetResourceBonus( BaseArmor armor, AosArmorAttribute attribute )
		{
			if( armor == null || armor.Deleted )
				return 0;

			CraftResourceInfo resInfo = CraftResources.GetInfo( armor.Resource );
			if( resInfo == null )
				return 0;

			CraftAttributeInfo attInfo = resInfo.AttributeInfo;
			if( attInfo == null )
				return 0;

			int value = 0;

			switch( attribute )
			{
				case AosArmorAttribute.SelfRepair: 	value += attInfo.ArmorSelfRepair; break;
				case AosArmorAttribute.MageArmor: 	value += attInfo.ArmorMageArmor; break;
				default: break;
			}

			return value;
		}

		/// <summary>
		/// Static. Calculate bonus on BaseWeapon item for a particular AosWeaponAttributes
		/// </summary>
		/// <param name="weapon">BaseWeapon to check</param>
		/// <param name="attribute">AosWeaponAttributes we want the value</param>
        /// <returns>value for attribute on our weapon</returns>
		public static int GetResourceBonus( BaseWeapon weapon, AosWeaponAttribute attribute )
		{
			if( weapon == null || weapon.Deleted )
				return 0;

			CraftResourceInfo resInfo = CraftResources.GetInfo( weapon.Resource );
			if( resInfo == null )
				return 0;

			CraftAttributeInfo attInfo = resInfo.AttributeInfo;
			if( attInfo == null )
				return 0;

			int value = 0;

			switch( attribute )
			{				
				case AosWeaponAttribute.SelfRepair:				value += attInfo.WeaponSelfRepair; break;
				case AosWeaponAttribute.HitLeechHits:			value += attInfo.WeaponHitLeechHits; break;
				case AosWeaponAttribute.HitLeechStam:			value += attInfo.WeaponHitLeechStam; break;
				case AosWeaponAttribute.HitLeechMana:			value += attInfo.WeaponHitLeechMana; break;
				case AosWeaponAttribute.HitLowerAttack:			value += attInfo.WeaponHitLowerAttack; break;
				case AosWeaponAttribute.HitLowerDefend:			value += attInfo.WeaponHitLowerDefend; break;
				case AosWeaponAttribute.HitMagicArrow:			value += attInfo.WeaponHitMagicArrow; break;
				case AosWeaponAttribute.HitHarm:				value += attInfo.WeaponHitHarm; break;
				case AosWeaponAttribute.HitFireball:			value += attInfo.WeaponHitFireball; break;
				case AosWeaponAttribute.HitLightning:			value += attInfo.WeaponHitLightning; break;
				case AosWeaponAttribute.HitDispel:				value += attInfo.WeaponHitDispel; break;
				case AosWeaponAttribute.HitColdArea:			value += attInfo.WeaponHitColdArea; break;
				case AosWeaponAttribute.HitFireArea:			value += attInfo.WeaponHitFireArea; break;
				case AosWeaponAttribute.HitPoisonArea:			value += attInfo.WeaponHitPoisonArea; break;
				case AosWeaponAttribute.HitEnergyArea:			value += attInfo.WeaponHitEnergyArea; break;
				case AosWeaponAttribute.HitPhysicalArea:		value += attInfo.WeaponHitPhysicalArea; break;
				case AosWeaponAttribute.ResistPhysicalBonus:	value += attInfo.WeaponResistPhysicalBonus; break;
				case AosWeaponAttribute.ResistFireBonus:		value += attInfo.WeaponResistFireBonus; break;
				case AosWeaponAttribute.ResistColdBonus:		value += attInfo.WeaponResistColdBonus; break;
				case AosWeaponAttribute.ResistPoisonBonus:		value += attInfo.WeaponResistPoisonBonus; break;
				case AosWeaponAttribute.ResistEnergyBonus:		value += attInfo.WeaponResistEnergyBonus; break;
				case AosWeaponAttribute.UseBestSkill:			value += attInfo.WeaponUseBestSkill; break;
				case AosWeaponAttribute.MageWeapon:				value += attInfo.WeaponMageWeapon; break;
				default: break;
			}

			return value;
		}
	}
}
