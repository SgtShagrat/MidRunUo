/*
using System.Collections;
using System.IO;
using Server;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class Mobile : UOEntity
    {
        private static uint[] m_NotoHues = new uint[] { 0, 0x30d0e0, 0x60e000, 0x9090b2, 0x909090, 0xd88038, 0xb01000, 0xe0e000 };

        private bool m_Blessed;
        private ushort m_Body;
        private Point2D m_ButtonPoint;
        private Direction m_Direction;
        private bool m_Female;
        private ushort m_Hits;
        private ushort m_HitsMax;
        private ArrayList m_Items;
        protected ushort m_Mana;
        protected ushort m_ManaMax;
        private byte m_Map;
        private string m_Name;

        private byte m_Notoriety;
        private bool m_Poisoned;
        protected ushort m_Stam;
        protected ushort m_StamMax;
        private bool m_Visible;
        private bool m_Warmode;

        public Mobile( Serial serial )
            : base( serial )
        {
            m_ButtonPoint = Point2D.Zero;
            m_Items = new ArrayList();
            m_Map = ( World.Player == null ) ? ( (byte)0 ) : World.Player.Map;
            m_Visible = true;
            // Agent.InvokeMobileCreated( this );
        }

        public Mobile( BinaryReader reader, int version )
            : base( reader, version )
        {
            m_ButtonPoint = Point2D.Zero;
            m_Body = reader.ReadUInt16();
            m_Direction = (Direction)reader.ReadByte();
            m_Name = reader.ReadString();
            m_Notoriety = reader.ReadByte();
            ProcessPacketFlags( reader.ReadByte() );
            m_HitsMax = reader.ReadUInt16();
            m_Hits = reader.ReadUInt16();
            m_Map = reader.ReadByte();
            int capacity = reader.ReadInt32();
            m_Items = new ArrayList( capacity );
            for( int i = 0; i < capacity; i++ )
            {
                // m_Items.Add( (Serial)reader.ReadUInt32() );
            }
        }

        //public Item Backpack
        //{
        //    get { return GetItemOnLayer( Layer.Backpack ); }
        //}

        public bool Blessed
        {
            get { return m_Blessed; }
            set { m_Blessed = value; }
        }

        public ushort Body
        {
            get { return m_Body; }
            set { m_Body = value; }
        }

        internal Point2D ButtonPoint
        {
            get { return m_ButtonPoint; }
            set { m_ButtonPoint = value; }
        }

        public ArrayList Contains
        {
            get { return m_Items; }
        }

        public Direction Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public bool Female
        {
            get { return m_Female; }
            set { m_Female = value; }
        }

        public ushort Hits
        {
            get { return m_Hits; }
            set { m_Hits = value; }
        }

        public ushort HitsMax
        {
            get { return m_HitsMax; }
            set { m_HitsMax = value; }
        }

        //public bool InParty
        //{
        //    get { return PacketHandlers.Party.Contains( base.Serial ); }
        //}

        public bool IsGhost
        {
            get
            {
                if( ( ( m_Body != 0x192 ) && ( m_Body != 0x193 ) ) && ( ( m_Body != 0x25f ) && ( m_Body != 0x260 ) ) )
                {
                    return ( m_Body == 970 );
                }
                return true;
            }
        }

        public ushort Mana
        {
            get { return m_Mana; }
            set { m_Mana = value; }
        }

        public ushort ManaMax
        {
            get { return m_ManaMax; }
            set { m_ManaMax = value; }
        }

        public byte Map
        {
            get { return m_Map; }
            set
            {
                if( m_Map != value )
                {
                    OnMapChange( m_Map, value );
                    m_Map = value;
                }
            }
        }

        public string Name
        {
            get
            {
                if( m_Name == null )
                {
                    return "";
                }
                return m_Name;
            }
            set
            {
                if( value != null )
                {
                    string str = value.Trim();
                    if( str.Length > 0 )
                    {
                        m_Name = str;
                    }
                }
            }
        }

        public byte Notoriety
        {
            get { return m_Notoriety; }
            set
            {
                if( value != Notoriety )
                {
                    OnNotoChange( m_Notoriety, value );
                    m_Notoriety = value;
                }
            }
        }

        public bool Poisoned
        {
            get { return m_Poisoned; }
            set { m_Poisoned = value; }
        }

        public Item Quiver
        {
            get
            {
                //Item itemOnLayer = GetItemOnLayer( Layer.Cloak );
                //if( ( itemOnLayer != null ) && itemOnLayer.IsContainer )
                //{
                //    return itemOnLayer;
                //}
                return null;
            }
        }

        public ushort Stam
        {
            get { return m_Stam; }
            set { m_Stam = value; }
        }

        public ushort StamMax
        {
            get { return m_StamMax; }
            set { m_StamMax = value; }
        }

        public bool Visible
        {
            get { return m_Visible; }
            set { m_Visible = value; }
        }

        public bool Warmode
        {
            get { return m_Warmode; }
            set { m_Warmode = value; }
        }

        public void AddItem( Item item )
        {
            m_Items.Add( item );
        }

        public override void AfterLoad()
        {
            for( int i = 0; i < m_Items.Count; i++ )
            {
                if( m_Items[ i ] is Serial )
                {
                    // m_Items[ i ] = World.FindItem( (Serial)m_Items[ i ] );
                    if( m_Items[ i ] == null )
                    {
                        m_Items.RemoveAt( i );
                        i--;
                    }
                }
            }
        }

        //public Item FindItemByID( ItemID id )
        //{
        //    for( int i = 0; i < Contains.Count; i++ )
        //    {
        //        var item = (Item)Contains[ i ];
        //        if( item.ItemID == id )
        //        {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

        //public Item GetItemOnLayer( Layer layer )
        //{
        //    for( int i = 0; i < m_Items.Count; i++ )
        //    {
        //        var item = (Item)m_Items[ i ];
        //        if( item.Layer == layer )
        //        {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

        public uint GetNotorietyColor()
        {
            if( ( m_Notoriety >= 0 ) && ( m_Notoriety < m_NotoHues.Length ) )
            {
                return m_NotoHues[ m_Notoriety ];
            }
            return m_NotoHues[ 0 ];
        }

        public int GetPacketFlags()
        {
            int num = 0;
            if( m_Female )
            {
                num |= 2;
            }
            if( m_Poisoned )
            {
                num |= 4;
            }
            if( m_Blessed )
            {
                num |= 8;
            }
            if( m_Warmode )
            {
                num |= 0x40;
            }
            if( !m_Visible )
            {
                num |= 0x80;
            }
            return num;
        }

        public byte GetStatusCode()
        {
            if( m_Poisoned )
            {
                return 1;
            }
            return 0;
        }

        public virtual void OnMapChange( byte old, byte cur )
        {
        }

        protected virtual void OnNotoChange( byte old, byte cur )
        {
        }

        public override void OnPositionChanging( Point3D newPos )
        {
            //if( ( this != World.Player ) && ( Engine.MainWindow.MapWindow != null ) )
            //{
            //    Engine.MainWindow.MapWindow.CheckLocalUpdate( this );
            //}
            base.OnPositionChanging( newPos );
        }

        public void ProcessPacketFlags( byte flags )
        {
            //if( !PacketHandlers.UseNewStatus )
            //{
            //    m_Poisoned = ( flags & 4 ) != 0;
            //}
            m_Female = ( flags & 2 ) != 0;
            m_Blessed = ( flags & 8 ) != 0;
            m_Warmode = ( flags & 0x40 ) != 0;
            m_Visible = ( flags & 0x80 ) == 0;
        }

        public override void Remove()
        {
            var list = new ArrayList( m_Items );
            m_Items.Clear();
            for( int i = 0; i < list.Count; i++ )
            {
                ( (Item)list[ i ] ).Remove();
            }
            //if( !InParty )
            //{
            //    base.Remove();
            //    World.RemoveMobile( this );
            ////}
            //else
            //{
            //    Visible = false;
            //}
        }

        public void RemoveItem( Item item )
        {
            m_Items.Remove( item );
        }

        public override void SaveState( BinaryWriter writer )
        {
            base.SaveState( writer );

            writer.Write( m_Body );
            writer.Write( (byte)m_Direction );
            writer.Write( m_Name ?? "" );
            writer.Write( m_Notoriety );
            writer.Write( (byte)GetPacketFlags() );
            writer.Write( m_HitsMax );
            writer.Write( m_Hits );
            writer.Write( m_Map );
            writer.Write( m_Items.Count );
            for( int i = 0; i < m_Items.Count; i++ )
            {
                // writer.Write( (uint)( (Item)m_Items[ i ] ).Serial );
            }
        }
    }
}
*/