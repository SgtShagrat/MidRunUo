/***************************************************************************
 *							   DruidEmpowermentSpell.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Spells;

using Midgard.Gumps;

namespace Midgard.Engines.SpellSystem
{
	public class DruidEmpowermentSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Druid Empowerment", "Sepa En Crur",
			224,
			9011,
			Reagent.Kindling,
			Reagent.FertileDirt,
			Reagent.SpringWater
			);


		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
		(
			typeof( DruidEmpowermentSpell ),
			"Enhances the caster's healing/damaging spells and increases the toughness of summons.",
			"Il Druido riceve un bonus al danno e al potere curativo delle sue magie."+
			"Durata (15 + AnLor*(PL+1)) secondi.",
			0x59e7
		);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Third; }}

		public DruidEmpowermentSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( m_Table.ContainsKey( Caster ) )
			{
				Caster.SendLocalizedMessage( 501775 ); // This spell is already in effect.
			}
			else if( CheckSequence() )
			{
				Caster.PlaySound( 0x5C1 );
				Caster.SendMessage( (Caster.Language == "ITA"? "La natura ti dà forza." : "Thou are empowered in Nature.") );
				Caster.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );

				int level = GetPowerLevel();
				double skill = Caster.Skills[ SkillName.AnimalLore ].Value;

				TimeSpan duration = TimeSpan.FromSeconds( 15 + (int)( skill )*(level+1) );
				int bonus = (int)Math.Floor( skill / 12 ) + level * 5;

				m_Table[ Caster ] = new EmpowermentInfo( Caster, duration, bonus, level );
			}

			FinishSequence();
		}

		private static readonly Hashtable m_Table = new Hashtable();

		public static int GetGenericBonus( Mobile m )
		{
			EmpowermentInfo info = m_Table[ m ] as EmpowermentInfo;

			if( info != null )
				return 1;

			return 0;
		}

		public static bool HasBonus( Mobile m )
		{
			EmpowermentInfo info = m_Table[ m ] as EmpowermentInfo;

			if( info != null )
				return true;

			return false;
		}

		public static int GetSpellBonus( Mobile m, bool playerVsPlayer )
		{
			EmpowermentInfo info = m_Table[ m ] as EmpowermentInfo;

			if( info != null )
				return info.Bonus + ( playerVsPlayer ? info.Focus : 0 );

			return 0;
		}

		public static void AddHealBonus( Mobile m, ref int toHeal )
		{
			EmpowermentInfo info = m_Table[ m ] as EmpowermentInfo;

			if( info != null )
				toHeal = (int)Math.Floor( ( 1 + ( 10 + info.Bonus ) / 100.0 ) * toHeal );
		}

		public static void RemoveBonus( Mobile m )
		{
			EmpowermentInfo info = m_Table[ m ] as EmpowermentInfo;

			if( info != null && info.Timer != null )
				info.Timer.Stop();

			m_Table.Remove( m );
		}

		private class EmpowermentInfo
		{
			public readonly int Bonus;
			public readonly int Focus;
			public readonly ExpireTimer Timer;

			public EmpowermentInfo( Mobile caster, TimeSpan duration, int bonus, int focus )
			{
				Bonus = bonus;
				Focus = focus;
				caster.SendGump(new DruidEmpowermentGump());

				Timer = new ExpireTimer( caster, duration );
				Timer.Start();
			}
		}

		private class ExpireTimer : Timer
		{
			private readonly Mobile m_Mobile;

			public ExpireTimer( Mobile m, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				if (m_Mobile.HasGump(typeof(DruidEmpowermentGump)))
					m_Mobile.CloseGump(typeof(DruidEmpowermentGump));

				m_Mobile.PlaySound( 0x5C2 );
				m_Table.Remove( m_Mobile );
			}
		}
	}
}