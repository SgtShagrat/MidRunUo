/***************************************************************************
 *                               SacredBeamSpell.cs
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
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class SacredBeamSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Sacred Beam", "Albi Fulgoris Auxilium",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( SacredBeamSpell ),
            m_Info.Name,
            "This mightyful Miracle strikes the Paladins' enemies with a holy beam of energy.",
            "Questo potente Miracolo evoca un fulmine divino sui nemici del Paladino.",
            "Questo potente Miracolo evoca un fulmine divino sui nemici del Paladino." +
            "Il fulmine è effettivo contro i malvagi. Il danno base e' PowerValueScaled / 10 + 1d10;" +
            "La formula del danno è damage = baseDamage * vulnerableMul * immuneMul )." +
            "Il danno dimezza con un check su respell. Il danno massimo e' 60hp." +
            "Dal quarto livello il danno diventa ad area di level raggio.",
            0x5108
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "ora lo spell blocca il movimento durante il cast.<br>" +
                "Dopo il quarto livello diventa ad area e può essere targhttato anche il terreno.<br>" +
                "La formula del danno e' stata modificata in PowerValueScaled / 10 + 1d10;";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public override int GetMana()
        {
            return 20;
        }

        private const int MaxDamage = 60;

        public SacredBeamSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( SpellEffectContextHelper.IsUnderEffect( Caster, typeof( InvulnerabilitySpell ) ) )
            {
                Caster.SendLangMessage( 10000801 ); // "Thou cannot use this mighty miracle while in a such status."
                FinishSequence();
                return;
            }

            Caster.SendLangMessage( 10000802 ); // "Choose the target of your mighty beam!"
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

                Caster.PlaySound( 0x24A );

                List<Mobile> targets = new List<Mobile>();

                Map map = Caster.Map;
                Mobile directTarget = p as Mobile;
                int level = GetPowerLevel();

                if( map != null )
                {
                    if( directTarget != null && IsEnemy( directTarget ) )
                        targets.Add( directTarget );

                    if( level >= 4 )
                    {
                        IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), level / 2 );

                        foreach( Mobile m in eable )
                        {
                            if( IsEnemy( m ) )
                                targets.Add( m );
                        }

                        eable.Free();
                    }
                }

                if( targets.Count > 0 )
                {
                    foreach( Mobile m in targets )
                        Damage( m );

                    string enemies = targets.Count > 1 ? "enemies" : "enemy";
                    Caster.SendLangMessage( 10000803, enemies ); // "You have slayed your {0} with a beam of sacred light!"
                }
                else
                {
                    Caster.SendLangMessage( 10000804 ); // "There is no valid target for your mighty beam!"
                }
            }

            FinishSequence();
        }

        private const int DamageDivisor = 30;
        private static DiceRoll DamageDice = DiceRoll.OneDiceTen;
        private const double VulnerableScalar = 1.25;
        private const double ImmuneScalar = 0.50;

        private void Damage( Mobile m )
        {
            int baseDamage = ( PowerValueScaled / DamageDivisor ) + DamageDice.Roll();

            double vulnerableMul = IsSuperVulnerable( m ) ? VulnerableScalar : 1.0;
            double immuneMul = IsImmune( m ) ? ImmuneScalar : 1.0;

            int finalDamage = Math.Min( (int)( baseDamage * vulnerableMul * immuneMul ), MaxDamage );

            if( CheckResisted( m ) )
            {
                finalDamage = (int)( finalDamage * GetResistScalar( m ) );
                m.SendLangMessage( 10000805 ); // "Thou feel yourself resisting the sacred power."
            }

            if( Caster.PlayerDebug )
            {
                Caster.SendMessage( "Sacred beam: Base dmg is {0}", baseDamage );
                Caster.SendMessage( "Sacred beam: Dmg multipliers are: vul: {0}, imm {1}.", vulnerableMul.ToString( "F2" ), immuneMul.ToString( "F2" ) );
                Caster.SendMessage( "Sacred beam: Final dmg is {0}", finalDamage );
            }

            bool evil = m.Karma < 0 || m.Kills > 0 || m.Criminal;

            if( !evil )
                Caster.DoHarmful( m );

            AOS.Damage( m, Caster, finalDamage, 0, 0, 0, 0, 100, false );

            if( m.Player )
                m.SendLangMessage( 10000806, Caster.Name ); // "You have been slayed by {0}, the mighty paladin!"

            Caster.FixedParticles( 14201, 10, 15, 5012, EffectLayer.Waist ); // Little Sparkle

            m.PlaySound( 0x1E0 );
            m.BoltEffect( 0x480 );

            m.FixedParticles( 0x373A, 1, 15, 5012, 3, 2, EffectLayer.Waist ); // Sparkle
            m.FixedParticles( 0x375A, 10, 15, 5037, EffectLayer.Waist ); // Sparkle
        }

        private class InternalTarget : Target
        {
            private readonly SacredBeamSpell m_Owner;

            public InternalTarget( SacredBeamSpell owner )
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