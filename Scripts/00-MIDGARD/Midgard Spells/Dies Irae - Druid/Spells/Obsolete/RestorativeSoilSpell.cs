/***************************************************************************
 *                               RestorativeSoilSpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Misc;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class RestorativeSoilSpell : DruidSpell
    {
        private static SpellInfo m_Info = new SpellInfo(
            "Restorative Soil", "Ohm Sepa Ante",
            269,
            9020,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.SpringWater
            );

        private static ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( RestorativeSoilSpell ),
            "Saturates a patch of land with power, causing healing mud to seep through. The mud can restore the dead to life.",
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

        public RestorativeSoilSpell( Mobile caster, Item scroll )
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
            else if( CheckSequence() )
            {
                SpellHelper.Turn( Caster, p );

                SpellHelper.GetSurfaceTop( ref p );

                Effects.PlaySound( p, Caster.Map, 0x382 );

                Point3D loc = new Point3D( p.X, p.Y, p.Z );
                new InternalItem( loc, Caster.Map, Caster );
            }

            FinishSequence();
        }

        [DispellableField]
        private class InternalItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;

            public override bool BlocksFit
            {
                get { return true; }
            }

            public InternalItem( Point3D loc, Map map, Mobile caster )
                : base( 0x913 )
            {
                Visible = false;
                Movable = false;
                Name = "restorative soil";
                MoveToWorld( loc, map );

                if( caster.InLOS( this ) )
                    Visible = true;
                else
                    Delete();

                if( Deleted )
                    return;

                m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( 30.0 ) );
                m_Timer.Start();

                m_End = DateTime.Now + TimeSpan.FromSeconds( 30.0 );
            }

            public override bool HandlesOnMovement
            {
                get { return true; }
            }

            #region serialization
            public InternalItem( Serial serial )
                : base( serial )
            {
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 1 ); // version

                writer.Write( m_End - DateTime.Now );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                TimeSpan duration = reader.ReadTimeSpan();

                m_Timer = new InternalTimer( this, duration );
                m_Timer.Start();

                m_End = DateTime.Now + duration;
            }
            #endregion

            public override bool OnMoveOver( Mobile m )
            {
                if( m is PlayerMobile && !m.Alive )
                {
                    m.SendGump( new ResurrectGump( m ) );

                    m.SendMessage( "The power of the soil surges through you!" );
                }
                else
                    m.PlaySound( 0x339 );
                return true;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if( m_Timer != null )
                    m_Timer.Stop();
            }

            private class InternalTimer : Timer
            {
                private InternalItem m_Item;

                public InternalTimer( InternalItem item, TimeSpan duration )
                    : base( duration )
                {
                    m_Item = item;
                }

                protected override void OnTick()
                {
                    m_Item.Delete();
                }
            }
        }

        private class InternalTarget : Target
        {
            private RestorativeSoilSpell m_Owner;

            public InternalTarget( RestorativeSoilSpell owner )
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