/***************************************************************************
 *                               DiceRoll.cs
 *
 *   begin                : 23 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;

namespace Midgard
{
    public class DiceRoll
    {
        private static readonly Dictionary<string, DiceRoll> m_Dict = new Dictionary<string, DiceRoll>();

        /// <summary>
        /// The number of times our dice is rolled
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The number of sides of our dice
        /// </summary>
        public int Sides { get; private set; }

        /// <summary>
        /// The final bonus added to our roll
        /// </summary>
        public int Bonus { get; private set; }

        /// <summary>
        /// The minimum value rollable with this dice
        /// </summary>
        public int MinValue { get; private set; }

        /// <summary>
        /// The maximum value rollable with this dice
        /// </summary>
        public int MaxValue { get; private set; }

        public DiceRoll( string str )
        {
            int start = 0;
            int index = str.IndexOf( 'd', start );

            if( index < start )
                return;

            Count = Utility.ToInt32( str.Substring( start, index - start ) );

            start = index + 1;
            index = str.IndexOf( '+', start );

            bool negative = index < start;

            if( negative )
                index = str.IndexOf( '-', start );

            if( index < start )
                index = str.Length;

            Sides = Utility.ToInt32( str.Substring( start, index - start ) );

            if( index == str.Length )
                return;

            start = index + 1;
            index = str.Length;

            Bonus = Utility.ToInt32( str.Substring( start, index - start ) );

            if( negative )
                Bonus *= -1;
        }

        public DiceRoll( int count, int sides, int bonus )
        {
            Count = count;
            Sides = sides;
            Bonus = bonus;

            MinValue = ( Count * 1 ) + Bonus;
            MaxValue = ( Count * Sides ) + Bonus;
        }

        public DiceRoll this[ string diceVal ]
        {
            get
            {
                if( m_Dict.ContainsKey( diceVal ) )
                    return m_Dict[ diceVal ];
                else
                {
                    DiceRoll newDice = m_Dict[ diceVal ] = new DiceRoll( diceVal );
                    return newDice;
                }
            }
        }

        #region Serialize-Deserialize
        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 );

            writer.WriteEncodedInt( Count );
            writer.WriteEncodedInt( Sides );
            writer.WriteEncodedInt( Bonus );
        }

        public static DiceRoll Deserialize( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            int count = reader.ReadEncodedInt();
            int sides = reader.ReadEncodedInt();
            int bonus = reader.ReadEncodedInt();

            return new DiceRoll( count, sides, bonus );
        }
        #endregion

        public static int Roll( string diceVal )
        {
            if( m_Dict.ContainsKey( diceVal ) )
                return m_Dict[ diceVal ].Roll();
            else
            {
                DiceRoll newDice = m_Dict[ diceVal ] = new DiceRoll( diceVal );
                return newDice.Roll();
            }
        }

        public int Roll()
        {
            int v = Bonus;

            for( int i = 0; i < Count; ++i )
                v += Utility.Random( 1, Sides );

            return v;
        }

        public static DiceRoll OneDiceThree = new DiceRoll( 1, 3, 0 );
        public static DiceRoll OneDiceFour = new DiceRoll( 1, 4, 0 );
        public static DiceRoll OneDiceFive = new DiceRoll( 1, 5, 0 );
        public static DiceRoll OneDiceTen = new DiceRoll( 1, 10, 0 );
        public static DiceRoll OneDiceTwelve = new DiceRoll( 1, 12, 0 );
        public static DiceRoll OneDiceTwenty = new DiceRoll( 1, 20, 0 );
        public static DiceRoll OneDiceHundred = new DiceRoll( 1, 100, 0 );
        public static DiceRoll OneDiceThousand = new DiceRoll( 1, 1000, 0 );

        public static int Roll( DiceRoll dice )
        {
            return dice.Roll();
        }

        public override string ToString()
        {
            return string.Format( "{0}d{1}+{2}", Count, Sides, Bonus );
        }
    }
}