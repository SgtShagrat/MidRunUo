/***************************************************************************
 *							   Dies Irae - OldRegenRates.cs
 *
 *   begin				: 20 luglio 2010
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;
using Midgard.Engines.SpellSystem;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Misc
{
	public class OldRegenRates
	{
		[CallPriority( 11 )]
		public static void Configure()
		{
			if( Core.AOS )
				return;

			Mobile.ManaRegenRateHandler = new RegenRateHandler( MobileManaRegenRate );
			Mobile.StamRegenRateHandler = new RegenRateHandler( MobileStamRegenRate );
			Mobile.HitsRegenRateHandler = new RegenRateHandler( MobileHitsRegenRate );
		}

		private static bool CheckTransform( Mobile m, Type type )
		{
			return TransformationSpellHelper.UnderTransformation( m, type );
		}

		private static TimeSpan MobileHitsRegenRate( Mobile from )
		{
			if ( from is BaseCreature && !((BaseCreature)from).Controlled )
				return TimeSpan.FromSeconds( 3 );

			int points = 0;

			if( from is BaseCreature && !( (BaseCreature)from ).IsAnimatedDead )
				points += 4;

			if( CheckTransform( from, typeof( EvilAvatarSpell ) ) )
				points += 20;

			if( from is BaseCreature )
				points += ( (BaseCreature)from ).HitsRegenBonus;

			if( from is PlayerMobile )
				points += (int)( ( from.Hunger + from.Thirst ) * 0.05 );

			if( from.Player && ClassSystem.IsPaladine( from ) )
			{
				if( BaseMagicalFood.IsUnderInfluence( from, MagicalFood.HolyBread ) )
					points += RPGPaladinSpell.GetPowerLevelByType( from, typeof( SacredFeastSpell ) ) * 2;
			}

			if( points < 0 )
				points = 0;

			return TimeSpan.FromSeconds( 1.0 / ( 0.1 * ( 1 + points ) ) );
		}

		private static TimeSpan MobileStamRegenRate( Mobile from )
		{
			if( from.Skills == null )
				return Mobile.DefaultStamRate;

			if ( from is BaseCreature && !((BaseCreature)from).Controlled )
				return TimeSpan.FromSeconds( 3 );

			int points = 0;

			if( from is BaseCreature )
				points += ( (BaseCreature)from ).StaminaRegenBonus;

			if( from is PlayerMobile )
				points += (int)( ( from.Hunger + from.Thirst ) * 0.05 );

			if( points < -1 )
				points = -1;

			if( from.Player && ClassSystem.IsPaladine( from ) )
			{
				if( BaseMagicalFood.IsUnderInfluence( from, MagicalFood.HolyApple ) )
					points += RPGPaladinSpell.GetPowerLevelByType( from, typeof( SacredFeastSpell ) ) * 2;
			}

			return TimeSpan.FromSeconds( 1.0 / ( 0.1 * ( 2 + points ) ) );
		}

		private static TimeSpan MobileManaRegenRate( Mobile from )
		{
			if( from.Skills == null )
				return Mobile.DefaultManaRate;

			if ( from is BaseCreature && !((BaseCreature)from).Controlled )
				return TimeSpan.FromSeconds( 3 );

			double points = 0.0;

			bool isPaladine = from.Player && ClassSystem.IsPaladine( from );
			bool hasPrayed = false;
			bool hasHolyHorse = false;

			if( isPaladine )
			{
                ClassPlayerState state = ClassPlayerState.Find( from );
				if( state != null )
					hasPrayed = state.HasPrayed;

				if( from.Mount is HolyMount )
					hasHolyHorse = true;

				if( BaseMagicalFood.IsUnderInfluence( from, MagicalFood.HolyGrapes ) )
					points += RPGPaladinSpell.GetPowerLevelByType( from, typeof( SacredFeastSpell ) ) * 2;
			}

			points += ( from.Int + from.Skills[ SkillName.Meditation ].Value ) * 0.5;

            if( isPaladine )
                points = RPGPaladinSpell.GetMeditationPoints( from );

			if( from is BaseCreature )
				points += ( (BaseCreature)from ).ManaRegenBonus;

			double rate;
			double armorPenalty = GetArmorOffset( from );

			if( points <= 0 )
				rate = 7.0;
			else if( points <= 100 )
				rate = 7.0 - ( 239 * points / 2400 ) + ( 19 * points * points / 48000 );
			else if( points < 120 )
				rate = 1.0;
			else
				rate = 0.75;

			rate += armorPenalty;

			if( from.Meditating )
				rate *= 0.10;
			#region mod by Magius(CHE)
			else if( from.Skills[ SkillName.Meditation ].Value == 100.0 && from.Hidden && ( from is PlayerMobile ) && ( ( (PlayerMobile)from ).IsStealthing ) )
				rate *= 0.20;
			#endregion

			if( hasPrayed || hasHolyHorse )
				rate *= 0.33;

			if( rate < 0.10 )
				rate = 0.10;
			else if( rate > 7.0 )
				rate = 7.0;

			if( !from.Player && rate < 0.5 )
				rate = 0.5;

			//if ( DateTime.Now < (from.LastMoveTime + TimeSpan.FromSeconds(0.25)))//mezzo mana regen se si muove
			//	rate *= 2;

			return TimeSpan.FromSeconds( rate );
		}

		private static double GetArmorOffset( Mobile from )
		{
			return GetArmorMeditationValue( from.ShieldArmor as BaseArmor );
		}

		private static double GetArmorMeditationValue( BaseArmor ar )
		{
			if( ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0 )
				return 0.0;

			if( ar.RequiredClass == Classes.Paladin || ar.IsXmlHolyArmor )
				return 0.0;

			switch( ar.MeditationAllowance )
			{
				default:
				case ArmorMeditationAllowance.None: return ar.BaseArmorRatingScaled;
				case ArmorMeditationAllowance.Half: return ar.BaseArmorRatingScaled / 2.0;
				case ArmorMeditationAllowance.All: return 0.0;
			}
		}
	}
}