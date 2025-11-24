using System.Collections;
using Server.Misc;

namespace Midgard.Engines.RandomEncounterSystem
{
    //--------------------------------------------------------------------------
    //  RegionRecord -- a place to hang encounters, the per region timers, and
    //       so forth.
    //--------------------------------------------------------------------------
    public class RegionRecord
    {
        private string m_Key;
        private RedBlackTree m_PossibleEncounters;

        public string Key
        {
            get { return m_Key; }
        }

        public RedBlackTree PossibleEncounters
        {
            get { return m_PossibleEncounters; }
        }

        public void Clear()
        {
            m_PossibleEncounters.Clear();
        }

        public RegionRecord( string key )
        {
            m_Key = key;
            m_PossibleEncounters = new RedBlackTree();
        }

        public void AddEncounter( RandomEncounter encounter )
        {
            object o = m_PossibleEncounters.Find( new EncounterSet.QuickSearch( encounter.Probability ) );

            EncounterSet encounterSet;

            if( o == null )
            {
                encounterSet = new EncounterSet( encounter.Probability );
                m_PossibleEncounters.Add( encounterSet );
            }
            else
            {
                encounterSet = (EncounterSet)o;
            }

            encounterSet.Add( encounter );
        }

        public void DumpAll()
        {
            foreach( EncounterSet encounterSet in m_PossibleEncounters )
                foreach( RandomEncounter encounter in encounterSet )
                    RandomEncounterEngine.DumpEncounter( 1, encounter );
        }
    }
}