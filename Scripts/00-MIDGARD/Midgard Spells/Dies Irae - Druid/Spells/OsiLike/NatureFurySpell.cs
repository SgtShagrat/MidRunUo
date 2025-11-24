/***************************************************************************
 *							   NatureFury.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class NatureFurySpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Nature's Fury", "Ess Ohm En Sec Tia",
			224,
			9011,
			Reagent.DestroyingAngel,
			Reagent.FertileDirt,
			Reagent.Nightshade
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
				typeof( NatureFurySpell ),
				"Creates an uncontrollable swarm of insects that attack nearby enemies.",
				"Evoca uno sciame di insetti che infliggono punture velenose."+
				"Durata (SK/24 + FL*2 + 25); Danno in base ad AnimalTaming.",
				0x59dd
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}

		public override SpellCircle Circle{get { return SpellCircle.Fifth; }}

		public NatureFurySpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private bool m_MobileTarg;

		public override int GetMana()
		{
			int mana = base.GetMana();

			return mana > 25 ? 25 : mana;

			/*
			if( m_MobileTarg )
				mana *= 2;
			*/
		}

		public override double RequiredSkill{get { return 35; }}

		public override bool CheckCast()
		{
			if( !base.CheckCast() )
				return false;

			if( !SpellHelper.CheckUniqueSummon( typeof( NatureFury ), Caster, true ) )
				return false;

			if( ( Caster.Followers + 1 ) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D point )
		{
			Mobile m = point as Mobile;
			Point3D p = new Point3D( point );
			Map map = Caster.Map;

			m_MobileTarg = ( m != null );

			BaseHouse house = BaseHouse.FindHouseAt( p, map, 0 );
			if( house != null )
				if( !house.IsFriend( Caster ) )
					return;

			if( !SpellHelper.FindValidSpawnLocation( map, ref p, m_MobileTarg ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
			else if( SpellHelper.CheckTown( p, Caster ) && ( m_MobileTarg ? CheckHSequence( m ) : CheckSequence() ) )
			{
				TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills.Spellweaving.Value / 24 + 25 + FocusLevel * 2 );

				NatureFury nf = new NatureFury( m );
				if( Caster != null )
				{
					if( !Caster.InLOS( p ) )
					{
						Caster.SendMessage( (Caster.Language == "ITA" ? "Non riesci a vedere quel punto." : "Thou cannot see that point.") );
						return;
					}

					BaseCreature.Summon( nf, false, Caster, p, 0x5CB, duration );

					int level;
					int powerLevel = GetPowerLevel();

					if( powerLevel >= 5 && Caster.Skills.AnimalTaming.Value >= 95.0 )
						level = 3;
					else if( powerLevel > 4 )
						level = 2;
					else if( powerLevel > 2 )
						level = 1;
					else
						level = 0;

					nf.PoisonLevel = Poison.GetPoison( level );
				}

				Timer t = null;
				TimeSpan delay = TimeSpan.FromSeconds( 5.0 );
				t = Timer.DelayCall( delay, delay, delegate
								{
									if( ( !nf.Alive || nf.Deleted || nf.DamageMin > 20 ) && t != null )
										t.Stop();

									nf.DamageMin++;
									nf.DamageMax++;
								} );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private readonly NatureFurySpell m_Owner;

			public InternalTarget( NatureFurySpell owner ) : base( 10, true, TargetFlags.None )
			{
				m_Owner = owner;
				CheckLOS = true;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if( m_Owner != null )
					m_Owner.FinishSequence();
			}
		}
	}
}