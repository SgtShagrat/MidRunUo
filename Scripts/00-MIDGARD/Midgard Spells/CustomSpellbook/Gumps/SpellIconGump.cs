using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.SpellSystem
{
    public class SpellIconGump : Gump
    {
        public enum Buttons
        {
            Close,

            Cast,
            Dispose
        }

        int m_Icon;
        int m_SpellID;
        int m_Background;
        CustomSpellbook m_Book;

        public SpellIconGump( CustomSpellbook book, int x, int y, int icon, int spellID, int background )
            : base( x, y )
        {
            m_Book = book;
            m_Icon = icon;
            m_SpellID = spellID;
            m_Background = background;

            Closable = false;
            Disposable = false;
            Dragable = false;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 0, 0, 54, 54, background );
            AddButton( 45, 0, 9008, 9010, (int)Buttons.Dispose, GumpButtonType.Reply, 0 );
            AddButton( 5, 5, icon, icon, (int)Buttons.Cast, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( m_Book.Deleted )
                return;

            Mobile from = state.Mobile;

            switch( info.ButtonID )
            {
                case (int)Buttons.Dispose:
                    {
                        int remove = 0;
                        bool doRemove = false;

                        int count = m_Book.DragableIcons.Count;
                        for( int i = 0; i < count; i++ )
                        {
                            IconInfo ii = m_Book.DragableIcons[ i ];
                            if( ii.Icon == m_Icon )
                            {
                                doRemove = true;
                                remove = i;
                            }
                        }

                        if( doRemove )
                        {
                            m_Book.DragableIcons.RemoveAt( remove );
                            from.SendMessage( "That icon has been removed and will not open again." );
                        }
                        break;
                    }

                case (int)Buttons.Cast:
                    {
                        if( m_SpellID != 999 )
                        {
                            RPGSpellsSystem.CastSpellByID( m_SpellID, from, true, true );

                            from.SendGump( new SpellIconGump( m_Book, X, Y, m_Icon, m_SpellID, m_Background ) );
                        }
                        else
                        {
                            from.SendLocalizedMessage( 502345 ); // This spell has been temporarily disabled.
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
}