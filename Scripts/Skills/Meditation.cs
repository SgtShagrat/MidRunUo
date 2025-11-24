using System;

using Midgard.Engines.SpellSystem;
using Midgard.Items;

using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
	class Meditation
	{
		public static void Initialize()
		{
			SkillInfo.Table[46].Callback = new SkillUseCallback( OnUse );
		}

		public static bool CheckOkayHolding( Item item )
		{
			if ( item == null )
				return true;

			if ( item is Spellbook || item is Runebook || item is MageStaff || item is RodOfIniquity )
				return true;

			#region mod by Dies Irae
			if( item is LightStaff || item is DarkenStaff )
				return true;
			#endregion

			if ( /* Core.AOS && */ item is BaseWeapon && ((BaseWeapon)item).Attributes.SpellChanneling != 0 )
				return true;

			if ( /* Core.AOS && */ item is BaseArmor && ((BaseArmor)item).Attributes.SpellChanneling != 0 )
				return true;

			#region mod by Dies Irae
			if( item is CustomSpellbook )
				return true;
			#endregion

			return false;
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.RevealingAction( true );

			if ( m.Target != null )
			{
				m.SendLocalizedMessage( 501845 ); // You are busy doing something else and cannot focus.

				return TimeSpan.FromSeconds( 5.0 );
			} 
			else if ( !Core.AOS && m.Hits < (m.HitsMax / 10) ) // Less than 10% health
			{
				m.SendLocalizedMessage( 501849 ); // The mind is strong but the body is weak.

				return TimeSpan.FromSeconds( 5.0 );
			}
			else if ( m.Mana >= m.ManaMax )
			{
				m.SendLocalizedMessage( 501846 ); // You are at peace.

				return TimeSpan.FromSeconds( Core.AOS ? 10.0 : 5.0 );
			}
			else if ( Core.AOS && Server.Misc.RegenRates.GetArmorOffset( m ) > 0 )
			{
				m.SendLocalizedMessage( 500135 ); // Regenative forces cannot penetrate your armor!

				return TimeSpan.FromSeconds( 10.0 );
			}
			#region mod by Dies Irae
			else if( m.BAC > 0 )
			{
				m.SendMessage( m.Language == "ITA" ? "L'ebbrezza non conduce al nirvana." : "Inebriation is not conducive to meditation." );

				return TimeSpan.FromSeconds( 10.0 );
			}
			#endregion
			else 
			{
				Item oneHanded = m.FindItemOnLayer( Layer.OneHanded );
				Item twoHanded = m.FindItemOnLayer( Layer.TwoHanded );

				if ( Core.AOS )
				{
					if ( !CheckOkayHolding( oneHanded ) )
						m.AddToBackpack( oneHanded );

					if ( !CheckOkayHolding( twoHanded ) )
						m.AddToBackpack( twoHanded );
				}
				else if ( !CheckOkayHolding( oneHanded ) || !CheckOkayHolding( twoHanded ) )
				{
					// mod by Dies Irae
					// http://forum.uosecondage.com/viewtopic.php?f=7&t=5491
					// "Your hands must be free to cast spells or meditate." now appears above the players head for mediation
					// m.SendLocalizedMessage( 502626 ); // Your hands must be free to cast spells or meditate.
					m.PrivateOverheadMessage( MessageType.Regular, 0x3B2, 502626, m.NetState );
					return TimeSpan.FromSeconds( 2.5 );
				}

				double skillVal = m.Skills[SkillName.Meditation].Value;
				double chance = (50.0 + (( skillVal - ( m.ManaMax - m.Mana ) ) * 2)) / 100;

				#region mod by Dies Irae
				if( Core.T2A )
					chance = Math.Max( skillVal / 100.0, 0.5 );
				#endregion

				#region mod by Dies Irae : Publish 46
				if( m.Int >= 100 && m.Mana < m.ManaMax * 0.25 )
					chance *= 2.0;
				#endregion

				if ( chance > Utility.RandomDouble() )
				{
					m.CheckSkill( SkillName.Meditation, chance / 3.0 ); // mod by Dies Irae
					
					#region modifica by dies Irae
					m.PublicOverheadMessage( MessageType.Regular, 0x3B2, true , m.Language == "ITA" ? "*Meditando*" : "*Meditating*" );
					#endregion
					
					m.SendLocalizedMessage( 501851 ); // You enter a meditative trance.
					m.Meditating = true;
					BuffInfo.AddBuff( m, new BuffInfo( BuffIcon.ActiveMeditation, 1075657 ) );

					if ( m.Player || m.Body.IsHuman )
						m.PlaySound( 0xF9 );
				} 
				else 
				{
					m.SendLocalizedMessage( 501850 ); // You cannot focus your concentration.
				}

				return TimeSpan.FromSeconds( 10.0 );
			}
		}
	}
}