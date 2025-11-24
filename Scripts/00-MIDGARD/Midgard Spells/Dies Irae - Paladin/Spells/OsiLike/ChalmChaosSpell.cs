/***************************************************************************
 *							   ChalmChaosSpell.cs
 *
 *   begin				: 05 maggio 2011
 *   author			   :	Dies Irae
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class ChalmChaosSpell : RPGPaladinSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Chalm Chaos", "Dispiro Malas",
			266,
			9002
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( ChalmChaosSpell ),
			m_Info.Name,
			"This Miracle forces the evil summoned creatures to vanish.",
			"Questo Miracolo dissolve le creature evocate malvagie attorno al Paladino.",
			"Questo Miracolo dissolve le creature evocate malvagie attorno al Paladino. Le creature disoolvibili vengono dissole se falliscono il check di resistenza." +
			"dispelChance = ( 50.0 + ( ( 100 * ( chiv - bc.DispelDifficulty ) ) / ( bc.DispelFocus * 2 ) ) ) / 100 ." +
			"Le creature malvagie invece fuggono se falliscono il check di resistenza." +
			"fleeChance = ( ( 100 - Math.Sqrt( m.Fame / 2.0 ) ) * chiv * dispelSkill ) / 1000000 .",
			0x510E
			);

		public override ExtendedSpellInfo ExtendedInfo
		{
			get { return m_ExtendedInfo; }
		}

		public override SpellCircle Circle
		{
			get { return SpellCircle.Second; }
		}

		public ChalmChaosSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage
		{
			get { return false; }
		}

		public override void SendCastEffect()
		{
			Caster.FixedEffect( 0x37C4, 10, 7, 4, 3 ); // At player
			Caster.SendMessage( "You dispel th Evil forces." );
		}

		public override void OnCast()
		{
			Caster.SendMessage( "{0}, choose the evil target.", Caster.Name );
			Caster.Target = new InternalTarget( this );
		}

		private class InternalTarget : Target
		{
			private readonly ChalmChaosSpell m_Owner;

			public InternalTarget( ChalmChaosSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				//m_Owner.FinishSequence();
			}
		}

		public void Target( Mobile m )
		{
			if( CheckSequence() && Caster.InLOS( m ) )
			{
				Caster.PlaySound( 0x299 );
				Caster.FixedParticles( 0x37C4, 1, 25, 9922, 14, 3, EffectLayer.Head );

				int dispelSkill = PowerValueScaled / 2;
				double chiv = Caster.Skills.Chivalry.Value;

				BaseCreature bc = m as BaseCreature;
				if( bc != null && bc is BaseNecroFamiliar )
				{
				//public override double DispelDifficulty{ get{ return m_CusDispelDifficulty; } }
				//public override double DispelFocus{ get{ return 20.0; } }
				//((BaseNecroFamiliar)target).CusDispelDifficulty = necro + level*3.5;
					int lvl = GetPowerLevel();
					double dispelChance = 0.3 + (chiv + lvl*3.5 - bc.DispelDifficulty)/200.0;
					//Caster.Say(" dispel: {0}%", (int)( dispelChance * 100 ) );
 
					if( dispelChance > Utility.RandomDouble() )
					{
						Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
						Effects.PlaySound( m, m.Map, 0x201 );

						m.Delete();
					}
				}
				else if( bc != null )
				{
					bool evil = !bc.Controlled && IsEnemy( m );
					bool dispellable = bc.IsDispellable && ( bc.Summoned || bc.IsAnimatedDead );

					if( dispellable || ( evil && bc.Summoned ) )
					{
						double dispelChance = ( 50.0 + ( ( 100 * ( chiv - bc.DispelDifficulty ) ) / ( bc.DispelFocus * 2 ) ) ) / 100;

						if( evil )
							dispelChance += 50.0;

						dispelChance *= dispelSkill / 100.0;

						if( dispelChance > Utility.RandomDouble() )
						{
							Effects.SendLocationParticles( EffectItem.Create( m.Location, m.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
							Effects.PlaySound( m, m.Map, 0x201 );

							m.Delete();
						}
					}
					else if( evil )
					{
						double fleeChance = ( 100 - Math.Sqrt( m.Fame / 2.0 ) ) * chiv * dispelSkill;
						fleeChance /= 1000000;

						if( fleeChance > Utility.RandomDouble() )
							bc.BeginFlee( TimeSpan.FromSeconds( 30.0 ) );
					}
				}
			}

			FinishSequence();
		}
	}
}