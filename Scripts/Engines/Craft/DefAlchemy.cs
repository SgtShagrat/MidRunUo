using System;
using Midgard.Engines.Craft;
using Midgard.Engines.OldCraftSystem;
using Midgard.Engines.PlantSystem;
using Midgard.Items;
using Midgard.Misc;

using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	public class DefAlchemy : CraftSystem
	{
		#region mod by Dies Irae
        public override string Name { get{ return "Alchemy"; } }

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
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Alchemy.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;

        public override CraftECA ECA { get { return CraftECA.ZeroToFourPercentPlusBonus; } }

        public static void CheckAlchemyTable( Mobile from, int range, out bool alchemytable )
        {
            alchemytable = false;

            Map map = from.Map;

            if( map == null )
                return;

            IPooledEnumerable eable = map.GetItemsInRange( from.Location, range );

            foreach( Item item in eable )
            {
                Type t = item.GetType();

                if( t == typeof( AlchemistTableEastAddon ) || t == typeof( AlchemistTableSouthAddon ) )
                {
                    alchemytable = true;
                }

                if( alchemytable )
                    break;
            }

            eable.Free();
        }
		#endregion
		
		public override SkillName MainSkill
		{
			get	{ return SkillName.Alchemy;	}
		}

		public override int GumpTitleNumber
		{
			get { return 1044001; } // <CENTER>ALCHEMY MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefAlchemy();

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin( CraftItem item )
		{
            #region mod by Dies Irae
            if( item != null && item.ItemType.IsSubclassOf( typeof( BasePaganPotion ) ) )
            {
                Console.WriteLine( "Pagan bonus: 10%" );
                return 0.10;
            }
            #endregion

			return 0.0; // 0%
		}

		private DefAlchemy() : base( 2, 4, 1.25 ) // mod by Dies Irae // base( 1, 1, 3.1 )
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
            from.PlaySound( 0x242 );

            #region mod by Dies Irae
            if( from.Body.IsHuman )
                from.Animate( AnimsOnMount.GetAnim( 0x21, from.Mounted ), 7, 1, true, false, 0 );
            #endregion
        }

		private static Type typeofPotion = typeof( BasePotion );

		public static bool IsPotion( Type type )
		{
			return typeofPotion.IsAssignableFrom( type );
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( IsPotion( item.ItemType ) )
				{
					from.AddToBackpack( new Bottle() );
					return 500287; // You fail to create a useful potion.
				}
				else
				{
					return 1044043; // You failed to create the item, and some of your materials are lost.
				}
			}
			else
			{
				from.PlaySound( 0x240 ); // Sound of a filling bottle

				if ( IsPotion( item.ItemType ) )
				{
					if ( quality == -1 )
						return 1048136; // You create the potion and pour it into a keg.
					else
						return 500279; // You pour the potion into a bottle...
				}
				else
				{
					return 1044154; // You create the item.
				}
			}
		}

		#region modifica by Dies Irae
		private static List<Type> m_DebugItemList;

		public static void InitDebugItemsList()
		{
			if( m_DebugItemList == null )
				m_DebugItemList = new List<Type>();
			
            //m_DebugItemList.Add( typeof(ConflagrationPotion) );
            //m_DebugItemList.Add( typeof(GreaterConflagrationPotion) );
            //m_DebugItemList.Add( typeof(ConfusionBlastPotion) );
            //m_DebugItemList.Add( typeof(GreaterConfusionBlastPotion) );
            //m_DebugItemList.Add( typeof(InvisibilityPotion) );
            //m_DebugItemList.Add( typeof(ParasiticPotion) );
            //m_DebugItemList.Add( typeof(DarkglowPotion) );
            //m_DebugItemList.Add( typeof(HoveringWisp) );
		}
		#endregion

        public void InitOldCraftList()
        {
            int index = -1;

            #region Reagents
            AddCraft( typeof( Garlic ), 1065281, "garlic", 50.0, 80.0, typeof( GarlicBulb ), "Garlic Bulb", 1 );
            AddCraft( typeof( Ginseng ), 1065281, "ginseng", 50.0, 80.0, typeof( RawGinsengRoot ), "Raw Ginseng Root", 1 );
            AddCraft( typeof( MandrakeRoot ), 1065281, "mandrake root", 50.0, 80.0, typeof( RawMandrakeRoot ), "Raw Mandrake Root", 1 );
            AddCraft( typeof( Nightshade ), 1065281, "night shade", 50.0, 80.0, typeof( RawNightshade ), "Raw Nightshade", 1 );
            #endregion

            #region Refresh Potion
            index = AddCraft( typeof( RefreshPotion ), 1044530, 1044538, -25, 25.0, typeof( BlackPearl ), 1044353, 1, 1044361 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( TotalRefreshPotion ), 1044530, 1044539, 25.0, 75.0, typeof( BlackPearl ), 1044353, 5, 1044361 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Agility Potion
            index = AddCraft( typeof( AgilityPotion ), 1044531, 1044540, 15.0, 65.0, typeof( Bloodmoss ), 1044354, 1, 1044362 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( GreaterAgilityPotion ), 1044531, 1044541, 35.0, 85.0, typeof( Bloodmoss ), 1044354, 3, 1044362 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Nightsight Potion
            index = AddCraft( typeof( NightSightPotion ), 1044532, 1044542, -25.0, 25.0, typeof( SpidersSilk ), 1044360, 1, 1044368 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Heal Potion
            index = AddCraft( typeof( LesserHealPotion ), 1044533, 1044543, -25.0, 25.0, typeof( Ginseng ), 1044356, 1, 1044364 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( HealPotion ), 1044533, 1044544, 15.0, 65.0, typeof( Ginseng ), 1044356, 3, 1044364 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( GreaterHealPotion ), 1044533, 1044545, 55.0, 105.0, typeof( Ginseng ), 1044356, 7, 1044364 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Strength Potion
            index = AddCraft( typeof( StrengthPotion ), 1044534, 1044546, 25.0, 75.0, typeof( MandrakeRoot ), 1044357, 2, 1044365 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( GreaterStrengthPotion ), 1044534, 1044547, 45.0, 95.0, typeof( MandrakeRoot ), 1044357, 5, 1044365 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Poison Potion
            index = AddCraft( typeof( LesserPoisonPotion ), 1044535, 1044548, -5.0, 45.0, typeof( Nightshade ), 1044358, 1, 1044366 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( PoisonPotion ), 1044535, 1044549, 15.0, 65.0, typeof( Nightshade ), 1044358, 2, 1044366 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( GreaterPoisonPotion ), 1044535, 1044550, 55.0, 105.0, typeof( Nightshade ), 1044358, 4, 1044366 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( DeadlyPoisonPotion ), 1044535, 1044551, 90.0, 140.0, typeof( Nightshade ), 1044358, 8, 1044366 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Cure Potion
            index = AddCraft( typeof( LesserCurePotion ), 1044536, 1044552, -10.0, 40.0, typeof( Garlic ), 1044355, 1, 1044363 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( CurePotion ), 1044536, 1044553, 25.0, 75.0, typeof( Garlic ), 1044355, 3, 1044363 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( GreaterCurePotion ), 1044536, 1044554, 65.0, 115.0, typeof( Garlic ), 1044355, 6, 1044363 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Explosion Potion
            index = AddCraft( typeof( LesserExplosionPotion ), 1044537, 1044555, 5.0, 55.0, typeof( SulfurousAsh ), 1044359, 3, 1044367 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( ExplosionPotion ), 1044537, 1044556, 35.0, 85.0, typeof( SulfurousAsh ), 1044359, 5, 1044367 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            index = AddCraft( typeof( GreaterExplosionPotion ), 1044537, 1044557, 65.0, 115.0, typeof( SulfurousAsh ), 1044359, 10, 1044367 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Pagan Potions
            /*
            1064083 Black Moor 
            1064084 Dragon Blood 
            1064085 Serpent Scale 
            1064086 Fertile Dirt 
            1064087 Volcanin Ash 
            1064088 Wyrm Heart 
            1064089 Executioner Cap 
            1064090 Eye Of Newt 
            1064091 Pumices 
            1064092 Obsidian 
            1064093 Blood Spawn 
            1064094 Dead Wood 
            1064095 Bat Wing 
            1064096 Daemon Bone 
            1064097 Barbed Hides 
            1064098 Greater Heal Potion
            1041321 Greater Strength potion
             
            1065150 Amethyst 
            1065151 Ruby 
            1065152 Diamond 
            1065153 Citrine 
            1065154 Sapphire 
            1065155 Star Sapphire 
            1065156 Emerald 
            1065157 Amber
             
            1044353 Black Pearl 
            1044354 Blood Moss 
            1044355 Garlic 
            1044356 Ginseng 
            1044357 Mandrake Root 
            1044358 Nightshade 
            1044359 Sulfurous Ash 
            1044360 Spiders' Silk
            */

            #region GrandMageRefresh
            index = AddCraft( typeof( GrandMageRefreshElixirLesser ), "Gran Mage", 1064043, 50.0, 90.0, typeof( BlackMoor ), 1064083, 5 );
            AddRes( index, typeof( Amethyst ), 1065150, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( GrandMageRefreshElixir ), "Gran Mage", 1064044, 70.0, 110.0, typeof( BlackMoor ), 1064083, 10 );
            AddRes( index, typeof( Amethyst ), 1065150, 2 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( GrandMageRefreshElixirGreater ), "Gran Mage", 1064045, 90.0, 130.0, typeof( BlackMoor ), 1064083, 15 );
            AddRes( index, typeof( Amethyst ), 1065150, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region HomericMightPotion
            index = AddCraft( typeof( HomericMightPotion ), "Homeric", 1064046, 40.0, 80.0, typeof( DragonBlood ), 1064084, 1 );
            AddRes( index, typeof( GreaterStrengthPotion ), 1041321, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( HomericMightPotionGreater ), "Homeric", 1064047, 75.0, 115.0, typeof( DragonBlood ), 1064084, 4 );
            AddRes( index, typeof( GreaterStrengthPotion ), 1041321, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region MegoInvulnerability
            index = AddCraft( typeof( MegoInvulnerabilityPotionLesser ), "Invulnerability", 1064048, 30.0, 80.0, typeof( SerpentScale ), 1064085, 1 );
            AddRes( index, typeof( Ginseng ), 1044356, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( MegoInvulnerabilityPotion ), "Invulnerability", 1064049, 50.0, 100.0, typeof( SerpentScale ), 1064085, 4 );
            AddRes( index, typeof( Ginseng ), 1044356, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( MegoInvulnerabilityPotionGreater ), "Invulnerability", 1064050, 80.0, 130.0, typeof( SerpentScale ), 1064085, 6 );
            AddRes( index, typeof( Ginseng ), 1044356, 5 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region PhandelsIntellect
            index = AddCraft( typeof( PhandelsFineIntellectPotion ), "Intellect", 1064051, 30.0, 80.0, typeof( FertileDirt ), 1064086, 1 );
            AddRes( index, typeof( VolcaninAsh ), 1064087, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( PhandelsFabulousIntellectPotion ), "Intellect", 1064052, 60.0, 100.0, typeof( FertileDirt ), 1064086, 2 );
            AddRes( index, typeof( VolcaninAsh ), 1064087, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( PhandelsFantasticIntellectPotion ), "Intellect", 1064053, 80.0, 120.0, typeof( FertileDirt ), 1064086, 5 );
            AddRes( index, typeof( VolcaninAsh ), 1064087, 7 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region TaintsTransmutation
            index = AddCraft( typeof( TaintsMinorTransmutationPotion ), "Transmutation", 1064054, 70.0, 110.0, typeof( FertileDirt ), 1064086, 2 );
            AddRes( index, typeof( DeadWood ), 1064094, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( TaintsMajorTransmutationPotion ), "Transmutation", 1064055, 90.0, 130.0, typeof( FertileDirt ), 1064086, 5 );
            AddRes( index, typeof( DeadWood ), 1064094, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Totem-Elixir
            index = AddCraft( typeof( Totem ), "Totem", 1064056, 95.0, 135.0, typeof( BatWing ), 1064095, 2 );
            AddRes( index, typeof( DaemonBone ), 1064096, 5 );
            AddRes( index, typeof( SpinedHides ), "reptile hides", 6 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( Elixir ), "Totem", 1064057, 95.0, 135.0, typeof( Diamond ), "diamond", 1 );
            AddRes( index, typeof( WyrmHeart ), "wyrm heart", 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Varie
            index = AddCraft( typeof( InvisibilityPotion ), "Misc", 1064058, 90.0, 130.0, typeof( Nightshade ), 1044358, 1 );
            AddRes( index, typeof( ExecutionerCap ), 1064089, 2 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( TamlaPotion ), "Misc", 1064059, 95.0, 135.0, typeof( WyrmHeart ), 1064088, 3 );
            AddRes( index, typeof( StarSapphire ), 1065155, 4 );
            AddRes( index, typeof( GreaterHealPotion ), 1064098, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            //index = AddCraft( typeof( FlashBangPotion ), "Misc", 1064060, 85.0, 135.0, typeof( EyeOfNewt ), 1064090, 3 );
            //AddRes( index, typeof( VolcaninAsh ), 1064087, 3 );
            //AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion
            #endregion

            #region Pet Health Potions
            index = AddCraft( typeof( PetCurePotion ), 1064076, 1064077, 80.0, 130.0, typeof( Garlic ), 1044355, 10 );
            AddSkill( index, SkillName.AnimalLore, 50.0, 55.0 );
            AddRes( index, typeof( Emerald ), 1065156, 1 );
            AddRes( index, typeof( WyrmHeart ), 1064088, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( PetGreaterCurePotion ), 1064076, 1064078, 85.0, 135.0, typeof( Garlic ), 1044355, 15 );
            AddSkill( index, SkillName.AnimalLore, 80.0, 85.0 );
            AddRes( index, typeof( Emerald ), 1065156, 2 );
            AddRes( index, typeof( WyrmHeart ), 1064088, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( PetHealPotion ), 1064076, 1064079, 80.0, 130.0, typeof( Ginseng ), 1015009, 10 );
            AddSkill( index, SkillName.AnimalLore, 50.0, 55.0 );
            AddRes( index, typeof( Ruby ), 1065151, 1 );
            AddRes( index, typeof( DragonBlood ), 1064084, 1 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( PetGreaterHealPotion ), 1064076, 1064080, 85.0, 135.0, typeof( Ginseng ), 1015009, 15 );
            AddSkill( index, SkillName.AnimalLore, 80.0, 85.0 );
            AddRes( index, typeof( Ruby ), 1065151, 2 );
            AddRes( index, typeof( DragonBlood ), 1064084, 3 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( PetResurrectionPotion ), 1064076, 1064081, 90.0, 140.0, typeof( EyeOfNewt ), 1064090, 3 );
            AddSkill( index, SkillName.AnimalLore, 90.0, 95.0 );
            AddRes( index, typeof( Diamond ), 1065152, 2 );
            AddRes( index, typeof( DragonBlood ), 1064084, 5 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );
            #endregion

            #region Plant Health
            index = AddCraft( typeof( LightFertilizerPotion ), 1065775, 1065776, 45.0, 95.0, typeof( MandrakeRoot ), 1044357, 3, 1044365 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( MediumFertilizerPotion ), 1065775, 1065777, 65.0, 115.0, typeof( MandrakeRoot ), 1044357, 6, 1044365 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( HeavyFertilizerPotion ), 1065775, 1065778, 85.0, 135.0, typeof( MandrakeRoot ), 1044357, 9, 1044365 );
            AddRes( index, typeof( Bottle ), 1044529, 1, 500315 );

            index = AddCraft( typeof( LightFungicidePotion ), 1065775, 1065779, 45.0, 95.0, typeof( Nightshade ), 1044358, 3, 1044366 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );

            index = AddCraft( typeof( MediumFungicidePotion ), 1065775, 1065780, 65.0, 115.0, typeof( Nightshade ), 1044358, 6, 1044366 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );

            index = AddCraft( typeof( HeavyFungicidePotion ), 1065775, 1065781, 85.0, 135.0, typeof( Nightshade ), 1044358, 9, 1044366 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );

            index = AddCraft( typeof( LightPesticidePotion ), 1065775, 1065782, 45.0, 95.0, typeof( Ginseng ), 1044356, 3, 1044364 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );

            index = AddCraft( typeof( MediumPesticidePotion ), 1065775, 1065783, 65.0, 115.0, typeof( Ginseng ), 1044356, 6, 1044364 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );

            index = AddCraft( typeof( HeavyPesticidePotion ), 1065775, 1065784, 85.0, 135.0, typeof( Ginseng ), 1044356, 9, 1044364 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );
            #endregion

            #region Narcotics
            index = AddCraft( typeof( LightNarcoticPotion ), 1065735, 1065736, 15.0, 65.0, typeof( Nightshade ), 1044358, 2, 1044366 );
            AddRes( index, typeof( LesserPoisonPotion ), 1065740, 1, 1044253 );

            index = AddCraft( typeof( MediumNarcoticPotion ), 1065735, 1065737, 55.0, 105.0, typeof( Nightshade ), 1044358, 4, 1044366 );
            AddRes( index, typeof( PoisonPotion ), 1065741, 1, 1044253 );

            index = AddCraft( typeof( RegularNarcoticPotion ), 1065735, 1065738, 90.0, 140.0, typeof( Nightshade ), 1044358, 8, 1044366 );
            AddRes( index, typeof( GreaterPoisonPotion ), 1065742, 1, 1044253 );
            //AddRecipe( index, (int)NarcoticsRecipes.RegularNarcoticPotion );

            index = AddCraft( typeof( HeavyNarcoticPotion ), 1065735, 1065739, 95.0, 140.0, typeof( Nightshade ), 1044358, 16, 1044366 );
            AddRes( index, typeof( DeadlyPoisonPotion ), 1065743, 1, 1044253 );
            //AddRecipe( index, (int)NarcoticsRecipes.HeavyNarcoticPotion );
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

			int index = -1;

			// Refresh Potion
			index = AddCraft( typeof( RefreshPotion ), 1044530, 1044538, -25, 25.0, typeof( BlackPearl ), 1044353, 1, 1044361 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( TotalRefreshPotion ), 1044530, 1044539, 25.0, 75.0, typeof( BlackPearl ), 1044353, 5, 1044361 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Agility Potion
			index = AddCraft( typeof( AgilityPotion ), 1044531, 1044540, 15.0, 65.0, typeof( Bloodmoss ), 1044354, 1, 1044362 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( GreaterAgilityPotion ), 1044531, 1044541, 35.0, 85.0, typeof( Bloodmoss ), 1044354, 3, 1044362 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Nightsight Potion
			index = AddCraft( typeof( NightSightPotion ), 1044532, 1044542, -25.0, 25.0, typeof( SpidersSilk ), 1044360, 1, 1044368 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Heal Potion
			index = AddCraft( typeof( LesserHealPotion ), 1044533, 1044543, -25.0, 25.0, typeof( Ginseng ), 1044356, 1, 1044364 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( HealPotion ), 1044533, 1044544, 15.0, 65.0, typeof( Ginseng ), 1044356, 3, 1044364 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( GreaterHealPotion ), 1044533, 1044545, 55.0, 105.0, typeof( Ginseng ), 1044356, 7, 1044364 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Strength Potion
			index = AddCraft( typeof( StrengthPotion ), 1044534, 1044546, 25.0, 75.0, typeof( MandrakeRoot ), 1044357, 2, 1044365 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( GreaterStrengthPotion ), 1044534, 1044547, 45.0, 95.0, typeof( MandrakeRoot ), 1044357, 5, 1044365 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Poison Potion
			index = AddCraft( typeof( LesserPoisonPotion ), 1044535, 1044548, -5.0, 45.0, typeof( Nightshade ), 1044358, 1, 1044366 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( PoisonPotion ), 1044535, 1044549, 15.0, 65.0, typeof( Nightshade ), 1044358, 2, 1044366 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( GreaterPoisonPotion ), 1044535, 1044550, 55.0, 105.0, typeof( Nightshade ), 1044358, 4, 1044366 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( DeadlyPoisonPotion ), 1044535, 1044551, 90.0, 140.0, typeof( Nightshade ), 1044358, 8, 1044366 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Cure Potion
			index = AddCraft( typeof( LesserCurePotion ), 1044536, 1044552, -10.0, 40.0, typeof( Garlic ), 1044355, 1, 1044363 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( CurePotion ), 1044536, 1044553, 25.0, 75.0, typeof( Garlic ), 1044355, 3, 1044363 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( GreaterCurePotion ), 1044536, 1044554, 65.0, 115.0, typeof( Garlic ), 1044355, 6, 1044363 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			// Explosion Potion
			index = AddCraft( typeof( LesserExplosionPotion ), 1044537, 1044555, 5.0, 55.0, typeof( SulfurousAsh ), 1044359, 3, 1044367 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( ExplosionPotion ), 1044537, 1044556, 35.0, 85.0, typeof( SulfurousAsh ), 1044359, 5, 1044367 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );
			index = AddCraft( typeof( GreaterExplosionPotion ), 1044537, 1044557, 65.0, 115.0, typeof( SulfurousAsh ), 1044359, 10, 1044367 );
			AddRes( index, typeof ( Bottle ), 1044529, 1, 500315 );

			if( Core.SE )
			{
				index = AddCraft( typeof( SmokeBomb ), 1044537, 1030248, 90.0, 120.0, typeof( Eggs ), 1044477, 1, 1044253 );
				AddRes( index, typeof ( Ginseng ), 1044356, 3, 1044364 );
				SetNeededExpansion( index, Expansion.SE );

				// Conflagration Potions
				index = AddCraft( typeof(ConflagrationPotion), 1044109, 1072096, 55.0, 105.0, typeof(GraveDust), 1023983, 5, 1044253 );
				AddRes( index, typeof(Bottle), 1044529, 1, 500315 );
				SetNeededExpansion(index, Expansion.SE);
				index = AddCraft( typeof(GreaterConflagrationPotion), 1044109, 1072099, 65.0, 115.0, typeof(GraveDust), 1023983, 10, 1044253 );
				AddRes( index, typeof(Bottle), 1044529, 1, 500315 );
				SetNeededExpansion(index, Expansion.SE);
				// Confusion Blast Potions
				index = AddCraft( typeof(ConfusionBlastPotion), 1044109, 1072106, 55.0, 105.0, typeof(PigIron), 1023978, 5, 1044253 );
				AddRes( index, typeof(Bottle), 1044529, 1, 500315 );
				SetNeededExpansion(index, Expansion.SE);
				index = AddCraft( typeof(GreaterConfusionBlastPotion), 1044109, 1072109, 65.0, 115.0, typeof(PigIron), 1023978, 10, 1044253 );
				AddRes( index, typeof(Bottle), 1044529, 1, 500315 );
				SetNeededExpansion(index, Expansion.SE);
			}
		}

	    #region mod by Dies Irae
	    public static Item RandomPaganRecipe()
	    {
	        List<int> list = new List<int>();
	        foreach( int i in Enum.GetValues( typeof( PagansRecipes ) ) )
	            list.Add( i );

	        return new RecipeScroll( list[ Utility.Random( list.Count ) ] );
	    }

	    public static Item RandomPetPotionRecipe()
	    {
	        List<int> list = new List<int>();
	        foreach( int i in Enum.GetValues( typeof( PetPotionsRecipes ) ) )
	            list.Add( i );

	        return new RecipeScroll( list[ Utility.Random( list.Count ) ] );
	    }

	    public static Item RandomNarcoticRecipe()
	    {
	        List<int> list = new List<int>();
	        foreach( int i in Enum.GetValues( typeof( NarcoticsRecipes ) ) )
	            list.Add( i );

	        return new RecipeScroll( list[ Utility.Random( list.Count ) ] );
	    }

	    public static Item RandomAdvAlchemyRecipe()
	    {
	        List<int> list = new List<int>();
	        foreach( int i in Enum.GetValues( typeof( PagansRecipes ) ) )
	            list.Add( i );
	        foreach( int i in Enum.GetValues( typeof( PetPotionsRecipes ) ) )
	            list.Add( i );
	        foreach( int i in Enum.GetValues( typeof( NarcoticsRecipes ) ) )
	            list.Add( i );

	        return new RecipeScroll( list[ Utility.Random( list.Count ) ] );
	    }
	    #endregion
	}
}