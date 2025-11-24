/***************************************************************************
 *                               MailCompositionGump.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MailSystem
{
    public class MailCompositionGump : Gump
    {
        private enum Buttons
        {
            Close = 0,

            ChooseTown,
            ReceiptTextEntry,
            CheckReceipt,
            TitleTextEntry,
            MessageTextEntry,
            Send
        }

        private Mobile m_From;
        private MailScroll m_Scroll;
        private Mobile m_To;
        private MidgardTowns m_Town;

        public MailCompositionGump( Mobile from, MailScroll scroll )
            : base( 50, 50 )
        {
            m_Scroll = scroll;
            m_From = from;
            m_Town = m_Scroll.Town;
            m_To = m_Scroll.Message.To;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            m_From.CloseGump( typeof( MailCompositionGump ) );

            AddPage( 0 );

            AddImageTiled( 3, 5, 300, 37, 2080 ); // scroll top
            for( int i = 0; i < 4; i++ )
                AddImageTiled( 20, 41 + 70 * i, 263, 70, 2081 ); // scroll middle , lower portion
            AddImageTiled( 20, 391, 273, 34, 2083 ); // scroll bottom

            AddImageTiled( 20, 321, 263, 70, 2081 );
            AddImage( 40, 130, 2091 );
            AddImage( 100, 75, 2501 );

            AddLabel( 45, 45, 0, "Author:" );
            if( m_Scroll.Message.From != null )
                AddLabel( 105, 45, 0, m_Scroll.Message.From.Name );

            AddLabel( 45, 105, 0, "Town:" );
            if( m_Town != MidgardTowns.None )
                AddLabel( 105, 105, 0, TownHelper.FindTownName( m_Town ) );
            AddButton( 245, 110, 2103, 2104, (int)Buttons.ChooseTown, GumpButtonType.Reply, 0 );

            AddLabel( 45, 75, 0, "Receipt:" );
            if( m_Scroll.Message.To != null )
                AddTextEntry( 105, 75, 152, 20, 0, (int)Buttons.ReceiptTextEntry, m_Scroll.Message.To.Name );
            else
                AddTextEntry( 105, 75, 152, 20, 0, (int)Buttons.ReceiptTextEntry, "choose the receipt" );
            AddButton( 245, 80, 2103, 2104, (int)Buttons.CheckReceipt, GumpButtonType.Reply, 0 );

            AddLabel( 45, 145, 0, "Subject:" );
            if(!string.IsNullOrEmpty( m_Scroll.Message.Title  ) )
                AddTextEntry( 40, 165, 220, 20, 0, (int)Buttons.TitleTextEntry, m_Scroll.Message.Title );
            else
                AddTextEntry( 40, 165, 220, 20, 0, (int)Buttons.TitleTextEntry, "insert the title" );

            AddLabel( 45, 185, 0, "Message:" );
            if( !string.IsNullOrEmpty( m_Scroll.Message.Text ) )
                AddTextEntry( 40, 205, 220, 180, 0, (int)Buttons.MessageTextEntry, m_Scroll.Message.Text );
            else
                AddTextEntry( 40, 205, 220, 180, 0, (int)Buttons.MessageTextEntry, "compose the message" );

            AddButton( 65, 355, 52, 52, (int)Buttons.Send, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( info.ButtonID < 1 )
                return;

            TextRelay message = info.GetTextEntry( (int)Buttons.MessageTextEntry );
            TextRelay receipt = info.GetTextEntry( (int)Buttons.ReceiptTextEntry );
            TextRelay title = info.GetTextEntry( (int)Buttons.TitleTextEntry );

            bool isSendable = !String.IsNullOrEmpty( title.Text ) && !String.IsNullOrEmpty( message.Text ) && !String.IsNullOrEmpty( receipt.Text ) &&
                                m_Town != MidgardTowns.None && m_To != null;

            switch( info.ButtonID )
            {
                case (int)Buttons.CheckReceipt:
                    if( !m_Scroll.CanBeUsedBy( m_From, true ) )
                        return;

                    if( !String.IsNullOrEmpty( receipt.Text ) )
                    {
                        if( Core.Instance.IsValidRecipient( receipt.Text ) )
                        {
                            m_From.SendMessage( "You have chosen a valid mail receipt." );
                            m_Scroll.Message.To = Core.Instance.GetReceiptByName( receipt.Text );
                        }
                        else
                            m_From.SendMessage( "That is not a valid receipt." );

                        m_From.SendGump( new MailCompositionGump( m_From, m_Scroll ) );
                    }
                    break;
                case (int)Buttons.ChooseTown:
                    if( !m_Scroll.CanBeUsedBy( m_From, true ) )
                        return;

                    m_From.SendMessage( "Choose the town where the message will be delivered." );
                    m_From.SendGump( new MailTownSelectionGump( m_From, m_Scroll ) );
                    break;
                case (int)Buttons.Send:
                    if( !m_Scroll.CanBeUsedBy( m_From, true ) )
                        return;

                    m_Scroll.Message.Title = title.Text;
                    m_Scroll.Message.Text = message.Text;

                    if( !Core.Instance.IsMailBoxNear( m_From, true ) )
                        return;

                    if( isSendable )
                        m_From.SendGump( new ConfirmSendMailGump( m_From, m_Scroll ) );
                    else
                        m_From.SendMessage( "This mail cannot be sent until completed." );
                    break;
            }
        }
    }
}