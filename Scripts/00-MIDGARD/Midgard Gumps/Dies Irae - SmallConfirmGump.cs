/***************************************************************************
 *                               Dies Irae - SmallConfirmGump.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Gumps
{
    public delegate void ConfirmGumpCallback( Mobile from, bool okay, object state );

    public class SmallConfirmGump : Gump
    {
        private readonly ConfirmGumpCallback m_Callback;
        private readonly object m_State;

        public SmallConfirmGump( string text, ConfirmGumpCallback callback )
            : this( text, callback, null, true )
        {
        }

        public SmallConfirmGump( string text, ConfirmGumpCallback callback, bool cancellable )
            : this( text, callback, null, cancellable )
        {
        }

        public SmallConfirmGump( string text, ConfirmGumpCallback callback, object state )
            : this( text, callback, state, true )
        {
        }

        public SmallConfirmGump( string text, ConfirmGumpCallback callback, object state, bool cancellable )
            : base( 100, 100 )
        {
            m_Callback = callback;
            m_State = state;

            AddPage( 0 );
            AddImage( 0, 0, 2070 );

            AddHtml( 35, 25, 115, 40, String.Format( "<BIG>{0}", text ), false, false );

            if( cancellable )
                AddButton( 35, 75, 2071, 2072, 0, GumpButtonType.Reply, 0 );

            AddButton( 95, 75, 2074, 2075, 1, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == 1 && m_Callback != null )
                m_Callback( sender.Mobile, true, m_State );
            else if( m_Callback != null )
                m_Callback( sender.Mobile, false, m_State );
        }

        public override void OnServerClose( NetState owner )
        {
            if( m_Callback != null && owner != null && owner.Mobile != null )
                m_Callback( owner.Mobile, false, null );

            base.OnServerClose( owner );
        }
    }
}