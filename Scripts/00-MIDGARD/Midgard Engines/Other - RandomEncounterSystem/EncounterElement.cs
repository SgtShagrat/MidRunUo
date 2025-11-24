using System;
using System.Collections;
using System.Xml;
using Server;

namespace Midgard.Engines.RandomEncounterSystem
{
    //--------------------------------------------------------------------------
    //  Random Encounter Element : Each encounter might include several descriptions
    //   of mobiles and items... this is an "element"
    //--------------------------------------------------------------------------
    public class EncounterElement : IElementContainer
    {
        private XmlNode m_XmlNode; // e.g., "Mobile"
        private float m_Probability; // 0.0-1.0, where 1.0==100%
        private int m_Min, m_Max; // i.e., the range of possible N
        private int m_N; // actual is for the draw
        private string m_PickFrom; // e.g., "OrcCaptain"
        private int m_ID; // numeric: items only
        private bool m_ForceAttack; // force mob to attack player
        private EffectType m_Effect; // 
        private int m_EffectHue;
        private ArrayList m_Elements; // list of things in the encounter...
        //----------------------------------------------------------------------
        public XmlNode XmlNode
        {
            get { return m_XmlNode; }
        }

        public float Probability
        {
            get { return m_Probability; }
        }

        public int N
        {
            get { return m_N; }
        }

        public string PickFrom
        {
            get { return m_PickFrom; }
        }

        public int ID
        {
            get { return m_ID; }
        }

        public bool ForceAttack
        {
            get { return m_ForceAttack; }
        }

        public EffectType Effect
        {
            get { return m_Effect; }
        }

        public int EffectHue
        {
            get { return m_EffectHue; }
        }

        public ArrayList Elements
        {
            get { return m_Elements; }
        }

        //----------------------------------------------------------------------
        //  Ctor
        //----------------------------------------------------------------------
        public EncounterElement( XmlNode xmlNode, string probability, string pickFrom, string id, string min, string max,
                                 string force, string effect, string effectHue )
        {
            m_XmlNode = xmlNode;
            m_Probability = float.Parse( probability, RandomEncounterEngine.Language );
            m_PickFrom = pickFrom;
            m_ID = int.Parse( id );
            m_Min = int.Parse( min );
            m_Max = int.Parse( max );
            m_ForceAttack = bool.Parse( force );
            m_Effect = (EffectType)Enum.Parse( typeof( EffectType ), effect );
            m_EffectHue = int.Parse( effectHue );
            m_Elements = new ArrayList();
        }

        //--------------------------------------------------------------------- 
        public void AddElement( EncounterElement element )
        {
            m_Elements.Add( element );
        }

        //----------------------------------------------------------------------
        //  Pick() -- this takes an element description and returns a copy with
        //            its min-max range converted to an actual; nothing big here
        //----------------------------------------------------------------------
        public ArrayList Pick()
        {
            ArrayList pickedElements = new ArrayList();

            //-----------------------------------------------------------------  
            // for elements that may not be present (indicated by a p value),
            // then we check p, not on a per-n basis, but rather for the whole
            // set:
            //-----------------------------------------------------------------  

            if( Utility.RandomDouble() > Probability )
                return pickedElements; // empty list okay

            int n = Utility.RandomMinMax( m_Min, m_Max );
            string[] picks = m_PickFrom.Split( new Char[] { ',' } );
            string pick = "";

            for( int i = 0; i < n; i++ )
            {
                if( picks.Length == 1 )
                    pick = picks[ 0 ];
                else if( picks.Length >= 2 )
                    pick = picks[ Utility.RandomMinMax( 0, picks.Length - 1 ) ];

                EncounterElement pickedElement =
                    new EncounterElement(
                        m_XmlNode,
                        "1",
                        pick,
                        m_ID.ToString(),
                        m_Min.ToString(),
                        m_Max.ToString(),
                        m_ForceAttack.ToString(),
                        m_Effect.ToString(),
                        m_EffectHue.ToString()
                        );

                pickedElement.m_N = n;

                foreach( EncounterElement childElement in m_Elements )
                {
                    ArrayList pickedChildElements = childElement.Pick();
                    foreach( EncounterElement pickedChildElement in pickedChildElements )
                    {
                        pickedElement.m_Elements.Add( pickedChildElement );
                    }
                }

                pickedElements.Add( pickedElement );
            }

            return pickedElements; // empty list okay
        }

        //----------------------------------------------------------------------
        public override string ToString()
        {
            return m_XmlNode.Name + ":p=" + m_Probability + ":pick=" + m_PickFrom + ":id=" + m_ID + ":at=" +
                   m_XmlNode.Attributes[ "lineNumber" ].Value + ":min=" + m_Min + ":max=" + m_Max + ":n=" + m_N +
                   ":force=" + m_ForceAttack;
        }
    }
}