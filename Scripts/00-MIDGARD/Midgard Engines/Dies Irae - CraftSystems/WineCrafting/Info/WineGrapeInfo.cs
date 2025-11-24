namespace Midgard.Engines.WineCrafting
{
    public class WineGrapeInfo
    {
        //new WineGrapeInfo (index, hue, name)
        public static readonly WineGrapeInfo CabernetSauvignon = new WineGrapeInfo( 0, 0x000, "Cabernet Sauvignon" );
        public static readonly WineGrapeInfo Chardonnay = new WineGrapeInfo( 1, 0x1CC, "Chardonnay" );
        public static readonly WineGrapeInfo CheninBlanc = new WineGrapeInfo( 2, 0x16B, "Chenin Blanc" );
        public static readonly WineGrapeInfo Merlot = new WineGrapeInfo( 3, 0x2CE, "Merlot" );
        public static readonly WineGrapeInfo PinotNoir = new WineGrapeInfo( 4, 0x2CE, "Pinot Noir" );
        public static readonly WineGrapeInfo Riesling = new WineGrapeInfo( 5, 0x1CC, "Riesling" );
        public static readonly WineGrapeInfo Sangiovese = new WineGrapeInfo( 6, 0x000, "Sangiovese" );
        public static readonly WineGrapeInfo SauvignonBlanc = new WineGrapeInfo( 7, 0x16B, "Sauvignon Blanc" );
        public static readonly WineGrapeInfo Shiraz = new WineGrapeInfo( 8, 0x2CE, "Shiraz" );
        public static readonly WineGrapeInfo Viognier = new WineGrapeInfo( 9, 0x16B, "Viognier" );
        public static readonly WineGrapeInfo Zinfandel = new WineGrapeInfo( 10, 0x000, "Zinfandel" );

        private int m_Level;
        private int m_Hue;
        private string m_Name;

        public WineGrapeInfo( int level, int hue, string name )
        {
            m_Level = level;
            m_Hue = hue;
            m_Name = name;
        }

        public int Level
        {
            get
            {
                return m_Level;
            }
        }

        public int Hue
        {
            get
            {
                return m_Hue;
            }
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }
    }
}