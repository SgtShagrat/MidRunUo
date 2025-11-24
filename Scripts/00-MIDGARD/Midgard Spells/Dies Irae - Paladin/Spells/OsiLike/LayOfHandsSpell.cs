/***************************************************************************
 *                               LayOfHandsSpell.cs
 *
 *   begin                : 04 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Gumps;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class LayOfHandsSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Lay Of Hands", "Obsu Vulni",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( LayOfHandsSpell ),
            m_Info.Name,
            "This Miracle heals the creature touched by Paladin's hands",
            "Questo Miracolo cura la creatura toccata dal Paladino.",
            "Questo Miracolo cura la creatura toccata dal Paladino." +
            "Dal quarto livello diventa a raggio: level passi dal punto di target." +
            "Dal quinto livello prima di curare tenta di resuscitare." +
            "Le creature valide sono il party, la gilda e i non aggressori o aggrediti con karma positivo." +
            "La formula del danno curato: int toHeal = PowerValueScaled / 10;",
            0x5100
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato il funzionamento dello spell.<br>" +
                "Dal quarto livello diventa a raggio: level passi dal punto di target.<br>" +
                "Dal quinto livello prima di curare tenta di resuscitare.<br>" +
                "Le creature valide sono il party, la gilda e i non aggressori o aggrediti con karma positivo.<br>" +
                "La formula del danno curato: int toHeal = PowerValueScaled / 10 punti danno.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public override TimeSpan CastDelayBase
        {
            get { return TimeSpan.FromSeconds( 1.5 ); }
        }

        public LayOfHandsSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            Caster.SendMessage( "{0}, choose the target of your holy touch.", Caster.Name );
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
                Caster.FixedParticles( 0x376A, 9, 32, 5005, EffectLayer.Waist );

                List<Mobile> targets = new List<Mobile>();

                Map map = Caster.Map;
                Mobile directTarget = p as Mobile;
                int level = GetPowerLevel();

                if( map != null )
                {
                    if( directTarget != null )
                        targets.Add( directTarget );

                    if( level >= 4 )
                    {
                        IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), level );

                        foreach( Mobile m in eable )
                        {
                            if( !m.Player )
                                continue;

                            if( IsValidTarget( m ) )
                                targets.Add( m );
                        }

                        eable.Free();
                    }
                }

                if( targets.Count > 0 )
                {
                    foreach( Mobile m in targets )
                    {
                        if( !m.Alive && level > 4 )
                            Resurrect( m );
                        else if( m.Hits < m.HitsMax )
                            Heal( m );
                    }
                }
            }

            FinishSequence();
        }

        private void Resurrect( Mobile m )
        {
            Caster.DoBeneficial( m );

            m.PlaySound( 0x214 );
            m.FixedEffect( 0x376A, 10, 16 );

            m.CloseGump( typeof( ResurrectGump ) );
            m.SendGump( new ResurrectGump( m, Caster ) );
        }

        private void Heal( Mobile m )
        {
            Caster.DoBeneficial( m );

            m.PlaySound( 0x202 );
            m.FixedParticles( 0x376A, 1, 62, 9923, 3, 3, EffectLayer.Waist );
            m.FixedParticles( 0x3779, 1, 46, 9502, 5, 3, EffectLayer.Waist );

            int toHeal = PowerValueScaled / 15;

            if( ( m.Hits + toHeal ) > m.HitsMax )
                toHeal = m.HitsMax - m.Hits;

            SpellHelper.Heal( toHeal, m, Caster, false );
        }

        private bool IsValidTarget( Mobile m )
        {
            // members of Caster party are valid members
            if( Caster.Party != null && Caster.Party is Party )
            {
                if( ( (Party)Caster.Party ).Contains( m ) )
                    return true;
            }

            // members of Caster guils are always valid members
            Guild fromGuild = Caster.Guild as Guild;
            Guild targetGuild = m.Guild as Guild;

            if( fromGuild != null && targetGuild != null && ( targetGuild == fromGuild || fromGuild.IsAlly( targetGuild ) ) )
                return true;

            // beneficals, not aggressors or aggressed with positive karma are valid members
            return Caster.CanBeBeneficial( m, false ) && !IsAggressor( m ) && !IsAggressed( m ) && m.Karma > 0;
        }

        private bool IsAggressor( Mobile m )
        {
            foreach( AggressorInfo info in Caster.Aggressors )
            {
                if( m == info.Attacker && !info.Expired )
                    return true;
            }

            return false;
        }

        private bool IsAggressed( Mobile m )
        {
            foreach( AggressorInfo info in Caster.Aggressed )
            {
                if( m == info.Defender && !info.Expired )
                    return true;
            }

            return false;
        }

        private class InternalTarget : Target
        {
            private readonly LayOfHandsSpell m_Owner;

            public InternalTarget( LayOfHandsSpell owner )
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