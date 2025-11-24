/***************************************************************************
 *                                  DefCrystalCrafting.cs
 *                            		---------------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Engines.OldCraftSystem;
using Midgard.Items;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Engines.Craft
{
    public class DefCrystalCrafting : CraftSystem
    {
        public override string Name { get{ return "Crystals"; } }

        public enum CrystalCrafterRecipes
        {
            CrystalBrazier = 40001,
            CrystalThroneEast = 40002,
            CrystalThroneSouth = 40003,
            CrystalTableEastAddonDeed = 40004,
            CrystalTableSouthAddonDeed = 40005,
            CrystalAltarAddonDeed = 40006,
            CrystalPedestralSmall = 40007,
            CrystalPedestralMedium = 40008,
            CrystalPedestralLargeEmpty = 40009,
            CrystalPedestralLarge = 40010,
            CrystalBullSouthAddonDeed = 40011,
            CrystalBullEastAddonDeed = 40012,
            CrystalBeggarStatueSouth = 40013,
            CrystalBeggarStatueEast = 40014,
            CrystalSupplicantStatueSouth = 40015,
            CrystalSupplicantStatueEast = 40016,
            CrystalRunnerStatueSouth = 40017,
            CrystalRunnerStatueEast = 40018,
            PurpleCrystal = 40019,
            GreenCrystal = 40020,
            MagicCrystalBall = 40021,
            GlobeOfSosariaAddon = 40022
        }

        public static void Initialize()
        {
            CraftSystem sys = CraftSystem;
            InitDebugItemsList();
        }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "CrystalCrafting.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;

        #region fields
        private static CraftSystem m_CraftSystem;
        #endregion

        #region properties
        public override SkillName MainSkill { get { return SkillName.Tinkering; } }
        public override CraftECA ECA { get { return Core.AOS ? CraftECA.ChanceMinusSixtyToFourtyFive : CraftECA.ZeroToFourPercentPlusBonus; } }
        public override string GumpTitleString { get { return "<CENTER>Midgard Crystal Crafting</CENTER>"; } }

        public static CraftSystem CraftSystem
        {
            get
            {
                if( m_CraftSystem == null )
                    m_CraftSystem = new DefCrystalCrafting();

                return m_CraftSystem;
            }
        }
        #endregion

        #region members
        private static Type[] m_CrystalCraftingColorables = new Type[]
		{
		};

        public override bool RetainsColorFrom( CraftItem item, Type type )
        {
            type = item.ItemType;

            bool contains = false;

            for( int i = 0; !contains && i < m_CrystalCraftingColorables.Length; ++i )
                contains = ( m_CrystalCraftingColorables[ i ] == type );

            return contains;
        }

        public override double GetChanceAtMin( CraftItem item )
        {
            return 0.0;
        }

        public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
        {
            if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
                return 1044038; // You have worn out your tool!
            else if( !BaseTool.CheckAccessible( tool, from ) )
                return 1044263; // The tool must be on your person to use.

            if( from.AccessLevel == AccessLevel.Player && m_DebugItemList.Contains( itemType ) )
                return 1064915; // This item is only available to Midgard Staff members. They will decide soon if has to be implemented or removed.

            return 0;
        }

        public override void PlayCraftEffect( Mobile from )
        {
            from.PlaySound( 0x23D );
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

        private static List<Type> m_DebugItemList;

        public static void InitDebugItemsList()
        {
            if( m_DebugItemList == null )
                m_DebugItemList = new List<Type>();
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Miscellaneous
            index = AddCraft( typeof( RawCrystal ), "Miscellaneous", "raw crystal", 50.1, 100.0, typeof( CrystalOre ), "Crystal Ore", 5, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );

            index = AddCraft( typeof( LittleUnshapedCrystal ), "Miscellaneous", "unshaped crystal ( little )", 60.1, 100.0, typeof( RawCrystal ), "Raw Crystal", 3, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );

            index = AddCraft( typeof( LargeUnshapedCrystal ), "Miscellaneous", "unshaped crystal ( large )", 70.1, 100.0, typeof( RawCrystal ), "Raw Crystal", 5, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );

            index = AddCraft( typeof( TallUnshapedCrystal ), "Miscellaneous", "unshaped crystal ( tall )", 70.1, 100.0, typeof( RawCrystal ), "Raw Crystal", 5, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );

            index = AddCraft( typeof( CrystalPillar ), "Miscellaneous", "crystal pillar", 80.1, 100.0, typeof( RawCrystal ), "Raw Crystal", 10, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            #endregion

            #region Decorations
            index = AddCraft( typeof( CrystalBrazier ), "Decorations", "crystal brazier", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 1, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalBrazier );

            index = AddCraft( typeof( CrystalThroneEast ), "Decorations", "crystal throne ( east )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 1, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 4, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalThroneEast );

            index = AddCraft( typeof( CrystalThroneSouth ), "Decorations", "crystal throne ( south )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 1, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 4, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalThroneSouth );

            index = AddCraft( typeof( CrystalTableEastAddonDeed ), "Decorations", "crystal table ( east )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 2, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 4, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalTableEastAddonDeed );

            index = AddCraft( typeof( CrystalTableSouthAddonDeed ), "Decorations", "crystal table ( south )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 2, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 4, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalTableSouthAddonDeed );

            index = AddCraft( typeof( CrystalAltarAddonDeed ), "Decorations", "crystal altar", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 4, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 6, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalAltarAddonDeed );
            #endregion

            #region Pedestrals
            index = AddCraft( typeof( CrystalPedestralSmall ), "Pedestrals", "crystal pedestral ( small )", 50.1, 100.0, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            AddRes( index, typeof( Granite ), "Granite", 1, "You need more granite to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalPedestralSmall );

            index = AddCraft( typeof( CrystalPedestralMedium ), "Pedestrals", "crystal pedestral ( medium )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 2, "You need more crystal to craft that." );
            AddRes( index, typeof( Granite ), "Granite", 2, "You need more granite to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalPedestralMedium );

            index = AddCraft( typeof( CrystalPedestralLargeEmpty ), "Pedestrals", "crystal pedestral ( emtpy )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 1, "You need more crystal to craft that." );
            AddRes( index, typeof( Granite ), "Granite", 3, "You need more granite to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalPedestralLargeEmpty );

            index = AddCraft( typeof( CrystalPedestralLarge ), "Pedestrals", "crystal pedestral ( large )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 6, "You need more crystal to craft that." );
            AddRes( index, typeof( CrystalPedestralLargeEmpty ), "Crystal Pedestral", 1, "You need a crystal pedestral." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalPedestralLarge );
            #endregion

            #region Statues
            index = AddCraft( typeof( CrystalBullSouthAddonDeed ), "Statues", "crystal bull ( south )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 6, "You need more crystal to craft that." );
            AddRes( index, typeof( TallUnshapedCrystal ), "Tall Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalBullSouthAddonDeed );

            index = AddCraft( typeof( CrystalBullEastAddonDeed ), "Statues", "crystal bull ( east )", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 6, "You need more crystal to craft that." );
            AddRes( index, typeof( TallUnshapedCrystal ), "Tall Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalBullEastAddonDeed );

            index = AddCraft( typeof( CrystalBeggarStatueSouth ), "Statues", "crystal beggar ( south )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 3, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalBeggarStatueSouth );

            index = AddCraft( typeof( CrystalBeggarStatueEast ), "Statues", "crystal beggar ( east )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 3, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalBeggarStatueEast );

            index = AddCraft( typeof( CrystalSupplicantStatueSouth ), "Statues", "crystal supplicant ( south )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 3, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalSupplicantStatueSouth );

            index = AddCraft( typeof( CrystalSupplicantStatueEast ), "Statues", "crystal supplicant ( east )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 3, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalSupplicantStatueEast );

            index = AddCraft( typeof( CrystalRunnerStatueSouth ), "Statues", "crystal runner ( south )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 3, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalRunnerStatueSouth );

            index = AddCraft( typeof( CrystalRunnerStatueEast ), "Statues", "crystal runner ( east )", 50.1, 100.0, typeof( TallUnshapedCrystal ), "Tall Crystal", 3, "You need more crystal to craft that." );
            AddRes( index, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.CrystalRunnerStatueEast );
            #endregion

            #region Specials
            index = AddCraft( typeof( PurpleCrystal ), "Specials", "purple crystal", 50.1, 100.0, typeof( LittleUnshapedCrystal ), "Little Crystal", 1, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.PurpleCrystal );

            index = AddCraft( typeof( GreenCrystal ), "Specials", "green crystal", 50.1, 100.0, typeof( LittleUnshapedCrystal ), "Little Crystal", 1, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.GreenCrystal );

            index = AddCraft( typeof( MagicCrystalBall ), "Specials", "magic crystal ball", 50.1, 100.0, typeof( LittleUnshapedCrystal ), "Little Crystal", 2, "You need more crystal to craft that." );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.MagicCrystalBall );

            index = AddCraft( typeof( GlobeOfSosariaAddonDeed ), "Specials", "globe of Sosaria", 50.1, 100.0, typeof( LargeUnshapedCrystal ), "Large Crystal", 10, "You need more crystal to craft that." );
            AddRes( index, typeof( Granite ), 1044514, 10, 1044513 );
            AddRes( index, typeof( IronIngot ), 1044036, 100, 1044037 );
            SetNeededExpansion( index, Expansion.ML );
            AddRecipe( index, (int) CrystalCrafterRecipes.GlobeOfSosariaAddon );
            #endregion

            MarkOption = true;
        }
        #endregion

        #region costruttori
        private DefCrystalCrafting()
            : base( 5, 10, 1.5 )
        {
        }
        #endregion
    }
}
