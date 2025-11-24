using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BountyBoardMessage : Item
    {
        private Mobile m_Poster;
        private string m_Subject;
        private DateTime m_Time, m_LastPostTime;
        private BountyBoardMessage m_Thread;
        private string m_PostedName;
        private int m_PostedBody;
        private int m_PostedHue;
        private BountyBoardEquip[] m_PostedEquip;
        private string[] m_Lines;

        public string GetTimeAsString()
        {
            return m_Time.ToString( "MMM dd, yyyy" );
        }

        public override bool CheckTarget( Mobile from, Targeting.Target targ, object targeted )
        {
            return false;
        }

        public override bool IsAccessibleTo( Mobile check )
        {
            return false;
        }

        public BountyBoardMessage( Mobile poster, BountyBoardMessage thread, string subject, string[] lines )
            : base( 0xEB0 )
        {
            Movable = false;

            m_Poster = poster;
            m_Subject = subject;
            m_Time = DateTime.Now;
            m_LastPostTime = m_Time;
            m_Thread = thread;
            m_PostedName = m_Poster.Name;
            m_PostedBody = m_Poster.Body;
            m_PostedHue = m_Poster.Hue;
            m_Lines = lines;

            List<BountyBoardEquip> list = new List<BountyBoardEquip>();

            foreach( Item item in poster.Items )
            {
                if( item.Layer >= Layer.OneHanded && item.Layer <= Layer.Mount )
                    list.Add( new BountyBoardEquip( item.ItemID, item.Hue ) );
            }

            m_PostedEquip = list.ToArray();
        }

        public Mobile Poster { get { return m_Poster; } }
        public BountyBoardMessage Thread { get { return m_Thread; } }
        public string Subject { get { return m_Subject; } }
        public DateTime Time { get { return m_Time; } }
        public DateTime LastPostTime { get { return m_LastPostTime; } set { m_LastPostTime = value; } }
        public string PostedName { get { return m_PostedName; } }
        public int PostedBody { get { return m_PostedBody; } }
        public int PostedHue { get { return m_PostedHue; } }
        public BountyBoardEquip[] PostedEquip { get { return m_PostedEquip; } }
        public string[] Lines { get { return m_Lines; } }

        #region serialization
        public BountyBoardMessage( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Poster );
            writer.Write( m_Subject );
            writer.Write( m_Time );
            writer.Write( m_LastPostTime );
            writer.Write( m_Thread != null );
            writer.Write( m_Thread );
            writer.Write( m_PostedName );
            writer.Write( m_PostedBody );
            writer.Write( m_PostedHue );

            writer.Write( m_PostedEquip.Length );

            for( int i = 0; i < m_PostedEquip.Length; ++i )
            {
                writer.Write( m_PostedEquip[ i ].ItemID );
                writer.Write( m_PostedEquip[ i ].Hue );
            }

            writer.Write( m_Lines.Length );

            foreach( string t in m_Lines )
                writer.Write( t );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Poster = reader.ReadMobile();
                        m_Subject = reader.ReadString();
                        m_Time = reader.ReadDateTime();
                        m_LastPostTime = reader.ReadDateTime();
                        bool hasThread = reader.ReadBool();
                        m_Thread = reader.ReadItem() as BountyBoardMessage;
                        m_PostedName = reader.ReadString();
                        m_PostedBody = reader.ReadInt();
                        m_PostedHue = reader.ReadInt();

                        m_PostedEquip = new BountyBoardEquip[ reader.ReadInt() ];

                        for( int i = 0; i < m_PostedEquip.Length; ++i )
                        {
                            m_PostedEquip[ i ].ItemID = reader.ReadInt();
                            m_PostedEquip[ i ].Hue = reader.ReadInt();
                        }

                        m_Lines = new string[ reader.ReadInt() ];

                        for( int i = 0; i < m_Lines.Length; ++i )
                            m_Lines[ i ] = reader.ReadString();

                        if( hasThread && m_Thread == null )
                            Delete();

                        break;
                    }
            }
        }
        #endregion
    }
}