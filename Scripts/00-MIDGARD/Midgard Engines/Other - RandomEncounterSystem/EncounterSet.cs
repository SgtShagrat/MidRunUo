using System;
using System.Collections;

namespace Midgard.Engines.RandomEncounterSystem
{
    //--------------------------------------------------------------------------
    //  An "EncounterSet" is a set of encounters at equal probability. This
    //     exists so that we can collect and randomly pick from encounters at
    //     the same probability within the same region...
    //--------------------------------------------------------------------------
    public class EncounterSet : ArrayList, IComparable, IProbability
    {
        private float m_Probability;

        public float Probability
        {
            get { return m_Probability; }
        }

        public EncounterSet( float probability )
        {
            m_Probability = probability;
        }

        //--------------------------------------------------------------------- 
        //  This CompareTo() method will be used later by the RedBlackTree
        //  container; it will use this method to automatically keep everything
        //  ordered by probability (we'll need this ordering later when we make
        //  a random draw, and try to decide which encounter (if any) to give
        //  a particular player
        //--------------------------------------------------------------------- 
        public int CompareTo( object obj )
        {
            IProbability toCompare = (IProbability)obj;

            if( m_Probability < toCompare.Probability )
                return -1;
            if( m_Probability > toCompare.Probability )
                return 1;
            else
                return 0;
        }

        //--------------------------------------------------------------------- 
        //  So we don't have to generate an array to search the tree; just a
        //  hack for the persnickety.
        //--------------------------------------------------------------------- 
        public class QuickSearch : IComparable, IProbability
        {
            private float m_Probability;

            public float Probability
            {
                get { return m_Probability; }
            }

            public QuickSearch( float probability )
            {
                m_Probability = probability;
            }

            public int CompareTo( object obj )
            {
                IProbability toCompare = (IProbability)obj;

                if( m_Probability < toCompare.Probability )
                    return -1;
                if( m_Probability > toCompare.Probability )
                    return 1;
                else
                    return 0;
            }
        }
    }
}