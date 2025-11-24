/***************************************************************************
 *                               FishRanks.cs
 *
 *   begin                : 31 ottobre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Midgard.Engines.AdvancedFishing
{
    public class FishRanks
    {
        private static Dictionary<Type, FishRankState> m_Dict = new Dictionary<Type, FishRankState>();

        public static Dictionary<Type, FishRankState> FishRanksDict
        {
            get { return m_Dict; }
        }

        public static bool RegisterNewState( FishRankState state )
        {
            if( FishRanksDict == null )
                m_Dict = new Dictionary<Type, FishRankState>();

            if( FishRanksDict != null )
            {
                if( !FishRanksDict.ContainsKey( state.FishType ) )
                {
                    FishRanksDict[ state.FishType ] = state;
                    return true;
                }
                else
                {
                    FishRankState oldState = FishRanksDict[ state.FishType ];
                    if( oldState.FishWeight > state.FishWeight )
                    {
                        FishRanksDict[ state.FishType ] = state;
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<FishRankState> BuildList()
        {
            List<FishRankState> states = new List<FishRankState>();

            if( FishRanksDict == null )
                return states;
            else
            {
                foreach( KeyValuePair<Type, FishRankState> keyValuePair in FishRanksDict )
                    states.Add( keyValuePair.Value );
            }

            return states;
        }
    }
}