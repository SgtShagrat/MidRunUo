using System;

namespace Server.Gumps
{
    public static class Logos
    {
        public const int MidgardTextBlue = 0xa4;
        public const int MidgardTextBlueLittle = 0xa5;
        public const int MidgardTextBlack = 0xa6;
        public const int MidgardTextBlackLittle = 0xa7;
        public const int MidgardTextRed = 0xa8;
        public const int MidgardTextRedLittle = 0xa9;

        public const int IconThirdCrown = 0xaa;
        public const int IconThirdCrownLittle = 0xab;
        public const int IconMidgard = 0xac;
        public const int IconMidgardLittle = 0xad;
        public const int IconMScroll = 0xae;
        public const int IconMScrollStone = 0xaf;
        public const int IconM4D = 0xb0;

        public const int EmBig = 0xb0;
        public const int EmLittle = 0xb0;
        public const int MidgardLogo = 0xb0;
        public const int MidgardLogoLittle = 0xb0;
        public const int RunUOLogo = 0xb0;
        public const int RunUOLogoLittle = 0xb0;

        public enum LogosEnum
        {
            MidgardTextBlue = 0xa4,
            MidgardTextBlueLittle = 0xa5,
            MidgardTextBlack = 0xa6,
            MidgardTextBlackLittle = 0xa7,
            MidgardTextRed = 0xa8,
            MidgardTextRedLittle = 0xa9,
            IconThirdCrown = 0xaa,
            IconThirdCrownLittle = 0xab,
            IconMidgard = 0xac,
            IconMidgardLittle = 0xad,
            IconMScroll = 0xae,
            IconMScrollStone = 0xaf,
            IconM4D = 0xb0,
            EmBig = 0xb0,
            EmLittle = 0xb0,
            MidgardLogo = 0xb0,
            MidgardLogoLittle = 0xb0,
            RunUOLogo = 0xb0,
            RunUOLogoLittle = 0xb0,
        }

        private static int[] m_Logos = new int[]
        {
            0xa4, // scritta midgard blu
            0xa5, // scritta midgard blu piccola
            0xa6, // scritta midgard nero
            0xa7, // scritta midgard nero piccola
            0xa8, // scritta midgard rossa
            0xa9, // scritta midgard rossa piccola

            0xaa, // icona third crown
            0xab, // icona third crown piccola
            0xac, // icona midgard
            0xad, // icona midgard piccola
            0xae, // icona"M"con pergamena
            0xaf, // icona "M" con pergamena desaturizzata per gump in pietra
            0xB0, // icona m4d

            0xB1, // "M" grande
            0xB2, // "M" piccola
            0xb3, // logo midgard
            0xb4, // logo midgard piccolo
            0xb5, // logo runuo
            0xb6 // logo runuo piccolo
        };

        public static int GetLogoById( int hue )
        {
            if( hue < 0 || hue >= m_Logos.Length )
                return 0;

            return m_Logos[ hue ];
        }

        public static string GetLogoNameById( int hue )
        {
            if( hue < 0 || hue >= Enum.GetNames( typeof( LogosEnum ) ).Length )
                return "-null-";

            return Enum.GetName( typeof( LogosEnum ), hue );
        }
    }
}