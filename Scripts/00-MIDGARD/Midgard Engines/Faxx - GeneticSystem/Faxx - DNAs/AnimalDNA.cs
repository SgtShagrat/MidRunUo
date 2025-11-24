/***************************************************************************
FILE:	AnimalDNA.cs
AUTHOR: Fabrizio Castellano
EMAIL: fabrizio.castellano (at) gmail.com
DATE: 21/12/2008

CONTENT: 
	class AnimalDNA
	
DEPENDENCIES:
	BaseDNA class
	PhenotypeMap class
	Standard UO implementation of str, dex, int, bodyID, hue
	
INSTALLATION: Just add to scripts

DESCRIPTION:
	AnimalDNA provides a standard DNA for RunUO creatures.
	The DNA is composed of 4 genes.
	gene 0: str - linear hybridization
	gene 1: dex - linear hybridization
	gene 2: int - linear hybridization
	gene 3: hits - linear hybridization
	gene 4: look - Mendel hybridization
	
	gene 4 contains coding about BodyID and Hue.
		Body: first 2 bits
		Hue: last 6 bits
	
		Body is used to index a list of 4 possible bodies.
		Hue is used to index a phenotype map to get the Hue index (HueIdx)
		
		HueIdx is then used to index a hue list to get the actual hue in hues.mul
	
	
USE: Use AnimalDNA as base class to inherit specific DNAs, look at HorseDNA.cs as an example

SEE ALSO:
	HorseDNA.cs
	BaseDNA.cs
	PhenotypeMap.cs
***************************************************************************/

using System;
using System.Text;

namespace Server.Mobiles
{
    [PropertyObject]
    public class AnimalDNA : BaseDNA
    {
        #region Standard customization
        // These properties have to be overridden in order to customize genetic expression
        public virtual int[] HueList { get { return m_StandardHueList; } }
        public virtual int[] BodyList { get { return m_StandardBodyList; } }

        // you want to declare something like this in your derived classes.
        private static readonly int[] m_StandardHueList = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // 12 values for TripleGeneIncomplete map
        private static readonly int[] m_StandardBodyList = new int[] { 0, 0, 0, 0 }; // always 4 values

        // Override this only if you understand what a phenotype map is!!
        public virtual int[] ColorPhenotypeMap { get { return PhenotypeMap.TripleGeneIncomplete; } }
        #endregion

        #region Genetic expressions
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int Str { get { return (int)m_Sequence[ 0 ]; } set { m_Sequence[ 0 ] = (byte)value; } }	// str directly mapped to genetic value (0-255)

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int Dex { get { return (int)m_Sequence[ 1 ]; } set { m_Sequence[ 1 ] = (byte)value; } }	// dex directly mapped to genetic value (0-255)

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int Int { get { return (int)m_Sequence[ 2 ]; } set { m_Sequence[ 2 ] = (byte)value; } }	//int directly mapped to genetic value (0-255)

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int Color // The color of the animal
        {
            get
            {
                // last 6 bits of gene 3 are used to index the phenotype map
                // to get the hue idx (0 to 11 in m_StandardHueList)
                int hueIdx = ColorPhenotypeMap[ m_Sequence[ 3 ] & 0x3F ];

                // hueIdx is used to index the HueList and get the actual hues.mul index
                int hue = HueList[ hueIdx ];
                return hue;
            }
            set
            {
                int hueBits = Array.IndexOf( HueList, value ); // looks: 10 00 00 00 or 01 00 00 00

                if( hueBits > -1 )
                {
                    int phenoHue = PhenotypeMap.RandomValidSequence( ColorPhenotypeMap, hueBits );
                    if( phenoHue > -1 )
                    {
                        // first we clear the 1st->6th bits with 0xC0 = 11 00 00 00
                        // then set the 1st->6th bits to our new value
                        m_Sequence[ 3 ] = (byte)( ( m_Sequence[ 3 ] & 0xC0 ) | phenoHue );

                        if( Owner != null )
                            Owner.Hue = value;
                    }
                }
                else
                    Console.WriteLine( "Warning: trying to set an invalid DNA Color value: {0}", value );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int Body // the body ID
        {
            get
            {
                // first 2 bits of gene 3 are used to index the BodyList
                // and get a bodyID
                int idx = ( m_Sequence[ 3 ] & 0xC0 ) >> 6;
                return BodyList[ idx ];
            }
            set
            {
                int bodyBits = Array.IndexOf( BodyList, value ) << 6; // looks: 10 00 00 00 or 01 00 00 00
                if( bodyBits > -1 )
                {
                    // first we clear the 7th and 8th bits with 0x3F = 00 11 11 11
                    // then set the 7th and 8th bits to our new value
                    m_Sequence[ 3 ] = (byte)( ( m_Sequence[ 3 ] & 0x3F ) | bodyBits );

                    if( Owner != null )
                        Owner.Body = value;
                }
                else
                    Console.WriteLine( "Warning: trying to set an invalid DNA Body value: {0}", value );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int DamageMin { get { return (int)m_Sequence[ 4 ]; } set { m_Sequence[ 4 ] = (byte)value; } }	//damage directly mapped to genetic value (0-255)

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int DamageMax { get { return (int)m_Sequence[ 5 ]; } set { m_Sequence[ 5 ] = (byte)value; } }	//damage directly mapped to genetic value (0-255)

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int VirtualArmor { get { return (int)m_Sequence[ 6 ]; } set { m_Sequence[ 6 ] = (byte)value; } }
        #endregion

        #region Hybridization override
        [CommandProperty( AccessLevel.GameMaster )]
        public override int Length { get { return 7; } } // DNA length

        // Hybridization rules for DNA
        public override byte HybridizeGene( int n, byte father, byte mother )
        {
            switch( n )
            {
                case 0:
                    return LinearValueHybridization( father, mother, -10, 110 ); // str, dex, int are hybridized linearly
                case 1:
                    return LinearValueHybridization( father, mother, -10, 110 ); // str, dex, int are hybridized linearly
                case 2:
                    return LinearValueHybridization( father, mother ); // str, dex, int are hybridized linearly
                case 3:
                    return StandardGeneticHybridization( father, mother ); // color follows standard rules
                case 4:
                    return LinearValueHybridization( father, mother, -10, 110 ); // damage hybridized linearly
                case 5:
                    return LinearValueHybridization( father, mother, -10, 110 ); // damage hybridized linearly
                case 6:
                    return LinearValueHybridization( father, mother, -5, 105 ); // virtual armor hybridized linearly
            }

            return 0;
        }
        #endregion

        #region Constructors
        // We should initialize the DNA sequence to something
        // when the DNA is first created
        public AnimalDNA()
        {
            m_Sequence[ 0 ] = 50;   // str
            m_Sequence[ 1 ] = 50;   // dex
            m_Sequence[ 2 ] = 50;   // int
            m_Sequence[ 3 ] = 0x55; // body (1 byte) - hue (2 bytes)
            m_Sequence[ 4 ] = 0;    // damage min
            m_Sequence[ 5 ] = 0;    // damage max
            m_Sequence[ 6 ] = 0;    // virtual armor
        }

        public AnimalDNA( BaseDNA father, BaseDNA mother )
            : base( father, mother )
        {
        }

        public AnimalDNA( GenericReader reader, Mobile owner )
            : base( reader, owner )
        {
        }
        #endregion

        public override string Log()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine( string.Format("Str: {0} - Dex: {1} - Int {2} - Color {3} - Body {4} - DamageMin {5} - DamageMax {6} - VirtualArmor {7}",
                Str, Dex, Int, Color, Body, DamageMin, DamageMax, VirtualArmor ) );

            return sb.ToString();
        }
    }
}