/***************************************************************************
 *							   AttuneWeapon.cs
 *							-------------------
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class AttuneWeaponSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Attune Weapon", "Ante Ohm Sepa",
			224,
			9011,
			Reagent.DestroyingAngel,
			Reagent.PetrifiedWood
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( AttuneWeaponSpell ),
				"Creates magical shield around the caster that absorbs melee damage.",
				"Il Druido crea uno scudo magico intorno a se, impedendogli di colpirlo."+
				"Assorbe (10 + (SW/5) + FL*2) danni",
				0x59db
			);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override SpellCircle Circle{get { return SpellCircle.Third; }}

		public AttuneWeaponSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if( !Caster.CanBeginAction( typeof( DefensiveSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005385 ); // The spell will not adhere to you at this time.
				return false;
			}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				Caster.PlaySound( 0x5C3 );
				Caster.FixedParticles( 0x3B40, 1, 15, 0x251E, 0x0, 7, EffectLayer.Waist );
				
				double skill = Caster.Skills[ SkillName.Spellweaving ].Value;
				int damageAbsorb = (int)( 10 + skill / 5 + ( FocusLevel * 2 ) );

				Caster.MeleeDamageAbsorb = damageAbsorb;

				Caster.SendMessage( (Caster.Language == "ITA" ? "Assorbirai fino a {0} punti di danno fisico." : "Thou will absorb up to {0} points of melee damage."), Caster.MeleeDamageAbsorb );
			}

			FinishSequence();
		}

		public static void TryAbsorb( Mobile defender, ref int damage )
		{
			if( damage == 0 || defender.MeleeDamageAbsorb <= 0 )
				return;

			int absorbed = Math.Min( damage, defender.MeleeDamageAbsorb );

			damage -= absorbed;
			defender.MeleeDamageAbsorb -= absorbed;

			defender.SendMessage( (defender.Language == "ITA" ? "{0} punti di danno sono stati assorbiti. Rimangono {1} punti di protezione." : "{0} point(s) of damage have been absorbed. A total of {1} point(s) of shielding remain."), absorbed, defender.MeleeDamageAbsorb );
		}
	}
}