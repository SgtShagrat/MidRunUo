#define temp

using System;
using System.Xml;
using System.Xml.Linq;
using Server;

namespace Midgard.Engines.MonsterMasterySystem
{
    public class EnemyMasteryDefinition
    {
        public EnemyMasteryDefinition( XmlElement element )
        {
            try
            {
                m_Weights = new int[ Enum.GetNames( typeof( MasteryLevel ) ).Length ];

                foreach( string name in Enum.GetNames( typeof( MasteryLevel ) ) )
                {
                    int weight = 0;
                    int index = (int)Enum.Parse( typeof( MasteryLevel ), name );
                    Region.ReadInt32( element, name, ref weight, false );
                    Weights[ index ] = weight;
#if temp
                    Weights[ index ] = index == 0 ? 1 : 0;
#endif
                }

                m_TotalWeight = 0;
                foreach( int t in Weights )
                    m_TotalWeight += t;
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error in EnemyMasteryDefinition: " + ex.Message );
            }
        }

        public int[] Weights
        {
            get { return m_Weights; }
        }

        protected static void Write( ref XElement element, EnemyMasteryDefinition value )
        {
            // <enemyMasteryDefinition Normal = "4" Strong = "3" Elite = "2" Defiant = "1" Boss = "0" />
            element = new XElement( "enemyMasteryDefinition",
                new XAttribute( "Normal", value.Weights[ 0 ] ),
                new XAttribute( "Strong", value.Weights[ 1 ] ),
                new XAttribute( "Elite", value.Weights[ 2 ] ),
                new XAttribute( "Defiant", value.Weights[ 3 ] ),
                new XAttribute( "Boss", value.Weights[ 4 ] ) );
        }

        public EnemyMasteryDefinition( int[] weights )
        {
            m_Weights = weights;

            m_TotalWeight = 0;
            foreach( int t in weights )
                m_TotalWeight += t;
        }

        private readonly int[] m_Weights;
        private readonly int m_TotalWeight;

        public MasteryLevel GetLevel()
        {
            int index = Utility.Random( m_TotalWeight );

            for( int i = 0; i < Weights.Length; i++ )
            {
                int weight = Weights[ i ];

                if( weight > 0 && index < weight )
                    return (MasteryLevel)i;

                index -= weight;
            }

            return MasteryLevel.Normal;
        }

        public MasteryLevel this[ int index ]
        {
            get { return (MasteryLevel)Weights[ index ]; }
        }

        private static readonly double[] m_ChanceBonusScalars = new double[]
        {
            0.00,   // Normal
            0.25,   // Strong
            0.50,   // Elite
            0.75,   // Defiant
            0.90,   // Boss
        };

        public static double GetMasteryBonus( MasteryLevel masteryLevel )
        {
            return m_ChanceBonusScalars[ (int)masteryLevel ];
        }
    }
}