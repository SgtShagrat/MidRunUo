using System;

// using Midgard.Engines.StatueSystem;

using Server.Factions;
using Server.Items;
using Server.Targeting;
using System.Collections.Generic;

using Midgard.Engines.OldCraftSystem;
using Midgard.Items;
using Midgard.Engines.PlantSystem;

namespace Server.Engines.Craft
{
	public enum TinkerRecipes
	{
		InvisibilityPotion		= 400,
		DarkglowPotion			= 401,
		ParasiticPotion			= 402,
		
		EssenceOfBattle 		= 450,
		PendantOfTheMagi 		= 451,
		ResilientBracer 		= 452,		
		ScrappersCompendium		= 453,
		HoveringWisp			= 454,
	}

	public class DefTinkering : CraftSystem
	{
        #region mod by Dies Irae
        public override string Name { get{ return "Tinkering"; } }

        public static void Initialize()
        {
            InitDebugItemsList();
        }

        public override bool SupportOldMenu { get { return true; } }

        public override CraftDefinitionTree DefinitionTree
        {
            get
            {
                if( m_CraftDefinitionTree == null )
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Tinkering.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;

	    private static readonly Type[] m_NotLoopables = new Type[]
	    {
            typeof(GoldRing),
            typeof(SilverBeadNecklace),
            typeof(GoldNecklace),
            typeof(GoldEarrings),
            typeof(GoldBeadNecklace),
            typeof(GoldBracelet)
	    };

        public override bool SupportsAutoLoop( Type type )
        {
            bool contains = false;

            for( int i = 0; !contains && i < m_NotLoopables.Length; ++i )
                contains = ( m_NotLoopables[ i ] == type );

            return !contains;
        }

        public override void PlayCraftEffect( Mobile from, CraftItem craftItem, Type typeRes, BaseTool tool )
        {
            if( typeRes == null )
                typeRes = craftItem.Resources.GetAt( 0 ).ItemType;

            if( typeRes != null )
            {
                CraftResource res = CraftResources.GetFromType( typeRes );
                CraftResourceType type = CraftResources.GetType( res );
                switch( type )
                {
                    case CraftResourceType.Metal:
                        from.PlaySound( 0x2A );
                        break;
                    case CraftResourceType.Wood:
                        from.PlaySound( 0x23D );
                        break;
                    default:
                        from.PlaySound( 0x23D );
                        break;
                }
            }
        }

        private static readonly Type[] m_AdvTinkerColorables = new Type[]
                                                          {
                                                              typeof(CraftableFurniture),
                                                              typeof(BaseDecoLight),
                                                              typeof(CraftableTrashChest),
                                                              typeof(BaseContainer)
                                                          };

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

        public override SkillName BonusSkill { get { return SkillName.ArmsLore; } }
        #endregion
		
		#region Mondain's Legacy
		public override CraftECA ECA{ get{ return Core.AOS ? CraftECA.ChanceMinusSixtyToFourtyFive : CraftECA.ZeroToFourPercentPlusBonus; } } // mod by Dies Irae
		#endregion
		
		public override SkillName MainSkill
		{
			get	{ return SkillName.Tinkering; }
		}

		public override int GumpTitleNumber
		{
			get { return 1044007; } // <CENTER>TINKERING MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefTinkering();

				return m_CraftSystem;
			}
		}

		private DefTinkering() : base( 2, 4, 1.25 ) // base( 2, 4, 1.25 ) // mod by Dies Irae 
		{
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			if ( item.NameNumber == 1044258 || item.NameNumber == 1046445 ) // potion keg and faction trap removal kit
				return 0.5; // 50%

			return 0.0; // 0%
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.
			else if ( itemType != null && ( itemType.IsSubclassOf( typeof( BaseFactionTrapDeed ) ) || itemType == typeof( FactionTrapRemovalKit ) ) && Faction.Find( from ) == null )
				return 1044573; // You have to be in a faction to do that.

			#region modifica By Dies Irae
			if( from.AccessLevel == AccessLevel.Player && m_DebugItemList.Contains( itemType ) )
				return 1064915; // This item is only available to Midgard Staff members. They will decide soon if has to be implemented or removed.
			#endregion

			return 0;
		}

		public override void PlayCraftEffect( Mobile from )
		{
			// no sound
            //from.PlaySound( 0x241 );

            #region mod by Dies Irae
            if( !Core.AOS )
                from.PlaySound( 0x241 );
            #endregion
        }

		private static Type[] m_TinkerColorables = new Type[]
			{
				typeof( ForkLeft ), 		typeof( ForkRight ),
				typeof( SpoonLeft ), 		typeof( SpoonRight ),
				typeof( KnifeLeft ), 		typeof( KnifeRight ),
				typeof( Plate ),
				typeof( Goblet ), 			typeof( PewterMug ),
				typeof( KeyRing ),
				typeof( Candelabra ), 		typeof( Scales ),
				typeof( Key ), 				typeof( Globe ),
				typeof( Spyglass ), 		typeof( Lantern ),
				typeof( HeatingStand )
			};

		public override bool RetainsColorFrom( CraftItem item, Type type )
		{
            /*
			if ( !type.IsSubclassOf( typeof( BaseIngot ) ) )
				return false;
            */

            #region mod by Dies Irae
            if( !Core.AOS )
                return true;
            #endregion

			type = item.ItemType;

			bool contains = false;

			for ( int i = 0; !contains && i < m_TinkerColorables.Length; ++i )
				contains = ( m_TinkerColorables[i] == type );

		    #region mod by Dies Irae
		    for( int i = 0; !contains && i < m_AdvTinkerColorables.Length; ++i )
		        contains = ( m_AdvTinkerColorables[ i ] == type );
		    #endregion

			return contains;
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

		public override bool ConsumeOnFailure( Mobile from, Type resourceType, CraftItem craftItem )
		{
			if ( resourceType == typeof( Silver ) )
				return false;

			return base.ConsumeOnFailure( from, resourceType, craftItem );
		}

		public void AddJewelrySet( GemType gemType, Type itemType )
		{
			int offset = (int)gemType - 1;

			int index = AddCraft( typeof( GoldRing ), 1044049, 1044176 + offset, 40.0, 90.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

			index = AddCraft( typeof( SilverBeadNecklace ), 1044049, 1044185 + offset, 40.0, 90.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

			index = AddCraft( typeof( GoldNecklace ), 1044049, 1044194 + offset, 40.0, 90.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

			index = AddCraft( typeof( GoldEarrings ), 1044049, 1044203 + offset, 40.0, 90.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

			index = AddCraft( typeof( GoldBeadNecklace ), 1044049, 1044212 + offset, 40.0, 90.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

			index = AddCraft( typeof( GoldBracelet ), 1044049, 1044221 + offset, 40.0, 90.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddRes( index, itemType, 1044231 + offset, 1, 1044240 );
        }

        #region mod by Dies Irae
        private static readonly double[] m_GemDifficulty = new double[]
        {
            1.0, // None,
            1.5, // StarSapphire,
            1.3, // Emerald,
            1.3, // Sapphire,
            1.3, // Ruby,
            1.1, // Citrine,
            1.2, // Amethyst,
            1.1, // Tourmaline,
            1.0, // Amber,
            1.5, // Diamond                                            
	    };

        public static double GetGemDifficulty( GemType gemType )
        {
            return ( m_GemDifficulty[ (int)gemType ] );
        }

        public void AddOldJewelrySet( GemType gemType, Type itemType )
        {
            double diff = GetGemDifficulty( gemType );

            int offset = (int)gemType - 1;

            int index = AddCraft( typeof( GoldRing ), 1044049, 1044176 + offset, 40.0 * diff * 1.0, 60.0 * diff * 1.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

            index = AddCraft( typeof( SilverBeadNecklace ), 1044049, 1044185 + offset, 40.0 * diff * 1.1, 60.0 * diff * 1.1, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

            index = AddCraft( typeof( GoldNecklace ), 1044049, 1044194 + offset, 40.0 * diff * 1.05, 60.0 * diff * 1.05, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

            index = AddCraft( typeof( GoldEarrings ), 1044049, 1044203 + offset, 40.0 * diff * 1.1, 60.0 * diff * 1.1, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

            index = AddCraft( typeof( GoldBeadNecklace ), 1044049, 1044212 + offset, 40.0 * diff * 1.1, 60.0 * diff * 1.1, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddRes( index, itemType, 1044231 + offset, 1, 1044240 );

            index = AddCraft( typeof( GoldBracelet ), 1044049, 1044221 + offset, 40.0 * diff * 1.25, 60.0 * diff * 1.25, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddRes( index, itemType, 1044231 + offset, 1, 1044240 );
        }

        public void InitOldCraftList()
        {
            int index = -1;

            #region Wooden Items
            index = AddCraft( typeof( JointingPlane ), 1044042, 1024144, 0.0, 50.0, typeof( Log ), 1044041, 4, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( MouldingPlane ), 1044042, 1024140, 0.0, 50.0, typeof( Log ), 1044041, 4, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( SmoothingPlane ), 1044042, 1024146, 0.0, 50.0, typeof( Log ), 1044041, 4, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( ClockFrame ), 1044042, 1024173, 0.0, 50.0, typeof( Log ), 1044041, 6, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( Axle ), 1044042, 1024187, -25.0, 25.0, typeof( Log ), 1044041, 2, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( RollingPin ), 1044042, 1024163, 0.0, 50.0, typeof( Log ), 1044041, 5, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( ShipModel ), 1044042, "ship model", 0.0, 50.0, typeof( Log ), 1044041, 5, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( WigStand ), 1044042, "wig stand", 0.0, 50.0, typeof( Log ), 1044041, 5, 1044351 );
            SetUseSubRes2( index, true );

            index = AddCraft( typeof( Pipe ), 1044042, "pipe", 30.0, 50.0, typeof( Log ), 1044041, 1, 1044351 );
            SetUseSubRes2( index, true );

            AddCraft( typeof( BakersBoard ), 1044042, "baker's board", 60.0, 80.0, typeof( Log ), 1044041, 5, 1044351 );
            SetUseSubRes2( index, true );
            #endregion

            #region Tools
            AddCraft( typeof( Scissors ), 1044046, 1023998, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( MortarPestle ), 1044046, 1023739, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
            AddCraft( typeof( Scorp ), 1044046, 1024327, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( TinkerTools ), 1044046, 1044164, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( Hatchet ), 1044046, 1023907, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( DrawKnife ), 1044046, 1024324, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( SewingKit ), 1044046, 1023997, 10.0, 70.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( Saw ), 1044046, 1024148, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( DovetailSaw ), 1044046, 1024136, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Froe ), 1044046, 1024325, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( Shovel ), 1044046, 1023898, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Hammer ), 1044046, 1024138, 30.0, 80.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( Tongs ), 1044046, 1024028, 35.0, 85.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( SmithHammer ), 1044046, 1025091, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( SledgeHammer ), 1044046, 1024021, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Inshave ), 1044046, 1024326, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( Pickaxe ), 1044046, 1023718, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Lockpick ), 1044046, 1025371, 45.0, 95.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( Skillet ), 1044046, 1044567, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( FlourSifter ), 1044046, 1024158, 50.0, 100.0, typeof( IronIngot ), 1044036, 3, 1044037 );
            AddCraft( typeof( FletcherTools ), 1044046, 1044166, 35.0, 85.0, typeof( IronIngot ), 1044036, 3, 1044037 );
            AddCraft( typeof( MapmakersPen ), 1044046, 1044167, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( ScribesPen ), 1044046, 1044168, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );

            index = AddCraft( typeof( MetalContainerEngraver ), 1044046, 1072154, 75.0, 100.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );
            AddRes( index, typeof( Gears ), 1044254, 2, 1044253 );
            AddRes( index, typeof( Diamond ), 1062608, 1, 1044240 );

            AddCraft( typeof( StoneHarvester ), 1044046, "stone harvester", 60.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Blowpipe ), 1044046, 1044608, 60.0, 90.0, typeof( IronIngot ), 1044036, 6, 1044037 );
            AddCraft( typeof( MalletAndChisel ), 1044046, "mallet and chisel", 60.0, 80.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddCraft( typeof( GardeningShovel ), 1044046, "gardening shovel", 70.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            // AddCraft( typeof( StoneEngraver ), 1044046, "stone engraver", 90.0, 100.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            #endregion

            #region Parts
            AddCraft( typeof( Gears ), 1044047, 1024179, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( ClockParts ), 1044047, 1024175, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( BarrelTap ), 1044047, 1024100, 35.0, 85.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( Springs ), 1044047, 1024189, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( SextantParts ), 1044047, 1024185, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( BarrelHoops ), 1044047, 1024321, -15.0, 35.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddCraft( typeof( Hinge ), 1044047, 1024181, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            #endregion

            #region Utensils
            AddCraft( typeof( ButcherKnife ), 1044048, 1025110, 25.0, 75.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( SpoonLeft ), 1044048, 1044158, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( SpoonRight ), 1044048, 1044159, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( Plate ), 1044048, 1022519, 0.0, 50.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( ForkLeft ), 1044048, 1044160, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( ForkRight ), 1044048, 1044161, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( Cleaver ), 1044048, 1023778, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
            AddCraft( typeof( KnifeLeft ), 1044048, 1044162, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( KnifeRight ), 1044048, 1044163, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
            AddCraft( typeof( Goblet ), 1044048, 1022458, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( PewterMug ), 1044048, 1024097, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( SkinningKnife ), 1044048, 1023781, 25.0, 75.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( CooksCauldron ), 1044048, "cooks cauldron", 25.0, 75.0, typeof( IronIngot ), 1044036, 50, 1044037 );
            AddCraft( typeof( FryingPan ), 1044048, "frying pan", 25.0, 75.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddCraft( typeof( Pot ), 1044048, "pot", 25.0, 75.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddCraft( typeof( Pan ), 1044048, "pan", 25.0, 75.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Kettle ), 1044048, "kettle", 25.0, 75.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            #endregion

            #region Misc
            AddCraft( typeof( KeyRing ), 1044050, 1024113, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( Candelabra ), 1044050, 1022599, 55.0, 105.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Scales ), 1044050, 1026225, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Key ), 1044050, 1024112, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
            AddCraft( typeof( Globe ), 1044050, 1024167, 55.0, 105.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Spyglass ), 1044050, 1025365, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddCraft( typeof( Lantern ), 1044050, 1022597, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
            AddCraft( typeof( HeatingStand ), 1044050, 1026217, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );

            AddCraft( typeof( Monocle ), 1044050, "monocle", 80.0, 130.0, typeof( IronIngot ), 1044036, 3, 1044037 );
            AddRes( index, typeof( Sand ), 1044625, 1, 1044627 );
            AddCraft( typeof( TraditionalGlasses ), 1044050, "glasses", 80.0, 130.0, typeof( IronIngot ), 1044036, 4, 1044037 );
            AddRes( index, typeof( Sand ), 1044625, 1, 1044627 );

            AddCraft( typeof( CraftableWire ), 1065130, 1065131, 50.0, 100.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddCraft( typeof( MetalChest ), 1065130, 1064970, 75.0, 120.0, typeof( IronIngot ), 1044036, 30, 1044037 );
            AddCraft( typeof( Hook ), 1065130, "hook", 50.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );

            index = AddCraft( typeof( MetalGoldenChest ), 1065130, 1064971, 85.0, 120.0, typeof( IronIngot ), 1044036, 30, 1044037 );
            AddRes( index, typeof( GoldIngot ), 1065162, 8 );

            index = AddCraft( typeof( CraftableTrashChest ), 1065130, 1065136, 85.0, 130.0, typeof( IronIngot ), 1044036, 30, 1044037 );
            AddRes( index, typeof( GoldIngot ), 1065162, 5 );

            index = AddCraft( typeof( HitchingPostDeed ), 1065130, 1025351, 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddSkill( index, SkillName.Tailoring, 30.0, 40.0 );
            AddRes( index, typeof( Leather ), 1044462, 3, 1044463 );
            #endregion

            #region Jewelry
            AddOldJewelrySet( GemType.StarSapphire, typeof( StarSapphire ) );
            AddOldJewelrySet( GemType.Emerald, typeof( Emerald ) );
            AddOldJewelrySet( GemType.Sapphire, typeof( Sapphire ) );
            AddOldJewelrySet( GemType.Ruby, typeof( Ruby ) );
            AddOldJewelrySet( GemType.Citrine, typeof( Citrine ) );
            AddOldJewelrySet( GemType.Amethyst, typeof( Amethyst ) );
            AddOldJewelrySet( GemType.Tourmaline, typeof( Tourmaline ) );
            AddOldJewelrySet( GemType.Amber, typeof( Amber ) );
            AddOldJewelrySet( GemType.Diamond, typeof( Diamond ) );

            AddCraft( typeof( HeavyBracelet ), 1044049, "heavy bracelet", 50.0, 100.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            #endregion

            #region Multi-Component Items
            index = AddCraft( typeof( AxleGears ), 1044051, 1024177, 0.0, 0.0, typeof( Axle ), 1044169, 1, 1044253 );
            AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );

            index = AddCraft( typeof( ClockParts ), 1044051, 1024175, 0.0, 0.0, typeof( AxleGears ), 1044170, 1, 1044253 );
            AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );

            index = AddCraft( typeof( SextantParts ), 1044051, 1024185, 0.0, 0.0, typeof( AxleGears ), 1044170, 1, 1044253 );
            AddRes( index, typeof( Hinge ), 1044172, 1, 1044253 );

            index = AddCraft( typeof( ClockRight ), 1044051, 1044257, 0.0, 0.0, typeof( ClockFrame ), 1044174, 1, 1044253 );
            AddRes( index, typeof( ClockParts ), 1044173, 1, 1044253 );

            index = AddCraft( typeof( ClockLeft ), 1044051, 1044256, 0.0, 0.0, typeof( ClockFrame ), 1044174, 1, 1044253 );
            AddRes( index, typeof( ClockParts ), 1044173, 1, 1044253 );

            AddCraft( typeof( Sextant ), 1044051, 1024183, 0.0, 0.0, typeof( SextantParts ), 1044175, 1, 1044253 );

            index = AddCraft( typeof( Bola ), 1044051, 1046441, 60.0, 80.0, typeof( BolaBall ), 1046440, 4, 1042613 );
            AddRes( index, typeof( Leather ), 1044462, 3, 1044463 );

            index = AddCraft( typeof( PotionKeg ), 1044051, 1044258, 75.0, 100.0, typeof( Keg ), 1044255, 1, 1044253 );
            AddRes( index, typeof( Bottle ), 1044250, 10, 1044253 );
            AddRes( index, typeof( BarrelLid ), 1044251, 1, 1044253 );
            AddRes( index, typeof( BarrelTap ), 1044252, 1, 1044253 );
            #endregion

            #region Decorative Shield
            AddCraft( typeof( CraftableDecorativeShield1 ), 1065099, 1065100, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield2 ), 1065099, 1065101, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield3 ), 1065099, 1065102, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield4 ), 1065099, 1065103, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield5 ), 1065099, 1065104, 80.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield6 ), 1065099, 1065105, 80.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield7 ), 1065099, 1065106, 80.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield8 ), 1065099, 1065107, 80.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield9 ), 1065099, 1065108, 80.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield10 ), 1065099, 1065109, 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShield11 ), 1065099, 1065110, 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddCraft( typeof( CraftableDecorativeShieldSword1North ), 1065099, 1065111, 85.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
            AddCraft( typeof( CraftableDecorativeShieldSword1West ), 1065099, 1065112, 85.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
            AddCraft( typeof( CraftableDecorativeShieldSword2North ), 1065099, 1065113, 85.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
            AddCraft( typeof( CraftableDecorativeShieldSword2West ), 1065099, 1065114, 85.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
            #endregion

            #region Lights
            AddCraft( typeof( CraftableBrazier ), 1065075, 1065076, 50.0, 100.0, typeof( IronIngot ), 1044036, 7, 1044037 );
            AddCraft( typeof( CraftableBrazierTall ), 1065075, 1065077, 65.0, 115.0, typeof( IronIngot ), 1044036, 12, 1044037 );

            index = AddCraft( typeof( CraftableCandelabra ), 1065075, 1065078, 50.0, 100.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 3, 1065315 );

            index = AddCraft( typeof( CraftableCandelabraStand ), 1065075, 1065079, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 3, 1065315 );

            index = AddCraft( typeof( CraftableLampPost1 ), 1065075, 1065080, 85.0, 130.0, typeof( IronIngot ), 1044036, 150, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 1, 1065315 );

            index = AddCraft( typeof( CraftableLampPost2 ), 1065075, 1065081, 85.0, 130.0, typeof( IronIngot ), 1044036, 150, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 1, 1065315 );

            index = AddCraft( typeof( CraftableLampPost3 ), 1065075, 1065082, 85.0, 130.0, typeof( IronIngot ), 1044036, 150, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 1, 1065315 );

            AddCraft( typeof( CraftableWallTorch ), 1065075, 1065083, 30.0, 80.0, typeof( IronIngot ), 1044036, 3, 1044037 );

            index = AddCraft( typeof( CraftableWallSconce ), 1065075, 1065084, 30.0, 80.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 1, 1065315 );

            index = AddCraft( typeof( FineHangingLantern ), 1065075, 1065085, 85.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
            AddRes( index, typeof( BlankCandle ), 1065314, 1, 1065315 );

            AddCraft( typeof( OilLamp ), 1065075, 1065086, 65.0, 115.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            #endregion

            #region Games
            index = AddCraft( typeof( CheckerBoard ), 1065026, 1065027, 65.0, 115.0, typeof( Log ), 1044041, 5, 1044351 );
            AddRes( index, typeof( DeathLog ), 1065459, 2 );
            AddRes( index, typeof( PeachLog ), 1065449, 2 );

            index = AddCraft( typeof( Chessboard ), 1065026, 1065028, 65.0, 115.0, typeof( Log ), 1044041, 5, 1044351 );
            AddRes( index, typeof( DeathLog ), 1065459, 2 );
            AddRes( index, typeof( PeachLog ), 1065449, 2 );

            AddCraft( typeof( Dices ), 1065026, 1065029, 35.0, 50.0, typeof( Log ), 1044041, 2, 1044351 );

            index = AddCraft( typeof( Mahjong.MahjongGame ), 1065026, 1065030, 95.0, 130.0, typeof( Log ), 1044041, 5, 1044351 );
            AddRes( index, typeof( DeathLog ), 1065459, 5 );
            AddRes( index, typeof( PeachLog ), 1065449, 5 );
            #endregion

            #region Traps
            index = AddCraft( typeof( TrapRemovalKit ), 1044052, "trap removal kit", 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( Log ), 1044041, 8, 1044351 );

            index = AddCraft( typeof( LightExplosionTrapDeed ), 1044052, "light explosion trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( LesserExplosionPotion ), "Lesser Explosion Potion", 5, 1044253 );

            index = AddCraft( typeof( MediumExplosionTrapDeed ), 1044052, "light explosion trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( ExplosionPotion ), "Explosion Potion", 10, 1044253 );

            index = AddCraft( typeof( HeavyExplosionTrapDeed ), 1044052, "light explosion trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( GreaterExplosionPotion ), "Greater Explosion Potion", 10, 1044253 );

            index = AddCraft( typeof( LightGasTrapDeed ), 1044052, "light gas trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( LesserPoisonPotion ), "Lesser Poison Potion", 5, 1044253 );

            index = AddCraft( typeof( MediumGasTrapDeed ), 1044052, "medium gas trap deed", 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( PoisonPotion ), "Poison Potion", 10, 1044253 );

            index = AddCraft( typeof( HeavyGasTrapDeed ), 1044052, "heavy gas trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( GreaterPoisonPotion ), "Greater Poison Potion", 10, 1044253 );

            index = AddCraft( typeof( LightSawTrapDeed ), 1044052, "light saw trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );

            index = AddCraft( typeof( MediumSawTrapDeed ), 1044052, "medium saw trap deed", 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( Gears ), 1044254, 5, 1044253 );

            index = AddCraft( typeof( HeavySawTrapDeed ), 1044052, "heavy saw trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( Gears ), 1044254, 10, 1044253 );

            index = AddCraft( typeof( LightSpikeTrapDeed ), 1044052, "light spike trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );

            index = AddCraft( typeof( MediumSpikeTrapDeed ), 1044052, "medium spike trap deed", 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 5, 1044253 );

            index = AddCraft( typeof( HeavySpikeTrapDeed ), 1044052, "heavy spike trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 10, 1044253 );

            index = AddCraft( typeof( LightDismountTrapDeed ), 1044052, "light dismount trap deed", 65.0, 115.0, typeof( IronIngot ), 1044036, 5, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );
            AddRes( index, typeof( Rope ), "Rope", 1 );

            index = AddCraft( typeof( MediumDismountTrapDeed ), 1044052, "medium dismount trap deed", 85.0, 130.0, typeof( IronIngot ), 1044036, 10, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 5, 1044253 );
            AddRes( index, typeof( Rope ), "Rope", 1 );

            index = AddCraft( typeof( HeavyDismountTrapDeed ), 1044052, "heavy dismount trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 10, 1044253 );
            AddRes( index, typeof( Rope ), "Rope", 1 );

            index = AddCraft( typeof( ExplosionTrapDeed ), 1044052, "explosion trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( ExplosionPotion ), "Explosion Potion", 10, 1044253 );
            AddRes( index, typeof( Springs ), 1044171, 5, 1044253 );

            index = AddCraft( typeof( DartTrapDeed ), 1044052, "dart trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );
            AddRes( index, typeof( Springs ), 1044171, 5, 1044253 );

            index = AddCraft( typeof( PoisonTrapDeed ), 1044052, "poison trap deed", 95.0, 130.0, typeof( IronIngot ), 1044036, 20, 1044037 );
            AddRes( index, typeof( Springs ), 1044171, 10, 1044253 );
            AddRes( index, typeof( GreaterPoisonPotion ), "Greater Poison Potion", 10, 1044253 );
            #endregion

            #region SubRes: IronIngot
            SetSubRes( typeof( IronIngot ), 1044022 ); // Set the overridable material

            AddSubRes( typeof( IronIngot ), 1044022, 00.0, 1044036, 1044267 );

            AddSubRes( typeof( DullCopperIngot ), 1044023, 50.0, 1044036, 1044268, 1.10 );
            AddSubRes( typeof( ShadowIronIngot ), 1044024, 50.0, 1044036, 1044268, 1.12 );
            AddSubRes( typeof( BronzeIngot ), 1066401, 55.0, 1044036, 1044268, 1.14 );
            AddSubRes( typeof( CopperIngot ), 1066402, 55.0, 1044036, 1044268, 1.16 );
            AddSubRes( typeof( GraphiteIngot ), 1066403, 60.0, 1044036, 1044268, 1.18 );
            AddSubRes( typeof( AgapiteIngot ), 1066404, 60.0, 1044036, 1044268, 1.20 );
            AddSubRes( typeof( VeriteIngot ), 1044029, 65.0, 1044036, 1044268, 1.22 );
            AddSubRes( typeof( ValoriteIngot ), 1044030, 65.0, 1044036, 1044268, 1.24 );

            AddSubRes( typeof( PyriteIngot ), 1066405, 70.0, 1044036, 1044268, 1.28 );
            AddSubRes( typeof( AzuriteIngot ), 1066406, 70.0, 1044036, 1044268, 1.32 );
            AddSubRes( typeof( VanadiumIngot ), 1066407, 75.0, 1044036, 1044268, 1.36 );
            AddSubRes( typeof( SilverIngot ), 1066408, 80.0, 1044036, 1044268, 1.40 );
            AddSubRes( typeof( PlatinumIngot ), 1066409, 80.0, 1044036, 1044268, 1.44 );
            AddSubRes( typeof( AmethystIngot ), 1066410, 85.0, 1044036, 1044268, 1.48 );
            AddSubRes( typeof( TitaniumIngot ), 1066411, 85.0, 1044036, 1044268, 1.52 );

            AddSubRes( typeof( GoldIngot ), 1066412, 90.0, 1044036, 1044268, 1.60 );
            AddSubRes( typeof( XenianIngot ), 1066413, 90.0, 1044036, 1044268, 1.68 );
            AddSubRes( typeof( RubidianIngot ), 1066414, 92.5, 1044036, 1044268, 1.76 );
            AddSubRes( typeof( ObsidianIngot ), 1066415, 95.0, 1044036, 1044268, 1.84 );

            AddSubRes( typeof( EbonSapphireIngot ), 1066416, 97.5, 1044036, 1044268, 2.00 );
            AddSubRes( typeof( DarkRubyIngot ), 1066417, 99.0, 1044036, 1044268, 2.00 );
            AddSubRes( typeof( RadiantDiamondIngot ), 1066418, 99.9, 1044036, 1044268, 2.00 );
            #endregion

            #region SubRes: Log
            SetSubRes2( typeof( Log ), 1064747 );

            AddSubRes2( typeof( Log ), 1064747, 00.0, 1044267, 1044267 );
            AddSubRes2( typeof( OakLog ), 1064748, 20.0, 1064746, 1064745, 1.05 );
            AddSubRes2( typeof( WalnutLog ), 1064749, 35.0, 1064746, 1064745, 1.10 );
            AddSubRes2( typeof( OhiiLog ), 1064750, 35.0, 1064746, 1064745, 1.20 );
            AddSubRes2( typeof( CedarLog ), 1064751, 40.0, 1064746, 1064745, 1.22 );
            AddSubRes2( typeof( WillowLog ), 1064752, 55.0, 1064746, 1064745, 1.24 );
            AddSubRes2( typeof( CypressLog ), 1064753, 80.0, 1064746, 1064745, 1.28 );
            AddSubRes2( typeof( YewLog ), 1064754, 90.0, 1064746, 1064745, 1.32 );
            AddSubRes2( typeof( AppleLog ), 1064755, 85.0, 1064746, 1064745, 1.36 );
            AddSubRes2( typeof( PearLog ), 1064756, 90.0, 1064746, 1064745, 1.40 );
            AddSubRes2( typeof( PeachLog ), 1064757, 90.0, 1064746, 1064745, 1.44 );
            AddSubRes2( typeof( BananaLog ), 1064758, 95.0, 1064746, 1064745, 1.48 );
            AddSubRes2( typeof( StonewoodLog ), 1064759, 100.0, 1064746, 1064745, 1.52 );
            AddSubRes2( typeof( SilverLog ), 1064760, 100.0, 1064746, 1064745, 1.60 );
            AddSubRes2( typeof( BloodLog ), 1064761, 100.0, 1064746, 1064745, 1.68 );
            AddSubRes2( typeof( SwampLog ), 1064762, 100.0, 1064746, 1064745, 1.76 );
            AddSubRes2( typeof( CrystalLog ), 1064763, 100.0, 1064746, 1064745, 1.84 );
            AddSubRes2( typeof( EnchantedLog ), 1064766, 100.0, 1064746, 1064745, 2.00 );
            #endregion

            MarkOption = true;
            Repair = true;
            CanEnhance = false;
        }
        #endregion

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

			#region Wooden Items
			AddCraft( typeof( JointingPlane ), 1044042, 1024144, 0.0, 50.0, typeof( Log ), 1044041, 4, 1044351 );
			AddCraft( typeof( MouldingPlane ), 1044042, 1024140, 0.0, 50.0, typeof( Log ), 1044041, 4, 1044351 );
			AddCraft( typeof( SmoothingPlane ), 1044042, 1024146, 0.0, 50.0, typeof( Log ), 1044041, 4, 1044351 );
			AddCraft( typeof( ClockFrame ), 1044042, 1024173, 0.0, 50.0, typeof( Log ), 1044041, 6, 1044351 );
			AddCraft( typeof( Axle ), 1044042, 1024187, -25.0, 25.0, typeof( Log ), 1044041, 2, 1044351 );
			AddCraft( typeof( RollingPin ), 1044042, 1024163, 0.0, 50.0, typeof( Log ), 1044041, 5, 1044351 );

			if( Core.SE )
			{
				index = AddCraft( typeof( Nunchaku ), 1044042, 1030158, 70.0, 120.0, typeof( IronIngot ), 1044036, 3, 1044037 );
				AddRes( index, typeof( Log ), 1044041, 8, 1044351 );
				SetNeededExpansion( index, Expansion.SE );
			}
			#endregion

			#region Tools
			AddCraft( typeof( Scissors ), 1044046, 1023998, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( MortarPestle ), 1044046, 1023739, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( Scorp ), 1044046, 1024327, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( TinkerTools ), 1044046, 1044164, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Hatchet ), 1044046, 1023907, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( DrawKnife ), 1044046, 1024324, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SewingKit ), 1044046, 1023997, 10.0, 70.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Saw ), 1044046, 1024148, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( DovetailSaw ), 1044046, 1024136, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Froe ), 1044046, 1024325, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Shovel ), 1044046, 1023898, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Hammer ), 1044046, 1024138, 30.0, 80.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Tongs ), 1044046, 1024028, 35.0, 85.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( SmithHammer ), 1044046, 1025091, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( SledgeHammer ), 1044046, 1024021, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Inshave ), 1044046, 1024326, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Pickaxe ), 1044046, 1023718, 40.0, 90.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Lockpick ), 1044046, 1025371, 45.0, 95.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Skillet ), 1044046, 1044567, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( FlourSifter ), 1044046, 1024158, 50.0, 100.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( FletcherTools ), 1044046, 1044166, 35.0, 85.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( MapmakersPen ), 1044046, 1044167, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( ScribesPen ), 1044046, 1044168, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			#endregion

			#region Parts
			AddCraft( typeof( Gears ), 1044047, 1024179, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( ClockParts ), 1044047, 1024175, 25.0, 75.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( BarrelTap ), 1044047, 1024100, 35.0, 85.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Springs ), 1044047, 1024189, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SextantParts ), 1044047, 1024185, 30.0, 80.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( BarrelHoops ), 1044047, 1024321, -15.0, 35.0, typeof( IronIngot ), 1044036, 5, 1044037 );
			AddCraft( typeof( Hinge ), 1044047, 1024181, 5.0, 55.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( BolaBall ), 1044047, 1023699, 45.0, 95.0, typeof( IronIngot ), 1044036, 10, 1044037 );
			#endregion

			#region Utensils
			AddCraft( typeof( ButcherKnife ), 1044048, 1025110, 25.0, 75.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SpoonLeft ), 1044048, 1044158, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( SpoonRight ), 1044048, 1044159, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Plate ), 1044048, 1022519, 0.0, 50.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( ForkLeft ), 1044048, 1044160, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( ForkRight ), 1044048, 1044161, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Cleaver ), 1044048, 1023778, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( KnifeLeft ), 1044048, 1044162, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( KnifeRight ), 1044048, 1044163, 0.0, 50.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddCraft( typeof( Goblet ), 1044048, 1022458, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( PewterMug ), 1044048, 1024097, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( SkinningKnife ), 1044048, 1023781, 25.0, 75.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			#endregion

			#region Misc
			AddCraft( typeof( KeyRing ), 1044050, 1024113, 10.0, 60.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( Candelabra ), 1044050, 1022599, 55.0, 105.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Scales ), 1044050, 1026225, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Key ), 1044050, 1024112, 20.0, 70.0, typeof( IronIngot ), 1044036, 3, 1044037 );
			AddCraft( typeof( Globe ), 1044050, 1024167, 55.0, 105.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Spyglass ), 1044050, 1025365, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );
			AddCraft( typeof( Lantern ), 1044050, 1022597, 30.0, 80.0, typeof( IronIngot ), 1044036, 2, 1044037 );
			AddCraft( typeof( HeatingStand ), 1044050, 1026217, 60.0, 110.0, typeof( IronIngot ), 1044036, 4, 1044037 );

			if ( Core.SE )
			{
				index = AddCraft( typeof( ShojiLantern ), 1044050, 1029404, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
				AddRes( index, typeof( Log ), 1044041, 5, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( PaperLantern ), 1044050, 1029406, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
				AddRes( index, typeof( Log ), 1044041, 5, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( RoundPaperLantern ), 1044050, 1029418, 65.0, 115.0, typeof( IronIngot ), 1044036, 10, 1044037 );
				AddRes( index, typeof( Log ), 1044041, 5, 1044351 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( WindChimes ), 1044050, 1030290, 80.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( FancyWindChimes ), 1044050, 1030291, 80.0, 130.0, typeof( IronIngot ), 1044036, 15, 1044037 );
				SetNeededExpansion( index, Expansion.SE );

			}
			#endregion

			#region Jewelry
			AddJewelrySet( GemType.StarSapphire, typeof( StarSapphire ) );
			AddJewelrySet( GemType.Emerald, typeof( Emerald ) );
			AddJewelrySet( GemType.Sapphire, typeof( Sapphire ) );
			AddJewelrySet( GemType.Ruby, typeof( Ruby ) );
			AddJewelrySet( GemType.Citrine, typeof( Citrine ) );
			AddJewelrySet( GemType.Amethyst, typeof( Amethyst ) );
			AddJewelrySet( GemType.Tourmaline, typeof( Tourmaline ) );
			AddJewelrySet( GemType.Amber, typeof( Amber ) );
			AddJewelrySet( GemType.Diamond, typeof( Diamond ) );
			#endregion

			#region Multi-Component Items
			index = AddCraft( typeof( AxleGears ), 1044051, 1024177, 0.0, 0.0, typeof( Axle ), 1044169, 1, 1044253 );
			AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );

			index = AddCraft( typeof( ClockParts ), 1044051, 1024175, 0.0, 0.0, typeof( AxleGears ), 1044170, 1, 1044253 );
			AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );

			index = AddCraft( typeof( SextantParts ), 1044051, 1024185, 0.0, 0.0, typeof( AxleGears ), 1044170, 1, 1044253 );
			AddRes( index, typeof( Hinge ), 1044172, 1, 1044253 );

			index = AddCraft( typeof( ClockRight ), 1044051, 1044257, 0.0, 0.0, typeof( ClockFrame ), 1044174, 1, 1044253 );
			AddRes( index, typeof( ClockParts ), 1044173, 1, 1044253 );

			index = AddCraft( typeof( ClockLeft ), 1044051, 1044256, 0.0, 0.0, typeof( ClockFrame ), 1044174, 1, 1044253 );
			AddRes( index, typeof( ClockParts ), 1044173, 1, 1044253 );

			AddCraft( typeof( Sextant ), 1044051, 1024183, 0.0, 0.0, typeof( SextantParts ), 1044175, 1, 1044253 );

			index = AddCraft( typeof( Bola ), 1044051, 1046441, 60.0, 80.0, typeof( BolaBall ), 1046440, 4, 1042613 );
			AddRes( index, typeof( Leather ), 1044462, 3, 1044463 );

			index = AddCraft( typeof( PotionKeg ), 1044051, 1044258, 75.0, 100.0, typeof( Keg ), 1044255, 1, 1044253 );
			AddRes( index, typeof( Bottle ), 1044250, 10, 1044253 );
			AddRes( index, typeof( BarrelLid ), 1044251, 1, 1044253 );
			AddRes( index, typeof( BarrelTap ), 1044252, 1, 1044253 );
			
			
			#endregion

			#region Traps
			// Dart Trap
			index = AddCraft( typeof( DartTrapCraft ), 1044052, 1024396, 30.0, 80.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddRes( index, typeof( Bolt ), 1044570, 1, 1044253 );

			// Poison Trap
			index = AddCraft( typeof( PoisonTrapCraft ), 1044052, 1044593, 30.0, 80.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddRes( index, typeof( BasePoisonPotion ), 1044571, 1, 1044253 );

			// Explosion Trap
			index = AddCraft( typeof( ExplosionTrapCraft ), 1044052, 1044597, 55.0, 105.0, typeof( IronIngot ), 1044036, 1, 1044037 );
			AddRes( index, typeof( BaseExplosionPotion ), 1044569, 1, 1044253 );

			// Faction Gas Trap
			index = AddCraft( typeof( FactionGasTrapDeed ), 1044052, 1044598, 65.0, 115.0, typeof( Silver ), 1044572, Core.AOS ? 250 : 1000, 1044253 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddRes( index, typeof( BasePoisonPotion ), 1044571, 1, 1044253 );

			// Faction explosion Trap
			index = AddCraft( typeof( FactionExplosionTrapDeed ), 1044052, 1044599, 65.0, 115.0, typeof( Silver ), 1044572, Core.AOS ? 250 : 1000, 1044253 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddRes( index, typeof( BaseExplosionPotion ), 1044569, 1, 1044253 );

			// Faction Saw Trap
			index = AddCraft( typeof( FactionSawTrapDeed ), 1044052, 1044600, 65.0, 115.0, typeof( Silver ), 1044572, Core.AOS ? 250 : 1000, 1044253 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddRes( index, typeof( Gears ), 1044254, 1, 1044253 );

			// Faction Spike Trap			
			index = AddCraft( typeof( FactionSpikeTrapDeed ), 1044052, 1044601, 65.0, 115.0, typeof( Silver ), 1044572, Core.AOS ? 250 : 1000, 1044253 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );
			AddRes( index, typeof( Springs ), 1044171, 1, 1044253 );

			// Faction trap removal kit
			index = AddCraft( typeof( FactionTrapRemovalKit ), 1044052, 1046445, 90.0, 115.0, typeof( Silver ), 1044572, 500, 1044253 );
			AddRes( index, typeof( IronIngot ), 1044036, 10, 1044037 );
			#endregion

			// Set the overridable material
			SetSubRes( typeof( IronIngot ), 1044022 );

			// Add every material you want the player to be able to choose from
			// This will override the overridable material
			AddSubRes( typeof( IronIngot ),			1044022, 00.0, 1044036, 1044267 );
			AddSubRes( typeof( DullCopperIngot ),	1044023, 65.0, 1044036, 1044268 );
			AddSubRes( typeof( ShadowIronIngot ),	1044024, 70.0, 1044036, 1044268 );
			AddSubRes( typeof( CopperIngot ),		1044025, 75.0, 1044036, 1044268 );
			AddSubRes( typeof( BronzeIngot ),		1044026, 80.0, 1044036, 1044268 );
			AddSubRes( typeof( GoldIngot ),			1044027, 85.0, 1044036, 1044268 );
			AddSubRes( typeof( AgapiteIngot ),		1044028, 90.0, 1044036, 1044268 );
			AddSubRes( typeof( VeriteIngot ),		1044029, 95.0, 1044036, 1044268 );
			AddSubRes( typeof( ValoriteIngot ),		1044030, 99.0, 1044036, 1044268 );

			MarkOption = true;
			Repair = true;
			CanEnhance = Core.AOS;
		}
	}

	public abstract class TrapCraft : CustomCraft
	{
		private LockableContainer m_Container;

		public LockableContainer Container{ get{ return m_Container; } }

		public abstract TrapType TrapType{ get; }

		public TrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}

		private int Verify( LockableContainer container )
		{
			if ( container == null || container.KeyValue == 0 )
				return 1005638; // You can only trap lockable chests.
			if ( From.Map != container.Map || !From.InRange( container.GetWorldLocation(), 2 ) )
				return 500446; // That is too far away.
			if ( !container.Movable )
				return 502944; // You cannot trap this item because it is locked down.
			if ( !container.IsAccessibleTo( From ) )
				return 502946; // That belongs to someone else.
			if ( container.Locked )
				return 502943; // You can only trap an unlocked object.
			if ( container.TrapType != TrapType.None )
				return 502945; // You can only place one trap on an object at a time.

			return 0;
		}

		private bool Acquire( object target, out int message )
		{
			LockableContainer container = target as LockableContainer;

			message = Verify( container );

			if ( message > 0 )
			{
				return false;
			}
			else
			{
				m_Container = container;
				return true;
			}
		}

		public override void EndCraftAction()
		{
			From.SendLocalizedMessage( 502921 ); // What would you like to set a trap on?
			From.Target = new ContainerTarget( this );
		}

		private class ContainerTarget : Target
		{
			private TrapCraft m_TrapCraft;

			public ContainerTarget( TrapCraft trapCraft ) : base( -1, false, TargetFlags.None )
			{
				m_TrapCraft = trapCraft;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				int message;

				if ( m_TrapCraft.Acquire( targeted, out message ) )
					m_TrapCraft.CraftItem.CompleteCraft( m_TrapCraft.Quality, false, m_TrapCraft.From, m_TrapCraft.CraftSystem, m_TrapCraft.TypeRes, m_TrapCraft.Tool, m_TrapCraft );
				else
					Failure( message );
			}

			protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
			{
				if ( cancelType == TargetCancelType.Canceled )
					Failure( 0 );
			}

			private void Failure( int message )
			{
				Mobile from = m_TrapCraft.From;
				BaseTool tool = m_TrapCraft.Tool;

				if ( Core.AOS && tool != null && !tool.Deleted && tool.UsesRemaining > 0 )
					from.SendGump( new CraftGump( from, m_TrapCraft.CraftSystem, tool, message ) );
                else if( !Core.AOS )
                    OldCraftMenu.DisplayTo( from, DefTinkering.CraftSystem, tool, message ); // mod by Dies Irae
				else if ( message > 0 )
					from.SendLocalizedMessage( message );
			}
		}

		public override Item CompleteCraft( out int message )
		{
			message = Verify( this.Container );

			if ( message == 0 )
			{
				int trapLevel = (int)(From.Skills.Tinkering.Value / 10);

				Container.TrapType = this.TrapType;
				Container.TrapPower = trapLevel * 9;
				Container.TrapLevel = trapLevel;
				Container.TrapOnLockpick = true;

				message = 1005639; // Trap is disabled until you lock the chest.
			}

			return null;
		}
	}

	[CraftItemID( 0x1BFC )]
	public class DartTrapCraft : TrapCraft
	{
		public override TrapType TrapType{ get{ return TrapType.DartTrap; } }

		public DartTrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}
	}

	[CraftItemID( 0x113E )]
	public class PoisonTrapCraft : TrapCraft
	{
		public override TrapType TrapType{ get{ return TrapType.PoisonTrap; } }

		public PoisonTrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}
	}

	[CraftItemID( 0x370C )]
	public class ExplosionTrapCraft : TrapCraft
	{
		public override TrapType TrapType{ get{ return TrapType.ExplosionTrap; } }

		public ExplosionTrapCraft( Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool, int quality ) : base( from, craftItem, craftSystem, typeRes, tool, quality )
		{
		}
	}
}