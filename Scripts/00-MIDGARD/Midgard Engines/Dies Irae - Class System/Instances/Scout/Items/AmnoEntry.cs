using System;

namespace Midgard.Items
{
    public class AmnoEntry
    {
        public Type Type { get; private set; }
        public int ItemID { get; private set; }
        public int Hue { get; private set; }
        public string Name { get; private set; }
        public double SkillRequired { get; private set; }
        public double CoolingDelay { get; private set; }
        public int AnimID { get; private set; }
        public int AnimHue { private get; set; }

        public AmnoEntry( Type type, int itemID, int hue, string name, double skillRequired, double coolingDelay, int animID, int animHue )
        {
            Type = type;
            ItemID = itemID;
            Hue = hue;
            Name = name;
            SkillRequired = skillRequired;
            CoolingDelay = coolingDelay;
            AnimID = animID;
            AnimHue = animHue;
        }

        public override string ToString()
        {
            return String.Format( "{0} {1} {2} {3} {4} {5}", Type.Name, ItemID, Hue, Name, SkillRequired.ToString( "F2" ), CoolingDelay.ToString( "F2" ) );
        }
    }
}