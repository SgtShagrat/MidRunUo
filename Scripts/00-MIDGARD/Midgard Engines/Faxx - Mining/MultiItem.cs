/***************************************************************************
 *                               MultiItem.cs
 *                            ------------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Faxx	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Faxx - Dies Irae			
 *   revisione            : Dies Irae
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class MultiItem : Item
    {
        protected List<InternalItem> m_InternalItems;

        public MultiItem( int id )
            : base( id )
        {
            m_InternalItems = new List<InternalItem>();
        }

        public MultiItem( Serial serial )
            : base( serial )
        {
            m_InternalItems = new List<InternalItem>();
        }

        [Hue, CommandProperty( AccessLevel.GameMaster )]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                foreach( InternalItem i in m_InternalItems )
                    i.Hue = value;

                base.Hue = value;
            }
        }

        protected InternalItem AddSubItem( int id, int xoff, int yoff )
        {
            return AddSubItem( id, xoff, yoff, typeof( InternalItem ) );
        }

        protected InternalItem AddSubItem( int id, int xoff, int yoff, Type t )
        {
            if( !typeof( InternalItem ).IsAssignableFrom( t ) )
            {
                Console.WriteLine( "MultiItem:AddSubItem() invalid subitem type {0}", t.Name );
                return null;
            }

            if( m_InternalItems == null )
                m_InternalItems = new List<InternalItem>();

            InternalItem i = Activator.CreateInstance( t, new object[] { this, id, xoff, yoff } ) as InternalItem;

            m_InternalItems.Add( i );

            if( xoff == 0 && yoff == 0 )
            {
                if( i != null )
                    i.Visible = false; //This would hide root item and make it not targettable
            }

            return i;
        }

        public void RemoveItems()
        {
            if( m_InternalItems == null )
                return;

            foreach( InternalItem i in m_InternalItems )
            {
                i.m_Item = null;
                i.Delete();
            }

            m_InternalItems = null;
        }

        public override void OnLocationChange( Point3D oldLocation )
        {
            if( m_InternalItems != null )
            {
                foreach( InternalItem i in m_InternalItems )
                    i.Location = new Point3D( X + i.m_XOffset, Y + i.m_YOffset, Z );
            }
        }

        public override void OnMapChange()
        {
            if( m_InternalItems != null )
            {
                foreach( InternalItem i in m_InternalItems )
                    i.Map = Map;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            RemoveItems();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            if( m_InternalItems != null )
            {
                writer.Write( m_InternalItems.Count );

                for( int i = 0; i < m_InternalItems.Count; i++ )
                    writer.Write( m_InternalItems[ i ] );
            }
            else
                writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            int n = reader.ReadInt();

            if( m_InternalItems == null )
                m_InternalItems = new List<InternalItem>();

            for( int i = 0; i < n; ++i )
            {
                InternalItem item = reader.ReadItem() as InternalItem;

                if( item != null )
                    m_InternalItems.Add( item );
            }
        }

        #region Nested type: InternalItem
        public class InternalItem : Item
        {
            public MultiItem m_Item;
            public int m_XOffset;
            public int m_YOffset;

            public InternalItem( MultiItem item, int id, int xoff, int yoff )
                : base( id )
            {
                m_XOffset = xoff;
                m_YOffset = yoff;
                m_Item = item;

                Map = m_Item.Map;
                Location = new Point3D( m_Item.X + xoff, m_Item.Y + yoff, m_Item.Z );
                Movable = m_Item.Movable;
            }

            public InternalItem( Serial serial )
                : base( serial )
            {
            }

            [CommandProperty( AccessLevel.Counselor )]
            public MultiItem RootItem
            {
                get { return m_Item; }
            }

            public override void OnLocationChange( Point3D oldLocation )
            {
                if( m_Item != null )
                    m_Item.Location = new Point3D( X - m_XOffset, Y - m_YOffset, Z );
            }

            public override void OnMapChange()
            {
                if( m_Item != null )
                    m_Item.Map = Map;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if( m_Item != null )
                    m_Item.Delete();
            }

            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( (int)0 ); // version

                writer.Write( m_Item );
                writer.Write( m_XOffset );
                writer.Write( m_YOffset );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                m_Item = reader.ReadItem() as MultiItem;
                m_XOffset = reader.ReadInt();
                m_YOffset = reader.ReadInt();
            }
        }
        #endregion
    }
}