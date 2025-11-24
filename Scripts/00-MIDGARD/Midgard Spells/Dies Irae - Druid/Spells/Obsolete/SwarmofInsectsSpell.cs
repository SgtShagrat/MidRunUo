/***************************************************************************
 *                               SwarmofInsectsSpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class SwarmOfInsectsSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Swarm Of Insects", "Ess Ohm En Sec Tia",
            263,
            9032,
            false,
            Reagent.Garlic,
            Reagent.Nightshade,
            Reagent.DestroyingAngel
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( SwarmOfInsectsSpell ),
            "Summons a swam of insects that bite and sting the Druid's enemies.",
            "",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Seventh; }
        }

        public SwarmOfInsectsSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget( this );
        }

        public void Target( Mobile m )
        {
            if( CheckHSequence( m ) )
            {
                SpellHelper.Turn( Caster, m );

                SpellHelper.CheckReflect( (int) Circle, Caster, ref m );

                CheckResisted( m ); // Check magic resist for skill, but do not use return value

                m.FixedParticles( 0x91B, 1, 240, 9916, 1159, 3, EffectLayer.Head );

                // m.FixedParticles( 0x91B, 1, 240, 9916, 0, 3, EffectLayer.Head );
                m.PlaySound( 0x1E5 );

                double damage = ( ( Caster.Skills[ CastSkill ].Value - m.Skills[ SkillName.AnimalLore ].Value ) / 10 ) + 30;

                if( damage < 1 )
                    damage = 1;

                if( m_Table.Contains( m ) )
                    damage /= 10;
                else
                    new InternalTimer( m, damage * 0.5 ).Start();

                SpellHelper.Damage( this, m, damage );
            }

            FinishSequence();
        }

        private static Hashtable m_Table = new Hashtable();

        private class InternalTimer : Timer
        {
            private Mobile m_Mobile;
            private int m_ToRestore;

            public InternalTimer( Mobile m, double toRestore )
                : base( TimeSpan.FromSeconds( 20.0 ) )
            {
                Priority = TimerPriority.OneSecond;

                m_Mobile = m;
                m_ToRestore = (int) toRestore;

                m_Table[ m ] = this;
            }

            protected override void OnTick()
            {
                m_Table.Remove( m_Mobile );

                if( m_Mobile.Alive )
                    m_Mobile.Hits += m_ToRestore;
            }
        }

        private class InternalTarget : Target
        {
            private SwarmOfInsectsSpell m_Owner;

            public InternalTarget( SwarmOfInsectsSpell owner )
                : base( 10, false, TargetFlags.Harmful )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Mobile )
                    m_Owner.Target( (Mobile) o );
            }

            protected override void OnTargetFinish( Mobile from )
            {
                m_Owner.FinishSequence();
            }
        }
    }
}