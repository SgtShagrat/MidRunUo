/***************************************************************************
 *                               LureStoneSpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class LureStoneSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Lure Stone", "En Kes Ohm Crur",
            269,
            9020,
            false,
            Reagent.BlackPearl,
            Reagent.SpringWater
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( LureStoneSpell ),
            "Creates a magical stone that calls all nearby animals to it.",
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

        public LureStoneSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget( this );
            Caster.SendMessage( "{0}, choose where you will summon a Lure Stone.", Caster.Name );
        }

        public void Target( IPoint3D p )
        {
            if( !Caster.CanSee( p ) )
            {
                Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
            {
                SpellHelper.Turn( Caster, p );
                SpellHelper.GetSurfaceTop( ref p );
                Effects.PlaySound( p, Caster.Map, 0x243 );

                Point3D loc = new Point3D( p.X, p.Y, p.Z );

                LureStone stone = new LureStone( Caster );
                stone.MoveToWorld( loc, Caster.Map );

                double duration = Caster.Skills[ CastSkill ].Value * 2;
                ExpireTimer t = new ExpireTimer( stone, Caster, duration );
                t.Start();
                Caster.SendMessage( "The LureStone will last {0} seconds.", duration.ToString( "F0" ) );
            }

            FinishSequence();
        }

        private class ExpireTimer : Timer
        {
            private LureStone m_Stone;
            private DateTime m_Expiration;
            private Mobile m_Owner;
            private int m_DebugCounter;

            public ExpireTimer( LureStone stone, Mobile owner, double duration )
                : base( TimeSpan.FromSeconds( 5.0 ), TimeSpan.FromSeconds( 5.0 ) )
            {
                Priority = TimerPriority.FiftyMS;

                m_Stone = stone;
                m_Owner = owner;
                m_Expiration = DateTime.Now + TimeSpan.FromSeconds( duration );

                m_DebugCounter = 0;
            }

            protected override void OnTick()
            {
                m_DebugCounter = m_DebugCounter + 5;
                if( m_Stone != null && m_Stone.IsInDebugMode )
                {
                    Console.WriteLine( m_DebugCounter.ToString() );
                    m_Stone.PublicOverheadMessage( MessageType.Regular, 37, true, m_DebugCounter.ToString() );
                }

                if( m_Owner == null && m_Stone != null )
                    m_Stone.Delete();
                else if( m_Owner != null && m_Stone != null && !m_Owner.InLOS( m_Stone ) )
                {
                    m_Stone.Delete();
                    m_Owner.SendMessage( "{0}, your presence were needed to continue the Call!", m_Owner.Name );
                }

                if( m_Stone == null || m_Stone.Deleted || DateTime.Now > m_Expiration )
                {
                    if( m_Owner != null )
                        m_Owner.SendMessage( "Your LureStone vanishes in a pile of dust." );
                    Stop();
                }
            }
        }

        private class InternalTarget : Target
        {
            private LureStoneSpell m_Owner;

            public InternalTarget( LureStoneSpell owner )
                : base( 12, true, TargetFlags.None )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is IPoint3D )
                    m_Owner.Target( (IPoint3D)o );
            }

            protected override void OnTargetFinish( Mobile from )
            {
                m_Owner.FinishSequence();
            }
        }
    }
}