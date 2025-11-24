/***************************************************************************
 *                               LegalThoughts.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class LegalThoughtsSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Legal Thoughts", "Extermo Vomica",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( LegalThoughtsSpell ),
            m_Info.Name,
            "This Miracle remove the curses form the touched creature.",
            "Questo Miracolo rimuove il Male che affligge il Paladino.",
            "Questo Miracolo rimuove il Male che affligge il Paladino. Dal primo livello rimuove ogni malus alle stats." +
            "Dal secondo livello: curse; dal terzo livello: blood conjunction, lobotomy;" +
            "dal quarto livello: dark omen, choking;",
            0x510F
            );

        public override ExtendedSpellInfo ExtendedInfo { get { return m_ExtendedInfo; } }

        public override string GetModificationsForChangelog()
        {
            return "Modificato il funzionamento dello spell.<br>" +
                "Dal quarto livello diventa a raggio: level passi dal punto di target.<br>" +
                "Dal primo livello rimuove ogni malus alle stats.<br>" +
                "Dal terzo livello: curse e mindrot.<br>" +
                "Dal terzo livello: blood conjunction, lobotomy.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public override TimeSpan CastDelayBase
        {
            get { return TimeSpan.FromSeconds( 1.5 ); }
        }

        public LegalThoughtsSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget( this );
        }

        public void Target( IPoint3D p )
        {
            if( !Caster.CanSee( p ) )
            {
                Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if( !Caster.InRange( p, 12 ) )
            {
                Caster.SendLocalizedMessage( 1060178 ); // You are too far away to perform that action!
            }
            else if( CheckSequence() )
            {
                SpellHelper.Turn( Caster, p );

                SpellHelper.GetSurfaceTop( ref p );

                List<Mobile> targets = new List<Mobile>();

                Map map = Caster.Map;
                Mobile directTarget = p as Mobile;
                int level = GetPowerLevel();

                if( map != null )
                {
                    if( directTarget != null && Caster.CanBeBeneficial( directTarget, false ) && IsAlly( directTarget ) )
                        targets.Add( directTarget );

                    if( level >= 4 )
                    {
                        IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), level );

                        foreach( Mobile m in eable )
                        {
                            if( !m.Player )
                                continue;

                            if( IsAlly( m ) )
                                targets.Add( m );
                        }

                        eable.Free();
                    }
                }

                if( targets.Count > 0 )
                {
                    foreach ( Mobile m in targets )
                        RemoveCurse( m );
                }
            }

            FinishSequence();
        }

        private void RemoveCurse( Mobile m )
        {
            int chance;

            if( Caster.Karma < -5000 )
                chance = 0;
            else if( Caster.Karma < 0 )
                chance = (int)Math.Sqrt( 20000 + Caster.Karma ) - 122;
            else if( Caster.Karma < 5625 )
                chance = (int)Math.Sqrt( Caster.Karma ) + 25;
            else
                chance = 100;

            if( chance > Utility.Random( 100 ) )
            {
                m.PlaySound( 0xF6 );
                m.PlaySound( 0x1F7 );
                m.FixedParticles( 0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head );

                IEntity from = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z - 10 ), Caster.Map );
                IEntity to = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z + 50 ), Caster.Map );
                Effects.SendMovingParticles( from, to, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100 );

                StatMod mod = m.GetStatMod( "[Magic] Str Offset" );
                if( mod != null && mod.Offset < 0 )
                    m.RemoveStatMod( "[Magic] Str Offset" );

                mod = m.GetStatMod( "[Magic] Dex Offset" );
                if( mod != null && mod.Offset < 0 )
                    m.RemoveStatMod( "[Magic] Dex Offset" );

                mod = m.GetStatMod( "[Magic] Int Offset" );
                if( mod != null && mod.Offset < 0 )
                    m.RemoveStatMod( "[Magic] Int Offset" );

                m.Paralyzed = false;

                int level = GetPowerLevel();

                if( level > 3 )
                {
                    //DarkOmenSpell.CheckEffect( m );
                    ChokingSpell.RemoveCurse( m );
                }
                else if( level > 2 )
                {
                    BloodConjunctionSpell.RemoveCurse( m );
                    LobotomySpell.ClearMindRotScalar( m );
                }
                else
                {
                    CurseSpell.RemoveEffect( m );
                    MortalStrike.EndWound( m );
                }
            }
            else
            {
                m.PlaySound( 0x1DF );
            }
        }

        private class InternalTarget : Target
        {
            private readonly LegalThoughtsSpell m_Owner;

            public InternalTarget( LegalThoughtsSpell owner )
                : base( 12, true, TargetFlags.None )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                IPoint3D p = o as IPoint3D;

                if( p != null )
                    m_Owner.Target( p );
            }

            protected override void OnTargetFinish( Mobile from )
            {
                m_Owner.FinishSequence();
            }
        }
    }
}