using System;
using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.SkillHandlers
{
	public class Snooping
	{
		public static void Configure()
		{
			Container.SnoopHandler = new ContainerSnoopHandler( Container_Snoop );
		}

		public static bool CheckSnoopAllowed( Mobile from, Mobile to )
		{
			Map map = from.Map;

			if ( to.Player )
				return from.CanBeHarmful( to, false, true ); // normal restrictions

			if ( map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0 )
				return true; // felucca you can snoop anybody

			GuardedRegion reg = (GuardedRegion) to.Region.GetRegion( typeof( GuardedRegion ) );

			if ( reg == null || reg.IsDisabled() )
				return true; // not in town? we can snoop any npc

			BaseCreature cret = to as BaseCreature;

			if ( to.Body.IsHuman && (cret == null || (!cret.AlwaysAttackable && !cret.AlwaysMurderer)) )
				return false; // in town we cannot snoop blue human npcs

			return true;
		}

		public static void Container_Snoop( Container cont, Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player || from.InRange( cont.GetWorldLocation(), 1 ) )
			{
				Mobile root = cont.RootParent as Mobile;

				if ( root != null && !root.Alive )
					return;

				#region mod by Dies Irae
				if( Stealing.IsHeavyEquipped( from ) )
				{
					from.SendMessage( from.Language == "ITA" ? "Solo un pazzo ruberebbe con un vestito tanto rumoroso!" : "Only a fool would steal in noisey armor!" );
					return;
				}

				// mod by Dies Irae
				// http://forums.uosecondage.com/viewtopic.php?f=7&t=1169&sid=df224cd1f1c4de80a5fff7dcd66ef498
				// Snooping now requires Line of Sight between you and victim
				if( !from.InLOS( cont ) )
				{
					from.SendLocalizedMessage( 500209 ); // You can not peek into the container.
					return;
				}
				#endregion

				if ( root != null && root.AccessLevel > AccessLevel.Player && from.AccessLevel == AccessLevel.Player )
				{
					from.SendLocalizedMessage( 500209 ); // You can not peek into the container.
					return;
				}

				if ( root != null && from.AccessLevel == AccessLevel.Player && !CheckSnoopAllowed( from, root ) )
				{
					from.SendLocalizedMessage( 1001018 ); // You cannot perform negative acts on your target.
					return;
				}

				#region mod by Dies Irae
				if( from.AccessLevel == AccessLevel.Player )
				{
					if( from is Midgard2PlayerMobile && !( (Midgard2PlayerMobile)from ).CanSnoop )
					{
						from.SendMessage( from.Language == "ITA" ? "Devi aspettare qualche altro secondo." : "You can not peek into the container for another few seconds." );
						return;
					}
				}

				bool caught = false;
				if( root != null )
				{
					double attSkill = (int)( ( from.Skills[ SkillName.Snooping ].Value + from.Dex ) / 2.0 );
					double defSkill = (int)( ( root.Skills[ SkillName.Snooping ].Value + root.Dex ) / 5.0 );

					if( !( root is PlayerMobile ) && defSkill > 60.0 )
						defSkill = 60.0;

					if( attSkill < defSkill )
						caught = true;
				}
				#endregion

				if( root != null && from.AccessLevel == AccessLevel.Player && ( caught || from.Skills[ SkillName.Snooping ].Value < Utility.Random( 100 ) ) )
				{
					Map map = from.Map;

					if( map != null )
					{
						string message = String.Format( from.Language == "ITA" ? "Noti che {0} sta tentando di borseggiare {1}." : "You notice {0} attempting to peek into {1}'s belongings.", from.Name, root.Name );

						IPooledEnumerable eable = map.GetClientsInRange( from.Location, 8 );

						foreach( NetState ns in eable )
						{
							if( ns.Mobile != from )
								ns.Mobile.SendMessage( message );
						}

						eable.Free();
					}

					#region mod by Dies Irae
					from.SendMessage( from.Language == "ITA" ? "Qualcuno ha notato le tue mani nello zaino di {0}." : "You are been noticed snooping into {0}'s belongings.", root.Name );

					if( ( from.Skills[ SkillName.Snooping ].Value < Utility.Random( 150 ) ) )
					{
						from.CriminalAction( false );

						TownSystem sys = TownSystem.Find( from.Location, from.Map );
						if( sys != null )
							sys.RegisterCriminal( from, CrimeType.StealAction );
					}
					#endregion
				}

				#region mod by Dies Irae
				if( from is Midgard2PlayerMobile )
					( (Midgard2PlayerMobile)from ).LastSnoopAction = DateTime.Now;

                /*
		        var karma:=GetKarma(who);
		        var lossKarma:=0;
		        if (karma>-625)
			        lossKarma:=-RandomInt(300)+1;
		        endif
		        AdjustKarma(who,lossKarma);
                 */

				if( from.Karma > -625 && from.AccessLevel == AccessLevel.Player )
					Titles.AwardKarma( from, -( Utility.Random( 30 ) + 1 ), true );

				//if ( from.AccessLevel == AccessLevel.Player )
				//    Titles.AwardKarma( from, -4, true );
				#endregion

				if ( from.AccessLevel > AccessLevel.Player || ( !caught && from.CheckTargetSkill( SkillName.Snooping, cont, 0.0, 100.0 ) ) )
				{
					if ( cont is TrapableContainer && ((TrapableContainer)cont).ExecuteTrap( from ) )
						return;

					cont.DisplayTo( from );
				}
				else
				{
					from.SendLocalizedMessage( 500210 ); // You failed to peek into the container.
					
					if ( from.Skills[SkillName.Hiding].Value / 2 < Utility.Random( 100 ) )
						from.RevealingAction( true );
				}
			}
			else
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
		}
	}
}