using Midgard.Engines.Classes;

namespace Midgard.Engines.SpellSystem
{
    public sealed class DruidSchool : SchoolInfo
    {
        public override string Name
        {
            get { return "Druid"; }
        }

        public override SchoolFlag School
        {
            get { return SchoolFlag.Druid; }
        }

        public override int Background
        {
            get { return 9350; }
        }

        public override ClassSystem System
        {
            get { return ClassSystem.Druid; }
        }
    }
}