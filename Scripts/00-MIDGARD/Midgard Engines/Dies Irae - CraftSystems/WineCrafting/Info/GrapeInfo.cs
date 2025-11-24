using System;
using Midgard.Engines.BrewCrafing;

namespace Midgard.Engines.WineCrafting
{
    public class GrapeInfo
    {
        public string m_Name;
        public BrewVariety m_Variety;
        public Type m_VarietyType;

        public GrapeInfo( string name, BrewVariety variety, Type varietyType )
        {
            m_Name = name;
            m_Variety = variety;
            m_VarietyType = varietyType;
        }
    }
}