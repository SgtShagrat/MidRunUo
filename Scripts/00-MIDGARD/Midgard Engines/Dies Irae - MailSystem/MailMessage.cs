/***************************************************************************
 *                               MailMessage.cs
 *
 *   begin                : 31 December, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Engines.MailSystem
{
    public class MailMessage
    {
        public MailMessage()
            : this( null, null, "", "" )
        {
        }

        public MailMessage( Mobile from, Mobile to, string title, string text )
        {
            From = from;
            To = to;
            Title = title;
            Text = text;
        }

        #region serialization
        public MailMessage( GenericReader reader )
        {
            int version = reader.ReadInt();

            From = reader.ReadMobile();
            To = reader.ReadMobile();
            Title = reader.ReadString();
            Text = reader.ReadString();
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            writer.Write( From );
            writer.Write( To );
            writer.Write( Title );
            writer.Write( Text );
        }
        #endregion

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile From { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile To { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Title { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Text { get; set; }

        public void Deliver( Mobile m, MidgardTowns town )
        {
            ReceivedMailScroll mailScroll = new ReceivedMailScroll( this );

            if( m.Backpack != null && m.AddToBackpack( mailScroll ) )
                m.SendMessage( "A mail from {0} has been placed in your pack.", From.Name );
            else
            {
                m.BankBox.DropItem( mailScroll );
                m.SendMessage( "As your backpack is full, a mail from {0} has been placed in your pack.", From.Name );
            }
        }
    }
}