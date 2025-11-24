using System;
using Server;
using Server.Engines.Craft;
using Server.Items;
using Server.Menus.ItemLists;
using Server.Targeting;

namespace Midgard.Engines.OldCraftSystem
{
	public class SelectionTarget : Target
	{
		private CraftSystem m_System;
		private BaseTool m_Tool;
		private ParentNode m_Branch;

		public SelectionTarget( CraftSystem system, BaseTool tool, ParentNode branch )
			: base( 0, false, TargetFlags.None )
		{
			m_System = system;
			m_Tool = tool;
			m_Branch = branch;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if( !( targeted is Item ) )
				return;

			CraftContext context = m_System.GetContext( from );
			if( context == null )
				return;

			bool table = false;
			if (m_System == DefAlchemy.CraftSystem)
				DefAlchemy.CheckAlchemyTable( from, 2, out table );

			Item item = (Item)targeted;
			if( !item.IsChildOf( from.Backpack ) )
			{
				from.SendMessage( (from.Language == "ITA" ? "Devi averlo in borsa." : "That item must be in your backpack.") );
			}
			else if( m_System == DefTailoring.CraftSystem && targeted is IFortificable )
			{
				( (IFortificable)targeted ).Fortify( from, m_Tool );
			}
			else if( m_System == DefAlchemy.CraftSystem && targeted is BaseReagent && ( ((BaseReagent)targeted).PotionType != 0 ) && table )
			{
				//DefAlchemy.CheckAlchemyTable( from, 2, out table );

				//if( !table )
				//{
				//	from.SendMessage( (from.Language == "ITA" ? "Devi essere vicino ad un tavolo alchemico per la combinazione." : "You must be near an alchemic table for the combination.") );
				//	return;
				//}
				//else
				//{
					from.Target = new AlchemyTarget( m_Tool, (BaseReagent)targeted );
					from.SendMessage( (from.Language == "ITA" ? "Scegli un altro ingrediente." : "Choose another ingredient.") );
				//}
			}
			else if( targeted is IRepairable )
			{
				if( from.Skills[ m_System.MainSkill ].Value < m_System.MinSkillToRepair )
				{
					from.SendMessage( (from.Language == "ITA" ? "Ti serve più esperienza per riparare questo oggetto." : "You are too unexperted to repair this item.") );
					return;
				}
				else if( m_System == DefBlacksmithy.CraftSystem )
				{
					bool anvil, forge;
					DefBlacksmithy.CheckAnvilAndForge( from, 2, out anvil, out forge );

					if( !anvil )
					{
						from.SendMessage( (from.Language == "ITA" ? "Devi essere vicino ad una incudine per riparare oggetti." : "You must be near an anvil to repair items.") );
						return;
					}
				}

				( (IRepairable)targeted ).Repair( from, m_Tool );
			}
			else
			{
				Type type = CraftItem.GetBaseFromMutatedType( item.GetType() );
				CraftSubRes res = m_System.GetResourceFromType( type, m_Branch.UsesMainSubRes, m_Branch.UsesSecondarySubRes );

				if( m_Branch.SupportResource( type ) )
				{
					// if res is supported but null it means it is something like CLoth for 
					// DefTailoring. That material is not overridable so we do not need to check 
					// the material required skill
					if( res != null )
					{
						if( !m_System.CheckResourceSkill( from, res ) )
							return;

						int resIndex = -1;

						if( m_Branch.UsesMainSubRes )
						{
							resIndex = m_System.CraftSubRes.GetIndex( res );
							if( resIndex >= 0 && resIndex < m_System.CraftSubRes.Count )
								context.LastResourceIndex = resIndex;
						}

						if( resIndex == -1 && m_Branch.UsesSecondarySubRes )
						{
							resIndex = m_System.CraftSubRes2.GetIndex( res );
							if( resIndex >= 0 && resIndex < m_System.CraftSubRes2.Count )
								context.LastResourceIndex2 = resIndex;
						}
					}

					context.LastResourceSelected = type;

					if( Config.Debug )
						Config.Pkg.LogInfoLine( "m_Branch.LastResourceSelected = " + type.Name );

					ItemListEntry[] list = OldCraftMenu.GetEntriesByNode( from, m_System, m_Branch );

					if( list.Length > 0 )
						from.SendMenu( new OldCraftMenu( from, m_System, m_Tool, m_Branch, list ) );
					else
						from.SendMessage( (from.Language == "ITA" ? "Non puoi creare niente di questo tipo." : "You cannot make anything of that kind.") );
				}
				else
				{
					if (m_System == DefAlchemy.CraftSystem && targeted is BaseReagent && ( ((BaseReagent)targeted).PotionType != 0 ))
						from.SendMessage( (from.Language == "ITA" ? "Devi essere vicino ad un tavolo alchemico per la combinazione." : "You must be near an alchemic table for the combination.") );
					else
						from.SendMessage( (from.Language == "ITA" ? "Non è un materia prima utile." : "That is not a valid material.") );
				}
			}
		}
	}
}