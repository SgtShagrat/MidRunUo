using System.Collections;
using Server;

namespace Midgard.Engines.GroupsHandler
{
    public class ItemsGroup
    {
        private string m_Description;
        private Hashtable m_Items = new Hashtable();

        public string Name { get; private set; }

        public string Description
        {
            get { return m_Description; }
            set
            {
                if( value != null )
                    m_Description = value;
            }
        }

        public bool Secure { get; set; }

        public ArrayList Items
        {
            get { return new ArrayList( m_Items.Keys ); }
        }

        public int Count
        {
            get { return m_Items.Count; }
        }

        public bool AddItem( Item item, bool secure )
        {
            if( ( secure || !Secure ) && !GroupsHandler.IsInvalid( item ) )
            {
                m_Items[ item ] = true;
                return true;
            }

            return false;
        }

        public bool RemoveItem( Item item, bool secure )
        {
            if( item != null && ( secure || !Secure ) )
            {
                m_Items.Remove( item );
                return true;
            }

            return false;
        }

        public ItemsGroup( string name, string description, bool secure )
        {
            Name = name.ToLower();
            m_Description = ( description ?? "" );
            Secure = secure;
        }

        public void Save( BinaryFileWriter writer )
        {
            ArrayList items = new ArrayList( m_Items.Keys );

            foreach( Item item in items )
            {
                if( GroupsHandler.IsInvalid( item ) || item.Serial.Value == 0 )
                {
                    m_Items.Remove( item );
                }
                else
                {
                    writer.Write( item.Serial.Value );
                }
            }

            writer.Write( 0 ); // end
        }

        public void Load( BinaryFileReader reader )
        {
            while( !reader.End() )
            {
                int serial = reader.ReadInt();

                if( serial == 0 )
                    return;

                Item item = World.FindItem( serial );

                if( !GroupsHandler.IsInvalid( item ) )
                    m_Items[ item ] = true;
            }
        }

        public bool Contains( Item item )
        {
            return m_Items[ item ] != null;
        }

        public bool IsAccessible( bool secure )
        {
            return secure || !Secure;
        }
    }
}