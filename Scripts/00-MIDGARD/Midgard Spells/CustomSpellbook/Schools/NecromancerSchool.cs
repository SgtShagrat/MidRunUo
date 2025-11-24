using Midgard.Engines.Classes;

namespace Midgard.Engines.SpellSystem
{
    public sealed class NecromancerSchool : SchoolInfo
    {
        public override string Name
        {
            get { return "Necrom."; }
        }

        public override SchoolFlag School
        {
            get { return SchoolFlag.Necromancer; }
        }

        public override int Background
        {
            get { return 9350; }
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Necromancer; }
        }
    }
}