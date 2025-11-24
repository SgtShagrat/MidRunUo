using System;
using Midgard.Engines.OldCraftSystem;
using Midgard.Items;
using Midgard.Items.StatueSystem;

using Server.Items; 
using Server.Mobiles; 
using System.Collections.Generic;

namespace Server.Engines.Craft 
{ 
	public class DefMasonry : CraftSystem 
	{
		#region mod by Dies Irae
        public override string Name { get{ return "Masonry"; } }

		public static void Initialize()
		{
			InitDebugItemsList();
		}

        public override bool SupportOldMenu
        {
            get { return true; }
        }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Masonry.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;
		#endregion
		
		public override SkillName MainSkill 
		{ 
			get{ return SkillName.Carpentry; } 
		} 

		public override int GumpTitleNumber 
		{ 
			get{ return 1044500; } // <CENTER>MASONRY MENU</CENTER> 
		} 

		private static CraftSystem m_CraftSystem; 

		public static CraftSystem CraftSystem 
		{ 
			get 
			{ 
				if ( m_CraftSystem == null ) 
					m_CraftSystem = new DefMasonry(); 

				return m_CraftSystem; 
			} 
		} 

		public override double GetChanceAtMin( CraftItem item ) 
		{ 
			return 0.0; // 0% 
		} 

		private DefMasonry() : base( 2, 4, 1.25 ) // mod by Dies Irae // base( 1, 2, 1.7 ) 
		{ 
		} 

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
			return true;
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckTool( tool, from ) )
				return 1048146; // If you have a tool equipped, you must use that tool.
			/* else if ( !(from is PlayerMobile && ((PlayerMobile)from).Masonry && from.Skills[SkillName.Carpentry].Base >= 100.0) )
				return 1044633; // You havent learned stonecraft. */
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			#region modifica By Dies Irae
			if( from.AccessLevel == AccessLevel.Player && m_DebugItemList.Contains( itemType ) )
				return 1064915; // This item is only available to Midgard Staff members. They will decide soon if has to be implemented or removed.
			#endregion

			return 0;
		} 

		public override void PlayCraftEffect( Mobile from ) 
		{ 
			// no effects
            if( from.Body.Type == BodyType.Human && !from.Mounted )
                from.Animate( 9, 5, 1, true, false, 0 );
            new InternalTimer( from ).Start(); 
		} 

		// Delay to synchronize the sound with the hit on the anvil 
		private class InternalTimer : Timer 
		{ 
			private Mobile m_From; 

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 0.7 ) ) 
			{ 
				m_From = from; 
			} 

			protected override void OnTick() 
			{ 
				m_From.PlaySound( 0x23D ); 
			} 
		} 

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item ) 
		{ 
			if ( toolBroken ) 
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool 

			if ( failed ) 
			{ 
				if ( lostMaterial ) 
					return 1044043; // You failed to create the item, and some of your materials are lost. 
				else 
					return 1044157; // You failed to create the item, but no materials were lost. 
			} 
			else 
			{ 
				if ( quality == 0 ) 
					return 502785; // You were barely able to make this item.  It's quality is below average. 
				else if ( makersMark && quality == 2 ) 
					return 1044156; // You create an exceptional quality item and affix your maker's mark. 
				else if ( quality == 2 ) 
					return 1044155; // You create an exceptional quality item. 
				else             
					return 1044154; // You create the item. 
			} 
		} 

		#region modifica by Dies Irae
		private static List<Type> m_DebugItemList;

		public static void InitDebugItemsList()
		{
			if( m_DebugItemList == null )
				m_DebugItemList = new List<Type>();
			
            //m_DebugItemList.Add( typeof(StoneAnvilSouthDeed) );
            //m_DebugItemList.Add( typeof(StoneAnvilEastDeed) );
		}
		#endregion

        public void InitOldCraftList()
        {
            #region Decorations
            AddCraft( typeof( Vase ), 1044501, 1022888, 52.5, 102.5, typeof( Granite ), 1044514, 1, 1044513 );
            AddCraft( typeof( LargeVase ), 1044501, 1022887, 52.5, 102.5, typeof( Granite ), 1044514, 3, 1044513 );
            #endregion

            #region Furniture
            AddCraft( typeof( StoneChair ), 1044502, 1024635, 55.0, 105.0, typeof( Granite ), 1044514, 4, 1044513 );
            AddCraft( typeof( MediumStoneTableEastDeed ), 1044502, 1044508, 65.0, 115.0, typeof( Granite ), 1044514, 6, 1044513 );
            AddCraft( typeof( MediumStoneTableSouthDeed ), 1044502, 1044509, 65.0, 115.0, typeof( Granite ), 1044514, 6, 1044513 );
            AddCraft( typeof( LargeStoneTableEastDeed ), 1044502, 1044511, 75.0, 125.0, typeof( Granite ), 1044514, 9, 1044513 );
            AddCraft( typeof( LargeStoneTableSouthDeed ), 1044502, 1044512, 75.0, 125.0, typeof( Granite ), 1044514, 9, 1044513 );
            #endregion

            #region Statues
            AddCraft( typeof( StatueSouth ), 1044503, 1044505, 60.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
            AddCraft( typeof( StatueNorth ), 1044503, 1044506, 60.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
            AddCraft( typeof( StatueEast ), 1044503, 1044507, 60.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
            AddCraft( typeof( StatuePegasus ), 1044503, 1044510, 70.0, 130.0, typeof( Granite ), 1044514, 4, 1044513 );
            #endregion

            #region GraveStones
            AddCraft( typeof( GraveStone1 ), 1064552, 1064553, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone2 ), 1064552, 1064554, 85.0, 135.0, typeof( Granite ), 1044514, 1, 1044513 );
            AddCraft( typeof( GraveStone3 ), 1064552, 1064555, 85.0, 135.0, typeof( Granite ), 1044514, 5, 1044513 );
            AddCraft( typeof( GraveStone4 ), 1064552, 1064556, 85.0, 135.0, typeof( Granite ), 1044514, 6, 1044513 );
            AddCraft( typeof( GraveStone5 ), 1064552, 1064557, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone6 ), 1064552, 1064558, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone7 ), 1064552, 1064559, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone8 ), 1064552, 1064560, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone9 ), 1064552, 1064561, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone10 ), 1064552, 1064562, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone12 ), 1064552, 1064563, 85.0, 135.0, typeof( Granite ), 1044514, 5, 1044513 );
            AddCraft( typeof( GraveStone14 ), 1064552, 1064564, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone15 ), 1064552, 1064565, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone16 ), 1064552, 1064566, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone17 ), 1064552, 1064567, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone18 ), 1064552, 1064568, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone19 ), 1064552, 1064569, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone20 ), 1064552, 1064570, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone21 ), 1064552, 1064571, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone22 ), 1064552, 1064572, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone23 ), 1064552, 1064573, 85.0, 135.0, typeof( Granite ), 1044514, 2, 1044513 );
            AddCraft( typeof( GraveStone24 ), 1064552, 1064574, 85.0, 135.0, typeof( Granite ), 1044514, 3, 1044513 );
            #endregion

            #region Misc Addons
            AddCraft( typeof( StoneAnvilSouthDeed ), 1044290, 1072876, 78.0, 128.0, typeof( Granite ), 1044514, 20, 1044513 );
            AddCraft( typeof( StoneAnvilEastDeed ), 1044290, 1073392, 78.0, 128.0, typeof( Granite ), 1044514, 20, 1044513 );
            // AddCraft( typeof( MidgardStatueMaker ), 1044290, "statue maker", 95.0, 115.0, typeof( Granite ), 1044514, 30, 1044513 );
            #endregion

            #region SubRes: Granite
            SetSubRes( typeof( Granite ), 1044525 );

            AddSubRes( typeof( Granite ), 1044525, 50.0, 1044514, 1044527 );

            AddSubRes( typeof( DullCopperGranite ), 1044023, 50.0, 1044514, 1044527 );
            AddSubRes( typeof( ShadowIronGranite ), 1044024, 50.0, 1044514, 1044527 );
            AddSubRes( typeof( BronzeGranite ), 1044026, 55.0, 1044514, 1044527 );
            AddSubRes( typeof( CopperGranite ), 1044025, 55.0, 1044514, 1044527 );

            AddSubRes( typeof( GraphiteGranite ), 1044026, 60.0, 1044514, 1044527 );
            AddSubRes( typeof( AgapiteGranite ), 1044028, 60.0, 1044514, 1044527 );
            AddSubRes( typeof( VeriteGranite ), 1044029, 65.0, 1044514, 1044527 );
            AddSubRes( typeof( ValoriteGranite ), 1044030, 65.0, 1044514, 1044527 );

            AddSubRes( typeof( PyriteGranite ), 1044026, 70.0, 1044514, 1044527 );
            AddSubRes( typeof( AzuriteGranite ), 1044028, 70.0, 1044514, 1044527 );
            AddSubRes( typeof( VanadiumGranite ), 1044029, 75.0, 1044514, 1044527 );
            AddSubRes( typeof( ValoriteGranite ), 1044030, 75.0, 1044514, 1044527 );

            AddSubRes( typeof( SilverGranite ), 1044026, 80.0, 1044514, 1044527 );
            AddSubRes( typeof( PlatinumGranite ), 1044028, 80.0, 1044514, 1044527 );
            AddSubRes( typeof( AmethystGranite ), 1044029, 85.0, 1044514, 1044527 );
            AddSubRes( typeof( TitaniumGranite ), 1044030, 85.0, 1044514, 1044527 );

            AddSubRes( typeof( GoldGranite ), 1044027, 90.0, 1044514, 1044527 );
            AddSubRes( typeof( XenianGranite ), 1044028, 90.0, 1044514, 1044527 );
            AddSubRes( typeof( RubidianGranite ), 1044029, 92.5, 1044514, 1044527 );
            AddSubRes( typeof( ObsidianGranite ), 1044030, 95.0, 1044514, 1044527 );

            AddSubRes( typeof( EbonSapphireGranite ), 1044028, 97.5, 1044514, 1044527 );
            AddSubRes( typeof( DarkRubyGranite ), 1044029, 99.0, 1044514, 1044527 );
            AddSubRes( typeof( RadiantDiamondGranite ), 1044030, 99.9, 1044514, 1044527 );
            #endregion
        }

	    public override void InitCraftList() 
		{
            #region mod by Dies Irae
            if( !Core.AOS )
            {
                InitOldCraftList();
                return;
            }
            #endregion

			// Decorations
			AddCraft( typeof( Vase ), 1044501, 1022888, 52.5, 102.5, typeof( Granite ), 1044514, 1, 1044513 );
			AddCraft( typeof( LargeVase ), 1044501, 1022887, 52.5, 102.5, typeof( Granite ), 1044514, 3, 1044513 );
			
			if( Core.SE )
			{
				int index = AddCraft( typeof( SmallUrn ), 1044501, 1029244, 82.0, 132.0, typeof( Granite ), 1044514, 3, 1044513 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( SmallTowerSculpture ), 1044501, 1029242, 82.0, 132.0, typeof( Granite ), 1044514, 3, 1044513 );
				SetNeededExpansion( index, Expansion.SE );
			}

			// Furniture
			AddCraft( typeof( StoneChair ), 1044502, 1024635, 55.0, 105.0, typeof( Granite ), 1044514, 4, 1044513 );
			AddCraft( typeof( MediumStoneTableEastDeed ), 1044502, 1044508, 65.0, 115.0, typeof( Granite ), 1044514, 6, 1044513 );
			AddCraft( typeof( MediumStoneTableSouthDeed ), 1044502, 1044509, 65.0, 115.0, typeof( Granite ), 1044514, 6, 1044513 );
			AddCraft( typeof( LargeStoneTableEastDeed ), 1044502, 1044511, 75.0, 125.0, typeof( Granite ), 1044514, 9, 1044513 );
			AddCraft( typeof( LargeStoneTableSouthDeed ), 1044502, 1044512, 75.0, 125.0, typeof( Granite ), 1044514, 9, 1044513 );

			// Statues
			AddCraft( typeof( StatueSouth ), 1044503, 1044505, 60.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatueNorth ), 1044503, 1044506, 60.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatueEast ), 1044503, 1044507, 60.0, 120.0, typeof( Granite ), 1044514, 3, 1044513 );
			AddCraft( typeof( StatuePegasus ), 1044503, 1044510, 70.0, 130.0, typeof( Granite ), 1044514, 4, 1044513 );

			SetSubRes( typeof( Granite ), 1044525 );

			AddSubRes( typeof( Granite ),			1044525, 00.0, 1044514, 1044526 );
			AddSubRes( typeof( DullCopperGranite ),	1044023, 65.0, 1044514, 1044527 );
			AddSubRes( typeof( ShadowIronGranite ),	1044024, 70.0, 1044514, 1044527 );
			AddSubRes( typeof( CopperGranite ),		1044025, 75.0, 1044514, 1044527 );
			AddSubRes( typeof( BronzeGranite ),		1044026, 80.0, 1044514, 1044527 );
			AddSubRes( typeof( GoldGranite ),		1044027, 85.0, 1044514, 1044527 );
			AddSubRes( typeof( AgapiteGranite ),	1044028, 90.0, 1044514, 1044527 );
			AddSubRes( typeof( VeriteGranite ),		1044029, 95.0, 1044514, 1044527 );
			AddSubRes( typeof( ValoriteGranite ),	1044030, 99.0, 1044514, 1044527 );
		}
	}
}