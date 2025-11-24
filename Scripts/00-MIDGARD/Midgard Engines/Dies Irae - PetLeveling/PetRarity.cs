/***************************************************************************
 *                                 		PetRarity.cs
 *                            		-------------------
 *  begin                	: Febbraio, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *
 ***************************************************************************/

using Server;
using System;
using Server.Mobiles;

namespace Midgard.Engines.PetSystem
{
    public class PetRarity
    {
        public enum Rarity
        {
            Common = 0,		// 85%
            Uncommon,	    // 10%
            Rare,		    // 4%
            Unique,		    // 1%
            Legendary,	    // 1%%
        }

        #region metodi privati
        private static double[] m_RarityTable = new double[] { 0.85, 0.10, 0.01, 0.001 };

        private static Type[] m_RarePetsList =
		{
			typeof(Dragon),
			typeof(WhiteWyrm),
			typeof(Nightmare),
			typeof(Kirin),
			typeof(Unicorn),
		};

        public static int CheckPetHue( Mobile pet, Rarity rarity )
        {
            int hue = 0;

            for( int i = 0; i < PetEntryList.Length; i++ )
            {
                if( PetEntryList[ i ].PetType == pet.GetType() && PetEntryList[ i ].Rarity == rarity )
                {
                    return Utility.RandomList( PetEntryList[ i ].Hues );
                }
            }
            return hue;
        }

        private static int CheckMaxLevel( Rarity rarity )
        {
            int level = 10;

            switch( rarity )
            {
                case Rarity.Uncommon:
                    level = Utility.RandomMinMax( 21, 25 );
                    break;
                case Rarity.Rare:
                    level = Utility.RandomMinMax( 26, 29 );
                    break;
                case Rarity.Unique:
                    level = 30;
                    break;
                case Rarity.Legendary:
                    level = 35;
                    break;
                default:
                    level = Utility.RandomMinMax( 10, 20 );
                    break;
            }

            return level;
        }

        private static Rarity CheckPetRarity()
        {
            double chance = Utility.RandomDouble();

            for( int i = 0; i < m_RarityTable.Length; i++ )
            {
                if( chance < m_RarityTable[ i ] )
                    return (Rarity)i;
            }

            return Rarity.Common;
        }
        #endregion

        #region metodi pubblici
        public static bool IsRarePet( BaseCreature pet )
        {
            for( int i = 0; i < m_RarePetsList.Length; i++ )
            {
                if( pet.GetType() == m_RarePetsList[ i ] )
                {
                    return true;
                }
            }
            return false;
        }

        public static void DoRarityMorph( BaseCreature pet )
        {
            Rarity rarity = CheckPetRarity();

            pet.RarityLevel = rarity;

            if( rarity == Rarity.Common )
                return;

            // Scalatura delle stats del 5% per ogni livello
            pet.RawStr = (int)( pet.RawStr * ( 1.0 + ( (int)rarity * 5.0 / 100.0 ) ) );
            pet.RawDex = (int)( pet.RawDex * ( 1.0 + ( (int)rarity * 5.0 / 100.0 ) ) );
            pet.RawInt = (int)( pet.RawInt * ( 1.0 + ( (int)rarity * 5.0 / 100.0 ) ) );

            // Append del tag al nome
            if( rarity > Rarity.Uncommon )
            {
                string name = pet.Name;
                if( pet.Name.StartsWith( "a ", true, null ) )
                {
                    name = name.Remove( 0, 2 );
                    pet.Name = String.Concat( "a", " ", Enum.GetName( typeof( Rarity ), rarity ).ToLower(), " ", name );
                }
                else if( pet.Name.StartsWith( "an ", true, null ) )
                {
                    name = name.Remove( 0, 3 );
                    pet.Name = String.Concat( "a", " ", Enum.GetName( typeof( Rarity ), rarity ).ToLower(), " ", name );
                }
                else
                {
                    pet.Name = String.Concat( name, "( ", Enum.GetName( typeof( Rarity ), rarity ).ToLower(), " )" );
                }
            }

            // Settaggio del livello
            pet.MaxLevel = CheckMaxLevel( rarity );

            // Settaggio della skill minima per essere tamato
            pet.MinTameSkill = pet.MinTameSkill * Math.Pow( 1.02, (int)rarity );

            // Settaggio del colore
            pet.Hue = CheckPetHue( pet, rarity );
        }
        #endregion

        public class RarePetHueEntry
        {
            public Type PetType { get; private set; }
            public Rarity Rarity { get; private set; }
            public int[] Hues { get; private set; }

            public RarePetHueEntry( Type pettype, Rarity rarity, int[] hues )
            {
                PetType = pettype;
                Rarity = rarity;
                Hues = hues;
            }
        }

        #region Lista di RarePetHueEntry
        public static RarePetHueEntry[] PetEntryList = new RarePetHueEntry[]
		{	
			// WhiteWyrm
			new RarePetHueEntry( typeof( WhiteWyrm ), Rarity.Uncommon, new int[]{2332,2673,2200,2440,2357,2364,2653} ),
			new RarePetHueEntry( typeof( WhiteWyrm ), Rarity.Rare, new int[]{2337,2338,2340,1952,2619,2334} ),
			new RarePetHueEntry( typeof( WhiteWyrm ), Rarity.Unique, new int[]{2290,2336,2448,2348,2437} ),
			new RarePetHueEntry( typeof( WhiteWyrm ), Rarity.Legendary, new int[]{2560} ),
			
			// Dragon
			new RarePetHueEntry( typeof( Dragon ), Rarity.Uncommon, new int[]{2444,2185,2066,2667} ),
			new RarePetHueEntry( typeof( Dragon ), Rarity.Rare, new int[]{1762,1764,2649,1761,2283} ),
			new RarePetHueEntry( typeof( Dragon ), Rarity.Unique, new int[]{2776,2616,1760,2265} ),
			new RarePetHueEntry( typeof( Dragon ), Rarity.Legendary, new int[]{2358} ),
			
			// Nightmare
			new RarePetHueEntry( typeof( Nightmare ), Rarity.Uncommon, new int[]{2441,2468,2070,2025,2457} ),
			new RarePetHueEntry( typeof( Nightmare ), Rarity.Rare, new int[]{2508,2716,2500,2499,2495,2358,1175} ),
			new RarePetHueEntry( typeof( Nightmare ), Rarity.Unique, new int[]{2621,2985,2496,2539,2776,2283} ),
			new RarePetHueEntry( typeof( Nightmare ), Rarity.Legendary, new int[]{2026} ),
			
			// Unicorn
			new RarePetHueEntry( typeof( Unicorn ), Rarity.Uncommon, new int[]{1153} ),
			new RarePetHueEntry( typeof( Unicorn ), Rarity.Rare, new int[]{2024,2025,1915} ),
			new RarePetHueEntry( typeof( Unicorn ), Rarity.Unique, new int[]{2293} ),
			new RarePetHueEntry( typeof( Unicorn ), Rarity.Legendary, new int[]{2061} ),
			
			// Kirin
			new RarePetHueEntry( typeof( Kirin ), Rarity.Uncommon, new int[]{180,2169,1840,1900} ),
			new RarePetHueEntry( typeof( Kirin ), Rarity.Rare, new int[]{2506} ),
			new RarePetHueEntry( typeof( Kirin ), Rarity.Unique, new int[]{2568} ),
			new RarePetHueEntry( typeof( Kirin ), Rarity.Legendary, new int[]{1801} ),
		};
        #endregion
    }
}
