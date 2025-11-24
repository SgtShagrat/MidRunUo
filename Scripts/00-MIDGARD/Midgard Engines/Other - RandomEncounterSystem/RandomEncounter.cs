using System;
using System.Collections;
using System.Xml;
using Server;

namespace Midgard.Engines.RandomEncounterSystem
{
    public class RandomEncounter : IElementContainer
    {
        private XmlNode m_XmlNode; // e.g., "Mobile"
        private string m_Facet; // e.g., "Felucca"
        private string m_RegionType; // e.g., "Guarded"
        private string m_RegionName; // e.g., "Minoc"
        private float m_Probability; // 0.0-1.0, where 1.0==100%
        private int m_Shortest; // minimum range from player
        private int m_Farthest; // maximum range from player
        private int m_Distance; // actual range from player
        private LandType m_LandType; // encounter on a road T/F
        private EncounterTime m_EncounterTime; // encounter at particular time Day, Night, Any
        private double m_Level; // encounter on a road T/F
        private LevelType m_LevelType; // encounter on a road T/F
        private bool m_ScaleUp; // encounter on a road T/F
        private bool m_Inclusive; // encounter on a road T/F
        private ArrayList m_Elements; // list of things in the encounter...

        public XmlNode XmlNode
        {
            get { return m_XmlNode; }
        }

        public string Key
        {
            get { return m_Facet + ":" + m_RegionType + ":" + m_RegionName + ":" + m_LandType + ":" + m_EncounterTime; }
        }

        public string Facet
        {
            get { return m_Facet; }
        }

        public string RegionType
        {
            get { return m_RegionType; }
        }

        public string RegionName
        {
            get { return m_RegionName; }
        }

        public float Probability
        {
            get { return m_Probability; }
        }

        public int Distance
        {
            get { return m_Distance; }
        }

        public LandType LandType
        {
            get { return m_LandType; }
        }

        public EncounterTime EncounterTime
        {
            get { return m_EncounterTime; }
        }

        public double Level
        {
            get { return m_Level; }
        }

        public LevelType LevelType
        {
            get { return m_LevelType; }
        }

        public bool ScaleUp
        {
            get { return m_ScaleUp; }
        }

        public bool Inclusive
        {
            get { return m_Inclusive; }
        }

        public ArrayList Elements
        {
            get { return m_Elements; }
        }

        //--------------------------------------------------------------------- 
        //  Ctor
        //--------------------------------------------------------------------- 
        public RandomEncounter(
            XmlNode node,
            string facet,
            string regionType,
            string regionName,
            string probability,
            string shortest,
            string farthest,
            string landType,
            string time,
            string level,
            string levelType,
            string scaleUp
            )
        {
            m_XmlNode = node;
            m_Facet = facet;
            m_RegionType = regionType;
            m_RegionName = regionName;

            if( probability == "*" )
            {
                m_Probability = -1;
                m_Inclusive = true;
            }

            else
            {
                m_Probability = float.Parse( probability, RandomEncounterEngine.Language );
                m_Inclusive = false;
            }

            m_Shortest = int.Parse( shortest );
            m_Farthest = int.Parse( farthest );
            m_LandType = (LandType)Enum.Parse( typeof( LandType ), landType );
            m_EncounterTime = (EncounterTime)Enum.Parse( typeof( EncounterTime ), time );
            m_Level = double.Parse( level );
            m_LevelType = (LevelType)Enum.Parse( typeof( LevelType ), levelType );
            m_ScaleUp = bool.Parse( scaleUp );
            m_Elements = new ArrayList();
        }

        //--------------------------------------------------------------------- 
        public void AddElement( EncounterElement element )
        {
            m_Elements.Add( element );
        }

        //----------------------------------------------------------------------
        //  Pick() -- this takes an encounter description and creates a real
        //            instance. The primary purpose is to take the min/max
        //            ranges of the elements and convert them into a specific
        //            count -- nothing fancy. )
        //----------------------------------------------------------------------
        public RandomEncounter Pick()
        {
            RandomEncounter actualEncounter = new RandomEncounter(
                m_XmlNode,
                m_Facet,
                m_RegionType,
                m_RegionName,
                m_Inclusive ? "*" : m_Probability.ToString(),
                m_Shortest.ToString(),
                m_Farthest.ToString(),
                m_LandType.ToString(),
                m_EncounterTime.ToString(),
                m_Level.ToString(),
                m_LevelType.ToString(),
                m_ScaleUp.ToString()
                );

            actualEncounter.m_Distance = Utility.RandomMinMax( m_Shortest, m_Farthest );

            foreach( EncounterElement element in m_Elements )
            {
                ArrayList pickedElements = element.Pick();

                foreach( EncounterElement pickedElement in pickedElements )
                {
                    actualEncounter.m_Elements.Add( pickedElement );
                }
            }

            return actualEncounter;
        }

        //--------------------------------------------------------------------- 
        public override string ToString()
        {
            return Key + ":p=" + m_Probability + ":level=" + m_Level + ":class=" + m_LevelType + ":scaleUp=" + m_ScaleUp +
                   ":distance=(" + m_Shortest + ":" + m_Farthest + "=" + m_Distance + ")" + ":at=" +
                   m_XmlNode.Attributes[ "lineNumber" ].Value;
        }
    }
}