using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;

namespace Server.Engines.Harvest
{
	public class HarvestTarget : Target
	{
		private Item m_Tool;
		private HarvestSystem m_System;

		public HarvestTarget( Item tool, HarvestSystem system ) : base( -1, true, TargetFlags.None )
		{
			m_Tool = tool;
			m_System = system;

			DisallowMultis = true;
		}

		protected override void OnTarget( Mobile from, object targeted )
        {
            #region mod by Dies Irae
            if( m_System is Mining )
            {
                if( targeted == m_Tool && targeted is Pickaxe )
                {
                    PlayerMobile player = from as PlayerMobile;

                    if( player != null )
                    {
                        bool oldValue = player.ToggleMiningStone;

                        if( !oldValue )
                        {
                            if( !player.StoneMining || ( player.Skills[ SkillName.Mining ].Base < 100.0 && Core.AOS ) ) // mod by Dies irae
                            {
                                player.SendLocalizedMessage( 1054024 ); // You have not learned how to mine stone or you do not have enough skill!
                            }
                            else
                            {
                                player.ToggleMiningStone = true;
                                player.SendLocalizedMessage( 1054022 ); // You are now set to mine both ore and stone.
                            }
                        }
                        else
                        {
                            player.ToggleMiningStone = false;
                            player.SendLocalizedMessage( 1054020 ); // You are now set to mine only ore.
                        }

                        return;
                    }
                }
            }
            #endregion

            if ( m_System is Mining && targeted is StaticTarget )
			{
				int itemID = ((StaticTarget)targeted).ItemID;

				// grave
				if ( itemID == 0xED3 || itemID == 0xEDF || itemID == 0xEE0 || itemID == 0xEE1 || itemID == 0xEE2 || itemID == 0xEE8 )
				{
					PlayerMobile player = from as PlayerMobile;

					if ( player != null )
					{
						QuestSystem qs = player.Quest;

						if ( qs is WitchApprenticeQuest )
						{
							FindIngredientObjective obj = qs.FindObjective( typeof( FindIngredientObjective ) ) as FindIngredientObjective;

							if ( obj != null && !obj.Completed && obj.Ingredient == Ingredient.Bones )
							{
								player.SendLocalizedMessage( 1055037 ); // You finish your grim work, finding some of the specific bones listed in the Hag's recipe.
								obj.Complete();

								return;
							}
						}
					}
				}
			}

			if ( m_System is Lumberjacking && targeted is IChopable )
				((IChopable)targeted).OnChop( from );
			else if ( m_System is Lumberjacking && targeted is IAxe && (m_Tool is BaseAxe || m_Tool is Bardiche || m_Tool is Halberd))
			{
				IAxe obj = (IAxe)targeted;
				Item item = (Item)targeted;
					
				if ( !item.IsChildOf( from.Backpack ) )
					from.SendLocalizedMessage( 1062334 ); // This item must be in your backpack to be used.
				else if ( obj.Axe( from, (BaseWeapon)m_Tool ) )
					from.PlaySound( 0x13E );
			}
			else if ( m_System is Lumberjacking && targeted is ICarvable )
				((ICarvable)targeted).Carve( from, (Item)m_Tool );
			else if ( m_System is Lumberjacking && FurnitureAttribute.Check( targeted as Item ) )
				DestroyFurniture( from, (Item)targeted );
			else if ( m_System is Mining && targeted is TreasureMap )
				((TreasureMap)targeted).OnBeginDig( from );
			else
				m_System.StartHarvesting( from, m_Tool, targeted );
		}

		private void DestroyFurniture( Mobile from, Item item )
		{
			if ( !from.InRange( item.GetWorldLocation(), 3 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
				return;
			}
			else if ( !item.IsChildOf( from.Backpack ) && !item.Movable )
			{
				from.SendLocalizedMessage( 500462 ); // You can't destroy that while it is here.
				return;
			}

			from.SendLocalizedMessage( 500461 ); // You destroy the item.
			Effects.PlaySound( item.GetWorldLocation(), item.Map, 0x3B3 );

			if ( item is Container )
			{
				if ( item is TrapableContainer )
					(item as TrapableContainer).ExecuteTrap( from );

				((Container)item).Destroy();
			}
			else
			{
				item.Delete();
			}
		}
	}
}