/***************************************************************************
 *							   GiftOfLife.cs
 *							-------------------
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Menus;

using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells;
using Server.Targeting;

using Midgard.Gumps;

namespace Midgard.Engines.SpellSystem
{
	public class GiftOfLifeSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
		"Gift of Life", "Ess En Aeta",
			224,
			9011,
			Reagent.Kindling,
			Reagent.FertileDirt,
			Reagent.SpringWater
		);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( GiftOfLifeSpell ),
				"When in effect on the caster or caster's pet, the beneficiary will be resurrected upon death.",
				"Un druido o il suo animale possono ingannare la morte per una volta."+
				"Durata ((SK/24)*2 + FL) minuti.",
				0x59e6
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Eighth; }}

		public GiftOfLifeSpell( Mobile caster, Item scroll ): base( caster, scroll, m_Info )
		{
		}

		public static void Initialize()
		{
			EventSink.PlayerDeath += new MobileDeathEventHandler( delegate( MobileDeathEventArgs e )
			{
				HandleDeath( e.Mobile );
			} );
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			BaseCreature bc = m as BaseCreature;

			if( Caster == null )
				return;

			if( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( m.IsDeadBondedPet || !m.Alive )
			{
				// As per Osi: Nothing happens.
			}
			else if( m != Caster && ( bc == null || !bc.IsBonded || bc.ControlMaster != Caster ) )
			{
				Caster.SendLocalizedMessage( 1072077 ); // You may only cast this spell on yourself or a bonded pet.
			}
			else if( m_Table.ContainsKey( m ) )
			{
				Caster.SendLocalizedMessage( 501775 ); // This spell is already in effect.
			}
			else if( CheckBSequence( m ) )
			{
				if( Caster == m )
				{
					Caster.SendLocalizedMessage( 1074774 ); // You weave powerful magic, protecting yourself from death.
				}
				else
				{
					Caster.SendLocalizedMessage( 1074775 ); // You weave powerful magic, protecting your pet from death.
					SpellHelper.Turn( Caster, m );
				}


				m.PlaySound( 0x244 );
				m.FixedParticles( 0x3709, 1, 30, 0x26ED, 5, 2, EffectLayer.Waist );
				m.FixedParticles( 0x376A, 1, 30, 0x251E, 5, 3, EffectLayer.Waist );

				double skill = Caster.Skills[ SkillName.Spellweaving ].Value;

				TimeSpan duration = TimeSpan.FromMinutes( ( (int)( skill / 24 ) ) * 2 + FocusLevel );

				ExpireTimer t = new ExpireTimer( Caster, m, duration, this );
				t.Start();

				m_Table[ m ] = t;
			}

			FinishSequence();
		}

		private static readonly Dictionary<Mobile, ExpireTimer> m_Table = new Dictionary<Mobile, ExpireTimer>();

		public static bool HasProtection( Mobile from )
		{
			if( m_Table == null )
				return false;

			if( m_Table.ContainsKey( from ) )
				return true;

			return false;
		}

		public static void HandleDeath( Mobile m )
		{
			if( m_Table.ContainsKey( m ) )
				Timer.DelayCall( TimeSpan.FromSeconds( Utility.RandomMinMax( 2, 4 ) ), new TimerStateCallback<Mobile>( HandleDeath_OnCallback ), m );
		}

		private static void HandleDeath_OnCallback( Mobile m )
		{
			ExpireTimer timer;

			if( m_Table.TryGetValue( m, out timer ) )
			{
				double hitsScalar = timer.Spell.HitsScalar;

				if( m is BaseCreature && m.IsDeadBondedPet )
				{
					BaseCreature pet = (BaseCreature)m;
					Mobile master = pet.GetMaster();

					if( master != null && master.NetState != null && Utility.InUpdateRange( pet, master ) )
					{
						if( Core.AOS )
						{
							master.CloseGump( typeof( PetResurrectGump ) );
							master.SendGump( new PetResurrectGump( master, pet, hitsScalar ) );
						}
						else
						{
							if( !master.HasMenu( typeof( PetResurrectionMenu ) ) )
								master.SendMenu( new PetResurrectionMenu( pet ) );
						}
					}
					else
					{
						List<Mobile> friends = pet.Friends;

						for( int i = 0; friends != null && i < friends.Count; i++ )
						{
							Mobile friend = friends[ i ];

							if( friend.NetState != null && Utility.InUpdateRange( pet, friend ) )
							{
								if( Core.AOS )
								{
									friend.CloseGump( typeof( PetResurrectGump ) );
									friend.SendGump( new PetResurrectGump( friend, pet ) );
								}
								else
								{
									if( !friend.HasMenu( typeof( PetResurrectionMenu ) ) )
										friend.SendMenu( new PetResurrectionMenu( pet ) );
								}
								break;
							}
						}
					}
				}
				else
				{
					m.CloseGump( typeof( ResurrectGump ) );
					m.SendGump( new ResurrectGump( m, hitsScalar ) );
				}

				//Per OSI, buff is removed when gump sent, irregardless of online status or acceptence
				timer.DoExpire();
			}

		}

		public double HitsScalar { get { return ( ( Caster.Skills.Spellweaving.Value / 2.4 ) + FocusLevel ) / 100; } }

		public static void OnLogin( LoginEventArgs e )
		{
			Mobile m = e.Mobile;

			if( m == null || m.Alive || m_Table[ m ] == null )
				return;

			HandleDeath_OnCallback( m );
		}

		private class ExpireTimer : Timer
		{
			private readonly Mobile m_Mobile;
			private readonly Mobile m_Caster;

			private readonly GiftOfLifeSpell m_Spell;

			public GiftOfLifeSpell Spell { get { return m_Spell; } }

			public ExpireTimer( Mobile caster, Mobile m, TimeSpan delay, GiftOfLifeSpell spell ) : base( delay )
			{
				m_Caster = caster;
				m_Mobile = m;
				m_Spell = spell;
				if ( m_Caster == m_Mobile )
					m_Mobile.SendGump(new GiftOfLifeGump());
			}

			protected override void OnTick()
			{
				DoExpire();
			}

			public void DoExpire()
			{
				Stop();
				if ( m_Caster == m_Mobile && m_Mobile.HasGump(typeof(GiftOfLifeGump)) )
					m_Mobile.CloseGump(typeof(GiftOfLifeGump));

				if ( m_Caster == m_Mobile )
					m_Mobile.SendLocalizedMessage( 1074776 ); // You are no longer protected with Gift of Life.
				else
					m_Caster.SendMessage( m_Caster.Language == "ITA" ? "{0} non è più protetto dal Dono della Vita." : "{0} is no longer protected with Gift of Life.", m_Mobile.Name );
				m_Table.Remove( m_Mobile );
			}
		}

		public class InternalTarget : Target
		{
			private readonly GiftOfLifeSpell m_Owner;

			public InternalTarget( GiftOfLifeSpell owner ) : base( 10, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
				else
				{
					m.SendLocalizedMessage( 1072077 ); // You may only cast this spell on yourself or a bonded pet.
				}
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}