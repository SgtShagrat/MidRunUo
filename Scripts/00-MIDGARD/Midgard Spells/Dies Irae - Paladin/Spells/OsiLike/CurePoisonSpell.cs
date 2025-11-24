/***************************************************************************
 *                               CurePoisonSpell.cs
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
using Server.Network;

namespace Midgard.Engines.SpellSystem
{
    public class CurePoisonSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Cure Poison", "Expor Flamus",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( CurePoisonSpell ),
            m_Info.Name,
            "This Miracle removes the poison from the touched creature.",
            "Questo Miracolo cura la creatura toccata dal Paladino dal veleno.",
            "Questo Miracolo cura la creatura toccata dal Paladino dal veleno." +
            "La chance di curare vale: int chanceToCure = 10000 + (int)( chival * 75 ) - ( p.Level + 1 ) * 1750 / 100." +
            "Il paladino subisce un danno pari a 50 - PowerValueScaled / 20." +
            "Il target diventa immune al veleno per ( PowerValueScaled * 2 ) secondi",
            0x510c
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificata la chance di curare e l'effetto residuo (immunità al veleno per PowerValueScaled * 2 secondi).<br>" +
                "La chance di curare vale: int chanceToCure = 10000 + (int)( chival * 75 ) - ( p.Level + 1 ) * 1750 / 100.<br>" +
                "Il paladino subisce un danno pari a 50 - PowerValueScaled / 20.<br>" +
                "Il target diventa immune al veleno per ( PowerValueScaled * 2 ) secondi.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public override TimeSpan CastDelayBase
        {
            get { return TimeSpan.FromSeconds( 1.0 ); }
        }

        public CurePoisonSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            Caster.SendMessage( "{0}, choose the target of your holy touch.", Caster.Name );
            Caster.Target = new InternalTarget( this );
        }

        public void Target( Mobile m )
        {
            if( !m.Poisoned )
            {
                Caster.SendLocalizedMessage( 1060176 ); // That creature is not poisoned!
            }
            else if( CheckBSequence( m ) )
            {
                SpellHelper.Turn( Caster, m );

                Poison p = m.Poison;

                if( p != null )
                {
                    int chanceToCure = ( 10000 + (int)( Caster.Skills[ SkillName.Chivalry ].Value * 75 ) - ( p.Level + 1 ) * 1750 ) / 100;

                    if( chanceToCure > Utility.Random( 100 ) )
                    {
                        if( m.CurePoison( Caster ) )
                        {
                            if( Caster != m )
                                Caster.SendLocalizedMessage( 1010058 ); // You have cured the target of all poisons!

                            m.SendLocalizedMessage( 1010059 ); // You have been cured of all poisons.

                            CurePoisonContext context = GetContext( m );

                            if( context == null && GetPowerLevel() >= 3 )
                            {
                                m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1061905 );
                                m.PlaySound( 0x3B );

                                Timer timer = new InternalTimer( m, PowerValueScaled * 2 );
                                timer.Start();

                                AddContext( m, new CurePoisonContext( timer ) );
                            }
                        }
                    }
                    else
                    {
                        m.SendLocalizedMessage( 1010060 ); // You have failed to cure your target!
                    }
                }

                m.PlaySound( 0x1E0 );
                m.FixedParticles( 0x373A, 1, 15, 5012, 3, 2, EffectLayer.Waist );

                IEntity from = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z - 5 ), m.Map );
                IEntity to = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z + 45 ), m.Map );
                Effects.SendMovingParticles( from, to, 0x374B, 1, 0, false, false, 63, 2, 9501, 1, 0, EffectLayer.Head, 0x100 );

                Caster.PlaySound( 0x208 );
                Caster.FixedParticles( 0x3709, 1, 30, 9934, 0, 7, EffectLayer.Waist );
            }

            FinishSequence();
        }

        private static readonly Dictionary<Mobile, CurePoisonContext> m_Table = new Dictionary<Mobile, CurePoisonContext>();

        private static void AddContext( Mobile m, CurePoisonContext context )
        {
            m_Table[ m ] = context;
        }

        public static void RemoveContext( Mobile m )
        {
            CurePoisonContext context = GetContext( m );

            if( context != null )
                RemoveContext( m, context );
        }

        private static void RemoveContext( Mobile m, CurePoisonContext context )
        {
            m_Table.Remove( m );

            context.Timer.Stop();
        }

        private static CurePoisonContext GetContext( Mobile m )
        {
            return !m_Table.ContainsKey( m ) ? null : m_Table[ m ];
        }

        public static bool UnderEffect( Mobile m )
        {
            return ( GetContext( m ) != null );
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public InternalTimer( Mobile from, double durationInSeconds )
                : base( TimeSpan.FromSeconds( durationInSeconds ) )
            {
                m_Mobile = from;
            }

            protected override void OnTick()
            {
                if( !m_Mobile.Deleted )
                {
                    m_Mobile.LocalOverheadMessage( MessageType.Regular, 0x3F, true, "*You feel the effects of your poison resistance wearing off*" );
                }

                RemoveContext( m_Mobile );
            }
        }

        private class CurePoisonContext
        {
            public Timer Timer { get; private set; }

            public CurePoisonContext( Timer timer )
            {
                Timer = timer;
            }
        }

        private class InternalTarget : Target
        {
            private readonly CurePoisonSpell m_Owner;

            public InternalTarget( CurePoisonSpell owner )
                : base( 12, false, TargetFlags.Beneficial )
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