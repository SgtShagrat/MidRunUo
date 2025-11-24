using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Server.Items
{
    public enum CraftResource
    {
        None = 0,

        #region midgard metals
        Iron = 1,
        DullCopper,
        ShadowIron,
        Copper,
        Bronze,
        Gold,
        Agapite,
        Verite,
        Valorite,
        Platinum,
        Titanium,
        Obsidian,
        DarkRuby,
        EbonSapphire,
        RadiantDiamond,
        Eldar,
        Crystaline,
        Vulcan,
        Aqua,
        #endregion

        #region midgard woods
        RegularWood,
        Oak,
        Walnut,
        Ohii,
        Cedar,
        Willow,
        Cypress,
        Yew,
        Apple,
        Pear,
        Peach,
        Banana,
        Stonewood,
        Silver,
        Blood,
        Swamp,
        Crystal,
        Elven, // not supported in pre-aos
        Elder, // not supported in pre-aos
        Enchanted,
        Death, // not supported in pre-aos 
        #endregion

        #region old midgard metals
        OldDullCopper = 51,
        OldShadowIron,
        OldCopper,
        OldBronze,
        OldGold,
        OldAgapite,
        OldVerite,
        OldValorite,
        OldGraphite,
        OldPyrite,
        OldAzurite,
        OldVanadium,
        OldSilver,
        OldPlatinum,
        OldAmethyst,
        OldTitanium,
        OldXenian,
        OldRubidian,
        OldObsidian,
        OldEbonSapphire,
        OldDarkRuby,
        OldRadiantDiamond,
        #endregion

        #region midgard leathers
        RegularLeather = 101,
        SpinedLeather,
        HornedLeather,
        BarbedLeather,

        HumanoidLeather,
        UndeadLeather,
        WolfLeather,
        AracnidLeather,
        FeyLeather,
        GreenDragonLeather,
        BlackDragonLeather,
        BlueDragonLeather,
        RedDragonLeather,
        AbyssLeather,
        #endregion

        #region old midgard leather
        OldWolfLeather = 151,
        OldBearLeather,
        OldArachnidLeather,
        OldReptileLeather,
        OldOrcishLeather,
        OldHumanoidLeather,
        OldUndeadLeather,
        OldOphidianLeather,
        OldLavaLeather,
        OldArcticLeather,
        OldGreenDragonLeather,
        OldBlueDragonLeather,
        OldBlackDragonLeather,
        OldRedDragonLeather,
        OldDemonLeather,
        #endregion

        #region scales
        RedScales = 201,
        YellowScales,
        BlackScales,
        GreenScales,
        WhiteScales,
        BlueScales
        #endregion

        /*
        RegularWood = 301,
        OakWood,
        AshWood,
        YewWood,
        Heartwood,
        Bloodwood,
        Frostwood
        */
    }

    public enum CraftResourceType
    {
        None,
        Metal,
        Leather,
        Scales,
        Wood
    }

    public class CraftAttributeInfo
    {
        private int m_WeaponFireDamage;
        private int m_WeaponColdDamage;
        private int m_WeaponPoisonDamage;
        private int m_WeaponEnergyDamage;
        private int m_WeaponChaosDamage;
        private int m_WeaponDirectDamage;
        private int m_WeaponDurability;
        private int m_WeaponLuck;
        private int m_WeaponGoldIncrease;
        private int m_WeaponLowerRequirements;

        private int m_ArmorPhysicalResist;
        private int m_ArmorFireResist;
        private int m_ArmorColdResist;
        private int m_ArmorPoisonResist;
        private int m_ArmorEnergyResist;
        private int m_ArmorDurability;
        private int m_ArmorLuck;
        private int m_ArmorGoldIncrease;
        private int m_ArmorLowerRequirements;

        private int m_RunicMinAttributes;
        private int m_RunicMaxAttributes;
        private int m_RunicMinIntensity;
        private int m_RunicMaxIntensity;

        private int m_OldToolDurability;

        public int WeaponFireDamage { get { return m_WeaponFireDamage; } set { m_WeaponFireDamage = value; } }
        public int WeaponColdDamage { get { return m_WeaponColdDamage; } set { m_WeaponColdDamage = value; } }
        public int WeaponPoisonDamage { get { return m_WeaponPoisonDamage; } set { m_WeaponPoisonDamage = value; } }
        public int WeaponEnergyDamage { get { return m_WeaponEnergyDamage; } set { m_WeaponEnergyDamage = value; } }
        public int WeaponChaosDamage { get { return m_WeaponChaosDamage; } set { m_WeaponChaosDamage = value; } }
        public int WeaponDirectDamage { get { return m_WeaponDirectDamage; } set { m_WeaponDirectDamage = value; } }
        public int WeaponDurability { get { return m_WeaponDurability; } set { m_WeaponDurability = value; } }
        public int WeaponLuck { get { return m_WeaponLuck; } set { m_WeaponLuck = value; } }
        public int WeaponGoldIncrease { get { return m_WeaponGoldIncrease; } set { m_WeaponGoldIncrease = value; } }
        public int WeaponLowerRequirements { get { return m_WeaponLowerRequirements; } set { m_WeaponLowerRequirements = value; } }

        public int ArmorPhysicalResist { get { return m_ArmorPhysicalResist; } set { m_ArmorPhysicalResist = value; } }
        public int ArmorFireResist { get { return m_ArmorFireResist; } set { m_ArmorFireResist = value; } }
        public int ArmorColdResist { get { return m_ArmorColdResist; } set { m_ArmorColdResist = value; } }
        public int ArmorPoisonResist { get { return m_ArmorPoisonResist; } set { m_ArmorPoisonResist = value; } }
        public int ArmorEnergyResist { get { return m_ArmorEnergyResist; } set { m_ArmorEnergyResist = value; } }
        public int ArmorDurability { get { return m_ArmorDurability; } set { m_ArmorDurability = value; } }
        public int ArmorLuck { get { return m_ArmorLuck; } set { m_ArmorLuck = value; } }
        public int ArmorGoldIncrease { get { return m_ArmorGoldIncrease; } set { m_ArmorGoldIncrease = value; } }
        public int ArmorLowerRequirements { get { return m_ArmorLowerRequirements; } set { m_ArmorLowerRequirements = value; } }

        public int RunicMinAttributes { get { return m_RunicMinAttributes; } set { m_RunicMinAttributes = value; } }
        public int RunicMaxAttributes { get { return m_RunicMaxAttributes; } set { m_RunicMaxAttributes = value; } }
        public int RunicMinIntensity { get { return m_RunicMinIntensity; } set { m_RunicMinIntensity = value; } }
        public int RunicMaxIntensity { get { return m_RunicMaxIntensity; } set { m_RunicMaxIntensity = value; } }

        public int OldToolDurability { get { return m_OldToolDurability; } set { m_OldToolDurability = value; } }

        /*
        #region Mondain's Legacy		
        private int m_WeaponDamage;
        private int m_WeaponHitChance;
        private int m_WeaponHitLifeLeech;
        private int m_WeaponRegenHits;
        private int m_WeaponSwingSpeed;
		
        private int m_ArmorDamage;
        private int m_ArmorHitChance;
        private int m_ArmorRegenHits;
        private int m_ArmorMage;
		
        private int m_ShieldPhysicalResist;
        private int m_ShieldFireResist;
        private int m_ShieldColdResist;
        private int m_ShieldPoisonResist;
        private int m_ShieldEnergyResist;
		
        public int WeaponDamage{ get{ return m_WeaponDamage; } set{ m_WeaponDamage = value; } }
        public int WeaponHitChance{ get{ return m_WeaponHitChance; } set{ m_WeaponHitChance = value; } }
        public int WeaponHitLifeLeech{ get{ return m_WeaponHitLifeLeech; } set{ m_WeaponHitLifeLeech = value; } }
        public int WeaponRegenHits{ get{ return m_WeaponRegenHits; } set{ m_WeaponRegenHits = value; } }
        public int WeaponSwingSpeed{ get{ return m_WeaponSwingSpeed; } set{ m_WeaponSwingSpeed = value; } }
		
        public int ArmorDamage{ get{ return m_ArmorDamage; } set{ m_ArmorDamage = value; } }
        public int ArmorHitChance{ get{ return m_ArmorHitChance; } set{ m_ArmorHitChance = value; } }
        public int ArmorRegenHits{ get{ return m_ArmorRegenHits; } set{ m_ArmorRegenHits = value; } }
        public int ArmorMage{ get{ return m_ArmorMage; } set{ m_ArmorMage = value; } }
		
        public int ShieldPhysicalResist{ get{ return m_ShieldPhysicalResist; } set{ m_ShieldPhysicalResist = value; } }
        public int ShieldFireResist{ get{ return m_ShieldFireResist; } set{ m_ShieldFireResist = value; } }
        public int ShieldColdResist{ get{ return m_ShieldColdResist; } set{ m_ShieldColdResist = value; } }
        public int ShieldPoisonResist{ get{ return m_ShieldPoisonResist; } set{ m_ShieldPoisonResist = value; } }
        public int ShieldEnergyResist{ get{ return m_ShieldEnergyResist; } set{ m_ShieldEnergyResist = value; } }
        #endregion
        */

        #region modifica by Dies Irae
        #region AoSAttributes for weapons ( no luck! )
        private int m_WeaponAttackChance;
        private int m_WeaponBonusDex;
        private int m_WeaponBonusHits;
        private int m_WeaponBonusInt;
        private int m_WeaponBonusMana;
        private int m_WeaponBonusStam;
        private int m_WeaponBonusStr;
        private int m_WeaponCastRecovery;
        private int m_WeaponCastSpeed;
        private int m_WeaponDefendChance;
        private int m_WeaponEnhancePotions;
        private int m_WeaponLowerManaCost;
        private int m_WeaponLowerRegCost;
        private int m_WeaponNightSight;
        private int m_WeaponReflectPhysical;
        private int m_WeaponRegenHits;
        private int m_WeaponRegenMana;
        private int m_WeaponRegenStam;
        private int m_WeaponSpellChanneling;
        private int m_WeaponSpellDamage;
        private int m_WeaponDamage;
        private int m_WeaponSpeed;

        public int WeaponAttackChance { get { return m_WeaponAttackChance; } set { m_WeaponAttackChance = value; } }
        public int WeaponBonusDex { get { return m_WeaponBonusDex; } set { m_WeaponBonusDex = value; } }
        public int WeaponBonusHits { get { return m_WeaponBonusHits; } set { m_WeaponBonusHits = value; } }
        public int WeaponBonusInt { get { return m_WeaponBonusInt; } set { m_WeaponBonusInt = value; } }
        public int WeaponBonusMana { get { return m_WeaponBonusMana; } set { m_WeaponBonusMana = value; } }
        public int WeaponBonusStam { get { return m_WeaponBonusStam; } set { m_WeaponBonusStam = value; } }
        public int WeaponBonusStr { get { return m_WeaponBonusStr; } set { m_WeaponBonusStr = value; } }
        public int WeaponCastRecovery { get { return m_WeaponCastRecovery; } set { m_WeaponCastRecovery = value; } }
        public int WeaponCastSpeed { get { return m_WeaponCastSpeed; } set { m_WeaponCastSpeed = value; } }
        public int WeaponDefendChance { get { return m_WeaponDefendChance; } set { m_WeaponDefendChance = value; } }
        public int WeaponEnhancePotions { get { return m_WeaponEnhancePotions; } set { m_WeaponEnhancePotions = value; } }
        public int WeaponLowerManaCost { get { return m_WeaponLowerManaCost; } set { m_WeaponLowerManaCost = value; } }
        public int WeaponLowerRegCost { get { return m_WeaponLowerRegCost; } set { m_WeaponLowerRegCost = value; } }
        public int WeaponNightSight { get { return m_WeaponNightSight; } set { m_WeaponNightSight = value; } }
        public int WeaponReflectPhysical { get { return m_WeaponReflectPhysical; } set { m_WeaponReflectPhysical = value; } }
        public int WeaponRegenHits { get { return m_WeaponRegenHits; } set { m_WeaponRegenHits = value; } }
        public int WeaponRegenMana { get { return m_WeaponRegenMana; } set { m_WeaponRegenMana = value; } }
        public int WeaponRegenStam { get { return m_WeaponRegenStam; } set { m_WeaponRegenStam = value; } }
        public int WeaponSpellChanneling { get { return m_WeaponSpellChanneling; } set { m_WeaponSpellChanneling = value; } }
        public int WeaponSpellDamage { get { return m_WeaponSpellDamage; } set { m_WeaponSpellDamage = value; } }
        public int WeaponDamage { get { return m_WeaponDamage; } set { m_WeaponDamage = value; } }
        public int WeaponSpeed { get { return m_WeaponSpeed; } set { m_WeaponSpeed = value; } }
        #endregion

        #region AoSAttributes for armors ( no luck! )
        private int m_ArmorAttackChance;
        private int m_ArmorBonusDex;
        private int m_ArmorBonusHits;
        private int m_ArmorBonusInt;
        private int m_ArmorBonusMana;
        private int m_ArmorBonusStam;
        private int m_ArmorBonusStr;
        private int m_ArmorCastRecovery;
        private int m_ArmorCastSpeed;
        private int m_ArmorDefendChance;
        private int m_ArmorEnhancePotions;
        private int m_ArmorLowerManaCost;
        private int m_ArmorLowerRegCost;
        private int m_ArmorNightSight;
        private int m_ArmorReflectPhysical;
        private int m_ArmorRegenHits;
        private int m_ArmorRegenMana;
        private int m_ArmorRegenStam;
        private int m_ArmorSpellChanneling;
        private int m_ArmorSpellDamage;
        private int m_ArmorDamage;
        private int m_ArmorSpeed;

        public int ArmorAttackChance { get { return m_ArmorAttackChance; } set { m_ArmorAttackChance = value; } }
        public int ArmorBonusDex { get { return m_ArmorBonusDex; } set { m_ArmorBonusDex = value; } }
        public int ArmorBonusHits { get { return m_ArmorBonusHits; } set { m_ArmorBonusHits = value; } }
        public int ArmorBonusInt { get { return m_ArmorBonusInt; } set { m_ArmorBonusInt = value; } }
        public int ArmorBonusMana { get { return m_ArmorBonusMana; } set { m_ArmorBonusMana = value; } }
        public int ArmorBonusStam { get { return m_ArmorBonusStam; } set { m_ArmorBonusStam = value; } }
        public int ArmorBonusStr { get { return m_ArmorBonusStr; } set { m_ArmorBonusStr = value; } }
        public int ArmorCastRecovery { get { return m_ArmorCastRecovery; } set { m_ArmorCastRecovery = value; } }
        public int ArmorCastSpeed { get { return m_ArmorCastSpeed; } set { m_ArmorCastSpeed = value; } }
        public int ArmorDefendChance { get { return m_ArmorDefendChance; } set { m_ArmorDefendChance = value; } }
        public int ArmorEnhancePotions { get { return m_ArmorEnhancePotions; } set { m_ArmorEnhancePotions = value; } }
        public int ArmorLowerManaCost { get { return m_ArmorLowerManaCost; } set { m_ArmorLowerManaCost = value; } }
        public int ArmorLowerRegCost { get { return m_ArmorLowerRegCost; } set { m_ArmorLowerRegCost = value; } }
        public int ArmorNightSight { get { return m_ArmorNightSight; } set { m_ArmorNightSight = value; } }
        public int ArmorReflectPhysical { get { return m_ArmorReflectPhysical; } set { m_ArmorReflectPhysical = value; } }
        public int ArmorRegenHits { get { return m_ArmorRegenHits; } set { m_ArmorRegenHits = value; } }
        public int ArmorRegenMana { get { return m_ArmorRegenMana; } set { m_ArmorRegenMana = value; } }
        public int ArmorRegenStam { get { return m_ArmorRegenStam; } set { m_ArmorRegenStam = value; } }
        public int ArmorSpellChanneling { get { return m_ArmorSpellChanneling; } set { m_ArmorSpellChanneling = value; } }
        public int ArmorSpellDamage { get { return m_ArmorSpellDamage; } set { m_ArmorSpellDamage = value; } }
        public int ArmorWeaponDamage { get { return m_ArmorDamage; } set { m_ArmorDamage = value; } }
        public int ArmorWeaponSpeed { get { return m_ArmorSpeed; } set { m_ArmorSpeed = value; } }
        #endregion

        #region AoSArmorAttributes  ( no durability, no low req )
        private int m_ArmorSelfRepair;
        private int m_ArmorMageArmor;

        public int ArmorSelfRepair { get { return m_ArmorSelfRepair; } set { m_ArmorSelfRepair = value; } }
        public int ArmorMageArmor { get { return m_ArmorMageArmor; } set { m_ArmorMageArmor = value; } }
        #endregion

        #region AoSWeaponAttributs ( no durability, no low req )
        private int m_WeaponSelfRepair;
        private int m_WeaponHitLeechHits;
        private int m_WeaponHitLeechStam;
        private int m_WeaponHitLeechMana;
        private int m_WeaponHitLowerAttack;
        private int m_WeaponHitLowerDefend;
        private int m_WeaponHitMagicArrow;
        private int m_WeaponHitHarm;
        private int m_WeaponHitFireball;
        private int m_WeaponHitLightning;
        private int m_WeaponHitDispel;
        private int m_WeaponHitColdArea;
        private int m_WeaponHitFireArea;
        private int m_WeaponHitPoisonArea;
        private int m_WeaponHitEnergyArea;
        private int m_WeaponHitPhysicalArea;
        private int m_WeaponResistPhysicalBonus;
        private int m_WeaponResistFireBonus;
        private int m_WeaponResistColdBonus;
        private int m_WeaponResistPoisonBonus;
        private int m_WeaponResistEnergyBonus;
        private int m_WeaponUseBestSkill;
        private int m_WeaponMageWeapon;

        public int WeaponSelfRepair { get { return m_WeaponSelfRepair; } set { m_WeaponSelfRepair = value; } }
        public int WeaponHitLeechHits { get { return m_WeaponHitLeechHits; } set { m_WeaponHitLeechHits = value; } }
        public int WeaponHitLeechStam { get { return m_WeaponHitLeechStam; } set { m_WeaponHitLeechStam = value; } }
        public int WeaponHitLeechMana { get { return m_WeaponHitLeechMana; } set { m_WeaponHitLeechMana = value; } }
        public int WeaponHitLowerAttack { get { return m_WeaponHitLowerAttack; } set { m_WeaponHitLowerAttack = value; } }
        public int WeaponHitLowerDefend { get { return m_WeaponHitLowerDefend; } set { m_WeaponHitLowerDefend = value; } }
        public int WeaponHitMagicArrow { get { return m_WeaponHitMagicArrow; } set { m_WeaponHitMagicArrow = value; } }
        public int WeaponHitHarm { get { return m_WeaponHitHarm; } set { m_WeaponHitHarm = value; } }
        public int WeaponHitFireball { get { return m_WeaponHitFireball; } set { m_WeaponHitFireball = value; } }
        public int WeaponHitLightning { get { return m_WeaponHitLightning; } set { m_WeaponHitLightning = value; } }
        public int WeaponHitDispel { get { return m_WeaponHitDispel; } set { m_WeaponHitDispel = value; } }
        public int WeaponHitColdArea { get { return m_WeaponHitColdArea; } set { m_WeaponHitColdArea = value; } }
        public int WeaponHitFireArea { get { return m_WeaponHitFireArea; } set { m_WeaponHitFireArea = value; } }
        public int WeaponHitPoisonArea { get { return m_WeaponHitPoisonArea; } set { m_WeaponHitPoisonArea = value; } }
        public int WeaponHitEnergyArea { get { return m_WeaponHitEnergyArea; } set { m_WeaponHitEnergyArea = value; } }
        public int WeaponHitPhysicalArea { get { return m_WeaponHitPhysicalArea; } set { m_WeaponHitPhysicalArea = value; } }
        public int WeaponResistPhysicalBonus { get { return m_WeaponResistPhysicalBonus; } set { m_WeaponResistPhysicalBonus = value; } }
        public int WeaponResistFireBonus { get { return m_WeaponResistFireBonus; } set { m_WeaponResistFireBonus = value; } }
        public int WeaponResistColdBonus { get { return m_WeaponResistColdBonus; } set { m_WeaponResistColdBonus = value; } }
        public int WeaponResistPoisonBonus { get { return m_WeaponResistPoisonBonus; } set { m_WeaponResistPoisonBonus = value; } }
        public int WeaponResistEnergyBonus { get { return m_WeaponResistEnergyBonus; } set { m_WeaponResistEnergyBonus = value; } }
        public int WeaponUseBestSkill { get { return m_WeaponUseBestSkill; } set { m_WeaponUseBestSkill = value; } }
        public int WeaponMageWeapon { get { return m_WeaponMageWeapon; } set { m_WeaponMageWeapon = value; } }
        #endregion

        #region Shields
        private int m_ShieldPhysicalResist;
        private int m_ShieldFireResist;
        private int m_ShieldColdResist;
        private int m_ShieldPoisonResist;
        private int m_ShieldEnergyResist;

        public int ShieldPhysicalResist { get { return m_ShieldPhysicalResist; } set { m_ShieldPhysicalResist = value; } }
        public int ShieldFireResist { get { return m_ShieldFireResist; } set { m_ShieldFireResist = value; } }
        public int ShieldColdResist { get { return m_ShieldColdResist; } set { m_ShieldColdResist = value; } }
        public int ShieldPoisonResist { get { return m_ShieldPoisonResist; } set { m_ShieldPoisonResist = value; } }
        public int ShieldEnergyResist { get { return m_ShieldEnergyResist; } set { m_ShieldEnergyResist = value; } }
        #endregion
        #endregion

        #region pre-aos stuff
        public int OldMinRange { get; set; }
        public int OldMaxRange { get; set; }
        public int OldDamage { get; set; }
        public int OldSpeed { get; set; }
        public int OldWrestlerHitRate { get; set; }
        public int OldWrestlerEvasion { get; set; }
        public double OldStaticMultiply { get; set; }
        public int OldArmorBonus { get; set; }
        public int OldMalusDex { get; set; }
        public int OldMaxMagicalLevel { get; set; }
        public int OldMagicalLevelMalus { get; set; }
        public double OldSmeltingRequiredSkill { get; set; }
        public double OldAxeRequiredSkill { get; set; }
        #endregion

        public CraftAttributeInfo()
        {
        }

        public static readonly CraftAttributeInfo Blank;

        public static readonly CraftAttributeInfo OldDullCopper, OldShadowIron, OldCopper, OldBronze, OldGold, OldAgapite, OldVerite, OldValorite, OldGraphite, OldPyrite, OldAzurite, OldVanadium, OldSilver, OldPlatinum, OldAmethyst, OldTitanium, OldXenian, OldRubidian, OldObsidian, OldEbonSapphire, OldDarkRuby, OldRadiantDiamond;
        public static readonly CraftAttributeInfo DullCopper, ShadowIron, Copper, Bronze, Golden, Agapite, Verite, Valorite, Platinum, Titanium, Obsidian, DarkRuby, EbonSapphire, RadiantDiamond, Eldar, Crystaline, Vulcan, Aqua;
        public static readonly CraftAttributeInfo Spined, Horned, Barbed, Humanoid, Undead, Wolf, Aracnid, Fey, GreenDragon, BlackDragon, BlueDragon, RedDragon, Abyss;
        public static readonly CraftAttributeInfo Oak, Walnut, Cedar, Willow, Cypress, Apple, Pear, Peach, Banana, Yew, Ohii, Stonewood, Silver, Swamp, Blood, Crystal, Elder, Elven, Enchanted, Death;
        public static readonly CraftAttributeInfo RedScales, YellowScales, BlackScales, GreenScales, WhiteScales, BlueScales;
        // public static readonly CraftAttributeInfo OakWood, AshWood, YewWood, Heartwood, Bloodwood, Frostwood;
        public static readonly CraftAttributeInfo OldReptileLeather, OldArachnidLeather, OldOrcishLeather, OldLavaLeather, OldHumanoidLeather, OldUndeadLeather, OldOphidianLeather, OldArcticLeather, OldWolfLeather, OldBearLeather, OldGreenDragonLeather, OldBlueDragonLeather, OldBlackDragonLeather, OldRedDragonLeather, OldDemonLeather;

        static CraftAttributeInfo()
        {
            Blank = new CraftAttributeInfo();
            Blank.OldStaticMultiply = 1.0;
            Blank.OldArmorBonus = 0;
            Blank.OldMalusDex = 0;
            Blank.OldMaxMagicalLevel = 5;
            Blank.OldMagicalLevelMalus = 0;
            Blank.OldSmeltingRequiredSkill = 50.0;
            Blank.OldAxeRequiredSkill = 10.0;
            Blank.OldToolDurability = 00;

            #region ores

            #region pre-AoS stuff
            CraftAttributeInfo oldDullCopper = OldDullCopper = new CraftAttributeInfo();
            oldDullCopper.OldStaticMultiply = 1.03;
            oldDullCopper.OldArmorBonus = 2;
            oldDullCopper.OldMalusDex = 0;
            oldDullCopper.OldMaxMagicalLevel = 5;
            oldDullCopper.OldMagicalLevelMalus = 5;//0;
            oldDullCopper.OldSmeltingRequiredSkill = 60.0;
            oldDullCopper.ArmorDurability = 105;
            oldDullCopper.WeaponDurability = 105;
            oldDullCopper.OldToolDurability = 110;

            CraftAttributeInfo oldShadowIron = OldShadowIron = new CraftAttributeInfo();
            oldShadowIron.OldStaticMultiply = 1.04;
            oldShadowIron.OldArmorBonus = 2;
            oldShadowIron.OldMalusDex = -1;
            oldShadowIron.OldMaxMagicalLevel = 5;
            oldShadowIron.OldMagicalLevelMalus = 10;//0;
            oldShadowIron.OldSmeltingRequiredSkill = 62.0;
            oldShadowIron.ArmorDurability = 110;
            oldShadowIron.WeaponDurability = 110;
            oldShadowIron.OldToolDurability = 120;

            CraftAttributeInfo oldBronze = OldBronze = new CraftAttributeInfo();
            oldBronze.OldStaticMultiply = 1.05;
            oldBronze.OldArmorBonus = 3;
            oldBronze.OldMalusDex = 0;
            oldBronze.OldMaxMagicalLevel = 5;
            oldBronze.OldMagicalLevelMalus = 15;//10;
            oldBronze.OldSmeltingRequiredSkill = 64.0;
            oldBronze.ArmorDurability = 115;
            oldBronze.WeaponDurability = 115;
            oldBronze.OldToolDurability = 130;

            CraftAttributeInfo oldCopper = OldCopper = new CraftAttributeInfo();
            oldCopper.OldStaticMultiply = 1.06;
            oldCopper.OldArmorBonus = 3;
            oldCopper.OldMalusDex = -1;
            oldCopper.OldMaxMagicalLevel = 5;
            oldCopper.OldMagicalLevelMalus = 20;
            oldCopper.OldSmeltingRequiredSkill = 66.0;
            oldCopper.ArmorDurability = 120;
            oldCopper.WeaponDurability = 120;
            oldCopper.OldToolDurability = 140;

            CraftAttributeInfo oldGraphite = OldGraphite = new CraftAttributeInfo();
            oldGraphite.OldStaticMultiply = 1.08;
            oldGraphite.OldArmorBonus = 4;
            oldGraphite.OldMalusDex = -2;
            oldGraphite.OldMaxMagicalLevel = 5;
            oldGraphite.OldMagicalLevelMalus = 25;//30;
            oldGraphite.OldSmeltingRequiredSkill = 70.0;
            oldGraphite.ArmorDurability = 125;
            oldGraphite.WeaponDurability = 125;
            oldGraphite.OldToolDurability = 150;

            CraftAttributeInfo oldAgapite = OldAgapite = new CraftAttributeInfo();
            oldAgapite.OldStaticMultiply = 1.07;
            oldAgapite.OldArmorBonus = 4;
            oldAgapite.OldMalusDex = -2;
            oldAgapite.OldMaxMagicalLevel = 5;//4;
            oldAgapite.OldMagicalLevelMalus = 30;//0;
            oldAgapite.OldSmeltingRequiredSkill = 68.0;
            oldAgapite.ArmorDurability = 130;
            oldAgapite.WeaponDurability = 130;
            oldAgapite.OldToolDurability = 160;

            CraftAttributeInfo oldVerite = OldVerite = new CraftAttributeInfo();
            oldVerite.OldStaticMultiply = 1.09;
            oldVerite.OldArmorBonus = 5;
            oldVerite.OldMalusDex = -3;
            oldVerite.OldMaxMagicalLevel = 5;//4;
            oldVerite.OldMagicalLevelMalus = 35;//0;
            oldVerite.OldSmeltingRequiredSkill = 72.0;
            oldVerite.ArmorDurability = 135;
            oldVerite.WeaponDurability = 135;
            oldVerite.OldToolDurability = 170;

            CraftAttributeInfo oldValorite = OldValorite = new CraftAttributeInfo();
            oldValorite.OldStaticMultiply = 1.10;
            oldValorite.OldArmorBonus = 5;
            oldValorite.OldMalusDex = -4;
            oldValorite.OldMaxMagicalLevel = 5;//4;
            oldValorite.OldMagicalLevelMalus = 40;//10;
            oldValorite.OldSmeltingRequiredSkill = 74.0;
            oldValorite.ArmorDurability = 140;
            oldValorite.WeaponDurability = 140;
            oldValorite.OldToolDurability = 180;

            CraftAttributeInfo oldPyrite = OldPyrite = new CraftAttributeInfo();
            oldPyrite.OldStaticMultiply = 1.11;
            oldPyrite.OldArmorBonus = 6;
            oldPyrite.OldMalusDex = -5;
            oldPyrite.OldMaxMagicalLevel = 5;//4;
            oldPyrite.OldMagicalLevelMalus = 45;//10;
            oldPyrite.OldSmeltingRequiredSkill = 76.0;
            oldPyrite.ArmorDurability = 145;
            oldPyrite.WeaponDurability = 145;
            oldPyrite.OldToolDurability = 190;

            CraftAttributeInfo oldAzurite = OldAzurite = new CraftAttributeInfo();
            oldAzurite.OldStaticMultiply = 1.12;
            oldAzurite.OldArmorBonus = 6;
            oldAzurite.OldMalusDex = -5;
            oldAzurite.OldMaxMagicalLevel = 5;//4;
            oldAzurite.OldMagicalLevelMalus = 50;//20;
            oldAzurite.OldSmeltingRequiredSkill = 78.0;
            oldAzurite.ArmorDurability = 150;
            oldAzurite.WeaponDurability = 150;
            oldAzurite.OldToolDurability = 200;

            CraftAttributeInfo oldVanadium = OldVanadium = new CraftAttributeInfo();
            oldVanadium.OldStaticMultiply = 1.13;
            oldVanadium.OldArmorBonus = 7;
            oldVanadium.OldMalusDex = -6;
            oldVanadium.OldMaxMagicalLevel = 5;//4;
            oldVanadium.OldMagicalLevelMalus = 55;//30;
            oldVanadium.OldSmeltingRequiredSkill = 80.0;
            oldVanadium.ArmorDurability = 155;
            oldVanadium.WeaponDurability = 155;
            oldVanadium.OldToolDurability = 210;

            CraftAttributeInfo oldSilver = OldSilver = new CraftAttributeInfo();
            oldSilver.OldStaticMultiply = 1.14;
            oldSilver.OldArmorBonus = 7;
            oldSilver.OldMalusDex = -7;
            oldSilver.OldMaxMagicalLevel = 5;//3;
            oldSilver.OldMagicalLevelMalus = 60;//0;
            oldSilver.OldSmeltingRequiredSkill = 82.0;
            oldSilver.ArmorDurability = 160;
            oldSilver.WeaponDurability = 160;
            oldSilver.OldToolDurability = 220;

            CraftAttributeInfo oldPlatinum = OldPlatinum = new CraftAttributeInfo();
            oldPlatinum.OldStaticMultiply = 1.15;
            oldPlatinum.OldArmorBonus = 8;
            oldPlatinum.OldMalusDex = -8;
            oldPlatinum.OldMaxMagicalLevel = 5;//3;
            oldPlatinum.OldMagicalLevelMalus = 65;//10;
            oldPlatinum.OldSmeltingRequiredSkill = 84.0;
            oldPlatinum.ArmorDurability = 165;
            oldPlatinum.WeaponDurability = 165;
            oldPlatinum.OldToolDurability = 230;

            CraftAttributeInfo oldAmethyst = OldAmethyst = new CraftAttributeInfo();
            oldAmethyst.OldStaticMultiply = 1.16;
            oldAmethyst.OldArmorBonus = 8;
            oldAmethyst.OldMalusDex = -10;
            oldAmethyst.OldMaxMagicalLevel = 5;//3;
            oldAmethyst.OldMagicalLevelMalus = 70;//20;
            oldAmethyst.OldSmeltingRequiredSkill = 87.0;
            oldAmethyst.ArmorDurability = 170;
            oldAmethyst.WeaponDurability = 170;
            oldAmethyst.OldToolDurability = 240;

            CraftAttributeInfo oldTitanium = OldTitanium = new CraftAttributeInfo();
            oldTitanium.OldStaticMultiply = 1.17;
            oldTitanium.OldArmorBonus = 9;
            oldTitanium.OldMalusDex = -10;
            oldTitanium.OldMaxMagicalLevel = 5;//3;
            oldTitanium.OldMagicalLevelMalus = 75;//30;
            oldTitanium.OldSmeltingRequiredSkill = 90.0;
            oldTitanium.ArmorDurability = 175;
            oldTitanium.WeaponDurability = 175;
            oldTitanium.OldToolDurability = 250;

            CraftAttributeInfo oldGold = OldGold = new CraftAttributeInfo();
            oldGold.OldStaticMultiply = 1.18;
            oldGold.OldArmorBonus = 10;
            oldGold.OldMalusDex = -11;
            oldGold.OldMaxMagicalLevel = 5;//2;
            oldGold.OldMagicalLevelMalus = 80;//0;
            oldGold.OldSmeltingRequiredSkill = 93.0;
            oldGold.ArmorDurability = 180;
            oldGold.WeaponDurability = 180;
            oldGold.OldToolDurability = 260;

            CraftAttributeInfo oldXenian = OldXenian = new CraftAttributeInfo();
            oldXenian.OldStaticMultiply = 1.19;
            oldXenian.OldArmorBonus = 11;
            oldXenian.OldMalusDex = -12;
            oldXenian.OldMaxMagicalLevel = 5;//2;
            oldXenian.OldMagicalLevelMalus = 85;//10;
            oldXenian.OldSmeltingRequiredSkill = 96.0;
            oldXenian.ArmorDurability = 185;
            oldXenian.WeaponDurability = 185;
            oldXenian.OldToolDurability = 270;

            CraftAttributeInfo oldRubidian = OldRubidian = new CraftAttributeInfo();
            oldRubidian.OldStaticMultiply = 1.20;
            oldRubidian.OldArmorBonus = 12;
            oldRubidian.OldMalusDex = -12;
            oldRubidian.OldMaxMagicalLevel = 5;//2;
            oldRubidian.OldMagicalLevelMalus = 90;//20;
            oldRubidian.OldSmeltingRequiredSkill = 99.0;
            oldRubidian.ArmorDurability = 190;
            oldRubidian.WeaponDurability = 190;
            oldRubidian.OldToolDurability = 280;

            CraftAttributeInfo oldObsidian = OldObsidian = new CraftAttributeInfo();
            oldObsidian.OldStaticMultiply = 1.22;
            oldObsidian.OldArmorBonus = 13;
            oldObsidian.OldMalusDex = -13;
            oldObsidian.OldMaxMagicalLevel = 5;//2;
            oldObsidian.OldMagicalLevelMalus = 95;//30;
            oldObsidian.OldSmeltingRequiredSkill = 99.9;
            oldObsidian.ArmorDurability = 195;
            oldObsidian.WeaponDurability = 195;
            oldObsidian.OldToolDurability = 290;

            CraftAttributeInfo oldEbonSapphire = OldEbonSapphire = new CraftAttributeInfo();
            oldEbonSapphire.OldStaticMultiply = 1.24;
            oldEbonSapphire.OldArmorBonus = 14;
            oldEbonSapphire.OldMalusDex = -13;
            oldEbonSapphire.OldMaxMagicalLevel = 5;//1;
            oldEbonSapphire.OldMagicalLevelMalus = 95;//0;
            oldEbonSapphire.OldSmeltingRequiredSkill = 99.9;
            oldEbonSapphire.ArmorDurability = 200;
            oldEbonSapphire.WeaponDurability = 200;
            oldEbonSapphire.OldToolDurability = 300;

            CraftAttributeInfo oldDarkRuby = OldDarkRuby = new CraftAttributeInfo();
            oldDarkRuby.OldStaticMultiply = 1.26;
            oldDarkRuby.OldArmorBonus = 14;
            oldDarkRuby.OldMalusDex = -14;
            oldDarkRuby.OldMaxMagicalLevel = 5;//1;
            oldDarkRuby.OldMagicalLevelMalus = 95;//10;
            oldDarkRuby.OldSmeltingRequiredSkill = 99.9;
            oldDarkRuby.ArmorDurability = 205;
            oldDarkRuby.WeaponDurability = 205;
            oldDarkRuby.OldToolDurability = 310;

            CraftAttributeInfo oldRadiantDiamond = OldRadiantDiamond = new CraftAttributeInfo();
            oldRadiantDiamond.OldStaticMultiply = 1.28;
            oldRadiantDiamond.OldArmorBonus = 16;
            oldRadiantDiamond.OldMalusDex = -15;
            oldRadiantDiamond.OldMaxMagicalLevel = 5;//1;
            oldRadiantDiamond.OldMagicalLevelMalus = 95;//30;
            oldRadiantDiamond.OldSmeltingRequiredSkill = 99.9;
            oldRadiantDiamond.ArmorDurability = 210;
            oldRadiantDiamond.WeaponDurability = 210;
            oldRadiantDiamond.OldToolDurability = 400;
            #endregion

            #region AoS
            #region DullCopper
            CraftAttributeInfo dullCopper = DullCopper = new CraftAttributeInfo();

            dullCopper.ArmorPhysicalResist = 6;
            dullCopper.ShieldPhysicalResist = 6;
            dullCopper.ArmorDurability = 50;
            dullCopper.ArmorLowerRequirements = 20;
            dullCopper.WeaponDurability = 100;
            dullCopper.WeaponLowerRequirements = 50;
            dullCopper.RunicMinAttributes = 1;
            dullCopper.RunicMaxAttributes = 2;
            dullCopper.RunicMinIntensity = 40;
            dullCopper.RunicMaxIntensity = 100;
            #endregion

            #region ShadowIron
            CraftAttributeInfo shadowIron = ShadowIron = new CraftAttributeInfo();

            shadowIron.ArmorPhysicalResist = 2;
            shadowIron.ArmorFireResist = 1;
            shadowIron.ArmorEnergyResist = 5;
            shadowIron.ShieldPhysicalResist = 2;
            shadowIron.ShieldFireResist = 1;
            shadowIron.ShieldEnergyResist = 5;
            shadowIron.ArmorDurability = 100;
            shadowIron.WeaponColdDamage = 20;
            shadowIron.WeaponDurability = 50;
            shadowIron.RunicMinAttributes = 2;
            shadowIron.RunicMaxAttributes = 2;
            shadowIron.RunicMinIntensity = 45;
            shadowIron.RunicMaxIntensity = 100;
            #endregion

            #region Copper
            CraftAttributeInfo copper = Copper = new CraftAttributeInfo();

            copper.ArmorPhysicalResist = 1;
            copper.ArmorFireResist = 1;
            copper.ArmorPoisonResist = 5;
            copper.ArmorEnergyResist = 2;
            copper.ShieldPhysicalResist = 1;
            copper.ShieldFireResist = 1;
            copper.ShieldPoisonResist = 5;
            copper.ShieldEnergyResist = 2;

            copper.WeaponPoisonDamage = 10;
            copper.WeaponEnergyDamage = 20;
            copper.RunicMinAttributes = 2;
            copper.RunicMaxAttributes = 3;
            copper.RunicMinIntensity = 50;
            copper.RunicMaxIntensity = 100;
            #endregion

            #region Bronze
            CraftAttributeInfo bronze = Bronze = new CraftAttributeInfo();

            bronze.ArmorPhysicalResist = 3;
            bronze.ArmorColdResist = 5;
            bronze.ArmorPoisonResist = 1;
            bronze.ArmorEnergyResist = 1;
            bronze.ShieldPhysicalResist = 3;
            bronze.ShieldColdResist = 5;
            bronze.ShieldPoisonResist = 1;
            bronze.ShieldEnergyResist = 1;
            bronze.WeaponFireDamage = 40;
            bronze.RunicMinAttributes = 3;
            bronze.RunicMaxAttributes = 4;
            bronze.RunicMinIntensity = 60;
            bronze.RunicMaxIntensity = 100;
            #endregion

            #region Golden
            CraftAttributeInfo golden = Golden = new CraftAttributeInfo();

            golden.ArmorPhysicalResist = 1;
            golden.ArmorFireResist = 1;
            golden.ArmorColdResist = 2;
            golden.ArmorEnergyResist = 2;
            golden.ShieldPhysicalResist = 1;
            golden.ShieldFireResist = 1;
            golden.ShieldColdResist = 2;
            golden.ShieldEnergyResist = 2;
            golden.ArmorLuck = 40;
            golden.ArmorLowerRequirements = 30;
            golden.WeaponLuck = 40;
            golden.WeaponLowerRequirements = 50;
            golden.RunicMinAttributes = 3;
            golden.RunicMaxAttributes = 4;
            golden.RunicMinIntensity = 60;
            golden.RunicMaxIntensity = 100;
            #endregion

            #region Agapite
            CraftAttributeInfo agapite = Agapite = new CraftAttributeInfo();

            agapite.ArmorPhysicalResist = 2;
            agapite.ArmorFireResist = 3;
            agapite.ArmorColdResist = 2;
            agapite.ArmorPoisonResist = 2;
            agapite.ArmorEnergyResist = 2;
            agapite.ShieldPhysicalResist = 2;
            agapite.ShieldFireResist = 3;
            agapite.ShieldColdResist = 2;
            agapite.ShieldPoisonResist = 2;
            agapite.ShieldEnergyResist = 2;
            agapite.WeaponColdDamage = 30;
            agapite.WeaponEnergyDamage = 20;
            agapite.RunicMinAttributes = 4;
            agapite.RunicMaxAttributes = 4;
            agapite.RunicMinIntensity = 65;
            agapite.RunicMaxIntensity = 100;
            #endregion

            #region Verite
            CraftAttributeInfo verite = Verite = new CraftAttributeInfo();

            verite.ArmorPhysicalResist = 3;
            verite.ArmorFireResist = 3;
            verite.ArmorColdResist = 2;
            verite.ArmorPoisonResist = 3;
            verite.ArmorEnergyResist = 1;
            verite.ShieldPhysicalResist = 3;
            verite.ShieldFireResist = 3;
            verite.ShieldColdResist = 2;
            verite.ShieldPoisonResist = 3;
            verite.ShieldEnergyResist = 1;
            verite.WeaponPoisonDamage = 40;
            verite.WeaponEnergyDamage = 20;
            verite.RunicMinAttributes = 4;
            verite.RunicMaxAttributes = 5;
            verite.RunicMinIntensity = 70;
            verite.RunicMaxIntensity = 100;
            #endregion

            #region Valorite
            CraftAttributeInfo valorite = Valorite = new CraftAttributeInfo();

            valorite.ArmorPhysicalResist = 4;
            valorite.ArmorColdResist = 3;
            valorite.ArmorPoisonResist = 3;
            valorite.ArmorEnergyResist = 3;
            valorite.ShieldPhysicalResist = 4;
            valorite.ShieldColdResist = 3;
            valorite.ShieldPoisonResist = 3;
            valorite.ShieldEnergyResist = 3;
            valorite.ArmorDurability = 50;
            valorite.WeaponFireDamage = 10;
            valorite.WeaponColdDamage = 20;
            valorite.WeaponPoisonDamage = 10;
            valorite.WeaponEnergyDamage = 20;
            valorite.RunicMinAttributes = 5;
            valorite.RunicMaxAttributes = 5;
            valorite.RunicMinIntensity = 85;
            valorite.RunicMaxIntensity = 100;
            #endregion

            #region Platinum
            CraftAttributeInfo platinum = Platinum = new CraftAttributeInfo();

            platinum.ArmorPhysicalResist = 1;
            platinum.ArmorFireResist = 4;
            platinum.ArmorColdResist = 3;
            platinum.ArmorPoisonResist = 2;
            platinum.ArmorEnergyResist = 3;

            platinum.ShieldPhysicalResist = 1;
            platinum.ShieldFireResist = 4;
            platinum.ShieldColdResist = 3;
            platinum.ShieldPoisonResist = 2;
            platinum.ShieldEnergyResist = 3;

            platinum.ArmorDurability = 150;
            platinum.ArmorLowerRequirements = 50;
            platinum.ArmorSelfRepair = 2;

            platinum.WeaponColdDamage = 20;
            platinum.WeaponFireDamage = 20;

            platinum.WeaponDurability = 100;
            platinum.WeaponLowerRequirements = 50;
            platinum.WeaponResistEnergyBonus = 5;

            platinum.RunicMinAttributes = 5;
            platinum.RunicMaxAttributes = 5;
            platinum.RunicMinIntensity = 85;
            platinum.RunicMaxIntensity = 100;
            #endregion

            #region Titanium
            CraftAttributeInfo titanium = Titanium = new CraftAttributeInfo();

            titanium.ArmorPhysicalResist = 4;
            titanium.ArmorFireResist = 1;
            titanium.ArmorColdResist = 4;
            titanium.ArmorPoisonResist = 2;
            titanium.ArmorEnergyResist = 2;

            titanium.ShieldPhysicalResist = 4;
            titanium.ShieldFireResist = 1;
            titanium.ShieldColdResist = 4;
            titanium.ShieldPoisonResist = 2;
            titanium.ShieldEnergyResist = 2;

            titanium.ArmorDurability = 150;
            titanium.ArmorLowerRequirements = 70;
            titanium.ArmorSelfRepair = 3;

            titanium.WeaponDurability = 100;
            titanium.WeaponLowerRequirements = 70;
            titanium.WeaponSpeed = 5;

            titanium.WeaponEnergyDamage = 30;

            titanium.RunicMinAttributes = 5;
            titanium.RunicMaxAttributes = 5;
            titanium.RunicMinIntensity = 85;
            titanium.RunicMaxIntensity = 100;
            #endregion

            #region Obsidian
            CraftAttributeInfo obsidian = Obsidian = new CraftAttributeInfo();

            obsidian.ArmorPhysicalResist = 3;
            obsidian.ArmorFireResist = 6;
            obsidian.ArmorColdResist = 1;
            obsidian.ArmorPoisonResist = 2;
            obsidian.ArmorEnergyResist = 1;

            obsidian.ShieldPhysicalResist = 3;
            obsidian.ShieldFireResist = 6;
            obsidian.ShieldColdResist = 1;
            obsidian.ShieldPoisonResist = 2;
            obsidian.ShieldEnergyResist = 1;

            obsidian.ArmorLuck = 60;

            obsidian.WeaponLuck = 60;
            obsidian.WeaponResistFireBonus = 5;

            obsidian.WeaponFireDamage = 100;

            obsidian.RunicMinAttributes = 5;
            obsidian.RunicMaxAttributes = 5;
            obsidian.RunicMinIntensity = 85;
            obsidian.RunicMaxIntensity = 100;
            #endregion

            #region DarkRuby
            CraftAttributeInfo darkRuby = DarkRuby = new CraftAttributeInfo();

            darkRuby.ArmorPhysicalResist = 3;
            darkRuby.ArmorFireResist = 3;
            darkRuby.ArmorColdResist = 1;
            darkRuby.ArmorPoisonResist = 2;
            darkRuby.ArmorEnergyResist = 5;

            darkRuby.ShieldPhysicalResist = 3;
            darkRuby.ShieldFireResist = 3;
            darkRuby.ShieldColdResist = 1;
            darkRuby.ShieldPoisonResist = 2;
            darkRuby.ShieldEnergyResist = 5;

            darkRuby.ArmorDurability = 50;
            darkRuby.ArmorBonusStam = 1;

            darkRuby.WeaponDurability = 60;
            darkRuby.WeaponHitFireArea = 5;

            darkRuby.WeaponFireDamage = 35;
            darkRuby.WeaponEnergyDamage = 35;

            darkRuby.RunicMinAttributes = 5;
            darkRuby.RunicMaxAttributes = 5;
            darkRuby.RunicMinIntensity = 85;
            darkRuby.RunicMaxIntensity = 100;
            #endregion

            #region EbonSapphire
            CraftAttributeInfo ebonSapphire = EbonSapphire = new CraftAttributeInfo();

            ebonSapphire.ArmorPhysicalResist = 3;
            ebonSapphire.ArmorFireResist = 1;
            ebonSapphire.ArmorColdResist = 5;
            ebonSapphire.ArmorPoisonResist = 2;
            ebonSapphire.ArmorEnergyResist = 3;

            ebonSapphire.ShieldPhysicalResist = 3;
            ebonSapphire.ShieldFireResist = 1;
            ebonSapphire.ShieldColdResist = 5;
            ebonSapphire.ShieldPoisonResist = 2;
            ebonSapphire.ShieldEnergyResist = 3;

            ebonSapphire.ArmorDurability = 50;
            ebonSapphire.ArmorLowerRegCost = 5;

            ebonSapphire.WeaponDurability = 70;
            ebonSapphire.WeaponHitColdArea = 5;

            ebonSapphire.WeaponColdDamage = 100;

            ebonSapphire.RunicMinAttributes = 5;
            ebonSapphire.RunicMaxAttributes = 5;
            ebonSapphire.RunicMinIntensity = 85;
            ebonSapphire.RunicMaxIntensity = 100;
            #endregion

            #region RadiantDiamond
            CraftAttributeInfo radiantDiamond = RadiantDiamond = new CraftAttributeInfo();

            radiantDiamond.ArmorPhysicalResist = 4;
            radiantDiamond.ArmorFireResist = 3;
            radiantDiamond.ArmorColdResist = 1;
            radiantDiamond.ArmorPoisonResist = 2;
            radiantDiamond.ArmorEnergyResist = 4;

            radiantDiamond.ShieldPhysicalResist = 4;
            radiantDiamond.ShieldFireResist = 3;
            radiantDiamond.ShieldColdResist = 1;
            radiantDiamond.ShieldPoisonResist = 2;
            radiantDiamond.ShieldEnergyResist = 4;

            radiantDiamond.ArmorDurability = 50;
            radiantDiamond.ArmorRegenHits = 1;

            radiantDiamond.WeaponDurability = 80;
            radiantDiamond.WeaponHitEnergyArea = 5;

            radiantDiamond.WeaponEnergyDamage = 80;

            radiantDiamond.RunicMinAttributes = 5;
            radiantDiamond.RunicMaxAttributes = 5;
            radiantDiamond.RunicMinIntensity = 85;
            radiantDiamond.RunicMaxIntensity = 100;
            #endregion

            #region Eldar
            CraftAttributeInfo eldar = Eldar = new CraftAttributeInfo();

            eldar.ArmorPhysicalResist = 3;
            eldar.ArmorFireResist = 4;
            eldar.ArmorColdResist = 4;
            eldar.ArmorPoisonResist = 2;
            eldar.ArmorEnergyResist = 2;

            eldar.ShieldPhysicalResist = 3;
            eldar.ShieldFireResist = 4;
            eldar.ShieldColdResist = 4;
            eldar.ShieldPoisonResist = 2;
            eldar.ShieldEnergyResist = 2;

            eldar.ArmorLuck = 40;
            eldar.ArmorLowerRequirements = 50;
            eldar.ArmorSelfRepair = 4;

            eldar.WeaponLuck = 40;
            eldar.WeaponLowerRequirements = 50;
            eldar.WeaponSelfRepair = 4;

            eldar.RunicMinAttributes = 5;
            eldar.RunicMaxAttributes = 5;
            eldar.RunicMinIntensity = 85;
            eldar.RunicMaxIntensity = 100;
            #endregion

            #region Crystaline
            CraftAttributeInfo crystaline = Crystaline = new CraftAttributeInfo();

            crystaline.ArmorPhysicalResist = 2;
            crystaline.ArmorFireResist = 4;
            crystaline.ArmorColdResist = 4;
            crystaline.ArmorPoisonResist = 3;
            crystaline.ArmorEnergyResist = 2;

            crystaline.ShieldPhysicalResist = 2;
            crystaline.ShieldFireResist = 4;
            crystaline.ShieldColdResist = 4;
            crystaline.ShieldPoisonResist = 3;
            crystaline.ShieldEnergyResist = 2;

            crystaline.ArmorLowerRequirements = 50;
            crystaline.ArmorRegenHits = 2;

            crystaline.WeaponLowerRequirements = 50;
            crystaline.WeaponSelfRepair = 3;

            crystaline.WeaponPoisonDamage = 90;

            crystaline.RunicMinAttributes = 5;
            crystaline.RunicMaxAttributes = 5;
            crystaline.RunicMinIntensity = 85;
            crystaline.RunicMaxIntensity = 100;
            #endregion

            #region Vulcan
            CraftAttributeInfo vulcan = Vulcan = new CraftAttributeInfo();

            vulcan.ArmorPhysicalResist = 3;
            vulcan.ArmorFireResist = 4;
            vulcan.ArmorColdResist = 2;
            vulcan.ArmorPoisonResist = 2;
            vulcan.ArmorEnergyResist = 4;

            vulcan.ShieldPhysicalResist = 3;
            vulcan.ShieldFireResist = 4;
            vulcan.ShieldColdResist = 2;
            vulcan.ShieldPoisonResist = 2;
            vulcan.ShieldEnergyResist = 4;

            vulcan.ArmorDurability = 10;
            vulcan.ArmorNightSight = 1;

            vulcan.WeaponDurability = 10;
            vulcan.WeaponResistFireBonus = 3;
            vulcan.WeaponResistPhysicalBonus = 3;

            vulcan.WeaponFireDamage = 90;
            vulcan.WeaponPoisonDamage = 10;

            vulcan.RunicMinAttributes = 5;
            vulcan.RunicMaxAttributes = 5;
            vulcan.RunicMinIntensity = 85;
            vulcan.RunicMaxIntensity = 100;
            #endregion

            #region Aqua
            CraftAttributeInfo aqua = Aqua = new CraftAttributeInfo();

            aqua.ArmorPhysicalResist = 3;
            aqua.ArmorFireResist = 2;
            aqua.ArmorColdResist = 3;
            aqua.ArmorPoisonResist = 5;
            aqua.ArmorEnergyResist = 3;

            aqua.ShieldPhysicalResist = 3;
            aqua.ShieldFireResist = 2;
            aqua.ShieldColdResist = 3;
            aqua.ShieldPoisonResist = 5;
            aqua.ShieldEnergyResist = 3;

            aqua.ArmorDurability = 30;

            aqua.WeaponDurability = 30;
            aqua.WeaponResistPoisonBonus = 5;

            aqua.WeaponPoisonDamage = 100;

            aqua.RunicMinAttributes = 5;
            aqua.RunicMaxAttributes = 5;
            aqua.RunicMinIntensity = 85;
            aqua.RunicMaxIntensity = 100;
            #endregion
            #endregion

            #endregion

            #region leathers

            #region pre-AoS stuff
            //CraftAttributeInfo oldSpinedLeather = OldSpinedLeather = new CraftAttributeInfo();
            //oldSpinedLeather.OldStaticMultiply = 1.03;
            //oldSpinedLeather.OldArmorBonus = 9;
            //oldSpinedLeather.OldMalusDex = 0;
            //oldSpinedLeather.OldMaxMagicalLevel = 2;
            //oldSpinedLeather.OldMagicalLevelMalus = 0;

            //CraftAttributeInfo oldHornedLeather = OldHornedLeather = new CraftAttributeInfo();
            //oldHornedLeather.OldStaticMultiply = 1.00;
            //oldHornedLeather.OldArmorBonus = 15;
            //oldHornedLeather.OldMalusDex = 0;
            //oldHornedLeather.OldMaxMagicalLevel = 1;
            //oldHornedLeather.OldMagicalLevelMalus = 0;

            //CraftAttributeInfo oldBarbedLeather = OldBarbedLeather = new CraftAttributeInfo();
            //oldBarbedLeather.OldStaticMultiply = 1.00;
            //oldBarbedLeather.OldArmorBonus = 6;
            //oldBarbedLeather.OldMalusDex = 0;
            //oldBarbedLeather.OldMaxMagicalLevel = 3;
            //oldBarbedLeather.OldMagicalLevelMalus = 0;

            CraftAttributeInfo oldWolfLeather = OldWolfLeather = new CraftAttributeInfo();
            oldWolfLeather.OldStaticMultiply = 1.03;
            oldWolfLeather.OldArmorBonus = 4;
            oldWolfLeather.OldMalusDex = 0;
            oldWolfLeather.OldMaxMagicalLevel = 5;//4
            oldWolfLeather.OldMagicalLevelMalus = 20;//0

            CraftAttributeInfo oldBearLeather = OldBearLeather = new CraftAttributeInfo();
            oldBearLeather.OldStaticMultiply = 1.04;
            oldBearLeather.OldArmorBonus = 6;
            oldBearLeather.OldMalusDex = 0;
            oldBearLeather.OldMaxMagicalLevel = 5;//4;
            oldBearLeather.OldMagicalLevelMalus = 30;//10;

            CraftAttributeInfo arachnidLeather = OldArachnidLeather = new CraftAttributeInfo();
            arachnidLeather.OldStaticMultiply = 1.05;
            arachnidLeather.OldArmorBonus = 6;
            arachnidLeather.OldMalusDex = 0;
            arachnidLeather.OldMaxMagicalLevel = 5;//4;
            arachnidLeather.OldMagicalLevelMalus = 30;//10;

            CraftAttributeInfo oldReptileLeather = OldReptileLeather = new CraftAttributeInfo();
            oldReptileLeather.OldStaticMultiply = 1.06;
            oldReptileLeather.OldArmorBonus = 7;
            oldReptileLeather.OldMalusDex = 0;
            oldReptileLeather.OldMaxMagicalLevel = 5;//4;
            oldReptileLeather.OldMagicalLevelMalus = 40;//20;

            CraftAttributeInfo oldOrcishLeather = OldOrcishLeather = new CraftAttributeInfo();
            oldOrcishLeather.OldStaticMultiply = 1.07;
            oldOrcishLeather.OldArmorBonus = 8;
            oldOrcishLeather.OldMalusDex = 0;
            oldOrcishLeather.OldMaxMagicalLevel = 5;//4;
            oldOrcishLeather.OldMagicalLevelMalus = 50;//30;

            CraftAttributeInfo oldLavaLeather = OldLavaLeather = new CraftAttributeInfo();
            oldLavaLeather.OldStaticMultiply = 1.11;
            oldLavaLeather.OldArmorBonus = 10;
            oldLavaLeather.OldMalusDex = 0;
            oldLavaLeather.OldMaxMagicalLevel = 5;//3;
            oldLavaLeather.OldMagicalLevelMalus = 50;//30;

            CraftAttributeInfo oldHumanodLeather = OldHumanoidLeather = new CraftAttributeInfo();
            oldHumanodLeather.OldStaticMultiply = 1.08;
            oldHumanodLeather.OldArmorBonus = 8;
            oldHumanodLeather.OldMalusDex = 0;
            oldHumanodLeather.OldMaxMagicalLevel = 5;//3;
            oldHumanodLeather.OldMagicalLevelMalus = 20;//0;

            CraftAttributeInfo oldUndeadLeather = OldUndeadLeather = new CraftAttributeInfo();
            oldUndeadLeather.OldStaticMultiply = 1.09;
            oldUndeadLeather.OldArmorBonus = 9;
            oldUndeadLeather.OldMalusDex = 0;
            oldUndeadLeather.OldMaxMagicalLevel = 5;//3;
            oldUndeadLeather.OldMagicalLevelMalus = 30;//10;

            CraftAttributeInfo oldOphidianLeather = OldOphidianLeather = new CraftAttributeInfo();
            oldOphidianLeather.OldStaticMultiply = 1.10;
            oldOphidianLeather.OldArmorBonus = 9;
            oldOphidianLeather.OldMalusDex = 0;
            oldOphidianLeather.OldMaxMagicalLevel = 5;//3;
            oldOphidianLeather.OldMagicalLevelMalus = 40;//20;

            CraftAttributeInfo oldArticLeather = OldArcticLeather = new CraftAttributeInfo();
            oldArticLeather.OldStaticMultiply = 1.12;
            oldArticLeather.OldArmorBonus = 10;
            oldArticLeather.OldMalusDex = 0;
            oldArticLeather.OldMaxMagicalLevel = 5;//2;
            oldArticLeather.OldMagicalLevelMalus = 50;//0;

            CraftAttributeInfo oldGreenDragonLeather = OldGreenDragonLeather = new CraftAttributeInfo();
            oldGreenDragonLeather.OldStaticMultiply = 1.20;
            oldGreenDragonLeather.OldArmorBonus = 12;
            oldGreenDragonLeather.OldMalusDex = 0;
            oldGreenDragonLeather.OldMaxMagicalLevel = 5;//2;
            oldGreenDragonLeather.OldMagicalLevelMalus = 50;//10;

            CraftAttributeInfo oldBlueDragonLeather = OldBlueDragonLeather = new CraftAttributeInfo();
            oldBlueDragonLeather.OldStaticMultiply = 1.20;
            oldBlueDragonLeather.OldArmorBonus = 12;
            oldBlueDragonLeather.OldMalusDex = 0;
            oldBlueDragonLeather.OldMaxMagicalLevel = 5;//2;
            oldBlueDragonLeather.OldMagicalLevelMalus = 50;//20;

            CraftAttributeInfo oldBlackDragonLeather = OldBlackDragonLeather = new CraftAttributeInfo();
            oldBlackDragonLeather.OldStaticMultiply = 1.20;
            oldBlackDragonLeather.OldArmorBonus = 12;
            oldBlackDragonLeather.OldMalusDex = 0;
            oldBlackDragonLeather.OldMaxMagicalLevel = 5;//2;
            oldBlackDragonLeather.OldMagicalLevelMalus = 50;//30;

            CraftAttributeInfo oldRedDragonLeather = OldRedDragonLeather = new CraftAttributeInfo();
            oldRedDragonLeather.OldStaticMultiply = 1.25;
            oldRedDragonLeather.OldArmorBonus = 14;
            oldRedDragonLeather.OldMalusDex = 0;
            oldRedDragonLeather.OldMaxMagicalLevel = 5;//1;
            oldRedDragonLeather.OldMagicalLevelMalus = 50;//0;

            CraftAttributeInfo oldDemonLeather = OldDemonLeather = new CraftAttributeInfo();
            oldDemonLeather.OldStaticMultiply = 1.25;
            oldDemonLeather.OldArmorBonus = 16;
            oldDemonLeather.OldMalusDex = 0;
            oldDemonLeather.OldMaxMagicalLevel = 5;//1;
            oldDemonLeather.OldMagicalLevelMalus = 60;//10;
            #endregion

            #region AoS
            #region spined
            CraftAttributeInfo spined = Spined = new CraftAttributeInfo();
            if( Core.AOS )
            {
                spined.ArmorPhysicalResist = 5;
                spined.ArmorLuck = 40;

                spined.RunicMinAttributes = 1;
                spined.RunicMaxAttributes = 3;
                spined.RunicMinIntensity = 40;
                spined.RunicMaxIntensity = 100;
            }
            else
            {
                spined.OldArmorBonus = 10;
            }
            #endregion

            #region horned
            CraftAttributeInfo horned = Horned = new CraftAttributeInfo();
            if( Core.AOS )
            {
                horned.ArmorPhysicalResist = 2;
                horned.ArmorFireResist = 3;
                horned.ArmorColdResist = 2;
                horned.ArmorPoisonResist = 2;
                horned.ArmorEnergyResist = 2;

                horned.RunicMinAttributes = 3;
                horned.RunicMaxAttributes = 4;
                horned.RunicMinIntensity = 45;
                horned.RunicMaxIntensity = 100;
            }
            else
            {
                horned.OldArmorBonus = 12;
            }
            #endregion

            #region barbed
            CraftAttributeInfo barbed = Barbed = new CraftAttributeInfo();
            if( Core.AOS )
            {
                barbed.ArmorPhysicalResist = 2;
                barbed.ArmorFireResist = 1;
                barbed.ArmorColdResist = 2;
                barbed.ArmorPoisonResist = 3;
                barbed.ArmorEnergyResist = 4;

                barbed.RunicMinAttributes = 4;
                barbed.RunicMaxAttributes = 5;
                barbed.RunicMinIntensity = 50;
                barbed.RunicMaxIntensity = 100;
            }
            else
            {
                barbed.OldArmorBonus = 14;
            }
            #endregion

            #region humanoid
            CraftAttributeInfo humanoid = Humanoid = new CraftAttributeInfo();
            if( Core.AOS )
            {
                humanoid.ArmorPhysicalResist = 5;
                humanoid.ArmorFireResist = 4;
                humanoid.ArmorColdResist = 1;
                humanoid.ArmorPoisonResist = 0;
                humanoid.ArmorEnergyResist = 1;

                humanoid.ArmorDurability = 150;

                humanoid.RunicMinAttributes = 4;
                humanoid.RunicMaxAttributes = 5;
                humanoid.RunicMinIntensity = 40;
                humanoid.RunicMaxIntensity = 100;
            }
            else
            {
                humanoid.OldArmorBonus = 6;
            }
            #endregion

            #region undead
            CraftAttributeInfo undead = Undead = new CraftAttributeInfo();
            if( Core.AOS )
            {
                undead.ArmorPhysicalResist = 0;
                undead.ArmorFireResist = 0;
                undead.ArmorColdResist = 1;
                undead.ArmorPoisonResist = 5;
                undead.ArmorEnergyResist = 5;

                undead.RunicMinAttributes = 4;
                undead.RunicMaxAttributes = 5;
                undead.RunicMinIntensity = 40;
                undead.RunicMaxIntensity = 100;
            }
            else
            {
                undead.OldArmorBonus = 7;
            }
            #endregion

            #region wolf
            CraftAttributeInfo wolf = Wolf = new CraftAttributeInfo();
            if( Core.AOS )
            {
                wolf.ArmorPhysicalResist = 3;
                wolf.ArmorFireResist = 4;
                wolf.ArmorColdResist = 4;
                wolf.ArmorPoisonResist = 0;
                wolf.ArmorEnergyResist = 1;

                wolf.ArmorDurability = 50;

                wolf.RunicMinAttributes = 4;
                wolf.RunicMaxAttributes = 5;
                wolf.RunicMinIntensity = 40;
                wolf.RunicMaxIntensity = 100;
            }
            else
            {
                wolf.OldArmorBonus = 8;
            }
            #endregion

            #region aracnid
            CraftAttributeInfo aracnid = Aracnid = new CraftAttributeInfo();
            if( Core.AOS )
            {
                aracnid.ArmorPhysicalResist = 4;
                aracnid.ArmorFireResist = 3;
                aracnid.ArmorColdResist = 4;
                aracnid.ArmorPoisonResist = 1;
                aracnid.ArmorEnergyResist = 0;

                aracnid.RunicMinAttributes = 4;
                aracnid.RunicMaxAttributes = 5;
                aracnid.RunicMinIntensity = 40;
                aracnid.RunicMaxIntensity = 100;
            }
            else
            {
                aracnid.OldArmorBonus = 9;
            }
            #endregion

            #region fey
            CraftAttributeInfo fey = Fey = new CraftAttributeInfo();
            if( Core.AOS )
            {
                fey.ArmorPhysicalResist = 0;
                fey.ArmorFireResist = 3;
                fey.ArmorColdResist = 3;
                fey.ArmorPoisonResist = 3;
                fey.ArmorEnergyResist = 3;

                fey.ArmorLuck = 40;
                fey.ArmorLowerRegCost = 5;

                fey.RunicMinAttributes = 4;
                fey.RunicMaxAttributes = 5;
                fey.RunicMinIntensity = 40;
                fey.RunicMaxIntensity = 100;
            }
            else
            {
                fey.OldArmorBonus = 10;
            }
            #endregion

            #region greenDragon
            CraftAttributeInfo greenDragon = GreenDragon = new CraftAttributeInfo();
            if( Core.AOS )
            {
                greenDragon.ArmorPhysicalResist = 4;
                greenDragon.ArmorFireResist = 2;
                greenDragon.ArmorColdResist = 2;
                greenDragon.ArmorPoisonResist = 4;
                greenDragon.ArmorEnergyResist = 1;

                greenDragon.ArmorDurability = 30;
                greenDragon.ArmorRegenHits = 1;

                greenDragon.RunicMinAttributes = 4;
                greenDragon.RunicMaxAttributes = 5;
                greenDragon.RunicMinIntensity = 40;
                greenDragon.RunicMaxIntensity = 100;
            }
            else
            {
                greenDragon.OldArmorBonus = 10;
            }
            #endregion

            #region blackDragon
            CraftAttributeInfo blackDragon = BlackDragon = new CraftAttributeInfo();
            if( Core.AOS )
            {
                blackDragon.ArmorPhysicalResist = 3;
                blackDragon.ArmorFireResist = 1;
                blackDragon.ArmorColdResist = 2;
                blackDragon.ArmorPoisonResist = 3;
                blackDragon.ArmorEnergyResist = 5;

                blackDragon.ArmorDurability = 40;
                blackDragon.ArmorNightSight = 1;

                blackDragon.RunicMinAttributes = 4;
                blackDragon.RunicMaxAttributes = 5;
                blackDragon.RunicMinIntensity = 40;
                blackDragon.RunicMaxIntensity = 100;
            }
            else
            {
                blackDragon.OldArmorBonus = 12;
            }
            #endregion

            #region blueDragon
            CraftAttributeInfo blueDragon = BlueDragon = new CraftAttributeInfo();
            if( Core.AOS )
            {
                blueDragon.ArmorPhysicalResist = 3;
                blueDragon.ArmorFireResist = 1;
                blueDragon.ArmorColdResist = 5;
                blueDragon.ArmorPoisonResist = 3;
                blueDragon.ArmorEnergyResist = 2;

                blueDragon.ArmorDurability = 50;
                blueDragon.ArmorSelfRepair = 2;

                blueDragon.RunicMinAttributes = 4;
                blueDragon.RunicMaxAttributes = 5;
                blueDragon.RunicMinIntensity = 40;
                blueDragon.RunicMaxIntensity = 100;
            }
            else
            {
                blueDragon.OldArmorBonus = 12;
            }
            #endregion

            #region redDragon
            CraftAttributeInfo redDragon = RedDragon = new CraftAttributeInfo();
            if( Core.AOS )
            {
                redDragon.ArmorPhysicalResist = 3;
                redDragon.ArmorFireResist = 5;
                redDragon.ArmorColdResist = 2;
                redDragon.ArmorPoisonResist = 2;
                redDragon.ArmorEnergyResist = 2;

                redDragon.ArmorDurability = 60;
                redDragon.ArmorRegenHits = 2;

                redDragon.RunicMinAttributes = 4;
                redDragon.RunicMaxAttributes = 5;
                redDragon.RunicMinIntensity = 40;
                redDragon.RunicMaxIntensity = 100;
            }
            else
            {
                redDragon.OldArmorBonus = 14;
            }
            #endregion

            #region abyss
            CraftAttributeInfo abyss = Abyss = new CraftAttributeInfo();
            if( Core.AOS )
            {
                abyss.ArmorPhysicalResist = 4;
                abyss.ArmorFireResist = 2;
                abyss.ArmorColdResist = 2;
                abyss.ArmorPoisonResist = 5;
                abyss.ArmorEnergyResist = 2;

                abyss.ArmorDurability = 100;
                abyss.ArmorLowerRequirements = 100;

                abyss.RunicMinAttributes = 4;
                abyss.RunicMaxAttributes = 5;
                abyss.RunicMinIntensity = 40;
                abyss.RunicMaxIntensity = 100;
            }
            else
            {
                abyss.OldArmorBonus = 16;
            }
            #endregion
            #endregion

            #endregion

            #region legni

            #region oak
            CraftAttributeInfo oak = Oak = new CraftAttributeInfo();
            if( Core.AOS )
            {
                oak.ArmorPhysicalResist = 2;
                oak.ArmorFireResist = 2;
                oak.ArmorColdResist = 0;
                oak.ArmorPoisonResist = 0;
                oak.ArmorEnergyResist = 2;

                oak.ShieldPhysicalResist = 2;
                oak.ShieldFireResist = 2;
                oak.ShieldColdResist = 0;
                oak.ShieldPoisonResist = 0;
                oak.ShieldEnergyResist = 2;

                oak.ArmorLuck = 40;
                oak.WeaponLuck = 40;

                oak.WeaponPoisonDamage = 70;
            }
            else
            {
                oak.OldStaticMultiply = 1.03;
                oak.OldMinRange = 0;
                oak.OldMaxRange = 1;
                oak.OldDamage = 0;
                oak.OldSpeed = 0;
                oak.OldWrestlerEvasion = 1;
                oak.OldWrestlerHitRate = 6;
                oak.OldAxeRequiredSkill = 40.0;
                oak.OldToolDurability = 110;
            }

            oak.RunicMinAttributes = 1;
            oak.RunicMaxAttributes = 2;
            oak.RunicMinIntensity = 1;
            oak.RunicMaxIntensity = 50;
            #endregion

            #region walnut
            CraftAttributeInfo walnut = Walnut = new CraftAttributeInfo();
            if( Core.AOS )
            {
                walnut.ArmorPhysicalResist = 3;
                walnut.ArmorFireResist = 1;
                walnut.ArmorColdResist = 1;
                walnut.ArmorPoisonResist = 0;
                walnut.ArmorEnergyResist = 1;

                walnut.ShieldPhysicalResist = 3;
                walnut.ShieldFireResist = 1;
                walnut.ShieldColdResist = 1;
                walnut.ShieldPoisonResist = 0;
                walnut.ShieldEnergyResist = 1;

                walnut.WeaponDurability = 20;
                walnut.ArmorDurability = 20;

                walnut.WeaponColdDamage = 70;
            }
            else
            {
                walnut.OldStaticMultiply = 1.04;
                walnut.OldMinRange = 0;
                walnut.OldMaxRange = 0;
                walnut.OldDamage = -3;
                walnut.OldSpeed = 2;
                walnut.OldWrestlerEvasion = 1;
                walnut.OldWrestlerHitRate = 6;
                walnut.OldAxeRequiredSkill = 45.0;
                walnut.OldToolDurability = 115;
            }

            walnut.RunicMinAttributes = 1;
            walnut.RunicMaxAttributes = 2;
            walnut.RunicMinIntensity = 5;
            walnut.RunicMaxIntensity = 55;
            #endregion

            #region ohii
            CraftAttributeInfo ohii = Ohii = new CraftAttributeInfo();
            if( Core.AOS )
            {
                ohii.ArmorPhysicalResist = 2;
                ohii.ArmorFireResist = 0;
                ohii.ArmorColdResist = 2;
                ohii.ArmorPoisonResist = 0;
                ohii.ArmorEnergyResist = 3;

                ohii.ShieldPhysicalResist = 2;
                ohii.ShieldFireResist = 0;
                ohii.ShieldColdResist = 2;
                ohii.ShieldPoisonResist = 0;
                ohii.ShieldEnergyResist = 3;

                ohii.WeaponSpeed = 5;

                ohii.RunicMinAttributes = 1;
                ohii.RunicMaxAttributes = 2;
                ohii.RunicMinIntensity = 10;
                ohii.RunicMaxIntensity = 60;
            }
            else
            {
                ohii.OldStaticMultiply = 1.05;
                ohii.OldMinRange = 0;
                ohii.OldMaxRange = 0;
                ohii.OldDamage = 0;
                ohii.OldSpeed = 1;
                ohii.OldWrestlerEvasion = 1;
                ohii.OldWrestlerHitRate = 6;
                ohii.OldAxeRequiredSkill = 50.0;
                ohii.OldToolDurability = 120;
            }
            #endregion

            #region cedar
            CraftAttributeInfo cedar = Cedar = new CraftAttributeInfo();
            if( Core.AOS )
            {
                cedar.ArmorPhysicalResist = 1;
                cedar.ArmorFireResist = 0;
                cedar.ArmorColdResist = 0;
                cedar.ArmorPoisonResist = 5;
                cedar.ArmorEnergyResist = 1;

                cedar.ShieldPhysicalResist = 1;
                cedar.ShieldFireResist = 0;
                cedar.ShieldColdResist = 0;
                cedar.ShieldPoisonResist = 5;
                cedar.ShieldEnergyResist = 1;

                cedar.WeaponSpeed = 5;

                cedar.WeaponFireDamage = 70;

                cedar.RunicMinAttributes = 2;
                cedar.RunicMaxAttributes = 2;
                cedar.RunicMinIntensity = 15;
                cedar.RunicMaxIntensity = 65;
            }
            else
            {
                cedar.OldStaticMultiply = 1.06;
                cedar.OldMinRange = -1;
                cedar.OldMaxRange = 0;
                cedar.OldDamage = 0;
                cedar.OldSpeed = 0;
                cedar.OldWrestlerEvasion = 2;
                cedar.OldWrestlerHitRate = 5;
                cedar.OldAxeRequiredSkill = 55.0;
                cedar.OldToolDurability = 125;
            }
            #endregion

            #region willow
            CraftAttributeInfo willow = Willow = new CraftAttributeInfo();
            if( Core.AOS )
            {
                willow.ArmorPhysicalResist = 3;
                willow.ArmorFireResist = 0;
                willow.ArmorColdResist = 4;
                willow.ArmorPoisonResist = 1;
                willow.ArmorEnergyResist = 1;

                willow.ShieldPhysicalResist = 3;
                willow.ShieldFireResist = 0;
                willow.ShieldColdResist = 4;
                willow.ShieldPoisonResist = 1;
                willow.ShieldEnergyResist = 1;

                willow.WeaponDamage = 3;
                willow.RunicMinAttributes = 3;
                willow.RunicMaxAttributes = 3;
                willow.RunicMinIntensity = 20;
                willow.RunicMaxIntensity = 70;
            }
            else
            {
                willow.OldStaticMultiply = 1.07;
                willow.OldMinRange = 1;
                willow.OldMaxRange = 0;
                willow.OldDamage = 0;
                willow.OldSpeed = 2;
                willow.OldWrestlerEvasion = 2;
                willow.OldWrestlerHitRate = 5;
                willow.OldAxeRequiredSkill = 60.0;
                willow.OldToolDurability = 130;
            }
            #endregion

            #region cypress
            CraftAttributeInfo cypress = Cypress = new CraftAttributeInfo();
            if( Core.AOS )
            {
                cypress.ArmorPhysicalResist = 2;
                cypress.ArmorFireResist = 1;
                cypress.ArmorColdResist = 2;
                cypress.ArmorPoisonResist = 3;
                cypress.ArmorEnergyResist = 1;

                cypress.ShieldPhysicalResist = 2;
                cypress.ShieldFireResist = 1;
                cypress.ShieldColdResist = 2;
                cypress.ShieldPoisonResist = 3;
                cypress.ShieldEnergyResist = 1;

                cypress.WeaponPoisonDamage = 50;

                cypress.WeaponDamage = 5;
                cypress.RunicMinAttributes = 2;
                cypress.RunicMaxAttributes = 3;
                cypress.RunicMinIntensity = 25;
                cypress.RunicMaxIntensity = 75;
            }
            else
            {
                cypress.OldStaticMultiply = 1.08;
                cypress.OldMinRange = 0;
                cypress.OldMaxRange = -1;
                cypress.OldDamage = 0;
                cypress.OldSpeed = 2;
                cypress.OldWrestlerEvasion = 2;
                cypress.OldWrestlerHitRate = 5;
                cypress.OldAxeRequiredSkill = 65.0;
                cypress.OldToolDurability = 135;
            }
            #endregion

            #region yew
            CraftAttributeInfo yew = Yew = new CraftAttributeInfo();
            if( Core.AOS )
            {
                yew.ArmorPhysicalResist = 3;
                yew.ArmorFireResist = 2;
                yew.ArmorColdResist = 2;
                yew.ArmorPoisonResist = 0;
                yew.ArmorEnergyResist = 2;

                yew.ShieldPhysicalResist = 3;
                yew.ShieldFireResist = 2;
                yew.ShieldColdResist = 2;
                yew.ShieldPoisonResist = 0;
                yew.ShieldEnergyResist = 2;

                yew.WeaponEnergyDamage = 33;
                yew.WeaponFireDamage = 33;
                yew.WeaponColdDamage = 33;

                yew.WeaponHitLeechHits = 10;
                yew.RunicMinAttributes = 3;
                yew.RunicMaxAttributes = 3;
                yew.RunicMinIntensity = 30;
                yew.RunicMaxIntensity = 80;
            }
            else
            {
                yew.OldStaticMultiply = 1.09;
                yew.OldMinRange = -1;
                yew.OldMaxRange = 1;
                yew.OldDamage = -2;
                yew.OldSpeed = 1;
                yew.OldWrestlerEvasion = 3;
                yew.OldWrestlerHitRate = 4;
                yew.OldAxeRequiredSkill = 70.0;
                yew.OldToolDurability = 140;
            }
            #endregion

            #region apple
            CraftAttributeInfo apple = Apple = new CraftAttributeInfo();
            if( Core.AOS )
            {
                apple.ArmorPhysicalResist = 2;
                apple.ArmorFireResist = 0;
                apple.ArmorColdResist = 5;
                apple.ArmorPoisonResist = 0;
                apple.ArmorEnergyResist = 3;

                apple.ShieldPhysicalResist = 2;
                apple.ShieldFireResist = 0;
                apple.ShieldColdResist = 5;
                apple.ShieldPoisonResist = 0;
                apple.ShieldEnergyResist = 3;

                apple.ArmorDurability = 40;
                apple.ArmorLuck = 40;

                apple.WeaponDurability = 40;
                apple.WeaponLuck = 30;

                apple.WeaponEnergyDamage = 60;
                apple.WeaponFireDamage = 40;
                apple.RunicMinAttributes = 3;
                apple.RunicMaxAttributes = 3;
                apple.RunicMinIntensity = 35;
                apple.RunicMaxIntensity = 85;
            }
            else
            {
                apple.OldStaticMultiply = 1.10;
                apple.OldMinRange = 1;
                apple.OldMaxRange = 0;
                apple.OldDamage = 2;
                apple.OldSpeed = 0;
                apple.OldWrestlerEvasion = 3;
                apple.OldWrestlerHitRate = 4;
                apple.OldAxeRequiredSkill = 75.0;
                apple.OldToolDurability = 150;
            }
            #endregion

            #region pear
            CraftAttributeInfo pear = Pear = new CraftAttributeInfo();
            if( Core.AOS )
            {
                pear.ArmorPhysicalResist = 2;
                pear.ArmorFireResist = 0;
                pear.ArmorColdResist = 3;
                pear.ArmorPoisonResist = 0;
                pear.ArmorEnergyResist = 5;

                pear.ShieldPhysicalResist = 2;
                pear.ShieldFireResist = 0;
                pear.ShieldColdResist = 3;
                pear.ShieldPoisonResist = 0;
                pear.ShieldEnergyResist = 5;

                pear.ArmorDurability = 60;
                pear.ArmorLuck = 40;

                pear.WeaponDurability = 60;
                pear.WeaponLuck = 35;

                pear.WeaponEnergyDamage = 50;
                pear.WeaponPoisonDamage = 50;
                pear.RunicMinAttributes = 3;
                pear.RunicMaxAttributes = 4;
                pear.RunicMinIntensity = 40;
                pear.RunicMaxIntensity = 90;
            }
            else
            {
                pear.OldStaticMultiply = 1.11;
                pear.OldMinRange = -2;
                pear.OldMaxRange = 2;
                pear.OldDamage = 0;
                pear.OldSpeed = 0;
                pear.OldWrestlerEvasion = 3;
                pear.OldWrestlerHitRate = 4;
                pear.OldAxeRequiredSkill = 80.0;
                pear.OldToolDurability = 155;
            }
            #endregion

            #region peach
            CraftAttributeInfo peach = Peach = new CraftAttributeInfo();
            if( Core.AOS )
            {
                peach.ArmorPhysicalResist = 1;
                peach.ArmorFireResist = 3;
                peach.ArmorColdResist = 5;
                peach.ArmorPoisonResist = 1;
                peach.ArmorEnergyResist = 0;

                peach.ShieldPhysicalResist = 1;
                peach.ShieldFireResist = 3;
                peach.ShieldColdResist = 5;
                peach.ShieldPoisonResist = 1;
                peach.ShieldEnergyResist = 0;

                peach.ArmorDurability = 80;
                peach.ArmorLuck = 40;

                peach.WeaponDurability = 80;
                peach.WeaponLuck = 40;
                peach.RunicMinAttributes = 4;
                peach.RunicMaxAttributes = 4;
                peach.RunicMinIntensity = 45;
                peach.RunicMaxIntensity = 95;
            }
            else
            {
                peach.OldStaticMultiply = 1.12;
                peach.OldMinRange = 0;
                peach.OldMaxRange = -1;
                peach.OldDamage = 2;
                peach.OldSpeed = 0;
                peach.OldWrestlerEvasion = 4;
                peach.OldWrestlerHitRate = 3;
                peach.OldAxeRequiredSkill = 85.0;
                peach.OldToolDurability = 160;
            }
            #endregion

            #region banana
            CraftAttributeInfo banana = Banana = new CraftAttributeInfo();
            if( Core.AOS )
            {
                banana.ArmorPhysicalResist = 1;
                banana.ArmorFireResist = 5;
                banana.ArmorColdResist = 3;
                banana.ArmorPoisonResist = 0;
                banana.ArmorEnergyResist = 1;

                banana.ShieldPhysicalResist = 1;
                banana.ShieldFireResist = 5;
                banana.ShieldColdResist = 3;
                banana.ShieldPoisonResist = 0;
                banana.ShieldEnergyResist = 1;

                banana.ArmorDurability = 100;
                banana.ArmorLuck = 45;

                banana.WeaponDurability = 100;
                banana.WeaponLuck = 45;

                banana.WeaponPoisonDamage = 100;
                banana.RunicMinAttributes = 4;
                banana.RunicMaxAttributes = 4;
                banana.RunicMinIntensity = 50;
                banana.RunicMaxIntensity = 100;
            }
            else
            {
                banana.OldStaticMultiply = 1.13;
                banana.OldMinRange = 0;
                banana.OldMaxRange = 0;
                banana.OldDamage = 3;
                banana.OldSpeed = 0;
                banana.OldWrestlerEvasion = 4;
                banana.OldWrestlerHitRate = 3;
                banana.OldAxeRequiredSkill = 90.0;
                banana.OldToolDurability = 165;
            }
            #endregion

            #region stonewood
            CraftAttributeInfo stonewood = Stonewood = new CraftAttributeInfo();
            if( Core.AOS )
            {
                stonewood.ArmorPhysicalResist = 8;
                stonewood.ArmorFireResist = 1;
                stonewood.ArmorColdResist = 1;
                stonewood.ArmorPoisonResist = 0;
                stonewood.ArmorEnergyResist = 1;

                stonewood.ShieldPhysicalResist = 8;
                stonewood.ShieldFireResist = 1;
                stonewood.ShieldColdResist = 1;
                stonewood.ShieldPoisonResist = 0;
                stonewood.ShieldEnergyResist = 1;

                stonewood.ArmorDurability = 150;
                stonewood.WeaponDurability = 150;

                stonewood.ArmorNightSight = 1;
                stonewood.WeaponNightSight = 1;
                stonewood.RunicMinAttributes = 1;
                stonewood.RunicMaxAttributes = 3;
                stonewood.RunicMinIntensity = 5;
                stonewood.RunicMaxIntensity = 30;
            }
            else
            {
                stonewood.OldStaticMultiply = 1.14;
                stonewood.OldMinRange = -1;
                stonewood.OldMaxRange = 0;
                stonewood.OldDamage = -4;
                stonewood.OldSpeed = 3;
                stonewood.OldWrestlerEvasion = 4;
                stonewood.OldWrestlerEvasion = 3;
                stonewood.OldAxeRequiredSkill = 90.0;
                stonewood.OldToolDurability = 170;
            }
            #endregion

            #region silver
            CraftAttributeInfo silver = Silver = new CraftAttributeInfo();
            if( Core.AOS )
            {
                silver.ArmorPhysicalResist = 1;
                silver.ArmorFireResist = 3;
                silver.ArmorColdResist = 3;
                silver.ArmorPoisonResist = 1;
                silver.ArmorEnergyResist = 3;

                silver.ShieldPhysicalResist = 1;
                silver.ShieldFireResist = 3;
                silver.ShieldColdResist = 3;
                silver.ShieldPoisonResist = 1;
                silver.ShieldEnergyResist = 3;

                silver.ArmorLowerRequirements = 100;
                silver.WeaponSpeed = 5;

                silver.WeaponEnergyDamage = 70;
                silver.RunicMinAttributes = 1;
                silver.RunicMaxAttributes = 3;
                silver.RunicMinIntensity = 5;
                silver.RunicMaxIntensity = 30;
            }
            else
            {
                silver.OldStaticMultiply = 1.15;
                silver.OldMinRange = 0;
                silver.OldMaxRange = 1;
                silver.OldDamage = 4;
                silver.OldSpeed = 0;
                silver.OldWrestlerEvasion = 5;
                silver.OldWrestlerHitRate = 2;
                silver.OldAxeRequiredSkill = 90.0;
                silver.OldToolDurability = 175;
            }
            #endregion

            #region blood
            CraftAttributeInfo blood = Blood = new CraftAttributeInfo();
            if( Core.AOS )
            {
                blood.ArmorPhysicalResist = 2;
                blood.ArmorFireResist = 6;
                blood.ArmorColdResist = 0;
                blood.ArmorPoisonResist = 2;
                blood.ArmorEnergyResist = 2;

                blood.ShieldPhysicalResist = 2;
                blood.ShieldFireResist = 6;
                blood.ShieldColdResist = 0;
                blood.ShieldPoisonResist = 4;
                blood.ShieldEnergyResist = 2;

                blood.ArmorDefendChance = 5;
                blood.WeaponHitLeechHits = 10;
                blood.RunicMinAttributes = 1;
                blood.RunicMaxAttributes = 3;
                blood.RunicMinIntensity = 5;
                blood.RunicMaxIntensity = 30;
            }
            else
            {
                blood.OldStaticMultiply = 1.16;
                blood.OldMinRange = -1;
                blood.OldMaxRange = 2;
                blood.OldDamage = 3;
                blood.OldSpeed = 0;
                blood.OldWrestlerEvasion = 5;
                blood.OldWrestlerHitRate = 2;
                blood.OldAxeRequiredSkill = 90.0;
                blood.OldToolDurability = 180;
            }
            #endregion

            #region swamp
            CraftAttributeInfo swamp = Swamp = new CraftAttributeInfo();
            if( Core.AOS )
            {
                swamp.ArmorPhysicalResist = 2;
                swamp.ArmorFireResist = 1;
                swamp.ArmorColdResist = 1;
                swamp.ArmorPoisonResist = 6;
                swamp.ArmorEnergyResist = 2;

                swamp.ShieldPhysicalResist = 2;
                swamp.ShieldFireResist = 1;
                swamp.ShieldColdResist = 1;
                swamp.ShieldPoisonResist = 6;
                swamp.ShieldEnergyResist = 4;

                swamp.ArmorRegenHits = 1;
                swamp.WeaponSpeed = 5;
                swamp.RunicMinAttributes = 2;
                swamp.RunicMaxAttributes = 3;
                swamp.RunicMinIntensity = 5;
                swamp.RunicMaxIntensity = 35;
            }
            else
            {
                swamp.OldStaticMultiply = 1.18;
                swamp.OldMinRange = 0;
                swamp.OldMaxRange = 2;
                swamp.OldDamage = -3;
                swamp.OldSpeed = 3;
                swamp.OldWrestlerEvasion = 5;
                swamp.OldWrestlerHitRate = 2;
                swamp.OldAxeRequiredSkill = 90.0;
                swamp.OldToolDurability = 185;
            }
            #endregion

            #region crystal
            CraftAttributeInfo crystal = Crystal = new CraftAttributeInfo();
            if( Core.AOS )
            {
                crystal.ArmorPhysicalResist = 2;
                crystal.ArmorFireResist = 1;
                crystal.ArmorColdResist = 5;
                crystal.ArmorPoisonResist = 2;
                crystal.ArmorEnergyResist = 3;

                crystal.ShieldPhysicalResist = 2;
                crystal.ShieldFireResist = 1;
                crystal.ShieldColdResist = 7;
                crystal.ShieldPoisonResist = 2;
                crystal.ShieldEnergyResist = 3;

                crystal.ArmorLowerRequirements = 50;
                crystal.WeaponDamage = 10;

                crystal.WeaponEnergyDamage = 50;
                crystal.WeaponColdDamage = 50;
                crystal.RunicMinAttributes = 1;
                crystal.RunicMaxAttributes = 3;
                crystal.RunicMinIntensity = 5;
                crystal.RunicMaxIntensity = 30;
            }
            else
            {
                crystal.OldStaticMultiply = 1.20;
                crystal.OldMinRange = 0;
                crystal.OldMaxRange = 4;
                crystal.OldDamage = 0;
                crystal.OldSpeed = 2;
                crystal.OldWrestlerEvasion = 6;
                crystal.OldWrestlerHitRate = 1;
                crystal.OldAxeRequiredSkill = 90.0;
                crystal.OldToolDurability = 190;
            }
            #endregion

            #region elven
            CraftAttributeInfo elven = Elven = new CraftAttributeInfo();
            if( Core.AOS )
            {
                elven.ArmorPhysicalResist = 2;
                elven.ArmorFireResist = 2;
                elven.ArmorColdResist = 2;
                elven.ArmorPoisonResist = 5;
                elven.ArmorEnergyResist = 2;

                elven.ShieldPhysicalResist = 2;
                elven.ShieldFireResist = 4;
                elven.ShieldColdResist = 2;
                elven.ShieldPoisonResist = 5;
                elven.ShieldEnergyResist = 2;

                elven.ArmorRegenHits = 2;

                elven.WeaponEnergyDamage = 100;

                elven.WeaponSpeed = 10;
            }
            else
            {
                elven.OldMinRange = 0;
                elven.OldMaxRange = 0;
                elven.OldDamage = 0;
                elven.OldSpeed = 0;
                elven.OldAxeRequiredSkill = 90.0;
                elven.OldWrestlerEvasion = 16;
                elven.OldToolDurability = 195;
            }

            elven.RunicMinAttributes = 2;
            elven.RunicMaxAttributes = 4;
            elven.RunicMinIntensity = 10;
            elven.RunicMaxIntensity = 40;
            #endregion

            #region elder
            CraftAttributeInfo elder = Elder = new CraftAttributeInfo();
            if( Core.AOS )
            {
                elder.ArmorPhysicalResist = 2;
                elder.ArmorFireResist = 3;
                elder.ArmorColdResist = 3;
                elder.ArmorPoisonResist = 0;
                elder.ArmorEnergyResist = 6;

                elder.ShieldPhysicalResist = 2;
                elder.ShieldFireResist = 3;
                elder.ShieldColdResist = 3;
                elder.ShieldPoisonResist = 0;
                elder.ShieldEnergyResist = 6;

                elder.WeaponColdDamage = 100;

                elder.WeaponNightSight = 1;
                elder.ArmorNightSight = 1;
            }
            else
            {
                elder.OldMinRange = 0;
                elder.OldMaxRange = 0;
                elder.OldDamage = 0;
                elder.OldSpeed = 0;
                elder.OldWrestlerEvasion = 17;
                elder.OldAxeRequiredSkill = 90.0;
                elder.OldToolDurability = 200;
            }

            elder.RunicMinAttributes = 2;
            elder.RunicMaxAttributes = 4;
            elder.RunicMinIntensity = 10;
            elder.RunicMaxIntensity = 40;
            #endregion

            #region enchanted
            CraftAttributeInfo enchanted = Enchanted = new CraftAttributeInfo();
            if( Core.AOS )
            {
                enchanted.ArmorPhysicalResist = 0;
                enchanted.ArmorFireResist = 5;
                enchanted.ArmorColdResist = 4;
                enchanted.ArmorPoisonResist = 1;
                enchanted.ArmorEnergyResist = 4;

                enchanted.ShieldPhysicalResist = 0;
                enchanted.ShieldFireResist = 6;
                enchanted.ShieldColdResist = 5;
                enchanted.ShieldPoisonResist = 1;
                enchanted.ShieldEnergyResist = 4;

                enchanted.ArmorLowerRegCost = 10;
                enchanted.WeaponSpeed = 5;

                enchanted.WeaponFireDamage = 100;
                enchanted.RunicMinAttributes = 1;
                enchanted.RunicMaxAttributes = 3;
                enchanted.RunicMinIntensity = 5;
                enchanted.RunicMaxIntensity = 30;
            }
            else
            {
                enchanted.OldStaticMultiply = 1.25;
                enchanted.OldMinRange = -1;
                enchanted.OldMaxRange = 3;
                enchanted.OldDamage = 3;
                enchanted.OldSpeed = 3;
                enchanted.OldWrestlerEvasion = 6;
                enchanted.OldWrestlerHitRate = 1;
                enchanted.OldAxeRequiredSkill = 90.0;
                enchanted.OldToolDurability = 205;
            }
            #endregion

            #region death
            CraftAttributeInfo death = Death = new CraftAttributeInfo();
            if( Core.AOS )
            {
                death.ArmorPhysicalResist = 2;
                death.ArmorFireResist = 2;
                death.ArmorColdResist = 4;
                death.ArmorPoisonResist = 6;
                death.ArmorEnergyResist = 1;

                death.ShieldPhysicalResist = 5;
                death.ShieldFireResist = 2;
                death.ShieldColdResist = 4;
                death.ShieldPoisonResist = 6;
                death.ShieldEnergyResist = 1;

                death.WeaponPoisonDamage = 100;

                death.ArmorSelfRepair = 2;
                death.WeaponHitLeechHits = 10;
            }
            else
            {
                death.OldMinRange = 0;
                death.OldMaxRange = 0;
                death.OldDamage = 0;
                death.OldSpeed = 0;
                death.OldAxeRequiredSkill = 0.0;
                death.OldToolDurability = 210;
            }

            death.RunicMinAttributes = 3;
            death.RunicMaxAttributes = 5;
            death.RunicMinIntensity = 15;
            death.RunicMaxIntensity = 50;
            #endregion

            #endregion

            #region scales

            CraftAttributeInfo red = RedScales = new CraftAttributeInfo();

            red.ArmorPhysicalResist = 3;
            red.ArmorFireResist = 9;
            red.ArmorColdResist = -2;
            red.ArmorPoisonResist = 3;
            red.ArmorEnergyResist = 3;

            red.ArmorDurability = 20;
            red.ArmorLuck = 10;

            red.ArmorRegenHits = 1;

            if( Core.T2A )
            {
                red.OldStaticMultiply = 1.06;
                red.OldArmorBonus = 3;
                red.OldMalusDex = -1;
                red.OldMaxMagicalLevel = 5;
                red.OldMagicalLevelMalus = 20;
                red.OldSmeltingRequiredSkill = 66.0;
            }

            CraftAttributeInfo yellow = YellowScales = new CraftAttributeInfo();

            yellow.ArmorPhysicalResist = -2;
            yellow.ArmorFireResist = 5;
            yellow.ArmorColdResist = 3;
            yellow.ArmorPoisonResist = 5;
            yellow.ArmorEnergyResist = 4;

            yellow.ArmorDurability = 10;
            yellow.ArmorLuck = 20;

            yellow.ArmorSelfRepair = 2;

            if( Core.T2A )
            {
                yellow.OldStaticMultiply = 1.06;
                yellow.OldArmorBonus = 3;
                yellow.OldMalusDex = -1;
                yellow.OldMaxMagicalLevel = 5;
                yellow.OldMagicalLevelMalus = 20;
                yellow.OldSmeltingRequiredSkill = 66.0;
            }

            CraftAttributeInfo black = BlackScales = new CraftAttributeInfo();

            black.ArmorPhysicalResist = 9;
            black.ArmorFireResist = 3;
            black.ArmorColdResist = 3;
            black.ArmorPoisonResist = 3;
            black.ArmorEnergyResist = -2;

            black.ArmorDurability = 30;
            black.ArmorLuck = 0;

            black.ArmorReflectPhysical = 2;

            if( Core.T2A )
            {
                black.OldStaticMultiply = 1.17;
                black.OldArmorBonus = 9;
                black.OldMalusDex = -10;
                black.OldMaxMagicalLevel = 5;//3;
                black.OldMagicalLevelMalus = 70;//30;
                black.OldSmeltingRequiredSkill = 90.0;
            }

            CraftAttributeInfo green = GreenScales = new CraftAttributeInfo();

            green.ArmorPhysicalResist = 3;
            green.ArmorFireResist = -2;
            green.ArmorColdResist = 3;
            green.ArmorPoisonResist = 9;
            green.ArmorEnergyResist = 3;

            green.ArmorDurability = 30;
            green.ArmorLuck = 0;

            green.ArmorLowerRequirements = 50;

            if( Core.T2A )
            {
                green.OldStaticMultiply = 1.14;
                green.OldArmorBonus = 7;
                green.OldMalusDex = -7;
                green.OldMaxMagicalLevel = 5;//3;
                green.OldMagicalLevelMalus = 70;//0;
                green.OldSmeltingRequiredSkill = 82.0;
            }

            CraftAttributeInfo white = WhiteScales = new CraftAttributeInfo();

            white.ArmorPhysicalResist = -2;
            white.ArmorFireResist = 3;
            white.ArmorColdResist = 9;
            white.ArmorPoisonResist = 3;
            white.ArmorEnergyResist = 3;

            white.ArmorDurability = 30;
            white.ArmorLuck = 0;

            white.ArmorNightSight = 1;

            if( Core.T2A )
            {
                white.OldStaticMultiply = 1.10;
                white.OldArmorBonus = 5;
                white.OldMalusDex = -4;
                white.OldMaxMagicalLevel = 5;//4;
                white.OldMagicalLevelMalus = 60;//10;
                white.OldSmeltingRequiredSkill = 74.0;
            }

            CraftAttributeInfo blue = BlueScales = new CraftAttributeInfo();

            blue.ArmorPhysicalResist = 3;
            blue.ArmorFireResist = 3;
            blue.ArmorColdResist = 3;
            blue.ArmorPoisonResist = -2;
            blue.ArmorEnergyResist = 9;

            blue.ArmorDurability = 0;
            blue.ArmorLuck = 60;

            blue.ArmorLowerRegCost = 5;

            if( Core.T2A )
            {
                blue.OldStaticMultiply = 1.11;
                blue.OldArmorBonus = 6;
                blue.OldMalusDex = -5;
                blue.OldMaxMagicalLevel = 5;//4;
                blue.OldMagicalLevelMalus = 60;//10;
                blue.OldSmeltingRequiredSkill = 76.0;
            }

            /*
            #region Mondain's Legacy
            CraftAttributeInfo oak = OakWood = new CraftAttributeInfo();

            oak.ArmorPhysicalResist = 3;
            oak.ArmorFireResist = 3;
            oak.ArmorPoisonResist = 2;
            oak.ArmorEnergyResist = 3;
            oak.ArmorLuck = 40;
            oak.ShieldPhysicalResist = 1;
            oak.ShieldFireResist = 1;
            oak.ShieldColdResist = 1;
            oak.ShieldPoisonResist = 1;
            oak.ShieldEnergyResist = 1;
            oak.WeaponLuck = 40;
            oak.WeaponDamage = 5;			
            oak.RunicMinAttributes = 1;
            oak.RunicMaxAttributes = 2;
            oak.RunicMinIntensity = 1;
            oak.RunicMaxIntensity = 50;
			
            CraftAttributeInfo ash = AshWood = new CraftAttributeInfo();

            ash.ArmorPhysicalResist = 4;
            ash.ArmorFireResist = 2;
            ash.ArmorColdResist = 4;
            ash.ArmorPoisonResist = 1;
            ash.ArmorEnergyResist = 6;
            ash.ArmorLowerRequirements = 20;
            ash.ShieldEnergyResist = 3;
            ash.WeaponSwingSpeed = 10;
            ash.WeaponLowerRequirements = 20;			
            ash.RunicMinAttributes = 2;
            ash.RunicMaxAttributes = 3;
            ash.RunicMinIntensity = 35;
            ash.RunicMaxIntensity = 75;
			
            CraftAttributeInfo yew = YewWood = new CraftAttributeInfo();

            yew.ArmorPhysicalResist = 6;
            yew.ArmorFireResist = 3;
            yew.ArmorColdResist = 3;
            yew.ArmorEnergyResist = 3;
            yew.ArmorRegenHits = 1;
            yew.ShieldPhysicalResist = 3;
            yew.WeaponHitChance = 5;
            yew.WeaponDamage = 10;			
            yew.RunicMinAttributes = 3;
            yew.RunicMaxAttributes = 3;
            yew.RunicMinIntensity = 40;
            yew.RunicMaxIntensity = 90;		
			
            CraftAttributeInfo heartwood = Heartwood = new CraftAttributeInfo();
			
            heartwood.ArmorPhysicalResist = 2;
            heartwood.ArmorFireResist = 3;
            heartwood.ArmorColdResist = 2;
            heartwood.ArmorPoisonResist = 7;
            heartwood.ArmorEnergyResist = 2;
			
            // one of below
            heartwood.ArmorDamage = 10;
            heartwood.ArmorHitChance = 5;
            heartwood.ArmorLuck = 40;
            heartwood.ArmorLowerRequirements = 20;
            heartwood.ArmorMage = 1;
			
            // one of below
            heartwood.WeaponDamage = 10;
            heartwood.WeaponHitChance = 5;		
            heartwood.WeaponHitLifeLeech = 13;
            heartwood.WeaponLuck = 40;
            heartwood.WeaponLowerRequirements = 20;	
            heartwood.WeaponSwingSpeed = 10;			
			
            heartwood.RunicMinAttributes = 4;
            heartwood.RunicMaxAttributes = 4;
            heartwood.RunicMinIntensity = 50;
            heartwood.RunicMaxIntensity = 100;
			
            CraftAttributeInfo bloodwood = Bloodwood = new CraftAttributeInfo();

            bloodwood.ArmorPhysicalResist = 3;
            bloodwood.ArmorFireResist = 8;
            bloodwood.ArmorColdResist = 1;
            bloodwood.ArmorPoisonResist = 3;
            bloodwood.ArmorEnergyResist = 3;
            bloodwood.ArmorRegenHits = 2;
            bloodwood.ShieldFireResist = 3;
            bloodwood.WeaponRegenHits = 2;
            bloodwood.WeaponHitLifeLeech = 16;
			
            CraftAttributeInfo frostwood = Frostwood = new CraftAttributeInfo();

            frostwood.ArmorPhysicalResist = 2;
            frostwood.ArmorFireResist = 1;
            frostwood.ArmorColdResist = 8;
            frostwood.ArmorPoisonResist = 3;
            frostwood.ArmorEnergyResist = 4;
            frostwood.ShieldColdResist = 3;
            frostwood.WeaponColdDamage = 40;
            frostwood.WeaponDamage = 12;
            #endregion
            */

            #endregion
        }
    }

    public class CraftResourceInfo
    {
        private int m_Hue;
        private int m_Number;
        private string m_Name;
        private CraftAttributeInfo m_AttributeInfo;
        private CraftResource m_Resource;
        private Type[] m_ResourceTypes;

        public int Hue
        {
            get
            {
                return m_Hue;
            }
        }

        public int Number { get { return m_Number; } }
        public string Name { get { return m_Name; } }
        public CraftAttributeInfo AttributeInfo { get { return m_AttributeInfo; } }
        public CraftResource Resource { get { return m_Resource; } }
        public Type[] ResourceTypes { get { return m_ResourceTypes; } }

        public CraftResourceInfo( int hue, int number, string name, CraftAttributeInfo attributeInfo, CraftResource resource, params Type[] resourceTypes )
        {
            m_Hue = hue;
            m_Number = number;
            m_Name = name;
            m_AttributeInfo = attributeInfo;
            m_Resource = resource;
            m_ResourceTypes = resourceTypes;

            for( int i = 0; i < resourceTypes.Length; ++i )
                CraftResources.RegisterType( resourceTypes[ i ], resource );
        }
    }

    public class CraftResources
    {
        #region m_MetalInfo
        private static CraftResourceInfo[] m_MetalInfo = new CraftResourceInfo[]
			{
				new CraftResourceInfo( 0x000, 1053109, "Iron",			CraftAttributeInfo.Blank,			CraftResource.Iron,				typeof( IronIngot ),			typeof( IronOre ),			typeof( Granite ) ),
				new CraftResourceInfo( 0x973, 1053108, "Dull Copper",	CraftAttributeInfo.DullCopper,		CraftResource.DullCopper,		typeof( DullCopperIngot ),		typeof( DullCopperOre ),	typeof( DullCopperGranite ) ),
				new CraftResourceInfo( 0x966, 1053107, "Shadow Iron",	CraftAttributeInfo.ShadowIron,		CraftResource.ShadowIron,		typeof( ShadowIronIngot ),		typeof( ShadowIronOre ),	typeof( ShadowIronGranite ) ),
				new CraftResourceInfo( 0x96D, 1053106, "Copper",		CraftAttributeInfo.Copper,			CraftResource.Copper,			typeof( CopperIngot ),			typeof( CopperOre ),		typeof( CopperGranite ) ),
				new CraftResourceInfo( 0x972, 1053105, "Bronze",		CraftAttributeInfo.Bronze,			CraftResource.Bronze,			typeof( BronzeIngot ),			typeof( BronzeOre ),		typeof( BronzeGranite ) ),
				new CraftResourceInfo( 0x8A5, 1053104, "Gold",			CraftAttributeInfo.Golden,			CraftResource.Gold,				typeof( GoldIngot ),			typeof( GoldOre ),			typeof( GoldGranite ) ),
				new CraftResourceInfo( 0x979, 1053103, "Agapite",		CraftAttributeInfo.Agapite,			CraftResource.Agapite,			typeof( AgapiteIngot ),			typeof( AgapiteOre ),		typeof( AgapiteGranite ) ),
				new CraftResourceInfo( 0x89F, 1053102, "Verite",		CraftAttributeInfo.Verite,			CraftResource.Verite,			typeof( VeriteIngot ),			typeof( VeriteOre ),		typeof( VeriteGranite ) ),
				new CraftResourceInfo( 0x8AB, 1053101, "Valorite",		CraftAttributeInfo.Valorite,		CraftResource.Valorite,			typeof( ValoriteIngot ),		typeof( ValoriteOre ),		typeof( ValoriteGranite ) ),
				new CraftResourceInfo( 0x7d7, 1064700, "Platinum",		CraftAttributeInfo.Platinum,		CraftResource.Platinum,			typeof( PlatinumIngot ),		typeof( PlatinumOre ),		typeof( PlatinumGranite ) ),
				new CraftResourceInfo( 0x54a, 1064701, "Titanium",		CraftAttributeInfo.Titanium,		CraftResource.Titanium,			typeof( TitaniumIngot ),		typeof( TitaniumOre ),		typeof( TitaniumGranite ) ),
				new CraftResourceInfo( 0x77E, 1064702, "Obsidian",		CraftAttributeInfo.Obsidian,		CraftResource.Obsidian,			typeof( ObsidianIngot ),		typeof( ObsidianOre ),		typeof( ObsidianGranite ) ),
				new CraftResourceInfo( 0x78D, 1064703, "Dark Ruby",		CraftAttributeInfo.DarkRuby,		CraftResource.DarkRuby,			typeof( DarkRubyIngot ),		typeof( DarkRubyOre ),		typeof( DarkRubyGranite ) ),
				new CraftResourceInfo( 0x78C, 1064704, "Ebon Sapphire",	CraftAttributeInfo.EbonSapphire,	CraftResource.EbonSapphire,		typeof( EbonSapphireIngot ),	typeof( EbonSapphireOre ),	typeof( EbonSapphireGranite ) ),
				new CraftResourceInfo( 0x791, 1064705, "Radiant Diamond",CraftAttributeInfo.RadiantDiamond,	CraftResource.RadiantDiamond,	typeof( RadiantDiamondIngot ),	typeof( RadiantDiamondOre ),typeof( RadiantDiamondGranite ) ),
				new CraftResourceInfo( 0x7E8, 1064706, "Eldar",			CraftAttributeInfo.Eldar,			CraftResource.Eldar,			typeof( EldarIngot ),			typeof( EldarOre ),			typeof( EldarGranite ) ),
				new CraftResourceInfo( 0x9A1, 1064707, "Crystaline",	CraftAttributeInfo.Crystaline,		CraftResource.Crystaline,		typeof( CrystalineIngot ),		typeof( CrystalineOre ),	typeof( CrystalineGranite ) ),
				new CraftResourceInfo( 0x98E, 1064708, "Vulcan",		CraftAttributeInfo.Vulcan,			CraftResource.Vulcan,			typeof( VulcanIngot ),			typeof( VulcanOre ),		typeof( VulcanGranite ) ),
				new CraftResourceInfo( 0x98F, 1064709, "Aqua",			CraftAttributeInfo.Aqua,			CraftResource.Aqua,				typeof( AquaIngot ),			typeof( AquaOre ),			typeof( AquaGranite ) ),	
		};
        #endregion

        #region m_WoodInfo
        private static CraftResourceInfo[] m_WoodInfo = new CraftResourceInfo[]
			{
				new CraftResourceInfo( 0,	 1064711, "Log",			CraftAttributeInfo.Blank,				CraftResource.RegularWood,		typeof( Log ) ),
				new CraftResourceInfo( 2195, 1064712, "Oak",			CraftAttributeInfo.Oak,					CraftResource.Oak,				typeof( OakLog ) ),
				new CraftResourceInfo( 2194, 1064713, "Walnut",			CraftAttributeInfo.Walnut,				CraftResource.Walnut,			typeof( WalnutLog ) ),
				new CraftResourceInfo( 2178, 1064714, "Ohii",			CraftAttributeInfo.Ohii,				CraftResource.Ohii,				typeof( OhiiLog ) ),
				new CraftResourceInfo( 2140, 1064715, "Cedar",			CraftAttributeInfo.Cedar,				CraftResource.Cedar,			typeof( CedarLog ) ),
				new CraftResourceInfo( 2159, 1064716, "Willow",			CraftAttributeInfo.Willow,				CraftResource.Willow,			typeof( WillowLog ) ),
				new CraftResourceInfo( 2176, 1064717, "Cypress",		CraftAttributeInfo.Cypress,				CraftResource.Cypress,			typeof( CypressLog ) ),
				new CraftResourceInfo( 1921, 1064718, "Yew",			CraftAttributeInfo.Yew,					CraftResource.Yew,				typeof( YewLog ) ),
				new CraftResourceInfo( 2182, 1064719, "Apple",			CraftAttributeInfo.Apple,				CraftResource.Apple,			typeof( AppleLog ) ),
				new CraftResourceInfo( 1920, 1064720, "Pear",			CraftAttributeInfo.Pear,				CraftResource.Pear,				typeof( PearLog ) ),
				new CraftResourceInfo( 2171, 1064721, "Peach",			CraftAttributeInfo.Peach,				CraftResource.Peach,			typeof( PeachLog ) ),
				new CraftResourceInfo( 2295, 1064722, "Banana",			CraftAttributeInfo.Banana,				CraftResource.Banana,			typeof( BananaLog ) ),
				new CraftResourceInfo( 2151, 1064723, "Stonewood",		CraftAttributeInfo.Stonewood,			CraftResource.Stonewood,		typeof( StonewoodLog ) ),
				new CraftResourceInfo( 2169, 1064724, "Silver",			CraftAttributeInfo.Silver,				CraftResource.Silver,			typeof( SilverLog ) ),
				new CraftResourceInfo( 2456, 1064725, "Blood",			CraftAttributeInfo.Blood,				CraftResource.Blood,			typeof( BloodLog ) ),
				new CraftResourceInfo( 2453, 1064726, "Swamp",			CraftAttributeInfo.Swamp,				CraftResource.Swamp,			typeof( SwampLog ) ),
				new CraftResourceInfo( 1931, 1064727, "Crystal",		CraftAttributeInfo.Crystal,				CraftResource.Crystal,			typeof( CrystalLog ) ),
				new CraftResourceInfo( 2457, 1064728, "Elven",			CraftAttributeInfo.Elven,				CraftResource.Elven,			typeof( ElvenLog ) ),
				new CraftResourceInfo( 2185, 1064729, "Elder",			CraftAttributeInfo.Elder,				CraftResource.Elder,			typeof( ElderLog ) ),	
				new CraftResourceInfo( 2187, 1064730, "Enchanted",		CraftAttributeInfo.Enchanted,			CraftResource.Enchanted,		typeof( EnchantedLog ) ),
				new CraftResourceInfo( 2468, 1064731, "Death",			CraftAttributeInfo.Death,				CraftResource.Death,			typeof( DeathLog ) ),
		};
        #endregion

        #region m_ScaleInfo
        private static CraftResourceInfo[] m_ScaleInfo = new CraftResourceInfo[]
			{
				new CraftResourceInfo( 0x66D, 1053129, "Red Scales",	CraftAttributeInfo.RedScales,		CraftResource.RedScales,		typeof( RedScales ) ),
				new CraftResourceInfo( 0x8A8, 1053130, "Yellow Scales",	CraftAttributeInfo.YellowScales,	CraftResource.YellowScales,		typeof( YellowScales ) ),
				new CraftResourceInfo( 0x455, 1053131, "Black Scales",	CraftAttributeInfo.BlackScales,		CraftResource.BlackScales,		typeof( BlackScales ) ),
				new CraftResourceInfo( 0x851, 1053132, "Green Scales",	CraftAttributeInfo.GreenScales,		CraftResource.GreenScales,		typeof( GreenScales ) ),
				new CraftResourceInfo( 0x8FD, 1053133, "White Scales",	CraftAttributeInfo.WhiteScales,		CraftResource.WhiteScales,		typeof( WhiteScales ) ),
				new CraftResourceInfo( 0x8B0, 1053134, "Blue Scales",	CraftAttributeInfo.BlueScales,		CraftResource.BlueScales,		typeof( BlueScales ) )
			};
        #endregion

        #region m_LeatherInfo
        private static CraftResourceInfo[] m_LeatherInfo = new CraftResourceInfo[]
			{
				new CraftResourceInfo( 0x000, 1049353, "Normal",		CraftAttributeInfo.Blank,		CraftResource.RegularLeather,	typeof( Leather ),			typeof( Hides ) ),
				new CraftResourceInfo( 0x283, 1049354, "Spined",		CraftAttributeInfo.Spined,		CraftResource.SpinedLeather,	typeof( SpinedLeather ),	typeof( SpinedHides ) ),
				new CraftResourceInfo( 0x227, 1049355, "Horned",		CraftAttributeInfo.Horned,		CraftResource.HornedLeather,	typeof( HornedLeather ),	typeof( HornedHides ) ),
				new CraftResourceInfo( 0x1C1, 1049356, "Barbed",		CraftAttributeInfo.Barbed,		CraftResource.BarbedLeather,	typeof( BarbedLeather ),	typeof( BarbedHides ) )
            };
        #endregion

        #region m_AOSLeatherInfo
        private static CraftResourceInfo[] m_AOSLeatherInfo = new CraftResourceInfo[]
			{
				new CraftResourceInfo( 0x000, 1049353, "Normal",		CraftAttributeInfo.Blank,		CraftResource.RegularLeather,	typeof( Leather ),			typeof( Hides ) ),
				new CraftResourceInfo( 0x8AC, 1049354, "Spined",		CraftAttributeInfo.Spined,		CraftResource.SpinedLeather,	typeof( SpinedLeather ),	typeof( SpinedHides ) ),
				new CraftResourceInfo( 0x845, 1049355, "Horned",		CraftAttributeInfo.Horned,		CraftResource.HornedLeather,	typeof( HornedLeather ),	typeof( HornedHides ) ),
				new CraftResourceInfo( 0x851, 1049356, "Barbed",		CraftAttributeInfo.Barbed,		CraftResource.BarbedLeather,	typeof( BarbedLeather ),	typeof( BarbedHides ) ),
                
                new CraftResourceInfo( 0x878, 1066220, "Humanoid",		CraftAttributeInfo.Humanoid,	CraftResource.HumanoidLeather,	typeof( HumanoidLeather ),	typeof( HumanoidHides ) ),
                new CraftResourceInfo( 0x819, 1066221, "Undead",		CraftAttributeInfo.Undead,		CraftResource.UndeadLeather,	typeof( UndeadLeather ),	typeof( UndeadHides ) ),
                new CraftResourceInfo( 0x531, 1066222, "Wolf",		    CraftAttributeInfo.Wolf,		CraftResource.WolfLeather,	    typeof( WolfLeather ),	    typeof( WolfHides ) ),
                new CraftResourceInfo( 0x80C, 1066223, "Aracnid",		CraftAttributeInfo.Aracnid,		CraftResource.AracnidLeather,	typeof( AracnidLeather ),	typeof( AracnidHides ) ),
                new CraftResourceInfo( 0x687, 1066224, "Fey",		    CraftAttributeInfo.Fey,		    CraftResource.FeyLeather,	    typeof( FeyLeather ),	    typeof( FeyHides ) ),
                new CraftResourceInfo( 0x780, 1066225, "Green Dragon",	CraftAttributeInfo.GreenDragon,	CraftResource.GreenDragonLeather,	typeof( GreenDragonLeather ),	typeof( GreenDragonHides ) ),
                new CraftResourceInfo( 0x9BC, 1066226, "Black Dragon",	CraftAttributeInfo.BlackDragon,	CraftResource.BlackDragonLeather,	typeof( BlackDragonLeather ),	typeof( BlackDragonHides ) ),
                new CraftResourceInfo( 0xB0A, 1066227, "Blue Dragon",	CraftAttributeInfo.BlueDragon,	CraftResource.BlueDragonLeather,	typeof( BlueDragonLeather ),	typeof( BlueDragonHides ) ),
                new CraftResourceInfo( 0x98C, 1066228, "Red Dragon",	CraftAttributeInfo.RedDragon,	CraftResource.RedDragonLeather,	typeof( RedDragonLeather ),	typeof( RedDragonHides ) ),
                new CraftResourceInfo( 0x9A4, 1066229, "Abyss",		    CraftAttributeInfo.Abyss,		CraftResource.AbyssLeather,	    typeof( AbyssLeather ),	    typeof( AbyssHides ) )
            };
        #endregion

        #region m_OldMetalInfo
        private static CraftResourceInfo[] m_OldMetalInfo = new CraftResourceInfo[]
			{
                new CraftResourceInfo( 0x000, 1053109, "Iron",			            CraftAttributeInfo.Blank,			    CraftResource.Iron,				typeof( IronIngot ),			typeof( IronOre ),			typeof( Granite ) ),
				new CraftResourceInfo( 0x973, 1053108, "Dull Copper",	            CraftAttributeInfo.OldDullCopper,		CraftResource.OldDullCopper,    typeof( DullCopperIngot ),		typeof( DullCopperOre ),	typeof( DullCopperGranite ) ),
				new CraftResourceInfo( 0x902, 1053107, "Shadow Iron",	            CraftAttributeInfo.OldShadowIron,		CraftResource.OldShadowIron,	typeof( ShadowIronIngot ),		typeof( ShadowIronOre ),	typeof( ShadowIronGranite ) ),
				new CraftResourceInfo( 0x641, 1053106, "Copper",		            CraftAttributeInfo.OldCopper,			CraftResource.OldCopper,		typeof( CopperIngot ),			typeof( CopperOre ),		typeof( CopperGranite ) ),
				new CraftResourceInfo( 0x465, 1053105, "Bronze",		            CraftAttributeInfo.OldBronze,			CraftResource.OldBronze,		typeof( BronzeIngot ),			typeof( BronzeOre ),		typeof( BronzeGranite ) ),
				new CraftResourceInfo( 0x96D, 1053104, "Gold",			            CraftAttributeInfo.OldGold,			    CraftResource.OldGold,			typeof( GoldIngot ),			typeof( GoldOre ),			typeof( GoldGranite ) ),
				new CraftResourceInfo( 0x979, 1053103, "Agapite",		            CraftAttributeInfo.OldAgapite,			CraftResource.OldAgapite,		typeof( AgapiteIngot ),			typeof( AgapiteOre ),		typeof( AgapiteGranite ) ),
				new CraftResourceInfo( 0x84D, 1053102, "Verite",		            CraftAttributeInfo.OldVerite,			CraftResource.OldVerite,		typeof( VeriteIngot ),			typeof( VeriteOre ),		typeof( VeriteGranite ) ),
				new CraftResourceInfo( 0x847, 1053101, "Valorite",		            CraftAttributeInfo.OldValorite,		    CraftResource.OldValorite,		typeof( ValoriteIngot ),		typeof( ValoriteOre ),		typeof( ValoriteGranite ) ),
                new CraftResourceInfo( 0x852, 1066303, "Graphite",			        CraftAttributeInfo.OldGraphite,			CraftResource.OldGraphite,		typeof( GraphiteIngot ),		typeof( GraphiteOre ),	    typeof( GraphiteGranite ) ),
                new CraftResourceInfo( 0x60E, 1066305, "Pyrite",			        CraftAttributeInfo.OldPyrite,			CraftResource.OldPyrite,		typeof( PyriteIngot ),			typeof( PyriteOre ),	    typeof( PyriteGranite ) ),
                new CraftResourceInfo( 0x52D, 1066306, "Azurite",			        CraftAttributeInfo.OldAzurite,			CraftResource.OldAzurite,		typeof( AzuriteIngot ),			typeof( AzuriteOre ),	    typeof( AzuriteGranite ) ),
                new CraftResourceInfo( 0x6A4, 1066307, "Vanadium",			        CraftAttributeInfo.OldVanadium,			CraftResource.OldVanadium,		typeof( VanadiumIngot ),		typeof( VanadiumOre ),	    typeof( VanadiumGranite ) ),
                new CraftResourceInfo( 0x961, 1066308, "Silver",			        CraftAttributeInfo.OldSilver,			CraftResource.OldSilver,		typeof( SilverIngot ),		    typeof( SilverOre ),        typeof( SilverGranite ) ),
                new CraftResourceInfo( 0x887, 1066309, "Platinum",			        CraftAttributeInfo.OldPlatinum,			CraftResource.OldPlatinum,		typeof( PlatinumIngot ),		typeof( PlatinumOre ),	    typeof( PlatinumGranite ) ),
                new CraftResourceInfo( 0x4DF, 1066310, "Amethyst",			        CraftAttributeInfo.OldAmethyst,			CraftResource.OldAmethyst,		typeof( AmethystIngot ),		typeof( AmethystOre ),	    typeof( AmethystGranite ) ),
                new CraftResourceInfo( 0x54A, 1066311, "Titanium",			        CraftAttributeInfo.OldTitanium,			CraftResource.OldTitanium,		typeof( TitaniumIngot ),		typeof( TitaniumOre ),	    typeof( TitaniumGranite ) ),
                new CraftResourceInfo( 0x896, 1066313, "Xenian",			        CraftAttributeInfo.OldXenian,			CraftResource.OldXenian,		typeof( XenianIngot ),			typeof( XenianOre ),	    typeof( XenianGranite ) ),
                new CraftResourceInfo( 0x676, 1066314, "Rubidian",			        CraftAttributeInfo.OldRubidian,			CraftResource.OldRubidian,		typeof( RubidianIngot ),		typeof( RubidianOre ),	    typeof( RubidianGranite ) ),
                new CraftResourceInfo( 0x9CF, 1066315, "Obsidian",			        CraftAttributeInfo.OldObsidian,			CraftResource.OldObsidian,		typeof( ObsidianIngot ),	    typeof( ObsidianOre ),		typeof( ObsidianGranite ) ),
                new CraftResourceInfo( 0x78C, 1066316, "Ebon Twilight Sapphire",	CraftAttributeInfo.OldEbonSapphire,     CraftResource.OldEbonSapphire,	typeof( EbonSapphireIngot ),	typeof( EbonSapphireOre ),  typeof( EbonSapphireGranite ) ),
                new CraftResourceInfo( 0x78D, 1066317, "Dark Sable Ruby",			CraftAttributeInfo.OldDarkRuby,			CraftResource.OldDarkRuby,		typeof( DarkRubyIngot ),		typeof( DarkRubyOre ),	    typeof( DarkRubyGranite ) ),
                new CraftResourceInfo( 0x791, 1066318, "Radiant Nimbus Diamond",    CraftAttributeInfo.OldRadiantDiamond,	CraftResource.OldRadiantDiamond,typeof( RadiantDiamondIngot ),	typeof( RadiantDiamondOre ),typeof( RadiantDiamondGranite ) ),
            };
        #endregion

        #region m_OldWoodInfo
        private static CraftResourceInfo[] m_OldWoodInfo = new CraftResourceInfo[]
			{
				new CraftResourceInfo( 0,	 1064711, "Log",			CraftAttributeInfo.Blank,				CraftResource.RegularWood,		typeof( Log ) ),
				new CraftResourceInfo( 2195, 1064712, "Oak",			CraftAttributeInfo.Oak,					CraftResource.Oak,				typeof( OakLog ) ),
				new CraftResourceInfo( 2194, 1064713, "Walnut",			CraftAttributeInfo.Walnut,				CraftResource.Walnut,			typeof( WalnutLog ) ),
				new CraftResourceInfo( 2178, 1064714, "Ohii",			CraftAttributeInfo.Ohii,				CraftResource.Ohii,				typeof( OhiiLog ) ),
				new CraftResourceInfo( 2140, 1064715, "Cedar",			CraftAttributeInfo.Cedar,				CraftResource.Cedar,			typeof( CedarLog ) ),
				new CraftResourceInfo( 2159, 1064716, "Willow",			CraftAttributeInfo.Willow,				CraftResource.Willow,			typeof( WillowLog ) ),
				new CraftResourceInfo( 2176, 1064717, "Cypress",		CraftAttributeInfo.Cypress,				CraftResource.Cypress,			typeof( CypressLog ) ),
				new CraftResourceInfo( 1921, 1064718, "Yew",			CraftAttributeInfo.Yew,					CraftResource.Yew,				typeof( YewLog ) ),
				new CraftResourceInfo( 2182, 1064719, "Apple",			CraftAttributeInfo.Apple,				CraftResource.Apple,			typeof( AppleLog ) ),
				new CraftResourceInfo( 1920, 1064720, "Pear",			CraftAttributeInfo.Pear,				CraftResource.Pear,				typeof( PearLog ) ),
				new CraftResourceInfo( 2171, 1064721, "Peach",			CraftAttributeInfo.Peach,				CraftResource.Peach,			typeof( PeachLog ) ),
				new CraftResourceInfo( 2295, 1064722, "Banana",			CraftAttributeInfo.Banana,				CraftResource.Banana,			typeof( BananaLog ) ),
				new CraftResourceInfo( 2151, 1064723, "Stonewood",		CraftAttributeInfo.Stonewood,			CraftResource.Stonewood,		typeof( StonewoodLog ) ),
				new CraftResourceInfo( 2169, 1064724, "Silver",			CraftAttributeInfo.Silver,				CraftResource.Silver,			typeof( SilverLog ) ),
				new CraftResourceInfo( 2456, 1064725, "Blood",			CraftAttributeInfo.Blood,				CraftResource.Blood,			typeof( BloodLog ) ),
				new CraftResourceInfo( 2453, 1064726, "Swamp",			CraftAttributeInfo.Swamp,				CraftResource.Swamp,			typeof( SwampLog ) ),
				new CraftResourceInfo( 1931, 1064727, "Crystal",		CraftAttributeInfo.Crystal,				CraftResource.Crystal,			typeof( CrystalLog ) ),
				new CraftResourceInfo( 2457, 1064728, "Elven",			CraftAttributeInfo.Blank,				CraftResource.RegularWood,		typeof( Log ) ),
				new CraftResourceInfo( 2185, 1064729, "Elder",			CraftAttributeInfo.Blank,				CraftResource.RegularWood,		typeof( Log ) ),	
				new CraftResourceInfo( 2187, 1064730, "Enchanted",		CraftAttributeInfo.Enchanted,			CraftResource.Enchanted,		typeof( EnchantedLog ) ),
				new CraftResourceInfo( 2468, 1064731, "Death",			CraftAttributeInfo.Blank,				CraftResource.RegularWood,		typeof( Log ) ),
			};
        #endregion

        #region m_OldLeatherInfo
        private static CraftResourceInfo[] m_OldLeatherInfo = new CraftResourceInfo[]
			{
                new CraftResourceInfo( 0x000, 1049353, "Normal",        CraftAttributeInfo.Blank,		            CraftResource.RegularLeather,       typeof( Leather ),              typeof( Hides ) ),
                new CraftResourceInfo( 0x763, 1066160, "Wolf",		    CraftAttributeInfo.OldWolfLeather,	        CraftResource.OldWolfLeather,	    typeof( WolfLeather ),	        typeof( WolfHides ) ),
                new CraftResourceInfo( 0x8EF, 1066161, "Bear",		    CraftAttributeInfo.OldBearLeather,	        CraftResource.OldBearLeather,	    typeof( BearLeather ),	        typeof( BearHides ) ),
                new CraftResourceInfo( 0x455, 1066162, "Arachnid",	    CraftAttributeInfo.OldArachnidLeather,	    CraftResource.OldArachnidLeather,   typeof( AracnidLeather ),	    typeof( AracnidHides ) ),
                new CraftResourceInfo( 0x851, 1066163, "Reptile",	    CraftAttributeInfo.OldReptileLeather,	    CraftResource.OldReptileLeather,    typeof( SpinedLeather ),	    typeof( SpinedHides ) ),
                new CraftResourceInfo( 0x7B7, 1066164, "Orcish",	    CraftAttributeInfo.OldOrcishLeather,		CraftResource.OldOrcishLeather,	    typeof( OrcishLeather ),		typeof( OrcishHides ) ),
                new CraftResourceInfo( 0x878, 1066165, "Humanoid",      CraftAttributeInfo.OldHumanoidLeather,		CraftResource.OldHumanoidLeather,   typeof( BarbedLeather ),		typeof( BarbedHides ) ),
                new CraftResourceInfo( 0x9F9, 1066166, "Undead",        CraftAttributeInfo.OldUndeadLeather,		CraftResource.OldUndeadLeather,	    typeof( UndeadLeather ),		typeof( UndeadHides ) ),
                new CraftResourceInfo( 0x7D7, 1066167, "Ophidian",      CraftAttributeInfo.OldOphidianLeather,		CraftResource.OldOphidianLeather,	typeof( HornedLeather ),		typeof( HornedHides ) ),
                new CraftResourceInfo( 0x444, 1066168, "Lava",	        CraftAttributeInfo.OldLavaLeather,		    CraftResource.OldLavaLeather,	    typeof( LavaLeather ),		    typeof( LavaHides ) ),
                new CraftResourceInfo( 0x427, 1066169, "Arctic",		CraftAttributeInfo.OldArcticLeather,		CraftResource.OldArcticLeather,	    typeof( ArcticLeather ),		typeof( ArcticHides ) ),
                new CraftResourceInfo( 0x8D8, 1066170, "Green Dragon",  CraftAttributeInfo.OldGreenDragonLeather,   CraftResource.OldGreenDragonLeather,typeof( GreenDragonLeather ),	typeof( GreenDragonHides ) ),
                new CraftResourceInfo( 0x98F, 1066171, "Blue Dragon",	CraftAttributeInfo.OldBlueDragonLeather,	CraftResource.OldBlueDragonLeather, typeof( BlueDragonLeather ),	typeof( BlueDragonHides ) ),
                new CraftResourceInfo( 0x9BC, 1066172, "Black Dragon",	CraftAttributeInfo.OldBlackDragonLeather,	CraftResource.OldBlackDragonLeather,typeof( BlackDragonLeather ),	typeof( BlackDragonHides ) ),
                new CraftResourceInfo( 0x993, 1066173, "Red Dragon",	CraftAttributeInfo.OldRedDragonLeather,	    CraftResource.OldRedDragonLeather,  typeof( RedDragonLeather ),		typeof( RedDragonHides ) ),
                new CraftResourceInfo( 0x510, 1066174, "Demon",		    CraftAttributeInfo.OldDemonLeather,		    CraftResource.OldDemonLeather,	    typeof( AbyssLeather ),		    typeof( AbyssHides ) ),
            };

        public static CraftResourceInfo[] OldMetalInfo
        {
            get { return m_OldMetalInfo; }
        }

        public static CraftResourceInfo[] OldWoodInfo
        {
            get { return m_OldWoodInfo; }
        }

        public static CraftResourceInfo[] OldLeatherInfo
        {
            get { return m_OldLeatherInfo; }
        }

        public static CraftResourceInfo[] ScaleInfo
        {
            get { return m_ScaleInfo; }
        }

        #endregion

        /*
        #region Mondain's Legacy
        private static CraftResourceInfo[] m_WoodInfo = new CraftResourceInfo[]
            {
                new CraftResourceInfo( 0x000, 1011542, "Normal",		CraftAttributeInfo.Blank,		CraftResource.RegularWood,	typeof( Log ),			typeof( Board ) ),
                new CraftResourceInfo( 0x7DA, 1072533, "Oak",			CraftAttributeInfo.OakWood,		CraftResource.OakWood,		typeof( OakLog ),		typeof( OakBoard ) ),
                new CraftResourceInfo( 0x4A7, 1072534, "Ash",			CraftAttributeInfo.AshWood,		CraftResource.AshWood,		typeof( AshLog ),		typeof( AshBoard ) ),
                new CraftResourceInfo( 0x4A8, 1072535, "Yew",			CraftAttributeInfo.YewWood,		CraftResource.YewWood,		typeof( YewLog ),		typeof( YewBoard ) ),
                new CraftResourceInfo( 0x4A9, 1072536, "Heartwood",		CraftAttributeInfo.Heartwood,	CraftResource.Heartwood,	typeof( HeartwoodLog ),		typeof( HeartwoodBoard ) ),
                new CraftResourceInfo( 0x4AA, 1072538, "Bloodwood",		CraftAttributeInfo.Bloodwood,	CraftResource.Bloodwood,	typeof( BloodwoodLog ),		typeof( BloodwoodBoard ) ),
                new CraftResourceInfo( 0x47F, 1072539, "Frostwood",		CraftAttributeInfo.Frostwood,	CraftResource.Frostwood,	typeof( FrostwoodLog ),	typeof( FrostwoodBoard ) )
        };
        #endregion
        */

        /// <summary>
        /// Returns true if '<paramref name="resource"/>' is None, Iron, RegularLeather or RegularWood. False if otherwise.
        /// </summary>
        public static bool IsStandard( CraftResource resource )
        {
            return ( resource == CraftResource.None || resource == CraftResource.Iron || resource == CraftResource.RegularLeather || resource == CraftResource.RegularWood );
        }

        private static Hashtable m_TypeTable;

        /// <summary>
        /// Registers that '<paramref name="resourceType"/>' uses '<paramref name="resource"/>' so that it can later be queried by <see cref="CraftResources.GetFromType"/>
        /// </summary>
        public static void RegisterType( Type resourceType, CraftResource resource )
        {
            if( m_TypeTable == null )
                m_TypeTable = new Hashtable();

            if( Core.Debug )
                Utility.Log( "CraftResourceInfoRegisterType.log", "RegisterType: {0} -> {1}", resourceType.Name, resource.ToString() );

            m_TypeTable[ resourceType ] = resource;
        }

        #region debug method by Dies Irae
        public static void Initialize()
        {
            Commands.CommandSystem.Register( "ShowResourceTable", AccessLevel.Developer, delegate { ShowTable(); } );
        }

        [Usage( "ShowResourceTable" )]
        [Description( "Broadcast to console the actual Midgard resource table." )]
        public static void ShowTable()
        {
            Console.WriteLine( "CraftResources.TypeTable" );
            foreach( DictionaryEntry entry in m_TypeTable )
            {
                Console.WriteLine( "{0} -> {1}", entry.Key, entry.Value );
            }
            Console.WriteLine( "" );
        }
        #endregion

        /// <summary>
        /// Returns the <see cref="CraftResource"/> value for which '<paramref name="resourceType"/>' uses -or- CraftResource.None if an unregistered type was specified.
        /// </summary>
        public static CraftResource GetFromType( Type resourceType )
        {
            if( m_TypeTable == null )
                return CraftResource.None;

            object obj = m_TypeTable[ resourceType ];

            if( !( obj is CraftResource ) )
                return CraftResource.None;

            //Console.WriteLine( "{0} -> {1}", resourceType.Name, ((CraftResource)obj).ToString());

            return (CraftResource)obj;
        }

        /// <summary>
        /// Returns a <see cref="CraftResourceInfo"/> instance describing '<paramref name="resource"/>' -or- null if an invalid resource was specified.
        /// </summary>
        public static CraftResourceInfo GetInfo( CraftResource resource )
        {
            CraftResourceInfo[] list = null;

            switch( GetType( resource ) )
            {
                // case CraftResourceType.Metal: list = m_MetalInfo; break;
                case CraftResourceType.Metal: list = /* Core.AOS ? m_MetalInfo : */ OldMetalInfo; break;

                // case CraftResourceType.Leather: list = Core.AOS ? m_AOSLeatherInfo : m_LeatherInfo; break;
                case CraftResourceType.Leather: list = /* Core.AOS ? m_AOSLeatherInfo : */ OldLeatherInfo; break;

                case CraftResourceType.Scales: list = ScaleInfo; break;
                case CraftResourceType.Wood: list = /* Core.AOS ? m_WoodInfo : */ OldWoodInfo; break;
            }

            if( list != null )
            {
                int index = GetIndex( resource );

                if( index >= 0 && index < list.Length )
                {
                    //Console.WriteLine( "CraftResources.GetInfo(): {0} -> {1}", resource, list[ index ].Name );
                    return list[ index ];
                }
            }

            //Console.WriteLine( "CraftResources.GetInfo(): {0} -> null info", resource );
            return null;
        }

        /// <summary>
        /// Returns a <see cref="CraftResourceType"/> value indiciating the type of '<paramref name="resource"/>'.
        /// </summary>
        public static CraftResourceType GetType( CraftResource resource )
        {
            if( resource >= CraftResource.Iron && resource <= CraftResource.Aqua )
                return CraftResourceType.Metal;

            if( resource >= CraftResource.OldDullCopper && resource <= CraftResource.OldRadiantDiamond )
                return CraftResourceType.Metal;

            if( resource >= CraftResource.RegularLeather && resource <= CraftResource.AbyssLeather )
                return CraftResourceType.Leather;

            if( resource >= CraftResource.OldWolfLeather && resource <= CraftResource.OldDemonLeather )
                return CraftResourceType.Leather;

            if( resource >= CraftResource.RedScales && resource <= CraftResource.BlueScales )
                return CraftResourceType.Scales;

            /*
            if ( resource >= CraftResource.RegularWood && resource <= CraftResource.Frostwood )
                return CraftResourceType.Wood;
            */

            if( resource >= CraftResource.RegularWood && resource <= CraftResource.Death )
                return CraftResourceType.Wood;

            return CraftResourceType.None;
        }

        /// <summary>
        /// Returns the first <see cref="CraftResource"/> in the series of resources for which '<paramref name="resource"/>' belongs.
        /// </summary>
        public static CraftResource GetStart( CraftResource resource )
        {
            switch( GetType( resource ) )
            {
                case CraftResourceType.Metal: return Core.AOS ? CraftResource.Iron : CraftResource.OldDullCopper;
                case CraftResourceType.Leather: return Core.AOS ? CraftResource.RegularLeather : CraftResource.OldWolfLeather;
                case CraftResourceType.Scales: return CraftResource.RedScales;
                case CraftResourceType.Wood: return CraftResource.RegularWood;
            }

            return CraftResource.None;
        }

        /// <summary>
        /// Returns the index of '<paramref name="resource"/>' in the seriest of resources for which it belongs.
        /// </summary>
        public static int GetIndex( CraftResource resource )
        {
            CraftResource start = GetStart( resource );

            if( start == CraftResource.None )
                return 0;

            #region mod by Dies Irae : pre-aos stuff
            if( !Core.AOS )
            {
                // this fix is required because old resources does not start with a relative standard info
                if( resource == CraftResource.Iron || resource == CraftResource.RegularLeather )
                    return 0;

                switch( GetType( resource ) )
                {
                    case CraftResourceType.Metal:
                    case CraftResourceType.Leather:
                        return resource - start + 1;
                    default:
                        return resource - start;
                }

            }
            #endregion

            return (int)( resource - start );
        }

        /// <summary>
        /// Returns the <see cref="CraftResourceInfo.Number"/> property of '<paramref name="resource"/>' -or- 0 if an invalid resource was specified.
        /// </summary>
        public static int GetLocalizationNumber( CraftResource resource )
        {
            CraftResourceInfo info = GetInfo( resource );

            return ( info == null ? 0 : info.Number );
        }

        /// <summary>
        /// Returns the <see cref="CraftResourceInfo.Hue"/> property of '<paramref name="resource"/>' -or- 0 if an invalid resource was specified.
        /// </summary>
        public static int GetHue( CraftResource resource )
        {
            CraftResourceInfo info = GetInfo( resource );

            return ( info == null ? 0 : info.Hue );
        }

        /// <summary>
        /// Returns the <see cref="CraftResourceInfo.Name"/> property of '<paramref name="resource"/>' -or- an empty string if the resource specified was invalid.
        /// </summary>
        public static string GetName( CraftResource resource )
        {
            CraftResourceInfo info = GetInfo( resource );

            return ( info == null ? String.Empty : info.Name );
        }

        /// <summary>
        /// Returns the <see cref="CraftResource"/> value which represents '<paramref name="info"/>' -or- CraftResource.None if unable to convert.
        /// </summary>
        public static CraftResource GetFromOreInfo( OreInfo info )
        {
            if( info.Name.IndexOf( "Spined" ) >= 0 )
                return CraftResource.SpinedLeather;
            else if( info.Name.IndexOf( "Horned" ) >= 0 )
                return CraftResource.HornedLeather;
            else if( info.Name.IndexOf( "Barbed" ) >= 0 )
                return CraftResource.BarbedLeather;
            else if( info.Name.IndexOf( "Leather" ) >= 0 )
                return CraftResource.RegularLeather;

            if( info.Level == 0 )
                return CraftResource.Iron;
            else if( info.Level == 1 )
                return CraftResource.DullCopper;
            else if( info.Level == 2 )
                return CraftResource.ShadowIron;
            else if( info.Level == 3 )
                return CraftResource.Copper;
            else if( info.Level == 4 )
                return CraftResource.Bronze;
            else if( info.Level == 5 )
                return CraftResource.Gold;
            else if( info.Level == 6 )
                return CraftResource.Agapite;
            else if( info.Level == 7 )
                return CraftResource.Verite;
            else if( info.Level == 8 )
                return CraftResource.Valorite;
            else if( info.Level == 9 )

                try
                {
                    using( StreamWriter op = new StreamWriter( "Logs/resource-info-errors.log", true ) )
                    {
                        op.WriteLine( "{0}\t{1}", DateTime.Now, "Deprecated method 'GetFromOreInfo' used." );
                        op.WriteLine( new StackTrace( 2 ).ToString() );
                        op.WriteLine();
                    }
                }
                catch
                {
                }

            return CraftResource.None;
        }

        /// <summary>
        /// Returns the <see cref="CraftResource"/> value which represents '<paramref name="info"/>', using '<paramref name="material"/>' to help resolve leather OreInfo instances.
        /// </summary>
        public static CraftResource GetFromOreInfo( OreInfo info, ArmorMaterialType material )
        {
            if( material == ArmorMaterialType.Studded || material == ArmorMaterialType.Leather || material == ArmorMaterialType.Spined ||
                material == ArmorMaterialType.Horned || material == ArmorMaterialType.Barbed )
            {
                if( info.Level == 0 )
                    return CraftResource.RegularLeather;
                else if( info.Level == 1 )
                    return CraftResource.SpinedLeather;
                else if( info.Level == 2 )
                    return CraftResource.HornedLeather;
                else if( info.Level == 3 )
                    return CraftResource.BarbedLeather;

                return CraftResource.None;
            }

            return GetFromOreInfo( info );
        }
    }

    // NOTE: This class is only for compatability with very old RunUO versions.
    // No changes to it should be required for custom resources.
    public class OreInfo
    {
        public static readonly OreInfo Iron = new OreInfo( 0, 0x000, "Iron" );
        public static readonly OreInfo DullCopper = new OreInfo( 1, 0x973, "Dull Copper" );
        public static readonly OreInfo ShadowIron = new OreInfo( 2, 0x966, "Shadow Iron" );
        public static readonly OreInfo Copper = new OreInfo( 3, 0x96D, "Copper" );
        public static readonly OreInfo Bronze = new OreInfo( 4, 0x972, "Bronze" );
        public static readonly OreInfo Gold = new OreInfo( 5, 0x8A5, "Gold" );
        public static readonly OreInfo Agapite = new OreInfo( 6, 0x979, "Agapite" );
        public static readonly OreInfo Verite = new OreInfo( 7, 0x89F, "Verite" );
        public static readonly OreInfo Valorite = new OreInfo( 8, 0x8AB, "Valorite" );

        private int m_Level;
        private int m_Hue;
        private string m_Name;

        public OreInfo( int level, int hue, string name )
        {
            m_Level = level;
            m_Hue = hue;
            m_Name = name;
        }

        public int Level
        {
            get
            {
                return m_Level;
            }
        }

        public int Hue
        {
            get
            {
                return m_Hue;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }
    }
}