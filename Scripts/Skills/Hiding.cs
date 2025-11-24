using System;

using Midgard.Engines.Classes;
using Midgard.Items;

using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Multis;

namespace Server.SkillHandlers
{
	public class Hiding
	{
		private static bool m_CombatOverride;

		public static bool CombatOverride
		{
			get{ return m_CombatOverride; }
			set{ m_CombatOverride = value; }
		}

		public static void Initialize()
		{
		    if( Core.AOS ) // mod by Dies Irae
			    SkillInfo.Table[21].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			if ( m.Spell != null )
			{
				m.SendLocalizedMessage( 501238 ); // You are busy doing something else and cannot hide.
				return TimeSpan.FromSeconds( 1.0 );
			}

			if ( /*Core.ML && */ m.Target != null )
			{
				Targeting.Target.Cancel( m );
			}

			#region mod by Dies Irae
			if( m.Frozen || m.Paralyzed )
			{
				m.LocalOverheadMessage( MessageType.Regular, 0x22, 501237 ); // You can't seem to hide right now.
				return TimeSpan.FromSeconds( 1.0 );
			}
			#endregion

			double bonus = 0.0;

			BaseHouse house = BaseHouse.FindHouseAt( m );

			if ( house != null && house.IsFriend( m ) )
			{
				bonus = 100.0;
			}
			else if ( !Core.AOS )
			{
				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( m.X - 1, m.Y, 127 ), m.Map, 16 );

				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( m.X + 1, m.Y, 127 ), m.Map, 16 );

				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( m.X, m.Y - 1, 127 ), m.Map, 16 );

				if ( house == null )
					house = BaseHouse.FindHouseAt( new Point3D( m.X, m.Y + 1, 127 ), m.Map, 16 );

				if ( house != null )
					bonus = 50.0;
			}

			#region mod by Dies Irae
			if( m.Mounted && m.Skills[ SkillName.Hiding ].Value < 80.0 )
			{
				m.LocalOverheadMessage( MessageType.Regular, 0x22, true, m.Language == "ITA" ? "Non sei abbastanza abile per nasconderti cavalcando." : "You are too unexperted to hide while mounted." );
				return TimeSpan.FromSeconds( 1.0 );
			}

			int armorRating = Stealth.GetArmorRating( m );

			if( armorRating >= 42 )
			{
				m.SendLocalizedMessage( 502727 ); // You could not hope to move quietly wearing this much armor.
				m.RevealingAction( true );
				return TimeSpan.FromSeconds( 1.0 );
			}

			if( ClassSystem.IsScout( m ) && ScoutMimeticPaint.IsUnderMimetism( m ) && ScoutSystem.IsInForest( m ) )
				bonus = 100;
			#endregion

			//int range = 18 - (int)(m.Skills[SkillName.Hiding].Value / 10);
			int range = Math.Min( (int)((100 - m.Skills[SkillName.Hiding].Value)/2) + 8, 18 );	//Cap of 18 not OSI-exact, intentional difference

			bool badCombat = ( !m_CombatOverride && m.Combatant != null && m.InRange( m.Combatant.Location, range ) && m.Combatant.InLOS( m ) );
			bool ok = ( !badCombat /*&& m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus )*/ );

			bool oldCheckCombat = ThiefSystem.CheckCombat( m, (int)(13 - m.Skills[SkillName.Hiding].Value / 10 ));
			int minCheckCombatMalus = 90;
			int maxCheckCombatMalus = 40;

			if ( ok )
			{
				/*
				if ( !m_CombatOverride )
				{
					foreach ( Mobile check in m.GetMobilesInRange( range ) )
					{
						if ( check.InLOS( m ) && check.Combatant == m )
						{
							badCombat = true;
							ok = false;
							break;
						}
					}
				}
				*/

				#region mod by Dies Irae
				double malus = ThiefSystem.GetNearbyMobilesMalus( m );
				double value = m.Skills[ SkillName.Hiding ].Value;
				double minSkill = -20.0 + ( armorRating * 2 ) + ( oldCheckCombat ? minCheckCombatMalus : 0 );
				double maxSkill = 80.0 + ( armorRating * 2 ) + ( oldCheckCombat ? maxCheckCombatMalus : 0 );

				double chance = ( ( value - minSkill ) / ( maxSkill - minSkill ) ) - malus;

				// ok = ( !badCombat && m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus ) );
				// ok = !badCombat && m.CheckSkill( SkillName.Hiding, -20.0 + ( armorRating * 2 ), 80 + ( armorRating * 2 ) ); // mod by Dies Irae
				ok = !badCombat && m.CheckSkill( SkillName.Hiding, chance );

				if( m.PlayerDebug )
					m.SendMessage( "Hiding debug: malus: {0:F2} - value {1:F1} - chance {2:F2} - success {3}.", malus, value, chance, ok );

				if( !ok && bonus > 0 )
					ok = m.CheckSkill( SkillName.Hiding, 0.0 - bonus, 100.0 - bonus );
				#endregion
			}
            
			if ( badCombat )
			{
				m.RevealingAction( true );

				m.LocalOverheadMessage( MessageType.Regular, 0x22, 501237 ); // You can't seem to hide right now.

				return TimeSpan.FromSeconds( 1.0 );
			}
			else 
			{
				if ( ok )
				{
					m.Hidden = true;
					m.Warmode = false;
					m.LocalOverheadMessage( MessageType.Regular, 0x1F4, 501240 ); // You have hidden yourself well.
				}
				else
				{
					m.RevealingAction( true );

					m.LocalOverheadMessage( MessageType.Regular, 0x22, 501241 ); // You can't seem to hide here.
				}

				return TimeSpan.FromSeconds( 10.0 );
			}
		}
	}
}