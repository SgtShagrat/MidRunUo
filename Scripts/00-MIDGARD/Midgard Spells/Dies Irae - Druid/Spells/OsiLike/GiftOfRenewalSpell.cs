/***************************************************************************
 *							   GiftOfRenewal.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Spells;
using Server.Targeting;

using Midgard.Gumps;

namespace Midgard.Engines.SpellSystem
{
	public class GiftOfRenewalSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Gift of Renewal", "Aete En Telwa",
			224,
			9011,
			Reagent.Ginseng,
			Reagent.FertileDirt,
			Reagent.SpringWater
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( GiftOfRenewalSpell ),
				"Heals a target repeatedly for a short period of time.",
				"Questo arteficio è in grado di velocizzare la rigenerazione delle ferite e di neutralizzare i veleni."+
				"Durata (30 + FL*10 + PL*5); HP (5+ PL*2 + SK/24 + FL).",
				0x59d9
			);

		public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

		public override SpellCircle Circle{get { return SpellCircle.Fifth; }}
		public override bool BlocksMovement{get { return true; }}

		public GiftOfRenewalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( m_Table.ContainsKey( m ) )
			{
				Caster.SendLocalizedMessage( 501775 ); // This spell is already in effect.
			}
			else if( !Caster.CanBeginAction( typeof( GiftOfRenewalSpell ) ) )
			{
				Caster.SendLocalizedMessage( 501789 ); // You must wait before trying again.
			}
			else if( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				Caster.FixedEffect( 0x374A, 10, 20 );
				Caster.PlaySound( 0x5C9 );

				if( m.Poisoned )
				{
					m.CurePoison( m );
				}
				else
				{
					double skill = Caster.Skills[ CastSkill ].Value;
					int level = GetPowerLevel();
					int hitsPerRound = 5 + ( level * 2 ) + (int)( skill / 24 ) + FocusLevel;

					TimeSpan duration = TimeSpan.FromSeconds( 30 + ( FocusLevel * 10 ) + ( level * 5 ) );

					DruidEmpowermentSpell.AddHealBonus( Caster, ref hitsPerRound );

					GiftOfRenewalInfo info = new GiftOfRenewalInfo( Caster, m, hitsPerRound );

					Caster.SendGump(new GiftOfRenewalGump());
					if (Caster != m && m.Player)
						m.SendGump(new GiftOfRenewalGump());

					Timer.DelayCall( duration,
							delegate
							{
								if( StopEffect( m ) )
								{
									if (Caster.HasGump(typeof(GiftOfRenewalGump)))
										Caster.CloseGump(typeof(GiftOfRenewalGump));
									if (Caster != m && m.Player && m.HasGump(typeof(GiftOfRenewalGump)))
										m.CloseGump(typeof(GiftOfRenewalGump));

									m.PlaySound( 0x455 );
									m.SendLocalizedMessage( 1075071 ); // The Gift of Renewal has faded.
								}
							} );

					m_Table[ m ] = info;

					Caster.BeginAction( typeof( GiftOfRenewalSpell ) );
				}
			}

			FinishSequence();
		}

		private static readonly Dictionary<Mobile, GiftOfRenewalInfo> m_Table = new Dictionary<Mobile, GiftOfRenewalInfo>();

		private class GiftOfRenewalInfo
		{
			public readonly Mobile m_Caster;
			public readonly Mobile m_Mobile;
			public readonly int m_HitsPerRound;
			public readonly InternalTimer m_Timer;

			public GiftOfRenewalInfo( Mobile caster, Mobile mobile, int hitsPerRound )
			{
				m_Caster = caster;
				m_Mobile = mobile;
				m_HitsPerRound = hitsPerRound;

				m_Timer = new InternalTimer( this );
				m_Timer.Start();
			}
		}

		private class InternalTimer : Timer
		{
			private readonly GiftOfRenewalInfo m_GiftInfo;

			public InternalTimer( GiftOfRenewalInfo giftInfo ) : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.0 ) )
			{
				m_GiftInfo = giftInfo;
			}

			protected override void OnTick()
			{
				Mobile m = m_GiftInfo.m_Mobile;

				if( !m_Table.ContainsKey( m ) )
				{
					Stop();
					return;
				}

				if( !m.Alive )
				{
					Stop();
					StopEffect( m );
					return;
				}

				if( m.Poisoned && PerCheck( (int)m_GiftInfo.m_Caster.Skills[ SkillName.AnimalLore ].Value ) )
				{
					m.CurePoison( m_GiftInfo.m_Caster );
					m.FixedParticles( 0x376A, 9, 32, 5005, EffectLayer.Waist );
					return;
				}

				if( m.Hits >= m.HitsMax )
					return;

				SpellHelper.Heal( m_GiftInfo.m_HitsPerRound, m, m_GiftInfo.m_Caster );
				m.FixedParticles( 0x376A, 9, 32, 5005, EffectLayer.Waist );
			}
		}

		public static bool StopEffect( Mobile m )
		{
			GiftOfRenewalInfo info;

			if( m_Table.TryGetValue( m, out info ) )
			{
				m_Table.Remove( m );

				info.m_Timer.Stop();
				Timer.DelayCall( TimeSpan.FromSeconds( 60 ),
						delegate {
								if ((info.m_Caster).HasGump(typeof(GiftOfRenewalGump)))
									(info.m_Caster).CloseGump(typeof(GiftOfRenewalGump));
								if (info.m_Caster != m && m.Player && m.HasGump(typeof(GiftOfRenewalGump)))
									m.CloseGump(typeof(GiftOfRenewalGump));
								m.SendMessage( m.Language == "ITA" ? "Puoi usare di nuovo il tuo dono." : "You can use again your gift." );
								info.m_Caster.EndAction( typeof( GiftOfRenewalSpell ) );
								} );

				return true;
			}

			return false;
		}

		public class InternalTarget : Target
		{
			private readonly GiftOfRenewalSpell m_Owner;

			public InternalTarget( GiftOfRenewalSpell owner ) : base( 10, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}