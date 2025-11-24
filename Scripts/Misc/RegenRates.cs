using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Mobiles;

namespace Server.Misc
{
	public class RegenRates
	{
		[CallPriority( 10 )]
		public static void Configure()
		{
			Mobile.DefaultHitsRate = TimeSpan.FromSeconds( 11.0 );
			Mobile.DefaultStamRate = TimeSpan.FromSeconds(  7.0 );
			Mobile.DefaultManaRate = TimeSpan.FromSeconds(  7.0 );

			Mobile.ManaRegenRateHandler = new RegenRateHandler( Mobile_ManaRegenRate );

			if ( Core.AOS )
			{
				Mobile.StamRegenRateHandler = new RegenRateHandler( Mobile_StamRegenRate );
				Mobile.HitsRegenRateHandler = new RegenRateHandler( Mobile_HitsRegenRate );
			}
		}

		private static void CheckBonusSkill( Mobile m, int cur, int max, SkillName skill )
		{
			if ( !m.Alive )
				return;

			double n = (double)cur / max;
			double v = Math.Sqrt( m.Skills[skill].Value * 0.005 );

			n *= (1.0 - v);
			n += v;

            if( !Core.AOS ) // Mod by Dies Irae
                n /= 50.0;

			m.CheckSkill( skill, n );
		}

		private static bool CheckTransform( Mobile m, Type type )
		{
			return TransformationSpellHelper.UnderTransformation( m, type );
		}

		private static bool CheckAnimal( Mobile m, Type type )
		{
			return AnimalForm.UnderTransformation( m, type );
		}

		private static TimeSpan Mobile_HitsRegenRate( Mobile from )
		{
			int points = AosAttributes.GetValue( from, AosAttribute.RegenHits );

			if ( from is BaseCreature && !((BaseCreature)from).IsAnimatedDead )
				points += 4;

			if ( (from is BaseCreature && ((BaseCreature)from).IsParagon) || from is Leviathan )
				points += 40;

			if ( CheckTransform( from, typeof( HorrificBeastSpell ) ) )
				points += 20;

			if ( CheckAnimal( from, typeof( Dog ) ) || CheckAnimal( from, typeof( Cat ) ) )
				points += from.Skills[SkillName.Ninjitsu].Fixed / 300;
			//TODO: What's the new increased rate?

			if( Core.ML && from.Race == Race.Human )	//Is this affected by the cap?
				points += 2;

			if ( points < 0 )
				points = 0;

			if( Core.ML && from is PlayerMobile )	//does racial bonus go before/after?
				points = Math.Min( points, 18 );

			if ( CheckTransform( from, typeof( HorrificBeastSpell ) ) )
				points += 20;

			if ( CheckAnimal( from, typeof( Dog ) ) || CheckAnimal( from, typeof( Cat ) ) )
				points += from.Skills[SkillName.Ninjitsu].Fixed / 30;

			return TimeSpan.FromSeconds( 1.0 / (0.1 * (1 + points)) );
		}

		private static TimeSpan Mobile_StamRegenRate( Mobile from )
		{
			if ( from.Skills == null )
				return Mobile.DefaultStamRate;

			CheckBonusSkill( from, from.Stam, from.StamMax, SkillName.Focus );

			int points =(int)(from.Skills[SkillName.Focus].Value * 0.1);

			if( (from is BaseCreature && ((BaseCreature)from).IsParagon) || from is Leviathan )
				points += 40;

			int cappedPoints = AosAttributes.GetValue( from, AosAttribute.RegenStam );

			if ( CheckTransform( from, typeof( VampiricEmbraceSpell ) ) )
				cappedPoints += 15;

			if ( CheckAnimal( from, typeof( Kirin ) ) )
				cappedPoints += 20;

			if( Core.ML && from is PlayerMobile )
				cappedPoints += (int)((from.Hunger + from.Thirst) * 0.225);
			
			if( Core.ML && from is PlayerMobile )
				cappedPoints = Math.Min( cappedPoints, 24 );

			points += cappedPoints;

			if ( points < -1 )
				points = -1;

			return TimeSpan.FromSeconds( 1.0 / (0.1 * (2 + points)) );
		}

		private static TimeSpan Mobile_ManaRegenRate( Mobile from )
		{
			if ( from.Skills == null )
				return Mobile.DefaultManaRate;

			if ( !from.Meditating && Core.AOS ) // mod by Dies Irae
				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Meditation );

			double rate;
			double armorPenalty = GetArmorOffset( from );

			if ( Core.AOS )
			{
				double medPoints = from.Int + (from.Skills[SkillName.Meditation].Value * 3);

				medPoints *= ( from.Skills[SkillName.Meditation].Value < 100.0 ) ? 0.025 : 0.0275;

				CheckBonusSkill( from, from.Mana, from.ManaMax, SkillName.Focus );

				double focusPoints = (from.Skills[SkillName.Focus].Value * 0.05);

				if ( armorPenalty > 0 )
					medPoints = 0; // In AOS, wearing any meditation-blocking armor completely removes meditation bonus

			    double totalPoints = AosAttributes.GetValue( from, AosAttribute.RegenMana ) + focusPoints + medPoints + (from.Meditating ? (medPoints > 13.0 ? 13.0 : medPoints) : 0.0);

				if( (from is BaseCreature && ((BaseCreature)from).IsParagon) || from is Leviathan )
					totalPoints += 40;

				int cappedPoints = AosAttributes.GetValue( from, AosAttribute.RegenMana );

				if ( CheckTransform( from, typeof( VampiricEmbraceSpell ) ) )
					cappedPoints += 3;
				else if ( CheckTransform( from, typeof( LichFormSpell ) ) )
					cappedPoints += 13;

                #region mod by Dies Irae : publish 46

                // The hard cap on MR has been removed.
                // if( Core.ML && from is PlayerMobile )
                //	cappedPoints = Math.Min( cappedPoints, 18 );

                // As the total MR from items increases, each successive point of MR will give less overall mana regeneration.
                // Having higher Meditation and / or Focus skill will give a bonus to mana regeneration gained from MR items. 
                if( Core.ML && from is PlayerMobile )
                {
                    double med = from.Skills[ SkillName.Meditation ].Value;
                    double foc = from.Skills[ SkillName.Focus ].Value;

                    double max = med;
                    if( foc > max )
                        max = foc;

                    double low = ( 3.0 * Math.Sqrt( cappedPoints ) - 1.2 );
                    double high = ( 4.0 * Math.Sqrt( cappedPoints ) - 2.0 );
                    double skillbonus = ( high - low ) * ( max / 120.0 );

                    cappedPoints = (int)( low + skillbonus );
                    if( cappedPoints < 0 )
                        cappedPoints = 0;
                    // double mrMed = med > 100.0 ? ( med * 3 + from.Int * 1.1 ) : ( med * 3 + from.Int );

                    // mrMed = Math.Floor( mrMed / 40.0 )
                    /*
                     * 
                    // funzione lineare decrescente.
                    // passa per 1,1.5 e 12,0.5
                    double scaleFactor = ( 0.09090909 * ( 1 - cappedPoints ) ) + 1.5;
                    if( scaleFactor > 1.5 )
                        scaleFactor = 1.5;
                    else if( scaleFactor < 0.3 )
                        scaleFactor = 0.3;

                    if( from.Skills[ SkillName.Focus ].Value >= 90.0 )
                        scaleFactor += 0.1;

                    if( from.Skills[ SkillName.Meditation ].Value >= 90.0 )
                        scaleFactor += 0.2;

                    // il manareg finale e' l'area sotto la funzione scaleFactor
                    cappedPoints = (int)( ( 1.5 + scaleFactor ) * ( cappedPoints - 1.0 ) * 0.5 );
                     * 
                     */
                }

                #endregion

				totalPoints += cappedPoints;

				if ( totalPoints < -1 )
					totalPoints = -1;

				if ( Core.ML )
					totalPoints = Math.Floor( totalPoints );

				rate = 1.0 / (0.1 * (2 + totalPoints));
			}
			else
			{
				double medPoints = (from.Int + from.Skills[SkillName.Meditation].Value) * 0.5;

				if ( medPoints <= 0 )
					rate = 7.0;
				else if ( medPoints <= 100 )
					rate = 7.0 - (239*medPoints/2400) + (19*medPoints*medPoints/48000);
				else if ( medPoints < 120 )
					rate = 1.0;
				else
					rate = 0.75;

				rate += armorPenalty;

				if ( from.Meditating )
					rate *= 0.5;

				if ( rate < 0.5 )
					rate = 0.5;
				else if ( rate > 7.0 )
					rate = 7.0;
			}

			return TimeSpan.FromSeconds( rate );
		}

		public static double GetArmorOffset( Mobile from )
		{
			double rating = 0.0;

			if ( !Core.AOS )
				rating += GetArmorMeditationValue( from.ShieldArmor as BaseArmor );

			rating += GetArmorMeditationValue( from.NeckArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.HandArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.HeadArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.ArmsArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.LegsArmor as BaseArmor );
			rating += GetArmorMeditationValue( from.ChestArmor as BaseArmor );

			return rating / 4;
		}

		private static double GetArmorMeditationValue( BaseArmor ar )
		{
			if ( ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0 )
				return 0.0;

			switch ( ar.MeditationAllowance )
			{
				default:
				case ArmorMeditationAllowance.None: return ar.BaseArmorRatingScaled;
				case ArmorMeditationAllowance.Half: return ar.BaseArmorRatingScaled / 2.0;
				case ArmorMeditationAllowance.All:  return 0.0;
			}
		}
	}
}