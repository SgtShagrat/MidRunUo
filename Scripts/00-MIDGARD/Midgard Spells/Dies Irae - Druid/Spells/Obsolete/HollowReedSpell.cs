/***************************************************************************
 *                               HollowReedSpell.cs
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

namespace Midgard.Engines.SpellSystem
{
    public class HollowReedSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Hollow Reed", "En Crur Aeta Sec En Ess",
            203,
            9061,
            false,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.Nightshade
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( HollowReedSpell ),
            "Increases both the strength and the intelligence of the Druid.",
            "",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public HollowReedSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                double duration = Caster.Skills[ DamageSkill ].Fixed / 10.0;
                int offset = Caster.Skills[ CastSkill ].Fixed / 80;

                SpellHelper.AddStatOffset( Caster, StatType.Str, offset, TimeSpan.FromSeconds( duration ) );
                SpellHelper.AddStatOffset( Caster, StatType.Int, offset, TimeSpan.FromSeconds( duration ) );

                Caster.PlaySound( 0x15 );
                Caster.FixedParticles( 0x373A, 10, 15, 5018, EffectLayer.Waist );
            }

            FinishSequence();
        }
    }
}