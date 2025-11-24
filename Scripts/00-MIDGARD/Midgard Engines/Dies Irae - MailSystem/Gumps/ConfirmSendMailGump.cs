/***************************************************************************
 *                               ConfirmSendMailGump.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Gumps;

using Server;

namespace Midgard.Engines.MailSystem
{
    public class ConfirmSendMailGump : SmallConfirmGump
    {
        public ConfirmSendMailGump( Mobile from, MailScroll scroll )
            : base( "Will thou send this mail?", ConfirmJoin_Callback, scroll )
        {
            from.CloseGump( typeof( ConfirmSendMailGump ) );
        }

        private static void ConfirmJoin_Callback( Mobile from, bool okay, object state )
        {
            MailScroll mailScroll = state as MailScroll;
            if( okay && mailScroll != null )
                mailScroll.Send( from );
        }
    }
}