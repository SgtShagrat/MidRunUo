namespace Midgard.Engines.SpellSystem
{
    public interface ICustomSpell
    {
        /// <summary>
        /// Additional info for custom spells.
        /// The main ones: iconID and description
        /// </summary>
        ExtendedSpellInfo ExtendedInfo{ get; }

        /// <summary>
        /// The minimum required skill to cast this spell
        /// </summary>
        double RequiredSkill{ get; }

        /// <summary>
        /// The school this spell belongs to
        /// </summary>
        SchoolFlag SpellSchool{ get; }
    }
}