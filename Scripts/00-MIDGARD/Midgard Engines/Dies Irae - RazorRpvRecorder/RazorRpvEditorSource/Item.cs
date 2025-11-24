/*
using System;
using System.Collections;
using System.IO;
using Server;
using Ultima;
using MultiComponentList=Ultima.MultiComponentList;
using TileFlag=Ultima.TileFlag;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class Item : UOEntity
    {
        private static ArrayList m_AutoStackCache = new ArrayList();
        private static ArrayList m_NeedContUpdate = new ArrayList();
        private ushort m_Amount;
        private bool m_AutoStack;
        private string m_BuyDesc;
        private byte m_Direction;
        private byte m_GridNum;
        private byte[] m_HousePacket;
        private int m_HouseRev;
        private bool m_IsNew;
        private int m_ItemID;
        private ArrayList m_Items;
        private Layer m_Layer;
        private bool m_Movable;
        private string m_Name;
        private object m_Parent;
        private int m_Price;
        private Timer m_RemoveTimer;
        private bool m_Visible;

        public Item( Serial serial ) : base( serial )
        {
            m_RemoveTimer = null;
            m_Items = new ArrayList();
            m_Visible = true;
            m_Movable = true;
            // Agent.InvokeItemCreated( this );
        }

        public Item( BinaryReader reader, byte version ) : base( reader, version )
        {
            m_RemoveTimer = null;
            m_ItemID = reader.ReadUInt16();
            m_Amount = reader.ReadUInt16();
            m_Direction = reader.ReadByte();
            ProcessPacketFlags( reader.ReadByte() );
            m_Layer = (Layer)reader.ReadByte();
            m_Name = reader.ReadString();
            m_Parent = reader.ReadUInt32();
            if( ( (Serial)m_Parent ) == base.Serial.Zero )
            {
                m_Parent = null;
            }
            int capacity = reader.ReadInt32();
            m_Items = new ArrayList( capacity );
            for( int i = 0; i < capacity; i++ )
            {
                m_Items.Add( (Serial)reader.ReadUInt32() );
            }
            if( version > 2 )
            {
                m_HouseRev = reader.ReadInt32();
                if( m_HouseRev != 0 )
                {
                    int count = reader.ReadUInt16();
                    m_HousePacket = reader.ReadBytes( count );
                }
            }
            else
            {
                m_HouseRev = 0;
                m_HousePacket = null;
            }
        }

        public ushort Amount
        {
            get { return m_Amount; }
            set { m_Amount = value; }
        }

        public bool AutoStack
        {
            get { return m_AutoStack; }
            set { m_AutoStack = value; }
        }

        public string BuyDesc
        {
            get { return m_BuyDesc; }
            set { m_BuyDesc = value; }
        }

        public object Container
        {
            get
            {
                if( ( m_Parent is Serial ) && UpdateContainer() )
                {
                    m_NeedContUpdate.Remove( this );
                }
                return m_Parent;
            }
            set
            {
                if( ( ( ( m_Parent == null ) || !m_Parent.Equals( value ) ) &&
                      ( ( !( value is Serial ) || !( m_Parent is UOEntity ) ) ||
                        ( ( (UOEntity)m_Parent ).Serial != ( (Serial)value ) ) ) ) &&
                    ( ( !( m_Parent is Serial ) || !( value is UOEntity ) ) ||
                      ( ( (UOEntity)value ).Serial != ( (Serial)m_Parent ) ) ) )
                {
                    if( m_Parent is Mobile )
                    {
                        ( (Mobile)m_Parent ).RemoveItem( this );
                    }
                    else if( m_Parent is Item )
                    {
                        ( (Item)m_Parent ).RemoveItem( this );
                    }
                    if( ( World.Player != null ) &&
                        ( IsChildOf( World.Player.Backpack ) || IsChildOf( World.Player.Quiver ) ) )
                    {
                        Counter.Uncount( this );
                    }
                    if( value is Mobile )
                    {
                        m_Parent = ( (Mobile)value ).Serial;
                    }
                    else if( value is Item )
                    {
                        m_Parent = ( (Item)value ).Serial;
                    }
                    else
                    {
                        m_Parent = value;
                    }
                    if( !UpdateContainer() && ( m_NeedContUpdate != null ) )
                    {
                        m_NeedContUpdate.Add( this );
                    }
                }
            }
        }

        public ArrayList Contains
        {
            get { return m_Items; }
        }

        public byte Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public byte GridNum
        {
            get { return m_GridNum; }
            set { m_GridNum = value; }
        }

        public byte[] HousePacket
        {
            get { return m_HousePacket; }
            set { m_HousePacket = value; }
        }

        public int HouseRevision
        {
            get { return m_HouseRev; }
            set { m_HouseRev = value; }
        }

        public bool IsBagOfSending
        {
            get { return ( ( Hue >= 0x400 ) && ( m_ItemID.Value == 0xe76 ) ); }
        }

        public bool IsContainer
        {
            get
            {
                ushort num = m_ItemID.Value;
                if( ( ( ( ( m_Items.Count <= 0 ) || IsCorpse ) && ( ( num < 0x9a8 ) || ( num > 0x9ac ) ) ) &&
                      ( ( ( num < 0x9b0 ) || ( num > 0x9b2 ) ) && ( ( num < 0xa2c ) || ( num > 0xa53 ) ) ) ) &&
                    ( ( ( ( num < 0xa97 ) || ( num > 0xa9e ) ) && ( ( num < 0xe3c ) || ( num > 0xe43 ) ) ) &&
                      ( ( ( ( num < 0xe75 ) || ( num > 0xe80 ) ) || ( num == 0xe7b ) ) &&
                        ( ( ( ( num != 0x1e80 ) && ( num != 0x1e81 ) ) && ( ( num != 0x232a ) && ( num != 0x232b ) ) ) &&
                          ( ( ( num != 0x2b02 ) && ( num != 0x2b03 ) ) && ( num != 0x2fb7 ) ) ) ) ) )
                {
                    return ( num == 0x3171 );
                }
                return true;
            }
        }

        public bool IsCorpse
        {
            get { return ( ( m_ItemID.Value == 0x2006 ) || ( ( m_ItemID.Value >= 0xeca ) && ( m_ItemID.Value <= 0xed2 ) ) ); }
        }

        public bool IsDoor
        {
            get
            {
                ushort num = m_ItemID.Value;
                if( ( ( ( num < 0x675 ) || ( num > 0x6f6 ) ) && ( ( num < 0x821 ) || ( num > 0x875 ) ) ) &&
                    ( ( ( num < 0x1fed ) || ( num > 0x1ffc ) ) && ( ( num < 0x241f ) || ( num > 0x2424 ) ) ) )
                {
                    return ( ( num >= 0x2a05 ) && ( num <= 0x2a1c ) );
                }
                return true;
            }
        }

        public bool IsInBank
        {
            get
            {
                if( m_Parent is Item )
                {
                    return ( (Item)m_Parent ).IsInBank;
                }
                return ( ( m_Parent is Mobile ) && ( Layer == Layer.Bank ) );
            }
        }

        public bool IsMulti
        {
            get { return ( m_ItemID.Value >= 0x4000 ); }
        }

        public bool IsNew
        {
            get { return m_IsNew; }
            set { m_IsNew = value; }
        }

        public bool IsPotion
        {
            get
            {
                if( ( ( m_ItemID.Value < 0xf06 ) || ( m_ItemID.Value > 0xf0d ) ) && ( m_ItemID.Value != 0x2790 ) )
                {
                    return ( m_ItemID.Value == 0x27db );
                }
                return true;
            }
        }

        public bool IsPouch
        {
            get { return ( m_ItemID.Value == 0xe79 ); }
        }

        public bool IsResource
        {
            get
            {
                ushort num = m_ItemID.Value;
                if( ( ( ( num < 0x19b7 ) || ( num > 0x19ba ) ) && ( ( num < 0x9cc ) || ( num > 0x9cf ) ) ) &&
                    ( ( ( num < 0x1bdd ) || ( num > 0x1be2 ) ) && ( ( num != 0x1779 ) && ( num != 0x11ea ) ) ) )
                {
                    return ( num == 0x11eb );
                }
                return true;
            }
        }

        public bool IsTwoHanded
        {
            get
            {
                ushort num = m_ItemID.Value;
                if( ( ( ( Layer != Layer.LeftHand ) || ( ( num >= 0x1b72 ) && ( num <= 0x1b7b ) ) ) ||
                      IsVirtueShield ) &&
                    ( ( ( ( num != 0x13fc ) && ( num != 0x13fd ) ) && ( ( num != 0x13af ) && ( num != 0x13b2 ) ) ) &&
                      ( ( num < 0xf43 ) || ( num > 0xf50 ) ) ) )
                {
                    switch( num )
                    {
                        case 0x1438:
                        case 0x1439:
                        case 0x1442:
                        case 0x1443:
                        case 0x1402:
                        case 0x1403:
                        case 0x26c1:
                        case 0x26cb:
                        case 0x26c2:
                        case 0x26cc:
                            goto Label_00C9;
                    }
                    if( num != 0x26c3 )
                    {
                        return ( num == 0x26cd );
                    }
                    return true;
                }
                Label_00C9:
                return true;
            }
        }

        public bool IsVirtueShield
        {
            get
            {
                ushort num = m_ItemID.Value;
                return ( ( num >= 0x1bc3 ) && ( num <= 0x1bc5 ) );
            }
        }

        public int ItemID
        {
            get { return m_ItemID; }
            set { m_ItemID = value; }
        }

        public Layer Layer
        {
            get
            {
                if( ( ( m_Layer < Layer.FirstValid ) || ( m_Layer > Layer.Bank ) ) &&
                    ( ( ( ( ItemID.ItemData.Flags & TileFlag.Wearable ) != TileFlag.None ) ||
                        ( ( ItemID.ItemData.Flags & TileFlag.Armor ) != TileFlag.None ) ) ||
                      ( ( ItemID.ItemData.Flags & TileFlag.Weapon ) != TileFlag.None ) ) )
                {
                    m_Layer = (Layer)( (byte)ItemID.ItemData.Quality );
                }
                return m_Layer;
            }
            set { m_Layer = value; }
        }

        public bool Movable
        {
            get { return m_Movable; }
            set { m_Movable = value; }
        }

        public string Name
        {
            get
            {
                if( ( m_Name != null ) && ( m_Name != "" ) )
                {
                    return m_Name;
                }
                return m_ItemID.ToString();
            }
            set
            {
                if( value != null )
                {
                    m_Name = value.Trim();
                }
                else
                {
                    m_Name = null;
                }
            }
        }

        public bool OnGround
        {
            get { return ( Container == null ); }
        }

        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        public object RootContainer
        {
            get
            {
                int num = 100;
                object container = Container;
                while( ( ( container != null ) && ( container is Item ) ) && ( num-- > 0 ) )
                {
                    container = ( (Item)container ).Container;
                }
                return container;
            }
        }

        public bool Visible
        {
            get { return m_Visible; }
            set { m_Visible = value; }
        }

        private void AddItem( Item item )
        {
            for( int i = 0; i < m_Items.Count; i++ )
            {
                if( m_Items[ i ] == item )
                {
                    return;
                }
            }
            m_Items.Add( item );
        }

        public override void AfterLoad()
        {
            for( int i = 0; i < m_Items.Count; i++ )
            {
                if( m_Items[ i ] is Serial )
                {
                    m_Items[ i ] = World.FindItem( (Serial)m_Items[ i ] );
                    if( m_Items[ i ] == null )
                    {
                        m_Items.RemoveAt( i );
                        i--;
                    }
                }
            }
            UpdateContainer();
        }

        public void AutoStackResource()
        {
            if( ( IsResource && Config.GetBool( "AutoStack" ) ) && !m_AutoStackCache.Contains( base.Serial ) )
            {
                foreach( Item item in World.Items.Values )
                {
                    if( ( ( item.Container == null ) && ( item.ItemID == ItemID ) ) &&
                        ( ( item.Hue == Hue ) && Utility.InRange( World.Player.Position, item.Position, 2 ) ) )
                    {
                        DragDropManager.DragDrop( this, item );
                        m_AutoStackCache.Add( base.Serial );
                        return;
                    }
                }
                DragDropManager.DragDrop( this, World.Player.Position );
                m_AutoStackCache.Add( base.Serial );
            }
        }

        public bool CancelRemove()
        {
            if( ( m_RemoveTimer != null ) && m_RemoveTimer.Running )
            {
                m_RemoveTimer.Stop();
                return true;
            }
            return false;
        }

        public int DistanceTo( Mobile m )
        {
            int num = Math.Abs( (int)( Position.X - m.Position.X ) );
            int num2 = Math.Abs( (int)( Position.Y - m.Position.Y ) );
            if( num <= num2 )
            {
                return num2;
            }
            return num;
        }

        public Item FindItemByID( int id )
        {
            return FindItemByID( id, true );
        }

        public Item FindItemByID( int id, bool recurse )
        {
            for( int i = 0; i < m_Items.Count; i++ )
            {
                var item = (Item)m_Items[ i ];
                if( item.ItemID == id )
                {
                    return item;
                }
                if( recurse )
                {
                    item = item.FindItemByID( id, true );
                    if( item != null )
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public int GetCount( ushort iid )
        {
            int num = 0;
            for( int i = 0; i < m_Items.Count; i++ )
            {
                var item = (Item)m_Items[ i ];
                if( item.ItemID == iid )
                {
                    num += item.Amount;
                }
                else if( ( ( item.ItemID == 0xe34 ) && ( iid == 0xef3 ) ) ||
                         ( ( item.ItemID == 0xef3 ) && ( iid == 0xe34 ) ) )
                {
                    num += item.Amount;
                }
                num += item.GetCount( iid );
            }
            return num;
        }

        public byte GetPacketFlags()
        {
            byte num = 0;
            if( !m_Visible )
            {
                num = (byte)( num | 0x80 );
            }
            if( m_Movable )
            {
                num = (byte)( num | 0x20 );
            }
            return num;
        }

        public Point3D GetWorldPosition()
        {
            int num = 100;
            object container = Container;
            while( ( ( container != null ) && ( container is Item ) ) &&
                   ( ( ( (Item)container ).Container != null ) && ( num-- > 0 ) ) )
            {
                container = ( (Item)container ).Container;
            }
            if( container is Item )
            {
                return ( (Item)container ).Position;
            }
            if( container is Mobile )
            {
                return ( (Mobile)container ).Position;
            }
            return Position;
        }

        public bool IsChildOf( object parent )
        {
            Serial serial = 0;
            if( parent is Mobile )
            {
                return ( parent == RootContainer );
            }
            if( parent is Item )
            {
                serial = ( (Item)parent ).Serial;
            }
            else
            {
                return false;
            }
            object container = this;
            int num = 100;
            while( ( ( container != null ) && ( container is Item ) ) && ( num-- > 0 ) )
            {
                if( ( (Item)container ).Serial == serial )
                {
                    return true;
                }
                container = ( (Item)container ).Container;
            }
            return false;
        }

        public void MakeHousePacket()
        {
            m_HousePacket = null;
            try
            {
                string filePath =
                    Client.GetFilePath( string.Format( "Desktop/{0}/{1}/{2}/Multicache.dat", World.AccountName,
                                                       World.ShardName, World.OrigPlayerName ) );
                if( ( ( filePath != null ) && ( filePath != "" ) ) && File.Exists( filePath ) )
                {
                    using(
                        var reader =
                            new StreamReader( new FileStream( filePath, FileMode.Open, FileAccess.Read,
                                                              FileShare.ReadWrite ) ) )
                    {
                        string str2;
                        reader.ReadLine();
                        int num = 0;
                        int index = 0;
                        while( ( str2 = reader.ReadLine() ) != null )
                        {
                            if( ( ( index++ >= num ) && ( str2 != "" ) ) && ( str2[ 0 ] != ';' ) )
                            {
                                string[] strArray = str2.Split( new char[] { ' ', '\t' } );
                                if( strArray.Length <= 0 )
                                {
                                    return;
                                }
                                num = 0;
                                var serial = (Serial)Utility.ToInt32( strArray[ 0 ], 0 );
                                int num3 = Utility.ToInt32( strArray[ 1 ], 0 );
                                int num4 = Utility.ToInt32( strArray[ 2 ], 0 );
                                if( serial == base.Serial )
                                {
                                    m_HouseRev = num3;
                                    Assistant.MultiTileEntry[] tiles = new Assistant.MultiTileEntry[ num4 ];
                                    index = 0;
                                    MultiComponentList components = Ultima.Multis.GetComponents( (int)m_ItemID );
                                    while( ( ( str2 = reader.ReadLine() ) != null ) && ( index < num4 ) )
                                    {
                                        strArray = str2.Split( new char[] { ' ', '\t' } );
                                        tiles[ index ] = new Assistant.MultiTileEntry();
                                        tiles[ index ].m_ItemID = (ushort)Utility.ToInt32( strArray[ 0 ], 0 );
                                        tiles[ index ].m_OffsetX =
                                            (short)( Utility.ToInt32( strArray[ 1 ], 0 ) + components.Center.X );
                                        tiles[ index ].m_OffsetX =
                                            (short)( Utility.ToInt32( strArray[ 2 ], 0 ) + components.Center.Y );
                                        tiles[ index ].m_OffsetX = (short)Utility.ToInt32( strArray[ 3 ], 0 );
                                        index++;
                                    }
                                    m_HousePacket =
                                        new DesignStateDetailed( base.Serial, m_HouseRev, components.Min.X,
                                                                 components.Min.Y, components.Max.X, components.Max.Y,
                                                                 tiles ).Compile();
                                    return;
                                }
                                num = num4;
                                index = 0;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public override void OnPositionChanging( Point3D newPos )
        {
            if( ( IsMulti && ( Position != Point3D.Zero ) ) && ( ( newPos != Point3D.Zero ) && ( Position != newPos ) ) )
            {
                ClientCommunication.PostRemoveMulti( this );
                ClientCommunication.PostAddMulti( m_ItemID, newPos );
            }
            base.OnPositionChanging( newPos );
        }

        public void ProcessPacketFlags( byte flags )
        {
            m_Visible = ( flags & 0x80 ) == 0;
            m_Movable = ( flags & 0x20 ) != 0;
        }

        public override void Remove()
        {
            if( IsMulti )
            {
                ClientCommunication.PostRemoveMulti( this );
            }
            var list = new ArrayList( m_Items );
            m_Items.Clear();
            for( int i = 0; i < list.Count; i++ )
            {
                ( (Item)list[ i ] ).Remove();
            }
            Counter.Uncount( this );
            if( m_Parent is Mobile )
            {
                ( (Mobile)m_Parent ).RemoveItem( this );
            }
            else if( m_Parent is Item )
            {
                ( (Item)m_Parent ).RemoveItem( this );
            }
            World.RemoveItem( this );
            base.Remove();
        }

        private void RemoveItem( Item item )
        {
            m_Items.Remove( item );
        }

        public void RemoveRequest()
        {
            if( m_RemoveTimer == null )
            {
                m_RemoveTimer = Timer.DelayedCallback( TimeSpan.FromSeconds( 0.25 ), new TimerCallback( Remove ) );
            }
            else if( m_RemoveTimer.Running )
            {
                m_RemoveTimer.Stop();
            }
            m_RemoveTimer.Start();
        }

        public override void SaveState( BinaryWriter writer )
        {
            base.SaveState( writer );

            writer.Write( (ushort)m_ItemID );
            writer.Write( m_Amount );
            writer.Write( m_Direction );
            writer.Write( GetPacketFlags() );
            writer.Write( (byte)m_Layer );
            writer.Write( ( m_Name == null ) ? "" : m_Name );
            if( m_Parent is UOEntity )
            {
                writer.Write( (uint)( (UOEntity)m_Parent ).Serial );
            }
            else if( m_Parent is Serial )
            {
                writer.Write( (uint)( (Serial)m_Parent ) );
            }
            else
            {
                writer.Write( (uint)0 );
            }
            writer.Write( 0 );
            if( ( m_HouseRev != 0 ) && ( m_HousePacket == null ) )
            {
                MakeHousePacket();
            }
            if( ( m_HouseRev != 0 ) && ( m_HousePacket != null ) )
            {
                writer.Write( m_HouseRev );
                writer.Write( (ushort)m_HousePacket.Length );
                writer.Write( m_HousePacket );
            }
            else
            {
                writer.Write( 0 );
            }
        }

        public override string ToString()
        {
            return string.Format( "{0} ({1})", Name, base.Serial );
        }

        public bool UpdateContainer()
        {
            if( ( m_Parent is Serial ) && !base.Deleted )
            {
                object obj2 = null;
                var parent = (Serial)m_Parent;
                if( parent.IsItem )
                {
                    obj2 = World.FindItem( parent );
                }
                else if( parent.IsMobile )
                {
                    obj2 = World.FindMobile( parent );
                }
                if( obj2 == null )
                {
                    return false;
                }
                m_Parent = obj2;
                if( m_Parent is Item )
                {
                    ( (Item)m_Parent ).AddItem( this );
                }
                else if( m_Parent is Mobile )
                {
                    ( (Mobile)m_Parent ).AddItem( this );
                }
                if( IsChildOf( World.Player.Backpack ) || IsChildOf( World.Player.Quiver ) )
                {
                    bool flag = SearchExemptionAgent.IsExempt( this );
                    if( !flag )
                    {
                        Counter.Count( this );
                    }
                    if( m_IsNew )
                    {
                        if( m_AutoStack )
                        {
                            AutoStackResource();
                        }
                        if( ( ( IsContainer && !flag ) && ( !IsPouch || !Config.GetBool( "NoSearchPouches" ) ) ) &&
                            Config.GetBool( "AutoSearch" ) )
                        {
                            PacketHandlers.IgnoreGumps.Add( this );
                            PlayerData.DoubleClick( this );
                            for( int i = 0; i < Contains.Count; i++ )
                            {
                                var item = (Item)Contains[ i ];
                                if( ( item.IsContainer && !SearchExemptionAgent.IsExempt( item ) ) &&
                                    ( !item.IsPouch || !Config.GetBool( "NoSearchPouches" ) ) )
                                {
                                    PacketHandlers.IgnoreGumps.Add( item );
                                    PlayerData.DoubleClick( item );
                                }
                            }
                        }
                    }
                }
                m_AutoStack = m_IsNew = false;
            }
            return true;
        }

        public static void UpdateContainers()
        {
            int index = 0;
            while( index < m_NeedContUpdate.Count )
            {
                if( ( (Item)m_NeedContUpdate[ index ] ).UpdateContainer() )
                {
                    m_NeedContUpdate.RemoveAt( index );
                }
                else
                {
                    index++;
                }
            }
        }
    }
}
*/