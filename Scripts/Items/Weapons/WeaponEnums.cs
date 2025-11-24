using System;

namespace Server.Items
{
	public enum WeaponQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public enum WeaponType
	{
		Axe,		// Axes, Hatches, etc. These can give concussion blows
		Slashing,	// Katana, Broadsword, Longsword, etc. Slashing weapons are poisonable
		Staff,		// Staves
		Bashing,	// War Hammers, Maces, Mauls, etc. Two-handed bashing delivers crushing blows
		Piercing,	// Spears, Warforks, Daggers, etc. Two-handed piercing delivers paralyzing blows
		Polearm,	// Halberd, Bardiche
		Ranged,		// Bow, Crossbows
		Fists		// Fists
	}

	public enum WeaponDamageLevel
	{
		Regular,
		Ruin,
		Might,
		Force,
		Power,
		Vanq
	}

	public enum WeaponAccuracyLevel
	{
		Regular,
		Accurate,
		Surpassingly,
		Eminently,
		Exceedingly,
		Supremely
	}

	public enum WeaponDurabilityLevel
	{
		Regular,
		Durable,
		Substantial,
		Massive,
		Fortified,
		Indestructible
	}

	public enum WeaponAnimation
	{
		Slash1H = 9,
		Pierce1H = 10,
		Bash1H = 11,
		Bash2H = 12,
		Slash2H = 13,
		Pierce2H = 14,
		ShootBow = 18,
		ShootXBow = 19,
		Wrestle = 31
    }

    #region mod by Dies Irae
    public enum WeaponMagicalAttribute
    {
        None,               // "hit silver" 
        Clumsiness,         // "hit clumsy"
        Feeblemindedness,   // "hit feeblemind"
        Burning,            // "hit magic arrow"
        Weakness,           // "hit weaken"	
        Wounding,           // "hit harm"
        DaemonBreath,       // "hit fireball"
        Evil,               // "hit curse"	
        Thunder,            // "hit lightning"
        MagesBane,          // "hit mana drain"
        GhoulTouch          // "hit paralyze"	
    }
    #endregion
}