using System;

namespace Server
{
    public class Quality
    {
        public enum ItemQuality
        {
            Undefined,
            VeryLow,
            Low,
            Decent,
            BelowNormal,
            Standard,
            Superior,
            Great,
            Exceptional
        }

        public const double Undefined = -1.0;

        public const double VeryLow = 0.7;
        public const double Low = 0.8;
        public const double Decent = 0.9;
        public const double BelowNormal = 1.0;
        public const double Standard = 1.1;
        public const double Superior = 1.2;
        public const double Great = 1.25;
        public const double Exceptional = 1.30;

        public static string GetQualityName( Item item )
        {
            if( item.CustomQuality == Undefined )
                return String.Empty;
            else if( item.CustomQuality <= VeryLow )
                return "very low";
            else if( item.CustomQuality <= Low )
                return "low";
            else if( item.CustomQuality <= Decent )
                return "moderate";
            else if( item.CustomQuality <= BelowNormal )
                return "decent";
            else if( item.CustomQuality <= Standard )
                return "standard";
            else if( item.CustomQuality <= Superior )
                return "superior";
            else if( item.CustomQuality <= Great )
                return "great";
            else
                return "exceptional";
        }
    }
}