using System;

using Midgard.Items;

using Server;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using Server.Mobiles;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;

using Midgard.Engines.PlagueBeastLordPuzzle;

namespace Server.Targets
{
	public class BladedItemTarget : Target
	{
		private Item m_Item;

		public BladedItemTarget( Item item ) : base( 2, false, TargetFlags.None )
		{
			m_Item = item;
		}

		protected override void OnTargetOutOfRange( Mobile from, object targeted )
		{
			if ( targeted is UnholyBone && from.InRange( ((UnholyBone)targeted), 12 ) )
				((UnholyBone)targeted).Carve( from, m_Item );
			else
				base.OnTargetOutOfRange (from, targeted);
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if ( m_Item.Deleted )
				return;

            #region mod by Dies Irae
            if( from == targeted )
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "That is not a bad idea!" );
                return;
            }
            else if( targeted is Mobile && !( (Mobile)targeted ).Alive )
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "But that's not dead!" );
                return;
            }
            #endregion

			if ( targeted is ICarvable )
			{
				((ICarvable)targeted).Carve( from, m_Item );
			}
			else if ( targeted is SwampDragon && ((SwampDragon)targeted).HasBarding )
			{
				SwampDragon pet = (SwampDragon)targeted;

				if ( !pet.Controlled || pet.ControlMaster != from )
					from.SendLocalizedMessage( 1053022 ); // You cannot remove barding from a swamp dragon you do not own.
				else
					pet.HasBarding = false;
			}
			#region Modifica by Dies Irae
			else if ( targeted is Midgard.Mobiles.MidgardWarHorse && ((Midgard.Mobiles.MidgardWarHorse)targeted).HasBarding )
			{
				Midgard.Mobiles.MidgardWarHorse pet = (Midgard.Mobiles.MidgardWarHorse)targeted;

				if ( !pet.Controlled || pet.ControlMaster != from )
					from.SendLocalizedMessage( 1053022 ); // You cannot remove barding from a swamp dragon you do not own.
				else
				{
				    pet.HasBarding = false;
                    HorseBardingDeed.ReturnBardingToOwner( pet, from );
				}
			}
			#endregion
			#region Modifica by Dies Irae
			else if ( targeted is PuzzlePlagueBeastLord )
			{
				from.RevealingAction( true );

				PuzzlePlagueBeastLord pbl = targeted as PuzzlePlagueBeastLord;

				if( pbl.IsFrosted )
				{
					if( pbl.IsCutOpen )
						pbl.Say( 1066136 ); // * The creature has already been opened *
					else
					{
						pbl.Say( 1066102 ); // * You slice through the plague beast's amorphous tissue *
						pbl.InitializePuzzle();
						from.Direction = from.GetDirectionTo( pbl );
						pbl.CutOpenBy = from;
						pbl.Backpack.DisplayTo( from );
					}
				}
				else
					pbl.Say( 1066137 ); // * The beast must be solidified to open it! *
			}
			#endregion
			else
			{
				if ( targeted is StaticTarget )
				{
					int itemID = ((StaticTarget)targeted).ItemID;

					if ( itemID == 0xD15 || itemID == 0xD16 ) // red mushroom
					{
						PlayerMobile player = from as PlayerMobile;

						if ( player != null )
						{
							QuestSystem qs = player.Quest;

							if ( qs is WitchApprenticeQuest )
							{
								FindIngredientObjective obj = qs.FindObjective( typeof( FindIngredientObjective ) ) as FindIngredientObjective;

								if ( obj != null && !obj.Completed && obj.Ingredient == Ingredient.RedMushrooms )
								{
									player.SendLocalizedMessage( 1055036 ); // You slice a red cap mushroom from its stem.
									obj.Complete();
									return;
								}
							}
						}
					}
				}

				HarvestSystem system = Lumberjacking.System;
				HarvestDefinition def = Lumberjacking.System.Definition;

				int tileID;
				Map map;
				Point3D loc;

				if ( !system.GetHarvestDetails( from, m_Item, targeted, out tileID, out map, out loc ) )
				{
					from.SendLocalizedMessage( 500494 ); // You can't use a bladed item on that!
				}
				else if ( !def.Validate( tileID ) )
				{
					from.SendLocalizedMessage( 500494 ); // You can't use a bladed item on that!
				}
				else
				{
					#region modifica by Dies Irae
					HarvestBank bank = def.GetBank( map, loc.X, loc.Y,loc.Z,tileID);
					#endregion

					if ( bank == null )
						return;

					if ( bank.Current < 5 )
					{
						from.SendLocalizedMessage( 500493 ); // There's not enough wood here to harvest.
					}
					else
					{
						bank.Consume( 5, from );

						Item item = new Kindling();

						if ( from.PlaceInBackpack( item ) )
						{
							from.SendLocalizedMessage( 500491 ); // You put some kindling into your backpack.
							from.SendLocalizedMessage( 500492 ); // An axe would probably get you more wood.
						}
						else
						{
							from.SendLocalizedMessage( 500490 ); // You can't place any kindling into your backpack!

							item.Delete();
						}
					}
				}
			}
		}
	}
}
