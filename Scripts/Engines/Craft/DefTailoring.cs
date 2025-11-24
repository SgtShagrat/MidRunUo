using System;
using System.Collections.Generic;
using Server.Items;

using Midgard.Engines.OldCraftSystem;
using Midgard.Items;

namespace Server.Engines.Craft
{
    public enum TailorRecipe
    {
        ElvenQuiver = 501,
        QuiverOfFire = 502,
        QuiverOfIce = 503,
        QuiverOfBlight = 504,
        QuiverOfLightning = 505,
        VestOfTheDragon = 506,
        VestOfTheMage = 507,

        SongWovenMantle = 550,
        SpellWovenBritches = 551,
        StitchersMittens = 552,
    }

    public class DefTailoring : CraftSystem
    {
        #region mod by Dies Irae
        public override string Name { get{ return "Tailoring"; } }

        public static void Initialize()
        {
            InitDebugItemsList();
        }

        public override bool SupportOldMenu
        {
            get { return true; }
        }

        public override bool SupportsQuality
        {
            get { return true; }
        }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Tailoring.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;
        #endregion

        public override SkillName MainSkill
        {
            get { return SkillName.Tailoring; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044005; } // <CENTER>TAILORING MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefTailoring();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA { get { return CraftECA.ZeroToFourPercentPlusBonus; } } // .ChanceMinusSixtyToFourtyFive; } } // mod by Dies Irae

        public override double GetChanceAtMin( CraftItem item )
        {
            return Core.AOS ? 0.5 : 0.0; // mod by Dies Irae
        }

        private DefTailoring()
            : /*base( 2, 4, 1.25 ) // mod by Dies Irae */ base( 2, 4, 1.25 )
        {
        }

        public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
        {
            if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
                return 1044038; // You have worn out your tool!
            else if( !BaseTool.CheckAccessible( tool, from ) )
                return 1044263; // The tool must be on your person to use.

            #region modifica By Dies Irae
            if( from.AccessLevel == AccessLevel.Player && m_DebugItemList.Contains( itemType ) )
                return 1064915; // This item is only available to Midgard Staff members. They will decide soon if has to be implemented or removed.
            #endregion

            return 0;
        }

		private static Type[] m_TailorColorables = new Type[]
			{
				typeof( GozaMatEastDeed ), typeof( GozaMatSouthDeed ),
				typeof( SquareGozaMatEastDeed ), typeof( SquareGozaMatSouthDeed ),
				typeof( BrocadeGozaMatEastDeed ), typeof( BrocadeGozaMatSouthDeed ),
				typeof( BrocadeSquareGozaMatEastDeed ), typeof( BrocadeSquareGozaMatSouthDeed )
			};

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
			if ( type != typeof( Cloth ) && type != typeof( UncutCloth ) )
				return false;

			type = item.ItemType;

			bool contains = false;

			for ( int i = 0; !contains && i < m_TailorColorables.Length; ++i )
				contains = ( m_TailorColorables[i] == type );

		    #region mod by Dies Irae
		    for( int i = 0; !contains && i < m_AdvTailoringColorables.Length; ++i )
		        contains = ( m_AdvTailoringColorables[ i ] == type );
		    #endregion

			return contains;
		}

        public override void PlayCraftEffect( Mobile from )
        {
            from.PlaySound( 0x248 );
        }

        public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
        {
            if( toolBroken )
                from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

            if( failed )
            {
                if( lostMaterial )
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if( quality == 0 )
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if( makersMark && quality == 2 )
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if( quality == 2 )
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        #region modifica by Dies Irae
        private static Type[] m_AdvTailoringColorables = new Type[]
         {
             typeof(LargeBanner1WestAddonDeed),
             typeof(LargeBanner1SouthAddonDeed),
             typeof(LargeBanner2WestAddonDeed),
             typeof(LargeBanner2SouthAddonDeed),
             typeof(LargeBanner3WestAddonDeed),
             typeof(LargeBanner3SouthAddonDeed),
             typeof(LargeBanner4WestAddonDeed),
             typeof(LargeBanner4SouthAddonDeed),
             typeof(SmallBanner1WestAddonDeed),
             typeof(SmallBanner1SouthAddonDeed),
             typeof(SmallBanner2WestAddonDeed),
             typeof(SmallBanner2SouthAddonDeed),
             typeof(PetLeash),
         };

        private static List<Type> m_DebugItemList;

        public static void InitDebugItemsList()
        {
            if( m_DebugItemList == null )
                m_DebugItemList = new List<Type>();
        }

        public static void CheckCarpetLoom( Mobile from, int range, out bool carpetloom )
        {
            carpetloom = false;

            Map map = from.Map;

            if( map == null )
                return;

            IPooledEnumerable eable = map.GetItemsInRange( from.Location, range );

            foreach( Item item in eable )
            {
                Type t = item.GetType();

                if( t == typeof( CarpetLoomEastAddon ) || t == typeof( CarpetLoomSouthAddon ) )
                {
                    carpetloom = true;
                }

                if( carpetloom )
                    break;
            }

            eable.Free();
        }

        public override void SetCustomQuality( Mobile from, CraftItem craftItem, Item item, bool exceptional, Type typeRes, double bonus )
        {
            Type resourceType = typeRes;

            if( resourceType == null )
                resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

            double modBonus = 0.0;
            CraftResource res = CraftResources.GetFromType( resourceType );
            CraftResourceInfo resInfo = CraftResources.GetInfo( res );
            if( resInfo != null )
            {
                CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
                if( attrInfo != null )
                    modBonus = -( attrInfo.OldStaticMultiply - 1.0 );
            }

            base.SetCustomQuality( from, craftItem, item, exceptional, typeRes, modBonus );
        }

        public override SkillName BonusSkill { get { return SkillName.ItemID; } }
        #endregion

        public void InitOldCraftList()
        {
            int index = -1;

            #region Hats
            AddCraft( typeof( SkullCap ), 1011375, 1025444, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
            AddCraft( typeof( Bandana ), 1011375, 1025440, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
            AddCraft( typeof( FloppyHat ), 1011375, 1025907, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
            AddCraft( typeof( Cap ), 1011375, 1025909, -18.8, 6.2, typeof( Cloth ), 1044286, 11, 1044287 );
            AddCraft( typeof( WideBrimHat ), 1011375, 1025908, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( StrawHat ), 1011375, 1025911, 6.2, 31.2, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( TallStrawHat ), 1011375, 1025910, 6.7, 31.7, typeof( Cloth ), 1044286, 13, 1044287 );
            AddCraft( typeof( WizardsHat ), 1011375, 1025912, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );
            AddCraft( typeof( Bonnet ), 1011375, 1025913, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
            AddCraft( typeof( FeatheredHat ), 1011375, 1025914, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( TricorneHat ), 1011375, 1025915, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( JesterHat ), 1011375, 1025916, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );

		AddCraft( typeof( FlowerGarland ), 1011375, 1028965, 10.0, 35.0, typeof( Cloth ), 1044286, 5, 1044287 );

            AddCraft( typeof( DeerMask ), 1011375, "deer mask", 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );
            AddCraft( typeof( BearMask ), 1011375, "bear mask", 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );

            index = AddCraft( typeof( BandanaWithPearls ), 1011375, "bandana with pearls", 80.0, 105.0, typeof( Cloth ), 1044286, 12, 1044287 );
            AddRes( index, typeof( WhitePearl ), 1032694, 2, 1042081 );

            AddCraft( typeof( PiratesHat ), 1011375, "pirate's hat", 60.0, 85.0, typeof( Cloth ), 1044286, 9, 1044287 );
            #endregion

            #region Shirts
            AddCraft( typeof( Doublet ), 1015269, 1028059, 0, 25.0, typeof( Cloth ), 1044286, 8, 1044287 );
            AddCraft( typeof( Shirt ), 1015269, 1025399, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
            AddCraft( typeof( FancyShirt ), 1015269, 1027933, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
            AddCraft( typeof( Tunic ), 1015269, 1028097, 00.0, 25.0, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( Surcoat ), 1015269, 1028189, 8.2, 33.2, typeof( Cloth ), 1044286, 14, 1044287 );
            AddCraft( typeof( PlainDress ), 1015269, 1027937, 12.4, 37.4, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( FancyDress ), 1015269, 1027935, 33.1, 58.1, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( Cloak ), 1015269, 1025397, 41.4, 66.4, typeof( Cloth ), 1044286, 14, 1044287 );
            AddCraft( typeof( Robe ), 1015269, 1027939, 53.9, 78.9, typeof( Cloth ), 1044286, 16, 1044287 );
            AddCraft( typeof( JesterSuit ), 1015269, 1028095, 8.2, 33.2, typeof( Cloth ), 1044286, 24, 1044287 );
            AddCraft( typeof( DecoratedFancyDress ), 1015269, 1064985, 65.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
            AddCraft( typeof( ServantShirt ), 1015269, "servant shirt", 65.0, 80.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( Gilet ), 1015269, "gilet", 65.0, 80.0, typeof( Cloth ), 1044286, 15, 1044287 );
            AddCraft( typeof( AdventurerTunic ), 1015269, "adventurer tunic", 65.0, 80.0, typeof( Cloth ), 1044286, 20, 1044287 );
            AddCraft( typeof( EveningGown ), 1015269, "evening gown", 65.0, 80.0, typeof( Cloth ), 1044286, 30, 1044287 );
            AddCraft( typeof( LaceDress ), 1015269, "lace dress", 65.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
            AddCraft( typeof( SexyDress ), 1015269, "sexy dress", 65.0, 80.0, typeof( Cloth ), 1044286, 15, 1044287 );
            AddCraft( typeof( Blouse ), 1015269, "blouse", 65.0, 80.0, typeof( Cloth ), 1044286, 14, 1044287 );
            AddCraft( typeof( NobleShirt ), 1015269, "noble shirt", 65.0, 80.0, typeof( Cloth ), 1044286, 15, 1044287 );
            AddCraft( typeof( PiratesJacket ), 1015269, "pirate's jacket", 65.0, 80.0, typeof( Cloth ), 1044286, 20, 1044287 );
            AddCraft( typeof( FurCape ), 1015269, 1028969, 35.0, 60.0, typeof( Cloth ), 1044286, 13, 1044287 );

            AddCraft( typeof( ElvenShirt ), 1015269, 1032661, 80.0, 105.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( ElvenDarkShirt ), 1015269, 1032662, 80.0, 105.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( MaleElvenRobe ), 1015269, 1032659, 80.0, 105.0, typeof( Cloth ), 1044286, 30, 1044287 );
            AddCraft( typeof( FemaleElvenRobe ), 1015269, 1032660, 80.0, 105.0, typeof( Cloth ), 1044286, 30, 1044287 );
            #endregion

            #region Pants
            AddCraft( typeof( ShortPants ), 1015279, 1025422, 24.8, 49.8, typeof( Cloth ), 1044286, 6, 1044287 );
            AddCraft( typeof( LongPants ), 1015279, 1025433, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
            AddCraft( typeof( Kilt ), 1015279, 1025431, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
            AddCraft( typeof( Skirt ), 1015279, 1025398, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( ElvenPants ), 1015279, 1032665, 80.0, 105.0, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( SuperiorKilt ), 1015279, "superior kilt", 80.0, 105.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( TwoColouredPants ), 1015279, "two colored pants", 80.0, 105.0, typeof( Cloth ), 1044286, 15, 1044287 );
            #endregion

            #region Misc
            AddCraft( typeof( Muzzle ), 1015283, "muzzle", 20.7, 45.7, typeof( Cloth ), 1044286, 6, 1044287 );
            AddCraft( typeof( BodySash ), 1015283, 1025441, 4.1, 29.1, typeof( Cloth ), 1044286, 4, 1044287 );
            AddCraft( typeof( HalfApron ), 1015283, 1025435, 20.7, 45.7, typeof( Cloth ), 1044286, 6, 1044287 );
            AddCraft( typeof( FullApron ), 1015283, 1025437, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );
            AddCraft( typeof( FishnetStockings ), 1015283, "fishnet stockings", 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );

            index = AddCraft( typeof( LeatherContainerEngraver ), 1015283, 1072152, 75.0, 100.0, typeof( Bone ), 1049064, 1, 1049063 );
            AddRes( index, typeof( Leather ), 1044462, 6, 1044463 );
            AddRes( index, typeof( SpoolOfThread ), 1073462, 2, 1073463 );
            AddRes( index, typeof( Dyes ), 1024009, 6, 1044253 );

            AddCraft( typeof( OilCloth ), 1015283, 1041498, 74.6, 99.6, typeof( Cloth ), 1044286, 1, 1044287 );
            AddCraft( typeof( Rope ), 1015283, "rope", 74.6, 99.6, typeof( Cloth ), 1044286, 5, 1044287 );

            AddCraft( typeof( PetLeash ), 1015283, 1064340, 75.0, 100.0, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( Bag ), 1015283, 1064979, 20.0, 45.0, typeof( Leather ), 1044462, 5, 1044463 );
            AddCraft( typeof( Pouch ), 1015283, 1064978, 30.0, 55.0, typeof( Leather ), 1044462, 6, 1044463 );
            AddCraft( typeof( Backpack ), 1015283, 1064980, 40.0, 65.0, typeof( Leather ), 1044462, 7, 1044463 );
            AddCraft( typeof( SmallBagBall ), 1015283, 1064981, 60.0, 85.0, typeof( Leather ), 1044462, 15, 1044463 );
            AddCraft( typeof( LargeBagBall ), 1015283, 1064982, 80.0, 95.0, typeof( Leather ), 1044462, 20, 1044463 );           
            #endregion

            #region Footwear
            AddCraft( typeof( ElvenBoots ), 1015283, 1072902, 80.0, 105.0, typeof( Leather ), 1044462, 15, 1044463 );
            AddCraft( typeof( FurBoots ), 1015288, 1028967, 50.0, 75.0, typeof( Cloth ), 1044286, 12, 1044287 );
            AddCraft( typeof( Sandals ), 1015288, 1025901, 12.4, 37.4, typeof( Leather ), 1044462, 4, 1044463 );
            AddCraft( typeof( Shoes ), 1015288, 1025904, 16.5, 41.5, typeof( Leather ), 1044462, 6, 1044463 );
            AddCraft( typeof( Boots ), 1015288, 1025899, 33.1, 58.1, typeof( Leather ), 1044462, 8, 1044463 );
            AddCraft( typeof( ThighBoots ), 1015288, 1025906, 41.4, 66.4, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( ReinforcedBoots ), 1015288, "reinforced boots", 70.0, 95.0, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( SharpedShoes ), 1015288, "sharped shoes", 41.4, 66.4, typeof( Cloth ), 1044286, 10, 1044287 );
            #endregion

            #region Leather Armor
            AddCraft( typeof( LeatherGorget ), 1015293, 1025063, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
            AddCraft( typeof( LeatherCap ), 1015293, 1027609, 6.2, 31.2, typeof( Leather ), 1044462, 2, 1044463 );
            AddCraft( typeof( LeatherGloves ), 1015293, 1025062, 51.8, 76.8, typeof( Leather ), 1044462, 3, 1044463 );
            AddCraft( typeof( LeatherArms ), 1015293, 1025061, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
            AddCraft( typeof( LeatherLegs ), 1015293, 1025067, 66.3, 91.3, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( LeatherChest ), 1015293, 1025068, 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );

            AddCraft( typeof( LeafChest ), 1015293, 1032667, 75.0, 100.0, typeof( Leather ), 1044462, 15, 1044463 );
            AddCraft( typeof( LeafArms ), 1015293, 1032670, 60.0, 85.0, typeof( Leather ), 1044462, 12, 1044463 );
            AddCraft( typeof( LeafGloves ), 1015293, 1032668, 60.0, 85.0, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( LeafLegs ), 1015293, 1032671, 75.0, 100.0, typeof( Leather ), 1044462, 15, 1044463 );
            AddCraft( typeof( LeafGorget ), 1015293, 1032669, 65.0, 90.0, typeof( Leather ), 1044462, 12, 1044463 );
            AddCraft( typeof( LeafTonlet ), 1015293, 1032672, 70.0, 95.0, typeof( Leather ), 1044462, 12, 1044463 );

            AddCraft( typeof( ShortLeatherChest ), 1015293, "short leather chest", 70.5, 95.5, typeof( Leather ), 1044462, 9, 1044463 );
            AddCraft( typeof( ShortLeatherSkirt ), 1015293, "short leather skirt", 68.0, 93.0, typeof( Leather ), 1044462, 5, 1044463 );
            AddCraft( typeof( LongLeatherChest ), 1015293, "long leather chest", 70.5, 95.5, typeof( Leather ), 1044462, 14, 1044463 );
            AddCraft( typeof( LongLeatherSkirt ), 1015293, "long leather skirt", 68.0, 93.0, typeof( Leather ), 1044462, 10, 1044463 );

            AddCraft( typeof( ScoutGorget ), 1015293, "scout gorget", 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
            AddCraft( typeof( ScoutHelm ), 1015293, "scout cap", 6.2, 31.2, typeof( Leather ), 1044462, 2, 1044463 );
            AddCraft( typeof( ScoutGloves ), 1015293, "scout gloves", 51.8, 76.8, typeof( Leather ), 1044462, 3, 1044463 );
            AddCraft( typeof( ScoutSleeves ), 1015293, "scout sleeves", 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
            AddCraft( typeof( ScoutLegs ), 1015293, "scout legs", 66.3, 91.3, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( ScoutChest ), 1015293, "scout chest", 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );
            #endregion

            #region Studded Armor
            const double handicap = 5.0;

            AddCraft( typeof( StuddedGorget ), 1015300, 1025078, 78.8, 103.8 - handicap, typeof( Leather ), 1044462, 6, 1044463 );
            AddCraft( typeof( StuddedGloves ), 1015300, 1025077, 82.9, 107.9 - handicap, typeof( Leather ), 1044462, 8, 1044463 );
            AddCraft( typeof( StuddedArms ), 1015300, 1025076, 87.1, 112.1 - handicap, typeof( Leather ), 1044462, 10, 1044463 );
            AddCraft( typeof( StuddedLegs ), 1015300, 1025082, 91.2, 116.2 - handicap, typeof( Leather ), 1044462, 12, 1044463 );
            AddCraft( typeof( StuddedChest ), 1015300, 1025083, 94.0, 119.0 - handicap, typeof( Leather ), 1044462, 14, 1044463 );
            #endregion

            #region Female Armor
            AddCraft( typeof( LeatherShorts ), 1015306, 1027168, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
            AddCraft( typeof( LeatherSkirt ), 1015306, 1027176, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
            AddCraft( typeof( LeatherBustierArms ), 1015306, 1027178, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
            AddCraft( typeof( StuddedBustierArms ), 1015306, 1027180, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
            AddCraft( typeof( FemaleLeatherChest ), 1015306, 1027174, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
            AddCraft( typeof( FemaleStuddedChest ), 1015306, 1027170, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
            #endregion

            #region Bone Armor
            index = AddCraft( typeof( BoneHelm ), 1049149, 1025206, 85.0, 110.0, typeof( Leather ), 1044462, 4, 1044463 );
            AddRes( index, typeof( Bone ), 1049064, 2, 1049063 );

            index = AddCraft( typeof( BoneGloves ), 1049149, 1025205, 89.0, 114.0, typeof( Leather ), 1044462, 6, 1044463 );
            AddRes( index, typeof( Bone ), 1049064, 2, 1049063 );

            index = AddCraft( typeof( BoneArms ), 1049149, 1025203, 92.0, 117.0, typeof( Leather ), 1044462, 8, 1044463 );
            AddRes( index, typeof( Bone ), 1049064, 4, 1049063 );

            index = AddCraft( typeof( BoneLegs ), 1049149, 1025202, 95.0, 120.0, typeof( Leather ), 1044462, 10, 1044463 );
            AddRes( index, typeof( Bone ), 1049064, 6, 1049063 );

            index = AddCraft( typeof( BoneChest ), 1049149, 1025199, 96.0, 121.0, typeof( Leather ), 1044462, 12, 1044463 );
            AddRes( index, typeof( Bone ), 1049064, 10, 1049063 );
            #endregion

            #region carpets
            // Brown Arabesque
            AddCraft( typeof( SquareArabesqueCarpetSmallDeed ), "Carpets", 1064403, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 9, 1042081 );
            AddCraft( typeof( LargeArabesqueCarpetSouthDeed ), "Carpets", 1064404, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddCraft( typeof( LargeArabesqueCarpetEastDeed ), "Carpets", 1064405, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );

            // Blue Arabesque
            AddCraft( typeof( SquareBlueArabesqueCarpetDeed ), "Carpets", 1064409, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 14, 1042081 );
            AddCraft( typeof( LargeBlueArabesqueCarpetSouthDeed ), "Carpets", 1064410, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddCraft( typeof( LargeBlueArabesqueCarpetEastDeed ), "Carpets", 1064411, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );

            // Blue
            AddCraft( typeof( SquareBlueCarpetLargeDeed ), "Carpets", 1064415, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 14, 1042081 );
            AddCraft( typeof( LargeBlueCarpetSouthDeed ), "Carpets", 1064416, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddCraft( typeof( LargeBlueCarpetEastDeed ), "Carpets", 1064417, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );

            // Brown Decorated
            AddCraft( typeof( SquareDecoratedCarpetMediumDeed ), "Carpets", 1064421, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 14, 1042081 );
            AddCraft( typeof( LargeDecoratedCarpetSouthDeed ), "Carpets", 1064422, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddCraft( typeof( LargeDecoratedCarpetEastDeed ), "Carpets", 1064423, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );

            // Red
            AddCraft( typeof( RedCarpetSouthDeed ), "Carpets", 1064427, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 16, 1042081 );
            AddCraft( typeof( RedCarpetEastDeed ), "Carpets", 1064428, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 16, 1042081 );

            // Red Decorated
            AddCraft( typeof( RedDecoratedCarpetSouthDeed ), "Carpets", 1064433, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 16, 1042081 );
            AddCraft( typeof( RedDecoratedCarpetEastDeed ), "Carpets", 1064434, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 16, 1042081 );

            // Blue Decorated
            AddCraft( typeof( SmallSquareBlueDecoratedCarpetAddonDeed ), "Carpets", 1064433, 70.1, 80.1, typeof( SpoolOfThread ), 1024000, 14, 1042081 );
            AddCraft( typeof( MediumSquareBlueDecoratedCarpetAddonDeed ), "Carpets", 1064434, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 18, 1042081 );
            AddCraft( typeof( LargeSquareBlueDecoratedCarpetAddonDeed ), "Carpets", 1064434, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 22, 1042081 );
            #endregion

            #region Pillows
            index = AddCraft( typeof( Pillow1 ), 1064452, 1064453, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow2 ), 1064452, 1064454, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow3 ), 1064452, 1064455, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow4 ), 1064452, 1064456, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow5 ), 1064452, 1064457, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow6 ), 1064452, 1064458, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow7 ), 1064452, 1064459, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow8 ), 1064452, 1064460, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow12 ), 1064452, 1064461, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow13 ), 1064452, 1064462, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );

            index = AddCraft( typeof( Pillow14 ), 1064452, 1064463, 65.1, 75.1, typeof( Wool ), 1064469, 2 );
            AddRes( index, typeof( Cloth ), 1044286, 2 );
            #endregion

            #region Banners
            index = AddCraft( typeof( LargeBanner1WestAddonDeed ), 1065000, 1065002, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner1SouthAddonDeed ), 1065000, 1065001, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner2WestAddonDeed ), 1065000, 1065004, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner2SouthAddonDeed ), 1065000, 1065003, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner3WestAddonDeed ), 1065000, 1065006, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner3SouthAddonDeed ), 1065000, 1065005, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner4WestAddonDeed ), 1065000, 1065008, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( LargeBanner4SouthAddonDeed ), 1065000, 1065007, 80.1, 90.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( SmallBanner1WestAddonDeed ), 1065000, 1065010, 60.1, 70.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( SmallBanner1SouthAddonDeed ), 1065000, 1065009, 60.1, 70.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( SmallBanner2WestAddonDeed ), 1065000, 1065012, 60.1, 70.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );

            index = AddCraft( typeof( SmallBanner2SouthAddonDeed ), 1065000, 1065011, 60.1, 70.1, typeof( SpoolOfThread ), 1024000, 20, 1042081 );
            AddSkill( index, SkillName.Carpentry, 50.0, 55.0 );
            AddRes( index, typeof( Cloth ), 1044286, 10 );
            AddRes( index, typeof( Wool ), 1064469, 5 );
            AddRes( index, typeof( Log ), 1044041, 5 );
            #endregion

            #region SubRes: Leather
            SetSubRes( typeof( Leather ), 1049150 ); // Set the overridable material

            AddSubRes( typeof( Leather ), 1049150, 00.0, 1044462, 1049311, 1.00 );
            AddSubRes( typeof( WolfLeather ), 1066280, 65.0, 1044462, 1049311, 1.03 );
            AddSubRes( typeof( BearLeather ), 1066281, 70.0, 1044462, 1049311, 1.04 );
            AddSubRes( typeof( AracnidLeather ), 1066282, 72.5, 1044462, 1049311, 1.05 );
            AddSubRes( typeof( SpinedLeather ), 1066283, 75.0, 1044462, 1049311, 1.06 );
            AddSubRes( typeof( OrcishLeather ), 1066284, 77.5, 1044462, 1049311, 1.07 );
            AddSubRes( typeof( BarbedLeather ), 1066285, 80.0, 1044462, 1049311, 1.08 );
            AddSubRes( typeof( UndeadLeather ), 1066286, 85.0, 1044462, 1049311, 1.09 );
            AddSubRes( typeof( HornedLeather ), 1066287, 87.5, 1044462, 1049311, 1.10 );
            AddSubRes( typeof( LavaLeather ), 1066288, 90.0, 1044462, 1049311, 1.11 );
            AddSubRes( typeof( ArcticLeather ), 1066289, 92.5, 1044462, 1049311, 1.12 );
            AddSubRes( typeof( GreenDragonLeather ), 1066290, 95.0, 1044462, 1049311, 1.20 );
            AddSubRes( typeof( BlueDragonLeather ), 1066291, 95.0, 1044462, 1049311, 1.20 );
            AddSubRes( typeof( BlackDragonLeather ), 1066292, 95.0, 1044462, 1049311, 1.20 );
            AddSubRes( typeof( RedDragonLeather ), 1066293, 99.0, 1044462, 1049311, 1.25 );
            AddSubRes( typeof( AbyssLeather ), 1066294, 99.0, 1044462, 1049311, 1.25 );
            #endregion

            MarkOption = true;
            Repair = true;
            CanEnhance = false;
        }

		public override void InitCraftList()
		{
			int index = -1;

            #region mod by Dies Irae
            if( !Core.AOS )
            {
                InitOldCraftList();
                return;
            }
            #endregion

			#region Hats
			AddCraft( typeof( SkullCap ), 1011375, 1025444, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
			AddCraft( typeof( Bandana ), 1011375, 1025440, 0.0, 25.0, typeof( Cloth ), 1044286, 2, 1044287 );
			AddCraft( typeof( FloppyHat ), 1011375, 1025907, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( Cap ), 1011375, 1025909, -18.8, 6.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( WideBrimHat ), 1011375, 1025908, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( StrawHat ), 1011375, 1025911, 6.2, 31.2, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( TallStrawHat ), 1011375, 1025910, 6.7, 31.7, typeof( Cloth ), 1044286, 13, 1044287 );
			AddCraft( typeof( WizardsHat ), 1011375, 1025912, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );
			AddCraft( typeof( Bonnet ), 1011375, 1025913, 6.2, 31.2, typeof( Cloth ), 1044286, 11, 1044287 );
			AddCraft( typeof( FeatheredHat ), 1011375, 1025914, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( TricorneHat ), 1011375, 1025915, 6.2, 31.2, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( JesterHat ), 1011375, 1025916, 7.2, 32.2, typeof( Cloth ), 1044286, 15, 1044287 );

			if ( Core.AOS )
				AddCraft( typeof( FlowerGarland ), 1011375, 1028965, 10.0, 35.0, typeof( Cloth ), 1044286, 5, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( ClothNinjaHood ), 1011375, 1030202, 80.0, 105.0, typeof( Cloth ), 1044286, 13, 1044287 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( Kasa ), 1011375, 1030211, 60.0, 85.0, typeof( Cloth ), 1044286, 12, 1044287 );	
				SetNeededExpansion( index, Expansion.SE );
			}
			#endregion

			#region Shirts
			AddCraft( typeof( Doublet ), 1015269, 1028059, 0, 25.0, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Shirt ), 1015269, 1025399, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( FancyShirt ), 1015269, 1027933, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Tunic ), 1015269, 1028097, 00.0, 25.0, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Surcoat ), 1015269, 1028189, 8.2, 33.2, typeof( Cloth ), 1044286, 14, 1044287 );
			AddCraft( typeof( PlainDress ), 1015269, 1027937, 12.4, 37.4, typeof( Cloth ), 1044286, 10, 1044287 );
			AddCraft( typeof( FancyDress ), 1015269, 1027935, 33.1, 58.1, typeof( Cloth ), 1044286, 12, 1044287 );
			AddCraft( typeof( Cloak ), 1015269, 1025397, 41.4, 66.4, typeof( Cloth ), 1044286, 14, 1044287 );
			AddCraft( typeof( Robe ), 1015269, 1027939, 53.9, 78.9, typeof( Cloth ), 1044286, 16, 1044287 );
			AddCraft( typeof( JesterSuit ), 1015269, 1028095, 8.2, 33.2, typeof( Cloth ), 1044286, 24, 1044287 );

			if ( Core.AOS )
			{
				AddCraft( typeof( FurCape ), 1015269, 1028969, 35.0, 60.0, typeof( Cloth ), 1044286, 13, 1044287 );
				AddCraft( typeof( GildedDress ), 1015269, 1028973, 37.5, 62.5, typeof( Cloth ), 1044286, 16, 1044287 );
				AddCraft( typeof( FormalShirt ), 1015269, 1028975, 26.0, 51.0, typeof( Cloth ), 1044286, 16, 1044287 );
			}

			if( Core.SE )
			{
				index = AddCraft( typeof( ClothNinjaJacket ), 1015269, 1030207, 75.0, 100.0, typeof( Cloth ), 1044286, 12, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( Kamishimo ), 1015269, 1030212, 75.0, 100.0, typeof( Cloth ), 1044286, 15, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( HakamaShita ), 1015269, 1030215, 40.0, 65.0, typeof( Cloth ), 1044286, 14, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( MaleKimono ), 1015269, 1030189, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( FemaleKimono ), 1015269, 1030190, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( JinBaori ), 1015269, 1030220, 30.0, 55.0, typeof( Cloth ), 1044286, 12, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Pants
			AddCraft( typeof( ShortPants ), 1015279, 1025422, 24.8, 49.8, typeof( Cloth ), 1044286, 6, 1044287 );
			AddCraft( typeof( LongPants ), 1015279, 1025433, 24.8, 49.8, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Kilt ), 1015279, 1025431, 20.7, 45.7, typeof( Cloth ), 1044286, 8, 1044287 );
			AddCraft( typeof( Skirt ), 1015279, 1025398, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );

			if ( Core.AOS )
				AddCraft( typeof( FurSarong ), 1015279, 1028971, 35.0, 60.0, typeof( Cloth ), 1044286, 12, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( Hakama ), 1015279, 1030213, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( TattsukeHakama ), 1015279, 1030214, 50.0, 75.0, typeof( Cloth ), 1044286, 16, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Misc
			AddCraft( typeof( BodySash ), 1015283, 1025441, 4.1, 29.1, typeof( Cloth ), 1044286, 4, 1044287 );
			AddCraft( typeof( HalfApron ), 1015283, 1025435, 20.7, 45.7, typeof( Cloth ), 1044286, 6, 1044287 );
			AddCraft( typeof( FullApron ), 1015283, 1025437, 29.0, 54.0, typeof( Cloth ), 1044286, 10, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( Obi ), 1015283, 1030219, 20.0, 45.0, typeof( Cloth ), 1044286, 6, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			if( Core.ML )
			{
				index = AddCraft( typeof( ElvenQuiver ), 1015283, 1032657, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				AddRecipe( index, 501 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( QuiverOfFire ), 1015283, 1073109, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				AddRes( index, typeof( FireRuby ), 1032695, 15, 1042081 );
				AddRecipe( index, 502 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( QuiverOfIce ), 1015283, 1073110, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				AddRes( index, typeof( WhitePearl ), 1032694, 15, 1042081 );
				AddRecipe( index, 503 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( QuiverOfBlight ), 1015283, 1073111, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				AddRes( index, typeof( Blight ), 1032675, 10, 1042081 );
				AddRecipe( index, 504 );
				SetNeededExpansion( index, Expansion.ML );

				index = AddCraft( typeof( QuiverOfLightning ), 1015283, 1073112, 65.0, 115.0, typeof( Leather ), 1044462, 28, 1044463 );
				AddRes( index, typeof( Corruption ), 1032676, 10, 1042081 );
				AddRecipe( index, 505 );
				SetNeededExpansion( index, Expansion.ML );
			}

			AddCraft( typeof( OilCloth ), 1015283, 1041498, 74.6, 99.6, typeof( Cloth ), 1044286, 1, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( GozaMatEastDeed ), 1015283, 1030404, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( GozaMatSouthDeed ), 1015283, 1030405, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( SquareGozaMatEastDeed ), 1015283, 1030407, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( SquareGozaMatSouthDeed ), 1015283, 1030406, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeGozaMatEastDeed ), 1015283, 1030408, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeGozaMatSouthDeed ), 1015283, 1030409, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeSquareGozaMatEastDeed ), 1015283, 1030411, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( BrocadeSquareGozaMatSouthDeed ), 1015283, 1030410, 55.0, 80.0, typeof( Cloth ), 1044286, 25, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Footwear
			if ( Core.AOS )
				AddCraft( typeof( FurBoots ), 1015288, 1028967, 50.0, 75.0, typeof( Cloth ), 1044286, 12, 1044287 );

			if( Core.SE )
			{
				index = AddCraft( typeof( NinjaTabi ), 1015288, 1030210, 70.0, 95.0, typeof( Cloth ), 1044286, 10, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( SamuraiTabi ), 1015288, 1030209, 20.0, 45.0, typeof( Cloth ), 1044286, 6, 1044287 );
				SetNeededExpansion( index, Expansion.SE );
			}

			AddCraft( typeof( Sandals ), 1015288, 1025901, 12.4, 37.4, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( Shoes ), 1015288, 1025904, 16.5, 41.5, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( Boots ), 1015288, 1025899, 33.1, 58.1, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( ThighBoots ), 1015288, 1025906, 41.4, 66.4, typeof( Leather ), 1044462, 10, 1044463 );
			#endregion

			#region Leather Armor

			AddCraft( typeof( LeatherGorget ), 1015293, 1025063, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( LeatherCap ), 1015293, 1027609, 6.2, 31.2, typeof( Leather ), 1044462, 2, 1044463 );
			AddCraft( typeof( LeatherGloves ), 1015293, 1025062, 51.8, 76.8, typeof( Leather ), 1044462, 3, 1044463 );
			AddCraft( typeof( LeatherArms ), 1015293, 1025061, 53.9, 78.9, typeof( Leather ), 1044462, 4, 1044463 );
			AddCraft( typeof( LeatherLegs ), 1015293, 1025067, 66.3, 91.3, typeof( Leather ), 1044462, 10, 1044463 );
			AddCraft( typeof( LeatherChest ), 1015293, 1025068, 70.5, 95.5, typeof( Leather ), 1044462, 12, 1044463 );

			if( Core.SE )
			{
				index = AddCraft( typeof( LeatherJingasa ), 1015293, 1030177, 45.0, 70.0, typeof( Leather ), 1044462, 4, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherMempo ), 1015293, 1030181, 80.0, 105.0, typeof( Leather ), 1044462, 8, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherDo ), 1015293, 1030182, 75.0, 100.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherHiroSode ), 1015293, 1030185, 55.0, 80.0, typeof( Leather ), 1044462, 5, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherSuneate ), 1015293, 1030193, 68.0, 93.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherHaidate ), 1015293, 1030197, 68.0, 93.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaPants ), 1015293, 1030204, 80.0, 105.0, typeof( Leather ), 1044462, 13, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaJacket ), 1015293, 1030206, 85.0, 110.0, typeof( Leather ), 1044462, 13, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaBelt ), 1015293, 1030203, 50.0, 75.0, typeof( Leather ), 1044462, 5, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaMitts ), 1015293, 1030205, 65.0, 90.0, typeof( Leather ), 1044462, 12, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( LeatherNinjaHood ), 1015293, 1030201, 90.0, 115.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Studded Armor
			AddCraft( typeof( StuddedGorget ), 1015300, 1025078, 78.8, 103.8, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( StuddedGloves ), 1015300, 1025077, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( StuddedArms ), 1015300, 1025076, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
			AddCraft( typeof( StuddedLegs ), 1015300, 1025082, 91.2, 116.2, typeof( Leather ), 1044462, 12, 1044463 );
			AddCraft( typeof( StuddedChest ), 1015300, 1025083, 94.0, 119.0, typeof( Leather ), 1044462, 14, 1044463 );

			if( Core.SE )
			{
				index = AddCraft( typeof( StuddedMempo ), 1015300, 1030216, 80.0, 105.0, typeof( Leather ), 1044462, 8, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedDo ), 1015300, 1030183, 95.0, 120.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedHiroSode ), 1015300, 1030186, 85.0, 110.0, typeof( Leather ), 1044462, 8, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedSuneate ), 1015300, 1030194, 92.0, 117.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
				index = AddCraft( typeof( StuddedHaidate ), 1015300, 1030198, 92.0, 117.0, typeof( Leather ), 1044462, 14, 1044463 );
				SetNeededExpansion( index, Expansion.SE );
			}

			#endregion

			#region Female Armor
			AddCraft( typeof( LeatherShorts ), 1015306, 1027168, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( LeatherSkirt ), 1015306, 1027176, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( LeatherBustierArms ), 1015306, 1027178, 58.0, 83.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddCraft( typeof( StuddedBustierArms ), 1015306, 1027180, 82.9, 107.9, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( FemaleLeatherChest ), 1015306, 1027174, 62.2, 87.2, typeof( Leather ), 1044462, 8, 1044463 );
			AddCraft( typeof( FemaleStuddedChest ), 1015306, 1027170, 87.1, 112.1, typeof( Leather ), 1044462, 10, 1044463 );
			#endregion

			#region Bone Armor
			index = AddCraft( typeof( BoneHelm ), 1049149, 1025206, 85.0, 110.0, typeof( Leather ), 1044462, 4, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 2, 1049063 );
			
			index = AddCraft( typeof( BoneGloves ), 1049149, 1025205, 89.0, 114.0, typeof( Leather ), 1044462, 6, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 2, 1049063 );

			index = AddCraft( typeof( BoneArms ), 1049149, 1025203, 92.0, 117.0, typeof( Leather ), 1044462, 8, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 4, 1049063 );

			index = AddCraft( typeof( BoneLegs ), 1049149, 1025202, 95.0, 120.0, typeof( Leather ), 1044462, 10, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 6, 1049063 );
		
			index = AddCraft( typeof( BoneChest ), 1049149, 1025199, 96.0, 121.0, typeof( Leather ), 1044462, 12, 1044463 );
			AddRes( index, typeof( Bone ), 1049064, 10, 1049063 );
			#endregion

			// Set the overridable material
			SetSubRes( typeof( Leather ), 1049150 );

			// Add every material you want the player to be able to choose from
			// This will override the overridable material
			AddSubRes( typeof( Leather ),		1049150, 00.0, 1044462, 1049311 );
			AddSubRes( typeof( SpinedLeather ),	1049151, 65.0, 1044462, 1049311 );
			AddSubRes( typeof( HornedLeather ),	1049152, 80.0, 1044462, 1049311 );
			AddSubRes( typeof( BarbedLeather ),	1049153, 99.0, 1044462, 1049311 );

			MarkOption = true;
			Repair = Core.AOS;
			CanEnhance = Core.AOS;
		}
	}
}