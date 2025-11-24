/***************************************************************************
 *                               ShieldOfEarthSpell.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Misc;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class ShieldOfEarthSpell : DruidSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Shield Of Earth", "Kes En Sepa Ohm",
            227,
            9011,
            true,
            Reagent.SpidersSilk,
            Reagent.FertileDirt
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( ShieldOfEarthSpell ),
            "A quick-growing wall of foliage springs up at the bidding of the Druid.",
            "Crea un muro di rami e radici.",
            0x517
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public override double CastDelayBonus
        {
            get { return 0; }
        }

        public override int CastManaBonus
        {
            get { return 0; }
        }

        public ShieldOfEarthSpell( Mobile caster, Item scroll )
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
            else if( MidgardSpellHelper.CheckBlockField( p, Caster ) && SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
            {
                SpellHelper.Turn( Caster, p );

                SpellHelper.GetSurfaceTop( ref p );

                int level = GetPowerLevel() + 2;
                int dx = Caster.Location.X - p.X;
                int dy = Caster.Location.Y - p.Y;
                int rx = ( dx - dy ) * 44;
                int ry = ( dx + dy ) * 44;

                bool eastToWest;

                if( rx >= 0 && ry >= 0 )
                    eastToWest = false;
                else if( rx >= 0 )
                    eastToWest = true;
                else if( ry >= 0 )
                    eastToWest = true;
                else
                    eastToWest = false;

                Effects.PlaySound( p, Caster.Map, 0x50 );

                for( int i = -level; i <= level; ++i )
                {
                    Point3D loc = new Point3D( eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z );
                    bool canFit = SpellHelper.AdjustField( ref loc, Caster.Map, 22, true );

                    if( !canFit )
                        continue;

                    Item item = new InternalItem( loc, Caster.Map, Caster );

                    Effects.SendLocationParticles( item, 0x376A, 9, 10, 5025 );
                }
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
                : base( 0x0C92 )
            {
                Visible = false;
                Movable = false;

                MoveToWorld( loc, map );

                if( caster.InLOS( this ) )
                    Visible = true;
                else
                    Delete();

                if( Deleted )
                    return;

                m_Timer = new InternalTimer( this, TimeSpan.FromSeconds( 10.0 ) );
                m_Timer.Start();

                m_End = DateTime.Now + TimeSpan.FromSeconds( 10.0 );
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

                switch( version )
                {
                    case 1:
                        {
                            TimeSpan duration = reader.ReadTimeSpan();

                            m_Timer = new InternalTimer( this, duration );
                            m_Timer.Start();

                            m_End = DateTime.Now + duration;

                            break;
                        }
                    case 0:
                        {
                            TimeSpan duration = TimeSpan.FromSeconds( 10.0 );

                            m_Timer = new InternalTimer( this, duration );
                            m_Timer.Start();

                            m_End = DateTime.Now + duration;

                            break;
                        }
                }
            }
            #endregion

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if( m_Timer != null )
                    m_Timer.Stop();
            }

            private class InternalTimer : Timer
            {
                private readonly InternalItem m_Item;

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
            private readonly ShieldOfEarthSpell m_Owner;

            public InternalTarget( ShieldOfEarthSpell owner )
                : base( 10, true, TargetFlags.None )
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