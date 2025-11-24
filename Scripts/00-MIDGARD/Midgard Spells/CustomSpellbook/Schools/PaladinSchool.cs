using Midgard.Engines.Classes;

namespace Midgard.Engines.SpellSystem
{
    public sealed class PaladinSchool : SchoolInfo
    {
        public override string Name
        {
            get { return "Paladin"; }
        }

        public override SchoolFlag School
        {
            get { return SchoolFlag.Paladin; }
        }

        public override int Background
        {
            get { return 9350; }
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Paladin; }
        }
    }
}