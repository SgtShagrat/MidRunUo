using System;

namespace Server.Gumps
{
    public static class Colors
    {
        #region funny html colors
        public const int AliceBlue = 0xF0F8FF;
        public const int AntiqueWhite = 0xFAEBD7;
        public const int Aqua = 0x00FFFF;
        public const int Aquamarine = 0x7FFFD4;
        public const int Azure = 0xF0FFFF;
        public const int Beige = 0xF5F5DC;
        public const int Bisque = 0xFFE4C4;
        public const int Black = 0x000000;
        public const int BlanchedAlmond = 0xFFEBCD;
        public const int Blue = 0x0000FF;
        public const int BlueViolet = 0x8A2BE2;
        public const int Brown = 0xA52A2A;
        public const int BurlyWood = 0xDEB887;
        public const int CadetBlue = 0x5F9EA0;
        public const int Chartreuse = 0x7FFF00;
        public const int Chocolate = 0xD2691E;
        public const int Coral = 0xFF7F50;
        public const int CornflowerBlue = 0x6495ED;
        public const int Cornsilk = 0xFFF8DC;
        public const int Crimson = 0xDC143C;
        public const int Cyan = 0x00FFFF;
        public const int DarkBlue = 0x00008B;
        public const int DarkCyan = 0x008B8B;
        public const int DarkGoldenRod = 0xB8860B;
        public const int DarkGray = 0xA9A9A9;
        public const int DarkGreen = 0x006400;
        public const int DarkKhaki = 0xBDB76B;
        public const int DarkMagenta = 0x8B008B;
        public const int DarkOliveGreen = 0x556B2F;
        public const int Darkorange = 0xFF8C00;
        public const int DarkOrchid = 0x9932CC;
        public const int DarkRed = 0x8B0000;
        public const int DarkSalmon = 0xE9967A;
        public const int DarkSeaGreen = 0x8FBC8F;
        public const int DarkSlateBlue = 0x483D8B;
        public const int DarkSlateGray = 0x2F4F4F;
        public const int DarkTurquoise = 0x00CED1;
        public const int DarkViolet = 0x9400D3;
        public const int DeepPink = 0xFF1493;
        public const int DeepSkyBlue = 0x00BFFF;
        public const int DimGray = 0x696969;
        public const int DodgerBlue = 0x1E90FF;
        public const int FireBrick = 0xB22222;
        public const int FloralWhite = 0xFFFAF0;
        public const int ForestGreen = 0x228B22;
        public const int Fuchsia = 0xFF00FF;
        public const int Gainsboro = 0xDCDCDC;
        public const int GhostWhite = 0xF8F8FF;
        public const int Gold = 0xFFD700;
        public const int GoldenRod = 0xDAA520;
        public const int Gray = 0x808080;
        public const int Green = 0x008000;
        public const int GreenYellow = 0xADFF2F;
        public const int HoneyDew = 0xF0FFF0;
        public const int HotPink = 0xFF69B4;
        public const int IndianRed = 0xCD5C5C;
        public const int Indigo = 0x4B0082;
        public const int Ivory = 0xFFFFF0;
        public const int Khaki = 0xF0E68C;
        public const int Lavender = 0xE6E6FA;
        public const int LavenderBlush = 0xFFF0F5;
        public const int LawnGreen = 0x7CFC00;
        public const int LemonChiffon = 0xFFFACD;
        public const int LightBlue = 0xADD8E6;
        public const int LightCoral = 0xF08080;
        public const int LightCyan = 0xE0FFFF;
        public const int LightGoldenRodYellow = 0xFAFAD2;
        public const int LightGrey = 0xD3D3D3;
        public const int LightGreen = 0x90EE90;
        public const int LightPink = 0xFFB6C1;
        public const int LightSalmon = 0xFFA07A;
        public const int LightSeaGreen = 0x20B2AA;
        public const int LightSkyBlue = 0x87CEFA;
        public const int LightSlateGray = 0x778899;
        public const int LightSteelBlue = 0xB0C4DE;
        public const int LightYellow = 0xFFFFE0;
        public const int Lime = 0x00FF00;
        public const int LimeGreen = 0x32CD32;
        public const int Linen = 0xFAF0E6;
        public const int Magenta = 0xFF00FF;
        public const int Maroon = 0x800000;
        public const int MediumAquaMarine = 0x66CDAA;
        public const int MediumBlue = 0x0000CD;
        public const int MediumOrchid = 0xBA55D3;
        public const int MediumPurple = 0x9370D8;
        public const int MediumSeaGreen = 0x3CB371;
        public const int MediumSlateBlue = 0x7B68EE;
        public const int MediumSpringGreen = 0x00FA9A;
        public const int MediumTurquoise = 0x48D1CC;
        public const int MediumVioletRed = 0xC71585;
        public const int MidnightBlue = 0x191970;
        public const int MintCream = 0xF5FFFA;
        public const int MistyRose = 0xFFE4E1;
        public const int Moccasin = 0xFFE4B5;
        public const int NavajoWhite = 0xFFDEAD;
        public const int Navy = 0x000080;
        public const int OldLace = 0xFDF5E6;
        public const int Olive = 0x808000;
        public const int OliveDrab = 0x6B8E23;
        public const int Orange = 0xFFA500;
        public const int OrangeRed = 0xFF4500;
        public const int Orchid = 0xDA70D6;
        public const int PaleGoldenRod = 0xEEE8AA;
        public const int PaleGreen = 0x98FB98;
        public const int PaleTurquoise = 0xAFEEEE;
        public const int PaleVioletRed = 0xD87093;
        public const int PapayaWhip = 0xFFEFD5;
        public const int PeachPuff = 0xFFDAB9;
        public const int Peru = 0xCD853F;
        public const int Pink = 0xFFC0CB;
        public const int Plum = 0xDDA0DD;
        public const int PowderBlue = 0xB0E0E6;
        public const int Purple = 0x800080;
        public const int Red = 0xFF0000;
        public const int RosyBrown = 0xBC8F8F;
        public const int RoyalBlue = 0x4169E1;
        public const int SaddleBrown = 0x8B4513;
        public const int Salmon = 0xFA8072;
        public const int SandyBrown = 0xF4A460;
        public const int SeaGreen = 0x2E8B57;
        public const int SeaShell = 0xFFF5EE;
        public const int Sienna = 0xA0522D;
        public const int Silver = 0xC0C0C0;
        public const int SkyBlue = 0x87CEEB;
        public const int SlateBlue = 0x6A5ACD;
        public const int SlateGray = 0x708090;
        public const int Snow = 0xFFFAFA;
        public const int SpringGreen = 0x00FF7F;
        public const int SteelBlue = 0x4682B4;
        public const int Tan = 0xD2B48C;
        public const int Teal = 0x008080;
        public const int Thistle = 0xD8BFD8;
        public const int Tomato = 0xFF6347;
        public const int Turquoise = 0x40E0D0;
        public const int Violet = 0xEE82EE;
        public const int Wheat = 0xF5DEB3;
        public const int White = 0xFFFFFF;
        public const int WhiteSmoke = 0xF5F5F5;
        public const int Yellow = 0xFFFF00;
        public const int YellowGreen = 0x9ACD32;

        public static int[] ColorList
        {
            get { return m_Hues; }
        }

        #endregion

        public static int GetHue( int hue )
        {
            if( hue < 0 || hue >= ColorList.Length )
                return 0;

            return ColorList[ hue ];
        }

        public static string GetName( int hue )
        {
            if( hue < 0 || hue >= Enum.GetNames( typeof( ColorsEnum ) ).Length )
                return "-null-";

            return Enum.GetName( typeof( ColorsEnum ), hue );
        }

        #region m_Hues
        private static int[] m_Hues = new int[]
                                          {
                                              AliceBlue,
                                              AntiqueWhite,
                                              Aqua,
                                              Aquamarine,
                                              Azure,
                                              Beige,
                                              Bisque,
                                              Black,
                                              BlanchedAlmond,
                                              Blue,
                                              BlueViolet,
                                              Brown,
                                              BurlyWood,
                                              CadetBlue,
                                              Chartreuse,
                                              Chocolate,
                                              Coral,
                                              CornflowerBlue,
                                              Cornsilk,
                                              Crimson,
                                              Cyan,
                                              DarkBlue,
                                              DarkCyan,
                                              DarkGoldenRod,
                                              DarkGray,
                                              DarkGreen,
                                              DarkKhaki,
                                              DarkMagenta,
                                              DarkOliveGreen,
                                              Darkorange,
                                              DarkOrchid,
                                              DarkRed,
                                              DarkSalmon,
                                              DarkSeaGreen,
                                              DarkSlateBlue,
                                              DarkSlateGray,
                                              DarkTurquoise,
                                              DarkViolet,
                                              DeepPink,
                                              DeepSkyBlue,
                                              DimGray,
                                              DodgerBlue,
                                              FireBrick,
                                              FloralWhite,
                                              ForestGreen,
                                              Fuchsia,
                                              Gainsboro,
                                              GhostWhite,
                                              Gold,
                                              GoldenRod,
                                              Gray,
                                              Green,
                                              GreenYellow,
                                              HoneyDew,
                                              HotPink,
                                              IndianRed,
                                              Indigo,
                                              Ivory,
                                              Khaki,
                                              Lavender,
                                              LavenderBlush,
                                              LawnGreen,
                                              LemonChiffon,
                                              LightBlue,
                                              LightCoral,
                                              LightCyan,
                                              LightGoldenRodYellow,
                                              LightGrey,
                                              LightGreen,
                                              LightPink,
                                              LightSalmon,
                                              LightSeaGreen,
                                              LightSkyBlue,
                                              LightSlateGray,
                                              LightSteelBlue,
                                              LightYellow,
                                              Lime,
                                              LimeGreen,
                                              Linen,
                                              Magenta,
                                              Maroon,
                                              MediumAquaMarine,
                                              MediumBlue,
                                              MediumOrchid,
                                              MediumPurple,
                                              MediumSeaGreen,
                                              MediumSlateBlue,
                                              MediumSpringGreen,
                                              MediumTurquoise,
                                              MediumVioletRed,
                                              MidnightBlue,
                                              MintCream,
                                              MistyRose,
                                              Moccasin,
                                              NavajoWhite,
                                              Navy,
                                              OldLace,
                                              Olive,
                                              OliveDrab,
                                              Orange,
                                              OrangeRed,
                                              Orchid,
                                              PaleGoldenRod,
                                              PaleGreen,
                                              PaleTurquoise,
                                              PaleVioletRed,
                                              PapayaWhip,
                                              PeachPuff,
                                              Peru,
                                              Pink,
                                              Plum,
                                              PowderBlue,
                                              Purple,
                                              Red,
                                              RosyBrown,
                                              RoyalBlue,
                                              SaddleBrown,
                                              Salmon,
                                              SandyBrown,
                                              SeaGreen,
                                              SeaShell,
                                              Sienna,
                                              Silver,
                                              SkyBlue,
                                              SlateBlue,
                                              SlateGray,
                                              Snow,
                                              SpringGreen,
                                              SteelBlue,
                                              Tan,
                                              Teal,
                                              Thistle,
                                              Tomato,
                                              Turquoise,
                                              Violet,
                                              Wheat,
                                              White,
                                              WhiteSmoke,
                                              Yellow,
                                              YellowGreen
                                          };

        #endregion

        #region ColorsEnum
        public enum ColorsEnum
        {
            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenRod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            Darkorange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            FireBrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            GoldenRod,
            Gray,
            Green,
            GreenYellow,
            HoneyDew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenRodYellow,
            LightGrey,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquaMarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenRod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen
        };

        #endregion
    }
}