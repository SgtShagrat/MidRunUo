/***************************************************************************
 *                               XmlHintAttach.cs
 *                            -------------------
 *   begin                : 10 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlHintAttach : XmlAttachment
    {
        public enum MessageType
        {
            ScrollMessage,
            SimpleMessage
        }

        private string m_Hint = string.Empty;
        private bool m_Once;
        private MessageType m_Type;
        private bool m_Identified;

        [Attachable]
        public XmlHintAttach( string hint )
            : this( hint, false, MessageType.ScrollMessage )
        {
        }

        [Attachable]
        public XmlHintAttach( string hint, bool once )
            : this( hint, once, MessageType.ScrollMessage )
        {
        }

        [Attachable]
        public XmlHintAttach( string hint, bool once, MessageType type )
        {
            Name = "Xml Hint Attach";

            m_Hint = hint;
            m_Once = once;
            m_Type = type;

            m_Identified = false;
        }

        public override bool BlockDefaultOnUse( Mobile from, object target )
        {
            return !m_Identified;
        }

        public override void OnUse( Mobile from )
        {
            if( !m_Identified )
                from.SendMessage( "You must identify this item before using it." );

            base.OnUse( from );
        }

        public override string OnIdentify( Mobile from )
        {
            m_Identified = true;

            DisplayHint( from );

            if( m_Once && !Deleted )
                Delete();

            return null;
        }

        public virtual void DisplayHint( Mobile from )
        {
            if( m_Type == MessageType.ScrollMessage )
            {
                if( from is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)from ).SendCustomScrollMessage( m_Hint );
            }
            else
                from.SendMessage( m_Hint );
        }

        #region serialization
        public XmlHintAttach( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( m_Hint );
            writer.Write( m_Once );
            writer.Write( (int)m_Type );
            writer.Write( m_Identified );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt();

            m_Hint = reader.ReadString();
            m_Once = reader.ReadBool();
            m_Type = (MessageType)reader.ReadInt();
            m_Identified = reader.ReadBool();
        }
        #endregion
    }

    public class XmlEffectAttach : XmlAttachment
    {
        public enum MessageType
        {
            ScrollMessage,
            SimpleMessage
        }

        private string m_Hint = string.Empty;
        private bool m_Once;
        private MessageType m_Type;
        private bool m_Identified;

        [Attachable]
        public XmlEffectAttach( string hint )
            : this( hint, false, MessageType.ScrollMessage )
        {
        }

        [Attachable]
        public XmlEffectAttach( string hint, bool once )
            : this( hint, once, MessageType.ScrollMessage )
        {
        }

        [Attachable]
        public XmlEffectAttach( string hint, bool once, MessageType type )
        {
            Name = "Xml Hint Attach";

            m_Hint = hint;
            m_Once = once;
            m_Type = type;

            m_Identified = false;
        }

        public override bool BlockDefaultOnUse( Mobile from, object target )
        {
            return !m_Identified;
        }

        public override void OnUse( Mobile from )
        {
            if( !m_Identified )
                from.SendMessage( "You must identify this item before using it." );

            base.OnUse( from );
        }

        public override string OnIdentify( Mobile from )
        {
            m_Identified = true;

            DisplayHint( from );

            if( m_Once && !Deleted )
                Delete();

            return null;
        }

        public virtual void DisplayHint( Mobile from )
        {
            if( m_Type == MessageType.ScrollMessage )
            {
                if( from is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)from ).SendCustomScrollMessage( m_Hint );
            }
            else
                from.SendMessage( m_Hint );
        }

        #region serialization
        public XmlEffectAttach( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( m_Hint );
            writer.Write( m_Once );
            writer.Write( (int)m_Type );
            writer.Write( m_Identified );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            reader.ReadInt();

            m_Hint = reader.ReadString();
            m_Once = reader.ReadBool();
            m_Type = (MessageType)reader.ReadInt();
            m_Identified = reader.ReadBool();
        }
        #endregion
    }
}