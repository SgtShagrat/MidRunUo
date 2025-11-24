namespace Midgard.Engines.MonsterMasterySystem
{
    public class MasteryValuesDefinition
    {
        public string Label { get; private set; }
        public int DamageBuff { get; private set; }
        public double KarmaBuff { get; private set; }
        public double FameBuff { get; private set; }
        public double SpeedBuff { get; private set; }
        public double SkillsBuff { get; private set; }
        public double DexBuff { get; private set; }
        public double IntBuff { get; private set; }
        public double StrBuff { get; private set; }
        public double HitsBuff { get; private set; }
        public int Hue { get; private set; }

        public MasteryValuesDefinition( int hue, double hitsBuff, double strBuff,
                                    double intBuff, double dexBuff, double skillsBuff, double speedBuff,
                                    double fameBuff, double karmaBuff, int damageBuff, string label )
        {
            Label = label;
            DamageBuff = damageBuff;
            KarmaBuff = karmaBuff;
            FameBuff = fameBuff;
            SpeedBuff = speedBuff;
            SkillsBuff = skillsBuff;
            DexBuff = dexBuff;
            IntBuff = intBuff;
            StrBuff = strBuff;
            HitsBuff = hitsBuff;
            Hue = hue;
        }
    }
}