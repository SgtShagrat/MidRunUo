using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Factions;

using Midgard.Items;
using Midgard.Engines.MidgardTownSystem;

namespace Server.SkillHandlers
{
	public class RemoveTrap
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			if ( m.Skills[SkillName.Lockpicking].Value < 50 )
			{
				m.SendLocalizedMessage( 502366 ); // You do not know enough about locks.  Become better at picking locks.
			}
            /*
			else if ( m.Skills[SkillName.DetectHidden].Value < 50 )
			{
				m.SendLocalizedMessage( 502367 ); // You are not perceptive enough.  Become better at detect hidden.
			}
            */
			else
			{
				m.Target = new InternalTarget();

				m.SendLocalizedMessage( 502368 ); // Wich trap will you attempt to disarm?
			}

			return TimeSpan.FromSeconds( 10.0 ); // 10 second delay before beign able to re-use a skill
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate
				}
				else if ( targeted is TrapableContainer )
				{
					TrapableContainer targ = (TrapableContainer)targeted;

					from.Direction = from.GetDirectionTo( targ );

					if ( targ.TrapType == TrapType.None )
					{
						from.SendLocalizedMessage( 502373 ); // That doesn't appear to be trapped
						return;
					}

					#region mod by Dies Irae
					if( from.Skills[ SkillName.RemoveTrap ].Value < targ.TrapPower )
					{
						from.SendMessage( from.Language == "ITA" ? "Devi avere almeno {0} in Rimuovere Trappole per questa trappola." : "You must have at least {0} Remove Traps skill to try this trap.", targ.TrapPower.ToString( "F0" ) );
						return;
					}
					#endregion

					from.PlaySound( 0x241 );
					
					if ( from.CheckTargetSkill( SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 30 ) )
					{
						if ( !(targeted is TrapableExplosionWoodenBox) )
						{
							targ.TrapPower = 0;
							targ.TrapLevel = 0;
						}
						targ.TrapType = TrapType.None;
						from.SendLocalizedMessage( 502377 ); // You successfully render the trap harmless
					}
					else
					{
						from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
					}
				}
				else if ( targeted is BaseFactionTrap )
				{
					BaseFactionTrap trap = (BaseFactionTrap) targeted;
					Faction faction = Faction.Find( from );

					FactionTrapRemovalKit kit = ( from.Backpack == null ? null : from.Backpack.FindItemByType( typeof( FactionTrapRemovalKit ) ) as FactionTrapRemovalKit );

					bool isOwner = ( trap.Placer == from || ( trap.Faction != null && trap.Faction.IsCommander( from ) ) );

					if ( faction == null )
					{
						from.SendLocalizedMessage( 1010538 ); // You may not disarm faction traps unless you are in an opposing faction
					}
					else if ( faction == trap.Faction && trap.Faction != null && !isOwner )
					{
						from.SendLocalizedMessage( 1010537 ); // You may not disarm traps set by your own faction!
					}
					else if ( !isOwner && kit == null )
					{
						from.SendLocalizedMessage( 1042530 ); // You must have a trap removal kit at the base level of your pack to disarm a faction trap.
					}
					else
					{
						if ( (Core.ML && isOwner) || (from.CheckTargetSkill( SkillName.RemoveTrap, trap, 80.0, 100.0 ) && from.CheckTargetSkill( SkillName.Tinkering, trap, 80.0, 100.0 )) )
						{
							from.PrivateOverheadMessage( MessageType.Regular, trap.MessageHue, trap.DisarmMessage, from.NetState );

							if ( !isOwner )
							{
								int silver = faction.AwardSilver( from, trap.SilverFromDisarm );

								if ( silver > 0 )
									from.SendLocalizedMessage( 1008113, true, silver.ToString( "N0" ) ); // You have been granted faction silver for removing the enemy trap :
							}

							trap.Delete();
						}
						else
						{
							from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
						}

						if ( !isOwner && kit != null )
							kit.ConsumeCharge( from );
					}
				}
				#region modifica by Dies Irae
				else if ( targeted is BaseCraftableTrap )
				{
					BaseCraftableTrap trap = (BaseCraftableTrap)targeted;
					TownSystem system = TownSystem.Find( from );

					TrapRemovalKit kit = ( from.Backpack == null ? null : from.Backpack.FindItemByType( typeof( TrapRemovalKit ) ) as TrapRemovalKit );

					bool isOwner = ( trap.Placer == from );

					if ( trap.System != null && system == null )
					{
						from.SendMessage( from.Language == "ITA" ? "Non puoi disarmare trappole cittadine se non sei tu stesso cittadino." : "You may not disarm citizen traps unless you are also a citizen." );
					}
					else if ( !isOwner && kit == null )
					{
						from.SendMessage( from.Language == "ITA" ? "Devi avere un kit rimozione trappole nello zaino principale per disarmare una trappola." : "You must have a trap removal kit at the base level of your pack to disarm a trap." );
					}
					else
					{
						double difficulty = BaseCraftableTrap.GetDifficultyToRemoveSkill( trap.Level );

						if( from.Skills[ SkillName.RemoveTrap ].Value < difficulty )
						{
							from.SendMessage( from.Language == "ITA" ? "Devi avere almeno {0} in Rimuovere Trappole per questa trappola." : "You must have at least {0} Remove Traps skill to try this trap.", difficulty.ToString( "F0" ) );
							return;
						}

						if ( from.CheckTargetSkill( SkillName.RemoveTrap, trap, difficulty, difficulty + 20.0 ) )
						{
							from.PrivateOverheadMessage( MessageType.Regular, trap.MessageHue, trap.DisarmMessage, from.NetState );

							if( Utility.RandomDouble() < 0.25 )
							trap.Redeed( from );

							if( !trap.Deleted )
								trap.Delete();
						}
						else
						{
							from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
						}

						if ( !isOwner )
							kit.ConsumeCharge( from );
					}
				}
				else if( targeted is BaseDoor )
				{
					BaseDoor door = (BaseDoor)targeted;

					TrapRemovalKit kit = ( from.Backpack == null ? null : from.Backpack.FindItemByType( typeof( TrapRemovalKit ) ) as TrapRemovalKit );

					from.Direction = from.GetDirectionTo( door );

					if ( door.TrapType == TrapType.None )
					{
						from.SendLocalizedMessage( 502373 ); // That doesn't appear to be trapped
						return;
					}

					if( door.TrapType == TrapType.MagicTrap )
					{
						from.SendMessage( from.Language == "ITA" ? "Non puoi disarmare questa potente trappola magica solo con la tua abilità." : "You cannot disarm this powerfull magical trap only with your skills!" );
						return;
					}

					from.PlaySound( 0x241 );

					if( from.CheckTargetSkill( SkillName.RemoveTrap, door, door.TrapPower, door.TrapPower + 30 ) )
					{
						if( Utility.RandomDouble() < 0.25 )
							door.Redeed( from );

						door.RemoveTrap( from, true );

						from.SendLocalizedMessage( 502377 ); // You successfully render the trap harmless
					}
					else
					{
						if( Utility.Dice( 1, 4, 0 ) == 1 )
							door.ExecuteTrap( from );
						else
							from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off
					}

					if ( kit != null )
						kit.ConsumeCharge( from );
				}
				#endregion
				else
				{
					from.SendLocalizedMessage( 502373 ); // That does'nt appear to be trapped
				}
			}
		}
	}
}