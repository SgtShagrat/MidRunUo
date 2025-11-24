/***************************************************************************
FILE:	PhenotypeMap.cs
AUTHOR: Fabrizio Castellano
EMAIL: fabrizio.castellano (at) gmail.com
DATE: 21/12/2008

CONTENT: 
	class PhenotypeMap
	
DEPENDENCIES: none

INSTALLATION: Just add to scripts

DESCRIPTION: Helper class with only static properties to provide standard phenotype maps
	
USE: Use the provided properties to access predefined phenotype maps in DNA scripts

SEE ALSO:
	AnimalDNA.cs
***************************************************************************/

using System.Collections.Generic;

namespace Server.Mobiles
{
    public class PhenotypeMap
    {
        public static List<int> GetValidIndexes( int[] array, int indexValue )
        {
            List<int> list = new List<int>();
            for( int i = 0; i < array.Length; i++ )
            {
                if( array[ i ] == indexValue )
                    list.Add( i );
            }

            return list;
        }

        public static int RandomValidSequence( int[] array, int indexValue )
        {
            List<int> list = GetValidIndexes( array, indexValue );
            return list.Count > 0 ? Utility.RandomList( list.ToArray() ) : -1;
        }

        // A 3-gene phenotype map with complete dominance
        //
        //	There are 8 possible phenotypes:
        //	0: 00 00 00				(recessive, recessive, recessive) (1/64)
        //	1: 00 00 (01 10 11)			(recessive, recessive, dominant) (3/64)
        //	2: 00 (01 10 11) 00			(recessive, dominant, recessive) (3/64)
        //	3: 00 (01 10 11) (01 10 11)		(recessive, dominant, dominant) (9/64)
        //
        //	4 : (01 10 11) 00 00			(dominant, recessive, recessive) (3/64)
        //	5: (01 10 11) 00 (01 10 11)		(dominant, recessive, dominant) (9/64)
        //	6: (01 10 11) (01 10 11) 00		(dominant, dominant, recessive) (9/64)
        //	7: (01 10 11) (01 10 11) (01 10 11)	(dominant, dominant, dominant) (27/64)
        //
        public static readonly int[] TripleGene = new int[ 64 ]{
			// (00 xx xx)
			0, 1, 1, 1,	//(00 00 xx) 
			2, 3, 3, 3,	//(00 01 xx)
			2, 3, 3, 3,	//(00 10 xx)
			2, 3, 3, 3,	//(00 11 xx)
			
			// (01 xx xx)
			4, 5, 5, 5,	//(01 00 xx) 
			6, 7, 7, 7,	//(01 01 xx)
			6, 7, 7, 7,	//(01 10 xx)
			6, 7, 7, 7,	//(01 11 xx)
			
			// (10 xx xx)
			4, 5, 5, 5,	//(10 00 xx) 
			6, 7, 7, 7,	//(10 01 xx)
			6, 7, 7, 7,	//(10 10 xx)
			6, 7, 7, 7,	//(10 11 xx)
			
			// type1 (11 xx xx)
			4, 5, 5, 5,	//(11 00 xx) 
			6, 7, 7, 7,	//(11 01 xx)
			6, 7, 7, 7,	//(11 10 xx)
			6, 7, 7, 7,	//(11 11 xx)
		};

        // A 3-gene phenotype map with incomplete dominance on the first gene
        // 
        //	There are 12 possible phenotypes:
        //	0: 00 00 00				(type0, recessive, recessive) (1/64)
        //	1: 00 00 (01 10 11)			(type0, recessive, dominant) (3/64)
        //	2: 00 (01 10 11) 00			(type0, dominant, recessive) (3/64)
        //	3: 00 (01 10 11) (01 10 11)		(type0, dominant, dominant) (9/64)
        //
        //	4 : (01 10) 00 00			(type1, recessive, recessive) (2/64)
        //	5: (01 10) 00 (01 10 11)		(type1, recessive, dominant) (6/64)
        //	6: (01 10) (01 10 11) 00		(type1, dominant, recessive) (6/64)
        //	7: (01 10) (01 10 11) (01 10 11)	(type1, dominant, dominant) (18/64)
        //
        //	8 : 10 00 00				(type2, recessive, recessive) (1/64)
        //	9: 10 00 (01 10 11)			(type2, recessive, dominant) (3/64)
        //	10: 10 (01 10 11) 00			(type2, dominant, recessive) (3/64)
        //	11: 10 (01 10 11) (01 10 11)		(type2, dominant, dominant) (9/64)
        //
        public static readonly int[] TripleGeneIncomplete = new int[ 64 ]{
			// type0 (00 xx xx)
			0, 1, 1, 1,	//(00 00 xx) 
			2, 3, 3, 3,	//(00 01 xx)
			2, 3, 3, 3,	//(00 10 xx)
			2, 3, 3, 3,	//(00 11 xx)
			
			// type1 (01 xx xx)
			4, 5, 5, 5,	//(01 00 xx) 
			6, 7, 7, 7,	//(01 01 xx)
			6, 7, 7, 7,	//(01 10 xx)
			6, 7, 7, 7,	//(01 11 xx)
			
			// type1 (10 xx xx)
			7, 7, 7, 6,	//(10 00 xx) 
			7, 7, 7, 6,	//(10 01 xx)
			7, 7, 7, 6,	//(10 10 xx)
			5, 5, 5, 4,	//(10 11 xx)
			
			// type2 (11 xx xx)
			11, 11, 11, 10,	//(11 00 xx) 
			11, 11, 11, 10,	//(11 01 xx)
			11, 11, 11, 10,	//(11 10 xx)
			9, 9, 9, 8,	//(11 11 xx)
		};
    }
}