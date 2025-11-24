/***************************************************************************
FILE:	BaseDNA.cs
AUTHOR: Fabrizio Castellano
EMAIL: fabrizio.castellano (at) gmail.com
DATE: 21/12/2008

CONTENT: 
	class BaseDNA
	
DEPENDENCIES:
	Standard RunUO 2.0 GenericReader class
	Standard RunUO 2.0 GenericWriter class
	
INSTALLATION: Just add to scripts

DESCRIPTION:
	BaseDNA class provides a data class to hold DNA information
	and methods for genetic hybridization
	BaseDNA can Serialize and Deserialize itself using standard RunUO Serialization classes
	
	DNA is a sequence of genes (bytes)
	Every gene (byte) is composed of 4 sub-genes (bit pairs)
	Every sub-gene is composed of 2 alleles (bits)
	
	Every gene should be used to code the information about a specific characteristics of the animal
	so there are 256 possible expressions for every characteristic.

USE: This is an abstract class and you probably don't need to mess with it directly.
	Derived classes should provide the mapping between genes ans physical characteristics
	as well as a way to decode DNA.
	Look into AnimalDNA.cs for an example

SEE ALSO:
	AnimalDNA.cs
	PhenotypeMap.cs
***************************************************************************/

namespace Server.Mobiles
{
    public abstract class BaseDNA
    {
        protected byte[] m_Sequence; // the DNA sequence

        [CommandProperty( AccessLevel.GameMaster )]
        public byte[] Sequence { get { return m_Sequence; } set { m_Sequence = value; } } // m_Sequence accessor

        public abstract int Length { get; } // derived classes must declare the length of DNA

        // derived classes must provide hybridization rules for each gene
        public abstract byte HybridizeGene( int geneIndex, byte father, byte mother );

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Owner { get; set; }

        #region Hybridization methods
        // this is a collection of hybridization methods
        // derived classes can use them in their implementation of HybridizeGene method

        // standard Mendel-like hybridization for each of the 4 sub-genes
        // Every sub-gene is a bit pair AB.
        // Suppose we have a father AB and a mother CD
        // The son will be: AC or AD or BC or BD
        //
        // Genes obeying this rules should be used to index a properly filled 
        // phenotype map in order to get their meaning
        public static byte StandardGeneticHybridization( byte father, byte mother )
        {
            byte child = 0x00;
            for( int i = 0; i < 4; i++ )
            {
                bool fromFather = ( father & ( Utility.RandomBool() ? 0x01 << 2 * i : 0x1 << 2 * i + 1 ) ) != 0; // the allele value from father
                bool fromMother = ( mother & ( Utility.RandomBool() ? 0x01 << 2 * i : 0x1 << 2 * i + 1 ) ) != 0; // the allele value from mother

                if( fromFather )
                    child |= (byte)( 0x1 << 2 * i );	// set the father value if it is 1 (0 already set)
                if( fromMother )
                    child |= (byte)( 0x1 << 2 * i + 1 ); // set the mother value if it is 1 (0 already set)
            }

            return child;
        }

        // Linear hybridization:
        // We assume that this genetic expression is controlled by many genes
        // and that the final result is that the offspring hinerits a linear combination
        // of parents' values
        //
        // Parameters:
        // father: father's gene
        // mother: mother's gene
        // min,max: extension indices
        //
        // when min=0 and max=100 the offspring will inherit a random value between the parents' values.
        // min < 0 means that the offspring can inherit a lower value
        // max > 100 means that the offspring can inherit a higher value
        public static byte LinearValueHybridization( byte father, byte mother, int min, int max )
        {
            int x = Utility.RandomMinMax( min, max );
            x = ( father * x + mother * ( 100 - x ) ) / 100; // use x as intermediate variable to avoid overflow of byte type

            return (byte)x;	// cast back to byte
        }

        // standard inperpolation between parents' values
        public static byte LinearValueHybridization( byte father, byte mother )
        {
            return LinearValueHybridization( father, mother, 0, 100 );
        }
        #endregion

        public BaseDNA()
        {
            m_Sequence = new byte[ Length ];
            Owner = null;
        }

        // hybridization constructor
        public BaseDNA( BaseDNA father, BaseDNA mother )
            : this()
        {
            for( int i = 0; i < Length; i++ )
                m_Sequence[ i ] = HybridizeGene( i, father.m_Sequence[ i ], mother.m_Sequence[ i ] );
        }

        public virtual void ApplySpecialCriteria( Mobile m )
        {
        }

        #region serialization
        public BaseDNA( GenericReader reader, Mobile owner )
            : this()
        {
            Owner = owner;

            int length = reader.ReadInt();

            for( int i = 0; i < length; i++ )
                m_Sequence[ i ] = reader.ReadByte();
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( Length );

            for( int i = 0; i < Length; i++ )
                writer.Write( m_Sequence[ i ] );
        }
        #endregion

        public virtual string Log()
        {
            return m_Sequence.ToString();
        }
    }
}