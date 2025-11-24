/***************************************************************************
FILE:	HorseDNA.cs
AUTHOR: Fabrizio Castellano
EMAIL: fabrizio.castellano (at) gmail.com
DATE: 21/12/2008

CONTENT: 
	class HorseDNA
	
DEPENDENCIES:
	AnimalDNA class
	PhenotypeMap class
	Standard RunUO 2.0 GenericReader class
	
INSTALLATION: Just add to scripts

DESCRIPTION: A sample Horse DNA.
	
USE: Use in the Horse.cs script

SEE ALSO:
	AnimalDNA.cs
	PhenotypeMap.cs
	Horse.cs
***************************************************************************/

namespace Server.Mobiles
{
    public class ColorDNA : AnimalDNA
    {
        // colormap 1
        private static readonly int[] m_HueList = new int[] 
		{ 
			// black
			2879, // black pure rare
			2880, // black pure common
			2881, // black normal rare
			2882, // black normal common
		
			// brown
			2883, 	// brown pure rare
			2884, 	// brown pure common
			2885, 	// brown normal rare
			0, 	// brown normal common
		
			// white
			2887,	// white pure rare
			2888,	// white pure common
			2889,	// white normal rare
			2890	// white normal common
		};

        public override int[] HueList { get { return m_HueList; } } // override base HueList

        public virtual int Colormap { get { return 0; } } // support for other colormaps

        public ColorDNA()
        {
        }

        public ColorDNA( BaseDNA father, BaseDNA mother )
            : base( father, mother )
        {
        }

        public ColorDNA( GenericReader reader, Mobile owner )
            : base( reader, owner )
        {
        }
    }
}