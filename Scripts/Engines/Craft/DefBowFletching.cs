using System;
using Midgard.Engines.OldCraftSystem;
using Midgard.Items;

using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	#region Mondain's Legacy
	public enum BowRecipes
	{		
		//magical
		BarbedLongbow 			= 200,
		SlayerLongbow			= 201,
		FrozenLongbow 			= 202,
		LongbowOfMight 			= 203,
		RangersShortbow 		= 204,
		LightweightShortbow		= 205,
		MysticalShortbow 		= 206,
		AssassinsShortbow 		= 207,
		
		// arties
		BlightGrippedLongbow 	= 250,
		FaerieFire 				= 251,
		SilvanisFeywoodBow 		= 252,
		MischiefMaker			= 253,
		TheNightReaper 			= 254,
	}
	#endregion

	public class DefBowFletching : CraftSystem
	{
		#region mod by Dies Irae
		public override string Name { get{ return "Fletching"; } }

		public override bool SupportsQuality
		{
			get { return true; }
		}

		public static void Initialize()
		{
			InitDebugItemsList();
		}

		private static Type[] m_FletcherColorables = new Type[]
			{
				typeof( Kindling ), 		typeof( Shaft ),
				typeof( Arrow ), 			typeof( Bolt ),
				typeof( FukiyaDarts ),
			};

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
			if( !type.IsSubclassOf( typeof(BaseLog) ) && !type.IsSubclassOf( typeof(BaseBoards) ) )
				return false;

			type = item.ItemType;

			bool contains = false;

			for ( int i = 0; !contains && i < m_FletcherColorables.Length; ++i )
				contains = ( m_FletcherColorables[i] == type );

			return contains;
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
                    m_CraftDefinitionTree = new CraftDefinitionTree( "BowFletching.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;
		#endregion

		public override SkillName MainSkill
		{
			get	{ return SkillName.Fletching; }
		}

		public override int GumpTitleNumber
		{
			get { return 1044006; } // <CENTER>BOWCRAFT AND FLETCHING MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefBowFletching();

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			return Core.AOS ? 0.5 : 0.0; // mod by Dies Irae
		}

		private DefBowFletching() : base( 2, 4, 1.25 ) // mod by Dies Irae // base( 1, 2, 1.7 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
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
			// no animation
			//if ( from.Body.Type == BodyType.Human && !from.Mounted )
			//	from.Animate( 33, 5, 1, true, false, 0 );

			from.PlaySound( 0x55 );
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

		public override CraftECA ECA{ get{ return CraftECA.FiftyPercentChanceMinusTenPercent; } }

		#region modifica by Dies Irae
		private static List<Type> m_DebugItemList;

		public static void InitDebugItemsList()
		{
			if( m_DebugItemList == null )
				m_DebugItemList = new List<Type>();
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

            #region Materials
            index = AddCraft( typeof( Kindling ), 1044457, 1023553, 0.0, 00.0, typeof( Log ), 1044041, 1, 1044351 );
            SetUseAllRes( index, true );

            index = AddCraft( typeof( Shaft ), 1044457, 1027124, 0.0, 40.0, typeof( Log ), 1044041, 1, 1044351 );
            SetUseAllRes( index, true );
            #endregion

            #region Ammunition
            index = AddCraft( typeof( Arrow ), 1044565, 1023903, 20.0, 40.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            SetUseAllRes( index, true );

            index = AddCraft( typeof( IncendiaryArrow ), 1044565, "incendiary arrow", 80.0, 100.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            AddRes( index, typeof( GreaterExplosionPotion ), "Greater Explosion Potion", 3, 1044253 );

            index = AddCraft( typeof( AcidArrow ), 1044565, "acid arrow", 85.0, 105.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            AddRes( index, typeof( NoxCrystal ), "Nox Crystal", 10 );

            index = AddCraft( typeof( DisarmingArrow ), 1044565, "disarming arrow", 90.0, 150.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            AddRes( index, typeof( Gears ), 1044254, 2, 1044253 );

            index = AddCraft( typeof( DismountingArrow ), 1044565, "dismounting arrow", 95.0, 110.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );
            AddRes( index, typeof( Rope ), "Rope", 1 );

            index = AddCraft( typeof( StuningArrow ), 1044565, "stuning arrow", 97.5, 107.5, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );
            AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );

            index = AddCraft( typeof( Bolt ), 1044565, 1027163, 40.0, 60.0, typeof( Shaft ), 1044560, 1, 1044561 );
            AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
            SetUseAllRes( index, true );
            #endregion

            #region Weapons
            AddCraft( typeof( Bow ), 1044566, 1025042, 30.0, 70.0, typeof( Log ), 1044041, 7, 1044351 );
            AddCraft( typeof( Crossbow ), 1044566, 1023919, 60.0, 100.0, typeof( Log ), 1044041, 7, 1044351 );
            AddCraft( typeof( HeavyCrossbow ), 1044566, 1025117, 80.0, 120.0, typeof( Log ), 1044041, 10, 1044351 );
            AddCraft( typeof( ElvenCompositeLongbow ), 1044566, 1031562, 40.0, 80.0, typeof( Log ), 1044041, 20, 1044351 );
            AddCraft( typeof( MagicalShortbow ), 1044566, 1031551, 30.0, 70.0, typeof( Log ), 1044041, 15, 1044351 );
            #endregion

            #region SubRes: Log
            SetSubRes( typeof( Log ), 1072643 );

            AddSubRes( typeof( Log ), 1064747, 00.0, 1044267, 1044267 );
            AddSubRes( typeof( OakLog ), 1064748, 20.0, 1064746, 1064745, 1.05 );
            AddSubRes( typeof( WalnutLog ), 1064749, 35.0, 1064746, 1064745, 1.10 );
            AddSubRes( typeof( OhiiLog ), 1064750, 35.0, 1064746, 1064745, 1.20 );
            AddSubRes( typeof( CedarLog ), 1064751, 40.0, 1064746, 1064745, 1.22 );
            AddSubRes( typeof( WillowLog ), 1064752, 55.0, 1064746, 1064745, 1.24 );
            AddSubRes( typeof( CypressLog ), 1064753, 80.0, 1064746, 1064745, 1.28 );
            AddSubRes( typeof( YewLog ), 1064754, 90.0, 1064746, 1064745, 1.32 );
            AddSubRes( typeof( AppleLog ), 1064755, 85.0, 1064746, 1064745, 1.36 );
            AddSubRes( typeof( PearLog ), 1064756, 90.0, 1064746, 1064745, 1.40 );
            AddSubRes( typeof( PeachLog ), 1064757, 90.0, 1064746, 1064745, 1.44 );
            AddSubRes( typeof( BananaLog ), 1064758, 95.0, 1064746, 1064745, 1.48 );
            AddSubRes( typeof( StonewoodLog ), 1064759, 100.0, 1064746, 1064745, 1.52 );
            AddSubRes( typeof( SilverLog ), 1064760, 100.0, 1064746, 1064745, 1.60 );
            AddSubRes( typeof( BloodLog ), 1064761, 100.0, 1064746, 1064745, 1.68 );
            AddSubRes( typeof( SwampLog ), 1064762, 100.0, 1064746, 1064745, 1.76 );
            AddSubRes( typeof( CrystalLog ), 1064763, 100.0, 1064746, 1064745, 1.84 );
            AddSubRes( typeof( EnchantedLog ), 1064766, 100.0, 1064746, 1064745, 2.00 );
            #endregion

            MarkOption = true;
            Repair = false;
            CanEnhance = false;
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

			int index = -1;

			// Materials
			AddCraft( typeof( Kindling ), 1044457, 1023553, 0.0, 00.0, typeof( Log ), 1044041, 1, 1044351 );

			index = AddCraft( typeof( Shaft ), 1044457, 1027124, 0.0, 40.0, typeof( Log ), 1044041, 1, 1044351 );
			SetUseAllRes( index, true );

			// Ammunition
			index = AddCraft( typeof( Arrow ), 1044565, 1023903, 0.0, 40.0, typeof( Shaft ), 1044560, 1, 1044561 );
			AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( Bolt ), 1044565, 1027163, 0.0, 40.0, typeof( Shaft ), 1044560, 1, 1044561 );
			AddRes( index, typeof( Feather ), 1044562, 1, 1044563 );
			SetUseAllRes( index, true );

			if( Core.SE )
			{
				index = AddCraft( typeof( FukiyaDarts ), 1044565, 1030246, 50.0, 90.0, typeof( Log ), 1044041, 1, 1044351 );
				SetUseAllRes( index, true );
				SetNeededExpansion( index, Expansion.SE );
			}

			// Weapons
			AddCraft( typeof( Bow ), 1044566, 1025042, 30.0, 70.0, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( Crossbow ), 1044566, 1023919, 60.0, 100.0, typeof( Log ), 1044041, 7, 1044351 );
			AddCraft( typeof( HeavyCrossbow ), 1044566, 1025117, 80.0, 120.0, typeof( Log ), 1044041, 10, 1044351 );

			if ( Core.AOS )
			{
				AddCraft( typeof( CompositeBow ), 1044566, 1029922, 70.0, 110.0, typeof( Log ), 1044041, 7, 1044351 );
				AddCraft( typeof( RepeatingCrossbow ), 1044566, 1029923, 90.0, 130.0, typeof( Log ), 1044041, 10, 1044351 );
			}

			if( Core.SE )
			{
				index = AddCraft( typeof( Yumi ), 1044566, 1030224, 90.0, 130.0, typeof( Log ), 1044041, 10, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
			}

			MarkOption = true;
			Repair = Core.AOS;
		}
	}
}