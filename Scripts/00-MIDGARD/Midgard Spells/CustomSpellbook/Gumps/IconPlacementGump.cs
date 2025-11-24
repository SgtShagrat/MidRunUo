using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class IconPlacementGump : Gump
    {
        public enum Buttons
        {
            Close,

            Apply,
            Move,

            XEntry,
            YEntry,
            Entry,

            Up,
            Right,
            Left,
            Down
        }

        private CustomSpellbook m_Book;
        private Mobile m_From;

        private int m_X;
        private int m_Y;
        private int m_Delta;
        private int m_Icon;
        private int m_Spell;
        private int m_Background;

        public IconPlacementGump( CustomSpellbook book, Mobile from, int x, int y, int delta, int icon, int spellID, int background )
            : base( 0, 0 )
        {
            m_Book = book;
            m_From = from;

            m_X = x;
            m_Y = y;
            m_Delta = delta;
            m_Icon = icon;
            m_Spell = spellID;
            m_Background = background;

            Closable = true;
            Disposable = true;
            Dragable = false;
            Resizable = false;

            AddPage( 0 );

            // AddImage( 260, 160, 5011 );

            if( CustomSpellbookGump.OldStyle )
                AddOldHtmlHued( 353, 230, 200, 17, "Icon Placement", Colors.White );
            else
                AddLabel( 353, 230, 1153, "Icon Placement" );

            ExtendedSpellInfo si = SpellRegistry.GetExtendedSpellInfoByID( spellID );
            if( si == null )
                return;

            if( CustomSpellbookGump.OldStyle )
                AddOldHtmlHued( 338, 350, 200, 17, si.Name, Colors.White );
            else
                AddLabel( 338, 350, 1153, si.Name );

            AddButton( 412, 288, 2444, 2443, (int)Buttons.Apply, GumpButtonType.Reply, 0 );
            AddButton( 325, 288, 2444, 2443, (int)Buttons.Move, GumpButtonType.Reply, 0 );

            if( CustomSpellbookGump.OldStyle )
            {
                AddOldHtmlHued( 425, 290, 200, 17, "Apply", Colors.White );
                AddOldHtmlHued( 339, 290, 200, 17, "Move", Colors.White );
            }
            else
            {
                AddLabel( 425, 290, 1153, "Apply" );
                AddLabel( 339, 290, 1153, "Move" );
            }

            AddButton( 377, 178, 4500, 4500, (int)Buttons.Up, GumpButtonType.Reply, 0 );
            AddButton( 474, 276, 4502, 4502, (int)Buttons.Right, GumpButtonType.Reply, 0 );
            AddButton( 377, 375, 4504, 4504, (int)Buttons.Down, GumpButtonType.Reply, 0 );
            AddButton( 278, 276, 4506, 4506, (int)Buttons.Left, GumpButtonType.Reply, 0 );

            AddBackground( 348, 260, 105, 20, 9300 );
            AddBackground( 348, 318, 105, 20, 9300 );
            AddBackground( 388, 290, 25, 20, 9300 );

            AddTextEntry( 348, 260, 105, 20, 1153, 7, "" + m_X );
            AddTextEntry( 348, 318, 105, 20, 1153, 8, "" + m_Y );
            AddTextEntry( 388, 290, 25, 20, 1153, 9, "" + m_Delta );

            AddBackground( x, y, 54, 56, background );
            AddImage( x + 45, y, 9008 );
            AddImage( x + 5, y + 5, icon );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( m_Book.Deleted )
                return;

            Mobile from = state.Mobile;
            switch( info.ButtonID )
            {
                case (int)Buttons.Apply:
                    {
                        int replaceI = 0;
                        bool replace = false;

                        int count = m_Book.DragableIcons.Count;
                        for( int i = 0; i < count; i++ )
                        {
                            IconInfo ii = m_Book.DragableIcons[ i ];
                            if( ii.Icon == m_Icon )
                            {
                                replace = true;
                                replaceI = i;
                            }
                        }
                        if( replace )
                        {
                            m_Book.DragableIcons.RemoveAt( replaceI );
                        }
                        m_Book.DragableIcons.Add( new IconInfo( m_Icon, m_Spell, m_X, m_Y, m_Background ) );

                        from.SendGump( new SpellIconGump( m_Book, m_X, m_Y, m_Icon, m_Spell, m_Background ) );
                        break;
                    }
                case (int)Buttons.Move:
                    {
                        TextRelay xrelay = info.GetTextEntry( 7 );
                        TextRelay yrelay = info.GetTextEntry( 8 );
                        string xstring = ( xrelay == null ? null : xrelay.Text.Trim() );
                        string ystring = ( yrelay == null ? null : yrelay.Text.Trim() );
                        if( string.IsNullOrEmpty( xstring ) || string.IsNullOrEmpty( ystring ) )
                        {
                            from.SendMessage( "Please enter a X coordinate in the top box and a Y coordinate in the bottom box" );
                        }
                        else
                        {
                            int x = m_X;
                            int y = m_Y;
                            try
                            {
                                x = Int32.Parse( xstring );
                                y = Int32.Parse( ystring );
                                m_X = x;
                                m_Y = y;
                            }
                            catch
                            {
                                from.SendMessage( "Please enter a X coordinate in the top box and a Y coordinate in the bottom box" );
                            }
                        }
                        if( m_X < 0 )
                        {
                            m_X = 0;
                            from.SendMessage( "You cannot go any farther left" );
                        }
                        if( m_Y < 0 )
                        {
                            m_Y = 0;
                            from.SendMessage( "You cannot go any farther up" );
                        }

                        from.CloseGump( typeof( IconPlacementGump ) );
                        from.SendGump( new IconPlacementGump( m_Book, from, m_X, m_Y, m_Delta, m_Icon, m_Spell, m_Background ) );
                        break;
                    }
                case (int)Buttons.Up:
                    {
                        ParseDelta( info );
                        from.CloseGump( typeof( IconPlacementGump ) );
                        if( ( m_Y - m_Delta ) < 0 )
                        {
                            m_Y = 0;
                            from.SendMessage( "You cannot go any farther up" );
                            from.SendGump( new IconPlacementGump( m_Book, from, m_X, m_Y, m_Delta, m_Icon, m_Spell, m_Background ) );
                        }
                        else
                        {
                            from.SendGump( new IconPlacementGump( m_Book, from, m_X, ( m_Y - m_Delta ), m_Delta, m_Icon, m_Spell, m_Background ) );
                        }
                        break;
                    }
                case (int)Buttons.Right:
                    {
                        ParseDelta( info );
                        from.CloseGump( typeof( IconPlacementGump ) );
                        from.SendGump( new IconPlacementGump( m_Book, from, ( m_X + m_Delta ), m_Y, m_Delta, m_Icon, m_Spell, m_Background ) );
                        break;
                    }
                case (int)Buttons.Down:
                    {
                        ParseDelta( info );
                        from.CloseGump( typeof( IconPlacementGump ) );
                        from.SendGump( new IconPlacementGump( m_Book, from, m_X, ( m_Y + m_Delta ), m_Delta, m_Icon, m_Spell, m_Background ) );
                        break;
                    }
                case (int)Buttons.Left:
                    {
                        ParseDelta( info );
                        from.CloseGump( typeof( IconPlacementGump ) );
                        if( ( m_X - m_Delta ) < 0 )
                        {
                            m_X = 0;
                            from.SendMessage( "You cannot go any farther left" );
                            from.SendGump( new IconPlacementGump( m_Book, from, m_X, m_Y, m_Delta, m_Icon, m_Spell, m_Background ) );
                        }
                        else
                        {
                            from.SendGump( new IconPlacementGump( m_Book, from, ( m_X - m_Delta ), m_Y, m_Delta, m_Icon, m_Spell, m_Background ) );
                        }
                        break;
                    }
                default:
                    return;
            }
        }

        private void ParseDelta( RelayInfo info )
        {
            TextRelay irelay = info.GetTextEntry( 9 );

            string istring = ( irelay == null ? null : irelay.Text.Trim() );

            if( string.IsNullOrEmpty( istring ) )
            {
                m_From.SendMessage( "Please enter an integer in the middle text field." );
            }
            else
            {
                int i = m_Delta;

                try
                {
                    i = Int32.Parse( istring );
                    m_Delta = i;
                }
                catch
                {
                    m_From.SendMessage( "Please enter an integer in the middle text field." );
                }
            }
        }
    }
}