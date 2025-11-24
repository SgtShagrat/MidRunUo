/*
using System;
using System.Collections;
using System.IO;
using Server;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class PlayerData : Mobile
    {
        public static int FastWalkKey = 0;
        private static bool m_ExternZ = false;

        private static Timer m_OpenDoorReq = Timer.DelayedCallback( TimeSpan.FromSeconds( 0.005 ),
                                                                    new TimerCallback( OpenDoor ) );

        public uint CurrentGumpI;
        public uint CurrentGumpS;
        public ushort CurrentMenuI;
        public uint CurrentMenuS;
        public bool HasGump;
        public bool HasMenu;
        private ushort m_AR;
        private short m_ColdResist;
        private DateTime m_CriminalStart;
        private Timer m_CriminalTime;
        private ushort m_DamageMax;
        private ushort m_DamageMin;
        private ushort m_Dex;
        private LockType m_DexLock;
        private short m_EnergyResist;
        private ushort m_Features;
        private short m_FireResist;
        private byte m_Followers;
        private byte m_FollowersMax;
        private byte m_GlobalLight;
        private uint m_Gold;
        private ushort m_Int;
        private LockType m_IntLock;
        private Serial m_LastDoor;
        private DateTime m_LastDoorTime;
        private Serial m_LastObj;
        private int m_LastSkill;
        private int m_LastSpell;
        private sbyte m_LocalLight;
        private short m_Luck;
        private int[] m_MapPatches;
        private int m_MaxWeight;
        private Hashtable m_MoveInfo;

        private int m_OutstandingMoves;
        private short m_PoisonResist;
        private byte m_Season;
        private Skill[] m_Skills;
        private bool m_SkillsSent;
        private ushort m_SpeechHue;
        private ushort m_StatCap;
        private ushort m_Str;
        private LockType m_StrLock;
        private int m_Tithe;
        private byte m_WalkSeq;
        private ushort m_Weight;
        public int VisRange;

        public PlayerData( Serial serial )
            : base( serial )
        {
            VisRange = 0x12;
            m_MaxWeight = -1;
            m_MapPatches = new int[ 10 ];
            m_CriminalStart = DateTime.MinValue;
            m_OutstandingMoves = 0;
            m_LastDoor = Serial.Zero;
            m_LastDoorTime = DateTime.MinValue;
            m_LastSkill = -1;
            m_LastObj = Serial.Zero;
            m_LastSpell = -1;
            m_MoveInfo = new Hashtable( 0x100 );
            m_Skills = new Skill[ 100 ]; // :D
            for( int i = 0; i < m_Skills.Length; i++ )
            {
                // m_Skills[ i ] = new Skill( i );
            }
        }

        public PlayerData( BinaryReader reader, int version )
            : base( reader, version )
        {
            int num;
            VisRange = 0x12;
            m_MaxWeight = -1;
            m_MapPatches = new int[ 10 ];
            m_CriminalStart = DateTime.MinValue;
            m_OutstandingMoves = 0;
            m_LastDoor = ( (UOEntity)this ).Serial.Zero;
            m_LastDoorTime = DateTime.MinValue;
            m_LastSkill = -1;
            m_LastObj = ( (UOEntity)this ).Serial.Zero;
            m_LastSpell = -1;
            m_Str = reader.ReadUInt16();
            m_Dex = reader.ReadUInt16();
            m_Int = reader.ReadUInt16();
            base.m_StamMax = reader.ReadUInt16();
            base.m_Stam = reader.ReadUInt16();
            base.m_ManaMax = reader.ReadUInt16();
            base.m_Mana = reader.ReadUInt16();
            m_StrLock = (LockType)reader.ReadByte();
            m_DexLock = (LockType)reader.ReadByte();
            m_IntLock = (LockType)reader.ReadByte();
            m_Gold = reader.ReadUInt32();
            m_Weight = reader.ReadUInt16();
            m_MoveInfo = new Hashtable( 0x100 );
            if( version >= 4 )
            {
                Skill.Count = num = reader.ReadByte();
            }
            else if( version != 3 )
            {
                Skill.Count = num = 0x34;
            }
            else
            {
                long position = reader.BaseStream.Position;
                num = 0;
                reader.BaseStream.Seek( 0x157L, SeekOrigin.Current );
                for( int k = 0x30; k < 60; k++ )
                {
                    ushort num4 = reader.ReadUInt16();
                    ushort num5 = reader.ReadUInt16();
                    ushort num6 = reader.ReadUInt16();
                    byte num7 = reader.ReadByte();
                    if( ( ( num4 > 0x7d0 ) || ( num5 > 0x7d0 ) ) || ( ( num6 > 0x7d0 ) || ( num7 > 2 ) ) )
                    {
                        num = k;
                        break;
                    }
                }
                if( num == 0 )
                {
                    num = 0x34;
                }
                else if( num > 0x36 )
                {
                    num = 0x36;
                }
                Skill.Count = num;
                reader.BaseStream.Seek( position, SeekOrigin.Begin );
            }
            m_Skills = new Skill[ num ];
            for( int i = 0; i < num; i++ )
            {
                m_Skills[ i ] = new Skill( i );
                m_Skills[ i ].FixedBase = reader.ReadUInt16();
                m_Skills[ i ].FixedCap = reader.ReadUInt16();
                m_Skills[ i ].FixedValue = reader.ReadUInt16();
                m_Skills[ i ].Lock = (LockType)reader.ReadByte();
            }
            m_AR = reader.ReadUInt16();
            m_StatCap = reader.ReadUInt16();
            m_Followers = reader.ReadByte();
            m_FollowersMax = reader.ReadByte();
            m_Tithe = reader.ReadInt32();
            m_LocalLight = reader.ReadSByte();
            m_GlobalLight = reader.ReadByte();
            m_Features = reader.ReadUInt16();
            m_Season = reader.ReadByte();
            if( version >= 4 )
            {
                num = reader.ReadByte();
            }
            else
            {
                num = 8;
            }
            m_MapPatches = new int[ num ];
            for( int j = 0; j < num; j++ )
            {
                m_MapPatches[ j ] = reader.ReadInt32();
            }
        }

        public ushort AR
        {
            get { return m_AR; }
            set { m_AR = value; }
        }

        public int CalcZ
        {
            get
            {
                if( m_ExternZ && ClientCommunication.IsCalibrated() )
                {
                    return Position.Z;
                }
                return Map.ZTop( base.Map, Position.X, Position.Y, Position.Z );
            }
        }

        public short ColdResistance
        {
            get { return m_ColdResist; }
            set { m_ColdResist = value; }
        }

        public int CriminalTime
        {
            get
            {
                if( m_CriminalStart != DateTime.MinValue )
                {
                    var span = (TimeSpan)( DateTime.Now - m_CriminalStart );
                    var totalSeconds = (int)span.TotalSeconds;
                    if( totalSeconds <= 300 )
                    {
                        return totalSeconds;
                    }
                    if( m_CriminalTime != null )
                    {
                        m_CriminalTime.Stop();
                    }
                    m_CriminalStart = DateTime.MinValue;
                }
                return 0;
            }
        }

        public ushort DamageMax
        {
            get { return m_DamageMax; }
            set { m_DamageMax = value; }
        }

        public ushort DamageMin
        {
            get { return m_DamageMin; }
            set { m_DamageMin = value; }
        }

        public ushort Dex
        {
            get { return m_Dex; }
            set { m_Dex = value; }
        }

        public LockType DexLock
        {
            get { return m_DexLock; }
            set { m_DexLock = value; }
        }

        public short EnergyResistance
        {
            get { return m_EnergyResist; }
            set { m_EnergyResist = value; }
        }

        public static bool ExternalZ
        {
            get { return m_ExternZ; }
            set { m_ExternZ = value; }
        }

        public ushort Features
        {
            get { return m_Features; }
            set { m_Features = value; }
        }

        public short FireResistance
        {
            get { return m_FireResist; }
            set { m_FireResist = value; }
        }

        public byte Followers
        {
            get { return m_Followers; }
            set { m_Followers = value; }
        }

        public byte FollowersMax
        {
            get { return m_FollowersMax; }
            set { m_FollowersMax = value; }
        }

        public byte GlobalLightLevel
        {
            get { return m_GlobalLight; }
            set { m_GlobalLight = value; }
        }

        public uint Gold
        {
            get { return m_Gold; }
            set { m_Gold = value; }
        }

        public ushort Int
        {
            get { return m_Int; }
            set { m_Int = value; }
        }

        public LockType IntLock
        {
            get { return m_IntLock; }
            set { m_IntLock = value; }
        }

        public Serial LastObject
        {
            get { return m_LastObj; }
        }

        public int LastSkill
        {
            get { return m_LastSkill; }
            set { m_LastSkill = value; }
        }

        public int LastSpell
        {
            get { return m_LastSpell; }
            set { m_LastSpell = value; }
        }

        public sbyte LocalLightLevel
        {
            get { return m_LocalLight; }
            set { m_LocalLight = value; }
        }

        public short Luck
        {
            get { return m_Luck; }
            set { m_Luck = value; }
        }

        public int[] MapPatches
        {
            get { return m_MapPatches; }
            set { m_MapPatches = value; }
        }

        public ushort MaxWeight
        {
            get
            {
                if( m_MaxWeight == -1 )
                {
                    return (ushort)( ( m_Str * 3.5 ) + 40.0 );
                }
                return (ushort)m_MaxWeight;
            }
            set { m_MaxWeight = value; }
        }

        public int MultiVisRange
        {
            get { return ( VisRange + 5 ); }
        }

        public int OutstandingMoveReqs
        {
            get { return m_OutstandingMoves; }
        }

        public short PoisonResistance
        {
            get { return m_PoisonResist; }
            set { m_PoisonResist = value; }
        }

        public override Point3D Position
        {
            get
            {
                if( m_ExternZ && ClientCommunication.IsCalibrated() )
                {
                    var pointd = new Point3D( base.Position );
                    pointd.Z = ClientCommunication.GetZ( pointd.X, pointd.Y, pointd.Z );
                    return pointd;
                }
                return base.Position;
            }
            set
            {
                base.Position = value;
                if( ( Engine.MainWindow != null ) && ( Engine.MainWindow.MapWindow != null ) )
                {
                    Engine.MainWindow.MapWindow.PlayerMoved();
                }
            }
        }

        public byte Season
        {
            get { return m_Season; }
            set { m_Season = value; }
        }

        public Skill[] Skills
        {
            get { return m_Skills; }
        }

        public bool SkillsSent
        {
            get { return m_SkillsSent; }
            set { m_SkillsSent = value; }
        }

        public ushort SpeechHue
        {
            get { return m_SpeechHue; }
            set { m_SpeechHue = value; }
        }

        public ushort StatCap
        {
            get { return m_StatCap; }
            set { m_StatCap = value; }
        }

        public ushort Str
        {
            get { return m_Str; }
            set { m_Str = value; }
        }

        public LockType StrLock
        {
            get { return m_StrLock; }
            set { m_StrLock = value; }
        }

        public int Tithe
        {
            get { return m_Tithe; }
            set { m_Tithe = value; }
        }

        public byte WalkSequence
        {
            get { return m_WalkSeq; }
        }

        public ushort Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }

        public static bool DoubleClick( object clicked )
        {
            return DoubleClick( clicked, true );
        }

        //public static bool DoubleClick( object clicked, bool silent )
        //{
        //    Serial zero;
        //    if( clicked is Mobile )
        //    {
        //        zero = ( (Mobile)clicked ).Serial.Value;
        //    }
        //    else if( clicked is Item )
        //    {
        //        zero = ( (Item)clicked ).Serial.Value;
        //    }
        //    else if( clicked is Serial )
        //    {
        //        var serial2 = (Serial)clicked;
        //        zero = serial2.Value;
        //    }
        //    else
        //    {
        //        zero = ( (UOEntity)this ).Serial.Zero;
        //    }
        //    if( zero != ( (UOEntity)this ).Serial.Zero )
        //    {
        //        Item i = null;
        //        Item backpack = World.Player.Backpack;
        //        if( ( zero.IsItem && ( backpack != null ) ) &&
        //            ( Config.GetBool( "PotionEquip" ) && ClientCommunication.AllowBit( FeatureBit.AutoPotionEquip ) ) )
        //        {
        //            Item item3 = World.FindItem( zero );
        //            if( ( ( item3 != null ) && item3.IsPotion ) && ( item3.ItemID != 0xf0d ) )
        //            {
        //                Item itemOnLayer = World.Player.GetItemOnLayer( Layer.LeftHand );
        //                Item item5 = World.Player.GetItemOnLayer( Layer.FirstValid );
        //                if( ( itemOnLayer != null ) && ( ( item5 != null ) || itemOnLayer.IsTwoHanded ) )
        //                {
        //                    i = itemOnLayer;
        //                }
        //                else if( ( item5 != null ) && item5.IsTwoHanded )
        //                {
        //                    i = item5;
        //                }
        //                if( i != null )
        //                {
        //                    if( DragDropManager.HasDragFor( i.Serial ) )
        //                    {
        //                        i = null;
        //                    }
        //                    else
        //                    {
        //                        DragDropManager.DragDrop( i, backpack );
        //                    }
        //                }
        //            }
        //        }
        //        ActionQueue.DoubleClick( silent, zero );
        //        if( i != null )
        //        {
        //            DragDropManager.DragDrop( i, World.Player, i.Layer, true );
        //        }
        //        if( zero.IsItem )
        //        {
        //            World.Player.m_LastObj = zero;
        //        }
        //    }
        //    return false;
        //}

        public MoveEntry GetMoveEntry( byte seq )
        {
            return (MoveEntry)m_MoveInfo[ seq ];
        }

        public bool HasWalkEntry( byte seq )
        {
            return ( m_MoveInfo[ seq ] != null );
        }

        public bool MoveAck( byte seq )
        {
            m_OutstandingMoves--;
            var entry = (MoveEntry)m_MoveInfo[ seq ];
            if( entry == null )
            {
                return true;
            }
            if( entry.IsStep && !base.IsGhost )
            {
                StealthSteps.OnMove();
            }
            return !entry.FilterAck;
        }

        public void MoveRej( byte seq, Direction dir, Point3D pos )
        {
            m_OutstandingMoves--;
            base.Direction = dir;
            Position = pos;
            Resync();
        }

        //public void MoveReq( Direction dir, byte seq )
        //{
        //    m_OutstandingMoves++;
        //    FastWalkKey++;
        //    var entry = new MoveEntry();
        //    m_MoveInfo[ seq ] = entry;
        //    entry.IsStep = ( dir & base.Direction.Up ) == ( base.Direction & base.Direction.Up );
        //    entry.Dir = dir;
        //    ProcessMove( dir );
        //    entry.Position = Position;
        //    if( ( ( ( base.Body != 0x3db ) && !base.IsGhost ) &&
        //          ( ( ( ( entry.Dir & base.Direction.Up ) % ( base.Direction.North | base.Direction.East ) ) == base.Direction.North ) &&
        //            Config.GetBool( "AutoOpenDoors" ) ) ) && ClientCommunication.AllowBit( FeatureBit.AutoOpenDoors ) )
        //    {
        //        int x = Position.X;
        //        int y = Position.Y;
        //        Utility.Offset( entry.Dir, ref x, ref y );
        //        int calcZ = CalcZ;
        //        foreach( Item item in World.Items.Values )
        //        {
        //            if( ( ( ( item.Position.X == x ) && ( item.Position.Y == y ) ) &&
        //                  ( item.IsDoor && ( ( item.Position.Z - 15 ) <= calcZ ) ) ) &&
        //                ( ( ( item.Position.Z + 15 ) >= calcZ ) &&
        //                  ( ( m_LastDoor != item.Serial ) ||
        //                    ( ( m_LastDoorTime + TimeSpan.FromSeconds( 1.0 ) ) < DateTime.Now ) ) ) )
        //            {
        //                m_LastDoor = item.Serial;
        //                m_LastDoorTime = DateTime.Now;
        //                m_OpenDoorReq.Start();
        //                break;
        //            }
        //        }
        //    }
        //    entry.FilterAck = false;
        //    m_WalkSeq = ( seq >= 0xff ) ? ( (byte)1 ) : ( (byte)( seq + 1 ) );
        //}

        public override void OnMapChange( byte old, byte cur )
        {
            var list = new ArrayList( World.Mobiles.Values );
            for( int i = 0; i < list.Count; i++ )
            {
                var mobile = (Mobile)list[ i ];
                if( ( mobile != this ) && ( mobile.Map != cur ) )
                {
                    mobile.Remove();
                }
            }
            World.Items.Clear();
            Counter.Reset();
            for( int j = 0; j < base.Contains.Count; j++ )
            {
                var item = (Item)base.Contains[ j ];
                World.AddItem( item );
                item.Contains.Clear();
            }
            if( Config.GetBool( "AutoSearch" ) && ( base.Backpack != null ) )
            {
                DoubleClick( base.Backpack );
            }
            ClientCommunication.PostMapChange( cur );
            if( ( Engine.MainWindow != null ) && ( Engine.MainWindow.MapWindow != null ) )
            {
                Engine.MainWindow.MapWindow.PlayerMoved();
            }
        }

        protected override void OnNotoChange( byte old, byte cur )
        {
            if( ( ( old == 3 ) || ( old == 4 ) ) && ( ( cur != 3 ) && ( cur != 4 ) ) )
            {
                if( m_CriminalTime != null )
                {
                    m_CriminalTime.Stop();
                }
                m_CriminalStart = DateTime.MinValue;
                ClientCommunication.RequestTitlebarUpdate();
            }
            else if( ( ( cur == 3 ) || ( cur == 4 ) ) && ( ( ( old != 3 ) && ( old != 4 ) ) && ( old != 0 ) ) )
            {
                ResetCriminalTimer();
            }
        }

        public override void OnPositionChanging( Point3D newPos )
        {
            var list = new ArrayList( World.Mobiles.Values );
            for( int i = 0; i < list.Count; i++ )
            {
                var m = (Mobile)list[ i ];
                if( m != this )
                {
                    if( !Utility.InRange( m.Position, newPos, VisRange ) )
                    {
                        m.Remove();
                    }
                    else
                    {
                        Targeting.CheckLastTargetRange( m );
                    }
                }
            }
            list = new ArrayList( World.Items.Values );
            ScavengerAgent instance = ScavengerAgent.Instance;
            for( int j = 0; j < list.Count; j++ )
            {
                var item = (Item)list[ j ];
                if( !item.Deleted && ( item.Container == null ) )
                {
                    int num3 = Utility.Distance( item.GetWorldPosition(), newPos );
                    if( ( item != DragDropManager.Holding ) &&
                        ( ( num3 > MultiVisRange ) || ( !item.IsMulti && ( num3 > VisRange ) ) ) )
                    {
                        item.Remove();
                    }
                    else if( ( ( !base.IsGhost && base.Visible ) && ( ( num3 <= 2 ) && instance.Enabled ) ) &&
                             item.Movable )
                    {
                        instance.Scavenge( item );
                    }
                }
            }
            base.OnPositionChanging( newPos );
        }

        private static void OpenDoor()
        {
            if( World.Player != null )
            {
                ClientCommunication.SendToServer( new OpenDoorMacro() );
            }
        }

        public void ProcessMove( Direction dir )
        {
            if( ( dir & base.Direction.Up ) == ( base.Direction & base.Direction.Up ) )
            {
                int x = Position.X;
                int y = Position.Y;
                Utility.Offset( dir & base.Direction.Up, ref x, ref y );
                int z = Position.Z;
                try
                {
                    z = Map.ZTop( base.Map, x, y, z );
                }
                catch
                {
                }
                Position = new Point3D( x, y, z );
            }
            base.Direction = dir;
        }

        public void ResetCriminalTimer()
        {
            if( ( m_CriminalStart == DateTime.MinValue ) ||
                ( ( DateTime.Now - m_CriminalStart ) >= TimeSpan.FromSeconds( 1.0 ) ) )
            {
                m_CriminalStart = DateTime.Now;
                if( m_CriminalTime == null )
                {
                    m_CriminalTime = new CriminalTimer( this );
                }
                m_CriminalTime.Start();
                ClientCommunication.RequestTitlebarUpdate();
            }
        }

        public void Resync()
        {
            m_OutstandingMoves = m_WalkSeq = 0;
            m_MoveInfo.Clear();
        }

        public override void SaveState( BinaryWriter writer )
        {
            base.SaveState( writer );
            writer.Write( m_Str );
            writer.Write( m_Dex );
            writer.Write( m_Int );
            writer.Write( base.m_StamMax );
            writer.Write( base.m_Stam );
            writer.Write( base.m_ManaMax );
            writer.Write( base.m_Mana );
            writer.Write( (byte)m_StrLock );
            writer.Write( (byte)m_DexLock );
            writer.Write( (byte)m_IntLock );
            writer.Write( m_Gold );
            writer.Write( m_Weight );
            writer.Write( (byte)Skill.Count );
            for( int i = 0; i < Skill.Count; i++ )
            {
                writer.Write( m_Skills[ i ].FixedBase );
                writer.Write( m_Skills[ i ].FixedCap );
                writer.Write( m_Skills[ i ].FixedValue );
                writer.Write( (byte)m_Skills[ i ].Lock );
            }
            writer.Write( m_AR );
            writer.Write( m_StatCap );
            writer.Write( m_Followers );
            writer.Write( m_FollowersMax );
            writer.Write( m_Tithe );
            writer.Write( m_LocalLight );
            writer.Write( m_GlobalLight );
            writer.Write( m_Features );
            writer.Write( m_Season );
            writer.Write( (byte)m_MapPatches.Length );
            for( int j = 0; j < m_MapPatches.Length; j++ )
            {
                writer.Write( m_MapPatches[ j ] );
            }
        }

        internal void SendMessage( LocString loc )
        {
            this.SendMessage( MsgLevel.Debug, Language.GetString( loc ) );
        }

        internal void SendMessage( string text )
        {
            SendMessage( MsgLevel.Debug, text );
        }

        internal void SendMessage( LocString loc, params object[] args )
        {
            this.SendMessage( MsgLevel.Debug, Language.Format( loc, args ) );
        }

        internal void SendMessage( MsgLevel lvl, LocString loc )
        {
            this.SendMessage( lvl, Language.GetString( loc ) );
        }

        internal void SendMessage( MsgLevel lvl, string text )
        {
            if( ( lvl >= Config.GetInt( "MessageLevel" ) ) && ( text.Length > 0 ) )
            {
                int @int;
                switch( lvl )
                {
                    case MsgLevel.Warning:
                    case MsgLevel.Error:
                        @int = Config.GetInt( "WarningColor" );
                        break;

                    default:
                        @int = Config.GetInt( "SysColor" );
                        break;
                }
                ClientCommunication.SendToClient( new UnicodeMessage( -1, -1, MessageType.Regular, @int, 3,
                                                                      Language.CliLocName, "System", text ) );
                PacketHandlers.SysMessages.Add( text.ToLower() );
                if( PacketHandlers.SysMessages.Count >= 0x19 )
                {
                    PacketHandlers.SysMessages.RemoveRange( 0, 10 );
                }
            }
        }

        internal void SendMessage( string format, params object[] args )
        {
            SendMessage( MsgLevel.Debug, string.Format( format, args ) );
        }

        internal void SendMessage( MsgLevel lvl, LocString loc, params object[] args )
        {
            this.SendMessage( lvl, Language.Format( loc, args ) );
        }

        internal void SendMessage( MsgLevel lvl, string format, params object[] args )
        {
            SendMessage( lvl, string.Format( format, args ) );
        }

        #region Nested type: CriminalTimer

        private class CriminalTimer : Timer
        {
            private PlayerData m_Player;

            public CriminalTimer( PlayerData player )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                m_Player = player;
            }

            protected override void OnTick()
            {
                ClientCommunication.RequestTitlebarUpdate();
            }
        }

        #endregion

        #region Nested type: MoveEntry

        public class MoveEntry
        {
            public Direction Dir;
            public bool FilterAck;
            public bool IsStep;
            public Point3D Position;
        }

        #endregion
    }
}
*/