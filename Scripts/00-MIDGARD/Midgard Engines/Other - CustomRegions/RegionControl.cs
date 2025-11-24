using System;
using System.Collections;
using System.Collections.Generic;
using Midgard.Engines.Gumps;
using Midgard.Regions;
using Server.Spells;

namespace Server.Items
{
    public class RegionControl : Item
    {
        public static List<RegionControl> AllControls { get; private set; }

        #region Region Restrictions

        public BitArray RestrictedSpells { get; private set; }

        public BitArray RestrictedSkills { get; private set; }

        #endregion

        public CustomRegion Region { get; private set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Rectangle3D[] RegionArea { get; set; }

        private bool m_Active = true;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if( m_Active != value )
                {
                    m_Active = value;
                    UpdateRegion();
                }
            }
        }

        #region Region Properties

        private string m_RegionName;
        private int m_RegionPriority;
        private MusicName m_Music;
        private TimeSpan m_PlayerLogoutDelay;
        private int m_LightLevel;

        private int m_MinZ;
        private int m_MaxZ;

        private Map m_MoveNPCToMap;
        private Point3D m_MoveNPCToLoc;
        private Map m_MovePlayerToMap;
        private Point3D m_MovePlayerToLoc;

        [CommandProperty( AccessLevel.GameMaster )]
        public string RegionName
        {
            get { return m_RegionName; }
            set
            {
                if( Map != null && !RegionNameTaken( value ) )
                    m_RegionName = value;
                else if( Map != null )
                    Console.WriteLine( "RegionName not changed for {0}, {1} already has a Region with the name of {2}", this, Map, value );
                else if( Map == null )
                    Console.WriteLine( "RegionName not changed for {0} to {1}, it's Map value was null", this, value );

                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int RegionPriority
        {
            get { return m_RegionPriority; }
            set
            {
                m_RegionPriority = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MinZ
        {
            get { return m_MinZ; }
            set
            {
                m_MinZ = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxZ
        {
            get { return m_MaxZ; }
            set
            {
                m_MaxZ = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public MusicName Music
        {
            get { return m_Music; }
            set
            {
                m_Music = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan PlayerLogoutDelay
        {
            get { return m_PlayerLogoutDelay; }
            set
            {
                m_PlayerLogoutDelay = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int LightLevel
        {
            get { return m_LightLevel; }
            set
            {
                m_LightLevel = value;
                UpdateRegion();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Map MoveNPCToMap
        {
            get { return m_MoveNPCToMap; }
            set
            {
                if( value != Map.Internal )
                    m_MoveNPCToMap = value;
                else
                    SetFlag( RegionFlag.MoveNPCOnDeath, false );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Point3D MoveNPCToLoc
        {
            get { return m_MoveNPCToLoc; }
            set
            {
                if( value != Point3D.Zero )
                    m_MoveNPCToLoc = value;
                else
                    SetFlag( RegionFlag.MoveNPCOnDeath, false );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Map MovePlayerToMap
        {
            get { return m_MovePlayerToMap; }
            set
            {
                if( value != Map.Internal )
                    m_MovePlayerToMap = value;
                else
                    SetFlag( RegionFlag.MovePlayerOnDeath, false );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Point3D MovePlayerToLoc
        {
            get { return m_MovePlayerToLoc; }
            set
            {
                if( value != Point3D.Zero )
                    m_MovePlayerToLoc = value;
                else
                    SetFlag( RegionFlag.MovePlayerOnDeath, false );
            }
        }

        #endregion

        #region Region Flags properties

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowBenefitPlayer
        {
            get { return GetFlag( RegionFlag.AllowBenefitPlayer ); }
            set { SetFlag( RegionFlag.AllowBenefitPlayer, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowHarmPlayer
        {
            get { return GetFlag( RegionFlag.AllowHarmPlayer ); }
            set { SetFlag( RegionFlag.AllowHarmPlayer, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowHousing
        {
            get { return GetFlag( RegionFlag.AllowHousing ); }
            set { SetFlag( RegionFlag.AllowHousing, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowSpawn
        {
            get { return GetFlag( RegionFlag.AllowSpawn ); }
            set { SetFlag( RegionFlag.AllowSpawn, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanBeDamaged
        {
            get { return GetFlag( RegionFlag.CanBeDamaged ); }
            set { SetFlag( RegionFlag.CanBeDamaged, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanMountEthereal
        {
            get { return GetFlag( RegionFlag.CanMountEthereal ); }
            set { SetFlag( RegionFlag.CanMountEthereal, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanEnter
        {
            get { return GetFlag( RegionFlag.CanEnter ); }
            set { SetFlag( RegionFlag.CanEnter, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanHeal
        {
            get { return GetFlag( RegionFlag.CanHeal ); }
            set { SetFlag( RegionFlag.CanHeal, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanRessurect
        {
            get { return GetFlag( RegionFlag.CanRessurect ); }
            set { SetFlag( RegionFlag.CanRessurect, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanUseStuckMenu
        {
            get { return GetFlag( RegionFlag.CanUseStuckMenu ); }
            set { SetFlag( RegionFlag.CanUseStuckMenu, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ItemDecay
        {
            get { return GetFlag( RegionFlag.ItemDecay ); }
            set { SetFlag( RegionFlag.ItemDecay, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowBenefitNPC
        {
            get { return GetFlag( RegionFlag.AllowBenefitNPC ); }
            set { SetFlag( RegionFlag.AllowBenefitNPC, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowHarmNPC
        {
            get { return GetFlag( RegionFlag.AllowHarmNPC ); }
            set { SetFlag( RegionFlag.AllowHarmNPC, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ShowEnterMessage
        {
            get { return GetFlag( RegionFlag.ShowEnterMessage ); }
            set { SetFlag( RegionFlag.ShowEnterMessage, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ShowExitMessage
        {
            get { return GetFlag( RegionFlag.ShowExitMessage ); }
            set { SetFlag( RegionFlag.ShowExitMessage, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanLootPlayerCorpse
        {
            get { return GetFlag( RegionFlag.CanLootPlayerCorpse ); }
            set { SetFlag( RegionFlag.CanLootPlayerCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanLootNPCCorpse
        {
            get { return GetFlag( RegionFlag.CanLootNPCCorpse ); }
            set { SetFlag( RegionFlag.CanLootNPCCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanLootOwnCorpse
        {
            get { return GetFlag( RegionFlag.CanLootOwnCorpse ); }
            set { SetFlag( RegionFlag.CanLootOwnCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CanUsePotions
        {
            get { return GetFlag( RegionFlag.CanUsePotions ); }
            set { SetFlag( RegionFlag.CanUsePotions, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsGuarded
        {
            get { return GetFlag( RegionFlag.IsGuarded ); }
            set
            {
                SetFlag( RegionFlag.IsGuarded, value );
                if( Region != null )
                    Region.Disabled = !value;

                Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerCallback( UpdateRegion ) );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool NoPlayerItemDrop
        {
            get { return GetFlag( RegionFlag.NoPlayerItemDrop ); }
            set { SetFlag( RegionFlag.NoPlayerItemDrop, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool NoNPCItemDrop
        {
            get { return GetFlag( RegionFlag.NoNPCItemDrop ); }
            set { SetFlag( RegionFlag.NoNPCItemDrop, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool EmptyNPCCorpse
        {
            get { return GetFlag( RegionFlag.EmptyNPCCorpse ); }
            set { SetFlag( RegionFlag.EmptyNPCCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool EmptyPlayerCorpse
        {
            get { return GetFlag( RegionFlag.EmptyPlayerCorpse ); }
            set { SetFlag( RegionFlag.EmptyPlayerCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DeleteNPCCorpse
        {
            get { return GetFlag( RegionFlag.DeleteNPCCorpse ); }
            set { SetFlag( RegionFlag.DeleteNPCCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DeletePlayerCorpse
        {
            get { return GetFlag( RegionFlag.DeletePlayerCorpse ); }
            set { SetFlag( RegionFlag.DeletePlayerCorpse, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ResNPCOnDeath
        {
            get { return GetFlag( RegionFlag.ResNPCOnDeath ); }
            set { SetFlag( RegionFlag.ResNPCOnDeath, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ResPlayerOnDeath
        {
            get { return GetFlag( RegionFlag.ResPlayerOnDeath ); }
            set { SetFlag( RegionFlag.ResPlayerOnDeath, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool MoveNPCOnDeath
        {
            get { return GetFlag( RegionFlag.MoveNPCOnDeath ); }
            set
            {
                if( MoveNPCToMap == null || MoveNPCToMap == Map.Internal || MoveNPCToLoc == Point3D.Zero )
                    SetFlag( RegionFlag.MoveNPCOnDeath, false );
                else
                    SetFlag( RegionFlag.MoveNPCOnDeath, value );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool MovePlayerOnDeath
        {
            get { return GetFlag( RegionFlag.MovePlayerOnDeath ); }
            set
            {
                if( MovePlayerToMap == null || MovePlayerToMap == Map.Internal || MovePlayerToLoc == Point3D.Zero )
                    SetFlag( RegionFlag.MovePlayerOnDeath, false );
                else
                    SetFlag( RegionFlag.MovePlayerOnDeath, value );
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CannotMount
        {
            get { return GetFlag( RegionFlag.CannotMount ); }
            set { SetFlag( RegionFlag.CannotMount, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CannotRun
        {
            get { return GetFlag( RegionFlag.CannotRun ); }
            set { SetFlag( RegionFlag.CannotRun, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CheckCombat
        {
            get { return GetFlag( RegionFlag.CheckCombat ); }
            set { SetFlag( RegionFlag.CheckCombat, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool TravelRestricted
        {
            get { return GetFlag( RegionFlag.TravelRestricted ); }
            set { SetFlag( RegionFlag.TravelRestricted, value ); }
        }

        #endregion

        #region constructors

        [Constructable]
        public RegionControl()
            : base( 5609 )
        {
            Visible = false;
            Movable = false;
            Name = "Region Controller";

            if( AllControls == null )
                AllControls = new List<RegionControl>();

            AllControls.Add( this );

            m_RegionName = FindNewName( "Custom Region" );
            m_RegionPriority = Server.Region.DefaultPriority;

            m_MinZ = Server.Region.MinZ;
            m_MaxZ = Server.Region.MaxZ;

            RestrictedSpells = new BitArray( SpellRegistry.Types.Length );
            RestrictedSkills = new BitArray( SkillInfo.Table.Length );
        }

        [Constructable]
        public RegionControl( Rectangle2D rect )
            : this()
        {
            Rectangle3D newrect = Server.Region.ConvertTo3D( rect );
            DoChooseArea( null, Map, newrect.Start, newrect.End, this );

            UpdateRegion();
        }

        /*
        [Constructable]
        public RegionControl( Rectangle3D rect )
            : this()
        {
            DoChooseArea( null, Map, rect.Start, rect.End, this );

            UpdateRegion();
        }

        [Constructable]
        public RegionControl( IEnumerable<Rectangle2D> rects )
            : this()
        {
            foreach( Rectangle2D rect2D in rects )
            {
                Rectangle3D newrect = Server.Region.ConvertTo3D( rect2D );
                DoChooseArea( null, Map, newrect.Start, newrect.End, this );
            }

            UpdateRegion();
        }

        [Constructable]
        public RegionControl( IEnumerable<Rectangle3D> rects )
            : this()
        {
            foreach( Rectangle3D rect3D in rects )
            {
                DoChooseArea( null, Map, rect3D.Start, rect3D.End, this );
            }

            UpdateRegion();
        }
        */

        public RegionControl( Serial serial )
            : base( serial )
        {
        }

        #endregion

        #region Control Special Voids

        public bool RegionNameTaken( string testName )
        {
            if( AllControls != null )
            {
                foreach( RegionControl control in AllControls )
                {
                    if( control.RegionName == testName && control != this )
                        return true;
                }
            }
            return false;
        }

        public string FindNewName( string oldName )
        {
            int i = 1;

            string newName = oldName;
            while( RegionNameTaken( newName ) )
            {
                newName = oldName;
                newName += String.Format( " {0}", i );
                i++;
            }

            return newName;
        }

        public void UpdateRegion()
        {
            if( Region != null )
                Region.Unregister();

            if( Map != null && Active )
            {
                if( RegionArea != null && RegionArea.Length > 0 )
                {
                    Region = new CustomRegion( this );
                    Region.Register();
                    Region.GoLocation = Location;
                }
                else
                    Region = null;
            }
            else
                Region = null;
        }

        public void RemoveArea( int index, Mobile from )
        {
            try
            {
                List<Rectangle3D> rects = new List<Rectangle3D>();
                foreach( Rectangle3D rect in RegionArea )
                    rects.Add( rect );

                rects.RemoveAt( index );
                RegionArea = rects.ToArray();

                UpdateRegion();
                from.SendMessage( "Area Removed!" );
            }
            catch
            {
                from.SendMessage( "Removing of Area Failed!" );
            }
        }

        public static int GetRegistryNumber( ISpell s )
        {
            Type[] t = SpellRegistry.Types;

            for( int i = 0; i < t.Length; i++ )
            {
                if( s.GetType() == t[ i ] )
                    return i;
            }

            return -1;
        }

        public bool IsRestrictedSpell( ISpell s )
        {
            if( RestrictedSpells.Length != SpellRegistry.Types.Length )
            {
                RestrictedSpells = new BitArray( SpellRegistry.Types.Length );

                for( int i = 0; i < RestrictedSpells.Length; i++ )
                    RestrictedSpells[ i ] = false;
            }

            int regNum = GetRegistryNumber( s );

            if( regNum < 0 ) //Happens with unregistered Spells
                return false;

            return RestrictedSpells[ regNum ];
        }

        public bool IsRestrictedSkill( int skill )
        {
            if( RestrictedSkills.Length != SkillInfo.Table.Length )
            {
                RestrictedSkills = new BitArray( SkillInfo.Table.Length );

                for( int i = 0; i < RestrictedSkills.Length; i++ )
                    RestrictedSkills[ i ] = false;
            }

            if( skill < 0 )
                return false;

            return RestrictedSkills[ skill ];
        }

        public void ChooseArea( Mobile m )
        {
            BoundingBoxPicker.Begin( m, new BoundingBoxCallback( CustomRegion_Callback ), this );
        }

        public void CustomRegion_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
        {
            DoChooseArea( from, map, start, end, state );
        }

        public void DoChooseArea( Mobile from, Map map, Point3D start, Point3D end, object control )
        {
            List<Rectangle3D> areas = new List<Rectangle3D>();

            if( RegionArea != null )
            {
                foreach( Rectangle3D rect in RegionArea )
                    areas.Add( rect );
            }

            if( start.Z == end.Z || start.Z < end.Z )
            {
                if( start.Z != Server.Region.MinZ )
                    --start.Z;
                if( end.Z != Server.Region.MaxZ )
                    ++end.Z;
            }
            else
            {
                if( start.Z != Server.Region.MaxZ )
                    ++start.Z;
                if( end.Z != Server.Region.MinZ )
                    --end.Z;
            }

            if( MinZ != Server.Region.MinZ )
                start.Z = MinZ;

            if( MaxZ != Server.Region.MaxZ )
                end.Z = MaxZ;

            Rectangle3D newrect = new Rectangle3D( start, end );
            areas.Add( newrect );

            RegionArea = areas.ToArray();

            UpdateRegion();
        }

        #endregion

        #region Control Overrides

        public override void OnDoubleClick( Mobile m )
        {
            if( m.AccessLevel >= AccessLevel.GameMaster )
            {
                if( RestrictedSpells.Length != SpellRegistry.Types.Length )
                {
                    RestrictedSpells = new BitArray( SpellRegistry.Types.Length );

                    for( int i = 0; i < RestrictedSpells.Length; i++ )
                        RestrictedSpells[ i ] = false;

                    m.SendMessage( "Resetting all restricted Spells due to Spell change" );
                }

                if( RestrictedSkills.Length != SkillInfo.Table.Length )
                {
                    RestrictedSkills = new BitArray( SkillInfo.Table.Length );

                    for( int i = 0; i < RestrictedSkills.Length; i++ )
                        RestrictedSkills[ i ] = false;

                    m.SendMessage( "Resetting all restricted Skills due to Skill change" );
                }

                m.CloseGump( typeof( RegionControlGump ) );
                m.SendGump( new RegionControlGump( this ) );
                m.SendMessage( "Don't forget to props this object for more options!" );
                m.CloseGump( typeof( RemoveAreaGump ) );
                m.SendGump( new RemoveAreaGump( this ) );
            }
        }

        public override void OnMapChange()
        {
            UpdateRegion();
            base.OnMapChange();
        }

        public override void OnDelete()
        {
            if( Region != null )
                Region.Unregister();

            if( AllControls != null )
                AllControls.Remove( this );

            base.OnDelete();
        }

        #endregion

        #region Serialization

        #region Region Flags

        [Flags]
        private enum RegionFlag : ulong
        {
            None = 0x000000000,

            AllowBenefitPlayer = 0x000000001,
            AllowHarmPlayer = 0x000000002,
            AllowHousing = 0x000000004,
            AllowSpawn = 0x000000008,
            CanBeDamaged = 0x000000010,
            CanHeal = 0x000000020,
            CanRessurect = 0x000000040,
            CanUseStuckMenu = 0x000000080,
            ItemDecay = 0x000000100,
            ShowEnterMessage = 0x000000200,
            ShowExitMessage = 0x000000400,
            AllowBenefitNPC = 0x000000800,
            AllowHarmNPC = 0x000001000,
            CanMountEthereal = 0x000002000,
            CanEnter = 0x000004000,
            CanLootPlayerCorpse = 0x000008000,
            CanLootNPCCorpse = 0x000010000,
            CanLootOwnCorpse = 0x000020000,
            CanUsePotions = 0x000040000,
            IsGuarded = 0x000080000,
            NoPlayerItemDrop = 0x000100000,
            NoNPCItemDrop = 0x000200000,
            EmptyNPCCorpse = 0x000400000,
            EmptyPlayerCorpse = 0x000800000,
            DeleteNPCCorpse = 0x001000000,
            DeletePlayerCorpse = 0x002000000,
            ResNPCOnDeath = 0x004000000,
            ResPlayerOnDeath = 0x008000000,
            MoveNPCOnDeath = 0x010000000,
            MovePlayerOnDeath = 0x020000000,

            CannotRun = 0x040000000,
            CannotMount = 0x080000000,
            CheckCombat = 0x100000000,
            TravelRestricted = 0x200000000
        }

        private RegionFlag m_Flags;

        static RegionControl()
        {
            AllControls = new List<RegionControl>();
        }

        private bool GetFlag( RegionFlag flag )
        {
            return ( ( m_Flags & flag ) != 0 );
        }

        private void SetFlag( RegionFlag flag, bool value )
        {
            if( value )
                m_Flags |= flag;
            else
            {
                m_Flags &= ~flag;
            }
        }

        #endregion

        public static void WriteBitArray( GenericWriter writer, BitArray ba )
        {
            writer.Write( ba.Length );

            for( int i = 0; i < ba.Length; i++ )
            {
                writer.Write( ba[ i ] );
            }
            return;
        }

        public static BitArray ReadBitArray( GenericReader reader )
        {
            int size = reader.ReadInt();

            BitArray readBitArray = new BitArray( size );

            for( int i = 0; i < size; i++ )
            {
                readBitArray[ i ] = reader.ReadBool();
            }

            return readBitArray;
        }

        public static void WriteRect3DArray( GenericWriter writer, Rectangle3D[] ary )
        {
            if( ary == null )
            {
                writer.Write( 0 );
                return;
            }

            writer.Write( ary.Length );

            for( int i = 0; i < ary.Length; i++ )
            {
                Rectangle3D rect = ary[ i ];
                writer.Write( rect.Start );
                writer.Write( rect.End );
            }
            return;
        }

        public static List<Rectangle2D> ReadRect2DArray( GenericReader reader )
        {
            int size = reader.ReadInt();
            List<Rectangle2D> newAry = new List<Rectangle2D>();

            for( int i = 0; i < size; i++ )
            {
                newAry.Add( reader.ReadRect2D() );
            }

            return newAry;
        }

        public static Rectangle3D[] ReadRect3DArray( GenericReader reader )
        {
            int size = reader.ReadInt();
            List<Rectangle3D> newAry = new List<Rectangle3D>();

            for( int i = 0; i < size; i++ )
            {
                Point3D start = reader.ReadPoint3D();
                Point3D end = reader.ReadPoint3D();
                newAry.Add( new Rectangle3D( start, end ) );
            }

            return newAry.ToArray();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 7 ); // version

            writer.Write( MinZ );
            writer.Write( MaxZ );

            WriteRect3DArray( writer, RegionArea );

            writer.Write( (ulong)m_Flags );

            WriteBitArray( writer, RestrictedSpells );
            WriteBitArray( writer, RestrictedSkills );

            writer.Write( m_Active );

            writer.Write( m_RegionName );
            writer.Write( m_RegionPriority );
            writer.Write( (int)m_Music );
            writer.Write( m_PlayerLogoutDelay );
            writer.Write( m_LightLevel );

            writer.Write( m_MoveNPCToMap );
            writer.Write( m_MoveNPCToLoc );
            writer.Write( m_MovePlayerToMap );
            writer.Write( m_MovePlayerToLoc );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 7:
                    {
                        m_MinZ = reader.ReadInt();
                        m_MaxZ = reader.ReadInt();
                    }
                    goto case 6;
                case 6:
                    goto case 5;
                case 5:
                    goto case 4;
                case 4:
                    {
                        RegionArea = ReadRect3DArray( reader );

                        if( version < 5 )
                            m_Flags = (RegionFlag)reader.ReadInt();
                        else
                            m_Flags = (RegionFlag)reader.ReadULong();

                        RestrictedSpells = ReadBitArray( reader );
                        RestrictedSkills = ReadBitArray( reader );

                        m_Active = reader.ReadBool();

                        m_RegionName = reader.ReadString();
                        m_RegionPriority = reader.ReadInt();
                        m_Music = (MusicName)reader.ReadInt();
                        m_PlayerLogoutDelay = reader.ReadTimeSpan();
                        m_LightLevel = reader.ReadInt();

                        m_MoveNPCToMap = reader.ReadMap();
                        m_MoveNPCToLoc = reader.ReadPoint3D();
                        m_MovePlayerToMap = reader.ReadMap();
                        m_MovePlayerToLoc = reader.ReadPoint3D();

                        break;
                    }
            }

            AllControls.Add( this );

            if( RegionNameTaken( m_RegionName ) )
                m_RegionName = FindNewName( m_RegionName );

            UpdateRegion();
        }

        #endregion
    }
}