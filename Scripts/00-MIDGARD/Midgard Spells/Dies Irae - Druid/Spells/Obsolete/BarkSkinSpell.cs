/***************************************************************************
 *                               BarkSkinSpell.cs
 *                            -------------------
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
    public class BarkSkinSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Bark Skin", "Porm Helma",
            224,
            9011,
            Reagent.Garlic,
            Reagent.PetrifiedWood,
            Reagent.MandrakeRoot
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( BarkSkinSpell ),
            "The druid becomes a treantlike being.",
            "Il druido acquista la resistenza fisica e mentale di un Treant.",
            2303
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public BarkSkinSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                if( Caster.BeginAction( typeof( BarkSkinSpell ) ) )
                {
                    TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills[ SkillName.Spellweaving ].Value * 0.2 );

                    int amountPos = (int) ( Caster.Skills[ SkillName.AnimalLore ].Value * 0.125 );
                    int amountNeg = Caster.Skills[ SkillName.AnimalLore ].Value > 100.0 ? -(int) ( amountPos * 0.06 ) : -(int) ( amountPos * 0.125 );

                    Caster.SendMessage( "Your skin has turned to bark." );
                    Caster.HueMod = 1885;

                    ResistanceMod mod1 = new ResistanceMod( ResistanceType.Physical, amountPos );
                    ResistanceMod mod2 = new ResistanceMod( ResistanceType.Fire, amountNeg );
                    ResistanceMod mod3 = new ResistanceMod( ResistanceType.Cold, amountNeg );
                    ResistanceMod mod4 = new ResistanceMod( ResistanceType.Poison, amountPos );
                    ResistanceMod mod5 = new ResistanceMod( ResistanceType.Energy, amountPos );

                    Caster.FixedParticles( 0x373A, 10, 15, 5012, 0x14, 3, EffectLayer.Waist );

                    Caster.AddResistanceMod( mod1 );
                    Caster.AddResistanceMod( mod2 );
                    Caster.AddResistanceMod( mod3 );
                    Caster.AddResistanceMod( mod4 );
                    Caster.AddResistanceMod( mod5 );

                    Timer t = new InternalTimer( Caster, duration, mod1, mod2, mod3, mod4, mod5 );
                    t.Start();

                    Timer.DelayCall( duration, new TimerStateCallback( ReleaseBearSkin ), Caster );
                }
                else
                {
                    Caster.SendMessage( "You are already under BarkSkin effect." );
                }
            }
            FinishSequence();
        }

        private static void ReleaseBearSkin( object state )
        {
            ( (Mobile) state ).EndAction( typeof( BarkSkinSpell ) );
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;
            private ResistanceMod m_Mod1;
            private ResistanceMod m_Mod2;
            private ResistanceMod m_Mod3;
            private ResistanceMod m_Mod4;
            private ResistanceMod m_Mod5;

            public InternalTimer( Mobile from, TimeSpan duration,
                                  ResistanceMod mod1, ResistanceMod mod2,
                                  ResistanceMod mod3, ResistanceMod mod4,
                                  ResistanceMod mod5 )
                : base( duration )
            {
                Priority = TimerPriority.OneSecond;

                m_From = from;

                m_Mod1 = mod1;
                m_Mod2 = mod2;
                m_Mod3 = mod3;
                m_Mod4 = mod4;
                m_Mod5 = mod5;
            }

            protected override void OnTick()
            {
                if( m_From != null )
                {
                    m_From.RemoveResistanceMod( m_Mod1 );
                    m_From.RemoveResistanceMod( m_Mod2 );
                    m_From.RemoveResistanceMod( m_Mod3 );
                    m_From.RemoveResistanceMod( m_Mod4 );
                    m_From.RemoveResistanceMod( m_Mod5 );
                    m_From.SendMessage( "The effect of bark skin wears off." );
                    m_From.HueMod = -1;
                    Stop();
                }
            }
        }
    }
}