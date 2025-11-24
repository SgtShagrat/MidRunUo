using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlDrag : XmlAttachment
    {
        private Mobile m_DraggedBy; // mobile doing the dragging

        private InternalTimer m_Timer;

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile DraggedBy
        {
            get { return m_DraggedBy; }
            set
            {
                m_DraggedBy = value;
                if( m_DraggedBy != null )
                {
                    DoTimer();
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Distance { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Point3D CurrentLoc { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Map CurrentMap { get; set; }

        public XmlDrag( ASerial serial )
            : base( serial )
        {
            CurrentLoc = Point3D.Zero;
            Distance = -3;
        }

        [Attachable]
        public XmlDrag()
        {
            CurrentLoc = Point3D.Zero;
            Distance = -3;
        }

        [Attachable]
        public XmlDrag( Mobile draggedby )
        {
            CurrentLoc = Point3D.Zero;
            Distance = -3;
            DraggedBy = draggedby;
        }

        [Attachable]
        public XmlDrag( string name, Mobile draggedby )
        {
            CurrentLoc = Point3D.Zero;
            Distance = -3;
            Name = name;
            DraggedBy = draggedby;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            // version 0
            writer.Write( m_DraggedBy );
            writer.Write( Distance );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            // version 0
            m_DraggedBy = reader.ReadMobile();
            Distance = reader.ReadInt();
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if( AttachedTo is Mobile || AttachedTo is Item )
                DoTimer();
            else
                Delete();
        }

        public override void OnReattach()
        {
            base.OnReattach();

            DoTimer();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if( m_Timer != null )
                m_Timer.Stop();
        }

        public void DoTimer()
        {
            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = new InternalTimer( this );
            m_Timer.Start();
        }

        // added the duration timer that begins on spawning
        private class InternalTimer : Timer
        {
            private XmlDrag m_attachment;

            public InternalTimer( XmlDrag attachment )
                : base( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 0.5 ) )
            {
                Priority = TimerPriority.TwentyFiveMS;
                m_attachment = attachment;
            }

            protected override void OnTick()
            {
                if( m_attachment == null )
                    return;

                Mobile draggedby = m_attachment.DraggedBy;
                object parent = m_attachment.AttachedTo;

                if( parent == null || !( parent is Mobile || parent is Item ) || draggedby == null || draggedby.Deleted ||
                    draggedby == parent )
                {
                    Stop();
                    return;
                }

                // get the location of the mobile dragging
                Point3D newloc = draggedby.Location;

                Map newmap = draggedby.Map;

                if( newmap == null || newmap == Map.Internal )
                {
                    // if the mobile dragging has an invalid map, then disconnect
                    m_attachment.DraggedBy = null;
                    Stop();
                    return;
                }

                // update the location of the dragged object if the parent has moved
                if( newloc != m_attachment.CurrentLoc || newmap != m_attachment.CurrentMap )
                {
                    m_attachment.CurrentLoc = newloc;
                    m_attachment.CurrentMap = newmap;

                    int x = newloc.X;
                    int y = newloc.Y;
                    int distance = m_attachment.Distance;
                    // compute the new location for the dragged object
                    switch( draggedby.Direction & Direction.Mask )
                    {
                        case Direction.North:
                            y -= distance;
                            break;
                        case Direction.Right:
                            x += distance;
                            y -= distance;
                            break;
                        case Direction.East:
                            x += distance;
                            break;
                        case Direction.Down:
                            x += distance;
                            y += distance;
                            break;
                        case Direction.South:
                            y += distance;
                            break;
                        case Direction.Left:
                            x -= distance;
                            y += distance;
                            break;
                        case Direction.West:
                            x -= distance;
                            break;
                        case Direction.Up:
                            x -= distance;
                            y -= distance;
                            break;
                    }

                    if( parent is Mobile )
                    {
                        ( (Mobile)parent ).Location = new Point3D( x, y, newloc.Z );
                        ( (Mobile)parent ).Map = newmap;
                    }
                    else if( parent is Item )
                    {
                        ( (Item)parent ).Location = new Point3D( x, y, newloc.Z );
                        ( (Item)parent ).Map = newmap;
                    }
                }
            }
        }

        public override string OnIdentify( Mobile from )
        {
            if( from == null || from.AccessLevel == AccessLevel.Player )
                return null;

            return Expiration > TimeSpan.Zero ? String.Format( "{2}: Dragged by {0} expires in {1} mins", DraggedBy, Expiration.TotalMinutes, Name ) : String.Format( "{1}: Dragged by {0}", DraggedBy, Name );
        }
    }
}