namespace Midgard.Misc
{
    /// <summary>
    /// Currently specifies translations for actions while on a mount.
    /// Will probably be used to specify translations for creatures, so they
    /// don't do illegal things and turn invisible.
    /// </summary>
    public static class AnimsOnMount
    {
        public enum MountedAnims
        {
            Walkunarmed,
            Walkarmed,
            Rununarmed,
            Runarmed,
            Stand,
            Lookaround,
            Fidget,
            OneHwarmode,
            TwoHwarmode,
            OneHgenericmeleeswing,
            OneHFencingJab,
            OneHOverheadMace,
            TwoHMaceJab,
            TwoHGenericMeleeSwing,
            TwoHSpearJab,
            WalkWarmode,
            DirectionalSpellcast,
            AreaEffectSpellcast,
            BowAttack,
            CrossbowAttack,
            TakeAHit,
            DieOntoBack,
            DieOntoFace,
            RideSlow,
            RideFast,
            SitOnHorse,
            MountedAttack,
            MountedBowShot,
            MountedCrossbowShot,
            SlapHorse,
            Dodge,
            Punch,
            Bow,
            Salute,
            Eat
        }

        private static readonly int[] m_Anims = new int[]
			{
                0x00, 0x17, // Walk (unarmed) / Ride - slow
                0x01, 0x17, // Walk (armed) / Ride - slow
                0x02, 0x18, // Run (unarmed) / Ride - fast
                0x03, 0x18, // Run (armed) / Ride - fast
                0x04, 0x19, // Stand / Sit on horse
                0x05, 0x19, // Look around / Sit on horse
                0x06, 0x19, // Fidget / Sit on horse
                0x07, 0x19, // 1H warmode / Sit on horse
                0x08, 0x19, // 2H warmode / Sit on horse
                0x09, 0x1a, // 1H generic melee swing / Mounted melee attack
                0x0a, 0x1a, // 1H fencing jab / Mounted melee attack
                0x0b, 0x1a, // 1H overhead mace / Mounted melee attack
                0x0c, 0x1d, // 2H mace jab / Mounted melee attack
                0x0d, 0x1d, // 2H generic melee swing / Mounted melee attack
                0x0e, 0x1d, // 2H spear jab / Mounted melee attack
                0x0f, 0x17, // Walk (warmode) / Ride - slow
                0x10, 0x18, // Directional spellcast / Ride - fast (bobs up and down)
                0x11, 0x17, // Area-effect spellcast / Ride - slow (bobs up and down)
                0x12, 0x1b, // Bow attack / Mounted bow attack
                0x13, 0x1c, // Crossbow attack / Mounted crossbow attack
                0x14, 0x1d, // Take a hit / Slap horse (looks like a jolt backwards)
                0x15, 0x15, // (Die onto back)
                0x16, 0x16, // (Die onto face)
                0x17, 0x17, // (Ride - slow)
                0x18, 0x18, // (Ride - fast)
                0x19, 0x19, // (Sit on horse)
                0x1a, 0x1a, // (Mounted attack)
                0x1b, 0x1b, // (Mounted bow-shot)
                0x1c, 0x1c, // (Mounted crossbow-shot)
                0x1d, 0x1d, // (Slap horse)
                0x1e, 0x19, // Dodge / Sit on horse
                0x1f, 0x1a, // Punch / Mounted melee attack (looks right with no weapon)
                0x20, 0x1c, // Bow / Mounted crossbow shot (looks about right :) )
                0x21, 0x1b, // Salute / Mounted bow shot (almost... or try xbow)
                0x22, 0x19 // Eat / Sit on horse
            };

        public static int GetMountAnim( int walkAnim )
        {
            if( walkAnim >= 0x0 && walkAnim < 0x22 )
            {
                return m_Anims[ walkAnim * 2 + 1 ];
            }

            return walkAnim;
        }

        public static int GetMountAnim( MountedAnims walkAnim )
        {
            return GetMountAnim( (int)walkAnim );
        }

        public static int GetAnim( int walkAnim, bool mounted )
        {
            return mounted ? GetMountAnim( walkAnim ) : walkAnim;
        }
    }
}
