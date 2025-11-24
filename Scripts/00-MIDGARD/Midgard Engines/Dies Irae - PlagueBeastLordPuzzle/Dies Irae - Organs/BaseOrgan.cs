using Server;
using Server.Items;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public enum BrainHues
    {
        None = 0x0001,
        Green = 0x0047,
        Blue = 0x0060,
        Yellow = 0x0035,
        Purple = 0x0010
    }

    public enum BrainTypes
    {
        None = 0,
        Green,
        Blue,
        Yellow,
        Purple
    }

    public enum OrganTypes
    {
        Maiden = 1,
        Veins = 2,
        Polyp = 3,
        Squid = 4,
        Central = 5
    }

    public abstract class BaseOrgan : Item
    {
        #region fields
        private bool m_IsOpened;
        private int m_LabelNumber;
        #endregion

        #region properties

        [CommandProperty( AccessLevel.Developer )]
        public Item Brain { get; set; }

        [CommandProperty( AccessLevel.Developer )]
        public BrainTypes BrainType { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsOpened
        {
            get { return m_IsOpened; }
            set
            {
                bool oldValue = m_IsOpened;
                if( m_IsOpened != value )
                {
                    m_IsOpened = value;
                    OnIsOpenedChanged( oldValue );
                }
            }
        }

        public override int LabelNumber { get { return m_LabelNumber; } }
        public override bool DisplayWeight { get { return false; } }
        #endregion

        #region constructors
        public BaseOrgan( int itemID, object name, int hue, BrainTypes type )
            : base( itemID )
        {
            if( name is int )
                m_LabelNumber = (int)name;
            else if( name is string )
                Name = (string)name;

            Hue = hue;

            BrainType = type;
            m_IsOpened = false;

            Movable = false;
            Weight = 1;
        }
        #endregion

        #region members
        public abstract void CreateTissue();

        public abstract void OpenOrganTo( Mobile from );

        public virtual void OnIsOpenedChanged( bool oldValue )
        {
            PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
            if( pbl != null && pbl.IsInDebugMode )
                pbl.Say( "OnIsOpenedChanged oldValue: {0}", oldValue.ToString() );

            if( !oldValue )
            {
                if( Brain != null && BrainType != BrainTypes.None )
                    Brain.Visible = true;
            }
        }

        public static int GetBrainHueFromType( BrainTypes type )
        {
            return m_BrainHues[ (int)type ];
        }

        private static int[] m_BrainHues = new int[] { 0x01, 0x47, 0x60, 0x35, 0x10 };

        public void PutGuts( Item guts, int offSetX, int offSetY )
        {
            Container c = ( Parent as Container );

            if( c != null )
                c.AddItem( guts );
            guts.Location = new Point3D( X + offSetX, Y + offSetY, 0 );
        }

        public override bool IsAccessibleTo( Mobile check )
        {
            return true;
        }

        #region messages helper
        public new void SendLocalizedMessageTo( Mobile to, int number )
        {
            SendLocalizedMessageTo( this, to, number );
        }

        public void SendLocalizedMessageTo( Item from, Mobile to, int number )
        {
            SendLocalizedMessageTo( from, to, number, 0x66B );
        }

        public static void SendLocalizedMessageTo( Item from, Mobile to, int number, int hue )
        {
            MessageHelper.SendLocalizedMessageTo( from, to, number, "", hue );
        }
        #endregion
        #endregion

        #region serial-deserial
        public BaseOrgan( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_IsOpened );
            writer.Write( (int)BrainType );
            writer.Write( Brain );
            writer.Write( m_LabelNumber );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_IsOpened = reader.ReadBool();
            BrainType = (BrainTypes)reader.ReadInt();
            Brain = reader.ReadItem();
            m_LabelNumber = reader.ReadInt();

            if( Brain == null )
                Delete();
        }
        #endregion
    }
}