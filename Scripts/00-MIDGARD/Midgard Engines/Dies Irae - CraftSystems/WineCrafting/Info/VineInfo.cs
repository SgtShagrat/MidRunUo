using System;

namespace Midgard.Engines.WineCrafting
{
    public class VineInfo
    {
        public string m_VineName;
        public Type m_Type;
        public int m_BaseID;

        public VineInfo( string vinename, Type type, int baseID )
        {
            m_VineName = vinename;
            m_Type = type;
            m_BaseID = baseID;
        }
    }
}