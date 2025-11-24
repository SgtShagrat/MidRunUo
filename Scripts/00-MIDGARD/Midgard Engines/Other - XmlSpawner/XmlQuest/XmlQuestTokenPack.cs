using System.Collections.Generic;
using System.IO;
using Server.Network;

namespace Server.Items
{
    public class XmlQuestTokenPack : Container
    {
        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            return false;
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            return false;
        }

        public override void OnAdded( object target )
        {
            base.OnAdded( target );

            UpdateTotal( this, TotalType.Weight, 0 );
            UpdateTotal( this, TotalType.Gold, 0 );
            UpdateTotal( this, TotalType.Items, 0 );
        }

        public sealed class ForcedContainerContent : Packet
        {
            public ForcedContainerContent( Mobile beholder, Item beheld )
                : base( 0x3C )
            {
                List<Item> items = beheld.Items;
                int count = items.Count;

                EnsureCapacity( 5 + ( count * 19 ) );

                long pos = m_Stream.Position;

                int written = 0;

                m_Stream.Write( (ushort)0 );

                for( int i = 0; i < count; ++i )
                {
                    Item child = items[ i ];

                    if( !child.Deleted )
                    {
                        Point3D loc = child.Location;

                        var cid = (ushort)child.ItemID;

                        if( cid > 0x3FFF )
                            cid = 0x9D7;

                        m_Stream.Write( child.Serial );
                        m_Stream.Write( cid );
                        m_Stream.Write( (byte)0 ); // signed, itemID offset
                        m_Stream.Write( (ushort)child.Amount );
                        m_Stream.Write( (short)loc.X );
                        m_Stream.Write( (short)loc.Y );
                        m_Stream.Write( beheld.Serial );
                        m_Stream.Write( (ushort)child.Hue );

                        ++written;
                    }
                }

                m_Stream.Seek( pos, SeekOrigin.Begin );
                m_Stream.Write( (ushort)written );
            }
        }

        public override void DisplayTo( Mobile to )
        {
            if( to == null )
                return;

            to.Send( new ContainerDisplay( this ) );
            to.Send( new ForcedContainerContent( to, this ) );

            if( ObjectPropertyList.Enabled )
            {
                List<Item> items = Items;

                for( int i = 0; i < items.Count; ++i )
                    to.Send( items[ i ].OPLPacket );
            }
        }

        public XmlQuestTokenPack()
            : base( 0x9B2 )
        {
            Weight = 0;
        }

        public XmlQuestTokenPack( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}