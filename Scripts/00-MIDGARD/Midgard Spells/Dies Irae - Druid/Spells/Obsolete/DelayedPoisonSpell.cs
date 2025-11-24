/***************************************************************************
 *                               DelayedPoisonSpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class DelayedPoisonSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Delayed Poison", "Telwa Nox",
            230,
            9041,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( DelayedPoisonSpell ),
            "A poison strike hit the target of the druid anger.",
            "",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Sixth; }
        }

        public DelayedPoisonSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget( this );
        }

        public override bool DelayedDamage
        {
            get { return false; }
        }

        public void Target( Mobile target )
        {
            if( !Caster.CanSee( target ) )
            {
                Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if( Caster.CanBeHarmful( target ) && CheckSequence() )
            {
                SpellHelper.Turn( Caster, target );

                SpellHelper.CheckReflect( (int)Circle, Caster, ref target );

                target.Paralyzed = false;

                if( CheckResisted( target ) )
                {
                    target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
                }

                Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ), new TimerStateCallback( ApplyPoisonCallBack ), new object[] { Caster, target, this } );
            }

            FinishSequence();
        }

        public void ApplyPoisonCallBack( object state )
        {
            object[] states = (object[])state;
            Mobile attacker = (Mobile)states[ 0 ];
            Mobile defender = (Mobile)states[ 1 ];
            DelayedPoisonSpell spell = (DelayedPoisonSpell)states[ 2 ];

            if( attacker.HarmfulCheck( defender ) )
            {
                int rawLevel = attacker.Skills[ spell.DamageSkill ].Fixed;
                int rangeOffset = attacker.Skills[ spell.CastSkill ].Fixed / 600;
                int level;

                if( attacker.InRange( defender, 2 + rangeOffset ) )
                {
                    if( rawLevel >= 1200 )
                        level = 5;
                    else if( rawLevel >= 1100 )
                        level = 4;
                    else if( rawLevel >= 1000 )
                        level = 3;
                    else if( rawLevel > 850 )
                        level = 2;
                    else if( rawLevel > 650 )
                        level = 1;
                    else
                        level = 0;
                }
                else
                {
                    level = 0;
                }

                defender.ApplyPoison( attacker, Poison.GetPoison( level ) );
                defender.FixedParticles( 0x113A, 5, 30, 5052, EffectLayer.LeftFoot );
                defender.PlaySound( 0x208 );
                defender.Animate( 32, 5, 1, true, false, 0 );
            }
        }

        private class InternalTarget : Target
        {
            private DelayedPoisonSpell m_Owner;

            public InternalTarget( DelayedPoisonSpell owner )
                : base( 10, false, TargetFlags.Harmful )
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