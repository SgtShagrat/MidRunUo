/***************************************************************************
 *                               VolcanicEruptionSpell.cs
 * 
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class VolcanicEruptionSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Volcanic Eruption", "Vauk Ohm En Tia Crur",
            245,
            9042,
            false,
            Reagent.SulfurousAsh,
            Reagent.DestroyingAngel
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( VolcanicEruptionSpell ),
            "A blast of molten lava bursts from the ground, hitting every enemy nearby.",
            "",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Eighth; }
        }

        public VolcanicEruptionSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( SpellHelper.CheckTown( Caster, Caster ) && CheckSequence() )
            {
                List< Mobile > targets = new List< Mobile >();

                Map map = Caster.Map;

                if( map != null )
                {
                    foreach ( Mobile m in Caster.GetMobilesInRange( 1 + (int) ( Caster.Skills[ DamageSkill ].Value / 10.0 ) ) )
                    {
                        if( Caster != m && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) && Caster.InLOS( m ) )
                            targets.Add( m );
                    }
                }

                for ( int i = 0; i < targets.Count; ++i )
                {
                    Mobile m = targets[ i ];

                    double toDeal = 10.0 + Caster.Skills[ DamageSkill ].Value / 10.0 + Utility.Random( 5 );

                    if( CheckResisted( m ) )
                    {
                        toDeal *= 0.7;
                        m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
                    }

                    Caster.DoHarmful( m );
                    SpellHelper.Damage( this, m, toDeal, 50, 100, 0, 0, 0 );

                    m.FixedParticles( 0x3709, 20, 10, 5044, EffectLayer.RightFoot );
                    m.PlaySound( 0x21F );
                    m.FixedParticles( 0x36BD, 10, 30, 5052, EffectLayer.Head );
                    m.PlaySound( 0x208 );
                }
            }

            FinishSequence();
        }
    }
}