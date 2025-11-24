using System;
using System.Collections;
using Midgard.Engines.WineCrafting;

namespace Midgard.Engines.BrewCrafing
{
    public enum BrewQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public enum BrewVariety
    {
        None = 0,

        /*
        SweetHops = 1,
        BitterHops,
        SnowHops,
        ElvenHops,
        */

        StoutAle = 1,
        PorterAle,
        MildAle,
        WinterAle,
        BlondAle,
        TrappisteDoubelAle,
        TrappisteTripelAle,
        Pilsener,
        AltBier,
        KarakIdril,

        CabernetSauvignon = 100,
        Chardonnay,
        CheninBlanc,
        Merlot,
        PinotNoir,
        Riesling,
        Sangiovese,
        SauvignonBlanc,
        Shiraz,
        Viognier,
        Zinfandel,

        Burbon = 200,
        RyeWhiskey,
        IrishWhiskey,
        Brandy,
        Vodka,
        Gin,
        Rum,
        Scotch,
        Tequila
    }

    public enum BrewVarietyType
    {
        None,

        Ales,
        Grapes,
        Whiskey
    }

    public class BrewVarietyInfo
    {
        private int m_Hue;
        private int m_Number;
        private string m_Name;
        private BrewVariety m_Variety;
        private Type[] m_VarietyTypes;

        public int Hue { get { return m_Hue; } }
        public int Number { get { return m_Number; } }
        public string Name { get { return m_Name; } }
        public BrewVariety Resource { get { return m_Variety; } }
        public Type[] VarietyTypes { get { return m_VarietyTypes; } }

        public BrewVarietyInfo( int hue, int number, string name, BrewVariety variety, params Type[] varietyTypes )
        {
            m_Hue = hue;
            m_Number = number;
            m_Name = name;
            m_Variety = variety;
            m_VarietyTypes = varietyTypes;

            for( int i = 0; i < varietyTypes.Length; ++i )
                BrewingResources.RegisterType( varietyTypes[ i ], variety );
        }
    }

    public class BrewingResources
    {
        /* m_HopsInfo
        private static BrewVarietyInfo[] m_HopsInfo = new BrewVarietyInfo[]
		{
			new BrewVarietyInfo( 0x000, 0,	"Bitter Hops",		BrewVariety.BitterHops,		typeof( BitterHops ) ),
			new BrewVarietyInfo( 0x481, 0,	"Snow Hops",		BrewVariety.SnowHops,		typeof( SnowHops ) ),
			new BrewVarietyInfo( 0x17, 0,	"Elven Hops",		BrewVariety.ElvenHops,		typeof( ElvenHops ) ),
			new BrewVarietyInfo( 0x30, 0,	"Sweet Hops",		BrewVariety.SweetHops,		typeof( SweetHops ) ),
		};
        */

        #region m_AlesInfo
        private static BrewVarietyInfo[] m_WhiskeyInfo = new BrewVarietyInfo[]
        {
            new BrewVarietyInfo( 0x0, 0,	"Burbon",	    BrewVariety.Burbon,		    typeof( BurbonKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Rye Whiskey",	BrewVariety.RyeWhiskey,		typeof( RyeWhiskeyKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Irish Whiskey",BrewVariety.IrishWhiskey,	typeof( IrishWhiskeyKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Brandy",	    BrewVariety.Brandy,		    typeof( BrandyKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Vodka",	    BrewVariety.Vodka,		    typeof( VodkaKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Gin",	        BrewVariety.Gin,		    typeof( GinKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Rum",	        BrewVariety.Rum,		    typeof( RumKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Scotch",	    BrewVariety.Scotch,		    typeof( ScotchKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Tequila",	    BrewVariety.Tequila,		typeof( TequilaKeg ) ),
        };
        #endregion

        #region m_AlesInfo

        private static BrewVarietyInfo[] m_AlesInfo = new BrewVarietyInfo[]
		{
            // andrebbe inserito un type come risorse... ma le birre non lo hanno
            new BrewVarietyInfo( 0x0, 0,	"Stout Ale",	        BrewVariety.StoutAle,		typeof( StoutAleKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Porter Ale",	        BrewVariety.PorterAle,		typeof( PorterAleKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Mild Ale",		        BrewVariety.MildAle,		typeof( MildAleKeg ) ),
            new BrewVarietyInfo( 0x0, 0,	"Winter Ale",	        BrewVariety.WinterAle,		typeof( WinterAleKeg ) ),
		    new BrewVarietyInfo( 0x0, 0,	"Blond Ale",	        BrewVariety.BlondAle,		typeof( BlondAleKeg ) ),
        	new BrewVarietyInfo( 0x0, 0,	"Trappiste Doubel Ale",	BrewVariety.TrappisteDoubelAle,		typeof( TrappisteDoubelAleKeg ) ),
	        new BrewVarietyInfo( 0x0, 0,	"Trappiste Tripel Ale",	BrewVariety.TrappisteTripelAle,		typeof( TrappisteTripelAleKeg ) ),
	        new BrewVarietyInfo( 0x0, 0,	"Pilsener",	            BrewVariety.Pilsener,		typeof( PilsenerKeg ) ),
	        new BrewVarietyInfo( 0x0, 0,	"AltBier",	            BrewVariety.AltBier,		typeof( AltBierKeg ) ),
	        new BrewVarietyInfo( 0x0, 0,	"Karak Idril",	        BrewVariety.KarakIdril,		typeof( KarakIdrilKeg ) )
            };

        #endregion

        /* m_BrewMixInfo
        private static BrewVarietyInfo[] m_BrewMixInfo = new BrewVarietyInfo[]
		{
			new BrewVarietyInfo( 0x0, 0,	"Yeast",		BrewVariety.Yeast,		typeof( Yeast ) ),
			new BrewVarietyInfo( 0x0, 0,	"Sugar",		BrewVariety.Sugar,		typeof( Sugar ) ),
			new BrewVarietyInfo( 0x0, 0,	"Barley",		BrewVariety.Barley,		typeof( Barley ) ),
			new BrewVarietyInfo( 0x0, 0,	"Malt",			BrewVariety.Malt,		typeof( Malt ) )

		};
        */

        #region m_GrapeInfo

        private static BrewVarietyInfo[] m_GrapeInfo = new BrewVarietyInfo[]
            {
                new BrewVarietyInfo( 0x000, 0,	"Tannico di Yew",	            BrewVariety.CabernetSauvignon,	typeof( CabernetSauvignonGrapes ) ),
                new BrewVarietyInfo( 0x1CC, 0,	"Ambra di Vesper",	            BrewVariety.Chardonnay,		    typeof( ChardonnayGrapes ) ),
                new BrewVarietyInfo( 0x16B, 0,	"L'Oleoso di Buccaner's",	    BrewVariety.CheninBlanc,		typeof( CheninBlancGrapes ) ),
                new BrewVarietyInfo( 0x2CE, 0,	"Tempestoso di Daekarthane",	BrewVariety.Merlot,			    typeof( MerlotGrapes ) ),
                new BrewVarietyInfo( 0x2CE, 0,	"Dolce di Magincia",	        BrewVariety.PinotNoir,			typeof( PinotNoirGrapes ) ),
                new BrewVarietyInfo( 0x1CC, 0,	"Luminoso di Trinsic",	        BrewVariety.Riesling,			typeof( RieslingGrapes ) ),
                new BrewVarietyInfo( 0x000, 0,	"Nebbiolo di Serpent's",	    BrewVariety.Sangiovese,		    typeof( SangioveseGrapes ) ),
                new BrewVarietyInfo( 0x16B, 0,	"Perlato di Moonglow",	        BrewVariety.SauvignonBlanc,	    typeof( SauvignonBlancGrapes ) ),
                new BrewVarietyInfo( 0x2CE, 0,	"Longevo di Nunjelm",		    BrewVariety.Shiraz,			    typeof( ShirazGrapes ) ),
                new BrewVarietyInfo( 0x16B, 0,	"Dorato di Cove",		        BrewVariety.Viognier,			typeof( ViognierGrapes ) ),
                new BrewVarietyInfo( 0x000, 0,	"Luminoso di Britain",		    BrewVariety.Zinfandel,			typeof( ZinfandelGrapes ) )

            };

        #endregion

        /// <summary>
        /// Returns true if '<paramref name="variety"/>' is None, BitterHops, CabernetSauvignonRedGrapes or AligoteWhiteGrapes. False if otherwise.
        /// </summary>
        public static bool IsStandard( BrewVariety variety )
        {
            return ( variety == BrewVariety.None /*|| variety == BrewVariety.BitterHops || variety == BrewVariety.Yeast */ || variety == BrewVariety.CabernetSauvignon );
        }

        private static Hashtable m_TypeTable;

        /// <summary>
        /// Registers that '<paramref name="resourceType"/>' uses '<paramref name="variety"/>' so that it can later be queried by <see cref="BrewingResources.GetFromType"/>
        /// </summary>
        public static void RegisterType( Type resourceType, BrewVariety variety )
        {
            if( m_TypeTable == null )
                m_TypeTable = new Hashtable();

            m_TypeTable[ resourceType ] = variety;
        }

        /// <summary>
        /// Returns the <see cref="BrewVariety"/> value for which '<paramref name="resourceType"/>' uses -or- BrewVariety.None if an unregistered type was specified.
        /// </summary>
        public static BrewVariety GetFromType( Type resourceType )
        {
            if( m_TypeTable == null )
                return BrewVariety.None;

            object obj = m_TypeTable[ resourceType ];

            if( !( obj is BrewVariety ) )
                return BrewVariety.None;

            return (BrewVariety)obj;
        }

        /// <summary>
        /// Returns a <see cref="BrewVarietyInfo"/> instance describing '<paramref name="variety"/>' -or- null if an invalid variety was specified.
        /// </summary>
        public static BrewVarietyInfo GetInfo( BrewVariety variety )
        {
            BrewVarietyInfo[] list = null;

            switch( GetType( variety ) )
            {
                
                case BrewVarietyType.Whiskey:
                    list = m_WhiskeyInfo;
                    break;
                case BrewVarietyType.Grapes:
                    list = m_GrapeInfo;
                    break;
                case BrewVarietyType.Ales:
                    list = m_AlesInfo;
                    break;
            }

            if( list != null )
            {
                int index = GetIndex( variety );

                if( index >= 0 && index < list.Length )
                    return list[ index ];
            }

            return null;
        }

        /// <summary>
        /// Returns a <see cref="BrewVarietyType"/> value indiciating the type of '<paramref name="variety"/>'.
        /// </summary>
        public static BrewVarietyType GetType( BrewVariety variety )
        {
            if( variety >= BrewVariety.Burbon && variety <= BrewVariety.Tequila )
                return BrewVarietyType.Whiskey;

            if( variety >= BrewVariety.CabernetSauvignon && variety <= BrewVariety.Zinfandel )
                return BrewVarietyType.Grapes;

            if( variety >= BrewVariety.StoutAle && variety <= BrewVariety.KarakIdril )
                return BrewVarietyType.Ales;

            return BrewVarietyType.None;
        }

        /// <summary>
        /// Returns the first <see cref="BrewVariety"/> in the series of varietys for which '<paramref name="variety"/>' belongs.
        /// </summary>
        public static BrewVariety GetStart( BrewVariety variety )
        {
            switch( GetType( variety ) )
            {
                case BrewVarietyType.Grapes:
                    return BrewVariety.CabernetSauvignon;

                case BrewVarietyType.Whiskey:
                    return BrewVariety.Burbon;

                case BrewVarietyType.Ales:
                    return BrewVariety.StoutAle;
            }

            return BrewVariety.None;
        }

        /// <summary>
        /// Returns the index of '<paramref name="variety"/>' in the seriest of varietys for which it belongs.
        /// </summary>
        public static int GetIndex( BrewVariety variety )
        {
            BrewVariety start = GetStart( variety );

            if( start == BrewVariety.None )
                return 0;

            return variety - start;
        }

        /// <summary>
        /// Returns the <see cref="BrewVarietyInfo.Number"/> property of '<paramref name="variety"/>' -or- 0 if an invalid variety was specified.
        /// </summary>
        public static int GetLocalizationNumber( BrewVariety variety )
        {
            BrewVarietyInfo info = GetInfo( variety );

            return ( info == null ? 0 : info.Number );
        }

        /// <summary>
        /// Returns the <see cref="BrewVarietyInfo.Hue"/> property of '<paramref name="variety"/>' -or- 0 if an invalid variety was specified.
        /// </summary>
        public static int GetHue( BrewVariety variety )
        {
            BrewVarietyInfo info = GetInfo( variety );

            return ( info == null ? 0 : info.Hue );
        }

        /// <summary>
        /// Returns the <see cref="BrewVarietyInfo.Name"/> property of '<paramref name="variety"/>' -or- an empty string if the variety specified was invalid.
        /// </summary>
        public static string GetName( BrewVariety variety )
        {
            BrewVarietyInfo info = GetInfo( variety );

            return ( info == null ? String.Empty : info.Name );
        }
    }
}