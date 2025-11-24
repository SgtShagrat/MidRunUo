using System;
using Midgard.Engines.OldCraftSystem;
using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	public class DefCartography : CraftSystem
	{
		#region mod by Dies Irae
        public override string Name { get{ return "Cartography"; } }

		public static void Initialize()
		{
			CraftSystem sys = CraftSystem;
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
                    m_CraftDefinitionTree = new CraftDefinitionTree( "Cartography.xml", CraftSystem );

                return m_CraftDefinitionTree;
            }
        }

        private static CraftDefinitionTree m_CraftDefinitionTree;
		#endregion
		
		public override SkillName MainSkill
		{
			get	{ return SkillName.Cartography; }
		}

		public override int GumpTitleNumber
		{
			get { return 1044008; } // <CENTER>CARTOGRAPHY MENU</CENTER>
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.0; // 0%
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefCartography();

				return m_CraftSystem;
			}
		}

		private DefCartography() : base( 2, 4, 1.25 ) // mod by Dies Irae // base( 1, 1, 3.0 )
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
			from.PlaySound( 0x249 );
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
		}
		#endregion

        public void InitOldCraftList()
        {
            #region maps
            AddCraft( typeof( LocalMap ), 1044448, 1015230, 10.0, 70.0, typeof( BlankMap ), 1044449, 1, 1044450 );
            AddCraft( typeof( CityMap ), 1044448, 1015231, 25.0, 85.0, typeof( BlankMap ), 1044449, 1, 1044450 );
            AddCraft( typeof( SeaChart ), 1044448, 1015232, 35.0, 95.0, typeof( BlankMap ), 1044449, 1, 1044450 );
            AddCraft( typeof( WorldMap ), 1044448, 1015233, 39.5, 99.5, typeof( BlankMap ), 1044449, 1, 1044450 );
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

			AddCraft( typeof( LocalMap ), 1044448, 1015230, 10.0, 70.0, typeof( BlankMap ), 1044449, 1, 1044450 );
			AddCraft( typeof(  CityMap ), 1044448, 1015231, 25.0, 85.0, typeof( BlankMap ), 1044449, 1, 1044450 );
			AddCraft( typeof( SeaChart ), 1044448, 1015232, 35.0, 95.0, typeof( BlankMap ), 1044449, 1, 1044450 );
			AddCraft( typeof( WorldMap ), 1044448, 1015233, 39.5, 99.5, typeof( BlankMap ), 1044449, 1, 1044450 );
		}
	}
}