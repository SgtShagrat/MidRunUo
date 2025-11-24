using System;

namespace Midgard.Engines.SpellSystem
{
    public class SkinList
    {
        public static readonly BaseSkin[] SkinsList = new BaseSkin[]
            {
                new LeftDock(),
                new TopDock(),
                new RightDock(),
                new BottomDock()
            };

        public static int HighestSkin = 3;

        public static Type[] Skins = new Type[]
        {
			typeof( LeftDock ),
			typeof( TopDock ),
			typeof( RightDock ),
			typeof( BottomDock ),
        };

        public static BaseSkin GetSkin( int id )
        {
            return id < SkinsList.Length ? SkinsList[ id ] : SkinsList[ 0 ];
        }

        public static Type[] OptionSkins = new Type[]
        {
            typeof( LeftOptionSkin ),
            typeof( TopOptionSkin ),
            typeof( RightOptionSkin ),
        };

        public static readonly BaseOptionSkin[] OptionSkinsList = new BaseOptionSkin[]
        {
            new LeftOptionSkin(),
            new TopOptionSkin(),
            new RightOptionSkin()
        };

        public static BaseOptionSkin GetOptionSkin( int id )
        {
            return id < OptionSkinsList.Length ? OptionSkinsList[ id ] : OptionSkinsList[ 0 ];
        }
    }
}