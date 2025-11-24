using System;
using System.Collections.Generic;

using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;

namespace Midgard.Engines.OldCraftSystem
{
    internal class OldDisplayCache : Container
    {
        private static OldDisplayCache m_Cache;

        public static OldDisplayCache Cache
        {
            get
            {
                if( m_Cache == null || m_Cache.Deleted )
                    m_Cache = new OldDisplayCache();

                return m_Cache;
            }
        }

        private Dictionary<Type, Item> m_Table;

        public int ItemsCount { get { return Items.Count; } }

        public virtual void ClearCache()
        {
            for( int i = Items.Count - 1; i >= 0; --i )
                if( i < Items.Count )
                    Items[ i ].Delete();
        }

        public OldDisplayCache()
            : base( 0 )
        {
            m_Table = new Dictionary<Type, Item>();
        }

        public Item Lookup( Type key )
        {
            Item e;
            m_Table.TryGetValue( key, out e );

            if( e == null )
            {
                e = GetEntity( key );
                if( e != null )
                    Store( key, e );
            }

            return e;
        }

        private static Item GetEntity( Type t )
        {
            return (Item)Activator.CreateInstance( t );
        }

        private void Store( Type key, Item obj )
        {
            m_Table[ key ] = obj;

            XmlAttach.AttachTo( obj, new XmlCachedItem() );

            AddItem( obj );
        }

        public OldDisplayCache( Serial serial )
            : base( serial )
        {
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for( int i = Items.Count - 1; i >= 0; --i )
                if( i < Items.Count )
                    Items[ i ].Delete();

            if( m_Cache == this )
                m_Cache = null;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt(); // version

            for( int i = Items.Count - 1; i >= 0; --i )
                if( i < Items.Count )
                    Items[ i ].Delete();

            if( m_Cache == null )
                m_Cache = this;
            else
                Delete();

            m_Table = new Dictionary<Type, Item>();
        }
    }
}