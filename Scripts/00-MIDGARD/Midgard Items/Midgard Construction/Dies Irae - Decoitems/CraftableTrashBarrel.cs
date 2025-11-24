using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    [Furniture]
    public class CraftableTrashBarrel : Container
    {
        public override int LabelNumber { get { return 1064925; } } // trash barrel
        public override int DefaultMaxWeight { get { return 0; } }
        public override bool IsDecoContainer { get { return false; } }

        [Constructable]
        public CraftableTrashBarrel()
            : base( 0xE77 )
        {
            Hue = 0x3B2;
            Weight = 20;
        }

        public CraftableTrashBarrel( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( Items.Count > 0 )
            {
                m_Timer = new EmptyTimer( this );
                m_Timer.Start();
            }
        }
        #endregion

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( !base.OnDragDrop( from, dropped ) )
                return false;

            SendLocalizedMessageTo( from, 1010442 ); // The item will be deleted in three minutes

            if( m_Timer != null )
                m_Timer.Stop();
            else
                m_Timer = new EmptyTimer( this );

            m_Timer.Start();

            return true;
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            if( !base.OnDragDropInto( from, item, p ) )
                return false;

            SendLocalizedMessageTo( from, 1010442 ); // The item will be deleted in three minutes

            if( m_Timer != null )
                m_Timer.Stop();
            else
                m_Timer = new EmptyTimer( this );

            m_Timer.Start();

            return true;
        }

        public void Empty( int message )
        {
            List<Item> items = Items;

            if( items.Count > 0 )
            {
                PublicOverheadMessage( MessageType.Regular, 0x3B2, message, "" );

                for( int i = items.Count - 1; i >= 0; --i )
                {
                    if( i >= items.Count )
                        continue;

                    items[ i ].Delete();
                }
            }

            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = null;
        }

        private Timer m_Timer;

        private class EmptyTimer : Timer
        {
            private CraftableTrashBarrel m_Barrel;

            public EmptyTimer( CraftableTrashBarrel barrel )
                : base( TimeSpan.FromMinutes( 3.0 ) )
            {
                m_Barrel = barrel;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Barrel.Empty( 501479 ); // Emptying the trashcan!
            }
        }
    }
}