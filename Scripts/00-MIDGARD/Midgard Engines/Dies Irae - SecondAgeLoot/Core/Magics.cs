/***************************************************************************
 *                               Magics.cs
 *
 *   begin                : 06 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Engines.SecondAgeLoot
{
    public static class Magics
    {
        #region [accessors]
        public static BaseWeapon RandomMagicWeapon()
        {
            BaseWeapon weapon = Core.BuildRandomWeapon() as BaseWeapon;
            if( weapon != null )
                ApplyWeaponBonus( weapon );

            return weapon;
        }

        public static BaseArmor RandomMagicArmor()
        {
            BaseArmor armor = Core.BuildRandomArmor() as BaseArmor;
            if( armor != null )
                ApplyArmorBonus( armor );

            return armor;
        }

        public static BaseJewel RandomMagicJewel()
        {
            BaseJewel jewel = Core.BuildRandomJewel() as BaseJewel;
            if( jewel != null )
                ApplyJewelBonus( jewel );

            return jewel;
        }

        public static BaseClothing RandomMagicClothing()
        {
            BaseClothing clothing = Core.BuildRandomClothing() as BaseClothing;
            if( clothing != null )
                ApplyClothingBonus( clothing );

            return clothing;
        }
        #endregion

        private static int GetRandomWeightedIndex( int[] array )
        {
            if( array.Length == 1 )
                return 0;

            int totalChance = 0;

            foreach( int t in array )
                totalChance += t;

            if( array.Length == totalChance )
                return Utility.Random( array.Length );

            int rnd = Utility.Random( totalChance );

            for( int i = 0; i < array.Length; ++i )
            {
                if( rnd < array[ i ] )
                    return i;

                rnd -= array[ i ];
            }

            return 0;
        }

        public static void ApplyBonusTo( Item item )
        {
            if( item is BaseArmor )
                ApplyArmorBonus( item as BaseArmor );
            else if( item is BaseWeapon )
                ApplyWeaponBonus( item as BaseWeapon );
            else if( item is BaseJewel )
                ApplyJewelBonus( item as BaseJewel );
            else if( item is BaseClothing )
                ApplyClothingBonus( item as BaseClothing );
        }

        #region [weapons]
        /// <summary>
        /// 50 effect "weapon damage 1" { weapons 1 }
        /// 25 effect "weapon damage 2" { weapons 1 }
        /// 14 effect "weapon damage 3" { weapons 1 }
        /// 8  effect "weapon damage 4" { weapons 1 }
        /// 3  effect "weapon damage 5" { weapons 1 }
        /// </summary>
        private static readonly int[] m_WeaponDamageWeights = new int[] { 0, 50, 25, 14, 8, 3 };

        /*
        private static readonly int[] m_POLWeaponDamageWeights = new int[]{ 0, 50, 25, 15, 5, 1 };
        */

        public static WeaponDamageLevel GetWeaponDamageLevelBonus()
        {
            return (WeaponDamageLevel)GetRandomWeightedIndex( m_WeaponDamageWeights );
        }

        /// <summary>
        /// 50 effect "weapon skill 1" { weapons 1 }
        /// 25 effect "weapon skill 2" { weapons 1 }
        /// 14 effect "weapon skill 3" { weapons 1 }
        /// 8  effect "weapon skill 4" { weapons 1 }
        /// 3  effect "weapon skill 5" { weapons 1 }
        /// </summary>
        private static readonly int[] m_WeaponAccuracyWeights = new int[] { 0, 50, 25, 14, 8, 3 };

        public static WeaponAccuracyLevel GetWeaponAccuracyLevelBonus()
        {
            return (WeaponAccuracyLevel)GetRandomWeightedIndex( m_WeaponAccuracyWeights );
        }

        /// <summary>
        /// 50 effect "durability 1" { armor 1 weapons 1 }
        /// 25 effect "durability 2" { armor 1 weapons 1 }
        /// 14 effect "durability 3" { armor 1 weapons 1 }
        /// 8  effect "durability 4" { armor 1 weapons 1 }
        /// 3  effect "durability 5" { armor 1 weapons 1 }
        /// </summary>
        private static readonly int[] m_WeaponDurabilityWeights = new int[] { 0, 50, 25, 14, 8, 3 };

        public static WeaponDurabilityLevel GetWeaponDurabilityLevelBonus()
        {
            return (WeaponDurabilityLevel)GetRandomWeightedIndex( m_WeaponDurabilityWeights );
        }

        /// <summary>
        /// 20  effect "hit silver"         { weapons 1 }
        /// 12	effect "hit clumsy"		    { weapons 1 }
        /// 12	effect "hit feeblemind"		{ weapons 1 }
        /// 10	effect "hit magic arrow"	{ weapons 1 }
        /// 12	effect "hit weaken"			{ weapons 1 }
        /// 8 	effect "hit harm"			{ weapons 1 }
        /// 4 	effect "hit fireball"		{ weapons 1 }
        /// 4	effect "hit curse"			{ weapons 1 }
        /// 2	effect "hit lightning"		{ weapons 1 }
        /// 4	effect "hit mana drain"		{ weapons 1 }
        /// 12	effect "hit paralyze"		{ weapons 1 }
        /// </summary>
        private static readonly int[] m_WeaponOtherEffectsWeights = new int[] { 20, 12, 12, 10, 12, 8, 4, 4, 2, 4, 12 };

        public static WeaponMagicalAttribute GetWeaponOtherEffectsLevelBonus()
        {
            return (WeaponMagicalAttribute)GetRandomWeightedIndex( m_WeaponOtherEffectsWeights );
        }

        private static void GetWeaponCharges( WeaponMagicalAttribute attribute, out int min, out int max )
        {
            min = 0; max = 0;

            switch( attribute )
            {
                case WeaponMagicalAttribute.None: min = 1; max = 10; return; // "hit silver" 
                case WeaponMagicalAttribute.Clumsiness: min = 3; max = 15; return; // "hit clumsy"
                case WeaponMagicalAttribute.Feeblemindedness: min = 3; max = 15; return; // "hit feeblemind"
                case WeaponMagicalAttribute.Burning: min = 3; max = 30; return; // "hit magic arrow"
                case WeaponMagicalAttribute.Weakness: min = 3; max = 15; return; // "hit weaken"	
                case WeaponMagicalAttribute.Wounding: min = 6; max = 60; return; // "hit harm"
                case WeaponMagicalAttribute.DaemonBreath: min = 9; max = 90; return; // "hit fireball"
                case WeaponMagicalAttribute.Evil: min = 24; max = 40; return; // "hit curse"	
                case WeaponMagicalAttribute.Thunder: min = 15; max = 50; return; // "hit lightning"
                case WeaponMagicalAttribute.MagesBane: min = 15; max = 50; return; // "hit mana drain"
                case WeaponMagicalAttribute.GhoulTouch: min = 24; max = 40; return; // "hit paralyze"	
            }
        }

        /// <summary>
        /// 35 effecttable "weapon skill increasers"
        /// 30 effecttable "weapon damage increasers"
        /// 25 effecttable "durability effects"
        /// 10 effecttable "weapon other effects"
        /// </summary>
        private static readonly int[] m_WeaponWeights = new int[] { 35, 30, 25, 10 };

        public static void ClearWeaponBonus( BaseWeapon weapon )
        {
            weapon.AccuracyLevel = WeaponAccuracyLevel.Regular;
            weapon.DamageLevel = WeaponDamageLevel.Regular;
            weapon.DurabilityLevel = WeaponDurabilityLevel.Regular;
            weapon.MagicalAttribute = WeaponMagicalAttribute.None;
            weapon.MagicalCharges = 0;
            weapon.Slayer = SlayerName.None;
        }

        public static void ApplyWeaponBonus( BaseWeapon weapon )
        {
            if( Utility.Random( 100 ) < m_WeaponWeights[ 0 ] )
                weapon.AccuracyLevel = GetWeaponAccuracyLevelBonus();

            if( Utility.Random( 100 ) < m_WeaponWeights[ 1 ] )
                weapon.DamageLevel = GetWeaponDamageLevelBonus();

            if( Utility.Random( 100 ) < m_WeaponWeights[ 2 ] )
                weapon.DurabilityLevel = GetWeaponDurabilityLevelBonus();

            if( Utility.Random( 100 ) < m_WeaponWeights[ 3 ] )
            {
                WeaponMagicalAttribute other = GetWeaponOtherEffectsLevelBonus();

                if( other == WeaponMagicalAttribute.None )
                    weapon.Slayer = SlayerName.Silver;
                else
                {
                    weapon.MagicalAttribute = other;

                    int min, max;
                    GetWeaponCharges( other, out min, out max );
                    weapon.MagicalCharges = Utility.RandomMinMax( min, max );
                }
            }
        }
        #endregion

        #region [armors]
        /// <summary>
        /// 50 effect "durability 1" { armor 1 weapons 1 }
        /// 25 effect "durability 2" { armor 1 weapons 1 }
        /// 14 effect "durability 3" { armor 1 weapons 1 }
        /// 8  effect "durability 4" { armor 1 weapons 1 }
        /// 3  effect "durability 5" { armor 1 weapons 1 }
        /// </summary>
        private static readonly int[] m_ArmorDurabilityWeights = new int[] { 0, 50, 25, 14, 8, 3 };

        public static ArmorDurabilityLevel GetArmorDurabilityLevelBonus()
        {
            return (ArmorDurabilityLevel)GetRandomWeightedIndex( m_ArmorDurabilityWeights );
        }

        /// <summary>
        /// 50 effect "armor defense 1" { armor 1 }
        /// 25 effect "armor defense 2" { armor 1 }
        /// 14 effect "armor defense 3" { armor 1 }
        /// 8  effect "armor defense 4" { armor 1 }
        /// 3  effect "armor defense 5" { armor 1 }
        /// </summary>
        private static readonly int[] m_ArmorProtectionWeights = new int[] { 0, 50, 25, 14, 8, 3 };

        public static ArmorProtectionLevel GetArmorProtectionLevelBonus()
        {
            return (ArmorProtectionLevel)GetRandomWeightedIndex( m_ArmorProtectionWeights );
        }

        /// <summary>
        /// 20	 effecttable "wear benificial items"
        /// 20	 effecttable "wear protections"
        /// 10   effecttable "wear cursed items"
        /// </summary>
        private static readonly int[] m_ArmorOtherEffectWeights = new int[] { 20, 20, 10 };

        public static ArmorMagicalAttribute GetArmorOtherEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ArmorOtherEffectWeights ) )
            {
                case 0: return GetCursedEffectsLevelBonus();
                case 1: return GetBeneficalEffectsLevelBonus();
                case 2: return GetProtectionsEffectsLevelBonus();
                default: return ArmorMagicalAttribute.None;
            }
        }

        private static void GetArmorCharges( ArmorMagicalAttribute attribute, out int min, out int max )
        {
            switch( attribute )
            {
                case ArmorMagicalAttribute.None: min = 3; max = 15; return;
                case ArmorMagicalAttribute.Clumsiness: min = 3; max = 15; return;
                case ArmorMagicalAttribute.Feeblemindedness: min = 3; max = 15; return;
                case ArmorMagicalAttribute.Weakness: min = 3; max = 15; return;
                case ArmorMagicalAttribute.Agility: min = 6; max = 60; return;
                case ArmorMagicalAttribute.Cunning: min = 6; max = 60; return;
                case ArmorMagicalAttribute.Strength: min = 6; max = 60; return;
                case ArmorMagicalAttribute.Curses: min = 15; max = 75; return;
                case ArmorMagicalAttribute.NightEyes: min = 3; max = 30; return;
                case ArmorMagicalAttribute.Blessings: min = 9; max = 90; return;
                case ArmorMagicalAttribute.SpellReflection: min = 24; max = 36; return;
                default: min = 3; max = 15; return;
            }
        }

        /// <summary>
        /// 30	 effect "wear clumsy" { armor 2 armor 1 }
        /// 30   effect "wear feeblemind" { armor 2 armor 1 }
        /// 30   effect "wear weaken" { armor 2 armor 1 }
        /// 10	 effect "wear curse" { armor 2 armor 1 }
        /// </summary>
        private static readonly int[] m_ArmorCursedItemsWeights = new int[] { 30, 30, 30, 10 };

        public static ArmorMagicalAttribute GetCursedEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ArmorCursedItemsWeights ) )
            {
                case 0: return ArmorMagicalAttribute.Clumsiness;
                case 1: return ArmorMagicalAttribute.Feeblemindedness;
                case 2: return ArmorMagicalAttribute.Weakness;
                case 3: return ArmorMagicalAttribute.Curses;

                default: return ArmorMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 30   effect "wear agility" { armor 1 armor 1 }
        /// 30   effect "wear cunning" { armor 1 armor 1 }
        /// 30	 effect "wear strength" { armor 1 armor 1 }
        /// 10	 effect "wear bless" { armor 1 armor 1 }
        /// </summary>
        private static readonly int[] m_ArmorBeneficalItemsWeights = new int[] { 30, 30, 30, 10 };

        public static ArmorMagicalAttribute GetBeneficalEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ArmorBeneficalItemsWeights ) )
            {
                case 0: return ArmorMagicalAttribute.Agility;
                case 1: return ArmorMagicalAttribute.Cunning;
                case 2: return ArmorMagicalAttribute.Strength;
                case 3: return ArmorMagicalAttribute.Blessings;

                default: return ArmorMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 50	 effect "wear protection" { armor 1 armor 1  }
        /// 20 	 effect "wear magic reflect" { armor 1 armor 1 }
        /// </summary>
        private static readonly int[] m_ArmorProtectionsItemsWeights = new int[] { 50, 20 };

        public static ArmorMagicalAttribute GetProtectionsEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ArmorProtectionsItemsWeights ) )
            {
                case 0: return ArmorMagicalAttribute.Blessings; // should be protection
                case 1: return ArmorMagicalAttribute.SpellReflection;

                default: return ArmorMagicalAttribute.None;
            }
        }

        public static void ClearArmorBonus( BaseArmor armor )
        {
            armor.ProtectionLevel = ArmorProtectionLevel.Regular;
            armor.Durability = ArmorDurabilityLevel.Regular;
            armor.MagicalAttribute = ArmorMagicalAttribute.None;
            armor.MagicalCharges = 0;
        }

        /// <summary>
        /// 75 effecttable "armor defense increasers"
        /// 25 effecttable "durability effects"
        /// 10 effecttable "weapon other effects" (added by Dies Irae, not confirmed in the demo)
        /// </summary>
        private static readonly int[] m_ArmorWeights = new int[] { 75, 25, 10 };

        public static void ApplyArmorBonus( BaseArmor armor )
        {
            if( Utility.Random( 100 ) < m_ArmorWeights[ 0 ] )
                armor.ProtectionLevel = GetArmorProtectionLevelBonus();

            if( Utility.Random( 100 ) < m_ArmorWeights[ 1 ] )
                armor.Durability = GetArmorDurabilityLevelBonus();

            if( Utility.Random( 100 ) < m_ArmorWeights[ 2 ] )
            {
                ArmorMagicalAttribute other = GetArmorOtherEffectsLevelBonus();
                armor.MagicalAttribute = other;

                int min, max;
                GetArmorCharges( other, out min, out max );
                armor.MagicalCharges = Utility.RandomMinMax( min, max );
            }
        }
        #endregion

        #region [jewels]
        /// <summary>
        /// 30	 effect "wear clumsy" { wearables 2 armor 1 }
        /// 30   effect "wear feeblemind" { wearables 2 armor 1 }
        /// 30   effect "wear weaken" { wearables 2 armor 1 }
        /// 10	 effect "wear curse" { wearables 2 armor 1 }
        /// </summary>
        private static readonly int[] m_JewelsCursedItemsWeights = new int[] { 30, 30, 30, 10 };

        public static JewelMagicalAttribute GetJewelCursedEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_JewelsCursedItemsWeights ) )
            {
                case 0: return JewelMagicalAttribute.Clumsiness;
                case 1: return JewelMagicalAttribute.Feeblemindedness;
                case 2: return JewelMagicalAttribute.Weakness;
                case 3: return JewelMagicalAttribute.Curses;

                default: return JewelMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 30   effect "wear agility" { wearables 1 armor 1 }
        /// 30   effect "wear cunning" { wearables 1 armor 1 }
        /// 30	 effect "wear strength" { wearables 1 armor 1 }
        /// 10	 effect "wear bless" { wearables 1 armor 1 }
        /// </summary>
        private static readonly int[] m_JewelBeneficalItemsWeights = new int[] { 30, 30, 30, 10 };

        public static JewelMagicalAttribute GetJewelBeneficalEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_JewelBeneficalItemsWeights ) )
            {
                case 0: return JewelMagicalAttribute.Agility;
                case 1: return JewelMagicalAttribute.Cunning;
                case 2: return JewelMagicalAttribute.Strength;
                case 3: return JewelMagicalAttribute.Blessings;

                default: return JewelMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 50	 effect "wear protection" { wearables 1 armor 1  }
        /// 20 	 effect "wear magic reflect" { wearables 1 armor 1 }
        /// 30   effect "wear invisibility" { wearables 1 }
        /// </summary>
        private static readonly int[] m_JewelProtectionsItemsWeights = new int[] { 50, 20, 30 };

        public static JewelMagicalAttribute GetJewelProtectionsEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_JewelProtectionsItemsWeights ) )
            {
                case 0: return JewelMagicalAttribute.Blessings; // should be protection
                case 1: return JewelMagicalAttribute.SpellReflection;
                case 2: return JewelMagicalAttribute.Invisibility;

                default: return JewelMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 50	 effect "wear light" { wearables 1 }
        /// 50   effect "wear birds eye" { wearables 1 }
        /// </summary>
        private static readonly int[] m_JewelUsefulThingsItemsWeights = new int[] { 50, 50 };

        public static JewelMagicalAttribute GetJewelUsefulThingsEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_JewelUsefulThingsItemsWeights ) )
            {
                case 0: return JewelMagicalAttribute.NightEyes;
                case 1: return JewelMagicalAttribute.NightEyes; // should be 'birds eye'

                default: return JewelMagicalAttribute.None;
            }
        }

        public static void GetJewelCharges( JewelMagicalAttribute attribute, out int min, out int max )
        {
            /*
            			switch ( e )
			{
				case SpellEffect.Clumsy:
				case SpellEffect.Feeblemind:
				case SpellEffect.MagicArrow:
					return Utility.Random( 25 ) + 15;
             * 
				case SpellEffect.Weaken:
				case SpellEffect.Harm:
					return Utility.Random( 15 ) + 10;
				case SpellEffect.Paralyze:
					return Utility.Random( 15 ) + 5;
				case SpellEffect.Fireball:
				case SpellEffect.Curse:
					return Utility.Random( 10 ) + 10;
				case SpellEffect.ManaDrain:
				case SpellEffect.Lightning:
					return Utility.Random( 10 ) + 5;
				
				case SpellEffect.ItemID:
					return Utility.Random( 150 ) + 25;
				case SpellEffect.MiniHeal:
					return Utility.Random( 15 ) + 10;
				case SpellEffect.GHeal:
					return Utility.Random( 15 ) + 5;

				case SpellEffect.NightSight:
					return Utility.Random( 25 ) + 25;
				case SpellEffect.Protection:
				case SpellEffect.Agility:
				case SpellEffect.Cunning:
				case SpellEffect.Strength:
					return Utility.Random( 20 ) + 10;
				case SpellEffect.Bless:
					return Utility.Random( 15 ) + 10;
				case SpellEffect.Reflect:
					return Utility.Random( 15 ) + 1;
				case SpellEffect.Invis:
				case SpellEffect.Teleportation:
					return Utility.Random( 10 ) + 1;

				case SpellEffect.None:
				default:
					return 0;
			}
             */
            switch( attribute )
            {
                case JewelMagicalAttribute.None: min = 3; max = 15; return;

                case JewelMagicalAttribute.Clumsiness: min = 3; max = 15; return;
                case JewelMagicalAttribute.Feeblemindedness: min = 3; max = 15; return;
                case JewelMagicalAttribute.Weakness: min = 3; max = 15; return;
                case JewelMagicalAttribute.Agility: min = 6; max = 60; return;
                case JewelMagicalAttribute.Cunning: min = 6; max = 60; return;
                case JewelMagicalAttribute.Strength: min = 6; max = 60; return;
                case JewelMagicalAttribute.Curses: min = 15; max = 75; return;
                case JewelMagicalAttribute.NightEyes: min = 3; max = 30; return;
                case JewelMagicalAttribute.Blessings: min = 9; max = 90; return;
                case JewelMagicalAttribute.SpellReflection: min = 24; max = 36; return;
                case JewelMagicalAttribute.Invisibility: min = 39; max = 39; return;
                case JewelMagicalAttribute.Teleportation: min = 15; max = 15; return;
                default: min = 3; max = 15; return;
            }
        }

        public static void ClearJewelBonus( BaseJewel jewel )
        {
            jewel.MagicalAttribute = JewelMagicalAttribute.None;
            jewel.MagicalCharges = 0;
        }

        /// <summary>
        /// 50	 effecttable "wear useful things"
        /// 20	 effecttable "wear benificial items"
        /// 20	 effecttable "wear protections"
        /// 10   effecttable "wear cursed items"
        /// </summary>
        private static readonly int[] m_JewelWeights = new int[] { 50, 20, 20, 10 };

        public static void ApplyJewelBonus( BaseJewel jewel )
        {
            switch( GetRandomWeightedIndex( m_JewelWeights ) )
            {
                case 0: jewel.MagicalAttribute = GetJewelUsefulThingsEffectsLevelBonus(); break;
                case 1: jewel.MagicalAttribute = GetJewelCursedEffectsLevelBonus(); break;
                case 2: jewel.MagicalAttribute = GetJewelBeneficalEffectsLevelBonus(); break;
                case 3: jewel.MagicalAttribute = GetJewelProtectionsEffectsLevelBonus(); break;
                default: return;
            }

            if( jewel.MagicalAttribute != JewelMagicalAttribute.None )
            {
                int min, max;
                GetJewelCharges( jewel.MagicalAttribute, out min, out max );
                jewel.MagicalCharges = Utility.RandomMinMax( min, max );
            }
        }
        #endregion

        #region [clothings]
        /// <summary>
        /// 30	 effect "wear clumsy" { wearables 2 armor 1 }
        /// 30   effect "wear feeblemind" { wearables 2 armor 1 }
        /// 30   effect "wear weaken" { wearables 2 armor 1 }
        /// 10	 effect "wear curse" { wearables 2 armor 1 }
        /// </summary>
        private static readonly int[] m_ClothingsCursedItemsWeights = new int[] { 30, 30, 30, 10 };

        public static ClothingMagicalAttribute GetClothingCursedEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ClothingsCursedItemsWeights ) )
            {
                case 0: return ClothingMagicalAttribute.Clumsiness;
                case 1: return ClothingMagicalAttribute.Feeblemindedness;
                case 2: return ClothingMagicalAttribute.Weakness;
                case 3: return ClothingMagicalAttribute.Curses;

                default: return ClothingMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 30   effect "wear agility" { wearables 1 armor 1 }
        /// 30   effect "wear cunning" { wearables 1 armor 1 }
        /// 30	 effect "wear strength" { wearables 1 armor 1 }
        /// 10	 effect "wear bless" { wearables 1 armor 1 }
        /// </summary>
        private static readonly int[] m_ClothingBeneficalItemsWeights = new int[] { 30, 30, 30, 10 };

        public static ClothingMagicalAttribute GetClothingBeneficalEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ClothingBeneficalItemsWeights ) )
            {
                case 0: return ClothingMagicalAttribute.Agility;
                case 1: return ClothingMagicalAttribute.Cunning;
                case 2: return ClothingMagicalAttribute.Strength;
                case 3: return ClothingMagicalAttribute.Blessings;

                default: return ClothingMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 50	 effect "wear protection" { wearables 1 armor 1  }
        /// 20 	 effect "wear magic reflect" { wearables 1 armor 1 }
        /// 30   effect "wear invisibility" { wearables 1 }
        /// </summary>
        private static readonly int[] m_ClothingProtectionsItemsWeights = new int[] { 50, 20, 30 };

        public static ClothingMagicalAttribute GetClothingProtectionsEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ClothingProtectionsItemsWeights ) )
            {
                case 0: return ClothingMagicalAttribute.Blessings; // should be protection
                case 1: return ClothingMagicalAttribute.SpellReflection;
                case 2: return ClothingMagicalAttribute.Invisibility;

                default: return ClothingMagicalAttribute.None;
            }
        }

        /// <summary>
        /// 50	 effect "wear light" { wearables 1 }
        /// 50   effect "wear birds eye" { wearables 1 }
        /// </summary>
        private static readonly int[] m_ClothingUsefulThingsItemsWeights = new int[] { 50, 50 };

        public static ClothingMagicalAttribute GetClothingUsefulThingsEffectsLevelBonus()
        {
            switch( GetRandomWeightedIndex( m_ClothingUsefulThingsItemsWeights ) )
            {
                case 0: return ClothingMagicalAttribute.NightEyes;
                case 1: return ClothingMagicalAttribute.NightEyes; // should be 'birds eye'

                default: return ClothingMagicalAttribute.None;
            }
        }

        private static void GetClothingCharges( ClothingMagicalAttribute attribute, out int min, out int max )
        {
            switch( attribute )
            {
                case ClothingMagicalAttribute.None: min = 3; max = 15; return;
                case ClothingMagicalAttribute.Clumsiness: min = 3; max = 15; return;
                case ClothingMagicalAttribute.Feeblemindedness: min = 3; max = 15; return;
                case ClothingMagicalAttribute.Weakness: min = 3; max = 15; return;
                case ClothingMagicalAttribute.Agility: min = 6; max = 60; return;
                case ClothingMagicalAttribute.Cunning: min = 6; max = 60; return;
                case ClothingMagicalAttribute.Strength: min = 6; max = 60; return;
                case ClothingMagicalAttribute.Curses: min = 15; max = 75; return;
                case ClothingMagicalAttribute.NightEyes: min = 3; max = 30; return;
                case ClothingMagicalAttribute.Blessings: min = 9; max = 90; return;
                case ClothingMagicalAttribute.SpellReflection: min = 24; max = 36; return;
                case ClothingMagicalAttribute.Invisibility: min = 39; max = 39; return;
                default: min = 3; max = 15; return;
            }
        }

        public static void ClearClothingBonus( BaseClothing clothing )
        {
            clothing.MagicalAttribute = ClothingMagicalAttribute.None;
            clothing.MagicalCharges = 0;
        }

        /// <summary>
        /// 50	 effecttable "wear useful things"
        /// 20	 effecttable "wear benificial items"
        /// 20	 effecttable "wear protections"
        /// 10   effecttable "wear cursed items"
        /// </summary>
        private static readonly int[] m_ClothingWeights = new int[] { 50, 20, 20, 10 };

        public static void ApplyClothingBonus( BaseClothing clothing )
        {
            switch( GetRandomWeightedIndex( m_ClothingWeights ) )
            {
                case 0: clothing.MagicalAttribute = GetClothingUsefulThingsEffectsLevelBonus(); break;
                case 1: clothing.MagicalAttribute = GetClothingCursedEffectsLevelBonus(); break;
                case 2: clothing.MagicalAttribute = GetClothingBeneficalEffectsLevelBonus(); break;
                case 3: clothing.MagicalAttribute = GetClothingProtectionsEffectsLevelBonus(); break;
                default: return;
            }

            if( clothing.MagicalAttribute != ClothingMagicalAttribute.None )
            {
                int min, max;
                GetClothingCharges( clothing.MagicalAttribute, out min, out max );
                clothing.MagicalCharges = Utility.RandomMinMax( min, max );
            }
        }
        #endregion
    }
}