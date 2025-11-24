using Server;
using Server.ContextMenus;
using Server.Network;

namespace Midgard.Engines.JailSystem
{
    public class ReviewEntry : ContextMenuEntry
    {
        private readonly Mobile m_Gm;
        private readonly Mobile m_Player;

        public ReviewEntry( Mobile gm, Mobile player )
            : base( 10004, 200 )
        {
            m_Gm = gm;
            m_Player = player;
        }

        public override void OnClick()
        {
            m_Gm.SendGump( new JailReviewGump( m_Gm, m_Player, 0, null ) );
        }
    }

    public class JailEntry : ContextMenuEntry
    {
        private readonly Mobile m_Gm;
        private readonly Mobile m_Player;

        public JailEntry( Mobile gm, Mobile player )
            : base( 5008, 200 )
        {
            m_Gm = gm;
            m_Player = player;
        }

        public override void OnClick()
        {
            JailSystem.NewJailingFromGMandPlayer( m_Gm, m_Player );
            //this is where we jail them
        }
    }

    public class UnJailEntry : ContextMenuEntry
    {
        private readonly JailSystem m_Js;
        private readonly Mobile m_Gm;
        private readonly Mobile m_Player;

        public UnJailEntry( Mobile gm, Mobile player )
            : base( 5135, 200 )
        {
            m_Gm = gm;
            m_Player = player;
            m_Js = JailSystem.FromMobile( m_Player );
            if( m_Js == null )
                Flags |= CMEFlags.Disabled;
            else if( !m_Js.Jailed )
                Flags |= CMEFlags.Disabled;
        }

        public override void OnClick()
        {
            if( m_Js == null )
                m_Gm.SendMessage( "They are not jailed" );
            else if( m_Js.Jailed )
                m_Js.ForceRelease( m_Gm );
        }
    }

    public class MacroerEntry : ContextMenuEntry
    {
        private readonly JailSystem m_Js;
        private readonly Mobile m_Gm;
        private readonly Mobile m_Player;

        public MacroerEntry( Mobile gm, Mobile player )
            : base( 394, 200 )
        {
            m_Gm = gm;
            m_Player = player;
            m_Js = JailSystem.FromMobile( m_Player );
            if( m_Js == null )
            {
            }
            else if( m_Js.Jailed )
                Flags |= CMEFlags.Disabled;
        }

        public override void OnClick()
        {
            if( m_Js == null )
                JailSystem.MacroTest( m_Gm, m_Player );
            else if( !m_Js.Jailed )
                JailSystem.MacroTest( m_Gm, m_Player );
            else
                m_Gm.SendMessage( "They are already in jail." );
        }
    }
}