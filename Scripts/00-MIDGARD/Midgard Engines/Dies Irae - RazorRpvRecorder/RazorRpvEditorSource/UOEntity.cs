/*
using System.Collections;
using System.IO;
using Server;
using Server.Network;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class UOEntity
    {
        private Hashtable m_ContextMenu;
        private bool m_Deleted;
        private ushort m_Hue;
        protected ObjectPropertyList m_ObjPropList;
        private Point3D m_Pos;
        private Serial m_Serial;

        public UOEntity( Serial ser )
        {
            m_ContextMenu = new Hashtable();
            m_ObjPropList = null;
            // this.m_ObjPropList = new ObjectPropertyList( this );
            m_Serial = ser;
            m_Deleted = false;
        }

        public UOEntity( BinaryReader reader, int version )
        {
            m_ContextMenu = new Hashtable();
            m_ObjPropList = null;

            // this.m_Serial = (Serial)reader.ReadUInt32();
            reader.ReadUInt32();

            m_Pos = new Point3D( reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32() );
            m_Hue = reader.ReadUInt16();
            m_Deleted = false;
            // this.m_ObjPropList = new ObjectPropertyList( this );
        }

        public Hashtable ContextMenu
        {
            get { return m_ContextMenu; }
        }

        public bool Deleted
        {
            get { return m_Deleted; }
        }

        public virtual ushort Hue
        {
            get { return m_Hue; }
            set { m_Hue = value; }
        }

        //public bool ModifiedOPL
        //{
        //    get { return m_ObjPropList.Customized; }
        //}

        public ObjectPropertyList ObjPropList
        {
            get { return m_ObjPropList; }
        }

        //public int OPLHash
        //{
        //    get
        //    {
        //        if( m_ObjPropList != null )
        //        {
        //            return m_ObjPropList.Hash;
        //        }
        //        return 0;
        //    }
        //    set
        //    {
        //        if( m_ObjPropList != null )
        //        {
        //            m_ObjPropList.Hash = value;
        //        }
        //    }
        //}

        public virtual Point3D Position
        {
            get { return m_Pos; }
            set
            {
                if( value != m_Pos )
                {
                    OnPositionChanging( value );
                    m_Pos = value;
                }
            }
        }

        public Serial Serial
        {
            get { return m_Serial; }
        }

        public virtual void AfterLoad()
        {
        }

        public override int GetHashCode()
        {
            return m_Serial.GetHashCode();
        }

        public virtual void OnPositionChanging( Point3D newPos )
        {
        }

        public void OPLChanged()
        {
            // ClientCommunication.SendToClient( new OPLInfo( this.Serial, this.OPLHash ) );
        }

        //public void ReadPropertyList( PacketReader p )
        //{
        //    m_ObjPropList.Read( p );
        //}

        public virtual void Remove()
        {
            m_Deleted = true;
        }

        public virtual void SaveState( BinaryWriter writer )
        {
            writer.Write( (uint)m_Serial );
            writer.Write( m_Pos.X );
            writer.Write( m_Pos.Y );
            writer.Write( m_Pos.Z );
            writer.Write( m_Hue );
        }
    }
}
*/