/***************************************************************************
 *							   GraspingRootsSpell.cs
 *
 *   begin				: 27 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;
using Server.Targeting;
using Server.Mobiles;

namespace Midgard.Engines.SpellSystem
{
	public class GraspingRootsSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Grasping Roots", "En Ohm Sepa Tia Kes",
			218,
			9012,
			true,
			Reagent.PetrifiedWood,
			Reagent.MandrakeRoot,
			Reagent.SpidersSilk
			);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
			(
			typeof( GraspingRootsSpell ),
			"Summons roots from the ground to entangle a single target.",
			"Le piante proteggono il druido intralciando il suo nemico."+
			"Durata (2 + SK/5 + FL*2).",
			0x4ef
			);

		public override ExtendedSpellInfo ExtendedInfo{get { return m_ExtendedInfo; }}
		public override SpellCircle Circle{get { return SpellCircle.Fourth; }}

		public GraspingRootsSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool BlocksMovement{get { return true; }}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( Caster.Language == "ITA" ? "Scegli il nemico da imprigionare!" : "Choose your enemy thou want to tangle!" );
		}

		public void Target( Mobile target )
		{
			if( !Caster.CanSee( target ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if( target.Frozen || target.Paralyzed || ( target.Spell != null && target.Spell.IsCasting ) )
			{
				Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
			}
			else if( CheckHSequence( target ) )
			{
				SpellHelper.Turn( Caster, target );

				SpellHelper.CheckReflect( (int)Circle, Caster, ref target );

				if( HandleSelfMagicalAbsorption( target ) )
					return;

				double duration = ( FocusLevel * 2 ) + ( Caster.Skills[ DamageSkill ].Value * 0.2 ) + 2;

				// Resist if Str + Dex / 2 is greater than CastSkill
				if( ( Caster.Skills[ CastSkill ].Value ) < ( ( target.Str + target.Dex ) * 0.5 ) )
					duration *= 0.5;

				// no less than 0 seconds no more than 9 seconds
				if( duration < 0.0 )
					duration = 0.0;
				if( duration > 15.0 )
					duration = 15.0;

				if( target is BaseCreature && ((BaseCreature)target).Tamable)
					duration *= 4;

				target.PlaySound( 0x2A1 );

				target.Paralyze( TimeSpan.FromSeconds( duration ) );
				target.FixedParticles( 0x3B6A, 2, 10, 5027, Utility.RandomNeutralHue(), 2, EffectLayer.Waist );

				new Roots( Caster, target, duration );
			}

			FinishSequence();
		}

		private class Roots : Item
		{
			private readonly Mobile m_Caster;
			private readonly Mobile m_Target;
			private readonly TimeSpan m_Duration;

			public Roots( Mobile caster, Mobile target, double duration ) : base( 0xC5F )
			{
				m_Caster = caster;
				m_Target = target;
				m_Duration = TimeSpan.FromSeconds( duration );

				Visible = false;
				Movable = false;

				Map map = target.Map;
				if( map != null )
					MoveToWorld( target.Location, map );

				if( m_Caster.InLOS( this ) )
					Visible = true;
				else
					Delete();

				if( Deleted )
					return;

				Timer.DelayCall( m_Duration, new TimerCallback( RemoveRoots ) );
			}

			public Roots( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( 0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();
			}

			private void RemoveRoots()
			{
				if( !Deleted )
					Delete();

				m_Caster.SendMessage( m_Caster.Language == "ITA" ? "Le radici si rompono: il tuo nemico è libero!" : "The roots are broken: your enemy is free!" );
				m_Target.SendMessage( m_Target.Language == "ITA" ? "Le radici si rompono: sei libero!" : "The roots are broken: your are free!" );
			}
		}

		private class InternalTarget : Target
		{
			private readonly GraspingRootsSpell m_Owner;

			public InternalTarget( GraspingRootsSpell owner ) : base( 10, true, TargetFlags.None )
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
				m_Owner.FinishSequence();
			}
		}
	}
}