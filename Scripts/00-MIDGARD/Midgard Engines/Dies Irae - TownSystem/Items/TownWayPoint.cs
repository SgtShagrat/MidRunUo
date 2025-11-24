using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownWayPoint : WayPoint
    {
        public static void StartWayPointChain( Item item, Mobile from )
        {
            TownSystem system = TownSystem.Find( from.Region );
            if( system == null || !system.HasAccess( TownAccessFlags.CommandGuards, from ) )
            {
                from.SendMessage( "You cannot use that command." );
                return;
            }

            if( system != TownSystem.Find( from ) )
            {
                from.SendMessage( "You must be in your town to place a way point." );
            }
            else
            {
                from.SendMessage( "Target the position of the first way point." );
                from.Target = new StartChainTarget( null );
            }
        }

        internal class StartChainTarget : Target
        {
            private readonly TownWayPoint m_Last;

            public StartChainTarget( TownWayPoint last )
                : base( -1, true, TargetFlags.None )
            {
                m_Last = last;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is TownWayPoint )
                {
                    if( m_Last != null )
                        m_Last.NextPoint = (TownWayPoint)targeted;
                }
                else if( targeted is IPoint3D )
                {
                    TownSystem system = TownSystem.Find( from.Region );
                    if( system == null || !system.HasAccess( TownAccessFlags.CommandGuards, from ) )
                        return;

                    if( system != TownSystem.Find( from ) )
                        from.SendMessage( "You must be in your town to place a way point." );
                    else
                    {
                        Point3D p = new Point3D( (IPoint3D)targeted );

                        TownWayPoint point = new TownWayPoint( m_Last );
                        point.MoveToWorld( p, from.Map );
                        point.Town = system.Definition.Town;

                        from.Target = new InternalTarget( point );
                        from.SendMessage( "Target the position of the next way point in the sequence, or target a way point link the newest way point to." );
                    }
                }
                else
                {
                    from.SendMessage( "Target a position, or another way point." );
                }
            }
        }

        public override string DefaultName
        {
            get { return "Town Way Point"; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public TownSystem System
        {
            get { return TownSystem.Find( Town ); }
        }

        private MidgardTowns m_Town;

        public MidgardTowns Town
        {
            get { return m_Town; }
            set { m_Town = value; }
        }

        [Constructable]
        public TownWayPoint()
        {
        }

        public TownWayPoint( WayPoint prev )
            : this()
        {
            if( prev != null )
                prev.NextPoint = this;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( System != null && System.HasAccess( TownAccessFlags.CommandGuards, from ) )
            {
                from.SendMessage( "Target the next way point in the sequence." );

                from.Target = new InternalTarget( this );
            }
        }

        public TownWayPoint( Serial serial )
            : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Town = (MidgardTowns)reader.ReadInt();
                        break;
                    }
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            writer.Write( (int)m_Town );
        }

        internal class InternalTarget : Target
        {
            private readonly TownWayPoint m_Point;

            public InternalTarget( TownWayPoint pt )
                : base( -1, false, TargetFlags.None )
            {
                m_Point = pt;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( target is TownWayPoint && m_Point != null )
                {
                    m_Point.NextPoint = (TownWayPoint)target;
                }
                else
                {
                    from.SendMessage( "Target a town way point." );
                }
            }
        }
    }
}