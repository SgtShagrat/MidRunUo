// #define Kamuflaro

using System;
using System.Collections.Generic;

namespace Server.Items
{
#if Kamuflaro
	public static class BladeweaveHelper
	{
		private static Dictionary<Mobile, WeaponAbility> m_AbilityLinks = new Dictionary<Mobile, WeaponAbility>();

		public static void ClearAbilityLink( Mobile from )
		{
			if ( from != null )
				m_AbilityLinks.Remove( from );
		}

		public static bool IsUsingBladeweave( Mobile from )
		{
			if ( from != null && m_AbilityLinks.ContainsKey( from ) )
				return m_AbilityLinks[ from ] != null;

			return false;
		}

		public static WeaponAbility GetAbilityLink( Mobile from )
		{
			if ( from != null && m_AbilityLinks.ContainsKey( from ) )
				return m_AbilityLinks[ from ];

			return null;
		}

		public static void SetAbilityLink( Mobile from, WeaponAbility ability )
		{
			if ( from != null )
				m_AbilityLinks[ from ] = ability;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Bladeweave : WeaponAbility
	{
		public Bladeweave()
		{
		}

		public override bool Validate( Mobile from )
		{
			if ( !base.Validate( from ) )
				return false;

			if ( !BladeweaveHelper.IsUsingBladeweave( from ) )
			{
				List<WeaponAbility> list =  AvailableAbilities( from );

				if ( list == null || list.Count <= 0 )
					return false;

				BladeweaveHelper.SetAbilityLink( from, list[Utility.Random( list.Count )] );
			}

			return true;
		}

		public override int BaseMana { get { return 25; } }

		public override bool OnBeforeSwing( Mobile attacker, Mobile defender )
		{
			WeaponAbility a = BladeweaveHelper.GetAbilityLink( attacker );
			if ( a != null )
				return a.OnBeforeSwing( attacker, defender );

			return true;
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			WeaponAbility a = BladeweaveHelper.GetAbilityLink( attacker );
			if ( a != null )
				a.OnMiss( attacker, defender );

			ClearCurrentAbility( attacker );
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if ( !Validate( attacker ) || !CheckMana( attacker, true ) )
				return;

			WeaponAbility a = BladeweaveHelper.GetAbilityLink( attacker );
			if ( a != null )
				a.OnHit( attacker, defender, damage );

			ClearCurrentAbility( attacker );
		}

		private List<WeaponAbility> AvailableAbilities( Mobile from )
		{
			if ( from == null || !(from.Weapon is BaseWeapon) )
				return null; // Sanity

			List<WeaponAbility> list = new List<WeaponAbility>();
			bool hasNinjitsu = GetSkill( from, SkillName.Ninjitsu ) >= 50.0;
			bool hasBushido = GetSkill( from, SkillName.Bushido ) >= 50.0;
			bool hasBow = from.Weapon is BaseRanged;

			list.Add( WeaponAbility.ArmorIgnore );

			if( hasNinjitsu || hasBushido )
			{
				list.Add( WeaponAbility.ArmorPierce );
				list.Add( WeaponAbility.Block );
				list.Add( WeaponAbility.DefenseMastery );
				list.Add( WeaponAbility.Feint );

				if ( from.Weapon is BaseRanged )
					list.Add( WeaponAbility.DoubleShot );
				else
					list.Add( WeaponAbility.FrenziedWhirlwind );

				if ( hasNinjitsu )
				{
					list.Add( WeaponAbility.TalonStrike );
					if ( !hasBow && ((BaseWeapon)from.Weapon).Layer == Layer.TwoHanded )
						list.Add( WeaponAbility.DualWield );
				}

				if ( hasBushido )
				{
					list.Add( WeaponAbility.NerveStrike );
					list.Add( WeaponAbility.RidingSwipe );
				}
			}

			list.Add( WeaponAbility.BleedAttack );
			list.Add( WeaponAbility.CrushingBlow );
			list.Add( WeaponAbility.ConcussionBlow );
			list.Add( WeaponAbility.Disarm );
			list.Add( WeaponAbility.Dismount );
			list.Add( WeaponAbility.DoubleStrike );
			list.Add( WeaponAbility.MortalStrike );
			list.Add( WeaponAbility.ParalyzingBlow );
			list.Add( WeaponAbility.PsychicAttack );

			if ( from.Skills[SkillName.Stealth].Value >= 80 )
				list.Add( WeaponAbility.ShadowStrike );
				

			if ( hasBow )
			{
				list.Add( WeaponAbility.ForceArrow );
				list.Add( WeaponAbility.ForceOfNature );
				list.Add( WeaponAbility.LightningArrow );
				list.Add( WeaponAbility.SerpentArrow );
				//list.Add( WeaponAbility.MovingShot );
			}
			else
				list.Add( WeaponAbility.WhirlwindAttack );

			if ( ((BaseWeapon)from.Weapon).Poison != null && ((BaseWeapon)from.Weapon).PoisonCharges > 0 )
				list.Add( WeaponAbility.InfectiousStrike );

			return list;
		}
	}
#else
	/// <summary>
	/// 
	/// </summary>
	public class Bladeweave : WeaponAbility
	{
		private class BladeWeaveRedirect
		{
			public WeaponAbility NewAbility;
			public int ClilocEntry;

			public BladeWeaveRedirect( WeaponAbility ability, int cliloc )
			{
				NewAbility = ability;
				ClilocEntry = cliloc;
			}
		}

		private static Dictionary<Mobile, BladeWeaveRedirect> m_NewAttack = new Dictionary<Mobile, BladeWeaveRedirect>();

		public static bool BladeWeaving( Mobile attacker, out WeaponAbility a )
		{
			BladeWeaveRedirect bwr;
			bool success = m_NewAttack.TryGetValue( attacker, out bwr );
			if( success )
				a = bwr.NewAbility;
			else
				a = null;

			return success;
		}

		public Bladeweave()
		{
		}

		public override int BaseMana { get { return 30; } }

		public override bool OnBeforeSwing( Mobile attacker, Mobile defender )
		{
			if( !Validate( attacker ) || !CheckMana( attacker, false ) )
				return false;

			switch( Utility.Random( 9 ) )
			{
				case 0:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.ArmorIgnore, 1028838 );
					break;
				case 1:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.BleedAttack, 1028839 );
					break;
				case 2:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.ConcussionBlow, 1028840 );
					break;
				case 3:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.CrushingBlow, 1028841 );
					break;
				case 4:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.DoubleStrike, 1028844 );
					break;
				case 5:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.MortalStrike, 1028846 );
					break;
				case 6:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.ParalyzingBlow, 1028848 );
					break;
				case 7:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.Feint, 1028857 );
					break;
				case 8:
					m_NewAttack[ attacker ] = new BladeWeaveRedirect( WeaponAbility.Block, 1028853 );
					break;
				default:
					return false;
			}

			return ( ( BladeWeaveRedirect )m_NewAttack[ attacker ] ).NewAbility.OnBeforeSwing( attacker, defender );
		}

		public override bool OnBeforeDamage( Mobile attacker, Mobile defender )
		{
			BladeWeaveRedirect bwr;
			if( m_NewAttack.TryGetValue( attacker, out bwr ) )
				return bwr.NewAbility.OnBeforeDamage( attacker, defender );
			else
				return base.OnBeforeDamage( attacker, defender );
		}

		public override void OnHit( Mobile attacker, Mobile defender, int damage )
		{
			if( CheckMana( attacker, true ) )
			{
				BladeWeaveRedirect bwr;
				if( m_NewAttack.TryGetValue( attacker, out bwr ))
				{
					attacker.SendLocalizedMessage( 1072841, "#" + bwr.ClilocEntry.ToString() );
					bwr.NewAbility.OnHit( attacker, defender, damage );
				}
				else
					base.OnHit( attacker, defender, damage );

				m_NewAttack.Remove( attacker );
				ClearCurrentAbility( attacker );
			}
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			BladeWeaveRedirect bwr;
			if( m_NewAttack.TryGetValue( attacker, out bwr ) )
				bwr.NewAbility.OnMiss( attacker, defender );
			else
				base.OnMiss( attacker, defender );

			m_NewAttack.Remove( attacker );
		}
	}
#endif
}
