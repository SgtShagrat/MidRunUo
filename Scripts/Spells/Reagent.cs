using System;
using System.Collections.Generic;
using System.Text;

using Midgard.Items;

using Server.Items;

namespace Server.Spells
{
    public class Reagent
    {
        #region modifica by Dies Irae
        private static Type[] m_Types = new Type[ 18 ]
			{
				typeof( BlackPearl ),
				typeof( Bloodmoss ),
				typeof( Garlic ),
				typeof( Ginseng ),
				typeof( MandrakeRoot ),

				typeof( Nightshade ),
				typeof( SulfurousAsh ),
				typeof( SpidersSilk ),
				typeof( BatWing ),
				typeof( GraveDust ),

				typeof( DaemonBlood ),
				typeof( NoxCrystal ),
				typeof( PigIron ),
				typeof( SpringWater ),
				typeof( DestroyingAngel ),

				typeof( PetrifiedWood ),
				typeof( Kindling ),
				typeof( FertileDirt )
			};

        private static readonly string[] m_Abbreviations = new string[]
			{
				"BP",
				"BM",
				"GA",
				"GI",
				"MR",

				"NS",
				"SA",
				"SS",
				"BW",
				"GD",

				"DB",
				"NC",
				"PI",
				"SW",
				"DA",

				"PW",
				"KI",
				"FD"
			};

        public static string[] RegAbbreviations
        {
            get { return m_Abbreviations; }
        }

        public static string FindAbvByType( Type t )
        {
            for( int i = 0; i < m_Types.Length; i++ )
            {
                if( t == m_Types[ i ] )
                    return m_Abbreviations[ i ];
            }

            return null;
        }

        public static string PackRegs( Type[] regs )
        {
            if( regs == null || regs.Length == 0 )
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach( Type t in regs )
            {
                string s = FindAbvByType( t );
                if( s == null )
                    continue;

                sb.AppendFormat( "{0},", s );
            }

            if( Core.Debug )
                Utility.Log( "Reagents.log", sb.ToString( 0, sb.Length - 1 ) );

            return sb.ToString( 0, sb.Length - 1 );
        }

        /*
        BP = Black Pearn
        BM = Bloodmoss
        GA = Garlic
        GI = Ginseng
        MR = Mandrake Root
        NS = Nightshade
        SA = Sulfurous Ash
        SS = Spider's Silk
        BW = Bat Wing
        GD = Grave Dust
        DB = Daemon Blood
        NC = Nox Crystal
        PI = Pig Iron
        PW = Petrafied Wood
        SW = Spring Water
        DA = Destroying Angel
        T.# = Tithe
         ^ is a period
        */

        public static List<string> ConvertRegs( string regs )
        {
            string[] regAbrv = regs.Split( ',' );
            List<string> returnRegs = new List<string>();

            for( int i = 0; i < regAbrv.Length && i < 5; i++ )
            {
                string reg = regAbrv[ i ];
                if( reg.StartsWith( "T." ) )
                {
                    string[] titheReq = reg.Split( '.' );
                    returnRegs.Add( "Tithe: " + titheReq[ 1 ] );
                }

                /*********************\ATTENTION/*********************/
                /*      If you added new Abreviations above, you     */
                /*   will need to add the conversions here as well!  */
                /********************\ATTENTION/**********************/

                switch( reg )
                {
                    case ( "BP" ): { returnRegs.Add( "Black Pearl" ); break; }
                    case ( "BM" ): { returnRegs.Add( "Bloodmoss" ); break; }
                    case ( "GA" ): { returnRegs.Add( "Garlic" ); break; }
                    case ( "GI" ): { returnRegs.Add( "Ginseng" ); break; }
                    case ( "MR" ): { returnRegs.Add( "Mandr. Root" ); break; }

                    case ( "NS" ): { returnRegs.Add( "Nightshade" ); break; }
                    case ( "SA" ): { returnRegs.Add( "Sulf. Ash" ); break; }
                    case ( "SS" ): { returnRegs.Add( "Sp.'s Silk" ); break; }
                    case ( "BW" ): { returnRegs.Add( "Bat Wing" ); break; }
                    case ( "GD" ): { returnRegs.Add( "Grave Dust" ); break; }

                    case ( "DB" ): { returnRegs.Add( "Daemon Blood" ); break; }
                    case ( "NC" ): { returnRegs.Add( "Nox Crystal" ); break; }
                    case ( "PI" ): { returnRegs.Add( "Pig Iron" ); break; }
                    case ( "PW" ): { returnRegs.Add( "Petr. Wood" ); break; }
                    case ( "SW" ): { returnRegs.Add( "Spring Water" ); break; }

                    case ( "DA" ): { returnRegs.Add( "Destr. Angel" ); break; }
                    case ( "KI" ): { returnRegs.Add( "Kindling" ); break; }
                    case ( "FD" ): { returnRegs.Add( "Fert. Dirt" ); break; }

                    default: { returnRegs.Add( "" ); break; }
                }
            }

            return returnRegs;
        }
        #endregion

		public Type[] Types
		{
			get{ return m_Types; }
		}

		public static Type BlackPearl
		{
			get{ return m_Types[0]; }
			set{ m_Types[0] = value; }
		}

		public static Type Bloodmoss
		{
			get{ return m_Types[1]; }
			set{ m_Types[1] = value; }
		}

		public static Type Garlic
		{
			get{ return m_Types[2]; }
			set{ m_Types[2] = value; }
		}

		public static Type Ginseng
		{
			get{ return m_Types[3]; }
			set{ m_Types[3] = value; }
		}

		public static Type MandrakeRoot
		{
			get{ return m_Types[4]; }
			set{ m_Types[4] = value; }
		}

		public static Type Nightshade
		{
			get{ return m_Types[5]; }
			set{ m_Types[5] = value; }
		}

		public static Type SulfurousAsh
		{
			get{ return m_Types[6]; }
			set{ m_Types[6] = value; }
		}

		public static Type SpidersSilk
		{
			get{ return m_Types[7]; }
			set{ m_Types[7] = value; }
		}

		public static Type BatWing
		{
			get{ return m_Types[8]; }
			set{ m_Types[8] = value; }
		}

		public static Type GraveDust
		{
			get{ return m_Types[9]; }
			set{ m_Types[9] = value; }
		}

		public static Type DaemonBlood
		{
			get{ return m_Types[10]; }
			set{ m_Types[10] = value; }
		}

		public static Type NoxCrystal
		{
			get{ return m_Types[11]; }
			set{ m_Types[11] = value; }
		}

		public static Type PigIron
		{
			get{ return m_Types[12]; }
			set{ m_Types[12] = value; }
		}

        #region modifica by dies irae per i reagenti pagani
        public static Type SpringWater
        {
            get { return m_Types[ 13 ]; }
            set { m_Types[ 13 ] = value; }
        }

        public static Type DestroyingAngel
        {
            get { return m_Types[ 14 ]; }
            set { m_Types[ 14 ] = value; }
        }

        public static Type PetrifiedWood
        {
            get { return m_Types[ 15 ]; }
            set { m_Types[ 15 ] = value; }
        }

        public static Type Kindling
        {
            get { return m_Types[ 16 ]; }
            set { m_Types[ 16 ] = value; }
        }

        public static Type FertileDirt
        {
            get { return m_Types[ 17 ]; }
            set { m_Types[ 17 ] = value; }
        }
        #endregion
    }
}