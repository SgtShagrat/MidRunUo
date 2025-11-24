/***************************************************************************
 *							   ImmolatingWeapon.cs
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
using Server.Items;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class ImmolatingWeaponSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Immolating Weapon", "Vauk En Crur",
			224,
			9011,
			Reagent.Kindling,
			Reagent.PigIron
			);

		public override TimeSpan CastDelayBase{get { return TimeSpan.FromSeconds( 1 ); }}

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( ImmolatingWeaponSpell ),
				"Enchants a weapon with fire bursting enemy's hands if he is not good.",
				"Incanta temporaneamente un arco o arma da mischia bruciando le mani di qualsiasi cattivo la indossi."+
				"Durata (10 + SK/24 + FL + PL); Danno (10 + SK/24 + FL + PL).",
				0x59da
			);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override SpellCircle Circle{get { return SpellCircle.Fourth; }}

		public ImmolatingWeaponSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private readonly ImmolatingWeaponSpell m_Owner;

			public InternalTarget( ImmolatingWeaponSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;

				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}

		private void Target( Mobile mobile )
		{
			BaseWeapon weapon = mobile.Weapon as BaseWeapon;

			if( weapon == null || weapon is Fists )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Il tuo bersaglio deve indossare un'arma!" : "Your target must be wielding a weapon!") );
			}
			else if( CheckSequence() )
			{
				Caster.PlaySound( 0x5CA );
				Caster.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );

				int level = GetFocusLevel( Caster ) + GetPowerLevel();
				double skill = Caster.Skills[ SkillName.Spellweaving ].Value;

				TimeSpan duration = TimeSpan.FromSeconds( 10 + Math.Min( (int)( skill / 24 ), 1 ) + level );
				int damage = 10 + Math.Min( (int)Math.Floor( skill / 24 ), 1 ) + level;

				ImmolatingWeaponEntry entry = m_Table[ weapon ] as ImmolatingWeaponEntry;

				if( entry != null )
					entry.Timer.Stop();

				weapon.Immolating = true;

				m_Table[ weapon ] = new ImmolatingWeaponEntry( damage, weapon, duration );

				new InternalTimer( this,Caster, duration, weapon, damage ).Start();
			}

			FinishSequence();
		}

		private static readonly Hashtable m_Table = new Hashtable();

		public static int GetDamage( BaseWeapon weapon )
		{
			ImmolatingWeaponEntry entry = m_Table[ weapon ] as ImmolatingWeaponEntry;

			if( entry != null )
				return entry.Damage;

			return 0;
		}

		public class ImmolatingWeaponEntry
		{
			public int Damage;
			public Timer Timer;

			public ImmolatingWeaponEntry( int damage, BaseWeapon weapon, TimeSpan duration )
			{
				Damage = damage;

				Timer = new ExpireTimer( weapon, duration );
				Timer.Start();
			}
		}

		private class ExpireTimer : Timer
		{
			private readonly BaseWeapon m_Weapon;

			public ExpireTimer( BaseWeapon weapon, TimeSpan delay ) : base( delay )
			{
				m_Weapon = weapon;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Weapon.Immolating = false;
				m_Table.Remove( m_Weapon );
			}
		}

		private class InternalTimer : Timer
		{
			private readonly DateTime m_Expiration;
			private readonly Mobile m_Caster;
			private readonly BaseWeapon m_Weapon;
			private readonly int m_Damage;
			private readonly Spell m_Source;

			public InternalTimer( Spell source, Mobile caster, TimeSpan duration, BaseWeapon weapon, int damage )
				: base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ) )
			{
				m_Source = source;
				
				Priority = TimerPriority.OneSecond;

				m_Expiration = DateTime.Now + duration;

				m_Caster = caster;
				m_Weapon = weapon;
				m_Damage = damage;
			}

			protected override void OnTick()
			{
				if( DateTime.Now > m_Expiration )
				{
					Stop();
					return;
				}

				Mobile from = m_Weapon.Parent as Mobile;
				if( from == null )
					return;

				int noto = Notoriety.Compute( m_Caster, from );
				if( noto == Notoriety.Enemy || noto == Notoriety.Murderer || noto == Notoriety.Criminal || from.PlayerDebug )
				{
					Effects.PlaySound( from.Location, from.Map, 0x207 );
					Effects.SendLocationEffect( from.Location, from.Map, 0x36BD, 20 );
					MidgardSpellHelper.Damage( m_Source, from, m_Damage, SpellType.Fire );

					from.SendMessage( (from.Language == "ITA" ? "La tua arma è in fiamme!" : "Your weapon is burning!") );
				}
			}
		}

		public static void AlterDamage( BaseWeapon weapon, Mobile attacker, Mobile defender, ref int damage )
		{
			if( !weapon.Immolating )
				return;

			int d = GetDamage( weapon );
			d = AOS.Damage( defender, attacker, d, 0, 100, 0, 0, 0 );

			AttuneWeaponSpell.TryAbsorb( defender, ref d );

			if( d > 0 )
				defender.Damage( d );
		}
	}
}
