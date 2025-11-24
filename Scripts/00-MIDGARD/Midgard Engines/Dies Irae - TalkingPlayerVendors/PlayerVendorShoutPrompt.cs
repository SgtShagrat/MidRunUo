using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Prompts;

namespace Midgard.Engines.TalkingPlayerVendorSystem
{
    public class PlayerVendorShoutPrompt : Prompt
    {
        private Mobile m_From;
        private PlayerVendor m_Vendor;

        public PlayerVendorShoutPrompt( Mobile from, PlayerVendor vendor )
        {
            m_From = from;
            m_Vendor = vendor;
        }

        public override void OnResponse( Mobile from, string text )
        {
            if( m_Vendor == null || m_Vendor.Deleted )
                return;

            if( m_Vendor.ShoutEntries == null )
                m_Vendor.ShoutEntries = new List<string>();

            if( !String.IsNullOrEmpty( text ) )
            {
                m_Vendor.ShoutEntries.Add( text );
                m_From.SendMessage( "Message has been added to vendor shouts list." );
                m_Vendor.Say( true, "Well done! I will shout for you that things..." );
            }
            else
                m_From.SendMessage( "That message is not valid" );

            m_From.SendGump( new PlayerVendorShoutGump( m_From, m_Vendor ) );
        }

        public override void OnCancel( Mobile from )
        {
            from.SendGump( new PlayerVendorShoutGump( from, m_Vendor ) );
        }
    }
}