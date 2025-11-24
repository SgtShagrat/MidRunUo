using System;

using Server;
using Server.Gumps;

namespace Midgard.Items
{
    public class NoticeScroll : Item
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler( OnLogin );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            if( e.Mobile == null || !e.Mobile.Player )
                return;

            int numOfScrolls = 0;

            foreach( Item item in e.Mobile.Backpack.Items )
            {
                if( item is NoticeScroll )
                    Timer.DelayCall( TimeSpan.FromSeconds( 3.0 * numOfScrolls ), new TimerStateCallback( Notify ), new object[] { e.Mobile, item as NoticeScroll, numOfScrolls++ } );
            }
        }

        private int m_Size = 1;

        [Constructable]
        public NoticeScroll( string message, string title )
            : base( 0x14EE )
        {
            NoteString = message;
            TitleString = title;
        }

        public override string DefaultName
        {
            get { return "A note"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string NoteString { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string TitleString { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Size
        {
            get { return m_Size; }
            set
            {
                m_Size = value;
                if( m_Size < 1 )
                    m_Size = 1;
            }
        }

        #region serialization
        public NoticeScroll( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version 

            writer.Write( NoteString );
            writer.Write( TitleString );
            writer.Write( Size );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            switch( version )
            {
                case 0:
                    {
                        NoteString = reader.ReadString();
                        TitleString = reader.ReadString();
                        Size = reader.ReadInt();
                    }
                    break;
            }
        }
        #endregion

        public static void Notify( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            NoticeScroll scroll = (NoticeScroll)states[ 1 ];
            int scrolls = (int)states[ 2 ];

            from.SendGump( new SimpleNoteGump( scroll, scrolls * 50, scrolls * 50 ) );
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.SendGump( new SimpleNoteGump( this ) );
        }

        private class SimpleNoteGump : Gump
        {
            public SimpleNoteGump( NoticeScroll note, int offsetX, int offsetY )
                : base( offsetX, offsetY )
            {
                AddPage( 0 );

                // scroll top
                AddImageTiled( 3, 5, 300, 37, 0x820 );

                // scroll middle, upper portion
                AddImageTiled( 19, 41, 263, 70, 0x821 );

                for( int i = 1; i < note.Size; i++ )
                {
                    // scroll middle , lower portion
                    AddImageTiled( 19, 41 + 70 * i, 263, 70, 0x822 );
                }

                // scroll bottom
                AddImageTiled( 20, 111 + 70 * ( note.Size - 1 ), 273, 34, 0x823 );

                // title string
                AddFirstCharHuedHtml( 55, 10, 200, 37, note.TitleString, Colors.Indigo, true );

                // text string
                AddOldHtml( 40, 41, 225, 70 * note.Size, note.NoteString );
            }

            public SimpleNoteGump( NoticeScroll note )
                : this( note, 50, 50 )
            {
            }
        }
    }
}