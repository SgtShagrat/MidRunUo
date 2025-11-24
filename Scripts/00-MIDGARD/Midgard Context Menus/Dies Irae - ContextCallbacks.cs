/***************************************************************************
 *                               Dies Irae - ContextCallbacks.cs
 *
 *   begin                : 09 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.ContextMenus
{
    delegate void ContextPlayerCallback( Mobile player );

    class CallbackPlayerEntry : Server.ContextMenus.ContextMenuEntry
    {
        private readonly Mobile m_Player;
        private readonly ContextPlayerCallback m_Callback;

        public CallbackPlayerEntry( int number, ContextPlayerCallback callback, Mobile player )
            : this( number, -1, callback )
        {
            m_Player = player;
        }

        private CallbackPlayerEntry( int number, int range, ContextPlayerCallback callback )
            : base( number, range )
        {
            m_Callback = callback;
        }

        public override void OnClick()
        {
            if( m_Callback != null )
                m_Callback( m_Player );
        }
    }

    delegate void ContextCallback();

    class CallbackEntry : Server.ContextMenus.ContextMenuEntry
    {
        private readonly ContextCallback m_Callback;

        public CallbackEntry( int number, ContextCallback callback )
            : this( number, -1, callback )
        {
        }

        private CallbackEntry( int number, int range, ContextCallback callback )
            : base( number, range )
        {
            m_Callback = callback;
        }

        public override void OnClick()
        {
            if( m_Callback != null )
                m_Callback();
        }
    }
}