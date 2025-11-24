using System;

using Server.Gumps;

/*
** SimpleNote
** updated 1/3/04
** ArteGordon
** adds a simple item that displays text messages in a scroll gump.  The size can be varied and the note text and text-color can be specified.
** The title of the note and its color can also be set.
*/

namespace Server.Items
{
    public class SimpleNote : Item
    {
        private int m_Size = 1;

        [Constructable]
        public SimpleNote()
            : base( 0x14EE )
        {
            TitleColor = 0xef0000;
            TextColor = 0x3e8;
            TitleString = "A note";
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

        [CommandProperty( AccessLevel.GameMaster )]
        public int TextColor { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TitleColor { get; set; }

        #region serialization
        public SimpleNote( Serial serial )
            : base( serial )
        {
            TitleColor = 0xef0000;
            TextColor = 0x3e8;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version 

            writer.Write( NoteString );
            writer.Write( TitleString );
            writer.Write( TextColor );
            writer.Write( TitleColor );
            writer.Write( m_Size );
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
                        TextColor = reader.ReadInt();
                        TitleColor = reader.ReadInt();
                        m_Size = reader.ReadInt();
                    }
                    break;
            }
        }
        #endregion

        public override void OnDoubleClick( Mobile from )
        {
            from.SendGump( new SimpleNoteGump( this ) );
        }

        private class SimpleNoteGump : Gump
        {
            public SimpleNoteGump( SimpleNote note )
                : base( 0, 0 )
            {
                AddPage( 0 );
                AddAlphaRegion( 40, 41, 225, 70 * note.Size );

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
                AddHtml( 55, 10, 200, 37, HtmlFormat( note.TitleString, note.TitleColor ), false, false );

                // text string
                AddHtml( 40, 41, 225, 70 * note.Size, HtmlFormat( note.NoteString, note.TextColor ), false, false );
            }

            private static string HtmlFormat( string text, int color )
            {
                return String.Format( "<BASEFONT COLOR=#{0}>{1}</BASEFONT>", color, text );
            }
        }
    }
}